using System.Collections.Generic;
using System.Text;
using Daybreak.Common.Features.ModPanel;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.Utilities;

namespace InfernalEclipseAPI.Core
{
    [Autoload(Side = ModSide.Client)]
    [ExtendsFromMod("Daybreak")]
    [JITWhenModsEnabled("Daybreak")]
    public class CustomModPanel : ModPanelStyle
    {
        // Animated text with flowing fiery gradient
        public class AnimatedModName : UIText
        {
            private readonly string originalText;

            // Constructor stores original text for animation
            public AnimatedModName(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
            {
                originalText = text;
            }

            // Applies animated gradient text formatting
            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var formattedText = GetAnimatedText(originalText, Main.GlobalTimeWrappedHourly);
                SetText(formattedText);
                base.DrawSelf(spriteBatch);
            }

            // Generates flowing gradient text with wave animation
            private static string GetAnimatedText(string text, float time)
            {
                var sb = new StringBuilder(text.Length * 12);
                var startColor = new Color(139, 0, 0);
                var endColor = new Color(255, 165, 0);
                
                for (var i = 0; i < text.Length; i++)
                {
                    float wave = MathF.Sin(time * 1.5f + i * 0.3f) * 0.5f + 0.5f;
                    var color = Color.Lerp(startColor, endColor, wave);
                    color.A = 255;
                    sb.Append($"[c/{color.Hex3()}:{text[i]}]");
                }
                
                return sb.ToString();
            }
        }

        // Complex animated icon with shaders, particles, and layered effects
        public class AnimatedIcon : UIImage
        {
            // Color constants
            private static readonly Color StartGradientColor = new Color(139, 0, 0);
            private static readonly Color EndGradientColor = new Color(255, 165, 0);
            private static readonly Color Circle1Color = new Color(255, 200, 150, 160);
            private static readonly Color Circle2Color = new Color(255, 180, 120, 160);
            private static readonly Color Circle3Color = new Color(255, 220, 180, 160);
            private static readonly Color AnkhColor = new Color(255, 255, 200);
            
            // Animation constants
            private const float BaseScale = 0.8f;
            private const float HoverScaleBonus = 0.2f;
            private const float PulseAmplitude = 0.1f;
            private const float PulseFrequency = 4f;
            private const float RotationAmplitude = 0.15f;
            private const float RotationFrequency = 2f;
            private const float Circle1ScaleMultiplier = 0.225f;
            private const float Circle2ScaleMultiplier = 0.185f;
            private const float Circle3ScaleMultiplier = 0.15f;
            private const float AnkhScaleMultiplier = 1.0f;
            private const float AnkhPulseScaleMultiplier = 0.5f;
            private const float AnkhPulseSpeed = 1.5f;
            private const float AnkhPulseColorIntensity = 0.8f;
            private const float CircleRotationSpeed = 0.3f;
            private const float ParticleSpawnInterval = 0.1f;
            private const float ParticleDistance = 40f;
            private const float ParticleDistanceVariation = 30f;
            private const float ParticleBaseLife = 2f;
            private const float ParticleLifeVariation = 2f;
            private const float ParticleBaseScale = 0.1125f;
            private const float ParticleScaleVariation = 0.15f;
            private const float ParticleGravityStrength = 30f;
            private const float ParticleVelocityDamping = 0.95f;
            
            private float hoverIntensity;
            private static UnifiedRandom random = new UnifiedRandom();
            private List<MagicParticle> particles = new List<MagicParticle>();
            private float particleTimer;
            
            // Particle data structure for floating sparkles
            private class MagicParticle
            {
                public Vector2 Position;
                public Vector2 Velocity;
                public float Life;
                public float MaxLife;
                public float Scale;
                public Color Color;
                public float Rotation;
                public float RotationSpeed;
                public bool UseAnkhIcon;
            }
            
            // Constructor 
            public AnimatedIcon(Texture2D texture) : base(texture) 
            {
            }

            // Main rendering method with layered drawing approach
            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var dims = GetDimensions().ToRectangle();
                Vector2 center = dims.Center();
                
                var ankhTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Assets/Panel/AnkhIcon").Value;
                var magicCircle = ModContent.Request<Texture2D>("InfernalEclipseAPI/Assets/Panel/MagicCircle").Value;
                var sparkleTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Assets/Panel/Sparkle").Value;
                
                bool panelHovered = Parent?.IsMouseHovering ?? false;
                hoverIntensity = MathHelper.Lerp(hoverIntensity, panelHovered ? 1f : 0f, 0.15f);
                
