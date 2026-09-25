using InfernumMode.Content.Items.Relics;
using InfernalEclipseAPI.Content.Tiles.MusicBoxes;
using ThoriumMod.Items.Placeable.Relics;

namespace InfernalEclipseAPI.Content.Items.Placeables.MusicBoxes
{
    public class QueenJellyfishMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (Main.dedServ)
                return;
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = 576;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/MutantMajesty"), Type, ModContent.TileType<QueenJellyfishMusicBoxTile>(), 0);
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<QueenJellyfishMusicBoxTile>();
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
            Item.accessory = true;
        }

    /*    public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<QueenJellyfishRelic>(1)
                .AddIngredient(ItemID.MusicBox)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    */
    }
}
