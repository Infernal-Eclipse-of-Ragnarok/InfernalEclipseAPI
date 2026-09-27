using CalamityMod;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
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
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.DeveloperMode;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.damage = 100;
            Item.useTime = Item.useAnimation = 19;
            Item.knockBack = 3f;
            Item.mana = 10;
            //Item.buffType
            Item.shoot = ModContent.ProjectileType<FiendsmithsRequiemPro>();

            Item.autoReuse = true;
            Item.noMelee = true;
            Item.DamageType = LegendarySummon.Instance;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item8;
            Item.rare = ModContent.RarityType<InfernumProfanedRarity>();
            //Item.value =
        }

        public override bool CanUseItem(Player player)
        {
            return player.maxMinions >= 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //player.AddBuff(Item.buffType, 2);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FiendsmithsRequiemPro>()] == 1)
            {
                return false;
            }
            
            var minion = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Color lerpedColor = Color.Lerp(Color.White, new Color(30, 144, 255), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5));

            if (!DownedBossSystem.downedPolterghast)
            {
                TooltipLine line3 = new(Mod, "Progression2", Language.GetTextValue("Mods.InfernalEclipseAPI.LegendaryTooltip.Base"))
                {
                    OverrideColor = lerpedColor
                };
                tooltips.Add(line3);
            }

            /*
            TooltipLine line = new(Mod, "Progression", GetProgressionTooltip());
            line.OverrideColor = lerpedColor;
            tooltips.Add(line);
            */

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Bomber"))}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Donor")}");
                line5.OverrideColor = new Color(196, 35, 44);
                tooltips.Add(line5);
            }
            else
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Playtester"));
                line5.OverrideColor = new Color(196, 35, 44);
                tooltips.Add(line5);
            }
        }
    }
}
