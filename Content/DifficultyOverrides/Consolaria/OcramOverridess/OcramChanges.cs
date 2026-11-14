using CalamityMod.Events;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumActive = InfernalEclipseAPI.Content.DifficultyOverrides.hellActive;
namespace InfernalEclipseAPI.Content.DifficultyOverrides.Consolaria.OcramOverridess
{
    public class OcramBehavior : GlobalNPC
    {
        private static bool bloodmoonStartedByOcram = false;
        private static bool bloodmoonStartedByDreadnaut = false;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            if (InfernalCrossmod.Consolaria.Loaded) 
            {
                if (entity.type == InfernalCrossmod.Consolaria.Mod.Find<ModNPC>("Ocram").Type)
                    return true;
            }

            return entity.type == NPCID.BloodNautilus;
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.BloodNautilus)
            {
                if (InfernumActive.InfernumActive && !Main.bloodMoon)
                {
                    Main.bloodMoon = true;
                    bloodmoonStartedByDreadnaut = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData); // sync the blood moon
                }

                if (Main.bloodMoon && bloodmoonStartedByDreadnaut)
                {
                    bool aPlayerIsAliveAndInRange = false;
                    foreach (Player player in Main.player)
                    {
                        if (aPlayerIsAliveAndInRange)
                            continue;

                        if (player.active && !player.dead && npc.Distance(player.Center) < 10000f)
                        {
                            aPlayerIsAliveAndInRange = true;
                        }
                    }

                    if (!aPlayerIsAliveAndInRange)
                    {
                        DisableBloodMoon();
                    }
                }
            }

            return base.PreAI(npc);
        }

        public override void AI(NPC npc)
        {
            if (!InfernumActive.InfernumActive)
                { return; }

            if (InfernalCrossmod.Consolaria.Loaded) 
            {
                if (npc.type == InfernalCrossmod.Consolaria.Mod.Find<ModNPC>("Ocram").Type)
                {
                    if (!Main.bloodMoon && BossRushEvent.BossRushActive)
                    {
                        Main.bloodMoon = true;
                        bloodmoonStartedByOcram = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData); // sync the blood moon
                    }
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            if (!InfernumActive.InfernumActive)
                return;

            if (npc.type == NPCID.BloodNautilus && bloodmoonStartedByDreadnaut)
            {
                DisableBloodMoon();
            }

            if (InfernalCrossmod.Consolaria.Loaded && bloodmoonStartedByOcram)
            {
                if (npc.type == InfernalCrossmod.Consolaria.Mod.Find<ModNPC>("Ocram").Type)
                    DisableBloodMoon();
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (InfernumActive.InfernumActive) 
            {
                if (npc.type == NPCID.BloodNautilus && bloodmoonStartedByDreadnaut)
                {
                    DisableBloodMoon();
                }

                if (InfernalCrossmod.Consolaria.Loaded && bloodmoonStartedByOcram)
                {
                    if (npc.type == InfernalCrossmod.Consolaria.Mod.Find<ModNPC>("Ocram").Type)
                        DisableBloodMoon();
                }
            }

            return base.CheckDead(npc);
        }

        private static void DisableBloodMoon()
        {
            if (Main.bloodMoon)
            {
                Main.bloodMoon = false;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData); // Resync after ending
            }
        }

        public override bool InstancePerEntity => true;
    }
}
