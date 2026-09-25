using CalamityMod.NPCs.Providence;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Providence;
using InfernumMode.Core.GlobalInstances;
using InfernumMode.Core.TrackedMusic;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static InfernumMode.Content.BehaviorOverrides.BossAIs.Providence.ProvidenceBehaviorOverride;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.ProvidenceOverrides
{
    public class ProvidenceChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
        {
            return npc.type == ModContent.NPCType<Providence>();
        }

        public override void Load()
        {
            GlobalNPCOverrides.OnKillEvent += SetNighttimeKilledIfApplicable;
        }

        private void SetNighttimeKilledIfApplicable(NPC npc)
        {
            if (npc.type == ModContent.NPCType<Providence>() && IsEnraged)
                InfernalWorld.providenceNightDowned = true;
        }

        public static List<ProvidenceAttackSection> Phase2AttackStatesNight =>
        [
            // Quiet section, prelude to fire form.
            new(new(BaseTrackedMusic.TimeFormat(0, 0, 0), BaseTrackedMusic.TimeFormat(0, 20, 0)), ProvidenceAttackType.EnterFireFormBulletHell),
            new(new(BaseTrackedMusic.TimeFormat(0, 20, 0), BaseTrackedMusic.TimeFormat(0, 32, 0)), ProvidenceAttackType.EnvironmentalFireEffects),

            // Fire form.
            new(new(BaseTrackedMusic.TimeFormat(0, 32, 0), BaseTrackedMusic.TimeFormat(0, 40, 0)), ProvidenceAttackType.CleansingFireballBombardment),
            new(new(BaseTrackedMusic.TimeFormat(0, 40, 0), BaseTrackedMusic.TimeFormat(0, 43, 0)), ProvidenceAttackType.CooldownState),
            new(new(BaseTrackedMusic.TimeFormat(0, 43, 0), BaseTrackedMusic.TimeFormat(0, 51, 0)), ProvidenceAttackType.ExplodingSpears),
            new(new(BaseTrackedMusic.TimeFormat(0, 51, 0), BaseTrackedMusic.TimeFormat(0, 53, 0)), ProvidenceAttackType.CooldownState),
            new(new(BaseTrackedMusic.TimeFormat(0, 53, 0), BaseTrackedMusic.TimeFormat(1, 3, 0)), ProvidenceAttackType.SpiralOfExplodingHolyBombs),
            new(new(BaseTrackedMusic.TimeFormat(1, 3, 0), BaseTrackedMusic.TimeFormat(1, 11, 0)), ProvidenceAttackType.ExplodingSpears),
            new(new(BaseTrackedMusic.TimeFormat(1, 11, 0), BaseTrackedMusic.TimeFormat(1, 12, 0)), ProvidenceAttackType.CooldownState),
            new(new(BaseTrackedMusic.TimeFormat(1, 12, 0), BaseTrackedMusic.TimeFormat(1, 18, 0)), ProvidenceAttackType.CleansingFireballBombardment),

            // Holy magic form.
            new(new(BaseTrackedMusic.TimeFormat(1, 18, 0), BaseTrackedMusic.TimeFormat(1, 23, 360)), ProvidenceAttackType.EnterHolyMagicForm),
            new(new(BaseTrackedMusic.TimeFormat(1, 23, 360), BaseTrackedMusic.TimeFormat(1, 37, 0)), ProvidenceAttackType.RockMagicRitual),
            new(new(BaseTrackedMusic.TimeFormat(1, 37, 0), BaseTrackedMusic.TimeFormat(1, 45, 0)), ProvidenceAttackType.ErraticMagicBursts),
            new(new(BaseTrackedMusic.TimeFormat(1, 45, 0), BaseTrackedMusic.TimeFormat(2, 6, 0)), ProvidenceAttackType.DogmaLaserBursts),

            // Light form and cycle restart.
            new(new(BaseTrackedMusic.TimeFormat(2, 6, 0), BaseTrackedMusic.TimeFormat(2, 8, 0)), ProvidenceAttackType.EnterLightForm),
            new(new(BaseTrackedMusic.TimeFormat(2, 8, 0), BaseTrackedMusic.TimeFormat(2, 37, 0)), ProvidenceAttackType.FinalPhaseRadianceBursts)
        ];

        public override bool PreAI(NPC npc)
        {
            HandleFightRestrictions();

            return base.PreAI(npc);
        }

        public static void HandleFightRestrictions()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead)
                    continue;

                if (InfernalWorld.RagnarokModeEnabled)
                    player.RemoveAllGrapplingHooks();

                if (InfernalCrossmod.Clamity.Loaded)
                {
                    if (player.mount?.Type == InfernalCrossmod.Clamity.Mod.Find<ModMount>("PlagueChairMount").Type)
                        player.mount.Dismount(player);
                }
            }
        }
    }

    public class NightimeProvidenceAIStateHookInjection : ModSystem
    {
        private Hook getLocalAttackInformationHook;
        private delegate ProvidenceAttackInformation Orig_GetLocalAttackInformation(NPC npc);

        public override void Load()
        {
            MethodInfo method = typeof(ProvidenceBehaviorOverride).GetMethod( nameof(GetLocalAttackInformation), LumUtils.UniversalBindingFlags);

            getLocalAttackInformationHook = new Hook(method, GetLocalAttackInformation_Detour);
        }

        public override void Unload()
        {
            getLocalAttackInformationHook?.Dispose();
            getLocalAttackInformationHook = null;
        }

        private static ProvidenceAttackInformation GetLocalAttackInformation_Detour(Orig_GetLocalAttackInformation orig, NPC npc)
        {
            if (npc.life > npc.lifeMax * Phase2LifeRatio || !IsEnraged || !InfernalWorld.RagnarokModeEnabled)
                return orig(npc);

            List<ProvidenceAttackSection> attackCycle = ProvidenceChanges.Phase2AttackStatesNight;

            ref float attackTimer = ref npc.ai[1];
            ref float startedWithMusicDisabled = ref npc.Infernum().ExtraAI[StartedWithMusicDisabledIndex];
            if (SyncAttacksWithMusic && startedWithMusicDisabled == 0f)
                attackTimer = (int)Math.Round(TrackedMusicManager.SongElapsedTime.TotalMilliseconds * 0.06f);

            else
            {
                attackTimer++;


                if (attackTimer >= attackCycle.Last().EndingTime)
                    attackTimer = 0f;

                startedWithMusicDisabled = 1f;
            }

            ProvidenceAttackSection attackSection = attackCycle.FirstOrDefault(a => npc.ai[1] >= a.StartingTime && npc.ai[1] < a.EndingTime);
            if (attackSection.StartingTime == 0 && attackSection.EndingTime == 0)
                attackSection = attackCycle[0];

            ProvidenceAttackType currentAttack = attackSection.AttackToUse;
            int localAttackTimer = (int)(attackTimer - attackSection.StartingTime);
            int localAttackDuration = attackSection.EndingTime - attackSection.StartingTime;

            /*
            if (InfernalConfig.Instance.DeveloperMode)
            {
                Main.NewText($"Attack Timer: {attackTimer}");
                Main.NewText($"Current Attack {currentAttack}");
                Main.NewText($"Local Attack Timer: {localAttackTimer}");
                Main.NewText($"Local Attack Duration: {localAttackDuration}");
                Main.NewText($"Full Cycle Time: {attackCycle.Last().EndingTime}");
            }
            */

            return new ProvidenceAttackInformation(localAttackTimer, localAttackDuration, currentAttack);
        }
    }
 }
