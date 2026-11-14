using CalamityMod.NPCs.SlimeGod;
using InfernalEclipseAPI.Core.Utils;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class PulledInSlime : DashingSlime
    {
        public override string Texture => ModContent.GetInstance<SplitBigSlime>().Texture;
        bool released = false;
        int attackTimer = 0;
        public override void AI()
        {
            ref float targetIndex = ref Projectile.ai[0];

            NPC slimeGodCore = Main.npc[(int)targetIndex];

            if (!slimeGodCore.active)
            {
                Projectile.Kill();
            }

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

            Projectile.Opacity += 0.05f;

            float speed = 20;
            float inertia = 30;

            if (!released)
            {
                InfernalUtilities.FloatAbovePlayer(Projectile.Center + new Vector2(0, -1), Projectile, speed, inertia, slimeGodCore.Center, 90, 100, false, false, 0);

                Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, -0.2f, 0.2f);
                Projectile.spriteDirection = (Projectile.velocity.X < 0f).ToDirectionInt();
            }
            else
            {
                int explodeTime = 120;
                attackTimer++;
                Projectile.velocity *= 0.98f;
                if (attackTimer == explodeTime)
                {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
                    SoundEngine.PlaySound(SlimeGodCore.ShotSound);

                    int projectileCount = 2;
                    int type = ModContent.ProjectileType<TelegraphedGel>();
                    InfernalUtilities.ShootProjectile(Projectile.GetSource_Death(), Projectile.Center, 2.5f, type, false, projectileCount, Main.rand.Next(0, 361), 360 / projectileCount, Projectile.Center, 0, SGCoreChanges.GlobDamage);

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

            if (slimeGodCore.Infernum().ExtraAI[4] == 1)
            {
                float releaseSpeed = 6;
                Projectile.velocity *= releaseSpeed;
                released = true;
                Projectile.damage = 0;
            }
        }
    }
}
