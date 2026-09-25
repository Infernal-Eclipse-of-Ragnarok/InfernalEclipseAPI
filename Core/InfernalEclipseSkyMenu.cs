using System.Collections.Generic;
using InfernalEclipseAPI.Core.Systems.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace InfernalEclipseAPI.Core
{
    public class InfernalEclipseSkyMenu : ModMenu
    {
        public class TwinklingStar
        {
            public int Time;
            public int Lifetime;
            public float BaseScale;
            public float TwinkleOffset;
            public float RotationSpeed;
            public Vector2 Position;
            public Color BaseColor;

            public TwinklingStar(int lifetime, float scale, Vector2 position, Color color, float twinkleOffset, float rotationSpeed)
            {
                Lifetime = lifetime;
                BaseScale = scale;
                Position = position;
                BaseColor = color;
                TwinkleOffset = twinkleOffset;
                RotationSpeed = rotationSpeed;
            }
        }

        private const string MenuAssetPath = "InfernalEclipseAPI/Assets/Textures/Menu";
        private const int BaseWidth = 1920;
        private const int BaseHeight = 1080;

        private const float LogoOffsetPixels = 20f;
        public static List<TwinklingStar> Stars { get; internal set; } = [];

        // Texture assets
        private Asset<Texture2D> purpleSky;
        private Asset<Texture2D> purpleEclipse;
        private Asset<Texture2D> purpleClouds;
        private Asset<Texture2D> purpleMountains;
        private Asset<Texture2D> purpleHills;
        private Asset<Texture2D> purpleLogoText;
        private Asset<Texture2D> purpleVortex;

        public override string DisplayName => "Infernal Eclipse of Ragnarok";

        public override void Load()
        {
            purpleSky = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleSky");
            purpleEclipse = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleEclipse");
            purpleClouds = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleClouds");
            purpleMountains = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleMountains");
            purpleHills = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleHills");
            purpleLogoText = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleLogoText");
            purpleVortex = ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleVortex");
        }

        public override void OnDeselected()
        {
            Stars?.Clear();
        }

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{MenuAssetPath}/PurpleLogoText");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Pixel");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Pixel");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NullSurfaceBackground>();

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/TeardropsofDragonfire");

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            // Calculate scale to fit screen (maintain aspect ratio)
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / BaseWidth;
            float yScale = (float)Main.screenHeight / BaseHeight;
            float scale = xScale;

            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (BaseWidth * scale - Main.screenWidth) * 0.5f;
                }
                else
                {
                    drawOffset.Y -= (BaseHeight * scale - Main.screenHeight) * 0.5f;
                }
            }

            Main.time = 27000;
            Main.dayTime = true;
            drawColor = Color.White;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            // Draw sky background (static, full screen)
            DrawLayer(spriteBatch, purpleSky.Value, drawOffset, scale, Color.White, BlendState.NonPremultiplied);

            // Draw twinkling stars immediately in front of sky
            HandleTwinklingStars(spriteBatch);

            // Draw vortex with custom positioning and rotation
            Vector2 vortexPosition = new Vector2(Main.screenWidth / 2f, drawOffset.Y);
            Vector2 vortexOrigin = new Vector2(purpleVortex.Value.Width / 2f, purpleVortex.Value.Height / 2f);
            float vortexRotation = Main.GlobalTimeWrappedHourly * 0.1f;
            spriteBatch.Draw(purpleVortex.Value, vortexPosition, null, Color.White * 0.9f, vortexRotation, vortexOrigin, scale, SpriteEffects.None, 0f);

            // Draw eclipse
            Vector2 eclipseCenter = new Vector2(Main.screenWidth / 2f, drawOffset.Y);
            Vector2 eclipseOrigin = new Vector2(purpleEclipse.Value.Width / 2f, purpleEclipse.Value.Height / 2f);
            spriteBatch.Draw(purpleEclipse.Value, eclipseCenter, null, Color.White, 0f, eclipseOrigin, scale, SpriteEffects.None, 0f);

            // Draw parallax layers with horizontal tiling
            DrawParallaxLayer(spriteBatch, purpleClouds.Value, scale, 0.1f, Color.White);
            DrawParallaxLayer(spriteBatch, purpleMountains.Value, scale, 0.2f, Color.White);
            DrawParallaxLayer(spriteBatch, purpleHills.Value, scale, 0.3f, Color.White);

            // Draw logo (aligned with vortex center) with pulsing animation
            Vector2 logoPosition = new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.125f);
            logoPosition.X += LogoOffsetPixels * scale;
            Vector2 logoOrigin = new Vector2(purpleLogoText.Value.Width / 2f, purpleLogoText.Value.Height / 2f);
            float pulseInterpolant = (1f + (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 0.4f)) * 0.5f;
            float logoScalePulse = scale * MathHelper.Lerp(0.93f, 1.07f, pulseInterpolant);
            spriteBatch.Draw(purpleLogoText.Value, logoPosition, null, Color.White, 0f, logoOrigin, logoScalePulse, SpriteEffects.None, 0f);

            // Restart spritebatch with normal blend mode
            spriteBatch.End();

            // Creates final spritebatch to draw the title links - Ropro
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            TitleLinksSystem.DrawMenu();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            return false;
        }

        private void DrawLayer(SpriteBatch spriteBatch, Texture2D texture, Vector2 offset, float scale, Color color, BlendState blendState)
        {
            spriteBatch.Draw(texture, offset, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void DrawParallaxLayer(SpriteBatch spriteBatch, Texture2D texture, float scale, float parallaxSpeed, Color color)
        {
            // Calculate horizontal parallax offset
            int xOffset = (int)(Main.screenPosition.X * parallaxSpeed);
            xOffset %= (int)(texture.Width * scale);

            float screenCenterX = Main.screenWidth / 2f;
            float screenCenterY = Main.screenHeight / 2f;

            // Draw multiple copies to ensure screen coverage with horizontal tiling
            for (int k = -1; k <= 1; k++)
            {
                Vector2 position = new Vector2(screenCenterX - xOffset + texture.Width * k * scale, screenCenterY);
                spriteBatch.Draw(texture, position, null, color, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), scale, SpriteEffects.None, 0f);
            }
        }

        private void InitializeStars()
        {
            Stars.Clear();
            int maxStars = 4000;

            for (int i = 0; i < maxStars; i++)
            {
                Vector2 position = new Vector2(
                    Main.rand.Next(0, Main.screenWidth),
                    Main.rand.Next(0, (int)(Main.screenHeight * 0.9f))
                );
                int lifetime = Main.rand.Next(300, 600);
                float scale = Main.rand.NextFloat(0.3f, 2.5f);
                float twinkleOffset = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float rotationSpeed = Main.rand.NextFloat(-2.5f, 2.5f);

                // Color continuum: Orange -> Yellow -> White
                float colorProgress = Main.rand.NextFloat();
                Color color;
                if (colorProgress < 0.5f)
                {
                    // Lerp from Orange to Yellow
                    color = Color.Lerp(Color.Orange, Color.Yellow, colorProgress * 2f);
                }
                else
                {
                    // Lerp from Yellow to White
                    color = Color.Lerp(Color.Yellow, Color.White, (colorProgress - 0.5f) * 2f);
                }

                var star = new TwinklingStar(lifetime, scale, position, color, twinkleOffset, rotationSpeed);
                // Set random initial time so stars appear at different lifecycle stages
                star.Time = Main.rand.Next(0, lifetime);
                Stars.Add(star);
            }
        }

        private void HandleTwinklingStars(SpriteBatch spriteBatch)
        {
            // Initialize stars on first frame
            if (Stars.Count == 0)
            {
                InitializeStars();
            }

            // Remove dead stars
            Stars.RemoveAll(s => s.Time >= s.Lifetime);

            // Spawn new stars to replace dead ones
            int maxStars = 4000;
            for (int i = 0; i < 20; i++)
            {
                if (Stars.Count < maxStars && Main.rand.NextBool(2))
                {
                    Vector2 position = new Vector2(
                        Main.rand.Next(0, Main.screenWidth),
                        Main.rand.Next(0, (int)(Main.screenHeight * 0.9f))
                    );
                    int lifetime = Main.rand.Next(300, 600);
                    float scale = Main.rand.NextFloat(0.3f, 2.5f);
                    float twinkleOffset = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    float rotationSpeed = 0f;

                    // Color continuum: Orange -> Yellow -> White
                    float colorProgress = Main.rand.NextFloat();
                    Color color;
                    if (colorProgress < 0.5f)
                    {
                        // Lerp from Orange to Yellow
                        color = Color.Lerp(Color.Orange, Color.Yellow, colorProgress * 2f);
                    }
                    else
                    {
                        // Lerp from Yellow to White
                        color = Color.Lerp(Color.Yellow, Color.White, (colorProgress - 0.5f) * 2f);
                    }

                    Stars.Add(new TwinklingStar(lifetime, scale, position, color, twinkleOffset, rotationSpeed));
                }
            }

            // Update and draw stars
            Texture2D starTexture = ModContent.Request<Texture2D>($"{MenuAssetPath}/Pixel").Value;
            foreach (var star in Stars)
            {
                star.Time++;

                // Calculate twinkle opacity (sine wave for smooth pulsing)
                float lifetimeProgress = (float)star.Time / star.Lifetime;
                float fadeIn = MathHelper.Clamp(lifetimeProgress * 3f, 0f, 1f);
                float fadeOut = MathHelper.Clamp((1f - lifetimeProgress) * 3f, 0f, 1f);
                float fade = System.Math.Min(fadeIn, fadeOut);

                float twinkle = (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 1.8f + star.TwinkleOffset) * 0.5f + 0.5f;
                float opacity = fade * MathHelper.Lerp(0.3f, 1f, twinkle);

                // Calculate rotation
                float rotation = Main.GlobalTimeWrappedHourly * star.RotationSpeed;

                // Draw star
                Color drawColor = star.BaseColor * opacity;
                spriteBatch.Draw(
                    starTexture,
                    star.Position,
                    null,
                    drawColor,
                    rotation,
                    Vector2.One * 0.5f,
                    star.BaseScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
