using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace InfernalEclipseAPI.Core.Systems.Hooks.BossChecklistChanges
{
    [JITWhenModsEnabled("Consolaria")]
    [ExtendsFromMod("Consolaria")]
    public class ConsolariaBCLkeyChanger : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        private ILHook ilHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("Consolaria", out Mod consolaria))
                return;

            Type integrationType = consolaria.Code.GetType("Consolaria.Common.CrossContentIntegration");
            if (integrationType == null)
                return;

            MethodInfo method = integrationType.GetMethod(
                "DoBossChecklistIntegration",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
                return;

            ilHook = new ILHook(method, EditOcramBossChecklistValue);
        }

        public override void Unload()
        {
            ilHook?.Dispose();
            ilHook = null;
        }

        private void EditOcramBossChecklistValue(ILContext il)
        {
            ILCursor c = new(il);

            while (c.TryGotoNext(MoveType.After, i => i.MatchLdstr("Ocram")))
            {
                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(13f)))
                    break;

                c.Prev.Operand = 12.99f;
                return;
            }
        }
    }
}
