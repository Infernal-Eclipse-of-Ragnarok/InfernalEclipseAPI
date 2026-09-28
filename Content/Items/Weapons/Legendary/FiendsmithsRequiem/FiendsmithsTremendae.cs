using CalamityMod;
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

        private int shootTimer;

        public static Item FalseGun = null;
        public static Item Requiem = null;

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

        private static void DefineFalseGun(int baseDamage)
        {
            int sniperID = ItemID.SniperRifle;
            int FRID = ModContent.ItemType<FiendsmithsRequiem>();
            FalseGun = new Item();
            Requiem = new Item();
            FalseGun.SetDefaults(sniperID, true);
            Requiem.SetDefaults(FRID, true);
            FalseGun.damage = baseDamage;
            FalseGun.knockBack = Requiem.knockBack;
            FalseGun.shootSpeed = Requiem.shootSpeed;
            FalseGun.consumeAmmoOnFirstShotOnly = false;
            FalseGun.consumeAmmoOnLastShotOnly = false;

            FalseGun.DamageType = LegendarySummon.Instance;
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

            if (Projectile.localAI[0] == 0f)
            {
                //Spawn dust
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 direction = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                    direction = direction.RotatedBy((double)((dustIndex - (dustAmt / 2f - 1f)) * TwoPi / dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = direction - Projectile.Center;
                    int fire = Dust.NewDust(direction + dustVel, 0, 0, DustID.LavaMoss, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity = dustVel;
                }

                // Construct a fake item to use with vanilla code for the sake of firing bullets.
                if (FalseGun is null)
                    DefineFalseGun(Projectile.originalDamage);

                Projectile.localAI[0] += 1f;
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
                Vector2 directionToTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);

                // Point directly toward the target.
                Projectile.rotation = directionToTarget.ToRotation();

                Projectile.direction = Projectile.spriteDirection = directionToTarget.X >= 0f ? 1 : -1;

                // If the texture naturally faces RIGHT, flip it vertically when aiming to the left so it doesn't appear upside-down.
                if (Projectile.spriteDirection == -1) Projectile.rotation += Pi;

                shootTimer++;

                if (shootTimer >= ShootRate)
                {
                    shootTimer = 0;

                    SoundEngine.PlaySound(SoundID.Item40, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        int projType = ProjectileID.BulletHighVelocity;

                        bool dontConsumeAmmo = Main.rand.NextBool();
                        int projIndex;

                        player.PickAmmo(FalseGun, out int projID, out float shootSpeed, out int damage, out float kb, out _, dontConsumeAmmo);

                        Vector2 bulletVelocity = directionToTarget * shootSpeed;

                        if (projID == ProjectileID.Bullet || projID == 0)
                            projID = projType;

                        Vector2 bulletSpawnPosition = Projectile.Center + directionToTarget * (Projectile.width * 0.5f) + directionToTarget.RotatedBy(PiOver2) * (8f * Projectile.spriteDirection);

                        projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), bulletSpawnPosition, bulletVelocity, projID, damage, kb, Projectile.owner);

                        if (projIndex.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[projIndex].DamageType = LegendarySummon.Instance;
                            Main.projectile[projIndex].minion = false;
                        }
                        Projectile.netUpdate = true;
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
