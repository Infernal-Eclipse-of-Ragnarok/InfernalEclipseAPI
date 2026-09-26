using CalamityMod.Systems;
using InfernalEclipseAPI.Core.Configs;
using InfernumMode.Content.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static InfernalEclipseAPI.Core.Systems.BossChecklistEntryEditor;

namespace InfernalEclipseAPI.Core.Systems
{
    public static class BossChecklistEntryEditor
    {
        readonly static Mod bossChecklist = ModLoader.GetMod("BossChecklist");
        readonly static Type BossChecklist = bossChecklist.GetType();
        readonly static Type[] TypeList = bossChecklist.Code.GetTypes();
        readonly static object trackerInstance = BossChecklist?.GetField("bossTracker", LumUtils.UniversalBindingFlags)?.GetValue(null);
        readonly static Type BossTracker = TypeList.Where(type => type?.Name == "BossTracker")?.First();
        internal readonly static Type EntryInfo = TypeList.Where(type => type?.Name == "EntryInfo")?.First();
        // Code above by Habble
        // Code below by Ropro
        public static IList SortedEntriesDupe() => BossTracker?.GetField("SortedEntries", LumUtils.UniversalBindingFlags)?.GetValue(trackerInstance) as IList;
        public static object BossEntry(string Key) => BossTracker?.GetMethod("FindEntryFromKey", LumUtils.UniversalBindingFlags)?.Invoke(trackerInstance, [Key]);
        public static void ModifyBossImage(this object bossEntry, string assetPath) => EntryInfo?.GetField("portraitTexture", LumUtils.UniversalBindingFlags)?.SetValue(bossEntry, ModContent.Request<Texture2D>(assetPath));
        public static void ModifyBossHead(this object bossEntry, string headPath) => EntryInfo?.GetField("headIconTextures", LumUtils.UniversalBindingFlags)?.SetValue(bossEntry, (Func<List<Asset<Texture2D>>>)(() => [ModContent.Request<Texture2D>(headPath)]));
        public static void ModifyBossHead(this object bossEntry, Func<List<Asset<Texture2D>>> headInfo) => EntryInfo?.GetField("headIconTextures", LumUtils.UniversalBindingFlags)?.SetValue(bossEntry, headInfo);
        public static List<int> BossSpawnList(this object bossEntry) => EntryInfo?.GetField("spawnItem", LumUtils.UniversalBindingFlags)?.GetValue(bossEntry) as List<int>;
        public static void ModifyBossProgression(this object bossEntry, float progression) => EntryInfo?.GetField("progression", LumUtils.UniversalBindingFlags)?.SetValue(bossEntry, progression);
        public static float GetProgression(this object bossEntry) => (float)EntryInfo?.GetField("progression", LumUtils.UniversalBindingFlags)?.GetValue(bossEntry);
        //public static void ModifyBossName(this object bossEntry, string displayName) => EntryInfo?.GetProperty("DisplayName", LumUtils.UniversalBindingFlags)?.SetValue(bossEntry, displayName);
    }

    public class BossChecklistEntryEdits : ModSystem
    {
        static readonly Func<bool> InfernumOrMore = () => DifficultyModeSystem.GetCurrentDifficulty.CountAs<InfernumDifficulty>() || DifficultyModeSystem.GetCurrentDifficulty is InfernumDifficulty;
        const string path = "InfernalEclipseAPI/Assets/Images/UI/BossChecklist";

        public override void PostSetupContent()
        {
            if (!InfernalCrossmod.BossChecklist.Loaded)
                return;

            BossEntry("CalamityMod BossRush").ModifyBossProgression(28.1f);

            BossEntry("CalamityMod HiveMind").ModifyBossImage($"{path}/HiveMind");
            BossEntry("CalamityMod Calamitas").ModifyBossImage($"{path}/SupremeCalamitasHooded");

            object cc = BossEntry("CalamityMod CalamitasClone");
            cc.ModifyBossHead(() => InfernumOrMore() ? [ModContent.Request<Texture2D>("InfernumMode/Content/BehaviorOverrides/BossAIs/CalamitasShadow/CalShadowMapIcon")] : [ModContent.Request<Texture2D>("CalamityMod/NPCs/CalClone/CalamitasClone_Head_Boss")]);

            static Texture2D texture() => DifficultyModeSystem.GetCurrentDifficulty.CountAs<InfernumDifficulty>() || DifficultyModeSystem.GetCurrentDifficulty is InfernumDifficulty ? ModContent.Request<Texture2D>("InfernalEclipseAPI/Assets/Images/UI/BossChecklist/CalCloneShadow").Value : ModContent.Request<Texture2D>("InfernalEclipseAPI/Assets/Images/UI/BossChecklist/CalClone").Value;
            Action<SpriteBatch, Rectangle, Color> drawingCode = (spritebatch, rect, color) =>
            {
                Vector2 centered = new(rect.X + (rect.Width / 2) - (texture().Width / 2), rect.Y + (rect.Height / 2) - (texture().Height / 2));
                spritebatch.Draw(texture(), centered, color);
            };
            EntryInfo?.GetField("customDrawing", LumUtils.UniversalBindingFlags)?.SetValue(cc, drawingCode);

            if (InfernalConfig.Instance.MoveDeerclopsChecklistEntry)
                BossEntry("Terraria Deerclops").ModifyBossProgression(5.9f);

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                BossEntry("Consolaria Turkor").ModifyBossProgression(6.5f + 0.1f);
                BossEntry("Consolaria Ocram").ModifyBossProgression(12.99f);
            }

            if (InfernalCrossmod.Clamity.Loaded)
                BossEntry("Clamity Pyrogen").ModifyBossProgression(8.51f);

            if (ModLoader.HasMod("HypnosMod"))
                BossEntry("HypnosMod Hypnos").ModifyBossProgression(22.991f);

            if (InfernalCrossmod.Thorium.Loaded)
            {
                /*    
                    BossEntry("ThoriumMod GraniteEnergyStorm").ModifyBossImage($"{path}/GES");
                    BossEntry("ThoriumMod GraniteEnergyStorm").ModifyBossHead($"{path}/GES_Head");

                    BossEntry("ThoriumMod QueenJellyfish").ModifyBossImage($"{path}/QueenJelly");
                    BossEntry("ThoriumMod QueenJellyfish").ModifyBossHead($"{path}/QueenJelly_Head");

                    BossEntry("ThoriumMod Viscount").ModifyBossImage($"{path}/Viscount");
                    BossEntry("ThoriumMod Viscount").ModifyBossHead($"{path}/QueenJelly_Head");
                */

                BossEntry("ThoriumMod StarScouter").ModifyBossProgression(6.91f);

                if (InfernalCrossmod.ThoriumRework.Loaded)
                    BossEntry("ThoriumMod ThePrimordials").ModifyBossProgression(21.5f);
            }
        }
    }
}

// Old code shoved here so it doesn't fill up main file

// THANK GOD for Habble on the Fargo team for coding this
/*
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
*/