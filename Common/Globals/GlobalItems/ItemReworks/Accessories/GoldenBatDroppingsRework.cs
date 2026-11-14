using System.Collections.Generic;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using RagnarokMod.Items.RevItems;
using RagnarokMod.Utils;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Accessories
{
    [JITWhenModsEnabled(InfernalCrossmod.RagnarokMod.Name)]
    [ExtendsFromMod(InfernalCrossmod.RagnarokMod.Name)]
    public class GoldenBatDroppingsRework : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            return item.type == ModContent.ItemType<GoldenBatDroppings>();
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetRagnarokModPlayer().batpoop = false;
            player.GetModPlayer<InfernalPlayer>().batPoop = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; ++i)
            {
                if (tooltips[i].Mod == "Terraria")
                {
                    if (tooltips[i].Name == "Tooltip0")
                    {
                        TooltipLine tooltip = tooltips[i];

                        tooltip.Text = $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GoldenBatRework")}";
                    }
                    else if (tooltips[i].Name.Contains("Tooltip"))
                    {
                        tooltips[i].Hide();
                    }
                }
            }
        }
    }
}
