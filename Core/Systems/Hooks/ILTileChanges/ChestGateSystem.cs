using InfernalEclipseAPI.Core.Systems.UI;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Core.Systems.Hooks.ILTileChanges
{
    public class ChestGateSystem : ModSystem
    {
        private static int PermafrostPlatingCapsuleTileType = -1;

        public override void Load()
        {
            On_Player.OpenChest += BlockDungeonChestOpening;
            On_Chest.Unlock += BlockDungeonChestUnlock;

            if (ModLoader.TryGetMod("SOTS", out Mod sots))
            {
                PermafrostPlatingCapsuleTileType = sots.Find<ModTile>("PermafrostPlatingCapsuleTile").Type;
                On_Chest.Unlock += BlockPermafrostCapsuleUnlock;
            }
        }
        
        public override void Unload()
        {
            /* we should be fine here since tmod is good about cleaning up after modder's messes
            On_Player.OpenChest -= BlockDungeonChestOpening;
            On_Chest.Unlock -= BlockDungeonChestUnlock;
            On_Chest.Unlock -= BlockPermafrostCapsuleUnlock;
            */

            PermafrostPlatingCapsuleTileType = -1;
        }

        private static void BlockDungeonChestOpening(On_Player.orig_OpenChest orig, Player self, int x, int y, int newChest)
        {
            if (!NPC.downedBoss3 && self.ZoneDungeon)
            {
                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.HasTile && (tile.TileType == TileID.Containers || tile.TileType == TileID.Containers2))
                {
                    if (self.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server)
                    {
                        Main.NewText("This chest is cursed by the same magic that causes the old man at the entrance to suffer...", Color.MediumPurple);
                    }

                    return;
                }
            }

            orig(self, x, y, newChest);
        }

        private static bool BlockDungeonChestUnlock(On_Chest.orig_Unlock orig, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            if (!tile.HasTile)
                return orig(x, y);

            // Normalize to chest top-left just in case.
            int left = x;
            int top = y;

            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            Tile topLeft = Framing.GetTileSafely(left, top);
            if (!topLeft.HasTile)
                return orig(x, y);

            if ((tile.TileType == TileID.Containers || tile.TileType == TileID.Containers2) && Main.LocalPlayer.ZoneDungeon && !NPC.downedBoss3)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("This chest is cursed by the same magic that causes the old man at the entrance to suffer...", Color.MediumPurple);
                }

                return false;
            }

            return orig(x, y);
        }

        private static bool BlockPermafrostCapsuleUnlock(On_Chest.orig_Unlock orig, int x, int y)
        {
            if (PermafrostPlatingCapsuleTileType <= 0)
                return orig(x, y);

            Tile tile = Framing.GetTileSafely(x, y);
            if (!tile.HasTile)
                return orig(x, y);

            // Normalize to chest top-left just in case.
            int left = x;
            int top = y;

            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            Tile topLeft = Framing.GetTileSafely(left, top);
            if (!topLeft.HasTile)
                return orig(x, y);

            if (topLeft.TileType == PermafrostPlatingCapsuleTileType && !NPC.downedDeerclops)
            {
                return false;
            }

            return orig(x, y);
        }
    }

    public class ChestHoverSystem : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j];

            if (ModLoader.TryGetMod("SOTS", out Mod sots))
            {
                if (tile.TileType == sots.Find<ModTile>("PermafrostPlatingCapsuleTile").Type && tile.TileFrameX / 36 == 1 && !NPC.downedDeerclops)
                {
                    Main.NewText("This capsule is frozen shut by a winter beast from another world.", Color.LightBlue);
                }
            }
        }

        public override void MouseOver(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j];

            if ((type == TileID.Containers || type == TileID.Containers2 || type == TileID.FakeContainers || type == TileID.FakeContainers2) && Main.LocalPlayer.ZoneDungeon && !NPC.downedBoss3)
            {
                HoverItemSystem.QueueHoverItem(ModContent.ItemType<ChestLockIcon>());
            }

            if (ModLoader.TryGetMod("SOTS", out Mod sots))
            {
                if (tile.TileType == sots.Find<ModTile>("PermafrostPlatingCapsuleTile").Type && tile.TileFrameX / 36 == 1 && !NPC.downedDeerclops)
                {
                    HoverItemSystem.QueueHoverItem(ModContent.ItemType<ChestLockIcon>());
                }
            }
        }
    }
}
