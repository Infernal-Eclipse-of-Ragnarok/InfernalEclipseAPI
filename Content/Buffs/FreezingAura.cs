using CalamityMod;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Content.Buffs
{
    [LegacyName("LowGround")]
    public class FreezingAura : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().weakPetrification = true;

            if (player.grapCount > 0)
                player.RemoveAllGrapplingHooks();

            if (player.mount.Active)
                player.mount.Dismount(player);

            Tile thisTile = Framing.GetTileSafely(player.Bottom);
            Tile bottomTile = Framing.GetTileSafely(player.Bottom + Vector2.UnitY * 8);

            if (!Collision.SolidCollision(player.BottomLeft, player.width, 16))
            {
                if (player.velocity.Y >= 0 && (IsPlatform(thisTile.TileType) || IsPlatform(bottomTile.TileType)))
                {
                    player.position.Y += 2;
                }
                if (player.velocity.Y == 0)
                {
                    player.position.Y += 16;
                }
            }

            if (player.mount.Active)
            {
                player.mount.Dismount(player);
            }

            static bool IsPlatform(int tileType)
            {
                return tileType == TileID.Platforms || tileType == TileID.PlanterBox;
            }
        }
    }

    public class CrimulanAura : FreezingAura
    {
    }
}
