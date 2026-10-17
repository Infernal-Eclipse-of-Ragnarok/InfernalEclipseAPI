using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.Balance.Recipes
{
    public class DuplicateItemEctoMistConversions : ModSystem
    {
        public override void PostAddRecipes()
        {
            #region Catalyst/SimpleWhipAddon Conversions
            if (InfernalCrossmod.Catalyst.Loaded && ModLoader.TryGetMod("CalamitySimpleWhipAddon", out Mod calSimpleWhip))
            {
                Mod catalyst = InfernalCrossmod.Catalyst.Mod;

                int[] catalystWhips =
                {
                    GetItemID("CoralCrusher", catalyst),
                    GetItemID("PrismBreak", catalyst),
                    GetItemID("CongeledDuoWhip", catalyst),
                    ItemID.MaceWhip,
                    GetItemID("BlossomsBlessing", catalyst)
                };

                int[] simpleWhips =
                {
                    GetItemID("Droptide", calSimpleWhip),
                    GetItemID("BreezePiercer", calSimpleWhip),
                    GetItemID("Gelxyribose", calSimpleWhip),
                    GetItemID("Ectopia", calSimpleWhip),
                    GetItemID("EntwinedBranches", calSimpleWhip)
                };

                for (int i = 0; i < simpleWhips.Length; i++)
                {
                    int simpleID = simpleWhips[i];
                    int catalystID = catalystWhips[i];

                    if (simpleID == 0 || catalystID == 0)
                    {
                        continue;
                    }

                    Recipe originalRecipe = null;

                    // Disable ragnarok item recipe
                    foreach (var recipe in Main.recipe)
                    {
                        if (recipe.createItem.type == simpleID)
                        {
                            originalRecipe = recipe;
                            recipe.DisableRecipe();
                            break;
   
                        }
                    }
                    
                    int originalTile = originalRecipe?.requiredTile.Count > 0 ? originalRecipe.requiredTile[0] : TileID.Anvils; // fallback to something valid

                    if (simpleID == GetItemID("Ectopia", calSimpleWhip))
                    {
                        Recipe.Create(simpleID)
                            .AddIngredient(catalystID)
                            .AddIngredient(ItemID.SpectreBar, 12)
                            .AddCondition(Condition.InGraveyard)
                            .AddTile(originalTile)
                            .DisableDecraft()
                            .Register();
                        continue;
                    }

                    // Forward: Simple Whip -> Catalyst
                    Recipe forward = Recipe.Create(catalystID);
                    forward.AddIngredient(simpleID);
                    forward.AddCondition(Condition.InGraveyard);
                    if (originalTile != TileID.Anvils)
                        forward.AddTile(originalTile);
                    forward.DisableDecraft();
                    forward.Register();

                    // Reverse: Catalyst -> Simple Whip
                    Recipe reverse = Recipe.Create(simpleID);
                    reverse.AddIngredient(catalystID);
                    reverse.AddCondition(Condition.InGraveyard);
                    if (originalTile != TileID.Anvils)
                        reverse.AddTile(originalTile);
                    reverse.DisableDecraft();
                    reverse.Register();
                }
            }
            #endregion

            #region Ragnarok/CalamityBardHealer Armor Conversions
            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok) && ModLoader.TryGetMod("CalamityBardHealer", out Mod calBardHealer))
            {
                int[] ragnarokArmor =
                {
                    GetItemID("AerospecBard", ragnarok),
                    GetItemID("AerospecHealer", ragnarok),
                    //GetItemID("AuricTeslaFrilledHelmet", ragnarok),
                    //GetItemID("AuricTeslaHealerHead", ragnarok),
                    GetItemID("BloodflareHeadBard", ragnarok),
                    GetItemID("BloodflareHeadHealer", ragnarok),
                    GetItemID("DaedalusHeadBard", ragnarok),
                    GetItemID("DaedalusHeadHealer", ragnarok),
                    GetItemID("GodSlayerHeadBard", ragnarok),
                    GetItemID("SilvaHeadHealer", ragnarok),
                    GetItemID("StatigelHeadBard", ragnarok),
                    GetItemID("StatigelHeadHealer", ragnarok),
                    GetItemID("TarragonCowl", ragnarok),
                    GetItemID("TarragonShroud", ragnarok),
                    GetItemID("VictideHeadBard", ragnarok),
                    GetItemID("IntergelacticRamhelm", ragnarok),
                    GetItemID("IntergelacticRobohelm", ragnarok)
                };

                int[] calBardHealerArmor =
                {
                    GetItemID("AerospecHeadphones", calBardHealer),
                    GetItemID("AerospecBiretta", calBardHealer),
                    //GetItemID("AuricTeslaFeatheredHeadwear", calBardHealer),
                    //GetItemID("AuricTeslaValkyrieVisage", calBardHealer),
                    GetItemID("BloodflareSirenSkull", calBardHealer),
                    GetItemID("BloodflareRitualistMask", calBardHealer),
                    GetItemID("DaedalusHat", calBardHealer),
                    GetItemID("DaedalusCowl", calBardHealer),
                    GetItemID("GodSlayerDeathsingerCowl", calBardHealer),
                    GetItemID("SilvaGuardianHelmet", calBardHealer),
                    GetItemID("StatigelEarrings", calBardHealer),
                    GetItemID("StatigelFoxMask", calBardHealer),
                    GetItemID("TarragonParagonCrown", calBardHealer),
                    GetItemID("TarragonChapeau", calBardHealer),
                    GetItemID("VictideAmmoniteHat", calBardHealer),
                    GetItemID("IntergelacticProtectorHelm", calBardHealer),
                    GetItemID("IntergelacticCloche", calBardHealer)
                };

                for (int i = 0; i < ragnarokArmor.Length; i++)
                {
                    int ragID = ragnarokArmor[i];
                    int calID = calBardHealerArmor[i];

                    if (ragID == 0 || calID == 0)
                    {
                        continue;
                    }

                    Recipe originalRecipe = null;

                    // Disable ragnarok item recipe
                    foreach (var recipe in Main.recipe)
                    {
                        if (recipe.createItem.type == ragID)
                        {
                            originalRecipe = recipe;
                            recipe.DisableRecipe();
                            break;
                        }
                    }

                    int originalTile = originalRecipe?.requiredTile.Count > 0 ? originalRecipe.requiredTile[0] : TileID.Anvils; // fallback to something valid

                    // Forward: Ragnarok -> CalamityBH
                    Recipe forward = Recipe.Create(calID);
                    forward.AddIngredient(ragID);
                    forward.AddCondition(Condition.InGraveyard);
                    if (originalTile != TileID.Anvils)
                        forward.AddTile(originalTile);
                    forward.DisableDecraft();
                    forward.Register();

                    // Reverse: CalamityBH -> Ragnarok
                    Recipe reverse = Recipe.Create(ragID);
                    reverse.AddIngredient(calID);
                    reverse.AddCondition(Condition.InGraveyard);
                    if (originalTile != TileID.Anvils)
                        reverse.AddTile(originalTile);
                    reverse.DisableDecraft();
                    reverse.Register();
                }
            }
            #endregion
        }

        private static int GetItemID(string name, Mod mod)
        {
            if (mod.TryFind(name, out ModItem item))
                return item.Type;
            return 0;
        }
    }
}
