using CalamityMod;
using InfernalEclipseAPI.Content.Items.Lore.Other;
using InfernalEclipseAPI.Content.Items.Placeables.Relics;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.CalamityAddons;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.Platinum;
using InfernalEclipseAPI.Content.Items.Placeables.Trophies;
using Terraria.GameContent.ItemDropRules;
using YouBoss.Content.NPCs.Bosses.TerraBlade;
using YouBoss.Core;
using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;

namespace InfernalEclipseAPI.Common.GlobalNPCs.LootAdjustments
{
    [ExtendsFromMod("YouBoss")]
    public class YouNPCLootAdjustments : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<TerraBladeBoss>())
            {
                bool firstTerraBladeKill() => !WorldSaveSystem.HasDefeatedYourself;
                bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;
                npcLoot.AddConditionalPerPlayer(firstTerraBladeKill, ModContent.ItemType<LoreMirror>(), desc: DropHelper.FirstKillText);
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerraBladeTrophy>(), 10, 1, 1));
                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TerraBladeRelic>()));
                npcLoot.Add(ItemDropRule.ByCondition(new RevengenceMode(), ModContent.ItemType<TerraBladeRelic>(), 1, 1, 1, 1));
                npcLoot.AddIf(isInfernum, ModContent.ItemType<InfernalTerraBladeRelic>());
                npcLoot.AddIf(() => !DownedBossSystem.downedDoG, ModContent.ItemType<TerraBladeRelicPlatinum>());
            }
        }
    }
}
