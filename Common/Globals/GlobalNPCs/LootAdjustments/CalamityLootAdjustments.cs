using Terraria.GameContent.ItemDropRules;
using RagnarokMod.Items.HealerItems.Other;
using CalamityMod.NPCs.AcidRain;
using CalamityMod;
using InfernalEclipseAPI.Content.Items.Placeables.Relics;
using InfernumMode.Core.GlobalInstances.Systems;
using InfernalEclipseAPI.Core.Systems;
using CalamityMod.NPCs.Abyss;
using CalamityMod.Items.Weapons.Melee;

namespace InfernalEclipseAPI.Common.GlobalNPCs.LootAdjustments
{
    public class CalamityLootAdjustments : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (InfernalCrossmod.RagnarokMod.Loaded)
            {
                foreach (IItemDropRule rule in npcLoot.Get())
                {
                    if (rule is CommonDrop drop && drop.itemId == InfernalCrossmod.RagnarokMod.Mod.Find<ModItem>("Virusprayer").Type && InfernalCrossmod.InfernalEclipseWeaponsDLC.Loaded)
                    {
                        npcLoot.Remove(drop);
                    }
                }
            }

            static bool isInfernum() => WorldSaveSystem.InfernumModeEnabled;

            if (npc.type == ModContent.NPCType<Mauler>())
            {
                if (InfernalCrossmod.RagnarokMod.Loaded && InfernalCrossmod.InfernalEclipseWeaponsDLC.Loaded)
                    npcLoot.Add(ItemDropRule.Common(InfernalCrossmod.RagnarokMod.Mod.Find<ModItem>("Virusprayer").Type, 2, 1, 1));

                npcLoot.AddIf(isInfernum, ModContent.ItemType<MaulerRelic>());
            }

            if (npc.type == ModContent.NPCType<CragmawMire>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<CragmawMireRelic>());
            }

            if (npc.type == ModContent.NPCType<Viperfish>())
            {
                foreach (IItemDropRule rule in npcLoot.Get())
                {
                    if (rule is CommonDrop drop && drop.itemId == ModContent.ItemType<DepthCrusher>())
                    {
                        int dcDropChanceDenom = drop.chanceDenominator;
                        int dcDropChanceMin = drop.itemId;
                        int dcDropChanceMax = drop.amountDroppedMaximum;
                        int dcDropChanceNume = drop.chanceNumerator;

                        npcLoot.Remove(rule);

                        npcLoot.Add(ItemDropRule.ByCondition(new InfernalGlobalNPC.EvilBossDownedCondition(), ModContent.ItemType<DepthCrusher>(), dcDropChanceDenom, dcDropChanceMin, dcDropChanceMax, dcDropChanceNume));
                    }
                }
            }
        }
    }

    [JITWhenModsEnabled("RagnarokMod")]
    [ExtendsFromMod("RagnarokMod")]
    public class CalamityLootBagAdjustments : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            foreach (IItemDropRule rule in itemLoot.Get())
            {
                if (rule is CommonDrop drop && drop.itemId == ModContent.ItemType<Virusprayer>() && InfernalCrossmod.InfernalEclipseWeaponsDLC.Loaded)
                {
                    itemLoot.Remove(drop);
                }
            }
        }
    }
}
