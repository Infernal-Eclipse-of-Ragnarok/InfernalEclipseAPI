using CalamityMod;
using CalamityMod.Enums;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Potions.Alcohol;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Content.RogueThrower;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.DamageClasses;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Players.SOTSPlayerOverrides;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.Utils;
using InfernumMode.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Buffs.MinionBuffs;
using SOTS.FakePlayer;
using SOTS.Items;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Celestial;
using SOTS.Items.Chaos;
using SOTS.Items.ChestItems;
using SOTS.Items.CritBonus;
using SOTS.Items.Earth;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Evil;
using SOTS.Items.Fishing;
using SOTS.Items.Gems;
using SOTS.Items.Invidia;
using SOTS.Items.Permafrost;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.Potions;
using SOTS.Items.Pyramid;
using SOTS.Items.Secrets;
using SOTS.Items.Slime;
using SOTS.Items.Temple;
using SOTS.Items.Tide;
using SOTS.Items.Void;
using SOTS.Void;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class SOTSCrateFix : GlobalItem
    {
        public override bool InstancePerEntity => false;

        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            Fraction fifteenPercent = new Fraction(15, 100);

            if (item.type == ItemType<OtherworldCrate>())
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

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class SOTSGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemType<MartianWarhorn>())
            {
                if (CalamityUtils.AnyBossNPCS())
                {
                    return false;
                }
            }

            if (item.ModItem is VoidConsumable)
            {
                if (player.HasBuff(BuffType<VoidSickness2>()))
                {
                    int buffIndex = player.FindBuffIndex(BuffType<VoidSickness2>());

                    if (buffIndex != -1 && player.buffTime[buffIndex] >= 10 * 60)
                    {
                        if (player.GetModPlayer<InfernalPlayer>().voidSicknessTextCooldown <= 0)
                        {
                            CombatText.NewText(player.Hitbox, Color.Lerp(Color.Red, Color.Magenta, 0.5f), "You don't have the appetite for that!", true);
                            player.GetModPlayer<InfernalPlayer>().voidSicknessTextCooldown = 60 * 5;
                        }
                        return false;
                    }
                }
            }

            return base.CanUseItem(item, player);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if ((equippedItem.type == ItemType<Calculator>() || equippedItem.type == ItemType<Purity>()) && (incomingItem.type == ItemType<Calculator>() || incomingItem.type == ItemType<Purity>()))
                return false;

            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void UpdateAccessory(Item item, Player player, bool hidevisual)
        {
            InfernalPlayer modPlayer = player.GetModPlayer<InfernalPlayer>();
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            SOTSPlayerAdjustments sotsAdjustmentPlayer = player.GetModPlayer<SOTSPlayerAdjustments>();

            if (InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (item.type == ModContent.ItemType<HarvestersScythe>())
                {
                    sotsPlayer.CritBonusMultiplier -= 0.15f;
                }

                if (item.type == ModContent.ItemType<RockCandy>())
                {
                    sotsPlayer.bonusPickaxePower -= 1;
                }

                if (item.type == ItemType<EarthDrive>())
                {
                    sotsPlayer.Earthdrive = false;
                    modPlayer.Earthdrive = true;
                }

                if (item.type == ModContent.ItemType<HydrokineticAntennae>())
                {
                    sotsPlayer.StatShareMeleeAndSummon = false;
                    player.GetDamage<TrueMeleeDamageClass>() -= 0.15f;
                }

                if (item.type == ItemType<SubspaceLocket>())
                {
                    ref StatModifier local = ref player.GetDamage(DamageClass.Generic);
                    local *= 0.667f;
                    player.GetDamage<TrueMeleeDamageClass>() -= 0.2f;
                    player.stealthTimer = 0;
                    player.stealth = 1f;
                    sotsPlayer.additionalHeal -= 35;

                    if (InfernalCrossmod.Thorium.Loaded)
                    {
                        player.GetModPlayer<RogueThrowerPlayer>().subspaceLocketThorClassNerf = true;
                    }
                }

                if (InfernalCrossmod.Thorium.Loaded && InfernalConfig.Instance.MergeCraftingTrees)
                {
                    if (item.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>("MaskoftheCrystalEye").Type)
                    {
                        sotsPlayer.CritBonusDamage += 15;
                    }
                }

                if (item.type == ModContent.ItemType<EyeOfChaos>())
                {
                    player.GetCritChance(DamageClass.Generic) -= 18f;
                    modPlayer.eyeOfChaos = true;

                    if (InfernalCrossmod.Thorium.Loaded && InfernalConfig.Instance.MergeCraftingTrees)
                        sotsPlayer.CritBonusDamage += 15;
                }

                if (item.type == ModContent.ItemType<SnakeEyes>())
                {
                    player.GetCritChance(DamageClass.Generic) -= 7f;
                    modPlayer.snakeEyes = true;
                }

                if (item.type == ModContent.ItemType<ChaosBadge>())
                {
                    player.GetCritChance(DamageClass.Generic) -= 9f;
                    modPlayer.chaosBadge = true;
                }

                if (item.type == ModContent.ItemType<FocusReticle>())
                {
                    player.GetCritChance(DamageClass.Generic) -= 20f;
                    modPlayer.focusReticle = true;
                    sotsPlayer.CritBonusDamage -= 40;
                    sotsPlayer.CritBonusDamage += GetReticleCritBonus(player);
                }

                if (item.type == ModContent.ItemType<Starbelt>())
                {
                    player.GetCritChance(DamageClass.Magic) -= 5f;
                }

                if (item.type == ModContent.ItemType<GlowSpores>())
                {
                    player.GetCritChance(DamageClass.Magic) -= 3f;
                }

                if (item.type == ModContent.ItemType<SpiritGlove>())
                {
                    player.GetCritChance(DamageClass.Melee) -= 4f;
                }

                if (item.type == ModContent.ItemType<SwallowedPenny>())
                {
                    player.GetCritChance(DamageClass.Generic) -= 2f;
                }

                if (item.type == ItemType<SyntheticLiver>())
                {
                    sotsPlayer.DrainDebuffs = false;
                }

                if (item.type == ItemType<CrestofDasuver>())
                {
                    player.GetCritChance(DamageClass.Generic) -= 3f;
                }

                if (item.type == ItemType<MidnightPrism>() || item.type == ItemType<WitchHeart>() || ((item.type == ItemType<SandSharkToothNecklace>() || item.type == ItemType<ReaperToothNecklace>()) && InfernalConfig.Instance.MergeCraftingTrees))
                {
                    sotsPlayer.CritNightmare = false;
                    modPlayer.CritNightmare = true;
                }

                if (item.type == ItemType<RoyalJelly>())
                {
                    sotsPlayer.additionalHeal -= 40;
                    sotsAdjustmentPlayer.royalJelly = true;
                }

                if (item.type == ItemType<GlowSpores>())
                {
                    sotsPlayer.additionalPotionMana -= 40;
                    sotsAdjustmentPlayer.glowSpores = true;
                }

                if (item.type == ItemType<GlowJelly>())
                {
                    sotsPlayer.additionalHeal -= 40;
                    sotsPlayer.additionalPotionMana -= 40;
                    sotsAdjustmentPlayer.glowJelly = true;
                }

                if (item.type == ItemType<Sandwich>())
                {
                    sotsPlayer.additionalHeal -= 40;
                    sotsAdjustmentPlayer.sandwich = true;
                }

                if (item.type == ItemType<AlchemistsCharm>())
                {
                    sotsPlayer.additionalHeal -= 100;
                    sotsPlayer.additionalPotionMana -= 100;
                    sotsAdjustmentPlayer.alchemistsCharm = true;
                }

                if (item.type == ItemType<PlatformGenerator>() || item.type == ItemType<FortressGenerator>())
                {
                    player.GetDamage(DamageClass.Summon) -= 0.1f;
                }

                if (item.type == ItemType<OtherworldlyAmplifier>())
                {
                    sotsPlayer.CritBonusDamage -= 8;
                    sotsPlayer.CritBonusDamage += GetAmpliferCritBonus(player);
                }

                if (item.type == ItemType<FocusCrystal>())
                {
                    sotsPlayer.CritBonusDamage -= 40;
                    sotsPlayer.CritBonusDamage += GetFocusCritBonus(player);
                }

                if (item.type == ItemType<Calculator>())
                {
                    var cplayer = player.Calamity();
                    cplayer.critDamage -= 75 * 0.01f;
                    player.GetCritChance(DamageClass.Generic) -= 30f;
                    player.GetDamage(DamageClass.Summon) -= 0.2f;

                    sotsPlayer.CritBonusDamage = (int)(sotsPlayer.CritBonusDamage * 0.25f);
                    sotsPlayer.CritBonusMultiplier *= 0.75f;
                    sotsPlayer.CritCurseFire = false;
                    sotsPlayer.CritFire = false;
                    sotsPlayer.CritFrost = false;

                    if (InfernalPlayer.PlayerHasPurity(player))
                    {
                        sotsPlayer.HomingRange = 0;
                    }
                }

                if (item.type == ItemType<ShoeIce>())
                {
                    player.moveSpeed -= 0.25f;
                    player.maxRunSpeed -= 0.4f;
                }

                if (item.type == ItemType<BagOfCharms>())
                {
                    player.GetModPlayer<InfernalPlayer>().bagOfCharms = true;
                }

                if (item.type == ItemType<Hyperdrive>())
                {
                    sotsPlayer.attackSpeedMod -= 0.25f;
                    player.GetAttackSpeed<VoidGeneric>() += 0.25f;
                }

                if (InfernalCrossmod.SOTSBardHealer.Loaded)
                {
                    Mod sBH = InfernalCrossmod.SOTSBardHealer.Mod;
                    int FindItem(string name) => sBH.Find<ModItem>(name).Type;

                    if (item.type == FindItem("SerpentsTongue"))
                    {
                        SOTSPlayer.ModPlayer(player).CritBonusMultiplier -= 0.1f;
                    }
                }
            }
        }

        private static int GetAmpliferCritBonus(Player player)
        {
            if (player.HeldItem.useTime == 0)
            {
                return 0;
            }
            else if (player.HeldItem.useTime <= 9)
            {
                return 2;
            }
            else if (player.HeldItem.useTime <= 14)
            {
                return 3;
            }
            else if (player.HeldItem.useTime <= 22)
            {
                return 4;
            }
            else if (player.HeldItem.useTime <= 29)
            {
                return 5;
            }
            else if (player.HeldItem.useTime <= 37)
            {
                return 6;
            }
            else if (player.HeldItem.useTime <= 45)
            {
                return 7;
            }
            return 8;
        }

        private static int GetFocusCritBonus(Player player)
        {
            if (player.HeldItem.useTime == 0)
            {
                return 0;
            }
            else if (player.HeldItem.useTime <= 9)
            {
                return 8;
            }
            else if (player.HeldItem.useTime <= 14)
            {
                return 10;
            }
            else if (player.HeldItem.useTime <= 22)
            {
                return 13;
            }
            else if (player.HeldItem.useTime <= 29)
            {
                return 16;
            }
            else if (player.HeldItem.useTime <= 37)
            {
                return 20;
            }
            else if (player.HeldItem.useTime <= 45)
            {
                return 25;
            }
            return 30;
        }

        public static int GetReticleCritBonus(Player player)
        {
            if (player.HeldItem.useTime == 0)
            {
                return 0;
            }
            else if (player.HeldItem.useTime <= 9)
            {
                return 15;
            }
            else if (player.HeldItem.useTime <= 14)
            {
                return 17;
            }
            else if (player.HeldItem.useTime <= 22)
            {
                return 20;
            }
            else if (player.HeldItem.useTime <= 29)
            {
                return 25;
            }
            else if (player.HeldItem.useTime <= 37)
            {
                return 30;
            }
            else if (player.HeldItem.useTime <= 45)
            {
                return 35;
            }
            return 40;
        }

        public override void SetDefaults(Item item)
        {
            if (item.type == ItemType<GelWings>())
            {
                item.rare = ItemRarityID.LightRed;
                item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            }

            if (InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (item.type == ItemType<ShatterShardChestplate>())
                {
                    item.defense = 5;
                }


                if (item.type == ItemType<FrostArtifactHelmet>())
                {
                    item.defense = 13;
                }

                if (item.type == ItemType<FrostArtifactChestplate>())
                {
                    item.defense = 21;
                }

                if (item.type == ItemType<FrostArtifactTrousers>())
                {
                    item.defense = 13;
                }


                if (item.type == ItemType<TwilightAssassinsCirclet>())
                {
                    item.defense = 7;
                }

                if (item.type == ItemType<TwilightAssassinsChestplate>())
                {
                    item.defense = 9;
                }

                if (item.type == ItemType<TwilightAssassinsLeggings>())
                {
                    item.defense = 8;
                }


                if (item.type == ItemType<ElementalHelmet>())
                {
                    item.defense = 13;
                }

                if (item.type == ItemType<ElementalBreastplate>())
                {
                    item.defense = 19;
                }

                if (item.type == ItemType<ElementalLeggings>())
                {
                    item.defense = 13;
                }


                if (item.type == ItemType<VoidspaceMask>())
                {
                    item.defense = 9;
                }

                if (item.type == ItemType<VoidspaceBreastplate>())
                {
                    item.defense = 16;
                }

                if (item.type == ItemType<VoidspaceLeggings>())
                {
                    item.defense = 13;
                }

                if (item.type == ItemType<InfernoHook>())
                {
                    item.shootSpeed = 14;
                }

                if (item.type == ItemType<DreamLamp>() && SOTSWorld.DreamLampSolved)
                {
                    item.damage = 18;
                }
            }

            if (InfernalCrossmod.SOTSBardHealer.Loaded && InfernalConfig.Instance.SOTSThrowerToRogue)
            {
                Mod SOTSBardHealer = InfernalCrossmod.SOTSBardHealer.Mod;
                if (item.type == SOTSBardHealer.Find<ModItem>("ForbiddenMaelstrom").Type || item.type == SOTSBardHealer.Find<ModItem>("GoopwoodSplit").Type || item.type == SOTSBardHealer.Find<ModItem>("Serpentbite").Type)
                {
                    item.DamageType = GetInstance<VoidRogue>();
                }
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (item.type == ItemType<FrostArtifactHelmet>())
                {
                    player.GetDamage(DamageClass.Melee) -= 0.05f;
                    player.GetDamage(DamageClass.Ranged) -= 0.05f;
                }

                if (item.type == ItemType<FrostArtifactChestplate>())
                {
                    player.GetCritChance(DamageClass.Melee) -= 5f;
                    player.GetCritChance(DamageClass.Ranged) -= 5f;
                }


                if (item.type == ItemType<TwilightAssassinsCirclet>())
                {
                    player.maxMinions -= 1;
                }


                if (item.type == ItemType<ElementalBreastplate>())
                {
                    player.GetDamage(DamageClass.Melee) -= 0.2f;
                    player.GetDamage(DamageClass.Summon) -= 0.2f;
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemType<NightmarePotion>() || item.type == ItemType<WitchHeart>() || item.type == ItemType<MidnightPrism>())
            {
                foreach (TooltipLine tooltip in tooltips)
                {
                    if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Nightmare.Orig")))
                    {
                        tooltip.Text = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Nightmare.Rework");
                    }
                }
            }

            if (InfernalConfig.Instance.SOTSBalanceChanges)
            {
                Color InfernalRed = Color.Lerp(
                  Color.White,
                  new Color(255, 80, 0), // Infernal red/orange
                  (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
                );

                if (item.type == ModContent.ItemType<SubspaceLocket>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, InfernalCrossmod.Thorium.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SubspaceLocketThorium") : Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SubspaceLocket"));
                }

                if (item.type == ModContent.ItemType<EyeOfChaos>())
                {
                    if (InfernalCrossmod.Thorium.Loaded && InfernalConfig.Instance.MergeCraftingTrees)
                        InfernalUtilities.FullTooltipOveride(tooltips, $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.EyeOfChaos")}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.EyeOfChaosThorium")}");
                    else
                        InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.EyeOfChaos"));
                }

                if (item.type == ModContent.ItemType<SnakeEyes>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SnakeEyes"));
                }

                if (item.type == ModContent.ItemType<ChaosBadge>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ChaosBadge"));
                }

                if (item.type == ModContent.ItemType<FocusReticle>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.FocusReticle", GetReticleCritBonus(Main.LocalPlayer)));
                }

                if (item.type == ModContent.ItemType<Starbelt>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Starbelt"));
                }

                if (item.type == ModContent.ItemType<GlowSpores>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GlowSpores"));
                }

                if (item.type == ModContent.ItemType<SpiritGlove>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SpiritGlove"));
                }

                if (item.type == ModContent.ItemType<SwallowedPenny>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SwallowedPenny"));
                }

                if (item.type == ItemType<EarthDrive>())
                {
                    InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.EarthDrive"), InfernalRed);
                }

                if (item.type == ItemType<SyntheticLiver>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SyntheticLiver"));
                }

                if (item.type == ItemType<GelWings>())
                {
                    if (item.wingSlot == -1) return;
                    WingStats stats = ArmorIDs.Wing.Sets.Stats[item.wingSlot];
                    int time = stats.FlyTime;
                    float run = stats.AccRunSpeedOverride;
                    float rAcc = stats.AccRunAccelerationMult * 0.08f;
                    bool hover = stats.HasDownHoverStats;
                    float hSpeed = stats.DownHoverSpeedOverride;
                    float hAcc = stats.DownHoverAccelerationMult * 0.08f;
                    float baseJumpSpeed = (CalamityServerConfig.Instance.FasterJumpSpeed ? 5.71f : 5.01f) + 1f;
                    StringBuilder sb = new StringBuilder(512);
                    sb.Append('\n');
                    if (Main.keyState.PressingShift())
                    {
                        sb.Append(CalamityUtils.GetText($"Common.WingStatsFull").Format(time.FramesToSeconds(),
                        BaseWings.HorizontalSpeedText(run), run.ToMph(),
                        BaseWings.VerticalSpeedText(1.35f), (1.35f * baseJumpSpeed).ToMph(),
                        BaseWings.HorizontalAccelerationText(stats.AccRunAccelerationMult), rAcc.ToMphps(),
                        BaseWings.VerticalAccelerationText(0.195f), 0.195f.ToMphps(),
                        (0.1f + 0.15f).ToMphps(), (1 * baseJumpSpeed).ToMph(),
                        (0.195f + 0.85f).ToMphps()));
                        if (hover)
                        {
                            sb.Append('\n');
                            sb.Append(Language.GetText($"Common.WingStatsHover").Format(hSpeed.ToMph(), hAcc.ToMphps()));
                        }
                    }
                    else
                    {
                        sb.Append(CalamityUtils.GetText($"Common.WingStats").Format(time.FramesToSeconds(), BaseWings.HorizontalSpeedText(run), BaseWings.VerticalSpeedText(1.35f),
                        BaseWings.HorizontalAccelerationText(stats.AccRunAccelerationMult), BaseWings.VerticalAccelerationText(0.195f)));
                        sb.Append('\n');
                        sb.Append($"[c/B8B8B8:{Language.GetTextValue("Mods.CalamityMod.UI.HoldShiftTooltipExtensionIndicator")}]");
                    }

                    // Add stats below the common "Allows flight" line
                    TooltipLine wingTooltip = tooltips.FirstOrDefault(x => x.Text == Language.GetTextValue("CommonItemTooltip.FlightAndSlowfall") && x.Mod == "Terraria");
                    if (wingTooltip != null)
                        wingTooltip.Text += sb.ToString();
                }

                if (item.type == ItemType<CrestofDasuver>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dasuver"));
                }

                if (item.type == ItemType<VibePotion>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.VibePotion"));
                }

                if (item.type == ItemType<FlowerCrown>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.FlowerCrown"));
                }

                if (item.type == ItemType<Helios>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Helios.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Helios.Nerf"));
                }

                if (item.type == ItemType<BorealisIcosahedron>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Borealis.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Borealis.Nerf"));
                }

                if (item.type == ItemType<CursedIcosahedron>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Cursed.OrigCursed"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Cursed.NerfCursed"));
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Cursed.OrigOther"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Cursed.NerfOther"));
                }

                if (item.type == ItemType<HellfireIcosahedron>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Hellfire.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Hellfire.Nerf"));
                }

                if (item.type == ItemType<BagOfCharms>())
                {
                    //InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Charms.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Icosahedron.Charms.Nerf"));
                }

                if (item.type == ItemType<PlasmaShrimp>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PlasmaShrimp.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PlasmaShrimp.Nerf"));
                }

                if (item.type == ItemType<WishingStar>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ShatteredDreams.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ShatteredDreams.Nerf"));
                }

                if (item.type == ItemType<OtherworldlyAmplifier>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.OtherworldlyAmplifier", GetAmpliferCritBonus(Main.LocalPlayer)));
                }

                if (item.type == ItemType<FocusCrystal>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.FocusCrystal", GetFocusCritBonus(Main.LocalPlayer)));
                }

                if (item.type == ItemType<Sandwich>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AlchCharm.SandwichOrig"), 
                    Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AlchCharm.SandwichOrig") + "\n" + Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DoesNotStack"));
                }

                if (item.type == ItemType<GlowJelly>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AlchCharm.JellyOrig"),
                    Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AlchCharm.JellyOrig") + "\n" + Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DoesNotStack"));
                }

                if (item.type == ItemType<AlchemistsCharm>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AlchCharm.AlchOrig"),
                    Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AlchCharm.AlchOrig") + "\n" + Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DoesNotStack"));
                }

                if (item.type == ItemType<InfernoHook>())
                {
                    void ApplyTooltipEdits(IList<TooltipLine> lines, Func<Item, TooltipLine, bool> predicate, Action<TooltipLine> action)
                    {
                        foreach (TooltipLine line in lines)
                            if (predicate.Invoke(item, line))
                                action.Invoke(line);
                    }
                    Func<Item, TooltipLine, bool> LineName(string s) => (Item i, TooltipLine l) => l.Mod == "Terraria" && l.Name == s;
                    void EditTooltipByName(string lineName, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineName(lineName), action);
                    void AddGrappleStats(float r, float l, float e, float p) => EditTooltipByName("Equipable", (line) => line.Text += "\n" + CalamityUtils.GetText("Common.GrappleStats").Format(r.ToString(), l.ToString(), e.ToString(), p.ToString()));

                    AddGrappleStats(510 / 16f, item.shootSpeed, 17f, 11f);
                }

                if (item.type == ItemType<PlatformGenerator>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PlatformGenerator"));
                }

                if (item.type == ItemType<FortressGenerator>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.FortressGenerator"));

                    if (InfernalConfig.Instance.MergeCraftingTrees)
                    {
                        if (InfernalCrossmod.Clamity.Loaded)
                            InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.CyanPearl"), InfernalRed);

                        if (InfernalCrossmod.Thorium.Loaded)
                        {
                            //InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Skull"), InfernalRed);
                            InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.Watch"), InfernalRed);
                        }
                    }
                }

                if (item.type == ItemType<RubyRing>())
                {
                    InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.RubyRing"));
                }

                if (item.type == ItemType<ChallengerRing>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.Items.RubyRing.Tooltip"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.RubyRing"));
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ChallengerRubyHover.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ChallengerRubyHover.Nerf"));
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.Items.ChallengerRing.I5"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DiamondInvertedRing", Main.LocalPlayer.GetModPlayer<InfernalPlayer>().defenseGain, Main.LocalPlayer.GetModPlayer<InfernalPlayer>().defenseGain));
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ChallengerInvertedRingHover.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ChallengerInvertedRingHover.Rework", Main.LocalPlayer.GetModPlayer<InfernalPlayer>().defenseGain, Main.LocalPlayer.GetModPlayer<InfernalPlayer>().defenseGain));
                }

                if (item.type == ItemType<MartianWarhorn>())
                {
                    InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.NoBoss"), InfernalRed);
                }

                if (item.type == ItemType<Calculator>())
                {
                    InfernalUtilities.AddTooltip(tooltips, $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GrapeBeer.Nerf")}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Calculator")}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GrapeBeer.SOTSAdditional")}", InfernalRed);
                }

                if (item.type == ItemType<ShoeIce>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ShoeIce.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ShoeIce.Nerf"));
                }

                if (item.type == ItemType<Hyperdrive>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Hyperdrive.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Hyperdrive.Nerf"));
                }

                if (item.type == ItemType<SpiritSurfer>())
                {
                    InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SpiritSurfer.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SpiritSurfer.Replace"));
                }
            }
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class SOTSModPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges)
                return;

            Item head = Player.armor[0];
            Item body = Player.armor[1];
            Item legs = Player.armor[2];

            bool wearingEarthen =
                head.type == ModContent.ItemType<EarthenHelmet>() &&
                body.type == ModContent.ItemType<EarthenChestplate>() &&
                legs.type == ModContent.ItemType<EarthenLeggings>();

            if (wearingEarthen)
            {
                Player.GetAttackSpeed(DamageClass.Generic) -= 0.10f;
            }
        }

        public override void PostUpdate()
        {
            if (FakeModPlayer.ModPlayer(Player).servantActive == true)
            {
                Player.Calamity().rogueStealth = 0;
                Player.Calamity().wearingRogueArmor = false;
            }

            if (Player.HasBuff<TesseractBuff>())
            {
                Player.Calamity().rogueStealth = 0;
                Player.Calamity().wearingRogueArmor = false;
                Player.GetDamage<TrueMeleeDamageClass>() -= 0.15f;
            }
        }
    }

}
