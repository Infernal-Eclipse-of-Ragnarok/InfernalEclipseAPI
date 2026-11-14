using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.OmniSpeakerCraftingTree
{
    public class OmniSpeakerRecipeChanges : ModSystem
    {
        private Mod Ragnarok
        {
            get
            {
                ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok);
                return ragnarok;
            }
        }

        private Mod CalBardHealer
        {
            get
            {
                ModLoader.TryGetMod("CalamityBardHealer", out Mod calbh);
                return calbh;
            }
        }
        private Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thor);
                return thor;
            }
        }
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (Ragnarok != null && CalBardHealer != null)
                {
                    if (recipe.HasResult(Ragnarok.Find<ModItem>("SigilOfACruelWorld")))
                    {
                        if (InfernalCrossmod.Clamity.Loaded)
                        {
                            recipe.AddIngredient(InfernalCrossmod.Clamity.Mod.Find<ModItem>("HuskOfCalamity"), 5);
                        }
                    }

                    if (recipe.HasResult(Ragnarok.Find<ModItem>("UniversalHeadset")))
                    {
                        recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                        recipe.AddIngredient(ItemID.LunarBar, 8);
                        recipe.AddIngredient(thorium.Find<ModItem>("ShootingStarFragment"), 4);
                        recipe.RemoveTile(ModContent.TileType<CosmicAnvil>());
                        recipe.AddTile(TileID.LunarCraftingStation);
                    }

                    if (recipe.HasResult(CalBardHealer.Find<ModItem>("OmniSpeaker")))
                    {
                        recipe.RemoveIngredient(ItemID.LunarBar);
                        recipe.AddIngredient(Ragnarok.Find<ModItem>("UniversalHeadset"));
                        if (!recipe.HasIngredient(thorium.Find<ModItem>("TerrariumCore"))) recipe.AddIngredient(thorium.Find<ModItem>("TerrariumCore"), 3);
                    }
                }
            }
        }
    }
}