                float scale = BaseScale + hoverIntensity * HoverScaleBonus + MathF.Sin(Main.GlobalTimeWrappedHourly / PulseFrequency) * PulseAmplitude;
                float rotation = MathF.Sin(Main.GlobalTimeWrappedHourly / RotationFrequency) * RotationAmplitude;
                
                UpdateParticles(center, hoverIntensity);
                
                // 1. Draw rotating magical circles in background
                float circle1Rotation = Main.GlobalTimeWrappedHourly * CircleRotationSpeed;
                float circle1Scale = scale * Circle1ScaleMultiplier;
                
                spriteBatch.Draw(
                    magicCircle,
                    center,
                    magicCircle.Frame(),
                    Circle1Color,
                    circle1Rotation,
                    magicCircle.Size() / 2,
                    circle1Scale,
                    SpriteEffects.None,
                    0f
                );
                
                float circle2Rotation = Main.GlobalTimeWrappedHourly * -CircleRotationSpeed;
                float circle2Scale = scale * Circle2ScaleMultiplier;
                
                spriteBatch.Draw(
                    magicCircle,
                    center,
                    magicCircle.Frame(),
                    Circle2Color,
                    circle2Rotation,
                    magicCircle.Size() / 2,
                    circle2Scale,
                    SpriteEffects.None,
                    0f
                );
                
                float circle3Rotation = Main.GlobalTimeWrappedHourly * CircleRotationSpeed * 1.4f;
                float circle3Scale = scale * Circle3ScaleMultiplier;
                
                spriteBatch.Draw(
                    magicCircle,
                    center,
                    magicCircle.Frame(),
                    Circle3Color,
                    circle3Rotation,
                    magicCircle.Size() / 2,
                    circle3Scale,
                    SpriteEffects.None,
                    0f
                );
                
                // 2. Draw main ankh (on top of circles)
                float ankhScale = scale * AnkhScaleMultiplier;
                spriteBatch.Draw(
                    ankhTexture,
                    center,
                    ankhTexture.Frame(),
                    AnkhColor,
                    rotation,
                    ankhTexture.Size() / 2,
                    ankhScale,
                    SpriteEffects.None,
                    0f
                );
                
                // 3. Draw pulsing ankh effect (expanding pulse)
                float pulseTime = Main.GlobalTimeWrappedHourly / AnkhPulseSpeed % 1f;
                float pulseScale = ankhScale * (1.0f + pulseTime * AnkhPulseScaleMultiplier);
                float transparency = AnkhPulseColorIntensity * MathF.Sin(pulseTime * MathHelper.Pi);
                
                var pulseColor = AnkhColor * transparency;
                
                spriteBatch.Draw(
                    ankhTexture,
                    center,
                    ankhTexture.Frame(),
                    pulseColor,
                    rotation,
                    ankhTexture.Size() / 2,
                    pulseScale,
                    SpriteEffects.None,
                    0f
                );
                
                // 4. Draw floating particles (on top)
                DrawParticles(spriteBatch, sparkleTexture, ankhTexture);
            }
            
            
            // Updates particle positions and spawns new particles based on hover state
            private void UpdateParticles(Vector2 center, float hoverIntensity)
            {
                particleTimer += 1f / 60f;
                
                if (particleTimer > ParticleSpawnInterval)
                {
                    particleTimer = 0f;
                    int spawnCount = (int)((2 + hoverIntensity * 4) / 3f);
                    
                    for (int i = 0; i < spawnCount; i++)
                    {
                        float angle = random.NextFloat(MathHelper.TwoPi);
                        float distance = ParticleDistance + random.NextFloat(ParticleDistanceVariation);
                        Vector2 spawnPos = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * distance;
                        
                        float life = ParticleBaseLife + random.NextFloat(ParticleLifeVariation);
                        particles.Add(new MagicParticle
                        {
                            Position = spawnPos,
                            Velocity = new Vector2(random.NextFloat(-0.5f, 0.5f), random.NextFloat(-0.5f, 0.5f)) * 20f,
                            Life = life,
                            MaxLife = life,
                            Scale = ParticleBaseScale + random.NextFloat(ParticleScaleVariation),
                            Color = Color.Lerp(StartGradientColor, EndGradientColor, random.NextFloat()),
                            Rotation = random.NextFloat(MathHelper.TwoPi),
                            RotationSpeed = random.NextFloat(-1f, 1f) * 2f,
                            UseAnkhIcon = random.NextFloat() < 0.5f
                        });
                    }
                }
                
                for (int i = particles.Count - 1; i >= 0; i--)
                {
                    var particle = particles[i];
                    particle.Life -= 1f / 60f;
                    particle.Position += particle.Velocity * (1f / 60f);
                    particle.Rotation += particle.RotationSpeed * (1f / 60f);
                    
                    Vector2 toCenter = center - particle.Position;
                    particle.Velocity += toCenter.SafeNormalize(Vector2.Zero) * ParticleGravityStrength * (1f / 60f);
                    particle.Velocity *= ParticleVelocityDamping;
                    
                    if (particle.Life <= 0)
                    {
                        particles.RemoveAt(i);
                    }
                }
            }
            
