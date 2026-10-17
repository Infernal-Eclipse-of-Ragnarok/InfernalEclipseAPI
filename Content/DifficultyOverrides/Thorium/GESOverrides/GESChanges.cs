using CalamityMod;
using CalamityMod.World;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Core.GlobalInstances.Systems;
using Terraria.DataStructures;
using ThoriumMod.NPCs.BossGraniteEnergyStorm;
using ThoriumMod.Projectiles.Boss;
using ThoriumRework.Projectiles;
using GESRework = ThoriumRework.BossChanges.GraniteEnergyStormRework;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Thorium.GESOverrides
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class GESProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<ThoriumMod.Projectiles.Boss.GraniteCharge>(),
                ModContent.ProjectileType<GraniteBurst>(),
            ];

            foreach (int type in types)
            {
                if (entity.type == type)
                    return true;
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                return GESHelheimChanges.IsReworkProjectile(entity);
            }

            return false;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (!entity.ModProjectile.Name.Contains("EnergySurge") && entity.type != ModContent.ProjectileType<GraniteBurst>())
            {
                entity.Calamity().DealsDefenseDamage = false;
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.ModProjectile.Name.Contains("EnergySurge"))
            {
                target.AddBuff(BuffID.Electrified, 60);
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class GESHelheimChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private int spawnTimer;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<GraniteEnergyStorm>() || entity.type == ModContent.NPCType<CoalescedEnergy>() || entity.type == ModContent.NPCType<EncroachingEnergy>() || entity.type == ModContent.NPCType<EnergyConduit>();

        public override void SetDefaults(NPC entity)
        {
            if (entity.type == ModContent.NPCType<GraniteEnergyStorm>())
            {
                if (WorldSaveSystem.InfernumModeEnabled)
                {
                    entity.defense = 15;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (npc.type != ModContent.NPCType<GraniteEnergyStorm>())
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
            spawnTimer = 60 * 3;

            if (WorldSaveSystem.InfernumModeEnabled)
            {
                if (npc.type == ModContent.NPCType<GraniteEnergyStorm>())
                {
                    GESRework.SuperSurgeDamage += 1;
                    GESRework.SpikeDamage += 1;
                    GESRework.GraniteDamage += 3;
                    GESRework.ConduitDamage += 1;

                    if (InfernalConfig.Instance.DeveloperMode)
                    {
                        Main.NewText(GESRework.SuperSurgeDamage);
                        Main.NewText(GESRework.SpikeDamage);
                        Main.NewText(GESRework.GraniteDamage);
                        Main.NewText(GESRework.ConduitDamage);
                    }
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (spawnTimer > 0)
            {
                spawnTimer--;

                if (spawnTimer <= 0)
                    npc.dontTakeDamage = false;
                else
                    npc.dontTakeDamage = true;
            }

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (WorldSaveSystem.InfernumModeEnabled)
            {
                bool anyCoalesced = NPC.AnyNPCs(ModContent.NPCType<CoalescedEnergy>());
                bool anyEncroaching = NPC.AnyNPCs(ModContent.NPCType<EncroachingEnergy>());

                if (npc.type == ModContent.NPCType<GraniteEnergyStorm>())
                {
                    if (npc.ai[0] != 7f)
                        npc.dontTakeDamage = anyCoalesced || anyEncroaching;
                }

                if (npc.type == ModContent.NPCType<EncroachingEnergy>())
                {
                    npc.dontTakeDamage = anyCoalesced;
                }
            }
        }

        public static bool IsReworkProjectile(Projectile projectile)
        {
            int[] reworkType =
            [
                ModContent.ProjectileType<EnergySurge>(),
                ModContent.ProjectileType<EnergizedGranite>(),
                ModContent.ProjectileType<EnergySpike>()
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
