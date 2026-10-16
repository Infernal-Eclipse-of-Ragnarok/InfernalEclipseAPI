using System.Collections.Generic;
using InfernalEclipseAPI.Core.Graphics.Automators;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfernalEclipseAPI.Content.Items.Weapons.Ranged.ExoDisintegrator
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class DisintegratorBeam : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];

        public ref float LaserbeamDirection => ref Projectile.ai[1];

        public ref float LaserbeamLength => ref Projectile.ai[2];

        public static int Lifetime => 3600;

        public static float MaxLaserbeamLength => 5600f;

        public override string Texture => "Luminance/Assets/InvisiblePixel";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 8000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            Player player = Main.player[this.Projectile.owner];
            if (!player.active || player.dead || !player.channel)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.Center = player.Center;
                LaserbeamDirection = player.AngleTo(Main.MouseWorld);
                Projectile.velocity = Utils.ToRotationVector2(this.LaserbeamDirection);
                LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 172f, 0.0f, MaxLaserbeamLength);
                ++Time;
            }
        }

        public float LaserWidthFunction(float completionRatio)
        {
            float num1 = (float)((double)Luminance.Common.Utilities.Utilities.Convert01To010(Luminance.Common.Utilities.Utilities.InverseLerp(0.15f, 0.85f, this.LaserbeamLength / MaxLaserbeamLength, true)) * (double)Luminance.Common.Utilities.Utilities.InverseLerp(0.0f, 0.05f, completionRatio, true) * 32.0 + (double)MathF.Cos(Main.GlobalTimeWrappedHourly * 90f) * 6.0 + 300.0);
            float num2 = Luminance.Common.Utilities.Utilities.InverseLerp(0.0f, 8f, (float)Lifetime - Time, true);
            float num3 = MathF.Sqrt(1.001f - MathF.Pow(Luminance.Common.Utilities.Utilities.InverseLerp(0.05f, 0.012f, completionRatio, true), 2f));
            return MathHelper.Lerp(4f, num1, LaserbeamLength / MaxLaserbeamLength) * num2 * num3;
        }

        public float BloomWidthFunction(float completionRatio)
        {
            return this.LaserWidthFunction(completionRatio) * 1.9f;
        }

        public Color LaserColorFunction(float completionRatio)
        {
            double num1 = (double)Luminance.Common.Utilities.Utilities.InverseLerp(0.0f, 0.45f, this.LaserbeamLength / MaxLaserbeamLength, true);
            float num2 = Luminance.Common.Utilities.Utilities.InverseLerp(0.0f, 0.032f, completionRatio, true);
            float num3 = Luminance.Common.Utilities.Utilities.InverseLerp(0.95f, 0.81f, completionRatio, true);
            double num4 = (double)num2;
            float num5 = (float)(num1 * num4) * num3;
            return Projectile.GetAlpha(new Color(255, 40, 55)) * num5;
        }

        public static Color BloomColorFunction(float completionRatio)
        {
            return new Color(255, 10, 20) * (Luminance.Common.Utilities.Utilities.InverseLerpBump(0.02f, 0.05f, 0.81f, 0.95f, completionRatio) * 0.54f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float pulse = MathHelper.Lerp(0f, 1f, (MathF.Cos(Main.GlobalTimeWrappedHourly * 85f) * 0.5f) + 0.5f);

            // Beam path points
            List<Vector2> points = new();
            for (int i = 0; i <= 12; i++)
            {
                float t = i / 12f;
                points.Add(Projectile.Center + Projectile.velocity * t * LaserbeamLength);
            }
            // Pull the first point back a bit
            points[0] -= Projectile.velocity * 100f;

            // Bloom pass
            ManagedShader bloomShader = ShaderManager.GetShader("NoxusBoss.PrimitiveBloomShader");
            bloomShader.TrySetParameter("innerGlowIntensity", 0.45f);

            var bloomSettings = new PrimitiveSettings(
                BloomWidthFunction,
                BloomColorFunction,
                null,          // your offset function matching the original b__19_0
                true,
                false,
                bloomShader,
                null,
                null,
                false,
                null
            );
            PrimitiveRenderer.RenderTrail(points, bloomSettings, 70);

            // Main beam pass
            ManagedShader beamShader = ShaderManager.GetShader("NoxusBoss.ExoelectricDisintegrationRayShader");
            beamShader.TrySetParameter("centerGlowExponent", 2.9f);
            beamShader.TrySetParameter("centerGlowCoefficient", 9.3f);
            beamShader.TrySetParameter("edgeGlowIntensity", 23f / 500f);
            beamShader.TrySetParameter("centerDarkeningFactor", 0.6f);
            beamShader.TrySetParameter("innerScrollSpeed", 0.85f);
            beamShader.TrySetParameter("middleScrollSpeed", 0.5f);
            beamShader.TrySetParameter("outerScrollSpeed", 0.2f);
            beamShader.TrySetParameter("globalTime", Main.GlobalTimeWrappedHourly);
            beamShader.SetTexture(SolynTexturesRegistry.Noise.DendriticNoiseZoomedOut, 1, SamplerState.LinearWrap);
            beamShader.SetTexture(SolynTexturesRegistry.Noise.PerlinNoise, 2, SamplerState.LinearWrap);

            var beamSettings = new PrimitiveSettings(
                LaserWidthFunction,
                LaserColorFunction,
                null,          // your offset function matching the original b__19_1
                true,
                false,
                beamShader,
                null,
                null,
                false,
                null
            );
            PrimitiveRenderer.RenderTrail(points, beamSettings, 120);

            // Center bloom/flair
            float scale = Luminance.Common.Utilities.Utilities.InverseLerp(0f, 12f, Time, true) *
                          MathHelper.Lerp(1f, 1.2f, pulse);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D bloomSmall = MiscTexturesRegistry.BloomCircleSmall.Value;
            Texture2D shineFlare = MiscTexturesRegistry.ShineFlareTexture.Value;

            // Shine flares
            for (int i = 0; i < 3; i++)
            {
                Color c = Projectile.GetAlpha(new Color(Color.Wheat.R, Color.Wheat.G, Color.Wheat.B, 0));
                Main.spriteBatch.Draw(
                    shineFlare,
                    drawPos,
                    null,
                    c,
                    0f,
                    shineFlare.Size() * 0.5f,
                    scale * 2f,
                    SpriteEffects.None,
                    0f
                );
            }

            // Small blooms
            for (int i = 0; i < 2; i++)
            {
                Color c = Projectile.GetAlpha(new Color(Color.White.R, Color.White.G, Color.White.B, 0));
                Main.spriteBatch.Draw(
                    bloomSmall,
                    drawPos,
                    null,
                    c,
                    0f,
                    bloomSmall.Size() * 0.5f,
                    scale * 2f,
                    SpriteEffects.None,
                    0f
                );
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float lineWidth = LaserWidthFunction(0.25f) * 1.8f;

            Vector2 start = Projectile.Center;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + dir * LaserbeamLength * 0.95f;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                start,
                end,
                lineWidth,
                ref collisionPoint
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);

            for (int i = 0; i < 12; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2Square(-30f, 30f);
                Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail);

                d.velocity = dir.RotatedByRandom(0.85f) * Main.rand.NextFloat(2f, 12f);
                d.color = Color.Lerp(Color.Crimson, Color.Orange, Main.rand.NextFloat(0.65f));
                if (Main.rand.NextBool(4))
                    d.color = Color.Wheat;

                d.scale = Main.rand.NextFloat(0.7f, 2.2f);
                d.fadeIn = 0.85f;
                d.noGravity = true;
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
