using CalamityMod;
using CalamityMod.Items.Accessories;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players.SOTSPlayerOverrides;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Items.Chaos;
using SOTS.Items.Wings;
using SOTS.Void;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.InsigniaCraftingTree
{
    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class InsigniaAccessoryChanges : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<AscendantInsignia>() || entity.type == ModContent.ItemType<GildedBladeWings>() || 
                                                                                     entity.type == ModContent.ItemType<MachinaBooster>() || entity.type == ModContent.ItemType<SpiritInsignia>();

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<AscendantInsignia>() && InfernalConfig.Instance.MergeCraftingTrees)
            {
                SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
                VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
                ++voidPlayer.bonusVoidGain;
                voidPlayer.voidRegenSpeed += 0.25f;
                sotsPlayer.SpiritSymphony = true;
            }

            if (item.type == ModContent.ItemType<GildedBladeWings>())
            {
                if (InfernalConfig.Instance.MergeCraftingTrees)
                {
                    player.Calamity().ascendantInsignia = true;
                }
                MachinaBoosterPlayer modPlayer = player.GetModPlayer<MachinaBoosterPlayer>();
                modPlayer.CreativeFlightTier2 = false;
                player.wingTimeMax = 350;
                player.wingAccRunSpeed = 11f;
                player.wingRunAccelerationMult = 2f;
                player.GetModPlayer<SOTSPlayerAdjustments>().bladeWings = true;
            }
        }

        private void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip, bool InfernalRedActive = false, bool CalNerf = false)
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
                TooltipLine customLine = new(Mod, "StealthTooltip", stealthTooltip);
                if (InfernalRedActive)
                    customLine.OverrideColor = InfernalRed;
                if (CalNerf)
                    customLine.OverrideColor = Color.Red;

                tooltips.Insert(insertIndex, customLine);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string spirtInfo1 = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SpiritInsignia.Effect");

            if (item.type == ModContent.ItemType<SpiritInsignia>() && InfernalConfig.Instance.MergeCraftingTrees)
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SpiritInsignia.Replace"));
            }

            if (item.type == ModContent.ItemType<AscendantInsignia>() && InfernalConfig.Instance.MergeCraftingTrees)
            {
                AddTooltip(tooltips, spirtInfo1, true);
            }

            string str1 = Language.GetTextValue("Mods.SOTS.Common.Unbound");
            string str2 = str1;
            using (List<string>.Enumerator enumerator = SOTS.SOTS.MachinaBoosterHotKey.GetAssignedKeys((InputMode)0).GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str1 = enumerator.Current;
            }
            using (List<string>.Enumerator enumerator = SOTS.SOTS.SlowFlightHotKey.GetAssignedKeys((InputMode)0).GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str2 = enumerator.Current;
            }

            if (item.type == ModContent.ItemType<MachinaBooster>())
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.MachinaBooster.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.MachinaBooster.Replace", str1));
            }

            if (item.type == ModContent.ItemType<GildedBladeWings>())
            {
                if (InfernalConfig.Instance.MergeCraftingTrees)
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.GildedBladeWings.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.GildedBladeWings.Replace", str1, str2));
                }

                ModKeybind keybind = item.GetDynamicModHotkey();

                string key = keybind?.TooltipHotkeyString() ?? Language.GetTextValue("Mods.CalamityMod.Misc.HotkeyNotEquipped");

                string tooltip = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.AscendantInsignia", AscendantInsignia.AbilityDuration.FramesToSeconds(), AscendantInsignia.AbilityCooldown.FramesToSeconds());

                AddTooltip(tooltips, tooltip.Replace("[KEY]", key), true);

                AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.GildedVoid"), true);
            }
        }
    }
}
