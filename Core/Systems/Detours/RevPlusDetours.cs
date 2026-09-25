using Microsoft.Xna.Framework;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace InfernalEclipseAPI.Core.Systems.Detours
{
    public class RevPlusPolarisIntroChange : ModSystem
    {
        private ILHook ilHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("RevengeancePlus", out Mod revPlus))
                return;

            var infernumTitlesType = revPlus.Code.GetType("RevengeancePlus.InfernumTitles");
            if (infernumTitlesType == null)
                return;

            var method = infernumTitlesType.GetMethod("PostSetupContent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
                return;

            ilHook = new ILHook(method, IL_RemovePolarisAddInfernumTitle);
        }

        public override void Unload()
        {
            ilHook?.Dispose();
            ilHook = null;
        }

        private void IL_RemovePolarisAddInfernumTitle(ILContext il)
        {
            var c = new ILCursor(il);
            int removed = 0;

            while (removed < 2)
            {
                // Find ldstr "Polaris" or "NewPolaris"
                if (!c.TryGotoNext(i => i.MatchLdstr("Polaris") || i.MatchLdstr("NewPolaris")))
                    break;

                // Find the AddInfernumTitle call (should be 26 ahead)
                if (!c.TryGotoNext(i => i.MatchCall(typeof(RevengeancePlus.InfernumTitles), "AddInfernumTitle")))
                    break;

                int callIndex = c.Index;

                // NOP all instructions from callIndex - 27 up to and including callIndex
                for (int i = 0; i <= 27; i++)
                {
                    c.Index = callIndex - i;
                    c.Next.OpCode = Mono.Cecil.Cil.OpCodes.Nop;
                    c.Next.Operand = null;
                }

                removed++;
                c.Index = callIndex + 1; // Move cursor forward for next match
            }
        }
    }

    public class RevPlusOcramIntroChange : ModSystem
    {
        private static Hook hook;

        public override void Load()
        {
            if (!ModLoader.HasMod("RevengeancePlus"))
                return;

            var asm = ModLoader.GetMod("RevengeancePlus").Code;
            var type = asm.GetType("RevengeancePlus.InfernumTitles");
            var method = type?.GetMethod("AddInfernumTitle", BindingFlags.Static | BindingFlags.NonPublic);

            if (method != null)
                hook = new Hook(method, AddInfernumTitle_Detour);
        }

        public override void Unload()
        {
            hook?.Dispose();
            hook = null;
        }

        private static void AddInfernumTitle_Detour(Action<ModNPC, Color[], float[], Color, float> orig, ModNPC mnpc, Color[] titleColors, float[] healthGates, Color screenOverlay, float fontSize)
        {
            if (mnpc?.Name == "Ocram")
                screenOverlay = Color.Multiply(Color.BlueViolet, 0.1f);

            orig(mnpc, titleColors, healthGates, screenOverlay, fontSize);
        }
    }
}
