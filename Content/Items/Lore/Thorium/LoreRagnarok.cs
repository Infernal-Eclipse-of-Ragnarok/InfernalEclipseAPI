using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items.LoreItems;
using CalamityMod.Rarities;
using InfernalEclipseAPI.Content.Items.Materials;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;
using ThoriumMod.Items.BossThePrimordials;

namespace InfernalEclipseAPI.Content.Items.Lore.Thorium
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class LoreRagnarok : LoreItem
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
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient<DreamEssence>()
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}
