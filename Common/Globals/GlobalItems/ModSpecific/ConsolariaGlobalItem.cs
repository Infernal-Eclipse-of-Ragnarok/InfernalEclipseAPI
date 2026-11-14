using CalamityMod;
using static Terraria.ModLoader.ModContent;
using Consolaria.Content.Items.Misc;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Utils;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific
{
    [JITWhenModsEnabled(InfernalCrossmod.Consolaria.Name)]
    [ExtendsFromMod(InfernalCrossmod.Consolaria.Name)]
    public class ConsolariaGlobalItem : GlobalItem
    {
        public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
        {
            if (item.type == ItemType<HornoPlenty>() && !ModLoader.HasMod("HornScale"))
            {
                item.healLife = !DownedBossSystem.downedDoG ? (!DownedBossSystem.downedProvidence ? (!NPC.downedMoonlord ? (!Main.hardMode ? 100 : 150) : 200) : 250) : 300;
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.type == ItemType<HornoPlenty>() && !ModLoader.HasMod("HornScale"))
            {
                item.healLife = !DownedBossSystem.downedDoG ? (!DownedBossSystem.downedProvidence ? (!NPC.downedMoonlord ? (!Main.hardMode ? 100 : 150) : 200) : 250) : 300;
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (InfernalConfig.Instance.ConsolariaBalanceChanges && InfernalCrossmod.Thorium.Loaded)
            {
                Mod console = InfernalCrossmod.Consolaria.Mod;

                if (item.type == console.Find<ModItem>("ViperHelmet").Type)
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.10f;
                }
                if (item.type == console.Find<ModItem>("ViperChestplate").Type)
                {
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.15f;
                }
                if (item.type == console.Find<ModItem>("ViperLegs").Type)
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.10f;
                    player.GetCritChance(DamageClass.Throwing) -= 10;
                }

                if (item.type == console.Find<ModItem>("OldViperHelmet").Type)
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.10f;
                }
                if (item.type == console.Find<ModItem>("OldViperChestplate").Type)
                {
                    player.GetAttackSpeed(DamageClass.Throwing) -= 0.15f;
                }
                if (item.type == console.Find<ModItem>("OldViperLegs").Type)
                {
                    player.GetDamage(DamageClass.Throwing) -= 0.10f;
                    player.GetCritChance(DamageClass.Throwing) -= 10;
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Color InfernalRed = Color.Lerp(
                Color.White,
                new Color(255, 80, 0), // Infernal red/orange
                (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );

            if (item.type == ItemType<HornoPlenty>())
            {
                InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.HornScale"), InfernalRed);
            }
        }
    }
}
