using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.Localization;
using ThoriumMod.Buffs;
using ThoriumMod.Items;
using ThoriumMod.Items.Donate;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Accessories
{
    // Thorium's Mana Injection Kit caps its decaying magic damage at 40% (600 ticks)
    // Supreme Mana Potions now raise that cap to 50% (750 ticks)
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ManaInjectionKitSupremeTier : GlobalItem
    {
        public const int SupremeMaxBuffTime = 750;

        public static int SupremeMaxPercentage => SupremeMaxBuffTime / ManaInjectionKit.TimeLeftScaleFactor;

        private delegate void orig_ManaPotionEffects(Item item, int healerLife, int healerMana, Player player);

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<SupremeManaPotion>() || entity.type == ModContent.ItemType<ManaInjectionKit>();
        }

        public override void Load()
        {
            // Both the normal use path and the quick mana path go through this method
            MethodInfo manaPotionEffects = typeof(ThoriumGlobalItem).GetMethod("ManaPotionEffects", BindingFlags.NonPublic | BindingFlags.Static);
            if (manaPotionEffects != null)
                MonoModHooks.Add(manaPotionEffects, ManaPotionEffects_Detour);
            else
                Mod.Logger.Warn("ManaInjectionKitSupremeTier: ThoriumGlobalItem.ManaPotionEffects not found");

            MethodInfo drawSelf = typeof(ThoriumGlobalItem).Assembly.GetType("ThoriumMod.UI.ResourceBars.ManaInjectionKitGauge")?.GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance);
            if (drawSelf != null)
                MonoModHooks.Modify(drawSelf, IL_GaugeDrawSelf);
            else
                Mod.Logger.Warn("ManaInjectionKitSupremeTier: ManaInjectionKitGauge.DrawSelf not found");
        }

        private static void ManaPotionEffects_Detour(orig_ManaPotionEffects orig, Item item, int healerLife, int healerMana, Player player)
        {
            int buffType = ModContent.BuffType<ManaInjectionKitBuff>();
            int buffIndex = player.FindBuffIndex(buffType);
            int timeBefore = buffIndex > -1 ? player.buffTime[buffIndex] : 0;

            orig(item, healerLife, healerMana, player);

            buffIndex = player.FindBuffIndex(buffType);
            if (!player.GetThoriumPlayer().accManaInjectionKit || buffIndex == -1)
                return;

            if (item.type == ModContent.ItemType<SupremeManaPotion>())
            {
                int buffTimeIncrease = ThoriumGlobalItem.GetManaInjectionKitManaPotionCost(item) * 10;
                player.buffTime[buffIndex] = Math.Max(player.buffTime[buffIndex], Math.Min(timeBefore + buffTimeIncrease, SupremeMaxBuffTime));
            }
            // Thorium clamps the total back down to 600, so lower tier potions would otherwise erase the Supreme bonus
            else if (player.buffTime[buffIndex] < timeBefore)
            {
                player.buffTime[buffIndex] = timeBefore;
            }
        }

        // The gauge's fill and "+X% / Y%" text use the Supreme cap instead of Thorium's 40%
        private void IL_GaugeDrawSelf(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(i => i.MatchLdsfld<ManaInjectionKit>(nameof(ManaInjectionKit.MaxBuffTime))))
            {
                c.Next.OpCode = OpCodes.Ldc_I4;
                c.Next.Operand = SupremeMaxBuffTime;
            }
            else
            {
                Mod.Logger.Warn("ManaInjectionKitSupremeTier: MaxBuffTime not found in ManaInjectionKitGauge.DrawSelf");
            }

            c.Index = 0;
            if (c.TryGotoNext(i => i.MatchLdsfld<ManaInjectionKit>(nameof(ManaInjectionKit.MaxPercentage))))
            {
                c.Next.OpCode = OpCodes.Ldc_I4;
                c.Next.Operand = SupremeMaxPercentage;
            }
            else
            {
                Mod.Logger.Warn("ManaInjectionKitSupremeTier: MaxPercentage not found in ManaInjectionKitGauge.DrawSelf");
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<SupremeManaPotion>())
            {
                // Thorium's own line is clamped to 40%, rebuild it with the Supreme cap
                TooltipLine injectionLine = tooltips.Find(t => t.Mod == "ThoriumMod" && t.Name == "InjectionText");
                if (injectionLine == null)
                    return;

                int manaCost = ThoriumGlobalItem.GetManaInjectionKitManaPotionCost(item);
                int percentageValue = Math.Min((int)Ceiling(manaCost * 10f / ManaInjectionKit.TimeLeftScaleFactor), SupremeMaxPercentage);
                Color color = new Color(100, 200, 250) * (Main.mouseTextColor / 255f);
                injectionLine.Text = ManaInjectionKit.ManaPotionTooltipText.Format(manaCost, Terraria.Utils.Hex3(color), percentageValue);
                return;
            }

            int index = tooltips.FindLastIndex(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
            if (index == -1)
                return;

            string text = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.ManaInjectionKitSupreme", Lang.GetItemNameValue(ModContent.ItemType<SupremeManaPotion>()), SupremeMaxPercentage);
            tooltips.Insert(index + 1, new TooltipLine(Mod, "SupremeManaPotionTier", text));
        }
    }
}
