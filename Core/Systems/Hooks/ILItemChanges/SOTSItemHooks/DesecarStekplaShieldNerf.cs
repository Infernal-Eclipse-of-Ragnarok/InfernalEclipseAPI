using System.Collections.Generic;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Utils;
using SOTS.Items.ChestItems;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;


namespace InfernalEclipseAPI.Core.Systems.Hooks.ILItemChanges.SOTSItemHooks
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class DesecarStekplaShieldNerf : ModSystem
    {
        private static Hook desecarHook = null;
        private static Hook stekplaHook = null;

        public override void OnModLoad()
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges)
                return;

            Mod sots = ModLoader.GetMod("SOTS");

            Type desecarShield = sots.Code.GetType("SOTS.Items.ChestItems.ShieldofDesecar");
            MethodInfo desecarOrig = desecarShield.GetMethod("UpdateAccessory", BindingFlags.Public | BindingFlags.Instance);
            desecarHook = new Hook(desecarOrig, DesecarUpdateAccessory);

            Type stekplaShield = sots.Code.GetType("SOTS.Items.ChestItems.ShieldofStekpla");
            MethodInfo stekplaOrig = stekplaShield.GetMethod("UpdateAccessory", BindingFlags.Public | BindingFlags.Instance);
            stekplaHook = new Hook(stekplaOrig, StekplaUpdateAccessory);
        }

        public override void OnModUnload()
        {
            desecarHook?.Dispose();
            desecarHook = null;

            stekplaHook?.Dispose();
            stekplaHook = null;
        }

        private static void DesecarUpdateAccessory(Action<ModItem, Player, bool> orig, ModItem self, Player player, bool hideVisual)
        {
            self.Item.defense = 0;
            float shield = 0.0f;
            for (int index = 0; index < 50; ++index)
            {
                if (player.inventory[index].type == ItemID.None)
                    shield += 0.125f;
            }
            self.Item.defense += (int)shield;
        }

        private static void StekplaUpdateAccessory(Action<ModItem, Player, bool> orig, ModItem self, Player player, bool hideVisual)
        {
            int critbonus = 0;
            for (int index = 0; index < 50; ++index)
            {
                if (player.inventory[index].type != ItemID.None)
                    ++critbonus;
            }
            player.GetCritChance(DamageClass.Generic) += critbonus * 0.125f;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class DesecarStekplaGlobal : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.SOTSBalanceChanges;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<ShieldofDesecar>() || entity.type == ModContent.ItemType<ShieldofStekpla>();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<ShieldofDesecar>())
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Desecar"));
            }
            if (item.type == ModContent.ItemType<ShieldofStekpla>())
            {
                InfernalUtilities.FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Stekpla"));
            }
        }
    }
}
