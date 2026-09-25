using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using InfernalEclipseAPI.Core.Systems;
using InfernumMode.Content.Rarities.InfernumRarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Melee.TyrantYharimsUltisword
{
    public class TyrantYharimsUltisword : CustomUseProjItem
    {
        public static readonly SoundStyle SwingSound = new("CalamityMod/Sounds/Item/HellkiteSwing", 2);
        public static readonly SoundStyle SwingSoundBig = new("CalamityMod/Sounds/Item/HellkiteHeavySwing");
        public static readonly SoundStyle HitSoundSmall = new("CalamityMod/Sounds/Item/HellkiteSmallHit", 3);
        public static readonly SoundStyle HitSoundBig = new("CalamityMod/Sounds/Item/HellkiteBigHit", 2);
        public static readonly SoundStyle ChargeSound = new("CalamityMod/Sounds/Item/HellkiteCharge");
        public static readonly SoundStyle FullChargeSound = new("CalamityMod/Sounds/Item/HellkiteFullCharge");
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Venom];
        }

        public override void SetDefaults()
        {
            Item.width = 140;
            Item.height = 140;
            Item.damage = 3750;
            Item.DamageType = ModContent.GetInstance<MythicMelee>();
            Item.useAnimation = Item.useTime = 45;
            Item.useTurn = true;
            Item.knockBack = 15f;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<InfernumVassalRarity>(); 
            Item.autoReuse = true;

            Item.crit = 41;

            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<TyrantYharimsUltiswordHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool MeleePrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().mouseRight)
            {
                Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, 0, 0, 5);
            }
            else
                Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, 0, 0, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            //Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/HellkiteGlow").Value);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
            if (index != -1)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "SignatureWeapon", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MythicTooltips.Base", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MythicTooltips.Yharim")))
                {
                    OverrideColor = Color.Cyan
                });
            }
        }

        public override void AddRecipes()
        {
            Recipe ultisword = CreateRecipe();
            ultisword.AddIngredient<DefiledGreatsword>();
            ultisword.AddIngredient<Hellkite>();
            ultisword.AddIngredient<ShadowspecBar>(8);
            ultisword.AddIngredient<AuricBar>(InfernalCrossmod.Thorium.Loaded ? 7 : 10);
            if (InfernalCrossmod.Thorium.Loaded)
                ultisword.AddIngredient(InfernalCrossmod.Thorium.Mod.Find<ModItem>("InfernoEssence"), 3);
            ultisword.AddTile<DraedonsForge>();
            ultisword.Register();
        }
    }
}