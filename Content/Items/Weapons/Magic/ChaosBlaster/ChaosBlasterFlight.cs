using System.Collections.Generic;
using System.Linq;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Content.Items.Weapons.Magic.ChaosBlaster
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class ChaosBlasterFlight : ModPlayer, IPixelatedPrimitiveRenderer
    {
        private const float FlightSpeed = 21f;
        private bool wasRightClickHeld;

        public bool IsFlying { get; private set; }
        public int FlightTimer { get; private set; }
        public int AfterimageCount => 8;
        public Vector2[] OldPositions { get; private set; } = new Vector2[45];
        public int Frame { get; private set; }
        public float Scale => IsFlying ? 1f : 1.2f;

        public override void PreUpdate()
        {
            if (Player.HeldItem.type != ModContent.ItemType<ChaosBlaster>()) return;

            bool flag = Main.mouseRight && Player.HeldItem.type == ModContent.ItemType<ChaosBlaster>();
            if (flag && !wasRightClickHeld && !IsFlying) StartFlying();
            if (!flag && wasRightClickHeld && IsFlying) StopFlying();
            wasRightClickHeld = flag;
            if (!IsFlying) return;
            UpdateFlight();
            ++FlightTimer;
            if (Player.HeldItem.type == ModContent.ItemType<ChaosBlaster>() && Main.mouseRight) return;
            StopFlying();
        }

        public void StartFlying()
        {
            if (Player.HeldItem.type != ModContent.ItemType<ChaosBlaster>()) return;

            IsFlying = true;
            FlightTimer = 0;
            SoundStyle soundStyle = SoundID.Item82;
            soundStyle.Volume = 0.7f;
            soundStyle.Pitch = 0.5f;
            SoundEngine.PlaySound(soundStyle, new Vector2?(Player.Center), null);
            for (int index = 0; index < OldPositions.Length; ++index) OldPositions[index] = Player.Center;
        }

        public void StopFlying()
        {
            IsFlying = false;
            FlightTimer = 0;
        }

        private void UpdateFlight()
        {
            if (Player.HeldItem.type != ModContent.ItemType<ChaosBlaster>()) return;

            // Shift trail positions
            for (int i = OldPositions.Length - 1; i > 0; --i)
                OldPositions[i] = OldPositions[i - 1];
            OldPositions[0] = Player.Center;

            // Fly toward mouse
            Vector2 dir = Utils.SafeNormalize(Main.MouseWorld - Player.Center, Vector2.UnitX);
            Player.velocity = dir * 21f;

            // Flight flags
            Player.noFallDmg = true;
            Player.gravity = 0f;
            Player.maxFallSpeed = 21f;
            Player.fallStart = (int)(Player.position.Y / 16f);
            Player.noKnockback = true;

            // Simulate holding DOWN (as in star-fall dive)
            Player.controlDown = true;
            Player.releaseDown = false;
            Player.controlDownHold = true;
            Player.GoingDownWithGrapple = true;

            UseStarFlyEffects();

            Player.direction = Math.Sign(dir.X);
            if (Player.direction == 0)
                Player.direction = 1;
        }

        public void UseStarFlyEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            float t = Main.rand.NextFloat();
            float scale = MathHelper.Lerp(0.2f, 0.4f, t) * Scale;

            Color col = Color.Lerp(new Color(1f, 0.41f, 0.51f),    // pinkish (float ctor 0..1)
                                   new Color(1f, 0.85f, 0.37f),    // gold-ish
                                   Main.rand.NextFloat());

            Vector2 spawn =
                Player.Center +
                new Vector2(Player.direction * 10f, 8f) +
                Main.rand.NextVector2Circular(16f, 16f);

            Dust d = Dust.NewDustPerfect(
                spawn,
                DustID.PortalBoltTrail,
                Main.rand.NextVector2Circular(3f, 3f) + Player.velocity,
                0,
                col,
                scale
            );
            d.noGravity = true;
            d.fadeIn = scale * 0.5f;

            Frame = 25;
        }

        public override void PostUpdate()
        {
            if (Player.HeldItem.type != ModContent.ItemType<ChaosBlaster>()) return;

            if (!IsFlying)
                return;

            Player.gravity = 0f;
            Player.maxFallSpeed = 21f;
        }

        public float StarFallTrailWidthFunction(float completionRatio)
        {
            // Wide near the player, taper along the trail
            return Scale * MathHelper.Lerp(32f, 1f, MathHelper.Clamp(completionRatio / 0.9f, 0f, 1f));
        }

        public Color StarFallTrailColorFunction(float completionRatio)
        {
            // Blue-white that fades out along the trail
            return new Color(75, 128, 250, 0) * (float)Math.Sqrt(1.0 - completionRatio);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            if (!IsFlying)
                return;

            var settings = new PrimitiveSettings(
                StarFallTrailWidthFunction,
                StarFallTrailColorFunction,
                // Offset to sprite center and a slight push along velocity
                _ => Player.Size * 0.5f + Utils.SafeNormalize(Player.velocity, Vector2.Zero) * 4f,
                Pixelate: true
            );

            List<Vector2> pts = OldPositions.Take(5).ToList();
            if (pts.Count < 2)
                return;

            PrimitiveRenderer.RenderTrail(pts, settings, 42);
        }

        public override void DrawEffects(
            PlayerDrawSet drawInfo,
            ref float r,
            ref float g,
            ref float b,
            ref float a,
            ref bool fullBright)
        {
            if (!IsFlying)
                return;

            var tint = new Color(255, 178, 97);
            r = tint.R / 255f;
            g = tint.G / 255f;
            b = tint.B / 255f;
            fullBright = true;

            if (Main.rand.NextBool(4))
            {
                var c = Color.White; c.A = 0;
                Dust d = Dust.NewDustPerfect(
                    drawInfo.Position + Main.rand.NextVector2Circular(Player.width, Player.height),
                    DustID.Electric,
                    Vector2.Zero,
                    0,
                    c * 0.5f,
                    1.5f
                );
                d.noGravity = true;
                d.fadeIn = 1.2f;
            }

            if (Main.rand.NextBool(3))
            {
                var c = new Color(0f, 0.25f, 1f, 0f) * 0.2f;
                Dust d = Dust.NewDustPerfect(
                    Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height) * 0.5f,
                    DustID.Electric,
                    Main.rand.NextVector2Circular(1f, 1f),
                    0,
                    c,
                    1.2f
                );
                d.noGravity = true;
                d.fadeIn = 0.8f;
            }
        }
    }
}
