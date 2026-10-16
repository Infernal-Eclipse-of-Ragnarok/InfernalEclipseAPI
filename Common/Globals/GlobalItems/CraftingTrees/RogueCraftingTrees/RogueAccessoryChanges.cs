using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Configs;
using ThrowerUnification;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.RogueCraftingTrees
{
    public class RogueAccessoryChanges : GlobalItem
    {
        private Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thor);
                return thor;
            }
        }

        private Mod clamity
        {
            get
            {
                ModLoader.TryGetMod("Clamity", out Mod clam);
                return clam;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees)
                return;

            if (thorium != null)
            {
                //Vampiric Talisman
                if (item.type == ModContent.ItemType<VampiricTalisman>() || item.type == ModContent.ItemType<Nanotech>())
                {
                    ModItem shinobiSigil = thorium.Find<ModItem>("ShinobiSigil");
                    shinobiSigil.UpdateAccessory(player, hideVisual);
                }

                if (clamity != null)
                {
                    if (item.type == clamity.Find<ModItem>("DraculasCharm").Type)
                    {
                        ModItem shinobiSigil = thorium.Find<ModItem>("ShinobiSigil");
                        shinobiSigil.UpdateAccessory(player, hideVisual);
                    }
                }

                //Nanotech
                ModItem scutterGem = InfernalCrossmod.Calamity.Mod.Find<ModItem>("ScuttlersJewel");

                if (item.type == thorium.Find<ModItem>("BoneGrip").Type)
                {
                    scutterGem.UpdateAccessory(player, hideVisual);
                }

                if (item.type == thorium.Find<ModItem>("MagnetoGrip").Type)
                {
                    scutterGem.UpdateAccessory(player, hideVisual);
                }

                if (item.type == ModContent.ItemType<Nanotech>() || item.type == ModContent.ItemType<ElectriciansGlove>())
                {
                    scutterGem.UpdateAccessory(player, hideVisual);
                    ModItem magnetoGrip = thorium.Find<ModItem>("MagnetoGrip");
                    magnetoGrip.UpdateAccessory(player, hideVisual);
                }

                if (InfernalCrossmod.FargosSouls.Loaded)
                {
                    if (item.type == InfernalCrossmod.FargosSouls.Mod.Find<ModItem>("UniverseSoul").Type)
                    {
                        scutterGem.UpdateAccessory(player, hideVisual);
                        ModItem magnetoGrip = thorium.Find<ModItem>("MagnetoGrip");
                        magnetoGrip.UpdateAccessory(player, hideVisual);
                    }

                    if (item.type == InfernalCrossmod.FargosSouls.Mod.Find<ModItem>("EternitySoul").Type)
                    {
                        scutterGem.UpdateAccessory(player, hideVisual);
                        ModItem magnetoGrip = thorium.Find<ModItem>("MagnetoGrip");
                        magnetoGrip.UpdateAccessory(player, hideVisual);
                    }

                    if (InfernalCrossmod.FargosDLC.Loaded)
                    {
                        if (item.type == InfernalCrossmod.FargosDLC.Mod.Find<ModItem>("VagabondsSoul").Type)
                        {
                            scutterGem.UpdateAccessory(player, hideVisual);
                            ModItem magnetoGrip = thorium.Find<ModItem>("MagnetoGrip");
                            magnetoGrip.UpdateAccessory(player, hideVisual);
                        }
                    }
                }
            }
        }

        public void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip, bool InfernalRedActive = false)
        {
            Color InfernalRed = Color.Lerp(
               Color.White,
               new Color(255, 80, 0), // Infernal red/orange
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

            string scuttlerInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Scuttler");
            string boneInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Bone");
            string boneInfoNew = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Bone2");
            //string bloodyfilthyInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.BloodyFilthy");
            //string magnetoInfo = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Magneto");

            string shinobiSigil = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.ShinobiEffect");

            if (thorium != null)
            {
                if (item.type == thorium.Find<ModItem>("ShinobiSigil").Type)
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.ShinobiNerf"), true);
                }

                if (item.type == ModContent.ItemType<VampiricTalisman>() || item.type == ModContent.ItemType<Nanotech>())
                {
                    AddTooltip(tooltips, shinobiSigil, true);
                }

                if (clamity != null)
                {
                    if (item.type == clamity.Find<ModItem>("DraculasCharm").Type)
                    {
                        AddTooltip(tooltips, shinobiSigil, true);
                    }
                }

                if (item.type == thorium.Find<ModItem>("BoneGrip").Type)
                {
                    AddTooltip(tooltips, scuttlerInfo, true);
                }

                if (item.type == thorium.Find<ModItem>("MagnetoGrip").Type)
                {
                    AddTooltip(tooltips, scuttlerInfo, true);
                }

                if (item.type == ModContent.ItemType<Nanotech>() || item.type == ModContent.ItemType<ElectriciansGlove>())
                {
                    //AddTooltip(tooltips, magnetoInfo, true);

                    if (ThrowerModConfig.Instance.ConsumableWeaponConversion)
                        AddTooltip(tooltips, boneInfoNew, true);
                    else
                        AddTooltip(tooltips, boneInfo, true);

                    AddTooltip(tooltips, scuttlerInfo, true);
                }
            }
        }
    }
}
