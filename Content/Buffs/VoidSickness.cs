using InfernalEclipseAPI.Core.Systems;

namespace InfernalEclipseAPI.Content.Buffs
{
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    public class VoidSickness2 : ModBuff
    {
        public override string Texture => "SOTS/Buffs/VoidSickness";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            base.ModifyBuffText(ref buffName, ref tip, ref rare);
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] += (5 * 60);
            if (player.buffTime[buffIndex] > 900)
                player.buffTime[buffIndex] = 900;
            return true;
        }
    }
}
