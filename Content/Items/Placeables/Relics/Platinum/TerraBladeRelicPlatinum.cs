using InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific;
using InfernalEclipseAPI.Content.Tiles.Relics.Platinum;
using InfernumMode;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using static InfernalEclipseAPI.Core.Systems.InfernalCrossmod;

namespace InfernalEclipseAPI.Content.Items.Placeables.Relics.Platinum
{
	public class TerraBladeRelicPlatinum : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<TerraBladeRelicTilePlatinum>(), 0);
			Item.width = 30;
			Item.height = 40;
			Item.maxStack = 9999;
            Item.rare = Catalyst.Mod != null ? Catalyst.Mod.Find<ModRarity>("SuperbossMasterRarity").Type : ItemRarityID.Master;
            Item.master = true;
			Item.value = Item.buyPrice(0, 5, 0, 0);

            Item.GetGlobalItem<InfernalGlobalItem>().hasEnchantmentShader = true;
        }

        public override LocalizedText Tooltip => Language.GetText($"Mods.InfernalEclipseAPI.Items.{Name}.Tooltip").WithFormatArgs(PersonalMessage);
        public static string PersonalMessage => Language.GetTextValue("Mods.InfernalEclipseAPI.Items.TerraBladeRelicPlatinum.PlatinumTooltip");

        public static int MaxLines => 1;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < MaxLines; i++)
            {
                TooltipLine obj = tooltips.FirstOrDefault((x) => x.Name == $"Tooltip{i}" && x.Mod == "Terraria");
                if (obj != null)
                {
                    float colorInterpolant = (float)(Math.Sin(Pi * 0.3f * Main.GlobalTimeWrappedHourly + 1f) * 0.5) + 0.5f;
                    Color c = LumUtils.MulticolorLerp(colorInterpolant,
                        new Color(180, 225, 255, 255), // Icy blue
                        new Color(80, 165, 255, 255),  // Bright blue
                        new Color(45, 85, 220, 255),   // Cobalt
                        new Color(75, 55, 190, 255),   // Indigo
                        new Color(145, 70, 225, 255),  // Violet
                        new Color(215, 165, 255, 255), // Light lavender
                        new Color(200, 230, 255, 255)  // Icy highlight
                    );
                    obj.Text = LumUtils.ColorMessage(obj?.Text, c);
                }
            }
        }
    }
}