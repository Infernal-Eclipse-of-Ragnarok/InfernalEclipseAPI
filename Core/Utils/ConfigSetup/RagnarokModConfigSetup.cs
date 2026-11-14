using System.IO;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfernalEclipseAPI.Core.Utils.ConfigSetup
{
    [JITWhenModsEnabled(InfernalCrossmod.RagnarokMod.Name)]
    [ExtendsFromMod(InfernalCrossmod.RagnarokMod.Name)]
    public static class RagnarokModConfigSetup
    {
        private static JObject DesiredBossDefaults() => new JObject
        {
            ["bossstatstweak"] = false,
            ["bossrush"] = 3,
            ["bird"] = 3,
            ["jelly"] = 3,
            ["viscount"] = 3,
            ["granite"] = 3,
            ["champion"] = 3,
            ["scouter"] = 3,
            ["strider"] = 3
        };

        private static JObject DesiredBossProgression() => new JObject
        {
            ["EyeOfDesolation"] = false,
            ["DeathWhistle"] = false,
            ["RuneOfKos"] = false
        };

        private static JObject DesiredItemBalancer() => new JObject
        {
            ["genericweaponchanges"] = false
        };

        private static JObject DesiredModCompat() => new JObject
        {
            ["item_deduplication_mode"] = 0
        };

        public static void SetupConfigs(string cfgDir)
        {
            SetupConfig(Path.Combine(cfgDir, "RagnarokMod_BossConfig.json"), DesiredBossDefaults());
            SetupConfig(Path.Combine(cfgDir, "RagnarokMod_BossProgressionConfig.json"), DesiredBossProgression());
            SetupConfig(Path.Combine(cfgDir, "RagnarokMod_ItemBalancerConfig.json"), DesiredItemBalancer());
            SetupConfig(Path.Combine(cfgDir, "RagnarokMod_ModCompatConfig.json"), DesiredModCompat());
        }

        private static void SetupConfig(string cfgPath, JObject desired)
        {
            if (!File.Exists(cfgPath))
            {
                AtomicWrite(cfgPath, desired);
                return;
            }

            try
            {
                var existing = JObject.Parse(File.ReadAllText(cfgPath));

                foreach (var prop in desired)
                    existing[prop.Key] = prop.Value;

                AtomicWrite(cfgPath, existing);
            }
            catch
            {
                AtomicWrite(cfgPath, desired);
            }
        }

        private static void AtomicWrite(string path, JObject json)
        {
            string tmp = path + ".tmp";
            File.WriteAllText(tmp, json.ToString(Formatting.Indented));
            File.Copy(tmp, path, overwrite: true);
            File.Delete(tmp);
        }
    }
}
