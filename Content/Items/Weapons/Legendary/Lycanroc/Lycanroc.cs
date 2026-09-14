using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles;
using CalamityMod.Sounds;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using InfernumMode;
using InfernumMode.Content.Rarities.InfernumRarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.Lycanroc
{
    public class Lycanroc : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 36;
            Item.damage = 55;
            Item.DamageType = LegendaryRanged.Instance;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<InfernumProfanedRarity>();
            Item.UseSound = CommonCalamitySounds.LargeWeaponFireSound;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 16f;
            //Item.Calamity().canFirePointBlankShots = true; Renoved in Calamity 2.1
            Item.Infernum_Tooltips().DeveloperItem = true;
            Item.scale = 0.65f;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (NPC.downedMoonlord)
                damage += 2.00f;
            else if (NPC.downedAncientCultist)
                damage += 1.25f;
            else if (NPC.downedGolemBoss)
                damage += 1.15f;
            else if (NPC.downedPlantBoss)
                damage += 0.75f;
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                damage += 0.25f;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (NPC.downedMoonlord)
                crit += 10;
            else if (NPC.downedAncientCultist)
                crit += 10;
            else if (NPC.downedGolemBoss)
                crit += 10;
            else if (NPC.downedPlantBoss)
                crit += 10;
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                crit += 10;
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            if (NPC.downedMoonlord)
                knockback += 3.75f;
            else if (NPC.downedAncientCultist)
                knockback += 3.65f;
            else if (NPC.downedGolemBoss)
                knockback += 2.75f;
            else if (NPC.downedPlantBoss)
                knockback += 2.5f;
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                knockback += 2;
            else if (Main.hardMode)
                knockback += 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && type == ProjectileID.Bullet) type = ProjectileID.BulletHighVelocity;

            if (player.HasBuff<RageMode>() && NPC.downedPlantBoss)
            {
                Item.useTime = 8;
                Item.useAnimation = 8;
            }
            else
            {
                Item.useTime = 18;
                Item.useAnimation = 18;
            }

            Projectile shot = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            shot.DamageType = LegendaryRanged.Instance;
            CalamityGlobalProjectile cgp = shot.Calamity();
            LycanrocGlobalProjectile lgp = shot.GetGlobalProjectile<LycanrocGlobalProjectile>();

            if (NPC.downedAncientCultist)
            {
                int superCrit = Main.rand.Next(1, 21);
                if (superCrit == 20 || (NPC.downedMoonlord && superCrit >= 15))
                {
                    cgp.supercritHits = -1;
                }
            }
            if (NPC.downedGolemBoss && player.HasBuff<AdrenalineMode>()) cgp.forcedCrit = true;
            if (NPC.downedMoonlord) lgp.appliesArmorCrunch = true;
            else if (Main.hardMode) lgp.appliesCrumbling = true;

            return false;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        // Make the gun have visible recoil when fired for extra cool factor. - thanks calamity
        #region Firing Animation
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(40, 20);
            Vector2 itemOrigin = new Vector2(-15, 1);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(15, -10);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.45f * (float)Math.Pow((0.4f - animProgress) / 0.4f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        #endregion

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Color lerpedColor = Color.Lerp(Color.White, new Color(30, 144, 255), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5));

            TooltipLine line4 = new(Mod, "Lore", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Lore"));
            tooltips.Add(line4);

            if (!NPC.downedMoonlord)
            {
                TooltipLine line3 = new(Mod, "Progression2", Language.GetTextValue("Mods.InfernalEclipseAPI.LegendaryTooltip.Base"));
                line3.OverrideColor = lerpedColor;
                tooltips.Add(line3);
            }

            TooltipLine line = new(Mod, "Progression", GetProgressionTooltip());
            line.OverrideColor = lerpedColor;
            tooltips.Add(line);

            Color color = CalamityUtils.ColorSwap(Color.OrangeRed, Color.DarkRed, 2f);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Yob"))}");
                line5.OverrideColor = color;
                tooltips.Add(line5);
            }
        }

        private string GetProgressionTooltip()
        {
            if (NPC.downedMoonlord)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.Full");
            if (NPC.downedAncientCultist)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.MoonLord");
            if (NPC.downedGolemBoss)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.Cultist");
            if (NPC.downedPlantBoss)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.Golem");
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.Plantera");
            if (Main.hardMode)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.Mechs");
            return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Lycanroc.Progression.WoF");
        }

        public override void AddRecipes()
        {
            Recipe lycanrocRecipe = Recipe.Create(ModContent.ItemType<Lycanroc>());
            lycanrocRecipe.AddIngredient(ItemID.PhoenixBlaster);
            lycanrocRecipe.AddIngredient<CrackshotColt>();

            if (ModLoader.TryGetMod("SOTS", out Mod sots))
            {
                lycanrocRecipe.AddIngredient(sots.Find<ModItem>("OtherworldlyAlloy"), 10);
            }
            else
            {
                lycanrocRecipe.AddIngredient(ItemID.MeteoriteBar, 10);
            }

            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok))
            {
                lycanrocRecipe.AddIngredient(ragnarok.Find<ModItem>("StrangeAlienMotherBoard"));
            }
            else
            {
                lycanrocRecipe.AddIngredient<MysteriousCircuitry>(10);
            }

            lycanrocRecipe.AddIngredient<PurifiedGel>(5);
            lycanrocRecipe.AddTile(TileID.Anvils);
            lycanrocRecipe.Register();
        }
    }
}
