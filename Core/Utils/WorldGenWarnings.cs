using CalamityMod;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.States;

namespace InfernalEclipseAPI.Core.Utils
{
    public class WorldGenWarnings : ModSystem
    {
        // what team lol
        public override void Load()
        {
            IL_UIWorldCreation.AddWorldSizeOptions += SwapMediumDescriptionKey;
            IL_UIWorldCreation.AddWorldSizeOptions += SwapLargeDescriptionKey;
        }
        private static void SwapMediumDescriptionKey(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After, x => x.MatchLdstr("UI.WorldDescriptionSizeMedium")))
            {
                InfernalEclipseAPI.Instance.Logger.ILFailure("Change Medium World Description", "Could not match string \"UI.WorldDescriptionSizeMedium\".");
                return;
            }
            c.Emit(OpCodes.Pop);


            if (ModLoader.HasMod("Remnants"))
                c.Emit(OpCodes.Ldstr, "Mods.InfernalEclipseAPI.UI.WorldWarningRemnants");
            else
                c.Emit(OpCodes.Ldstr, "Mods.InfernalEclipseAPI.UI.MediumWorldWarning");
        }
        private static void SwapLargeDescriptionKey(ILContext il)
        {
            if (ModLoader.HasMod("Remnants"))
            {
                var c = new ILCursor(il);
                if (!c.TryGotoNext(MoveType.After, x => x.MatchLdstr("UI.WorldDescriptionSizeLarge")))
                {
                    InfernalEclipseAPI.Instance.Logger.ILFailure("Change Large World Description", "Could not match string \"UI.WorldDescriptionSizeLarge\".");
                    return;
                }
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldstr, "Mods.InfernalEclipseAPI.UI.WorldWarningRemnants");
            }
        }
    }
}