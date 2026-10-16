using CalamityMod.Items;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Materials
{
    public class InfectedMothwingSpore : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalCrossmod.SOTS.Loaded;
        }

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 40;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.maxStack = 9999;
        }
    }
}
