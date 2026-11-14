using System.Collections.Generic;
using CalamityMod.Items;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using NoxusBoss.Content.Rarities;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using InfernalEclipseAPI.Content.Items.Weapons.Magic.ChaosBlaster;
using InfernalEclipseAPI.Content.Items.Materials;
using NoxusBoss.Content.Items;
using NoxusBoss.Content.Tiles;
using CalamityMod.Items.Placeables;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Weapons.Nameless.NebulaGigabeam
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class NebulaGigabeam : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 38;
            Item.damage = 6000;
            Item.DamageType = MythicMagic.Instance;
            Item.useTime = 200;
            Item.useAnimation = 200;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<NamelessDeityRarity>();
            Item.autoReuse = false;
            Item.shootSpeed = 1f;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<NebulaChargeUp>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0.0f, 180f, 0.0f);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(tooltip => tooltip.Name == "Damage" || tooltip.Name == "CritChance" || tooltip.Name == "Speed" || tooltip.Name == "Knockback" || tooltip.Name == "UseMana");

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

            tooltips.Add(new TooltipLine(Mod, "BigCosmicLaserBeam", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.NBeamUsage")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChaosBlaster>()
                .AddIngredient<PrimordialOrchid>(5)
                .AddIngredient<Rock>()
                .AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition)
                .AddTile<StarlitForgeTile>()
                .Register();
        }
    }
}
