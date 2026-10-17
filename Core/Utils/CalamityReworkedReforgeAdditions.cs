using System.Reflection;
using CalamityMod.Prefixes;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using SOTS.Void;
using SOTSBardHealer;
using Terraria.Utilities;
using ThoriumMod;

namespace InfernalEclipseAPI.Core.Utils
{
    public class CalamityReforgeIL : ModSystem
    {
        private static ILHook _hook;

        public override void Load()
        {
            MethodInfo m = typeof(ReforgeChange).GetMethod(
                "GetReworkedReforge",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
            );

            if (m == null)
                throw new MissingMethodException("ReforgeChange.GetReworkedReforge(Item, UnifiedRandom, int) not found.");

            _hook = new ILHook(m, InjectCrossmodReforgeLogic);
        }

        public override void Unload()
        {
            _hook?.Dispose();
            _hook = null;
        }

        private static void InjectCrossmodReforgeLogic(ILContext il)
        {
            var c = new ILCursor(il);

            // Find: int prefix = -1;
            int prefixLocal = -1;
            if (!c.TryGotoNext(
                MoveType.After,
                i => i.MatchLdcI4(-1),
                i => i.MatchStloc(out prefixLocal)
            ))
            {
                InfernalEclipseAPI.Instance.Logger.Error("[CalamityReforgeIL] Failed to locate 'prefix = -1' initialization in ReforgeChange.GetReworkedReforge.");
                return;
            }

            // Labels
            var skipSOTS = c.DefineLabel();
            var skipThorium = c.DefineLabel();
            var skipBH = c.DefineLabel();

            // ------------------------------------------------------------
            // if (InfernalCrossmod.SOTS.Loaded)
            // {
            //     prefix = CalamityVoidClassReforgeRework.ChooseSOTSPrefix(item, rand, currentPrefix);
            //     if (prefix != -1) return prefix;
            // }
            // ------------------------------------------------------------
            EmitNestedLoadedCheckOrBranchFalse(c, nestedClassName: "SOTS", skipLabel: skipSOTS);

            c.Emit(OpCodes.Ldarg_0); // item
            c.Emit(OpCodes.Ldarg_1); // rand
            c.Emit(OpCodes.Ldarg_2); // currentPrefix
            c.Emit(OpCodes.Call, RequireMethod(typeof(CalamityVoidClassReforgeRework), "ChooseSOTSPrefix"));

            c.Emit(OpCodes.Stloc, prefixLocal);
            EmitReturnIfLocalNotMinusOne(c, prefixLocal);

            c.MarkLabel(skipSOTS);

            // ------------------------------------------------------------
            // if (InfernalCrossmod.Thorium.Loaded)
            // {
            //     if (InfernalCrossmod.SOTSBardHealer.Loaded)
            //     {
            //         prefix = CalamitySOTSThoriumVoidHybridClassReforgeRework.ChooseSOTSPrefix(item, rand, currentPrefix);
            //         if (prefix != -1) return prefix;
            //     }
            //
            //     prefix = CalamityThoriumClassReforgeRework.ChooseThoriumPrefix(item, rand, currentPrefix);
            //     if (prefix != -1) return prefix;
            // }
            // ------------------------------------------------------------
            EmitNestedLoadedCheckOrBranchFalse(c, nestedClassName: "Thorium", skipLabel: skipThorium);

            EmitNestedLoadedCheckOrBranchFalse(c, nestedClassName: "SOTSBardHealer", skipLabel: skipBH);

            c.Emit(OpCodes.Ldarg_0); // item
            c.Emit(OpCodes.Ldarg_1); // rand
            c.Emit(OpCodes.Ldarg_2); // currentPrefix
            c.Emit(OpCodes.Call, RequireMethod(typeof(CalamitySOTSThoriumVoidHybridClassReforgeRework), "ChooseSOTSPrefix"));

            c.Emit(OpCodes.Stloc, prefixLocal);
            EmitReturnIfLocalNotMinusOne(c, prefixLocal);

            c.MarkLabel(skipBH);

            c.Emit(OpCodes.Ldarg_0); // item
            c.Emit(OpCodes.Ldarg_1); // rand
            c.Emit(OpCodes.Ldarg_2); // currentPrefix
            c.Emit(OpCodes.Call, RequireMethod(typeof(CalamityThoriumClassReforgeRework), "ChooseThoriumPrefix"));

            c.Emit(OpCodes.Stloc, prefixLocal);
            EmitReturnIfLocalNotMinusOne(c, prefixLocal);

            c.MarkLabel(skipThorium);

            InfernalEclipseAPI.Instance.Logger.Info("[CalamityReforgeIL] Injection applied successfully (ReforgeChange.GetReworkedReforge).");
        }

