using CalamityMod;
using CalamityMod.CalPlayer;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using InfernalEclipseAPI.Core.Systems;
using Terraria.GameContent.Events;
using ThoriumMod;
using ThoriumMod.Buffs;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Core.Players.ThoriumPlayerOverrides.ThoriumMulticlassNerf
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumMulticlassPlayerNerfs : ModPlayer
    {
        //HEALER MULTICLASS NERF
        public const int PenaltyDuration = 60 * 10; // 10s
        public int switchToHealerPenaltyTimer;

        //BARD MULTICLASS NERF
        public const int WindowTicks = 60 * 5; // 5 seconds
        internal int lastBardUseTick = int.MinValue / 2;

        public override void ResetEffects()
        {
            if (switchToHealerPenaltyTimer > 0)
                switchToHealerPenaltyTimer--;

            Player.buffImmune[ModContent.BuffType<KineticPotionBuff>()] = true;

            if (InfernalCrossmod.SOTS.Loaded)
                Player.buffImmune[ModContent.BuffType<FrenzyPotionBuff>()] = true;

            if (InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    if (InfernalCrossmod.ThoriumRework.Mod.TryFind("Deathsinger", out ModBuff deathsinger))
                        Player.buffImmune[deathsinger.Type] = true;
                }
            }

            if (InfernalConfig.Instance.DisableDuplicateContent)
            {
                Player.buffImmune[ModContent.BuffType<DeepFreezeCoatingBuff>()] = true;
                Player.buffImmune[ModContent.BuffType<ExplosiveCoatingBuff>()] = true;
                Player.buffImmune[ModContent.BuffType<GorgonCoatingBuff>()] = true;
                Player.buffImmune[ModContent.BuffType<SporeCoatingBuff>()] = true;
                Player.buffImmune[ModContent.BuffType<ToxicCoatingBuff>()] = true;
            }
        }

        // === Helpers ===
        private static bool IsHealerDamage(Item item) => item.CountsAsClass<HealerDamage>();
        private static bool IsHealerDamage(Projectile proj) => proj.CountsAsClass<HealerDamage>();

        // Treat these custom tools as "Healer usage"
        private static bool IsHealerToolOrHybrid(Item item)
        {
            var mi = item?.ModItem;
            if (mi is null) return false;

            string typeName = mi.GetType().Name;
            return typeName == "HealerTool" || typeName == "HealerToolDamageHybrid";
        }
         
        private static bool IsHealerWeaponOrTool(Item item) =>
            IsHealerDamage(item) || IsHealerToolOrHybrid(item);

        private static bool HasAllGemTech()
        {
            Player player = Main.LocalPlayer;

            var cal = player.GetModPlayer<CalamityPlayer>();
            return cal.GemTechSet &&
                   cal.GemTechState.IsYellowGemActive &&
                   cal.GemTechState.IsGreenGemActive &&
                   cal.GemTechState.IsPurpleGemActive &&
                   cal.GemTechState.IsBlueGemActive &&
                   cal.GemTechState.IsRedGemActive &&
                   cal.GemTechState.IsPinkGemActive;
        }

        private static bool IsExcluded(Item item) =>
            item.CountsAsClass<LegendaryMelee>() || item.CountsAsClass<LegendaryRanged>() || item.CountsAsClass<LegendaryMagic>() || item.CountsAsClass<LegendarySummon>() || item.CountsAsClass<LegendarySummonMeleeSpeed>() ||
            item.CountsAsClass<MythicMelee>() || item.CountsAsClass<MythicMagic>() || item.CountsAsClass<MythicRanged>() || item.CountsAsClass<MythicSummon>() 
            || item.CountsAsClass<AverageDamageClass>();

        private static bool IsExcluded(Projectile proj) =>
            proj.CountsAsClass<LegendaryMelee>() || proj.CountsAsClass<LegendaryRanged>() || proj.CountsAsClass<LegendaryMagic>() || proj.CountsAsClass<LegendarySummon>() || proj.CountsAsClass<LegendarySummonMeleeSpeed>() ||
            proj.CountsAsClass<MythicMelee>() || proj.CountsAsClass<MythicMagic>() || proj.CountsAsClass<MythicRanged>() || proj.CountsAsClass<MythicSummon>() ||
            proj.CountsAsClass<AverageDamageClass>();

        private static bool ShouldIgnoreContext() => HasAllGemTech() || DD2Event.Ongoing;

        public static bool IsCombatWeapon(Item item) =>
            item is not null &&
            item.type != ItemID.None &&
            item.useStyle != ItemUseStyleID.None &&
            !item.accessory &&
            item.ammo == AmmoID.None &&
            item.pick <= 0 && item.axe <= 0 && item.hammer <= 0 && item.damage > 0 &&
            !IsExcluded(item) &&
            !ShouldIgnoreContext();

        public bool InWindow
        {
            get
            {
                int now = (int)Main.GameUpdateCount;
                return now - lastBardUseTick <= WindowTicks;
            }
        }

        public void MarkBardUse() => lastBardUseTick = (int)Main.GameUpdateCount;

        public override void OnEnterWorld() => lastBardUseTick = int.MinValue / 2;

        // === Item hits (melee/direct) ===
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalConfig.Instance.NerfThoriumMulticlass) return;

            if (IsExcluded(item) || ShouldIgnoreContext())
            {
                switchToHealerPenaltyTimer = 0;
                return;
            }

            if (Player.HasBuff<BrokenOath>())
            {
                bool lunateCharm = false;
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    lunateCharm = LunateCharmChecker.PlayerHasLunateCharm(Player);
                }

                if (!lunateCharm)
                    modifiers.FinalDamage *= 0.5f;
            }

            /*
            if (IsCombatWeapon(item) && !IsHealerWeaponOrTool(item))
            {
                // Any non-healer weapon hit starts/refreshes the "recently non-healer" window
                switchToHealerPenaltyTimer = PenaltyDuration;
                return;
            }

            if (IsHealerWeaponOrTool(item) && switchToHealerPenaltyTimer > 0)
            {
                Player.AddBuff(ModContent.BuffType<BrokenOath>(), switchToHealerPenaltyTimer);
            }
            */
        }

        public int[] HealerPenaltyTimerProjectiles;
        public int[] IgnoreForPenatly;

        // === Projectile hits ===
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalConfig.Instance.NerfThoriumMulticlass) return;
            if (proj.owner != Player.whoAmI) return;

            // Do NOT let Legendary/Mythic/GemTech/OOA interact with the penalty window at all.
            if (IsExcluded(proj) || ShouldIgnoreContext())
            {
                switchToHealerPenaltyTimer = 0;
                return;
            }

            bool lunateCharm = false;
            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                lunateCharm = LunateCharmChecker.PlayerHasLunateCharm(Player);
            }

            if (Player.HasBuff<BrokenOath>() && !lunateCharm)
                modifiers.FinalDamage *= 0.5f;

            bool healerAttack = IsHealerDamage(proj);

            // If the projectile itself isn't tagged healer, but the held item is a healer tool/hybrid,
            // treat it as healer usage (common for tools spawning neutral projectiles).
            if (!healerAttack && IsHealerWeaponOrTool(Player.HeldItem))
                healerAttack = true;

            if (!healerAttack && (proj.DamageType == DamageClass.Summon 
                                //|| HealerPenaltyTimerProjectiles.Contains(proj.type)
                                )
                                //&& !IgnoreForPenatly.Contains(proj.type)
                                )
            {
                // Non-healer projectile -> start/refresh window (since not excluded)
                switchToHealerPenaltyTimer = PenaltyDuration;
                return;
            }

            /*
            // Healer projectile during active window -> apply debuff
            if (switchToHealerPenaltyTimer > 0)
                Player.AddBuff(ModContent.BuffType<BrokenOath>(), switchToHealerPenaltyTimer);
            */
        }

        public override void UpdateBadLifeRegen()
        {
            ThoriumPlayer thoriumPlayer = Player.GetThoriumPlayer();

            if (thoriumPlayer.conflagrate)
            {
                Player.lifeRegen -= 15;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            InfernalPlayer infernalPlayer = Player.GetModPlayer<InfernalPlayer>();
            if (infernalPlayer.soltanBullying)
            {
                float emptySummonSlots = Player.maxMinions - Player.slotsMinions;
                ref StatModifier melee = ref Player.GetDamage(ThoriumDamageBase<BardDamage>.Instance);
                melee += (float)(0.02 * emptySummonSlots);
                ref StatModifier ranged = ref Player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance);
                ranged += (float)(0.02 * emptySummonSlots);
            }

            if (Player.HeldItem.CountsAsClass<LegendaryMelee>() || Player.HeldItem.CountsAsClass<LegendaryRanged>() || Player.HeldItem.CountsAsClass<LegendaryMagic>() || Player.HeldItem.CountsAsClass<LegendarySummon>() || Player.HeldItem.CountsAsClass<LegendarySummonMeleeSpeed>() ||
                Player.HeldItem.CountsAsClass<MythicMelee>() || Player.HeldItem.CountsAsClass<MythicRanged>() || Player.HeldItem.CountsAsClass<MythicMagic>() || Player.HeldItem.CountsAsClass<MythicSummon>())
            {
                bool hasLunateCharm = false;
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    hasLunateCharm = LunateCharmChecker.PlayerHasLunateCharm(Player);
                }

                if (!hasLunateCharm)
                {
                    StatModifier healerModifier = Player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance);

                    float transferAmount = (healerModifier.Additive - 1f) * healerModifier.Multiplicative / 2f;

                    Player.GetDamage(DamageClass.Generic) += transferAmount;
                    Player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) -= transferAmount;
                }
            }
        }

        private int inspirationGainTimer;

        public override void PostUpdateBuffs()
        {
            if (Player.Calamity().rageModeActive && InfernalCrossmod.CalBardHealer.Loaded)
            {
                if (inspirationGainTimer == 0)
                {
                    inspirationGainTimer = 30;
                }
                else 
                {
                    if (inspirationGainTimer != 30)
                    {
                        if (Player.GetThoriumPlayer().bardResource > 0)
                            Player.GetThoriumPlayer().bardResource--;
                    }
                }
                inspirationGainTimer--;
            }
            else
            {
                if (inspirationGainTimer != 0)
                    inspirationGainTimer = 0;
            }
        }

        public override void PostUpdateEquips()
        {
            if (!InfernalConfig.Instance.NerfThoriumMulticlass) return;

            if (Player.HasBuff<BrokenOath>())
            {
                ThoriumPlayer thor = Player.GetThoriumPlayer();

                thor.healBonus -= 10;

                if (thor.healBonus < 0)
                    thor.healBonus = 0;
            }
        }
    }
}
