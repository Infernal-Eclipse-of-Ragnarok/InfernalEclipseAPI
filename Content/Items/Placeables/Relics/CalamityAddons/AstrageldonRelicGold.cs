using InfernalEclipseAPI.Content.Tiles.Relics.CalamityAddons;

namespace InfernalEclipseAPI.Content.Items.Placeables.Relics.CalamityAddons
{
    public class AstrageldonRelicGold : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AstrageldonRelicTileGold>(), 0);
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
        }
    //    public override string Texture => "InfernalEclipseAPI/Content/Items/Placeables/Relics/CalamityAddons/AstrageldonRelicGoldTile";
    }
}
