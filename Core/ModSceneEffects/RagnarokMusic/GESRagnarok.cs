using CalamityMod.Events;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using ThoriumMod.NPCs.BossGraniteEnergyStorm;

namespace InfernalEclipseAPI.Core.ModSceneEffects.RagnarokMusic
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class GESRagnarok : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh + 5;

        public override int Music => MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/Stonewarden");

        public override bool IsSceneEffectActive(Player player)
        {
            if (BossRushEvent.BossRushActive || !InfernalConfig.Instance.GESRagnarok || !InfernalWorld.RagnarokModeEnabled)
                return false;

            return NPC.AnyNPCs(ModContent.NPCType<GraniteEnergyStorm>());
        }
    }
}
