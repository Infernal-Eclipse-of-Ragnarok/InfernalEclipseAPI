using CalamityMod.Items.Mounts;
using CatalystMod.NPCs.Boss.Astrageldon;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems.Compatibility;


namespace InfernalEclipseAPI.Common.GlobalNPCs.NPCDebuffs
{
    [ExtendsFromMod("CatalystMod")]
    public class AstrageldonDebuff : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (!InfernalConfig.Instance.CalamityBalanceChanges || !npc.active || npc.type != ModContent.NPCType<Astrageldon>()) return base.PreAI(npc);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    if (player.mount?.Type == ModContent.MountType<DraedonGamerChairMount>())
                        player.mount.Dismount(player);
                }
            }
            if (InfernalCrossmod.Clamity.Loaded)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead)
                    {
                        if (player.mount?.Type == InfernalCrossmod.Clamity.Mod.Find<ModMount>("PlagueChairMount").Type)
                            player.mount.Dismount(player);
                    }
                }
            }

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (!npc.active || npc.type != ModContent.NPCType<Astrageldon>())
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && npc.WithinRange(player.Center, 8000f) && player.GetModPlayer<InfernalPlayer>().teleportRespawnKilldown <= 0)
                {
                    player.AddBuff(ModContent.BuffType<StarboundHorrification>(), 60);
                }
            }
        }
    }
}
