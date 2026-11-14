using CatalystMod.NPCs.Boss.Astrageldon;
using CatalystMod.Projectiles.Enemy;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using InfernumMode.Core.GlobalInstances.Systems;
using InfernumActive = InfernalEclipseAPI.Content.DifficultyOverrides.hellActive;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity
{
    [JITWhenModsEnabled(InfernalCrossmod.Catalyst.Name)]
    [ExtendsFromMod(InfernalCrossmod.Catalyst.Name)]
    public class CatalystBossStatScaling : GlobalNPC
    {
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return (npc.boss || npc.type == ModContent.NPCType<NovaSlime>() || npc.type == ModContent.NPCType<NovaSlimer>()) && ((ModType)npc.ModNPC)?.Mod.Name == "CatalystMod";
        }

        public override void SetDefaults(NPC npc)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                npc.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;

                if (npc.type == ModContent.NPCType<Astrageldon>())
                    npc.GetGlobalNPC<SOTSGlobalNPC>().strongVoidDamge = true;
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
                ModNPC modNPC14 = npc.ModNPC;
                if ((modNPC14 != null ? modNPC14.Name.Contains("Astrageldon") ? 1 : 0 : 0) != 0)
                {
                    npc.lifeMax *= 10;
                }
            }
            else
            {
                if (NPC.downedMoonlord && npc.type == ModContent.NPCType<Astrageldon>())
                {
                    npc.lifeMax += (int)((double).25 * npc.lifeMax);
                }
            }

            if (InfernumActive.InfernumActive && !ModLoader.HasMod("CnI"))
            {
                npc.lifeMax += (int)((double).35 * npc.lifeMax);
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (NPC.downedMoonlord)
            {
                modifiers.SourceDamage *= 1.05f;
            }

            if (InfernumActive.InfernumActive && !ModLoader.HasMod("CnI") && npc.type == ModContent.NPCType<Astrageldon>())
            {
                modifiers.SourceDamage *= 1.15f;
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Catalyst.Name)]
    [ExtendsFromMod(InfernalCrossmod.Catalyst.Name)]
    public class CatalystBossProjStatScaling : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<AstraBlade>(),
                ModContent.ProjectileType<AstrageldonStar>(),
                ModContent.ProjectileType<AstrageldonLaser>(),
                ModContent.ProjectileType<AstraRock>(),

                ModContent.ProjectileType<NebulaSphere>(),

                ModContent.ProjectileType<SlimerSpear>(),
                ModContent.ProjectileType<NebulaResidue>(),
                ModContent.ProjectileType<AstrageldonStar2>(),
                ModContent.ProjectileType<AstrageldonStar3>(),
                ModContent.ProjectileType<StarLaser>(),
                ModContent.ProjectileType<AstrageldonStar4>(),
                ModContent.ProjectileType<SpiralLaser>(),
                ModContent.ProjectileType<NebulaResidueHB>(),

                ModContent.ProjectileType<HostileAsteroid>()
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
                if (entity.type != ModContent.ProjectileType<AstraRock>() && entity.type != ModContent.ProjectileType<HostileAsteroid>())
                {
                    entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                    entity.GetGlobalProjectile<VoidDamageProjectile>().strongVoidDamge = true;
                }

                if (entity.type == ModContent.ProjectileType<NebulaSphere>())
                {
                    entity.GetGlobalProjectile<VoidDamageProjectile>().strongerVoidDamage = true;
                }
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (ModLoader.HasMod("CnI")) return;

            float damageMod = 1f;

            if (NPC.downedMoonlord)
            {
                damageMod += 0.15f;
            }

            if (WorldSaveSystem.InfernumModeEnabled || InfernalUtilities.GetFargoDifficullty("MasochistMode"))
            {
                damageMod *= 1.15f;
            }
            else if (InfernalUtilities.GetFargoDifficullty("EternityMode"))
            {
                damageMod *= 1.05f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }
}