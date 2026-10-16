using CalamityMod.Items;
using CalamityMod.Items.Materials;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using NoxusBoss.Content.Items;
using NoxusBoss.Content.Rarities;
using NoxusBoss.Content.Tiles;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Content.Items.Materials
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class AlloyofEden : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 18));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<GenesisComponentRarity>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient<ShadowspecBar>();
            recipe.AddIngredient<MetallicChunk>();
            recipe.AddIngredient<PrimordialOrchid>();
            recipe.AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition);
            recipe.AddTile<StarlitForgeTile>();
            recipe.Register();
        }
    }
}
