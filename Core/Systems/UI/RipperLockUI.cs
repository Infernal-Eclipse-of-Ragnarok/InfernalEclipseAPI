using CalamityMod;
using InfernalEclipseAPI.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;

namespace InfernalEclipseAPI.Core.Systems.UI
{
    public sealed class RipperLockUI : ModSystem
    {
        internal const int LockAnimLength = 15;
        private static int lockClickTime = 0;
        private static bool previousLockStatus = false;

        private static void ClearVariables()
        {
            lockClickTime = 0;
            previousLockStatus = false;
        }

        public static void TickVariables()
        {
            if (lockClickTime > 0)
                lockClickTime--;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Rage and Adrenaline UI Locks", delegate ()
                {
                    Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));
            }
        }

        public static void Draw(SpriteBatch spriteBarch, Player player)
        {
            Vector2 rageScreenRatioPos = new Vector2(CalamityClientConfig.Instance.RageMeterPosX, CalamityClientConfig.Instance.RageMeterPosY);
            if (rageScreenRatioPos.X < 0f || rageScreenRatioPos.X > 100f)
                rageScreenRatioPos.X = 35.77406f;
            if (rageScreenRatioPos.Y < 0f || rageScreenRatioPos.Y > 100f)
                rageScreenRatioPos.Y = 4.5761431f;

            Vector2 adrenScreenRatioPos = new Vector2(CalamityClientConfig.Instance.AdrenalineMeterPosX, CalamityClientConfig.Instance.AdrenalineMeterPosY);
            if (adrenScreenRatioPos.X < 0f || adrenScreenRatioPos.X > 100f)
                adrenScreenRatioPos.X = 35.77406f;
            if (adrenScreenRatioPos.Y < 0f || adrenScreenRatioPos.Y > 100f)
                adrenScreenRatioPos.Y = 8.846918f;

            Vector2 rageScreenPos = rageScreenRatioPos;
            rageScreenPos.X = (int)(rageScreenPos.X * 0.01f * Main.screenWidth);
            rageScreenPos.Y = (int)(rageScreenPos.Y * 0.01f * Main.screenHeight) - 12;
            Vector2 adrenScreenPos = adrenScreenRatioPos;
            adrenScreenPos.X = (int)(adrenScreenPos.X * 0.01f * Main.screenWidth);
            adrenScreenPos.Y = (int)(adrenScreenPos.Y * 0.01f * Main.screenHeight) - 12;

            GetLockStatus(player, out bool locked);

            if (locked)
            {
                DrawLock(spriteBarch, rageScreenPos);
                DrawLock(spriteBarch, adrenScreenPos);
            }
            else
            {
                ClearVariables();
                return;
            }

            TickVariables();
        }

        #region Lock Drawing
        private static CalamityUtils.CurveSegment lockGrow = new CalamityUtils.CurveSegment(CalamityUtils.SineOutEasing, 0f, 1f, 0.4f);
        private static CalamityUtils.CurveSegment lockShrink = new CalamityUtils.CurveSegment(CalamityUtils.SineInEasing, 0.6f, 1.4f, -0.4f);
        private static CalamityUtils.CurveSegment lockBump = new CalamityUtils.CurveSegment(CalamityUtils.SineBumpEasing, 0.9f, 1f, -0.2f);
        internal static float LockShakeScale => CalamityUtils.PiecewiseAnimation(lockClickTime / (float)LockAnimLength, new CalamityUtils.CurveSegment[] { lockGrow, lockShrink, lockBump });
        public static void GetLockStatus(Player player, out bool locked)
        {
            locked = false;
            if (player.HasBuff<HormonalBlockade>())
            {
                locked = true;
            }

            if (locked != previousLockStatus && lockClickTime == 0)
                lockClickTime = LockAnimLength;

            previousLockStatus = locked;
        }

        public static void DrawLock(SpriteBatch spriteBatch, Vector2 DrawCenter)
        {
            Texture2D lockTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicatorLock").Value;
            float rotationShift = lockClickTime == 0 ? 0f : (float)Math.Sin((1 - lockClickTime / (float)LockAnimLength) * MathHelper.TwoPi * 2f) * 0.5f * (lockClickTime / (float)LockAnimLength);
            spriteBatch.Draw(lockTexture, DrawCenter + Vector2.UnitY * 24, null, Color.White, 0f + rotationShift, lockTexture.Size() * 0.5f, LockShakeScale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}
