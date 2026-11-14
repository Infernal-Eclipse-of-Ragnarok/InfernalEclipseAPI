using System.IO;
using System.Reflection;
using InfernalEclipseAPI.Core.World;
using System;
using CalamityMod;
using CalamityMod.Particles;
using InfernumMode;
using InfernumMode.Common.Graphics.Particles;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Cryogen;
using InfernumMode.Core.GlobalInstances.Systems;
using MonoMod.RuntimeDetour;
using Terraria.Audio;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.Cryogen.CryogenBehaviorOverride;
using Microsoft.Xna.Framework;
using InfernumUtilities = InfernumMode.Utilities;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.CryogenOverrides
{
    internal sealed class CryogenWindupShake : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public Vector2 AppliedOffset;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>();
    }
    //no detour because im lazy 
    internal sealed class CryogenRingSpike : IcicleSpike
    {
        public const float LaunchSpeed = 18f;
        private const int SpikeLifetime = 170;

        public float OrbitRadius;
        public bool Clockwise;

        private Vector2 orbitCenter;
        private float spinTime;

        public override string Texture => "InfernumMode/Content/BehaviorOverrides/BossAIs/Cryogen/IcicleSpike";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = SpikeLifetime;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.alpha = Math.Max(Projectile.alpha - 12, 0);

            if (Projectile.velocity != Vector2.Zero)
            {
                if (Projectile.velocity.Length() < 40f * IcicleSpike.SpeedPower)
                    Projectile.velocity *= 1f + IcicleSpike.SpeedPower * 0.03f;

                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                if (Projectile.timeLeft % 10 == 0)
                    GeneralParticleHandler.SpawnParticle(new SnowyIceParticle(Projectile.Center, Projectile.velocity * 0.5f, Color.White, Main.rand.NextFloat(0.75f, 0.95f), 30));

                return;
            }
            if (spinTime == 0f)
                orbitCenter = Projectile.Center - Projectile.ai[0].ToRotationVector2() * OrbitRadius;

            float angle = Projectile.ai[0] + (Clockwise ? 1f : -1f) * 0.05f * spinTime;
            Projectile.Center = orbitCenter + angle.ToRotationVector2() * OrbitRadius;
            Projectile.rotation = angle - MathHelper.PiOver2;
            spinTime++;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(OrbitRadius);
            writer.Write(Clockwise);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            OrbitRadius = reader.ReadSingle();
            Clockwise = reader.ReadBoolean();
        }

        public override bool? CanDamage() => Projectile.alpha < 20;
    }

    internal sealed class CryogenRagnarokHooks : ModSystem
    {
        private static readonly MethodInfo HorizontalDashMethod = typeof(CryogenBehaviorOverride).GetMethod("DoAttack_HorizontalDash", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo ShatteringIcePillarsMethod = typeof(CryogenBehaviorOverride).GetMethod("DoAttack_ShatteringIcePillars", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo PreAIMethod = typeof(CryogenBehaviorOverride).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo[] CycleGetterMethods =
        {
            typeof(CryogenBehaviorOverride).GetMethod("get_Subphase4AttackCycle", LumUtils.UniversalBindingFlags),
            typeof(CryogenBehaviorOverride).GetMethod("get_Subphase5AttackCycle", LumUtils.UniversalBindingFlags),
            typeof(CryogenBehaviorOverride).GetMethod("get_Subphase6AttackCycle", LumUtils.UniversalBindingFlags),
        };

        private const int AttackSubstateIndex = 0;
        private const int ChargeCounterIndex = 1;
        private const float SpinSubstate = 1f;
        private const float AimingSubstate = 2f;
        private const float ChargingSubstate = 3f;

        private const CryogenAttackState RingDashAttack = (CryogenAttackState)8;
        private const int FirstRingDashSubphase = 3;

        
        private const int RingSubstateIndex = 0;
        private const int RingCompletedPassesIndex = 1;
        private const int RingCenterXIndex = 2;
        private const int RingCenterYIndex = 3;
        private const float SpawnRingSubstate = 0f;
        private const float RetreatSubstate = 1f;
        private const float DashSubstate = 2f;

        //the dash gotta reach the ring so keep that in mind when changing this 
        private const float RetreatDistance = 520f;
        private const float DashSpeed = 40f;
        private const float DashDurationTicks = 24f;

        private delegate void Orig_Attack(NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower);

        private delegate void Hook_Attack(Orig_Attack orig, NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower);

        private delegate bool Orig_PreAI(CryogenBehaviorOverride self, NPC npc);

        private delegate bool Hook_PreAI(Orig_PreAI orig, CryogenBehaviorOverride self, NPC npc);

        private delegate CryogenAttackState[] Orig_Cycle();

        private delegate CryogenAttackState[] Hook_Cycle(Orig_Cycle orig);

        private static readonly CryogenAttackState[][] ExtendedCycles = new CryogenAttackState[CycleGetterMethods.Length][];
        private static readonly Hook[] CycleHooks = new Hook[CycleGetterMethods.Length];

        private static Hook HorizontalDashHook;
        private static Hook ShatteringIcePillarsHook;
        private static Hook PreAIHook;

        public override void OnModLoad()
        {
            HorizontalDashHook = Apply(HorizontalDashMethod, "DoAttack_HorizontalDash", "dash changes", new Hook_Attack(HorizontalDash_Detour));
            ShatteringIcePillarsHook = Apply(ShatteringIcePillarsMethod, "DoAttack_ShatteringIcePillars", "pillar changes", new Hook_Attack(ShatteringIcePillars_Detour));

            PreAIHook = Apply(PreAIMethod, "PreAI", "dash ring attack", new Hook_PreAI(PreAI_Detour));
            if (PreAIHook == null)
                return;

            for (int i = 0; i < CycleGetterMethods.Length; i++)
            {
                int cycleIndex = i;
                CycleHooks[i] = Apply(CycleGetterMethods[i], $"Subphase{FirstRingDashSubphase + 1 + i}AttackCycle", "dash ring attack", new Hook_Cycle(orig => ExtendCycle(orig, cycleIndex)));
            }
        }

        public override void Unload()
        {
            Unhook(ref HorizontalDashHook);
            Unhook(ref ShatteringIcePillarsHook);
            Unhook(ref PreAIHook);
            for (int i = 0; i < CycleHooks.Length; i++)
                Unhook(ref CycleHooks[i]);
            Array.Clear(ExtendedCycles, 0, ExtendedCycles.Length);
        }

        private static void Unhook(ref Hook hook)
        {
            hook?.Dispose();
            hook = null;
        }

        private Hook Apply(MethodInfo method, string methodName, string disabledChanges, Delegate detour)
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

            const float attackPowerBonus = 0.25f;

            bool doublesBack = (int)npc.Infernum().ExtraAI[ChargeCounterIndex] % 2 == 1;

            if (doublesBack && attackTimer == 48f && npc.Infernum().ExtraAI[AttackSubstateIndex] == ChargingSubstate)
            {
                EmitIceParticles(npc.Center, 5f, 30);
                SoundEngine.PlaySound(SoundID.Item28, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.velocity = CalamityUtils.SafeDirectionTo(npc, target.Center) * npc.velocity.Length();
                    npc.netUpdate = true;
                }
            }

            orig(npc, target, ref attackTimer, ref attackState, attackPower + attackPowerBonus);

            float currentSubstate = npc.Infernum().ExtraAI[AttackSubstateIndex];
            if (doublesBack && (currentSubstate == SpinSubstate || currentSubstate == AimingSubstate))
            {
                //This actually shakes the hitbox too but if it causes any issues i'll fix it
                shake.AppliedOffset = Main.rand.NextVector2Circular(14f, 14f);
                npc.position += shake.AppliedOffset;
                EmitIceParticles(npc.Center, 2.5f, 1);
            }
        }

        private static void ShatteringIcePillars_Detour(Orig_Attack orig, NPC npc, Player target, ref float attackTimer, ref float attackState, float attackPower)
        {
            Vector2 velocityBeforeOrig = npc.velocity;
            orig(npc, target, ref attackTimer, ref attackState, attackPower);

            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            Vector2 orbitPosition = target.Center + (target.AngleTo(npc.Center) + 0.03f).ToRotationVector2() * 380f;
            float orbitSpeed = MathF.Min(npc.Distance(orbitPosition), 15f);

            npc.velocity = Terraria.Utils.MoveTowards(velocityBeforeOrig, CalamityUtils.SafeDirectionTo(npc, orbitPosition) * orbitSpeed + target.velocity, 1.4f);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                TrackPlayerAltitude();
        }

        private static CryogenAttackState[] ExtendCycle(Orig_Cycle orig, int cycleIndex)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return orig();

            if (ExtendedCycles[cycleIndex] == null)
            {
                CryogenAttackState[] original = orig();
                //puts the ring dash attack in the middle of the cycle
                int ringDashPosition = original.Length / 2;
                CryogenAttackState[] extended = new CryogenAttackState[original.Length + 1];
                Array.Copy(original, 0, extended, 0, ringDashPosition);
                extended[ringDashPosition] = RingDashAttack;
                Array.Copy(original, ringDashPosition, extended, ringDashPosition + 1, original.Length - ringDashPosition);
                ExtendedCycles[cycleIndex] = extended;
            }

            return ExtendedCycles[cycleIndex];
        }

        private static bool PreAI_Detour(Orig_PreAI orig, CryogenBehaviorOverride self, NPC npc)
        {
            //Runs after orig so subphase transitions have already reset the attack state
            bool result = orig(self, npc);

            int subphase = (int)npc.ai[0];
            Player target = Main.player[npc.target];
            if (!InfernalWorld.RagnarokModeEnabled || subphase < FirstRingDashSubphase || !target.active || target.dead)
                return result;

            CryogenAttackState[] cycle = ExtendedCycles[subphase - FirstRingDashSubphase];
            if (cycle != null && cycle[(int)npc.ai[2] % cycle.Length] == RingDashAttack)
                DoRingDashAttack(npc, target, ref npc.ai[1], ref npc.ai[2]);

            return result;
        }

        private static void DoRingDashAttack(NPC npc, Player target, ref float attackTimer, ref float attackState)
        {
            ref float substate = ref npc.Infernum().ExtraAI[RingSubstateIndex];
            ref float completedPasses = ref npc.Infernum().ExtraAI[RingCompletedPassesIndex];
            ref float ringCenterX = ref npc.Infernum().ExtraAI[RingCenterXIndex];
            ref float ringCenterY = ref npc.Infernum().ExtraAI[RingCenterYIndex];
            Vector2 ringCenter = new(ringCenterX, ringCenterY);
            bool isAuthority = Main.netMode != NetmodeID.MultiplayerClient;

            npc.rotation = npc.velocity.X * 0.02f;

            if (substate == SpawnRingSubstate)
            {
                npc.damage = 0;
                if (!isAuthority)
                    return;

                ringCenterX = npc.Center.X;
                ringCenterY = npc.Center.Y;
                SpawnRingSpikes(npc, count: 22, orbitRadius: 155f, clockwise: true);
                SpawnRingSpikes(npc, count: 7, orbitRadius: 78f, clockwise: false);

                substate = RetreatSubstate;
                attackTimer = 0f;
                npc.netUpdate = true;
            }
            else if (substate == RetreatSubstate)
            {
                npc.damage = 0;
                Vector2 towardTarget = Terraria.Utils.SafeNormalize(target.Center - ringCenter, -Vector2.UnitY);
                Vector2 retreatPosition = ringCenter - towardTarget * RetreatDistance;
                npc.velocity = Terraria.Utils.MoveTowards(npc.velocity, CalamityUtils.SafeDirectionTo(npc, retreatPosition) * 15f, 2.5f);

                //The tick limit keeps the attack from stalling if Cryogen never reaches the retreat point
                if (isAuthority && (npc.Distance(retreatPosition) < 60f || attackTimer >= 75f))
                {
                    substate = DashSubstate;
                    attackTimer = 0f;
                    npc.velocity = CalamityUtils.SafeDirectionTo(npc, ringCenter) * DashSpeed;
                    npc.netUpdate = true;
                }
            }
            else if (substate == DashSubstate && isAuthority)
            {
                ShatterRing(npc, ringCenter);

                if (attackTimer >= DashDurationTicks)
                {
                    npc.velocity *= 0.6f;
                    completedPasses++;
                    substate = SpawnRingSubstate;
                    attackTimer = 0f;

                    if (completedPasses >= 3f)
                    {
                        completedPasses = 0f;
                        ringCenterX = 0f;
                        ringCenterY = 0f;
                        attackState++;
                        npc.TargetClosest();
                    }

                    npc.netUpdate = true;
                }
            }
        }

        private static void SpawnRingSpikes(NPC npc, int count, float orbitRadius, bool clockwise)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi * i / count;

                ProjectileSpawnManagementSystem.PrepareProjectileForSpawning(projectile =>
                {
                    CryogenRingSpike spike = (CryogenRingSpike)projectile.ModProjectile;
                    spike.OrbitRadius = orbitRadius;
                    spike.Clockwise = clockwise;
                });

                Vector2 spikePosition = npc.Center + angle.ToRotationVector2() * orbitRadius;
                InfernumUtilities.NewProjectileBetter(spikePosition, Vector2.Zero, ModContent.ProjectileType<CryogenRingSpike>(), IcicleSpikeDamage, 0f, -1, angle, npc.whoAmI);
            }
        }

        private static void ShatterRing(NPC npc, Vector2 ringCenter)
        {
            float distanceToCenter = npc.Distance(ringCenter);
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is not CryogenRingSpike spike || projectile.ai[1] != npc.whoAmI || projectile.velocity != Vector2.Zero || distanceToCenter >= spike.OrbitRadius)
                    continue;

                projectile.velocity = Terraria.Utils.SafeNormalize(projectile.Center - ringCenter, Vector2.UnitY) * CryogenRingSpike.LaunchSpeed * IcicleSpike.SpeedPower;
                projectile.netUpdate = true;
            }
        }

        private static void TrackPlayerAltitude()
        {
            const float heightBonus = 150f;
            const float heightMargin = 100f;
            const float verticalVelocityLead = 15f;

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is not IcePillar pillar || pillar.MaxPillarHeight == 0f || pillar.Time >= 45f)
                    continue;

                Player nearestPlayer = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
                float trackedHeight = MathF.Max(0f, projectile.Top.Y - nearestPlayer.Top.Y) + IcePillar.StartingHeight + heightMargin + MathF.Abs(nearestPlayer.velocity.Y * verticalVelocityLead) + heightBonus;

                if (trackedHeight <= pillar.MaxPillarHeight)
                    continue;

                pillar.MaxPillarHeight = trackedHeight;

                //Throttled so a growing MaxPillarHeight isn't sent every tick
                if (pillar.Time % 6f == 0f)
                    projectile.netUpdate = true;
            }
        }
    }
}