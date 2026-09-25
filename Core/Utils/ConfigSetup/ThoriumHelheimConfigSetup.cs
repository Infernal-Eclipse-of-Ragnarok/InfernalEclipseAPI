using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThoriumRework;

namespace InfernalEclipseAPI.Core.Utils.ConfigSetup
{
    [JITWhenModsEnabled("ThoriumRework")]
    [ExtendsFromMod("ThoriumRework")]
    public static class ThoriumHelheimConfigSetup
    {
        public static void DisableItemReworks()
        {
            ModContent.GetInstance<ItemReworksConfig>().ChampionsRebuttal = false;
        }
        private static JObject DesiredItemReworks() => new()
        {
            ["ChampionsRebuttal"] = false
        };
        public static void SetupConfigs(string cfgDir)
        {
            SetupConfig(Path.Combine(cfgDir, "ThoriumRework_ItemReworksConfig.json"), DesiredItemReworks());
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
