using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Graphics;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.FiendsmithsRequiem
{
    // A lot of help from CrystalLightning projectile from Calamity Hunt of the Old Gods
    public class FiendsmithsParadise : ModProjectile
    {
        private const string LightningTexturePath = "InfernalEclipseAPI/Content/Items/Weapons/Legendary/FiendsmithsRequiem/FiendsmithsParadise";
        private const string LightningGlowTexturePath = "InfernalEclipseAPI/Content/Items/Weapons/Legendary/FiendsmithsRequiem/FiendsmithsParadiseGlow";
        private const string LightningEffectPath = "InfernalEclipseAPI/Assets/Effects/CrystalLightningEffect";

        private static Asset<Texture2D> lightningTexture;
        private static Asset<Texture2D> lightningGlowTexture;
        private static Asset<Effect> lightningEffect;

        private List<Vector2> points;
        private List<Vector2> offsets;
        private List<Vector2> velocities;

        private Vector2 midPoint;
        private Vector2 endPoint;

        public ref float Time => ref Projectile.localAI[0];

        // Target position supplied by the coffin.
        public ref float TargetX => ref Projectile.ai[0];
        public ref float TargetY => ref Projectile.ai[1];

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void Load()
        {
            if (Main.dedServ)
                return;

            lightningTexture = ModContent.Request<Texture2D>(LightningTexturePath, AssetRequestMode.ImmediateLoad);
            lightningGlowTexture = ModContent.Request<Texture2D>(LightningGlowTexturePath, AssetRequestMode.ImmediateLoad);
            lightningEffect = ModContent.Request<Effect>(LightningEffectPath, AssetRequestMode.ImmediateLoad);
        }

        public override void Unload()
        {
            lightningTexture = null;
            lightningGlowTexture = null;
            lightningEffect = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.DamageType = LegendarySummon.Instance;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.extraUpdates = 3;

            Projectile.timeLeft = 10000;
        }

        public override void OnSpawn(IEntitySource source)
        {
            endPoint = new Vector2(TargetX, TargetY);
            GenerateLightningPath();
        }

        private void GenerateLightningPath()
        {
            Vector2 startPoint = Projectile.Center;

            float distance = Vector2.Distance(startPoint, endPoint);

            Vector2 mid0 = Vector2.Lerp(startPoint, endPoint, 0.3f);
            Vector2 mid1 = Vector2.Lerp(startPoint, endPoint + Main.rand.NextVector2CircularEdge(20f, 400f).RotatedBy(Projectile.AngleTo(endPoint)), 0.5f);
            Vector2 mid2 = Vector2.Lerp(startPoint, endPoint, 0.9f);

            points = new BezierCurve(new List<Vector2>{ startPoint, mid0, mid1, midPoint, mid2 }).GetPoints(Main.rand.Next(1, 4) + (int)(distance * 0.017f));
            points.Add(endPoint);

            offsets = new List<Vector2>();
            velocities = new List<Vector2>();

            for (int i = 0; i < points.Count; i++)
            {
                offsets.Add(Main.rand.NextVector2Circular(5f, 20f).RotatedBy( Projectile.AngleTo(endPoint)) * Utils.GetLerpValue(1f, points.Count * 0.3f, i, true) * Utils.GetLerpValue(points.Count - 1f, points.Count * 0.7f, i, true));
                velocities.Add(Projectile.DirectionTo(endPoint).RotatedByRandom(1.5f) * Main.rand.NextFloat(1f, 3f));
            }
        }

        public override void AI()
        {
            if (Time == 0f)
            {
                endPoint = new Vector2(TargetX, TargetY);

                Vector2 midOff = Main.rand.NextVector2Circular(10f, 350f).RotatedBy(Projectile.AngleTo(endPoint)) * (0.1f + Utils.GetLerpValue(0f, 2000f, Projectile.Distance(endPoint)));

                midPoint = Vector2.Lerp( Projectile.Center, endPoint, 0.7f) + midOff;
            }

            if (Time == 1f)
            {
                GenerateLightningPath();
            }

            if (Time > 1f && points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    float progress = Utils.GetLerpValue(1f, points.Count, i, true) * Utils.GetLerpValue(points.Count - 1f, 0f, i, true) * 3f;

                    offsets[i] += (velocities[i] + Main.rand.NextVector2Circular(4f, 4f)) * progress;
                    velocities[i] *= Main.rand.NextFloat(0.95f, 1f) - i * 0.002f;
                }
            }

            if (Time > 40f)
            {
                Projectile.Kill();
                return;
            }

            Time++;

            Projectile.rotation += 0.2f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (points == null || points.Count < 2)
            {
                return false;
            }

            float collisionPoint = 0f;

            if (Time > 2f && Time < 30f)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 start = points[i] + offsets[i];
                    Vector2 end = points[i + 1] + offsets[i + 1];

                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 30f, ref collisionPoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (points == null || points.Count < 2 || lightningTexture == null || lightningGlowTexture == null || lightningEffect == null)
            {
                return false;
            }

            Texture2D texture = lightningTexture.Value;

            Texture2D bloom = lightningGlowTexture.Value;

            VertexStrip strip = new VertexStrip();

            Color StripColor(float progress)
            {
                float pulse = (Sin(Time * 0.15f + progress * Pi) + 1f) * 0.5f;

                Color lightningColor = Color.Lerp(Color.Red, Color.Orange, pulse);

                return lightningColor * Utils.GetLerpValue(40f, 10f, Time, true);
            }

            float StripWidth(float progress)
            {
                return (Utils.GetLerpValue(0.1f, 0f, progress, true) * 1.6f + progress * 3f) * 30f * Sqrt(Utils.GetLerpValue(0f, 20f, Time, true));
            }

            Vector2[] positions = new Vector2[points.Count];
            float[] rotations = new float[points.Count];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = points[i] + offsets[i];
            }

            for (int i = 0; i < rotations.Length; i++)
            {
                rotations[i] = Projectile.AngleTo(endPoint);
            }

            strip.PrepareStrip(positions, rotations, StripColor, StripWidth, -Main.screenPosition, positions.Length * 2, true);

            Effect effect = lightningEffect.Value;

            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uTexture"].SetValue(lightningTexture.Value);
            effect.Parameters["uGlow"].SetValue(lightningGlowTexture.Value);
            effect.Parameters["uColor"].SetValue(Vector3.One);
            effect.Parameters["uTime"].SetValue(Time * 0.07f % 1f);
            effect.CurrentTechnique.Passes[0].Apply();
            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            return false;
        }
    }
}
