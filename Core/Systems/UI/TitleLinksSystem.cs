using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace InfernalEclipseAPI.Core.Systems.UI
{
    public class TitleLinksSystem : ModSystem
    {
        // Credit to Fargo's team
        // Modified by Ropro, the world's greatest coder (Larp)
        internal static List<TitleLinkButton> InfernalTitleLinks = [];
        public override void Load()
        {
            MakeSimpleButton("TitleLinks.Discord", "https://discord.gg/ieor", 0);
            MakeSimpleButton("TitleLinks.Wiki", "https://terrariamods.wiki.gg/wiki/Infernal_Eclipse_of_Ragnarok", 1);
            MakeSimpleButton("Mods.InfernalEclipseAPI.UI.TitleLinks.Patreon", "https://www.patreon.com/cw/InfernalEclipseofRagnarokTeam", 2);
            MakeSimpleButton("Mods.InfernalEclipseAPI.UI.TitleLinks.Github", "https://github.com/Infernal-Eclipse-of-Ragnarok", 3);
        }
        static void MakeSimpleButton(string textKey, string linkUrl, int horizontalFrameIndex)
        {
            //yummy vanilla code
            // Slightly modified by Ropro0923
            Asset<Texture2D> val = ModContent.Request<Texture2D>("InfernalEclipseAPI/Assets/Textures/Menu/TitleLinkButtons", (AssetRequestMode)1);
            Rectangle value = val.Frame(4, 2, horizontalFrameIndex);
            Rectangle value2 = val.Frame(4, 2, horizontalFrameIndex, 1);
            value.Width--;
            value.Height--;
            value2.Width--;
            value2.Height--;
            InfernalTitleLinks.Add(new TitleLinkButton
            {
                TooltipTextKey = textKey,
                LinkUrl = linkUrl,
                FrameWehnSelected = value2,
                FrameWhenNotSelected = value,
                Image = val
            });
        }
        public static void DrawTitleLinks(float upBump)
        {
            List<TitleLinkButton> titleLinks = InfernalTitleLinks;
            Vector2 anchorPosition = new(18f, Main.screenHeight - 85 - 22 - upBump);
            for (int i = 0; i < titleLinks.Count; i++)
            {
                titleLinks[i].Draw(Main.spriteBatch, anchorPosition);
                anchorPosition.X += 30f;
            }
        }
        internal static void DrawMenu()
        {
            if (!WorldGen.generatingWorld)
            {
                float upBump = 0;
                InfernalEclipseAPI mod = InfernalEclipseAPI.Instance;
                upBump += 32f;
                if (!WorldGen.drunkWorldGen && Main.menuMode == 0)
                {
                    DrawTitleLinks(upBump);
                    upBump += 32f;
                }
                if (!WorldGen.drunkWorldGen)
                {
                    string text = mod.DisplayName + " " + mod.Version;
                    Vector2 origin = FontAssets.MouseText.Value.MeasureString(text);
                    origin.X *= 0.5f;
                    origin.Y *= 0.5f;
                    for (int i = 0; i < 5; i++)
                    {
                        Color color2 = Color.Black;
                        if (i == 4)
                        {
                            byte b = (byte)((255 + Main.tileColor.R * 2) / 3);
                            color2 = new(b, b, b, 255);
                            color2.R = (byte)((255 + color2.R) / 2);
                            color2.G = (byte)((255 + color2.R) / 2);
                            color2.B = (byte)((255 + color2.R) / 2);
                        }
                        color2.A = (byte)(color2.A * 0.3f);
                        int num = 0;
                        int num2 = 0;
                        if (i == 0)
                        {
                            num = -2;
                        }
                        if (i == 1)
                        {
                            num = 2;
                        }
                        if (i == 2)
                        {
                            num2 = -2;
                        }
                        if (i == 3)
                        {
                            num2 = 2;
                        }
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, text, new Vector2(origin.X + num + 10f, Main.screenHeight - origin.Y + num2 - (Main.menuMode == 0 ? 85f : 25f) - upBump), color2, 0f, origin, 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}