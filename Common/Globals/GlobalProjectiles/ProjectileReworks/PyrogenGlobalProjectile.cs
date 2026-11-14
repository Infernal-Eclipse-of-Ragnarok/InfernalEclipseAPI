using CalamityMod.Buffs.DamageOverTime;
using Clamity.Content.Bosses.Pyrogen.NPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalProjectiles.ProjectileReworks
{
    [JITWhenModsEnabled(InfernalCrossmod.Clamity.Name)]
    [ExtendsFromMod(InfernalCrossmod.Clamity.Name)]
    public class PyrogenGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool applyDebuff;

        private static bool IsClamityProj(Projectile proj, string name)
        {
            return InfernalCrossmod.Clamity.Mod.Find<ModProjectile>(name)?.Type == proj.type;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<PyrogenBoss>()))
                return;

            applyDebuff = false;
            int intendedDamage;

            if (IsClamityProj(projectile, "FireBarrage") || IsClamityProj(projectile, "FireBarrageHoming"))
            {
                intendedDamage = 140;
                applyDebuff = true;
            }
            else if (IsClamityProj(projectile, "Fireblast"))
            {
                intendedDamage = 155;
                applyDebuff = true;
            }
            else if (IsClamityProj(projectile, "FireBomb") || IsClamityProj(projectile, "Firethrower"))
            {
                intendedDamage = 120;
                applyDebuff = true;
            }
            else if (IsClamityProj(projectile, "FireBombExplosion"))
            {
                intendedDamage = 200;
                applyDebuff = true;
            }
            else
            {
                return;
            }

            modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) =>
            {
                //pyrogens damage is so broken we have to manually do terraria's damage calculation....
                info.Damage = (intendedDamage - target.statDefense * (Main.masterMode ? 1f : Main.expertMode ? 0.75f : 0.5f));
            };
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (applyDebuff)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 60);
        }
    }
}
