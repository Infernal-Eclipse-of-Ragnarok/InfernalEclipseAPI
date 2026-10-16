using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Players.ThoriumPlayerOverrides;

namespace InfernalEclipseAPI.Core.Menu
{
    public class MiscEffectsLayer : PlayerDrawLayer
    {
        public override PlayerDrawLayer.Position GetDefaultPosition()
        {
            return (PlayerDrawLayer.Position)new PlayerDrawLayer.AfterParent(PlayerDrawLayers.ElectrifiedDebuffFront);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.whoAmI == Main.myPlayer && ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                var modPlayer = drawPlayer.GetModPlayer<SoulAnchorPlayer>();

                // Anchor exists and is within 20 second window
                if (modPlayer.anchorLocation != Vector2.Zero &&
                    Main.GameUpdateCount - modPlayer.anchorSetTime <= 60 * 20)
                {
                    Texture2D texture = thorium.Assets.Request<Texture2D>(
                                "Textures/SoulAnchorLocation",
                                ReLogic.Content.AssetRequestMode.ImmediateLoad
                    ).Value;


                    Vector2 drawPosition = modPlayer.anchorLocation - Main.screenPosition;
                    Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                    Color color = new Color(255, 255, 255, 0) * 0.5f;
                    float scale = 1f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f);
                    float rotation = Main.GlobalTimeWrappedHourly * 0.5f;

                    DrawData drawData = new DrawData(
                        texture,
                        drawPosition,
                        null,
                        color,
                        rotation,
                        origin,
                        1f,
                        SpriteEffects.None,
                        0
                    )
                    {
                        ignorePlayerRotation = true
                    };

                    drawInfo.DrawDataCache.Add(drawData);
                }
            }
        }
    }
}
