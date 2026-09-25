using CalamityMod;
using System.Reflection;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Melee;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Systems;
using InfernumMode.Content.Items.Weapons.Magic;
using InfernumMode.Content.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using InfernalEclipseAPI.Core.Configs;
using Terraria.GameInput;
using System.Linq;
using InfernumMode.Content.Rarities.InfernumRarities;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems
{
    public class InfernalItemBalanceChange : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public float damage;
        public float pick;
        public float useTime;
        public float useAnimation;
        public float shootSpeed;
        public float crit;
        public float defense;

        public bool modified = false;

        public override void SetDefaults(Item item)
        {
            damage = item.damage;
            pick = item.pick;
            useTime = item.useTime;
            useAnimation = item.useAnimation;
            shootSpeed = item.shootSpeed;
            crit = item.crit;
            defense = item.defense;

            #region Vanilla
            if (item.type == ItemID.ReaverShark)
            {
                item.pick = 59;
                item.useTime = 11;
            }

            if (ModContent.GetInstance<InfernalConfig>().VanillaBalanceChanges)
            {
                if (item.type == ItemID.FieryGreatsword)
                {
                    item.damage = 50;
                    item.useTime = 36;
                    item.useAnimation = 36;
                }

                if (item.type == ItemID.Muramasa)
                {
                    item.useTime = 13;
                    item.useAnimation = 13;
                    item.damage = 22;
                }

                if (item.type == ItemID.NightsEdge)
                {
                    item.useTime = 16;
                    item.useAnimation = 16;
                    item.damage = 45;
                }

                if (item.type == ItemID.BoneWhip)
                {
                    item.useTime = 22;
                    item.useAnimation = 22;
                    item.damage = 30;
                }

                if (item.type == ItemID.ThornWhip)
                {
                    item.useTime = 30;
                    item.useAnimation = 30;
                }

                if (item.type == ItemID.Zenith)
                {
                    item.damage = 310;
                }

                if (item.type == ItemID.TrueNightsEdge)
                {
                    item.damage = 165;
                }

                if (item.type == ItemID.LucyTheAxe)
                {
                    item.damage = 37;
                }

                if (item.type == ItemID.PewMaticHorn)
                {
                    item.damage = 35;
                }

                if (item.type == ItemID.WeatherPain)
                {
                    item.useTime = 16;
                    item.useAnimation = 16;
                    item.damage = 30;
                }

                if (item.type == ItemID.HoundiusShootius)
                {
                    item.damage = 40;
                }

                if (item.type == ItemID.ButchersChainsaw)
                {
                    item.damage = 110;
                }

                if (item.type == ItemID.BreakerBlade)
                {
                    item.scale *= 1.5f;
                }

                if (item.type == ItemID.StaffoftheFrostHydra)
                {
                    item.damage = 300;
                }

                if (item.type == ItemID.StormTigerStaff)
                {
                    item.damage = 60;
                }

                if (item.type == ItemID.MolotovCocktail)
                {
                    item.damage = 14;
                }

                if (item.type == ItemID.RainbowCrystalStaff)
                {
                    item.damage = 70;
                }
            }
            #endregion

            #region Zenith Toilet
            if (ModLoader.TryGetMod("ZenithToilet", out Mod toilet))
            {
                if (item.type == toilet.Find<ModItem>("ZenithToilet").Type)
                {
                    item.damage = 50000;
                    item.crit = 4;
                }

                if (item.type == toilet.Find<ModItem>("TrueZenithToilet").Type)
                {
                    item.damage = 100000;
                    item.crit = 0;
                }
            }
            #endregion

            #region You
            if (ModLoader.TryGetMod("YouBoss", out Mod youBoss))
            {
                //First Fractal
                if (GetItem(youBoss, "FirstFractal", item))
                {
                    item.damage = 315;
                    item.DamageType = ModContent.GetInstance<MythicMelee>();
                    item.rare = ModContent.RarityType<InfernumVassalRarity>();
                    item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
                }
            }
            #endregion

            #region Shields of Cthulhu
            if (ModLoader.TryGetMod("ShieldsOfCthulhu", out Mod SoC))
            {
                if (GetItem(SoC, "CobaltShieldOfCthulhu", item))
                {
                    item.defense = 3;
                }

                if (GetItem(SoC, "ObsidianShieldOfCthulhu", item))
                {
                    item.defense = 4;
                }

                if (GetItem(SoC, "AnkhShieldOfCthulhu", item))
                {
                    item.defense = 6;
                }
            }
            #endregion

            #region Calamity
            if (InfernalConfig.Instance.CalamityBalanceChanges)
            {
                ModLoader.TryGetMod("CalamityMod", out Mod cal);

                #region Melee
                /*
                if (item.type == ModContent.ItemType<Auger>())
                {
                    item.damage = 76;
                }
                /*

                /*
                if (GetItem(cal, "SubmarineShocker", item))
                {
                    item.damage = 55;
                }
                */

                if (item.type == ModContent.ItemType<InfernaCutter>())
                {
                    item.DamageType = DamageClass.Melee;
                }

                if (item.type == ModContent.ItemType<ForbiddenOathblade>())
                {
                    item.damage = 72;
                }

                if (item.type == ModContent.ItemType<ExaltedOathblade>())
                {
                    item.damage = 110;
                }

                /*
                if (GetItem(cal, "EmpyreanKnives", item))
                {
                    item.useTime = 16;
                    item.useAnimation = 16;
                    item.damage = 90;
                }
                */

                if (item.type == ModContent.ItemType<GalvanizingGlaive>())
                {
                    item.damage = 160;
                }
                #endregion

                if (item.type == ModContent.ItemType<PrismaticBreaker>())
                {
                    item.damage = 300;
                }

                #region Ranged
                if (item.type == ModContent.ItemType<ShortCircuit>())
                {
                    item.damage = 14;
                }

                if (GetItem(cal, "CrackshotColt", item))
                {
                    item.useTime = 28;
                    item.useAnimation = 28;
                    item.damage = 30;
                }

                if (GetItem(cal, "MidasPrime", item))
                {
                    item.useTime = 26;
                    item.useAnimation = 26;
                }

                if (GetItem(cal, "Barinade", item))
                {
                    item.useTime = 15;
                    item.useAnimation = 15;
                }

                if (GetItem(cal, "Galeforce", item))
                {
                    item.damage = 25;
                }

                if (GetItem(cal, "FlurrystormCannon", item))
                {
                    item.damage = 14;
                }

                if (GetItem(cal, "Goobow", item))
                {
                    item.damage = 47;
                    item.useTime = 24;
                    item.useAnimation = 24;
                }

                if (GetItem(cal, "OverloadedBlaster", item))
                {
                    item.useTime = 24;
                    item.useAnimation = 24;
                }

                if (GetItem(cal, "ThermoclineBlaster", item))
                {
                    item.damage = 62;
                }

                if (GetItem(cal, "VernalBolter", item))
                {
                    item.useTime = 19;
                    item.useAnimation = 19;
                }

                if (item.type == ModContent.ItemType<Megalodon>())
                {
                    item.damage = 48;
                }

                /*
                if (GetItem(cal, "Arietes41", item))
                {
                    item.damage = 70;
                }
                */

                if (GetItem(cal, "ElementalEruption", item))
                {
                    item.damage = 116;
                }

                if (item.type == ModContent.ItemType<Voidragon>())
                {
                    item.damage = 450;
                }
                #endregion

                #region Mage
                if (item.type == ModContent.ItemType<AcidGun>())
                {
                    item.damage = 13;
                }

                if (item.type == ModContent.ItemType<HyphaeRod>())
                {
                    item.damage = 28;
                }

                if (item.type == ModContent.ItemType<PulsePistol>())
                {
                    item.useTime = 30;
                    item.useAnimation = 30;
                }

                if (item.type == ModContent.ItemType<NightsRay>())
                {
                    item.damage = 17;
                }

                /*
                if (item.type == ModContent.ItemType<VividClarity>())
                {
                    item.damage = 144;
                }
                */
                #endregion

                #region Summonerr
                if (item.type == ModContent.ItemType<EnchantedKnifeStaff>())
                {
                    item.damage = 13;
                }

                if (item.type == ModContent.ItemType<SunSpiritStaff>())
                {
                    item.damage = 20;
                }

                //Deathstare Rod
                if (item.type == cal.Find<ModItem>("DeathstareRod").Type)
                {
                    item.damage = 30;
                }

                //Herring Staff
                if (item.type == cal.Find<ModItem>("HerringStaff").Type)
                {
                    item.damage = 20;
                }

                //Mounted Scanner
                if (GetItem(cal, "MountedScanner", item))
                {
                    item.damage = 33;
                }

                //Plantation Staff 
                if (GetItem(cal, "PlantationStaff", item))
                {
                    item.damage = 82;
                }

                if (item.type == ModContent.ItemType<Vigilance>())
                {
                    item.damage = 325;
                }

                if (item.type == ModContent.ItemType<CosmicImmaterializer>())
                {
                    item.damage = 1020;
                }

                if (item.type == ModContent.ItemType<CadaverousCarrion>())
                {
                    item.damage = 244;
                }

                if (item.type == ModContent.ItemType<StarspawnHelixStaff>())
                {
                    item.damage = 125;
                }

                if (item.type == ModContent.ItemType<PolypLauncher>())
                {
                    item.damage = 30;
                }

                if (item.type == ModContent.ItemType<DreadmineStaff>())
                {
                    item.damage = 50;
                }
                #endregion

                #region Rogue
                if (item.type == ModContent.ItemType<Turbulance>())
                {
                    item.damage = 24;
                }

                if (item.type == ModContent.ItemType<FeatherKnife>())
                {
                    item.damage = 22;
                }

                if (item.type == ModContent.ItemType<LeviathanTeeth>())
                {
                    item.damage = 40;
                    item.useTime = 12;
                    item.useAnimation = 12;
                }
                #endregion
            }

            /* Added by Calamity in 2.1
            if (InfernalConfig.Instance.CalamityExpertAccessories)
            {
                int[] expertAccessoires =
                [
                    3883,
                    3090,
                    3097,
                    3224,
                    3223,
                    3333,
                    3245,
                    5100,
                    4987,
                    3336,
                    3337,
                    4989,
                    1131,
                    ModContent.ItemType<SandCloak>(),
                    ModContent.ItemType<ManaPolarizer>(),
                    ModContent.ItemType<FungalClump>(),
                    ModContent.ItemType<HideofAstrumDeus>(),
                    ModContent.ItemType<FleshTotem>(),
                    ModContent.ItemType<ToxicHeart>(),
                    ModContent.ItemType<GravistarSabaton>(),
                    ModContent.ItemType<LeviathanAmbergris>(),
                    ModContent.ItemType<VoidofCalamity>(),
                    ModContent.ItemType<FlameLickedShell>(),
                    ModContent.ItemType<CorrosiveSpine>(),
                    ModContent.ItemType<FrostFlare>(),
                    ModContent.ItemType<Calamity>(),
                    ModContent.ItemType<DraedonsHeart>(),
                    ModContent.ItemType<YharimsGift>(),
                    ModContent.ItemType<NebulousCore>(),
                    ModContent.ItemType<Affliction>(),
                    ModContent.ItemType<SpectralVeil>(),
                    ModContent.ItemType<TheEvolution>(),
                    ModContent.ItemType<DynamoStemCells>(),
                    ModContent.ItemType<BloodyWormTooth>(),
                    ModContent.ItemType<BloodyWormScarf>(),
                    ModContent.ItemType<RottenBrain>(),
                    ModContent.ItemType<AmalgamatedBrain>(),
                    ModContent.ItemType<TheAmalgam>(),
                    ModContent.ItemType<AscendantInsignia>(),
                    ModContent.ItemType<TheCamper>(),
                    ModContent.ItemType<ShieldoftheHighRuler>(),
                    ModContent.ItemType<OldDukeScales>(),
                    ModContent.ItemType<BlazingCore>(),
                    ModContent.ItemType<RelicOfResilience>(),
                    ModContent.ItemType<RelicOfDeliverance>(),
                    ModContent.ItemType<RelicOfConvergence>(),
                ];

                int[] masterAccessories =
                [
                    ModContent.ItemType<Regenator>(),
                    ModContent.ItemType<TheBee>(),
                    ModContent.ItemType<TheCommunity>(),
                    ModContent.ItemType<ShatteredCommunity>(),
                    ModContent.ItemType<WarbanneroftheSun>(),
                    5075
                ];

                if (expertAccessoires.Contains(item.type))
                {
                    item.expert = true;
                    item.rare = ItemRarityID.Expert;
                }

                if (masterAccessories.Contains(item.type))
                {
                    item.master = true;
                    item.rare = ItemRarityID.Master;
                }
            }
            */

            #endregion

            #region Infernum
            if (InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (item.type == ModContent.ItemType<AridBattlecry>())
                {
                    item.damage = 163;
                }

                if (item.type == ModContent.ItemType<WanderersShell>())
                {
                    item.damage = 160;
                }

                if (item.type == ModContent.ItemType<Dreamtastic>())
                {
                    item.damage = 842;
                }

                if (item.type == ModContent.ItemType<EyeOfMadness>())
                {
                    item.damage = 388;
                }

                if (item.type == ModContent.ItemType<IllusionersReverie>())
                {
                    item.damage = 460;
                }

                if (item.type == ModContent.ItemType<StormMaidensRetribution>())
                {
                    item.damage = 5056;
                }
            }
            #endregion

            #region Catalyst
            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
            {
                if (InfernalConfig.Instance.CalamityBalanceChanges)
                {
                    #region Ranger
                    //Desert Scorcher
                    if (item.type == catalyst.Find<ModItem>("DesertScorcher").Type)
                    {
                        item.shootSpeed = 10;
                        item.damage = 8;
                        item.useTime = 40;
                        item.useAnimation = 40;
                    }

                    //Ichorthrower
                    if (item.type == catalyst.Find<ModItem>("Ichorthrower").Type)
                    {
                        item.shootSpeed = (int)3.5;
                        item.damage = 21;
                    }

                    if (GetItem(catalyst, "BreathofGlacies", item))
                    {
                        item.damage = 70;
                    }

                    if (GetItem(catalyst, "StaticInferno", item))
                    {
                        item.useTime = 26;
                        item.useAnimation = 26;
                    }
                    #endregion

                    #region Summoner
                    //Coral Crusher
                    if (item.type == catalyst.Find<ModItem>("CoralCrusher").Type)
                    {
                        item.useTime = 32;
                        item.useAnimation = 32;
                        item.damage = 21;
                    }

                    //Congealed Duo-Whip
                    if (item.type == catalyst.Find<ModItem>("CongeledDuoWhip").Type)
                    {
                        item.useTime = 36;
                        item.useAnimation = 36;
                    }

                    if (GetItem(catalyst, "BlossomsBlessing", item))
                    {
                        item.useTime = 28;
                        item.useAnimation = 28;
                    }

                    if (GetItem(catalyst, "AstralpodStaff", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(catalyst, "Catharsis", item))
                    {
                        item.damage = 220;
                    }
                    #endregion

                    #region Rogue
                    if (GetItem(catalyst, "InterstellarVolution", item))
                    {
                        item.damage = 110;
                    }
                    #endregion

                    #region Armor
                    if (GetItem(catalyst, "IntergelacticBreastplate", item))
                    {
                        item.defense = 20;
                    }

                    if (GetItem(catalyst, "IntergelacticGreaves", item))
                    {
                        item.defense = 12;
                    }

                    if (GetItem(catalyst, "IntergelacticHeadMagic", item))
                    {
                        item.defense = 14;
                    }

                    if (GetItem(catalyst, "IntergelacticHeadMelee", item))
                    {
                        item.defense = 44;
                    }

                    if (GetItem(catalyst, "IntergelacticHeadRanged", item))
                    {
                        item.defense = 14;
                    }

                    if (GetItem(catalyst, "IntergelacticHeadRogue", item))
                    {
                        item.defense = 34;
                    }

                    if (GetItem(catalyst, "IntergelacticHeadSummon", item))
                    {
                        item.defense = 6;
                    }
                    #endregion
                }
            }
            #endregion

            #region Calamity: Hunt of the Old God
            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                /*
                if (InfernalConfig.Instance.CalamityBalanceChanges)
                {
                    #region Melee
                    if (GetItem(calHunt, "Parasanguine", item))
                    {
                        item.useTime = 22;
                        item.useAnimation = 22;
                    }

                    if (GetItem(calHunt, "ScytheOfTheOldGod", item))
                    {
                        item.damage = 2700;
                    }
                    #endregion

                    #region Ranged
                    if (GetItem(calHunt, "SludgeShaker", item))
                    {
                        item.damage = 2800;
                    }

                    if (GetItem(calHunt, "Trailblazer", item))
                    {
                        item.damage = 1050;
                    }
                    #endregion

                    #region Mage
                    if (GetItem(calHunt, "CrystalGauntlets", item))
                    {
                        item.damage = 900;
                    }
                    #endregion

                    #region Summoner
                    if (GetItem(calHunt, "SlimeCane", item))
                    {
                        item.damage = 650;
                    }
                    #endregion

                    #region Rogue
                    if (GetItem(calHunt, "FissionFlyer", item))
                    {
                        item.damage = 1000;
                    }
                    #endregion
                }
                */

                /* Implemented by hunt after cal 2.1
                if (InfernalConfig.Instance.CalamityExpertAccessories)
                {
                    if (GetItem(calHunt, "SplendorJam", item))
                    {
                        item.expert = true;
                    }

                    if (GetItem(calHunt, "TendrilCursorAttachment", item))
                    {
                        item.master = true;
                    }
                }
                */
            }
            #endregion

            #region Draedon's Expansion
            if (ModLoader.TryGetMod("DraedonExpansion", out Mod draedonExpansion) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (UnsafeGetItem(draedonExpansion, "StunGun", item))
                {
                    item.damage = 19;
                }

                if (UnsafeGetItem(draedonExpansion, "EnergyPistol", item))
                {
                    item.useTime = 24;
                    item.useAnimation = 24;
                    item.damage = 16;
                }
            }
            #endregion

            #region War Machine Invasion
            if (ModLoader.TryGetMod("CalamityAddon", out Mod warMachineInvasion) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (UnsafeGetItem(warMachineInvasion, "WulfrumLuncher", item))
                {
                    item.damage = 20;
                }

                if (UnsafeGetItem(warMachineInvasion, "WulfrumBook", item))
                {
                    item.damage = 48;
                }
            }
            #endregion

            #region Calamity Overdrive
            if (ModLoader.TryGetMod("WulfrumExpansion", out Mod overdrive) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (UnsafeGetItem(overdrive, "WulfrumSapper", item))
                {
                    item.damage = 18;
                }
                /*
                if (UnsafeGetItem(overdrive, "ArcScepterBeam", item))
                {
                    item.damage = 48;
                }
                */
            }
            #endregion

            #region Calamity Simple Whip Addon
            if (ModLoader.TryGetMod("CalamitySimpleWhipAddon", out Mod simpleWhipAddon) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                //Pre-Hardmode
                if (UnsafeGetItem(simpleWhipAddon, "RapierWhip", item))
                {
                    item.damage = 9;
                    item.useAnimation = 30;
                    item.useTime = 30;
                }
                if (UnsafeGetItem(simpleWhipAddon, "MandibleLash", item))
                {
                    item.rare = ItemRarityID.Blue;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Droptide", item))
                {
                    item.damage = 28;
                }
                if (UnsafeGetItem(simpleWhipAddon, "DarkMandibleLash", item))
                {
                    item.damage = 20;
                }
                if (UnsafeGetItem(simpleWhipAddon, "BreezePiercer", item))
                {
                    item.damage = 21;
                }
                if (UnsafeGetItem(simpleWhipAddon, "AncientBonds", item))
                {
                    item.useAnimation = 42;
                    item.useTime = 42;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Gelxyribose", item))
                {
                    item.useAnimation = 40;
                    item.useTime = 40;
                }

                //Hardmode
                if (UnsafeGetItem(simpleWhipAddon, "NightButterfly", item))
                {
                    item.useAnimation = 36;
                    item.useTime = 36;
                }
                if (UnsafeGetItem(simpleWhipAddon, "GlitterGutter", item))
                {
                    item.useAnimation = 19;
                    item.useTime = 19;
                }
                if (UnsafeGetItem(simpleWhipAddon, "ActiasAliena", item))
                {
                    item.useAnimation = 42;
                    item.useTime = 42;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Loadout", item))
                {
                    item.useAnimation = 36;
                    item.useTime = 36;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Ectopia", item))
                {
                    item.useAnimation = 18;
                    item.useTime = 18;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Unfathomable", item))
                {
                    item.damage = 250;
                }
                if (UnsafeGetItem(simpleWhipAddon, "DestructionChain", item))
                {
                    item.useAnimation = 32;
                    item.useTime = 32;
                }
                if (UnsafeGetItem(simpleWhipAddon, "GoldRush", item))
                {
                    item.useAnimation = 40;
                    item.useTime = 40;
                }
                if (UnsafeGetItem(simpleWhipAddon, "ExistenceBonds", item))
                {
                    item.useAnimation = 26;
                    item.useTime = 26;
                }

                //Post Moonlord
                if (UnsafeGetItem(simpleWhipAddon, "ButterflyEffect", item))
                {
                    item.useAnimation = 42;
                    item.useTime = 42;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Satellite", item))
                {
                    item.useAnimation = 12;
                    item.useTime = 12;
                    item.damage = 120;
                }
                if (UnsafeGetItem(simpleWhipAddon, "EntwinedBranches", item))
                {
                    item.damage = 270;
                }
                if (UnsafeGetItem(simpleWhipAddon, "ShieldConduitMkIV", item))
                {
                    item.useAnimation = 40;
                    item.useTime = 40;
                    item.damage = 200;
                }
                if (UnsafeGetItem(simpleWhipAddon, "GildedReliquary", item))
                {
                    item.useAnimation = 44;
                    item.useTime = 44;
                    item.damage = 216;
                }
                if (UnsafeGetItem(simpleWhipAddon, "EtaCarinae", item))
                {
                    item.useAnimation = 36;
                    item.useTime = 36;
                }
                if (UnsafeGetItem(simpleWhipAddon, "ResonantVoid", item))
                {
                    item.useAnimation = 31;
                    item.useTime = 31;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Milkyway", item))
                {
                    item.useAnimation = 36;
                    item.useTime = 36;
                }

                //Added in updates
                if (UnsafeGetItem(simpleWhipAddon, "KingsMajesty", item))
                {
                    item.damage = 18;
                }
                if (UnsafeGetItem(simpleWhipAddon, "EmeraldSplash", item))
                {
                    item.damage = 100;
                }
                if (UnsafeGetItem(simpleWhipAddon, "WulfrumArm", item))
                {
                    item.useAnimation = 60;
                    item.useTime = 60;
                }
                if (UnsafeGetItem(simpleWhipAddon, "Rubellus", item))
                {
                    item.rare = ModContent.RarityType<PureGreen>();
                    item.damage = 130;
                    item.useAnimation = 40;
                    item.useTime = 40;
                }
                if (UnsafeGetItem(simpleWhipAddon, "MassofWailing", item))
                {
                    item.damage = 96;
                }

                int[] bleachedAcessories =
                {
                    FindItem(simpleWhipAddon, "BleachedNucleogenesis"),
                    FindItem(simpleWhipAddon, "BleachedStatisCurse"),
                    FindItem(simpleWhipAddon, "BleachedStarTaintedGenerator"),
                    FindItem(simpleWhipAddon, "BleachedStarbusterCore"),
                    FindItem(simpleWhipAddon, "BleachedNuclearFuelRod"),
                    FindItem(simpleWhipAddon, "BleachedTheFirstShadowflame"),
                    FindItem(simpleWhipAddon, "BleachedJellyChargedBattery"),
                    FindItem(simpleWhipAddon, "BleachedVoltaicJelly"),
                };

                foreach (int bleachedItem in bleachedAcessories)
                {
                    if (item.type == bleachedItem)
                        item.accessory = false;
                }

                /*
                if (UnsafeGetItem(simpleWhipAddon, "BuddyEmblem", item))
                {
                    item.rare = ItemRarityID.LightRed;
                    item.expert = false;

                    item.accessory = false; //temp disabled
                }

                if (UnsafeGetItem(simpleWhipAddon, "StrikerEmblem", item))
                {
                    item.rare = ItemRarityID.LightRed;
                    item.expert = false;
                }
                */
            }
            #endregion

            #region Calamity Ranger Expansion
            if (ModLoader.TryGetMod("CalamityAmmo", out Mod calAmmo) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (UnsafeGetItem(calAmmo, "SandWorm", item))
                {
                    item.damage = 16;
                    item.useTime = 22;
                    item.useAnimation = 22;
                }

                if (UnsafeGetItem(calAmmo, "SpectreRifle", item))
                {
                    item.damage = 117;
                }

                if (UnsafeGetItem(calAmmo, "PlaguenadeLauncher", item))
                {
                    item.damage = 30;
                }

                //Bullets
                if (UnsafeGetItem(calAmmo, "WulfrumBullet", item))
                {
                    item.damage = 4;
                }

                if (UnsafeGetItem(calAmmo, "VictideBullet", item))
                {
                    item.damage = 7;
                }

                if (UnsafeGetItem(calAmmo, "PearlBullet", item))
                {
                    item.damage = 6;
                }

                if (UnsafeGetItem(calAmmo, "SPBullet", item))
                {
                    item.damage = 8;
                }

                if (UnsafeGetItem(calAmmo, "BloodBullet", item))
                {
                    item.damage = 3;
                }

                if (UnsafeGetItem(calAmmo, "RottenBullet", item))
                {
                    item.damage = 13;
                }

                if (UnsafeGetItem(calAmmo, "ElementalBullet", item))
                {
                    item.damage = 13;
                }

                if (UnsafeGetItem(calAmmo, "HydrothermicBullet", item))
                {
                    item.damage = 3;
                }

                if (UnsafeGetItem(calAmmo, "FossilBullet", item))
                {
                    item.damage = 10;
                }

                if (UnsafeGetItem(calAmmo, "WeakAstralBullet", item))
                {
                    item.damage = 7;
                }

                if (UnsafeGetItem(calAmmo, "AstralBullet", item))
                {
                    item.damage = 7;
                }

                if (UnsafeGetItem(calAmmo, "DazzlingAstralBullet", item))
                {
                    item.damage = 1;
                }

                if (UnsafeGetItem(calAmmo, "NapalmBullet", item))
                {
                    item.damage = 9;
                }

                //Arrows
                if (UnsafeGetItem(calAmmo, "WulfrumArrow", item))
                {
                    item.damage = 10;
                }

                if (UnsafeGetItem(calAmmo, "DesertFeatherArrow", item))
                {
                    item.damage = 9;
                }

                if (UnsafeGetItem(calAmmo, "GoldenFeatherArrow", item))
                {
                    item.damage = 9;
                }

                if (UnsafeGetItem(calAmmo, "DivineArrow", item))
                {
                    item.damage = 30;
                }

                if (UnsafeGetItem(calAmmo, "FossilArrow", item))
                {
                    item.damage = 8;
                }

                if (UnsafeGetItem(calAmmo, "HydrothermicArrow", item))
                {
                    item.damage = 3;
                }

                if (UnsafeGetItem(calAmmo, "AstralArrow", item))
                {
                    item.damage = 10;
                }

                if (UnsafeGetItem(calAmmo, "DazzlingAstralArrow", item))
                {
                    item.damage = 1;
                }

                if (UnsafeGetItem(calAmmo, "MushroomUnitedNations", item))
                {
                    item.rare = item.rare = ItemRarityID.Purple;
                }

                if (UnsafeGetItem(calAmmo, "AutoCalculationCoil", item))
                {
                    item.rare = ModContent.RarityType<ExoticRainbow>();
                }
            }
            #endregion

            #region Clamity
            if (ModLoader.TryGetMod("Clamity", out Mod clam) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                /*
                if (UnsafeGetItem(clam, "MoonstoneKnives", item))
                {
                    item.damage = 8;
                }

                if (UnsafeGetItem(clam, "Obsidigun", item))
                {
                    item.useTime = 18;
                    item.useAnimation = 18;
                }

                if (UnsafeGetItem(clam, "TheGenerator", item))
                {
                    item.damage = 77;
                }

                if (UnsafeGetItem(clam, "MoltenPiercer", item))
                {
                    item.damage = 144;
                    item.useTime = 16;
                    item.useAnimation = 16;
                }

                if (UnsafeGetItem(clam, "ClamitasCrusher", item))
                {
                    item.damage = 130;
                }

                if (UnsafeGetItem(clam, "DepthsEchoRifle", item))
                {
                    item.damage = 40;
                }

                if (UnsafeGetItem(clam, "HellstoneShellfishStaff", item))
                {
                    item.damage = 200;
                }

                if (UnsafeGetItem(clam, "Disease", item))
                {
                    item.damage = 194;
                }

                if (UnsafeGetItem(clam, "PlanterrorStaff", item))
                {
                    item.damage = 100;
                }

                if (UnsafeGetItem(clam, "RoseBow", item))
                {
                    item.damage = 400;
                }

                if (UnsafeGetItem(clam, "AuricKunai", item))
                {
                    item.damage = 140;
                }

                if (UnsafeGetItem(clam, "Omega", item))
                {
                    item.damage = 80;
                }

                if (UnsafeGetItem(clam, "Everest", item))
                {
                    item.damage = 1300;
                }

                if (UnsafeGetItem(clam, "FrozenVolcano", item))
                {
                    item.damage = 2500;
                    item.useTime = 12;
                    item.useAnimation = 12;
                }

                if (UnsafeGetItem(clam, "FrozenStarShuriken", item))
                {
                    item.useAnimation = 8;
                    item.useTime = 8;
                    item.damage = 900;
                }

                if (UnsafeGetItem(clam, "WulfrumLeechDagger", item))
                {
                    item.value = 0;
                }
                */

                if (UnsafeGetItem(clam, "HellFlare", item))
                {
                    item.expert = true;
                    item.rare = ItemRarityID.Expert;
                }

                if (UnsafeGetItem(clam, "WitheredBoneBow", item))
                {
                    item.damage = 70;
                }
            }
            #endregion

            #region Hypnos
            if (InfernalConfig.Instance.CalamityBalanceChanges && ModLoader.TryGetMod("HypnosMod", out Mod hypnos))
            {
                if (item.type == hypnos.Find<ModItem>("Neuraze").Type)
                {
                    item.damage = 100;
                    item.useAnimation = 6;
                    item.useTime = 6;
                }

                if (item.type == hypnos.Find<ModItem>("Planetarium").Type)
                {
                    item.damage = 400;
                }

                if (item.type == hypnos.Find<ModItem>("AergianTechnistaff").Type)
                {
                    item.damage = 87;
                }
            }
            #endregion

            #region Miscellanaria
            if (InfernalConfig.Instance.VanillaBalanceChanges && ModLoader.TryGetMod("Miscellanaria", out Mod misc))
            {
                if (item.type == ItemID.SkeletonBow)
                {
                    item.useTime = 24;
                    item.useAnimation = 24;
                }

                if (item.type == misc.Find<ModItem>("Icemourne").Type)
                {
                    item.damage = 52;
                    item.useTime = 17;
                    item.scale = 1.25f;
                }

                if (item.type == misc.Find<ModItem>("Scythe").Type)
                {

                }

                if (item.type == misc.Find<ModItem>("SoulScythe").Type)
                {
                    item.scale = 1.25f;
                }
            }
            #endregion

            if (ModLoader.TryGetMod("NoxusBoss", out Mod wrath))
            {
                if (UnsafeGetItem(wrath, "MetallicChunk", item))
                {
                    item.maxStack = Item.CommonMaxStack;
                }
            }

            if (ModLoader.TryGetMod("CalValEX", out Mod calVal))
            {
                if (UnsafeGetItem(calVal, "ScryllianWings", item))
                {
                    ArmorIDs.Wing.Sets.Stats[item.wingSlot] = new Terraria.DataStructures.WingStats(25);
                }
            }

            #region Thorium
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                if (GetItem(thorium, "EnchantedPickaxe", item))
                {
                    item.pick = 60;
                }

                if (GetItem(thorium, "DragonsPickAxe", item) || GetItem(thorium, "DragonDrill", item) || GetItem(thorium, "FleshPickAxe", item) || GetItem(thorium, "FleshDrill", item))
                {
                    item.pick = 115;
                }

                if (GetItem(thorium, "ValadiumPickaxe", item) || GetItem(thorium, "LodeStonePickaxe", item))
                {
                    item.pick = 190;
                }

                if (GetItem(thorium, "DreadDrill", item) || GetItem(thorium, "DemonBloodDrill", item))
                {
                    item.pick = 200;
                }

                if (InfernalConfig.Instance.ThoriumBalanceChangess)
                {
                    if (UnsafeGetItem(thorium, "WaterwickCandle", item))
                    {
                        item.useTime = 15;
                        item.useAnimation = 15;
                    }

                    #region Melee
                    #region Pre-Hardmode
                    //The Snowball
                    if (item.type == thorium.Find<ModItem>("TheSnowball").Type)
                    {
                        item.knockBack = 6;
                        item.damage = 10;
                    }

                    //Ice Breaker
                    if (item.type == thorium.Find<ModItem>("IceBreaker").Type)
                    {
                        item.damage = 11;
                    }

                    //Cold Front
                    if (item.type == thorium.Find<ModItem>("ColdFront").Type)
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 18;
                    }

                    //Bent Zombie Arm
                    if (item.type == thorium.Find<ModItem>("BentZombieArm").Type)
                    {
                        //projectile speed = 15
                        item.damage = 21;
                    }

                    //Coral Slasher
                    if (item.type == thorium.Find<ModItem>("CoralSlasher").Type)
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;

                        item.scale *= 1.5f;
                    }

                    //Coral Polearm
                    if (item.type == FindItem(thorium, "CoralPolearm"))
                    {
                        item.useTime = 21;
                        item.useAnimation = 21;
                        //projectile speed = 5
                    }

                    //Heartstriker
                    if (item.type == FindItem(thorium, "Heartstriker"))
                    {
                        item.useTime = 30;
                        item.damage = 24;
                        item.useAnimation = 30;
                    }

                    //Thorium Blade
                    if (item.type == FindItem(thorium, "ThoriumBlade"))
                    {
                        item.damage = 26;
                        item.useTime = 17;
                        item.useAnimation = 17;
                    }

                    //Thorium Boomerang
                    if (item.type == FindItem(thorium, "ThoriumBoomerang"))
                    {
                        item.damage *= 2;
                        item.useTime = 22;
                        item.useAnimation = 22;
                    }

                    //Thorium Spear
                    if (item.type == FindItem(thorium, "ThoriumSpear"))
                    {
                        item.damage = 16;
                        item.shootSpeed = 3.5f;
                        item.useTime = item.useAnimation = 31;
                    }

                    //Sandstorm Scimitar
                    if (item.type == FindItem(thorium, "dSandStoneScimtar"))
                    {
                        item.damage = 20;
                    }

                    //Sandstone Spear
                    if (item.type == FindItem(thorium, "fSandStoneSpear"))
                    {
                        item.damage = 21;
                        item.shootSpeed = 6;
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }

                    //Thunder Talon
                    if (item.type == FindItem(thorium, "ThunderTalon"))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    //Spoon
                    if (item.type == FindItem(thorium, "Spoon"))
                    {
                        item.damage = 48;

                        item.scale *= 2;
                    }

                    //Kitchen Knife
                    if (item.type == FindItem(thorium, "Knife"))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    //Fork
                    if (item.type == FindItem(thorium, "Fork"))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.damage = 23;
                        item.shootSpeed = 8;
                    }

                    //Whip
                    if (item.type == FindItem(thorium, "Whip"))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.knockBack = (float)1.5;
                    }

                    //Steel Blade
                    if (item.type == FindItem(thorium, "SteelBlade"))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.damage = 30;
                    }

                    /*
                    //Harpy Talon
                    if (item.type == FindItem(thorium, "HarpyTalon"))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.shootSpeed = 12;
                    }
                    */

                    //Bellerose
                    if (item.type == FindItem(thorium, "Bellerose"))
                    {
                        item.useTime = 17;
                        item.useAnimation = 17;
                    }

                    //Blooming BLade
                    if (item.type == FindItem(thorium, "BloomingBlade"))
                    {
                        item.damage = 45;
                        item.useTime = 26;
                        item.useAnimation = 26;

                        item.scale *= 1.5f;
                    }

                    //Pearl Pike
                    if (item.type == FindItem(thorium, "PearlPike"))
                    {
                        item.useTime = 22;
                        item.useAnimation = 22;
                        item.shootSpeed = 8;
                    }

                    //Drenched Dirk
                    if (item.type == FindItem(thorium, "DrenchedDirk"))
                    {
                        item.useTime = 6;
                        item.useAnimation = 6;
                        item.damage = 14;
                        item.shootSpeed = 18;
                    }

                    //Illustrious
                    if (item.type == FindItem(thorium, "Illustrious"))
                    {
                        item.damage = 30;
                    }

                    //Thor's Hammer: Melee
                    if (item.type == FindItem(thorium, "MeleeThorHammer"))
                    {
                        item.damage = 45;
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }

                    //Giant Glowstick
                    if (item.type == FindItem(thorium, "GiantGlowstick"))
                    {
                        item.shootSpeed = 14;
                    }

                    //Sparking Jelly Ball
                    if (item.type == FindItem(thorium, "SparkingJellyBall"))
                    {
                        item.damage = 60;
                    }

                    //Pollen Pike
                    if (item.type == FindItem(thorium, "PollenPike"))
                    {
                        item.damage = 38;
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.shootSpeed = 12;
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
                    }

                    //Whirlpool Saber
                    if (item.type == FindItem(thorium, "WhirlpoolSaber"))
                    {
                        item.shootSpeed = 10;
                    }

                    //Moonlight
                    if (item.type == FindItem(thorium, "Moonlight"))
                    {
                        item.useTime = 20;
                        item.useAnimation = 20;
                        item.shootSpeed = (float)7.5;
                        item.damage = 47;
                    }

                    //Darksteel Broadsword
                    if (item.type == FindItem(thorium, "eDarksteelBroadSword"))
                    {
                        item.useTime = 17;
                        item.useAnimation = 17;
                        item.damage = 40;
                    }

                    //Champion's Swift Blade
                    if (item.type == thorium.Find<ModItem>("ChampionSwiftBlade").Type)
                    {
                        item.DamageType = DamageClass.Melee;
                        item.damage = 50;
                    }

                    //Gorgon's Eye
                    if (item.type == thorium.Find<ModItem>("GorgonsEye").Type)
                    {
                        item.damage = 35;
                    }

                    //Granite Reflector
                    if (item.type == FindItem(thorium, "GraniteReflector"))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.shootSpeed = 14;
                    }

                    //Star Trail
                    if (item.type == thorium.Find<ModItem>("StarTrail").Type)
                    {
                        item.damage = 35;
                    }
                    #endregion

                    #region Hardmode
                    if (UnsafeGetItem(thorium, "ToothOfTheConsumer", item))
                    {
                        item.crit = 0;
                        item.useTime = 35;
                        item.useAnimation = 35;
                    }

                    if (UnsafeGetItem(thorium, "EssenceofFlame", item))
                    {
                        item.damage = 300;
                    }

                    //Durasteel Blade
                    if (item.type == FindItem(thorium, "DurasteelBlade"))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    if (GetItem(thorium, "GrimFlayer", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    if (GetItem(thorium, "BloodDrinker", item))
                    {
                        item.useTime = 13;
                        item.useAnimation = 13;

                        item.scale *= 1.25f;
                    }

                    if (GetItem(thorium, "Saba", item))
                    {
                        item.useAnimation = 15;
                        item.useTime = 30;

                    }

                    if (GetItem(thorium, "DragonTooth", item))
                    {
                        item.crit = 0;
                        item.damage = 94;
                    }

                    if (GetItem(thorium, "DoomFireAxe", item))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                    }

                    if (GetItem(thorium, "Scalper", item))
                    {
                        item.shootSpeed = 16;
                    }
                    
                    if (GetItem(thorium, "Rapier", item) && !InfernalCrossmod.ThoriumRework.Loaded)
                    {
                        item.useTime = 8;
                        item.useAnimation = 8;
                        item.shootSpeed = 8;
                    }

                    if (GetItem(thorium, "BackStabber", item))
                    {
                        item.useTime = 8;
                        item.useAnimation = 8;
                    }

                    if (GetItem(thorium, "RifleSpear", item))
                    {
                        item.useTime = 10;
                        item.useAnimation = 10;
                    }

                    if (GetItem(thorium, "PoseidonCharge", item))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.shootSpeed = 12;
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
                    }

                    if (GetItem(thorium, "DragonTalon", item))
                    {
                        item.crit = 0;
                        item.damage = 54;
                    }

                    if (GetItem(thorium, "Schmelze", item))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.damage = 90;
                    }

                    if (GetItem(thorium, "Blitzzard", item))
                    {
                        item.useTime = 10;
                        item.useAnimation = 10;
                        item.damage = 60;
                    }

                    if (GetItem(thorium, "Glacier", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                        item.damage = 80;

                        item.scale *= 1.5f;
                    }

                    if (GetItem(thorium, "Executioner", item))
                    {
                        item.scale *= 1.25f;
                    }

                    if (GetItem(thorium, "ShipsHelm", item))
                    {
                        item.useAnimation = 8;
                        item.useTime = 8;
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "TheJuggernaut", item))
                    {
                        item.crit = 26;
                    }

                    if (GetItem(thorium, "MidasGavel", item))
                    {
                        item.damage = 200;
                    }

                    if (GetItem(thorium, "LodeStoneClaymore", item))
                    {
                        item.damage = 75;
                    }

                    if (GetItem(thorium, "ValadiumSlicer", item))
                    {
                        item.damage = 131;
                    }

                    if (GetItem(thorium, "TheSeaMine", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "BloodyHighClaws", item))
                    {
                        item.crit = 16;
                        item.damage = 73;
                    }

                    if (GetItem(thorium, "PrimesFury", item))
                    {
                        item.damage = 200;
                    }

                    if (GetItem(thorium, "EyeoftheDestroyer", item))
                    {
                        item.damage = 110;
                    }

                    if (GetItem(thorium, "ClimbersIceAxe", item))
                    {
                        item.damage = 140;
                    }

                    if (GetItem(thorium, "TitanBoomerang", item))
                    {
                        item.damage = 36;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "TitanSword", item))
                    {
                        item.damage = 88;
                    }

                    if (GetItem(thorium, "GoldenLocks", item))
                    {
                        item.useTime = 4;
                        item.useAnimation = 4;
                        item.damage = 39;
                    }

                    if (GetItem(thorium, "SoulRender", item))
                    {
                        item.useTime = 20;
                        item.useAnimation = 20;
                        item.damage = 200;
                    }

                    if (GetItem(thorium, "DreadRazor", item))
                    {
                        item.useTime = 20;
                        item.useAnimation = 20;
                        item.damage = 144;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "DemonBloodSword", item))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.crit = 0;
                        item.damage = 60;
                    }

                    if (GetItem(thorium, "IllumiteBlade", item))
                    {
                        item.crit = 0;
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "SolScorchedSlab", item))
                    {
                        item.damage = 158;

                        item.scale *= 1.5f;
                    }

                    if (GetItem(thorium, "TheBlackBlade", item))
                    {
                        item.shootSpeed = 20;
                    }

                    if (GetItem(thorium, "DreadFork", item))
                    {
                        item.crit = 0;
                        item.damage = 130;
                    }

                    if (GetItem(thorium, "DemonBloodSpear", item))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.crit = 0;
                        item.damage = 100;
                    }

                    if (GetItem(thorium, "IllumiteSpear", item))
                    {
                        item.damage = 46;
                    }

                    if (GetItem(thorium, "Spearmint", item))
                    {
                        item.damage = 108;
                    }

                    if (GetItem(thorium, "SoulReaver", item))
                    {
                        item.useTime = 10;
                        item.useAnimation = 10;
                        item.damage = 98;
                    }

                    if (GetItem(thorium, "LingeringWill", item))
                    {
                        item.useTime = 24;
                        item.useAnimation = 24;
                        item.damage = 265;
                        item.shootSpeed = 20;
                    }

                    if (GetItem(thorium, "WyvernSlayer", item))
                    {
                        //item.damage = 254;
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
                    }

                    if (GetItem(thorium, "GolemsGaze", item))
                    {
                        item.damage = 192;
                    }

                    if (GetItem(thorium, "LivewireCrasher", item))
                    {
                        item.damage = 87;
                    }

                    if (GetItem(thorium, "TheWhirlpool", item))
                    {
                        item.damage = 184;
                    }

                    if (GetItem(thorium, "MantisShrimpPunch", item))
                    {
                        item.damage = 264;
                    }

                    /*
                    if (GetItem(thorium, "EclipseFang", item))
                    {
                        item.damage = 175;

                        item.scale *= 1.15f;
                    }
                    */

                    if (GetItem(thorium, "TerrariumSaber", item))
                    {
                        item.damage = 140;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "Skadoosh", item))
                    {
                        item.damage = 53;
                    }

                    if (GetItem(thorium, "TerrariumHyperDisc", item))
                    {
                        item.shootSpeed = 14;
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 134;
                    }

                    if (GetItem(thorium, "BloodGlory", item))
                    {
                        item.damage = 400;
                    }

                    if (GetItem(thorium, "QuakeGauntlet", item))
                    {
                        item.useTime = 3;
                        item.useAnimation = 3;
                        item.shootSpeed = 20;
                        item.damage = 147;
                    }

                    if (GetItem(thorium, "SevenSeasDevastator", item))
                    {
                        item.damage = 300;
                    }

                    if (GetItem(thorium, "OceansJudgement", item))
                    {
                        item.damage = 600;
                        item.shootSpeed = 25;
                    }

                    if (GetItem(thorium, "TerrariansLastKnife", item))
                    {
                        item.damage = 900;
                    }

                    #endregion
                    #endregion

                    #region Ranged
                    #region Pre-Hardmode
                    //Champions Trifecta Shot
                    if (item.type == thorium.Find<ModItem>("ChampionsTrifectaShot").Type)
                    {
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }

                    //Frost Fury
                    if (item.type == thorium.Find<ModItem>("FrostFury").Type)
                    {
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }

                    //Frost Pelter
                    if (UnsafeGetItem(thorium, "FrostPelter", item))
                    {
                        item.shootSpeed = 20;
                    }

                    //Coral Crossbow
                    if (item.type == thorium.Find<ModItem>("CoralCrossbow").Type)
                    {
                        item.useTime = 26;
                        item.useAnimation = 26;
                        item.damage = 10;
                    }

                    //Thorium Revolver
                    if (item.type == thorium.Find<ModItem>("ThoriumRevolver").Type)
                    {
                        item.damage = 17;
                    }

                    //Talon Burst
                    if (item.type == thorium.Find<ModItem>("TalonBurst").Type)
                    {
                        item.damage = 9;
                    }

                    //Feather Foe
                    if (item.type == thorium.Find<ModItem>("FeatherFoe").Type)
                    {
                        item.damage = 22;
                    }

                    //Blooming Bow
                    if (item.type == thorium.Find<ModItem>("BloomingBow").Type)
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    //Harpy Pelter
                    if (item.type == thorium.Find<ModItem>("HarpyPelter").Type)
                    {
                        item.damage = 15;
                    }

                    //Spud Bomber
                    if (item.type == thorium.Find<ModItem>("SpudBomber").Type)
                    {
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }

                    //Golbin Warpipe
                    if (item.type == thorium.Find<ModItem>("GoblinWarpipe").Type)
                    {
                        item.useAnimation = 20;
                        item.useTime = 20;
                    }

                    //Aqua Pelter
                    if (item.type == thorium.Find<ModItem>("AquaPelter").Type)
                    {
                        item.useTime = 6;
                        item.useAnimation = 6;
                        item.damage = 12;
                    }

                    //Thor's Hammer: Ranged
                    if (item.type == thorium.Find<ModItem>("RangedThorHammer").Type)
                    {
                        item.damage = 40;
                    }

                    //Guano Gunner
                    if (item.type == thorium.Find<ModItem>("GuanoGunner").Type)
                    {
                        item.damage = 24;
                    }

                    //Stream Sting
                    if (item.type == thorium.Find<ModItem>("StreamSting").Type)
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 16;
                    }

                    //Darksteel Crossbow
                    if (item.type == thorium.Find<ModItem>("gDarkSteelCrossBow").Type)
                    {
                        item.damage = 56;
                    }

                    //Elephant Gun
                    if (item.type == thorium.Find<ModItem>("ElephantGun").Type)
                    {
                        item.damage = 45;
                    }

                    //Champion's Trifecta-Shot
                    if (item.type == thorium.Find<ModItem>("ChampionsTrifectaShot").Type)
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    //Granite Crossbow
                    if (item.type == thorium.Find<ModItem>("GraniteCrossbow").Type)
                    {
                        item.useAnimation = 20;
                        item.useTime = 20;
                    }

                    //Energy Storm Bolter
                    if (item.type == thorium.Find<ModItem>("EnergyStormBolter").Type)
                    {
                        item.damage = 26;
                    }

                    //Hit Scanner
                    if (item.type == thorium.Find<ModItem>("HitScanner").Type)
                    {
                        item.damage = 30;
                        item.useAnimation = 16;
                        item.useTime = 16;
                    }

                    if (item.type == thorium.Find<ModItem>("BuccaneerBlunderBuss").Type)
                    {
                        item.damage = 26;
                        item.useAnimation = 30;
                        item.useTime = 30;
                    }
                    #endregion

                    #region Hardmode
                    if (GetItem(thorium, "DurasteelRepeater", item))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.damage = 45;
                    }

                    if (GetItem(thorium, "CometCrossfire", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                        item.crit = 16;
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "CupidString", item))
                    {
                        item.crit = 26;
                    }

                    if (GetItem(thorium, "NagaRecurve", item))
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                    }

                    if (GetItem(thorium, "FleshBow", item))
                    {
                        item.useTime = 18;
                        item.useAnimation = 18;
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "CinderString", item))
                    {
                        item.damage = 50;
                    }

                    if (GetItem(thorium, "SpineBuster", item))
                    {
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }

                    if (GetItem(thorium, "DragonsGaze", item))
                    {
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "BloodBelcher", item))
                    {
                        item.shootSpeed = 20;
                        item.damage = 70;
                    }

                    if (GetItem(thorium, "Funggat", item))
                    {
                        item.damage = 36;
                    }

                    if (GetItem(thorium, "VegaPhaser", item))
                    {
                        item.useTime = 28;
                        item.useAnimation = 28;
                        item.damage = 26;
                    }

                    /*
                    if (GetItem(thorium, "BulletStorm", item))
                    {
                        item.useTime = 7;
                        item.useAnimation = 7;
                    }
                    */

                    /*
                    if (GetItem(thorium, "ArmorBane", item))
                    {
                        item.damage = 40;
                    }
                    */

                    if (GetItem(thorium, "ShusWrath", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "TommyGun", item))
                    {
                        item.damage = 26;
                    }

                    if (GetItem(thorium, "FreezeRay", item))
                    {
                        item.damage = 65;
                    }

                    if (GetItem(thorium, "GlacialSting", item))
                    {
                        item.useTime = 18;
                        item.useAnimation = 18;
                        item.damage = 70;
                    }

                    if (GetItem(thorium, "HandCannon", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "Obliterator", item))
                    {
                        item.crit = 0;
                        item.damage = 30;
                    }

                    if (GetItem(thorium, "LodeStoneBow", item))
                    {
                        item.damage = 30;
                        item.useTime = 20;
                        item.useAnimation = 20;
                    }

                    if (GetItem(thorium, "LodeStoneQuickDraw", item))
                    {
                        //item.damage = 15;
                    }

                    if (GetItem(thorium, "ValadiumBow", item))
                    {
                        item.damage = 35;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "ValadiumFoeBlaster", item))
                    {
                        //item.damage = 26;
                        //item.crit = 0;
                    }

                    if (GetItem(thorium, "OrichalcumPelter", item))
                    {
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }

                    if (GetItem(thorium, "MythrilPelter", item))
                    {
                        item.damage = 50;
                    }

                    if (GetItem(thorium, "ChargedSplasher", item))
                    {
                        item.damage = 36;
                    }

                    if (GetItem(thorium, "AdamantiteCarbine", item))
                    {
                        item.damage = 114;
                    }

                    if (GetItem(thorium, "Trigun", item))
                    {
                        item.damage = 70;
                    }

                    if (GetItem(thorium, "Shockbuster", item))
                    {
                        item.damage = 44;
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    if (GetItem(thorium, "DestroyersRage", item))
                    {
                        item.useAnimation = 9;
                        item.useTime = 9;
                        item.damage = 50;
                    }

                    if (GetItem(thorium, "TitanBow", item))
                    {
                        item.damage = 60;
                    }

                    if (GetItem(thorium, "LittleRed", item))
                    {
                        item.damage = 148;
                        item.useTime = 37;
                        item.useAnimation = 37;
                    }

                    if (GetItem(thorium, "DecayingSorrow", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "DemonBloodCrossBow", item))
                    {
                        item.useTime = 3;
                        item.useAnimation = 6;
                    }

                    if (GetItem(thorium, "IllumiteShotbow", item))
                    {
                        item.crit = 0;
                        item.damage = 28;
                    }

                    if (GetItem(thorium, "IllumiteBlaster", item))
                    {
                        item.crit = 0;
                        item.damage = 25;
                    }

                    if (GetItem(thorium, "ShadowFlareBow", item))
                    {
                        item.damage = 146;
                    }

                    if (GetItem(thorium, "TheBlackBow", item))
                    {
                        item.useTime = 4;
                        item.useAnimation = 12;
                    }

                    if (GetItem(thorium, "UmbraBlaster", item))
                    {
                        item.damage = 35;
                    }

                    if (GetItem(thorium, "BeetleBlaster", item))
                    {
                        item.damage = 26;
                    }

                    if (GetItem(thorium, "IllumiteBarrage", item))
                    {
                        item.damage = 31;
                        item.crit = 0;
                        item.useTime = 12;
                        item.useAnimation = 36;
                    }

                    if (GetItem(thorium, "DreadLauncher", item))
                    {
                        item.damage = 93;
                    }

                    if (GetItem(thorium, "TheMassacre", item))
                    {
                        item.damage = 260;
                    }

                    if (GetItem(thorium, "SupersonicBomber", item))
                    {
                        item.damage = 88;
                    }

                    if (GetItem(thorium, "SpiritBreaker", item))
                    {
                        item.damage = 47;
                        item.crit = 4;
                        item.useTime = 5;
                        item.useAnimation = 20;
                    }

                    if (GetItem(thorium, "TrenchSpitter", item))
                    {
                        item.damage = 57;
                    }

                    if (GetItem(thorium, "TerrariumLongbow", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    if (GetItem(thorium, "TerrariumPulseRifle", item))
                    {
                        item.damage = 94;
                    }

                    if (GetItem(thorium, "WyrmDecimator", item))
                    {
                        item.damage = 300;
                    }

                    if (GetItem(thorium, "OmniBow", item))
                    {
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "OmniCannon", item))
                    {
                        item.damage = 370;
                    }

                    if (GetItem(thorium, "TheJavelin", item))
                    {
                        item.damage = 1350;
                    }

                    if (GetItem(thorium, "QuasarsFlare", item))
                    {
                        item.useTime = 11;
                        item.useAnimation = 11;
                        item.damage = 620;
                    }
                    #endregion
                    #endregion

                    #region Magic
                    #region Pre-Hardmode
                    //Ice Cube
                    if (item.type == thorium.Find<ModItem>("IceCube").Type)
                    {
                        item.mana = 2;
                        item.damage = 10;
                    }

                    //Flurry
                    if (item.type == thorium.Find<ModItem>("Flurry").Type)
                    {
                        item.mana = 2;
                        item.useTime = 8;
                        item.useAnimation = 8;
                    }

                    //Opal Staff
                    if (item.type == thorium.Find<ModItem>("OpalStaff").Type)
                    {
                        item.mana = 4;
                        item.damage = 27;
                        item.useTime = 36;
                        item.useAnimation = 36;
                    }

                    //Aquamarine Staff
                    if (item.type == thorium.Find<ModItem>("AquamarineStaff").Type)
                    {
                        item.mana = 4;
                        item.damage = 20;
                        item.useTime = 33;
                        item.useAnimation = 33;
                    }

                    //Vessel Buster
                    if (item.type == thorium.Find<ModItem>("VesselBuster").Type)
                    {
                        item.useTime = 10;
                        item.useAnimation = 10;
                    }

                    //Magic Conch
                    if (item.type == thorium.Find<ModItem>("MagicConch").Type)
                    {
                        item.mana = 10;
                        item.useTime = 2;
                        item.useAnimation = 10;
                        item.damage = 16;
                    }

                    //Detached Blaster
                    if (item.type == thorium.Find<ModItem>("DetachedBlaster").Type)
                    {
                        item.damage = 34;
                    }

                    //Shadoowflame Staff
                    if (item.type == thorium.Find<ModItem>("ShadowflameStaff").Type)
                    {
                        item.damage *= 2;
                    }

                    //Blooming Staff
                    if (item.type == thorium.Find<ModItem>("BloomingStaff").Type)
                    {
                        item.damage = 16;
                        item.useTime = 2;
                        item.useAnimation = 6;
                    }

                    //Thor's Hammer: Magic
                    if (item.type == thorium.Find<ModItem>("MagicThorHammer").Type)
                    {
                        item.damage = 44;
                    }

                    if (UnsafeGetItem(thorium, "InfernoStaff", item))
                    {
                        item.damage = 45;
                    }

                    //Dark Tome
                    if (item.type == thorium.Find<ModItem>("DarkTome").Type)
                    {
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }

                    //Jelly Pond Wand
                    if (item.type == thorium.Find<ModItem>("JellyPondWand").Type)
                    {
                        item.damage = 24;
                        item.shootSpeed = 10;
                    }

                    //Vampire Scepter
                    if (item.type == thorium.Find<ModItem>("VampireScepter").Type)
                    {
                        item.shootSpeed = 16;
                    }

                    //Phantom Camera
                    if (item.type == thorium.Find<ModItem>("PhantomCamera").Type)
                    {
                        item.damage = 85;
                    }

                    //Eel-rod
                    if (item.type == thorium.Find<ModItem>("Eelrod").Type)
                    {
                        item.damage = 24;
                    }

                    //Bloody Wand
                    if (item.type == thorium.Find<ModItem>("BloodyWand").Type)
                    {
                        item.damage = 15;
                    }

                    //Energy Projector
                    if (item.type == thorium.Find<ModItem>("EnergyProjector").Type)
                    {
                        item.damage = 30;
                    }

                    //Champion's Bomber Staff
                    if (item.type == thorium.Find<ModItem>("ChampionBomberStaff").Type)
                    {
                        item.damage = 40;
                    }

                    //Particle Whip
                    if (item.type == thorium.Find<ModItem>("ParticleWhip").Type)
                    {
                        item.damage = 38;
                    }
                    #endregion

                    #region Hardmode
                    if (GetItem(thorium, "NightStaff", item))
                    {
                        item.useTime = 17;
                        item.useAnimation = 17;
                    }

                    if (GetItem(thorium, "CobaltStaff", item))
                    {
                        item.shootSpeed = 13;
                        item.useTime = 8;
                        item.useAnimation = 8;
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "PalladiumStaff", item))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 48;
                    }

                    if (GetItem(thorium, "FrostPlagueStaff", item))
                    {
                        item.shootSpeed = 16;
                        item.damage = 28;
                    }

                    if (GetItem(thorium, "SeaFoamScepter", item))
                    {
                        item.shootSpeed = 12;
                        item.damage = 44;
                    }

                    if (GetItem(thorium, "PrismStaff", item))
                    {
                        item.damage = 55;
                    }

                    if (GetItem(thorium, "BloodClotStaff", item))
                    {
                        item.damage = 45;
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    if (GetItem(thorium, "Stalagmite", item))
                    {
                        item.damage = 58;
                    }

                    if (GetItem(thorium, "DragonsBreath", item))
                    {
                        item.shootSpeed = 18;
                        item.damage = 25;
                    }

                    if (GetItem(thorium, "Cyclone", item))
                    {
                        item.damage = 70;
                    }

                    if (GetItem(thorium, "DevilDagger", item))
                    {
                        item.shootSpeed = 20;
                        item.damage = 45;
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    if (GetItem(thorium, "GeomancersBrush", item))
                    {
                        item.damage = 90;
                    }

                    /*
                    if (GetItem(thorium, "DynastyWarFan", item))
                    {
                        item.shootSpeed = 15;
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }
                    */

                    if (GetItem(thorium, "KineticKnife", item))
                    {
                        item.damage = 41;
                    }

                    if (GetItem(thorium, "HeavensGate", item))
                    {
                        item.damage = 38;
                    }

                    if (GetItem(thorium, "DutchmansAvarice", item))
                    {
                        item.damage = 60;
                    }

                    //Lodestone Staff
                    if (GetItem(thorium, "LodeStoneStaff", item))
                    {
                        item.damage = 34;
                    }

                    if (GetItem(thorium, "ValadiumStaff", item))
                    {
                        item.useTime = 30;
                        item.useAnimation = 30;
                        item.damage = 44;
                    }

                    if (GetItem(thorium, "MythrilStaff", item))
                    {
                        item.damage = 56;
                    }

                    if (GetItem(thorium, "OrichalcumStaff", item))
                    {
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "TitaniumStaff", item))
                    {
                        item.damage = 60;
                    }

                    if (GetItem(thorium, "AdamantiteStaff", item))
                    {
                        item.damage = 50;
                        item.useAnimation = 22;
                        item.useTime = 22;
                    }

                    if (GetItem(thorium, "HallowedStaff", item))
                    {
                    }

                    if (GetItem(thorium, "StaticProd", item))
                    {
                        item.useTime = 4;
                    }

                    if (GetItem(thorium, "WondrousWand", item))
                    {
                        item.damage = 86;
                    }

                    if (GetItem(thorium, "ChlorophyteStaff", item))
                    {
                        item.damage = 51;
                    }

                    if (GetItem(thorium, "SnowWhite", item))
                    {
                        item.damage = 56;
                    }

                    if (GetItem(thorium, "BassBooster", item))
                    {
                        item.damage = 48;
                    }

                    if (GetItem(thorium, "DarkFlame", item))
                    {
                        item.damage = 120;
                    }

                    if (GetItem(thorium, "Razorlash", item))
                    {
                        item.damage = 68;
                    }

                    if (GetItem(thorium, "DemonBloodStaff", item))
                    {
                        item.damage = 62;
                        item.useTime = 10;
                        item.useAnimation = 10;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "DarkGrip", item))
                    {
                        item.damage = 100;
                    }

                    if (GetItem(thorium, "Legacy", item))
                    {
                        item.damage = 120;
                    }

                    if (GetItem(thorium, "LightningStaff", item))
                    {
                        item.damage = 145;
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }

                    if (GetItem(thorium, "EruptingFlare", item))
                    {
                        item.damage = 104;
                    }

                    if (GetItem(thorium, "GodKiller", item))
                    {
                        item.damage = 88;
                    }

                    if (GetItem(thorium, "DreadBlaster", item))
                    {
                        item.damage = 24;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "PLG", item))
                    {
                        item.damage = 55;
                    }

                    if (GetItem(thorium, "Spores", item))
                    {
                        item.useTime = 9;
                        item.useAnimation = 9;
                    }

                    if (GetItem(thorium, "CharonsBeacon", item))
                    {
                        item.damage = 110;
                    }

                    if (GetItem(thorium, "SuperPlasmaCannon", item))
                    {
                        item.damage = 50;
                    }

                    if (GetItem(thorium, "OldGodsVision", item))
                    {
                        item.damage = 45;
                    }

                    if (GetItem(thorium, "AncientLight", item))
                    {
                        item.damage = 110;
                    }

                    if (GetItem(thorium, "AncientSpark", item))
                    {
                        item.damage = 120;
                    }

                    if (UnsafeGetItem(thorium, "ChromaticFury", item))
                    {
                        item.damage = 30;
                    }

                    if (GetItem(thorium, "TerrariumSageStaff", item))
                    {
                        item.damage = 120;
                    }

                    if (GetItem(thorium, "CatsEyeGreatStaff", item))
                    {
                        item.damage = 210;
                    }

                    if (GetItem(thorium, "TeleologicImposition", item))
                    {
                        item.damage = 70;
                    }

                    if (GetItem(thorium, "AlmanacofAgony", item))
                    {
                        item.damage = 600;
                    }

                    if (GetItem(thorium, "DevilsClaw", item))
                    {
                        item.damage = 675;
                    }

                    if (GetItem(thorium, "NorthernLight", item))
                    {
                        item.damage = 300;
                    }
                    #endregion
                    #endregion

                    #region Summoner
                    #region Pre-Hardmode
                    //Storm Hatchling Staff
                    if (item.type == thorium.Find<ModItem>("StormHatchlingStaff").Type)
                    {
                        item.damage = 20;
                    }

                    //Meteor Head Staff
                    if (item.type == thorium.Find<ModItem>("MeteorHeadStaff").Type)
                    {
                        item.damage = 21;
                    }

                    //Bleeding Heart Staff
                    if (item.type == thorium.Find<ModItem>("BleedingHeartStaff").Type)
                    {
                        item.damage = 35;
                    }

                    //Taboo Wand
                    if (item.type == thorium.Find<ModItem>("TabooWand").Type)
                    {
                        item.damage = 30;
                    }

                    //Weed Eater
                    if (item.type == thorium.Find<ModItem>("WeedEater").Type)
                    {
                        item.damage = 26;
                    }

                    //Arsenal Staff
                    if (item.type == thorium.Find<ModItem>("ArsenalStaff").Type)
                    {
                        item.damage = 24;
                    }

                    //Yarn Ball
                    if (item.type == thorium.Find<ModItem>("YarnBall").Type)
                    {
                        item.damage = 21;
                    }

                    //Strongest Link
                    if (item.type == thorium.Find<ModItem>("StrongestLink").Type)
                    {
                        item.damage = 65;
                    }

                    //Strange Skull
                    if (item.type == thorium.Find<ModItem>("StrangeSkull").Type)
                    {
                        item.damage = 45;
                    }

                    //Boulder Probe Staff
                    if (item.type == thorium.Find<ModItem>("BoulderProbeStaff").Type)
                    {
                        item.damage = 35;
                    }

                    if (item.type == thorium.Find<ModItem>("ButterflyStaff").Type)
                    {
                        item.damage = 9;
                    }

                    if (item.type == thorium.Find<ModItem>("PrehistoricAmberStaff").Type)
                    {
                        item.damage = 14;
                    }

                    if (item.type == thorium.Find<ModItem>("ViscountCane").Type)
                    {
                        item.damage = 22;
                    }
                    #endregion

                    #region Hardmode
                    if (UnsafeGetItem(thorium, "Thrombosis", item))
                    {
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "DraconicMagmaStaff", item))
                    {
                        item.damage = 35;
                    }

                    if (GetItem(thorium, "CrimsonHoundStaff", item))
                    {
                        item.damage = 37;
                    }

                    if (GetItem(thorium, "LadyLight", item))
                    {
                        item.damage = 34;
                    }

                    if (GetItem(thorium, "MortarStaff", item))
                    {
                        item.damage = 111;
                    }

                    if (GetItem(thorium, "BeholderStaff", item))
                    {
                        item.damage = 43;
                    }

                    if (GetItem(thorium, "AeonStaff", item))
                    {
                        item.damage = 46;
                    }

                    if (GetItem(thorium, "SteamgunnerController", item))
                    {
                        item.damage = 24;
                    }

                    if (GetItem(thorium, "ValkyrieBlade", item))
                    {
                        item.damage = 57;
                    }

                    if (GetItem(thorium, "CorrodlingStaff", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "EyeofOdin", item))
                    {
                        item.damage = 75;
                    }

                    if (GetItem(thorium, "VoltModule", item))
                    {
                        item.damage = 170;
                    }

                    if (GetItem(thorium, "TheIncubator", item))
                    {
                        item.damage = 88;
                    }

                    if (GetItem(thorium, "NebulaReflection", item))
                    {
                        item.damage = 133;
                    }

                    if (GetItem(thorium, "TerrariumEnigmaStaff", item))
                    {
                        item.damage = 50;
                    }

                    if (GetItem(thorium, "EmberStaff", item))
                    {
                        item.damage = 260;
                    }

                    if (GetItem(thorium, "PrometheanStaff", item))
                    {
                        item.damage = 200;
                    }

                    if (GetItem(thorium, "TheBlackCane", item))
                    {
                        item.damage = 40;
                    }
                    #endregion
                    #endregion

                    #region Rogue & Thrower
                    #region Pre-Hardmode
                    //Stone Throwing Spear
                    if (item.type == thorium.Find<ModItem>("StoneThrowingSpear").Type)
                    {
                        item.damage = 16;
                    }

                    //Cactus Needle
                    if (item.type == thorium.Find<ModItem>("CactusNeedle").Type)
                    {
                        item.shootSpeed = 15;
                        item.useTime = 10;
                        item.useAnimation = 10;
                        item.damage = 10;
                    }

                    //Iron Tomahawk
                    if (item.type == thorium.Find<ModItem>("IronTomahawk").Type)
                    {
                        item.useTime = 22;
                        item.useAnimation = 22;
                        item.damage = 11;
                    }

                    //Lead Tomahawk
                    if (item.type == thorium.Find<ModItem>("LeadTomahawk").Type)
                    {
                        item.shootSpeed = 14;
                        item.damage = 15;
                    }

                    //Demonite Tomahawk
                    if (item.type == thorium.Find<ModItem>("DemoniteTomahawk").Type)
                    {
                        item.useTime = 20;
                        item.useAnimation = 20;
                    }

                    //Crimtane Tomahawk
                    if (item.type == thorium.Find<ModItem>("CrimtaneTomahawk").Type)
                    {
                        item.shootSpeed = 16;
                    }

                    //Enchanted Knife
                    if (item.type ==thorium.Find<ModItem>("EnchantedKnife").Type)
                    {
                        item.damage = 14;
                    }

                    //Lasting Pliers
                    if (item.type == thorium.Find<ModItem>("LastingPliers").Type)
                    {
                        item.damage = 26;
                        item.useAnimation = 26;
                        item.useTime = 26;
                    }

                    //Crude Bat
                    if (item.type == thorium.Find<ModItem>("BaseballBat").Type)
                    {
                        item.useTime = 6;
                        item.useAnimation = 6;
                        item.damage = 14;
                    }

                    //Coral Caltrop
                    if (item.type == thorium.Find<ModItem>("CoralCaltrop").Type)
                    {
                        item.damage = 13;
                    }

                    //Severed Hand
                    if (item.type == thorium.Find<ModItem>("SeveredHand").Type)
                    {
                        item.damage = 20;
                        item.shootSpeed = 12;
                    }

                    //Thorium Dagger
                    if (item.type == thorium.Find<ModItem>("ThoriumDagger").Type)
                    {
                        item.shootSpeed = 18;
                        item.damage = 30;
                    }

                    //Bolas
                    if (item.type == thorium.Find<ModItem>("Bolas").Type)
                    {
                        item.damage = 35;
                    }

                    //Obsidian Striker
                    if (item.type == thorium.Find<ModItem>("ObsidianStriker").Type)
                    {
                        item.damage = 21;
                    }

                    //Sandstone Throwing Knife
                    if (item.type == thorium.Find<ModItem>("gSandStoneThrowingKnife").Type)
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                        item.damage = 20;
                    }

                    //Shinobi Slicer
                    if (item.type == thorium.Find<ModItem>("ShinobiSlicer").Type)
                    {
                        item.shootSpeed = 8;
                        item.damage = 20;
                    }

                    //Gel Glove
                    if (item.type == thorium.Find<ModItem>("GelGlove").Type)
                    {
                        item.damage = 24;
                        item.shootSpeed = 15;
                    }

                    //Steel Throwing Axe
                    if (item.type == thorium.Find<ModItem>("SteelThrowingAxe").Type)
                    {
                        item.shootSpeed = 10;
                        item.damage = 30;
                    }

                    //Blooming Shuriken
                    if (item.type == thorium.Find<ModItem>("BloomingShuriken").Type)
                    {
                        item.useTime = 30;
                        item.useAnimation = 30;
                        item.damage = 15;
                    }

                    //Harpy’s Barrage
                    if (item.type == thorium.Find<ModItem>("HarpiesBarrage").Type)
                    {
                        item.damage = 36;
                    }

                    //Spike Bomb
                    if (item.type == thorium.Find<ModItem>("SpikeBomb").Type)
                    {
                        item.shootSpeed = 5;
                    }

                    //Goblin War Spear
                    if (item.type == thorium.Find<ModItem>("GoblinWarSpear").Type)
                    {
                        item.shootSpeed = 14;
                        item.damage = 32;
                    }

                    //Meteorite Cluster Bomb
                    if (item.type == thorium.Find<ModItem>("MeteoriteClusterBomb").Type)
                    {
                        item.damage = 20;
                    }

                    //Aquaite Knife
                    if (item.type == thorium.Find<ModItem>("AquaiteKnife").Type)
                    {
                        item.useAnimation = 15;
                        item.useTime = 15;
                        item.damage = 21;
                    }

                    //Molten Knife
                    if (item.type == thorium.Find<ModItem>("MoltenKnife").Type)
                    {
                        item.damage = 40;
                        item.useAnimation = 24;
                        item.useTime = 24;
                    }

                    //Arcane Anelace
                    if (item.type == thorium.Find<ModItem>("ArcaneAnelace").Type)
                    {
                        item.damage = 30;
                        item.useTime = 17;
                        item.useAnimation = 17;
                    }

                    //Naiad's Shiv
                    if (item.type == thorium.Find<ModItem>("NaiadShiv").Type)
                    {
                        item.useTime = 8;
                        item.useAnimation = 8;
                        item.damage = 25;
                    }

                    //Spiky Caltrop
                    if (item.type == thorium.Find<ModItem>("SpikyCaltrop").Type)
                    {
                        item.damage = 24;
                    }

                    //Phase Chopper
                    if (item.type == thorium.Find<ModItem>("PhaseChopper").Type)
                    {
                        item.damage = 22;
                    }

                    //Wack Wrench
                    if (item.type == thorium.Find<ModItem>("WackWrench").Type)
                    {
                        item.damage = 28;
                    }

                    //Champion's God Hand
                    if (item.type == thorium.Find<ModItem>("ChampionsGodHand").Type)
                    {
                        item.damage = 75;
                        item.shootSpeed = 20;
                    }

                    //Bronze Throwing Axe
                    if (item.type == thorium.Find<ModItem>("BronzeThrowingAxe").Type)
                    {
                        item.shootSpeed = 10;
                        item.damage = 39;
                    }

                    //Bronze Throwing Axe
                    if (item.type == thorium.Find<ModItem>("GraniteThrowingAxe").Type)
                    {
                        item.shootSpeed = 10;
                        item.damage = 37;
                    }

                    //Light's Anquish
                    if (item.type == thorium.Find<ModItem>("LightAnguish").Type)
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.shootSpeed = 16;
                        item.damage = 50;
                    }

                    //Embowellment
                    if (item.type == thorium.Find<ModItem>("Embowelment").Type)
                    {
                        item.useTime = 18;
                        item.useAnimation = 18;
                        item.shootSpeed = 16;
                        item.damage = 60;
                    }

                    //Gauss Flinger
                    if (item.type == thorium.Find<ModItem>("GaussFlinger").Type)
                    {
                        item.useTime = 10;
                        item.useAnimation = 10;
                    }
                    #endregion

                    #region Hardmode
                    //Durasteel Throwing Spear
                    if (item.type == thorium.Find<ModItem>("DurasteelThrowingSpear").Type)
                    {
                        item.damage = 70;
                    }

                    //Eviscerating Claw
                    if (item.type == thorium.Find<ModItem>("EvisceratingClaw").Type)
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 39;
                    }

                    //Kunai
                    if (item.type == thorium.Find<ModItem>("Kunai").Type)
                    {
                        item.damage = 60;
                    }

                    //Venom Kunai
                    if (item.type == thorium.Find<ModItem>("VenomKunai").Type)
                    {
                        item.damage = 66;
                    }

                    //Corrupter's Balloon, Crystal Balloon, Festering Balloon
                    if (item.type == thorium.Find<ModItem>("CorrupterBalloon").Type || item.type == thorium.Find<ModItem>("CrystalBalloon").Type || item.type == thorium.Find<ModItem>("FesteringBalloon").Type)
                    {
                        item.useTime = 10;
                        item.useAnimation = 10;
                        item.shootSpeed = 13;
                        item.damage = 50;
                    }

                    if (GetItem(thorium, "FungalPopper", item))
                    {
                        item.damage = 41;
                    }

                    if (GetItem(thorium, "MorelGrenade", item))
                    {
                        item.damage = 48;
                    }

                    if (GetItem(thorium, "ShadowTippedJavelin", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    if (GetItem(thorium, "HotPot", item))
                    {
                        item.damage = 54;
                    }

                    if (GetItem(thorium, "RocketFist", item))
                    {
                        item.useTime = 15;
                        item.useAnimation = 15;
                    }

                    if (GetItem(thorium, "VoltHatchet", item))
                    {
                        item.damage = 96;
                    }

                    if (GetItem(thorium, "AxeBlade", item))
                    {
                        item.damage = 67;
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    if (GetItem(thorium, "LegionOrnament", item))
                    {
                        item.damage = 51;
                    }

                    if (GetItem(thorium, "TheCryoFang", item))
                    {
                        item.shootSpeed = 22;
                        item.damage = 82;
                        item.useTime = 22;
                        item.useAnimation = 22;
                    }

                    if (GetItem(thorium, "CaptainsPoniard", item))
                    {
                        item.useTime = 15;
                        item.useAnimation = 15;
                        item.damage = 41;
                    }

                    if (GetItem(thorium, "HellRoller", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                        item.damage = 66;
                    }

                    if (GetItem(thorium, "ClockWorkBomb", item))
                    {
                        item.damage = 50;
                        item.shootSpeed = 14;
                    }

                    if (GetItem(thorium, "MagicCard", item))
                    {
                        item.crit = 21;
                        item.shootSpeed = 14;
                        item.damage = 60;
                        item.useTime = 8;
                        item.useAnimation = 8;
                    }

                    if (GetItem(thorium, "SoulBomb", item))
                    {
                        item.damage = 140;
                    }

                    if (GetItem(thorium, "SparkTaser", item))
                    {
                        item.damage = 65;
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    if (GetItem(thorium, "HadronCollider", item))
                    {
                        item.useTime = 20;
                        item.useAnimation = 20;
                    }

                    if (GetItem(thorium, "Carnwennan", item))
                    {
                        item.useTime = 13;
                        item.useAnimation = 13;
                    }

                    if (GetItem(thorium, "TrueCarnwennan", item))
                    {
                        item.useTime = 13;
                        item.useAnimation = 13;
                    }

                    if (GetItem(thorium, "RiftTearer", item))
                    {
                        item.damage = 150;
                    }

                    if (GetItem(thorium, "StalkersSnippers", item))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 124;
                    }

                    if (GetItem(thorium, "TrueLightAnguish", item))
                    {
                        item.shootSpeed = 26;
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    if (GetItem(thorium, "TrueEmbowelment", item))
                    {
                        item.shootSpeed = 26;
                        item.useTime = 30;
                        item.useAnimation = 30;
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "BudBomb", item))
                    {
                        item.damage = 38;
                    }

                    if (GetItem(thorium, "LihzahrdKukri", item))
                    {
                        item.damage = 99;
                    }

                    if (GetItem(thorium, "ProximityMine", item))
                    {
                        item.damage = 111;
                    }

                    if (GetItem(thorium, "Soulslasher", item))
                    {
                        item.damage = 109;
                    }

                    if (GetItem(thorium, "BugenkaiShuriken", item))
                    {
                        item.damage = 85;
                    }

                    if (GetItem(thorium, "ShadeKunai", item))
                    {
                        item.useTime = 8;
                        item.useAnimation = 8;
                    }

                    if (GetItem(thorium, "TerraKnife", item))
                    {
                        item.damage = 45;
                    }

                    if (GetItem(thorium, "FireAxe", item))
                    {
                        item.damage = 350;
                    }

                    if (GetItem(thorium, "ShadeKusarigama", item))
                    {
                        item.shootSpeed = 22;
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.damage = 310;
                    }

                    if (GetItem(thorium, "PharaohsSlab", item))
                    {
                        item.damage = 160;
                    }

                    if (GetItem(thorium, "TheBlackDagger", item))
                    {
                        item.damage = 55;
                        item.useTime = 6;
                        item.useAnimation = 6;
                    }

                    if (GetItem(thorium, "SoftServeSunderer", item))
                    {
                        item.damage = 120;
                    }

                    if (GetItem(thorium, "Witchblade", item))
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.damage = 180;
                    }

                    if (GetItem(thorium, "CosmicDagger", item))
                    {
                        item.damage = 224;
                    }

                    if (GetItem(thorium, "ElectroRebounder", item))
                    {
                        item.damage = 177;
                        item.shootSpeed = 20;
                    }

                    if (GetItem(thorium, "Brinefang", item))
                    {
                        item.damage = 150;
                    }

                    if (GetItem(thorium, "DragonFang", item))
                    {
                        item.damage = 125;
                    }

                    if (GetItem(thorium, "TerrariumRippleKnife", item))
                    {
                        item.damage = 66;
                    }

                    if (GetItem(thorium, "StarEater", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "AngelsEnd", item))
                    {
                        item.damage = 525;
                    }

                    if (GetItem(thorium, "DeitysTrefork", item))
                    {
                        item.damage = 500;
                    }

                    if (GetItem(thorium, "TidalWave", item))
                    {
                        item.damage = 190;
                    }
                    #endregion
                    #endregion

                    #region Healer
                    if (GetItem(thorium, "LustrousBaton", item))
                    {
                        item.damage = 54;
                    }

                    if (GetItem(thorium, "HolyFire", item))
                    {
                        item.damage = 40;
                    }

                    #region Pre-Hardmode
                    if (GetItem(thorium, "CrimsonScythe", item))
                    {
                        item.damage = 14;
                    }

                    if (GetItem(thorium, "DarkScythe", item))
                    {
                        item.damage = 13;
                    }

                    //Wooden Baton
                    if (item.type == thorium.Find<ModItem>("WoodenBaton").Type)
                    {
                        item.damage = 7;
                    }

                    //Pill
                    if (item.type == thorium.Find<ModItem>("Pill").Type)
                    {
                        item.damage = 11;
                        item.shootSpeed = 9;
                    }

                    //Bonesaw 
                    if (item.type == thorium.Find<ModItem>("Bonesaw").Type)
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                    }

                    //Life Quartz Claymore
                    if (item.type == thorium.Find<ModItem>("LifeQuartzClaymore").Type)
                    {
                        item.damage = 17;
                        item.useAnimation = 26;
                        item.shootSpeed = 26;

                        item.scale *= 1.5f;
                    }

                    //Palm Cross
                    if (item.type == thorium.Find<ModItem>("PalmCross").Type)
                    {
                        item.damage = 15;
                    }

                    //Ice Shaver
                    if (item.type == thorium.Find<ModItem>("IceShaver").Type)
                    {
                        item.damage = 11;
                    }

                    //Poison Prickler
                    if (item.type == thorium.Find<ModItem>("PoisonPrickler").Type)
                    {
                        item.shootSpeed = 10;
                        item.useTime = 10;
                        item.useAnimation = 10;
                        item.damage = 15;
                        item.mana = 5;
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
                    }

                    //The Digester
                    if (item.type == thorium.Find<ModItem>("TheDigester").Type)
                    {
                        item.mana = 10;
                    }

                    //Rotten Cod
                    if (item.type == thorium.Find<ModItem>("RottenCod").Type)
                    {
                        item.damage = 24;
                    }

                    //Deep Staff
                    if (item.type == thorium.Find<ModItem>("DeepStaff").Type)
                    {
                        item.damage = 25;
                    }

                    //Heretic Breaker
                    if (item.type == thorium.Find<ModItem>("HereticBreaker").Type)
                    {
                        item.useTime = 16;
                        item.useAnimation = 16;
                        item.damage = 20;

                        item.scale *= 1.25f;
                    }

                    //Renew
                    if (item.type == thorium.Find<ModItem>("Renew").Type)
                    {
                        item.autoReuse = true;
                    }

                    if (item.type == thorium.Find<ModItem>("BountifulHarvest").Type)
                    {
                        item.damage = 17;
                    }

                    //Leech Bolt
                    if (item.type == thorium.Find<ModItem>("LeechBolt").Type)
                    {
                        item.damage = 18;
                    }

                    //Cleric's Cross
                    if (item.type == thorium.Find<ModItem>("ClericsCross").Type)
                    {
                        item.damage = 19;
                    }

                    //Purified Water
                    if (item.type == thorium.Find<ModItem>("PurifiedWater").Type)
                    {
                        item.damage = 26;
                    }

                    //Shadow Wand
                    if (item.type == thorium.Find<ModItem>("ShadowWand").Type)
                    {
                        item.damage = 17;
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    //Aquaite Scythe
                    if (item.type == thorium.Find<ModItem>("AquaiteScythe").Type)
                    {
                        item.useTime = 23;
                        item.useAnimation = 23;
                        item.damage = 12;
                    }

                    //Samsara Lotus
                    if (item.type == thorium.Find<ModItem>("SamsaraLotus").Type)
                    {
                        item.damage = 28;
                    }

                    //The Stalker
                    if (item.type == thorium.Find<ModItem>("TheStalker").Type)
                    {
                        item.damage = 36;
                    }

                    //Eater of Pain
                    if (item.type == thorium.Find<ModItem>("EaterOfPain").Type)
                    {
                        item.useTime = 13;
                        item.useAnimation = 13;
                    }

                    //Molten Thresher
                    if (item.type == thorium.Find<ModItem>("MoltenThresher").Type)
                    {
                        item.damage = 28;
                    }

                    //Omen
                    if (item.type == thorium.Find<ModItem>("Omen").Type)
                    {
                        item.damage = 40;
                        item.useTime = 20;
                        item.useAnimation = 20;
                    }

                    //Bat Scythe
                    if (item.type == thorium.Find<ModItem>("BatScythe").Type)
                    {
                        item.damage = 26;
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }

                    //Life Disperser
                    if (item.type == thorium.Find<ModItem>("LifeDisperser").Type)
                    {
                        item.mana = 5;
                    }

                    //Bone Reaper
                    if (item.type == thorium.Find<ModItem>("BoneReaper").Type)
                    {
                        item.crit = 16;
                        item.damage = 11;
                    }

                    //Spirit Blast Wand
                    if (item.type == thorium.Find<ModItem>("SpiritBlastWand").Type)
                    {
                        item.damage = 28;
                    }

                    //Templar's Judgement
                    if (item.type == thorium.Find<ModItem>("TemplarJudgment").Type)
                    {
                        item.damage = 62;
                    }

                    //Light's Lament
                    if (item.type == thorium.Find<ModItem>("LightsLament").Type)
                    {
                        item.mana = 10;
                        item.useTime = 15;
                        item.useAnimation = 15;
                        item.damage = 40;
                        item.shootSpeed = 20;
                    }

                    //Dark Contagion
                    if (item.type == thorium.Find<ModItem>("DarkContagion").Type)
                    {
                        item.shootSpeed = 40;
                        item.useTime = 28;
                        item.useAnimation = 28;
                        item.damage = 60;
                        item.mana = 5;
                    }

                    //Falling Twilight
                    if (item.type == thorium.Find<ModItem>("FallingTwilight").Type)
                    {
                        item.damage = 30;
                    }

                    //Blood Harvest
                    if (item.type == thorium.Find<ModItem>("BloodHarvest").Type)
                    {
                        item.damage = 27;
                    }
                    #endregion

                    #region Hardmode
                    if (GetItem(thorium, "DemonBloodRipper", item))
                    {
                        item.damage = 36;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "BalanceBloom", item))
                    {
                        item.damage = 45;
                        item.shootSpeed = 18;
                    }

                    //Bone Baton
                    if (GetItem(thorium, "BoneBaton", item))
                    {
                        item.damage = 60;
                    }

                    //Sacred Bludgeon
                    if (GetItem(thorium, "SacredBludgeon", item))
                    {
                        item.damage = 65;
                    }

                    //Tranquil Lyre
                    if (GetItem(thorium, "TranquilLyre", item))
                    {
                        item.shootSpeed = 16;
                        item.damage = 55;
                    }

                    //Blood Transfusion
                    if (GetItem(thorium, "BloodTransfusion", item))
                    {
                        item.damage = 36;
                        item.shootSpeed = 14;
                    }

                    //Wild Umbra
                    if (GetItem(thorium, "WildUmbra", item))
                    {
                        item.damage = 48;
                        item.useTime = 16;
                        item.useAnimation = 16;
                    }

                    //Iridescent Staff
                    if (GetItem(thorium, "IridescentStaff", item))
                    {
                        item.useTime = 19;
                        item.useAnimation = 19;
                    }

                    //Twilight Staff
                    if (GetItem(thorium, "TwilightStaff", item))
                    {
                        item.damage = 40;
                    }

                    //Hallowed Scythe
                    if (GetItem(thorium, "HallowedScythe", item))
                    {
                        item.damage = 44;
                    }

                    //True Hallowed Scythe
                    if (GetItem(thorium, "TrueHallowedScythe", item))
                    {
                        item.damage = 54;
                    }

                    if (GetItem(thorium, "TitanScythe", item))
                    {
                        item.crit = 0;
                        item.damage = 49;
                    }

                    if (GetItem(thorium, "MorningDew", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "TrueFallingTwilight", item))
                    {
                        item.damage = 51;
                    }

                    if (GetItem(thorium, "TrueBloodHarvest", item))
                    {
                        item.damage = 33;
                    }

                    if (GetItem(thorium, "TheEffuser", item))
                    {
                        item.damage = 152;
                    }

                    if (GetItem(thorium, "HallowedBlessing", item))
                    {
                        item.damage = 90;
                    }

                    if (GetItem(thorium, "SmitingHammer", item))
                    {
                        item.damage = 139;
                    }

                    if (GetItem(thorium, "MindMelter", item))
                    {
                        item.damage = 125;
                        item.useTime = 10;
                        item.useAnimation = 10;
                    }

                    if (GetItem(thorium, "DreadTearer", item))
                    {
                        item.damage = 58;
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "IllumiteScythe", item))
                    {
                        item.damage = 71;
                    }

                    if (GetItem(thorium, "LethalInjection", item))
                    {
                        item.damage = 78;
                    }

                    if (GetItem(thorium, "LightBringerWarhammer", item))
                    {
                        item.useTime = 26;
                        item.useAnimation = 26;
                        item.damage = 104;
                    }

                    if (UnsafeGetItem(thorium, "BubbleBulwarkWand", item))
                    {
                        item.useTime = 56;
                        item.useAnimation = 56;
                    }

                    if (GetItem(thorium, "HolyHammer", item))
                    {
                        item.damage = 144;
                    }

                    if (GetItem(thorium, "PillPopper", item))
                    {
                        item.damage = 104;
                    }

                    if (GetItem(thorium, "TheBlackScythe", item))
                    {
                        item.damage = 90;
                    }

                    if (GetItem(thorium, "TerraScythe", item))
                    {
                        item.damage = 67;
                    }

                    if (GetItem(thorium, "ChristmasCheer", item))
                    {
                        item.damage = 240;
                    }

                    if (GetItem(thorium, "Kinetoscythe", item))
                    {
                        item.damage = 160;
                    }

                    if (GetItem(thorium, "PaganGrasp", item))
                    {
                        item.damage = 276;
                    }

                    if (GetItem(thorium, "CosmicFluxStaff", item))
                    {
                        item.damage = 140;
                    }

                    if (GetItem(thorium, "TerrariumHolyScythe", item))
                    {
                        item.damage = 30;
                    }

                    if (UnsafeGetItem(thorium, "CelestialWand", item))
                    {
                        item.useTime = 46;
                        item.useAnimation = 46;
                    }

                    //Lucidty
                    if (GetItem(thorium, "Lucidity", item))
                    {
                        item.damage = InfernalCrossmod.ThoriumRework.Loaded ? 150 : 105;
                    }

                    //Reality Slasher
                    if (GetItem(thorium, "RealitySlasher", item))
                    {
                        item.damage = InfernalCrossmod.ThoriumRework.Loaded ? 175 : 75;
                    }

                    //Life and Death
                    if (GetItem(thorium, "LifeAndDeath", item))
                    {
                        item.damage = 140;
                    }
                    #endregion

                    #region Tools
                    if (UnsafeGetItem(thorium, "Syringe", item))
                    {
                        item.shootSpeed = 12;
                    }

                    if (UnsafeGetItem(thorium, "SpiritBendersStaff", item))
                    {
                        item.damage = 100;
                        item.useTime = 30;
                        item.useAnimation = 30;
                    }

                    if (UnsafeGetItem(thorium, "PocketGuardian", item))
                    {
                        item.useTime = 28;
                        item.useAnimation = 28;
                    }
                    #endregion
                    #endregion

                    #region Bard
                    if (UnsafeGetItem(thorium, "HellBell", item))
                    {
                        item.damage = 66;
                    }

                    if (UnsafeGetItem(thorium, "SongofIceAndFire", item))
                    {
                        item.damage = 34;
                        item.crit = 0;
                    }

                    #region Pre-Hardmode
                    //Wooden Whistle
                    if (item.type == thorium.Find<ModItem>("WoodenWhistle").Type)
                    {
                        item.damage = 9;
                    }

                    //Drum Mallet
                    if (item.type == thorium.Find<ModItem>("DrumMallet").Type)
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                    }

                    //Ukulele
                    if (item.type == thorium.Find<ModItem>("Ukulele").Type)
                    {
                        item.damage = 12;
                    }

                    //Tambourine
                    if (item.type == thorium.Find<ModItem>("Tambourine").Type)
                    {
                        item.damage = 13;
                        item.useAnimation = 10;
                        item.useTime = 10;
                        item.shootSpeed = 12;
                    }

                    //Antlion Maraca
                    if (item.type == thorium.Find<ModItem>("AntlionMaraca").Type)
                    {
                        item.shootSpeed = 16;
                    }

                    //Lightning Claves
                    if (item.type == thorium.Find<ModItem>("LightningClaves").Type)
                    {
                        item.shootSpeed = 8;
                    }

                    //Gold Bugle Horn
                    if (item.type == thorium.Find<ModItem>("GoldBugleHorn").Type)
                    {
                        item.damage = 15;
                        item.useTime = 30;
                        item.useAnimation = 30;
                    }

                    //Ebonwood Tambourine
                    if (item.type == thorium.Find<ModItem>("EbonWoodTambourine").Type)
                    {
                        item.damage = 18;
                        item.useTime = 6;
                        item.useAnimation = 6;
                    }

                    //Shadewood Tambourine
                    if (item.type == thorium.Find<ModItem>("ShadeWoodTambourine").Type)
                    {
                        item.damage = 26;
                        item.useTime = 22;
                        item.useAnimation = 22;
                    }

                    //Vicious Mockery
                    if (item.type == thorium.Find<ModItem>("ViciousMockery").Type)
                    {
                        item.damage = 19;
                        item.shootSpeed = 12;
                        item.useTime = 26;
                        item.useAnimation = 26;
                    }

                    //Didgeridoo
                    if (item.type == thorium.Find<ModItem>("Didgeridoo").Type)
                    {
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }

                    //Sinister Honk
                    if (item.type == thorium.Find<ModItem>("SinisterHonk").Type)
                    {
                        item.shootSpeed = 13;
                        item.damage = 25;
                    }

                    //Yew Wood Lute
                    if (item.type == thorium.Find<ModItem>("YewWoodLute").Type)
                    {
                        item.damage = 26;
                    }

                    //Dynasty Guzheng
                    if (item.type == thorium.Find<ModItem>("DynastyGuzheng").Type)
                    {
                        //item.damage = 28;
                    }

                    //Marine Wine Glasss
                    if (item.type == thorium.Find<ModItem>("MarineWineGlass").Type)
                    {
                        item.damage = 30;
                        item.useTime = 10;
                        item.useAnimation = 10;
                    }

                    //Bongos
                    if (item.type == thorium.Find<ModItem>("Bongos").Type)
                    {
                        item.damage = 38;
                    }

                    //Jar O' Mayo
                    if (item.type == thorium.Find<ModItem>("JarOMayo").Type)
                    {
                        item.damage = 30;
                    }

                    if (UnsafeGetItem(thorium, "MeteoriteOboe", item))
                    {
                        TrySetInspirationCost(item, 2);
                    }

                    //Sonar Cannon
                    if (item.type == thorium.Find<ModItem>("SonarCannon").Type)
                    {
                        item.damage = 40;
                        item.useTime = 26;
                        item.useAnimation = 26;
                    }

                    //Bone Trumpet
                    if (item.type == thorium.Find<ModItem>("BoneTrumpet").Type)
                    {
                        item.damage = 50;
                    }

                    //Calaveras
                    if (item.type == thorium.Find<ModItem>("Calaveras").Type)
                    {
                        item.damage = 55;
                        item.damage = 18;
                        item.useTime = 18;
                    }

                    //Microphone
                    if (item.type == thorium.Find<ModItem>("Microphone").Type)
                    {
                        item.damage = 34;
                    }

                    //Nocturne
                    if (item.type == thorium.Find<ModItem>("Nocturne").Type)
                    {
                        item.damage = 72;
                    }

                    //Roboboe
                    if (item.type == thorium.Find<ModItem>("Roboboe").Type)
                    {
                        item.damage = 55;
                    }
                    #endregion

                    #region Hardmode
                    if (UnsafeGetItem(thorium, "SunflareGuitar", item))
                    {
                        item.damage = 104;
                    }

                    if (UnsafeGetItem(thorium, "Organ", item))
                    {
                        item.damage = 40;
                    }

                    if (UnsafeGetItem(thorium, "SerpentsCry", item))
                    {
                        TrySetInspirationCost(item, 2);
                        item.useTime = 16;
                        item.useAnimation = 16;
                    }

                    if (UnsafeGetItem(thorium, "CallofCthulhu", item))
                    {
                        TrySetInspirationCost(item, 1);
                        item.damage = 204;
                        item.useTime = 28;
                        item.useAnimation = 28;
                    }

                    if (UnsafeGetItem(thorium, "TheTriangle", item))
                    {
                        item.damage = 114;
                    }

                    //Dragon's Wail
                    if (GetItem(thorium, "DragonsWail", item))
                    {
                        item.crit = 0;
                    }

                    if (GetItem(thorium, "FleshHorn", item))
                    {
                        item.shootSpeed = 20;
                        item.damage = 110;
                    }

                    if (GetItem(thorium, "Trombone", item))
                    {
                        item.damage = 55;
                    }

                    if (GetItem(thorium, "CobaltGong", item))
                    {
                        item.damage = 140;
                    }

                    if (GetItem(thorium, "ResonatorsArm", item))
                    {
                        item.shootSpeed = 14;
                        item.useTime = 15;
                        item.useAnimation = 15;
                        item.damage = 70;
                    }

                    if (GetItem(thorium, "Xylophone", item))
                    {
                        item.damage = 94;
                        item.shootSpeed = 25;
                    }

                    if (GetItem(thorium, "AcousticGuitar", item))
                    {
                        item.damage = 85;
                    }

                    if (GetItem(thorium, "ScholarsHarp", item))
                    {
                        item.damage = 26;
                    }

                    if (GetItem(thorium, "TheLullaby", item))
                    {
                        item.damage = 100;
                    }

                    if (GetItem(thorium, "RiffWeaver", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "GeodeSaxophone", item))
                    {
                        item.damage = 66;
                    }

                    if (GetItem(thorium, "WindChimes", item))
                    {
                        item.shootSpeed = 15;
                        item.damage = 54;
                    }

                    if (GetItem(thorium, "GreedfulGurdy", item))
                    {
                        item.damage = 80;
                    }

                    if (GetItem(thorium, "Cello", item))
                    {
                        item.damage = 85;
                        item.useTime = 18;
                        item.useAnimation = 18;
                    }

                    if (GetItem(thorium, "LodestoneRadio", item))
                    {
                        item.damage = 17;
                    }

                    if (GetItem(thorium, "Kazoo", item))
                    {
                        item.damage = 44;
                    }

                    if (GetItem(thorium, "MythrilMelodica", item))
                    {
                        item.damage = 82;
                    }

                    if (GetItem(thorium, "OrichalcumSlideWhistle", item))
                    {
                        item.damage = 55;
                    }

                    if (GetItem(thorium, "GuiltyPleasure", item))
                    {
                        item.damage = 42;
                    }

                    if (GetItem(thorium, "SteamFlute", item))
                    {
                        item.damage = 64;
                    }

                    if (GetItem(thorium, "BaritoneSaxophone", item))
                    {
                        item.damage = 64;
                    }

                    if (GetItem(thorium, "AdamantiteKlaxon", item))
                    {
                        item.damage = 144;
                        TrySetInspirationCost(item, 1);
                    }

                    if (GetItem(thorium, "TitaniumCimbasso", item))
                    {
                        item.damage = 60;
                    }

                    if (GetItem(thorium, "Zunpet", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                        item.damage = 40;
                    }

                    if (GetItem(thorium, "PrimeRoar", item))
                    {
                        item.damage = 200;
                        item.useTime = 34;
                        item.useAnimation = 34;
                    }

                    if (GetItem(thorium, "PortableWintergatan", item))
                    {
                        item.damage = 97;
                    }

                    if (GetItem(thorium, "TheGreenTambourine", item))
                    {
                        item.damage = 108;
                        item.shootSpeed = 18;
                    }

                    if (GetItem(thorium, "HallowedMegaphone", item))
                    {
                        TrySetInspirationCost(item, 1);
                    }

                    if (GetItem(thorium, "MidnightBassBooster", item))
                    {
                        item.damage = 65;
                        item.useTime = 6;
                        item.useAnimation = 6;
                    }

                    if (GetItem(thorium, "CadaverCornet", item))
                    {
                        item.damage = 102;
                        TrySetInspirationCost(item, 1);
                    }

                    if (GetItem(thorium, "VuvuzelaBlue", item) || GetItem(thorium, "VuvuzelaYellow", item) || GetItem(thorium, "VuvuzelaRed", item) || GetItem(thorium, "VuvuzelaGreen", item))
                    {
                        item.damage = 100;
                    }

                    if (GetItem(thorium, "TheMaw", item))
                    {
                        item.damage = 120;
                        item.shootSpeed = 30;
                    }

                    if (GetItem(thorium, "Buisine", item))
                    {
                        TrySetInspirationCost(item, 1);
                        item.damage = 100;
                    }

                    if (GetItem(thorium, "Clarinet", item))
                    {
                        item.damage = 172;
                    }

                    if (GetItem(thorium, "Bassoon", item))
                    {
                        TrySetInspirationCost(item, 1);
                        item.damage = 188;
                    }

                    if (GetItem(thorium, "TheBopper", item))
                    {
                        item.shootSpeed = 17.5f;
                    }

                    if (GetItem(thorium, "IdolsMicrophone", item))
                    {
                        item.damage = 155;
                    }

                    if (GetItem(thorium, "GhastlyFrenchHorn", item))
                    {
                        item.damage = 124;
                    }

                    if (GetItem(thorium, "StrawberryHeart", item))
                    {
                        item.damage = 158;
                    }

                    if (GetItem(thorium, "Fishbone", item))
                    {
                        item.damage = 177;
                    }

                    if (GetItem(thorium, "Pungi", item))
                    {
                        item.damage = 85;
                        item.useTime = 12;
                        item.useAnimation = 12;
                    }

                    if (GetItem(thorium, "HauntingBassDrum", item))
                    {
                        item.damage = 174;
                    }

                    if (GetItem(thorium, "JingleBells", item))
                    {
                        item.damage = 130;
                        item.shootSpeed = 15;
                    }

                    if (GetItem(thorium, "Turntable", item))
                    {
                        item.damage = 190;
                    }

                    if (GetItem(thorium, "SirensLyre", item))
                    {
                        item.damage = 154;
                        item.crit = 21;
                    }

                    if (GetItem(thorium, "BetsysBellow", item))
                    {
                        item.damage = 94;
                    }

                    if (GetItem(thorium, "TerrariumAutoharp", item))
                    {
                        item.damage = 42;
                    }

                    if (GetItem(thorium, "SonicAmplifier", item))
                    {
                        item.useTime = 3;
                    }

                    //Edge of Imagination
                    if (GetItem(thorium, "EdgeofImagination", item))
                    {
                        item.damage = 400;
                    }

                    //Holophonor
                    if (GetItem(thorium, "Holophonor", item))
                    {
                        item.damage = 340;
                    }

                    //The Set
                    if (GetItem(thorium, "TheSet", item))
                    {
                        item.useTime = 34;
                        item.useAnimation = 34;
                        item.damage = 1000;
                    }

                    //Sousaphone
                    if (GetItem(thorium, "Sousaphone", item))
                    {
                        item.damage = 750;
                    }

                    if (GetItem(thorium, "BlackMIDI", item))
                    {
                        item.useTime = 14;
                        item.useAnimation = 14;
                        item.damage = 260;
                    }
                    #endregion
                    //Provided by Wardrobe Hummus
                    #region Insipration Changes
                    if (item.ModItem != null && item.ModItem.Mod.Name == "ThoriumMod")
                    {
                        string itemName = item.ModItem.Name;

                        // Change this to the internal name of the item you want to modify
                        if (itemName == "Nocturne")
                        {
                            TrySetInspirationCost(item, 1);
                        }
                        if (itemName == "SonarCannon")
                        {
                            TrySetInspirationCost(item, 1);
                        }
                        if (itemName == "CobaltGong")
                        {
                            TrySetInspirationCost(item, 2);
                        }
                        if (itemName == "HellBell")
                        {
                            TrySetInspirationCost(item, 1);
                        }
                    }
                    #endregion
                    #endregion

                    #region True
                    if (GetItem(thorium, "Mjolnir", item))
                    {
                        item.damage = 600;
                    }
                    #endregion

                    #region Armor
                    if (GetItem(thorium, "DurasteelChestplate", item))
                    {
                        item.defense = 12;
                    }

                    if (GetItem(thorium, "DurasteelGreaves", item))
                    {
                        item.defense = 11;
                    }

                    if (GetItem(thorium, "DurasteelHelmet", item))
                    {
                        item.defense = 12;
                    }

                    if (GetItem(thorium, "SteelChestplate", item))
                    {
                        item.defense = 7;
                    }

                    if (GetItem(thorium, "SteelGreaves", item))
                    {
                        item.defense = 7;
                    }

                    if (GetItem(thorium, "SteelHelmet", item))
                    {
                        item.defense = 6;
                    }
                    #endregion

                    #region Accessories
                    //Obsidian Scale
                    if (GetItem(thorium, "ObsidianScale", item))
                    {
                        item.defense = 4;
                    }

                    //Blacksmith's Barrier
                    if (UnsafeGetItem(thorium, "BlacksmithsBarrier", item))
                    {
                        item.defense = 4;
                    }

                    /*
                    //Champion's Wings
                    if (item.type == thorium.Find<ModItem>("ChampionWing").Type)
                    {
                        item.defense = 7;
                    }

                    //Dragon Wings
                    if (GetItem(thorium, "DragonWings", item))
                    {
                        item.defense = 10;
                    }
                    */

                    //Abyssal Shell
                    if (GetItem(thorium, "AbyssalShell", item))
                    {
                        item.defense = 0;
                    }

                    /*
                    //Subspace Wings
                    if (GetItem(thorium, "SubspaceWings", item))
                    {
                        item.defense = 10;
                        item.lifeRegen = 2;
                    }

                    //Terrarium Wings
                    if (GetItem(thorium, "TerrariumWings", item))
                    {
                        item.defense = 10;
                        item.lifeRegen = 4;
                    }
                    */

                    //Shinobi Sigil
                    if (UnsafeGetItem(thorium, "ShinobiSigil", item))
                    {
                        TrySetAccessoryDamage(item, $"25% {Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.BasicDamage")}");
                    }
                    #endregion

                    #region Ammo
                    if (UnsafeGetItem(thorium, "IcyArrow", item))
                    {
                        item.damage = 7;
                    }
                    #endregion
                }
            }
            #endregion

            #region Unofficial Calamity Bard & Healer
            if (ModLoader.TryGetMod("CalamityBardHealer", out Mod calBardHeal) && (ModContent.GetInstance<InfernalConfig>().ThoriumBalanceChangess || ModContent.GetInstance<InfernalConfig>().CalamityBalanceChanges))
            {
                if (UnsafeGetItem(calBardHeal, "InfestedCastanet", item))
                {
                    item.damage = 22;
                    item.useTime = 7;
                    item.useAnimation = 7;
                }

                if (UnsafeGetItem(calBardHeal, "HyphaeBaton", item))
                {
                    item.damage = 16;
                }

                if (UnsafeGetItem(calBardHeal, "TheWindmill", item))
                {
                    item.damage = 20;
                }

                if (UnsafeGetItem(calBardHeal, "SlagFurysIntent", item))
                {
                    item.damage = 22;
                }

                if (UnsafeGetItem(calBardHeal, "HarpY", item))
                {
                    item.damage = 22;
                }

                if (UnsafeGetItem(calBardHeal, "ColdShoulder", item))
                {
                    item.damage = 59;
                }

                #region Healer
                #region Pre-Hardmode
                //Wulfrum Weed Wacker
                if (item.type == calBardHeal.Find<ModItem>("WulfrumWeedWacker").Type)
                {
                    item.useTime = 36;
                    item.useAnimation = 36;
                }

                //Dry Mouth
                if (item.type == calBardHeal.Find<ModItem>("DryMouth").Type)
                {
                    item.useTime = 40;
                    item.useAnimation = 40;
                    item.damage = 19;
                }

                //The Windmill
                if (item.type == calBardHeal.Find<ModItem>("TheWindmill").Type)
                {
                    //item.damage = 5;
                }

                //Duality
                if (item.type == calBardHeal.Find<ModItem>("Duality").Type)
                {
                    item.damage = 40;
                }

                //Gelatin Therapy
                if (item.type == calBardHeal.Find<ModItem>("GelatinTherapy").Type)
                {
                    item.useTime = 86;
                    item.useAnimation = 86;
                    item.autoReuse = true;
                }

                //Birthplace of Stars
                if (calBardHeal.TryFind("StarBirth", out ModItem starBirth))
                {
                    if (item.type == starBirth.Type && catalyst != null)
                    {
                        TrySetHealAmmount(item, 2);
                        item.autoReuse = true;
                    }
                }
                #endregion

                #region Hardmode
                if (GetItem(calBardHeal, "CottonMouth", item))
                {
                    item.damage = 30;
                }

                if (GetItem(calBardHeal, "ElectricQuarterstaff", item))
                {
                    item.damage = 32;
                }

                //Fire Hazard
                if (item.type == calBardHeal.Find<ModItem>("FireHazard").Type)
                {
                    item.damage = 30;
                }

                if (GetItem(calBardHeal, "TidalForce", item))
                {
                    item.damage = 231;
                }

                if (UnsafeGetItem(calBardHeal, "HydrogenSulfide", item))
                {
                    item.damage = 101;
                }

                if (GetItem(calBardHeal, "SARS", item))
                {
                    item.damage = 100;
                    item.crit = 0;
                }

                if (GetItem(calBardHeal, "OmnicidesLaw", item))
                {
                    item.damage = 90;
                }

                //Syzygy
                if (GetItem(calBardHeal, "Syzygy", item))
                {
                    item.damage = 80;
                }

                //Singularity
                if (catalyst != null)
                {
                    if (GetItem(calBardHeal, "Singularity", item))
                    {
                        item.damage = 110;
                    }
                }

                //Blooming Saintess' Devotion
                if (GetItem(calBardHeal, "BloomingSaintessDevotion", item))
                {
                    item.damage = 79;
                }

                //Death Adder
                if (GetItem(calBardHeal, "DeathAdder", item))
                {
                    item.damage = 180;
                }

                if (GetItem(calBardHeal, "IrradiatedKusarigama", item))
                {
                    item.damage = 229;
                }

                //Purgatorium Pandemonium
                if (GetItem(calBardHeal, "PurgatoriumPandemonium", item))
                {
                    item.useTime = 30;
                    item.useAnimation = 30;
                    item.damage = 235;
                }

                //Exorectionist
                if (GetItem(calBardHeal, "Exorectionist", item))
                {
                    item.Calamity().ChargePerUse = (float)0.5;
                }

                //Milky Way
                if (GetItem(calBardHeal, "MilkyWay", item))
                {
                    item.damage = 165;
                    item.crit = 0;
                }

                //Saving Grace
                if (GetItem(calBardHeal, "SavingGrace", item))
                {
                    item.useTime = 80;
                    item.useAnimation = 80;
                    item.autoReuse = true;
                }

                //Will of the Ragnarok
                if (GetItem(calBardHeal, "WilloftheRagnarok", item))
                {
                    item.damage = 100;
                }

                //Cherubim Omega
                if (GetItem(calBardHeal, "CherubimOmega", item))
                {
                    item.damage = 600;
                }

                //Disaster
                if (GetItem(calBardHeal, "Disaster", item))
                {
                    item.damage = 550;
                }

                //Times Old Roman
                if (calHunt != null)
                {
                    if (GetItem(calBardHeal, "TimesOldRoman", item))
                    {
                        item.crit = 0;
                        item.damage = 1000;
                        item.useTime = 24;
                        item.useAnimation = 24;
                    }
                }

                #endregion
                #endregion

                #region Bard
                if (GetItem(calBardHeal, "BigBangCymbals", item))
                {
                    item.damage = 252;
                }

                if (GetItem(calBardHeal, "TreeWhisperersHarp", item))
                {
                    item.damage = 100;
                }

                #region Pre-Hardmode
                //Wullfrum Megaphone
                if (UnsafeGetItem(calBardHeal, "WulfrumMegaphone", item))
                {
                    item.damage = 13;
                }

                //Crystal Hydraulophone
                if (UnsafeGetItem(calBardHeal, "CrystalHydraulophone", item))
                {
                    item.damage = 26;
                }

                //Violince
                if (GetItem(calBardHeal, "Violince", item))
                {
                    item.damage = 9;
                }

                if (UnsafeGetItem(calBardHeal, "FilthyFlute", item))
                {
                    item.damage = 32;
                }

                if (UnsafeGetItem(calBardHeal, "ScrapGuitar", item))
                {
                    item.damage = 53;
                }

                //Return to Sludge
                if (item.type == calBardHeal.Find<ModItem>("ReturntoSludge").Type)
                {
                    item.damage = 33;
                    item.shootSpeed = 8;
                }
                #endregion

                #region Hardmode
                //Song of the Elements
                if (GetItem(calBardHeal, "ArcticReinforcement", item))
                {
                    item.damage = 49;
                }

                if (GetItem(calBardHeal, "SongoftheAncients", item))
                {
                    item.damage = 25;
                }

                if (GetItem(calBardHeal, "AnahitasArpeggio", item))
                {
                    item.damage = 144;
                }

                if (GetItem(calBardHeal, "TectonicPlates", item))
                {
                    item.damage = 80;
                }

                if (GetItem(calBardHeal, "SongoftheElements", item))
                {
                    item.damage = 77;
                }

                if (GetItem(calBardHeal, "DustDevilDrrums", item))
                {
                    item.damage = 87;
                }

                //Supercluster
                if (catalyst != null)
                {
                    if (GetItem(calBardHeal, "Supercluster", item))
                    {
                        item.damage = 60;
                    }
                }

                if (GetItem(calBardHeal, "TreeWhisperersHarp", item))
                {
                    item.damage = 115;
                }

                if (GetItem(calBardHeal, "FeralKeytar", item))
                {
                    item.damage = 150;
                }

                if (GetItem(calBardHeal, "OldDukesWisdom", item))
                {
                    item.damage = 120;
                }

                if (GetItem(calBardHeal, "SoulSplicer", item))
                {
                    item.damage = 90;
                }

                if (GetItem(calBardHeal, "FaceMelter", item))
                {
                    item.damage = 65;
                }

                if (GetItem(calBardHeal, "ChristmasCarol", item))
                {
                    item.damage = 250;
                }

                if (GetItem(calBardHeal, "SpookyMonth", item))
                {
                    item.damage = 266;
                    item.useTime = 4;
                    item.useAnimation = 16;
                }

                if (GetItem(calBardHeal, "DoomsdayCatharsis", item))
                {
                    item.damage = 90;
                }

                if (GetItem(calBardHeal, "SongoftheCosmos", item))
                {
                    item.damage = 190;
                }

                if (GetItem(calBardHeal, "SymphonicFabrications", item))
                {
                    item.damage = 140;
                }

                if (GetItem(calBardHeal, "Gashadokuro", item))
                {
                    item.damage = 333;
                }

                if (calHunt != null)
                {
                    if (GetItem(calBardHeal, "HarmonyoftheOldGod", item))
                    {
                        item.crit = 16;
                    }
                }
                #endregion
                #endregion

                #region Armor
                if (catalyst != null)
                {
                    if (GetItem(calBardHeal, "IntergelacticCloche", item))
                    {
                        item.defense = 44;
                    }

                    if (GetItem(calBardHeal, "IntergelacticProtectorHelm", item))
                    {
                        item.defense = 48;
                    }
                }
                #endregion
            }
            #endregion

            #region Thorium Helheim
            if (ModLoader.TryGetMod("ThoriumRework", out Mod rethorium) && ModContent.GetInstance<InfernalConfig>().ThoriumBalanceChangess)
            {
                if (GetItem(rethorium, "HeadHunter", item))
                {
                    item.damage = 60;
                }

                if (GetItem(rethorium, "BiterofSouls", item))
                {
                    item.damage = 45;
                    item.useTime = 8;
                    item.useAnimation = 8;
                }

                if (GetItem(rethorium, "EaterPiper", item))
                {
                    item.damage = 66;
                }

                if (GetItem(rethorium, "FluteofCurses", item))
                {
                    item.damage = 40;
                }

                #region Melee
                //Will of Coznix
                if (GetItem(rethorium, "BeholderBlade", item))
                {
                    item.crit = 0;
                }
                #endregion

                #region Ranged

                #endregion

                #region Summoner
                if (GetItem(rethorium, "LichWhip", item))
                {
                    item.damage = 21;
                }
                #endregion

                #region Thrower
                //Pocket Energy Storm
                if (GetItem(rethorium, "PocketEnergyStorm", item))
                {
                    item.damage = 28;
                    item.crit = 0;
                }
                #endregion

                #region Healer
                if (GetItem(rethorium, "RedCresent", item))
                {
                    item.useTime = 24;
                    item.useAnimation = 24;
                    item.damage = 45;
                }

                if (GetItem(rethorium, "LodeStoneMagnetMace", item))
                {
                    item.damage = 70;
                }

                if (GetItem(rethorium, "SoulSnatcher", item))
                {
                    item.damage = 150;
                }

                if (GetItem(rethorium, "ThoriumBludgeon", item))
                {
                    item.damage = 15;
                }

                if (GetItem(rethorium, "ValadiumHeavyScythe", item))
                {
                    item.damage = 100;
                    item.useTime = 24;
                    item.useAnimation = 24;
                }

                if (GetItem(rethorium, "TerrariumSoulScourge", item))
                {
                    item.damage = 132;
                }
                #endregion

                #region Bard
                if (GetItem(rethorium, "JellyfishJam", item))
                {
                    item.damage = 36;
                }

                if (GetItem(rethorium, "StellarTune", item))
                {
                    item.damage = 140;
                }

                if (GetItem(rethorium, "PickShapedPebble", item))
                {
                    item.defense = 0;
                }
                #endregion

                #region True
                if (GetItem(rethorium, "Oneirophobia", item))
                {
                    item.damage = 1000;
                }
                #endregion

                if (GetItem(rethorium, "FanDonations", item))
                {
                    item.defense = 0;
                }
            }
            #endregion

            #region Ragnarok
            /*
            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok) && ModContent.GetInstance<InfernalConfig>().ThoriumBalanceChangess)
            {
                if (UnsafeGetItem(ragnarok, "GraspofVoid", item))
                {
                    item.damage = 150;
                }

                if (UnsafeGetItem(ragnarok, "PhantasmalEdge", item))
                {
                    item.damage = 320;
                }

                if (UnsafeGetItem(ragnarok, "DragonForce", item))
                {
                    item.damage = 145;
                }

                #region Healer
                //Prisma
                if (item.type == ragnarok.Find<ModItem>("Prisma").Type)
                {
                    item.damage = 8;
                }

                //Equivalent Exchange
                if (item.type == ragnarok.Find<ModItem>("EquivalentExchange").Type)
                {
                    item.shootSpeed = 13;
                    item.damage = 42;
                }

                //Wind Reaver
                if (item.type == ragnarok.Find<ModItem>("WindReaver").Type)
                {
                    item.damage = 26;
                    item.useTime = 22;
                    item.useAnimation = 22;
                }

                //Marble Scythe
                if (item.type == ragnarok.Find<ModItem>("MarbleScythe").Type)
                {
                    item.damage = 21;
                    item.healLife = 3;
                }

                //Jelly Slicer
                if (item.type == ragnarok.Find<ModItem>("JellySlicer").Type)
                {
                    item.damage = 20;
                }

                //Scoria Dualscythe
                if (GetItem(ragnarok, "ScoriaDualscythe", item))
                {
                    item.damage = 65;
                }

                //Astral Ripper
                if (GetItem(ragnarok, "AstralRipper", item))
                {
                    item.useTime = 8;
                    item.useAnimation = 8;
                    item.damage = 150;
                }

                //Profaned Scythe
                if (GetItem(ragnarok, "ProfanedScythe", item))
                {
                    item.useTime = 12;
                    item.useAnimation = 12;
                    item.damage = 240;
                }

                //Verdurant Bloom
                if (GetItem(ragnarok, "VerdurantBloom", item))
                {
                    item.damage = 150;
                }

                //Splattercannon
                if (GetItem(ragnarok, "Splattercannon", item))
                {
                    item.useTime = 6;
                    item.useAnimation = 6;
                    item.damage = 200;
                }

                //Fractal
                if (GetItem(ragnarok, "Fractal", item))
                {
                    item.shootSpeed = 2;
                }

                if (UnsafeGetItem(ragnarok, "Virusprayer", item))
                {
                    item.damage = 160;
                }

                //Nightmare Freezer
                if (GetItem(ragnarok, "NightmareFreezer", item))
                {
                    item.useTime = 12;
                    item.useAnimation = 12;
                    item.damage = 210;
                }

                //Cosmic Injector
                if (GetItem(ragnarok, "CosmicInjector", item))
                {
                    item.damage = 120;
                }

                if (GetItem(ragnarok, "ExecutionerMark05", item))
                {
                    item.damage = 100;
                }

                #endregion

                #region Bard
                if (GetItem(ragnarok, "CalamityBell", item))
                {
                    item.shootSpeed = 20;
                    TrySetInspirationCost(item, 2);
                    item.useTime = 40;
                    item.useAnimation = 40;
                }

                if (GetItem(ragnarok, "Steampipes", item))
                {
                    item.shootSpeed = 20;
                }

                //Drum Stick
                if (GetItem(ragnarok, "DrumStick", item))
                {
                    item.useTime = 12;
                    item.useAnimation = 12;
                    item.damage = 110;
                }

                //Profaned Bell
                if (GetItem(ragnarok, "ProfanedBell", item))
                {
                    item.damage = 600;
                    item.useTime = 21;
                    item.useAnimation = 21;
                    TrySetInspirationCost(item, 2);
                }

                //Elysian Song
                if (GetItem(ragnarok, "ElysianSong", item))
                {
                    item.damage = 340;
                    item.useAnimation = 12;
                    item.useTime = 12;
                }

                if (GetItem(ragnarok, "RadioMic", item))
                {
                    item.damage = 435;
                    TrySetInspirationCost(item, 2);
                    item.useTime = 24;
                    item.useAnimation = 24;
                }

                if (GetItem(ragnarok, "UnbreakableCombatUkulele", item))
                {
                    item.damage = 1400;
                }

                if (GetItem(ragnarok, "Arpeggiator", item))
                {
                    item.damage = 275;
                    item.useAnimation = 5;
                }

                if (GetItem(ragnarok, "Korobeiniki", item))
                {
                    item.damage = 250;
                }

                if (UnsafeGetItem(ragnarok, "Lamentation", item))
                {
                    item.damage = 1300;
                    item.shootSpeed = 24;
                    item.useTime = 50;
                    item.useAnimation = 50;
                }
                #endregion
            }
            */
            #endregion

            #region Secrets of the Shadows
            if (ModLoader.TryGetMod("SOTS", out Mod sots) && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                #region Pre-Hardmode
                #region Melee
                //Spore Club
                if (GetItem(sots, "SporeClub", item))
                {
                    item.shootSpeed = 8;
                }

                //Crab Claw
                if (GetItem(sots, "CrabClaw", item))
                {
                    item.damage = 36;
                }

                //Spiked Club
                if (GetItem(sots, "SpikedClub", item))
                {
                    item.shootSpeed = 5;
                    item.damage = 15;
                }

                if (GetItem(sots, "MinersSword", item))
                {
                    item.scale *= 1.25f;
                }

                if (GetItem(sots, "AncientSteelSword", item))
                {
                    item.scale *= 1.25f;
                }

                if (GetItem(sots, "AncientSteelHalberd", item))
                {
                    item.damage = 18;
                    item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (GetItem(sots, "PlatinumScythe", item))
                {
                    item.scale *= 1.25f;
                }

                if (GetItem(sots, "ShatterBlade", item))
                {
                    item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (GetItem(sots, "VesperaNanDao", item))
                {
                    item.damage = 21;
                }

                if (GetItem(sots, "SoulEater", item))
                {
                    item.damage = 34;
                }

                if (GetItem(sots, "CardiacCollapse", item))
                {
                    item.damage = 48;
                }

                if (GetItem(sots, "OlympianAxe", item))
                {
                    item.damage = 42;
                    item.shootSpeed = 13;
                }

                if (GetItem(sots, "BetrayersKnife", item))
                {
                    item.damage = 30;
                    item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (GetItem(sots, "MantisGrip", item))
                {
                    item.damage = 17;
                }

                if (GetItem(sots, "IlluminantAxe", item))
                {
                    item.damage = 26;
                }

                if (GetItem(sots, "ToothAche", item))
                {
                    item.damage = 20;
                }

                if (GetItem(sots, "Vertebraeker", item))
                {
                    item.damage = 22;
                }

                if (GetItem(sots, "BlazingClub", item))
                {
                    item.damage = 25;
                    item.useTime = 30;
                    item.useAnimation = 30;
                    item.shootSpeed = 12;
                }

                if (GetItem(sots, "StarshardSaber", item))
                {
                    item.damage = 21;
                }

                if (GetItem(sots, "Hellbreaker", item))
                {
                    item.damage = 70;
                }

                if (GetItem(sots, "ObsidianEruption", item))
                {
                    item.damage = 21;
                }

                if (GetItem(sots, "ZephyrousZeppelin", item))
                {
                    item.damage = 24;
                }

                if (GetItem(sots, "Aten", item))
                {
                    item.damage = 34;
                }

                if (GetItem(sots, "WormWoodCollapse", item))
                {
                    item.damage = 64;
                }

                if (GetItem(sots, "ImperialPike", item))
                {
                    item.damage = 13;
                }

                if (GetItem(sots, "CursedImpale", item))
                {
                    item.shootSpeed = 8;
                    //item.useTime = 17;
                    //item.useAnimation = 17;
                }

                if (GetItem(sots, "JeweledGauntlet", item))
                {
                    item.damage = 46;
                }

                if (GetItem(sots, "BoneClapper", item))
                {
                    item.damage = 70;
                }

                if (GetItem(sots, "EarthGrinder", item))
                {
                    item.damage = 34;
                }

                if (GetItem(sots, "GuardianGreatsword", item))
                {
                    item.damage = 32;
                }

                if (GetItem(sots, "SupernovaHammer", item))
                {
                    item.crit = 6;
                }

                if (GetItem(sots, "DigitalDaito", item))
                {
                    item.damage = 27;
                }
                #endregion

                #region Ranged
                if (GetItem(sots, "PintOPunch", item))
                {
                    item.damage = 16;
                    item.crit = 0;
                }

                if (GetItem(sots, "PlatinumDart", item))
                {
                    item.damage = 25;
                }

                if (GetItem(sots, "GlazeBow", item))
                {
                    item.damage = 13;
                }

                if (GetItem(sots, "AncientSteelLongbow", item))
                {
                    item.damage = 21;
                    item.useTime = 40;
                    item.useAnimation = 40;
                }

                if (GetItem(sots, "GlazeBow", item))
                {
                    item.damage = 18;
                }

                if (GetItem(sots, "EchoDisk", item))
                {
                    item.damage = 14;
                }

                if (GetItem(sots, "VibrantPistol", item))
                {
                    item.damage = 21;
                }

                if (GetItem(sots, "VibrantCannon", item))
                {
                    item.damage = 15;
                    item.useTime = 70;
                    item.useAnimation = 70;
                }

                if (GetItem(sots, "CryoCannon", item))
                {
                    item.damage = 12;
                }

                if (GetItem(sots, "ExplosiveKnife", item))
                {
                    item.damage = 7;
                }

                if (GetItem(sots, "MineralSpewer", item))
                {

                }

                if (GetItem(sots, "ThundershockShortbow", item))
                {
                    item.damage = 40;
                }

                if (GetItem(sots, "SharkPog", item))
                {
                    item.damage = 7;
                }

                if (GetItem(sots, "Icebreaker", item))
                {
                    item.damage = 15;
                }

                if (GetItem(sots, "SandstormPouch", item))
                {
                    item.damage = 21;
                }

                if (GetItem(sots, "CoconutGun", item))
                {
                    item.damage = 8;
                }

                if (GetItem(sots, "IlluminantBow", item))
                {
                    item.damage = 12;
                }

                if (GetItem(sots, "RoyalMagnum", item))
                {
                    item.damage = 7;
                }

                if (GetItem(sots, "WormWoodHelix", item))
                {
                    item.useTime = 21;
                    item.useAnimation = 21;
                }

                if (GetItem(sots, "Sharanga", item))
                {
                    item.damage = 35;
                }

                if (GetItem(sots, "FortressCrasher", item))
                {
                    item.useTime = 26;
                    item.useAnimation = 26;
                }

                if (GetItem(sots, "FlareDetonator", item))
                {
                    item.damage = 27;
                }

                if (GetItem(sots, "ArclightOrbs", item))
                {
                    item.damage = 15;
                }

                if (GetItem(sots, "SkywardBlades", item))
                {
                    item.damage = 28;
                    item.crit = 0;
                }

                if (GetItem(sots, "CataclysmDisc", item))
                {
                    item.damage = 16;
                }
                #endregion

                #region Magic
                if (GetItem(sots, "VibrantStaff", item))
                {
                    item.useTime = 18;
                    item.useAnimation = 18;
                    item.damage = 18;
                }

                if (GetItem(sots, "ShardStaff", item))
                {
                    item.damage = 18;
                }

                if (GetItem(sots, "StormSpell", item))
                {
                    item.damage = 16;
                }

                if (GetItem(sots, "PlatinumSoulStaff", item))
                {
                    item.damage = 12;
                }

                if (GetItem(sots, "PinkJellyfishStaff", item))
                {
                    item.damage = 13;
                }

                if (GetItem(sots, "PiscesPuncher", item))
                {
                    item.damage = 16;
                }

                if (GetItem(sots, "IlluminantStaff", item))
                {
                    item.damage = 13;
                }

                if (GetItem(sots, "PlagueSpitter", item))
                {
                    item.useTime = 12;
                    item.useAnimation = 12;
                }

                if (GetItem(sots, "Blongus", item))
                {
                    item.useTime = 22;
                    item.useAnimation = 22;
                }

                if (GetItem(sots, "PhotonGeyser", item))
                {
                    item.damage = 20;
                }

                if (GetItem(sots, "WormWoodStaff", item))
                {
                    item.damage = 24;
                }

                if (GetItem(sots, "CurseballTome", item))
                {
                    item.damage = 26;
                }

                if (GetItem(sots, "CrescentStaff", item))
                {
                    item.damage = 26;
                }

                if (GetItem(sots, "ArcStaffMk2", item))
                {
                    item.damage = 80;
                }
                #endregion

                #region Summon
                if (GetItem(sots, "BrassWhip", item))
                {
                    item.damage = 14;
                }

                if (GetItem(sots, "KelpWhip", item))
                {
                    item.useTime = 34;
                    item.useAnimation = 34;
                }

                if (GetItem(sots, "JarOfPineapple", item))
                {
                    item.damage = 13;
                }

                if (GetItem(sots, "NightIlluminator", item))
                {
                    item.damage = 13;
                }

                if (GetItem(sots, "CursedBlade", item))
                {
                    item.damage = 25;
                }

                if (GetItem(sots, "OtherworldlySpiritStaff", item))
                {
                    item.damage = 30;
                }
                #endregion

                #region Accessories

                #endregion
                #endregion
                if (GetItem(sots, "FlowerCrown", item))
                {
                    item.defense = 3;
                }
                #region Hardmode
                #region Melee
                if (GetItem(sots, "Colossus", item))
                {
                    item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (GetItem(sots, "AquaticEclipse", item))
                {
                    item.damage = 102;
                }

                if (GetItem(sots, "AbyssalFury", item))
                {
                    item.damage = 21;
                }

                if (GetItem(sots, "Pulverizer", item))
                {
                    item.damage = 25;
                }

                if (GetItem(sots, "IrradiatedChainReactor", item))
                {
                    item.damage = 80;
                }

                if (GetItem(sots, "Sawflake", item))
                {
                    item.damage = 90;
                }

                if (GetItem(sots, "Helios", item))
                {
                    item.damage = 80;
                }

                if (GetItem(sots, "Pyrocide", item))
                {
                    item.damage = 96;
                }

                if (GetItem(sots, "DuneSplicer", item))
                {
                    item.damage = 30;
                }

                if (GetItem(sots, "KingBlade", item))
                {
                    item.damage = 31;
                }

                if (GetItem(sots, "SkipScythe", item))
                {
                    item.crit = 0;
                    item.damage = 85;
                }

                if (GetItem(sots, "RealityShatter", item))
                {
                    item.damage = 115;
                    item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (GetItem(sots, "PlasmaCutterButOnAChain", item))
                {
                    item.damage = 260;
                }

                if (GetItem(sots, "SubspaceScissors", item))
                {
                    item.damage = 220;
                }
                #endregion

                #region Ranged
                if (GetItem(sots, "TorchGunMk2", item))
                {
                    item.damage = 10;
                    item.useTime = 100;
                    item.useAnimation = 100;
                }

                if (GetItem(sots, "GhoulBlaster", item))
                {
                    item.damage = 22;
                }

                if (GetItem(sots, "HypericeClusterCannon", item))
                {
                    item.damage = 28;
                }

                if (GetItem(sots, "PBow", item))
                {
                    item.damage = 38;
                }

                if (GetItem(sots, "AccretionDisc", item))
                {
                    item.damage = 16;
                    item.useTime = 60;
                    item.useAnimation = 60;
                }

                if (GetItem(sots, "Traingun", item))
                {
                    item.damage = 80;
                }

                if (GetItem(sots, "RebarRifle", item))
                {
                    item.useTime = 26;
                    item.useAnimation = 26;
                }

                if (GetItem(sots, "PathogenRegurgitator", item))
                {
                    item.damage = 55;
                }

                if (GetItem(sots, "TheBlaspha", item))
                {
                    item.damage = 35;
                }

                if (GetItem(sots, "SpectreSpiritStorm", item))
                {
                    item.damage = 50;
                }

                if (GetItem(sots, "SupernovaScatter", item))
                {
                    item.useTime = 120;
                    item.useAnimation = 120;
                    item.damage = 15;
                }

                if (GetItem(sots, "IcicleImpale", item))
                {
                    item.damage = 55;
                }

                if (GetItem(sots, "ChaosChamber", item))
                {
                    item.damage = 55;
                }

                if (GetItem(sots, "RoseBow", item))
                {
                    item.damage = 72;
                }

                if (GetItem(sots, "StellarSerpentLauncher", item))
                {
                    item.damage = 30;
                }

                if (GetItem(sots, "CataclysmSpheres", item))
                {
                    item.damage = 40;
                }

                if (GetItem(sots, "DimensionShredder", item))
                {
                    item.useTime = 5;
                    item.useAnimation = 5;
                    item.damage = 32;
                }
                #endregion

                #region Magic
                if (GetItem(sots, "PurpleJellyfishStaff", item))
                {
                    item.damage = 22;
                }

                if (GetItem(sots, "TomeOfTheReaper", item))
                {
                    item.damage = 66;
                }

                if (GetItem(sots, "BiomassBarrage", item))
                {
                    item.damage = 38;
                }

                if (GetItem(sots, "BrachialLance", item))
                {
                    item.damage = 76;
                }

                if (GetItem(sots, "TangleStaff", item))
                {
                    item.damage = 120;
                }

                if (GetItem(sots, "Revolution", item))
                {
                    item.damage = 78;
                }

                if (GetItem(sots, "HyperlightGeyser", item))
                {
                    item.damage = 35;
                }

                if (GetItem(sots, "SupernovaStorm", item))
                {
                    item.damage = 52;
                }

                if (GetItem(sots, "DanceOfDeath", item))
                {
                    item.damage = 53;
                }

                if (GetItem(sots, "Apocalypse", item))
                {
                    item.damage = 180;
                }
                #endregion

                #region Summon
                if (GetItem(sots, "TidalSpiritStaff", item))
                {
                    item.damage = 70;
                }

                if (GetItem(sots, "EvilSpiritStaff", item))
                {
                    item.damage = 46;
                }

                if (GetItem(sots, "CruiseCaller", item))
                {
                    item.damage = 50;
                }

                if (GetItem(sots, "StarcallerStaff", item))
                {
                    item.damage = 86;
                }

                if (GetItem(sots, "ChaosSpiritStaff", item))
                {
                    item.damage = 27;
                }

                if (GetItem(sots, "EtherealScepter", item))
                {
                    item.damage = 96;
                }

                if (GetItem(sots, "Lemegeton", item))
                {
                    item.damage = 46;
                }
                #endregion
                #endregion
            }
            #endregion

            #region SOTS Bard, Thrower, & Healer Balance Changes
            if (ModLoader.TryGetMod("SOTSBardHealer", out Mod sotsBH) && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (GetItem(sotsBH, "VibrantTomahawk", item))
                {
                    item.damage = 50;
                }

                if (GetItem(sotsBH, "GoopwoodWiggle", item))
                {
                    item.shootSpeed = 8;
                    item.damage = 21;
                    item.useTime = 6;
                    item.useAnimation = 12;
                }

                if (GetItem(sotsBH, "GoopwoodSplit", item))
                {
                    item.damage = 52;
                }

                if (GetItem(sotsBH, "GoopwoodReap", item))
                {
                    item.damage = 33;
                }

                if (GetItem(sotsBH, "AncientHarp", item))
                {
                    item.shootSpeed = 8;
                    item.damage = 34;
                    item.useTime = 4;
                    item.useAnimation = 12;
                }

                if (GetItem(sotsBH, "ForbiddenMaelstrom", item))
                {
                    item.damage = 44;
                }

                if (GetItem(sotsBH, "CursedAnkhStaff", item))
                {
                    item.damage = 45;
                }

                if (GetItem(sotsBH, "BladedSnowflake", item))
                {
                    item.damage = 57;
                }

                if (GetItem(sotsBH, "TurboSlicer", item))
                {
                    item.damage = 104;
                }

                if (GetItem(sotsBH, "UltrasonicBassBooster", item))
                {
                    item.damage = 70;
                }

                if (GetItem(sotsBH, "TwilightAscendance", item))
                {
                    item.damage = 175;
                }

                if (GetItem(sotsBH, "SerpentCharmerofSage", item))
                {
                    item.damage = 24;
                }

                if (GetItem(sotsBH, "Serpentbite", item))
                {
                    item.crit = 6;
                    item.damage = 66;
                }

                if (GetItem(sotsBH, "ChoirofOne", item))
                {
                    item.damage = 160;
                }

                if (GetItem(sotsBH, "PromiseofDemise", item))
                {
                    item.damage = 402;
                }

                if (GetItem(sotsBH, "DeathThroesThrows", item))
                {
                    item.damage = 508;
                }
            }
            #endregion

            #region Consolaria
            if (ModLoader.TryGetMod("Consolaria", out Mod console) && ModContent.GetInstance<InfernalConfig>().ConsolariaBalanceChanges)
            {
                if (UnsafeGetItem(console, "AlbinoMandible", item))
                {
                    item.damage = 20;
                }

                if (UnsafeGetItem(console, "RomanCandle", item))
                {
                    item.useTime = 30;
                    item.useAnimation = 30;
                }

                if (UnsafeGetItem(console, "EggCannon", item))
                {
                    item.useTime = 16;
                    item.useAnimation = 16;
                    item.damage = 24;
                }

                if (UnsafeGetItem(console, "GreatDrumstick", item))
                {
                    item.scale *= 1.5f;
                    item.useTime = 26;
                    item.useAnimation = 26;
                    item.damage = 62;
                }

                if (UnsafeGetItem(console, "FeatherStorm", item))
                {
                    item.damage = 35;
                }

                if (UnsafeGetItem(console, "DragonBreath", item))
                {
                    item.damage = 44;
                }

                if (UnsafeGetItem(console, "VolcanicRepeater", item))
                {
                    item.damage = 36;
                }

                if (UnsafeGetItem(console, "EternityStaff", item))
                {
                    item.damage = 42;
                }

                if (UnsafeGetItem(console, "OcramsEye", item))
                {
                    item.useTime = 7;
                    item.useAnimation = 21;
                }

                if (UnsafeGetItem(console, "Tizona", item))
                {
                    item.useTime = 18;
                    item.useAnimation = 18;
                    item.damage = 87;
                }

                if (UnsafeGetItem(console, "Tonbogiri", item))
                {
                    item.useTime = 26;
                    item.useAnimation = 26;
                    item.damage = 132;
                    item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (thorium != null)
                {
                    if (GetItem(console, "ScytheFantasma", item))
                    {
                        item.damage = 140;
                    }

                    if (GetItem(console, "Omunikodo", item))
                    {
                        item.damage = 70;
                    }

                    if (GetItem(console, "UtensilPoker", item))
                    {
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
                    }
                }
            }
            #endregion

            if (damage != item.damage || pick != item.pick || useTime != item.useTime || useAnimation != item.useAnimation || shootSpeed != item.shootSpeed || crit != item.crit || defense != item.defense)
                modified = true;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (ModLoader.TryGetMod("SOTS", out Mod sots) && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (GetItem(sots, "FlowerCrown", item))
                {
                    player.lifeRegen -= 2;
                }
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (InfernalConfig.Instance.ThoriumBalanceChangess && InfernalCrossmod.Thorium.Loaded)
            {
                if (item.type == FindItem(InfernalCrossmod.Thorium.Mod, "PollenPike"))
                {
                    if (player.altFunctionUse == 2)
                    {
                        item.useTime = 25;
                        item.useAnimation = 25;
                        item.shootSpeed = 9;
                    }
                    else
                    {
                        item.useTime = 12;
                        item.useAnimation = 12;
                        item.shootSpeed = 12;
                    }
                }
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (modified)
            {
                Item clone = new();
                clone.SetDefaults(item.type);

                float damageBalance = clone.damage / damage;
                float pickBalance = clone.pick - pick;
                float useTimeBalance = clone.useTime / useTime;
                float useAnimationBalance = clone.useAnimation / useAnimation;
                float shootSpeedBalance = clone.shootSpeed / shootSpeed;
                float critBalance = clone.crit - crit;
                float defenseBalnce = clone.defense - defense;

                Color InfernalRed = Color.Lerp(
                   Color.White,
                   new Color(255, 80, 0), // Infernal red/orange
                   (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
                );

                int maxTooltipIndex = -1;
                int maxNumber = -1;
                var key = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus["SmartCursor"].First().ToString();

                // Find the TooltipLine with the highest TooltipX name
                bool foundTooltips = false;
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.StartsWith("Tooltip"))
                    {
                        foundTooltips = true;
                        if (int.TryParse(tooltips[i].Name.Substring(7), out int num) && num > maxNumber)
                        {
                            maxNumber = num;
                            maxTooltipIndex = i;
                        }
                    }

                    if (i == tooltips.Count -1 && !foundTooltips)
                        maxTooltipIndex = i;
                }

                int insertIndex = maxTooltipIndex + 1;

                if (maxTooltipIndex != -1)
                {
                    if (PlayerInput.Triggers.Current.SmartCursor)
                    {
                        TooltipLine balanceChange = new(Mod, "IEoR:BalanceHeader", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemBalance.Modified")}:") { OverrideColor = InfernalRed };
                        tooltips.Insert(insertIndex, balanceChange);

                        insertIndex++;

                        TooltipLine damageChange = null;
                        if (damageBalance > 1)
                            damageChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.DamageUpGeneric").Format(Math.Round((damageBalance - 1) * 100)));
                        else if (damageBalance < 1)
                            damageChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.DamageDownGeneric").Format(Math.Round((1 - damageBalance) * 100)));
                        if (damageChange != null)
                        {
                            tooltips.Insert(insertIndex, damageChange);
                            insertIndex++;
                        }

                        TooltipLine pickChange = null;
                        if (pickBalance > 0)
                            pickChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.PickUpGeneric").Format(pickBalance));
                        if (pickBalance < 0)
                            pickChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.PickDownGeneric").Format(-pickBalance));
                        if (pickChange != null)
                        {
                            tooltips.Insert(insertIndex, pickChange);
                            insertIndex++;
                        }

                        if (useTimeBalance == useAnimationBalance)
                        {
                            TooltipLine useSpeedChange = null;
                            if (useTimeBalance < 1)
                                useSpeedChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.UseSpeedUpGeneric").Format(Math.Round(-(useTimeBalance - 1) * 100)));
                            else if (useTimeBalance > 1)
                                useSpeedChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.UseSpeedDownGeneric").Format(Math.Round((1 - useTimeBalance) * 100)));
                            if (useSpeedChange != null)
                            {
                                tooltips.Insert(insertIndex, useSpeedChange);
                                insertIndex++;
                            }
                        }
                        else
                        {
                            TooltipLine useTimeChange = null;
                            if (useTimeBalance < 1)
                                useTimeChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.UseTimeUpGeneric").Format(Math.Round(-(useTimeBalance - 1) * 100)));
                            else if (useTimeBalance > 1)
                                useTimeChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.UseTimeDownGeneric").Format(Math.Round((1 - useTimeBalance) * 100)));
                            if (useTimeChange != null)
                            {
                                tooltips.Insert(insertIndex, useTimeChange);
                                insertIndex++;
                            }

                            TooltipLine useAniChange = null;
                            if (useAnimationBalance < 1)
                                useAniChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.UseAnimationUpGeneric").Format(Math.Round(-(useTimeBalance - 1) * 100)));
                            else if (useAnimationBalance > 1)
                                useAniChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.UseAnimationDownGeneric").Format(Math.Round((1 - useTimeBalance) * 100)));
                            if (useAniChange != null)
                            {
                                tooltips.Insert(insertIndex, useAniChange);
                                insertIndex++;
                            }
                        }

                        TooltipLine shootSpeedChange = null;
                        if (shootSpeedBalance > 1)
                            shootSpeedChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.ShootSpeedUpGeneric").Format(Math.Round((shootSpeedBalance - 1) * 100)));
                        else if (shootSpeedBalance < 1)
                            shootSpeedChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.ShootSpeedDownGeneric").Format(Math.Round(-(1 - shootSpeedBalance) * 100)));
                        if (shootSpeedChange != null)
                        {
                            tooltips.Insert(insertIndex, shootSpeedChange);
                            insertIndex++;
                        }

                        TooltipLine critChange = null;
                        if (critBalance > 0)
                            critChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.CritUpGeneric").Format(critBalance));
                        else if (critBalance < 0)
                            critChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.CritDownGeneric").Format(-critBalance));
                        if (critChange != null)
                        {
                            tooltips.Insert(insertIndex, critChange);
                            insertIndex++;
                        }

                        TooltipLine defenseChange = null;
                        if (defenseBalnce > 0)
                            defenseChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.DefenseUpGeneric").Format(defenseBalnce));
                        else if (defenseBalnce < 0)
                            defenseChange = new(Mod, "IEoR:BalanceChange", Language.GetText("Mods.InfernalEclipseAPI.ItemBalance.DefenseDownGeneric").Format(-defenseBalnce));
                        if (defenseChange != null)
                        {
                            tooltips.Insert(insertIndex, defenseChange);
                            //insertIndex++;
                        }
                    }
                    else
                    {
                        TooltipLine balanceChange = new(Mod, "IEoR:BalanceHeader", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemBalance.Modified")) { OverrideColor = InfernalRed };
                        tooltips.Insert(insertIndex, balanceChange);

                        insertIndex++;

                        TooltipLine hint = new(Mod, "IEoR:BalanceHint", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemBalance.Modified2", key)) { OverrideColor = new Color(170, 170, 170) };
                        tooltips.Insert(insertIndex, hint);
                    }
                }
            }
        }

        public static int FindItem(Mod mod, string name)
        {
            return mod.Find<ModItem>(name).Type;
        }

        public static bool GetItem(Mod mod, string name, Item item)
        {
            ModItem foundItem;
            ModLoader.TryGetMod("CalamityBardHealer", out Mod calBardHealer);
            ModLoader.TryGetMod("ThoriumMod", out Mod thorium);
            ModLoader.TryGetMod("ThoriumRework", out Mod thorRework);
            if (mod == calBardHealer || mod == thorium || mod == thorRework)
            {
                if (!mod.TryFind(name, out foundItem))
                    return false;
            }
            else
            {
                foundItem = mod.Find<ModItem>(name);
            }

            if (item.type == foundItem.Type)
            {
                return true;
            }
            return false;
        }

        //used for testing
        public static bool UnsafeGetItem(Mod mod, string name, Item item)
        {
            ModItem foundItem;
            foundItem = mod.Find<ModItem>(name);

            if (item.type == foundItem.Type)
            {
                return true;
            }
            return false;
        }

        //method by Wardrobe Hummus
        private void TrySetInspirationCost(Item item, int newCost)
        {
            try
            {
                if (item.ModItem == null)
                {
                    Main.NewText("No ModItem attached");
                    return;
                }

                Type modItemType = item.ModItem.GetType();

                // Try field first
                FieldInfo field = modItemType.GetField("InspirationCost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(item.ModItem, newCost);
                    Main.NewText($"[Field] Set InspirationCost of {item.Name} to {newCost}");
                    return;
                }

                // Then try property
                PropertyInfo prop = modItemType.GetProperty("InspirationCost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(item.ModItem, newCost);
                    return;
                }

                Main.NewText("InspirationCost not found on ModItem.");
            }
            catch (Exception ex)
            {
                Main.NewText($"Error setting InspirationCost: {ex.Message}");
            }
        }
        //Refactored by Akira
        private void TrySetHealAmmount(Item item, int newCost)
        {
            try
            {
                if (item.ModItem == null)
                {
                    Main.NewText("No ModItem attached");
                    return;
                }

                Type modItemType = item.ModItem.GetType();

                // Try field first
                FieldInfo field = modItemType.GetField("healAmount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(item.ModItem, newCost);
                    //Main.NewText($"[Field] Set healAmount of {item.Name} to {newCost}");
                    return;
                }

                // Then try property
                PropertyInfo prop = modItemType.GetProperty("healAmount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(item.ModItem, newCost);
                    return;
                }

                //Main.NewText("healAmount not found on ModItem.");
            }
            catch (Exception)
            {
                //Main.NewText($"Error setting healAmount: {ex.Message}");
            }
        }

        private void TrySetAccessoryDamage(Item item, string newDamage)
        {
            try
            {
                if (item.ModItem == null)
                {
                    Main.NewText("No ModItem attached");
                    return;
                }

                Type modItemType = item.ModItem.GetType();

                // Try field first
                FieldInfo field = modItemType.GetField("accDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(item.ModItem, newDamage);
                    //Main.NewText($"[Field] Set healAmount of {item.Name} to {newCost}");
                    return;
                }

                // Then try property
                PropertyInfo prop = modItemType.GetProperty("accDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(item.ModItem, newDamage);
                    return;
                }

                //Main.NewText("healAmount not found on ModItem.");
            }
            catch (Exception)
            {
                //Main.NewText($"Error setting healAmount: {ex.Message}");
            }
        }
    }
}
