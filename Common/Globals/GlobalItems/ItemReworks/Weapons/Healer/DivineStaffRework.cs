using ThoriumMod.Projectiles.Healer;
using ThoriumMod;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Weapons.Healer
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class DivineStaffRework : GlobalItem
    {
        private static readonly Mod thorium = InfernalCrossmod.Thorium.Mod;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return thorium != null && entity.type == thorium.Find<ModItem>("DivineStaff").Type;
        }

        public override void SetDefaults(Item item)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("DivineStaff").Type)
            {
                Item.staff[item.type] = true;

                item.damage = 225;
                item.knockBack = 4;
                item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                item.useStyle = ItemUseStyleID.Shoot;
                item.noMelee = true;
                item.autoReuse = true;
            }
        }

        public override bool AllowPrefix(Item item, int pre)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("DivineStaff").Type)
                return true;
            return base.AllowPrefix(item, pre);
        }

        public override bool CanReforge(Item item)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("DivineStaff").Type)
                return true;
            return base.CanReforge(item);
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class HealingBeamGlobal : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
        {
            return projectile.type == ModContent.ProjectileType<HealingBeam>();
        }

        public override void SetDefaults(Projectile projectile)
        {
            projectile.friendly = true;
            projectile.hostile = false;

            projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            projectile.penetrate = 4;
        }
    }
}
