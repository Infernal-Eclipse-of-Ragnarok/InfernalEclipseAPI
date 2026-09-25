using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace InfernalEclipseAPI.Core.Systems.Hooks.BossChecklistChanges
{
    public class HypnosBossChecklistOverride : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        private static ILHook hook;

        public override void Load()
        {
            if (!ModLoader.HasMod("HypnosMod"))
                return;

            Mod hypnos = ModLoader.GetMod("HypnosMod");
            Assembly asm = hypnos.Code;

            // namespace HypnosMod { public class HypnosMod : Mod { ... } }
            Type t = asm.GetType("HypnosMod.HypnosMod");
            MethodInfo m = t?.GetMethod("PostSetupContent", BindingFlags.Instance | BindingFlags.Public);

            if (m is null)
                return;

            hook = new ILHook(m, Patch_PostSetupContent);
        }

        public override void Unload()
        {
            hook?.Dispose();
            hook = null;
        }

        private static void Patch_PostSetupContent(ILContext il)
        {
            var c = new ILCursor(il);

            // Replace the FIRST occurrence of 22.5f with 22.991f.
            if (c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(22.5f)))
            {
                // We are positioned AFTER the ldc.r4 instruction; go back one and replace it.
                c.Index--;
                c.Remove();
                c.EmitLdcR4(22.991f);
            }
        }
    }
}
