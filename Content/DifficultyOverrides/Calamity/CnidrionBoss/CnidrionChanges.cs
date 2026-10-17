using CalamityMod.NPCs.NormalNPCs;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.World;
using Terraria.Audio;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.CnidrionBoss
{
    public class CnidrionChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<Cnidrion>();

        public override bool PreAI(NPC npc)
        {
            InfernalGlobalNPC.cnidrion = npc.whoAmI;

            return base.PreAI(npc);
        }

        public override void OnKill(NPC npc)
        {
            InfernalWorld.cnidrionDowned = true;
        }
    }
}
