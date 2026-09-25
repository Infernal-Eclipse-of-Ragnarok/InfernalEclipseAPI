using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using InfernalEclipseAPI.Content.Tiles.Paintings;

namespace InfernalEclipseAPI.Content.Items.Placeables.Paintings
{
    public class InfernalTwilight : ModItem
    {
        public override string Texture => "InfernalEclipseAPI/icon";
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
            Item.rare = ItemRarityID.White;
            if (ModLoader.TryGetMod("NoxusBoss", out Mod noxus) && noxus.TryFind("NamelessDeityRarity", out ModRarity r))
                Item.rare = r.Type;
            Item.createTile = ModContent.TileType<InfernalTwilightTile>();
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                return;

            string tooltip = "";

            int namesPerLine = 8;
            for (int i = 0; i < devList.Count; i++)
            {
                tooltip += devList[i];

                if (i == devList.Count - 1)
                    break;

                if (i % namesPerLine == 0 && i != 0)
                    tooltip += "\n";
                else
                    tooltip += ", ";
            }
            tooltip += "\n";

            int teamsPerLine = 3;
            for (int i = 0; i < teamList.Count; i++)
            {
                tooltip += teamList[i];

                if (i == teamList.Count - 1)
                    break;

                if (i % teamsPerLine == 0 && i != 0) 
                    tooltip += "\n";
                else 
                    tooltip += ", ";
            }

            TooltipLine line = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Tooltip3");
            if (line != null)
                line.Text = tooltip;
        }

        public static IList<string> devList = new List<string>()
        {
            //Developers
            "Akira",
            "WardrobeHummus",
            "Advantaje",

            //Designers
            "Tyr",
            "ma3allim",
            "Yob",
            "Soltan",

            //Major Contributors
            "StarlightCat",
            "MajOfMyth",
            "Ropro0923",
            "Pixus",
            "cheesenuggets",
            "Prime",
            "Mr. Puzzles",
            "Dandel",
            "Nobody",
            "Pudd1ng",
            "Eduarrdo",
            "JalapenoDance",
            "Arkangel",
            "Eram",
            "IceSpider",

            //Composers
            "PFungi",
            "Techhy",

            //Partners
            "brome",
            "WitherTaco",
            "Verveine",
            "CrafterDaemon",

            //Other
            "Kes",
            "Feldy",
            "TacoConKvass",
            "KainSett",
            "nicole",
            "KaiTheExaminer",
            "LeEr206",
            "Pil",
            "Yelmut",
            "yfn",
            "Aizen522",
            "De1tA5",
            "Suu",
            "fiend",
            "JuJiao",
            "Uncle Sciz",
            "cleo.",
            "ABrickToTheHead",
            "Ino",

            //Playtesters
            "Tencvin",
            "Jillyfish",
            "DaVids",
            "Georgerrier",
            "rose",

            //Management
            "TheRogueX",

            //Donors
            "bryce27", 
            "Bomberr",
            "Goldsockz2",
            "Minesky",
            "TechQueen22",
            "Sir_Yeetus_III",
            "rocket",
            "FlamingSock",
            "brxdyfrxmdxg",
            "Alextricle",
            "StormUprising",
            "Shanakor",
            "Cherie",
            "Yardis",
            "Heatstroke",
            "Dylanomeneer",
            "Redwort",

            //Supporters
            "CuddlySnake",
            "joshuwham",
            "Lucy",
            "Mando",
            "Romain TISSERAND",
            "Imagiknight_Parad0xical",
            "Soopns",

            //Helpers
            "fjiown",
            "DuskMender",
            "Coof",

            //Special Thanks
            "N0t_UNowen",
            "Blockaroz",
            "javyz",
            "habble",
            "Lucille Karma",
            "tomat",
            "Nycro",
            "Blast",
            "Vortex",
            "Cataclysmic Armageddon",
            "Yardis",
        };

        public static IList<string> teamList = new List<string>()
        {
            "Calamity Mod Team",
            "Fargo Team",
            "Thorium Team",
            "Secrets of the Shadows Team",
            "Consolaria Team",
            //"Heartbeataria Team",
            "Ragnarok Development Team",
            "Bereft Souls Team"
        };
    }
}
