using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.ModSceneEffects.RagnarokMusic
{
    public class HorribleHogMusicHandler : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override int Music
        {
            get
            {
                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                    return MusicLoader.GetMusicSlot(thorium, "Sounds/Music/Stronger_Foe");
                else if (ModLoader.TryGetMod("InfernumModeMusic", out Mod musicMod))
                    return MusicLoader.GetMusicSlot(musicMod, "Sounds/Music/Minibosses"); 
                else 
                    return -1;
            }
        }

        public override bool IsSceneEffectActive(Player player)
        {
            if (BossRushEvent.BossRushActive || !InfernalCrossmod.Thorium.Loaded || !ModLoader.HasMod("InfernumModeMusic"))
                return false;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active && npc.type == ModContent.NPCType<HorribleHog>())
                    return true;
            }

            return false;
        }
    }
}
