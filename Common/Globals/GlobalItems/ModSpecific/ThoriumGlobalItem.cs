using System.Collections.Generic;
using System.Reflection;
using CalamityMod;
using CalamityMod.Enums;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Weapons.Melee;
using InfernalEclipseAPI.Content.Items.Materials;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Systems.Hooks.ILItemChanges.ThoriumItemHooks;
using InfernalEclipseAPI.Core.Utils;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using ThoriumMod;
using ThoriumMod.Items.ArcaneArmor;
using ThoriumMod.Items.BasicAccessories;
using ThoriumMod.Items.BossFallenBeholder;
using ThoriumMod.Items.BossForgottenOne;
using ThoriumMod.Items.BossGraniteEnergyStorm;
using ThoriumMod.Items.BossLich;
using ThoriumMod.Items.BossThePrimordials;
using ThoriumMod.Items.BossThePrimordials.Aqua;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.BossThePrimordials.Omni;
using ThoriumMod.Items.BossThePrimordials.Slag;
using ThoriumMod.Items.Bronze;
using ThoriumMod.Items.Consumable;
using ThoriumMod.Items.Coral;
using ThoriumMod.Items.Cultist;
using ThoriumMod.Items.DemonBlood;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.Dread;
using ThoriumMod.Items.Flesh;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.Icy;
using ThoriumMod.Items.Misc;
using ThoriumMod.Items.NPCItems;
using ThoriumMod.Items.Sandstone;
using ThoriumMod.Items.SummonItems;
using ThoriumMod.Items.Terrarium;
using ThoriumMod.Items.Thorium;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Items.Titan;
using ThoriumMod.Items.Valadium;
using ThoriumMod.Utilities;
using static Terraria.ModLoader.ModContent;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ThoriumCrateFix : GlobalItem
    {
        public override bool InstancePerEntity => false;

        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            Fraction fifteenPercent = new Fraction(15, 100);

            if (item.type == ItemType<WondrousCrate>())
            {
                RemoveHardmodeOresFromStandardCrates(loot);
                RemoveHardmodeOresFromBiomeCrates(loot);
                loot.AddHardmodeOresToCrates(HardmodeCrateType.Titanium);
            }

            if (item.type == ItemType<AbyssalCrate>() || item.type == ItemType<SinisterCrate>())
            {
                RemoveHardmodeOresFromStandardCrates(loot);
                RemoveHardmodeOresFromBiomeCrates(loot);
                loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
            }
        }

        private static void RemoveHardmodeOresFromStandardCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);

            // This is the primary rule which contains every drop
            AlwaysAtleastOneSuccessDropRule mainRule = null;
            foreach (IItemDropRule rule in rules)
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    mainRule = a;
            if (mainRule is null)
                return;

            // Find ones that are supposed to be for the ore and not the other loot
            foreach (IItemDropRule rule in mainRule.rules)
            {
                // Hardmode ores/bars are both nested within *another* nested rule
                if (rule is SequentialRulesNotScalingWithLuckRule oreRule)
                {
                    // Confirm that this is for the ore/bar then pop the numerator for the big rule
                    foreach (IItemDropRule nestedRule in oreRule.rules)
                    {
                        if (nestedRule is SequentialRulesNotScalingWithLuckRule s)
                        {
                            oreRule.chanceNumerator = 0;
                            return;
                        }
                    }
                }
            }
        }

        private static void RemoveHardmodeOresFromBiomeCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);

            // This is the primary rule which contains every drop
            AlwaysAtleastOneSuccessDropRule mainRule = null;
            foreach (IItemDropRule rule in rules)
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    mainRule = a;
            if (mainRule is null)
                return;

            foreach (IItemDropRule rule in mainRule.rules)
            {
                // 2 rules, one for ore and another for bar, nested within *another* nested rule
                if (rule is SequentialRulesNotScalingWithLuckRule oreRule)
                {
                    // Confirm that this is for the ore/bar then pop the numerator for the big rule
                    foreach (IItemDropRule nestedRule in oreRule.rules)
                    {
                        if (nestedRule is OneFromRulesRule o)
                            oreRule.chanceNumerator = 0;
                    }
                }
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ThoriumGlobalItem : GlobalItem
    {

        public override void SetDefaults(Item item)
        {
            if (item.type == ItemType<IceLance>() ||
                item.type == ItemType<fSandStoneSpear>() ||
                item.type == ItemType<Fork>() ||
                item.type == ItemType<CoralPolearm>() ||
                //item.type == ItemType<HarpyTalon>() ||
                item.type == ItemType<PearlPike>() ||
                item.type == ItemType<Moonlight>() ||
                item.type == ItemType<EnergyStormPartisan>() ||
                item.type == ItemType<FleshSkewer>() ||
                item.type == ItemType<HellishHalberd>() ||
                item.type == ItemType<ValadiumSpear>() ||
                item.type == ItemType<BloodGlory>()
    )
            {
                item.DamageType = GetInstance<TrueMeleeDamageClass>();
            }

            if (InfernalConfig.Instance.ChanageWeaponClasses) 
            {
                if (item.type == ItemID.TheAxe)
                {
                    item.DamageType = ThoriumDamageBase<BardDamage>.Instance;
                }

                if (item.type == ItemType<AncientFlame>())
                {
                    item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                    item.damage = 32;
                }

                if (item.type == ItemType<TheBurningSky>())
                {
                    item.damage = 259;
                    item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                    item.mana = 35;
                }
            }

            if (InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (item.type == ItemType<ShadeMasterMask>())
                {
                    item.defense = 10;
                }
                if (item.type == ItemType<ShadeMasterTreads>())
                {
                    item.defense = 12;
                }
                if (item.type == ItemType<ShadeMasterGarb>())
                {
                    item.defense = 16;
                }

                if (item.type == ItemType<CoralHelmet>())
                {
                    item.defense = 4;
                }
                if (item.type == ItemType<CoralChestGuard>())
                {
                    item.defense = 5;
                }
                if (item.type == ItemType<CoralGreaves>())
                {
                    item.defense = 4;
                }

                if (item.type == ItemType<IridescentHelmet>())
                {
                    item.defense = 5;
                }
                if (item.type == ItemType<IridescentMail>())
                {
                    item.defense = 6;
                }

                if (item.type == ItemType<WhisperingHood>())
                {
                    item.defense = 10;
                }
                if (item.type == ItemType<WhisperingTabard>())
                {
                    item.defense = 20;
                }
                if (item.type == ItemType<WhisperingLeggings>())
                {
                    item.defense = 14;
                }

                if (item.type == ItemID.BreakerBlade)
                {
                    SheathCompatibilitySystem.SetIncompatible(item.type);
                }
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ItemID.EyeoftheGolem)
                player.Calamity().critDamage -= 0.15f;

            if (item.type == ItemType<MaskoftheCrystalEye>())
            {
                player.GetThoriumPlayer().bonusCritDamage -= 0.12f;
                if (!InfernalCrossmod.SOTS.Loaded)
                    player.Calamity().critDamage += 0.15f;
            }

            if (item.type == ItemType<TravelersBoots>())
            {
                player.runAcceleration -= 0.06f;
            }

            if (item.type == ItemType<GutWrenchersGauntlet>())
            {
                player.GetThoriumPlayer().gutWrench = false;
                if (player.statLife <= player.statLifeMax2 * 0.5)
                {
                    player.GetModPlayer<InfernalPlayer>().gutWrench = true;
                }
            }

            if (item.type == ItemType<SteamkeeperWatch>())
            {
                player.GetDamage(DamageClass.Summon) -= SteamkeeperWatch.DamageIncrease / 100f;
            }

            if (item.type == ItemType<YumasPendant>())
            {
                player.GetDamage(DamageClass.Summon) -= 0.05f;
            }

            if (item.type == ItemType<PlagueLordFlask>())
            {
                player.GetDamage(DamageClass.Throwing) += 0.05f;
                player.ThrownVelocity += 0.05f;
                player.GetAttackSpeed(DamageClass.Throwing) -= 0.1f;
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (InfernalConfig.Instance.ThoriumBalanceChangess && !InfernalCrossmod.Hummus.Loaded)
            {
                if (item.type == ItemType<CrystalHoney>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.05f;
                }

                if (item.type == ItemType<DemonTongue>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.02f;
                }

                if (item.type == ItemType<DarkGlaze>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.05f;
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 5f;
                }

                if (item.type == ItemType<ArchDemonCurse>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.08f;
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 7f;
                }

                if (item.type == ItemType<MasterMarksmansScouter>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.bulletDamage -= 0.24f;
                    player.GetAttackSpeed(DamageClass.Ranged) -= 0.12f;
                }

                if (item.type == ItemType<MasterArbalestHood>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.GetArmorPenetration(DamageClass.Ranged) -= 100f;
                }

                if (item.type == ItemType<AssassinsGuard>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.GetAttackSpeed(DamageClass.Ranged) -= 0.2f;
                    player.GetDamage(DamageClass.Ranged).Flat -= 20f;
                }

                if (item.type == ItemType<MagmaSeersMask>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.GetDamage(DamageClass.Summon).Flat -= 30f;
                    player.maxMinions -= 4;
                    player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) -= 0.2f;
                }

                if (item.type == ItemType<PyromancerTabard>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.whipRangeMultiplier -= 0.50f;
                    player.maxMinions -= 1;
                }

                if (item.type == ItemType<FlightMask>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.03f;
                    player.GetCritChance(DamageClass.Throwing) -= 1f;
                }

                if (item.type == ItemType<FlightMail>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.07f;
                }

                if (item.type == ItemType<BronzeHelmet>())
                {
                    player.GetCritChance(DamageClass.Throwing) -= 4f;
                }
                if (item.type == ItemType<BronzeBreastplate>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.1f;
                }
                if (item.type == ItemType<BronzeGreaves>())
                {
                    player.GetDamage(DamageClass.Throwing) += 0.1f;
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.1f;
                }

                if (item.type == ItemType<PlagueDoctorsMask>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.07f;
                }
                if (item.type == ItemType<PlagueDoctorsGarb>())
                {
                    player.GetDamage(DamageClass.Throwing) += 0.01f;
                }
                if (item.type == ItemType<PlagueDoctorsLeggings>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.04f;
                }

                if (item.type == ItemType<FungusHat>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.02f;
                }
                if (item.type == ItemType<FungusGuard>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.02f;
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.05f;
                }

                if (item.type == ItemType<HallowedGuise>() || item.type == ItemType<AncientHallowedGuise>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.15f;
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.08f;
                }

                if (item.type == ItemType<LichCarapace>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.05f;
                }
                if (item.type == ItemType<LichTalon>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.025f;
                    player.GetCritChance(DamageClass.Throwing) -= 5f;
                }

                if (item.type == ItemType<ShadeMasterMask>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.05f;
                }
                if (item.type == ItemType<ShadeMasterGarb>())
                {
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.1f;
                }
                if (item.type == ItemType<ShadeMasterTreads>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.025f;
                    player.GetCritChance(DamageClass.Throwing) -= 5f;
                }

                if (item.type == ItemType<WhiteDwarfMask>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.05f;
                }
                if (item.type == ItemType<WhiteDwarfGuard>())
                {
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.15f;
                }
                if (item.type == ItemType<WhiteDwarfGreaves>())
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.05f;
                }

                if (item.type == ItemType<TideTurnersGaze>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.GetDamage(DamageClass.Throwing) += 0.10f;
                }
                if (item.type == ItemType<TideTurnerBreastplate>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.10f;
                }
                if (item.type == ItemType<TideTurnerGreaves>() && InfernalCrossmod.ThoriumRework.Loaded)
                {
                    player.GetAttackSpeed(DamageClass.Melee) -= 0.2f;
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.2f;
                }

                if (item.type == ItemType<EbonHood>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.1f;
                }

                if (item.type == ItemType<EbonCloak>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.1f;
                }

                if (item.type == ItemType<EbonLeggings>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.07f;
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance).Flat += 1f;
                }

                if (item.type == ItemType<CoralHelmet>())
                {
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) += 2f;
                }
                if (item.type == ItemType<CoralChestGuard>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.03f;
                }
                if (item.type == ItemType<CoralGreaves>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.02f;
                }

                if (item.type == ItemType<IridescentHelmet>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.11f;
                }
                if (item.type == ItemType<IridescentMail>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.02f;
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 6f;
                }
                if (item.type == ItemType<IridescentGreaves>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.05f;
                }

                if (item.type == ItemType<BloomingTabard>())
                {
                    InfernalCrossmod.Thorium.Mod.Call(new object[]
                    {
                        "BonusHealerHealBonus",
                        player,
                        +1
                    });
                }

                if (item.type == ItemType<WarlockHood>())
                {
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 5f;
                }
                if (item.type == ItemType<WarlockGarb>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.2f;
                    player.GetDamage(DamageClass.Generic) += 0.1f;
                }
                if (item.type == ItemType<WarlockLeggings>())
                {
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 5f;
                }

                if (item.type == ItemType<BioTechGarment>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.1f;
                }

                if (item.type == ItemType<WhisperingHood>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.1f;
                    player.GetDamage(DamageClass.Generic) += 0.05f;
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 2f;
                }
                if (item.type == ItemType<WhisperingTabard>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.2f;
                    player.GetDamage(DamageClass.Generic) += 0.1f;
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 2f;
                }
                if (item.type == ItemType<WhisperingLeggings>())
                {
                    player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.2f;
                    player.GetDamage(DamageClass.Generic) += 0.1f;
                    player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 6f;
                }

                if (item.type == ItemType<DreadGreaves>())
                {
                    player.moveSpeed -= 0.11f;
                }

                if (item.type == ItemType<DemonBloodBreastPlate>())
                {
                    player.GetThoriumPlayer().demonBloodBreastplateDodge = false;
                }

                if (item.type == ItemType<BloomingShield>())
                {
                    player.GetThoriumPlayer().MetalShieldMax += 20;

                    InfernalCrossmod.Thorium.Mod.Call(new object[]
                    {
                        "BonusHealerHealBonus",
                        player,
                        +1
                    });

                    if (player.GetThoriumPlayer().shieldHealth >= player.GetThoriumPlayer().MetalShieldMax)
                    {
                        InfernalCrossmod.Thorium.Mod.Call(new object[3]
                        {
                            "BonusHealerHealBonus",
                            player,
                            +1
                        });
                    }
                }

                if (item.type == ItemType<AstroBeetleHusk>())
                {
                    player.GetThoriumPlayer().MetalShieldMax += 50;
                }

                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    if (item.type == ItemType<DreamWeaversHelmet>())
                    {
                        player.GetCritChance((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance) += 20f;
                    }
                    if (item.type == ItemType<DreamWeaversHood>())
                    {
                        player.GetDamage((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance) += 0.3f;
                        player.GetCritChance((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance) += 16f;
                    }
                    if (item.type == ItemType<DreamWeaversTabard>())
                    {
                        player.GetCritChance((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance) += 14f;
                    }

                    if (item.type == ItemType<TerrariumWings>())
                    {
                        if (player.armor[0].type == ItemType<TerrariumHelmet>() && player.armor[1].type == ItemType<TerrariumBreastPlate>() && player.armor[2].type == ItemType<TerrariumGreaves>() && player.miscCounter % 2 == 0 && player.wingTime > 0f)
                        {
                            player.wingTime -= 1f;
                        }
                    }

                    //remove if helhiem ever removes thier fallen paladins nerf as this brings it back to Thorium vanilla
                    /*
                    if (item.type == ItemType<FallenPaladinFaceguard>())
                    {
                        player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.1f;
                        player.GetDamage(DamageClass.Generic) -= 0.05f;
                    }
                    if (item.type == ItemType<FallenPaladinCuirass>())
                    {
                        player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.1f;
                        player.GetDamage(DamageClass.Generic) -= 0.05f;
                    }
                    if (item.type == ItemType<FallenPaladinGreaves>())
                    {
                        player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.05f;
                    }
                    */
                }

                /*
                if (InfernalCrossmod.RagnarokMod.Loaded)
                {
                    if (item.type == ItemType<NinjaEmblem>())
                    {
                        player.GetDamage(DamageClass.Generic) += 0.02f;
                        player.GetCritChance(DamageClass.Generic) -= 3f;
                        player.GetAttackSpeed(DamageClass.Generic) -= 0.05f;
                    }
                }
                */

                if (InfernalCrossmod.CalBardHealer.Loaded)
                {
                    Mod cBH = InfernalCrossmod.CalBardHealer.Mod;
                    int FindItem(string name) => cBH.Find<ModItem>(name).Type;

                    if (item.type == FindItem("TarragonParagonCrown"))
                    {
                        InfernalCrossmod.Thorium.Mod.Call(new object[]
                        {
                        "BonusHealerHealBonus",
                        player,
                        -2
                        });
                    }

                    if (item.type == FindItem("BloodflareRitualistMask"))
                    {
                        InfernalCrossmod.Thorium.Mod.Call(new object[]
                        {
                        "BonusHealerHealBonus",
                        player,
                        -6
                        });
                    }

                    if (item.type == FindItem("SilvaGuardianHelmet"))
                    {
                        InfernalCrossmod.Thorium.Mod.Call(new object[]
                        {
                        "BonusHealerHealBonus",
                        player,
                        -8
                        });
                    }

                    if (item.type == FindItem("AuricTeslaValkyrieVisage"))
                    {
                        InfernalCrossmod.Thorium.Mod.Call(new object[]
                        {
                        "BonusHealerHealBonus",
                        player,
                        -11
                        });
                    }

                    if (item.type == ItemType<DemonshadeGreaves>())
                    {
                        InfernalCrossmod.Thorium.Mod.Call(new object[]
                        {
                            "BonusHealerHealBonus",
                            player,
                            -15
                        });
                    }

                    if (InfernalCrossmod.Catalyst.Loaded)
                    {
                        if (item.type == FindItem("AugmentedAuricTeslaValkyrieVisage"))
                        {
                            InfernalCrossmod.Thorium.Mod.Call(new object[]
                            {
                                "BonusHealerHealBonus",
                                player,
                                -11
                            });
                        }
                        if (item.type == FindItem("IntergelacticProtectorHelm"))
                        {
                            InfernalCrossmod.Thorium.Mod.Call(new object[]
                            {
                                "BonusHealerHealBonus",
                                player,
                                -4
                            });
                        }
                    }

                    if (item.type == FindItem("ElementalBloom"))
                    {
                        player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= 0.05f;
                        player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) -= 7f;
                    }
                }
            }
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemType<DreadSkull>() && body.type == ItemType<DreadChestPlate>() && legs.type == ItemType<DreadGreaves>())
                return "Dread";

            if (head.type == ItemType<DemonBloodHelmet>() && body.type == ItemType<DemonBloodBreastPlate>() && legs.type == ItemType<DemonBloodGreaves>())
                return "DemonBlood";

            return base.IsArmorSet(head, body, legs);
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "Dread")
            {
                player.maxRunSpeed -= 1.5f;
                player.runAcceleration -= 0.04f;
            }
            if (set == "DemonBlood")
            {
                player.GetThoriumPlayer().demonBloodBreastplateDodge = true;
                player.setBonus += $"\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DemonBlood.SetBonus")}";
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemType<GatewayGlass>() && player.Calamity().ZoneAbyss && !DownedBossSystem.downedYharon)
                return false;

            if (item.type == ItemType<LihzahrdKukri>() && !NPC.downedPlantBoss) // -Arkangel
                return false;

            return base.CanUseItem(item, player);
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemType<ThePrimordialsTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ItemType<DreamEssence>(), 1, 5, 5));
            }
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (!InfernalConfig.Instance.ThoriumBalanceChangess)
                return;

            if (item.type != ItemType<TheOmegaCore>())
                return;

            if (source is EntitySource_ItemOpen itemOpen && itemOpen.ItemType == ItemType<ThePrimordialsTreasureBag>())
            {
                item.TurnToAir();
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.EyeoftheGolem) {
                foreach (var line in tooltips)
                {
                    if (line.Mod == "Terraria" && line.Name == "Tooltip0")
                    {
                        line.Text = Language.GetTextValue("ItemTooltip.EyeoftheGolem");
                        break;
                    }
                }
            }

            if (item.type == ItemType<LihzahrdKukri>() && !NPC.downedPlantBoss)
            {
                TooltipLine curseWarning = new TooltipLine(Mod, "PlanteraCurse", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.JungleCurse")); //Easiest way I found to give the jungle curse tooltip -Arkangel
                int insertIndex = tooltips.FindLastIndex(t => t.Name.StartsWith("Tooltip"));
                if (insertIndex != -1)
                {
                    tooltips.Insert(insertIndex + 1, curseWarning);
                }
                else
                {
                    tooltips.Add(curseWarning); // not sure this is needed? -Arkangel
                }
            }

            if (item.type == ItemType<MaskoftheCrystalEye>()) 
            {
                if (InfernalCrossmod.SOTS.Loaded)
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.MaskoftheCrystalEye.SOTS"));
                else
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.MaskoftheCrystalEye.Thorium"));
            }

            if (item.type == ItemType<BloodPotion>())
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.BloodPotion"));

            if (item.type == ItemType<FrenzyPotion>())
            {
                if (InfernalCrossmod.SOTS.Loaded)
                {
                    InfernalUtilities.AddDisabledItemTag(tooltips);
                }
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.FrenzyPotion"));
            }

            if (item.type == ItemType<KineticPotion>())
            {
                InfernalUtilities.AddDisabledItemTag(tooltips);
            }

            if (item.type == ItemType<TravelersBoots>())
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.TravelBoot"));
            }

            if (item.type == ItemType<DreadGreaves>())
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DreadGreaves.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DreadGreaves.Nerf"));
            }

            if (item.type == ItemType<DemonBloodBreastPlate>())
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DemonBlood.Replace"));
            }

            if (item.type == ItemType<SteamkeeperWatch>())
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SteamkeeperWatch"));
            }

            if (item.type == ItemType<YumasPendant>())
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.YumasPendant.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.YumasPendant.Nerf"));
            }

            if (item.type == ItemType<PlagueLordFlask>())
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PlagueLordFlask"));
            }

            if (InfernalConfig.Instance.DisableDuplicateContent)
            {
                string[] coatings =
                {
                    "DeepFreezeCoatingItem",
                    "ExplosiveCoatingItem",
                    "GorgonCoatingItem",
                    "SporeCoatingItem",
                    "ToxicCoatingItem",
                };

                foreach (string coating in coatings)
                    if (item.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>(coating).Type)
                        InfernalUtilities.AddDisabledItemTag(tooltips);

                string[] disabledItems =
                {
                    //"ChlorophyteTomahawk",
                    "DemonBloodBow",
                    //"MyceliumGatlingGun",
                    "TimeWarp",
                    "MoltenKnife",
                };

                foreach (string disabledItem in disabledItems)
                    if (item.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>(disabledItem).Type)
                        InfernalUtilities.AddDisabledItemTag(tooltips);
            }

            if (InfernalConfig.Instance.DisableUnnecessaryContent)
            {
                if (item.type == ItemType<TitanBreastplate>())
                    InfernalUtilities.AddDisabledItemTag(tooltips);

                if (item.type == ItemType<TitanGreaves>())
                    InfernalUtilities.AddDisabledItemTag(tooltips);

                if (item.type == ItemType<TitanHelmet>())
                    InfernalUtilities.AddDisabledItemTag(tooltips);

                if (item.type == ItemType<TitanMask>())
                    InfernalUtilities.AddDisabledItemTag(tooltips);

                if (item.type == ItemType<TitanHeadgear>())
                    InfernalUtilities.AddDisabledItemTag(tooltips);

                if (item.type == ItemType<TitanWings>())
                    InfernalUtilities.AddDisabledItemTag(tooltips);

                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    if (InfernalCrossmod.ThoriumRework.Mod.TryFind("TitanHat", out ModItem titanHat))
                    {
                        if (item.type == titanHat.Type)
                            InfernalUtilities.AddDisabledItemTag(tooltips);

                        if (item.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModItem>("TitanHood").Type)
                            InfernalUtilities.AddDisabledItemTag(tooltips);

                        if (item.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModItem>("TitanVisage").Type)
                            InfernalUtilities.AddDisabledItemTag(tooltips);

                        if (item.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModItem>("TitanVisor").Type)
                            InfernalUtilities.AddDisabledItemTag(tooltips);
                    }
                }
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                if (InfernalCrossmod.ThoriumRework.Mod.TryFind("DeathsingerPotion", out ModItem deathsingerPot))
                {
                    if (item.type == deathsingerPot.Type)
                        InfernalUtilities.AddDisabledItemTag(tooltips);
                }
            }
        }
    }

    public class InfernoLordsFocusAccDamageEdit : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            return item.ModItem?.Mod.Name == "ThoriumMod"
                && item.ModItem.Name == "InfernoLordsFocus";
        }

        public override void SetDefaults(Item entity)
        {
            if (entity.ModItem == null)
                return;

            object modItem = entity.ModItem;

            // Look for field named "accDamage"
            FieldInfo field = modItem.GetType().GetField(
                "accDamage",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (field != null)
            {
                field.SetValue(modItem, "5% basic damage");
            }
        }
    }
}
