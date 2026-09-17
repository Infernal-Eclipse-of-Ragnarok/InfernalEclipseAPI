using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs.SupremeCalamitas;
using InfernumMode;
using MonoMod.RuntimeDetour;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using InfernumMode.Content.Credits;
using Terraria.ModLoader.IO;

namespace InfernalEclipseAPI.Core.Systems.MultiplayerFixes.SupCal
{
    public class SupremeCalamitasFixes : GlobalNPC
    {
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {

        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.type == ModContent.NPCType<SupremeCalamitas>())
            {
                base.ReceiveExtraAI(npc, bitReader, binaryReader);

                float attackType = npc.ai[0];
                float attackState = npc.Infernum().ExtraAI[4];

                if (Main.netMode == NetmodeID.MultiplayerClient && !npc.active && attackType == 13 && attackState == 4f)
                {
                    npc.NPCLoot();
                    if (DownedBossSystem.downedExoMechs)
                    {
                        CreditManager.BeginCredits();
                    }
                }
            }
        }
    }
}
