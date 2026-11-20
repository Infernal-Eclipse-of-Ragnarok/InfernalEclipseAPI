using InfernumMode;
using InfernumMode.Assets.ExtraTextures;
using InfernumMode.Common.BaseEntities;
using InfernumMode.Content.BehaviorOverrides.BossAIs.AquaticScourge;
using InfernumMode.Content.BehaviorOverrides.BossAIs.EyeOfCthulhu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SlimeGodPR
{
    public class EyeChanges: GlobalNPC
    {
        public bool Active => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCID.EyeofCthulhu;
        }
        public static float Phase4LifeRatio => 0.1f;
        public override bool InstancePerEntity => true;
        bool hasEnteredFinalPhase = false;
        public enum EoCAttackType
        {
            HoverCharge,
            ChargingServants,
            HorizontalBloodCharge,
            TeethSpit,
            SpinDash,
            BloodShots,
            TrailDash,
            FinalPhaseCharge
        }
        public static EoCAttackType[] Phase1AttackPattern =>
        [
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.ChargingServants,
            EoCAttackType.HoverCharge,
            EoCAttackType.HorizontalBloodCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.ChargingServants,
        ];

        public static EoCAttackType[] Phase2AttackPattern =>
        [
            EoCAttackType.TrailDash,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.ChargingServants,
            EoCAttackType.SpinDash,
            EoCAttackType.TrailDash,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.TeethSpit,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.SpinDash,
            EoCAttackType.TrailDash,
            EoCAttackType.HorizontalBloodCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.TeethSpit,
            EoCAttackType.SpinDash,
        ];

        public static EoCAttackType[] Phase3AttackPattern =>
        [
            EoCAttackType.TrailDash,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.BloodShots,
            EoCAttackType.ChargingServants,
            EoCAttackType.SpinDash,
            EoCAttackType.TrailDash,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.BloodShots,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.TrailDash,
            EoCAttackType.HorizontalBloodCharge,
            EoCAttackType.BloodShots,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.TeethSpit,
            EoCAttackType.SpinDash,
        ];

        public static EoCAttackType[] Phase4AttackPattern =>
        [
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.TrailDash,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.HorizontalBloodCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.HoverCharge,
            EoCAttackType.SpinDash,
        ];

        public static int TrailDamage => 80;

        public static void SelectNextAttack(NPC npc)
        {
            npc.ai[3]++;

            EoCAttackType[] patternToUse = Phase1AttackPattern;
            if (npc.life < npc.lifeMax * EyeOfCthulhuBehaviorOverride.Phase2LifeRatio)
                patternToUse = Phase2AttackPattern;
            if (npc.life < npc.lifeMax * EyeOfCthulhuBehaviorOverride.Phase3LifeRatio)
                patternToUse = Phase3AttackPattern;
            if (npc.life < npc.lifeMax * Phase4LifeRatio)
                patternToUse = Phase4AttackPattern;
            EoCAttackType nextAttackType = patternToUse[(int)(npc.ai[3] % patternToUse.Length)];

            // Select the next AI state.
            npc.ai[1] = (int)nextAttackType;

            // Reset the attack timer.
            npc.ai[2] = 0f;

            // And reset the misc ai slots.
            for (int i = 0; i < 5; i++)
                npc.Infernum().ExtraAI[i] = 0f;

            npc.TargetClosest();
            npc.netUpdate = true;
        }

        public static void DoBehaviour_TrailDash(NPC npc)
        {
            ref float[] ExtraAi = ref npc.Infernum().ExtraAI;
            ref float attackTimer = ref npc.ai[2];
            ref float attackState = ref ExtraAi[0];

            Player target = Main.player[npc.target];

            int directionTime = npc.life < npc.lifeMax * Phase4LifeRatio ? 90 : 120;
            int telegraphTime = npc.life < npc.lifeMax * Phase4LifeRatio ? 30 : 60;

            float hoverSpeed = 20;
            float hoverAcceleration = 0.25f;

            float predictionStrength = 20;

            int dashTime = npc.life < npc.lifeMax * Phase4LifeRatio ? 80 : 100;
            float dashSpeed = 20;

            switch (attackState)
            {
                case 0:
                    Vector2 destination = target.Center - Vector2.UnitY * 185f;
                    npc.SimpleFlyMovement(npc.SafeDirectionTo(destination) * hoverSpeed, hoverAcceleration);
                    npc.rotation = npc.AngleTo(target.Center) + MathHelper.ToRadians(270);

                    if (attackTimer >= directionTime)
                    {
                        attackState++;
                        attackTimer = 0;
                    }
                    break;

                case 1:
                    npc.velocity *= 0.95f;
                    npc.rotation = npc.AngleTo(target.Center + target.velocity * predictionStrength) + MathHelper.ToRadians(270);

                    if (attackTimer >= telegraphTime)
                    {
                        attackState++;
                        attackTimer = 0;
                    }
                    break;
                case 2:
                    npc.velocity = new Vector2(0, dashSpeed).RotatedBy(npc.rotation);
                    SoundEngine.PlaySound(SoundID.Roar with { Pitch = -0.3f }, npc.position);

                    attackState++;
                    attackTimer = 0;
                    break;
                case 3:
                    if (attackTimer % 4 == 0)
                    {
                        Utilities.NewProjectileBetter(npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeBloodTrail>(), TrailDamage, 0f);
                    }
                    if (attackTimer >= dashTime)
                    {
                        SelectNextAttack(npc);
                    }
                    break;
            }
        }

        public override void PostAI(NPC npc)
        {
            float speedMod = npc.life <= npc.lifeMax * Phase4LifeRatio ? 0.8f : 0.15f;
            //this makes the eye faster and thus harder
            npc.position += npc.velocity * speedMod;

            if (!hasEnteredFinalPhase && npc.life <= npc.lifeMax * Phase4LifeRatio)
            {
                SelectNextAttack(npc);
                hasEnteredFinalPhase = true;
                SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.5f }, npc.position);
                Utilities.CreateShockwave(npc.Center, 2, 8, 75, false);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Utilities.NewProjectileBetter(npc.Center + Vector2.UnitY * 8f, Vector2.Zero, ModContent.ProjectileType<EyeWave>(), 0, 0f);
            }

            switch (npc.ai[1])
            {
                case (int)EoCAttackType.TrailDash:
                    DoBehaviour_TrailDash(npc);
                    break;
            }
        }
    }
    internal sealed class EyeSelectAttackHookSystem : ModSystem
    {
        MethodInfo? method = typeof(EyeOfCthulhuBehaviorOverride).GetMethod(nameof(EyeOfCthulhuBehaviorOverride.SelectNextAttack), Luminance.Common.Utilities.Utilities.UniversalBindingFlags);

        private static Hook? SelectAttackHook;

        private delegate void Orig_SelectAttackHook(
            NPC npc
        );

        private delegate void Hook_SelectAttackHook(
            Orig_SelectAttackHook orig,
            NPC npc
        );

        public override void OnModLoad()
        {
            if (method != null)
            {
                SelectAttackHook = new Hook(method, (Hook_SelectAttackHook)SelectAttackHook_Detour);
                SelectAttackHook.Apply();
            }
        }

        public override void Unload()
        {
            SelectAttackHook?.Dispose();
            SelectAttackHook = null;
        }

        private static void SelectAttackHook_Detour(Orig_SelectAttackHook orig, NPC npc)
        {
            EyeChanges.SelectNextAttack(npc);
        }
    }


    public class EyeBloodTrail: SulphuricGas
    {
        public static int NewLifeTime => 600;
        public override void SetDefaults()
        {
            Projectile.timeLeft = NewLifeTime;
        }
        public override void AI()
        {
            if (IdealScale == 0f)
            {
                IdealScale = Main.rand.NextFloat(4f, 5.5f);
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.netUpdate = true;
            }

            // Grow in scale.
            float idealScale = IdealScale + Utils.Remap(Projectile.timeLeft, 60f, 0f, 0f, 10f);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, idealScale, 0.064f);

            if (Main.netMode != NetmodeID.Server)
            {
                // Calculate light power. This checks below the position of the fog to check if this fog is underground.
                // Without this, it may render over the fullblack that the game renders for obscured tiles.
                float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / MathF.Sqrt(3f);
                if (Framing.GetTileSafely((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16).LiquidAmount >= 25)
                    lightPowerBelow = 1f;

                LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
            }

            Projectile.Opacity = Utils.GetLerpValue(NewLifeTime, NewLifeTime - 20f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 40f, Projectile.timeLeft, true) * 0.675f;
            Projectile.rotation += Projectile.velocity.X * 0.002f;
            Projectile.velocity *= 0.985f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //COPIED FROM SULPHURIC GAS!
            Vector2 screenArea = new(Main.screenWidth, Main.screenHeight);
            Rectangle screenRectangle = Utils.CenteredRectangle(Main.screenPosition + screenArea * 0.5f, screenArea * 1.33f);

            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 20;
            if (!Projectile.Hitbox.Intersects(screenRectangle))
                return false;

            // Decide which gas texture to use.
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.identity % 2 == 1)
                texture = InfernumTextureRegistry.Cloud2.Value;

            // Calculate drawing variables for the mist.
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity;

            int b = 160 + (int)(Math.Sin(MathHelper.Pi * Projectile.identity / 8f + Main.GlobalTimeWrappedHourly * 10f) * 80f);
            Color drawColor = new Color(255, 0, 0) * opacity;
            Vector2 scale = Vector2.One * 50f / texture.Size() * Projectile.scale * 1.35f;
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.ResetBlendState();
            return false;
        }
    }
}


public class EyeWave : BaseWaveExplosionProjectile
{
    public override int Lifetime => 80;

    public override float MaxRadius => 1000f;

    public override float RadiusExpandRateInterpolant => 0.15f;

    public override string Texture => "InfernumMode/Assets/ExtraTextures/GreyscaleObjects/Gleam";

    public override float DetermineScreenShakePower(float lifetimeCompletionRatio, float distanceFromPlayer)
    {
        float baseShakePower = MathHelper.Lerp(1f, 5f, MathF.Sin(MathHelper.Pi * lifetimeCompletionRatio));
        return baseShakePower * Utils.GetLerpValue(2200f, 1050f, distanceFromPlayer, true);
    }

    public override Color DetermineExplosionColor(float lifetimeCompletionRatio)
    {
        return Color.Red;
    }
}
