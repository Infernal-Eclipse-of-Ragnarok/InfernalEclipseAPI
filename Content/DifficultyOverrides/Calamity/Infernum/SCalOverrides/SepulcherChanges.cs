using CalamityMod.NPCs.SupremeCalamitas;
using InfernalEclipseAPI.Core.World;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SCalOverrides
{
    public class SepulcherChanges : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<SepulcherHead>() || entity.type == ModContent.NPCType<SepulcherBody>() || entity.type == ModContent.NPCType<SepulcherTail>()
                                                                                 || entity.type == ModContent.NPCType<SepulcherArm>() || entity.type == ModContent.NPCType<SepulcherBodyEnergyBall>();

        const int duration = 30 * 60;
        public override bool PreAI(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return base.PreAI(npc);

            if (npc.type == ModContent.NPCType<SepulcherHead>())
            {
                npc.HitSound = SoundID.DD2_SkeletonHurt with { Volume = 0.925f };

                if (npc.dontTakeDamage == false)
                {
                    if (npc.life - (npc.lifeMax / duration) <= 0)
                    {
                        npc.immortal = false;
                        npc.takenDamageMultiplier = 1f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            npc.StrikeInstantKill();
                    }
                    else
                    {
                        npc.life -= (npc.lifeMax / duration);
                    }
                }
            }
            else
                npc.HitSound = null;

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled)
                return;

            npc.immortal = true;
            npc.takenDamageMultiplier = 0f;
        }
    }
}
