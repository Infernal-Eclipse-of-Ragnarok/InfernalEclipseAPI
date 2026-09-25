using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using CalamityMod;
using InfernalEclipseAPI.Core.Systems.Hooks.ILItemChanges.CalamityItemHooks;
using InfernumMode.Content.Rarities.InfernumRarities;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;
using ReLogic.Content;

namespace InfernalEclipseAPI.Content.Items.Weapons.Catlight
{
    public class Catlight : ModItem
    {
        internal const string Path = "InfernalEclipseAPI/Content/Items/Weapons/Catlight/";

        private static readonly int[] ProgressionDamage =
        [
            15, 20, 30, 45, 50, 55, 60, 75, 90, 120, 200, 7400, 74000
        ];

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ModContent.RarityType<InfernumRedSparkRarity>();
            Item.DamageType = CatlightDamage.Instance;
            Item.damage = 740;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<CatlightDeathray>();
        }

        private static int GetProgression()
        {
            if (DownedBossSystem.downedBossRush)
                return 12;

            if (DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas)
            {
                if (!ModLoader.HasMod("CalamityHunt") || StormMaidenConditionOverride.DownedGoozma())
                    return 11;
            }

            if (NPC.downedMoonlord) return 10;
            if (NPC.downedAncientCultist) return 9;
            if (NPC.downedGolemBoss) return 8;
            if (NPC.downedPlantBoss) return 7;
            if (NPC.downedMechBoss3) return 6;
            if (NPC.downedMechBoss2) return 5;
            if (NPC.downedMechBoss1) return 4;
            if (Main.hardMode) return 3;
            if (NPC.downedBoss3) return 2;
            if (NPC.downedBoss2) return 1;

            return 0;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            int baseDamage = ProgressionDamage[GetProgression()];
            damage.Flat = baseDamage * 0.75f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projectileType = ModContent.ProjectileType<CatlightDeathrayJRR>();

            if (NPC.downedMoonlord)
                projectileType = ModContent.ProjectileType<CatlightDeathray>();
            else if (Main.hardMode)
                projectileType = ModContent.ProjectileType<CatlightDeathrayJR>();

            Projectile.NewProjectile(source, position, velocity, projectileType, damage, knockback, player.whoAmI);
            SoundEngine.PlaySound(new SoundStyle(Path + "CatlightExplosion"), player.Center);

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = Main.keyState.IsKeyDown(Keys.LeftShift)
                ? $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.cat"))}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Contributor")}"
                : Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Contributor");

            TooltipLine line = new(Mod, "DedicatedItem", text)
            {
                OverrideColor = new Color(50, 205, 50)
            };

            tooltips.Add(line);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient(ItemID.Catfish)
                .AddIngredient(ItemID.FallenStar, 3996)
                .AddTile(TileID.Bookcases)
                .Register();
        }

        public override bool WeaponPrefix() => false;
        public override bool MeleePrefix() => false;
        public override bool MagicPrefix() => false;
        public override bool RangedPrefix() => false;
    }

    public class CatlightDamage : DamageClass
    {
        internal static CatlightDamage Instance;
        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) => StatInheritanceData.None;
        public override bool GetEffectInheritance(DamageClass damageClass) => true;
    }

    public struct VertexInfo : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0));

        public Vector2 Position;

        public Color Color;

        public Vector3 TexCoord;

        public VertexDeclaration VertexDeclaration => VertexInfo._vertexDeclaration;

        public VertexInfo(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
    }

    #region Projectiles
    public abstract class CatlightDeathrayBase : ModProjectile
    {
        private const int LaserPointCount = 140;
        private const float LaserSpacing = 15f;
        private const float LaserLength = 2400f;
        private const float LaserCollisionWidth = 200f;

        private readonly Vector2[] lasersTop = new Vector2[LaserPointCount];
        private readonly Vector2[] lasersBot = new Vector2[LaserPointCount];
        private readonly VertexInfo[] vertices = new VertexInfo[LaserPointCount * 2];

        private static readonly float[] CachedX = new float[LaserPointCount];
        private static readonly float[] CachedPow = new float[LaserPointCount];

        private static Asset<Texture2D> FadedGlowStreakTexture;
        private static Asset<Texture2D> DevInnerStreakTexture;

        private static readonly BlendState CatlightBlendState = new()
        {
            AlphaBlendFunction = BlendState.AlphaBlend.AlphaBlendFunction,
            AlphaDestinationBlend = BlendState.AlphaBlend.AlphaDestinationBlend,
            AlphaSourceBlend = BlendState.AlphaBlend.AlphaSourceBlend,
            ColorBlendFunction = BlendFunction.Add,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorSourceBlend = BlendState.Additive.ColorSourceBlend,
            ColorWriteChannels = ColorWriteChannels.All,
            ColorWriteChannels1 = ColorWriteChannels.All,
            ColorWriteChannels2 = ColorWriteChannels.All,
            ColorWriteChannels3 = ColorWriteChannels.All,
            BlendFactor = Color.White,
            MultiSampleMask = -1
        };

        protected virtual int NPCImmunityTime => 0;
        protected virtual bool DrawInnerStreak => true;
        protected virtual Color OuterTopColor => Color.Red;
        protected virtual Color OuterBottomColor => Color.DarkRed;
        protected virtual Color InnerTopColor => new(Main.DiscoR, 0, 0);
        protected virtual Color InnerBottomColor => new(255 - Main.DiscoR, 0, 0);

        public override string Texture => "InfernalEclipseAPI/Assets/Textures/Backgrounds/BlankPixel";

        public override void Load()
        {
            FadedGlowStreakTexture ??= ModContent.Request<Texture2D>(Catlight.Path + "FadedGlowStreak");
            DevInnerStreakTexture ??= ModContent.Request<Texture2D>(Catlight.Path + "DevInnerStreak");

            for (int i = 0; i < LaserPointCount; i++)
            {
                float x = i * LaserSpacing;
                CachedX[i] = x;
                CachedPow[i] = Pow(0.1f * x, 0.45f);
            }
        }

        public override void Unload()
        {
            FadedGlowStreakTexture = null;
            DevInnerStreakTexture = null;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 550;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = CatlightDamage.Instance;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[0] <= 4.8f || Projectile.timeLeft < 15)
                return false;

            float point = 0f;
            Vector2 direction = Projectile.rotation.ToRotationVector2();

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + direction * LaserLength,
                LaserCollisionWidth,
                ref point
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = NPCImmunityTime;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 direction = Projectile.rotation.ToRotationVector2();
            Projectile.velocity = direction * 6f;

            UpdateLaserShape();

            if (Projectile.localAI[0] <= 8f && player.channel)
                Projectile.localAI[0] += 0.5f;

            bool shouldFade = !player.channel || Projectile.localAI[2] == 1f;

            if (Projectile.ai[2] % 5f == 0f && !player.CheckMana(player.HeldItem, 8, pay: true))
                Projectile.localAI[2] = 1f;

            if (shouldFade)
            {
                FadeOut(player, direction);
                return;
            }

            if (player.channel)
                Channel(player);
        }

        private void UpdateLaserShape()
        {
            float strength = 29f * (0.08f * Projectile.localAI[0]);

            for (int i = 0; i < LaserPointCount; i++)
            {
                float x = CachedX[i];
                float y = strength * CachedPow[i];

                lasersTop[i] = new Vector2(x, y);
                lasersBot[i] = new Vector2(x, -y);
            }
        }

        private void FadeOut(Player player, Vector2 direction)
        {
            Projectile.Center = player.Center + direction * 40f;
            player.itemTime = player.itemAnimation = 2;

            Projectile.localAI[0] -= 1f;

            if (Projectile.localAI[0] < 0f)
                Projectile.Kill();
        }

        private void Channel(Player player)
        {
            player.itemTime = player.itemAnimation = 2;
            Projectile.timeLeft = 55;

            Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.MouseWorld), 0.06f);

            Vector2 direction = Projectile.rotation.ToRotationVector2();

            Projectile.Center = player.Center + direction * 40f;
            Projectile.velocity = direction;
            Projectile.ai[2]++;

            player.itemRotation = direction.ToRotation();

            if (player.direction != 1)
                player.itemRotation += Pi;

            player.heldProj = Projectile.whoAmI;
        }

        private static Vector2 Rotate(Vector2 vector, float cos, float sin)
        {
            return new Vector2(
                vector.X * cos - vector.Y * sin,
                vector.X * sin + vector.Y * cos
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            float rotation = Projectile.rotation;
            float cos = Cos(rotation);
            float sin = Sin(rotation);
            float moveFactor = Main.GlobalTimeWrappedHourly / 0.7f;
            Vector2 origin = Projectile.Center - Main.screenPosition;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, CatlightBlendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            graphicsDevice.Textures[0] = FadedGlowStreakTexture.Value;

            int vertexCount = 0;

            for (int i = 0; i < LaserPointCount; i++)
            {
                float progress = 1f - i / 70f;

                vertices[vertexCount++] = new VertexInfo(
                    Rotate(lasersTop[i], cos, sin) + origin,
                    new Vector3(0f, progress + moveFactor, progress),
                    OuterTopColor
                );

                vertices[vertexCount++] = new VertexInfo(
                    Rotate(lasersBot[i], cos, sin) + origin,
                    new Vector3(1f, progress + moveFactor, progress),
                    OuterBottomColor
                );
            }

            if (vertexCount >= 3)
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertexCount - 2);

            if (DrawInnerStreak)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                graphicsDevice.Textures[0] = DevInnerStreakTexture.Value;

                for (int j = 0; j < 3; j += 2)
                {
                    vertexCount = 0;

                    for (int i = 0; i < LaserPointCount; i += 2)
                    {
                        float fac = 0.13f * i;
                        float sinFactor = 0.545f * Sin((1.1f - 0.03f * fac) * fac - Main.GlobalTimeWrappedHourly * 9.15f) * (j - 1);
                        float progress = 1f - i / 70f;

                        Vector2 top = new(lasersTop[i].X, lasersTop[i].Y * sinFactor + 84f);
                        Vector2 bot = new(lasersTop[i].X, lasersTop[i].Y * sinFactor - 84f);

                        vertices[vertexCount++] = new VertexInfo(
                            Rotate(top, cos, sin) + origin,
                            new Vector3(progress + moveFactor, 0f, progress),
                            InnerTopColor
                        );

                        vertices[vertexCount++] = new VertexInfo(
                            Rotate(bot, cos, sin) + origin,
                            new Vector3(progress + moveFactor, 1f, progress),
                            InnerBottomColor
                        );
                    }

                    if (vertexCount >= 3)
                        graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertexCount - 2);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
    public class CatlightDeathray : CatlightDeathrayBase
    {
        protected override int NPCImmunityTime => 0;
        protected override Color OuterTopColor => Color.DarkRed;
        protected override Color OuterBottomColor => Color.White;
        protected override Color InnerTopColor => new(Main.DiscoR, 0, 0);
        protected override Color InnerBottomColor => new(255 - Main.DiscoR, 0, 0);
    }

    //pre ml
    public class CatlightDeathrayJR : CatlightDeathrayBase
    {
        protected override int NPCImmunityTime => 5;
        protected override Color OuterTopColor => Color.Red;
        protected override Color OuterBottomColor => Color.DarkRed;
        protected override Color InnerTopColor => Color.MediumVioletRed;
        protected override Color InnerBottomColor => Color.OrangeRed;
    }

    //pre hm
    public class CatlightDeathrayJRR : CatlightDeathrayBase
    {
        protected override int NPCImmunityTime => 10;
        protected override bool DrawInnerStreak => false;
        protected override Color OuterTopColor => Color.Red;
        protected override Color OuterBottomColor => Color.DarkRed;
    }
    #endregion
}
