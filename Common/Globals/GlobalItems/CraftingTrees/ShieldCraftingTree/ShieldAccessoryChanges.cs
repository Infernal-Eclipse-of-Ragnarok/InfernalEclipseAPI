using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Accessories;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.ShieldCraftingTree
{
    public class ShieldCraftingTree : GlobalItem
    {
        private static Mod Thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thor);
                return thor;
            }
        }

        private static Mod SOTS
        {
            get
            {
                ModLoader.TryGetMod("SOTS", out Mod sots);
                return sots;
            }
        }

        private static Mod Clamity
        {
            get
            {
                ModLoader.TryGetMod("Clamity", out Mod clam);
                return clam;
            }
        }

        public override void SetDefaults(Item item)
        {
            if (item.ModItem != null && item.ModItem.Mod.Name == "ThoriumMod" && item.ModItem.Name == "TerrariumDefender")
            {
                item.defense = 8;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees)
                return;

            if (item.type == ModContent.ItemType<OrnateShield>() && SOTS != null)
            {
                SOTS.Find<ModItem>("ShatterHeartShield").UpdateAccessory(player, hideVisual);
            }

            if (item.type == ModContent.ItemType<AsgardsValor>() && Thorium != null && SOTS != null)
            {
                //ModItem moltenScale = thorium.Find<ModItem>("ObsidianScale");
                //moltenScale.UpdateAccessory(player, hideVisual);
                //base.UpdateAccessory(item, player, hideVisual);
                SOTS.Find<ModItem>("ShatterHeartShield").UpdateAccessory(player, hideVisual);
            }

            ModItem plasmaGen = null;
            if (Thorium != null)
            {
                if (Thorium.TryFind("PlasmaGenerator", out plasmaGen))
                {
                    if (item.type == plasmaGen.Type)
                    {
                        Thorium.Find<ModItem>("ObsidianScale").UpdateAccessory(player, hideVisual);
                    }
                }
            }

            if (item.type == ModContent.ItemType<AsgardianAegis>() && Thorium != null)
            {
                Thorium.Find<ModItem>("ObsidianScale").UpdateAccessory(player, hideVisual);
                if (plasmaGen != null)
                {
                    plasmaGen.UpdateAccessory(player, hideVisual);
                }
                if (SOTS != null)
                {
                    SOTS.Find<ModItem>("ShatterHeartShield").UpdateAccessory(player, hideVisual);
                }
            }

            if (item.ModItem != null && item.ModItem.Mod.Name == "ThoriumMod" && item.ModItem.Name == "TerrariumDefender")
            {
                player.longInvince = false;
                player.statLifeMax2 -= 20;

                if (SOTS != null)
                {
                    SOTS.Find<ModItem>("OlympianAegis").UpdateAccessory(player, hideVisual);
                    SOTS.Find<ModItem>("ChiseledBarrier").UpdateAccessory(player, hideVisual);
                }
                else
                {
                    Thorium.Find<ModItem>("LifeQuartzShield").UpdateAccessory(player, hideVisual);
                }

                if (player.statLife <= player.statLifeMax2 * 0.25)
                {
                    player.lifeRegen -= 10;
                }
            }

            if (item.type == ModContent.ItemType<DeificAmulet>())
            {
                /*
                if (thorium != null)
                {
                    thorium.Find<ModItem>("SweetVengeance").UpdateAccessory(player, hideVisual);
                }
                */
                base.UpdateAccessory(item, player, hideVisual);
            }

            if (item.ModItem != null && item.ModItem.Mod.Name == "ThoriumMod" && item.ModItem.Name == "MantleoftheProtector")
            {
                InfernalCrossmod.Calamity.Mod.Find<ModItem>("DeificAmulet").UpdateAccessory(player, hideVisual);
                Thorium.Find<ModItem>("CapeoftheSurvivor").UpdateAccessory(player, hideVisual);
                Thorium.Find<ModItem>("SweetVengeance").UpdateAccessory(player, hideVisual);
                base.UpdateAccessory(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<RampartofDeities>())
            {
                if (Thorium != null)
                {
                    player.noKnockback = true;
                    player.fireWalk = true;
                    player.buffImmune[20] = true;
                    player.buffImmune[22] = true;
                    player.buffImmune[23] = true;
                    player.buffImmune[30] = true;
                    player.buffImmune[31] = true;
                    player.buffImmune[32] = true;
                    player.buffImmune[33] = true;
                    player.buffImmune[35] = true;
                    player.buffImmune[36] = true;
                    player.buffImmune[46] = true;
                    player.buffImmune[47] = true;
                    player.buffImmune[156] = true;

                    if (SOTS != null)
                    {
                        SOTS.Find<ModItem>("OlympianAegis").UpdateAccessory(player, hideVisual);
                        SOTS.Find<ModItem>("ChiseledBarrier").UpdateAccessory(player, hideVisual);
                    }
                    else
                    {
                        Thorium.Find<ModItem>("LifeQuartzShield").UpdateAccessory(player, hideVisual);
                    }

                    if (player.statLife <= player.statLifeMax2 * 0.25)
                    {
                        if (Thorium.TryFind("TerrariumDefenderBuff", out ModBuff tdBuff))
                            player.AddBuff(tdBuff.Type, 10);

                        player.lifeRegen += 10;
                        player.statDefense += 20;
                    }
                    else if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer != player.whoAmI)
                    {
                        Player localPlayer = Main.LocalPlayer;
                        if (Vector2.DistanceSquared(localPlayer.Center, player.Center) < 160000f)
                            localPlayer.AddBuff(BuffID.Regeneration, 30);
                    }

                    Thorium.Find<ModItem>("MantleoftheProtector").UpdateAccessory(player, hideVisual);
                    Thorium.Find<ModItem>("CapeoftheSurvivor").UpdateAccessory(player, hideVisual);
                    //thorium.Find<ModItem>("SweetVengeance").UpdateAccessory(player, hideVisual);
                }
                else if (SOTS != null)
                {
                    SOTS.Find<ModItem>("OlympianAegis").UpdateAccessory(player, hideVisual);
                    SOTS.Find<ModItem>("ChiseledBarrier").UpdateAccessory(player, hideVisual);
                }
                base.UpdateAccessory(item, player, hideVisual);
            }

            if (item.ModItem != null && item.ModItem.Mod.Name == "Clamity" && item.ModItem.Name == "SupremeBarrier")
            {
                if (plasmaGen != null)
                {
                    plasmaGen.UpdateAccessory(player, hideVisual);
                }
                if (SOTS != null)
                {
                    player.noKnockback = true;
                    player.fireWalk = true;
                    player.buffImmune[20] = true;
                    player.buffImmune[22] = true;
                    player.buffImmune[23] = true;
                    player.buffImmune[30] = true;
                    player.buffImmune[31] = true;
                    player.buffImmune[32] = true;
                    player.buffImmune[33] = true;
                    player.buffImmune[35] = true;
                    player.buffImmune[36] = true;
                    player.buffImmune[46] = true;
                    player.buffImmune[47] = true;
                    player.buffImmune[156] = true;

                    SOTS.Find<ModItem>("ShatterHeartShield").UpdateAccessory(player, hideVisual);
                }

                if (Thorium != null)
                {
                    player.noKnockback = true;
                    player.fireWalk = true;
                    player.buffImmune[20] = true;
                    player.buffImmune[22] = true;
                    player.buffImmune[23] = true;
                    player.buffImmune[30] = true;
                    player.buffImmune[31] = true;
                    player.buffImmune[32] = true;
                    player.buffImmune[33] = true;
                    player.buffImmune[35] = true;
                    player.buffImmune[36] = true;
                    player.buffImmune[46] = true;
                    player.buffImmune[47] = true;
                    player.buffImmune[156] = true;

                    Thorium.Find<ModItem>("ObsidianScale").UpdateAccessory(player, hideVisual);
                    if (SOTS != null)
                    {
                        SOTS.Find<ModItem>("OlympianAegis").UpdateAccessory(player, hideVisual);
                        SOTS.Find<ModItem>("ChiseledBarrier").UpdateAccessory(player, hideVisual);
                    }
                    else
                    {
                        Thorium.Find<ModItem>("LifeQuartzShield").UpdateAccessory(player, hideVisual);
                    }

                    if (player.statLife <= player.statLifeMax2 * 0.25)
                    {
                        if (Thorium.TryFind("TerrariumDefenderBuff", out ModBuff tdBuff))
                            player.AddBuff(tdBuff.Type, 10);

                        player.lifeRegen += 10;
                        player.statDefense += 20;
                    }
                    else if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer != player.whoAmI)
                    {
                        Player localPlayer = Main.LocalPlayer;
                        if (Vector2.DistanceSquared(localPlayer.Center, player.Center) < 160000f)
                            localPlayer.AddBuff(BuffID.Regeneration, 30);
                    }

                    Thorium.Find<ModItem>("MantleoftheProtector").UpdateAccessory(player, hideVisual);
                    Thorium.Find<ModItem>("CapeoftheSurvivor").UpdateAccessory(player, hideVisual);
                    //thorium.Find<ModItem>("SweetVengeance").UpdateAccessory(player, hideVisual);
                }
                else if (SOTS != null)
                {
                    SOTS.Find<ModItem>("OlympianAegis").UpdateAccessory(player, hideVisual);
                    SOTS.Find<ModItem>("ChiseledBarrier").UpdateAccessory(player, hideVisual);
                }
                base.UpdateAccessory(item, player, hideVisual);
            }
        }

        public void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip, bool InfernalRedActive = false, bool NoSOTSPinkActive = false)
        {
            Color InfernalRed = Color.Lerp(
               Color.White,
               new Color(255, 80, 0), // Infernal red/orange
               (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );
            Color NoSOTSPink = Color.Lerp(
                Color.White,
                new Color(251, 198, 207),
                (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );

            int maxTooltipIndex = -1;
            int maxNumber = -1;

            // Find the TooltipLine with the highest TooltipX name
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.StartsWith("Tooltip"))
                {
                    if (int.TryParse(tooltips[i].Name.Substring(7), out int num) && num > maxNumber)
                    {
                        maxNumber = num;
                        maxTooltipIndex = i;
                    }
                }
            }

            // If found, insert a new TooltipLine right after it with the desired color
            if (maxTooltipIndex != -1)
            {
                int insertIndex = maxTooltipIndex + 1;
                TooltipLine customLine = new TooltipLine(Mod, "StealthTooltip", stealthTooltip);
                if (InfernalRedActive)
                    customLine.OverrideColor = InfernalRed;

                tooltips.Insert(insertIndex, customLine);
            }
        }


        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees)
                return;

            Color InfernalRed = Color.Lerp(
               Color.White,
               new Color(255, 80, 0), // Infernal red/orange
               (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );
            Color NoSOTSPink = Color.Lerp(
                Color.White,
                new Color(251, 198, 207),
                (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );

            string moltenScaleInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.MoltenScale");
            string chiseledBarrierInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.ChiseledBarrier");
            string chiseledHiddenInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.ChiseledHidden");
            string olympianAegisInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Olympian");
            string lifeQuartzShieldInfo1 = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.lifeQuartzShield.CommoTooltip");
            string lifeUnder25Info = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.lifeQuartzShield.Under25HP1");
            string lifeUnder25Info2 = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.lifeQuartzShield.Under25HP2");
            string motpInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Mantle");
            string cotsInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.COTS");
            string sweetInfo1 = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Sweet1");
            string sweetInfo2 = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SweetAlt");
            string sweetAltInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Sweet2");
            string tdInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.TerraDefend");
            string daInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.DA");
            string shsInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SHS");
            string pgInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.PlasmaGen");
            string ankhInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.TerrariumDefender.immunity");

            if (SOTS != null && (item.type == ModContent.ItemType<AsgardsValor>() || item.type == ModContent.ItemType<AsgardianAegis>() || item.type == ModContent.ItemType<OrnateShield>()))
            {
                AddTooltip(tooltips, shsInfo, true);
            }

            ModItem plasmaGen = null;
            if (Thorium != null)
            {
                if (Thorium.TryFind("PlasmaGenerator", out plasmaGen))
                {
                    if (item.type == plasmaGen.Type)
                    {
                        AddTooltip(tooltips, moltenScaleInfo, true);
                    }
                }
            }

            if (item.type == ModContent.ItemType<AsgardianAegis>() & Thorium != null)
            {
                AddTooltip(tooltips, moltenScaleInfo, true);
                if (plasmaGen != null)
                {
                    AddTooltip(tooltips, pgInfo, true);
                }
            }

            if (Thorium != null)
            {
                if (item.type == ModContent.ItemType<DeificAmulet>())
                {
                    /*
                    AddTooltip(tooltips, sweetInfo1, true);
                    AddTooltip(tooltips, sweetAltInfo, true);
                    */
                }

                if (item.type == Thorium.Find<ModItem>("MantleoftheProtector").Type)
                {
                    AddTooltip(tooltips, cotsInfo, true);
                    AddTooltip(tooltips, sweetInfo1, true);
                    AddTooltip(tooltips, sweetInfo2, true);
                    AddTooltip(tooltips, daInfo, true);
                }

                if (SOTS != null)
                {
                    if (item.type == Thorium.Find<ModItem>("TerrariumDefender").Type)
                    {
                        foreach (TooltipLine tooltip in tooltips)
                        {
                            if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.TerrariumDefender.lifeUP")))
                            {
                                tooltip.Text = chiseledBarrierInfo;
                                tooltip.OverrideColor = new Color?(InfernalRed);
                            }
                            if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.TerrariumDefender.invincibility")))
                            {
                                tooltip.Text = olympianAegisInfo + "\n" + chiseledHiddenInfo;
                                tooltip.OverrideColor = new Color?(InfernalRed);
                            }
                        }
                    }
                    if (Clamity != null)
                    {
                        if (item.type == Clamity.Find<ModItem>("SupremeBarrier").Type)
                        {
                            AddTooltip(tooltips, shsInfo, true);
                            if (plasmaGen != null)
                            {
                                AddTooltip(tooltips, pgInfo, true);
                            }
                            AddTooltip(tooltips, moltenScaleInfo, true);

                            AddTooltip(tooltips, tdInfo, true);

                            AddTooltip(tooltips, motpInfo, true);
                            
                            AddTooltip(tooltips, chiseledHiddenInfo, true);
                            AddTooltip(tooltips, chiseledBarrierInfo, true);
                            AddTooltip(tooltips, olympianAegisInfo, true);

                            AddTooltip(tooltips, cotsInfo, true);
                            //AddTooltip(tooltips, sweetInfo1, true);
                            //AddTooltip(tooltips, sweetInfo2, true);

                            AddTooltip(tooltips, ankhInfo, true);
                        }
                    }
                    if (item.type == ModContent.ItemType<RampartofDeities>())
                    {
                        AddTooltip(tooltips, tdInfo, true);

                        AddTooltip(tooltips, motpInfo, true);

                        AddTooltip(tooltips, chiseledHiddenInfo, true);
                        AddTooltip(tooltips, chiseledBarrierInfo, true);
                        AddTooltip(tooltips, olympianAegisInfo, true);

                        AddTooltip(tooltips, cotsInfo, true);
                        //AddTooltip(tooltips, sweetInfo1, true);
                        //AddTooltip(tooltips, sweetInfo2, true);

                        AddTooltip(tooltips, ankhInfo, true);
                    }
                }
                else
                {
                    if (item.type == Thorium.Find<ModItem>("TerrariumDefender").Type)
                    {
                        foreach (TooltipLine tooltip in tooltips)
                        {
                            if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.TerrariumDefender.lifeUP")))
                            {
                                tooltip.Text = lifeQuartzShieldInfo1;
                                tooltip.OverrideColor = new Color?(NoSOTSPink);
                            }
                            if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.TerrariumDefender.invincibility")))
                            {
                                tooltip.Text = lifeUnder25Info + "\n" + lifeUnder25Info2;
                                tooltip.OverrideColor = new Color?(NoSOTSPink);
                            }
                        }
                    }
                    if (Clamity != null)
                    {
                        if (item.type == Clamity.Find<ModItem>("SupremeBarrier").Type)
                        {
                            if (plasmaGen != null)
                            {
                                AddTooltip(tooltips, pgInfo, true);
                            }
                            AddTooltip(tooltips, tdInfo, true);

                            AddTooltip(tooltips, lifeQuartzShieldInfo1, false, true);
                            AddTooltip(tooltips, lifeUnder25Info, false, true);
                            AddTooltip(tooltips, lifeUnder25Info2, false, true);

                            AddTooltip(tooltips, cotsInfo, true);
                            //AddTooltip(tooltips, sweetInfo1, true);
                            //AddTooltip(tooltips, sweetInfo2, true);

                            AddTooltip(tooltips, moltenScaleInfo, true);

                            AddTooltip(tooltips, ankhInfo, true);
                        }
                    }
                    if (item.type == ModContent.ItemType<RampartofDeities>())
                    {
                        AddTooltip(tooltips, tdInfo, true);

                        AddTooltip(tooltips, lifeQuartzShieldInfo1, false, true);
                        AddTooltip(tooltips, lifeUnder25Info, false, true);
                        AddTooltip(tooltips, lifeUnder25Info2, false, true);

                        AddTooltip(tooltips, cotsInfo, true);
                        //AddTooltip(tooltips, sweetInfo1, true);
                        //AddTooltip(tooltips, sweetInfo2, true);

                        AddTooltip(tooltips, ankhInfo, true);
                    }
                }
            }
            else if (SOTS != null)
            {
                if (Clamity != null)
                {
                    if (item.type == Clamity.Find<ModItem>("SupremeBarrier").Type)
                    {
                        AddTooltip(tooltips, shsInfo, true);
                        
                        AddTooltip(tooltips, chiseledHiddenInfo, true);
                        AddTooltip(tooltips, chiseledBarrierInfo, true);
                        AddTooltip(tooltips, olympianAegisInfo, true);

                        AddTooltip(tooltips, ankhInfo, true);
                    }
                }
                if (item.type == ModContent.ItemType<RampartofDeities>())
                {
                    AddTooltip(tooltips, chiseledHiddenInfo, true);
                    AddTooltip(tooltips, chiseledBarrierInfo, true);
                    AddTooltip(tooltips, olympianAegisInfo, true);

                    AddTooltip(tooltips, ankhInfo, true);
                }
            }
        }
    }
}
