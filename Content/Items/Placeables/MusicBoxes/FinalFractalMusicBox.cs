using CalamityMod.Rarities;
using InfernalEclipseAPI.Content.Tiles.MusicBoxes;
using YouBoss.Content.Items.SummonItems;

namespace InfernalEclipseAPI.Content.Items.Placeables.MusicBoxes
{
    [JITWhenModsEnabled("YouBoss")]
    [ExtendsFromMod("YouBoss")]
    public class FinalFractalMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (Main.dedServ)
                return;
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = 576;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot("YouBoss/Assets/Sounds/Music/You"), ModContent.ItemType<FinalFractalMusicBox>(), ModContent.TileType<FinalFractalMusicBoxTile>(), 0);
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<FinalFractalMusicBoxTile>();
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Terraria.Item.sellPrice(silver: 20);
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient<CursedMirror>()
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
