using InfernumMode.Core.GlobalInstances.Systems;
using Terraria.DataStructures;
using System.Reflection;
using InfernalEclipseAPI.Core.World;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Players
{
    //Respawn Prevention Code Credit: Fargo's Souls Team
    public class RespawnPlayer : ModPlayer
    {
        public int Respawns;

        public static bool AnyBosses()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || (npc.type >= NPCID.EaterofWorldsBody && npc.type <= NPCID.EaterofWorldsTail)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool appliedThisDeath;

        public override void ResetEffects()
        {
            if (!AnyBosses() & !Player.dead)
            {
                appliedThisDeath = false;
                Respawns = 0;
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (AnyBosses()) Respawns++;
        }

        private static bool GetCalDifficulty(string diff)
        {
            return ModLoader.TryGetMod("CalamityMod", out Mod calamity) &&
                   calamity.Call("GetDifficultyActive", diff) is bool b && b;
        }

        private static bool IsInfernumActive()
        {
            return WorldSaveSystem.InfernumModeEnabled;
        }
        private static bool IsWorldLegendary()
        {
            FieldInfo findInfo = typeof(Main).GetField("_currentGameModeInfo", BindingFlags.Static | BindingFlags.NonPublic);
            GameModeData data = (GameModeData)findInfo.GetValue(null);
            return (Main.getGoodWorld && data.IsMasterMode);
        }

        public static bool isMinimumDiffToPreventRespawn()
        {
            if (InfernalCrossmod.NoxusBoss.Loaded)
            {
                if (NPC.AnyNPCs(InfernalCrossmod.NoxusBoss.Mod.Find<ModNPC>("AvatarOfEmptiness").Type))
                    return false;

                if (NPC.AnyNPCs(InfernalCrossmod.NoxusBoss.Mod.Find<ModNPC>("NamelessDeityBoss").Type))
                    return true;
            }

            Difficulty diff = InfernalConfig.Instance.MinimumDifficultyToPreventRespawns;
            switch (diff)
            {
                case Difficulty.AlwaysOff:
                    return false;
                case Difficulty.AlwaysOn:
                    return true;
                case Difficulty.Expert:
                    return Main.expertMode;
                case Difficulty.Master:
                    return Main.masterMode;
                case Difficulty.Legendary:
                    return IsWorldLegendary();
                case Difficulty.Revengence:
                    return GetCalDifficulty("death") || GetCalDifficulty("revengeance") || IsInfernumActive();
                case Difficulty.Death:
                    return GetCalDifficulty("death") || IsInfernumActive();
                case Difficulty.Infernum:
                    return IsInfernumActive();
                case Difficulty.Ragnarok:
                    return InfernalWorld.RagnarokModeEnabled;
                default:
                    return false;
            }
        }

        public bool RespawnsUsed()
        {
            if (InfernalCrossmod.NoxusBoss.Loaded)
            {
                if (NPC.AnyNPCs(InfernalCrossmod.NoxusBoss.Mod.Find<ModNPC>("NamelessDeityBoss").Type))
                    return true;
            }

            return Respawns > InfernalConfig.Instance.MultiplayerRespawnsAllowed;
        }

        public bool PreventRespawn() => isMinimumDiffToPreventRespawn() && AnyBosses() && RespawnsUsed() && Main.netMode != NetmodeID.SinglePlayer;
        public override void UpdateDead()
        {
            base.UpdateDead();

            ResetEffects();

            if (PreventRespawn()) Player.respawnTimer = 60 * 5;
            else
            {
                int respawnTimerSet = AnyBosses() ? (60 * InfernalConfig.Instance.MultiplayerBossRespawnTimer) : 180;
                if (Player.respawnTimer < respawnTimerSet || Player.respawnTimer > respawnTimerSet)
                {
                    if (!appliedThisDeath)
                    {
                        Player.respawnTimer = respawnTimerSet;
                        appliedThisDeath = true;
                    }
                }
            }
        }

        public override void OnRespawn()
        {
            appliedThisDeath = false;
        }
    }
}
