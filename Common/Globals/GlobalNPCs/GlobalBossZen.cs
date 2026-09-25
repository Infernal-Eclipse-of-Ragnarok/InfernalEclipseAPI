using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Common.Globals.GlobalNPCs
{
    public class GlobalBossZen : GlobalNPC
    {
        private static readonly HashSet<string> ThoriumBossNames = new()
        {
            "TheGrandThunderBird",
            "QueenJellyfish",
            "Viscount",
            "GraniteEnergyStorm",
            "BuriedChampion",
            "StarScouter",
            "BoreanStrider",
            "BoreanStriderPopped",
            "FallenBeholder",
            "Lich",
            "LichHeadless",
            "ForgottenOne",
            "ForgottenOneCracked",
            "ForgottenOneReleased",
            "DreamEater",
            "Omnicide",
            "SlagFury",
            "Aquaius",
            "PatchWerk",
            "CorpseBloom",
            "Illusionist",
        };

        private static readonly HashSet<string> SOTSBossNames = new()
        {
            "SubspaceSerpentHead"
        };

        private static readonly HashSet<string> ConsolariaBossNames = new()
        {
            "Lepus",
            "TurkorTheUngrateful",
            "Ocram"
        };

        public override void PostAI(NPC npc)
        {
            if (!npc.active || !npc.boss || npc.ModNPC == null || !CalamityServerConfig.Instance.BossZen)
                return;

            if ((npc.ModNPC.Mod.Name == "ThoriumMod" && ThoriumBossNames.Contains(npc.ModNPC.Name)) ||
                (npc.ModNPC.Mod.Name == "Consolaria" && ConsolariaBossNames.Contains(npc.ModNPC.Name)) ||
                (npc.ModNPC.Mod.Name == "SOTS" && SOTSBossNames.Contains(npc.ModNPC.Name))
                )
            {
                ApplyBossEffects(npc);
            }
        }

        public static void ApplyBossEffects(NPC npc)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead)
                    continue;

                if (Vector2.Distance(player.Center, npc.Center) < (npc.ModNPC.Name == "SubspaceSerpentHead" ? 12000 : 6400f))
                {
                    // give at least 1 second to confirm it’s being applied
                    player.AddBuff(ModContent.BuffType<BossEffects>(), 60);
                }
            }
        }
    }
}
