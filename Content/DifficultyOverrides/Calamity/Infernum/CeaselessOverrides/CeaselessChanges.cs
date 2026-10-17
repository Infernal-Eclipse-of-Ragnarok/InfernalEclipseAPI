using CalamityMod.NPCs.CeaselessVoid;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.BehaviorOverrides.BossAIs.CeaselessVoid;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.CeaselessOverrides
{
    public class CeaselessChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CeaselessVoid>() ||
                   entity.type == ModContent.NPCType<DarkEnergy>();
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

    public class CeaselessProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<AcceleratingDarkEnergy>(),
                ModContent.ProjectileType<OtherworldlyBolt>(),
                ModContent.ProjectileType<CeaselessVortex>(),
                ModContent.ProjectileType<SpinningDarkEnergy>(),
                ModContent.ProjectileType<TelegraphedOtherwordlyBolt>()
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
