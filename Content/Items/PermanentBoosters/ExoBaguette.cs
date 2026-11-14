using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Content.Items.Consumables;
using InfernalEclipseAPI.Core.Players;

namespace InfernalEclipseAPI.Content.Items.PermanentBoosters
{
    public class ExoBaguette : ModItem
    {
        public static int alcoholCapBoost = 1;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 20;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 52;
            Item.useAnimation = Item.useTime = 38;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.maxStack = 9999;
            Item.autoReuse = false;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<InfernalPlayer>().exoBaguette;
        }

        public override bool? UseItem(Player player)
        {
            if (player.GetModPlayer<InfernalPlayer>().exoBaguette == true)
                return false;

            player.GetModPlayer<InfernalPlayer>().exoBaguette = true;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulBaguette>()
                .AddIngredient<MiracleMatter>()
                .AddTile<DraedonsForge>()
                .Register();
        }
    }
}
