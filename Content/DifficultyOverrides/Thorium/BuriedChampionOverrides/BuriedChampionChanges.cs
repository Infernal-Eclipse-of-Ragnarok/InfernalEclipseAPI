using CalamityMod.World;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Core.GlobalInstances.Systems;
using Terraria.DataStructures;
using ThoriumMod.NPCs.BossBuriedChampion;
using ThoriumRework.BossChanges;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Thorium.BuriedChampionOverrides
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class BuriedChampionProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
            ];

            foreach (int type in types)
            {
                if (entity.type == type)
                    return true;
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                return BCHelheimChanges.IsReworkProjectile(entity);
            }

            return false;
        }

        public override void SetDefaults(Projectile entity)
        {

        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {

        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class BCHelheimChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<BuriedChampion>();

        public override void SetDefaults(NPC entity)
        {
            if (entity.type == ModContent.NPCType<BuriedChampion>())
            {
                if (WorldSaveSystem.InfernumModeEnabled)
                {
                    entity.defense = 15;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (WorldSaveSystem.InfernumModeEnabled)
            {
                npc.damage += (int)(npc.damage * 0.4);
            }
            else if (CalamityWorld.death)
            {
                npc.damage += (int)(npc.damage * 0.25);
            }
            else if (CalamityWorld.revenge)
            {
                npc.damage += (int)(npc.damage * 0.15);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (WorldSaveSystem.InfernumModeEnabled)
            {
                BuriedChampionRework.ArrowDamage += 8;
                BuriedChampionRework.DaggerDamage += 8;
                BuriedChampionRework.BombDamage += 8;
                BuriedChampionRework.SliceDamage += 10;

                if (InfernalConfig.Instance.DeveloperMode)
                {
                    Main.NewText(BuriedChampionRework.ArrowDamage);
                    Main.NewText(BuriedChampionRework.DaggerDamage);
                    Main.NewText(BuriedChampionRework.BombDamage);
                    Main.NewText(BuriedChampionRework.SliceDamage);
                }
            }
        }

        public override void PostAI(NPC npc)
        {
            if (WorldSaveSystem.InfernumModeEnabled)
            {


            }
        }

        public static bool IsReworkProjectile(Projectile projectile)
        {
            int[] reworkType =
            [
            ];

            foreach (int type in reworkType)
            {
                if (projectile.type == type)
                    return true;
            }

            return false;
        }
    }
}
