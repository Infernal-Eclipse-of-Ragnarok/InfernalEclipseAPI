using InfernalEclipseAPI.Content.Items.Weapons.Legendary.FiendsmithsRequiem;
using InfernalEclipseAPI.Core.Players;

namespace InfernalEclipseAPI.Content.Buffs
{
    public class FiendsmithsRequiemBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FiendsmithsRequiemPro>()] > 0)
            {
                player.GetModPlayer<InfernalPlayer>().fiendsmithRequiemSummon = true;
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                player.GetModPlayer<InfernalPlayer>().fiendsmithParadise = false;
                buffIndex--;
            }
        }
    }
}
