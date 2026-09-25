using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using NoxusBoss.Content.Items;
using NoxusBoss.Content.Rarities;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using CalamityMod;
using System.IO;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using System.Linq;
using NoxusBoss.Content.Tiles.SolynCampsite;
using NoxusBoss.Core.Netcode.Packets;
using Terraria.DataStructures;
using NoxusBoss.Core.Graphics.UI.Books;
using NoxusBoss.Core.World.WorldSaving;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity;
using NoxusBoss.Core.Netcode;
using Terraria.ModLoader.IO;
using InfernalEclipseAPI.Content.Items.Materials;
using CalamityMod.Items.Placeables;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Content.Items.Weapons.BossRush.NovaBomb;
using InfernalEclipseAPI.Content.Items.Weapons.BossRush.Swordofthe14thGlitch;
using InfernalEclipseAPI.Content.Items.Weapons.Magic.ChaosBlaster;
using InfernalEclipseAPI.Content.Items.Weapons.Nameless.NebulaGigabeam;
using InfernalEclipseAPI.Content.Items.Weapons.Ranged.ExoDisintegrator;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using NoxusBoss.Content.Tiles;
using System.Reflection;
using NoxusBoss.Content.Items.Placeable;
using Mono.Cecil.Cil;
using InfernalEclipseAPI.Core.Configs;

namespace InfernalEclipseAPI.Content.Items.Other
{
    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class SolynsSpellbook : ModItem
    {
        private static readonly Asset<Texture2D> starFilled = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Rank_Light");

        private static readonly Asset<Texture2D> starBlank = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Rank_Dim");

        public Action<Item, int, int> PreDrawTooltipAction
        {
            get;
            set;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 1;

            bool dontSpawn = !SolynBooksSystem.BooksObtainable;
            if (dontSpawn && !Main.gameMenu)
                Item.stack = 0;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && (!InfernalRecipeUnlockHandler.HasUnlockedSolynBookRecipes || !InfernalRecipeUnlockHandler.HasFoundSolynSpellbook))
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player textPlayer = Main.player[i];

                    if (!textPlayer.active)
                        continue;

                    CombatText.NewText(textPlayer.Hitbox, Color.Magenta, CalamityUtils.GetTextValue("Misc.LearnedSchematic"), true);
                }

