using InfernalEclipseAPI.Content.Items.Placeables.Relics;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using CalamityMod;
using InfernumMode.Core.GlobalInstances.Systems;

namespace InfernalEclipseAPI.Common.GlobalNPCs.LootAdjustments
{
    public class VanillaLootAdjustments : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.BloodNautilus)
            {
                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<DreadnautilusRelic>()));
                npcLoot.Add(ItemDropRule.ByCondition(new RevengenceMode(), ModContent.ItemType<DreadnautilusRelic>(), 1, 1, 1, 1));
            }

            if (ModLoader.TryGetMod("HypnosMod", out Mod hypnos))
            {
                if (npc.type == hypnos.Find<ModNPC>("HypnosBoss").Type)
                {
                    npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<HypnosRelic>()));
                    npcLoot.Add(ItemDropRule.ByCondition(new RevengenceMode(), ModContent.ItemType<HypnosRelic>(), 1, 1, 1, 1));
                    npcLoot.AddIf(() => WorldSaveSystem.InfernumModeEnabled, ModContent.ItemType<InfernalHypnosRelic>());
                }
            }

            if (IsAncientCobaltEnemy(npc.type))
            {
                npcLoot.RemoveWhere(rule =>
                    rule is CommonDrop cd &&
                    (
                        cd.itemId == ItemID.AncientCobaltHelmet ||
                        cd.itemId == ItemID.AncientCobaltBreastplate ||
                        cd.itemId == ItemID.AncientCobaltLeggings
                    )
                );

                // Gate drops behind Eye of Cthulhu
                LeadingConditionRule eyeGate = new LeadingConditionRule(new DownedEyeOfCthulhu());

                // 0.33% chance ≈ 1 / 300
                eyeGate.OnSuccess(ItemDropRule.OneFromOptions(
                    300,
                    ItemID.AncientCobaltHelmet,
                    ItemID.AncientCobaltBreastplate,
                    ItemID.AncientCobaltLeggings
                ));

                npcLoot.Add(eyeGate);
            }
        }

        private static bool IsAncientCobaltEnemy(int npcType)
        {
            return npcType == NPCID.ManEater ||
                   npcType == NPCID.Hornet ||
                   npcType == NPCID.HornetFatty ||
                   npcType == NPCID.HornetHoney ||
                   npcType == NPCID.HornetLeafy ||
                   npcType == NPCID.HornetSpikey ||
                   npcType == NPCID.HornetStingy;
        }
    }

    public class RevengenceMode : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        private static LocalizedText Description;

        public RevengenceMode()
        {
            if (Description != null)
                return;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (Main.GameModeInfo.IsMasterMode)
                return false;

            if (!ModLoader.HasMod("CalamityMod"))
                return false;

            Mod calamity = ModLoader.GetMod("CalamityMod");

            bool active =
                (bool)calamity.Call("GetDifficultyActive", "revengeance") ||
                (bool)calamity.Call("GetDifficultyActive", "death");

            if (!active &&
                ModLoader.TryGetMod("FargowiltasSouls", out Mod fargo))
            {
                active =
                    (bool)fargo.Call("EternityMode") ||
                    (bool)fargo.Call("MasochistMode");
            }

            if (!active &&
                ModLoader.TryGetMod("InfernumMode", out Mod infernum))
            {
                active = (bool)infernum.Call("GetInfernumActive");
            }

            return active;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Description?.Value ?? "";
    }

    public class DownedEyeOfCthulhu : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss1;

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => "Drops after the Eye of Cthulhu has been defeated";
    }
}
