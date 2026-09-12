using Terraria.ModLoader.IO;
using System.IO;
using CalamityMod.UI;
using InfernalEclipseAPI.Core.Systems;
using InfernumMode.Core.GlobalInstances.Systems;
using CalamityMod.Events;
using InfernalEclipseAPI.Core.Systems.BossRush;
using SubworldLibrary;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Core.Configs;
using Microsoft.Xna.Framework;

namespace InfernalEclipseAPI.Core.World
{
    public class InfernalWorld : ModSystem
    {
        public static bool sulfurScourgeDialoguePlayed = false;
        public static bool brimstoneDialoguePlayed = false;
        public static bool namelessDeveloperDiagloguePlayed = false;
        public static bool craftedWorkshop = false;
        public static bool RagnarokModeEnabled;
        public static bool hasChosenDifficulty = false;
        public static bool tier5Downed = false;
        public static bool tier6Downed = false;
        public static bool providenceNightDowned = false;
        public static bool cnidrionDowned = false;
        public static bool codebreakerCompleted = false;

        public static int dreamEaterAttempts = 0;

        public static void ResetFlags()
        {
            sulfurScourgeDialoguePlayed =false;
            brimstoneDialoguePlayed =false;
            namelessDeveloperDiagloguePlayed = false;
            craftedWorkshop = false;
            RagnarokModeEnabled = false;
            hasChosenDifficulty = false;
            tier5Downed = false;
            tier6Downed = false;
            providenceNightDowned = false;
            cnidrionDowned = false;
            codebreakerCompleted = false;

            dreamEaterAttempts = 0;
        }

        public override void PreUpdateWorld()
        {
            if (InfernalConfig.Instance.ThereIsNoReasonDisableThis && (WorldSaveSystem.InfernumModeEnabled || RagnarokModeEnabled))
            {
                if (Main.getGoodWorld && (RagnarokModeEnabled || WorldSaveSystem.InfernumModeEnabled))
                {
                    RagnarokModeEnabled = false;
                    WorldSaveSystem.InfernumModeEnabled = false;
                }

                if (InfernalCrossmod.FargosSouls.Loaded)
                {
                    if (FargoWorldFlagAdjustments.IsEmodeOrMasoActive())
                        FargoWorldFlagAdjustments.UnsetEmode();
                }
            }

            if (RagnarokModeEnabled && !WorldSaveSystem.InfernumModeEnabled)
                WorldSaveSystem.InfernumModeEnabled = true;

            if (SubworldSystem.AnyActive())
            {
                if (InfernalConfig.Instance.ForceRagnarokInfernumModeInSubworlds)
                {
                    if (Main.masterMode && !RagnarokModeEnabled)
                        RagnarokModeEnabled = true;
                    else if (Main.expertMode && !WorldSaveSystem.InfernumModeEnabled)
                        WorldSaveSystem.InfernumModeEnabled = true;
                }

                if (InfernalCrossmod.SOTS.Loaded)
                {
                    foreach (Projectile projectile in Main.projectile)
                    {
                        if (projectile.type == InfernalCrossmod.SOTS.Mod.Find<ModProjectile>("VoidAnomaly").Type)
                            projectile.active = false;
                    }
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.type == InfernalCrossmod.SOTS.Mod.Find<ModNPC>("Archaeologist").Type)
                            npc.active = false;
                    }
                }
            }

