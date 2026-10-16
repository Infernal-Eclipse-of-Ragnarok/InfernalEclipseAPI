using System.Collections.Generic;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using SOTS.Achievements;
using SOTS.Items.Fragments;
using SOTS.Void;
using Terraria.Localization;

namespace InfernalEclipseAPI.Core.Players.SOTSPlayerOverrides
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class DissolvingElementsOverrides : ModPlayer
    {
        public bool PolarizeNature;
        public bool PolarizeEarth;
        public bool PolarizeAurora;
        public bool PolarizeAether;
        public bool PolarizeDeluge;
        public bool PolarizeNether;
        public bool PolarizeBrilliance;

        public int DissolvingNature;
        public int DissolvingEarth;
        public int DissolvingAurora;
        public int DissolvingAether;
        public int DissolvingDeluge;
        public int DissolvingNether;
        public int DissolvingBrilliance;

        public override void UpdateBadLifeRegen()
        {
            NetherEffects();
            DissolvingNether = 0;
        }

        public override void ResetEffects()
        {
            if (DissolvingNature > 0 && DissolvingEarth > 0 && DissolvingAurora > 0 && DissolvingAether > 0 && DissolvingDeluge > 0 && DissolvingNether > 0 && DissolvingBrilliance > 0 &&
                !PolarizeNature && !PolarizeEarth && !PolarizeAurora && !PolarizeAether && !PolarizeDeluge && !PolarizeNether && !PolarizeBrilliance)
            {
                Main.NewText(true, new());
                Burdened instance = ModContent.GetInstance<Burdened>();
                if (!instance.BurdenedEntirely.IsCompleted)
                    instance.BurdenedEntirely.Complete();
            }

            if (DissolvingAether != 0)
                AetherEffects();
            NatureEffects();
            EarthEffects();
            AuroraEffects();
            if (DissolvingDeluge != 0)
                DelugeEffects();
            BrillianceEffects();

            PolarizeNature = false;
            PolarizeEarth = false;
            PolarizeAurora = false;
            PolarizeAether = false;
            PolarizeDeluge = false;
            PolarizeNether = false;
            PolarizeBrilliance = false;

            DissolvingNature = 0;
            DissolvingEarth = 0;
            DissolvingAurora = 0;
            DissolvingAether = 0;
            DissolvingDeluge = 0;
            DissolvingBrilliance = 0;
        }

        public void NatureEffects()
        {
            if (PolarizeNature)
            {
                if (DissolvingNature > 4)
                    DissolvingNature = 4;

                if (DissolvingNature >= 2)
                    Player.lifeRegen += 1;
                if (DissolvingNature == 4)
                    Player.lifeRegen += 1;
            }
            else
            {
                Player.GetDamage(DamageClass.Generic) -= 0.1f * DissolvingNature;
            }
        }

        public void EarthEffects()
        {
            if (PolarizeEarth)
            {
                if (DissolvingEarth > 4)
                    DissolvingEarth = 4;
                Player.statDefense += DissolvingEarth;
            }
            else
                Player.endurance -= 0.1f * MathHelper.Clamp(DissolvingEarth, 0, 20f);
        }

        public void AuroraEffects()
        {
            if (PolarizeAurora)
            {
                if (DissolvingAurora > 4)
                    DissolvingAurora = 4;
                Player.moveSpeed -= DissolvingAurora * 0.01f;
            }
            else
                Player.moveSpeed -= 0.2f * MathHelper.Clamp(DissolvingAurora, 0, 5f);
        }

        public void DelugeEffects()
        {
            if (PolarizeDeluge)
            {
                if (DissolvingDeluge > 4)
                    DissolvingDeluge = 4;
                Player.GetDamage(DamageClass.Ranged) += DissolvingDeluge * 0.02f;
            }
            else
            {
                Player.statLifeMax2 = (int)MathHelper.Clamp(Player.statLifeMax2 - DissolvingDeluge * 10, 100f, Player.statLifeMax2);
                Player.statManaMax2 = (int)MathHelper.Clamp(Player.statManaMax2 - DissolvingDeluge * 10, 20f, Player.statManaMax2);
            }
        }

        public void AetherEffects()
        {
            if (PolarizeAether)
            {
                if (DissolvingAether > 4)
                    DissolvingAether = 4;
                Player.GetDamage(DamageClass.Magic) += DissolvingAether * 0.02f;
            }
            else
            {
                float gravity = Player.gravity;
                float maxFallSpeed = Player.maxFallSpeed;
                float jumpSpeedBoost = Player.jumpSpeedBoost;
                float num1 = (float)(1.0 - 1.0 / (0.30000001192092896 * DissolvingAether + 1.0));
                float num2 = (float)(1.0 - 1.0 / (0.30000001192092896 * DissolvingAether + 1.0));
                float num3 = gravity - 1f * num1;
                float num4 = maxFallSpeed - 10f * num2;
                float num5 = jumpSpeedBoost + 5f * num1;
                if ((double)num5 > 5.0)
                    num5 = 5f;
                if ((double)num3 < 0.125)
                    num3 = 0.125f;
                if ((double)num4 < 1.75)
                    num4 = 1.75f;
                if (Player.gravity > (double)num3)
                    Player.gravity = num3;
                if (Player.maxFallSpeed > (double)num4)
                    Player.maxFallSpeed = num4;
                if (Player.jumpSpeedBoost < (double)num5)
                    Player.jumpSpeedBoost = num5;
                if (DissolvingAether < 4)
                    return;
                Player.noFallDmg = true;
            }
        }

        public void NetherEffects()
        {
            if (PolarizeNether)
            {
                if (DissolvingNether > 4)
                    DissolvingNether = 4;

                Player.GetDamage(DamageClass.Melee) += DissolvingNether * 0.02f;
            }
            else
            {
                if (DissolvingNether > 10)
                    DissolvingNether = 10;
                Player.lifeRegen -= DissolvingNether * 2;
            }
        }

        public void BrillianceEffects()
        {
            if (PolarizeBrilliance)
            {
                if (DissolvingBrilliance > 4)
                    DissolvingBrilliance = 4;
                Player.GetDamage(DamageClass.Summon) += DissolvingBrilliance * 0.02f;
            }
            else
                VoidPlayer.ModPlayer(Player).flatVoidRegen -= 0.5f * DissolvingBrilliance;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class DissolvingElementsGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            DissolvingElementsPlayer sotsPlayer = DissolvingElementsPlayer.ModPlayer(player);
            DissolvingElementsOverrides overrides = player.GetModPlayer<DissolvingElementsOverrides>();

            if (item.type == ModContent.ItemType<WorldlyPolarizer>())
            {
                overrides.PolarizeNature = true;
                overrides.PolarizeEarth = true;
                overrides.PolarizeDeluge = true;
            }

            if (item.type == ModContent.ItemType<ThermalPolarizer>())
            {
                overrides.PolarizeAurora = true;
                overrides.PolarizeNether = true;
            }

            if (item.type == ModContent.ItemType<ExoticPolarizer>())
            {
                overrides.PolarizeAether = true;
                overrides.PolarizeBrilliance = true;
            }

            if (item.type == ModContent.ItemType<UltimatePolarizer>())
            {
                overrides.PolarizeNature = true;
                overrides.PolarizeEarth = true;
                overrides.PolarizeAurora = true;
                overrides.PolarizeAether = true;
                overrides.PolarizeDeluge = true;
                overrides.PolarizeNether = true;
                overrides.PolarizeBrilliance = true;
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            DissolvingElementsPlayer sotsPlayer = DissolvingElementsPlayer.ModPlayer(player);
            DissolvingElementsOverrides overrides = player.GetModPlayer<DissolvingElementsOverrides>();

            if (item.type == ModContent.ItemType<DissolvingNature>())
            {
                sotsPlayer.DissolvingNature -= item.stack;
                overrides.DissolvingNature += item.stack;
            }
            if (item.type == ModContent.ItemType<DissolvingEarth>())
            {
                sotsPlayer.DissolvingEarth -= item.stack;
                overrides.DissolvingEarth += item.stack;
            }
            if (item.type == ModContent.ItemType<DissolvingAurora>())
            {
                sotsPlayer.DissolvingAurora -= item.stack;
                overrides.DissolvingAurora += item.stack;
            }
            if (item.type == ModContent.ItemType<DissolvingAether>())
            {
                sotsPlayer.DissolvingAether -= item.stack;
                overrides.DissolvingAether += item.stack;
            }
            if (item.type == ModContent.ItemType<DissolvingDeluge>())
            {
                sotsPlayer.DissolvingDeluge -= item.stack;
                overrides.DissolvingDeluge += item.stack;
            }
            if (item.type == ModContent.ItemType<DissolvingNether>())
            {
                sotsPlayer.DissolvingNether -= item.stack;
                overrides.DissolvingNether += item.stack;
            }
            if (item.type == ModContent.ItemType<DissolvingBrilliance>())
            {
                sotsPlayer.DissolvingBrilliance -= item.stack;
                overrides.DissolvingBrilliance += item.stack;
            }

            if (item.type == ModContent.ItemType<PrecariousCluster>())
            {
                sotsPlayer.DissolvingAether -= item.stack;
                sotsPlayer.DissolvingEarth -= item.stack;
                sotsPlayer.DissolvingAurora -= item.stack;
                sotsPlayer.DissolvingNature -= item.stack;

                overrides.DissolvingAether += item.stack;
                overrides.DissolvingEarth += item.stack;
                overrides.DissolvingAurora += item.stack;
                overrides.DissolvingNature += item.stack;
            }
            if (item.type == ModContent.ItemType<TerminalCluster>())
            {
                sotsPlayer.DissolvingNether -= item.stack;
                sotsPlayer.DissolvingDeluge -= item.stack;
                sotsPlayer.DissolvingBrilliance -= item.stack;

                overrides.DissolvingNether += item.stack;
                overrides.DissolvingDeluge += item.stack;
                overrides.DissolvingBrilliance += item.stack;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem is DissolvingElement DE)
            {
                if (item.type == ModContent.ItemType<DissolvingNature>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.NatureFlipped"));
                if (item.type == ModContent.ItemType<DissolvingEarth>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.EarthFlipped"));
                if (item.type == ModContent.ItemType<DissolvingAurora>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.AuroraFlipped"));
                if (item.type == ModContent.ItemType<DissolvingAether>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.AetherFlipped"));
                if (item.type == ModContent.ItemType<DissolvingDeluge>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.DelugeFlipped"));
                if (item.type == ModContent.ItemType<DissolvingNether>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.NetherFlipped"));
                if (item.type == ModContent.ItemType<DissolvingBrilliance>())
                    InfernalUtilities.ReplaceTooltip(tooltips, DE.PolarizeToolTip, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.BrillianceFlipped"));
            }

            if (item.type == ModContent.ItemType<PrecariousCluster>())
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.PrecariousClusterTextList.1"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.NatureFlipped"));
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.PrecariousClusterTextList.3"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.EarthFlipped"));
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.PrecariousClusterTextList.5"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.AuroraFlipped"));
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.PrecariousClusterTextList.7"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.AetherFlipped"));
            }
            if (item.type == ModContent.ItemType<TerminalCluster>())
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.TerminalClusterTextList.1"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.DelugeFlipped"));
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.TerminalClusterTextList.5"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.NetherFlipped"));
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.SOTS.TerminalClusterTextList.7"), Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DissolvingElements.BrillianceFlipped"));
            }
        }
    }
}
