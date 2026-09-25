using System.Collections.Generic;
using Terraria.Audio;
using ThoriumMod.NPCs.BossLich;
using ThoriumMod.Tiles;
using Microsoft.Xna.Framework;
using ThoriumMod.Utilities;
using CalamityMod.Items.Materials;
using ThoriumMod.Items.Misc;
using MonoMod.RuntimeDetour;
using System.Reflection;
using SOTS;
using InfernalEclipseAPI.Core.Systems;
using ThoriumMod.Items.ZRemoved;
using Terraria.Localization;
using ThoriumMod.Core.Handlers.HoverItemHandler;

namespace InfernalEclipseAPI.Content.Items.SpawnItems
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class SuspiciousLookingPumpkin : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 4988; //Queen Slime Summon
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.rare = ItemRarityID.LightPurple;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pumpkin, 30)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient<SoulofPlight>(5)
                .AddIngredient<EssenceofHavoc>(3)
                .AddTile<SoulForge>()
                .Register();
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class AncientPhylacteryAdjustments : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (type == ModContent.TileType<AncientPhylactery>())
            {
                if (!AncientPhylactery.DownedAllMechBosses)
                {
                    base.RightClick(i, j, type);
                }
                else
                {
                    bool canSummon = true;

                    if (InfernalCrossmod.SOTS.Loaded)
                    {
                        if (!AncientPhylacteryRightClickBlocker.DownedPolaris)
                        {
                            canSummon = false;
                        }
                    }

                    Player localPlayer = Main.LocalPlayer;
                    if (Main.IsItDay())
                    {
                        base.RightClick(i, j, type);
                    }
                    else if (canSummon)
                    {
                        if ((NPC.AnyNPCs(ModContent.NPCType<Lich>()) ? 1 : (NPC.AnyNPCs(ModContent.NPCType<LichHeadless>()) ? 1 : 0)) != 0)
                        {
                            base.RightClick(i, j, type);
                        }
                        else
                        {
                            Dictionary<int, int> hasSusPumpkin = localPlayer.CountInventoryItemIdxWithStack(ModContent.ItemType<SuspiciousLookingPumpkin>(), 1);
                            if (hasSusPumpkin.Count > 0)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath7, new Vector2?(localPlayer.Center), null);
                                int num1 = i;
                                Tile tile1 = Main.tile[i, j];
                                int num2 = tile1.TileFrameX / 18 % 3;
                                int num3 = num1 - num2;
                                int num4 = j;
                                Tile tile2 = Main.tile[i, j];
                                int num5 = tile2.TileFrameY / 18 % 3;
                                int num6 = num4 - num5;
                                Vector2 center = new Vector2((num3 + 1 + 0.5f) * 16f, (num6 + 1f - 0.2f) * 16f);
                                float dustCount = 50f;
                                for (int a = 0; a < dustCount; a++)
                                {
                                    // Angle around the circle
                                    double angle = i * (TwoPi / dustCount);

                                    // Get rotated offset
                                    Vector2 offset = Utils.RotatedBy(Vector2.UnitY * -1f, angle) * new Vector2(30f, 30f);

                                    // Spawn the dust
                                    int dustIndex = Dust.NewDust(center, 0, 0, DustID.GemAmethyst, 0f, 0f, 100, default, 1.25f);
                                    Dust dust = Main.dust[dustIndex];
                                    dust.noGravity = true;
                                    dust.position = center + offset;
                                    dust.velocity = Vector2.Normalize(offset) * 6f;
                                }
                                SoundEngine.PlaySound(SoundID.Roar, new Vector2?(localPlayer.Center), null);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NPC.SpawnOnPlayer(localPlayer.whoAmI, ModContent.NPCType<Lich>());
                                else
                                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, localPlayer.whoAmI, ModContent.NPCType<Lich>(), 0.0f, 0.0f, 0, 0, 0);
                            }
                        }
                    }
                }
            }
            base.RightClick(i, j, type);
        }

        public override void MouseOver(int i, int j, int type)
        {
            if (type == ModContent.TileType<AncientPhylactery>())
            {
                if (!AncientPhylactery.DownedAllMechBosses)
                {
                    base.MouseOver(i, j, type);
                    return;
                }

                if (Main.IsItDay())
                {
                    base.MouseOver(i, j, type);
                    return;
                }

                Player localPlayer = Main.LocalPlayer;

                if (InfernalCrossmod.SOTS.Loaded)
                {
                    if (!AncientPhylacteryRightClickBlocker.DownedPolaris)
                    {
                        HoverItemSystem.QueueHoverItem(0, 0);

                        int cursorItemIconId1 = localPlayer.cursorItemIconID;
                        localPlayer.cursorItemIconID = ModContent.ItemType<LichRequirement3>();
                        int cursorItemIconId2 = localPlayer.cursorItemIconID;
                        if (cursorItemIconId1 == cursorItemIconId2)
                            return;
                        localPlayer.noThrow = 2;
                        localPlayer.cursorItemIconText = "";
                        localPlayer.cursorItemIconEnabled = true;
                        return;
                    }
                }

                Dictionary<int, int> hasSusPumpkin = localPlayer.CountInventoryItemIdxWithStack(ModContent.ItemType<SuspiciousLookingPumpkin>(), 1);
                if (hasSusPumpkin.Count > 0)
                {
                    localPlayer.noThrow = 2;
                    HoverItemSystem.QueueHoverItem(ModContent.ItemType<SuspiciousLookingPumpkin>(), 1);
                }
            }
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class AncientPhylacteryRightClickBlocker : ModSystem
    {
        private Hook rightClickHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                return;

            ModTile tile = thorium.Find<ModTile>("AncientPhylactery");
            MethodInfo method = tile.GetType().GetMethod("RightClick", BindingFlags.Instance | BindingFlags.Public);

            if (method is null)
                return;

            rightClickHook = new Hook(method, RightClickHook);
        }

        public override void Unload()
        {
            rightClickHook?.Dispose();
            rightClickHook = null;
        }

        private static bool RightClickHook(Func<object, int, int, bool> orig, object self, int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!DownedPolaris)
            {
                Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.LichMessage"), Color.LightBlue);
                return false;
            }

            return orig(self, i, j);
        }

        public static bool DownedExcavator => SOTSWorld.downedExcavator;
        public static bool DownedAdvisor => SOTSWorld.downedAdvisor;
        public static bool DownedPolaris => SOTSWorld.downedAmalgamation;
    }
}
