using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;
using CalamityMod;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.Consolaria;
using InfernalEclipseAPI.Content.Items.Lore.Consolaria;

namespace InfernalEclipseAPI.Common.GlobalNPCs.InfernalRelics
{
    [JITWhenModsEnabled("Consolaria")]
    [ExtendsFromMod("Consolaria")]
    public class ConsolariaInfernalRelics : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            if (ModLoader.TryGetMod("Consolaria", out Mod console))
            {
                bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;
                if (npc.type == console.Find<ModNPC>("Lepus").Type)
                {
                    bool firstLepusKill() => Consolaria.Common.ModSystems.DownedBossSystem.downedLepus;
                    npcLoot.AddConditionalPerPlayer(firstLepusKill, ModContent.ItemType<LoreLepus>(), desc: DropHelper.FirstKillText);

                    npcLoot.AddIf(isInfernum, ModContent.ItemType<LepusRelic>());
                }
                if (npc.type == console.Find<ModNPC>("TurkortheUngrateful").Type)
                {
                    bool firstTurkorKill() => Consolaria.Common.ModSystems.DownedBossSystem.downedTurkor;
                    npcLoot.AddConditionalPerPlayer(firstTurkorKill, ModContent.ItemType<LoreTurkor>(), desc: DropHelper.FirstKillText);

                    npcLoot.AddIf(isInfernum, ModContent.ItemType<TurkorTheUngratefulRelic>());
                }
                if (npc.type == console.Find<ModNPC>("Ocram").Type)
                {
                    bool firstOcramKill() => Consolaria.Common.ModSystems.DownedBossSystem.downedOcram;
                    npcLoot.AddConditionalPerPlayer(firstOcramKill, ModContent.ItemType<LoreOcram>(), desc: DropHelper.FirstKillText);

                    npcLoot.AddIf(isInfernum, ModContent.ItemType<OcramRelic>());
                }
            }
        }
    }
}
