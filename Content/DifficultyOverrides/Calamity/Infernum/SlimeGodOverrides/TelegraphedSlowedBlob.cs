using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class TelegraphedSlowedBlob : SlowedGelBlob
    {
        public override string Texture => ModContent.GetInstance<DeceleratingCrimulanGlob>().Texture;

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
}
