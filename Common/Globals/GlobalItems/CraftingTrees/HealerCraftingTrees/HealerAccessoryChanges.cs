using System.Collections.Generic;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.GlobalItems.CraftingTrees.HealerCraftingTrees
{
    public class HealerAccessoryChanges : GlobalItem
    {
        private Mod CalBardHealer
        {
            get
            {
                ModLoader.TryGetMod("CalamityBardHealer", out Mod calbh);
                return calbh;
            }
        }

        private Mod SOTSBardHealer
        {
            get
            {
                ModLoader.TryGetMod("SOTSBardHealer", out Mod sotsbh);
                return sotsbh;
            }
        }

        private Mod ThoriumRework
        {
            get
            {
                ModLoader.TryGetMod("ThoriumRework", out Mod thorRe);
                return thorRe;
            }
        }
        private Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thor);
                return thor;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees)
                return;

            if (CalBardHealer != null)
            {
                if (item.type == CalBardHealer.Find<ModItem>("ElementalBloom").Type)
                {
                    ModItem soulGuard = thorium.Find<ModItem>("SoulGuard");

                    soulGuard.UpdateAccessory(player, hideVisual);
                }
            }

            if (ThoriumRework != null)
            {
                if (ThoriumRework.TryFind("SealedContract", out ModItem sc))
                {
                    if (item.type == sc.Type)
                    {
                        thorium.Call(new object[3]
                        {
                        "BonusHealerHealBonus",
                        player,
                        -3
                        });
                        player.statLifeMax2 -= 60;

                        if (SOTSBardHealer != null)
                        {
                            ModItem serpentsTongue = SOTSBardHealer.Find<ModItem>("SerpentsTongue");

                            serpentsTongue.UpdateAccessory(player, hideVisual);
                        }
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!InfernalConfig.Instance.MergeCraftingTrees)
                return;

            Color InfernalRed = Color.Lerp(
                Color.White,
                new Color(255, 80, 0), // Infernal red/orange
                (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );

            if (CalBardHealer != null)
            {
                if (item.type == CalBardHealer.Find<ModItem>("ElementalBloom").Type)
                {
                    InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.ThoriumMod.Items.SoulGuard.Tooltip", 5, 15), InfernalRed);
                }
            }

            if (ThoriumRework != null)
            {
                if (ThoriumRework.TryFind("SealedContract", out ModItem sealedContract))
                {
                    if (item.type == sealedContract.Type)
                    {
                        foreach (TooltipLine tooltip in tooltips)
                        {
                            if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SealedContract.OrigTooltip1")))
                            {
                                if (SOTSBardHealer != null)
                                {
                                    tooltip.Text = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SerpentsTongue");
                                    tooltip.OverrideColor = new Color?(InfernalRed);
                                }
                                else
                                {
                                    //tooltip.Text = null;
                                }
                            }
                            if (tooltip.Text.Contains(Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.SealedContract.OrigTooltip2")))
                            {
                                tooltip.Text = Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MergedCraftingTreeTooltip.ContractNerf");
                            }
                        }
                    }
                }
                
            }
        }
    }
}
