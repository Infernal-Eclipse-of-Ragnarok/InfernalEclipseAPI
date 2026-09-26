using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Healing;
using InfernalEclipseAPI.Core.Systems;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Common.GlobalProjectiles
{
    public class InfernalGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool casulJackalBullet = false;
        public bool spawnedBlood = false;

        public override void SetDefaults(Projectile projectile)
        {
            var calamity = ModLoader.GetMod("CalamityMod");

            int pro1Type = calamity.Find<ModProjectile>("AcidGunStream")?.Type ?? -1;
            int pro2Type = calamity.Find<ModProjectile>("WaterLeechProj")?.Type ?? -1;

            if (projectile.type == pro1Type || projectile.type == pro2Type)
            {
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 20;

                projectile.usesIDStaticNPCImmunity = false;
            }

            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
            {
                int pro3Type = catalyst.Find<ModProjectile>("RookOfVengeanceMFireballSpin")?.Type ?? -1;
                int pro4Type = catalyst.Find<ModProjectile>("RookOfVengeanceMFireball")?.Type ?? -1;

                if (projectile.type == pro3Type || projectile.type == pro4Type)
                {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = 40;

                    // Make sure it's NOT using static ID-based immunity
                    projectile.usesIDStaticNPCImmunity = false;
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            if (InfernalCrossmod.YouBoss.Loaded)
            {
                if (projectile.type == InfernalCrossmod.YouBoss.Mod.Find<ModProjectile>("FirstFractalHoldout").Type)
                {
                    if (player.mount.Active && player.altFunctionUse == 2)
                    {
                        player.mount.Dismount(player);
                    }
                    player.RemoveAllGrapplingHooks();
                }
            }

            return base.PreAI(projectile);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (casulJackalBullet)
            {
                if (target.HasBuff<Laceration>())
                {
                    Vector2 bloodpos = projectile.Center + projectile.DirectionTo(target.Center) * 84;
                    if (!spawnedBlood && Main.rand.NextBool() && target.Hitbox.Contains(bloodpos.ToPoint()))
                    {
                        Projectile.NewProjectile(projectile.GetSource_OnHit(target), bloodpos, projectile.DirectionTo(target.Center).RotatedBy(1 * MathHelper.PiOver2 * 0.9f).RotatedByRandom(0.1f) * Main.rand.NextFloat(3f, 5f), ModContent.ProjectileType<BloodstoneHealOrb>(), 10, 0f, projectile.owner);
                        spawnedBlood = true; 
                    }
                }
                else
                    target.AddBuff(ModContent.BuffType<Laceration>(), 60 * 5);
            }
        }
    }
}
