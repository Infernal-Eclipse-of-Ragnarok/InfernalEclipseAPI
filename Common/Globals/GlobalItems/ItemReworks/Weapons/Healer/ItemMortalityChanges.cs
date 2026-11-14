using System.Collections.Generic;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Weapons.Healer
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ItemMortalityChanges : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !InfernalCrossmod.Hummus.Loaded;
        }

        public override bool InstancePerEntity => false;
        private static HashSet<int> mortalityItems;
        private static bool initialized;

        public static Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thorium);
                return thorium;
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!IsMortality(item)) return base.Shoot(item, player, source, position, velocity, type, damage, knockback);

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) && thorium.TryFind("Mortality", out ModBuff mortality))
            {
                player.AddBuff(mortality.Type, 600);
            }

            return true;
        }

        public override void HoldItem(Item item, Player player)
        {
            if (!IsMortality(item))
                return;

            if (!player.channel)
                return;

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) &&
                thorium.TryFind("Mortality", out ModBuff mortality))
            {
                // refresh debuff while channeling
                player.AddBuff(mortality.Type, 2);
            }
        }

        private bool IsMortality(Item item)
        {
            EnsureInitialized();

            return item != null && mortalityItems.Contains(item.type);
        }

        private void EnsureInitialized()
        {
            if (initialized)
                return;

            mortalityItems = new HashSet<int>();

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                if (thorium.TryFind("Recuperate", out ModItem recuperate))
                    mortalityItems.Add(recuperate.Type);

                if (thorium.TryFind("UnboundFantasy", out ModItem unbound))
                    mortalityItems.Add(unbound.Type);
            }

            initialized = true;
        }
    }

    public class MortalityProjectileChanges : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            if (!ModLoader.TryGetMod("Consolaria", out Mod consolaria))
                return;

            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                return;

            if (!thorium.TryFind("Mortality", out ModBuff mortality))
                return;

            int buffetUse = consolaria.Find<ModProjectile>("FiveStarBuffet_Use").Type;

            if (projectile.type == buffetUse)
            {
                Player player = Main.player[projectile.owner];

                if (player.channel)
                {
                    player.AddBuff(mortality.Type, 2);
                }
            }
        }
    }
}
