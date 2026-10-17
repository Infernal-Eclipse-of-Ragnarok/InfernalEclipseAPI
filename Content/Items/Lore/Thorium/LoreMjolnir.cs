using System.Collections.Generic;
using CalamityMod.Items.LoreItems;
using CalamityMod.Rarities;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Lore.Thorium
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class LoreMjolnir : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.consumable = false;
        }
    }
}
