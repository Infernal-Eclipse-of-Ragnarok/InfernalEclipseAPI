using CalamityMod.NPCs.SupremeCalamitas;
using InfernalEclipseAPI.Core.World;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SCalOverrides
{
    public class SCalBrothersChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<SupremeCataclysm>() || entity.type == ModContent.NPCType<SupremeCatastrophe>();

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            if (InfernalWorld.RagnarokModeEnabled)
            {
                if (npc.type == ModContent.NPCType<SupremeCataclysm>())
                {
                    npc.lifeMax += (int)(npc.lifeMax * 0.65f);
                }
            }
        }
    }
}
