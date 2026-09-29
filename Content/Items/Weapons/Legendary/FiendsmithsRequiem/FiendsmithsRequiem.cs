using CalamityMod;
using CalamityMod.Items;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Content.Cooldowns;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using InfernalEclipseAPI.Core.Players;
using InfernumMode.Content.Rarities.InfernumRarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.FiendsmithsRequiem
{
    public class FiendsmithsRequiem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.damage = 90;
            Item.useTime = Item.useAnimation = 19;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.buffType = ModContent.BuffType<FiendsmithsRequiemBuff>();
            Item.shoot = ModContent.ProjectileType<FiendsmithsRequiemPro>();

            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = LegendarySummon.Instance;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item8;
            Item.rare = ModContent.RarityType<InfernumProfanedRarity>();
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;

            Item.shootSpeed = 16f; //bullet velocity
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (DownedBossSystem.downedPolterghast)
                damage += 1.30f;
            else if (DownedBossSystem.downedProvidence)
                damage += 1.20f;
            else if (DownedBossSystem.downedDragonfolly)
                damage += 1.00f;
            else if (DownedBossSystem.downedAstrumDeus)
                damage += 0.15f;
            else if (DownedBossSystem.downedRavager)
                damage += 0.15f;
        }

        public static int FiendSmithsRequiemDamage(int damage)
        {
            if (DownedBossSystem.downedPolterghast)
                return (int)(damage * 2.30f);

            if (DownedBossSystem.downedProvidence)
                return (int)(damage * 2.20f);

            if (DownedBossSystem.downedDragonfolly)
                return (int)(damage * 2.00f);

            if (DownedBossSystem.downedAstrumDeus)
                return (int)(damage * 1.15f);

            if (DownedBossSystem.downedRavager)
                return (int)(damage * 1.15f);

            return damage;
        }

        public override bool CanUseItem(Player player) =>player.maxMinions >= 2;
        public override bool AltFunctionUse(Player player) => player.GetModPlayer<InfernalPlayer>().fiendsmithParadise && player.GetModPlayer<InfernalPlayer>().fiendsmithParadiseCooldown <= 0 && DownedBossSystem.downedDragonfolly;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && player.GetModPlayer<InfernalPlayer>().fiendsmithParadiseCooldown <= 0 && DownedBossSystem.downedDragonfolly)
            {
                player.GetModPlayer<InfernalPlayer>().fiendsmithParadiseCooldown = 60 * 18;
                player.AddCooldown(ParadisePower.ID, 60 * 18);

                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.owner != player.whoAmI || projectile.type != ModContent.ProjectileType<FiendsmithsRequiemPro>())
                        continue;

                    if (projectile.ModProjectile is FiendsmithsRequiemPro requiem)
                    {
                        requiem.StartLightningBarrage();
                        projectile.netUpdate = true;
                    }

                    break;
                }

                return false;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<FiendsmithsRequiemPro>()] > 0)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<FiendsmithsTremendae>()] > 0)
                {
                    if (DownedBossSystem.downedDragonfolly)
                        player.GetModPlayer<InfernalPlayer>().fiendsmithParadise = true;

                    return false;
                }
                else if (DownedBossSystem.downedAstrumDeus)
                {
                    player.GetModPlayer<InfernalPlayer>().fiendsmithParadise = false;
                    type = ModContent.ProjectileType<FiendsmithsTremendae>();
                    knockback = 5f;
                }
                else
                {
                    player.GetModPlayer<InfernalPlayer>().fiendsmithParadise = false;
                    return false;
                }
            }
            else
            {
                player.GetModPlayer<InfernalPlayer>().fiendsmithParadise = false;
            }

            player.AddBuff(ModContent.BuffType<FiendsmithsRequiemBuff>(), 3600);

            var minion = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Color lerpedColor = Color.Lerp(Color.White, new Color(30, 144, 255), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5));

            if (DownedBossSystem.downedAstrumDeus)
                tooltips.Add(new(Mod, "Tremendae", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Tremendae")));

            if (DownedBossSystem.downedDragonfolly)
                tooltips.Add(new(Mod, "Tremendae", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Paradise")));

            if (!DownedBossSystem.downedPolterghast)
            {
                TooltipLine line3 = new(Mod, "Progression2", Language.GetTextValue("Mods.InfernalEclipseAPI.LegendaryTooltip.Base"))
                {
                    OverrideColor = lerpedColor
                };
                tooltips.Add(line3);
            }

            TooltipLine line = new(Mod, "Progression", GetProgressionTooltip());
            line.OverrideColor = lerpedColor;
            tooltips.Add(line);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Bomber"))}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Donor")}");
                line5.OverrideColor = new Color(196, 35, 44);
                tooltips.Add(line5);
            }
            else
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Donor"));
                line5.OverrideColor = new Color(196, 35, 44);
                tooltips.Add(line5);
            }
        }

        private string GetProgressionTooltip()
        {
            if (DownedBossSystem.downedPolterghast)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Progression.Full");
            if (DownedBossSystem.downedProvidence)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Progression.Polterghast");
            if (DownedBossSystem.downedDragonfolly)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Progression.Providence");
            if (DownedBossSystem.downedAstrumDeus)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Progression.Dragonfolly");
            if (DownedBossSystem.downedRavager)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Progression.Deus");
            return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.FiendsmithsRequiem.Progression.Ravager");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpookyWood, 40)
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
