using SOTS;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.ProgressionRework
{
    public class HallowedBarFix : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            if (!IsMechBoss(npc.type))
                return;

            // 1) Always remove vanilla Hallowed Bar drops from mech NPCs
            npcLoot.RemoveWhere(IsHallowedBarDrop);

            // 2) Re-add bars with a runtime condition (normal mode only; expert uses bags)
            var condRule = new LeadingConditionRule(new AllMechsDown_NotExpert_Cond());
            // adjust stack ranges if you want exact vanilla values
            condRule.OnSuccess(ItemDropRule.Common(ItemID.HallowedBar, chanceDenominator: 1, minimumDropped: 15, maximumDropped: 30));
            npcLoot.Add(condRule);
        }

        private static bool IsMechBoss(int npcType) =>
            npcType == NPCID.SkeletronPrime ||
            npcType == NPCID.Retinazer ||
            npcType == NPCID.Spazmatism ||
            npcType == NPCID.TheDestroyer;

        private static bool IsHallowedBarDrop(IItemDropRule rule)
        {
            if (rule is CommonDrop cd) return cd.itemId == ItemID.HallowedBar;
            if (rule is DropBasedOnExpertMode ex)
                return (ex.ruleForNormalMode is CommonDrop n && n.itemId == ItemID.HallowedBar)
                    || (ex.ruleForExpertMode is CommonDrop e && e.itemId == ItemID.HallowedBar);
            return false;
        }
    }

    // Condition that’s evaluated at runtime
    public sealed class AllMechsDown_Cond : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            bool flagV = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            bool flagS = true;
            if (ModLoader.TryGetMod("SOTS", out Mod _))
            {
                flagS = PolarisDownedCheck.isPolarisDowned();
            }
            return flagV && flagS;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() =>
            Language.GetTextValue("Mods.InfernalEclipseAPI.Conditions.AllMechsDown");
    }

    // Same as above but prevents NPC (non-bag) drops in Expert/Master (bags handle those)
    public sealed class AllMechsDown_NotExpert_Cond : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            bool flagV = !info.IsExpertMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            bool flagS = true;
            if (ModLoader.TryGetMod("SOTS", out Mod _))
            {
                flagS = PolarisDownedCheck.isPolarisDowned();
            }
            return flagV && flagS;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() =>
            Language.GetTextValue("Mods.InfernalEclipseAPI.Conditions.AllMechsDown");
    }

    public class HallowedBossBagFix : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (!IsMechBossBag(item.type))
                return;

            // 1) Always remove vanilla bar drops from bags
            itemLoot.RemoveWhere(IsHallowedBarDrop);

            // 2) Re-add bars gated by "all mechs down" (bags are Expert/Master anyway)
            var condRule = new LeadingConditionRule(new AllMechsDown_Cond());

            // Use Expert vs Normal counts if someone spawns bags in normal; otherwise Expert path applies in real worlds
            var normalBars = ItemDropRule.Common(ItemID.HallowedBar, 1, 15, 30);
            var expertBars = ItemDropRule.Common(ItemID.HallowedBar, 1, 20, 35);
            condRule.OnSuccess(new DropBasedOnExpertMode(expertBars, normalBars));

            itemLoot.Add(condRule);
        }

        private static bool IsMechBossBag(int type) =>
            type == ItemID.TwinsBossBag ||
            type == ItemID.SkeletronPrimeBossBag ||
            type == ItemID.DestroyerBossBag;

        private static bool IsHallowedBarDrop(IItemDropRule rule)
        {
            if (rule is CommonDrop cd) return cd.itemId == ItemID.HallowedBar;
            if (rule is DropBasedOnExpertMode ex)
                return (ex.ruleForNormalMode is CommonDrop n && n.itemId == ItemID.HallowedBar)
                    || (ex.ruleForExpertMode is CommonDrop e && e.itemId == ItemID.HallowedBar);
            return false;
        }
    }

    [ExtendsFromMod("SOTS")]
    public class PolarisDownedCheck
    {
        public static bool isPolarisDowned()
        {
            return SOTSWorld.downedAmalgamation;
        }
    }
}
