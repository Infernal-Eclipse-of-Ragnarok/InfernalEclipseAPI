using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Rarities;
using InfernalEclipseAPI.Content.Tiles.Paintings;

namespace InfernalEclipseAPI.Content.Items.Placeables.Paintings
{
    public class InfernalArsenalPainting : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 80;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 2, 0, 0); ;
            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.createTile = ModContent.TileType<InfernalArsenalTile>();
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                return;

            string tooltip = "";

            tooltip += "Low Percent Victors:\n";
            int lowPNamesPerLine = 8;
            for (int i = 0; i < lowPercentList.Count; i++)
            {
                tooltip += lowPercentList[i];

                if (i == lowPercentList.Count - 1)
                    break;

                if (i % lowPNamesPerLine == 0 && i != 0)
                    tooltip += "\n";
                else
                    tooltip += ", ";
            }
            tooltip += "\n";

            tooltip += "Whips Only Victors:\n";
            int whipNamesPerLine = 8;
            for (int i = 0; i < whipsList.Count; i++)
            {
                tooltip += whipsList[i];

                if (i == whipsList.Count - 1)
                    break;

                if (i % whipNamesPerLine == 0 && i != 0)
                    tooltip += "\n";
                else
                    tooltip += ", ";
            }
            tooltip += "\n";

            tooltip += "Boss Rush Victors:\n";
            int brNamesPerLine = 3;
            for (int i = 0; i < bossRushList.Count; i++)
            {
                tooltip += bossRushList[i];

                if (i == bossRushList.Count - 1)
                    break;

                if (i % brNamesPerLine == 0 && i != 0) 
                    tooltip += "\n";
                else 
                    tooltip += ", ";
            }

            TooltipLine line = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Tooltip3");
            if (line != null)
                line.Text = tooltip;
        }

        public static IList<string> lowPercentList = new List<string>()
        {
            "Georgerrier",
        };

        public static IList<string> whipsList = new List<string>()
        {
            "Exoegg"
        };

        public static IList<string> notHitList = new List<string>()
        {
            ""
        };

        public static IList<string> bossRushList = new List<string>()
        {
            "Bombshell",
            "Yardis"
        };
    }
}
