using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Systems;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.World;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.CnidrionBoss
{
    public class CnidrionMusicSceneRagnarok : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Seahorse");

        public override bool IsSceneEffectActive(Player player)
        {
            if (!InfernalWorld.RagnarokModeEnabled || !InfernalConfig.Instance.CnidrionMusic || BossRushEvent.BossRushActive || InfernalGlobalNPC.cnidrion == -1)
                return false;

            if (CalamityUtils.AnyBossNPCS() || Main.bloodMoon)
                return false;

            return (NPC.AnyNPCs(ModContent.NPCType<Cnidrion>()));
        }

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
