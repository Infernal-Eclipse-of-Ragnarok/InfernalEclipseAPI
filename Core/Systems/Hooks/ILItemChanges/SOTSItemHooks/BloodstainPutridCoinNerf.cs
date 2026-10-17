using System.Collections.Generic;
using System.Reflection;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using MonoMod.RuntimeDetour;
using SOTS;
using SOTS.Items.CritBonus;
using SOTS.Items.Gems;
using Terraria.Localization;

namespace InfernalEclipseAPI.Core.Systems.ILItemChanges
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class BloodstainPutridCoinNerf : ModSystem
    {
        private static Hook bloodstainHook = null;
        private static Hook putridHook = null;
        private static Hook polishedHook = null;

        public override void OnModLoad()
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges)
                return;

            Mod sots = ModLoader.GetMod("SOTS");

            Type bloodstainCoin = sots.Code.GetType("SOTS.Items.CritBonus.BloodstainedCoin");
            MethodInfo bloodOrig = bloodstainCoin.GetMethod("UpdateAccessory",  BindingFlags.Public | BindingFlags.Instance);
            bloodstainHook = new Hook(bloodOrig, BloodstainUpdateAccessory);

            Type putridCoin = sots.Code.GetType("SOTS.Items.CritBonus.PutridCoin");
            MethodInfo putridOrig = putridCoin.GetMethod("UpdateAccessory", BindingFlags.Public | BindingFlags.Instance);
            putridHook = new Hook(putridOrig, PutridUpdateAccessory);

            Type polishedCoin = sots.Code.GetType("SOTS.Items.CritBonus.PolishedCoin");
            MethodInfo polishedOrig = polishedCoin.GetMethod("UpdateAccessory", BindingFlags.Public | BindingFlags.Instance);
            polishedHook = new Hook(polishedOrig, PolishedUpdateAccessory);
        }

        public override void OnModUnload()
        {
            bloodstainHook?.Dispose();
            bloodstainHook = null;

            putridHook?.Dispose();
            putridHook = null;

            polishedHook?.Dispose();
            polishedHook = null;
        }

        private static void BloodstainUpdateAccessory(Action<ModItem, Player, bool> orig, ModItem self, Player player, bool hideVisual)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            InfernalPlayer infernalPlayer = player.GetModPlayer<InfernalPlayer>();

            infernalPlayer.bloodstainedCoin = true;
            if (Terraria.Utils.NextBool(Main.rand, 2))
                sotsPlayer.CritBonusDamage += 10;
        }

        private static void PutridUpdateAccessory(Action<ModItem, Player, bool> orig, ModItem self, Player player, bool hideVisual)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            InfernalPlayer infernalPlayer = player.GetModPlayer<InfernalPlayer>();

            infernalPlayer.putridCoin = true;
            if (Terraria.Utils.NextBool(Main.rand, 2))
                sotsPlayer.CritBonusDamage += 10;
        }

        private static void PolishedUpdateAccessory(Action<ModItem, Player, bool> orig, ModItem self, Player player, bool hideVisual)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            player.GetCritChance(DamageClass.Generic) += 2f;

            if (Terraria.Utils.NextBool(Main.rand, 2))
                sotsPlayer.CritBonusDamage += 15;

            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class BloodstainPutridGlobal : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.SOTSBalanceChanges;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<PutridCoin>() || entity.type == ModContent.ItemType<BloodstainedCoin>()  || entity.type == ModContent.ItemType<PolishedCoin>()
                || entity.type == ModContent.ItemType<DiamondRing>() || entity.type == ModContent.ItemType<ChallengerRing>();
        }
        private static void FullTooltipOveride(List<TooltipLine> tooltips, string stealthTooltip)
        {
            for (int index = 0; index < tooltips.Count; ++index)
            {
                if (tooltips[index].Mod == "Terraria")
                {
                    if (tooltips[index].Name == "Tooltip0")
                    {
                        TooltipLine tooltip = tooltips[index];
                        tooltip.Text = $"{stealthTooltip}";
                    }
                    else if (tooltips[index].Name.Contains("Tooltip"))
                    {
                        tooltips[index].Hide();
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<PutridCoin>())
            {
                FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PutridCoin"));
            }
            if (item.type == ModContent.ItemType<BloodstainedCoin>())
            {
                FullTooltipOveride(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.BloodstainedCoin"));
            }
            if (item.type == ModContent.ItemType<PolishedCoin>())
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PolishedCoin.Orig"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PolishedCoin.Nerf"));
            }

            if (item.type == ModContent.ItemType<DiamondRing>())
            {
                int previousDefense = SOTSPlayer.ModPlayer(Main.LocalPlayer).previousDefense;
                FullTooltipOveride(tooltips, $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DiamondRingText", Convert.ToString(previousDefense), Convert.ToString((int)(previousDefense * 0.66)))}");
            }
            if (item.type == ModContent.ItemType<ChallengerRing>())
            {
                foreach (TooltipLine tooltip in tooltips)
                {
                    if (tooltip.Text.Contains("Increase damage based on defense, then decreases defense by a third of the damage buff")) // im sorry localizers there was no other way...
                    {
                        int previousDefense = SOTSPlayer.ModPlayer(Main.LocalPlayer).previousDefense;
                        tooltip.Text = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DiamondRingText", Convert.ToString(previousDefense), Convert.ToString((int)(previousDefense * 0.66)));
                    }
                }
            }
        }
    }
}
