using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NoxusBoss.Core.BaseEntities;
using Luminance.Core.Graphics;
using InfernalEclipseAPI.Core.Graphics.Automators;
using CalamityMod;
using Terraria.DataStructures;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Weapons.Nameless.NebulaGigabeam
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class NebulaBeam : BasePrimitiveLaserbeam
    {
        public override int LaserPointCount => 45;

        public override float LaserExtendSpeedInterpolant => 0.15f;

        public override float MaxLaserLength => 3000f;

        public override ManagedShader LaserShader
        {
            get => ShaderManager.GetShader("NoxusBoss.NamelessDeityCosmicLaserShader");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 400;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.Opacity = 0.0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.MaxUpdates = 2;
        }

        int highScreenShake = 45;

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || !player.channel)
            {
                Projectile.Kill();
                return false;
            }

            if (highScreenShake-- > 0)
            {
                player.SetScreenshake(10f);
            }
            else player.SetScreenshake(2.5f);


            Projectile.Center = player.Center + Projectile.velocity * 165f;
            Projectile.scale = Utils.GetLerpValue(0f, 12f, Time, clamped: true);
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Time <= 1 ? false : base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2Square(-25f, 25f);
                Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail);

                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero)
                                                 .RotatedByRandom(0.72f);
                d.velocity = dir * Main.rand.NextFloat(1.2f, 8f);

                float t = Utils.GetLerpValue(0f, 1f,
                    (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f + i * 0.2f), true);

                d.color = Color.Lerp(Color.Cyan, Color.Fuchsia, t);
                d.scale = Main.rand.NextFloat(0.5f, 1.8f);
                d.fadeIn = 0.7f;
                d.noGravity = true;
            }
        }

        public override float LaserWidthFunction(float completionRatio)
        {
            return Projectile.scale * Projectile.width;
        }

        public override Color LaserColorFunction(float completionRatio)
        {
            float amp = (float)Math.Pow(Projectile.scale, 1.8) * Projectile.Opacity;
            return Color.White * amp;
        }

        public override void PrepareLaserShader(ManagedShader laserShader)
        {
            ref float local = ref LaserLengthFactor;
            double maxLaserLength = (double)MaxLaserLength;
            laserShader.TrySetParameter("scrollTime", (object)Main.GlobalTimeWrappedHourly);
            laserShader.TrySetParameter("uStretchReverseFactor", (object)0.15f);
            laserShader.TrySetParameter("scrollSpeedFactor", (object)1.3f);
            laserShader.TrySetParameter("lightSmashWidthFactor", (object)(1f / 500f));
            laserShader.TrySetParameter("lightSmashLengthFactor", (object)0.0f);
            laserShader.TrySetParameter("lightSmashLengthOffset", (object)-0.005f);
            laserShader.TrySetParameter("startingLightBrightness", (object)1f);
            laserShader.TrySetParameter("maxLightTexturingDarkness", (object)0.6f);
            laserShader.TrySetParameter("lightSmashEdgeNoisePower", (object)0.4f);
            laserShader.TrySetParameter("lightSmashOpacity", (object)0.0f);
            laserShader.TrySetParameter("playerCoords", (object)Vector2.Zero);
            laserShader.TrySetParameter("laserDirection", (object)((Entity)this.Projectile).velocity);
            laserShader.SetTexture(SolynTexturesRegistry.Extra.Cosmos, 1, SamplerState.AnisotropicWrap);
            laserShader.SetTexture(SolynTexturesRegistry.Noise.FireNoiseA, 2, SamplerState.AnisotropicWrap);
            laserShader.SetTexture(SolynTexturesRegistry.Noise.CrackedNoiseA, 3, SamplerState.AnisotropicWrap);
            laserShader.SetTexture(SolynTexturesRegistry.Extra.Void, 4, SamplerState.AnisotropicWrap);
            laserShader.SetTexture(SolynTexturesRegistry.Noise.SharpNoise, 5, SamplerState.AnisotropicWrap);
            laserShader.SetTexture(SolynTexturesRegistry.Noise.FireNoiseA, 6, SamplerState.AnisotropicWrap);
        }
    }
}
