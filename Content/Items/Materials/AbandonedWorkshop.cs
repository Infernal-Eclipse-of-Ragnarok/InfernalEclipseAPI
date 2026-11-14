using CalamityMod;
using InfernalEclipseAPI.Core.World;
using Terraria.GameContent.ItemDropRules;

namespace InfernalEclipseAPI.Content.Items.Materials
{
    public class AbandonedWorkshop : ModItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
        }
    }

    public class AbandonedWorkshopDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            int[] cthulhuBosses =
            {
            NPCID.EyeofCthulhu,
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,
            NPCID.BrainofCthulhu,
            NPCID.SkeletronHead,
            NPCID.WallofFlesh
            };

            foreach (int bossID in cthulhuBosses)
            {
                if (npc.type == bossID)
                {
                    if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
                    {
                        LeadingConditionRule EoWKill = new(DropHelper.If((info) => info.npc.boss && !InfernalWorld.craftedWorkshop));
                        EoWKill.Add(ModContent.ItemType<AbandonedWorkshop>());
                        npcLoot.Add(EoWKill);
                    }
                    else
                    {
                        LeadingConditionRule craftedWorkshop = new(DropHelper.If((info) => !InfernalWorld.craftedWorkshop));
                        craftedWorkshop.Add(ItemDropRule.Common(ModContent.ItemType<AbandonedWorkshop>()));
                        npcLoot.Add(craftedWorkshop);
                    }
                }
            }
        }
    }
}
