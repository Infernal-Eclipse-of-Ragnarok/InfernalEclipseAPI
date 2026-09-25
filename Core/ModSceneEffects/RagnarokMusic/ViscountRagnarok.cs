using CalamityMod.Events;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using ThoriumMod.NPCs.BossViscount;

namespace InfernalEclipseAPI.Core.ModSceneEffects.RagnarokMusic
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ViscountRagnarok : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh + 5;

        public override int Music => MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/VampiricVitality");

        public override bool IsSceneEffectActive(Player player)
        {
            if (BossRushEvent.BossRushActive || !InfernalConfig.Instance.ViscountRagnarok || !InfernalWorld.RagnarokModeEnabled)
                return false;

            return NPC.AnyNPCs(ModContent.NPCType<Viscount>());
        }
    }
}
