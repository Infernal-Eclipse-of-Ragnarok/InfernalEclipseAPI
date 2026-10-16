using System.Reflection;
using MonoMod.RuntimeDetour;
using SOTS.Dusts;
using SOTS;
using SOTS.Items.Potions;
using Microsoft.Xna.Framework;
using SOTS.Helpers;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Systems.Hooks
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class VigorDashPlayerChanges : ModSystem
    {
        private Hook resetEffectsHook;

        private delegate void ResetEffects_Replacement(object self);

        public override void Load()
        {
            var vigorType = typeof(SOTSVigorDashPlayer);
            var resetMi = vigorType.GetMethod("ResetEffects",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (resetMi == null)
                return;

            resetEffectsHook = new Hook(resetMi, new ResetEffects_Replacement(ResetEffects_Hook));
        }

        public override void Unload()
        {
            resetEffectsHook?.Dispose();
            resetEffectsHook = null;
        }

        private static void ResetEffects_Hook(object selfObj)
        {
            var self = (SOTSVigorDashPlayer)selfObj;
            Player player = self.Player;

            const int MaxDashDelay = 50;
            const int MaxDashTimer = 10;

            bool dashActive = self.DashTimer > 0;

            if (dashActive)
            {
                // ===== ACTIVE DASH: HALF DISTANCE =====

                if (self.DashTimer == MaxDashTimer)
                {
                    player.velocity.X *= 0.2f;
                    player.velocity += new Vector2(self.DashDir * 6f, 0f);

                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 velo = -player.velocity * Main.rand.NextFloat() + Main.rand.NextVector2Circular(2f, 2f);
                        var d = PixelDust.Spawn(player.position, player.width, player.height,
                            velo, ColorHelper.PinkPetal, Main.rand.Next(3, 7));
                        d.scale *= Main.rand.NextFloat(1.5f, 2.25f);
                    }
                }

                player.armorEffectDrawShadowEOCShield = true;

                player.velocity += new Vector2(self.DashDir * 0.08f * self.DashTimer / MaxDashTimer, 0f);

                Vector2 step = new Vector2(self.DashDir * 9f * self.DashTimer / MaxDashTimer, 0f);
                Vector2 collision = Collision.TileCollision(player.position, step, player.width, player.height, false, false, 1);
                player.position += collision;

                self.DashTimer--;

                // Trail (unchanged style, uses new movement)
                for (float t = 0f; t < 1f; t += 0.34f)
                {
                    Vector2 interp = player.velocity * t + collision * t;
                    Vector2 velo = -player.velocity * Main.rand.NextFloat(0.8f) + Main.rand.NextVector2Circular(1.4f, 1.4f);

                    PixelDust.Spawn(player.Center - new Vector2(12f) + interp, 24, 24,
                        velo, ColorHelper.PinkPetal, Main.rand.Next(9, 11))
                        .scale *= Main.rand.NextFloat(1f, 1.7f);

                    var dust = Dust.NewDustDirect(
                        player.position + interp + new Vector2(0f, player.height - 4),
                        player.width, 4,
                        ModContent.DustType<CopyDust4>(),
                        0f, 0f, 0,
                        ColorHelper.PinkPetal, 1f);

                    dust.velocity = velo * 0.4f + dust.velocity * 0.1f;
                    dust.noGravity = true;
                    dust.fadeIn = 0.2f;
                    dust.scale = (float)(dust.scale * 0.5 + 1.0);
                    dust.color.A = 0;
                }
            }
            else
            {
                // ===== DASH START / COOLDOWN =====

                SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);

                if (Main.myPlayer != player.whoAmI)
                    return;

                self.DashDelay--;

                if (!sotsPlayer.VigorActive ||
                    player.mount.Active ||
                    self.DashTimer > 0 ||              // DashActive
                    player.grappling[0] >= 0)
                    return;

                // Double-tap detect
                if (player.controlRight && player.releaseRight && player.doubleTapCardinalTimer[2] < 15)
                {
                    self.DashDir = 1;
                }
                else if (player.controlLeft && player.releaseLeft && player.doubleTapCardinalTimer[3] < 15)
                {
                    self.DashDir = -1;
                }
                else
                {
                    return;
                }

                if (self.DashDelay > 0)
                    return;

                sotsPlayer.VigorDashes--;
                self.DashDelay = MaxDashDelay;
                self.DashTimer = MaxDashTimer;

                if (Main.myPlayer == player.whoAmI)
                {
                    // Call original SendPacket via instance method
                    self.SendPacket();
                }
            }
        }
    }
}
