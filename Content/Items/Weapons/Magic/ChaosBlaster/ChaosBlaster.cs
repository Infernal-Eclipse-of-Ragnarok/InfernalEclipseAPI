using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using InfernalEclipseAPI.Content.Items.Materials;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using Microsoft.Xna.Framework;
using NoxusBoss.Content.Rarities;
using NoxusBoss.Content.Tiles;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Magic.ChaosBlaster
{
    [ExtendsFromMod("NoxusBoss")]
    public class ChaosBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.rare = ModContent.RarityType<GenesisComponentRarity>();
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.shoot = ModContent.ProjectileType<BeamChargeUp>(); 
            Item.shootSpeed = 1f;
            Item.channel = true;
            Item.consumable = false;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.DamageType = MythicMagic.Instance;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            ChaosBlasterFlight modPlayer = player.GetModPlayer<ChaosBlasterFlight>();
            int altFunctionUse = player.altFunctionUse;
            return !modPlayer.IsFlying;
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            var modPlayer = player.GetModPlayer<ChaosBlasterFlight>();

            // Right-click (alt use) starts flying instead of shooting
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.None;
                modPlayer.StartFlying();
                return false;
            }

            Item.useStyle = ItemUseStyleID.Shoot;

            // If not flying, fire a beam toward the mouse
            if (!modPlayer.IsFlying)
            {
                int projDamage = (DownedBossSystem.downedBossRush ? 1750 : 500);
                const float projKnockback = 6f;

                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                Projectile.NewProjectile(
                    source,
                    player.Center + dir * 50f,
                    dir,
                    type,
                    projDamage,
                    projKnockback,
                    player.whoAmI
                );
            }

            return false; // no vanilla projectile
        }

        public override void HoldItem(Player player)
        {
            var modPlayer = player.GetModPlayer<ChaosBlasterFlight>();
            bool hasBeam = HasActiveSolynBeam(player);

            // Hold right-click to fly
            if (Main.mouseRight && !Main.mouseRightRelease)
            {
                if (!modPlayer.IsFlying)
                    modPlayer.StartFlying();
            }
            else if (modPlayer.IsFlying)
            {
                modPlayer.StopFlying();
            }

            // Keep item "in use" while flying / channeling / beam active
            if (!(modPlayer.IsFlying || (player.channel && player.itemTime > 0) || hasBeam))
                return;

            float rot = (Main.MouseWorld - player.Center).ToRotation();
            if (player.direction == -1)
                rot += MathHelper.Pi;

            player.itemRotation = rot;

            if (player.itemTime <= 2)
                player.itemTime = 2;

            if (player.itemAnimation <= 2)
                player.itemAnimation = 2;
        }

        private static bool HasActiveSolynBeam(Player player)
        {
            int beamType = ModContent.ProjectileType<ChaosBlasterBeam>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == beamType)
                    return true;
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(x => x.Name.Contains("Damage") || x.Name.Contains("Knockback") || x.Name.Contains("CritChance"));

            int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
            if (index != -1)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "SignatureWeapon", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MythicTooltips.Base", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MythicTooltips.Solyn")))
                {
                    OverrideColor = Color.Cyan
                });
            }

            tooltips.Add(new TooltipLine(Mod, "SolynUsage", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SBeamUsage")));
            tooltips.Add(new TooltipLine(Mod, "SolynFlight", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.SFlight")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AlloyofEden>()
                .AddIngredient(ItemID.FallenStar, 30)
                .AddTile<StarlitForgeTile>()
                .AddCondition(SpellbookGatedRecipe.ConstructRecipeCondition(out Func<bool> condition), condition)
                .DisableDecraft()
                .Register();
        }
    }
}
