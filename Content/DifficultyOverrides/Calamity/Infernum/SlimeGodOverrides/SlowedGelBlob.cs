using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class SlowedGelBlob : AcceleratingGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingCrimulanGlob>().Texture;

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
}
