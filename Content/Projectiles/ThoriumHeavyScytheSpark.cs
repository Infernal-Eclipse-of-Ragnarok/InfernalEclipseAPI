using ThoriumMod;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Projectiles
{
    //WardrobeHummus
    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class ThoriumHeavyScytheSpark : ModProjectile
    {
        public override string Texture => "ThoriumMod/Projectiles/ThoriumSpark";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 1; // infinite penetrate (for bouncing)
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.extraUpdates = 1; // smooth movement
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesIDStaticNPCImmunity = false;
        }

        public override void AI()
        {
            // Optional: make it fade in
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            // Optional: add dust effect
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Bounce off walls
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

            // Never kill on bounce; die only by timeLeft
            return false;
        }

        public override bool PreAI()
        {
            // Delay before homing
            if (Projectile.ai[0] < 30f)
            {
                Projectile.ai[0]++;
                return true; // default movement
            }

            // Simple homing logic
            float speed = 7f;
            float homingStrength = 0.25f;
            NPC target = null;
            float closestDist = 800f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy(Projectile)) continue;

                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < closestDist && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1))
                {
                    closestDist = dist;
                    target = npc;
                }
            }

            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                direction *= speed;

                // Smoothly adjust velocity towards target
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction, homingStrength);
            }

            // If no target, velocity remains unchanged
            return true;
        }
    }
}
