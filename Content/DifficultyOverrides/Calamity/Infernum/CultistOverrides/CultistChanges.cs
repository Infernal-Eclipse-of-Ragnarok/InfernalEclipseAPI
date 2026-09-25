using CalamityMod.Events;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Cultist;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.CultistOverrides
{
    public class CultistChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.CultistBoss;

        public override bool PreAI(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return true;

            // Add lightning blasts to desperation attack.
            if (npc.Infernum().ExtraAI[6] == 1f)
            {
                Player target = Main.player[npc.target];
                float attackTimer = npc.ai[1];

                /*
                // LightBurst field.
                if (attackTimer >= 185f && attackTimer < 295f)
                    DoLightBurstAttack(npc, target, attackTimer - 185f);
                */

                // Phase 1 fireball barrage.
                if (attackTimer >= 295f && attackTimer < 355f)
                    DoFireballBarrage(npc, target, attackTimer - 295f);

                if (attackTimer is 185f or 535f)
                    SpawnLightningBlast(npc, target);

                // Ice Storm.
                if (attackTimer == 385f)
                    SpawnIceStorm(npc, target);
            }

            return true;
        }

        private static void DoFireballBarrage(NPC npc, Player target, float fireballTimer)
        {
            const int fireballShootRate = 7;
            const int skyFireballShootRate = fireballShootRate * 2;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 fireballSpawnPosition = npc.Center + new Vector2(npc.spriteDirection * 24f, 6f);

            // Same aiming behavior as the Phase 1 attack.
            ref float aimRotation = ref npc.Infernum().ExtraAI[2];

            if (fireballTimer % fireballShootRate == fireballShootRate - 1f)
            {
                if (aimRotation == 0f)
                    aimRotation = (target.Center - fireballSpawnPosition + target.velocity * 30f).ToRotation();
                else
                    aimRotation = aimRotation.AngleTowards(npc.AngleTo(target.Center), 0.18f);

                float shootSpeed = Main.rand.NextFloat(12f, 14f) + npc.Distance(target.Center) * 0.011f;

                Vector2 fireballShootVelocity = aimRotation.ToRotationVector2() * shootSpeed;
                fireballShootVelocity = fireballShootVelocity.RotatedByRandom(Pi * 0.1f);

                if (BossRushEvent.BossRushActive)
                    fireballShootVelocity *= 1.5f;

                Utilities.NewProjectileBetter(fireballSpawnPosition, fireballShootVelocity, ProjectileID.CultistBossFireBall, CultistBehaviorOverride.FireballDamage, 0f);
            }

            // Phase 1 additionally rains fireballs down from above.
            if (fireballTimer % skyFireballShootRate == skyFireballShootRate - 1f)
            {
                Vector2 skyFireballSpawnPosition = target.Center - new Vector2(Main.rand.NextFloatDirection() * 600f, -850f - target.velocity.Y * 20f);

                Utilities.NewProjectileBetter(skyFireballSpawnPosition, Vector2.UnitY * 7.75f, ProjectileID.CultistBossFireBall, CultistBehaviorOverride.FireballDamage, 0f);
            }
        }

        private static void SpawnLightningBlast(NPC npc, Player target)
        {
            Vector2[] handPositions =
            [
                npc.Top + new Vector2(-12f, 6f),
                npc.Top + new Vector2(12f, 6f),
            ];

            for (int i = 0; i < 2; i++)
            {
                Vector2 orbSummonPosition = npc.Center - Vector2.UnitY * 450f;
                orbSummonPosition.X -= (i == 0).ToDirectionInt() * 350f;

                // Visual electricity from the Cultist's hand to the orb.
                for (int k = 0; k < 200; k++)
                {
                    Vector2 dustPosition = Vector2.Lerp(handPositions[i], orbSummonPosition, k / 200f);

                    Dust electricity = Dust.NewDustPerfect(dustPosition, DustID.Vortex);

                    electricity.velocity = Main.rand.NextVector2Circular(0.15f, 0.15f);
                    electricity.scale = Main.rand.NextFloat(1f, 1.2f);
                    electricity.noGravity = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int lightningCircleCount = 4;

                    for (int k = 0; k < lightningCircleCount; k++)
                    {
                        Vector2 predictivenessOffset = target.velocity * new Vector2(40f, 20f) * 0.9f;

                        Vector2 lightningVelocity = (target.Center - orbSummonPosition + predictivenessOffset).SafeNormalize(Vector2.UnitY) * 8.5f;

                        lightningVelocity = lightningVelocity.RotatedBy(TwoPi * k / lightningCircleCount);

                        if (BossRushEvent.BossRushActive)
                            lightningVelocity *= 1.3f;

                        int lightning = Utilities.NewProjectileBetter(orbSummonPosition, lightningVelocity, ProjectileID.CultistBossLightningOrbArc, CultistBehaviorOverride.LightningDamage, 0f, -1, lightningVelocity.ToRotation(), Main.rand.Next(100));

                        if (Main.projectile.IndexInRange(lightning))
                            Main.projectile[lightning].tileCollide = false;
                    }
                }
            }

            SoundEngine.PlaySound(SoundID.Item72, target.Center);
        }

        private static void DoLightBurstAttack(NPC npc, Player target, float lightBurstTimer)
        {
            // Absorb magic into the Cultist before the LightBursts begin.
            if (lightBurstTimer < 20f)
            {
                Vector2 dustSpawnPosition = npc.Center + Main.rand.NextVector2CircularEdge(85f, 85f);

                Dust light = Dust.NewDustPerfect(dustSpawnPosition, DustID.PortalBoltTrail);

                light.color = Color.Orange;
                light.velocity = (npc.Center - light.position) * 0.08f;
                light.scale = 1.4f;
                light.fadeIn = 0.3f;
                light.noGravity = true;
            }

            // Start releasing LightBursts after the initial buildup.
            if (lightBurstTimer > 25f && lightBurstTimer % 5f == 4f)
            {
                Vector2 lightSpawnPosition = target.Center + target.velocity * 15f + Main.rand.NextVector2Circular(920f, 920f) * (BossRushEvent.BossRushActive ? 1.45f : 1f);

                lightSpawnPosition += target.velocity * Main.rand.NextFloat(5f, 32f);

                CultistBehaviorOverride.CreateTeleportTelegraph(npc.Center, lightSpawnPosition, 150, true, 1);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int explosionDelay = (int)(215f - lightBurstTimer + Main.rand.Next(20));

                    Utilities.NewProjectileBetter(lightSpawnPosition, Vector2.Zero, ModContent.ProjectileType<LightBurst>(), CultistBehaviorOverride.LightBurstDamage, 0f, -1, explosionDelay);
                }
            }
        }

        private static void SpawnIceStorm(NPC npc, Player target)
        {
            // Play the normal Ice Storm sound.
            SoundEngine.PlaySound(SoundID.Item120, target.position);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 iceMassSpawnPosition = npc.Top - Vector2.UnitY * 20f;

            for (int i = 0; i < 5; i++)
            {
                Vector2 shootVelocity = (target.Center - iceMassSpawnPosition).SafeNormalize(Vector2.UnitY).RotatedBy(TwoPi * i / 5f) * 3.2f;

                Utilities.NewProjectileBetter(iceMassSpawnPosition, shootVelocity, ModContent.ProjectileType<IceMass>(), CultistBehaviorOverride.IceMassDamage, 0f);
            }

            npc.netUpdate = true;
        }
    }
}
