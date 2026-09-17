using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Consumables
{
    public class AuricBratwurst : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            /*
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[]
            {
                new(180, 112, 82),
                new(205, 133, 81),
                new(255, 139, 190),
                new(255, 224, 96)
            };
            */

            //ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46 / 2;
            Item.height = 18 / 2;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 15;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = 1550000;

            Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            bool sotsCanUse = true;
            if (InfernalCrossmod.SOTS.Loaded)
                sotsCanUse = VoidFoodHelper.CanUse(player);

            return sotsCanUse;
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.WellFed3, 36000);
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            ++Item.stack;
            Activate(player);

            if (player?.active == true)
                player.AddBuff(ModContent.BuffType<VoidSickness2>(), 600);
        }

        public void Activate(Player player)
        {
            OnActivation(player);
            --Item.stack;
        }

        public static int GetVoidAmt() => 75;

        public static int GetSatiateDuration() => 5;

        public static void OnActivation(Player player)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                VoidFoodHelper.RefillEffect(player, GetVoidAmt());
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new(Mod, nameof(AngryPudding), Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AuricBratwurst.DynamicTooltip", InfernalCrossmod.SOTS.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AuricBratwurst.VoidTooltip") + "\n" : "", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.Major")));
            tooltips.Add(tooltipLine);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 0.5f;
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float offset = 10f;

            if (player.direction == -1)
                offset = -offset;

            player.itemLocation.X += offset; player.itemLocation.X += 10f;

            base.UseStyle(player, heldItemFrame);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AuricBar>())
                .AddIngredient(ItemID.ChickenNugget, 5)
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}
