using CalamityMod.Tiles.BaseTiles;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.Platinum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace InfernalEclipseAPI.Content.Tiles.Relics.Platinum
{
    public class TerraBladeRelicTilePlatinum : BaseBossRelic
    {
        public override string RelicTextureName => "InfernalEclipseAPI/Content/Tiles/Relics/Platinum/TerraBladeRelicTilePlatinum";
        public override int AssociatedItem => ModContent.ItemType<TerraBladeRelicPlatinum>();
        public override string Texture => "InfernalEclipseAPI/Content/Tiles/Relics/Platinum/PlatinumRelicBase";

        private static Asset<Texture2D> glintTex;
        private static Effect glintFx;

        private static void EnsureAssetsLoaded()
        {
            if (glintTex == null || !glintTex.IsLoaded)
            {
                glintTex = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Assets/Textures/Enchanted", AssetRequestMode.ImmediateLoad);
            }

            if (glintFx == null)
            {
                glintFx = ModContent.Request<Effect>("InfernalEclipseWeaponsDLC/Assets/Effects/Transform", AssetRequestMode.ImmediateLoad).Value;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (!tile.HasTile)
                return;

            EnsureAssetsLoaded();

            Texture2D texture = RelicTexture.Value;
            int frameY = tile.TileFrameX / 54;
            Rectangle frame = texture.Frame(1, 1, 0, frameY);
            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPosition = p.ToWorldCoordinates(24f, 64f);
            Color color = Lighting.GetColor(p.X, p.Y);
            SpriteEffects effects = tile.TileFrameY / 72 != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float bob = MathF.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            Vector2 drawPosition = worldPosition - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, bob * 4f);

            glintFx.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            Main.instance.GraphicsDevice.Textures[1] = glintTex.Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, glintFx, Main.GameViewMatrix.TransformationMatrix);

            glintFx.CurrentTechnique.Passes["EnchantedPass"].Apply();
            spriteBatch.Draw(texture, drawPosition, frame, color, 0f, origin, 1f, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float glowPulse = MathF.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3f + 0.7f;
            Color glowColor = color;
            glowColor.A = 0;
            glowColor *= 0.1f * glowPulse;
            float glowDistance = 6f + bob * 2f;

            for (float interpolant = 0f; interpolant < 1f; interpolant += 1f / 6f)
                spriteBatch.Draw(texture, drawPosition + (MathHelper.TwoPi * interpolant).ToRotationVector2() * glowDistance, frame, glowColor, 0f, origin, 1f, effects, 0f);
        }
    }
}
