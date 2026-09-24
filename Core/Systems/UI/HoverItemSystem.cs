using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace InfernalEclipseAPI.Core.Systems.UI
{
    //Credit: Thorium Mod
    public class HoverItemSystem : ModSystem
    {
        public static HoverItemData Data { get; private set; }

        public static void QueueHoverItem(int type, int stack = 1)
        {
            Player localPlayer = Main.LocalPlayer;
            localPlayer.cursorItemIconID = ModContent.ItemType<HoverItemDummy>();
            localPlayer.cursorItemIconText = "";
            localPlayer.cursorItemIconEnabled = true;
            Data = new HoverItemData(type, stack);
        }

        public override void PostUpdateInput()
        {
            Data = new HoverItemData(0, 0);
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            int type = Data.Type;
            int stack = Data.Stack;
            if (type > 0 && stack > 0)
            {
                Main.instance.LoadItem(type);
                Texture2D value = TextureAssets.Item[type].Value;
                Rectangle rectangle = Main.itemAnimations[type]?.GetFrame(value) ?? value.Frame();
                Vector2 vector = Main.MouseScreen + new Vector2(12f, 12f);
                Vector2 position = vector + new Vector2(16f, 16f);
                Vector2 origin = rectangle.Size() / 2f;
                spriteBatch.Draw(value, position, rectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
                if (stack > 1)
                {
                    Vector2 pos = vector + new Vector2(32f, 40f);
                    Terraria.Utils.DrawBorderString(spriteBatch, stack.ToString(), pos, Color.White, 0.8f, 1f, 1f);
                }
            }
        }
    }

    public record struct HoverItemData(int Type, int Stack);

    public class HoverItemDummy : ModItem
    {
        public override string Texture => "InfernalEclipseAPI/Assets/Textures/Backgrounds/BlankPixel";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 0;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = 0;
        }
    }

    public class ChestLockIcon : HoverItemDummy
    {
        public override string Texture => "CalamityMod/UI/ModeIndicator/ModeIndicatorLock";
    }
}
