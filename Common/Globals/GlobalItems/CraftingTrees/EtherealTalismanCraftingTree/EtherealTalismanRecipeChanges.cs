using CalamityMod.Items.Accessories;
using Terraria.Localization;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;


namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.EtherealTalismanCraftingTree
{
    public class EtherealTalismanRecipeChanges : ModSystem
    {
        private Mod calamity
        {
            get
            {
                ModLoader.TryGetMod("CalamityMod", out Mod cal);
                return cal;
            }
        }
        private Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thorium);
                return thorium;
            }
        }

        public override void AddRecipes()
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees || calamity == null || thorium == null)
                return;

            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.ManaFlower)}", new int[]
            {
                ItemID.ManaFlower,
                ItemID.ArcaneFlower,
                ItemID.MagnetFlower,
                ItemID.ManaCloak,
                thorium.Find<ModItem>("HungeringBlossom").Type
            });
            int AnyManaFlower = RecipeGroup.RegisterGroup("AnyManaFlowerAccessory", group);

            Recipe.Create(ModContent.ItemType<EtherealTalisman>())
               .AddIngredient<SigilofCalamitas>().
                AddRecipeGroup("AnyManaFlowerAccessory"). //Any mana flower accessory
                AddIngredient<AscendantSpiritEssence>(4).
                AddIngredient(thorium.Find<ModItem>("TerrariumCore"), 3).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override void PostAddRecipes()
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees || calamity == null || thorium == null)
                return;

            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult(ModContent.ItemType<SigilofCalamitas>()))
                {
                    recipe.AddIngredient(thorium.Find<ModItem>("MurkyCatalyst"), 1);

                    if (InfernalCrossmod.Clamity.Loaded)
                    {
                        recipe.AddIngredient(InfernalCrossmod.Clamity.Mod.Find<ModItem>("HuskOfCalamity"), 5);
                    }
                }

                if (recipe.HasResult(ModContent.ItemType<EtherealTalisman>()) && !recipe.HasIngredient(thorium.Find<ModItem>("TerrariumCore")))
                {
                    recipe.DisableRecipe();
                }
            }
        }
    }
}
