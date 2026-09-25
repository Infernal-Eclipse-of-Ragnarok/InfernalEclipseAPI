using System.Collections.Generic;
using System.Reflection;

namespace InfernalEclipseAPI.Core.Systems.Hooks.BossChecklistChanges
{
    public class CalamityBCLKeyChanger : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override void Load()
        {
            AdjustEntries();
        }

        private void AdjustEntries()
        {
            try
            {
                // Ensure Calamity is loaded
                Mod calamity = ModLoader.GetMod("CalamityMod");
                if (calamity == null)
                {
                    Mod.Logger.Warn("CalamityMod not loaded, skipping Calamity Boss Checklist patches.");
                    return;
                }

                // Get the internal CalamityMod.WeakReferenceSupport type via its assembly
                Type weakRefType = calamity.Code?.GetType("CalamityMod.WeakReferenceSupport", throwOnError: false, ignoreCase: false);
                if (weakRefType == null)
                {
                    Mod.Logger.Warn("Could not find CalamityMod.WeakReferenceSupport type.");
                    return;
                }

                // Grab the private static BossChecklistProgressionValues dictionary
                FieldInfo dictField = weakRefType.GetField("BossChecklistProgressionValues",
                    BindingFlags.NonPublic | BindingFlags.Static);
                if (dictField == null)
                {
                    Mod.Logger.Warn("Could not find BossChecklistProgressionValues field.");
                    return;
                }

                if (dictField.GetValue(null) is not Dictionary<string, float> dict)
                {
                    Mod.Logger.Warn("BossChecklistProgressionValues is null or of unexpected type.");
                    return;
                }

                // Apply the change
                /*
                if (dict.ContainsKey("GreatSandShark"))
                {
                    dict["GreatSandShark"] = 17.7f;
                    Mod.Logger.Info("Set Calamity GreatSandShark BossChecklist progression value to 17.7f.");
                }
                else
                {
                    Mod.Logger.Warn("GreatSandShark key not found in BossChecklistProgressionValues.");
                }
                */

                if (dict.ContainsKey("BossRush"))
                {
                    dict["BossRush"] = 28.1f;
                    Mod.Logger.Info("Set Calamity BossRush BossChecklist progression value to 28.1f.");
                }
                else
                {
                    Mod.Logger.Warn("BossRush key not found in BossChecklistProgressionValues.");
                }
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"Failed to adjust Calamity Boss Checklist values: {ex}");
            }
        }
    }
}
