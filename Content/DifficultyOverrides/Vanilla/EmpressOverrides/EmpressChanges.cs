using CalamityMod;
using CalamityMod.World;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.EmpressOfLight;
using InfernumMode.Core.GlobalInstances.Systems;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria.Audio;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.Yharon.YharonBehaviorOverride;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Vanilla.EmpressOverrides
{
    public class EmpressChanges : GlobalNPC
    {
        private Hook lanceWallBarrageHook;
        private delegate void LanceWallBarrageDelegate(NPC npc, Player target, ref float attackTimer, ref float leftArmFrame, ref float rightArmFrame);

        public override bool InstancePerEntity => true;
        private float startRotation;
        public Vector2 targetPos;

        // Synced through Infernum's ExtraAI array.
        private const int ForcedUltimateRainbowIndex = 40;
        private const int HasDoneUltimateRainbowIndex = 41;
        private const int CanDoLanceCircleBarrageIndex = 42;
        private const int HadDoneSpawnEffectsIndex = 43;
        private const int ShouldSetDaytimeAndShaderIndex = 44;
        private const int SpawnEffectsTimerIndex = 45;

        public override void Load()
        {
            MethodInfo lanceWallBarrageMethod = typeof(EmpressOfLightBehaviorOverride).GetMethod("DoBehavior_LanceWallBarrage", LumUtils.UniversalBindingFlags);

            if (lanceWallBarrageMethod is null)
                throw new MissingMethodException(typeof(EmpressOfLightBehaviorOverride).FullName, "DoBehavior_LanceWallBarrage");

            lanceWallBarrageHook = new Hook(lanceWallBarrageMethod, DoBehavior_LanceWallBarrage_Hook);
        }

        public override void Unload()
        {
            lanceWallBarrageHook?.Dispose();
            lanceWallBarrageHook = null;
        }

        private void DoBehavior_LanceWallBarrage_Hook(LanceWallBarrageDelegate orig, NPC npc, Player target, ref float attackTimer, ref float leftArmFrame, ref float rightArmFrame)
        {
            // Use Infernum's original attack outside Ragnarok Mode.
            if (!InfernalWorld.RagnarokModeEnabled)
            {
                orig(npc, target, ref attackTimer, ref leftArmFrame, ref rightArmFrame);

                return;
            }

            ref float canDoLanceCircleBarrage = ref npc.Infernum().ExtraAI[CanDoLanceCircleBarrageIndex];

            // Initialize this flag if necessary.
            if (canDoLanceCircleBarrage != 1f && canDoLanceCircleBarrage != -1f)
            {
                canDoLanceCircleBarrage = 1f;
            }

            // Completely replace Infernum's implementation.
            DoBehavior_LanceWallBarrage_Ragnarok(npc, target, ref attackTimer, ref leftArmFrame, ref rightArmFrame, ref canDoLanceCircleBarrage);
        }

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.HallowBoss;

        public override bool PreAI(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return base.PreAI(npc);

            CalamityWorld.StopRain();

            Main.dayTime = true;

            ref float attackType = ref npc.ai[0];
            ref float hasDoneSpawnEffects = ref npc.Infernum().ExtraAI[HadDoneSpawnEffectsIndex];
            ref float shouldSetDaytimeAndShader = ref npc.Infernum().ExtraAI[ShouldSetDaytimeAndShaderIndex];
            ref float spawnEffectsTimer = ref npc.Infernum().ExtraAI[SpawnEffectsTimerIndex];
            ref float forcedUltimateRainbow = ref npc.Infernum().ExtraAI[ForcedUltimateRainbowIndex];
            ref float hasDoneUltimateRainbow = ref npc.Infernum().ExtraAI[HasDoneUltimateRainbowIndex];
            ref float canDoLanceCircleBarrage = ref npc.Infernum().ExtraAI[CanDoLanceCircleBarrageIndex];

            if (hasDoneSpawnEffects != 1f)
            {
                DoBehavior_SpawnEffects(npc, Main.player[npc.target], ref spawnEffectsTimer, ref hasDoneSpawnEffects, ref shouldSetDaytimeAndShader);
            }

            if (shouldSetDaytimeAndShader == 1f)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    player.ZoneHallow = true;
                }

                Main.time = Lerp((float)Main.time, (float)Main.dayLength * 0.5f, 0.01f);
            }

            var currentAttack = (EmpressOfLightBehaviorOverride.EmpressOfLightAttackType)(int)attackType;

            if (currentAttack == EmpressOfLightBehaviorOverride.EmpressOfLightAttackType.UltimateRainbow)
                hasDoneUltimateRainbow = 1f;

            return base.PreAI(npc);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            var currentAttack = (EmpressOfLightBehaviorOverride.EmpressOfLightAttackType)(int)npc.ai[0];

            if (currentAttack == EmpressOfLightBehaviorOverride.EmpressOfLightAttackType.LanceWallBarrage)
                modifiers.FinalDamage *= 0.5f;
        }

        public override bool CheckDead(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return base.CheckDead(npc);

            ref float attackType = ref npc.ai[0];
            ref float forcedUltimateRainbow = ref npc.Infernum().ExtraAI[ForcedUltimateRainbowIndex];
            ref float hasDoneUltimateRainbow = ref npc.Infernum().ExtraAI[HasDoneUltimateRainbowIndex];

            var currentAttack = (EmpressOfLightBehaviorOverride.EmpressOfLightAttackType)(int)attackType;

            // Let normal death happen if she's already done the attack, or if she's already in the ending sequence.
            if (hasDoneUltimateRainbow == 1f && (currentAttack == EmpressOfLightBehaviorOverride.EmpressOfLightAttackType.UltimateRainbow || currentAttack == EmpressOfLightBehaviorOverride.EmpressOfLightAttackType.DeathAnimation))
            {
                return base.CheckDead(npc);
            }

            forcedUltimateRainbow = 1f;
            npc.life = 1;
            npc.active = true;

            attackType = (int)EmpressOfLightBehaviorOverride.EmpressOfLightAttackType.UltimateRainbow;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;

            for (int i = 0; i < npc.localAI.Length; i++)
                npc.localAI[i] = 0f;

            // Keep the synced state flags, clear the rest.
            for (int i = 0; i < npc.Infernum().ExtraAI.Length; i++)
            {
                if (i == ForcedUltimateRainbowIndex || i == HasDoneUltimateRainbowIndex)
                    continue;

                npc.Infernum().ExtraAI[i] = 0f;
            }

            EmpressOfLightBehaviorOverride.ClearAwayEntities();
            npc.netUpdate = true;
            return false;
        }

        public static void DoBehavior_SpawnEffects(NPC npc, Player target, ref float spawnEffectsTimer, ref float hasDoneSpawnEffects, ref float shouldSetDaytimeAndShader)
        {
            const int cameraPanDelay = 4;
            const int cameraPanTime = 120;
            const int cameraTransitionTime = 5;

            spawnEffectsTimer++;

            if (spawnEffectsTimer == 2)
            {
                SoundEngine.PlaySound(in SoundID.Item122, npc.Center);
                SoundEngine.PlaySound(in SoundID.Item160, npc.Center);
                Utilities.CreateShockwave(npc.Center, 2, 8, 75f, playSound: false);
                if (Main.netMode != 1)
                {
                    Utilities.NewProjectileBetter(npc.Center + Vector2.UnitY * 8f, Vector2.Zero, ModContent.ProjectileType<ShimmeringLightWave>(), 0, 0f);
                }
            }

            // Smoothly pan toward Empress, remain focused, then smoothly
            // return to the player.
            if (spawnEffectsTimer >= cameraPanDelay)
            {
                float cameraPanInterpolant = Utils.GetLerpValue(cameraPanDelay, cameraPanDelay + cameraTransitionTime, spawnEffectsTimer, true) * Utils.GetLerpValue(cameraPanTime, cameraPanTime - cameraTransitionTime, spawnEffectsTimer, true);

                target.Infernum_Camera().ScreenFocusInterpolant = cameraPanInterpolant;
                target.Infernum_Camera().ScreenFocusPosition = npc.Center;
            }

            shouldSetDaytimeAndShader = 1f;

            // The pan has completely returned to the player.
            if (spawnEffectsTimer >= cameraPanTime)
            {
                hasDoneSpawnEffects = 1f;
                spawnEffectsTimer = 0f;
                npc.netUpdate = true;
            }
            else
            {
                npc.dontTakeDamage = true;
            }
        }

        private void DoBehavior_LanceWallBarrage_Ragnarok(NPC npc, Player target, ref float attackTimer, ref float leftArmFrame, ref float rightArmFrame, ref float canDoLanceCircleBarrage)
        {
            int hoverRedirectTime = 20;
            int startingWallShootCountdown = 57;
            int endingWallShootCountdown = 43;
            int horizontalShootTime = 480;
            int horizontalLanceTransitionDelay = 20;
            int verticalLanceCount = 50;
            int verticalLanceTransitionDelay = 80;
            float horizontalLanceSpacing = 140f;
            float verticalLanceArea = 270f;

            if (EmpressOfLightBehaviorOverride.InPhase3(npc))
            {
                endingWallShootCountdown -= 5;
                horizontalShootTime += 60;
            }
            if (EmpressOfLightBehaviorOverride.InPhase4(npc))
            {
                horizontalLanceSpacing -= 12f;
            }

            ref float horizontalWallDirection = ref npc.Infernum().ExtraAI[0];
            ref float wallShootCountdown = ref npc.Infernum().ExtraAI[1];
            ref float wallShootDelay = ref npc.Infernum().ExtraAI[2];
            ref float wallShootCounter = ref npc.Infernum().ExtraAI[3];
            ref float attackSubstate = ref npc.Infernum().ExtraAI[4];

            bool goingAgainstWalls = Math.Abs(target.velocity.X) >= 6f && Math.Sign(target.velocity.X) != horizontalWallDirection;
            float wallHorizontalOffset = goingAgainstWalls ? 1254f : 876f;

            // Have the arm pointed towards the player aim downward, while the other hand points upward.
            leftArmFrame = 4f;
            rightArmFrame = 2f;
            if (target.Center.X < npc.Center.X)
                Utils.Swap(ref leftArmFrame, ref rightArmFrame);

            switch ((int)attackSubstate)
            {
                case 0:
                    // Decide the wall direction on the first frame.
                    // If the player has a lot of momentum in a certain direction, it will be chosen in such a way that the player can simply retain their current direction, so as to not require sudden, jarring turns.
                    // Otherwise, it'll simply be randomized.
                    if (attackTimer == 1f)
                    {
                        horizontalWallDirection = Main.rand.NextBool().ToDirectionInt();
                        if (Math.Abs(target.velocity.X) >= 10f)
                            horizontalWallDirection = Math.Sign(target.velocity.X);

                        wallShootDelay = startingWallShootCountdown;
                        wallShootCountdown = wallShootDelay;
                        npc.netUpdate = true;
                    }

                    // Redirect above the target.
                    Vector2 hoverDestination = target.Center + new Vector2(horizontalWallDirection * -270f, -196f);
                    npc.velocity *= 0.8f;
                    npc.Center = Vector2.SmoothStep(npc.Center, hoverDestination, 0.16f);
                    if (attackTimer < hoverRedirectTime)
                    {
                        EmpressOfLightBehaviorOverride.ClearAwayEntities();
                        break;
                    }

                    // Shoot lance walls.
                    wallShootCountdown--;

                    // Prepare for the next wall and fire.
                    if (wallShootCountdown <= 0f && attackTimer < hoverRedirectTime + horizontalShootTime)
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            break;

                        wallShootDelay = Clamp(wallShootDelay - 8f, endingWallShootCountdown, startingWallShootCountdown);
                        wallShootCountdown = wallShootDelay;
                        if (goingAgainstWalls)
                            wallShootCountdown *= 0.6f;

                        float lanceDirection = (Vector2.UnitX * horizontalWallDirection).ToRotation();
                        for (int i = -16; i < 16; i++)
                        {
                            float lanceHue = (i + 16f) / 32f * 4f % 1f;
                            Vector2 lanceSpawnPosition = target.Center + new Vector2(-horizontalWallDirection * wallHorizontalOffset, horizontalLanceSpacing * i);
                            if (wallShootCounter % 2f == 1f)
                            {
                                lanceSpawnPosition += Vector2.UnitY * horizontalLanceSpacing * 0.62f;
                                lanceHue = 1f - lanceHue;
                            }

                            ProjectileSpawnManagementSystem.PrepareProjectileForSpawning(lance =>
                            {
                                lance.MaxUpdates = 1;
                                lance.ModProjectile<EtherealLance>().Time = 10;
                                lance.ModProjectile<EtherealLance>().PlaySoundOnFiring = i == 0;
                                lance.ModProjectile<EtherealLance>().FlySpeedFactor = 0.55f;
                                lance.ModProjectile<EtherealLance>().SoundPitch = (npc.ai[1] - hoverRedirectTime) / horizontalShootTime * 0.35f;
                            });
                            int index = Utilities.NewProjectileBetter(lanceSpawnPosition, Vector2.Zero, ModContent.ProjectileType<EtherealLance>(), EmpressOfLightBehaviorOverride.LanceDamage, 0f, -1, lanceDirection, lanceHue);
                            Main.projectile[index].ai[2] = 40f;
                            Main.projectile[index].netUpdate = true;
                        }

                        wallShootCounter++;
                        npc.netUpdate = true;
                    }

                    if (attackTimer >= hoverRedirectTime + horizontalShootTime + horizontalLanceTransitionDelay)
                    {
                        Utilities.DeleteAllProjectiles(false, ModContent.ProjectileType<EtherealLance>());

                        attackTimer = 0f;
                        attackSubstate = 1f;
                        npc.netUpdate = true;
                    }
                    break;

                // Suddenly summon a bunch of lances above the target.
                case 1:
                    if (attackTimer == 1f)
                    {
                        EmpressOfLightBehaviorOverride.TeleportTo(npc, target.Center - Vector2.UnitY * 300f);
                        for (int i = 0; i < verticalLanceCount; i++)
                        {
                            float verticalLanceOffset = Lerp(-verticalLanceArea, verticalLanceArea, i / 35f);
                            float lanceHue = i / (float)verticalLanceCount * 2f % 1f;
                            //Vector2 lanceSpawnPosition = target.Center - (Vector2.UnitX * target.direction) * 950f + Utils.RotatedBy(Vector2.UnitX * target.direction, 1.5707963705062866, new Vector2()) * verticalLanceOffset + target.velocity * 18f;
                            Vector2 lanceSpawnPosition = target.Center + new Vector2(target.direction * 950f + target.velocity.X * 40f, verticalLanceOffset + target.velocity.Y * 18f);

                            ProjectileSpawnManagementSystem.PrepareProjectileForSpawning(lance =>
                            {
                                lance.ModProjectile<EtherealLance>().Time = -70;
                            });
                            int index = Utilities.NewProjectileBetter(lanceSpawnPosition, Vector2.Zero, ModContent.ProjectileType<EtherealLance>(), EmpressOfLightBehaviorOverride.LanceDamage, 0f, -1, (Vector2.UnitX * -target.direction).ToRotation(), lanceHue);
                            Main.projectile[index].ai[2] = 40f;
                            Main.projectile[index].netUpdate = true;
                        }
                    }

                    if (attackTimer >= verticalLanceTransitionDelay)
                    {
                        attackTimer = 0f;
                        attackSubstate = 2f;
                        npc.netUpdate = true;
                    }

                    break;

                case 2:
                    {
                        if (canDoLanceCircleBarrage == -1f)
                        {
                            canDoLanceCircleBarrage = 1f;
                            EmpressOfLightBehaviorOverride.SelectNextAttack(npc);
                            break;
                        }
                        else if (canDoLanceCircleBarrage == 1f)
                        {
                            // Redirect above the target.
                            Vector2 hoverDestination2 = target.Center + new Vector2(horizontalWallDirection * -270f, -196f);
                            npc.velocity *= 0.8f;
                            npc.Center = Vector2.SmoothStep(npc.Center, hoverDestination2, 0.16f);
                            if (attackTimer < hoverRedirectTime)
                            {
                                break;
                            }

                            DoBehavior_LanceCircleBarrage_Ragnarok(npc, target, ref attackTimer, ref canDoLanceCircleBarrage);
                        }
                        break;
                    }
            }
        }

        private void DoBehavior_LanceCircleBarrage_Ragnarok(NPC npc, Player target, ref float attackTimer, ref float canDoLanceCircleBarrage)
        {
            int spinTime = 210;
            float spins = EmpressOfLightBehaviorOverride.InPhase4(npc) ? 2.5f : EmpressOfLightBehaviorOverride.InPhase3(npc) ? 2f : 1.5f;
            int startDelay = 60;
            if (attackTimer == 0)
            {
                EmpressOfLightBehaviorOverride.TeleportTo(npc, target.Center - Vector2.UnitY * 300f);
                SoundEngine.PlaySound(SoundID.Item161, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center);
            }
            else if (attackTimer == startDelay)
            {
                targetPos = target.Center;
                startRotation = npc.HasValidTarget ? Main.player[npc.target].velocity.ToRotation() : 0;
            }
            else if (attackTimer < startDelay) // for ritual visual
            {
                targetPos = target.Center;
            }

            const float radius = 600;
            if (Main.player[npc.target].Distance(targetPos) > radius)
                targetPos = Main.player[npc.target].Center + Utilities.SafeDirectionTo(Main.player[npc.target], targetPos) * radius;

            if (attackTimer % 90 == 30) //rapid fire sound effect
                SoundEngine.PlaySound(SoundID.Item164, Main.player[npc.target].Center);

            if (attackTimer > startDelay && attackTimer <= spinTime * spins + startDelay && attackTimer % 2 == 0)
            {
                int max = 3;
                for (int i = 0; i < max; i++)
                {
                    int direction = -1;
                    float increment = TwoPi / spinTime * attackTimer * direction;
                    Vector2 offsetDirection = Vector2.UnitX.RotatedBy(startRotation + increment + TwoPi / max * i);
                    Vector2 spawnPos = targetPos + radius * offsetDirection;
                    Vector2 vel = Vector2.Normalize(targetPos - spawnPos);
                    float ai1 = (float)(attackTimer - startDelay) / spinTime % 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 appearVel = -vel;
                        appearVel *= 7.5f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - appearVel * 60, appearVel, ModContent.ProjectileType<EtherealLance>(), EmpressOfLightBehaviorOverride.ShouldBeEnraged ? 40 : 30, 0f, Main.myPlayer, vel.ToRotation(), ai1);
                    }
                }
            }

            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(false);
                if (!npc.HasValidTarget)
                    attackTimer += 9000;
            }

            if (attackTimer > spinTime * spins + startDelay)
            {
                canDoLanceCircleBarrage = -1f;
            }
        }
    }
}
