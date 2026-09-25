using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Content.Items.Placeables.MusicBoxes;

namespace InfernalEclipseAPI.Content.Tiles.MusicBoxes
{
    public class QueenJellyfishMusicBoxTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(191, 142, 111), CreateMapEntryName());
        }

        public override void MouseOver(int i, int j)
        {
            Player localPlayer = Main.LocalPlayer;
            localPlayer.noThrow = 2;
            localPlayer.cursorItemIconEnabled = true;
            localPlayer.cursorItemIconID = ModContent.ItemType<QueenJellyfishMusicBox>();
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.gamePaused || !Main.instance.IsActive || Lighting.UpdateEveryFrame && !Main.rand.NextBool(4))
                return;

            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0)
                return;

            if (Main.timeForVisualEffects % 7.0 != 0.0 || !Main.rand.NextBool(3))
                return;

            int goreType = Main.rand.Next(570, 573);

            Vector2 position = new Vector2(i * 16 + 8, j * 16 - 8);
            Vector2 velocity = new Vector2(Main.WindForVisuals * 2f, -0.5f);

            velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
            velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;

            if (goreType == 572)
                position.X -= 8f;
            else if (goreType == 571)
                position.X -= 4f;

            int index = Gore.NewGore(new EntitySource_TileUpdate(i, j), position, velocity, goreType, 0.8f);
            Main.gore[index].timeLeft = 120;
        }
    }
}
