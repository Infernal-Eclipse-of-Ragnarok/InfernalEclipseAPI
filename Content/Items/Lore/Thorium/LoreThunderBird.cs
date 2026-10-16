using System.Collections.Generic;
using CalamityMod.Items.LoreItems;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;
using ThoriumMod.Items.BossTheGrandThunderBird;

namespace InfernalEclipseAPI.Content.Items.Lore.Thorium
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class LoreThunderBird : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Item.ResearchUnlockCount = 1;
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
                .AddIngredient(ItemID.Book)
                .AddIngredient<TheGrandThunderBirdTrophy>()
                .AddTile(TileID.Bookcases)
                .DisableDecraft()
                .Register();
        }
    }
}
