using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Melee.Destinary
{
    public class Destinary : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 196;
            Item.height = 102;
            Item.damage = 4250;
            Item.knockBack = 36f;
            Item.useTime = 5;
            Item.useAnimation = 25;
            Item.axe = 5000 / 5;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DestinaryProjectile>();
            Item.shootSpeed = 1f;

            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 18;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool AltFunctionUse(Player player) => true;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float breakBlocks = 1;
            // If right clicking, the chainsaw won't be able to chop down trees
            if (player.Calamity().mouseRight && player.whoAmI == Main.myPlayer && !Main.mapFullscreen && !Main.blockMouse)
            {
                breakBlocks = 0;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f, breakBlocks);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Eduarrdo"))}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Contributor")}");
                line5.OverrideColor = new(50, 205, 50);
                tooltips.Add(line5);
            }
            else
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Contributor"));
                line5.OverrideColor = new(50, 205, 50);
                tooltips.Add(line5);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<PhotonRipper>();
            recipe.AddIngredient<RefractionRotor>();
            recipe.AddIngredient(ItemID.Zenith);
            recipe.AddIngredient<ShadowspecBar>(5);

            if (InfernalCrossmod.SOTS.Loaded)
            {
                recipe.AddIngredient(InfernalCrossmod.SOTS.Mod.Find<ModItem>("SoulOfPlight"), 5);
            }
            if (InfernalCrossmod.Consolaria.Loaded)
            {
                recipe.AddIngredient(InfernalCrossmod.Consolaria.Mod.Find<ModItem>("SoulofBlight").Type, 5);
            }
            if (InfernalCrossmod.Thorium.Loaded)
            {
                recipe.AddIngredient(InfernalCrossmod.Thorium.Mod.Find<ModItem>("SoulofPlight"), 5);
            }

            recipe.AddTile<DraedonsForge>();
            recipe.Register();
        }
    }
}
