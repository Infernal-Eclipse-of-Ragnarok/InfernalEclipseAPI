using System.Collections.Generic;
using InfernalEclipseAPI.Content.Projectiles;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using ThoriumMod;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Weapons.Healer.Scythes
{
    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class ThoriumHeavyScythePatch : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            if (InfernalCrossmod.ThoriumRework.Mod.TryFind("ThoriumHeavyScythe", out ModItem item))
            {
                return entity.type == item.Type;
            }
            return false;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int sparkProj = ModContent.ProjectileType<ThoriumHeavyScytheSpark>();

            // --- Always aim toward cursor ---
            Vector2 aimDirection = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);

            // --- Muzzle offset ---
            Vector2 muzzleOffset = aimDirection * 80f;

            // --- Delay firing slightly ---
            int delayFrames = 20;
            int sparkDamage = (int)(damage - damage / 3f);
            float sparkKnockback = knockback;
            int sparkOwner = player.whoAmI;

            // --- Two-projectile spread (offset from player instead of muzzle) ---
            float sideOffsetDistance = 15f; // how far apart they are (from player center)
            float spreadAngle = ToRadians(3f); // slight directional spread

            player.GetModPlayer<DelayedProjectileHelper>().ScheduleDelayedProjectile(delayFrames, () =>
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    // Offset spawn position to left/right of aim direction
                    Vector2 perpendicular = aimDirection.RotatedBy(MathHelper.PiOver2) * (sideOffsetDistance * i);
                    Vector2 spawnPosition = player.Center + muzzleOffset + perpendicular;

                    // Slight rotation for each projectile’s direction
                    Vector2 spreadDir = aimDirection.RotatedBy(spreadAngle * i);

                    int proj = Projectile.NewProjectile(source, spawnPosition, spreadDir * 10f, sparkProj, sparkDamage, sparkKnockback, sparkOwner);

                    // Make it Radiant / Healer damage
                    if (proj >= 0 && Main.projectile.IndexInRange(proj))
                    {
                        Main.projectile[proj].DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                    }
                }

                // Optional visual feedback
                SoundEngine.PlaySound(SoundID.Item20, player.Center);
                Lighting.AddLight(player.Center, Color.Gold.ToVector3() * 0.5f);
            });

            return true;
        }
    }

    // --- Helper for delayed projectile firing ---
    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class DelayedProjectileHelper : ModPlayer
    {
        private class DelayedAction
        {
            public int Frames;
            public Action Action;
            public DelayedAction(int frames, Action action)
            {
                Frames = frames;
                Action = action;
            }
        }

        private readonly Queue<DelayedAction> queuedProjectiles = new Queue<DelayedAction>();

        public override void PostUpdate()
        {
            if (queuedProjectiles.Count == 0)
                return;

            int count = queuedProjectiles.Count;
            for (int i = 0; i < count; i++)
            {
                DelayedAction entry = queuedProjectiles.Dequeue();
                entry.Frames--;
                if (entry.Frames <= 0)
                    entry.Action?.Invoke();
                else
                    queuedProjectiles.Enqueue(entry);
            }
        }

        public void ScheduleDelayedProjectile(int frames, Action action)
        {
            queuedProjectiles.Enqueue(new DelayedAction(frames, action));
        }
    }
}
