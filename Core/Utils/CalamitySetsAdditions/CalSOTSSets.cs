
using CalamityMod.Systems.Collections;
using SOTS.Items.Wings;
using SOTS.Projectiles.Earth;
using SOTS.Projectiles.Temple;

namespace InfernalEclipseAPI.Core.Utils.CalamitySetsAdditions
{
    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class CalSOTSSets : ModSystem
    {
        public override void PostSetupContent()
        {
            int[] hasAccessoryKeybind =
            {
                ModContent.ItemType<GildedBladeWings>()
            };

            foreach (int item in hasAccessoryKeybind)
            {
                CalamityItemSets.HasAccessoryKeybind[item] = true;
            }

            int[] noHomingWithGrapeBeer =
            {
                ModContent.ProjectileType<VibrantShard>(),
                ModContent.ProjectileType<SolarPetal>()
            };

            foreach (int proj in noHomingWithGrapeBeer)
            {
                CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[proj] = true;
            }
        }
    }
}
