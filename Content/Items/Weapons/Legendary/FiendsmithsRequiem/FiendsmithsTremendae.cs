using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.FiendsmithsRequiem
{
    public class FiendsmithsTremendae : ModProjectile
    {
        private const int ShootRate = 24;
        private const float BulletVelocity = 32f;

        private int shootTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
            Projectile.penetrate = -1;
            Projectile.hide = true;

            Projectile.DamageType = LegendarySummon.Instance;

            Projectile.width = 176;
            Projectile.height = 58;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;

            Projectile.timeLeft = 8;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (!player.HasBuff<FiendsmithsRequiemBuff>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            NPC target = null;

            float closestDistance = FiendsmithsRequiemPro.TargetRange;

            // Respect manually selected minion targets first.
            if (player.HasMinionAttackTargetNPC)
            {
                NPC manualTarget = Main.npc[player.MinionAttackTargetNPC];

                if (manualTarget.CanBeChasedBy(Projectile))
                {
                    float distance = Vector2.Distance(player.Center, manualTarget.Center);

                    if (distance <= FiendsmithsRequiemPro.TargetRange)
                        target = manualTarget;
                }
            }
            else
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.CanBeChasedBy(Projectile))
                        continue;

                    float distance = Vector2.Distance(player.Center, npc.Center);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = npc;
                    }
                }
            }

            Vector2 followPosition = player.Center;


            followPosition.X += 35f * player.direction;
            followPosition.Y -= 50f;

            float bobOffset = Sin(Main.GlobalTimeWrappedHourly * 2f) * 7.5f;

            Vector2 idlePosition = followPosition;

            idlePosition.Y += bobOffset;

            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePosition, 0.05f);

            Projectile.velocity *= 0.5f;

            if (target != null)
            {
                Vector2 directionToTarget =
                    (target.Center - Projectile.Center)
                    .SafeNormalize(Vector2.UnitX);

                // Point directly toward the target.
                Projectile.rotation = directionToTarget.ToRotation();

                Projectile.direction = Projectile.spriteDirection = directionToTarget.X >= 0f ? 1 : -1;

                // If the texture naturally faces RIGHT, flip it vertically
                // when aiming to the left so it doesn't appear upside-down.
                if (Projectile.spriteDirection == -1) Projectile.rotation += Pi;

                shootTimer++;

                if (shootTimer >= ShootRate)
                {
                    shootTimer = 0;

                    SoundEngine.PlaySound(SoundID.Item40, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 bulletVelocity =
                            directionToTarget * BulletVelocity;

                        Vector2 bulletSpawnPosition =
                            Projectile.Center +
                            directionToTarget * (Projectile.width * 0.5f) +
                            directionToTarget.RotatedBy(PiOver2) * (8f * Projectile.spriteDirection);

                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            bulletSpawnPosition,
                            bulletVelocity,
                            ProjectileID.BulletHighVelocity,
                            Projectile.damage,
                            Projectile.knockBack,
                            Projectile.owner
                        );
                    }
                }
            }
            else
            {
                shootTimer = 0;

                // No target: face whichever direction the player faces.
                Projectile.direction = Projectile.spriteDirection = player.direction;
                Projectile.rotation = 0f;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}
