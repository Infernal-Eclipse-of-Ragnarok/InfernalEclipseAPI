using CalamityMod;
using CalamityMod.Events;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.WallOfFlesh;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Vanilla.WoFOverrides
{
    internal class WallofFleshEyeChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCID.WallofFleshEye;
        }

        public override void PostAI(NPC npc)
        {
            if (!InfernalWorld.RagnarokModeEnabled) return;

            if (!Main.npc.IndexInRange(Main.wofNPCIndex)) return;

            NPC wall = Main.npc[Main.wofNPCIndex];
            if (!wall.active || wall.type != NPCID.WallofFlesh) return;

            Player target = Main.player[wall.target];

            if (npc.Infernum().ExtraAI[WallOfFleshEyeBehaviorOverride.IsDetachedFlagIndex] == 1f)
            {
                int laserShootRate = 120;
                float wallAttackTimer = Main.npc[Main.wofNPCIndex].ai[3];
                bool doCircleAttack = wallAttackTimer % 1200f < 600f || wall.life > wall.lifeMax * WallOfFleshMouthBehaviorOverride.Phase2LifeRatio;
                Vector2 hoverOffset = (TwoPi * (npc.Infernum().ExtraAI[1] + wallAttackTimer / laserShootRate) / 4f).ToRotationVector2() * 360f;
                Vector2 hoverDestination = target.Center + hoverOffset;

                if (!wall.WithinRange(target.Center, 4000f))
                    hoverDestination = wall.Center;

                if (doCircleAttack) 
                {
                  
                    Vector2 laserShootVelocity = Utilities.SafeDirectionTo(npc, target.Center) * 8.5f;
                    Vector2 laserShootPosition = npc.Center + laserShootVelocity * 7.5f;

                    float shiftedTimer = (wallAttackTimer + laserShootRate / 2f) % laserShootRate;
                    if (shiftedTimer > laserShootRate - 40f)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Dust laser = Dust.NewDustPerfect(laserShootPosition + Main.rand.NextVector2Circular(25f, 25f), DustID.TheDestroyer);
                            laser.velocity = (laserShootPosition - laser.position).SafeNormalize(Vector2.UnitY) * -Main.rand.NextFloat(2f, 8f);
                            laser.noGravity = true;
                        }
                    }

                    // Fire halfway between Infernum's normal cardinal laser shots.
                    if (wallAttackTimer % laserShootRate == laserShootRate / 2f - 1f && !npc.WithinRange(target.Center, 115f) && npc.WithinRange(hoverDestination, 105f))
                    {
                        SoundEngine.PlaySound(SoundID.Item12, npc.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int laser = Utilities.NewProjectileBetter(laserShootPosition, laserShootVelocity, ProjectileID.EyeLaser, WallOfFleshMouthBehaviorOverride.EyeLaserDamage, 0f);
                            if (Main.projectile.IndexInRange(laser))
                            {
                                Main.projectile[laser].hostile = true;
                                Main.projectile[laser].tileCollide = false;
                            }
                        }
                    }
                }
            }
            else
            {
                int circleHoverCount = 0; 
                for (int i = 0; i < Main.maxNPCs; i++) 
                { 
                    if (!Main.npc[i].active || Main.npc[i].type != npc.type || Main.npc[i].Infernum().ExtraAI[WallOfFleshEyeBehaviorOverride.IsDetachedFlagIndex] == 0f) 
                        continue; 
                    
                    circleHoverCount++; 
                }

                if (!BossRushEvent.BossRushActive)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        player.AddBuff(ModContent.BuffType<HormonalBlockade>(), 2);
                    }
                }

                int beamShootRate = 1600 - circleHoverCount * 270; 
                int normalShootTime = (beamShootRate + npc.whoAmI * 300) % beamShootRate; 
                int extraShootTime = (normalShootTime + beamShootRate / 2) % beamShootRate; 
                
                // Fire halfway between Infernum's normal attached-eye beam attacks.
                if (npc.ai[1] % beamShootRate == extraShootTime) 
                    WallOfFleshMouthBehaviorOverride.PrepareFireBeam(npc, target); 
            }
        }
    }
}
