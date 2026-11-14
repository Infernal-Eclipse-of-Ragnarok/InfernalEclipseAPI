using CalamityMod.NPCs.SlimeGod;
using InfernalEclipseAPI.Core.Utils;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class WeirdGel : AcceleratingGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingCrimulanGlob>().Texture;
        public float degrees = 0.5f;

        int homeTimer = 0;
        public virtual float speedCap => 17;
        public virtual int activationTime => 10000;
        public override void AI()
        {
            base.AI();
            float modifiedDegress = Projectile.ai[2] == 0 ? degrees : Projectile.ai[2];

            if (Projectile.velocity.Length() < speedCap)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(modifiedDegress) * (Projectile.ai[0] == 1 ? 1 : -1));
            }

            if (Projectile.velocity.Length() > speedCap)
            {
                Projectile.velocity /= 1.0138f;
            }

            homeTimer++;
            float maxRange = 550;
            if (NPC.FindFirstNPC(ModContent.NPCType<SlimeGodCore>()) == -1)
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[Main.npc[NPC.FindFirstNPC(ModContent.NPCType<SlimeGodCore>())].target];

            if (homeTimer >= activationTime - 30)
            {
                for (int i = 0; i < 100; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + Vector2.One.RotatedBy(ToRadians((360 / 100) * i)) * maxRange / 2, 0, 0, DustID.TintableDust, 0, 0, 150, isCrimson ? Color.Crimson : Color.Lavender);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (homeTimer >= activationTime && Projectile.WithinRange(target.Center, maxRange))
            {
                InfernalUtilities.FloatAbovePlayer(Vector2.Zero, Projectile, 200, 5, target.Center, 0, 0, false, false, 0);
            }
        }
    }
}
