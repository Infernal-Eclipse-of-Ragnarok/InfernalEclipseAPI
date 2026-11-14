using System.Runtime.CompilerServices;
using CalamityMod;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using RevengeancePlus.Projectiles;
using SOTS.NPCs.Boss;
using SOTS.Projectiles;
using SOTS.Projectiles.Laser;
using Terraria.Audio;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Core.Systems.BossChanges
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    public class PutridPinkyNerf : ModSystem
    {
        private readonly ConditionalWeakTable<NPC, Holder> last = new();
        private class Holder { public float ai1; public float ai0; }

        public override void PostUpdateNPCs()
        {
            foreach (var npc in Main.npc)
            {
                if (npc == null || !npc.active) continue;

                var mn = npc.ModNPC;
                if (mn == null || mn.Mod?.Name != "SOTS" || mn.GetType().Name != "PutridPinkyPhase2")
                    continue;

                if (!last.TryGetValue(npc, out var h))
                    last.Add(npc, h = new Holder { ai0 = npc.ai[0], ai1 = npc.ai[1] });

                // Expected vanilla: ai[1] ticks down ~1 per frame in phases 1 & 3.
                bool affectedPhase = npc.ai[0] == 1f || npc.ai[0] == 3f;
                float delta = h.ai1 - npc.ai[1]; // positive when time was fast-forwarded

                if (affectedPhase && delta > 1f && delta <= 120f && Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
                {
                    // Undo the extra so net change is -1
                    npc.ai[1] += (delta - 1f);
                    npc.netUpdate = true;
                }

                // Undo the 210 -> 1/60 jump if it happened this tick
                if (affectedPhase && h.ai1 == 210f && (npc.ai[1] == 1f || npc.ai[1] == 60f) && Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] = 209f; // proceed naturally next tick
                    npc.netUpdate = true;
                }

                h.ai0 = npc.ai[0];
                h.ai1 = npc.ai[1];
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    public class PutridPinkyDashRoar : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private Vector2 previousVelocity;
        private bool roarCooldown;

        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
        {
            return npc.ModNPC?.Name == "PutridPinkyPhase2";
        }

        public override void PostAI(NPC npc)
        {
            if (!npc.active || !npc.HasValidTarget)
            {
                previousVelocity = npc.velocity;
                roarCooldown = false;
                return;
            }

            if (npc.ModNPC?.Name != "PutridPinkyPhase2")
            {
                previousVelocity = npc.velocity;
                roarCooldown = false;
                return;
            }

            // The dash behavior is phase/state ai[0] == 3 in the original code.
            if (npc.ai[0] != 3f)
            {
                previousVelocity = npc.velocity;
                roarCooldown = false;
                return;
            }

            Player target = Main.player[npc.target];

            float oldSpeed = previousVelocity.Length();
            float newSpeed = npc.velocity.Length();

            // Original dash sets velocity directly to Normalize(player - npc) * 16f * num1.
            // So watch for a sudden jump into a strong velocity aimed at the player.
            bool suddenAcceleration = oldSpeed < 12f && newSpeed >= 14f;
            bool aimedAtTarget = Vector2.Dot(npc.velocity.SafeNormalize(Vector2.Zero), npc.SafeDirectionTo(target.Center)) > 0.8f;

            if (suddenAcceleration && aimedAtTarget && !roarCooldown)
            {
                if (Main.netMode != NetmodeID.Server)
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                roarCooldown = true;
            }

            // Reset the latch once the boss slows down again so the next dash can roar too.
            if (newSpeed < 10f)
                roarCooldown = false;

            previousVelocity = npc.velocity;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    public class PutridNPCAdjustments : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<PutridHook>() || entity.type == ModContent.NPCType<HookTurret>();
        }

        public override void SetDefaults(NPC entity)
        {
            entity.Calamity().canBreakPlayerDefense = true;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    public class PutridProjectileAdjustments : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ProjectileType<PinkBomb>() || entity.type == ModContent.ProjectileType<PinkHelix>() || entity.type == ModContent.ProjectileType<PinkPellet>() || entity.type == ModContent.ProjectileType<PinkSplat>() ||
                entity.type == ModContent.ProjectileType<PinkBullet>() || entity.type == ModContent.ProjectileType<PinkTracer>() || entity.type == ModContent.ProjectileType<PinkLaser>();
        }

        public override void SetDefaults(Projectile entity)
        {
            entity.Calamity().DealsDefenseDamage = false;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            float damageMod = 1f;

            if (InfernalUtilities.IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (InfernalUtilities.IsInfernumActive() || InfernalUtilities.GetFargoDifficullty("MasochistMode"))
            {
                damageMod *= 1.35f;
            }
            else if (InfernalUtilities.GetFargoDifficullty("EternityMode"))
            {
                damageMod *= 1.25f;
            }
            else if (InfernalUtilities.GetCalDifficulty("death"))
            {
                damageMod *= 1.1f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    public class PinkBombNerf : GlobalProjectile
    {
        private bool _fromPP2 = false;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) => projectile.type == ModContent.ProjectileType<PinkBomb>();

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            projectile.active = false;

            /*
            if (source is EntitySource_Parent p && p.Entity is NPC npc)
            {
                var mn = npc.ModNPC;
                if (mn != null &&
                    string.Equals(mn.Mod?.Name, "SOTS", StringComparison.Ordinal) &&
                    string.Equals(mn.GetType().Name, "PutridPinkyPhase2", StringComparison.Ordinal))
                {
                    _fromPP2 = true;
                    projectile.ai[1] = 6f;
                    projectile.velocity *= 0.75f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        projectile.netUpdate = true;
                }
            }
            */
        }

        public override void PostAI(Projectile projectile)
        {
            if (!_fromPP2) return;

            // keep it fixed at 6 in case other logic tries to mutate it later
            if (projectile.ai[1] != 6f)
                projectile.ai[1] = 6f;

            if (Main.netMode != NetmodeID.MultiplayerClient)
                projectile.netUpdate = true;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name, InfernalCrossmod.RevengeancePlus.Name)]
    public class PinkHelixNerf : GlobalProjectile
    {
        public override bool InstancePerEntity => false;

        private static int PinkHelixType
        {
            get
            {
                Mod revengeancePlus = ModLoader.GetMod("RevengeancePlus");
                return revengeancePlus.Find<ModProjectile>("PinkHelix").Type;
            }
        }

        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
            => ModLoader.HasMod("RevengeancePlus") && projectile.type == PinkHelixType;

        public override void PostAI(Projectile projectile)
        {
            // Only affect the "single helix" version.
            // In NPCChanges, the single helix is spawned with ai2 = 1f.
            if (projectile.ai[2] != 1f)
                return;

            Vector2 safeVelocity = projectile.velocity;
            if (safeVelocity.LengthSquared() <= 0.0001f)
                safeVelocity = Vector2.UnitX;

            Vector2 perp = safeVelocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            float wave = (float)System.Math.Sin(MathHelper.ToRadians(projectile.timeLeft * 10f) + MathHelper.PiOver2);

            // Undo half of the original helix offset so the final amplitude is 50%.
            Vector2 correction = perp * projectile.ai[1] * projectile.ai[0] * wave * 0.5f;
            projectile.position -= correction;
        }
    }
}
