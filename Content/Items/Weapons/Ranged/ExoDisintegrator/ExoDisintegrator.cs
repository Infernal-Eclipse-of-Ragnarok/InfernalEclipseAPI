using System.Collections.Generic;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Content.Items.Weapons.Magic.ChaosBlaster;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using NoxusBoss.Content.Rarities;
using NoxusBoss.Content.Tiles;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Ranged.ExoDisintegrator
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class ExoDisintegrator : ModItem
    {
        public override void SetStaticDefaults() => this.Item.ResearchUnlockCount = 1;
        public override void SetDefaults()
        {
            Item.width = Item.height = 62;
            Item.damage = 2000;
            Item.DamageType = ModContent.GetInstance<MythicRanged>();
            Item.useTime = Item.useAnimation = 200;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<GenesisComponentRarity>();
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DisintegratorChargeUp>();
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num = 180;
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, 0.0f, num, 0.0f);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll((Predicate<TooltipLine>)(tooltip => tooltip.Name == "Damage" || tooltip.Name == "CritChance" || tooltip.Name == "Speed" || tooltip.Name == "Knockback" || tooltip.Name == "UseMana"));

            /*
            int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
            if (index != -1)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "SignatureWeapon", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MythicTooltips.Base", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MythicTooltips.ND")))
                {
                    OverrideColor = Color.Cyan
                });
            }
            */

            tooltips.Add(new TooltipLine(((ModType)this).Mod, "BigCosmicLaserBeam", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MBeamUsage")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChaosBlaster>()
                .AddIngredient<ShadowspecBar>(3)
                .AddIngredient<MiracleMatter>()
                .AddIngredient<Rock>()
                .AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition)
                .AddTile<StarlitForgeTile>()
                .Register();
        }
    }
}
