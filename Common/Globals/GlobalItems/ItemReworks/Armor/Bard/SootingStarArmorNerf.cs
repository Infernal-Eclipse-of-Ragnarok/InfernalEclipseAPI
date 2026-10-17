using System.Reflection;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using MonoMod.Cil;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Armor.Bard
{
    //WardrobeHummus
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class SootingStarArmorNerf : ModSystem
    {
        public override void Load()
        {
            var loaderType = InfernalCrossmod.Thorium.Mod.Code.GetType("ThoriumMod.Empowerments.EmpowermentLoader");
            if (loaderType == null) return;

            var globalUpdateMethod = loaderType.GetMethod("GlobalUpdate", BindingFlags.NonPublic | BindingFlags.Static);
            if (globalUpdateMethod == null) return;

            MonoModHooks.Modify(globalUpdateMethod, IL_GlobalUpdate);
        }

        private void IL_GlobalUpdate(ILContext il)
        {
            var c = new ILCursor(il);

            // Look for the constant 100f (division of 5 / 100f)
            if (c.TryGotoNext(i => i.MatchLdcR4(100f)))
            {
                // Replace 100f with 200f - halves the bonus
                c.Next.Operand = 200f;
            }
        }
    }
}
