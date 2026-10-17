using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Providence;
using Terraria.GameContent.ItemDropRules;

namespace InfernalEclipseAPI.Common.ProgressionRework
{
    public class ProvidenceRuneOfKosFix : GlobalNPC
    {
        public override bool InstancePerEntity => false;
        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            //Ensures Providence drops the Rune of Kos
            if (npc.type == ModContent.NPCType<Providence>())
            {
                bool hasRuneDrop = false;
                foreach (var rule in npcLoot.Get(false))
                {
                    if (rule is CommonDrop commonDrop && commonDrop.itemId == ModContent.ItemType<MarkofProvidence>())
                    {
                        hasRuneDrop = true;
                        break;
                    }
                }

                if (!hasRuneDrop)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MarkofProvidence>(), 1));
                }
            }
        }
    }
}
