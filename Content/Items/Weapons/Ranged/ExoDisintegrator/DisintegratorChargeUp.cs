using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Luminance.Assets;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Weapons.Ranged.ExoDisintegrator
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class DisintegratorChargeUp : ModProjectile
    {
        private bool hasPlayedChargeSound;
        private bool hasPlayedFireSound;
        private static SlotId activeLoopSoundId;
        private static SlotId activeChargeSoundId;
        private static SlotId activeFireSoundId;

        private Player Owner => Main.player[Projectile.owner];

        public ref float ChargeTime => ref Projectile.ai[0];

        public ref float MaxChargeTime => ref Projectile.ai[1];

        private ref float IsFiring => ref Projectile.localAI[0];
        private ref float BeamID => ref Projectile.localAI[1];
        private ref float Time => ref Projectile.localAI[0];

        public float ConvergeTime => MaxChargeTime * 0.5f;

        public float GlowInterpolant { get; set; }

        public override string Texture => "Luminance/Assets/InvisiblePixel";

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
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return (double)Time <= 10.0 ? new bool?(false) : base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            if (!Owner.active || Owner.dead || !Owner.channel)
            {
                ActiveSound activeSound1;
                if (SoundEngine.TryGetActiveSound(activeLoopSoundId, out activeSound1))
                    activeSound1.Stop();
                ActiveSound activeSound2;
                if (SoundEngine.TryGetActiveSound(activeChargeSoundId, out activeSound2))
                    activeSound2.Stop();
                ActiveSound activeSound3;
                if (SoundEngine.TryGetActiveSound(activeFireSoundId, out activeSound3))
                    activeSound3.Stop();
                this.Projectile.Kill();
            }
            else
            {
                Vector2 aim = Main.MouseWorld - Owner.Center;
                if (aim == Vector2.Zero)
                    aim = Vector2.UnitX * Owner.direction;
                aim.Normalize();

                Projectile.Center = Owner.Center + aim * 120f;
                Projectile.rotation = aim.ToRotation();
                Projectile.velocity = aim;

                if (IsFiring == 0f)
                {
                    ChargeTime++;

                    // Start charge SFX once.
                    if (ChargeTime == 2 && !hasPlayedChargeSound)
                    {
                        hasPlayedChargeSound = true;
                        var charge = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/ExoelectricDisintegrationRayChargeUp");
                        activeChargeSoundId = SoundEngine.PlaySound(charge);
                    }

                    // Charging dust ribbon.
                    if (ChargeTime >= ConvergeTime * 0.3f)
                    {
                        float lerp = Utils.GetLerpValue(0f, MaxChargeTime, ChargeTime, clamped: false);

                        for (int i = 0; i < 8; i++)
                        {
                            // Spawn along a distant arc then travel inward.
                            Vector2 src =
                                Projectile.Center
                              + aim * Main.rand.NextFloat(100f, lerp * 600f + 540f)
                              + aim.RotatedByRandom(1.53f) * Main.rand.NextFloat(200f, 400f);

                            Vector2 vel = (Projectile.Center - src) * 0.085f;

                            Dust d = Dust.NewDustPerfect(src, DustID.PortalBoltTrail);
                            d.velocity = vel;
                            d.color = Color.Lerp(Color.Crimson, Color.Orange, Main.rand.NextFloat(0.65f));
                            if (Main.rand.NextBool(6))
                                d.color = Color.Wheat;
                            d.scale = Main.rand.NextFloat(0.7f, 1.4f);
                            d.noGravity = true;
                            d.fadeIn = 0.8f;
                        }
                    }

                    GlowInterpolant = MathHelper.Clamp(GlowInterpolant + 0.12f, 0f, 1f);

                    // Transition to firing.
                    if (ChargeTime >= MaxChargeTime)
                    {
                        IsFiring = 1f;

                        if (!hasPlayedFireSound)
                        {
                            hasPlayedFireSound = true;

                            var fire = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/ExoelectricDisintegrationRayFire");
                            activeFireSoundId = SoundEngine.PlaySound(fire, Owner.Center);

                            if (SoundEngine.TryGetActiveSound(activeFireSoundId, out var fireActive) && fireActive != null)
                                fireActive.Volume *= 2.1f;

                            if (Main.myPlayer == Projectile.owner)
                            {
                                if (SoundEngine.TryGetActiveSound(activeLoopSoundId, out var loopExisting))
                                    loopExisting.Stop();

                                var loop = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/CosmicLaserLoop")
                                {
                                    Volume = 1.4f,
                                    Pitch = -0.2f,
                                    IsLooped = true,
                                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                                };

                                activeLoopSoundId = SoundEngine.PlaySound(loop, Owner.Center);
                                if (SoundEngine.TryGetActiveSound(activeLoopSoundId, out var loopActive))
                                    loopActive.Position = Owner.Center;
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
                        Vector2 spawn = Owner.Center + aim * 165f;
                        int id = Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            spawn,
                            aim,
                            ModContent.ProjectileType<DisintegratorBeam>(),
                            Projectile.damage,
                            Projectile.knockBack,
                            Projectile.owner,
                            0f, 99999f, 0f
                        );

                        BeamID = id + 1f; // store as index+1 so 0 means "none"
                    }
                    else
                    {
                        int idx = (int)BeamID - 1;
                        if (idx < 0 || idx >= Main.maxProjectiles)
                            return;

                        Projectile beam = Main.projectile[idx];
                        if (beam.active && beam.type == ModContent.ProjectileType<DisintegratorBeam>())
                            beam.velocity = aim;
                        else
                            BeamID = 0f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if ((double)this.ChargeTime <= 0.0)
                return false;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
            this.DrawBloom();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void DrawBloom()
        {
            Color c1 = Color.Lerp(Color.Crimson, Color.Orange, 0.5f) * Projectile.Opacity * 0.85f;
            Color c2 = new Color(255, 40, 55) * Projectile.Opacity * 1.2f;

            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D bloomSmall = MiscTexturesRegistry.BloomCircleSmall.Value;
            Texture2D bloomFlare = MiscTexturesRegistry.BloomFlare.Value;

            // Small bloom (note: origin at 20% of texture size per original)
            Main.spriteBatch.Draw(
                bloomSmall,
                pos,
                null,
                c1,
                0f,
                bloomSmall.Size() * 0.2f,
                3f * Projectile.scale,
                SpriteEffects.None,
                0f
            );

            float t = Main.GlobalTimeWrappedHourly * -0.5f;
            Vector2 flareOrigin = bloomFlare.Size() * 0.5f;
            float flareScale = 1.25f * Projectile.scale;

            // Flare passes
            Main.spriteBatch.Draw(bloomFlare, pos, null, c2, t, flareOrigin, flareScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloomFlare, pos, null, c2, t * -0.7f, flareOrigin, flareScale, SpriteEffects.None, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            ActiveSound activeSound1;
            if (SoundEngine.TryGetActiveSound(activeLoopSoundId, out activeSound1))
                activeSound1.Stop();
            ActiveSound activeSound2;
            if (SoundEngine.TryGetActiveSound(activeChargeSoundId, out activeSound2))
                activeSound2.Stop();
            ActiveSound activeSound3;
            if (SoundEngine.TryGetActiveSound(activeFireSoundId, out activeSound3))
                activeSound3.Stop();
            if (Main.myPlayer != Projectile.owner || (double)BeamID <= 0.0)
                return;
            int index = (int)this.BeamID - 1;
            if (index < 0 || index >= Main.maxProjectiles)
                return;
            Projectile projectile = Main.projectile[index];
            if (!projectile.active || projectile.type != ModContent.ProjectileType<DisintegratorBeam>())
                return;
            projectile.Kill();
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
