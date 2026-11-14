using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;
using CalamityMod.Items.LoreItems;
using Microsoft.Xna.Framework.Graphics;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Lore.FargosSouls
{
    [JITWhenModsEnabled(InfernalCrossmod.FargosSouls.Name)]
    [ExtendsFromMod(InfernalCrossmod.FargosSouls.Name)]
    public class LoreMutant : LoreItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            if (!InfernalConfig.Instance.DontEnableThis) return false;
            if (!ModLoader.TryGetMod("FargowiltasSouls", out _)) return false;
            bool hasCSE = ModLoader.TryGetMod("ssm", out Mod cse) && cse.Version > Version.Parse("1.1.4.2");
            return !hasCSE;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.rare = ItemRarityID.Purple;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            ModLoader.TryGetMod("FargowiltasSouls", out Mod souls);

            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(souls.Find<ModItem>("MutantTrophy").Type)
                .AddTile(TileID.Bookcases)
                .Register();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if ((line.Mod == "Terraria" && line.Name == "ItemName") || line.Name == "FlavorText")
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Text");
                shader.TrySetParameter("mainColor", new Color(42, 66, 99));
                shader.TrySetParameter("secondaryColor", Color.Teal);
                shader.Apply("PulseUpwards");
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }
    }
}
