using CalamityMod.NPCs.Polterghast;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Polterghast;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.PolterghastOverrides
{
    public class PolterghastChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<Polterghast>() ||
                   entity.type == ModContent.NPCType<PolterghastLeg>() ||
                   entity.type == ModContent.NPCType<PolterPhantom>();
        }

        public override void SetDefaults(NPC entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
                entity.GetGlobalNPC<SOTSGlobalNPC>().strongVoidDamge = true;
            }
        }
    }

    public class PolterghastProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<EctoplasmShot>(),
                ModContent.ProjectileType<GhostlyVortex>(),
                ModContent.ProjectileType<CirclingEctoplasm>(),
                ModContent.ProjectileType<NotSpecialSoul>(),
                ModContent.ProjectileType<ArcingSoul>(),
                ModContent.ProjectileType<NonReturningSoul>(),
                ModContent.ProjectileType<SpinningSoul>(),

            ];

            foreach (int type in types)
            {
                if (entity.type == type)
                    return true;
            }
            return false;
        }
        public override void SetDefaults(Projectile entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                entity.GetGlobalProjectile<VoidDamageProjectile>().strongerVoidDamage = true;
            }
        }
    }
}
