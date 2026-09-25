using InfernalEclipseAPI.Common.GlobalNPCs;

namespace InfernalEclipseAPI.Content.Buffs
{
    public class HormonalBlockade : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            InfernalGlobalNPC.ClearRageAndAdrenaline(player);
        }
    }
}
