using CalamityMod;
using CalamityMod.Buffs.Alcohol;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.Globals.GlobalBuffs
{
    public class InfernalGlobalBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            /*
            if ((type == ModContent.BuffType<GrapeBeerBuff>() || type == ModContent.BuffType<MoonshineBuff>()) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                player.Calamity().alcoholPoisonLevel++;
                player.GetCritChance(DamageClass.Generic) -= 30f;
            }
            */

            if (InfernalCrossmod.NoxusBoss.Loaded && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (type == InfernalCrossmod.NoxusBoss.Mod.Find<ModBuff>("StarstrikinglySatiated").Type)
                {
                    player.GetAttackSpeed<MeleeDamageClass>() -= 0.125f;
                    player.moveSpeed -= 0.25f;
                    player.pickSpeed -= 0.05f;
                }
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Mod thorium = InfernalCrossmod.Thorium.Mod;

                if (type == thorium.Find<ModBuff>("Bubbled").Type)
                {
                    player.AddBuff(BuffID.Suffocation, 2);

                    if (player.mount.Active)
                        player.mount.Dismount(player);
                }

                if (type == thorium.Find<ModBuff>("FrenzyPotionBuff").Type)
                {
                    if (InfernalCrossmod.SOTS.Loaded)
                        player.ClearBuff(thorium.Find<ModBuff>("FrenzyPotionBuff").Type);
                    else
                        player.GetAttackSpeed(DamageClass.Generic) -= 0.03f;
                }

                if (type == thorium.Find<ModBuff>("KineticPotionBuff").Type)
                {
                    if (InfernalCrossmod.SOTS.Loaded)
                        player.ClearBuff(thorium.Find<ModBuff>("KineticPotionBuff").Type);
                }

                if (type == thorium.Find<ModBuff>("BloodRush").Type)
                    player.moveSpeed -= 0.15f;
            }

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                Mod console = InfernalCrossmod.Consolaria.Mod;

                if (type == console.Find<ModBuff>("Drunk").Type)
                {
                    player.Calamity().alcoholPoisonLevel++;
                }
            }
        }
    }
}
