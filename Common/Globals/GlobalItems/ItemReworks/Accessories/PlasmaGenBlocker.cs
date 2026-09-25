using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.SupremeCalamitas;
using System.Collections.Generic;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Accessories
{
    [ExtendsFromMod("ThoriumMod")]
    public class PlasmaGenBlocker : GlobalItem
    {
        private static readonly HashSet<int> BlockedBosses = [ModContent.NPCType<Apollo>(), ModContent.NPCType<Artemis>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<CalamitasClone>(), ModContent.NPCType<Cataclysm>(), ModContent.NPCType<Catastrophe>(),
                                                              ModContent.NPCType<SupremeCalamitas>(), ModContent.NPCType<SupremeCataclysm>(), ModContent.NPCType<SupremeCatastrophe>(), ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianCommander>()];

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            foreach (int boss in BlockedBosses)
            {
                if (NPC.AnyNPCs(boss))
                {
                    player.GetThoriumPlayer().accPlasmaGenerator = false;
                }
            }

            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
            {
                if (NPC.AnyNPCs(catalyst.Find<ModNPC>("Astrageldon").Type))
                {
                    player.GetThoriumPlayer().accPlasmaGenerator = false;
                }
            }

            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                if (NPC.AnyNPCs(calHunt.Find<ModNPC>("Goozma").Type))
                {
                    player.GetThoriumPlayer().accPlasmaGenerator = false;
                }
            }

            if (ModLoader.TryGetMod("NoxusBoss", out Mod wotg))
            {
                if (NPC.AnyNPCs(wotg.Find<ModNPC>("NamelessDeityBoss").Type) || NPC.AnyNPCs(wotg.Find<ModNPC>("AvatarOfEmptiness").Type))
                {
                    player.GetThoriumPlayer().accPlasmaGenerator = false;
                }
            }
        }
    }
}
