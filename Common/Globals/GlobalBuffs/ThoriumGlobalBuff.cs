using InfernalEclipseAPI.Core.Systems.Compatibility;
using ThoriumMod.Buffs;

namespace InfernalEclipseAPI.Common.Globals.GlobalBuffs
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumGlobalBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == ModContent.BuffType<NinjaBuff>() && !InfernalCrossmod.Hummus.Loaded)
            {
                player.GetAttackSpeed(DamageClass.Throwing) -= 0.1f;
            }
        }
    }
}
