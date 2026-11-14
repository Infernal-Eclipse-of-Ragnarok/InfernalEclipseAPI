using CalamityMod;
using CalamityMod.NPCs.SlimeGod;
using InfernalEclipseAPI.Core.Utils;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using InfernumMode.Content.Buffs;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria.Audio;
using Terraria.Utilities;
using static InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides.SGCoreChanges;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod.SlimeGodComboAttackManager;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    // Boatsoon
    public class SGCoreChanges : GlobalNPC
    {
        public static int GlobDamage => 95;
        public static int DeathrayDamage => 150;
        public static int BringInSlimeDamage => 100;

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

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            npc.lifeMax += (int)(npc.lifeMax * 0.3f);
        }

        public override void PostAI(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            ref float[] ExtraAi = ref npc.Infernum().ExtraAI;

            Player player = Main.player[npc.target];

            switch ((NewSlimeGodCoreAttackType)npc.ai[0]) 
            {
                case NewSlimeGodCoreAttackType.HoverAndDoNothing: SlimeGodCoreBehaviorOverride.DoBehavior_HoverAndDoNothing(npc, player); break;
                case NewSlimeGodCoreAttackType.SideToSide: DoBehaviour_SideToSide(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.QuickDeathray: DoBehaviour_QuickDeathray(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.BringSlimeMinions: DoBehaviour_BringSlimeMinions(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.CircleDash: DoBehaviour_CircleDash(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.Attach: DoBehaviour_Attach(npc, ref ExtraAi); break;
                case NewSlimeGodCoreAttackType.Explosions: DoBehaviour_Explosions(npc, ref ExtraAi); break;
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.ai[0] == (int)NewSlimeGodCoreAttackType.Explosions || !InfernalWorld.RagnarokModeEnabled)
                return true;

            ref float[] extraAI = ref npc.Infernum().ExtraAI;

            Utilities.DeleteAllProjectiles(true, ModContent.ProjectileType<ArenaSlime>(), ModContent.ProjectileType<AcceleratingGel>(), ModContent.ProjectileType<SlowedGelBlob>(), ModContent.ProjectileType<FastDeathray>());

            extraAI[1] = 0f;  // Attack timer
            extraAI[2] = 0f;  // Attack count
            extraAI[5] = 0f;  // Locked X
            extraAI[8] = 0f;  // Locked Y
            extraAI[9] = 0f;  // Slowdown timer
            extraAI[10] = 0f; // Explosion timer

            npc.ai[0] = (int)NewSlimeGodCoreAttackType.Explosions;
            npc.active = true;
            npc.life = 1;
            npc.netUpdate = true;
            return false;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (npc.ai[0] == 2 && InfernalWorld.RagnarokModeEnabled)
                return false;

            return null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (npc.ai[0] == 2 && InfernalWorld.RagnarokModeEnabled)
                return false;

            return null;
        }

        public static void SelectNextAttack(NPC npc, NewSlimeGodCoreAttackType? attack = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Utilities.DeleteAllProjectiles(true, ModContent.ProjectileType<ArenaSlime>(), ModContent.ProjectileType<AcceleratingGel>(), ModContent.ProjectileType<SlowedGelBlob>(), ModContent.ProjectileType<FastDeathray>());

            for (int i = 0; i < 5; i++)
                npc.Infernum().ExtraAI[i] = 0f;

            npc.Infernum().ExtraAI[8] = 0f;
            npc.Infernum().ExtraAI[9] = 0f;
            npc.Infernum().ExtraAI[10] = 0f;
            npc.Infernum().ExtraAI[11] = 0f;
            npc.Infernum().ExtraAI[12] = 0f;

            npc.TargetClosest();

            ref float[] ExtraAi = ref npc.Infernum().ExtraAI;
            ref float lockedPosX = ref ExtraAi[5];
            ref float lockedPosY = ref ExtraAi[8];

            lockedPosX = 0;
            lockedPosY = 0;

            npc.dontTakeDamage = false;

            float oldLocalState = npc.ai[0];
            ref float localState = ref npc.ai[0];

            int tries = 0;
            WeightedRandom<NewSlimeGodCoreAttackType> newStatePicker = new(Main.rand);

            if (FightState != SlimeGodFightState.CorePhase)
                newStatePicker.Add(NewSlimeGodCoreAttackType.HoverAndDoNothing);
            else if (localState == (float)NewSlimeGodCoreAttackType.BringSlimeMinions || localState == (float)NewSlimeGodCoreAttackType.Attach || localState == (float)NewSlimeGodCoreAttackType.QuickDeathray)
            {
                newStatePicker.Add(NewSlimeGodCoreAttackType.SideToSide);
                newStatePicker.Add(NewSlimeGodCoreAttackType.VerticalHoverBursts);
                newStatePicker.Add(NewSlimeGodCoreAttackType.SpinBursts);
                newStatePicker.Add(NewSlimeGodCoreAttackType.HorizontalCharges);
            }
            else
            {
                //newStatePicker.Add(NewSlimeGodCoreAttackType.SideToSide);
                newStatePicker.Add(NewSlimeGodCoreAttackType.QuickDeathray);
                //newStatePicker.Add(NewSlimeGodCoreAttackType.VerticalHoverBursts);
                newStatePicker.Add(NewSlimeGodCoreAttackType.BringSlimeMinions);
                newStatePicker.Add(NewSlimeGodCoreAttackType.CircleDash);
                newStatePicker.Add(NewSlimeGodCoreAttackType.Attach);
                newStatePicker.Add(NewSlimeGodCoreAttackType.SpinBursts);
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
            else if (npc.Infernum().ExtraAI[7] == 1f) // Do a spin after the final phase animation.
            {
                localState = (int)NewSlimeGodCoreAttackType.SpinBursts;
                npc.Infernum().ExtraAI[7] = 2f;
            }
            npc.ai[1] = 0f;
            npc.netUpdate = true;
        }

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
            int arenaSize = 750;

            npc.dontTakeDamage = true;

            if (npc.Opacity > 0f)
            {
                npc.Opacity -= 0.03f;
            }

            if (attackTimer == 0)
            {
                lockedPosX = target.Center.X;
                lockedPosY = target.Center.Y;
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<ArenaSlime>(), false, 1, 0, 0, npc.Center, 0, 0, lockedPosX, lockedPosY, visualArenaSize, totalAttackTime);
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead || target.WithinRange(new Vector2(lockedPosX, lockedPosY), arenaSize))
                    continue;

                player.AddBuff(ModContent.BuffType<Madness>(), 2);
            }

            InfernalUtilities.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

            attackTimer++;
            npc.rotation += npc.velocity.X * 0.04f;

            if (attackTimer % projectileDelay == 0)
            {
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center, projectileSpeed, ModContent.ProjectileType<SlowedGelBlob>(), false, 1, 0, 0, npc.Center + (Vector2.One.RotatedByRandom(360) * projectileOffset), 0, GlobDamage, npc.whoAmI);
            }

            if (attackTimer >= totalAttackTime)
            {
                do
                {
                    npc.Opacity += 0.05f;
                }
                while (npc.Opacity < 1f);

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
            ref float afterimageCount = ref npc.Infernum().ExtraAI[6];
            ref float lockedPosY = ref ExtraAi[8];
            ref float slowDownTimer = ref ExtraAi[9];
            ref float explodeTimer = ref ExtraAi[10];

            npc.damage = 0;
            npc.dontTakeDamage = true;
            npc.Calamity().ShouldCloseHPBar = true;

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
            float arenaSize = 750;

            int explosionDelay = 10;

            npc.rotation += 4f;

            if (lockedPosX == 0)
            {
                lockedPosX = target.Center.X;
                lockedPosY = target.Center.Y;
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<ArenaSlime>(), false, 1, 0, 0, npc.Center, 0, 0, lockedPosX, lockedPosY, visualArenaSize, slowDownTime + explodeTime + (timeBetweenBursts * totalBursts));
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead || target.WithinRange(new Vector2(lockedPosX, lockedPosY), arenaSize))
                    continue;

                player.AddBuff(ModContent.BuffType<Madness>(), 2);
            }

            Vector2 arenaCenter = new Vector2(lockedPosX, lockedPosY);

            float moveSpeed = 24f;
            float inertia = 12f;
            float arrivalDistance = 12f;

            Vector2 toCenter = arenaCenter - npc.Center;
            float distance = toCenter.Length();

            if (distance > arrivalDistance)
            {
                Vector2 desiredVelocity = toCenter.SafeNormalize(Vector2.Zero) *
                    Math.Min(moveSpeed, distance * 0.15f);

                npc.velocity = (npc.velocity * (inertia - 1f) + desiredVelocity) / inertia;

                npc.rotation += npc.velocity.X * 0.04f;
                return;
            }

            npc.velocity = Vector2.Zero;

            if (slowDownTimer < slowDownTime)
            {
                slowDownTimer++;
                return;
            }

            explodeTimer++;

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
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), target.Center, projectileSpeed, attackCount % shotsPerWeird == 0 ? ModContent.ProjectileType<WeirdGel>() : ModContent.ProjectileType<AcceleratingGel>(), false, projectileCount, Main.rand.Next(-360, 361), projectileAngle / projectileCount, npc.Center, 0, GlobDamage);

                if (attackCount >= totalBursts)
                {
                    Utilities.DeleteAllProjectiles(true, ModContent.ProjectileType<ArenaSlime>(), ModContent.ProjectileType<AcceleratingGel>(), ModContent.ProjectileType<SlowedGelBlob>());
                    npc.active = false;
                    npc.NPCLoot();
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
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), target.Center, projectileSpeed, ModContent.ProjectileType<SlowWeirdGel>(), false, projectileCount, projectileAngle / 2, projectileAngle / projectileCount, npc.Center, 0, GlobDamage, 0, 0, rotationModifier);

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

            InfernalUtilities.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

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

            npc.rotation += npc.velocity.X * 0.04f;
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

            int num = 155;

            if (npc.WithinRange(Main.LocalPlayer.Center, 2000f) && attackTimer < 150f)
            {
                float num3 = MathF.Pow(attackTimer / (float)num, 4.2f) * MathF.PI;
                float num4 = MathHelper.Lerp(24f, 300f, attackTimer / (float)num);
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Vector2 vector = -Vector2.UnitY.RotatedBy(num3 + MathF.PI * 2f * (float)i / 6f) * num4 + Main.rand.NextVector2Circular(20f, 20f);
                        Vector2 value = vector * -0.1f;
                        Dust dust = Dust.NewDustPerfect(npc.Center + vector, 136, value);
                        dust.color = Color.Lerp(Color.Red, Color.Purple, (float)i / 2f);
                        dust.color = Color.Lerp(dust.color, Color.Gray, 0.525f);
                        dust.color.A = 100;
                        dust.alpha = 100;
                        dust.scale = Main.rand.NextFloat(1.185f, 1.5f);
                        dust.fadeIn = 0.5f;
                        dust.noGravity = true;
                    }
                }
            }

            attackTimer++;
            if (attackTimer < setupTime)
            {
                npc.rotation += npc.velocity.X * 0.04f;

                float heightOffset = -200;
                float inertia = 5;
                float speedModifier = 10;

                Vector2 destination = target.Center + new Vector2(0, heightOffset);
                Vector2 distance = npc.Center - destination;
                float speed = distance.Length() / speedModifier;

                InfernalUtilities.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);
            }
            else if (attackTimer == setupTime)
            {
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<FastDeathray>(), false, 1, 0, 0, npc.Center, 0, DeathrayDamage, 0f, npc.whoAmI);
            }
            else
            {
                npc.velocity *= 0.8f;
            }

            if (attackTimer <= totalAttackTime / 2 + 60)
            {
                float num2 = Utils.Remap(attackTimer, 15f, num - 15f, 0f, PI / 24f);
                npc.rotation += num2;
            }
            else if (attackTimer >= totalAttackTime / 2 + 60 && attackTimer <= totalAttackTime)
            {
                float num2 = Utils.Remap(attackTimer / 2, 15f, num - 15f, 0f, PI / 24f);
                npc.rotation -= num2;
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

            float arenaSize = 750;
            float visualArenaSize = 700;

            if (flyTimer < flyTime)
            {
                flyTimer++;

                turn = -1;

                Vector2 destination = target.Center;
                Vector2 distance = npc.Center - destination;
                float speed = distance.Length() / speedModifier;

                InfernalUtilities.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

                npc.rotation += npc.velocity.X * 0.04f;
                return;
            }

            if (turn == -1)
            {
                turn = 0;
                attackTimer = setupTime;
            }

            npc.rotation += 8f;
            npc.velocity *= 0.9f;

            if (lockedPosX == 0)
            {
                lockedPosX = npc.Center.X;
                lockedPosY = npc.Center.Y;
                InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<ArenaSlime>(), false, 1, 0, 0, npc.Center, 0, 0, lockedPosX, lockedPosY, visualArenaSize, (amountOfSlimes * slimeTimer) + releaseCooldown);
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead || target.WithinRange(new Vector2(lockedPosX, lockedPosY), arenaSize))
                    continue;

                player.AddBuff(ModContent.BuffType<Madness>(), 2);
            }

            attackTimer--;
            if (attackTimer <= 0)
            {
                if (turn == 0)
                {
                    int num = 155;
                    float num3 = Pow(attackTimer / (float)num, 4.2f) * PI;
                    float num4 = Lerp(24f, 300f, attackTimer / (float)num);
                    for (int i = 0; i < 6; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            Vector2 vector = -Vector2.UnitY.RotatedBy(num3 + PI * 2f * (float)i / 6f) * num4 + Main.rand.NextVector2Circular(20f, 20f);
                            Vector2 value = vector * -0.1f;
                            Dust dust = Dust.NewDustPerfect(npc.Center + vector, DustID.Crimslime, value);
                            dust.color = Color.Lerp(Color.Red, Color.Purple, (float)i / 2f);
                            dust.color = Color.Lerp(dust.color, Color.Gray, 0.525f);
                            dust.color.A = 100;
                            dust.alpha = 100;
                            dust.scale = Main.rand.NextFloat(1.185f, 1.5f);
                            dust.fadeIn = 0.5f;
                            dust.noGravity = true;
                        }
                    }

                    attackTimer = slimeTimer;
                    attackCount++;
                    InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center, 0, ModContent.ProjectileType<PulledInSlime>(), false, 1, 0, 0, npc.Center + (Vector2.One.RotatedByRandom(360) * slimeOffset), 0, BringInSlimeDamage, npc.whoAmI);

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
                InfernalUtilities.FloatAbovePlayer(Vector2.Zero, npc, speed, inertia, destination, 0, 0, false, false, 0);

                if (attackTimer % circleProjTime == 0)
                {
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                    InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), target.Center, 2f, ModContent.ProjectileType<AcceleratingGel>(), false, 2, 75, 75, npc.Center, 0, GlobDamage);
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
                    InfernalUtilities.ShootProjectile(npc.GetSource_FromAI(), npc.Center + npc.velocity, projectileSpeed, ModContent.ProjectileType<TelegraphedGel>(), false, 2, 90, 180, npc.Center, 0, GlobDamage);
                }
            }

            npc.rotation += npc.velocity.X * 0.04f;
        }
    }

    internal sealed class SlimeGodSelectAttackHookSystem : ModSystem
    {
        MethodInfo? method = typeof(SlimeGodCoreBehaviorOverride).GetMethod(nameof(SlimeGodCoreBehaviorOverride.SelectNextAttack), LumUtils.UniversalBindingFlags);

        private static Hook? SelectAttackHook;

        private delegate void Orig_SelectAttackHook(NPC npc);

        private delegate void Hook_SelectAttackHook(Orig_SelectAttackHook orig, NPC npc);

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
            if (!InfernalWorld.RagnarokModeEnabled)
            {
                orig(npc);
                return;
            }

            SelectNextAttack(npc);
        }
    }
}
