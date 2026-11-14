using CalamityMod.NPCs.Signus;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Signus;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SignusOverrides
{
    public class SignusChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<Signus>();
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

    public class SignusProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<ShadowSlash>()
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

