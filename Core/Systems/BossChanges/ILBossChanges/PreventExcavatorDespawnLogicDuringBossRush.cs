using System.Reflection;
using MonoMod.RuntimeDetour;
using Microsoft.Xna.Framework;
using SOTS;
using CalamityMod.Events;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Systems.BossChanges.ILBossChanges
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class PreventExcavatorDespawnLogicDuringBossRush : ModSystem
    {
        private static Hook _hook;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("SecretsOfTheSouls");
        }

        public override void Load()
        {
            var excType = Type.GetType("SOTS.NPCs.Boss.Excavator.Excavator, SOTS", throwOnError: false);
            if (excType == null) return;

            var mi = excType.GetMethod("DespawnCheck",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi == null) return;

            _hook = new Hook(mi, PatchedDespawnCheck);
        }

        public override void Unload()
        {
            _hook?.Dispose();
            _hook = null;
        }

        private static bool PatchedDespawnCheck(object self)
        {
            var modNpc = (ModNPC)self;
            NPC npc = modNpc.NPC;

            var despawnCounterField = self.GetType().GetField("DespawnCounter",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (despawnCounterField == null)
                return false; // fail-safe

            if (BossRushEvent.BossRushActive)
                return false;

            int despawnCounter = (int)despawnCounterField.GetValue(self);

            Player target = Main.player[npc.target];

            bool tooFar = Vector2.Distance(target.Center, npc.Center) > 6400.0;
            if (target.dead || tooFar || !target.SOTSPlayer().AbandonedVillageBiome)
                despawnCounter++;
            else if (despawnCounter > 0)
                despawnCounter--;

            if (despawnCounter >= 600)
            {
                npc.active = false;
                despawnCounterField.SetValue(self, despawnCounter);
                return true;
            }

            npc.DiscourageDespawn(1000);
            despawnCounterField.SetValue(self, despawnCounter);
            return false;
        }
    }
}
