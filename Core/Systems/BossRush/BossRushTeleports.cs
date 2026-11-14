using InfernumMode.Core.GlobalInstances.Systems;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.WorldBuilding;
using static CalamityMod.Events.BossRushEvent;

namespace InfernalEclipseAPI.Core.Systems.BossRush
{
    public class BossRushTeleports : ModSystem
    {
        public static void BringPlayersBackToSpawn()
        {
            // Post-Wall of Flesh teleport back to spawn.
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead)
                {
                    player.Spawn(PlayerSpawnContext.RecallFromItem);
                    SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, player.Center);
                }
            }
        }

        public static void PyramidTeleportAll()
        {
            Vector2? target = null;

            // Find the sarcophagus tile once
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == ModLoader.GetMod("SOTS").Find<ModTile>("SarcophagusTile").Type)
                    {
                        target = new Vector2((x + 3f) * 16f, (y + 1.5f) * 16f);
                        break;
                    }
                }
                if (target.HasValue)
                    break;
            }

            if (!target.HasValue)
                return;

            // Teleport every active player
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead)
                {
                    player.Teleport(target.Value, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, target.Value.X, target.Value.Y, 1);
                    SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, player.Center);
                }
            }
        }

        public static void OceanTeleportAll()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead)
                {
                    player.MagicConch();
                    SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, player.Center);
                }
            }
        }

        private static uint _lastSearchFrame = UInt32.MaxValue;
        private static Vector2? _cachedHellSpotServer;

        public static void HellTeleportAll()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (_lastSearchFrame != Main.GameUpdateCount)
                {
                    _cachedHellSpotServer = GetHellCenterLandingFast();
                    _lastSearchFrame = Main.GameUpdateCount;
                }

                if (!_cachedHellSpotServer.HasValue)
                    return;

                Vector2 worldPos = _cachedHellSpotServer.Value;

                foreach (var player in Main.ActivePlayers)
                {
                    if (!player.active || player.dead)
                        continue;

                    player.Teleport(worldPos, TeleportationStyleID.RodOfDiscord);
                    SoundEngine.PlaySound(SoundID.Item6, worldPos);

                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, worldPos.X, worldPos.Y, TeleportationStyleID.RodOfDiscord);
                }
            }
        }

        private static Vector2? GetHellCenterLandingFast()
        {
            int centerX = Main.maxTilesX / 2;
            int startY = Main.UnderworldLayer + 8;
            const int maxSearchDown = 400;
            const int maxRadius = 500;
            const int bodyW = 2;
            const int bodyH = 4;

            for (int r = 0; r <= maxRadius; r += 2)
            {
                for (int side = 0; side < 2; side++)
                {
                    int x = side == 0 ? centerX + r : centerX - r;
                    if (x < 10 || x > Main.maxTilesX - 10)
                        continue;

                    if (WorldUtils.Find(new Point(x, startY),
                        Searches.Chain(new Searches.Down(maxSearchDown), new Conditions.IsSolid()),
                        out Point ground))
                    {
                        int standX = ground.X;
                        int standY = ground.Y - 1;

                        if (!IsSafeFloor(ground.X, ground.Y))
                            continue;

                        int tlX = standX;
                        int tlY = standY - (bodyH - 1);

                        if (!WorldGen.InWorld(tlX, tlY, 10) || !WorldGen.InWorld(tlX + bodyW - 1, tlY + bodyH - 1, 10))
                            continue;

                        // Fast empty-body check
                        bool blocked = Collision.SolidTiles(tlX, tlX + bodyW - 1, tlY, tlY + bodyH - 1);
                        if (blocked)
                            continue;

                        // Quick liquid probe (middle of the body)
                        var mid = Main.tile[tlX, tlY + (bodyH / 2)];
                        if (mid != null && mid.LiquidAmount > 128)
                            continue;

                        return new Point(standX, standY).ToWorldCoordinates(8f, 0f);
                    }
                }
            }
            return null;
        }

        private static bool HasClearance(int topLeftX, int topLeftY, int w, int h)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int tx = topLeftX + i;
                    int ty = topLeftY + j;
                    if (!WorldGen.InWorld(tx, ty, 10))
                        return false;

                    Tile t = Main.tile[tx, ty];
                    if (t == null)
                        return false;

                    // No solids or half-bricks in the body space
                    if (t.HasTile && Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType])
                        return false;

                    // Avoid deep liquid in the body space
                    if (t.LiquidAmount > 128) // ~50% liquid
                        return false;
                }
            }
            return true;
        }

        // Require the floor tile to be solid (or a platform) and not fully submerged in lava.
        private static bool IsSafeFloor(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 10))
                return false;

            Tile floor = Main.tile[x, y];
            if (floor == null)
                return false;

            bool solid = floor.HasTile && (Main.tileSolid[floor.TileType] || Main.tileSolidTop[floor.TileType]);
            if (!solid)
                return false;

            // Avoid standing on tiles covered by lava
            if (floor.LiquidType == LiquidID.Lava && floor.LiquidAmount > 0)
                return false;

            return true;
        }

        public static void GaurdiansTeleportAll()
        {
            Vector2 teleportPosition =
            WorldSaveSystem.ProvidenceArena.TopLeft() * 16f +
            new Vector2(WorldSaveSystem.ProvidenceArena.Width * 3.2f - 16f, 800f);

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead)
                {
                    player.Teleport(teleportPosition, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
                    SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, player.Center);
                }
            }
        }

        public static void TeleportToForbiddenArchive()
        {
            // Compute teleport position
            Vector2 teleportPosition = WorldSaveSystem.ForbiddenArchiveCenter.ToWorldCoordinates() + Vector2.UnitY * 1032f;

            // Teleport all active, living players
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead)
                {
                    player.Teleport(teleportPosition, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
                    SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, player.Center);
                }
            }
        }

        public static void TeleportToProvidence()
        {
            // Compute teleport position
            Vector2 teleportPosition =
                WorldSaveSystem.ProvidenceArena.TopRight() * 16f +
                new Vector2(WorldSaveSystem.ProvidenceArena.Width * -3.2f - 16f, 800f);

            // Teleport all active, living players
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead)
                {
                    player.Teleport(teleportPosition, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
                    SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, player.Center);
                }
            }
        }
    }
}
