using CalamityMod.Projectiles.Typeless;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.World;
using NoxusBoss.Content.Items;

namespace InfernalEclipseAPI.Common.GlobalProjectiles.ProjectileReworks
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class BossRushProjectile : GlobalProjectile
    {
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (projectile.type == ModContent.ProjectileType<BossRushEndEffectThing>() && InfernalWorld.RagnarokModeEnabled)
            {
                for (int i = Main.maxPlayers - 1; i >= 0; i--)
                {
                    Player p = Main.player[i];
                    if (p is null || !p.active)
                        continue;

                    int cheatPermissionSlip = Item.NewItem(p.GetSource_Misc("CalamityMod_BossRushRock"), (int)p.position.X, (int)p.position.Y, p.width, p.height, ModContent.ItemType<CheatPermissionSlip>());
                    if (Main.netMode == NetmodeID.Server)
                    {
                        Main.timeItemSlotCannotBeReusedFor[cheatPermissionSlip] = 54000;
                        NetMessage.SendData(MessageID.InstancedItem, i, -1, null, cheatPermissionSlip);
                    }
                }
            }
            return base.PreKill(projectile, timeLeft);
        }
    }
}
