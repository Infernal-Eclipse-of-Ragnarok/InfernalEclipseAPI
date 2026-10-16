using CalamityMod;
using Consolaria.Content.NPCs.Bosses.Lepus;
using Consolaria.Content.NPCs.Bosses.Ocram;
using Consolaria.Content.Projectiles.Enemies;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using InfernalEclipseAPI.Core.World;
using RevengeancePlus.Projectiles;
using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Consolaria
{
    [JITWhenModsEnabled(InfernalCrossmod.Consolaria.Name)]
    [ExtendsFromMod(InfernalCrossmod.Consolaria.Name)]
    public class ConsolariaBossStatScaling : GlobalNPC
    {
        private bool GetCalDifficulty(string diff)
        {
            return ModLoader.TryGetMod("CalamityMod", out Mod calamity) &&
                   calamity.Call("GetDifficultyActive", diff) is bool b && b;
        }

        private bool IsInfernumActive()
        {
            return InfernumSaveSystem.InfernumModeEnabled;
        }

        private bool GetFargoDifficullty(string diff)
        {
            if (!ModLoader.TryGetMod("FargowiltasSouls", out Mod fargoSouls))
            {
                return false;
            }

            return fargoSouls.Call(diff) is bool active && active;
        }
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return (npc.boss || npc.type == ModContent.NPCType<ServantofOcram>() || npc.type == ModContent.NPCType<DisasterBunny>()) && ((ModType)npc.ModNPC)?.Mod.Name == "Consolaria";
        }

        public override void SetDefaults(NPC entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                if (entity.type == ModContent.NPCType<Ocram>() || entity.type == ModContent.NPCType<ServantofOcram>())
                    entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
            }

            entity.Calamity().canBreakPlayerDefense = true;
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

            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                //Boss Rush Boost
                if ((num1 & num2) != 0)
                {
                    npc.lifeMax += (int)((double).25 * npc.lifeMax);
                }

                npc.lifeMax += (int)(0.35 * npc.lifeMax);
            }
            else
            {
                if (GetFargoDifficullty("EternityMode"))
                {
                    //Boss Rush Boost
                    if ((num1 & num2) != 0)
                    {
                        npc.lifeMax += (int)((double).2 * npc.lifeMax);
                    }

                    npc.lifeMax += (int)(0.25 * npc.lifeMax);
                }
                else if (GetCalDifficulty("death"))
                {
                    //Boss Rush Boost
                    if ((num1 & num2) != 0)
                    {
                        npc.lifeMax += (int)((double).15 * npc.lifeMax);
                    }

                    npc.lifeMax += (int)(0.2 * npc.lifeMax);
                }
                else if (GetCalDifficulty("revengeance"))
                {
                    //Boss Rush Boost
                    if ((num1 & num2) != 0)
                    {
                        npc.lifeMax += (int)((double).1 * npc.lifeMax);
                    }

                    npc.lifeMax += (int)(0.1 * npc.lifeMax);
                }
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            float sourceDamage = 0f;

            if (npc.type == ModContent.NPCType<Ocram>() || npc.type == ModContent.NPCType<ServantofOcram>())
            {
                sourceDamage += 0.15f;
            }

            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                sourceDamage += 1.35f;
            }
            else
            {
                if (GetFargoDifficullty("EternityMode"))
                {
                    sourceDamage += 1.25f;
                }
                else if (GetCalDifficulty("death"))
                {
                    sourceDamage += 1.2f;
                }
                else if (GetCalDifficulty("revengeance"))
                {
                    sourceDamage += 1.1f;
                }
            }

            modifiers.SourceDamage *= sourceDamage;
        }

        public override void PostAI(NPC npc)
        {
            ModNPC modNPC14 = npc.ModNPC;
            if (modNPC14.Name.Contains("Lepus") || modNPC14.Name.Contains("Turkor"))
            {
                return;
            }

            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                npc.position += npc.velocity * 0.35f;
            }
            else
            {
                if (GetFargoDifficullty("EternityMode"))
                {
                    npc.position += npc.velocity * 0.25f;
                }
                else if (GetCalDifficulty("death"))
                {
                    npc.position += npc.velocity * 0.2f;
                }
                else if (GetCalDifficulty("revengeance"))
                {
                    npc.position += npc.velocity * 0.1f;
                }
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Consolaria.Name)]
    [ExtendsFromMod(InfernalCrossmod.Consolaria.Name)]
    public class ConsolariaProjStatScaling : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            {
                // Ocram
                ModContent.ProjectileType<OcramLaser1>(),
                ModContent.ProjectileType<OcramSkull>(),
                ModContent.ProjectileType<OcramScythe>(),
                ModContent.ProjectileType<ManaDrain>(),
                ModContent.ProjectileType<OcramLaser2>(),
                ModContent.ProjectileType<OcramBeamTelegraph>(),
                ModContent.ProjectileType<OcramRay>(),
                ModContent.ProjectileType<OcramBeam>(),
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
            if (InfernalCrossmod.SOTS.Loaded)
            {
                if (entity.type != ModContent.ProjectileType<ManaDrain>())
                    entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.type != ModContent.ProjectileType<OcramBeamTelegraph>() && projectile.type != ModContent.ProjectileType<OcramRay>() && projectile.type != ModContent.ProjectileType<OcramBeam>())
                projectile.Calamity().DealsDefenseDamage = false;

            return base.PreAI(projectile);
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.type == ModContent.ProjectileType<ManaDrain>() && NPC.AnyNPCs(ModContent.NPCType<Ocram>()) && InfernalWorld.RagnarokModeEnabled)
            {
                target.statMana -= info.Damage / 3;
                target.ManaEffect(-(info.Damage / 3));

                if (target.statMana < 0)
                    target.statMana = 0;

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == ModContent.NPCType<Ocram>())
                    {
                        npc.life += info.Damage;
                        npc.HealEffect(info.Damage);

                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;
                    }
                }
            }
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
                if (projectile.type == ModContent.ProjectileType<OcramSkull>() || projectile.type == ModContent.ProjectileType<ManaDrain>())
                    damageMod *= 3.5f;
                else
                    damageMod *= 1.35f;
            }
            else if (InfernalUtilities.GetFargoDifficullty("EternityMode"))
            {
                if (projectile.type == ModContent.ProjectileType<OcramSkull>() || projectile.type == ModContent.ProjectileType<ManaDrain>())
                    damageMod *= 2.5f;
                else
                    damageMod *= 1.25f;
            }
            else if (InfernalUtilities.GetCalDifficulty("death"))
            {
                if (projectile.type == ModContent.ProjectileType<OcramSkull>() || projectile.type == ModContent.ProjectileType<ManaDrain>())
                    damageMod += 1f;
                else
                    damageMod *= 1.1f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }
}