            // Renders floating sparkle particles around the icon
            private void DrawParticles(SpriteBatch spriteBatch, Texture2D sparkleTexture, Texture2D ankhTexture)
            {
                foreach (var particle in particles)
                {
                    float alpha = particle.Life / particle.MaxLife;
                    var color = particle.Color * alpha;
                    
                    Texture2D currentTexture = particle.UseAnkhIcon ? ankhTexture : sparkleTexture;
                    float particleScale = particle.UseAnkhIcon ? particle.Scale * 0.67f : particle.Scale;
                    
                    spriteBatch.Draw(
                        currentTexture,
                        particle.Position,
                        currentTexture.Frame(),
                        color,
                        particle.Rotation,
                        currentTexture.Size() / 2,
                        particleScale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            
        }

        private static float panelHoverIntensity;
        private static Effect panelShader;
        
        // Panel constants
        private const int BorderThickness = 2;
        
        // Loads panel gradient shader on mod initialization
        public override void Load()
        {
            base.Load();
            if (Main.netMode != Terraria.ID.NetmodeID.Server && ModLoader.HasMod("Daybreak"))
            {
                panelShader = ModContent.Request<Effect>("InfernalEclipseAPI/Assets/Panel/PanelGradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }
        
        // Cleans up shader reference on mod unload
        public override void Unload()
        {
            panelShader = null;
            base.Unload();
        }

        // Replaces default icon with custom animated icon
        public override UIImage ModifyModIcon(UIPanel element, UIImage modIcon, ref int modIconAdjust)
        {
            return new AnimatedIcon(TextureAssets.MagicPixel.Value)
            {
                Left = modIcon.Left,
                Top = modIcon.Top,
                Width = modIcon.Width,
                Height = modIcon.Height,
            };
        }

        // Replaces default text with animated gradient text
        public override UIText ModifyModName(UIPanel element, UIText modName)
        {
            return new AnimatedModName(modName.Text)
            {
                Left = modName.Left,
                Top = modName.Top,
            };
        }

        // Custom panel drawing with shader-based gradient background
        public override bool PreDrawPanel(UIPanel element, SpriteBatch sb, ref bool drawDivider)
        {
            var dims = element.GetDimensions();
            
            panelHoverIntensity = MathHelper.Lerp(panelHoverIntensity, element.IsMouseHovering ? 1f : 0f, 0.15f);

            var panelRect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);

            sb.End(out var ss);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, 
                DepthStencilState.None, ss.RasterizerState, panelShader, Main.UIScaleMatrix);
            
            panelShader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
            panelShader.Parameters["uHoverIntensity"]?.SetValue(panelHoverIntensity);
            
            sb.Draw(TextureAssets.MagicPixel.Value, panelRect, Color.White);
            
            sb.Restart(ss);
            
            var borderColor = Color.Lerp(new Color(139, 69, 19), new Color(255, 140, 0), panelHoverIntensity * 0.5f);
            
            // Draw borders using arrays for cleaner code
            var borderRects = new Rectangle[]
            {
                new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, BorderThickness), // Top
                new Rectangle((int)dims.X, (int)(dims.Y + dims.Height - BorderThickness), (int)dims.Width, BorderThickness), // Bottom
                new Rectangle((int)dims.X, (int)dims.Y, BorderThickness, (int)dims.Height), // Left
                new Rectangle((int)(dims.X + dims.Width - BorderThickness), (int)dims.Y, BorderThickness, (int)dims.Height) // Right
            };
            
            foreach (var rect in borderRects)
            {
                sb.Draw(TextureAssets.MagicPixel.Value, rect, borderColor);
            }
            
            return false;
        }

        // Custom enabled/disabled text colors
        public override Color ModifyEnabledTextColor(bool enabled, Color color)
        {
            if (enabled)
            {
                return new Color(255, 140, 0);
            }
            else
            {
                return new Color(255, 180, 100);
            }
        }

        // Disable the panel behind enabled/disabled text completely
        public override bool PreDrawModStateTextPanel(UIElement self, bool enabled)
        {
            return false;
        }

        // Add pulsing effect when hovering
        public override void PostSetHoverColors(UIPanel element, bool hovered)
        {
            if (hovered)
            {
                element.BackgroundColor = Color.Lerp(element.BackgroundColor, Color.White, 0.1f);
            }
        }

    }
}