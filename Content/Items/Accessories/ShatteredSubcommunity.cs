using CalamityMod;
using System.Collections.Generic;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Clamity;
using Clamity.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Linq;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Accessories
{
    [JITWhenModsEnabled(InfernalCrossmod.Clamity.Name)]
    [ExtendsFromMod(InfernalCrossmod.Clamity.Name)]
    public class ShatteredSubcommunity : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        private static readonly Color rarityColorOne = new Color(128, 62, 128);
        private static readonly Color rarityColorTwo = new Color(245, 105, 245);

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TheSubcommunity>();
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            //Item.Calamity().devItem = true;
        }

        /* this isn't needed i guess
        public override ModItem Clone(Item item)
        {
            var clone = (ShatteredSubcommunity)base.Clone(Item);
            clone.level = level;
            clone.totalRageDamage = totalRageDamage;
            return clone;
        }
        */

        internal static Color GetRarityColor() => CalamityUtils.ColorSwap(rarityColorOne, rarityColorTwo, 3f);

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !player.Clamity().subcommunity;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            const float baseBoost = 0.2f;

            player.pickSpeed -= baseBoost * TheSubcommunity.MiningSpeedMult;

            player.Calamity().calamityBonusLuck += baseBoost * TheSubcommunity.LuckMult;

            player.fishingSkill += (int)(baseBoost * TheSubcommunity.FishingPower);

            player.tileSpeed += baseBoost * TheSubcommunity.TileAndWallPlacingSpeedMult;
            player.wallSpeed += baseBoost * TheSubcommunity.TileAndWallPlacingSpeedMult;

            Player.tileRangeX += (int)(baseBoost * TheSubcommunity.TileRangeMult);
            Player.tileRangeY += (int)(baseBoost * TheSubcommunity.TileRangeMult);
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().community;
        public override bool CanUseItem(Player player) => false;

        // Produces purple light while in the world.
        public override void PostUpdate()
        {
            float brightness = Main.essScale;
            Lighting.AddLight(Item.Center, 0.92f * brightness, 0.42f * brightness, 0.92f * brightness);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            if (nameLine != null)
                nameLine.OverrideColor = GetRarityColor();
        }
    }
}