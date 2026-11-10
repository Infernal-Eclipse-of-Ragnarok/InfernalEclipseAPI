using System.Reflection;
using InfernalEclipseAPI.Core.World;
using System;
using CalamityMod;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Cryogen;
using MonoMod.RuntimeDetour;
using Terraria.Audio;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.Cryogen.CryogenBehaviorOverride;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.CryogenOverrides
{
    public static class CryogenChanges
    {
        public const float HorizontalDashAttackPowerBonus = 0.25f;
        public const float DoubleBackTurnTick = 48f;
        public const int DoubleBackChargeParity = 1;
        public const float DoubleBackParticleSpeed = 5f;
        public const int DoubleBackParticleCount = 30;

        public const float WindupShakeStrength = 14f;
        public const float WindupParticleSpeed = 2.5f;
        public const int WindupParticleCount = 1;

        public const float PillarHeightTrackingEndTime = 45f;
        public const float PillarHeightBonus = 150f;

        public const float PillarOrbitRadius = 380f;
        public const float PillarOrbitAngularStepPerTick = 0.03f;
        public const float PillarOrbitMaxSpeed = 15f;
        public const float PillarOrbitAcceleration = 1.4f;
    }

    internal sealed class CryogenWindupShake : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public Vector2 AppliedOffset;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>();
    }

    internal sealed class CryogenRagnarokHooks : ModSystem
    {
        private static readonly MethodInfo HorizontalDashMethod = typeof(CryogenBehaviorOverride).GetMethod("DoAttack_HorizontalDash", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo ShatteringIcePillarsMethod = typeof(CryogenBehaviorOverride).GetMethod("DoAttack_ShatteringIcePillars", LumUtils.UniversalBindingFlags);

        private const int AttackSubstateIndex = 0;
        private const int ChargeCounterIndex = 1;
        private const float SpinSubstate = 1f;
        private const float AimingSubstate = 2f;
        private const float ChargingSubstate = 3f;

        private const float PillarHeightMargin = 100f;
        private const float PillarVerticalVelocityLead = 15f;
        private const float PillarSyncInterval = 6f;

        private delegate void Orig_Attack(NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower);

        private delegate void Hook_Attack(Orig_Attack orig, NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower);

        private static Hook HorizontalDashHook;
        private static Hook ShatteringIcePillarsHook;

        public override void OnModLoad()
        {
            HorizontalDashHook = Apply(HorizontalDashMethod, "DoAttack_HorizontalDash", "dash changes", HorizontalDash_Detour);
            ShatteringIcePillarsHook = Apply(ShatteringIcePillarsMethod, "DoAttack_ShatteringIcePillars", "pillar changes", ShatteringIcePillars_Detour);
        }

        public override void Unload()
        {
            HorizontalDashHook?.Dispose();
            HorizontalDashHook = null;
            ShatteringIcePillarsHook?.Dispose();
            ShatteringIcePillarsHook = null;
        }

        private Hook Apply(MethodInfo method, string methodName, string disabledChanges, Hook_Attack detour)
        {
            if (method == null)
            {
                InfernalEclipseAPI.Instance.Logger.Error($"[IEoR]: {this} could not find CryogenBehaviorOverride.{methodName}; Ragnarok Cryogen {disabledChanges} are disabled.");
                return null;
            }

            Hook hook = new(method, detour);
            hook.Apply();
            return hook;
        }

        private static void HorizontalDash_Detour(Orig_Attack orig, NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower)
        {
            CryogenWindupShake shake = npc.GetGlobalNPC<CryogenWindupShake>();
            npc.position -= shake.AppliedOffset;
            shake.AppliedOffset = Vector2.Zero;

            if (!InfernalWorld.RagnarokModeEnabled)
            {
                orig(npc, target, ref attackTimer, ref attackState, attackPower);
                return;
            }

            bool doublesBack = (int)npc.Infernum().ExtraAI[ChargeCounterIndex] % 2 == CryogenChanges.DoubleBackChargeParity;

            if (doublesBack && attackTimer == CryogenChanges.DoubleBackTurnTick && npc.Infernum().ExtraAI[AttackSubstateIndex] == ChargingSubstate)
            {
                EmitIceParticles(npc.Center, CryogenChanges.DoubleBackParticleSpeed, CryogenChanges.DoubleBackParticleCount);
                SoundEngine.PlaySound(SoundID.Item28, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.velocity = CalamityUtils.SafeDirectionTo(npc, target.Center) * npc.velocity.Length();
                    npc.netUpdate = true;
                }
            }

            orig(npc, target, ref attackTimer, ref attackState, attackPower + CryogenChanges.HorizontalDashAttackPowerBonus);

            float currentSubstate = npc.Infernum().ExtraAI[AttackSubstateIndex];
            if (doublesBack && (currentSubstate == SpinSubstate || currentSubstate == AimingSubstate))
            {
                //This actually shakes the hitbox too but if it causes any issues i'll fix it
                shake.AppliedOffset = Main.rand.NextVector2Circular(CryogenChanges.WindupShakeStrength, CryogenChanges.WindupShakeStrength);
                npc.position += shake.AppliedOffset;
                EmitIceParticles(npc.Center, CryogenChanges.WindupParticleSpeed, CryogenChanges.WindupParticleCount);
            }
        }

        private static void ShatteringIcePillars_Detour(Orig_Attack orig, NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower)
        {
            Vector2 velocityBeforeOrig = npc.velocity;
            orig(npc, target, ref attackTimer, ref attackState, attackPower);

            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            Vector2 orbitPosition = target.Center + (target.AngleTo(npc.Center) + CryogenChanges.PillarOrbitAngularStepPerTick).ToRotationVector2() * CryogenChanges.PillarOrbitRadius;
            float orbitSpeed = MathF.Min(npc.Distance(orbitPosition), CryogenChanges.PillarOrbitMaxSpeed);

            npc.velocity = Terraria.Utils.MoveTowards(velocityBeforeOrig, CalamityUtils.SafeDirectionTo(npc, orbitPosition) * orbitSpeed + target.velocity, CryogenChanges.PillarOrbitAcceleration);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                TrackPlayerAltitude();
        }

        private static void TrackPlayerAltitude()
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is not IcePillar pillar || pillar.MaxPillarHeight == 0f || pillar.Time >= CryogenChanges.PillarHeightTrackingEndTime)
                    continue;

                Player nearestPlayer = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
                float trackedHeight = MathF.Max(0f, projectile.Top.Y - nearestPlayer.Top.Y) + IcePillar.StartingHeight + PillarHeightMargin + MathF.Abs(nearestPlayer.velocity.Y * PillarVerticalVelocityLead) + CryogenChanges.PillarHeightBonus;

                if (trackedHeight <= pillar.MaxPillarHeight)
                    continue;

                pillar.MaxPillarHeight = trackedHeight;

                if (pillar.Time % PillarSyncInterval == 0f)
                    projectile.netUpdate = true;
            }
        }
    }
}