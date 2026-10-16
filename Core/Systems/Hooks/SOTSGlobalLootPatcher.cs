using System.Reflection;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using MonoMod.RuntimeDetour;
using Terraria.GameContent.ItemDropRules;

namespace InfernalEclipseAPI.Core.Systems.Hooks
{
    public sealed class SOTSGlobalLootPatcher : ModSystem
    {
        private static Hook canDropHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out Mod sots))
                return;

            Type condType = sots.Code.GetType("SOTS.Common.ItemDropConditions.DontDropOnFriendlyCondition");
            if (condType == null)
                return;

            MethodInfo canDrop = condType.GetMethod(
                "CanDrop",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (canDrop == null)
                return;

            canDropHook = new Hook(canDrop, Hooked_CanDrop);
        }

        public override void Unload()
        {
            canDropHook?.Dispose();
            canDropHook = null;
        }

        private delegate bool Orig_CanDrop(object self, DropAttemptInfo info);

        private static bool Hooked_CanDrop(Orig_CanDrop orig, object self, DropAttemptInfo info)
        {
            // First run original logic (friendly check)
            if (!orig(self, info))
                return false;

            NPC npc = info.npc;
            if (npc == null)
                return false;

            if (npc.friendly || npc.townNPC)
                return false;

            if (npc.catchItem > 0)
                return false;

            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (npc.type == InfernalCrossmod.Thorium.Mod.Find<ModNPC>("BiteyBaby").Type || npc.type == InfernalCrossmod.Thorium.Mod.Find<ModNPC>("AbyssalSpawn").Type)
                    return false;
            }

            return true;
        }
    }
}
