using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SOTS.Helpers;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace InfernalEclipseAPI.Core.ChatTags
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public sealed class ChaosConstructChatTag : ITagHandler
    {
        public class Snippet : TextSnippet
        {
            private readonly string text;

            public Snippet(string text)
            {
                this.text = text;
            }

            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
            {
                size = FontAssets.MouseText.Value.MeasureString(text) * scale;

                if (!justCheckingString)
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, ColorHelper.PastelRainbow, 0f, Vector2.Zero, new Vector2(scale));

                return false;
            }
        }

        public TextSnippet Parse(string text, Color baseColor = default, string options = null)
        {
            // Pull localized name
            string localized = Language.GetTextValue("Mods.InfernalEclipseAPI.Items.ChaosLure.Name");
            return new Snippet(localized);
        }
    }
}
