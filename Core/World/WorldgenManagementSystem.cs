using System.Collections.Generic;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Fragments;
using SOTS.Items.Invidia;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.Planetarium.Furniture;
using SOTS.WorldgenHelpers;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using ThoriumMod.NPCs;

namespace InfernalEclipseAPI.Core.World
{
    public class WorldgenManagementSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            base.ModifyWorldGenTasks(tasks, ref totalWeight);

            if (InfernalCrossmod.SOTS.Loaded)
                SOTSWorldGenModifications.RunSOTSWorldGenMods(tasks);
        }

        public override void PostWorldGen()
        {
            if (InfernalConfig.Instance.BossKillCheckOnOres)
            {
                if (InfernalCrossmod.SOTS.Loaded)
                {
                    SOTSWorldGenModifications.EmeraldGemChestNoHellstone();
                    SOTSWorldGenModifications.InvidiaChestHealingPotionNerf();
                    SOTSWorldGenModifications.ReplaceBlinkPackInSpecialChests();
                }

                for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];

                    if (chest is null)
                        continue;

                    bool isContainer1 = Main.tile[chest.x, chest.y].TileType == TileID.Containers;
                    bool isGoldChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 36 || Main.tile[chest.x, chest.y].TileFrameX == 2 * 36); // Includes Locked Gold Chests

                    // Fix vanilla's stupidity with Gold Chests being able to have Meteorite Bars in them near the Underworld
                    if (isGoldChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.MeteoriteBar)
                            {
                                int oldStack = chest.item[inventoryIndex].stack;
                                chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.NextBool() ? ItemID.PlatinumBar : ItemID.GoldBar);
                                chest.item[inventoryIndex].stack = oldStack;
                            }
                        }
                    }

                    // Replace ANY Hellstone bars in ANY chest with just hellstone ore.
                    for (int slot = 0; slot < Chest.maxItems; slot++)
                    {
                        Item item = chest.item[slot];

                        if (item.type == ItemID.HellstoneBar)
                        {
                            int bars = item.stack;

                            item.SetDefaults(ItemID.Hellstone);
                            item.stack = bars * 3; // optional conversion ratio
                        }
                    }
                }
            }
        }
    }

    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    public static class SOTSWorldGenModifications
    {
        public static void RunSOTSWorldGenMods(List<GenPass> tasks)
        {
            AddMutantModBarier(tasks);
        }

        public static void EmeraldGemChestNoHellstone()
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                    continue;

                Tile tile = Main.tile[chest.x, chest.y];
                if (tile == null)
                    continue;

                if (tile.TileType != GemStructureWorldgenHelper.RuinedChest)
                    continue;

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    Item item = chest.item[slot];

                    if (item == null || item.type != ItemID.HellstoneBar)
                        continue;

                    int replacement = WorldGen.crimson
                        ? ItemID.CrimtaneBar
                        : ItemID.DemoniteBar;

                    item.SetDefaults(replacement);
                    item.stack = 6;
                }
            }
        }

        public static void InvidiaChestHealingPotionNerf()
        {
            ushort invidiaChestTileType = (ushort)ModContent.TileType<InvidiaChestTile>();
            ushort ruinedChestTileType = (ushort)ModContent.TileType<RuinedChestTile>();
            ushort gulaVaultTileType = (ushort)ModContent.TileType<GulaVaultTile>();

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                    continue;

                Tile tile = Main.tile[chest.x, chest.y];
                if (tile == null || (tile.TileType != invidiaChestTileType && tile.TileType != ruinedChestTileType && tile.TileType != gulaVaultTileType))
                    continue;

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    Item item = chest.item[slot];
                    if (item == null || item.type != ItemID.GreaterHealingPotion)
                        continue;

                    int stack = item.stack;
                    item.SetDefaults(ItemID.HealingPotion);
                    item.stack = stack;
                }
            }
        }

        public static void ReplaceBlinkPackInSpecialChests()
        {
            int blinkPackType = ModContent.ItemType<BlinkPack>();
            int replacementType = ModContent.ItemType<FragmentOfChaos>();

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest is null)
                    continue;

                int x = chest.x;
                int y = chest.y;

                if (!WorldGen.InWorld(x, y, 1))
                    continue;

                Tile tile = Framing.GetTileSafely(x, y);
                ushort tileType = tile.TileType;

                bool validChest =
                    tileType == ModContent.TileType<LockedStrangeChest>() ||
                    tileType == ModContent.TileType<LockedSkywareChest>() ||
                    tileType == ModContent.TileType<LockedMeteoriteChest>();

                if (!validChest)
                    continue;

                for (int slot = 0; slot < Chest.maxItems; slot++)
                {
                    Item item = chest.item[slot];

                    if (item == null || item.IsAir || item.type != blinkPackType)
                        continue;

                    item.SetDefaults(replacementType);
                    item.stack = 3;
                }
            }
        }

        private static void AddMutantModBarier(List<GenPass> tasks)
        {
            if (ModLoader.HasMod("SecretsOfTheSouls") || !InfernalCrossmod.FargosMutant.Loaded)
                return;

            int sanctIdx = tasks.FindIndex(p => p.Name == "SOTS: Sanctuary");
            if (sanctIdx == -1) return;

            tasks.Insert(sanctIdx + 1, new PassLegacy(
                "Add Sanctuary Indestructible Zone",
                (progress, config) =>
                {
                    progress.Message = "Protecting the Sanctuary";

                    Rectangle worldRect = SanctuaryWorldgenHelper.Rectangle.Modified(-2, -2, 4, 4);

                    if (worldRect.Width > 0 && worldRect.Height > 0)
                    {
                        string command = "AddIndestructibleRectangle";
                        InfernalCrossmod.FargosMutant.Mod.Call(command, ToWorldCoords(worldRect));
                    }
                }
            ));
        }

        private static Rectangle ToWorldCoords(Rectangle rectangle)
        {
            return new Rectangle(rectangle.X * 16, rectangle.Y * 16, rectangle.Width * 16, rectangle.Height * 16);
        }
    }
}