                InfernalRecipeUnlockHandler.HasUnlockedSolynBookRecipes = true;
                InfernalRecipeUnlockHandler.HasFoundSolynSpellbook = true;
                CalamityNetcode.SyncWorld();
            }
        }

        // Recipe exists for posierity.
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Book).
                AddIngredient(ItemID.PinkDye, 2).
                AddIngredient(ItemID.FallenStar, 5).
                AddCondition(SpellbookRecipe.ConstructRecipeCondition(out Func<bool> condition), condition).
                AddTile(TileID.Bookcases).
                Register();
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (InfernalRecipeUnlockHandler.HasUnlockedSolynBookRecipes)
            {
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip2" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    /*
                    insertIndex++;
                    int summonItem = ModContent.ItemType<AqueousHunterDrone>();
                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{summonItem}] {CalamityUtils.GetItemName(summonItem)}");
                    summonDisplay.OverrideColor = new Color(149, 243, 43); //schematic green
                    list.Insert(insertIndex, summonDisplay);
                    */

                    insertIndex++;
                    int rockItem = ModContent.ItemType<Rock>();
                    TooltipLine rockDisplay = new TooltipLine(Mod, "IEoR:RockDisplay", $"[i:{rockItem}] {CalamityUtils.GetItemName(rockItem)}");
                    rockDisplay.OverrideColor = Color.Gray;
                    list.Insert(insertIndex, rockDisplay);

                    insertIndex++;
                    int alloyItem = ModContent.ItemType<AlloyofEden>();
                    TooltipLine alloyDisplay = new TooltipLine(Mod, "IEoR:AlloyDisplay", $"[i:{alloyItem}] {CalamityUtils.GetItemName(alloyItem)}");
                    alloyDisplay.OverrideColor = Color.Lerp(Color.MediumOrchid, Color.Orange, (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5));
                    list.Insert(insertIndex, alloyDisplay);

                    insertIndex++;
                    int glitchItem = ModContent.ItemType<Swordofthe14thGlitch>();
                    TooltipLine glitchDisplay = new TooltipLine(Mod, "IEoR:GlitchDisplay", $"[i:{glitchItem}] {CalamityUtils.GetItemName(glitchItem)}");
                    glitchDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex, glitchDisplay);

                    insertIndex++;
                    int marsItem = ModContent.ItemType<ExoDisintegrator>();
                    TooltipLine marsDisplay = new TooltipLine(Mod, "IEoR:MarsDisplay", $"[i:{marsItem}] {CalamityUtils.GetItemName(marsItem)}");
                    marsDisplay.OverrideColor = Color.IndianRed;
                    list.Insert(insertIndex, marsDisplay);

                    insertIndex++;
                    int yobItem = ModContent.ItemType<NovaBomb>();
                    TooltipLine yobDisplay = new TooltipLine(Mod, "IEoR:YobDisplay", $"[i:{yobItem}] {CalamityUtils.GetItemName(yobItem)}");
                    yobDisplay.OverrideColor = Color.MediumPurple;
                    list.Insert(insertIndex, yobDisplay);

                    insertIndex++;
                    int solynItem = ModContent.ItemType<ChaosBlaster>();
                    TooltipLine solynDisplay = new TooltipLine(this.Mod, "IEoR:SolynDisplay", $"[i:{solynItem} ] {CalamityUtils.GetItemName(solynItem)}");
                    solynDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex, solynDisplay);

                    insertIndex++;
                    int namelessItem = ModContent.ItemType<NebulaGigabeam>();
                    TooltipLine namelessDisplay = new TooltipLine(this.Mod, "IEoR:NamelessDisplay", $"[i:{namelessItem} ] {CalamityUtils.GetItemName(namelessItem)}");
                    namelessDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex, namelessDisplay);

                    if (ModLoader.TryGetMod("CalamitySimpleWhipAddon", out Mod calSimpleWhips))
                    {
                        insertIndex++;
                        int aurelianItem = calSimpleWhips.Find<ModItem>("AurelianSanctum").Type;
                        TooltipLine aurelianDisplay = new TooltipLine(Mod, "IEoR:AurelianSanctumDisplay", $"[i:{aurelianItem}] {CalamityUtils.GetItemName(aurelianItem)}");
                        aurelianDisplay.OverrideColor = new Color(236, 255, 31); //yellow used be schematics
                        list.Insert(insertIndex, aurelianDisplay);
                    }

                    if (InfernalCrossmod.SOTS.Loaded)
                    {
                        insertIndex++;
                        int tesseractItem = InfernalCrossmod.SOTS.Mod.Find<ModItem>("Tesseract").Type;
                        TooltipLine tesseractDisplay = new TooltipLine(Mod, "IEoR:TesseractDisplay", $"[i:{tesseractItem}] {CalamityUtils.GetItemName(tesseractItem)}");
                        tesseractDisplay.OverrideColor = Color.Purple;
                        list.Insert(insertIndex, tesseractDisplay);
                    }

                    if (ModLoader.TryGetMod("ZenithToilet", out Mod toilet))
                    {
                        insertIndex++;
                        int toiletItem = toilet.Find<ModItem>("TrueZenithToilet").Type;
                        TooltipLine toiletDisplay = new TooltipLine(Mod, "IEoR:ToiletDisplay", $"[i:{toiletItem}] {CalamityUtils.GetItemName(toiletItem)}");
                        toiletDisplay.OverrideColor = new Color(201, 41, 255); //yellow used be schematics
                        list.Insert(insertIndex, toiletDisplay);
                    }
                }
            }

            //tooltips.RemoveAll(t => t.Name.Contains("Tooltip"));

            list.Add(new TooltipLine(Mod, "StarsLine", Language.GetTextValue("Mods.NoxusBoss.UI.SolynBookExchange.RarityText"))
            {
                OverrideColor = ModContent.GetInstance<SolynRewardRarity>().RarityColor
            });
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (string.IsNullOrWhiteSpace(line.Text))
                yOffset += 1;

            if (line.Name == "StarsLine")
            {
                // Render the sets of stars next to the 'Rarity: ' line in accordance with how rare the books is.
                // By default, only three stars are rendered, but if a book has a rarity exceeding three, more are added.
                float starScale = 1.25f;
                float textWidth = line.Font.MeasureString(line.Text).X;
                int starCount = 5;
                for (int i = 0; i < starCount; i++)
                {
                    Texture2D starTexture = (i >= 5 ? starBlank : starFilled).Value;
                    Vector2 starDrawPosition = new Vector2(line.X + i * starScale * 15f + textWidth, line.Y + 2f);
                    Main.spriteBatch.Draw(starTexture, starDrawPosition, null, Color.White, 0f, Vector2.Zero, starScale, 0, 0f);
                }
            }
            return true;
        }

        public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            PreDrawTooltipAction?.Invoke(Item, x, y);
            return true;
        }

        public static void DrawWithOutline(Texture2D texture, Vector2 drawPosition, float opacity, float rotation)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 drawOffset = (TwoPi * i / 32f).ToRotationVector2() * 2f;
                Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, new Color(255, 255, 120, 0) * opacity, rotation, Vector2.Zero, 1f, 0, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White * opacity, rotation, Vector2.Zero, 1f, 0, 0f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawWithOutline(TextureAssets.Item[Type].Value, Item.position - Main.screenPosition - Vector2.UnitY * 5f, 1f, rotation);
            return false;
        }
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class SolynTentStuff : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (type != ModContent.TileType<SolynTent>()) return;

            Tile t = Framing.GetTileSafely(i, j);

            bool isCenterTile = t.TileFrameX == 126 && t.TileFrameY == 90;
            bool playerCloseToSelf = Main.LocalPlayer.WithinRange(new Vector2(i, j).ToWorldCoordinates(8f, 0f), 20f);
            if (isCenterTile && playerCloseToSelf && SpellbookManager.SpellbookIsInTent)
            {
                Item.NewItem(new EntitySource_WorldEvent(), Main.LocalPlayer.Center, ModContent.ItemType<SolynsSpellbook>());
                SpellbookManager.HasReceivedSpellbook = true;

                PacketManager.SendPacket<SpellbookPacket>();
            }
        }
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class StarlitForgeMapNameEdit : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.DeveloperMode;
        }

        private ILHook starlitForgeSetStaticDefaultsHook;

        public override void Load()
        {
            MethodInfo setStaticDefaultsMethod = typeof(StarlitForgeTile).GetMethod(nameof(StarlitForgeTile.SetStaticDefaults), LumUtils.UniversalBindingFlags);

            if (setStaticDefaultsMethod is null)
            {
                Mod.Logger.Error("Could not find StarlitForgeTile.SetStaticDefaults for the map-name IL edit.");

                return;
            }

            starlitForgeSetStaticDefaultsHook = new ILHook(setStaticDefaultsMethod, EditStarlitForgeMapEntry);
        }

        private void EditStarlitForgeMapEntry(ILContext context)
        {
            ILCursor cursor = new(context);

            MethodInfo addMapEntryWithoutName = typeof(ModTile).GetMethod(
                nameof(ModTile.AddMapEntry),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: [typeof(Color)],
                modifiers: null);

            MethodInfo addMapEntryWithName = typeof(ModTile).GetMethod(
                nameof(ModTile.AddMapEntry),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types:
                [
                    typeof(Color),
                    typeof(LocalizedText)
                ],
                modifiers: null);

            if (addMapEntryWithoutName is null || addMapEntryWithName is null)
            {
                Mod.Logger.Error("Could not locate the required ModTile.AddMapEntry overloads.");

                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before, instruction => instruction.MatchCallOrCallvirt(addMapEntryWithoutName)))
            {
                Mod.Logger.Error("Could not locate StarlitForgeTile's AddMapEntry(Color) call.");

                return;
            }

            cursor.EmitDelegate<Func<LocalizedText>>(static () => CalamityUtils.GetItemName<StarlitForge>());

            // Replace AddMapEntry(Color) with AddMapEntry(Color, LocalizedText).
            Instruction originalCall = cursor.Next;
            originalCall.OpCode = OpCodes.Call;
            originalCall.Operand = context.Import(addMapEntryWithName);

            Mod.Logger.Info("[IEoR]: Successfully added the Starlit Forge map entry name.");
        }

        public override void Unload()
        {
            starlitForgeSetStaticDefaultsHook?.Dispose();
            starlitForgeSetStaticDefaultsHook = null;
        }
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class SpellbookManager : ModSystem
    {
        public static bool HasReceivedSpellbook
        {
            get;
            set;
        }

        public static bool SpellbookIsInTent => !HasReceivedSpellbook && BossDownedSaveSystem.HasDefeated<NamelessDeityBoss>() && SolynBookExchangeRegistry.RedeemedAllBooks;

        public override void SaveWorldData(TagCompound tag) => tag[nameof(HasReceivedSpellbook)] = HasReceivedSpellbook;

        public override void LoadWorldData(TagCompound tag) => HasReceivedSpellbook = tag.GetBool(nameof(HasReceivedSpellbook));

        public override void NetSend(BinaryWriter writer) => writer.Write(HasReceivedSpellbook);

        public override void NetReceive(BinaryReader reader) => HasReceivedSpellbook = reader.ReadBoolean();
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class SpellbookPacket : Packet
    {
        public override void Write(ModPacket packet, params object[] context) => packet.Write(SpellbookManager.HasReceivedSpellbook);

        public override void Read(BinaryReader reader) => SpellbookManager.HasReceivedSpellbook = reader.ReadBoolean();
    }

    public static class InfernalRecipeUnlockHandler
    {
        public static bool HasUnlockedSolynBookRecipes = false;

        public static bool HasFoundSolynSpellbook = false;

        public static void Save(List<string> boolTagContainer)
        {
            boolTagContainer.AddWithCondition("HasUnlockedSolynBookRecipes", HasUnlockedSolynBookRecipes);

            boolTagContainer.AddWithCondition("HasFoundSolynSpellbook", HasFoundSolynSpellbook);
        }

        public static void Load(IList<string> boolTagContainer)
        {
            HasUnlockedSolynBookRecipes = boolTagContainer.Contains("HasUnlockedSolynBookRecipes");
            HasFoundSolynSpellbook = boolTagContainer.Contains("HasFoundSolynSpellbook");
        }

        public static void SendData(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = HasUnlockedSolynBookRecipes;
            flags[1] = HasFoundSolynSpellbook;

            writer.Write(flags);
        }

        public static void ReceiveData(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            HasUnlockedSolynBookRecipes = flags[0];
            HasFoundSolynSpellbook = flags[1];
        }
    }

    public static class SpellbookRecipe
    {
        public static LocalizedText ConstructRecipeCondition(out Func<bool> condition)
        {
            condition = new Func<bool>(() => InfernalRecipeUnlockHandler.HasFoundSolynSpellbook);
            return Language.GetOrRegister($"Mods.InfernalEclipseAPI.Misc.SolynSpellbookRecipeCondition");
        }
    }

    public static class SpellbookGatedRecipe
    {
        public static LocalizedText ConstructRecipeCondition(out Func<bool> condition)
        {
            condition = new Func<bool>(() => HasTierBeenLearned());
            return Language.GetOrRegister($"Mods.InfernalEclipseAPI.Misc.SpellbookRecipeCondition");
        }

        public static bool HasTierBeenLearned() => InfernalRecipeUnlockHandler.HasUnlockedSolynBookRecipes;
    }
}
