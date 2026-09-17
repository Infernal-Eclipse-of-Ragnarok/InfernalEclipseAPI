global using Terraria;
global using Terraria.ModLoader;
global using Terraria.ID;
global using System;
global using static System.MathF;
global using static Microsoft.Xna.Framework.MathHelper;
global using LumUtils = Luminance.Common.Utilities.Utilities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using InfernumMode.Core.GlobalInstances.Systems;
using CalamityMod.Systems;
using System.IO;
using InfernalEclipseAPI.Core.Players;
using System.Reflection;
using InfernalEclipseAPI.Core.World;
using InfernalEclipseAPI.Core;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.Utils.ConfigSetup;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using InfernalEclipseAPI.Content.UI;
using InfernalEclipseAPI.Core.Players.ThoriumPlayerOverrides.ThoriumMulticlassNerf;
using Terraria.Chat;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Utils;
using CalamityMod.World;
using Terraria.GameContent.Creative;
using static Terraria.GameContent.Creative.CreativePowers;
using System.Runtime.CompilerServices;
using NoxusBoss.Core.SolynEvents;
using InfernumMode.Core.TrackedMusic;
using InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.ProvidenceOverrides;
using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using InfernalEclipseAPI.Core.Configs;
using Terraria.UI.Chat;
using InfernalEclipseAPI.Core.ChatTags;

[assembly: InternalsVisibleTo("HomewardRagnarok")]
namespace InfernalEclipseAPI
{
    public enum InfernalEclipseMessageType : byte
    {
        SyncDownedBosses = 1,
        TriggerScytheCharge = 2,
        ThoriumEmpowerment = 3,
        ToggleRagnarok = 4,
        SyncRagnarokState = 5,
        MarsMultiplayerSync = 6
    }

    public partial class InfernalEclipseAPI : Mod
	{
        internal static SymbolTracker symbolTracker;

        public static ModKeybind SubpaceBoostHotkey;
        public static ModKeybind ItemAbility;

        public static InfernalEclipseAPI Instance;
        public InfernalEclipseAPI() => Instance = this;

        public const string ProvidenceNightPath = "InfernalEclipseAPI/Assets/Music/ProvidenceNight";

        public static int WhiteFlareType = 0;
        private bool _hijackInteraction;

        public override void Load()
        {
            DifficultyManagementSystem.DisableDifficultyModes = false;

            symbolTracker = new SymbolTracker();

            TrackedMusicLoader();

            if (InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    ThoriumHelheimConfigSetup.DisableItemReworks();
                }

                if (InfernalCrossmod.CalBardHealer.Loaded)
                {
                    CalBardHealerConfigSetup.DisableGenItemBalance();
                }
            }
            
            if (InfernalConfig.Instance.AutomatedConfigSetup)
            {
                string cfgDir = Path.Combine(Main.SavePath, "ModConfigs");
                Directory.CreateDirectory(cfgDir);

                if (InfernalCrossmod.RagnarokMod.Loaded)
                {
                    RagnarokModConfigSetup.SetupConfigs(cfgDir);
                }
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    ThoriumHelheimConfigSetup.SetupConfigs(cfgDir);
                }
            }

            // Cache the WhiteFlare projectile type from Thorium
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                if (thorium.TryFind<ModProjectile>("WhiteFlare", out var whiteFlare))
                    WhiteFlareType = whiteFlare.Type;

