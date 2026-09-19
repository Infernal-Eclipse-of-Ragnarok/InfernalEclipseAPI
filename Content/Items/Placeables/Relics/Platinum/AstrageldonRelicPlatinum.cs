using CatalystMod.Items;
using CatalystMod.Items.Placeable.Furniture.BossTrophies;
using CatalystMod.Tiles.Furniture;
using InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.GameContent;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Placeables.Relics.Platinum
{
    [JITWhenModsEnabled("CatalystMod")]
    [ExtendsFromMod("CatalystMod")]
    public class AstrageldonRelicPlatinum : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<AstrageldonRelic>();

        public static string PersonalMessage => Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.AstrageldonPlatinumRelic");
        public static int MaxLines => 1;

        public override void SetDefaults(Item entity)
        {
            entity.GetGlobalItem<InfernalGlobalItem>().hasEnchantmentShader = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            float colorInterpolant = (float)(Math.Sin(Pi * 0.3f * Main.GlobalTimeWrappedHourly + 1f) * 0.5) + 0.5f;
            Color c = LumUtils.MulticolorLerp(colorInterpolant,
                new Color(180, 225, 255, 255), // Icy blue
                new Color(80, 165, 255, 255),  // Bright blue
                new Color(45, 85, 220, 255),   // Cobalt
                new Color(75, 55, 190, 255),   // Indigo
                new Color(145, 70, 225, 255),  // Violet
                new Color(215, 165, 255, 255), // Light lavender
                new Color(200, 230, 255, 255)  // Icy highlight
            );

            InfernalUtilities.AddTooltip(tooltips, PersonalMessage, c);
        }
    }

    [JITWhenModsEnabled("CatalystMod")]
    [ExtendsFromMod("CatalystMod")]
    public class AstrageldonRelicPlatinumTile : ModSystem
    {
        private static Asset<Texture2D> glintTex;
        private static Effect glintFx;
        private static Hook specialDrawHook;

        private delegate void SpecialDrawDelegate(BossRelics self, int i, int j, SpriteBatch spriteBatch);

        public override void Load()
        {
            MethodInfo specialDrawMethod = typeof(BossRelics).GetMethod("SpecialDraw", BindingFlags.Public | BindingFlags.Instance);
            specialDrawHook = new Hook(specialDrawMethod, SpecialDrawDetour);
        }

        public override void Unload()
        {
            specialDrawHook?.Dispose();
            specialDrawHook = null;

            glintTex = null;
            glintFx = null;
        }

        private static void EnsureAssetsLoaded()
        {
            if (glintTex == null || !glintTex.IsLoaded)
            {
                glintTex = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Assets/Textures/Enchanted", AssetRequestMode.ImmediateLoad);
            }

            if (glintFx == null)
            {
                glintFx = ModContent.Request<Effect>("InfernalEclipseWeaponsDLC/Assets/Effects/Transform", AssetRequestMode.ImmediateLoad).Value;
            }
        }

        private static void SpecialDrawDetour(SpecialDrawDelegate orig, BossRelics self, int i, int j, SpriteBatch spriteBatch)
        {
            // Let Catalyst draw the relic normally first.
            orig(self, i, j, spriteBatch);

            EnsureAssetsLoaded();

            Vector2 offscreenOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Point point = new Point(i, j);
            Tile tile = Main.tile[point];

            if (tile == null || !tile.HasTile)
                return;

            Texture2D texture = ModContent.Request<Texture2D>(self.Texture + "_Relic").Value;
            int frameY = tile.TileFrameX / 54;
            Rectangle frame = texture.Frame(1, 1, 0, frameY);
            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPosition = point.ToWorldCoordinates(24f, 64f);
            Color color = Lighting.GetColor(point.X, point.Y, ModContent.GetInstance<SuperbossRarity>().RarityColor * 1.22f);
            SpriteEffects effects = tile.TileFrameY / 72 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float bob = MathF.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            Vector2 drawPosition = worldPosition + offscreenOffset - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, bob * 4f);

            glintFx.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            Main.instance.GraphicsDevice.Textures[1] = glintTex.Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, glintFx, Main.GameViewMatrix.TransformationMatrix);

            glintFx.CurrentTechnique.Passes["EnchantedPass"].Apply();
            spriteBatch.Draw(texture, drawPosition, frame, color, 0f, origin, 1f, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
