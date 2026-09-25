using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;
using InfernalEclipseAPI.Core.Configs;

namespace InfernalEclipseAPI.Content.Tiles.Relics.CalamityAddons
{
    public class AstrageldonRelicTileGold : ModTile
    {
        public const int FrameWidth = 54;
        public const int FrameHeight = 72;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1;

        public Asset<Texture2D> RelicTexture;

        public virtual string RelicTextureName => "InfernalEclipseAPI/Content/Tiles/Relics/DreadnautilusRelicTile";
        public virtual string ColoredRelicTextureName => "InfernalEclipseAPI/Content/Tiles/Relics/DreadnautilusRelicColoredTile";
        public override string Texture => "InfernalEclipseAPI/Content/Tiles/Relics/RelicPedestal";

        public override void Load()
        {
            if (Main.dedServ) return;

            bool colored = InfernalConfig.Instance.ColoredRelics;
            RelicTexture = ModContent.Request<Texture2D>(
                colored ? ColoredRelicTextureName : RelicTextureName,
                AssetRequestMode.ImmediateLoad
            );
        }

        public override void Unload() => RelicTexture = null;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        }

        public override bool CreateDust(int i, int j, ref int type) => false;

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= (short)(FrameHeight * 2);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth != 0 || drawData.tileFrameY % FrameHeight != 0)
                return;

            Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (RelicTexture == null) return;

            Vector2 offscreen = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Tile tile = Main.tile[i, j];
            if (!tile.HasTile) return;

            Texture2D tex = RelicTexture.Value;
            Rectangle src = Utils.Frame(tex, HorizontalFrames, VerticalFrames, 0, 0);
            Vector2 origin = Utils.Size(src) / 2f;

            // Center above the 3x4 pedestal (1.5 tiles across, 4 tiles up)
            Vector2 worldPos = Utils.ToWorldCoordinates(new Point(i, j), 24f, 64f);
            Color lit = Lighting.GetColor(i, j);

            SpriteEffects fx = (tile.TileFrameY / FrameHeight) > 0
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            float bob = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            Vector2 drawPos = worldPos - Main.screenPosition + offscreen + new Vector2(0f, -40f + bob * 4f);

            spriteBatch.Draw(tex, drawPos, src, lit, 0f, origin, 1f, fx, 0f);

            float pulse = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3 + 0.7);
            Color glow = lit; glow.A = 0;
            glow *= 0.1f * pulse;

            for (float t = 0f; t < 1f; t += 1f / 6f)
            {
                Vector2 ring = Utils.ToRotationVector2(MathHelper.TwoPi * t) * (6f + bob * 2f);
                spriteBatch.Draw(tex, drawPos + ring, src, glow, 0f, origin, 1f, fx, 0f);
            }
        }

    }
}
