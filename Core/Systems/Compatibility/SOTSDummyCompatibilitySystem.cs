using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseAPI.Core.Systems.Compatibility
{
    // Keeps ImproveGame dummies usable while SOTS's Advisor is dormant.
    public sealed class SOTSDummyCompatibilitySystem : ModSystem
    {
        private int advisorType = -1;
        private int dummyType = -1;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod("SOTS") && ModLoader.HasMod("ImproveGame");
        }

        public override void PostSetupContent()
        {
            if (!ModContent.TryFind<ModNPC>("SOTS/TheAdvisorHead", out var advisor) ||
                !ModContent.TryFind<ModNPC>("ImproveGame/DummyNPC", out var dummy))
            {
                Mod.Logger.Warn("SOTS dummy compatibility disabled: expected NPC content was not found.");
                return;
            }

            advisorType = advisor.Type;
            dummyType = dummy.Type;
        }

        public override void PostUpdateNPCs()
        {
            if (Main.gameMenu || advisorType < 0 || dummyType < 0)
                return;

            bool hasDormantAdvisor = false;
            bool hasDummy = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;

                if (npc.type == dummyType)
                    hasDummy = true;

                // Preserve ImproveGame's existing Celestial Pillar exceptions.
                if (npc.type is NPCID.LunarTowerNebula or NPCID.LunarTowerSolar or
                    NPCID.LunarTowerStardust or NPCID.LunarTowerVortex)
                    continue;

                // SOTS sets boss=true when this encounter actually starts.
                if (npc.type == advisorType && !npc.boss)
                {
                    hasDormantAdvisor = true;
                    continue;
                }

                // Retain all other upstream boss protections, including EoW.
                if (npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type])
                    return;
            }

            if (!hasDormantAdvisor || !hasDummy)
                return;

            // Run on both clients and servers, after DummyNPC.AI has written these flags.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == dummyType)
                {
                    npc.friendly = false;
                    npc.dontTakeDamage = false;
                }
            }
        }

        public override void Unload()
        {
            advisorType = -1;
            dummyType = -1;
        }
    }
}
