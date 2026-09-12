using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.LabFinders;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Content.Items.Accessories.ChromaticMassInABottle;
using InfernalEclipseAPI.Content.Items.Consumables;
using InfernalEclipseAPI.Content.Items.Materials;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Content.Items.Weapons.BossRush.NovaBomb;
using InfernalEclipseAPI.Content.Items.Weapons.BossRush.Swordofthe14thGlitch;
using InfernalEclipseAPI.Content.Items.Weapons.Legendary.Lycanroc;
using InfernalEclipseAPI.Content.Items.Weapons.Legendary.StellarSabre;
using InfernalEclipseAPI.Content.Items.Weapons.Magic.ChaosBlaster;
using InfernalEclipseAPI.Content.Items.Weapons.Nameless.NebulaGigabeam;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems;
using InfernumMode.Content.Items.Weapons.Magic;
using SOTS;
using System.Linq;
using Terraria.Localization;
using ThoriumMod.Items.ArcaneArmor;
using ThoriumMod.Tiles;
using Verdant.Items.Verdant.Armour;
using Verdant.Items.Verdant.Armour.ApotheoticArmor;

namespace InfernalEclipseAPI.Common.Balance.Recipes
{
    internal sealed class InfernalRecipeSystem : ModSystem
    {
        public static RecipeGroup EvilSkinRecipeGroup;
        public static RecipeGroup EvilBarRecipeGroup;
        public static RecipeGroup CobaltPalladiumRecipeGroup;

        public override void Unload()
        {
            EvilSkinRecipeGroup = null;
            EvilBarRecipeGroup = null;
        }
        public override void AddRecipeGroups()
        {
            EvilSkinRecipeGroup = new RecipeGroup(() => $"{Lang.GetItemNameValue(ItemID.ShadowScale)} or {Lang.GetItemNameValue(ItemID.TissueSample)}", ItemID.ShadowScale, ItemID.TissueSample);
            RecipeGroup.RegisterGroup("LimitedResourcesRecipes:EvilSkin", EvilSkinRecipeGroup);

            EvilBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CrimtaneBar)}", ItemID.DemoniteBar, ItemID.CrimtaneBar);
            RecipeGroup.RegisterGroup("LimitedResourcesRecipes:EvilBar", EvilBarRecipeGroup);

