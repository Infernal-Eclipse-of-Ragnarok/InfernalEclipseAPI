using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Projectiles.Boss;
using CalamityMod.UI.DialogueDisplay;
using CalamityMod.UI.DialogueDisplay.DisplayEffects;
using CalamityMod.World;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.DoG;
using InfernumMode.Content.BossIntroScreens;
using InfernumMode.Content.Projectiles.Pets;
using InfernumMode.Core.Netcode;
using InfernumMode.Core.Netcode.Packets;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System.Linq;
using System.Reflection;
using Terraria.Audio;
using Terraria.DataStructures;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.DoG.DoGPhase1HeadBehaviorOverride;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.DoG.DoGPhase2HeadBehaviorOverride;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.DoGOverrides
{
    public class DoGChanges : GlobalNPC
    {
        public const int HasCompletedPhase2HealIndex = 36;

        public static bool DesperationHasTriggered { get; set; }
        public static bool DesperationInitialized = false;
        public static bool DesperationCanDie = false;
        public static bool DesperationEnteringPortal = false;
        public static bool DesperationHasEmergedFromFirstPortal = false;
        public static bool HasDisplayedPhase2Text = false;
        public static int DesperationEntryTimer = 0;
        public static int DesperationChargeTimer = 0;
        public static int DesperationEntryPortalIndex = -1;
        public static bool DesperationHeadEnteredPortal = false;
        public static int DesperationPostEntryTimer = 0;

        /*
        private static readonly int AcceleratingFireballDamage = 380;
        private static readonly int LaserWallDamage = 400;
        */

        #region Coil Punishment

        private int coilDetectionTimer;
        private int coilLaserWallTimer;
        private bool coilLaserWallActive;

        // Store the phase the punishment started in so a phase transition does not change the laser pattern halfway through an instance.
        private bool coilLaserWallsPhase2;

        private const int CoilDetectionTime = 45;
        private const float CoilDetectionDistance = 650f;
        private const float CoilTangentialThreshold = 0.4f;

        // Exactly one Calamity laser-wall cycle.
        private const int CoilLaserWallDuration = 120;



        #endregion

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
        {
            return npc.type == ModContent.NPCType<DevourerofGodsHead>();
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            ResetDesperationState();
        }

        public override bool PreAI(NPC npc)
        {
            if (!npc.active)
                ResetDesperationState();

            if (npc.Infernum().ExtraAI[DeathAnimationTimerIndex] - 120 - 260 - 30 >= 75)
            {
                ResetDesperationState();
            }

            HandleMountRestrictions();

            if (InfernalWorld.RagnarokModeEnabled)
            {
                ref float phaseCycleTimer = ref npc.Infernum().ExtraAI[PhaseCycleTimerIndex];
                ref float uncoilTimer = ref npc.Infernum().ExtraAI[InitialUncoilTimerIndex];
                ref float passiveAttackDelay = ref npc.Infernum().ExtraAI[PassiveAttackDelayTimerIndex];
                ref float sentinelAttackTimer = ref npc.Infernum().ExtraAI[SentinelAttackTimerIndex];

                Player target = Main.player[npc.target];

                if (!target.dead && target.active)
                {
                    if (!InPhase2 && CurrentPhase2TransitionState == Phase2TransitionState.NotEnteringPhase2 && uncoilTimer >= 45f && phaseCycleTimer % (PassiveMovementTimeP1 + AggressiveMovementTimeP1) >= AggressiveMovementTimeP1)
                    {
                        if (phaseCycleTimer % 75f == 0f && phaseCycleTimer % 150f != 0f && passiveAttackDelay >= 300f)
                            SpawnPhase1LaserBeamLaserWall(npc, target);
                    }

                    if (InPhase2)
                    {
                        int wrappedPhaseCycleTimer = (int)phaseCycleTimer % (PassiveMovementTimeP2 + AggressiveMovementTimeP2);

                        float lifeRatio = npc.life / (float)npc.lifeMax;

                        bool finalPhase = lifeRatio < FinalPhaseLifeRatio;

                        bool doPassiveMovement = wrappedPhaseCycleTimer >= AggressiveMovementTimeP2 && !finalPhase;

                        bool sentinelAttackActive = sentinelAttackTimer > 0f;

                        // Only use our laser walls during the passive phase where Infernum's sentinel attack is absent.
                        if (doPassiveMovement && !sentinelAttackActive && passiveAttackDelay >= 300f && npc.Opacity >= 1f)
                        {
                            SpawnPhase2LaserBeamLaserWall(npc, target, wrappedPhaseCycleTimer);
                        }
                    }
                }
            }

            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (!npc.active)
                return;

            if (!DesperationHasTriggered && !IntroScreenManager.ScreenIsObstructed) 
            {
                Player target = Main.player[npc.target];
                Vector2 destination = target.Center + target.velocity * 2f;

                float lifeRatio = npc.life / (float)npc.lifeMax;
                float turnSpeed = CalamityWorld.death ? 0.33f : 0.3f;

                if (Main.expertMode)
                    turnSpeed += 0.06f * (1f - (lifeRatio * 0.75f + 0.25f));

                if (npc.Distance(destination) > 2400f)
                    npc.velocity += SafeNormalize(destination - npc.Center, Vector2.UnitY) * turnSpeed;
            }

            if (npc.Infernum().ExtraAI[Phase2IntroductionAnimationTimerIndex] >= DoGPhase2IntroPortalGate.Phase2AnimationTime && !HasDisplayedPhase2Text && !(npc.Infernum().ExtraAI[DeathAnimationTimerIndex] > 1))
            {
                LumUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.DoGPhase2", Color.Cyan);
                DialogueDisplaySystem.StartDialogue("Mods.CalamityMod.DevourerOfGods.Phases", npc, 2, 120, false, new BossText());
                HasDisplayedPhase2Text = true;
            }

            if (DesperationHasTriggered || DesperationCanDie)
            {
                npc.Calamity().CanHaveBossHealthBar = true;

                foreach (NPC segment in Main.ActiveNPCs)
                {
                    if (segment.type == ModContent.NPCType<DevourerofGodsBody>() ||
                        segment.type == ModContent.NPCType<DevourerofGodsTail>())
                    {
                        segment.damage = 0;
                        segment.dontTakeDamage = true;
                        segment.netUpdate = true;
                    }
                }
            }

            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            if (!DesperationHasTriggered && npc.Infernum().ExtraAI[InPhase2FlagIndex] == 1f && npc.Infernum().ExtraAI[HasCompletedPhase2HealIndex] != 1f)
            {
                int max = (int)(npc.lifeMax * 0.4f);
                int heal = max / 120;
                npc.life += heal;

                npc.netUpdate = true;

                if (npc.life >= npc.lifeMax)
                {
                    npc.life = npc.lifeMax;
                    npc.Infernum().ExtraAI[HasCompletedPhase2HealIndex] = 1f;
                }
            }

            npc.takenDamageMultiplier = 1f;

            UpdateCoilLaserWalls(npc);

            if (IntroScreenManager.ScreenIsObstructed)
                return;

            if (InPhase2)
            {
                if (npc.damage == 650)
                    npc.damage = 885;
            }
            else if (npc.damage == 600)
                npc.damage = 800;
        }

        public override bool CheckDead(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return true;

            if (npc.type != ModContent.NPCType<DevourerofGodsHead>())
                return true;

            if (InPhase2 && !DesperationHasTriggered && !DesperationCanDie)
            {
                StartDesperationFrom(npc);
                return false;
            }

            return true;
        }

        private static bool DetectCoiling(NPC npc, Player target)
        {
            // Don't mistake Infernum's normal chomp/lunge for coiling.
            if (npc.Infernum().ExtraAI[ChompEffectsCountdownIndex] > 0f)
                return false;

            if (npc.Distance(target.Center) > CoilDetectionDistance)
                return false;

            if (npc.velocity.LengthSquared() <= 0.001f)
                return false;

            Vector2 velocityDirection = npc.velocity.SafeNormalize(Vector2.UnitY);

            Vector2 directionToTarget = Utilities.SafeDirectionTo(npc, target.Center);

            //  1 = moving directly toward the player.
            //  0 = moving perpendicular/tangentially.
            // -1 = moving directly away from the player.
            float approachDot = Vector2.Dot(velocityDirection, directionToTarget);

            return Math.Abs(approachDot) < CoilTangentialThreshold;
        }

        private void UpdateCoilLaserWalls(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled || DesperationHasTriggered)
            {
                ResetCoilLaserWalls();
                return;
            }

            if (npc.target < 0 || npc.target >= Main.maxPlayers)
            {
                ResetCoilLaserWalls();
                return;
            }

            Player target = Main.player[npc.target];

            if (!target.active || target.dead)
            {
                ResetCoilLaserWalls();
                return;
            }

            bool coiling = DetectCoiling(npc, target);


            // Once activated, the laser walls repeat continuously every 120 ticks. The 45-tick detection period is NOT repeated.
            if (coilLaserWallActive)
            {
                // The player stopped coiling. End the punishment and require a fresh 45-tick detection next time.
                if (!coiling)
                {
                    coilLaserWallActive = false;
                    coilLaserWallTimer = 0;
                    coilDetectionTimer = 0;
                    coilLaserWallsPhase2 = false;

                    npc.netUpdate = true;
                    return;
                }

                DoCoilLaserWalls(npc, target);

                coilLaserWallTimer++;

                // Immediately begin the next cycle.
                if (coilLaserWallTimer >= CoilLaserWallDuration)
                    coilLaserWallTimer = 0;

                return;
            }

            if (coiling)
            {
                coilDetectionTimer++;

                if (coilDetectionTimer >= CoilDetectionTime)
                {
                    coilDetectionTimer = 0;
                    coilLaserWallTimer = 0;

                    // Lock the laser-wall pattern for this continuous punishment instance.
                    coilLaserWallsPhase2 = InPhase2;
                    coilLaserWallActive = true;

                    npc.netUpdate = true;
                }
            }
            else
            {
                coilDetectionTimer = Math.Max(0, coilDetectionTimer - 2);
            }
        }

        private void ResetCoilLaserWalls()
        {
            coilDetectionTimer = 0;
            coilLaserWallTimer = 0;
            coilLaserWallActive = false;
            coilLaserWallsPhase2 = false;
        }

        private void DoCoilLaserWalls(NPC npc, Player target)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (coilLaserWallsPhase2)
                DoPhase2CoilLaserWalls(npc, target);
            else
                DoPhase1CoilLaserWalls(npc, target);
        }

        private void DoPhase1CoilLaserWalls(NPC npc, Player target)
        {
            const int laserWallInterval = 120;

            if (coilLaserWallTimer % laserWallInterval != 0)
                return;

            const float time = 0.5f;
            const float spacing = 160f;

            int beamType = Main.rand.Next(0, 6);

            Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center + Main.rand.NextVector2CircularEdge(600f, 600f), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), DevourerofGodsHead.LaserWallDamage, 0f, Main.myPlayer, time, spacing, beamType);
        }

        private static void SpawnPhase1LaserBeamLaserWall(NPC npc, Player target)
        {
            const float time = 0.5f;
            const float spacing = 160f;

            int beamType = Main.rand.Next(0, 2);

            Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center + Main.rand.NextVector2CircularEdge(600f, 600f), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), DevourerofGodsHead.LaserWallDamage, 0f, Main.myPlayer, time, spacing, beamType);
        }

        private void DoPhase2CoilLaserWalls(NPC npc, Player target)
        {
            const float spacing = 320f;
            const int miniInterval = 12;
            const int megaInterval = 120;
            const float time = 0.35f;

            for (int i = 0; i < 3; i++)
            {
                if ((coilLaserWallTimer - miniInterval * i) % megaInterval != 0)
                    continue;

                int beamType = 2 - i;

                // Normal laser wall.
                Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center + Main.rand.NextVector2CircularEdge(600f, 600f), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), DevourerofGodsHead.LaserWallDamage, 0f, Main.myPlayer, time, spacing, beamType);

                // The third part of each sequence gets the giant center beam, just like Calamity's Death + phase 6 laser-wall attack.
                if (i == 2)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<DoGLaserWallsBigBeam>(), DevourerofGodsHead.LaserWallMiddleBeamDamage, 0f, Main.myPlayer, time, 0f, i);
                }
            }
        }

        internal static void SpawnPhase2LaserBeamLaserWall(NPC npc, Player target, int wrappedPhaseCycleTimer)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            const float spacing = 320f;
            const float time = 0.35f;

            // Passive movement begins at tick 720.
            const int firstSetStart = AggressiveMovementTimeP2 + 36;
            // One complete set every 120 ticks.
            const int setInterval = 120;
            // Three walls per set, 12 ticks apart.
            const int miniInterval = 12;

            const int totalSets = 3;

            for (int set = 0; set < totalSets; set++)
            {
                int setStart = firstSetStart + set * setInterval;

                for (int i = 0; i < 3; i++)
                {
                    int fireTime = setStart + i * miniInterval;

                    if (wrappedPhaseCycleTimer != fireTime)
                        continue;

                    int beamType = 2 - i;

                    Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center + Main.rand.NextVector2CircularEdge(600f, 600f), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), DevourerofGodsHead.LaserWallDamage, 0f, Main.myPlayer, time, spacing, beamType);

                    // Third wall gets the giant center beam.
                    if (i == 2)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<DoGLaserWallsBigBeam>(), DevourerofGodsHead.LaserWallMiddleBeamDamage, 0f, Main.myPlayer, time, 0f, i);
                    }
                }
            }
        }

        public void RunDesperationAI(NPC npc, Player target, ref float performingSpecialAttack, ref float specialAttackTimer, ref float segmentFadeType, ref float damageImmunityCountdown)
        {
            if (!DesperationCanDie)
                npc.Infernum().ExtraAI[DeathAnimationTimerIndex] = 0f;

            npc.TargetClosestIfTargetIsInvalid();
            if (!npc.HasValidTarget)
            {
                ResetDesperationState();
                Despawn(npc);
                return;
            }

            InflictDspDebuff();

            CalamityGlobalNPC.DoGHead = npc.whoAmI;
            CalamityGlobalNPC.DoGP2 = npc.whoAmI;

            npc.timeLeft = 7200;
            npc.dontTakeDamage = npc.Opacity < 0.5f;
            npc.damage = npc.dontTakeDamage ? 0 : npc.defDamage;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;

            if (!DesperationInitialized)
            {
                DesperationInitialized = true;

                if (npc.velocity.LengthSquared() < 16f)
                    npc.velocity = Utilities.SafeDirectionTo(npc, target.Center) * 24f;
                npc.Opacity = 1f;
                npc.damage = 0;
                npc.dontTakeDamage = true;

                npc.Infernum().ExtraAI[SpecialAttackTimerIndex] = 0f;
                npc.Infernum().ExtraAI[ChargeGatePortalTelegraphTimeIndex] = 0f;

                PreDesperationProjectileCleanup();

                npc.netUpdate = true;
            }

            if (!DesperationCanDie || !DesperationHasEmergedFromFirstPortal)
            {
                npc.Infernum().ExtraAI[DeathAnimationTimerIndex] = 0f;
                npc.life = Math.Max(npc.life, 1);
            }

            if (DesperationEnteringPortal)
            {
                DoDesperationPortalEntry(npc, target, ref segmentFadeType);

                return;
            }

            performingSpecialAttack = 1f;

            // Force charge gates forever during desperation.
            npc.Infernum().ExtraAI[SpecialAttackTypeIndex] =
                (int)SpecialAttackType.ChargeGates;

            DoDesperationChargeGates(npc, target, ref specialAttackTimer, ref segmentFadeType, ref damageImmunityCountdown);

            npc.spriteDirection = (npc.velocity.X > 0f).ToDirectionInt();
            npc.rotation = npc.velocity.ToRotation() + PiOver2;
        }

        private static void DoDesperationPortalEntry(NPC npc, Player target, ref float segmentFadeType)
        {
            const int PortalCreationTime = 1;
            const int PostHeadEntryDelay = 90;
            const int FailsafeTime = 180;

            if (!target.active || target.dead)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            if (DesperationEntryTimer == 0)
            {
                PreDesperationProjectileCleanup();

                npc.damage = 0;
                npc.dontTakeDamage = true;
                npc.Opacity = 1f;

                FadeToAntimatterForm = 0f;
                ChargePortalIndex = -1;
                GeneralPortalIndex = -1;
                DesperationEntryPortalIndex = -1;
                DesperationHeadEnteredPortal = false;
                DesperationPostEntryTimer = 0;

                Vector2 awayFromTarget = Utilities.SafeDirectionTo(target, npc.Center);

                if (npc.velocity.LengthSquared() < 16f)
                    npc.velocity = awayFromTarget * 36f;
                else if (Vector2.Dot(npc.velocity.SafeNormalize(Vector2.UnitX), awayFromTarget) < 0.2f)
                    npc.velocity = awayFromTarget * Math.Max(npc.velocity.Length(), 36f);

                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && DesperationEntryTimer == PortalCreationTime)
            {
                ClearChargeGateFlags();

                Vector2 portalPosition =
                    npc.Center + npc.velocity.SafeNormalize(Utilities.SafeDirectionTo(target, npc.Center)) * 575f;

                DesperationEntryPortalIndex = Projectile.NewProjectile(
                    npc.GetSource_FromAI(),
                    portalPosition,
                    Vector2.Zero,
                    ModContent.ProjectileType<DoGChargeGate>(),
                    0,
                    0f,
                    Main.myPlayer,
                    0f,
                    120f
                );

                if (Main.projectile.IndexInRange(DesperationEntryPortalIndex))
                {
                    Projectile portal = Main.projectile[DesperationEntryPortalIndex];

                    portal.localAI[0] = 1f;

                    DoGChargeGate gate = portal.ModProjectile<DoGChargeGate>();
                    gate.IsGeneralPortalIndex = true;

                    portal.netUpdate = true;
                }

                GeneralPortalIndex = DesperationEntryPortalIndex;
                npc.netUpdate = true;
            }

            npc.damage = 0;
            npc.dontTakeDamage = true;
            segmentFadeType = (int)BodySegmentFadeType.EnteringPortal;

            if (Main.projectile.IndexInRange(DesperationEntryPortalIndex) &&
                Main.projectile[DesperationEntryPortalIndex].active)
            {
                Projectile portal = Main.projectile[DesperationEntryPortalIndex];

                if (!DesperationHeadEnteredPortal)
                {
                    npc.velocity = Utilities.SafeDirectionTo(npc, portal.Center) *
                                   MathHelper.Lerp(npc.velocity.Length(), 105f, 0.15f);

                    if (npc.Hitbox.Intersects(portal.Hitbox))
                    {
                        npc.Opacity = 0f;
                        DesperationHeadEnteredPortal = true;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    // Keep the head invisible while body/tail segments continue entering the same portal.
                    npc.Opacity = 0f;
                    npc.velocity *= 0.96f;
                    DesperationPostEntryTimer++;
                }
            }
            else
            {
                npc.velocity = npc.velocity.SafeNormalize(Utilities.SafeDirectionTo(target, npc.Center)) *
                               MathHelper.Lerp(npc.velocity.Length(), 60f, 0.08f);
            }

            DesperationEntryTimer++;

            bool finishedEntry =
                DesperationHeadEnteredPortal && DesperationPostEntryTimer >= PostHeadEntryDelay;

            bool failsafe =
                DesperationEntryTimer >= FailsafeTime;

            if (finishedEntry || failsafe)
            {
                DesperationEnteringPortal = false;
                DesperationInitialized = true;

                npc.Opacity = 0f;
                npc.damage = 0;
                npc.dontTakeDamage = true;

                DesperationEntryTimer = 0;
                DesperationPostEntryTimer = 0;
                DesperationHeadEnteredPortal = false;

                DesperationChargeTimer = 0;
                npc.Infernum().ExtraAI[ChargeGatePortalTelegraphTimeIndex] = 0f;

                // Now it is safe to clear the entry portal.
                ClearChargeGateFlags();

                npc.netUpdate = true;
            }
        }

        private static void DoDesperationChargeGates(NPC npc, Player target, ref float attackTimer, ref float segmentFadeType, ref float damageImmunityCountdown)
        {
            const int AttackCycleTime = 100;
            const int PortalSpawnTime = 10;
            const int BasePortalTelegraphTime = 25;

            float lifeRatio = npc.life / (float)npc.lifeMax;

            int portalTelegraphBase = BasePortalTelegraphTime;

            if (lifeRatio < 0.05f)
                portalTelegraphBase -= 5;

            ref float portalTelegraphTime = ref npc.Infernum().ExtraAI[ChargeGatePortalTelegraphTimeIndex];

            if (portalTelegraphTime <= 0f)
            {
                portalTelegraphTime = portalTelegraphBase;
                npc.netUpdate = true;
            }

            int wrappedAttackTimer = DesperationChargeTimer % AttackCycleTime;

            // Telegraph phase
            if (wrappedAttackTimer < portalTelegraphTime + PortalSpawnTime)
            {
                npc.velocity *= 0.85f;
                npc.damage = 0;
                npc.dontTakeDamage = true;
                npc.Opacity = 1f;
            }

            FadeToAntimatterForm = Clamp(FadeToAntimatterForm + 0.05f, 0f, 1f);

            segmentFadeType = (int)BodySegmentFadeType.ApproachAheadSegmentOpacity;

            // Spawn entrance portal
            if (Main.netMode != NetmodeID.MultiplayerClient && wrappedAttackTimer == PortalSpawnTime)
            {
                ClearChargeGateFlags();

                Vector2 portalSpawnPosition = target.Center + Main.rand.NextVector2CircularEdge(600f, 600f);

                portalSpawnPosition = new Vector2(
                    Clamp(portalSpawnPosition.X, 200f, Main.maxTilesX * 16f - 200f),
                    Clamp(portalSpawnPosition.Y, 200f, Main.maxTilesY * 16f - 200f)
                );

                int portalIndex = Projectile.NewProjectile(
                    npc.GetSource_FromAI(),
                    portalSpawnPosition,
                    Vector2.Zero,
                    ModContent.ProjectileType<DoGChargeGate>(),
                    0,
                    0f,
                    Main.myPlayer,
                    0f,
                    portalTelegraphTime
                );

                if (Main.projectile.IndexInRange(portalIndex))
                {
                    Projectile portal = Main.projectile[portalIndex];
                    portal.ModProjectile<DoGChargeGate>().IsChargePortalIndex = true;
                    portal.netUpdate = true;
                }

                npc.netUpdate = true;
            }

            // Teleport + dash
            if (wrappedAttackTimer == (int)portalTelegraphTime + PortalSpawnTime)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int chargePortalIndex = GetChargePortalIndex();

                    if (Main.projectile.IndexInRange(chargePortalIndex))
                    {
                        Projectile portal = Main.projectile[chargePortalIndex];
                        DoGChargeGate gate = portal.ModProjectile<DoGChargeGate>();

                        npc.Center = portal.Center;

                        Vector2 destination = gate.Destination;
                        if (destination == Vector2.Zero)
                            destination = target.Center;

                        float chargeSpeed = 85f + npc.Distance(target.Center) * 0.0127f;

                        int segmentCount = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBody>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>()))
                            {
                                Main.npc[i].Center = npc.Center;
                                Main.npc[i].Opacity = Utils.GetLerpValue(15f, 0f, segmentCount, true);
                                Main.npc[i].netUpdate = true;
                                segmentCount++;
                            }
                        }

                        npc.velocity = CalamityUtils.SafeDirectionTo(npc, destination) * chargeSpeed;
                        npc.Opacity = 1f;
                        npc.dontTakeDamage = false;
                        npc.damage = npc.defDamage;

                        SpawnDesperationFireballs(npc, target);
                        SelfDamageAndLaserWall(npc, target, ref attackTimer, ref damageImmunityCountdown, ref portalTelegraphTime);

                        // mark that desperation is active (no special-case logic anymore)
                        DesperationHasEmergedFromFirstPortal = true;

                        npc.netUpdate = true;
                    }
                }

                SoundEngine.PlaySound(DevourerofGodsHead.AttackSound, target.Center);
            }

            // Fade into portals
            if (wrappedAttackTimer > portalTelegraphTime)
            {
                if (Main.projectile.IndexInRange(GeneralPortalIndex) &&
                    npc.Hitbox.Intersects(Main.projectile[GeneralPortalIndex].Hitbox))
                {
                    npc.Opacity = Clamp(npc.Opacity - 0.2f, 0f, 1f);
                }

                if (wrappedAttackTimer > portalTelegraphTime + PortalSpawnTime)
                    segmentFadeType = (int)BodySegmentFadeType.EnteringPortal;
            }

            // Timer progression
            DesperationChargeTimer++;
            attackTimer = DesperationChargeTimer;

            if (DesperationChargeTimer >= AttackCycleTime)
            {
                DesperationChargeTimer = 0;
                attackTimer = 0f;
                portalTelegraphTime = 0f;
                damageImmunityCountdown = 30f;
                npc.netUpdate = true;
            }
        }

        private static void SelfDamageAndLaserWall(NPC npc, Player target, ref float teleportTimer, ref float postTeleportTimer, ref float portalTelegraphTime)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float lifeRatio = npc.life / (float)npc.lifeMax;

            int damageDone = BossRushEvent.BossRushActive ? 18000 : 10000;

            npc.life -= damageDone;

            CombatText.NewText(npc.Hitbox, CombatText.DamagedHostile, damageDone, true);
            npc.HitEffect(0, damageDone);

            int bType = Main.rand.Next(0, (lifeRatio < 0.05f ? 6 : 2));

            float size = MathHelper.Max(300 * (lifeRatio * 10), 170f);
            //Main.NewText(size);

            Projectile.NewProjectile(
                npc.GetSource_FromAI(),
                target.Center + Main.rand.NextVector2CircularEdge(600f, 600f),
                Vector2.Zero,
                ModContent.ProjectileType<DoGLaserWalls>(),
                DevourerofGodsHead.LaserWallDamage,
                0f,
                Main.myPlayer,
                0.35f,
                size,
                bType
            );

            if (npc.life <= 1000 && DesperationHasEmergedFromFirstPortal)
            {
                DesperationCanDie = true;

                npc.life = 1;

                npc.Infernum().ExtraAI[PerformingSpecialAttackFlagIndex] = 0f;
                npc.Infernum().ExtraAI[SpecialAttackTimerIndex] = 0f;
                npc.Infernum().ExtraAI[SpecialAttackTypeIndex] = 0f;
                npc.Infernum().ExtraAI[ChargeGatePortalTelegraphTimeIndex] = 0f;
                npc.Infernum().ExtraAI[DeathAnimationTimerIndex] = 1f;

                foreach (NPC segment in Main.ActiveNPCs)
                {
                    if (segment.type == ModContent.NPCType<DevourerofGodsBody>() ||
                        segment.type == ModContent.NPCType<DevourerofGodsTail>())
                    {
                        segment.damage = 0;
                        segment.Opacity = 1f;
                        segment.dontTakeDamage = true;
                        segment.netUpdate = true;
                    }
                }

                PreDesperationProjectileCleanup();
                return;
            }

            SpawnExitChargeGate(npc, portalTelegraphTime);

            npc.netUpdate = true;
        }

        private static void ClearChargeGateFlags()
        {
            foreach (Projectile portal in Main.ActiveProjectiles)
            {
                if (portal.type != ModContent.ProjectileType<DoGChargeGate>())
                    continue;

                DoGChargeGate gate = portal.ModProjectile<DoGChargeGate>();

                gate.IsChargePortalIndex = false;
                gate.IsGeneralPortalIndex = false;
                portal.netUpdate = true;
            }
        }

        private static int GetChargePortalIndex()
        {
            foreach (Projectile portal in Main.ActiveProjectiles)
            {
                if (portal.type != ModContent.ProjectileType<DoGChargeGate>())
                    continue;

                if (portal.ModProjectile is DoGChargeGate gate && gate.IsChargePortalIndex)
                    return portal.whoAmI;
            }

            return -1;
        }

        private static void SpawnExitChargeGate(NPC npc, float portalTelegraphTime)
        {
            ClearChargeGateFlags();

            Vector2 portalSpawnPosition =
                npc.Center + npc.velocity.SafeNormalize(Vector2.UnitY) * 1900f;

            portalSpawnPosition = new Vector2(
                MathHelper.Clamp(portalSpawnPosition.X, 200f, Main.maxTilesX * 16f - 200f),
                MathHelper.Clamp(portalSpawnPosition.Y, 200f, Main.maxTilesY * 16f - 200f)
            );

            int portalIndex = Projectile.NewProjectile(
                npc.GetSource_FromAI(),
                portalSpawnPosition,
                Vector2.Zero,
                ModContent.ProjectileType<DoGChargeGate>(),
                0,
                0f,
                Main.myPlayer,
                0f,
                portalTelegraphTime
            );

            if (Main.projectile.IndexInRange(portalIndex))
            {
                Projectile portal = Main.projectile[portalIndex];
                DoGChargeGate gate = portal.ModProjectile<DoGChargeGate>();

                portal.localAI[0] = 1f;
                gate.IsGeneralPortalIndex = true;
                portal.netUpdate = true;
            }
        }

        private static void SpawnDesperationFireballs(NPC npc, Player target)
        {
            int fireballCount = 6;

            /*
            if (npc.life / (float)npc.lifeMax < 0.05f)
                fireballCount -= 2;
            */

            float flameBurstOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < fireballCount; i++)
            {
                Vector2 flameShootVelocity =
                    (MathHelper.TwoPi * i / fireballCount + flameBurstOffsetAngle)
                    .ToRotationVector2() * 13f;

                Projectile.NewProjectile(
                    npc.GetSource_FromAI(),
                    npc.Center + flameShootVelocity * 3f,
                    flameShootVelocity,
                    ModContent.ProjectileType<AcceleratingDoGBurst>(),
                    60,
                    0f,
                    Main.myPlayer
                );

                Vector2 secondVelocity =
                    flameShootVelocity.RotatedBy(MathHelper.Pi / fireballCount) * 0.5f;

                Projectile.NewProjectile(
                    npc.GetSource_FromAI(),
                    npc.Center + secondVelocity * 3f,
                    secondVelocity,
                    ModContent.ProjectileType<AcceleratingDoGBurst>(),
                    60,
                    0f,
                    Main.myPlayer
                );
            }
        }

        public static void ResetDesperationState()
        {
            DesperationHasTriggered = false;
            DesperationInitialized = false;
            DesperationCanDie = false;
            DesperationEnteringPortal = false;
            DesperationHasEmergedFromFirstPortal = false;
            HasDisplayedPhase2Text = false;

            DesperationEntryTimer = 0;
            DesperationChargeTimer = 0;
            DesperationEntryPortalIndex = -1;
            DesperationHeadEnteredPortal = false;
            DesperationPostEntryTimer = 0;

            ChargePortalIndex = -1;
            GeneralPortalIndex = -1;
        }

        public static void PreDesperationProjectileCleanup()
        {
            int[] typesToClear =
            [
                ModContent.ProjectileType<DoGChargeGate>(),
                ModContent.ProjectileType<AcceleratingDoGBurst>(),
                ModContent.ProjectileType<DoGLaserWalls>(),
                ModContent.ProjectileType<DoGLaserWallsBigBeam>(),
                ModContent.ProjectileType<DoGDeathInfernum>()
            ];

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (typesToClear.Contains(projectile.type))
                {
                    projectile.Kill();
                    projectile.netUpdate = true;
                }
            }
        }

        private static NPC GetDoGHeadFromAnySegment(NPC npc)
        {
            if (npc.realLife >= 0 && Main.npc.IndexInRange(npc.realLife))
                return Main.npc[npc.realLife];

            if (CalamityGlobalNPC.DoGHead >= 0 && Main.npc.IndexInRange(CalamityGlobalNPC.DoGHead))
                return Main.npc[CalamityGlobalNPC.DoGHead];

            return npc;
        }

        public static void StartDesperationFrom(NPC npc)
        {
            NPC head = GetDoGHeadFromAnySegment(npc);

            int targetLife = Math.Max(1, head.lifeMax / 10);

            head.life = targetLife;
            head.dontTakeDamage = true;
            head.damage = 0;

            head.Infernum().ExtraAI[DeathAnimationTimerIndex] = 0f;
            head.Infernum().ExtraAI[PerformingSpecialAttackFlagIndex] = 0f;
            head.Infernum().ExtraAI[SpecialAttackTimerIndex] = 0f;
            head.Infernum().ExtraAI[SpecialAttackTypeIndex] = 0f;
            head.Infernum().ExtraAI[ChargeGatePortalTelegraphTimeIndex] = 0f;

            DesperationEnteringPortal = true;
            DesperationInitialized = false;
            DesperationHasEmergedFromFirstPortal = false;
            DesperationCanDie = false;
            DesperationHasTriggered = true;

            DesperationEntryTimer = 0;
            DesperationChargeTimer = 0;
            DesperationEntryPortalIndex = -1;
            DesperationHeadEnteredPortal = false;
            DesperationPostEntryTimer = 0;

            PreDesperationProjectileCleanup();
            LumUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.DoGHeadDeath2", Color.Cyan);
            DialogueDisplaySystem.StartDialogueOnClient("Mods.CalamityMod.DevourerOfGods.Death", npc, 1, 60, false, new BossText());

            foreach (Player player in Main.ActivePlayers)
            {
                HatGirl.SayThingWhileOwnerIsAlive(player, "Mods.InfernalEclipseAPI.PetDialog.DoGDSPTip");
            }

            SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, head.Center);
            head.netUpdate = true;

            foreach (NPC segment in Main.ActiveNPCs)
            {
                if (segment.type == ModContent.NPCType<DevourerofGodsBody>() || segment.type == ModContent.NPCType<DevourerofGodsTail>())
                {
                    segment.life = targetLife;
                    segment.dontTakeDamage = true;
                    segment.damage = 0;
                    segment.netUpdate = true;
                }
            }
        }

        private static Vector2 SafeNormalize(Vector2 vector, Vector2 fallback)
        {
            if (vector.LengthSquared() <= 0.0001f)
                return fallback;

            vector.Normalize();
            return vector;
        }

        private static void HandleMountRestrictions()
        {
            if (InfernalConfig.Instance.CalamityBalanceChanges)
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (!player.active || player.dead)
                    continue;

                if (InfernalCrossmod.Clamity.Loaded)
                {
                    if (player.mount?.Type == InfernalCrossmod.Clamity.Mod.Find<ModMount>("PlagueChairMount").Type)
                        player.mount.Dismount(player);
                }

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    if (InfernalCrossmod.Thorium.Mod.TryFind("SuperAnvilMount", out ModMount supAnvil))
                    {
                        if (player.mount?.Type == supAnvil.Type)
                            player.mount.Dismount(player);
                    }
                }
            }
        }

        private static void InflictDspDebuff()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (player.active && !player.dead && player.GetModPlayer<InfernalPlayer>().teleportRespawnKilldown <= 0)
                {
                    player.AddBuff(ModContent.BuffType<StarboundHorrification>(), 2);
                    player.DoInfiniteFlightCheck(Color.Magenta);
                }
            }
        }
    }

    public class DoGDesperation : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<DevourerofGodsHead>() ||
                   entity.type == ModContent.NPCType<DevourerofGodsBody>() ||
                   entity.type == ModContent.NPCType<DevourerofGodsTail>();
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (npc.type == ModContent.NPCType<DevourerofGodsHead>()) return base.CanHitPlayer(npc, target, ref cooldownSlot);

            if (!DoGChanges.DesperationHasTriggered) return base.CanHitPlayer(npc, target, ref cooldownSlot);

            return false;
        }

        public override void PostAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<DevourerofGodsHead>()) return;

            if (!DoGChanges.DesperationHasTriggered) return;

            npc.immortal = true;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            if (!DoGChanges.DesperationHasTriggered)
                return;

            if (!DoGChanges.DesperationHasEmergedFromFirstPortal && !DoGChanges.DesperationEnteringPortal)
                return;

            if (DoGChanges.DesperationCanDie)
                return;

            // 99% DR → only 1% damage goes through
            if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
                modifiers.FinalDamage *= 0.01f;
            else
                modifiers.FinalDamage *= 0;

        }
    }

    internal sealed class HandleDoGLifeBasedHitTriggersDesperation : ModSystem
    {
        public static MethodInfo HandleDoGLifeBasedHitTriggersMethod = typeof(DoGPhase1HeadBehaviorOverride).GetMethod("HandleDoGLifeBasedHitTriggers", LumUtils.UniversalBindingFlags);

        public delegate bool Orig_HandleDoGLifeBasedHitTriggersMethod(NPC npc, double realDamage, ref NPC.HitModifiers modifiers);

        public delegate bool Hook_HandleDoGLifeBasedHitTriggersMethod(Orig_HandleDoGLifeBasedHitTriggersMethod orig, NPC npc, double realDamage, ref NPC.HitModifiers modifiers);

        private static Hook RagnarokDesperation_Detour_Hook;

        public override void OnModLoad()
        {
            if (HandleDoGLifeBasedHitTriggersMethod != null)
            {
                RagnarokDesperation_Detour_Hook = new Hook(
                    HandleDoGLifeBasedHitTriggersMethod,
                    (Hook_HandleDoGLifeBasedHitTriggersMethod)HandleDoGLifeBasedHitTriggers_Detour
                );

                RagnarokDesperation_Detour_Hook.Apply();
            }
            else
            {
                InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: " + this + " returned null on getting MethodInfo");
            }
        }

        public override void Unload()
        {
            RagnarokDesperation_Detour_Hook?.Dispose();
            RagnarokDesperation_Detour_Hook = null;
        }

        private static bool HandleDoGLifeBasedHitTriggers_Detour(
            Orig_HandleDoGLifeBasedHitTriggersMethod orig,
            NPC npc,
            double realDamage,
            ref NPC.HitModifiers modifiers
        )
        {
            int life = npc.realLife >= 0 ? Main.npc[npc.realLife].life : npc.life;

            // Preserve phase 2 transition behavior.
            if (life - realDamage <= npc.lifeMax * Phase2LifeRatio &&
                !InPhase2 &&
                CurrentPhase2TransitionState == Phase2TransitionState.NotEnteringPhase2)
            {
                modifiers.FinalDamage.Base *= 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.dontTakeDamage = true;
                    CurrentPhase2TransitionState = Phase2TransitionState.NeedsToSummonPortal;
                }
                else
                {
                    PacketManager.SendPacket<SyncDoGPacket>(npc.whoAmI, realDamage);
                }

                return false;
            }

            // Replace Infernum death trigger with your desperation phase.
            if (life - realDamage <= 1000 && InPhase2 && !DoGChanges.DesperationHasTriggered && InfernalWorld.RagnarokModeEnabled)
            {
                modifiers.FinalDamage.Base *= 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    DoGChanges.StartDesperationFrom(npc);
                else
                    PacketManager.SendPacket<SyncDoGPacket>(npc.whoAmI, realDamage);

                return false;
            }

            // After desperation, allow Infernum's normal death trigger.
            if (life - realDamage <= 1000 && InPhase2 && DoGChanges.DesperationHasTriggered)
            {
                if (DoGChanges.DesperationCanDie && DoGChanges.DesperationHasEmergedFromFirstPortal)
                    return orig(npc, realDamage, ref modifiers);

                modifiers.FinalDamage.Base *= 0;

                npc.life = Math.Max(npc.life, 1);
                npc.Infernum().ExtraAI[DeathAnimationTimerIndex] = 0f;
                npc.netUpdate = true;

                return false;
            }

            return orig(npc, realDamage, ref modifiers);
        }
    }

    internal sealed class UpdateDoGPhaseServerDesperation : ModSystem
    {
        public static MethodInfo UpdateDoGPhaseServerMethod = typeof(DoGPhase1HeadBehaviorOverride).GetMethod("UpdateDoGPhaseServer", LumUtils.UniversalBindingFlags);

        public delegate void Orig_UpdateDoGPhaseServerMethod(int npcIndex, double damage);

        public delegate void Hook_UpdateDoGPhaseServerMethod(Orig_UpdateDoGPhaseServerMethod orig, int npcIndex, double damage);

        private static Hook RagnarokDesperation_UpdateDoGPhaseServer_Hook;

        public override void OnModLoad()
        {
            if (UpdateDoGPhaseServerMethod != null)
            {
                RagnarokDesperation_UpdateDoGPhaseServer_Hook = new Hook(
                    UpdateDoGPhaseServerMethod,
                    (Hook_UpdateDoGPhaseServerMethod)UpdateDoGPhaseServer_Detour
                );

                RagnarokDesperation_UpdateDoGPhaseServer_Hook.Apply();
            }
            else
            {
                InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: " + this + " returned null on getting MethodInfo");
            }
        }

        public override void Unload()
        {
            RagnarokDesperation_UpdateDoGPhaseServer_Hook?.Dispose();
            RagnarokDesperation_UpdateDoGPhaseServer_Hook = null;
        }

        private static void UpdateDoGPhaseServer_Detour(
            Orig_UpdateDoGPhaseServerMethod orig,
            int npcIndex,
            double damage
        )
        {
            NPC npc = Main.npc[npcIndex];
            int life = npc.realLife >= 0 ? Main.npc[npc.realLife].life : npc.life;

            // Preserve Infernum phase 2 transition.
            if (life - damage <= npc.lifeMax * Phase2LifeRatio &&
                !InPhase2 &&
                CurrentPhase2TransitionState == Phase2TransitionState.NotEnteringPhase2)
            {
                npc.dontTakeDamage = true;
                CurrentPhase2TransitionState = Phase2TransitionState.NeedsToSummonPortal;
                npc.netUpdate = true;
                return;
            }

            // Replace first death trigger with desperation.
            if (life - damage <= 1000 && InPhase2 && !DoGChanges.DesperationHasTriggered && InfernalWorld.RagnarokModeEnabled)
            {
                DoGChanges.StartDesperationFrom(npc);
                return;
            }

            if (life - damage <= 1000 && InPhase2 && DoGChanges.DesperationHasTriggered)
            {
                if (DoGChanges.DesperationCanDie && DoGChanges.DesperationHasEmergedFromFirstPortal)
                {
                    orig(npcIndex, damage);
                    return;
                }

                npc.life = Math.Max(npc.life, 1);
                npc.Infernum().ExtraAI[DeathAnimationTimerIndex] = 0f;
                npc.netUpdate = true;

                return;
            }

            orig(npcIndex, damage);
        }
    }

    internal sealed class DoGPhase2AISuppressionHook : ModSystem
    {
        MethodInfo method = typeof(DoGPhase2HeadBehaviorOverride).GetMethod(nameof(Phase2AI), LumUtils.UniversalBindingFlags);

        private static Hook Phase2AIHook;

        private delegate bool Orig_Phase2AI(NPC npc, ref float phaseCycleTimer, ref float passiveAttackDelay, ref float segmentFadeType, ref float universalFightTimer);

        private delegate bool Hook_Phase2AI( Orig_Phase2AI orig, NPC npc, ref float phaseCycleTimer, ref float passiveAttackDelay, ref float segmentFadeType, ref float universalFightTimer);

        public override void OnModLoad()
        {
            if (method != null)
            {
                Phase2AIHook = new Hook(method, (Hook_Phase2AI)Phase2AI_Detour);
                Phase2AIHook.Apply();
            }
            else
                InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: Failed to find DoG Phase2AI.");
        }

        public override void Unload()
        {
            Phase2AIHook?.Dispose();
            Phase2AIHook = null;
        }

        private static bool Phase2AI_Detour(Orig_Phase2AI orig, NPC npc,ref float phaseCycleTimer, ref float passiveAttackDelay, ref float segmentFadeType, ref float universalFightTimer)
        {
            if (DoGChanges.DesperationCanDie && DoGChanges.DesperationHasEmergedFromFirstPortal)
            {
                return orig(npc, ref phaseCycleTimer, ref passiveAttackDelay, ref segmentFadeType, ref universalFightTimer);
            }

            if (DoGChanges.DesperationHasTriggered && InfernalWorld.RagnarokModeEnabled)
            {
                npc.GetGlobalNPC<DoGChanges>().RunDesperationAI(npc, Main.player[npc.target], ref npc.Infernum().ExtraAI[PerformingSpecialAttackFlagIndex], ref npc.Infernum().ExtraAI[SpecialAttackTimerIndex], ref npc.Infernum().ExtraAI[BodySegmentFadeTypeIndex], ref npc.Infernum().ExtraAI[DamageImmunityCountdownIndex]);
                return false;
            }

            return orig(npc, ref phaseCycleTimer, ref passiveAttackDelay, ref segmentFadeType, ref universalFightTimer);
        }
    }
}
