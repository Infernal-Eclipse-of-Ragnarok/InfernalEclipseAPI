using CalamityBardHealer;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Utils.ConfigSetup
{
    [JITWhenModsEnabled(InfernalCrossmod.CalBardHealer.Name)]
    [ExtendsFromMod(InfernalCrossmod.CalBardHealer.Name)]
    public static class CalBardHealerConfigSetup
    {
        public static void DisableGenItemBalance()
        {
            ModContent.GetInstance<BalanceConfig>().generalItemBalanceChanges = false;
        }
    }
}