            CobaltPalladiumRecipeGroup = new RecipeGroup(() => $"{Lang.GetItemNameValue(ItemID.CobaltBar)} or {Lang.GetItemNameValue(ItemID.PalladiumBar)}", ItemID.CobaltBar, ItemID.PalladiumBar);
            RecipeGroup.RegisterGroup("LimitedResourcesRecipes:CobaltPalladium", CobaltPalladiumRecipeGroup);
        }

        public override void AddRecipes()
        {
            #region Vanilla
            /*
            Recipe.Create(ItemID.Terragrim)
                .AddIngredient(ItemID.EnchantedSword, 1)
                .AddIngredient(ItemID.JungleSpores, 5)
                .AddRecipeGroup(EvilBarRecipeGroup, 5)
                .AddIngredient(ItemID.Obsidian, 3)
                .AddIngredient(ItemID.FossilOre, 3)
                .AddTile(TileID.Anvils)
                .Register();
            */

            Recipe.Create(ItemID.TinkerersWorkshop)
                .AddIngredient<AbandonedWorkshop>()
                .AddIngredient<TinkerersRepairBlueprints>()
                .AddTile(TileID.HeavyWorkBench)
                .Register();

            Recipe.Create(ItemID.BlandWhip)
                .AddIngredient(ItemID.Leather, 8)
                .AddTile(TileID.Loom)
                .Register();
            #endregion

            #region Wrath of the Gods
            if (ModLoader.TryGetMod("NoxusBoss", out Mod noxus))
            {
                Recipe.Create(ModContent.ItemType<Rock>(), 2)
                    .AddIngredient<Rock>()
                    .AddIngredient(ItemID.StoneBlock, 30)
                    .AddIngredient<AlloyofEden>()
                    .AddIngredient<YharonSoulFragment>(5)
                    .AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition)
                    .AddTile(noxus.Find<ModTile>("StarlitForgeTile"))
                    .DisableDecraft()
                    .Register();
            }
            #endregion

            #region AlchNPC
            if (ModLoader.TryGetMod("AlchemistNPCLite", out Mod AlchNPC))
            {
                int[] alchCombos =
                {
                    GetItem(AlchNPC, "BattleCombination").Type,
                    GetItem(AlchNPC, "BewitchingPotion").Type,
                    GetItem(AlchNPC, "BuilderCombination").Type,
                    GetItem(AlchNPC, "CalamityCombination").Type,
                    GetItem(AlchNPC, "ExplorerCombination").Type,
                    GetItem(AlchNPC, "FishingCombination").Type,
                    GetItem(AlchNPC, "MageCombination").Type,
                    GetItem(AlchNPC, "RangerCombination").Type,
                    GetItem(AlchNPC, "SummonerCombination").Type,
                    GetItem(AlchNPC, "VanTankCombination").Type
                };

                foreach (int potion in alchCombos)
                {
                    Recipe newRecipe = Recipe.Create(potion, 2);

                    if (InfernalConfig.Instance.BloodOrbPotionDuplication)
                    {
                        newRecipe.AddIngredient(potion);
                    }
                    else
                    {
                        newRecipe.AddIngredient(ItemID.BottledWater);
                    }

                    if (potion == GetItem(AlchNPC, "BattleCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(60);
                    if (potion == GetItem(AlchNPC, "BewitchingPotion").Type)
                        newRecipe.AddIngredient<BloodOrb>(10);
                    if (potion == GetItem(AlchNPC, "BuilderCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(30);
                    if (potion == GetItem(AlchNPC, "CalamityCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(40);
                    if (potion == GetItem(AlchNPC, "ExplorerCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(90);
                    if (potion == GetItem(AlchNPC, "MageCombination").Type)
                    {
                        newRecipe.AddIngredient<BloodOrb>(40);
                        newRecipe.AddIngredient(ItemID.FallenStar, 1);
                    }
                    if (potion == GetItem(AlchNPC, "FishingCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(80);
                    if (potion == GetItem(AlchNPC, "RangerCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(40);
                    if (potion == GetItem(AlchNPC, "SummonerCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(30);
                    if (potion == GetItem(AlchNPC, "FishingCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(80);
                    if (potion == GetItem(AlchNPC, "VanTankCombination").Type)
                        newRecipe.AddIngredient<BloodOrb>(60);

                    newRecipe.AddTile(TileID.AlchemyTable);
                    newRecipe.AddCondition(Condition.DownedSkeletron);
                    newRecipe.Register();
                }

                if (ModLoader.TryGetMod("ThoriumMod", out _))
                {
                    int thorComboId = GetItem(AlchNPC, "ThoriumCombination").Type;
                    Recipe thorComboBloodOrb = Recipe.Create(thorComboId, 2);
                    if (InfernalConfig.Instance.BloodOrbPotionDuplication)
                    {
                        thorComboBloodOrb.AddIngredient(thorComboId);
                    }
                    else
                    {
                        thorComboBloodOrb.AddIngredient(ItemID.BottledWater);
                    }
                    thorComboBloodOrb.AddIngredient<BloodOrb>(90);

                    thorComboBloodOrb.AddTile(TileID.AlchemyTable);
                    thorComboBloodOrb.AddCondition(Condition.DownedSkeletron);
                    thorComboBloodOrb.Register();
                }
            }
            #endregion

            #region Thorium
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                if (InfernalConfig.Instance.ThoriumBalanceChangess)
                {
                    Recipe omegaCore = Recipe.Create(thorium.Find<ModItem>("TheOmegaCore").Type);
                    if (InfernalCrossmod.NoxusBoss.Loaded)
                        omegaCore.AddIngredient(InfernalCrossmod.NoxusBoss.Mod.Find<ModItem>("MetallicChunk"));
                    omegaCore.AddIngredient(ModContent.ItemType<Rock>());
                    omegaCore.AddIngredient<DreamEssence>();
                    omegaCore.AddTile<DraedonsForge>();
                    omegaCore.Register();
                }

                if (ModLoader.TryGetMod("ThoriumRework", out Mod thorRework) && !InfernalConfig.Instance.DisableBloodOrbPotions)
                {
                    if (!InfernalConfig.Instance.ThoriumBalanceChangess)
                    {
                        Recipe.Create(thorRework.Find<ModItem>("DeathsingerPotion").Type)
                            .AddIngredient(ItemID.BottledWater)
                            .AddIngredient<BloodOrb>(10)
                            .AddTile(TileID.AlchemyTable)
                            .AddCondition(Condition.DownedSkeletron)
                            .Register();
                    }


                    if (thorRework.TryFind("InspirationRegenerationPotion", out ModItem inspRegenPotion))
                    {
                        Recipe.Create(thorRework.Find<ModItem>("InspirationRegenerationPotion").Type)
                            .AddIngredient(ItemID.BottledWater)
                            .AddIngredient<BloodOrb>(10)
                            .AddIngredient(thorium.Find<ModItem>("ManaBerry").Type)
                            .AddTile(TileID.AlchemyTable)
                            .AddCondition(Condition.DownedSkeletron)
                            .Register();
                    }
                }

                if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok))
                {
                    Recipe.Create(thorium.Find<ModItem>("VoltHatchet").Type)
                        .AddIngredient(thorium.Find<ModItem>("AbyssalChitin").Type, 8)
                        .AddIngredient(ragnarok.Find<ModItem>("EmpoweredGranite").Type, 3)
                        .AddIngredient(thorium.Find<ModItem>("AquaiteBar").Type, 10)
                        .AddTile(TileID.Anvils)
                        .Register();

                    if (thorRework != null)
                    {
                        if (thorRework.TryFind("ZephyrWingsCosmetic", out ModItem wings))
                        {
                            Recipe.Create(wings.Type)
                                .AddIngredient(ItemID.CreativeWings)
                                .AddIngredient(ragnarok.Find<ModItem>("StormFeather"), 10)
                                .AddTile(TileID.HeavyWorkBench)
                                .Register();
                        }
                    }
                }
            }
            #endregion

            #region Secrets of the Shadows
            if (InfernalCrossmod.SOTS.Loaded)
            {
                Mod sots = InfernalCrossmod.SOTS.Mod;

                /*
                Recipe.Create(ModContent.ItemType<LiliesOfFinality>())
                    .AddIngredient(ItemID.ClayPot)
                    .AddIngredient<PlantyMush>()
                    .AddIngredient<AscendantSpiritEssence>(3)
                    .AddIngredient<YharonSoulFragment>(4)
                    .AddIngredient(ItemID.DarkShard)
                    .AddIngredient(ItemID.LightShard)
                    .AddTile<CosmicAnvil>()
                    .Register();
                */

                Recipe.Create(ModContent.ItemType<ThePointer>())
                    .AddIngredient(sots.Find<ModItem>("Calculator"))
                    .AddRecipeGroup(CobaltPalladiumRecipeGroup, 5)
                    .AddIngredient(ItemID.ManaCrystal)
                    .AddTile(TileID.Anvils)
                    .Register();

                int[] sotsPotions =
                {
                    GetItem(sots, "AssassinationPotion").Type,
                    GetItem(sots, "BluefirePotion").Type,
                    GetItem(sots, "BrittlePotion").Type,
                    GetItem(sots, "DoubleVisionPotion").Type,
                    GetItem(sots, "HarmonyPotion").Type,
                    GetItem(sots, "NightmarePotion").Type,
                    GetItem(sots, "RipplePotion").Type,
                    GetItem(sots, "RoughskinPotion").Type,
                    GetItem(sots, "SoulAccessPotion").Type,
                    GetItem(sots, "VibePotion").Type
                };

                foreach (int potion in sotsPotions)
                {
                    Recipe newRecipe = Recipe.Create(potion, 2);

                    if (InfernalConfig.Instance.BloodOrbPotionDuplication)
                    {
                        newRecipe.AddIngredient(potion);
                    }
                    else
                    {
                        newRecipe.AddIngredient(ItemID.BottledWater);
                    }
                    newRecipe.AddIngredient<BloodOrb>(10);

                    if (potion == GetItem(sots, "HarmonyPotion").Type)
                    {
                        newRecipe.AddIngredient(ItemID.SoulofLight);
                    }
                    else if (potion == GetItem(sots, "NightmarePotion").Type)
                    {
                        newRecipe.AddIngredient(ItemID.SoulofNight);
                    }
                    newRecipe.AddTile(TileID.AlchemyTable);
                    newRecipe.AddCondition(Condition.DownedSkeletron);
                    newRecipe.Register();
                }

                Recipe.Create(GetItem(sots, "RoyalGoldBrick").Type, 25)
                    .AddIngredient(ItemID.AncientGoldBrick, 25)
                    .AddIngredient(GetItem(sots, "SoulResidue"))
                    .AddTile(TileID.HeavyWorkBench)
                    .Register();

                if (InfernalConfig.Instance.MergeCraftingTrees)
                {
                    Recipe.Create(ModContent.ItemType<Cosmolight>())
                        .AddIngredient(GetItem(sots, "LunarClock"))
                        .AddIngredient<Bakidon>()
                        .AddIngredient<AstralBar>(3)
                        .AddIngredient(ItemID.FragmentSolar, 15)
                        .AddTile(TileID.LunarCraftingStation)
                        .Register();
                }

                SOTSWormholeRecipes.Initialize();
            }
            #endregion

            #region Calamity Simple Whip Addon
            /*
            if (ModLoader.TryGetMod("CalamitySimpleWhipAddon", out Mod simpleWhipAddon))
            {
                Recipe.Create(simpleWhipAddon.Find<ModItem>("StrikerEmblem").Type)
                    .AddIngredient(ItemID.BlandWhip)
                    .AddIngredient(ItemID.SummonerEmblem)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            */
            #endregion
        }

        public override void PostAddRecipes()
        {
            base.PostAddRecipes();

            ModLoader.TryGetMod("ThoriumMod", out Mod thorium);

            foreach (var recipe in Main.recipe)
            {
                #region Verdant
                //do we have verdant
                //if (ModLoader.HasMod("VerdantMod"))
                //yes? do all of this stuff
                //{
                    //Check for any of the blossom armor peices
                    if (recipe.HasResult(ModContent.ItemType<VerdantChestplate>()) || recipe.HasResult(ModContent.ItemType<VerdantHelm>()) || recipe.HasResult(ModContent.ItemType<VerdantLeggings>()))
                    {
                        //Remove the anvil
                        recipe.RemoveTile(TileID.Anvils);
                        //Add the arcane armor fabricator
                        recipe.AddTile(thorium.Find<ModTile>("ArcaneArmorFabricator"));
                    }
                    //check for any peice of apoth armor
                    if (recipe.HasResult(ModContent.ItemType<ApotheoticChestplate>()) || recipe.HasResult(ModContent.ItemType<ApotheoticLeggings>()) || recipe.HasResult(ModContent.ItemType<ApotheoticTreeHelmet>()) || recipe.HasResult(ModContent.ItemType<ApotheoticBeeHelmet>()))
                {
                    //Add 4 stardust fragments to apoth armor
                    recipe.AddIngredient(ItemID.FragmentStardust, 4);
                }
                    //
                //}
                //no? do nothing
                #endregion
                #region tPackBuilder Subsititons
                if (!ModLoader.HasMod("PackBuilder"))
                {
                    if (recipe.HasResult(ModContent.ItemType<Kevin>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<Rock>(), 1);
                    }

                    if (InfernalConfig.Instance.VanillaBalanceChanges)
                    {
                        if (recipe.HasResult(ItemID.Zenith))
                        {
                            recipe.RemoveIngredient(ItemID.EnchantedSword);
                            recipe.AddIngredient(ItemID.Terragrim, 1);
                        }
                    }
                }
                #endregion

                #region Vanilla
                if (recipe.HasResult(ItemID.Sashimi))
                    recipe.DisableDecraft();

                if (recipe.HasResult(ItemID.TempleKey))
                    recipe.DisableRecipe();

                if (InfernalConfig.Instance.BossKillCheckOnOres)
                {
                    int[] lockUntilWorldEvil =
                    {
                        ItemID.DemonConch,
                        ItemID.HellstoneBar,
                        ItemID.MeteoriteBar,
                        ItemID.MeteorShot,
                        ItemID.BluePhaseblade,
                        ItemID.GreenPhaseblade,
                        ItemID.OrangePhaseblade,
                        ItemID.PurplePhaseblade,
                        ItemID.RedPhaseblade,
                        ItemID.WhitePhaseblade,
                        ItemID.YellowPhaseblade,
                        ItemID.FlameWakerBoots
                    };

                    int[] lockUntilSkeletron =
                    {
                        ItemID.ActiveStoneBlock,
                        ItemID.InactiveStoneBlock,
                        ItemID.AlchemyTable,
                        ItemID.BewitchingTable,
                        ItemID.Explosives,
                        ItemID.GenderChangePotion
                    };

                    foreach (int item in lockUntilWorldEvil)
                    {
                        if (recipe.HasResult(item))
                            recipe.DecraftConditions.Add(Condition.DownedEowOrBoc);
                    }

                    foreach (int item in lockUntilSkeletron)
                    {
                        if (recipe.HasResult(item))
                            recipe.DecraftConditions.Add(SkeletronOrHardmode);
                    }

                }
                
                if (InfernalConfig.Instance.VanillaBalanceChanges)
                {
                    if (recipe.HasResult(ItemID.DemonScythe))
                    {
                        recipe.AddCondition(Condition.DownedSkeletron);
                    }
                }

                #endregion

                #region Calamity
                //If any mods allow the terminus to be crafted, disable it.
                if (recipe.HasResult(ModContent.ItemType<Terminus>()))
                {
                    //recipe.AddIngredient(ModContent.ItemType<EvokingSearune>(), 1);
                    recipe.DisableRecipe();
                }

                if (recipe.HasResult(ModContent.ItemType<OnyxExcavatorKey>()))
                {
                    recipe.DisableRecipe();
                    recipe.DisableDecraft();
                }

                if (recipe.HasResult(ModContent.ItemType<Roxcalibur>()))
                {
                    recipe.DecraftConditions.Add(Condition.Hardmode);
                }

                if (recipe.HasResult(ModContent.ItemType<RedSeekingMechanism>()))
                {
                    recipe.DecraftConditions.Add(Condition.DownedEowOrBoc);
                }

                if (recipe.HasResult(ModContent.ItemType<Apollyon>()))
                {
                    if (InfernalCrossmod.Clamity.Loaded)
                    {
                        recipe.AddIngredient(InfernalCrossmod.Clamity.Mod.Find<ModItem>("HuskOfCalamity"), 5);
                    }
                }

                if (thorium != null)
                {
                    if (recipe.HasResult<TheAmalgam>())
                    {
                        recipe.AddIngredient(thorium.Find<ModItem>("SoulofPlight"), 5);
                    }

                    if (recipe.HasResult<OpalStriker>())
                    {
                        recipe.AddIngredient(thorium.Find<ModItem>("Opal"), 3);
                    }

                    if (recipe.HasResult<Portabulb>())
                    {
                        recipe.AddIngredient(thorium.Find<ModItem>("Petal"), 5);
                    }

                    if (recipe.HasResult<SunSpiritStaff>() && !InfernalCrossmod.Hummus.Loaded)
                    {
                        recipe.AddIngredient(thorium.Find<ModItem>("SandstoneIngot"), 8);
                    }

                    if (recipe.HasResult<Earth>())
                    {
                        recipe.AddIngredient(thorium.Find<ModItem>("InfernoEssence"));
                        recipe.AddIngredient(thorium.Find<ModItem>("DeathEssence"));
                        recipe.AddIngredient(thorium.Find<ModItem>("OceanEssence"));
                    }
                }

                if (InfernalCrossmod.SOTS.Loaded)
                {
                    if (recipe.HasResult<ThePointer>() && recipe.HasIngredient(ItemID.Glass))
                    {
                        recipe.DisableRecipe();
                    }

                    if (recipe.HasResult<OntologicalDespoiler>())
                    {
                        recipe.AddIngredient(InfernalCrossmod.SOTS.Mod.Find<ModItem>("DissolvingNihility"));
                    }
                }

                /*
                if (InfernalConfig.Instance.CalamityBalanceChanges)
                {
                    if (recipe.HasResult<Moonshine>())
                    {
                        recipe.AddIngredient<LivingShard>();
                    }

                    if (recipe.HasResult<GrapeBeer>())
                    {
                        recipe.AddIngredient<StarblightSoot>(5);
                    }
                }
                */

                #region Calamity Ranger Expansion
                if (ModLoader.TryGetMod("CalamityAmmo", out Mod calAmmo))
                {
                    if (recipe.HasResult(ItemID.PinkGel) && recipe.Mod == calAmmo)
                        recipe.DisableRecipe();

                    if (recipe.HasResult(calAmmo.Find<ModItem>("HardTack")))
                    {
                        recipe.DisableDecraft();
                    }

                    if (recipe.HasResult(calAmmo.Find<ModItem>("HydrothermicArrow")) || recipe.HasResult(calAmmo.Find<ModItem>("HydrothermicBullet")))
                    {
                        recipe.DecraftConditions.Add(Condition.DownedGolem);

                        if (InfernalConfig.Instance.CalamityBalanceChanges)
                            recipe.DisableRecipe();
                    }

                    if (recipe.HasResult(calAmmo.Find<ModItem>("DivineArrow")) || recipe.HasResult(calAmmo.Find<ModItem>("DivineBullet")) || recipe.HasResult(calAmmo.Find<ModItem>("WeakAstralBullet")))
                    {
                        if (InfernalConfig.Instance.CalamityBalanceChanges)
                            recipe.DisableRecipe();
                    }

                    if (InfernalConfig.Instance.MergeCraftingTrees)
                    {
                        if (recipe.HasResult(calAmmo.Find<ModItem>("AutoCalculationCoil")))
                        {
                            // Reduce Suspicious Scrap to 1
                            for (int j = 0; j < recipe.requiredItem.Count; j++)
                            {
                                Item req = recipe.requiredItem[j];

                                if (req.type == ModContent.ItemType<SuspiciousScrap>())
                                {
                                    req.stack = 1;
                                }
                            }
                            recipe.AddIngredient<ThePointer>();
                            recipe.AddIngredient<MiracleMatter>();
                            recipe.RemoveIngredient(ModContent.ItemType<PlasmaDriveCore>());
                            recipe.requiredTile.Clear();
                            recipe.AddTile(ModContent.TileType<DraedonsForge>());
                        }
                    }

                    if (recipe.HasResult(calAmmo.Find<ModItem>("MushroomUnitedNations")))
                    {
                        recipe.RemoveIngredient(ItemID.ShroomiteBar);
                        recipe.AddIngredient(ItemID.LunarBar, 10);
                        recipe.AddIngredient(ItemID.FragmentVortex, 6);
                    }

                    if (recipe.HasResult(calAmmo.Find<ModItem>("NapalmBullet")))
                    {
                        for (int i = 0; i < recipe.requiredItem.Count; i++)
                        {
                            if (recipe.requiredItem[i].type == ItemID.EmptyBullet)
                            {
                                recipe.requiredItem[i].stack = 100;
                            }
                        }

                        recipe.AddIngredient<UnholyCore>();

                        int napalmType = calAmmo.Find<ModItem>("NapalmBullet").Type;
                        recipe.ReplaceResult(napalmType, 100);
                    }
                }
                #endregion

                #region Calamity Simple Whip Addon
                if (ModLoader.TryGetMod("CalamitySimpleWhipAddon", out Mod simpleWhipAddon))
                {
                    if (recipe.HasResult(simpleWhipAddon.Find<ModItem>("WoodenWhip")))
                    {
                        recipe.AddIngredient(ItemID.Hay, 5);
                    }

                    if (thorium != null)
                    {
                        if (recipe.HasResult(simpleWhipAddon.Find<ModItem>("MandibleLash")))
                        {
                            recipe.AddIngredient(thorium.Find<ModItem>("SandstoneIngot"), 8);
                        }
                    }

                    if (recipe.HasResult(simpleWhipAddon.Find<ModItem>("AurelianSanctum")))
                    {
                        recipe.RemoveIngredient(ModContent.ItemType<ShadowspecBar>());
                        if (InfernalCrossmod.NoxusBoss.Loaded)
                        {
                            recipe.AddIngredient<AlloyofEden>(3);
                            recipe.AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition);
                        }
                        recipe.AddIngredient<Rock>(1);
                    }

                    if (recipe.HasResult(simpleWhipAddon.Find<ModItem>("Rubellus")))
                    {
                        recipe.AddIngredient(InfernalCrossmod.Calamity.Mod.Find<ModItem>("RuinousSoul"), 2);
                    }

                    if (InfernalConfig.Instance.CalamityBalanceChanges) 
                    {
                        ModItem[] bleachedAcessories =
                        {
                            GetItem(simpleWhipAddon, "BleachedNucleogenesis"),
                            GetItem(simpleWhipAddon, "BleachedStatisCurse"),
                            GetItem(simpleWhipAddon, "BleachedStarTaintedGenerator"),
                            GetItem(simpleWhipAddon, "BleachedStarbusterCore"),
                            GetItem(simpleWhipAddon, "BleachedNuclearFuelRod"),
                            GetItem(simpleWhipAddon, "BleachedTheFirstShadowflame"),
                            GetItem(simpleWhipAddon, "BleachedJellyChargedBattery"),
                            GetItem(simpleWhipAddon, "BleachedVoltaicJelly"),

                            //GetItem(simpleWhipAddon, "BuddyEmblem")
                        };

                        foreach (ModItem item in bleachedAcessories)
                        {
                            if (recipe.HasResult(item))
                                recipe.DisableRecipe();
                        }
                    }

                    /*
                    if (recipe.HasResult(simpleWhipAddon.Find<ModItem>("StrikerEmblem")) && recipe.HasIngredient(ItemID.Daybloom))
                        recipe.DisableRecipe();
                    */
                }
                #endregion

                #region Wrath of the Gods
                if (ModLoader.HasMod("NoxusBoss"))
                {
                    if (recipe.HasResult<GravityNormalizerPotion>())
                    {
                        recipe.DisableDecraft();
                    }
                }
                #endregion

                #region Hunt of the Old God
                if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
                {
                    if (recipe.HasResult(calHunt.Find<ModItem>("PluripotentSpawnEgg")))
                    {
                        recipe.RemoveIngredient(calHunt.Find<ModItem>("ChromaticMass").Type);
                        recipe.RemoveIngredient(ItemID.Gel);
                        if (InfernalCrossmod.Catalyst.Loaded)
                        {
                            recipe.AddIngredient(InfernalCrossmod.Catalyst.Mod.Find<ModItem>("AstralCommunicator"));
                        }
                        recipe.AddIngredient(ItemID.QueenSlimeCrystal);
                        recipe.AddIngredient<OverloadedSludge>();
                        if (InfernalCrossmod.SOTS.Loaded)
                        {
                            recipe.AddIngredient(InfernalCrossmod.SOTS.Mod.Find<ModItem>("JarOfPeanuts").Type);
                        }
                        recipe.AddIngredient(ItemID.SlimeCrown);
                        recipe.AddIngredient<PurifiedGel>(50);
                        if (InfernalCrossmod.SOTS.Loaded)
                        {
                            recipe.AddIngredient(InfernalCrossmod.SOTS.Mod.Find<ModItem>("CorrosiveGel").Type, 50);
                        }
                        recipe.AddIngredient<BlightedGel>(50);
                        recipe.AddIngredient(ItemID.PinkGel, 50);
                        recipe.AddIngredient(ItemID.Gel, 100);
                    }
                }
                #endregion
                #endregion

                #region Thorium
                if (thorium != null)
                {
                    thorium.TryFind("NinjaEmblem", out ModItem ninjaEmblem);
                    thorium.TryFind("BloomWeave", out ModItem bloomWeave);
                    thorium.TryFind("MermaidCanteen", out ModItem mermaidCanteen);
                    thorium.TryFind("TitanicBar", out ModItem titanBar);

                    if (recipe.HasResult(thorium.Find<ModItem>("TerraScythe")) || recipe.HasResult(thorium.Find<ModItem>("TerraKnife")))
                    {
                        recipe.RemoveIngredient(ModContent.ItemType<LivingShard>());
                        recipe.AddIngredient<LivingShard>(12);
                    }

                    if (recipe.HasIngredient<DepthCells>() && !recipe.HasIngredient(thorium.Find<ModItem>("AbyssalChitin")))
                    {
                        recipe.AddIngredient(thorium.Find<ModItem>("AbyssalChitin"), 5);
                    }

                    if (recipe.HasResult(thorium.Find<ModItem>("AquaiteBar")))
                    {
                        recipe.AddIngredient<SeaPrism>(5);
                    }

                    if (recipe.HasIngredient<Bloodstone>())
                    {
                        int count = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<Bloodstone>()).Sum(i => i.stack);
                        int stackSize = count / 2;

                        if (stackSize > 0)
                            recipe.AddIngredient(thorium.Find<ModItem>("BloodCell"), stackSize);
                    }

                    if (recipe.HasResult<AuricBar>())
                    {
                        recipe.RemoveIngredient(thorium.Find<ModItem>("InfernoEssence").Type);
                        recipe.RemoveIngredient(thorium.Find<ModItem>("OceanEssence").Type);
                        recipe.RemoveIngredient(thorium.Find<ModItem>("DeathEssence").Type);
                    }

                    if (InfernalConfig.Instance.ThoriumBalanceChangess)
                    {
                        if (recipe.HasResult<UnholyCore>())
                        {
                            recipe.AddIngredient(thorium.Find<ModItem>("SoulofPlight"), 1);
                        }

                        if (recipe.HasResult<MiracleMatter>() && !recipe.HasIngredient(thorium.Find<ModItem>("TerrariumCore")))
                        {
                            recipe.AddIngredient(thorium.Find<ModItem>("TerrariumCore"), 5);
                        }

                        /*
                        if (recipe.HasResult(thorium.Find<ModItem>("LodestoneJavelin")))
                        {
                            recipe.ReplaceResult(thorium.Find<ModItem>("LodestoneJavelin"), 200);
                        }

                        if (recipe.HasResult(thorium.Find<ModItem>("ValadiumBattleAxe")))
                        {
                            recipe.ReplaceResult(thorium.Find<ModItem>("ValadiumBattleAxe"), 200);
                        }
                        */

                        if (thorium.TryFind("KineticPotion", out ModItem kinetic))
                            if (recipe.HasResult(kinetic))
                                recipe.DisableRecipe();

                        if (recipe.HasResult(ItemID.JungleHat) || recipe.HasResult(ItemID.JungleShirt) || recipe.HasResult(ItemID.JunglePants) || recipe.HasResult(thorium.Find<ModItem>("BountifulHarvest")) || recipe.HasResult(thorium.Find<ModItem>("JunglesWrath")))
                        {
                            recipe.RemoveTile(TileID.Anvils);
                            recipe.RemoveTile(TileID.WorkBenches);
                            recipe.AddTile(thorium.Find<ModTile>("ArcaneArmorFabricator"));
                        }

                        /*
                        if (recipe.HasResult(thorium.Find<ModItem>("NecroticSkull")))
                        {
                            recipe.AddIngredient(ItemID.SoulofFright, 3);
                            recipe.RemoveTile(TileID.DemonAltar);
                            recipe.AddTile(TileID.MythrilAnvil);
                        }
                        */

                        if (recipe.HasResult(ModContent.ItemType<TheSponge>()) || recipe.HasResult(ModContent.ItemType<TheAmalgam>()) || recipe.HasResult(ModContent.ItemType<EclipseMirror>()))
                        {
                            recipe.AddIngredient(thorium.Find<ModItem>("DeathEssence").Type, 3);
                        }

                        if (recipe.HasResult(ModContent.ItemType<ChaliceOfTheBloodGod>()) || recipe.HasResult(ModContent.ItemType<AsgardianAegis>()))
                        {
                            recipe.AddIngredient(thorium.Find<ModItem>("InfernoEssence").Type, 3);
                        }

                        if (recipe.HasResult(ModContent.ItemType<StatisVoidSash>()))
                        {
                            recipe.AddIngredient(thorium.Find<ModItem>("OceanEssence").Type, 3);
                        }

                        if (InfernalCrossmod.SOTS.Loaded)
                        {
                            if (thorium.TryFind("FrenzyPotion", out ModItem frenzy))
                                if (recipe.HasResult(frenzy))
                                    recipe.DisableRecipe();

                            if (recipe.HasResult(InfernalCrossmod.SOTS.Mod.Find<ModItem>("PutridCoin")) || recipe.HasResult(InfernalCrossmod.SOTS.Mod.Find<ModItem>("BloodstainedCoin")))
                            {
                                recipe.RemoveTile(TileID.Anvils); ;
                                recipe.AddTile(thorium.Find<ModTile>("ArcaneArmorFabricator"));
                            }
                        }

                        if (InfernalCrossmod.Clamity.Loaded)
                        {
                            if (recipe.HasResult(InfernalCrossmod.Clamity.Mod.Find<ModItem>("SkullOfTheBloodGod")))
                            {
                                recipe.AddIngredient(thorium.Find<ModItem>("InfernoEssence").Type, 3);
                            }
                        }

                        if (!ModLoader.HasMod("WHummusMultiModBalancing"))
                        {
                            /*
                            if (recipe.HasResult(thorium.Find<ModItem>("Nocturnal")) || recipe.HasResult(thorium.Find<ModItem>("Sanguine")))
                            {
                                recipe.AddIngredient<PurifiedGel>(5);
                            }
                            */

                            if (thorium.TryFind("ThrowingGuideVolume2", out ModItem rogue101v2))
                            {
                                if (recipe.HasResult(rogue101v2))
                                {
                                    recipe.RemoveIngredient(ninjaEmblem.Type);
                                    recipe.AddIngredient(ModContent.ItemType<RogueEmblem>());
                                    recipe.AddIngredient(bloomWeave.Type, 5);
                                    recipe.AddIngredient(mermaidCanteen.Type);
                                }
                            }

                            if (thorium.TryFind("ThrowingGuideVolume3", out ModItem rouge101v3))
                            {
                                if (recipe.HasResult(rouge101v3))
                                {
                                    recipe.RemoveIngredient(ItemID.SoulofSight);
                                    recipe.AddIngredient(ModContent.ItemType<UelibloomBar>(), 3);
                                    recipe.RemoveIngredient(ItemID.SoulofMight);
                                    recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 4);
                                    recipe.RemoveIngredient(ItemID.SoulofFright);
                                }
                            }

                            if (thorium.TryFind("Mjolnir", out ModItem thorsHammer))
                            {
                                if (recipe.HasResult(thorsHammer))
                                {
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
                                }
                            }

                            if (thorium.TryFind("DarkContagion", out ModItem darkCont))
                            {
                                if (recipe.HasResult(darkCont))
                                {
                                    recipe.RemoveIngredient(ItemID.Ichor);
                                    recipe.RemoveIngredient(ItemID.SpellTome);
                                    recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 5);
                                    recipe.AddIngredient(ItemID.Deathweed, 2);
                                }
                            }

                            if (thorium.TryFind("AssassinsGuard", out ModItem assass1))
                            {
                                if (recipe.HasResult(assass1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("AssassinsWalkers", out ModItem assass2))
                            {
                                if (recipe.HasResult(assass2))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("DreamWeaversHelmet", out ModItem dreamweave1))
                            {
                                if (recipe.HasResult(dreamweave1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("DreamWeaversHood", out ModItem dreamweave2))
                            {
                                if (recipe.HasResult(dreamweave2))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("DreamWeaversTabard", out ModItem dreamweave3))
                            {
                                if (recipe.HasResult(dreamweave3))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("DreamWeaversTreads", out ModItem dreamweave4))
                            {
                                if (recipe.HasResult(dreamweave4))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("InspiratorsHelmet", out ModItem inspir1))
                            {
                                if (recipe.HasResult(inspir1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("MagmaSeersMask", out ModItem magmaseer1))
                            {
                                if (recipe.HasResult(magmaseer1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("MasterArbalestHood", out ModItem masterArba1))
                            {
                                if (recipe.HasResult(masterArba1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("MasterMarksmansScouter", out ModItem masterMark1))
                            {
                                if (recipe.HasResult(masterMark1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("PyromancerCowl", out ModItem pyro1))
                            {
                                if (recipe.HasResult(pyro1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("PyromancerLeggings", out ModItem pyro2))
                            {
                                if (recipe.HasResult(pyro2))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("PyromancerTabard", out ModItem pyro3))
                            {
                                if (recipe.HasResult(pyro3))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("RhapsodistBoots", out ModItem rhap1))
                            {
                                if (recipe.HasResult(rhap1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("RhapsodistChestWoofer", out ModItem rhap2))
                            {
                                if (recipe.HasResult(rhap2))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("SoloistHat", out ModItem solo1))
                            {
                                if (recipe.HasResult(solo1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("TideTurnerBreastplate", out ModItem tide1))
                            {
                                if (recipe.HasResult(tide1))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("TideTurnerGreaves", out ModItem tide2))
                            {
                                if (recipe.HasResult(tide2))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("TideTurnerHelmet", out ModItem tide3))
                            {
                                if (recipe.HasResult(tide3))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }
                            if (thorium.TryFind("TideTurnersGaze", out ModItem tide4))
                            {
                                if (recipe.HasResult(tide4))
                                {
                                    recipe.RemoveIngredient(ModContent.ItemType<AscendantSpiritEssence>());
                                    recipe.AddIngredient(ModContent.ItemType<AuricBar>());
                                }
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("MyceliumGattlingGun")))
                            {
                                int funggatType = thorium.Find<ModItem>("Funggat")?.Type ?? 0;
                                if (funggatType > 0 && !recipe.HasIngredient(funggatType))
                                {
                                    recipe.AddIngredient(funggatType);
                                }

                                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                                {
                                    int fungicideType = calamity.Find<ModItem>("Fungicide")?.Type ?? 0;
                                    if (fungicideType > 0 && !recipe.HasIngredient(fungicideType))
                                    {
                                        recipe.AddIngredient(fungicideType);
                                    }
                                }
                            }

                            if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                            {
                                ModItem[] preMechIngredients =
                                {
                                    GetItem(thorium, "BenignBalloon"),
                                    GetItem(thorium, "AbyssalChitin"),
                                    GetItem(thorium, "CrystalGeode"),
                                    GetItem(thorium, "UnfathomableFlesh"),
                                    GetItem(thorium, "GreenDragonScale"),
                                    GetItem(thorium, "HallowedCharm"),
                                    GetItem(thorium, "LodeStoneIngot"),
                                    GetItem(thorium, "CeruleanMorel"),
                                    GetItem(thorium, "PharaohsBreath"),
                                    GetItem(thorium, "SoulofPlight"),
                                    GetItem(thorium, "ValadiumIngot"),
                                    GetItem(thorium, "PotionChaser"),
                                    GetItem(thorium, "BronzeAlloyFragments"),
                                    GetItem(thorium, "BloodCell")
                                };

                                ModItem[] preMechItems =
                                {
                                    GetItem(thorium, "CrystalGeode"),
                                    GetItem(thorium, "UnfathomableFlesh"),
                                    GetItem(thorium, "GreenDragonScale"),
                                    GetItem(thorium, "SupportSash"),
                                    GetItem(thorium, "BalanceBloom"),
                                    GetItem(thorium, "ChronoOcarina"),
                                    GetItem(thorium, "CometCrossfire"),
                                    GetItem(thorium, "CorruptlingStaff"),
                                    GetItem(thorium, "CrimsonHoundStaff"),
                                    GetItem(thorium, "FrostwindCymbals"),
                                    GetItem(thorium, "IridescentStaff"),
                                    GetItem(thorium, "LustrousBaton"),
                                    GetItem(thorium, "MastersLibram"),
                                    GetItem(thorium, "Omniwrench"),
                                    GetItem(thorium, "StellarSystem"),
                                    GetItem(thorium, "Violin"),
                                    GetItem(thorium, "WindChimes"),
                                    GetItem(thorium, "LeechingSheath")
                                };

                                if (recipe.HasIngredient(ItemID.DynastyWood) && recipe.HasTile(TileID.MythrilAnvil))
                                {
                                    recipe.RemoveTile(TileID.MythrilAnvil);
                                    recipe.AddTile(TileID.Anvils);
                                }

                                foreach (ModItem item in preMechIngredients)
                                {
                                    if (recipe.HasIngredient(item) && recipe.HasTile(TileID.MythrilAnvil) && !recipe.HasResult(GetItem(thorium, "DragonTalonNecklace")))
                                    {
                                        recipe.RemoveTile(TileID.MythrilAnvil);
                                        recipe.AddTile(TileID.Anvils);
                                    }
                                }

                                foreach (ModItem iitem in preMechItems)
                                {
                                    if (recipe.HasResult(iitem) && recipe.HasTile(TileID.MythrilAnvil))
                                    {
                                        recipe.RemoveTile(TileID.MythrilAnvil);
                                        recipe.AddTile(TileID.Anvils);
                                    }
                                }
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("CrystalArrow")))
                            {
                                recipe.ReplaceResult(thorium.Find<ModItem>("CrystalArrow"), 75);
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("CapeoftheSurvivor")))
                            {
                                if (ModLoader.TryGetMod("Consolaria", out Mod console))
                                {
                                    recipe.AddIngredient(console.Find<ModItem>("SoulofBlight"), 5);
                                }
                                else
                                {
                                    recipe.AddIngredient(ItemID.BeetleHusk, 3);
                                }
                            }

                            if (recipe.HasResult(ModContent.ItemType<ReboundingRainbow>()))
                            {
                                recipe.AddIngredient(thorium.Find<ModItem>("TerraKnife"));
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("ClericsCross")))
                            {
                                recipe.RemoveIngredient(thorium.Find<ModItem>("PurifiedShards").Type);
                                recipe.AddIngredient(ItemID.FallenStar, 3);
                                recipe.AddIngredient(thorium.Find<ModItem>("Blood").Type);
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("Zunpet")))
                            {
                                recipe.RemoveIngredient(ItemID.HallowedBar);
                            }
                        }
                    }

                    if (InfernalConfig.Instance.MergeCraftingTrees)
                    {
                        if (recipe.HasResult(ModContent.ItemType<LifeAlloy>()))
                        {
                            recipe.AddIngredient(titanBar, 1);
                        }
                    }

                    if (InfernalConfig.Instance.DisableDuplicateContent)
                    {
                        if (thorium.TryFind("CobaltThrowingSpear", out ModItem cbthrowspear))
                        {
                            if (recipe.HasResult(cbthrowspear))
                                recipe.DisableRecipe();
                        }

                        if (thorium.TryFind("PalladiumThrowingSpear", out ModItem palthrowspear))
                        {
                            if (recipe.HasResult(palthrowspear))
                                recipe.DisableRecipe();
                        }

                        if (thorium.TryFind("IronTomahawk", out ModItem ironToma))
                        {
                            if (recipe.HasResult(ironToma))
                            {
                                recipe.DisableRecipe();
                            }
                        }

                        if (thorium.TryFind("LeadTomahawk", out ModItem leadToma))
                        {
                            if (recipe.HasResult(leadToma))
                            {
                                recipe.DisableRecipe();
                            }
                        }

                        if (thorium.TryFind("AromaticBulb", out ModItem bulb))
                        {
                            if (recipe.HasResult(bulb))
                                recipe.DisableRecipe();

                            if (recipe.HasIngredient(bulb))
                            {
                                recipe.RemoveIngredient(bulb.Type);
                                recipe.AddIngredient<Portabulb>();
                            }
                        }

                        string[] coatings =
                        {
                            "DeepFreezeCoatingItem",
                            "ExplosiveCoatingItem",
                            "GorgonCoatingItem",
                            "SporeCoatingItem",
                            "ToxicCoatingItem",
                        };

                        foreach (string coating in coatings)
                            if (thorium.TryFind(coating, out ModItem coatingItem))
                                if (recipe.HasResult(coatingItem))
                                    recipe.DisableRecipe();

                        if (thorium.TryFind("AdamantiteGlaive", out ModItem adamGlaive))
                            if (recipe.HasResult(adamGlaive))
                                recipe.DisableRecipe();

                        if (thorium.TryFind("TitaniumGlaive", out ModItem titanGlaive))
                            if (recipe.HasResult(titanGlaive))
                                recipe.DisableRecipe();

                        string[] disabledItems =
                        {
                            //"ChlorophyteTomahawk",
                            "DemonBloodBow",
                            //"MyceliumGatlingGun",
                            "TimeWarp",
                            "MoltenKnife",
                        };

                        foreach (string item in disabledItems)
                            if (thorium.TryFind(item, out ModItem tempItem))
                                if (recipe.HasResult(tempItem))
                                    recipe.DisableRecipe();

                        if (recipe.HasIngredient(thorium.Find<ModItem>("MoltenKnife")))
                        {
                            recipe.RemoveIngredient(thorium.Find<ModItem>("MoltenKnife").Type);
                            recipe.AddIngredient<InfernalKris>();
                        }
                    }

                    if (InfernalConfig.Instance.DisableUnnecessaryContent)
                    {
                        if (recipe.HasResult(GetItem(thorium, "TitanBreastplate"))
                         || recipe.HasResult(GetItem(thorium, "TitanGreaves"))
                         || recipe.HasResult(GetItem(thorium, "TitanHeadgear"))
                         || recipe.HasResult(GetItem(thorium, "TitanMask"))
                         || recipe.HasResult(GetItem(thorium, "TitanHelmet"))
                         || recipe.HasResult(GetItem(thorium, "TitanWings")))
                        {
                            recipe.DisableRecipe();
                        }
                    }
                }
                #endregion

                #region Zenith Toilet
                if (ModLoader.TryGetMod("ZenithToilet", out Mod toilet))
                {
                    if (toilet.TryFind("ZenithToilet", out ModItem zToilet))
                    {
                        if (recipe.HasResult(zToilet))
                        {
                            recipe.AddIngredient(ModContent.ItemType<Rock>(), 1);
                            recipe.RemoveTile(TileID.MythrilAnvil);
                            recipe.AddIngredient<AshesofAnnihilation>(3);
                            recipe.AddIngredient<MiracleMatter>(3);
                            recipe.AddIngredient<ShadowspecBar>(3);
                            recipe.AddTile(ModContent.TileType<DraedonsForge>());
                        }
                    }

                    if (toilet.TryFind("TrueZenithToilet", out ModItem trueToilet))
                    {
                        if (recipe.HasResult(trueToilet))
                        {
                            recipe.RemoveTile(TileID.MythrilAnvil);
                            recipe.AddTile(ModContent.TileType<DraedonsForge>());
                            recipe.AddIngredient(ModContent.ItemType<StellarSabre>(), 1);
                            recipe.AddIngredient(ModContent.ItemType<Lycanroc>());
                            recipe.AddIngredient(ModContent.ItemType<Swordofthe14thGlitch>(), 1);
                            recipe.AddIngredient(ModContent.ItemType<NovaBomb>(), 1);
                            recipe.AddIngredient(ModContent.ItemType<Kevin>(), 1);
                            if (InfernalCrossmod.NoxusBoss.Loaded)
                            {
                                recipe.AddIngredient(ModContent.ItemType<ChaosBlaster>());
                                recipe.AddIngredient(ModContent.ItemType<NebulaGigabeam>());
                                recipe.AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition);
                            }
                            recipe.AddIngredient(ModContent.ItemType<ChromaticMassInABottle>(), 1);
                            recipe.AddIngredient(ModContent.ItemType<Rock>(), 1);
                        }
                    }
                }
                #endregion

                #region Ragnarok
                if (ModLoader.TryGetMod("RagnarokMod", out Mod ragCal))
                {
                    /*
                    ragCal.TryFind("JellySlicer", out ModItem gelSlicer);

                    if (recipe.HasResult(gelSlicer) && !ModLoader.TryGetMod("WHummusMultiModBalancing", out _))
                    {
                        recipe.RemoveTile(TileID.Anvils);
                        recipe.AddTile(ModContent.TileType<StaticRefiner>());
                    }
                    */

                    ragCal.TryFind("ExecutionerMark05", out ModItem exMark5);
                    ragCal.TryFind("ElementalReaper", out ModItem elementalReaper);
                    thorium.TryFind("TerrariumHolyScythe", out ModItem holySycthe);

                    if (recipe.HasResult(exMark5) && holySycthe != null && !ModLoader.TryGetMod("WHummusMultiModBalancing", out _))
                    {
                        recipe.RemoveIngredient(holySycthe.Type);
                        recipe.AddIngredient(elementalReaper.Type);
                    }

                    if (recipe.HasResult(ragCal.Find<ModItem>("MarbleScythe")) && !ModLoader.TryGetMod("WHummusMultiModBalancing", out _))
                    {
                        recipe.ChangeIngredientStack(ragCal.Find<ModItem>("EnchantedMarble").Type, 3);
                        recipe.AddIngredient(thorium.Find<ModItem>("BronzeAlloyFragments"), 8);
                        recipe.AddIngredient(thorium.Find<ModItem>("SpiritDroplet"), 10);
                    }

                    if (recipe.HasResult(ragCal.Find<ModItem>("Virusprayer")))
                    {
                        recipe.DisableRecipe();
                    }
                }
                #endregion

                #region Thorium Helheim
                if (ModLoader.TryGetMod("ThoriumRework", out Mod thorRework))
                {
                    if (InfernalConfig.Instance.ThoriumBalanceChangess)
                    {
                        if (thorRework.TryFind("DeathsingerPotion", out ModItem deathSingerPotion) && recipe.HasResult(deathSingerPotion))
                        {
                            recipe.DisableRecipe();
                        }

                        if (!ModLoader.HasMod("WHummusMultiModBalancing"))
                        {
                            if (thorRework.TryFind("Loudener", out ModItem loud))
                            {
                                if (recipe.HasResult(loud))
                                {
                                    recipe.AddIngredient(thorium.Find<ModItem>("BronzeAlloyFragments"), 5);
                                }
                            }

                            if (thorRework.TryFind("ImpulseAmplifier", out ModItem impulse))
                            {
                                if (recipe.HasResult(impulse))
                                {
                                    recipe.RemoveIngredient(ItemID.Wire);
                                    recipe.AddIngredient(ModContent.ItemType<StormlionMandible>(), 1);
                                }
                            }

                            if (thorRework.TryFind("GildedFlyingFlute", out ModItem gff))
                            {
                                if (recipe.HasResult(gff))
                                {
                                    recipe.AddIngredient(ItemID.SoulofSight, 3);
                                    recipe.AddIngredient(ItemID.SoulofMight, 3);
                                    recipe.AddIngredient(ItemID.SoulofFright, 3);
                                }
                            }
                        }
                    }

                    if (InfernalConfig.Instance.DisableUnnecessaryContent)
                    {
                        if (recipe.HasResult(GetItem(thorRework, "TitanHat"))
                         || recipe.HasResult(GetItem(thorRework, "TitanHood"))
                         || recipe.HasResult(GetItem(thorRework, "TitanVisage"))
                         || recipe.HasResult(GetItem(thorRework, "TitanVisor")))
                        {
                            recipe.DisableRecipe();
                        }
                    }
                }
                #endregion

                #region Calamity Bard Healer
                if (ModLoader.TryGetMod("CalamityBardHealer", out Mod calBardHeal) && InfernalConfig.Instance.ThoriumBalanceChangess)
                {
                    thorium.TryFind("TerrariumHolyScythe", out ModItem holySycthe);

                    if (calBardHeal.TryFind("SongoftheAncients", out ModItem songAncinet))
                    {
                        if (recipe.HasResult(songAncinet))
                        {
                            thorium.TryFind("BrokenHeroFragment", out ModItem heroFragment);
                            recipe.ChangeIngredientStack(heroFragment.Type, 3);
                            recipe.AddIngredient<LivingShard>(12);
                        }
                    }

                    if (calBardHeal.TryFind("Syzygy", out ModItem syzygy) && !ModLoader.TryGetMod("WHummusMultiModBalancing", out _))
                    {
                        if (recipe.HasResult(syzygy))
                        {
                            thorium.TryFind("TerraScythe", out ModItem terraSycthe);

                            recipe.RemoveIngredient(terraSycthe.Type);
                            recipe.AddIngredient(holySycthe.Type);
                        }
                    }

                    if (calBardHeal.TryFind("CherubimOmega", out ModItem healOmega))
                    {
                        if (recipe.HasResult(healOmega))
                            recipe.RemoveIngredient(holySycthe.Type);
                    }
                }
                #endregion

                #region SOTS
                if (InfernalConfig.Instance.SOTSBalanceChanges)
                {
                    if (ModLoader.TryGetMod("SOTS", out Mod sots))
                    {
                        if (InfernalConfig.Instance.MergeCraftingTrees)
                        {
                            if (recipe.HasResult(ModContent.ItemType<RecitationoftheBeast>()))
                            {
                                recipe.RemoveIngredient(ItemID.DemonScythe);
                                recipe.AddIngredient(sots.Find<ModItem>("DanceOfDeath"));
                            }

                            if (recipe.HasResult<Cosmolight>() && !recipe.HasIngredient(sots.Find<ModItem>("LunarClock")))
                            {
                                recipe.DisableRecipe();
                            }
                        }

                        // Frigid Pickaxe
                        if (sots.TryFind("FrigidPickaxe", out ModItem frigidPick))
                        {
                            if (recipe.HasResult(frigidPick))
                            {
                                recipe.AddRecipeGroup(EvilSkinRecipeGroup, 6);
                            }
                        }

                        // Challenger's Ring
                        if (sots.TryFind("ChallengerRing", out ModItem challRing))
                        {
                            if (recipe.HasResult(challRing))
                            {
                                if (sots.TryFind("PhaseBar", out ModItem phaseBar))
                                {
                                    recipe.AddIngredient(phaseBar, 6);
                                }
                            }
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("DigitalDisplay")))
                            recipe.DisableDecraft();
                            
                        if (recipe.HasResult(sots.Find<ModItem>("NightmarePotion")))
                        {
                            recipe.DecraftConditions.Add(Condition.Hardmode);
                        }

                        // Tesseract
                        if (recipe.HasResult(sots.Find<ModItem>("Tesseract")))
                        {
                            recipe.RemoveIngredient(ModContent.ItemType<AuricBar>());
                            recipe.RemoveTile(TileID.LunarCraftingStation);
                            recipe.AddIngredient<Rock>();
                            if (InfernalCrossmod.NoxusBoss.Loaded)
                            {
                                recipe.AddIngredient<AlloyofEden>(3);
                                recipe.AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition);
                            }
                            recipe.AddTile(ModContent.TileType<DraedonsForge>());
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("PurpleJellyfishStaff")))
                        {
                            recipe.RemoveTile(TileID.MythrilAnvil);
                            recipe.AddTile(TileID.Anvils);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("EyeOfChaos")))
                        {
                            if (InfernalCrossmod.Thorium.Loaded)
                            {
                                recipe.RemoveIngredient(ItemID.EyeoftheGolem);
                                recipe.AddIngredient(InfernalCrossmod.Thorium.Mod.Find<ModItem>("MaskoftheCrystalEye").Type);
                            }
                            recipe.AddIngredient(sots.Find<ModItem>("PhaseBar"), 3);
                        }

                        if (recipe.HasResult<AuricQuantumCoolingCell>())
                        {
                            recipe.AddIngredient(sots.Find<ModItem>("DissolvingAurora"));
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("PhaseBar")) && recipe.HasIngredient(sots.Find<ModItem>("DissolvingBrilliance")))
                        {
                            recipe.DisableRecipe();
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("VoidmageIncubator")))
                        {
                            recipe.AddIngredient(ItemID.FragmentStardust, 10);
                            recipe.AddIngredient<Necroplasm>(5);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("FlowerCrown")))
                        {
                            recipe.AddIngredient<EssenceofHavoc>(4);
                            recipe.RemoveTile(TileID.WorkBenches);
                            recipe.AddTile(TileID.LivingLoom);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("WorldlyPolarizer")))
                        {
                            recipe.AddIngredient(sots.Find<ModItem>("HardlightAlloy"), 5);
                            recipe.RemoveTile(TileID.Anvils);
                            if (InfernalCrossmod.Thorium.Loaded)
                            {
                                recipe.RemoveTile(thorium.Find<ModTile>("ThoriumAnvil").Type);
                            }
                            recipe.AddTile(sots.Find<ModTile>("HardlightFabricatorTile"));
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("ThermalPolarizer")))
                        {
                            recipe.AddIngredient<ScoriaBar>(5);
                            recipe.RemoveTile(TileID.MythrilAnvil);
                            if (InfernalCrossmod.Thorium.Loaded)
                            {
                                recipe.AddTile(thorium.Find<ModTile>("SoulForge").Type);
                            }
                            else
                            {
                                recipe.AddTile(TileID.AdamantiteForge);
                            }
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("ExoticPolarizer")))
                        {
                            recipe.AddIngredient<MeldBlob>(5);
                            recipe.RemoveTile(TileID.MythrilAnvil);
                            recipe.AddTile(TileID.LunarCraftingStation);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("UltimatePolarizer")))
                        {
                            recipe.AddIngredient(ItemID.LunarBar, 5);
                            if (InfernalCrossmod.Catalyst.Loaded)
                                recipe.AddIngredient(InfernalCrossmod.Catalyst.Mod.Find<ModItem>("MetanovaBar"), 5);
                            recipe.AddIngredient(sots.Find<ModItem>("SanguiteBar"), 5);
                            recipe.RemoveTile(TileID.TinkerersWorkbench);
                            recipe.AddTile(TileID.LunarCraftingStation);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("CursedIcosahedron")))
                        {
                            recipe.AddIngredient(ItemID.Ectoplasm, 3);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("ShoeIce")))
                        {
                            recipe.RemoveTile(TileID.Anvils);
                            recipe.AddTile(TileID.TinkerersWorkbench);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("BlinkPack")))
                        {
                            recipe.AddIngredient(ItemID.SoulofLight, 5);
                            recipe.RemoveTile(sots.Find<ModTile>("HardlightFabricatorTile").Type);
                            recipe.AddTile(TileID.MythrilAnvil);
                        }

                        if (recipe.HasResult(sots.Find<ModItem>("DissolvingBrillianceBlock")))
                        {
                            recipe.AddTile(TileID.LunarCraftingStation);
                        }

                        #region Soul of Plight Additions
                        if (recipe.HasResult(ItemID.TrueNightsEdge))
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 3);

                        if (ModLoader.TryGetMod("Consolaria", out Mod console))
                        {
                            if (recipe.HasResult(console.Find<ModItem>("SuspiciousLookingSkull")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);
                        }

                        if (recipe.HasResult(ModContent.ItemType<HallowedRune>()))
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);

                        if (recipe.HasResult(ModContent.ItemType<MOAB>()))
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 1);

                        if (recipe.HasResult<SanguineTangerine>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);

                        if (recipe.HasResult<ValkyrieRay>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 1);

                        /* Removed in Calamity 2.1
                        if (recipe.HasResult<CatastropheClaymore>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 3);
                        */

                        if (recipe.HasResult<Pwnagehammer>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 3);

                        if (recipe.HasResult<TrueBiomeBlade>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 1);

                        if (recipe.HasResult<Exorcism>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 6);

                        if (recipe.HasResult<SpearofDestiny>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);
                            
                        if (recipe.HasResult<VengefulSunStaff>())
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 3);

                        if (recipe.HasResult<AngelTreads>())
                        {
                            recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"));
                        }

                        if (thorium != null)
                        {
                            if (recipe.HasIngredient(sots.Find<ModItem>("VibrantBar")) && recipe.HasTile(TileID.Anvils))
                            {
                                recipe.RemoveTile(TileID.Anvils);
                                recipe.AddTile(thorium.Find<ModTile>("ThoriumAnvil"));
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("SubspaceWings")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);

                            if (recipe.HasResult(thorium.Find<ModItem>("ValkyrieBlade")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 6);

                            if (recipe.HasResult(thorium.Find<ModItem>("ArchangelHeart")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);

                            if (recipe.HasResult(thorium.Find<ModItem>("ArchDemonCurse")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);

                            if (recipe.HasResult(thorium.Find<ModItem>("TrueBloodHarvest")) ||
                                recipe.HasResult(thorium.Find<ModItem>("TrueFallingTwilight")) ||
                                recipe.HasResult(thorium.Find<ModItem>("TrueEmbowelment")) ||
                                recipe.HasResult(thorium.Find<ModItem>("TrueLightAnguish"))
                                )
                            {
                                recipe.ChangeIngredientStack(ItemID.SoulofFright, 3);
                                recipe.ChangeIngredientStack(ItemID.SoulofMight, 3);
                                recipe.ChangeIngredientStack(ItemID.SoulofSight, 3);
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 3);
                            }

                            if (recipe.HasResult(thorium.Find<ModItem>("BlackholeCannon")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);

                            if (recipe.HasResult(thorium.Find<ModItem>("TimeWarp")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 4);

                            if (recipe.HasResult(thorium.Find<ModItem>("SoulForge")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 5);
                        }

                        if (ModLoader.TryGetMod("AlchemistNPCLite", out Mod alchNPC))
                        {
                            if (recipe.HasResult(alchNPC.Find<ModItem>("LuckCharmT2")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 10);
                        }

                        if (calHunt != null)
                        {
                            if (recipe.HasResult(calHunt.Find<ModItem>("GelatinousCatalyst")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 3);
                        }

                        if (ModLoader.TryGetMod("MagicStorage", out Mod magicStorage))
                        {
                            if (recipe.HasResult(magicStorage.Find<ModItem>("UpgradeHallowed")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 1);
                        }

                        //if (ModLoader.TryGetMod("Clamity", out Mod clam))
                        //{
                        //    if(recipe.HasResult(clam.Find<ModItem>("SoulBaguette")))
                        //        recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 1); //not needed anymore i believe -Arkangel
                        //}

                        if (ModLoader.TryGetMod("FishGunsPlus", out Mod fishGun))
                        {
                            if(recipe.HasResult(fishGun.Find<ModItem>("TrueMutantNightfish")))
                                recipe.AddIngredient(sots.Find<ModItem>("SoulOfPlight"), 20);
                        }
                        #endregion
                    }
                }
                #endregion

                #region Encahnted Moons
                if (InfernalCrossmod.BlueMoon.Loaded)
                {
                    Mod blueMoon = InfernalCrossmod.BlueMoon.Mod;

                    if (recipe.HasResult(blueMoon.Find<ModItem>("MoonsRing")))
                    {
                        recipe.RemoveTile(TileID.Anvils);
                        recipe.AddTile(TileID.TinkerersWorkbench);
                    }
                }
                #endregion

                #region More Pylons
                if (ModLoader.TryGetMod("EvilPylon", out Mod morePylon))
                {
                    if (recipe.HasResult(GetItem(morePylon, "BonePylon")))
                    {
                        recipe.DecraftConditions.Add(Condition.DownedSkeletron);
                    }

                    if (recipe.HasResult(GetItem(morePylon, "CorruptionPylon")))
                    {
                        recipe.DecraftConditions.Add(Condition.DownedEowOrBoc);
                    }

                    if (recipe.HasResult(GetItem(morePylon, "CrimsonPylon")))
                    {
                        recipe.DecraftConditions.Add(Condition.DownedEowOrBoc);
                    }

                    if (recipe.HasResult(GetItem(morePylon, "HellPylon")))
                    {
                        recipe.DecraftConditions.Add(Condition.DownedEowOrBoc);
                    }
                }
                #endregion
            }
        }

        private static ModItem GetItem(Mod mod, string name)
        {
            return mod.Find<ModItem>(name);
        }

        private static readonly Condition SkeletronOrHardmode = new("Conditions.DownedSkeletron", () => NPC.downedBoss3 || Main.hardMode);
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public static class SOTSWormholeRecipes
    {
        public static void Initialize()
        {
            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                ItemHelpers.WormholeRecipes.Add(new ItemHelpers.WormholeRecipe(calHunt.Find<ModItem>("ChromaticMass").Type, ModContent.ItemType<AngryPudding>()));
            }
        }
    }
}
