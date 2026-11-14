using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace InfernalEclipseAPI.Core.Configs
{
    public class InfernalConfig : ModConfig
    {
        public static InfernalConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;
        #region IEoR Main
        [Header("Main")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool AutomatedConfigSetup { get; set; }

        [DefaultValue(true)]
        public bool DisplayWorldEntryMessages { get; set; }

        [DefaultValue(true)]
        public bool ForceMenu { get; set; }

        [DefaultValue(false)]
        [ReloadRequired]
        public bool ColoredRelics { get; set; }
        #endregion

        #region Music
        [Header("Music")]

        [DefaultValue(true)]
        public bool CnidrionMusic { get; set; }

        /*
        [DrawTicks]
        [DefaultValue(MothershipSong.Infernum)]
        public MothershipSong MothershipMusic { get; set; }
        */

        [DefaultValue(true)]

        public bool GrandThunderBirdRagnarok { get; set; }

        [DefaultValue(true)]
        public bool QueenJellyfishRagnarok { get; set; }

        [DefaultValue(true)]
        public bool ViscountRagnarok { get; set; }

        [DefaultValue(true)]
        public bool GESRagnarok { get; set; }

        [DefaultValue(true)]
        public bool BereftVassalRagnarok { get; set; }

        [DrawTicks]
        [DefaultValue(DoGSong.On)]
        public DoGSong DoGRagnarok { get; set; }

        [DefaultValue(true)]
        public bool EnableCodebreakerInterlude { get; set; }

        [DefaultValue(true)]
        public bool EnableInterlude4 { get; set; }
        #endregion

        #region Compatibility Changes
        [Header("CompatibilityChanges")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool MergeCraftingTrees { get; set; }

        [DefaultValue(true)]
        public bool ChanageWeaponClasses { get; set; }

        [DefaultValue(true)]
        public bool ForceRagnarokInfernumModeInSubworlds { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool NerfThoriumMulticlass { get; set; }

        [DefaultValue(true)]
        public bool MaxVoidOnRespawn { get; set; }

        [DefaultValue(true)]
        public bool SOTSThrowerToRogue { get; set; }
        #endregion

        #region Balance Changes
        [Header("BalanceChanges")]

        [DefaultValue(true)]
        public bool VanillaBalanceChanges { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AdditonalVanillaBossAdjustments { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool CalamityBalanceChanges { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool BossKillCheckOnOres { get; set; }

        [DefaultValue(false)]
        [ReloadRequired]
        public bool CalamityRecipeTweaks { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool BloodOrbPotionDuplication { get; set; }

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableBloodOrbPotions { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool InfernumRecipeTweaks { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool ThoriumBalanceChangess { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool SOTSBalanceChanges { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool ConsolariaBalanceChanges { get; set; }
        #endregion

        #region Boss Rush Changes
        [Header("BossRushChanges")]

        /*
        [DefaultValue(true)]
        [ReloadRequired]
        public bool WarMachineBossInBossRush { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool DreadnautillusInBossRush { get; set; }

        [DefaultValue(false)]
        [ReloadRequired]
        public bool BetsyInBossRush { get; set; }
        */

        [DefaultValue(true)]
        [ReloadRequired]
        public bool TerraBladeBossInBossRush { get; set;}

        [DefaultValue(true)]
        [ReloadRequired]
        public bool WrathoftheGodsBossesInBossRush { get; set; }

        [DefaultValue(false)]
        public bool ForceFullXerocDialogue { get; set; }
        #endregion

        #region Multiplayer Adjustments
        [Header("MultiplayerAdjustments")]
        [DefaultValue(false)]
        public bool InfernumModeForced { get; set; }

        [DefaultValue(60)]
        public int MultiplayerBossRespawnTimer {  get; set; }

        [DrawTicks]
        [DefaultValue(Difficulty.Infernum)]
        public Difficulty MinimumDifficultyToPreventRespawns { get; set; }

        [DefaultValue(1)]
        public int MultiplayerRespawnsAllowed {  get; set; }

        #endregion

        #region Miscellaneous
        [Header ("Miscellaneous")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool DisableUnfinisedContent { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool DisableDuplicateContent { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool DisableUnnecessaryContent { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool MoveDeerclopsChecklistEntry { get; set; }
        #endregion

        #region Secret
        [Header("Secret")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DeveloperMode { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool ThereIsNoReasonDisableThis { get; set; }

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DontEnableThis { get; set; }
        #endregion
    }

    public enum DoGSong : byte
    {
        Off,
        DesperationOnly,
        On
    }

    public enum MothershipSong : byte
    {
        Vanilla,
        WarMachine,
        Thorium,
        Infernum
    }

    public enum Difficulty : byte
    {
        AlwaysOff,
        AlwaysOn,
        Expert,
        Master,
        Legendary,
        Revengence,
        Death,
        Infernum,
        Ragnarok
    }
}