            if (BossRushEvent.BossRushActive)
            {
                CustomBossRushDialogue.Tick();

                if (InfernalCrossmod.FargosSouls.Loaded || InfernalCrossmod.Thorium.Loaded) 
                {
                    if (BossRushEvent.BossRushSpawnCountdown == 179 && BossRushEvent.EndTimer == 0 && (BossRushEvent.CurrentlyFoughtBoss == (InfernalCrossmod.FargosSouls.Loaded ? InfernalBossRush.SoulsNPC("TrojanSquirrel") : (InfernalCrossmod.Thorium.Loaded ? InfernalBossRush.ThoriumNPC("TheGrandThunderBird") : NPCID.KingSlime))))
                        BossRushEvent.CreateTierAnimation(1);
                }
            }
        }

        private static float dimnessFade;
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Main.gameMenu)
                return;

            if (NPC.AnyNPCs(NPCID.HallowBoss) && RagnarokModeEnabled)
            {
                if (dimnessFade < 1f)
                    dimnessFade += 0.01f;
            }
            else if (dimnessFade > 0f)
                dimnessFade -= 0.01f;

            dimnessFade = Clamp(dimnessFade, 0f, 1f);

            // Same darkness/color used by Lux.
            float strength = 0.96f * dimnessFade;
            Color darknessColor = new(15, 0, 30);

            backgroundColor = Color.Lerp(backgroundColor, darknessColor, strength);
            tileColor = Color.Lerp(tileColor, darknessColor, strength);
        }

        public override void OnWorldLoad()
        {
            ResetFlags();

            if (InfernalCrossmod.SOTS.Loaded)
            {
                int advisorType = InfernalCrossmod.SOTS.Mod.Find<ModNPC>("TheAdvisorHead").Type;
                if (!BossHealthBarManager.BossExclusionList.Contains(advisorType))
                    BossHealthBarManager.BossExclusionList.Add(advisorType);
            }
        }

        public override void OnWorldUnload()
        {
            ResetFlags();

            dimnessFade = 0f;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var infernalDowned = new System.Collections.Generic.List<string>();

            InfernalRecipeUnlockHandler.Save(infernalDowned);

            tag["sulfurScourgeDialoguePlayed"] = sulfurScourgeDialoguePlayed;
            tag["brimstoneDialoguePlayed"] = brimstoneDialoguePlayed;
            tag["namelessDeveloperDiagloguePlayed"] = namelessDeveloperDiagloguePlayed;
            tag["craftedWorkshop"] = craftedWorkshop;
            tag["RagnarokModeEnabled"] = RagnarokModeEnabled;
            tag["hasChosenDifficulty"] = hasChosenDifficulty;
            tag["tier5Downed"] = tier5Downed;
            tag["tier6Downed"] = tier6Downed;
            tag["providenceNightDowned"] = providenceNightDowned;
            tag["cnidrionDowned"] = cnidrionDowned;
            tag["codebreakerCompleted"] = codebreakerCompleted;

            tag["dreamEaterAttempts"] = dreamEaterAttempts;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var infernalDowned = tag.GetList<string>("infernalDowned");

            InfernalRecipeUnlockHandler.Load(infernalDowned);

            GetData(ref sulfurScourgeDialoguePlayed, "sulfurScourgeDialoguePlayed", tag);
            GetData(ref brimstoneDialoguePlayed, "brimstoneDialoguePlayed", tag);
            GetData(ref namelessDeveloperDiagloguePlayed, "namelessDeveloperDiagloguePlayed", tag);
            GetData(ref craftedWorkshop, "craftedWorkshop", tag);
            GetData(ref hasChosenDifficulty, "hasChosenDifficulty", tag);
            GetData(ref tier5Downed, "tier5Downed", tag);
            GetData(ref tier6Downed, "tier6Downed", tag);
            GetData(ref providenceNightDowned, "providenceNightDowned", tag);
            GetData(ref cnidrionDowned, "cnidrionDowned", tag);
            GetData(ref codebreakerCompleted, "codebreakerCompleted", tag);

            if (tag.TryGet("RagnarokModeEnabled", out bool value))
                RagnarokModeEnabled = value;
            else
                RagnarokModeEnabled = false;

            if (tag.TryGet("dreamEaterAttempts", out int attempts))
                dreamEaterAttempts = attempts;
            else
                dreamEaterAttempts = 0;
        }

        public static void GetData(ref bool baseVar, string path, TagCompound tag)
        {
            if (tag.ContainsKey(path)) { baseVar = tag.Get<bool>(path); }
        }

        public override void NetSend(BinaryWriter writer)
        {
            InfernalRecipeUnlockHandler.SendData(writer);

            writer.Write(sulfurScourgeDialoguePlayed);
            writer.Write(brimstoneDialoguePlayed);
            writer.Write(namelessDeveloperDiagloguePlayed);
            writer.Write(craftedWorkshop);
            writer.Write(RagnarokModeEnabled);
            writer.Write(hasChosenDifficulty);
            writer.Write(tier5Downed);
            writer.Write(tier6Downed);
            writer.Write(providenceNightDowned);
            writer.Write(cnidrionDowned);
            writer.Write(codebreakerCompleted);

            writer.Write(dreamEaterAttempts);
        }

        public override void NetReceive(BinaryReader reader)
        {
            InfernalRecipeUnlockHandler.ReceiveData(reader);

            sulfurScourgeDialoguePlayed = reader.ReadBoolean();
            brimstoneDialoguePlayed = reader.ReadBoolean();
            namelessDeveloperDiagloguePlayed = reader.ReadBoolean();
            craftedWorkshop = reader.ReadBoolean();
            RagnarokModeEnabled = reader.ReadBoolean();
            hasChosenDifficulty = reader.ReadBoolean();
            tier5Downed = reader.ReadBoolean();
            tier6Downed = reader.ReadBoolean();
            providenceNightDowned = reader.ReadBoolean();
            cnidrionDowned = reader.ReadBoolean();
            codebreakerCompleted = reader.ReadBoolean();

            dreamEaterAttempts = reader.ReadInt32();
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.FargosSouls.Name)]
    [ExtendsFromMod(InfernalCrossmod.FargosSouls.Name)]
    public static class FargoWorldFlagAdjustments
    {
        public static bool IsEmodeOrMasoActive() => FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode || FargowiltasSouls.Core.Systems.WorldSavingSystem.MasochistModeReal;

        public static void UnsetEmode()
        {
            FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode = false;
            FargowiltasSouls.Core.Systems.WorldSavingSystem.MasochistModeReal = false;
        }
    }
}
