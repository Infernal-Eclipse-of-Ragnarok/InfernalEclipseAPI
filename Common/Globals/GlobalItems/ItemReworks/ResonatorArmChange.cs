using InfernalEclipseAPI.Core.Players;
using Terraria.DataStructures;
using ThoriumMod.Items.Donate;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalItems.ItemReworks
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ResonatorArmChange : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ModContent.ItemType<ResonatorsArm>();

        public override bool CanUseItem(Item item, Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.GetModPlayer<InfernalPlayer>().resonatorTimer > 0)
                    return false;
            }
            return true;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<InfernalPlayer>().resonatorTimer = 180;
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
