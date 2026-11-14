using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.GameContent.ItemDropRules;
using InfernalEclipseAPI.Core.Configs;

namespace InfernalEclipseAPI.Common.Balance.Vanilla
{
    public class DemonScytheFireFlowerSwap : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.VanillaBalanceChanges;
        }

        public override void Load()
        {
            WorldGen.DetourPass((PassLegacy)WorldGen.VanillaGenPasses["Reset"], ReplaceFlowerDetour);
        }

        void ReplaceFlowerDetour(WorldGen.orig_GenPassDetour orig, object self, GenerationProgress progress, GameConfiguration configuration)
        {
            orig(self, progress, configuration);
            for (int i = 0; i < GenVars.hellChestItem.Length; i++)
            {
                if (GenVars.hellChestItem[i] == ItemID.FlowerofFire)
                {
                    GenVars.hellChestItem[i] = ItemID.DemonScythe;
                }
                if (GenVars.hellChestItem[i] == ItemID.UnholyTrident)
                {
                    GenVars.hellChestItem[i] = ItemID.DemonScythe;
                }
            }
        }
    }

    public class DemonFireFlower : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.VanillaBalanceChanges;
        }

        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            int demonDropChanceDenom = 100;
            int demonDropChanceMin = 1;
            int demonDropChanceMax = 1;
            int demonDropChanceNume = 3;
            foreach (IItemDropRule rule in npcLoot.Get())
            {
                if (rule is CommonDrop drop && drop.itemId == ItemID.DemonScythe)
                {
                    demonDropChanceDenom = drop.chanceDenominator;
                    demonDropChanceMin = drop.amountDroppedMinimum;
                    demonDropChanceMax = drop.amountDroppedMaximum;
                    demonDropChanceNume = drop.chanceNumerator;
                    npcLoot.Remove(drop);
                }
            }
            if (npc.type == NPCID.FireImp)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new Terraria.GameContent.ItemDropRules.Conditions.NotRemixSeed(), ItemID.FlowerofFire, demonDropChanceDenom, demonDropChanceMin, demonDropChanceMax, demonDropChanceNume));
            }
        }
    }
    public class FireFlowerAdjustments : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalConfig.Instance.VanillaBalanceChanges;
        }

        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.UnholyTrident && Main.remixWorld)
            {
                entity.damage = 34;
            }

            if (entity.type == ItemID.FlowerofFire && !Main.remixWorld)
            {
                entity.damage = 38;
            }
        }
    }
}
