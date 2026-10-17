using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Materials;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.ShoeCraftingTree
{
    public class ShoeRecipeChanges : ModSystem
    {
        private Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thor);
                return thor;
            }
        }

        private Mod sots
        {
            get
            {
                ModLoader.TryGetMod("SOTS", out Mod sots);
                return sots;
            }
        }

        public Mod fargosSouls
        {
            get
            {
                ModLoader.TryGetMod("FargowiltasSouls", out Mod fargosSouls);
                return fargosSouls;
            }
        }

        public Mod calFargo
        {
            get
            {
                ModLoader.TryGetMod("FargowiltasCrossmod", out Mod fargoCrossmod);
                return fargoCrossmod;
            }
        }

        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (!InfernalConfig.Instance.MergeCraftingTrees)
                    return;

                if (calFargo != null)
                {
                    if (recipe.HasResult(fargosSouls.Find<ModItem>("AeolusBoots")))
                    {
                        if (!recipe.HasIngredient<AngelTreads>()) recipe.AddIngredient<AngelTreads>();

                        recipe.RemoveIngredient(ItemID.SoulofFright);
                        recipe.RemoveIngredient(ItemID.SoulofSight);
                        recipe.RemoveIngredient(ItemID.SoulofMight);

                        if (ModLoader.TryGetMod("Consolaria", out Mod console))
                        {
                            recipe.AddIngredient(console.Find<ModItem>("SoulofBlight"), 5);
                        }
                        else recipe.AddIngredient(ItemID.BeetleHusk, 5);
                    }
                }

                if (thorium != null)
                {
                    if (recipe.HasResult(thorium.Find<ModItem>("TerrariumParticleSprinters").Type))
                    {
                        recipe.RemoveIngredient(ItemID.TerrasparkBoots);

                        if (sots != null)
                        {
                            recipe.RemoveIngredient(sots.Find<ModItem>("SubspaceBoosters").Type);
                        }

                        if (calFargo != null || ModLoader.TryGetMod("ssm", out _))
                        {
                            recipe.RemoveIngredient(ModContent.ItemType<AngelTreads>());
                            if (!recipe.HasIngredient(fargosSouls.Find<ModItem>("AeolusBoots"))) recipe.AddIngredient(fargosSouls.Find<ModItem>("AeolusBoots"));
                        }
                        else
                        {
                            if (!recipe.HasIngredient(ModContent.ItemType<AngelTreads>())) recipe.AddIngredient<AngelTreads>();
                        }
                    }

                    if (sots == null)
                    {
                        if (recipe.HasResult<MoonWalkers>())
                        {
                            recipe.RemoveIngredient(ModContent.ItemType<AngelTreads>());
                            if (calFargo != null) recipe.RemoveIngredient(fargosSouls.Find<ModItem>("AeolusBoots").Type);

                            if (!recipe.HasIngredient(thorium.Find<ModItem>("TerrariumParticleSprinters").Type))
                            {
                                recipe.AddIngredient(thorium.Find<ModItem>("TerrariumParticleSprinters").Type);
                            }
                        }
                    }
                }

                if (sots != null)
                {
                    if (fargosSouls != null)
                    {
                        if (recipe.HasResult(fargosSouls.Find<ModItem>("AeolusBoots").Type))
                        {
                            recipe.RemoveIngredient(sots.Find<ModItem>("SubspaceBoosters").Type);
                        }
                    }

                    if (recipe.HasResult(sots.Find<ModItem>("FlashsparkBoots")))
                    {
                        recipe.RemoveIngredient(ItemID.TerrasparkBoots);
                        recipe.RemoveIngredient(ModContent.ItemType<AngelTreads>());
                        recipe.AddIngredient(ItemID.HellfireTreads);
                        recipe.AddIngredient<AshesofCalamity>(4);

                        if (InfernalCrossmod.Clamity.Loaded)
                        {
                            recipe.AddIngredient(InfernalCrossmod.Clamity.Mod.Find<ModItem>("HuskOfCalamity"), 3);
                        }
                    }

                    if (recipe.HasResult(sots.Find<ModItem>("SubspaceBoosters")))
                    {
                        if (thorium != null) recipe.AddIngredient(thorium.Find<ModItem>("TerrariumParticleSprinters"));
                        else if (calFargo != null) recipe.AddIngredient(fargosSouls.Find<ModItem>("AeolusBoots"));
                    }

                    if (recipe.HasResult<MoonWalkers>())
                    {
                        recipe.RemoveIngredient(ModContent.ItemType<AngelTreads>());
                        if (calFargo != null) recipe.RemoveIngredient(fargosSouls.Find<ModItem>("AeolusBoots").Type);
                        if (thorium != null) recipe.RemoveIngredient(thorium.Find<ModItem>("TerrariumParticleSprinters").Type);

                        recipe.AddIngredient(sots.Find<ModItem>("SubspaceBoosters"));
                    }
                }
            }
        }
    }
}
