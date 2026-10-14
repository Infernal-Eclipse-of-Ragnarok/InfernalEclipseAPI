using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace InfernalEclipseAPI.Core.HallowedBarFix
{
    public class BossDropFix : GlobalNPC
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
}
