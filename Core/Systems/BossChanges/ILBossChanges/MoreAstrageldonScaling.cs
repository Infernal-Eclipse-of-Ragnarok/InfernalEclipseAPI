using CatalystMod.NPCs.Boss.Astrageldon;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using CalamityMod.Events;
using CatalystMod.Tiles.Ores;
using CatalystMod;
using InfernalEclipseAPI.Core.Systems.Hooks.ILTileChanges;
using InfernumMode.Core.GlobalInstances.Systems;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Systems.BossChanges.ILBossChanges
{
    [ExtendsFromMod("CatalystMod")]
    public class AstrageldonScalerSystem : ModSystem
    {
        private ILHook astrageldonHook;

        public override void Load()
        {
            if (ModLoader.TryGetMod("CnI", out _))
                return;

            try
            {
                MethodInfo method = typeof(Astrageldon).GetMethod(
                    "SecondPhaseHealthIncrease",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                if (method != null)
                {
                    astrageldonHook = new ILHook(method, ApplyAstrageldonScaling);
                    Mod.Logger.Info("Astrageldon HP scaling hook applied.");
                }
                else
                {
                    Mod.Logger.Warn("Astrageldon method not found.");
                }
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Failed to hook Astrageldon scaling: " + ex);
            }
        }

        public override void Unload()
        {
            astrageldonHook?.Dispose();
        }

        public override void PostSetupContent()
        {
            if (InfernalCrossmod.SOTS.Loaded)
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<MetanovaOre>(), 11100, 1.5, WorldDefeats.downedAstrageldon);
        }

        private void ApplyAstrageldonScaling(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Inject at the end of the method
            cursor.Goto(il.Instrs.Count - 1);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate((Astrageldon self) =>
            {
                NPC npc = self.NPC;
                int baseHP = npc.lifeMax;
                int playerCount = GetActivePlayerCount();

                float multiplier = 1f;

                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    bool infernum = WorldSaveSystem.InfernumModeEnabled;
                    bool bossRush = BossRushEvent.BossRushActive is bool b2 && b2;

                    if (bossRush)
                        multiplier = 10.75f;
                    else
                    {
                        if (infernum)
                            multiplier *= 0.35f;
                        if (NPC.downedMoonlord)
                            multiplier += 0.25f;
                    }
                }

                npc.lifeMax += (int)(npc.lifeMax * multiplier);

                if (playerCount > 1)
                {
                    float scale = 0.5f * (playerCount - 1);
                    npc.lifeMax += (int)(npc.lifeMax * scale);
                }

                npc.life = 1;

                //Main.NewText($"Astrageldon HP scaled from {baseHP} → {npc.lifeMax}", Microsoft.Xna.Framework.Color.OrangeRed);
            });
        }

        private static int GetActivePlayerCount()
        {
            int count = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                    count++;
            }
            return count;
        }
    }
}
