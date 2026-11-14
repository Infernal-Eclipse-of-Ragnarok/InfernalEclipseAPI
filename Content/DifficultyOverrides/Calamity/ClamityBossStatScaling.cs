using System.Reflection;
using Clamity.Content.Bosses.Clamitas.NPCs;
using Clamity.Content.Bosses.Pyrogen.NPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Terraria.DataStructures;
using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity
{
    [JITWhenModsEnabled(InfernalCrossmod.Clamity.Name)]
    [ExtendsFromMod(InfernalCrossmod.Clamity.Name)]
    public class ClamityBossStatScaling : GlobalNPC
    {
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
        private bool IsWorldLegendary()
        {
            FieldInfo findInfo = typeof(Main).GetField("_currentGameModeInfo", BindingFlags.Static | BindingFlags.NonPublic);
            GameModeData data = (GameModeData)findInfo.GetValue(null);
            return Main.getGoodWorld && data.IsMasterMode;
        }
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return ((ModType)npc.ModNPC)?.Mod.Name == "Clamity";
        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == ModContent.NPCType<ClamitasBoss>() && npc.defense == 9999)
                npc.defense = 50;
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (npc.boss && npc.type != ModContent.NPCType<PyrogenBoss>() && npc.type != ModContent.NPCType<PyrogenShield>())
            {
                if (npc.type != ModContent.NPCType<ClamitasBoss>())
                {
                    npc.lifeMax += npc.lifeMax;
                }
                else if (npc.type == ModContent.NPCType<ClamitasBoss>())
                {
                    npc.lifeMax -= (int)(npc.lifeMax * 0.66);
                }

                if (IsWorldLegendary())
                {
                    npc.lifeMax += (int)(0.1 * npc.lifeMax);
                }
                if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
                {
                    npc.lifeMax += (int)((double).35 * npc.lifeMax);
                }
                else
                {
                    if (GetFargoDifficullty("EternityMode"))
                    {
                        npc.lifeMax += (int)(0.25 * npc.lifeMax);
                    }
                }
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (npc.type != ModContent.NPCType<PyrogenBoss>() && npc.type != ModContent.NPCType<PyrogenShield>())
                modifiers.SourceDamage *= 2.0f;

            if (IsWorldLegendary())
            {
                npc.lifeMax += (int)(0.1 * npc.lifeMax);
            }
            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                modifiers.SourceDamage *= 1.35f;
            }
            else
            {
                if (GetFargoDifficullty("EternityMode"))
                {
                    modifiers.SourceDamage *= 1.25f;
                }
            }
        }

        public override void AI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<ClamitasBoss>())
            {
                if (npc.defense == 9999)
                    npc.defense = 50;

                if (npc.defense == 35)
                    npc.defense = 25;
            }
        }

        public override void PostAI(NPC npc)
        {
            ModNPC modNPC14 = npc.ModNPC;
            if (!((modNPC14 != null ? modNPC14.Name.Contains("ClamitasBoss") ? 1 : 0 : 0) != 0) && npc.type != ModContent.NPCType<PyrogenBoss>())
            {
                npc.position += npc.velocity * 0.1f;

                if (IsWorldLegendary())
                {
                    npc.position += 0.1f * npc.velocity;
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
                }
            }
        }
    }
}