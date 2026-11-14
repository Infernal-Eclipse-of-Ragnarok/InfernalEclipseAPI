using CalamityMod.Items.LoreItems;
using Consolaria.Content.Items.Placeable;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Lore.Consolaria
{
    [JITWhenModsEnabled(InfernalCrossmod.Consolaria.Name)]
    [ExtendsFromMod(InfernalCrossmod.Consolaria.Name)]
    public class LoreLepus : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LepusTrophy>())
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}