using System.Linq;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using YouBoss.Content.NPCs.Bosses.TerraBlade;
using YouBoss.Content.NPCs.Bosses.TerraBlade.Projectiles;
using InfernumActive = InfernalEclipseAPI.Content.DifficultyOverrides.hellActive;

namespace InfernalEclipseAPI.Content.DifficultyOverrides
{
    [JITWhenModsEnabled(InfernalCrossmod.YouBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.YouBoss.Name)]
    public class YouBossStatScaling : GlobalNPC
    {
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return npc.type == ModContent.NPCType<TerraBladeBoss>();
        }

        public override void SetDefaults(NPC entity)
        {
            InfernalCrossmod.Calamity.Mod.Call("SetDefenseDamageNPC", entity, true);
            if (InfernalCrossmod.SOTS.Loaded)
            {
                entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            Mod mod;
            bool flag = false;
            int num1 = 0, num2 = 0;

            if (ModLoader.TryGetMod("CalamityMod", out mod))
            {
                object result = mod.Call("GetDifficultyActive", "BossRush");
                if (result is bool b)
                {
                    flag = b;
                    num1 = 1;
                }
            }
            num2 = flag ? 1 : 0;
            if ((num1 & num2) != 0)
            {
                npc.lifeMax *= 2;
            }

            if (InfernumActive.InfernumActive)
            {
                npc.lifeMax += (int)(((double).35) * (double)npc.lifeMax);
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (InfernumActive.InfernumActive)
            {   
                modifiers.SourceDamage *= 1.8f;
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.YouBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.YouBoss.Name)]
    public class YouBossProjectileScaling : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<AcceleratingTerraBeam>(),
                    ModContent.ProjectileType<ArcingLightBeam>(),
                    ModContent.ProjectileType<ArcingNightBeam>(),
                    ModContent.ProjectileType<ArcingTerraBeam>(),
                    ModContent.ProjectileType<TelegraphedTerraBeam>(),
                    ModContent.ProjectileType<TerraBladeSplit>(),
                    ModContent.ProjectileType<TerraGroundShock>(),
                ];

            return types.Contains(entity.type);
        }

        public override void SetDefaults(Projectile entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (projectile.damage > 0 && InfernumActive.InfernumActive)
            {
                modifiers.SourceDamage *= 1.5f;
            }
        }
    }
}