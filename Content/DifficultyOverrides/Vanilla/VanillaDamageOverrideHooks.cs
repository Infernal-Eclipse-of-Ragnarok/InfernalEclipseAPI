using System.Reflection;
using InfernalEclipseAPI.Core.World;
using InfernumMode.Content.BehaviorOverrides.BossAIs.WallOfFlesh;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Vanilla
{
    internal sealed class ReduceDetachDamageHook : ModSystem
    {
        public static MethodInfo DetachDamage = typeof(WallOfFleshEyeBehaviorOverride).GetMethod("CheckDead", LumUtils.UniversalBindingFlags);
        public static ILHook ReduceDetachDamage_IL_Hook;

        public override void OnModLoad()
        {
            if (DetachDamage != null)
            {
                ReduceDetachDamage_IL_Hook = new(DetachDamage, ReduceDeatchDamage_IL);
                ReduceDetachDamage_IL_Hook?.Apply();
            }
            else InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: " + this + " returned null on getting MethodInfo");
        }

        public override void OnModUnload()
        {
            ReduceDetachDamage_IL_Hook?.Dispose();
            ReduceDetachDamage_IL_Hook = null;
            DetachDamage = null;
        }

        private static int GetDetachDamage() => InfernalWorld.RagnarokModeEnabled ? WallOfFleshEyeBehaviorOverride.DetachDamage / 2 : WallOfFleshEyeBehaviorOverride.DetachDamage;

        public static void ReduceDeatchDamage_IL(ILContext context)
        {
            ILCursor c = new ILCursor(context);

            while (c.TryGotoNext(i => i.MatchLdcI4(WallOfFleshEyeBehaviorOverride.DetachDamage)))
            {
                c.Remove();
                c.EmitDelegate(GetDetachDamage);
            }
        }
    }
}
