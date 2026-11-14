using CalamityMod;
using CalamityMod.NPCs.SlimeGod;
using InfernumMode;
using InfernumMode.Assets.Effects;
using InfernumMode.Assets.ExtraTextures;
using InfernumMode.Common.BaseEntities;
using InfernumMode.Common.Graphics.Primitives;
using InfernumMode.Content.BehaviorOverrides.BossAIs.BrimstoneElemental;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class SlimeDeathray : BaseLaserbeamProjectile
    {
        public PrimitiveTrailCopy LaserDrawer
        {
            get;
            set;
        }

        public int OwnerIndex => (int)Projectile.ai[1];
        public override float Lifetime => 1200;
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => Color.MediumPurple;
        public override Texture2D LaserBeginTexture => TextureAssets.Projectile[Projectile.type].Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd", AssetRequestMode.ImmediateLoad).Value;
        public override string Texture => ModContent.GetInstance<BrimstoneDeathray>().Texture;
        public override float MaxLaserLength => 3100f;
        public override float MaxScale => 3f;
        public int TelegraphTime => 120;
        public float Acceleration = 0f;
        public virtual float MaxAcceleration => 0.0125f;
        public virtual float ReversalAccel => 0.00025f;
        public virtual float RegularAccel => 0.0001f;
        public virtual float ProjectileGaps => 175f;
        public virtual int ProjectileReleaseRate => 120;
        public bool Checked = false;
        public virtual Vector2 OwnerEyePosition => Main.npc[OwnerIndex].Center;
        // public override void SetStaticDefaults() => DisplayName.SetDefault("Brimstone Deathray");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public override void AttachToSomething()
        {
            if (!Main.npc[OwnerIndex].active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = OwnerEyePosition;



            if (Projectile.timeLeft <= Lifetime - TelegraphTime)
            {
                if (!Checked)
                {
                    Checked = true;
                    SoundEngine.PlaySound(SoundID.Zombie104 with { Pitch = -1 }, Projectile.position);
                }

                if (Projectile.timeLeft <= Lifetime / 2)
                {
                    Acceleration = MathHelper.Clamp(Acceleration - ReversalAccel, -MaxAcceleration, MaxAcceleration);
                    Projectile.velocity = Projectile.velocity.RotatedBy(Acceleration);
                }
                else
                {
                    Acceleration = MathHelper.Clamp(Acceleration + RegularAccel, 0, MaxAcceleration);
                    Projectile.velocity = Projectile.velocity.RotatedBy(Acceleration);
                }


                if (Projectile.timeLeft % ProjectileReleaseRate == 0)
                {

                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        return;

                    for (float petalOffset = 20f; petalOffset < LaserLength; petalOffset += ProjectileGaps)
                    {
                        Vector2 petalSpawnPosition = OwnerEyePosition + Projectile.velocity * petalOffset;
                        for (int i = -1; i <= 1; i++)
                        {

                            if (i == 0)
                            {
                                continue;
                            }
                            Vector2 petalVelocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * i) * 1f;
                            InfernumMode.Utilities.NewProjectileBetter(petalSpawnPosition, petalVelocity, ModContent.ProjectileType<AcceleratingGel>(), SGCoreChanges.GlobDamage, 0f);

                        }
                    }
                }
            }
            else
            {
                Projectile.velocity = Vector2.One.RotatedBy(Projectile.ai[2] + MathHelper.ToRadians(45) + MathHelper.ToRadians(180));
            }



        }

        public float LaserWidthFunction(float completionRatio) => Projectile.scale * Projectile.width * Utils.GetLerpValue(0.02f, 0.05f, completionRatio, true) * 3f;

        public static Color LaserColorFunction(float completionRatio)
        {
            float colorInterpolant = (float)Math.Sin(Main.GlobalTimeWrappedHourly * -5.2f + completionRatio * 23f) * 0.5f + 0.5f;
            return Color.Lerp(Color.Red, Color.MediumPurple, colorInterpolant);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            LaserDrawer ??= new(LaserWidthFunction, LaserColorFunction, null, true, InfernumEffectsRegistry.GenericLaserVertexShader);
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * LaserLength;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(Projectile.Center, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Select textures to pass to the shader, along with the electricity color.
            Color middleColor = new(252, 220, 178);
            Color middleColor2 = new(255, 162, 162);
            InfernumEffectsRegistry.GenericLaserVertexShader.UseColor(middleColor2 * 2f);
            InfernumEffectsRegistry.GenericLaserVertexShader.SetShaderTexture(InfernumTextureRegistry.StreakFire);

            LaserDrawer.Draw(baseDrawPoints, -Main.screenPosition, 60);
            return false;
        }

        public override bool? CanDamage() => Time > TelegraphTime ? null : false;
    }
}
