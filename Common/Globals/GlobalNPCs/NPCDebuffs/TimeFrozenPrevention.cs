using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Players.SOTSPlayerOverrides;
using InfernalEclipseAPI.Core.Systems;
using SOTS;
using static InfernalEclipseAPI.Core.Systems.InfernalCrossmod;

namespace InfernalEclipseAPI.Common.Globals.GlobalNPCs.NPCDebuffs
{
    [ExtendsFromMod("SOTS")]
    [JITWhenModsEnabled("SOTS")]
    public class TimeFrozenPrevention : GlobalNPC
    {
        private static readonly HashSet<int> HolyFlameBosses = [ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.NPCType<Providence>()];

        public override bool PreAI(NPC npc)
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges)
                return base.PreAI(npc);

            if (Catalyst.Loaded)
            {
                if (npc.type == Catalyst.Mod.Find<ModNPC>("Astrageldon").Type)
                    PreventTimeFreezeEffects();
            }

            if (HolyFlameBosses.Contains(npc.type))
            {
                PreventTimeFreezeEffects("HolyFlame");
            }

            if (InfernalCrossmod.YouBoss.Loaded)
            {
                if (npc.type == InfernalCrossmod.YouBoss.Mod.Find<ModNPC>("TerraBladeBoss").Type)
                    PreventTimeFreezeEffects("TerraBlade");
            }

            if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
            {
                PreventTimeFreezeEffects();
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (npc.ModNPC?.Name is string name && (name.Contains("SlagFury") || name.Contains("Aquaius") || name.Contains("Omnicide") || name.Contains("DreamEater")))
                    PreventTimeFreezeEffects();
            }

            if (npc.type == ModContent.NPCType<SupremeCalamitas>())
            {
                PreventTimeFreezeEffects("SCal");
            }

            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                if (npc.type == calHunt.Find<ModNPC>("Goozma").Type)
                    PreventTimeFreezeEffects("AuricSoul");
            }
            if (InfernalCrossmod.NoxusBoss.Loaded)
            {
                Mod wotg = InfernalCrossmod.NoxusBoss.Mod;

                if (npc.type == wotg.Find<ModNPC>("AvatarRift").Type || npc.type == wotg.Find<ModNPC>("AvatarOfEmptiness").Type || npc.type == wotg.Find<ModNPC>("NamelessDeityBoss").Type)
                    PreventTimeFreezeEffects("AuricSoul");
            }

            if (npc.type == ModContent.NPCType<PrimordialWyrmHead>() || BossRushEvent.BossRushActive)
            {
                PreventTimeFreezeEffects("Terminus");
            }

            return base.PreAI(npc);
        }

        private static void PreventTimeFreezeEffects(string bossMessage = "Other")
        {
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && player.SOTSPlayer().VMincubator)
                {
                    player.GetModPlayer<InfernalPlayer>().voidMagePrevention = 60;
                    player.GetModPlayer<SOTSPlayerAdjustments>().bossMessage = bossMessage;
                }

                SOTSWorld.GlobalTimeFreeze = 0;
                SOTSWorld.GlobalFrozen = false;
                SOTSWorld.GlobalFreezeCounter = 0.0f;
                SOTSWorld.IsFrozenThisFrame = false;
            }
        }
    }
}
