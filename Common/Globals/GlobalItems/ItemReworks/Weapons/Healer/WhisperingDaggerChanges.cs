using ThoriumMod.Buffs.Healer;
using ThoriumMod;
using InfernalEclipseAPI.Core.Systems;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Weapons.Healer
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class WhisperingDaggerChanges : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod) => !ModLoader.HasMod("WHummusMultiModBalancing");

        public override void PostUpdate()
        {
            if (Player.HasBuff(ModContent.BuffType<WhisperingDaggerBuff>()))
            {
                Player.GetDamage((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance) -= 0.35f;
            }
        }
    }
}
