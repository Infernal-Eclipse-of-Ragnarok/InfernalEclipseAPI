using CalamityMod.Systems;
using InfernalEclipseAPI.Content.UI;
using InfernalEclipseAPI.Core.Configs;
using InfernumMasterPatch;
using InfernumMode.Content.UI;

namespace InfernalEclipseAPI.Core.Systems.UI
{
    internal sealed class CalamityDifficultyListEdits : ModSystem
    {
        public override void PostSetupContent() => Apply();

        // Calamity calls CalculateDifficultyData() again here, so re-apply after world load.
        public override void PostWorldLoad() => Apply();

        private static void Apply()
        {
            if (DifficultyModeSystem.Difficulties is null)
                return;

            if (ModLoader.HasMod("InfernumMasterPatch"))
                MasterPatchDifficultyRemover.ApplyMasterPatchRemoval();

            if (!ShouldHideCustomDifficulties())
                return;

            // Remove from the source list.
            DifficultyModeSystem.Difficulties.RemoveAll(d =>
                d is RagnarokDifficulty ||
                d is InfernumDifficulty);

            // Rebuild tiers + MostAlternateDifficulties + _difficultyTier assignments.
            DifficultyModeSystem.CalculateDifficultyData();
        }

        private static bool ShouldHideCustomDifficulties()
        {
            if (!InfernalConfig.Instance.ThereIsNoReasonDisableThis)
                return false;

            /*
            if (InfernalCrossmod.FargosSouls.Loaded)
                return true;
            */

            if (Main.getGoodWorld)
                return true;

            return false;
        }
    }

    [JITWhenModsEnabled("InfernumMasterPatch")]
    [ExtendsFromMod("InfernumMasterPatch")]
    internal static class MasterPatchDifficultyRemover
    {
        public static void ApplyMasterPatchRemoval()
        {
            DifficultyModeSystem.Difficulties.RemoveAll(d =>
                d is MasterPatchDifficulty);
        }
    }
}
