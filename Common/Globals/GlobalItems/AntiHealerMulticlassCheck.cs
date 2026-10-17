using InfernalEclipseAPI.Content.Buffs;
using ThoriumMod;
using InfernalEclipseAPI.Core.Players.ThoriumPlayerOverrides.ThoriumMulticlassNerf;
using InfernalEclipseAPI.Core.DamageClasses;
using InfernalEclipseAPI.Core.Configs;
using ThoriumMod.Utilities;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class AntiHealerMulticlassCheck : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if (!InfernalConfig.Instance.NerfThoriumMulticlass) return base.UseItem(item, player);
            if (item is null || item.type == ItemID.None) return base.UseItem(item, player);

            var anti = player.GetModPlayer<ThoriumMulticlassPlayerNerfs>();
            bool healerUse = IsHealerWeaponOrTool(item);

            // Using a healer weapon/tool during penalty window => apply debuff (keeps -10 healing active)
            if (healerUse && anti.switchToHealerPenaltyTimer > 0)
                player.AddBuff(ModContent.BuffType<BrokenOath>(), anti.switchToHealerPenaltyTimer);

            // Using a non-healer combat item refreshes the window even if no hit connects
            if (!healerUse && ThoriumMulticlassPlayerNerfs.IsCombatWeapon(item))
                anti.switchToHealerPenaltyTimer = ThoriumMulticlassPlayerNerfs.PenaltyDuration;

            return base.UseItem(item, player);
        }

        public static void ZeroHealBonus(NPC npc)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.active || player.dead || player.ghost)
                    continue;

                if (!npc.WithinRange(player.Center, 10000f))
                    continue;

                player.GetThoriumPlayer().healBonus = -25;
            }
        }

        private static bool IsHealerWeaponOrTool(Item item)
        {
            if (item is null) return false;

            if (InfernalCrossmod.SOTSBardHealer.Loaded)
            {
                if (item.CountsAsClass<VoidRogue>()) return false;
            }

            if (item.CountsAsClass<HealerDamage>() || item.CountsAsClass<HealerToolDamageHybrid>() || item.CountsAsClass<HealerTool>()) return true;

            return false;
        }
    }
}
