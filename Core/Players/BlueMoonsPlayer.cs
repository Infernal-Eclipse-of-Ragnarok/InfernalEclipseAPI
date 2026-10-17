using System.Reflection;
using static InfernalEclipseAPI.Core.Systems.Compatibility.InfernalCrossmod;

namespace InfernalEclipseAPI.Core.Players
{
    public class BlueMoonsPlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return BlueMoon.Loaded;
        }

        public override void PostUpdateBuffs()
        {
            UpdateFloralBlessing();
            UpdateMintyFreshness();
        }

        public void UpdateFloralBlessing()
        {
            Type floralType = BlueMoon.Mod.Code.GetType("BlueMoon.Buffs.FloralBlessingPlayer");
            if (floralType == null)
                return;

            MethodInfo getModPlayerMethod = typeof(Player).GetMethod("GetModPlayer", Type.EmptyTypes);
            if (getModPlayerMethod == null)
                return;

            MethodInfo generic = getModPlayerMethod.MakeGenericMethod(floralType);

            object floralPlayer = generic.Invoke(Player, null);
            if (floralPlayer == null)
                return;

            FieldInfo hasFloralField = floralType.GetField("hasFloralBlessing", BindingFlags.Instance | BindingFlags.Public);
            if (hasFloralField == null)
                return;

            bool hasFloral = (bool)hasFloralField.GetValue(floralPlayer);
            if (!hasFloral)
                return;

            int lifeRegenValue = (Main.hardMode ? 1 : 2);
            Player player = Player;
            player.lifeRegen += lifeRegenValue;
        }

        public void UpdateMintyFreshness()
        {
            Type mintyType = BlueMoon.Mod.Code.GetType("BlueMoon.Buffs.MintyFreshnessPlayer");
            if (mintyType == null)
                return;

            MethodInfo getModPlayerMethod = typeof(Player).GetMethod("GetModPlayer", Type.EmptyTypes);
            if (getModPlayerMethod == null)
                return;

            MethodInfo generic = getModPlayerMethod.MakeGenericMethod(mintyType);

            object mintyPlayer = generic.Invoke(Player, null);
            if (mintyPlayer == null)
                return;

            FieldInfo hasMintyField = mintyType.GetField("hasMintyFreshness", BindingFlags.Instance | BindingFlags.Public);
            if (hasMintyField == null)
                return;

            bool hasMinty = (bool)hasMintyField.GetValue(mintyPlayer);
            if (!hasMinty)
                return;

            Player.moveSpeed -= 0.13f;
            Player.GetDamage(DamageClass.Melee) -= 0.2f;
            Player.GetDamage(DamageClass.Ranged) -= 0.2f;
            Player.GetDamage(DamageClass.Magic) -= 0.2f;
            Player.GetDamage(DamageClass.Summon) -= 0.2f;

            Player.GetDamage(DamageClass.Generic) += 0.07f;
        }
    }
}
