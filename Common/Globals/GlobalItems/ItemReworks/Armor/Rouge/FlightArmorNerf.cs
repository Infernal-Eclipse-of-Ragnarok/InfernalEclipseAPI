using InfernalEclipseAPI.Core.Systems.Compatibility;
using ThoriumMod.Items.ArcaneArmor;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Armor.Rouge
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class FlightArmorNerf : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            FlightMask.wingsSlot = EquipLoader.AddEquipTexture(Mod, "ThoriumMod/Items/ArcaneArmor/FlightMask_WingsFake", EquipType.Wings, name: "FlightMaskWings", equipTexture: new FlightWings());
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<FlightMask>();
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (head.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>("FlightMask").Type && body.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>("FlightMail").Type && legs.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>("FlightBoots").Type)
                {
                    return "Thorium:FlightSet";
                }
            }

            return base.IsArmorSet(head, body, legs);
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "Thorium:FlightSet")
                return;

            player.jumpBoost = false;
            player.GetThoriumPlayer().setFlight = false;

            const int ourWingTime = 25;

            if (player.wings <= 0 || player.wingTimeMax < ourWingTime)
            {
                player.wings = FlightMask.wingsSlot;
                player.wingsLogic = FlightMask.wingsSlot;
                player.wingTimeMax = 25;
            }
        }
    }

    public class FlightWings : EquipTexture
    {
        public override bool WingUpdate(Player player, bool inUse)
        {
            int frames = 6;
            int frameTime;

            if (inUse || player.jump > 0)
                frameTime = 4;
            else if (player.velocity.Y != 0.0)
                frameTime = !player.controlJump ? 6 : 9;
            else
            {
                frameTime = 0;
                player.wingFrame = 0;
            }

            if (frameTime > 0)
            {
                ++player.wingFrameCounter;
                if (player.wingFrameCounter >= frameTime)
                {
                    player.wingFrameCounter = 0;
                    ++player.wingFrame;
                    if (player.wingFrame == 0 || player.wingFrame >= frames)
                        player.wingFrame = 1;
                }
            }
            return true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0f;
            ascentWhenRising = 0.10f;
            maxCanAscendMultiplier = 0.75f;
            maxAscentMultiplier = 0.75f;
            constantAscend = 0.03f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 1f;
            acceleration = 0.25f;
        }
    }
}
