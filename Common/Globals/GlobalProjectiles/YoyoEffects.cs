using InfernalEclipseAPI.Core.Systems;
using Terraria.Audio;

namespace InfernalEclipseAPI.Common.Globals.GlobalProjectiles
{
    public class YoyoEffects : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        private int hitCounter;
        private int valorFireTimer;
        public bool SpawnedFromAddedYoyoEffect;

        private static int salivationPro2Type = -1;
        private static int headSpinnerPro2Type = -1;

        public override bool IsLoadingEnabled(Mod mod) => !InfernalCrossmod.Hummus.Loaded;

        public override void SetStaticDefaults()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                return;

            salivationPro2Type = thorium.Find<ModProjectile>("SalivationPro2")?.Type ?? -1;
            headSpinnerPro2Type = thorium.Find<ModProjectile>("HeadSpinnerPro2")?.Type ?? -1;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ProjectileID.Chik)
            {
                hitCounter++;

                if (hitCounter < 5)
                    return;

                hitCounter = 0;

                if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                    return;

                SoundEngine.PlaySound(SoundID.Item101, projectile.Center);

                for (int i = 0; i < 5; i++)
                {
                    int projectileIndex = Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * 8f, ProjectileID.CrystalShard, (projectile.damage / 4), projectile.knockBack, projectile.owner);

                    if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
                    {
                        Projectile spawnedProjectile = Main.projectile[projectileIndex];

                        spawnedProjectile.DamageType = DamageClass.Melee;
                        spawnedProjectile.penetrate = 2;

                        spawnedProjectile.usesLocalNPCImmunity = true;
                        spawnedProjectile.localNPCHitCooldown = 40;

                        spawnedProjectile.usesIDStaticNPCImmunity = false;
                    }
                }
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (projectile.type == ProjectileID.CorruptYoyo)
                {
                    hitCounter++;

                    if (hitCounter < 8)
                        return;

                    hitCounter = 0;

                    if (salivationPro2Type == -1)
                        return;

                    if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                        return;

                    int projectileIndex = Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * 8f, salivationPro2Type, projectile.damage, projectile.knockBack, projectile.owner);

                    if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
                    {
                        Projectile spawnedProjectile = Main.projectile[projectileIndex];

                        spawnedProjectile.GetGlobalProjectile<YoyoEffects>().SpawnedFromAddedYoyoEffect = true;

                        spawnedProjectile.velocity *= 0.5f;
                    }
                }

                if (projectile.type == ProjectileID.CrimsonYoyo)
                {
                    hitCounter++;

                    if (hitCounter < 8)
                        return;

                    hitCounter = 0;

                    if (headSpinnerPro2Type == -1)
                        return;

                    if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                        return;

                    int projectileIndex = Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * 8f, headSpinnerPro2Type, projectile.damage, projectile.knockBack, projectile.owner);

                    if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
                    {
                        Projectile spawnedProjectile = Main.projectile[projectileIndex];

                        spawnedProjectile.GetGlobalProjectile<YoyoEffects>().SpawnedFromAddedYoyoEffect = true;

                        spawnedProjectile.velocity *= 0.5f;
                    }
                }
            }
        }

        public override void AI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.Valor)
            {
                valorFireTimer++;

                const int fireRate = 90;
                const float searchRadius = 200f;

                if (valorFireTimer >= fireRate)
                {
                    valorFireTimer = 0;

                    if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
                    {
                        NPC closestNPC = null;
                        float closestDistance = searchRadius;

                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            if (npc.friendly || npc.dontTakeDamage || npc.life <= 0)
                                continue;

                            float distance = projectile.Distance(npc.Center);

                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestNPC = npc;
                            }
                        }

                        if (closestNPC != null)
                        {
                            Microsoft.Xna.Framework.Vector2 direction = (closestNPC.Center - projectile.Center).SafeNormalize(Microsoft.Xna.Framework.Vector2.Zero);

                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, direction * 8f, ProjectileID.BoneGloveProj, projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                }
            }

            if (!SpawnedFromAddedYoyoEffect)
                return;

            const float maxVelocity = 4f;

            if (projectile.velocity.LengthSquared() > maxVelocity * maxVelocity)
            {
                projectile.velocity = projectile.velocity.SafeNormalize(Microsoft.Xna.Framework.Vector2.Zero) * maxVelocity;
            }
        }
    }
}