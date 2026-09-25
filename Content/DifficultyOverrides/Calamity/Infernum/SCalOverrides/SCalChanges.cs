using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.UI.DialogueDisplay;
using CalamityMod.UI.DialogueDisplay.DisplayEffects;
using CalamityMod.World;
using InfernalEclipseAPI.Common.GlobalItems;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Assets.Sounds;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SupremeCalamitas;
using InfernumMode.Core.GlobalInstances.Systems;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Utilities;
using System.IO;
using System.Reflection;
using Terraria.Audio;
using Terraria.ModLoader.IO;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SCalOverrides
{
    public class SCalChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC npc, bool lateInstantiation) => npc.type == ModContent.NPCType<SupremeCalamitas>();
        public static string DialoguePath() => DownedBossSystem.downedCalamitas ? "Mods.InfernalEclipseAPI.SupremeCalamitas.PhasesRematch" : "Mods.InfernalEclipseAPI.SupremeCalamitas.Phases";

        #region Bullet Hell Variables
        public const int BulletHellDuration = 900;
        public const int SecondBulletHellEndValue = BulletHellDuration * 2;
        public const int ThirdBulletHellEndValue = BulletHellDuration * 3;
        public const int FourthBulletHellEndValue = BulletHellDuration * 4;
        public const int FifthBulletHellEndValue = BulletHellDuration * 5;

        public bool FinishedBH1 => bulletHellCounter2 >= BulletHellDuration;
        public bool FinishedBH2 => bulletHellCounter2 >= SecondBulletHellEndValue;
        public bool FinishedBH3 => bulletHellCounter2 >= ThirdBulletHellEndValue;
        public bool FinishedBH4 => bulletHellCounter2 >= FourthBulletHellEndValue;
        public bool FinishedBH5 => bulletHellCounter2 >= FifthBulletHellEndValue;

        public int bulletHellCounter = 0;
        public int bulletHellCounter2 = 0;

        public SlotId BulletHellRumbleSlot;
        #endregion

        public float uDieLul = 1f;
        public float passedVar = 0f;

        private const int HellblastDamage = 105;
        private const int GigablastDamage = 115;

        public bool hasTeleported = false;
        public bool despawnProj = false;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter writer)
        {
            writer.Write(despawnProj);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader reader)
        {
            despawnProj = reader.ReadBoolean();
        }

        public override bool PreAI(NPC npc)
        {
            ref float attackType = ref npc.ai[0];
            ref float attackTimer = ref npc.ai[1];
            ref float dialogueDisplayed = ref npc.Infernum().ExtraAI[25];

            if (InfernalCrossmod.Clamity.Loaded)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.dead || player.ghost || !npc.WithinRange(player.Center, 10000f))
                        continue;

                    if (player.mount?.Type == InfernalCrossmod.Clamity.Mod.Find<ModMount>("PlagueChairMount").Type)
                        player.mount.Dismount(player);
                }
            }

            if (SupremeCalamitasBehaviorOverride.Enraged)
            {
                float projectileVelocityMultCap = 2f;
                uDieLul = Clamp(uDieLul * 1.01f, 1f, projectileVelocityMultCap);
            }
            else
            {
                uDieLul = Clamp(uDieLul * 0.99f, 1f, 2f);
            }

            if (InfernalWorld.RagnarokModeEnabled)
            {
                if ((SupremeCalamitasBehaviorOverride.SCalAttackType)attackType == SupremeCalamitasBehaviorOverride.SCalAttackType.DesperationPhase)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (player.dead || player.ghost || !npc.WithinRange(player.Center, 10000f))
                            continue;

                        player.AddBuff(ModContent.BuffType<BrimstoneDesperation>(), 2);

                        if (InfernalCrossmod.Thorium.Loaded)
                        {
                            AntiHealerMulticlassCheck.ZeroHealBonus(npc);
                        }
                    }
                }

                if (!BossRushEvent.BossRushActive)
                {
                    if (npc.Infernum().ExtraAI[6] == 2f && (SupremeCalamitasBehaviorOverride.SCalAttackType)attackType == SupremeCalamitasBehaviorOverride.SCalAttackType.FireLaserSpin &&
                        !NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()) && !NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()) &&  //make sure there still aren't any brothers
                        FinishedBH3 && dialogueDisplayed < 5) //make sure bullet hell 3 has completed and we haven't already displayed this dialouge
                    {
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 5, 120, false, new BossText());
                        dialogueDisplayed = 5;
                    }

                    if (npc.Infernum().ExtraAI[4] == 4f && attackTimer == 660 && dialogueDisplayed < 10)
                    {
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 10, 120, false, new BossText());
                        dialogueDisplayed = 10;
                    }
                }
            }

            return base.PreAI(npc);
        }

        public bool DoBehaviour_BulletHell(NPC npc, Player player, int currentPhase, ref float frameType, ref float attackTimer)
        {
            if (!InfernalWorld.RagnarokModeEnabled) return true;

            ref float dialogueDisplayed = ref npc.Infernum().ExtraAI[25];

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool zenithAI = Main.zenithWorld;

            int bulletHellblast = zenithAI ? ModContent.ProjectileType<BrimstoneWave>() : ModContent.ProjectileType<BrimstoneHellblast2>();
            int barrage = ModContent.ProjectileType<BrimstoneBarrage>();
            int gigablast = zenithAI ? ModContent.ProjectileType<SCalBrimstoneFireblast>() : ModContent.ProjectileType<SCalBrimstoneGigablast>();
            int fireblast = zenithAI ? ModContent.ProjectileType<SCalBrimstoneGigablast>() : ModContent.ProjectileType<SCalBrimstoneFireblast>();
            int wave = zenithAI ? ModContent.ProjectileType<BrimstoneHellblast2>() : ModContent.ProjectileType<BrimstoneWave>();
            int hellblast = zenithAI ? ModContent.ProjectileType<BrimstoneWave>() : ModContent.ProjectileType<BrimstoneHellblast>();

            int baseBulletHellProjectileGateValue = revenge ? 8 : expertMode ? 9 : 10;

            //Spawn BH (BH1)
            if (currentPhase == 0f && !FinishedBH1)
            {
                ref float frameChangeSpeed = ref npc.localAI[1];

                despawnProj = true;
                bulletHellCounter2++;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                if (attackTimer == 1f && !hasTeleported)
                {
                    if (!BossRushEvent.BossRushActive)
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 0, 120, false, new BossText());
                    hasTeleported = true;
                }

                #region BulletHellEndTelegraphBH1
                if (bulletHellCounter2 == (BulletHellDuration - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(SupremeCalamitas.BulletHellSound, player.Center);
                if (bulletHellCounter2 > (BulletHellDuration - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                #endregion

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    AntiHealerMulticlassCheck.ZeroHealBonus(npc);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % (baseBulletHellProjectileGateValue * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        if (bulletHellCounter2 < 300 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 600) // Blasts from left and right
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from above, left, and right
                        {
                            if (!Main.zenithWorld)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 3f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                frameChangeSpeed = 0.2f;
                frameType = (int)SupremeCalamitasBehaviorOverride.SCalFrameType.MagicCircle;
                return false;
            }
            else if (FinishedBH1 && dialogueDisplayed < 1)
            {
                if (!BossRushEvent.BossRushActive)
                {
                    DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 1, 120, false, new BossText());
                }
                dialogueDisplayed = 1;
            }

            //BH2
            if (currentPhase == 1f && !FinishedBH2)
            {
                despawnProj = true;
                bulletHellCounter2++;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                if (attackTimer == 1f && !hasTeleported)
                {
                    TeleportToCenter(npc);
                    if (!BossRushEvent.BossRushActive)
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 2, 120, false, new BossText());
                }

                #region BulletHellEndTelegraphBH2
                if (bulletHellCounter2 == (SecondBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(SupremeCalamitas.BulletHellSound, player.Center);
                if (bulletHellCounter2 > (SecondBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == SecondBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(npc.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(SupremeCalamitas.BulletHellEndSound, npc.Center);
                }
                #endregion

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    AntiHealerMulticlassCheck.ZeroHealBonus(npc);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (bulletHellCounter2 < 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from top
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }
                    else if (bulletHellCounter2 < 1500 && bulletHellCounter2 > 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from right
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -5f * uDieLul, 0f, fireblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }
                    else if (bulletHellCounter2 > 1500)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from top
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 1)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 1) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 1200 && !Main.zenithWorld) // Blasts from below
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y + 1000f, 0f, -4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 1500) // Blasts from left
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                frameType = (int)SupremeCalamitasBehaviorOverride.SCalFrameType.MagicCircle;
                return false;
            }


            //BH3
            if (currentPhase == 2f && !FinishedBH3)
            {
                despawnProj = true;
                bulletHellCounter2++;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                if (attackTimer == 1f && !hasTeleported)
                {
                    TeleportToCenter(npc);
                    if (!BossRushEvent.BossRushActive)
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 3, 120, false, new BossText());
                }

                #region BulletHellEndTelegraphBH3
                if (bulletHellCounter2 == (ThirdBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(SupremeCalamitas.BulletHellSound, player.Center);
                if (bulletHellCounter2 > (ThirdBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == ThirdBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(npc.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(SupremeCalamitas.BulletHellEndSound, npc.Center);
                }
                #endregion

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    AntiHealerMulticlassCheck.ZeroHealBonus(npc);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (bulletHellCounter2 % 180 == 0) // Blasts from top
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 240 == 0) // Fireblasts from above
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 4)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 4) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 2100 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 2400) // Blasts from right
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                frameType = (int)SupremeCalamitasBehaviorOverride.SCalFrameType.MagicCircle;
                return false;
            }
            else if (FinishedBH3 && dialogueDisplayed < 4)
            {
                if (!BossRushEvent.BossRushActive)
                    DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 4, 120, false, new BossText());
                dialogueDisplayed = 4;
            }

            //BH4
            if (currentPhase == 3f && !FinishedBH4)
            {
                despawnProj = true;
                bulletHellCounter2++;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                if (attackTimer == 1f && !hasTeleported)
                {
                    TeleportToCenter(npc);
                    if (!BossRushEvent.BossRushActive)
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 6, 120, false, new BossText());
                }

                #region BulletHellEndTelegraphBH4
                if (bulletHellCounter2 == (FourthBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(SupremeCalamitas.BulletHellSound, player.Center);
                if (bulletHellCounter2 > (FourthBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == FourthBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(npc.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(SupremeCalamitas.BulletHellEndSound, npc.Center);
                }
                #endregion

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    AntiHealerMulticlassCheck.ZeroHealBonus(npc);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient) // More clustered attack
                {
                    if (bulletHellCounter2 % 180 == 0) // Blasts from top
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 240 == 0) // Fireblasts from above
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);

                    int divisor = revenge ? 225 : expertMode ? 450 : 675;

                    Vector2 spawnSpot = npc.Infernum().Arena.Center.ToVector2();
                    passedVar += 1f;
                    if (passedVar == 180) // Giant homing fireballs
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalAltarSummon") with { Pitch = 0.3f }, player.Center);
                        for (int i = 0; i < 2; i++)
                        {
                            Particle bloom = new BloomParticle(spawnSpot, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), 0f, 1.45f, 240, false);
                            GeneralParticleHandler.SpawnParticle(bloom);
                        }
                        Particle bloom2 = new BloomParticle(spawnSpot, Vector2.Zero, Color.White, 0f, 1.35f, 240, false);
                        GeneralParticleHandler.SpawnParticle(bloom2);
                    }
                    if (passedVar == 420) // Giant homing fireballs
                    {
                        for (int i = 0; i < 90; i++)
                        {
                            Dust spawnDust = Dust.NewDustPerfect(npc.Infernum().Arena.Center.ToVector2(), (int)CalamityDusts.Brimstone, new Vector2(30, 30).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 1.2f));
                            spawnDust.noGravity = true;
                            spawnDust.scale = Main.rand.NextFloat(1.2f, 2.3f);
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            Vector2 sparkVel = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.1f);
                            GlowOrbParticle orb = new GlowOrbParticle(npc.Infernum().Arena.Center.ToVector2() + sparkVel * 2, sparkVel, false, 120, Main.rand.NextFloat(1.55f, 2.75f), Color.Lerp(Color.Red, Color.Magenta, 0.3f), true, true);
                            GeneralParticleHandler.SpawnParticle(orb);
                        }

                        // Whispering Maelstrom passes through normal CanHitPlayer hooks then overrides them. This needs it to have contact damage but the number itself doesn't matter.
                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnSpot, Vector2.Zero, ModContent.ProjectileType<BrimstoneMonster>(), 100, 0f, Main.myPlayer, 0f);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 6)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 6) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 3000 && !Main.zenithWorld) // Blasts from below
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y + 1000f, 0f, -4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 3300) // Blasts from left
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                frameType = (int)SupremeCalamitasBehaviorOverride.SCalFrameType.MagicCircle;
                return false;
            }
            else if (FinishedBH4 && dialogueDisplayed < 7)
            {
                if (!BossRushEvent.BossRushActive)
                    DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 7, 120, false, new BossText());
                dialogueDisplayed = 7;
            }

            //BH5
            if (currentPhase == 4f && !FinishedBH5)
            {
                despawnProj = true;
                bulletHellCounter2++;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                if (attackTimer == 1f && !hasTeleported)
                {
                    TeleportToCenter(npc);
                    if (!BossRushEvent.BossRushActive)
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 8, 120, false, new BossText());
                }

                #region BulletHellEndTelegraphBH5
                if (bulletHellCounter2 == (FifthBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(SupremeCalamitas.BulletHellSound, player.Center);
                if (bulletHellCounter2 > (FifthBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == FifthBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(npc.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(SupremeCalamitas.BulletHellEndSound, npc.Center);
                }
                #endregion

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    AntiHealerMulticlassCheck.ZeroHealBonus(npc);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (bulletHellCounter2 % 240 == 0) // Blasts from top
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 360 == 0) // Fireblasts from above
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 30 == 0) // Projectiles that move in wave pattern
                    {
                        int random = Main.rand.Next(-500, 501);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + random, -5f * uDieLul, 0f, wave, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y - random, 5f * uDieLul, 0f, wave, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 8)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 8) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 3900 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 4200) // Blasts from left and right
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from above, left, and right
                        {
                            if (!Main.zenithWorld)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 3f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                frameType = (int)SupremeCalamitasBehaviorOverride.SCalFrameType.MagicCircle;
                return false;
            }
            else if (FinishedBH5)
            {
                Utilities.DeleteAllProjectiles(false, ModContent.ProjectileType<BrimstoneMonster>());

                if (dialogueDisplayed < 9)
                {
                    if (!BossRushEvent.BossRushActive)
                    {
                        DialogueDisplaySystem.StartDialogue(DialoguePath(), npc, 9, 120, false, new BossText());
                    }
                    dialogueDisplayed = 9;
                }
            }

            if (bulletHellCounter2 % BulletHellDuration == 0 && despawnProj)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active)
                    {
                        if (projectile.type == bulletHellblast ||
                            projectile.type == barrage ||
                            projectile.type == wave)
                        {
                            if (projectile.timeLeft > 60)
                                projectile.timeLeft = 60;
                        }
                        else if (projectile.type == fireblast || projectile.type == gigablast)
                        {
                            projectile.ai[2] = 1f;

                            if (projectile.timeLeft > 15)
                                projectile.timeLeft = 15;
                        }
                    }
                }
                despawnProj = false;
            }

            npc.chaseable = true;
            hasTeleported = false;
            return true;
        }

        private void TeleportToCenter(NPC npc)
        {
            SoundEngine.PlaySound(InfernumSoundRegistry.CalThunderStrikeSound, npc.Center);
            SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, npc.Center);

            for (int i = 0; i < 4; i++)
                SupremeCalamitasBehaviorOverride.ClearAllEntities();
            npc.Center = npc.Infernum().Arena.Center.ToVector2();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                ProjectileSpawnManagementSystem.PrepareProjectileForSpawning(explosion =>
                {
                    explosion.ModProjectile<DemonicExplosion>().MaxRadius = 600f;
                });
                Utilities.NewProjectileBetter(npc.Center, Vector2.Zero, ModContent.ProjectileType<DemonicExplosion>(), 0, 0f);

                npc.Infernum().ExtraAI[5] = 0f;
                npc.velocity = Vector2.Zero;
                npc.netUpdate = true;
            }

            hasTeleported = true;
        }
    }

    public class SCalPhaseTransitionHookSystem : ModSystem
    {
        private ILHook summonSepulcherHook;
        private ILHook phaseTransitionHook;

        private delegate bool SpawnBulletHellDelegate(NPC npc, Player target, ref float frameType, ref float attackTimer);
        private delegate bool BulletHellDelegate(NPC npc, Player target, ref float frameType, ref float attackTimer, int currentPhase = 0);

        public override void Load()
        {
            MethodInfo summonSepulcherMethod = typeof(SupremeCalamitasBehaviorOverride).GetMethod(nameof(SupremeCalamitasBehaviorOverride.DoBehavior_SummonSepulcher), LumUtils.UniversalBindingFlags, null,
            new[]
            {
                            typeof(NPC),
                            typeof(Player),
                            typeof(float).MakeByRefType(),
                            typeof(float).MakeByRefType(),
                            typeof(float).MakeByRefType()
            },
            null);

            if (summonSepulcherMethod is null)
                throw new MissingMethodException(typeof(SupremeCalamitasBehaviorOverride).FullName, nameof(SupremeCalamitasBehaviorOverride.DoBehavior_SummonSepulcher));

            summonSepulcherHook = new ILHook(summonSepulcherMethod, InsertSpawnBulletHell);

            MethodInfo phaseTransitionMethod = typeof(SupremeCalamitasBehaviorOverride).GetMethod(nameof(SupremeCalamitasBehaviorOverride.DoBehavior_PhaseTransition), LumUtils.UniversalBindingFlags, null,
                    new[]
                    {
                        typeof(NPC),
                        typeof(Player),
                        typeof(int),
                        typeof(float).MakeByRefType(),
                        typeof(float).MakeByRefType(),
                        typeof(float).MakeByRefType()
                    },
                    null);

            if (phaseTransitionMethod is null)
                throw new MissingMethodException(typeof(SupremeCalamitasBehaviorOverride).FullName, nameof(SupremeCalamitasBehaviorOverride.DoBehavior_PhaseTransition));

            phaseTransitionHook = new ILHook(phaseTransitionMethod, InsertBulletHell);
        }

        public override void Unload()
        {
            summonSepulcherHook?.Dispose();
            summonSepulcherHook = null;

            phaseTransitionHook?.Dispose();
            phaseTransitionHook = null;
        }

        private static void InsertSpawnBulletHell(ILContext il)
        {
            ILCursor cursor = new(il);

            ILLabel runNormalSepulcher = cursor.DefineLabel();

            cursor.EmitLdarg(0);
            cursor.EmitLdarg(1);
            cursor.EmitLdarg(2);
            cursor.EmitLdarg(4);

            cursor.EmitDelegate<SpawnBulletHellDelegate>
            (
                static (NPC npc, Player target,ref float frameType, ref float attackTimer) =>
                {
                    return RunBulletHell(npc, target, ref frameType, ref attackTimer, 0);
                }
            );

            // true: bullet hell has finished, so continue into Infernum's method.
            cursor.Emit(OpCodes.Brtrue, runNormalSepulcher);

            // false: bullet hell is still active, so skip the entire normal
            // phase-transition method for this frame.
            cursor.Emit(OpCodes.Ret);

            cursor.MarkLabel(runNormalSepulcher);
        }

        private static void InsertBulletHell(ILContext il)
        {
            ILCursor cursor = new(il);

            ILLabel runNormalTransition = cursor.DefineLabel();

            // arg0: NPC npc
            cursor.Emit(OpCodes.Ldarg_0);

            // arg1: Player target
            cursor.Emit(OpCodes.Ldarg_1);

            // arg3: ref float frameType
            cursor.Emit(OpCodes.Ldarg_3);

            // arg5: ref float attackTimer
            cursor.Emit(OpCodes.Ldarg_S, (byte)5);

            // arg2: int currentPhase
            cursor.Emit(OpCodes.Ldarg_2);

            cursor.EmitDelegate<BulletHellDelegate>(RunBulletHell);

            // true: bullet hell has finished, so continue into Infernum's method.
            cursor.Emit(OpCodes.Brtrue, runNormalTransition);

            // false: bullet hell is still active, so skip the entire normal
            // phase-transition method for this frame.
            cursor.Emit(OpCodes.Ret);

            cursor.MarkLabel(runNormalTransition);
        }

        private static bool RunBulletHell(NPC npc, Player target, ref float frameType, ref float attackTimer, int currentPhase = 0)
        {
            SCalChanges changes = npc.GetGlobalNPC<SCalChanges>();

            bool bulletHellFinished = changes.DoBehaviour_BulletHell(npc, target, currentPhase, ref frameType, ref attackTimer);

            if (!bulletHellFinished)
            {
                /* SupremeCalamitasBehaviorOverride.PreAI increments attackTimer after DoBehavior_PhaseTransition returns. 
                Keeping it at zero here means it becomes 1 at the end of every bullet-hell frame.
                Once the bullet hell finishes, the original transition method receives attackTimer == 1 and performs its normal first-frame initialization. */

                attackTimer = 0f;
                return false;
            }

            return true;
        }
    }
}
