using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Weapons.Bard
{
    //WardrobeHummus
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class BoomboxRadioOverride : GlobalItem
    {
        public override bool InstancePerEntity => true;

        // Cache Thorium weapon and projectile types
        private static int GraniteBoomBoxID = -1;
        private static int LodestoneRadioID = -1;

        private static int GraniteBoomBoxProID = -1;
        private static int LodestoneRadioProID = -1;

        public override void Load()
        {
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thor))
            {
                GraniteBoomBoxID = thor.Find<ModItem>("GraniteBoomBox").Type;
                LodestoneRadioID = thor.Find<ModItem>("LodestoneRadio").Type;

                GraniteBoomBoxProID = thor.Find<ModProjectile>("GraniteBoomBoxPro").Type;
                LodestoneRadioProID = thor.Find<ModProjectile>("LodestoneRadioPro").Type;
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Thorium not installed or lookups failed
            if (GraniteBoomBoxID == -1 || LodestoneRadioID == -1)
                return base.Shoot(item, player, source, position, velocity, type, damage, knockback);

            // ================================
            //   Granite Boom Box used
            // ================================
            if (item.type == GraniteBoomBoxID)
            {
                KillPlayerProjectiles(player, LodestoneRadioProID);
            }

            // ================================
            //   Lodestone Radio used
            // ================================
            if (item.type == LodestoneRadioID)
            {
                KillPlayerProjectiles(player, GraniteBoomBoxProID);
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        private void KillPlayerProjectiles(Player player, int projType)
        {
            if (projType <= 0) return;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == projType)
                {
                    proj.Kill();
                }
            }
        }
    }
}
