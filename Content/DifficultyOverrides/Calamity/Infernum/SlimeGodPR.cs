using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Projectiles.Enemy;
using InfernumMode;
using InfernumMode.Assets.Effects;
using InfernumMode.Assets.ExtraTextures;
using InfernumMode.Common.BaseEntities;
using InfernumMode.Common.Graphics.Primitives;
using InfernumMode.Content.BehaviorOverrides.BossAIs.BrimstoneElemental;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod.SlimeGodComboAttackManager;
using static SlimeGodPR.RagSlimeGod;

namespace SlimeGodPR
{

    public class ProjUtils
    {
        public static void FloatAbovePlayer(Vector2 lookAtPosition, NPC NPC, float speed, float inertia, Vector2 destination, float radians, float fault, bool slowDown, bool lerpRotation, float lerpRate)
        {
            if (slowDown && NPC.WithinRange(destination, fault))
            {
                NPC.velocity *= 0.85f;
                return;
            }

            Vector2 toPlayer = lookAtPosition - NPC.Center;
            Vector2 toAbovePlayer = destination - NPC.Center;
            Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

            Vector2 moveTo = toAbovePlayerNormalized * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
            if (lookAtPosition != Vector2.Zero)
            {
                if (lerpRotation)
                {
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, toPlayer.ToRotation() + MathHelper.ToRadians(radians), lerpRate);
                }
                else
                {
                    NPC.rotation = toPlayer.ToRotation() + MathHelper.ToRadians(radians);
                }

            }

        }

