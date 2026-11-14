using InfernumActive = InfernalEclipseAPI.Content.DifficultyOverrides.hellActive;
using CalamityMod.Events;
using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Advisor;
using CalamityMod.UI;
using SOTS.NPCs.Boss.Glowmoth;
using System.Linq;
using SOTS.Projectiles.Earth.Glowmoth;
using InfernalEclipseAPI.Core.Utils;
using CalamityMod;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using SOTS.Projectiles.Chaos;
using SOTS.Projectiles.Planetarium;
using RevengeancePlus.Projectiles;
using SOTS.NPCs;
using Terraria.DataStructures;
using Terraria;
using SOTS.Projectiles.AbandonedVillage;
using SOTS.NPCs.Boss.Lux;
using System.Security.Policy;
using SOTS.Projectiles.Celestial;
using InfernumMode.Content.BehaviorOverrides.BossAIs.DoG;
using Microsoft.Xna.Framework;
using SOTS;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.SecretsOfTheShadows
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod("SOTS")]
    public class SOTSBossStatScaling : GlobalNPC
    {
        public static readonly int[] sotsMinionTypes =
        {
            ModContent.NPCType<GlowmothMinion>(),
            ModContent.NPCType<PutridPinky1>(),
            ModContent.NPCType<PutridHook>(),
            ModContent.NPCType<SOTS.NPCs.Boss.Lux.FakeLux>(),
            ModContent.NPCType<PhaseEye>()
        };

        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return (npc.boss || sotsMinionTypes.Contains(npc.type)) && npc.ModNPC?.Mod.Name == "SOTS";
        }

        public override void SetDefaults(NPC entity)
        {
            if (entity.ModNPC.Name.Contains("Excavator") && InfernumActive.InfernumActive)
            {
                entity.defense += 5;
            }

            if (entity.type == ModContent.NPCType<GlowmothMinion>() || entity.type == ModContent.NPCType<PhaseEye>())
            {
                entity.Calamity().canBreakPlayerDefense = true;
            }

            if (entity.type == ModContent.NPCType<Glowmoth>() || entity.type == ModContent.NPCType<GlowmothMinion>() || entity.type == ModContent.NPCType<PhaseEye>() || entity.type == ModContent.NPCType<Lux>())
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
            num2 = flag ? 1 : 0;

            if (InfernumActive.InfernumActive)
            {
                //Boss Rush Boost
                if ((num1 & num2) != 0)
                {
                    string name = npc.ModNPC?.Name ?? "";
                    if (name.Contains("Excavator"))
                        npc.lifeMax *= 7; //less because holy fuck its tanky in boss rush

                    npc.lifeMax += (int)((double).25 * npc.lifeMax);
                }

                if (npc.ModNPC.Name.Contains("Lux"))
                {
                    npc.lifeMax += (int)(0.45 * npc.lifeMax);
                }
                else if (npc.ModNPC.Name.Contains("TheAdvisorHead"))
                {
                    npc.lifeMax += (int)(0.25 * npc.lifeMax);
                }

                if (npc.type == ModContent.NPCType<PutridPinky1>())
                {
                    npc.lifeMax += 3 * npc.lifeMax;
                }
                else if (npc.type == ModContent.NPCType<PutridHook>())
                {
                    npc.lifeMax /= 2;
                }
                else if (npc.ModNPC.Name.Contains("SubspaceSerpent"))
                {
                    npc.lifeMax += (int)(0.15f * npc.lifeMax);
                }
                else if (npc.ModNPC.Name.Contains("Excavator"))
                {
                    npc.lifeMax += (int)(0.05f * npc.lifeMax);
                }
                else if (npc.type != ModContent.NPCType<PutridPinkyPhase2>())
                    npc.lifeMax += (int)((double).35 * npc.lifeMax);
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (InfernumActive.InfernumActive)
            {
                if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
                {
                    modifiers.SourceDamage *= 1.20f;
                }
                else if (npc.type == ModContent.NPCType<PutridPinky1>())
                {
                    modifiers.SourceDamage *= 2.5f;
                }
                else modifiers.SourceDamage *= 1.35f;

                if (npc.ModNPC.Name.Contains("Excavator"))
                {
                    modifiers.SourceDamage *= 1.1f;
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.ModNPC.Name.Contains("Excavator") && InfernumActive.InfernumActive)
            {
                DoGPhase1BodyBehaviorOverride.KillUnbalancedDebuffs(npc);
            }

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (InfernumActive.InfernumActive)
            {
                /*
                if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
                {
                    npc.position += npc.velocity * 0.3f;
                }
                else 
                */
                if (npc.type == ModContent.NPCType<PutridPinky1>())
                    return;

                if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
                    npc.position += npc.velocity * 0.15f;
                else
                    npc.position += npc.velocity * 0.35f;
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod("SOTS")]
    public class AdvisorDefenseReset : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        // This issue has been fixed in main Revengence+ - ..nevermind
        private bool scaledBossRushHP = false;

        private const int DespawnDelay = 600;

        private const float PlayerCheckRange = 3200f;
        private const float PlayerCheckRangeSQ = PlayerCheckRange * PlayerCheckRange;

        private int ticksWithoutValidPlayer;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<TheAdvisorHead>();
        }

        public override bool PreAI(NPC npc)
        {
            if (!npc.boss && !BossHealthBarManager.BossExclusionList.Contains(npc.type))
                BossHealthBarManager.BossExclusionList.Add(npc.type);
            else if (npc.boss && BossHealthBarManager.BossExclusionList.Contains(npc.type))
                BossHealthBarManager.BossExclusionList.Remove(npc.type);

            if (Main.gameMenu || !npc.active || !npc.boss)
                return base.PreAI(npc);

            bool validPlayerFound = false;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (player is null || !player.active || player.dead)
                    continue;

                if (Vector2.DistanceSquared(player.Center, npc.Center) > PlayerCheckRangeSQ)
                    continue;

                if (player.SOTSPlayer().PlanetariumBiome || BossRushEvent.BossRushActive)
                {
                    validPlayerFound = true;
                    break;
                }
            }

            if (validPlayerFound)
            {
                ticksWithoutValidPlayer = 0;
                return base.PreAI(npc);
            }

            ticksWithoutValidPlayer++;

            if (ticksWithoutValidPlayer >= DespawnDelay)
                ForceDespawn(npc);

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (BossRushEvent.BossRushActive && !scaledBossRushHP)
            {
                npc.lifeMax += (int)((double).25 * npc.lifeMax);
                npc.life = npc.lifeMax;
                scaledBossRushHP = true;
                return;
            }

            int targetDefense = 24;

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                // These calls must match Calamity's internal names exactly:
                object isDeath = calamity.Call("GetDifficultyActive", "Death");
                object isRevenge = calamity.Call("GetDifficultyActive", "Revengeance");

                if (isDeath is bool bDeath && bDeath)
                {
                    // Death Mode
                    npc.position += npc.velocity * 0.35f;
                    targetDefense = 39;
                }
                else if (isRevenge is bool bRev && bRev)
                {
                    // Revengeance Mode
                    npc.position += npc.velocity * 0.25f;
                    targetDefense = 32;
                }
            }

            npc.defense = targetDefense;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (InfernumSaveSystem.InfernumModeEnabled)
                modifiers.SourceDamage *= 1.35f;
        }

        private static void ForceDespawn(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || BossRushEvent.BossRushActive)
                return;

            npc.active = false;
            npc.netUpdate = true;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod("SOTS")]
    public class SOTSProjStatScaling : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            {
                //Glowmoth
                ModContent.ProjectileType<WaveBall>(),
                ModContent.ProjectileType<GlowBombOrb>(),
                ModContent.ProjectileType<GlowBombShard>(),
                ModContent.ProjectileType<GlowSparkle>(),

                //Excavator
                ModContent.ProjectileType<ExcavatorOrb>(),
                ModContent.ProjectileType<ExcavatorBolt>(),
                ModContent.ProjectileType<ExcavatorLightning>(),
                ModContent.ProjectileType<ExcavatorRocket>(),
                ModContent.ProjectileType<ExcavatorSaw>(),
                ModContent.ProjectileType<CollapseBlock>(),
                ModContent.ProjectileType<ExcavatorBoltBig>(),
                ModContent.ProjectileType<ExcavatorBoltSmall>(),

                //Advisor
                ModContent.ProjectileType<OtherworldlyBall>(),
                ModContent.ProjectileType<OtherworldlyBolt>(),
                ModContent.ProjectileType<OtherworldlyTracer>(),
                ModContent.ProjectileType<HoloMissile>(),
                ModContent.ProjectileType<ChargeBeam>(),
                ModContent.ProjectileType<ThunderColumn>(),
                ModContent.ProjectileType<ThunderColumnBlue>(),
                ModContent.ProjectileType<ThunderColumnFast>(),
                ModContent.ProjectileType<PhaseSpear>(),

                ModContent.ProjectileType<LesserPhaseBolt>(),

                //Lux
                ModContent.ProjectileType<DogmaSphere>(),
                ModContent.ProjectileType<ChaosWave>(),
                ModContent.ProjectileType<ChaosStar>(),
                ModContent.ProjectileType<ChaosDart>(),
                ModContent.ProjectileType<ChaosDiamond>(),
                ModContent.ProjectileType<ChaosDiamondLaser>(),
                ModContent.ProjectileType<ChaosBall>(),
                ModContent.ProjectileType<ChaosDart2>(),
                ModContent.ProjectileType<ChaosEraser>(),
                ModContent.ProjectileType<ChaosEraser2>(),
                ModContent.ProjectileType<DogmaLaser>(),
                ModContent.ProjectileType<ThunderBall>(),
                ModContent.ProjectileType<ChaosHelixLaser>(),
                ModContent.ProjectileType<RevengeancePlus.Projectiles.FakeLux>(),

                //Subspace
                ModContent.ProjectileType<BossBabyLaser>(),
                ModContent.ProjectileType<GreaterCellBlast>(),
                ModContent.ProjectileType<WaveBlast>(),
                ModContent.ProjectileType<InfernoPhaseBolt>()
            };

            foreach (int type in types)
            {
                if (entity.type == type)
                    return true;
            }
            return false;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (!entity.ModProjectile.Name.Contains("Excavator") && entity.type != ModContent.ProjectileType<CollapseBlock>() &&
                entity.type != ModContent.ProjectileType<BossBabyLaser>() && entity.type != ModContent.ProjectileType<GreaterCellBlast>() && entity.type != ModContent.ProjectileType<WaveBlast>() && entity.type != ModContent.ProjectileType<InfernoPhaseBolt>())
                entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;

            if (entity.type != ModContent.ProjectileType<GlowBombOrb>() && entity.type != ModContent.ProjectileType<HoloMissile>() && 
                entity.type != ModContent.ProjectileType<ExcavatorRocket>() && entity.type != ModContent.ProjectileType<ExcavatorSaw>() && entity.type != ModContent.ProjectileType<CollapseBlock>() && 
                entity.type != ModContent.ProjectileType<RevengeancePlus.Projectiles.FakeLux>() && entity.type != ModContent.ProjectileType<InfernoPhaseBolt>())
                entity.Calamity().DealsDefenseDamage = false;
        }

        public override void OnSpawn(Projectile entity, IEntitySource source)
        {
            if (entity.type != ModContent.ProjectileType<GlowBombOrb>() && entity.type != ModContent.ProjectileType<HoloMissile>() &&
                entity.type != ModContent.ProjectileType<ExcavatorRocket>() && entity.type != ModContent.ProjectileType<ExcavatorSaw>() && entity.type != ModContent.ProjectileType<CollapseBlock>() &&
                entity.type != ModContent.ProjectileType<RevengeancePlus.Projectiles.FakeLux>())
                entity.Calamity().DealsDefenseDamage = false;

            if (entity.type == ModContent.ProjectileType<InfernoPhaseBolt>())
                entity.damage = 50;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            float damageMod = 1f;

            if (InfernalUtilities.IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (InfernalUtilities.IsInfernumActive() || InfernalUtilities.GetFargoDifficullty("MasochistMode"))
            {
                if (projectile.type == ModContent.ProjectileType<GlowBombOrb>() || projectile.type == ModContent.ProjectileType<GlowBombShard>() || projectile.type == ModContent.ProjectileType<GlowSparkle>())
                    damageMod *= 1.95f;
                else if (projectile.type == ModContent.ProjectileType<WaveBall>() || projectile.Name.Contains("Excavator") || projectile.type == ModContent.ProjectileType<CollapseBlock>())
                    damageMod *= 1.5f;
                else if ((projectile.ModProjectile.Name.Contains("Chaos") || projectile.ModProjectile.Name.Contains("Dogma")) && projectile.type != ModContent.ProjectileType<ChaosHelixLaser>())
                {
                    damageMod *= 1.2f;
                }
                else
                {
                    damageMod *= 1.35f;
                }
            }
            else if (InfernalUtilities.GetFargoDifficullty("EternityMode"))
            {
                if (projectile.type == ModContent.ProjectileType<GlowBombOrb>() || projectile.type == ModContent.ProjectileType<GlowBombShard>() || projectile.type == ModContent.ProjectileType<GlowSparkle>())
                    damageMod *= 1.45f;
                else if (projectile.type == ModContent.ProjectileType<WaveBall>() || projectile.Name.Contains("Excavator") || projectile.type == ModContent.ProjectileType<CollapseBlock>())
                    damageMod *= 1.25f;
                else
                    damageMod *= 1.25f;
            }
            else if (InfernalUtilities.GetCalDifficulty("death"))
            {
                if (projectile.type == ModContent.ProjectileType<GlowBombOrb>() || projectile.type == ModContent.ProjectileType<GlowBombShard>() || projectile.type == ModContent.ProjectileType<GlowSparkle>())
                    damageMod *= 1.15f;
                else if (projectile.type == ModContent.ProjectileType<WaveBall>() || projectile.Name.Contains("Excavator") || projectile.type == ModContent.ProjectileType<CollapseBlock>())
                    damageMod *= 1.1f;
                else
                    damageMod *= 1.1f;
            }

            if (projectile.type != ModContent.ProjectileType<InfernoPhaseBolt>())
                modifiers.SourceDamage *= damageMod;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod("SOTS")]
    public class HoloMissileRevPlusRevert : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        private bool revertedSpawnChanges;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<HoloMissile>();

        public override void SetDefaults(Projectile entity)
        {
            entity.tileCollide = false;    
        }

        public override void AI(Projectile projectile)
        {
            if (revertedSpawnChanges)
                return;

            revertedSpawnChanges = true;

            // Undo RevengeancePlus.OnSpawn:
            // ++projectile.extraUpdates;
            // projectile.velocity *= 0.8f;
            if (projectile.extraUpdates > 0)
                projectile.extraUpdates--;

            projectile.velocity /= 0.8f;
        }
    }
}