        /// <summary>
        /// Emits IL that evaluates InfernalCrossmod.{nestedClassName}.Loaded and branches to skipLabel if false.
        /// Supports either a static bool property 'Loaded' or a static bool field 'Loaded'.
        /// </summary>
        private static void EmitNestedLoadedCheckOrBranchFalse(ILCursor c, string nestedClassName, ILLabel skipLabel)
        {
            Type nested = typeof(InfernalCrossmod).GetNestedType(nestedClassName, BindingFlags.Public);
            if (nested == null)
                throw new MissingMemberException($"InfernalCrossmod nested class '{nestedClassName}' not found.");

            var loadedProp = nested.GetProperty("Loaded", BindingFlags.Public | BindingFlags.Static);
            if (loadedProp?.GetGetMethod() != null)
            {
                c.Emit(OpCodes.Call, loadedProp.GetGetMethod());
                c.Emit(OpCodes.Brfalse, skipLabel);
                return;
            }

            var loadedField = nested.GetField("Loaded", BindingFlags.Public | BindingFlags.Static);
            if (loadedField != null && loadedField.FieldType == typeof(bool))
            {
                c.Emit(OpCodes.Ldsfld, loadedField);
                c.Emit(OpCodes.Brfalse, skipLabel);
                return;
            }

            throw new MissingMemberException($"InfernalCrossmod.{nestedClassName}.Loaded not found as static property or field.");
        }

        /// <summary>
        /// Emits:
        /// if (local != -1) return local;
        /// </summary>
        private static void EmitReturnIfLocalNotMinusOne(ILCursor c, int localIndex)
        {
            var cont = c.DefineLabel();

            c.Emit(OpCodes.Ldloc, localIndex);
            c.Emit(OpCodes.Ldc_I4_M1);
            c.Emit(OpCodes.Beq, cont);

            c.Emit(OpCodes.Ldloc, localIndex);
            c.Emit(OpCodes.Ret);

            c.MarkLabel(cont);
        }

        private static MethodInfo RequireMethod(Type t, string name)
        {
            var m = t.GetMethod(name, BindingFlags.Public | BindingFlags.Static);
            if (m == null)
                throw new MissingMethodException($"{t.FullName}.{name} not found (public static).");
            return m;
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public static class CalamityThoriumClassReforgeRework
    {
        public static int ChooseThoriumPrefix(Item item, UnifiedRandom rand, int storedPrefix)
        {
            if (!item.CountsAsClass<HealerDamage>() && !item.CountsAsClass<HealerTool>() && !item.CountsAsClass<HealerToolDamageHybrid>() && !item.CountsAsClass<BardDamage>()) return -1;

            return ThoriumItemUtils.GetReworkedReforge(item, rand, storedPrefix);
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public static class CalamityVoidClassReforgeRework
    {
        public static int ChooseSOTSPrefix(Item item, UnifiedRandom rand, int storedPrefix)
        {
            if (item.ModItem is not VoidItem) return -1;

            //Main.NewText("Detected SOTS Void Item for Reforge Rework");

            return SOTSItemUtils.GetReworkedReforge(item, rand, storedPrefix);
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTSBardHealer.Name)]
    [ExtendsFromMod("SOTSBardHealer")]
    public static class CalamitySOTSThoriumVoidHybridClassReforgeRework
    {
        public static int ChooseSOTSPrefix(Item item, UnifiedRandom rand, int storedPrefix)
        {
            if (item.ModItem is not VoidHybrid) return -1;

            //Main.NewText("Detected SOTS Bard Healer Void Hybrid Item for Reforge Rework");

            return SOTSBardHealerItemUtils.GetReworkedReforge(item, rand, storedPrefix);
        }
    }
}
