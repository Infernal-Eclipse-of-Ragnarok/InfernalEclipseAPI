using Microsoft.Xna.Framework;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using YouBoss.Content.Items.ItemReworks;
using InfernalEclipseAPI.Core.Systems;

namespace InfernalEclipseAPI.Content.Items.Other
{
    [JITWhenModsEnabled("YouBoss")]
    [ExtendsFromMod("YouBoss")]
    public class TerraBladeTreasureBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.BossBag[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.EyeOfCthulhuBossBag);
        //    Item.maxStack = 999;
        //    Item.consumable = true;
        //    Item.width = 36;
        //    Item.height = 32;
        //    Item.rare = ItemRarityID.Expert;
        //    Item.expert = true;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }
        public override bool CanRightClick() => true;
        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
        public override void PostUpdate()
        {
            Item.TreasureBagLightAndDust();
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            if (ModLoader.HasMod("YouBoss"))
            {
                itemLoot.Add(ModContent.ItemType<FirstFractal>());
            }
            if (InfernalCrossmod.InfernalEclipseWeaponsDLC.Loaded)
            {
                
            }
        }
    }
}