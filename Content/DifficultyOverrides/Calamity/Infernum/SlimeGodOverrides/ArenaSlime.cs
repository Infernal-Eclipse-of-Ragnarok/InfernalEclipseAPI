using InfernalEclipseAPI.Core.Utils;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    // Boatsoon
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
            InfernalUtilities.FloatAbovePlayer(Vector2.Zero, Projectile, speed, 5, destination, 0, 0, false, false, 0);

            if (!spawned)
            {
                spawned = true;
                Projectile.Center = destination;
                Projectile.velocity = Vector2.Zero;
            }

            InfernalUtilities.ShootProjectile(Projectile.GetSource_FromAI(), new Vector2(lockedPosX, lockedPosY), 0f, ModContent.ProjectileType<AcceleratingGel>(), false, 1, 75, 0, Projectile.Center, 0, SGCoreChanges.GlobDamage);
        }
    }
}
