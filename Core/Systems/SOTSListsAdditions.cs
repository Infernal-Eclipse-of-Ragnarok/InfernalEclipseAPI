using System.Collections.Generic;
using System.Reflection;
using SOTS;
using ThoriumMod.Projectiles;

namespace InfernalEclipseAPI.Core.Systems
{
    [JITWhenModsEnabled("SOTS", "ThoriumMod")]
    [ExtendsFromMod("SOTS", "ThoriumMod")]
    public class ThorSOTSListsAdditions : ModSystem
    {
        private static readonly FieldInfo blacklistField = typeof(SOTSPlayer).GetField(nameof(SOTSPlayer.HomingProjectileBlacklist), BindingFlags.Public | BindingFlags.Static);

        private static void AddToBlacklist(IEnumerable<int> additions)
        {
            HashSet<int> current = (HashSet<int>)blacklistField.GetValue(null);

            current.UnionWith(additions);
        }

        public override void Load()
        {
            AddToBlacklist(new[]
            {
                ModContent.ProjectileType<DragonPulse>(),
                ModContent.ProjectileType<OmniCannonPro2>(),
                ModContent.ProjectileType<OmniCannonPro3>(),
                ModContent.ProjectileType<OmniCannonPro4>(),
            });

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                Mod rework = InfernalCrossmod.ThoriumRework.Mod;

                List<int> reworkAdditions = new();

                if (rework.TryFind("TerrariumPulseRay", out ModProjectile pulseRay))
                {
                    reworkAdditions.Add(pulseRay.Type);
                }

                AddToBlacklist(reworkAdditions);
            }
        }

        public override void PostSetupContent()
        {
            AddSpear("ThoriumMod", "RifleSpear");
            AddSpear("SOTS", "GoldGlaive");
            AddSpear("SOTS", "Riptide");
            AddSpear("SOTS", "AncientSteelHalberd");
            AddSpear("SOTS", "ImperialPike");
            AddSpear("SOTS", "CursedImpale");
            AddSpear("SOTS", "HardlightGlaive");
            AddSpear("SOTS", "Helios");
        }

        private void AddSpear(string modName, string itemName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod))
                return;

            if (!mod.TryFind(itemName, out ModItem spear))
                return;

            ItemID.Sets.Spears[spear.Type] = true;
        }
    }

    [JITWhenModsEnabled("SOTS", "CalamityAmmo")]
    [ExtendsFromMod("SOTS", "CalamityAmmo")]
    public class AmmoSOTSListsAdditions : ModSystem
    {
        public override void Load()
        {
            if (ModLoader.TryGetMod("CalamityAmmo", out Mod calAmmo))
            {
                FieldInfo blacklistField = typeof(SOTSPlayer).GetField(nameof(SOTSPlayer.HomingProjectileBlacklist), BindingFlags.Public | BindingFlags.Static);

                HashSet<int> current = (HashSet<int>)blacklistField.GetValue(null);

                current.Add(calAmmo.Find<ModProjectile>("seaPrismShard").Type);
            }
        }
    }
}