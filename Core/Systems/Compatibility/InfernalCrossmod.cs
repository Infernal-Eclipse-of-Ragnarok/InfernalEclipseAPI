using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfernalEclipseAPI.Core.Configs;

namespace InfernalEclipseAPI.Core.Systems.Compatibility
{
    public static class InfernalCrossmod
    {
        public static class Calamity
        {
            public static string Name = "CalamityMod";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }

        public static class Entropy
        {
            public static string Name = "CalamityEntropy";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }

        public static class Hunt
        {
            public static string Name = "CalamityHunt";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }

        public static class ThrowerUnification
        {
            public const string Name = "ThrowerUnification";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);

        }

        public static class Catalyst
        {
            public const string Name = "CatalystMod";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Clamity
        {
            public const string Name = "Clamity";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }
        public static class ClamityMusic
        {
            public const string Name = "ClamityMusic";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Consolaria
        {
            public const string Name = "Consolaria";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class FargosMutant
        {
            public const string Name = "Fargowiltas";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }
        public static class FargosSouls
        {
            public const string Name = "FargowiltasSouls";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }
        public static class FargosDLC
        {
            public const string Name = "FargowiltasCrossmod";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class InfernalEclipseWeaponsDLC
        {
            public const string Name = "InfernalEclipseWeaponsDLC";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Luminance
        {
            public const string Name = "Luminance";
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class NoxusBoss
        {
            public const string Name = "NoxusBoss";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class RagnarokMod
        {
            public const string Name = "RagnarokMod";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }
        public static class CalBardHealer
        {
            public const string Name = "CalamityBardHealer";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class RevengeancePlus
        {
            public const string Name = "RevengeancePlus";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);

        }

        public static class SOTSBardHealer
        {
            public const string Name = "SOTSBardHealer";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }
        public static class SOTS
        {
            public const string Name = "SOTS";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class SubworldLibrary
        {
            public const string Name = "SubworldLibrary";
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Thorium
        {
            public const string Name = "ThoriumMod";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }
        public static class ThoriumRework
        {
            public const string Name = "ThoriumRework";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class YouBoss
        {
            public const string Name = "YouBoss";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Starlight
        {
            public const string Name = "ssm";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class MusicDisplay
        {
            public const string Name = "MusicDisplay";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class BossChecklist
        {
            public const string Name = "BossChecklist";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class ColoredDamageTypes
        {
            public const string Name = "ColoredDamageTypes";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Hummus
        {
            public const string Name = "WHummusMultiModBalancing";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class CatalyzedInferno
        {
            public const string Name = "CnI";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class CalamityModMusic
        {
            public const string Name = "CalamityModMusic";
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class BlueMoon
        {
            public const string Name = "BlueMoon";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class Overdrive
        {
            public const string Name = "WulfrumExpansion";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class QoLC
        {
            public const string Name = "QoLCompendium";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);

            public static void RemoveQoLCompendiumInfiniteBuff(Player player, int buffID)
            {
                if (!ModLoader.TryGetMod("QoLCompendium", out Mod qol))
                    return;

                Type qolPlayerType = qol.Code.GetType("QoLCompendium.Core.QoLCPlayer");

                if (qolPlayerType is null)
                {
                    if (InfernalConfig.Instance.DeveloperMode)
                        Main.NewText("Error finding QoLCPlayer type");
                    return;
                }

                MethodInfo getModPlayer = typeof(Player).GetMethods().First(m => m.Name == "GetModPlayer" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);

                object qolPlayer = getModPlayer.MakeGenericMethod(qolPlayerType).Invoke(player, null);

                if (qolPlayer is null)
                {
                    if (InfernalConfig.Instance.DeveloperMode)
                        Main.NewText("Error finding QoLCPlayer");

                    return;
                }

                FieldInfo activeBuffsField = qolPlayerType.GetField("activeBuffs");

                if (activeBuffsField?.GetValue(qolPlayer) is List<int> activeBuffs && activeBuffs.Contains(buffID))
                    activeBuffs.Remove(buffID);
                else if (InfernalConfig.Instance.DeveloperMode)
                    Main.NewText("Error finding activeBuffs field; buff not removed from infinite buffs.");
            }
        }
    }
}