using CalamityMod;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Potions.Food;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Consumables
{
    public class SoulBaguette : ModItem
    {
        public static int RedWineHealValue = 250;
        public static int RedWineRegenLoss = 2;
        public static int RedWineRegenLossDuration = CalamityUtils.SecondsToFrames(15);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(InfernalCrossmod.SOTS.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SoulBaguette.VoidTooltip") + "\n" : "", RedWineRegenLoss.ToRegenPerSecond());

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Type] = new Color[3] {
                new Color(231, 137, 159),
                new Color(179, 104, 56),
                new Color(108, 47, 16)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HasBuff<RedWineBuff>())
            {
                Item.healLife = RedWineHealValue;
                Item.potion = true;
            }
            else
            {
                Item.healLife = 0;
                Item.potion = false;
            }
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(52, 38, BuffID.WellFed, CalamityUtils.MinutesToFrames(5));
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
        }

        public override bool CanUseItem(Player player)
        {
            bool sotsCanUse = true;
            if (InfernalCrossmod.SOTS.Loaded)
            {
                sotsCanUse = VoidFoodHelper.CanUse(player);
            }

            return sotsCanUse;
        }

        public override bool ConsumeItem(Player player)
        {
            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            Activate(player);

            player.AddBuff(BuffID.WellFed2, Item.buffTime);
            if (player.HasBuff<RedWineBuff>())
                player.AddBuff(ModContent.BuffType<BaguetteBuff>(), RedWineRegenLossDuration);

            if (player?.active == true)
                player.AddBuff(ModContent.BuffType<VoidSickness2>(), 600);
        }

        public void Activate(Player player)
        {
            OnActivation(player);
        }

        public static int GetVoidAmt() => 60;

        public static int GetSatiateDuration() => 5;

        public static void OnActivation(Player player)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                VoidFoodHelper.RefillEffect(player, GetVoidAmt());
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
                                .AddIngredient<Baguette>()
                                .AddIngredient(ItemID.SoulofMight)
                                .AddIngredient(ItemID.SoulofSight)
                                .AddIngredient(ItemID.SoulofFright);

            if (InfernalCrossmod.SOTS.Loaded)
            {
                recipe.AddIngredient(InfernalCrossmod.SOTS.Mod.Find<ModItem>("SoulOfPlight").Type);
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                recipe.AddTile(InfernalCrossmod.Thorium.Mod.Find<ModTile>("SoulForge").Type);
            }
            else
            {
                recipe.AddTile(TileID.AdamantiteForge);
            }

            recipe.Register();
        }
    }
}
