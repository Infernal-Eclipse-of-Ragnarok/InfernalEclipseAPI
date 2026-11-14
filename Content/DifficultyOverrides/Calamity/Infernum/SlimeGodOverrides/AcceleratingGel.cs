using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class AcceleratingGel : DeceleratingCrimulanGlob
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
            LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture: isCrimson ? TextureAssets.Projectile[ModContent.ProjectileType<DeceleratingCrimulanGlob>()].Value : null);
            return false;
        }
    }
}
