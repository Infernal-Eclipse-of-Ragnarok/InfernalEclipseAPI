using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using InfernalEclipseAPI.Common.Balance.Recipes;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems;

namespace InfernalEclipseAPI.Common.Balance.Calamity
{
    public class BloodOrbPotionsTweak : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];
                if (recipe.HasIngredient(126) && recipe.HasIngredient(ModContent.ItemType<BloodOrb>()))
                {
                    if (InfernalConfig.Instance.VanillaBalanceChanges)
                    {
                        recipe.DisableDecraft();
                    }

                    if (InfernalConfig.Instance.DisableBloodOrbPotions || InfernalConfig.Instance.BloodOrbPotionDuplication)
                    {
                        recipe.DisableRecipe();
                    }
                }
                if (recipe.HasIngredient(353) && recipe.HasIngredient(ModContent.ItemType<BloodOrb>()))
                {
                    if (InfernalConfig.Instance.VanillaBalanceChanges)
                    {
                        recipe.DisableDecraft();
                    }

                    if (InfernalConfig.Instance.DisableBloodOrbPotions || InfernalConfig.Instance.BloodOrbPotionDuplication)
                    {
                        recipe.DisableRecipe();
                    }
                }

                if (recipe.HasResult(ModContent.ItemType<BloodOrb>()))
                {
                    recipe.DisableDecraft();

                    if (recipe.Mod.Name == "CalamityBardHealer")
                    {
                        recipe.DisableRecipe();
                    }
                }

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    if (recipe.HasResult(InfernalCrossmod.Thorium.Mod.Find<ModItem>("UnholyShards")))
                    {
                        recipe.DisableDecraft();

                        if (recipe.Mod.Name == "CalamityBardHealer")
                        {
                            recipe.DisableRecipe();
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            if (!InfernalConfig.Instance.BloodOrbPotionDuplication || InfernalConfig.Instance.DisableBloodOrbPotions)
                return;

            short[] numArray1 = new short[47]
            {
                  (short) 2997,
                  (short) 2351,
                  (short) 290,
                  (short) 295,
                  (short) 305,
                  (short) 298,
                  (short) 297,
                  (short) 299,
                  (short) 296,
                  (short) 304,
                  (short) 2329,
                  (short) 300,
                  (short) 2324,
                  (short) 2349,
                  (short) 2347,
                  (short) 301,
                  (short) 292,
                  (short) 2346,
                  (short) 289,
                  (short) 2345,
                  (short) 2323,
                  (short) 2326,
                  (short) 303,
                  (short) 2344,
                  (short) 294,
                  (short) 293,
                  (short) 2328,
                  (short) 2348,
                  (short) 2359,
                  (short) 288,
                  (short) 291,
                  (short) 302,
                  (short) 2327,
                  (short) 2325,
                  (short) 2322,
                  (short) 2354,
                  (short) 2356,
                  (short) 2355,
                  (short) 2756,
                  (short) 2352,
                  (short) 2353,
                  (short) 2350,
                  (short) 4870,
                  (short) 4477,
                  (short) 4478,
                  (short) 4479,
                  (short) 5211
            };

            int[] numArray2 = new int[11]
            {
              ModContent.ItemType<PotionofOmniscience>(),
              ModContent.ItemType<BoundingPotion>(),
              ModContent.ItemType<CalciumPotion>(),
              ModContent.ItemType<CeaselessHungerPotion>(),
              ModContent.ItemType<GravityNormalizerPotion>(),
              ModContent.ItemType<PhotosynthesisPotion>(),
              ModContent.ItemType<ShadowPotion>(),
              ModContent.ItemType<SoaringPotion>(),
              ModContent.ItemType<SulphurskinPotion>(),
              ModContent.ItemType<ZenPotion>(),
              ModContent.ItemType<ZergPotion>()
            };

            foreach (short num in numArray1)
            {
                Recipe recipe = Recipe.Create(num, 2);
                recipe.AddIngredient(num, 1);
                recipe.AddIngredient<BloodOrb>(10);
                recipe.AddTile(TileID.AlchemyTable);
                recipe.AddCondition(Condition.DownedSkeletron);
                recipe.Register();
            }
            foreach (int num in numArray2)
            {
                Recipe recipe = Recipe.Create(num, 2);
                recipe.AddIngredient(num, 1);
                if (num == ModContent.ItemType<PhotosynthesisPotion>())
                {
                    recipe.AddIngredient<BloodOrb>(40);
                    recipe.AddIngredient<EssenceofSunlight>();
                }
                else if (num == ModContent.ItemType<GravityNormalizerPotion>())
                {
                    recipe.AddIngredient<BloodOrb>(10);
                    recipe.AddIngredient<AureusCell>();
                }
                else
                {
                    recipe.AddIngredient<BloodOrb>(10);
                }

                recipe.AddTile(TileID.AlchemyTable);
                recipe.AddCondition(Condition.DownedSkeletron);
                recipe.Register();
            }

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                int[] numArray3 =
                {
                    thorium.Find<ModItem>("CreativityPotion").Type,
                    thorium.Find<ModItem>("EarwormPotion").Type,
                    thorium.Find<ModItem>("InspirationReachPotion").Type,
                    thorium.Find<ModItem>("ArcanePotion").Type,
                    thorium.Find<ModItem>("ArtilleryPotion").Type,
                    thorium.Find<ModItem>("BloodPotion").Type,
                    thorium.Find<ModItem>("BouncingFlamePotion").Type,
                    thorium.Find<ModItem>("ConflagrationPotion").Type,
                    thorium.Find<ModItem>("HolyPotion").Type,
                    thorium.Find<ModItem>("WarmongerPotion").Type,
                    thorium.Find<ModItem>("AquaPotion").Type,
                    thorium.Find<ModItem>("FrenzyPotion").Type,
                    thorium.Find<ModItem>("GlowingPotion").Type,
                    thorium.Find<ModItem>("KineticPotion").Type,
                    thorium.Find<ModItem>("AssassinPotion").Type,
                    thorium.Find<ModItem>("HydrationPotion").Type
                };

                foreach (int num in numArray3) 
                {
                    Recipe recipe = Recipe.Create(num, 2);
                    recipe.AddIngredient(num, 1);
                    recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
                    if (num == thorium.Find<ModItem>("KineticPotion").Type)
                        recipe.AddIngredient(ItemID.BlackPearl);
                    recipe.AddTile(TileID.AlchemyTable);
                    recipe.AddCondition(InfernalRecipeSystem.SkeletronOrHardmode);
                    recipe.Register();
                }
            }
        }
    }
}
