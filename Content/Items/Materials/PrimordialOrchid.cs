using CalamityMod.Items;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using NoxusBoss.Content.Rarities;

namespace InfernalEclipseAPI.Content.Items.Materials
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class PrimordialOrchid : ModItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.rare = ModContent.RarityType<NamelessDeityRarity>();
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.maxStack = 9999;
        }
    }
}
