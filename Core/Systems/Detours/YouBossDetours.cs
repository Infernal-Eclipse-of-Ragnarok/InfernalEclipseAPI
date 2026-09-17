using CalamityMod.Events;
using MonoMod.RuntimeDetour;
using System.Reflection;
using YouBoss.Common.Utilities;
using YouBoss.Content.NPCs.Bosses.TerraBlade;
using YouBoss.Content.NPCs.Bosses.TerraBlade.SpecificEffectManagers;

namespace InfernalEclipseAPI.Core.Systems.Detours
{
    public class FirstFractaliFramesNerf : ModSystem
    {
        private Hook hook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("YouBoss", out Mod youBoss))
                return;

            Type firstFractalType = youBoss.Code.GetType("YouBoss.Content.Items.ItemReworks.FirstFractal");
            MethodInfo getter = firstFractalType?.GetProperty("PlayerPostHitIFrameGracePeriod", BindingFlags.Public | BindingFlags.Static)?.GetGetMethod();

            if (getter is not null)
                hook = new Hook(getter, OverridePlayerPostHitIFrames);
        }

        public override void Unload()
        {
            hook?.Dispose();
            hook = null;
        }

        private static int OverridePlayerPostHitIFrames(Func<int> orig)
        {
            return 12;
        }
    }

    [JITWhenModsEnabled("YouBoss")]
    [ExtendsFromMod("YouBoss")]
    public class YouBossModSceneFix : ModSystem
    {
        private static Hook _isSceneEffectActiveHook;

        public override void Load()
        {
            MethodInfo targetMethod = typeof(TerraBladeSkyScene).GetMethod(nameof(TerraBladeSkyScene.IsSceneEffectActive), BindingFlags.Instance | BindingFlags.Public);
            _isSceneEffectActiveHook = new Hook(targetMethod, IsSceneEffectActive_Detour);
        }

        public override void Unload()
        {
            _isSceneEffectActiveHook?.Dispose();
            _isSceneEffectActiveHook = null;
        }

        // Completely overrides original logic
        private static bool IsSceneEffectActive_Detour(Func<TerraBladeSkyScene, Player, bool> orig, TerraBladeSkyScene self, Player player)
        {
            bool bossActive = false;
            if (TerraBladeBoss.Myself != null)
            {
                var tbBoss = TerraBladeBoss.Myself.As<TerraBladeBoss>();
                if (tbBoss != null && !tbBoss.PerformingStartAnimation)
                    bossActive = true;
            }
            return bossActive && !BossRushEvent.BossRushActive;
        }
    }
}
