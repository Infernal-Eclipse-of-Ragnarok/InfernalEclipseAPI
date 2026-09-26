using System.Collections.Generic;
using CalamityMod.Buffs.StatDebuffs;
using InfernalEclipseAPI.Core.Configs;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.GlobalItems
{
    // Wardrobe Hummus
    public class MeleeDebuffs : GlobalItem
    {
        public bool WHummusEnabled = ModLoader.HasMod("WHummusMultiModBalancing");

        public override bool InstancePerEntity => true;

        private bool healedThisSwing;

        public override void HoldItem(Item item, Player player)
        {
            // When a new swing begins, reset
            if (player.itemAnimation == item.useAnimation)
            {
                healedThisSwing = false;
            }
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!WHummusEnabled && ModLoader.TryGetMod("ThoriumMod", out Mod thoriumMod) && InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                var thunderTalon = thoriumMod.Find<ModItem>("ThunderTalon");
                if (thunderTalon != null && item.type == thunderTalon.Type)
                {
                    target.AddBuff(BuffID.Electrified, 180);
                }

                if (item.type == ModLoader.GetMod("ThoriumMod")?.Find<ModItem>("LifeQuartzClaymore")?.Type)
                {
                    if (player.statLife >= player.statLifeMax2)
                    {
                        return;
                    }

                    int bonusHealing = 0;

                    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                    {
                        object result = thorium.Call("GetHealerHealBonus", player);

                        if (result is int healBonus)
                        {
                            bonusHealing = healBonus;
                        }
                    }

                    float healAmount = 1 + bonusHealing;

                    if (!healedThisSwing)
                    {
                        healAmount += 2;
                    }

                    if (healAmount > player.lifeSteal)
                    {
                        healAmount = player.lifeSteal;
                    }
                    if (player.lifeSteal < 0)
                    {
                        healAmount += player.lifeSteal;

                        if (healAmount <= 0)
                        {
                            return;
                        }

                        if (healAmount > player.statLifeMax2 - player.statLife)
                        {
                            healAmount = player.statLifeMax2 - player.statLife;
                        }
                    }

                    Projectile.NewProjectile(player.GetSource_ItemUse(item), target.Center, Vector2.Zero, ProjectileID.VampireHeal, 0, 0f, player.whoAmI, player.whoAmI, healAmount, 0f);
                    player.lifeSteal -= healAmount;

                    healedThisSwing = true;

                    return;
                }

                // Thorium HereticBreaker
                if (item.type == ModLoader.GetMod("ThoriumMod")?.Find<ModItem>("HereticBreaker")?.Type)
                {
                    HealPlayer(player, 1);
                    if (!healedThisSwing)
                    {
                        HealPlayer(player, 2);
                        healedThisSwing = true;
                    }
                    return;
                }

                // Thorium Terrarian's Last Knife
                if (item.type == ModLoader.GetMod("ThoriumMod")?.Find<ModItem>("TerrariansLastKnife")?.Type && ModLoader.TryGetMod("CalamityMod", out Mod calamity2))
                {
                    target.AddBuff(calamity2.Find<ModBuff>("Laceration")?.Type ?? -1, 180);
                    return;
                }
            }

            if (ModLoader.TryGetMod("SOTS", out Mod sots) && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                var blazingClub = sots.Find<ModItem>("BlazingClub");
                if (blazingClub != null && item.type == blazingClub.Type)
                {
                    target.AddBuff(BuffID.OnFire, 180);
                }

                if (item.type == sots.Find<ModItem>("IrradiatedChainReactor").Type)
                {
                    target.AddBuff(ModContent.BuffType<Irradiated>(), 60 * 3);
                }
            }

            if (!WHummusEnabled && item.type == ItemID.LucyTheAxe && InfernalConfig.Instance.VanillaBalanceChanges)
            {
                target.AddBuff(ModContent.BuffType<Crumbling>(), 180);
            }

            if (ModLoader.TryGetMod("Miscellanaria", out Mod misc) && InfernalConfig.Instance.VanillaBalanceChanges)
            {
                var icemourne = misc.Find<ModItem>("Icemourne");
                var scythe = misc.Find<ModItem>("Scythe");
                var soulscythe = misc.Find<ModItem>("SoulScythe");
                if (icemourne != null && item.type == icemourne.Type)
                {
                    target.AddBuff(BuffID.Frostburn2, 180);
                }
                if (scythe != null && item.type == scythe.Type)
                {
                    HealPlayer(player, 1);
                }
                if (soulscythe != null && item.type == soulscythe.Type)
                {
                    HealPlayer(player, 2);
                    target.AddBuff(BuffID.ShadowFlame, 180);
                }
            }
        }

        private void HealPlayer(Player player, int healAmount)
        {
            player.statLife += healAmount;
            player.HealEffect(healAmount, true);
        }

        public void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip, bool InfernalRedActive = false)
        {
            if (!WHummusEnabled)
            {
                Color InfernalRed = Color.Lerp(
               Color.White,
               new Color(255, 80, 0), // Infernal red/orange
               (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );

                int maxTooltipIndex = -1;
                int maxNumber = -1;

                // Find the TooltipLine with the highest TooltipX name
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.StartsWith("Tooltip"))
                    {
                        if (int.TryParse(tooltips[i].Name.Substring(7), out int num) && num > maxNumber)
                        {
                            maxNumber = num;
                            maxTooltipIndex = i;
                        }
                    }
                }

                // If found, insert a new TooltipLine right after it with the desired color
                if (maxTooltipIndex != -1)
                {
                    int insertIndex = maxTooltipIndex + 1;
                    TooltipLine customLine = new TooltipLine(Mod, "StealthTooltip", stealthTooltip);
                    if (InfernalRedActive)
                        customLine.OverrideColor = InfernalRed;

                    tooltips.Insert(insertIndex, customLine);
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!InfernalConfig.Instance.ThoriumBalanceChangess) return;

            if (WHummusEnabled) return;

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thoriumMod))
            {
                var lifeQuartz = thoriumMod.Find<ModItem>("LifeQuartzClaymore");
                if (lifeQuartz != null && item.type == lifeQuartz.Type)
                {
                    // Remove the existing tooltip
                    tooltips.RemoveAll(t => t.Text.Contains("Steals 1 life"));

                    // Add your custom tooltip
                    tooltips.Add(new TooltipLine(Mod, "CustomTooltip", "Steals 3 life"));
                }
            }

            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok))
            {
                if (item.type == ragnarok.Find<ModItem>("MarbleScythe").Type)
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.HolyGlare"));
                }
            }
        }
    }
}
