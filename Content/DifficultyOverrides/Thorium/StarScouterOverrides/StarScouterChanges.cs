using CalamityMod;
using CalamityMod.World;
using InfernalEclipseAPI.Content.DifficultyOverrides.Thorium.GESOverrides;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using InfernumMode.Core.GlobalInstances.Systems;
using Newtonsoft.Json.Converters;
using Terraria;
using Terraria.DataStructures;
using ThoriumMod.NPCs.BossStarScouter;
using ThoriumMod.Projectiles;
using ThoriumMod.Projectiles.Boss;
using ThoriumRework.BossChanges;
using ThoriumRework.Projectiles;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Thorium.StarScouterOverrides
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class StarScouterChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<StarScouter>() ||
                   entity.type == ModContent.NPCType<CryoCore>() ||
                   entity.type == ModContent.NPCType<PyroCore>() ||
                   entity.type == ModContent.NPCType<BioCore>();
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (WorldSaveSystem.InfernumModeEnabled)
            {
                if (npc.type != ModContent.NPCType<StarScouter>())
                    npc.lifeMax += (int)(npc.lifeMax * 2.5f);
            }
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class StarScouterProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<VaporizeBlast>(),
                ModContent.ProjectileType<GravitonSurge>(),
                ModContent.ProjectileType<VaporizePulse>(),
                ModContent.ProjectileType<ThoriumMod.Projectiles.Boss.Vaporize>(),
                ModContent.ProjectileType<GravitySpark>(),
                ModContent.ProjectileType<GravitonCharge>(),
                ModContent.ProjectileType<CryoVaporize>(),
                ModContent.ProjectileType<CryoCoreBeam>(),
                ModContent.ProjectileType<PyroBurst>(),
                ModContent.ProjectileType<PyroExplosion>(),
                ModContent.ProjectileType<PyroExplosion2>(),
                ModContent.ProjectileType<BioCoreBeam>(),
                ModContent.ProjectileType<BioVaporize>()
            ];

            foreach (int type in types)
            {
                if (entity.type == type)
                    return true;
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                return StarScouterHelheimChanges.IsReworkProjectile(entity);
            }

            return false;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (!entity.ModProjectile.Name.Contains("Stuff") && !entity.ModProjectile.Name.Contains("VaporizingBeam"))
            {
                entity.Calamity().DealsDefenseDamage = false;
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                if (StarScouterHelheimChanges.IsReworkProjectile(projectile)) return;
            }

            float damageMod = 1f;

            if (InfernalUtilities.IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (WorldSaveSystem.InfernumModeEnabled)
            {
                damageMod *= 1.15f;

            }
            else if (CalamityWorld.death)
            {
                damageMod *= 1.05f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class StarScouterHelheimChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<StarScouter>();
        }

        public override void SetDefaults(NPC entity)
        {
            if (entity.type == ModContent.NPCType<StarScouter>())
            {
                if (WorldSaveSystem.InfernumModeEnabled)
                {
                    entity.defense = 18;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (npc.type != ModContent.NPCType<StarScouter>())
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
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (WorldSaveSystem.InfernumModeEnabled)
            {
                if (npc.type == ModContent.NPCType<StarScouter>())
                {
                    StarScouterRework.VaporizeDamage += 4;
                    StarScouterRework.GravitonDamage += 0;
                    StarScouterRework.SniperDamage += 0;
                    StarScouterRework.BeamDamage += 0;

                    if (InfernalConfig.Instance.DeveloperMode)
                    {
                        Main.NewText(StarScouterRework.VaporizeDamage);
                        Main.NewText(StarScouterRework.GravitonDamage);
                        Main.NewText(StarScouterRework.SniperDamage);
                        Main.NewText(StarScouterRework.BeamDamage);
                    }
                }
            }
        }

        public static bool IsReworkProjectile(Projectile projectile)
        {
            int[] reworkType =
            [
                ModContent.ProjectileType<ThoriumRework.Projectiles.Vaporize>(),
                ModContent.ProjectileType<Stuff>(),
                ModContent.ProjectileType<GravitonOrb>(),
                ModContent.ProjectileType<VaporizingBeam>(),

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
