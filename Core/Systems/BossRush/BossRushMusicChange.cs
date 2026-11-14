using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.SupremeCalamitas;

namespace InfernalEclipseAPI.Core.Systems.BossRush
{
    public class BossRushMusicChange : ModSceneEffect
    {
        private bool playOverEverything = true;
        public override bool IsSceneEffectActive(Player player)
        {
            if (playOverEverything)
            {
                return BossRushEvent.BossRushActive;
            }
            else
            {
                return false;
            }
        }
        public override float GetWeight(Player player)
        {
            return 1f;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh + 10;

        public override int Music
        {
            get
            {
                int tier = 1;

                int profanedId = ModContent.NPCType<ProfanedGuardianCommander>();
                int skeletronId = NPCID.SkeletronHead;
                int golemId = NPCID.Golem;
                int calCloneId = ModContent.NPCType<CalamitasClone>();
                int calamitasId = ModContent.NPCType<SupremeCalamitas>();
                int namelessId = 0;
                int goozmaID = 0;
                int mutantID = 0;

                ModLoader.TryGetMod("CalamityMod", out Mod calamity);
                bool wotgOn = ModLoader.TryGetMod("NoxusBoss", out Mod wotg);
                if (wotgOn)
                {
                    namelessId = wotg.Find<ModNPC>("NamelessDeityBoss").Type;
                }
                bool huntOn = ModLoader.TryGetMod("CalamityHunt", out Mod calHunt);
                if (huntOn)
                {
                    goozmaID = calHunt.Find<ModNPC>("Goozma").Type;
                }
                bool fargoDLCOn = ModLoader.TryGetMod("FargowiltasSouls", out Mod fargoSouls) && ModLoader.TryGetMod("FargowiltasCrossMod", out Mod soulsDLC);
                if (fargoDLCOn)
                {
                    mutantID = fargoSouls.Find<ModNPC>("MutantBoss").Type;
                }

                List<(int, int, Action<int>, int, bool, float, int[], int[])> brEntries = (List<(int, int, Action<int>, int, bool, float, int[], int[])>)calamity.Call("GetBossRushEntries");

                for (int i = 0; i < brEntries.Count; i++)
                {
                    if (brEntries[i].Item1 == profanedId)
                        profanedId = i;
                    if (brEntries[i].Item1 == skeletronId)
                        skeletronId = i;
                    if (brEntries[i].Item1 == golemId)
                        golemId = i;
                    if (brEntries[i].Item1 == calCloneId)
                        calCloneId = i;
                    if (brEntries[i].Item1 == calamitasId)
                        calamitasId = i;
                    if (wotgOn && brEntries[i].Item1 == namelessId)
                        namelessId = i;
                }

                if (wotgOn && BossRushEvent.BossRushStage >= namelessId)
                {
                    if (BossRushEvent.BossRushStage > namelessId)
                        tier = 100;
                    else 
                        tier = 42;
                }
                else if (BossRushEvent.BossRushStage > calamitasId)
                {
                    if (fargoDLCOn && BossRushEvent.BossRushStage > mutantID)
                        tier = 100;
                    else if (huntOn && BossRushEvent.BossRushStage > goozmaID)
                        tier = 100;
                    else
                        tier = 6;
                }
                else if (BossRushEvent.BossRushStage > calCloneId)
                {
                    tier = 5;
                }
                else if (BossRushEvent.BossRushStage > golemId)
                {
                    tier = 4;
                }
                else if (BossRushEvent.BossRushStage > skeletronId)
                {
                    tier = 3;
                }
                else if (BossRushEvent.BossRushStage > profanedId)
                {
                    tier = 2;
                }

                if (tier >= 6)
                {
                    if (tier == 100)
                    {
                        return MusicLoader.GetMusicSlot(Mod, "Assets/Music/EnsembleofFools(EncoreMix)");
                    }
                    else if (tier == 42)
                    {
                        return MusicLoader.GetMusicSlot(Mod, "Assets/Music/TWISTEDGARDENRemix");
                    }

                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/tier6");
                }

                if (tier == 5)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/tier5");
                }

                return ModContent.GetInstance<CalamityMod.CalamityMod>().GetMusicFromMusicMod($"BossRushTier{tier}") ?? 0;
            }
        }
    }
}
