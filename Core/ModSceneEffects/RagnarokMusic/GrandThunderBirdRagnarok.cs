using CalamityMod.Events;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.World;
using ThoriumMod.NPCs.BossTheGrandThunderBird;

namespace InfernalEclipseAPI.Core.ModSceneEffects.RagnarokMusic
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class GrandThunderBirdRagnarok : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh + 5;

        public override int Music => MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/InfernalThunderBird");

        public override bool IsSceneEffectActive(Player player)
        {
            if (BossRushEvent.BossRushActive || !InfernalConfig.Instance.GrandThunderBirdRagnarok || !InfernalWorld.RagnarokModeEnabled)
                return false;

            return NPC.AnyNPCs(ModContent.NPCType<TheGrandThunderBird>());
        }
    }
}
