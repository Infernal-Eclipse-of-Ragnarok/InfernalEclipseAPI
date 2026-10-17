using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NoxusBoss.Core.Graphics.Meshes;
using ReLogic.Utilities;
using Terraria.Audio;
using InfernalEclipseAPI.Core.Graphics.Automators;
using Luminance.Core.Graphics;
using Luminance.Assets;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Weapons.Nameless.NebulaGigabeam
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class NebulaChargeUp : ModProjectile
    {
        private bool hasPlayedChargeSound;
        private bool hasPlayedFireSound;
        private int loopSoundTimer;
        private static SlotId activeLoopSoundId;
        private static SlotId activeChargeSoundId;

        private Player Owner => Main.player[Projectile.owner];

        public ref float ChargeTime => ref Projectile.ai[0];

        public ref float MaxChargeTime => ref Projectile.ai[1];

        private ref float IsFiring => ref Projectile.localAI[0];

        private ref float BeamID => ref Projectile.localAI[1];

        public ref float Time => ref Projectile.localAI[0];

        public float ConvergeTime => MaxChargeTime * 0.6f;

        public float MagicCircleOpacity
        {
            get
            {
                return Utils.GetLerpValue(this.ConvergeTime, this.ConvergeTime + this.MaxChargeTime * 0.4f, this.ChargeTime, true);
            }
        }

        public float GlowInterpolant { get; set; }

        public override string Texture => "Luminance/Assets/InvisiblePixel";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[this.Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 840;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return (double)Time <= 10.0 ? new bool?(false) : base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            if (!Owner.active || Owner.dead || !Owner.channel)
            {
                Projectile.Kill();
            }
            else
            {
                Vector2 aim = Main.MouseWorld - Owner.Center;
                if (aim == Vector2.Zero)
                    aim = Vector2.UnitX * Owner.direction;
                aim = Vector2.Normalize(aim);

                Projectile.Center = Owner.Center + aim * 120f;
                Projectile.rotation = aim.ToRotation();
                Projectile.velocity = aim;

                if (IsFiring == 0f)
                {
                    ChargeTime++;

                    if (ChargeTime == 2 && !hasPlayedChargeSound)
                    {
                        hasPlayedChargeSound = true;

                        var charge = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/CosmicLaserChargeUp")
                        {
                            Volume = 1.2f
                        };
                        activeChargeSoundId = SoundEngine.PlaySound(charge);
                    }

                    // Pink spiral-ish dust while charging
                    if (MagicCircleOpacity >= 0.3f && ChargeTime <= ConvergeTime + MaxChargeTime * 0.4f)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            // Off-axis source and target points
                            Vector2 lateral = aim.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloatDirection();
                            Vector2 src = Projectile.Center + lateral * Projectile.scale * 400f + Main.rand.NextVector2Circular(10f, 10f);
                            Vector2 start = Projectile.Center + aim * 75f + aim.RotatedByRandom(1.53f) * Main.rand.NextFloat(330f, 960f);

                            Vector2 vel = (src - start) * 0.06f;

                            Dust d = Dust.NewDustPerfect(start, DustID.PortalBoltTrail);
                            d.velocity = vel;
                            d.color = Color.Pink;
                            d.scale = Main.rand.NextFloat(0.6f, 1.2f);
                            d.noGravity = true;
                            d.fadeIn = 0.7f;
                        }
                    }

                    GlowInterpolant = MathHelper.Clamp(GlowInterpolant + 0.1f, 0f, 1f);

                    if (ChargeTime >= MaxChargeTime)
                    {
                        IsFiring = 1f;

                        if (!hasPlayedFireSound)
                        {
                            hasPlayedFireSound = true;

                            var start = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/CosmicLaserStart")
                            {
                                Volume = 30f
                            };
                            SoundEngine.PlaySound(start, Owner.Center);

                            if (Main.myPlayer == Projectile.owner)
                            {
                                if (SoundEngine.TryGetActiveSound(activeChargeSoundId, out var chargeActive))
                                    chargeActive.Stop();

                                var loop = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/CosmicLaserLoop")
                                {
                                    Volume = 1.2f,
                                    Pitch = 0.1f,
                                    IsLooped = true,
                                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                                };

                                activeLoopSoundId = SoundEngine.PlaySound(loop, Owner.Center);
                                if (SoundEngine.TryGetActiveSound(activeLoopSoundId, out var loopActive))
                                    loopActive.Position = Owner.Center;

                                loopSoundTimer = 0;
                            }
                        }
                    }

                    Projectile.scale = Utils.GetLerpValue(0f, 12f, ChargeTime, clamped: true);
                    Projectile.Opacity = Utils.GetLerpValue(0f, 45f, ChargeTime, clamped: true) * Projectile.scale;
                }
                else
                {
                    // Firing state
                    Projectile.scale = 1f;
                    Projectile.Opacity = 1f;
                    GlowInterpolant = 1f;

                    if (Main.myPlayer == Projectile.owner &&
                        SoundEngine.TryGetActiveSound(activeLoopSoundId, out var loopActive))
                    {
                        loopActive.Position = Owner.Center;
                    }

                    if (Main.myPlayer != Projectile.owner)
                        return;

                    if (BeamID == 0f)
                    {
                        Vector2 beamSpawn = Owner.Center + aim * 165f;
                        int p = Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            beamSpawn,
                            aim,
                            ModContent.ProjectileType<NebulaBeam>(),
                            Projectile.damage,
                            Projectile.knockBack,
                            Projectile.owner,
                            0f, 99999f, 0f
                        );

                        BeamID = p + 1f; // store as (index + 1) to use 0 as "none"
                    }
                    else
                    {
                        int idx = (int)BeamID - 1;
                        if (idx < 0 || idx >= Main.maxProjectiles)
                            return;

                        Projectile beam = Main.projectile[idx];
                        if (beam.active && beam.type == ModContent.ProjectileType<NebulaBeam>())
                        {
                            beam.velocity = aim;
                        }
                        else
                        {
                            BeamID = 0f; // beam lost; allow respawn
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if ((double)ChargeTime <= 0.0)
                return false;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            DrawBloom();
            DrawMagicCircle();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void DrawBloom()
        {
            Color c1 = Color.Bisque * Projectile.Opacity * 0.75f;
            Color c2 = new Color(75, 33, 164) * Projectile.Opacity * 1.1f;

            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D bloomSmall = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.spriteBatch.Draw(
                bloomSmall,
                pos,
                null,
                c1,
                0f,
                bloomSmall.Size() * 0.5f,
                5f * Projectile.scale,
                SpriteEffects.None,
                0f
            );

            Texture2D bloomFlare = MiscTexturesRegistry.BloomFlare.Value;
            float rot = Main.GlobalTimeWrappedHourly * -0.4f;

            Main.spriteBatch.Draw(
                bloomFlare,
                pos,
                null,
                c2,
                rot,
                bloomFlare.Size() * 0.5f,
                2f * Projectile.scale,
                SpriteEffects.None,
                0f
            );

            Main.spriteBatch.Draw(
                bloomFlare,
                pos,
                null,
                c2,
                rot * -0.7f,
                bloomFlare.Size() * 0.5f,
                2f * Projectile.scale,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawMagicCircle()
        {
            Vector2 screenPos = Projectile.Center + Projectile.velocity * 200f - Main.screenPosition;
            Vector2 scale2D = Vector2.One * Projectile.scale * Projectile.Opacity * 1.5f;

            Color baseCol = new Color(120, 60, 255) * (MagicCircleOpacity * 1.2f);

            Texture2D circleTex = SolynTexturesRegistry.Extra.CosmicLightCircle.Value;
            GraphicsDevice gd = Main.instance.GraphicsDevice;

            Vector2 ringPos = screenPos - Projectile.velocity * 46f;

            float sx = scale2D.X * circleTex.Width;
            float sy = scale2D.Y * circleTex.Height;

            Matrix rotMat = Matrix.CreateRotationX(1.67079639f) * Matrix.CreateRotationZ(Projectile.rotation + MathHelper.PiOver2);
            Matrix world =
                Matrix.CreateScale(sx * 0.97f, -sy, sx * 0.35f) *
                rotMat *
                Matrix.CreateTranslation(ringPos.X, ringPos.Y, 0f);

            Matrix proj = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, -sx, sx);

            gd.RasterizerState = RasterizerState.CullNone;

            float t = Main.GlobalTimeWrappedHourly * -3.87f;

            var ringShader = ShaderManager.GetShader("NoxusBoss.NamelessMagicCircleRingShader");
            ringShader.TrySetParameter("uWorldViewProjection", world * Main.GameViewMatrix.TransformationMatrix * proj);
            ringShader.TrySetParameter("localTime", t);

            Color gen = baseCol; gen.A = 0;
            ringShader.TrySetParameter("generalColor", gen * MagicCircleOpacity * 1.3f);
            ringShader.TrySetParameter("glowColor", Color.White * MagicCircleOpacity * 1.2f);
            ringShader.SetTexture(SolynTexturesRegistry.Extra.CosmicLightCircleRing, 1, SamplerState.LinearWrap);
            ringShader.Apply("AutoloadPass");

            gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
            gd.Indices = MeshRegistry.CylinderIndices;
            gd.DrawIndexedPrimitives(0, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);
            gd.SetVertexBuffer(null);
            gd.Indices = null;

            Luminance.Common.Utilities.Utilities.PrepareForShaders(Main.spriteBatch, null, false);

            var circleShader = ShaderManager.GetShader("NoxusBoss.MagicCircleShader");
            Matrix view, projection;
            Luminance.Common.Utilities.Utilities.CalculatePrimitiveMatrices(Main.screenWidth, Main.screenHeight, out view, out projection, false);

            circleShader.TrySetParameter("orientationRotation", Projectile.rotation);
            circleShader.TrySetParameter("spinRotation", t);
            circleShader.TrySetParameter("flip", false);
            circleShader.TrySetParameter("uWorldViewProjection", view * projection);
            circleShader.Apply("AutoloadPass");

            // Outer circle
            Color outer = baseCol; outer.A = 0;
            Main.EntitySpriteDraw(
                circleTex,
                screenPos,
                null,
                outer,
                0f,
                circleTex.Size() * 0.5f,
                scale2D,
                SpriteEffects.None,
                0f
            );

            // Pulsing glow passes
            for (float a = 0f; a < 0.04f; a += 0.01f)
            {
                Color glow = (Color.White * GlowInterpolant) * MagicCircleOpacity * 1.3f;
                glow.A = 0;

                Main.EntitySpriteDraw(
                    circleTex,
                    screenPos,
                    null,
                    glow,
                    0f,
                    circleTex.Size() * 0.5f,
                    scale2D * (a * GlowInterpolant + 1f),
                    SpriteEffects.None,
                    0f
                );
            }

            // Center piece
            Texture2D centerTex = SolynTexturesRegistry.Extra.CosmicLightCircleCenter.Value;
            circleShader.TrySetParameter("spinRotation", 0f);
            circleShader.Apply("AutoloadPass");

            Color centerCol = Color.Lerp(baseCol, Color.White * (MagicCircleOpacity * 1.3f), 0.6f);
            centerCol.A = 0;

            Main.EntitySpriteDraw(
                centerTex,
                screenPos,
                null,
                centerCol,
                0f,
                centerTex.Size() * 0.5f,
                scale2D,
                SpriteEffects.None,
                0f
            );
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(activeLoopSoundId, out var loop))
                loop.Stop();

            if (SoundEngine.TryGetActiveSound(activeChargeSoundId, out var charge))
                charge.Stop();

            if (Main.myPlayer != Projectile.owner || BeamID <= 0f)
                return;

            int idx = (int)BeamID - 1;
            if (idx < 0 || idx >= Main.maxProjectiles)
                return;

            Projectile beam = Main.projectile[idx];
            if (beam.active && beam.type == ModContent.ProjectileType<NebulaBeam>())
                beam.Kill();
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
