using CalamityMod;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Core.GlobalInstances.Systems;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Vanilla
{
    public class VanillaBossStatScaling : GlobalNPC
    {
        private bool GetFargoDifficullty(string diff)
        {
            if (!ModLoader.TryGetMod("FargowiltasSouls", out Mod fargoSouls))
            {
                return false;
            }

            return fargoSouls.Call(diff) is bool active && active;
        }

        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC entity)
        {
            if (InfernalWorld.RagnarokModeEnabled)
            {
                if (entity.type == NPCID.WallofFleshEye)
                {
                    entity.damage = 150;
                    entity.Calamity().canBreakPlayerDefense = true;

                    if (InfernalCrossmod.SOTS.Loaded)
                    {
                        entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
                    }
                }
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
                NPC modNPC14 = npc;
                if (modNPC14.type == NPCID.BloodNautilus)
                {
                    npc.lifeMax *= 50;
                }
                else
                {
                    NPC modNPC15 = npc;
                    if (modNPC14.type == NPCID.DD2Betsy)
                    {
                        npc.lifeMax *= 15;
                    }
                }
            }

            if (InfernalConfig.Instance.AdditonalVanillaBossAdjustments && !(GetFargoDifficullty("MasochistMode") || GetFargoDifficullty("EternityMode")))
            {
                if (npc.type == NPCID.QueenSlimeBoss)
                {
                    npc.lifeMax *= 2;
                }

                if (npc.type == NPCID.BloodNautilus)
                {
                    npc.lifeMax += npc.lifeMax / 2;
                }

                if (npc.type == NPCID.CultistBoss)
                {
                    if (NPC.downedGolemBoss)
                        npc.lifeMax += (int)(1.5 * npc.lifeMax);
                    else if (NPC.downedPlantBoss)
                        npc.lifeMax += (int)(1.25 * npc.lifeMax);
                    else if (NPC.downedMechBossAny)
                    {
                        if (NPC.downedMechBoss1)
                            npc.lifeMax += (int)(0.42 * npc.lifeMax);
                        if (NPC.downedMechBoss2)
                            npc.lifeMax += (int)(0.42 * npc.lifeMax);
                        if (NPC.downedMechBoss3)
                            npc.lifeMax += (int)(0.41 * npc.lifeMax);
                    }
                    else
                        npc.lifeMax *= 2;
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.WallofFleshEye && InfernalWorld.RagnarokModeEnabled && npc.Infernum().ExtraAI[2] != 1f)
                npc.damage = 150;

            if (npc.type == NPCID.SkeletronHead && WorldSaveSystem.InfernumModeEnabled)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.dead || !npc.WithinRange(player.Center, 1000f))
                        continue;

                    player.AddBuff(BuffID.ChaosState, 2);

                    if (InfernalCrossmod.Thorium.Loaded)
                    {
                        ThoriumEffectHandler.GrantThoriumChaosState(player);
                    }

                    if (InfernalCrossmod.SOTS.Loaded)
                    {
                        //SOTSEffectHandler.DisableSOTSEffects(player);
                    }

                    player.AddCooldown(CalamityMod.Cooldowns.ChaosState.ID, CalamityUtils.SecondsToFrames(180));
                }
            }

            return base.PreAI(npc);
        }

        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            //TODO: make cultist not drop two bags
        }
    }

    public class VanillaStatScaling : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ProjectileID.BloodShot,
                ProjectileID.EyeLaser,
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
                if (entity.type == ProjectileID.BloodShot || entity.type == ProjectileID.EyeLaser)
                {
                    entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                }
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            /*
            float damageMod = 1f;

            if (InfernalUtilities.IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (WorldSaveSystem.InfernumModeEnabled || InfernalUtilities.GetFargoDifficullty("MasochistMode"))
            {
                damageMod *= 2.2f;
            }
            else if (InfernalUtilities.GetFargoDifficullty("EternityMode"))
            {
                damageMod *= 1.675f;
            }
            else if (CalamityWorld.death)
            {
                damageMod *= 1.5f;
            }

            modifiers.SourceDamage *= damageMod;
            */
        }
    }
}