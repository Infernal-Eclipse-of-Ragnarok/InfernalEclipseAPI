using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.Rarities.InfernumRarities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Content.Items.Weapons.Melee.TheChickenWing
{
    public class TheChickenWing : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 158;
            Item.height = 156;
            Item.damage = 8620;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 23;
            Item.useAnimation = 23;

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;

            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.knockBack = 9f;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<InfernumEggRarity>();

            Item.shoot = ModContent.ProjectileType<ChickenWingHoldout>();
            Item.shootSpeed = 0f;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.HasBuff(BuffID.WellFed))
                damage.Base += 30;

            if (player.HasBuff(BuffID.WellFed2))
                damage.Base += 80;

            if (player.HasBuff(BuffID.WellFed3))
                damage.Base += 130;

            if (InfernalCrossmod.NoxusBoss.Loaded)
            {
                if (player.HasBuff(InfernalCrossmod.NoxusBoss.Mod.Find<ModBuff>("StarstrikinglySatiated").Type))
                {
                    damage.Base += 205;
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type].Equals(0))
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            if (InfernalCrossmod.Consolaria.Loaded)
                recipe.AddIngredient(InfernalCrossmod.Consolaria.Mod.Find<ModItem>("GreatDrumstick").Type);
            recipe.AddIngredient(ItemID.HamBat);
            recipe.AddIngredient(ItemID.ChickenNugget, 600);
            recipe.AddIngredient<YharonSoulFragment>(4);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();
        }
    }
}
