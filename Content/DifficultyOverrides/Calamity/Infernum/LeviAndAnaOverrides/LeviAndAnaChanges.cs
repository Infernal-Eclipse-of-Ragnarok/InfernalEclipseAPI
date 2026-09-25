using System.Reflection;
using InfernalEclipseAPI.Core.World;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Leviathan;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.LeviAndAnaOverrides
{
    internal sealed class AnahitaPhaseThresholdHook : ModSystem
    {
        private static Hook ThresholdHook;
        readonly MethodInfo getter = typeof(AnahitaBehaviorOverride).GetProperty(nameof(AnahitaBehaviorOverride.PhaseLifeRatioThresholds), LumUtils.UniversalBindingFlags)?.GetGetMethod();

        public override void OnModLoad()
        {
            if (getter is null)
            {
                Mod.Logger.Error("[IEoR] Failed to find PhaseLifeRatioThresholds getter.");
                return;
            }

            ThresholdHook = new Hook(getter, EditThresholds);
        }

        public override void Unload()
        {
            ThresholdHook?.Dispose();
            ThresholdHook = null;
        }

        private static float[] EditThresholds(Func<AnahitaBehaviorOverride, float[]> orig, AnahitaBehaviorOverride self)
        {
            if (InfernalWorld.RagnarokModeEnabled)
                return new float[] { 1f };

            return orig(self); // default (0.5f)
        }
    }

    internal sealed class AnahitaLeviathanSummonRatioHook : ModSystem
    {
        private static ILHook AnahitaPreAIHook;
        readonly MethodInfo preAI = typeof(AnahitaBehaviorOverride).GetMethod(nameof(AnahitaBehaviorOverride.PreAI), LumUtils.UniversalBindingFlags);


        public override void OnModLoad()
        {
            if (preAI is null)
            {
                Mod.Logger.Error("[IEoR] Failed to find AnahitaBehaviorOverride.PreAI.");
                return;
            }

            AnahitaPreAIHook = new ILHook(preAI, EditLeviathanSummonLifeRatio);
        }

        public override void Unload()
        {
            AnahitaPreAIHook?.Dispose();
            AnahitaPreAIHook = null;
        }

        private static void EditLeviathanSummonLifeRatio(ILContext il)
        {
            ILCursor c = new(il);

            MethodInfo summonMethod = typeof(AnahitaBehaviorOverride).GetMethod(nameof(AnahitaBehaviorOverride.DoBehavior_SummonLeviathan), LumUtils.UniversalBindingFlags);

            if (summonMethod is null)
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error(
                    "[IEoR] Failed to find DoBehavior_SummonLeviathan."
                );
                return;
            }

            if (!c.TryGotoNext(MoveType.Before, i => i.MatchCall(summonMethod)))
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error(
                    "[IEoR] Failed to find DoBehavior_SummonLeviathan call in Anahita PreAI."
                );
                return;
            }

            int summonCallIndex = c.Index;

            bool patched = false;

            for (int i = summonCallIndex; i >= 0; i--)
            {
                Instruction instr = il.Instrs[i];

                if (instr.OpCode == OpCodes.Ldc_R4 && instr.Operand is float value && value == 0.5f)
                {
                    ILCursor replace = new(il)
                    {
                        Index = i
                    };

                    replace.Remove();

                    replace.EmitDelegate(static () =>
                        InfernalWorld.RagnarokModeEnabled ? 1.1f : 0.5f //1.1f in Ragnarok Mode so she instantly does the spawn.
                    );

                    patched = true;
                    break;
                }
            }

            if (!patched)
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error(
                    "[IEoR] Failed to patch LeviathanSummonLifeRatio. Could not find preceding ldc.r4 0.5f."
                );
            }
        }
    }

    internal sealed class AnahitaReturnLifeRatioHook : ModSystem
    {
        private static ILHook FightStateGetterHook;
        readonly MethodInfo fightStateGetter = typeof(LeviathanComboAttackManager).GetProperty(nameof(LeviathanComboAttackManager.FightState), LumUtils.UniversalBindingFlags)?.GetGetMethod();

        public override void OnModLoad()
        {
            if (fightStateGetter is null)
            {
                Mod.Logger.Error("[IEoR]: Failed to find LeviathanComboAttackManager.FightState getter.");
                return;
            }

            FightStateGetterHook = new ILHook(fightStateGetter, EditAnahitaReturnLifeRatio);
        }

        public override void Unload()
        {
            FightStateGetterHook?.Dispose();
            FightStateGetterHook = null;
        }

        private static void EditAnahitaReturnLifeRatio(ILContext il)
        {
            ILCursor c = new(il);

            int patched = 0;

            while (c.TryGotoNext(
                MoveType.Before,
                i => i.MatchLdloc(out _),        // leviathanLifeRatio
                i => i.MatchLdcR4(0.5f),         // inlined AnahitaReturnLifeRatio
                i => i.MatchBgt(out _) ||
                     i.MatchBgtUn(out _) ||
                     i.MatchBle(out _) ||
                     i.MatchBleUn(out _)
            ))
            {
                c.Index += 1; // Move onto ldc.r4 0.5f.
                c.Remove();

                c.EmitDelegate(static () =>
                    InfernalWorld.RagnarokModeEnabled ? 0.7f : 0.5f
                );

                patched++;
                break;
            }

            if (patched != 1)
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error(
                    $"[IEoR]: Expected to patch AnahitaReturnLifeRatio once, but patched {patched} times."
                );
            }
        }
    }
}
