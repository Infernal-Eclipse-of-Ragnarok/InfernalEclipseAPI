using System.Collections.Generic;
using CalamityMod.Systems.Collections;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using ThoriumMod.Projectiles;

namespace InfernalEclipseAPI.Core.Utils.CalamitySetsAdditions
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class CalThorSets : ModSystem
    {
        public override void PostSetupContent()
        {
            int[] noHomingWithGrapeBeer =
            {
                ModContent.ProjectileType<DragonPulse>(),
                ModContent.ProjectileType<OmniCannonPro2>(),
                ModContent.ProjectileType<OmniCannonPro3>(),
                ModContent.ProjectileType<OmniCannonPro4>(),
            };

            foreach (int proj in noHomingWithGrapeBeer)
            {
                CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[proj] = true;
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                Mod rework = InfernalCrossmod.ThoriumRework.Mod;

                List<int> noHomingWithGrapeBeerRework = new();

                if (rework.TryFind("TerrariumPulseRay", out ModProjectile pulseRay))
                {
                    noHomingWithGrapeBeerRework.Add(pulseRay.Type);
                }

                foreach (int proj in noHomingWithGrapeBeerRework)
                {
                    CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[proj] = true;
                }
            }
        }
    }
}