        public static void FloatAbovePlayer(Vector2 lookAtPosition, Projectile NPC, float speed, float inertia, Vector2 destination, float radians, float fault, bool slowDown, bool lerpRotation, float lerpRate)
        {
            if (NPC.WithinRange(destination, fault))
            {
                if (slowDown)
                {
                    NPC.velocity *= 0.85f;
                }

                return;
            }

            Vector2 toPlayer = lookAtPosition - NPC.Center;
            Vector2 toAbovePlayer = destination - NPC.Center;
            Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

            Vector2 moveTo = toAbovePlayerNormalized * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
            if (lookAtPosition != Vector2.Zero)
            {
                if (lerpRotation)
                {
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, toPlayer.ToRotation() + MathHelper.ToRadians(radians), lerpRate);
                }
                else
                {
                    NPC.rotation = toPlayer.ToRotation() + MathHelper.ToRadians(radians);
                }

            }

        }
        public static void ShootProjectile(IEntitySource source, Vector2 targetPosition, float speed, int type, bool hasTileCollide, int count, float startAngle, float angleDecrement, Vector2 startPosition, float radius, int damage, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, int lifeTime = 0)
        {

            damage /= 2;

            if (Main.expertMode)
            {
                damage /= Main.masterMode ? 3 : 2;
            }

            Vector2 distance = targetPosition - startPosition;
            Vector2 distanceNormalized = distance.SafeNormalize(Vector2.UnitX);
            float angle = startAngle;
            for (int i = 0; i < count; i++)
            {
                int projectile = Projectile.NewProjectile(source, startPosition, (distanceNormalized.RotatedBy(MathHelper.ToRadians(angle)) * speed).RotatedByRandom(MathHelper.ToRadians(radius)), type, damage, 1, -1, ai0, ai1, ai2);

                Main.projectile[projectile].tileCollide = hasTileCollide;
                Main.projectile[projectile].friendly = false;
                Main.projectile[projectile].hostile = true;

                if (lifeTime != 0)
                {
                    Main.projectile[projectile].timeLeft = lifeTime;
                }

                angle -= angleDecrement;
            }
        }
    }

    public class RagSlimeGod: GlobalNPC
    {
        public static int GlobDamage => 90;
        public static int DeathrayDamage => 150;
        public static int BringInSlimeDamage => 120;
        #region New Attacks
        public static void DoBehaviour_Attach(NPC npc, ref float[] ExtraAi)
        {
            ref float phase = ref ExtraAi[0];
            ref float attackTimer = ref ExtraAi[1];
            ref float attackCount = ref ExtraAi[2];
            ref float attackTypeCounter = ref ExtraAi[3];
            ref float attackType = ref ExtraAi[4];
            ref float lockedPosX = ref ExtraAi[5];
            ref float lockedPosY = ref ExtraAi[8];

            float speedModifier = 10;
            float inertia = 5;

            Player target = Main.player[npc.target];

            Vector2 destination = target.Center;
            Vector2 distance = npc.Center - destination;
            float speed = distance.Length() / speedModifier;

            int projectileDelay = 9;
            int totalAttackTime = 900;
            float projectileOffset = 900;
            float projectileSpeed = 3;
            int visualArenaSize = 700;
            int arenaSize = 1100;

            if (attackTimer == 0)
            {
                lockedPosX = target.Center.X;
                lockedPosY = target.Center.Y;
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<ArenaSlime>(), false, 1, 0, 0, npc.Center, 0, 0, lockedPosX, lockedPosY, visualArenaSize, totalAttackTime);
            }

            if (!target.WithinRange(new Vector2(lockedPosX, lockedPosY), arenaSize))
            {
                target.velocity = Vector2.Zero;
            }

            ProjUtils.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

            attackTimer++;



            if (attackTimer % projectileDelay == 0)
            {
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center, projectileSpeed, ModContent.ProjectileType<SlowedGelBlob>(), false, 1, 0, 0, npc.Center + (Vector2.One.RotatedByRandom(360) * projectileOffset), 0, GlobDamage, npc.whoAmI);
            }

            if (attackTimer >= totalAttackTime)
            {
                SelectNextAttack(npc);
            }
        }
        public static void DoBehaviour_Explosions(NPC npc, ref float[] ExtraAi)
        {
            ref float phase = ref ExtraAi[0];
            ref float attackTimer = ref ExtraAi[1];
            ref float attackCount = ref ExtraAi[2];
            ref float attackTypeCounter = ref ExtraAi[3];
            ref float attackType = ref ExtraAi[4];
            ref float lockedPosX = ref ExtraAi[5];
            ref float lockedPosY = ref ExtraAi[8];
            ref float slowDownTimer = ref ExtraAi[9];
            ref float explodeTimer = ref ExtraAi[10];

            Player target = Main.player[npc.target];

            int slowDownTime = 60;
            int explodeTime = 120;
            int totalBursts = 25;
            int shotsPerWeird = 4;
            int timeBetweenBursts = 40;
            float projectileAngle = 360;
            int projectileCount = 20;
            float projectileSpeed = 2;

            float visualArenaSize = 700;
            float arenaSize = 1100;

            int explosionDelay = 10;


            if (lockedPosX == 0)
            {
                lockedPosX = target.Center.X;
                lockedPosY = target.Center.Y;
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<ArenaSlime>(), false, 1, 0, 0, npc.Center, 0, 0, lockedPosX, lockedPosY, visualArenaSize, slowDownTime + explodeTime + (timeBetweenBursts * totalBursts));
            }

            if (!target.WithinRange(new Vector2(lockedPosX, lockedPosY), arenaSize))
            {
                target.velocity = Vector2.Zero;
            }

            if (slowDownTimer < slowDownTime)
            {
                slowDownTimer++;
                npc.velocity *= 0.9f;
                return;
            }
            explodeTimer++;

            npc.position = new Vector2(lockedPosX + Main.rand.Next(-10, 10), lockedPosY + Main.rand.Next(-10, 10));
            if (explodeTimer % explosionDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, npc.position);
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, npc.position);

                for (int i = 0; i < 100; i++)
                {

                    Color dustColor;
                    if (Main.rand.NextBool())
                    {
                        dustColor = Color.Crimson;
                        dustColor.A = 150;
                    }
                    else
                    {
                        dustColor = Color.Lavender;
                        dustColor.A = 150;
                    }

                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.TintableDust, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), npc.alpha, dustColor, 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            if (explodeTimer < explodeTime)
            {
                return;
            }


            attackTimer++;
            if (attackTimer % timeBetweenBursts == 0)
            {
                attackCount++;
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), target.Center, projectileSpeed, attackCount % shotsPerWeird == 0 ? ModContent.ProjectileType<WeirdGel>() : ModContent.ProjectileType<AcceleratingGel>(), false, projectileCount, Main.rand.Next(-360, 361), projectileAngle / projectileCount, npc.Center, 0, GlobDamage);

                if (attackCount >= totalBursts)
                {
                    SelectNextAttack(npc);
                }
            }
        }
        public static void DoBehaviour_SideToSide(NPC npc, ref float[] ExtraAi)
        {
            ref float phase = ref ExtraAi[0];
            ref float attackTimer = ref ExtraAi[1];
            ref float attackCount = ref ExtraAi[2];
            ref float attackTypeCounter = ref ExtraAi[3];
            ref float attackType = ref ExtraAi[4];
            ref float turn = ref ExtraAi[5];
            ref float acceleration = ref ExtraAi[8];
            ref float attackCountCounter = ref ExtraAi[9];
            ref float stopTimer = ref ExtraAi[10];

            float accelerationMax = 40;
            float maxDistance = 900;

            int projectileTimer = 15;
            float projectileSpeed = 10f;
            int projectileCount = 13;
            float projectileAngle = 360;
            int burstWait = 80;
            int burstCount = 3;
            int attackStartDelay = 80;
            int totalBurstCount = 5;
            float speedModifier = 10;
            float inertia = 5;
            float heightDifference = 500;
            float rotationModifier = 10;

            if (stopTimer == 0)
            {
                attackTimer = attackStartDelay;
                stopTimer++;
            }


            Player target = Main.player[npc.target];

            attackTimer--;

            if (attackTimer <= 0)
            {
                attackCount++;
                attackTimer = projectileTimer;
                SoundEngine.PlaySound(SlimeGodCore.BigShotSound);
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), target.Center, projectileSpeed, ModContent.ProjectileType<SlowWeirdGel>(), false, projectileCount, projectileAngle / 2, projectileAngle / projectileCount, npc.Center, 0, GlobDamage, 0, 0, rotationModifier);

                if (attackCount >= burstCount)
                {
                    attackCount = 0;
                    attackTimer = burstWait + projectileTimer;
                    attackCountCounter++;

                    if (attackCountCounter >= totalBurstCount)
                    {
                        SelectNextAttack(npc);
                    }
                }
            }

            Vector2 destination = target.Center + new Vector2(acceleration, heightDifference);
            Vector2 distance = npc.Center - destination;
            float speed = distance.Length() / speedModifier;

            ProjUtils.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

            if (turn == 0)
            {
                acceleration += accelerationMax;

                if (acceleration > maxDistance)
                {
                    turn = 1;
                }
            }
            else
            {
                acceleration -= accelerationMax;

                if (acceleration < -maxDistance)
                {
                    turn = 0;
                }
            }
        }
        public static void DoBehaviour_QuickDeathray(NPC npc, ref float[] ExtraAi)
        {
            ref float phase = ref ExtraAi[0];
            ref float attackTimer = ref ExtraAi[1];
            ref float attackCount = ref ExtraAi[2];
            ref float attackTypeCounter = ref ExtraAi[3];
            ref float attackType = ref ExtraAi[4];
            ref float turn = ref ExtraAi[5];
            ref float acceleration = ref ExtraAi[8];
            ref float attackCountCounter = ref ExtraAi[9];

            Player target = Main.player[npc.target];

            int totalAttackTime = 900;
            int setupTime = 60;

            attackTimer++;
            if (attackTimer < setupTime)
            {
                float heightOffset = -200;
                float inertia = 5;
                float speedModifier = 10;

                Vector2 destination = target.Center + new Vector2(0, heightOffset);
                Vector2 distance = npc.Center - destination;
                float speed = distance.Length() / speedModifier;

                ProjUtils.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);
            }

            else if (attackTimer == setupTime)
            {
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<FastDeathray>(), false, 1, 0, 0, npc.Center, 0, DeathrayDamage, 0f, npc.whoAmI);
            }
            else
            {
                npc.velocity *= 0.8f;
            }

            if (attackTimer >= totalAttackTime)
            {
                SelectNextAttack(npc);
            }

        }
        public static void DoBehaviour_BringSlimeMinions(NPC npc, ref float[] ExtraAi)
        {
            ref float attackTimer = ref ExtraAi[1];
            ref float attackCount = ref ExtraAi[2];
            ref float turn = ref ExtraAi[3];
            ref float repulseTrigger = ref ExtraAi[4];
            ref float lockedPosX = ref ExtraAi[5];
            ref float lockedPosY = ref ExtraAi[9];
            ref float flyTimer = ref ExtraAi[10];

            Player target = Main.player[npc.target];

            int amountOfSlimes = 50;
            int slimeTimer = 5;
            float slimeOffset = 900;
            int releaseCooldown = 90;
            int flyTime = 60;
            int setupTime = 60;
            int cooldownTime = 330;

            float speedModifier = 10;
            float inertia = 5;

            float arenaSize = 1100;
            float visualArenaSize = 700;

            if (flyTimer < flyTime)
            {
                flyTimer++;

                turn = -1;

                Vector2 destination = target.Center;
                Vector2 distance = npc.Center - destination;
                float speed = distance.Length() / speedModifier;

                ProjUtils.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);
                return;
            }

            if (turn == -1)
            {
                turn = 0;
                attackTimer = setupTime;
            }

            npc.velocity *= 0.9f;

            if (lockedPosX == 0)
            {
                lockedPosX = npc.Center.X;
                lockedPosY = npc.Center.Y;
                ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<ArenaSlime>(), false, 1, 0, 0, npc.Center, 0, 0, lockedPosX, lockedPosY, visualArenaSize, (amountOfSlimes * slimeTimer) + releaseCooldown);
            }

            if (!target.WithinRange(new Vector2(lockedPosX, lockedPosY), arenaSize))
            {
                target.velocity = Vector2.Zero;
            }

            attackTimer--;
            if (attackTimer <= 0)
            {
                if (turn == 0)
                {
                    attackTimer = slimeTimer;
                    attackCount++;
                    ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<PulledInSlime>(), false, 1, 0, 0, npc.Center + (Vector2.One.RotatedByRandom(360) * slimeOffset), 0, BringInSlimeDamage, npc.whoAmI);

                    if (attackCount >= amountOfSlimes)
                    {
                        turn = 1;
                        attackTimer = releaseCooldown;
                    }
                }
                else
                {
                    if (repulseTrigger == 0)
                    {
                        repulseTrigger = 1;

                    }
                    else if (repulseTrigger == 1)
                    {
                        attackTimer = cooldownTime;
                        repulseTrigger = 2;
                    }
                    else
                    {
                        SelectNextAttack(npc);
                    }
                }

            }

        }
        public static void DoBehaviour_CircleDash(NPC npc, ref float[] ExtraAi)
        {
            ref float phase = ref ExtraAi[0];
            ref float attackTimer = ref ExtraAi[1];
            ref float attackCount = ref ExtraAi[2];
            ref float attackTypeCounter = ref ExtraAi[3];
            ref float attackType = ref ExtraAi[4];
            ref float dashing = ref ExtraAi[5];
            ref float rotation = ref ExtraAi[8];
            ref float slowDown = ref ExtraAi[9];

            if (slowDown > 0)
            {
                slowDown--;
                npc.velocity *= 0.8f;
                return;
            }

            Player target = Main.player[npc.target];

            int dashTime = 80;
            int cycles = 4;
            int circleTime = 180;
            int circleSlowdown = 30;

            int polterProjCooldown = 5;

            int circleProjTime = 18;

            float circleOffset = 400;

            float dashSpeed = 30;

            float speedModifier = 15;
            float inertia = 5;

            float rotateSpeed = 3f;

            float projectileSpeed = 2f;

            attackTimer++;

            if (dashing == 0)
            {
                Vector2 destination = target.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(rotation)) * circleOffset;
                Vector2 distance = npc.Center - destination;
                float speed = distance.Length() / speedModifier;

                rotation += rotateSpeed;
                ProjUtils.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

                if (attackTimer % circleProjTime == 0)
                {
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                    ProjUtils.ShootProjectile(npc.GetSource_FromAI(), target.Center, 2f, ModContent.ProjectileType<AcceleratingGel>(), false, 2, 75, 75, npc.Center, 0, GlobDamage);
                }

                if (attackTimer >= circleTime)
                {
                    dashing = 1;
                    slowDown = circleSlowdown;
                    attackTimer = 0;
                }
            }
            else
            {
                if (attackTimer == 1)
                {
                    Vector2 direction = (target.Center - npc.Center).SafeNormalize(Vector2.UnitX);

                    npc.velocity = new Vector2(dashSpeed) * direction;
                    SoundEngine.PlaySound(SlimeGodCore.BigShotSound);
                }
                else if (attackTimer >= dashTime)
                {
                    dashing = 0;
                    attackTimer = 0;
                    attackCount++;

                    if (attackCount >= cycles)
                    {
                        SelectNextAttack(npc);
                    }
                }

                if (attackTimer % polterProjCooldown == 0)
                {
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                    ProjUtils.ShootProjectile(npc.GetSource_FromAI(), npc.Center + npc.velocity, projectileSpeed, ModContent.ProjectileType<TelegraphedGel>(), false, 2, 90, 180, npc.Center, 0, GlobDamage);
                }
            }


        }
        #endregion

        public enum NewSlimeGodCoreAttackType
        {
            HoverAndDoNothing,
            DoAbsolutelyNothing,
            PhaseTransitionAnimation,
            SpinBursts,
            HorizontalCharges,
            VerticalHoverBursts,
            SideToSide,
            QuickDeathray,
            BringSlimeMinions,
            CircleDash,
            Attach,
            Explosions,
        }

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
        {
            return npc.type == ModContent.NPCType<SlimeGodCore>();
        }
        public static void SelectNextAttack(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int i = 0; i < 5; i++)
                npc.Infernum().ExtraAI[i] = 0f;

            npc.Infernum().ExtraAI[8] = 0f;
            npc.Infernum().ExtraAI[9] = 0f;
            npc.Infernum().ExtraAI[10] = 0f;
            npc.Infernum().ExtraAI[11] = 0f;
            npc.Infernum().ExtraAI[12] = 0f;

            npc.TargetClosest();

            float oldLocalState = npc.ai[0];
            ref float localState = ref npc.ai[0];

            int tries = 0;
            WeightedRandom<NewSlimeGodCoreAttackType> newStatePicker = new(Main.rand);

            if (FightState != SlimeGodFightState.CorePhase)
                newStatePicker.Add(NewSlimeGodCoreAttackType.HoverAndDoNothing);
            else
            {
                newStatePicker.Add(NewSlimeGodCoreAttackType.SideToSide);
                newStatePicker.Add(NewSlimeGodCoreAttackType.QuickDeathray);
                newStatePicker.Add(NewSlimeGodCoreAttackType.VerticalHoverBursts);
                newStatePicker.Add(NewSlimeGodCoreAttackType.BringSlimeMinions);
                newStatePicker.Add(NewSlimeGodCoreAttackType.CircleDash);
                newStatePicker.Add(NewSlimeGodCoreAttackType.Attach);
                //newStatePicker.Add(NewSlimeGodCoreAttackType.Explosions);
                //newStatePicker.Add(NewSlimeGodCoreAttackType.HorizontalCharges);
                //newStatePicker.Add(NewSlimeGodCoreAttackType.SpinBursts);
            }

            do
            {
                localState = (int)newStatePicker.Get();
                tries++;
            }
            while (localState == oldLocalState && tries < 1000);

            // Do the final phase animation if the previous attack was just an idle hover.
            if (npc.Infernum().ExtraAI[7] == 0f && FightState == SlimeGodFightState.CorePhase)
            {
                localState = (int)NewSlimeGodCoreAttackType.PhaseTransitionAnimation;
                npc.Infernum().ExtraAI[7] = 1f;
            }

            // Do a spin after the final phase animation.
            else if (npc.Infernum().ExtraAI[7] == 1f)
            {
                localState = (int)NewSlimeGodCoreAttackType.SpinBursts;
                npc.Infernum().ExtraAI[7] = 2f;
            }
            npc.ai[1] = 0f;
            npc.netUpdate = true;
        }

        public override void PostAI(NPC npc)
        {
            ref float[] ExtraAi = ref npc.Infernum().ExtraAI;

            switch ((NewSlimeGodCoreAttackType)npc.ai[0])
            {
                case NewSlimeGodCoreAttackType.SideToSide: DoBehaviour_SideToSide(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.QuickDeathray: DoBehaviour_QuickDeathray(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.BringSlimeMinions: DoBehaviour_BringSlimeMinions(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.CircleDash: DoBehaviour_CircleDash(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.Attach: DoBehaviour_Attach(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.Explosions: DoBehaviour_Explosions(npc, ref ExtraAi); break;
            }
         }
    }

    internal sealed class SlimeGodSelectAttackHookSystem : ModSystem
    {
        MethodInfo? method = typeof(SlimeGodCoreBehaviorOverride).GetMethod(nameof(SlimeGodCoreBehaviorOverride.SelectNextAttack), Luminance.Common.Utilities.Utilities.UniversalBindingFlags);

        private static Hook? SelectAttackHook;

        private delegate void Orig_SelectAttackHook(
            NPC npc
        );

        private delegate void Hook_SelectAttackHook(
            Orig_SelectAttackHook orig,
            NPC npc
        );

        public override void OnModLoad()
        {
            if (method != null)
            {
                SelectAttackHook = new Hook(method, (Hook_SelectAttackHook)SelectAttackHook_Detour);
                SelectAttackHook.Apply();
            }
        }

        public override void Unload()
        {
            SelectAttackHook?.Dispose();
            SelectAttackHook = null;
        }

        private static void SelectAttackHook_Detour(Orig_SelectAttackHook orig, NPC npc)
        {
            SelectNextAttack(npc);
        }
    }

    #region Projectiles
    public class ArenaSlime : DashingSlime
    {
        public override string Texture => ModContent.GetInstance<SplitBigSlime>().Texture;

        public float rotation = 0;
        public bool spawned = false;
        public override void AI()
        {
            ref float lockedPosX = ref Projectile.ai[0];
            ref float lockedPosY = ref Projectile.ai[1];
            ref float circleOffset = ref Projectile.ai[2];

            if (Projectile.frameCounter < 5)
                Projectile.frame = 0;
            else if (Projectile.frameCounter >= 5 && Projectile.frameCounter < 10)
                Projectile.frame = 1;
            else if (Projectile.frameCounter >= 10 && Projectile.frameCounter < 15)
                Projectile.frame = 2;
            else if (Projectile.frameCounter >= 15 && Projectile.frameCounter < 20)
                Projectile.frame = 3;
            else
                Projectile.frameCounter = 0;

            Projectile.frameCounter++;

            Projectile.Opacity += 0.05f;

            Vector2 destination = new Vector2(lockedPosX, lockedPosY) + Vector2.One.RotatedBy(MathHelper.ToRadians(rotation)) * circleOffset;
            Vector2 distance = Projectile.Center - destination;
            float speed = distance.Length() / 15;

            rotation += 5f;
            ProjUtils.FloatAbovePlayer(Vector2.Zero, Projectile, speed, 5, destination, 0, 0, false, false, 0);

            if (!spawned)
            {
                spawned = true;
                Projectile.Center = destination;
                Projectile.velocity = Vector2.Zero;
            }

            ProjUtils.ShootProjectile(Projectile.GetSource_FromAI(), new Vector2(lockedPosX, lockedPosY), 0f, ModContent.ProjectileType<AcceleratingGel>(), false, 1, 75, 0, Projectile.Center, 0, GlobDamage);
        }
    }
    public class PulledInSlime : DashingSlime
    {
        public override string Texture => ModContent.GetInstance<SplitBigSlime>().Texture;
        bool released = false;
        int attackTimer = 0;
        public override void AI()
        {
            ref float targetIndex = ref Projectile.ai[0];

            NPC slimeGodCore = Main.npc[(int)targetIndex];

            if (!slimeGodCore.active)
            {
                Projectile.Kill();
            }

            if (Projectile.frameCounter < 5)
                Projectile.frame = 0;
            else if (Projectile.frameCounter >= 5 && Projectile.frameCounter < 10)
                Projectile.frame = 1;
            else if (Projectile.frameCounter >= 10 && Projectile.frameCounter < 15)
                Projectile.frame = 2;
            else if (Projectile.frameCounter >= 15 && Projectile.frameCounter < 20)
                Projectile.frame = 3;
            else
                Projectile.frameCounter = 0;

            Projectile.frameCounter++;

            Projectile.Opacity += 0.05f;

            float speed = 20;
            float inertia = 30;

            if (!released)
            {
                ProjUtils.FloatAbovePlayer(Projectile.Center + new Vector2(0, -1), Projectile, speed, inertia, slimeGodCore.Center, 90, 100, false, false, 0);

                Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, -0.2f, 0.2f);
                Projectile.spriteDirection = (Projectile.velocity.X < 0f).ToDirectionInt();
            }
            else
            {
                int explodeTime = 120;
                attackTimer++;
                Projectile.velocity *= 0.98f;
                if (attackTimer == explodeTime)
                {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);

                    int projectileCount = 2;
                    int type = ModContent.ProjectileType<TelegraphedGel>();
                    ProjUtils.ShootProjectile(Projectile.GetSource_Death(), Projectile.Center, 2.5f, type, false, projectileCount, Main.rand.Next(0, 361), 360 / projectileCount, Projectile.Center, 0, GlobDamage);

                    for (int i = 0; i < 100; i++)
                    {

                        Color dustColor;
                        if (Main.rand.NextBool())
                        {
                            dustColor = Color.Crimson;
                            dustColor.A = 150;
                        }
                        else
                        {
                            dustColor = Color.Lavender;
                            dustColor.A = 150;
                        }

                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDust, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), Projectile.alpha, dustColor);
                        Main.dust[dust].noGravity = true;
                    }

                    Projectile.Kill();
                }

            }

            if (slimeGodCore.Infernum().ExtraAI[4] == 1)
            {
                float releaseSpeed = 6;
                Projectile.velocity *= releaseSpeed;
                released = true;
                Projectile.damage = 0;
            }
        }
    }
    public class DashingSlime : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<SplitBigSlime>().Texture;
        int DashTime = 120;
        int DashAmount = 4;
        float PredictionStrength = 5f;
        float DashSpeed = 20f;
        bool isCrimson = false;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0;

            DashAmount = Main.rand.Next(3, 6);
            DashTime = Main.rand.Next(40, 60);
            PredictionStrength = Main.rand.NextFloat(5f, 30f);
            DashSpeed = Main.rand.NextFloat(7f, 9.5f);
            isCrimson = Main.rand.NextBool();
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }
        public int dashes = 0;
        public int explodeTimer = 0;

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);

            if (isCrimson)
            {
                lightColor.G = 100;
                lightColor.B = 100;
            }
            Luminance.Common.Utilities.Utilities.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);

            return false;
        }
        public override void AI()
        {

            if (Projectile.frameCounter < 5)
                Projectile.frame = 0;
            else if (Projectile.frameCounter >= 5 && Projectile.frameCounter < 10)
                Projectile.frame = 1;
            else if (Projectile.frameCounter >= 10 && Projectile.frameCounter < 15)
                Projectile.frame = 2;
            else if (Projectile.frameCounter >= 15 && Projectile.frameCounter < 20)
                Projectile.frame = 3;
            else
                Projectile.frameCounter = 0;

            Projectile.frameCounter++;


            ref float target = ref Projectile.ai[0];
            ref float spawnTimer = ref Projectile.ai[1];
            ref float attackTimer = ref Projectile.ai[2];

            int spawnTime = 120;

            spawnTimer++;
            if (spawnTimer == 1)
            {
                Projectile.velocity = new Vector2(0, -7);
            }
            else if (spawnTimer <= spawnTime)
            {
                Projectile.velocity *= 0.97f;
                Projectile.Opacity += 0.01f;
            }
            else if (explodeTimer == 0)
            {
                Player targetPlayer = Main.player[(int)target];
                attackTimer++;

                if (isCrimson)
                {
                    float speedModifier = 30;
                    float inertia = 5;
                    float projectileSpeed = 5;
                    Vector2 targetPosition = targetPlayer.Center + (Vector2.One.RotatedBy((Projectile.Center - targetPlayer.Center).ToRotation() - MathHelper.ToRadians(45)) * 200);
                    Vector2 distance = Projectile.Center - targetPosition;
                    float speed = distance.Length() / speedModifier;

                    ProjUtils.FloatAbovePlayer(Vector2.Zero, Projectile, speed, inertia, targetPosition, 0, 0, false, false, 0);

                    Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, -0.2f, 0.2f);
                    Projectile.spriteDirection = (Projectile.velocity.X < 0f).ToDirectionInt();

                    if (attackTimer % DashTime == 0)
                    {
                        SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                        ProjUtils.ShootProjectile(Projectile.GetSource_FromAI(), targetPlayer.Center + (targetPlayer.velocity * PredictionStrength), projectileSpeed, ModContent.ProjectileType<TelegraphedGel>(), false, 1, 0, 0, Projectile.Center, 0, GlobDamage);
                        dashes++;

                        if (dashes == DashAmount)
                        {
                            explodeTimer++;
                        }
                    }
                }
                else
                {
                    if (attackTimer % DashTime == 0)
                    {

                        Vector2 direction = ((targetPlayer.Center + (targetPlayer.velocity * PredictionStrength)) - Projectile.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.velocity = new Vector2(DashSpeed, DashSpeed) * direction;

                        Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, -0.2f, 0.2f);
                        Projectile.spriteDirection = (Projectile.velocity.X < 0f).ToDirectionInt();

                        SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.position);

                        dashes++;

                        if (dashes == DashAmount)
                        {
                            explodeTimer++;
                        }
                    }
                }

            }
            else
            {
                Projectile.velocity *= 0.97f;
                explodeTimer++;

                int explodeTime = 120;

                if (explodeTimer == explodeTime)
                {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);

                    int projectileCount = 3;
                    int type = ModContent.ProjectileType<TelegraphedSlowedBlob>();
                    float projectileSpeed = 2.5f;
                    ProjUtils.ShootProjectile(Projectile.GetSource_Death(), Projectile.Center, projectileSpeed, type, false, projectileCount, Main.rand.Next(0, 361), 360 / projectileCount, Projectile.Center, 0, GlobDamage);

                    for (int i = 0; i < 100; i++)
                    {

                        Color dustColor;
                        if (Main.rand.NextBool())
                        {
                            dustColor = Color.Crimson;
                            dustColor.A = 150;
                        }
                        else
                        {
                            dustColor = Color.Lavender;
                            dustColor.A = 150;
                        }

                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDust, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), Projectile.alpha, dustColor);
                        Main.dust[dust].noGravity = true;
                    }

                    Projectile.Kill();
                }
            }
        }
    }
    public class TelegraphedGel : AcceleratingGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingEbonianGlob>().Texture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 1200;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float telegraphInterpolant = Utils.GetLerpValue(0f, 36f, Time, true) * Utils.GetLerpValue(42f, 36f, Time, true);
            if (telegraphInterpolant >= 1f)
                telegraphInterpolant = 0f;

            Color telegraphColor = isCrimson ? Color.Red : Color.MediumPurple;
            Main.spriteBatch.DrawLineBetter(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 6000f, telegraphColor * telegraphInterpolant, telegraphInterpolant * 12f);

            return base.PreDraw(ref lightColor);
        }
    }
    public class AcceleratingGel : DeceleratingEbonianGlob
    {
        public override string Texture => ModContent.GetInstance<DeceleratingEbonianGlob>().Texture;

        public bool isCrimson = false;
        public override void SetDefaults()
        {
            base.SetDefaults();

            isCrimson = Main.rand.NextBool();
            Projectile.timeLeft *= 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            Luminance.Common.Utilities.Utilities.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture: isCrimson ? TextureAssets.Projectile[ModContent.ProjectileType<DeceleratingCrimulanGlob>()].Value : null);
            return false;
        }
    }
    public class SlowWeirdGel : WeirdGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingEbonianGlob>().Texture;

        public override float speedCap => 13;
        public override int activationTime => 10000;
    }
    public class WeirdGel : AcceleratingGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingEbonianGlob>().Texture;
        public float degrees = 0.5f;

        int homeTimer = 0;
        public virtual float speedCap => 17;
        public virtual int activationTime => 10000;
        public override void AI()
        {
            base.AI();
            float modifiedDegress = Projectile.ai[2] == 0 ? degrees : Projectile.ai[2];

            if (Projectile.velocity.Length() < speedCap)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(modifiedDegress) * (Projectile.ai[0] == 1 ? 1 : -1));
            }

            if (Projectile.velocity.Length() > speedCap)
            {
                Projectile.velocity /= 1.0138f;
            }

            homeTimer++;
            float maxRange = 550;
            if (NPC.FindFirstNPC(ModContent.NPCType<SlimeGodCore>()) == -1)
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[Main.npc[NPC.FindFirstNPC(ModContent.NPCType<SlimeGodCore>())].target];

            if (homeTimer >= activationTime - 30)
            {
                for (int i = 0; i < 100; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + Vector2.One.RotatedBy(MathHelper.ToRadians((360 / 100) * i)) * maxRange / 2, 0, 0, DustID.TintableDust, 0, 0, 150, isCrimson ? Color.Crimson : Color.Lavender);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (homeTimer >= activationTime && Projectile.WithinRange(target.Center, maxRange))
            {
                ProjUtils.FloatAbovePlayer(Vector2.Zero, Projectile, 200, 5, target.Center, 0, 0, false, false, 0);
            }
        }
    }
    public class SlowedGelBlob : AcceleratingGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingEbonianGlob>().Texture;

        public override void AI()
        {
            base.AI();
            float speedCap = 6.5f;
            if (Projectile.velocity.Length() > speedCap)
            {
                Projectile.velocity /= 1.0138f;
            }
        }
    }
    public class TelegraphedSlowedBlob : SlowedGelBlob
    {
        public override string Texture => ModContent.GetInstance<DeceleratingEbonianGlob>().Texture;

        public override bool PreDraw(ref Color lightColor)
        {
            float telegraphInterpolant = Utils.GetLerpValue(0f, 36f, Time, true) * Utils.GetLerpValue(42f, 36f, Time, true);
            if (telegraphInterpolant >= 1f)
                telegraphInterpolant = 0f;

            Color telegraphColor = isCrimson ? Color.Red : Color.MediumPurple;
            Main.spriteBatch.DrawLineBetter(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 6000f, telegraphColor * telegraphInterpolant, telegraphInterpolant * 20f);

            return base.PreDraw(ref lightColor);
        }
    }
    public class FastDeathray : SlimeDeathray
    {
        public override float MaxAcceleration => 0.033f;
        public override float ReversalAccel => 0.0005f;
        public override float RegularAccel => 0.00025f;
        public override string Texture => ModContent.GetInstance<BrimstoneDeathray>().Texture;
        public override float Lifetime => 800;
        public override float ProjectileGaps => 225;
        public override int ProjectileReleaseRate => 80;
    }
    public class SlimeDeathray : BaseLaserbeamProjectile
    {
        public PrimitiveTrailCopy LaserDrawer
        {
            get;
            set;
        }

        public int OwnerIndex => (int)Projectile.ai[1];
        public override float Lifetime => 1200;
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => Color.MediumPurple;
        public override Texture2D LaserBeginTexture => TextureAssets.Projectile[Projectile.type].Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd", AssetRequestMode.ImmediateLoad).Value;
        public override string Texture => ModContent.GetInstance<BrimstoneDeathray>().Texture;
        public override float MaxLaserLength => 3100f;
        public override float MaxScale => 3f;
        public int TelegraphTime => 120;
        public float Acceleration = 0f;
        public virtual float MaxAcceleration => 0.0125f;
        public virtual float ReversalAccel => 0.00025f;
        public virtual float RegularAccel => 0.0001f;
        public virtual float ProjectileGaps => 175f;
        public virtual int ProjectileReleaseRate => 120;
        public bool Checked = false;
        public virtual Vector2 OwnerEyePosition => Main.npc[OwnerIndex].Center;
        // public override void SetStaticDefaults() => DisplayName.SetDefault("Brimstone Deathray");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
            //Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public override void AttachToSomething()
        {
            if (!Main.npc[OwnerIndex].active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = OwnerEyePosition;



            if (Projectile.timeLeft <= Lifetime - TelegraphTime)
            {
                if (!Checked)
                {
                    Checked = true;
                    SoundEngine.PlaySound(SoundID.Zombie104 with { Pitch = -1 }, Projectile.position);
                }

                if (Projectile.timeLeft <= Lifetime / 2)
                {
                    Acceleration = MathHelper.Clamp(Acceleration - ReversalAccel, -MaxAcceleration, MaxAcceleration);
                    Projectile.velocity = Projectile.velocity.RotatedBy(Acceleration);
                }
                else
                {
                    Acceleration = MathHelper.Clamp(Acceleration + RegularAccel, 0, MaxAcceleration);
                    Projectile.velocity = Projectile.velocity.RotatedBy(Acceleration);
                }


                if (Projectile.timeLeft % ProjectileReleaseRate == 0)
                {

                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        return;

                    for (float petalOffset = 20f; petalOffset < LaserLength; petalOffset += ProjectileGaps)
                    {
                        Vector2 petalSpawnPosition = OwnerEyePosition + Projectile.velocity * petalOffset;
                        for (int i = -1; i <= 1; i++)
                        {

                            if (i == 0)
                            {
                                continue;
                            }
                            Vector2 petalVelocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * i) * 1f;
                            InfernumMode.Utilities.NewProjectileBetter(petalSpawnPosition, petalVelocity, ModContent.ProjectileType<AcceleratingGel>(), GlobDamage, 0f);

                        }
                    }
                }
            }
            else
            {
                Projectile.velocity = Vector2.One.RotatedBy(Projectile.ai[2] + MathHelper.ToRadians(45) + MathHelper.ToRadians(180));
            }



        }

        public float LaserWidthFunction(float completionRatio) => Projectile.scale * Projectile.width * Utils.GetLerpValue(0.02f, 0.05f, completionRatio, true) * 3f;

        public static Color LaserColorFunction(float completionRatio)
        {
            float colorInterpolant = (float)Math.Sin(Main.GlobalTimeWrappedHourly * -5.2f + completionRatio * 23f) * 0.5f + 0.5f;
            return Color.Lerp(Color.Red, Color.MediumPurple, colorInterpolant);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            LaserDrawer ??= new(LaserWidthFunction, LaserColorFunction, null, true, InfernumEffectsRegistry.GenericLaserVertexShader);
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * LaserLength;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(Projectile.Center, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Select textures to pass to the shader, along with the electricity color.
            Color middleColor = new(252, 220, 178);
            Color middleColor2 = new(255, 162, 162);
            InfernumEffectsRegistry.GenericLaserVertexShader.UseColor(middleColor2 * 2f);
            InfernumEffectsRegistry.GenericLaserVertexShader.SetShaderTexture(InfernumTextureRegistry.StreakFire);

            LaserDrawer.Draw(baseDrawPoints, -Main.screenPosition, 60);
            return false;
        }

        public override bool? CanDamage() => Time > TelegraphTime ? null : false;
    }
    #endregion
}
