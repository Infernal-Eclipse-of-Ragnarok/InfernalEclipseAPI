using System.Collections.Generic;
using ThoriumMod;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems.ItemReworks
{
    [ExtendsFromMod("ThoriumMod")]
    public class EclipseFangAlwaysActive : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if (!InfernalCrossmod.Thorium.Mod.TryFind("EclipseFang", out ModItem modItem) || item.type != modItem.Type || !InfernalConfig.Instance.ThoriumBalanceChangess) 
                return base.UseItem(item, player);

            var thoriumPlayer = player.GetModPlayer<ThoriumPlayer>();
            thoriumPlayer.itemEclipseFangCharge = 40;

            return base.UseItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!InfernalCrossmod.Thorium.Mod.TryFind("EclipseFang", out ModItem modItem) || item.type != modItem.Type || !InfernalConfig.Instance.ThoriumBalanceChangess) 
                return;

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
                TooltipLine customLine = new TooltipLine(ModLoader.GetMod("InfernalEclipseAPI"), "TooltipZ", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.EclipseFang"));
                customLine.OverrideColor = Color.Lerp(Color.White, new Color(30, 144, byte.MaxValue), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5));
                tooltips.Insert(insertIndex, customLine);
            }
        }
    }
}