                if (ModLoader.TryGetMod("CellphonePylon", out _))
                {
                    On_Player.IsTileTypeInInteractionRange += On_Player_IsTileTypeInInteractionRange;
                    On_Player.InInteractionRange += On_Player_InInteractionRange;
                }
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                SOTSItemUtils.InitializeFakePlayerBlacklist();
            }

            RagnarokDifficulty difficulty = new();
            DifficultyModeSystem.Difficulties.Add(difficulty);
            DifficultyModeSystem.CalculateDifficultyData();

            SubpaceBoostHotkey = KeybindLoader.RegisterKeybind(this, Language.GetOrRegister("Mods.InfernalEclipseAPI.KeyBindName.SubspaceBoostHotkey").ToString(), "G");
            ItemAbility = KeybindLoader.RegisterKeybind(this, Language.GetOrRegister("Mods.InfernalEclipseAPI.KeyBindName.ItemAbility").ToString(), "C");

            ChatManager.Register<ChaosConstructChatTag>("IELuxTag");

            AchievementUpdateHandler = typeof(InfernumMode.Core.GlobalInstances.Players.AchievementPlayer).GetMethod("ExtraUpdateHandler", BindingFlags.Static | BindingFlags.NonPublic);

            LoadDetours();
        }

        public override void Unload()
        {
            DifficultyManagementSystem.DisableDifficultyModes = true;
            WhiteFlareType = 0; // Clean up on unload
            On_Player.IsTileTypeInInteractionRange -= On_Player_IsTileTypeInInteractionRange;
            On_Player.InInteractionRange -= On_Player_InInteractionRange;

            UnloadDetours();
        }

        private bool On_Player_IsTileTypeInInteractionRange(
            On_Player.orig_IsTileTypeInInteractionRange orig,
            Player self,
            int targetTileType,
            TileReachCheckSettings settings)
        {
            ModLoader.TryGetMod("ThoriumMod", out Mod thorium);
            // Only hijack for tile type 597
            if (targetTileType != 597)
                return orig(self, targetTileType, settings);

            _hijackInteraction = true;
            return orig(self, targetTileType, settings)
                || self.HasItemInInventoryOrOpenVoidBag(thorium.Find<ModItem>("WishingGlass").Type);
        }

        private bool On_Player_InInteractionRange(
            On_Player.orig_InInteractionRange orig,
            Player self,
            int interactX,
            int interactY,
            TileReachCheckSettings settings)
        {
            // If not hijacking, proceed as normal
            if (!_hijackInteraction)
                return orig(self, interactX, interactY, settings);

            // Reset hijack flag and allow interaction
            _hijackInteraction = false;
            return true;
        }

        public override void PostSetupContent()
        {
            RegisterProvidenceNightTrackedSong();

            TrackedMusicManager.TrackInformation[ProvidenceNightPath] = new ProvidenceNightTrackedMusic();

            #region Forced Menu Theme
            if (InfernalConfig.Instance.ForceMenu)
            {
                try
                {
                    if (typeof(MenuLoader).GetField("LastSelectedModMenu", BindingFlags.Static | BindingFlags.NonPublic) is null)
                        return;
                    ModMenu menu = ModContent.GetInstance<InfernalEclipseSkyMenu>();
                    if (menu is null)
                        return;
                    MenuLoader(menu);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");
                    Console.WriteLine("IEoRMenu");
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");
                }
            }
            #endregion

            #region Deerclops Boss Checklist repositioning
            //THANK GOD for habble on the Fargo Team for coding this
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                if (InfernalConfig.Instance.MoveDeerclopsChecklistEntry)
                {
                    #region Get Types
#nullable enable
                    Type? BossChecklist = bossChecklist.GetType(); // BossChecklist Type can be obtained via simply Mod.GetType()
                    // As Mod.Code.GetType(string name) is not implemented however, we use Mod.Code.GetTypes() and find the other ones we need
                    Type[]? TypeList = bossChecklist.Code.GetTypes();
                    Type? BossTracker = TypeList.Where<Type?>(type => type?.Name == "BossTracker")?.First();
                    Type? EntryInfo = TypeList.Where<Type?>(type => type?.Name == "EntryInfo")?.First();
#nullable disable
                    #endregion

                    #region Get Fields
                    // Get static instance field objects to utilize as initial object references
                    var BCInstance = BossChecklist?.GetField("instance", LumUtils.UniversalBindingFlags)?.GetValue(null);
                    var trackerInstance = BossChecklist?.GetField("bossTracker", LumUtils.UniversalBindingFlags)?.GetValue(null);
                    // Get the EntryInfo List<> field and object by using the Boss Tracker instance
#nullable enable
                    FieldInfo? SortedEntries_Field = BossTracker?.GetField("SortedEntries", LumUtils.UniversalBindingFlags);
#nullable disable
                    var SortedEntries = SortedEntries_Field?.GetValue(trackerInstance);
                    // Get the field needed to readd the portrait texture after we replace the EntryInfo that contained it
#nullable enable
                    FieldInfo? PortraitTexture_Field = EntryInfo?.GetField("portraitTexture", LumUtils.UniversalBindingFlags);
#nullable disable

                    #endregion

                    #region Get Methods
                    // As there's no way to normally use a List<> of a non-public type, hack into its List<T> and just get the methods that handle indexing
#nullable enable
                    PropertyInfo? List_EntryInfo_Property = SortedEntries?.GetType().GetProperty("Item", LumUtils.UniversalBindingFlags);
                    MethodInfo? List_EntryInfo_GetMethod = List_EntryInfo_Property?.GetGetMethod();
                    MethodInfo? List_EntryInfo_SetMethod = List_EntryInfo_Property?.GetSetMethod();

                    // This internal BossChecklist method returns the EntryInfo we need
                    MethodInfo FindEntryFromKey_Method = BossTracker?.GetMethod("FindEntryFromKey", LumUtils.UniversalBindingFlags);

                    // Very hackily resolve GetMethod ambiguity and obtain the method we require to make a replacement for Deerclops' EntryInfo
                    MethodInfo[]? MakeVanillaBoss_MethodList = EntryInfo?.GetMethods(LumUtils.UniversalBindingFlags);
                    MethodInfo? MakeVanillaBoss_Method = MakeVanillaBoss_MethodList?.Where(m => m.Name == "MakeVanillaBoss" && m.GetParameters().Any(p => p.Name == "npcID"))?.First();

                    void MakeVanillaBoss(ref object? info, string texturePath)
                    {
                        var obj = MakeVanillaBoss_Method?.Invoke(null, [0, 4.5f, "NPCName.Deerclops", Terraria.ID.NPCID.Deerclops, () => NPC.downedDeerclops]); // Make a replacement EntryInfo
                        if (ModContent.HasAsset(texturePath))
                        {
                            PortraitTexture_Field?.SetValue(obj, ModContent.Request<Texture2D>(texturePath)); // Readd the entry's portrait texture
                        }
                        info = obj;
                    }
#nullable disable
                    #endregion
                    // Finalize after getting everything necessary to replace Deerclops' entry
                    var DeerclopsEntry = FindEntryFromKey_Method?.Invoke(trackerInstance, ["Terraria Deerclops"]); // Get EntryInfo via FindEntryFromKey, where the key is "<ModSource> <NPCName>"
                    if (DeerclopsEntry == List_EntryInfo_GetMethod?.Invoke(SortedEntries, [6])) // Check whether the FindEntryFromKey retval matches List[] getval for the 7th entry (array 6) which contains the original Deerclops entry
                    {
                        MakeVanillaBoss(ref DeerclopsEntry, $"{bossChecklist.Name}/Resources/BossTextures/Boss{Terraria.ID.NPCID.Deerclops}"); // Tweak the matching entry's progression value
                        List_EntryInfo_SetMethod?.Invoke(SortedEntries, [6, DeerclopsEntry]); // Set the matching entry to the original List<>
                    }
                }
            }
            #endregion
        }

        public static void MenuLoader(ModMenu menu)
        {
            if (menu.FullName is null)
                return;
            typeof(MenuLoader).GetField("LastSelectedModMenu", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, menu.FullName);

            if ((ModMenu)typeof(MenuLoader).GetField("switchToMenu", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) is null || menu is null)
                return;
            if (((ModMenu)typeof(MenuLoader).GetField("switchToMenu", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).FullName is null || menu.FullName is null)
                return;
        }

        public static void TrackedMusicLoader()
        {
            if (!TrackedMusicManager.CustomTrackPaths.Contains(ProvidenceNightPath))
                TrackedMusicManager.CustomTrackPaths.Add(ProvidenceNightPath);
        }

        private static void RegisterProvidenceNightTrackedSong()
        {
            int musicSlot = MusicLoader.GetMusicSlot(ProvidenceNightPath);

            if (TrackedMusicManager.CustomTracks.ContainsKey(musicSlot))
                return;

            string fileName = "Assets/Music/ProvidenceNight.ogg";
            string diskPath = AssetPathHelper.CleanPath($"{TrackedMusicManager.MusicDirectory}/{ProvidenceNightPath}.ogg");

            Directory.CreateDirectory(Path.GetDirectoryName(diskPath));

            if (!File.Exists(diskPath))
            {
                using Stream input = Instance.GetFileStream(fileName, true);
                using FileStream output = File.Create(diskPath);
                input.CopyTo(output);
            }

            if (TrackedMusicManager.SongConstructor is null)
            {
                TrackedMusicManager.SongConstructor = typeof(Song).GetConstructor(
                    LumUtils.UniversalBindingFlags,
                    [typeof(string), typeof(string)]
                );
            }

            TrackedMusicManager.CustomTrackDiskPositions[musicSlot] = diskPath;
            TrackedMusicManager.TracksThatDontUseTerrariasSystem.Add(musicSlot);

            TrackedMusicManager.CustomTracks[musicSlot] =
                (Song)TrackedMusicManager.SongConstructor.Invoke([diskPath, ProvidenceNightPath]);
        }

        MethodInfo AchievementUpdateHandler;

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            InfernalEclipseMessageType msgType = (InfernalEclipseMessageType)reader.ReadByte();

            switch (msgType)
            {
                case InfernalEclipseMessageType.TriggerScytheCharge:
                    byte index = reader.ReadByte();
                    if (index < byte.MaxValue)
                    {
                        Main.player[index].GetModPlayer<HealerPlayer>().TriggerScytheCharge(true);
                    }
                    break;

                case InfernalEclipseMessageType.ThoriumEmpowerment:
                    {
                        if (!ModLoader.TryGetMod("ThoriumMod", out _))
                            break;

                        ThoriumEmpowermentMsg sub = (ThoriumEmpowermentMsg)reader.ReadByte();
                        switch (sub)
                        {
                            case ThoriumEmpowermentMsg.ClearEmpowerments:
                                {
                                    int plr = reader.ReadByte();
                                    if (plr >= 0 && plr < Main.maxPlayers)
                                    {
                                        Player target = Main.player[plr];
                                        ThoriumHelpers.ClearAllEmpowerments(target);

                                        // relay to others if server
                                        if (Main.netMode == NetmodeID.Server)
                                        {
                                            ModPacket p = GetPacket();
                                            p.Write((byte)InfernalEclipseMessageType.ThoriumEmpowerment);
                                            p.Write((byte)ThoriumEmpowermentMsg.ClearEmpowerments);
                                            p.Write((byte)plr);
                                            p.Send(-1, whoAmI);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }

                case InfernalEclipseMessageType.ToggleRagnarok:
                    {
                        Player player = reader.ReadByte() < Main.maxPlayers && Main.player[reader.ReadByte()].active && !Main.player[reader.ReadByte()].dead && !Main.player[reader.ReadByte()].ghost ? Main.player[reader.ReadByte()] : null;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            bool changed = false;
                            if (Main.GameModeInfo.IsJourneyMode)
                            {
                                float value = 1f;
                                var slider = CreativePowerManager.Instance.GetPower<DifficultySliderPower>();
                                typeof(DifficultySliderPower).GetMethod("SetValueKeyboardForced", LumUtils.UniversalBindingFlags).Invoke(slider, [value]);
                            }
                            else
                            {
                                if (Main.GameMode != GameModeID.Master)
                                    changed = true;
                                Main.GameMode = GameModeID.Master;
                            }
                            if (changed)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(Language.GetTextValue("Mods.InfernalEclipseAPI.DifficultyUI.MasterToggle")), new Color(175, 75, 255));

                            NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    }

                case InfernalEclipseMessageType.SyncRagnarokState:
                    {
                        bool enabled = reader.ReadBoolean();

                        if (Main.netMode == NetmodeID.Server)
                        {
                            InfernalWorld.RagnarokModeEnabled = enabled;

                            if (enabled)
                            {
                                CalamityWorld.revenge = true;
                                CalamityWorld.death = true;
                                WorldSaveSystem.InfernumModeEnabled = true;
                                if (!Main.GameModeInfo.IsJourneyMode)
                                    Main.GameMode = GameModeID.Master;
                            }

                            // rebroadcast to all clients (except sender)
                            ModPacket p = GetPacket();
                            p.Write((byte)InfernalEclipseMessageType.SyncRagnarokState);
                            p.Write(enabled);
                            p.Send(-1, whoAmI);

                            NetMessage.SendData(MessageID.WorldData);
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            InfernalWorld.RagnarokModeEnabled = enabled;
                            if (enabled)
                            {
                                CalamityWorld.revenge = true;
                                CalamityWorld.death = true;
                                WorldSaveSystem.InfernumModeEnabled = true;
                                if (!Main.GameModeInfo.IsJourneyMode)
                                    Main.GameMode = GameModeID.Master;
                            }
                        }

                        break;
                    }

                case InfernalEclipseMessageType.MarsMultiplayerSync:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        WrathPacketHandler.SetMarsCombatEvent();
                    }
                    break;
            }
        }
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public static class WrathPacketHandler
    {
        public static void SetMarsCombatEvent()
        {
            MarsCombatEvent.MarsBeingSummoned = true;
        }
    }
}
