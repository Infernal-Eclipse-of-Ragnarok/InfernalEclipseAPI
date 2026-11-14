using System.Collections.Generic;
using CalamityMod.Prefixes;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Terraria.Localization;
using ThoriumMod;

namespace InfernalEclipseAPI.Core.Prefixes
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class DauntlessPrefixAdjustment : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                return;

            int dauntlessCount = CountDauntlessAccessories(Player);

            if (dauntlessCount <= 0)
                return;

            Player.GetDamage<HealerDamage>() += 0.02f * dauntlessCount;
        }

        private static int CountDauntlessAccessories(Player player)
        {
            int count = 0;
            int dauntlessPrefix = ModContent.PrefixType<Dauntless>();

            int functionalSlots = 5 + player.extraAccessorySlots;

            for (int i = 3; i < 3 + functionalSlots; i++)
            {
                if (i < 0 || i >= player.armor.Length)
                    break;

                Item item = player.armor[i];

                if (item == null || item.IsAir || !item.accessory)
                    continue;

                if (item.prefix == dauntlessPrefix)
                    count++;
            }

            return count;
        }
    }

    public class DauntlessTooltipOverride : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.accessory && InfernalCrossmod.Thorium.Loaded;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.prefix != ModContent.PrefixType<Dauntless>())
                return;

            for (int i = 0; i < tooltips.Count; i++)
            {
                TooltipLine line = tooltips[i];

                if (line.Mod == "CalamityMod" && line.Name == "CalamityMod:PrefixMaxLifeBoost")
                {
                    line.Text += $"\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DauntlessBuff")}";
                    break;
                }
            }
        }
    }
}
