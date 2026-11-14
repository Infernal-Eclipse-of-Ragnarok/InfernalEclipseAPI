using CalamityMod.NPCs.SlimeGod;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class DashingSlime : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<SplitBigSlime>().Texture;
        int DashTime = 120;
        int DashAmount = 4;
        float PredictionStrength = 5f;
        float DashSpeed = 20f;
        bool isCrimson = false;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0;

            DashAmount = Main.rand.Next(3, 6);
            DashTime = Main.rand.Next(40, 60);
            PredictionStrength = Main.rand.NextFloat(5f, 30f);
            DashSpeed = Main.rand.NextFloat(7f, 9.5f);
            isCrimson = Main.rand.NextBool();

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Projectile.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }
        public int dashes = 0;
        public int explodeTimer = 0;

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);

            if (isCrimson)
            {
                lightColor.G = 100;
                lightColor.B = 100;
            }
            LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);

            return false;
        }
        public override void AI()
        {

            if (Projectile.frameCounter < 5)
                Projectile.frame = 0;
            else if (Projectile.frameCounter >= 5 && Projectile.frameCounter < 10)
                Projectile.frame = 1;
            else if (Projectile.frameCounter >= 10 && Projectile.frameCounter < 15)
                Projectile.frame = 2;
            else if (Projectile.frameCounter >= 15 && Projectile.frameCounter < 20)
                Projectile.frame = 3;
            else
                Projectile.frameCounter = 0;

            Projectile.frameCounter++;


            ref float target = ref Projectile.ai[0];
            ref float spawnTimer = ref Projectile.ai[1];
            ref float attackTimer = ref Projectile.ai[2];

            int spawnTime = 120;

            spawnTimer++;
            if (spawnTimer == 1)
            {
                Projectile.velocity = new Vector2(0, -7);
            }
            else if (spawnTimer <= spawnTime)
            {
                Projectile.velocity *= 0.97f;
                Projectile.Opacity += 0.01f;
            }
            else if (explodeTimer == 0)
            {
                Player targetPlayer = Main.player[(int)target];
                attackTimer++;

                if (isCrimson)
                {
                    float speedModifier = 30;
                    float inertia = 5;
                    float projectileSpeed = 5;
                    Vector2 targetPosition = targetPlayer.Center + (Vector2.One.RotatedBy((Projectile.Center - targetPlayer.Center).ToRotation() - MathHelper.ToRadians(45)) * 200);
                    Vector2 distance = Projectile.Center - targetPosition;
                    float speed = distance.Length() / speedModifier;

                    InfernalUtilities.FloatAbovePlayer(Vector2.Zero, Projectile, speed, inertia, targetPosition, 0, 0, false, false, 0);

                    Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, -0.2f, 0.2f);
                    Projectile.spriteDirection = (Projectile.velocity.X < 0f).ToDirectionInt();

                    if (attackTimer % DashTime == 0)
                    {
                        SoundEngine.PlaySound(SlimeGodCore.ShotSound);
                        InfernalUtilities.ShootProjectile(Projectile.GetSource_FromAI(), targetPlayer.Center + (targetPlayer.velocity * PredictionStrength), projectileSpeed, ModContent.ProjectileType<TelegraphedGel>(), false, 1, 0, 0, Projectile.Center, 0, SGCoreChanges.GlobDamage);
                        dashes++;

                        if (dashes == DashAmount)
                        {
                            explodeTimer++;
                        }
                    }
                }
                else
                {
                    if (attackTimer % DashTime == 0)
                    {

                        Vector2 direction = ((targetPlayer.Center + (targetPlayer.velocity * PredictionStrength)) - Projectile.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.velocity = new Vector2(DashSpeed, DashSpeed) * direction;

                        Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, -0.2f, 0.2f);
                        Projectile.spriteDirection = (Projectile.velocity.X < 0f).ToDirectionInt();

                        SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.position);

                        dashes++;

                        if (dashes == DashAmount)
                        {
                            explodeTimer++;
                        }
                    }
                }

            }
            else
            {
                Projectile.velocity *= 0.97f;
                explodeTimer++;

                int explodeTime = 120;

                if (explodeTimer == explodeTime)
                {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);

                    int projectileCount = 3;
                    int type = ModContent.ProjectileType<TelegraphedSlowedBlob>();
                    float projectileSpeed = 2.5f;
                    InfernalUtilities.ShootProjectile(Projectile.GetSource_Death(), Projectile.Center, projectileSpeed, type, false, projectileCount, Main.rand.Next(0, 361), 360 / projectileCount, Projectile.Center, 0, SGCoreChanges.GlobDamage);

                    for (int i = 0; i < 100; i++)
                    {

                        Color dustColor;
                        if (Main.rand.NextBool())
                        {
                            dustColor = Color.Crimson;
                            dustColor.A = 150;
                        }
                        else
                        {
                            dustColor = Color.Lavender;
                            dustColor.A = 150;
                        }

                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDust, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), Projectile.alpha, dustColor);
                        Main.dust[dust].noGravity = true;
                    }

                    Projectile.Kill();
                }
            }
        }
    }
}
