using System.Collections.Generic;
using System.Reflection;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Weapons.Healer
{
    public class HealingChanges : GlobalItem
    {
        // Heal overrides by mod and item name
        internal static readonly Dictionary<string, Dictionary<string, int>> healOverridesByMod = new()
        {
            ["CalamityBardHealer"] = new Dictionary<string, int>
            {
                //["PurgatoriumPandemonium"] = 3,
            },
            ["ThoriumMod"] = new Dictionary<string, int>
            {
                ["CelestialWand"] = 1,
                ["Gauze"] = 5,
                ["RecoveryWand"] = 5,
                ["TheGoodBook"] = 4,
                ["TheGigaNeedle"] = 6,
                ["LargePopcorn"] = 4,
                ["BrainCoral"] = 5,
                ["EaterOfPain"] = 3,
                ["ChiLantern"] = 5,
                ["SpiritBendersStaff"] = 5,
                // Add more ThoriumMod items here
            },
            ["ThoriumRework"] = new Dictionary<string, int>
            {
                //["RedCresent"] = 3
            }
        };

        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            return item.ModItem != null &&
                   healOverridesByMod.TryGetValue(item.ModItem.Mod.Name, out var itemOverrides) &&
                   itemOverrides.ContainsKey(item.ModItem.Name);
        }

        public override void SetDefaults(Item item)
        {
            if (item.ModItem == null)
                return;

            string modName = item.ModItem.Mod.Name;
            string itemName = item.ModItem.Name;

            if (healOverridesByMod.TryGetValue(modName, out var itemOverrides) &&
                itemOverrides.TryGetValue(itemName, out int newHealAmount))
            {
                SetHealAmount(item.ModItem, newHealAmount);
            }
        }

        private void SetHealAmount(ModItem modItem, int value)
        {
            var type = modItem.GetType();
            var healField = type.GetField("healAmount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (healField != null)
            {
                healField.SetValue(modItem, value);
            }
            else
            {
                Mod.Logger.Warn($"Could not find 'healAmount' field on {modItem.Mod.Name}.{modItem.Name}");
            }
        }
    }
}
