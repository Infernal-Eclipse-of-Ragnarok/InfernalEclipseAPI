using InfernalEclipseAPI.Core.Systems.Compatibility;
using ThoriumRework;

namespace InfernalEclipseAPI.Content.Buffs
{
    [ExtendsFromMod("ThoriumMod")]
    public class BrokenOath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public static class LunateCharmChecker
    {
        public static bool PlayerHasLunateCharm(Player player) => player.GetModPlayer<ThoriumPlayer>().lunateCharm; 
    }
}
