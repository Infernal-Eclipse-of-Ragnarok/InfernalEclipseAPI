using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Projectiles.Inferno;
using SOTS.Projectiles.Minions;
using SOTS.Projectiles.Permafrost;
using SOTS.Projectiles.Planetarium;
using SOTS.Projectiles.Tide;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace InfernalEclipseAPI.Common.Globals.GlobalProjectiles.ModSpecific
{
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    public class SOTSGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            SOTSPlayer sotsPlayer = player.SOTSPlayer();

            if (projectile.type == ModContent.ProjectileType<Seeker>())
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Seeker>()] > 5)
                    projectile.active = false;
                else if (projectile.damage > 300)
                    projectile.damage = 300;
            }

            if (projectile.type == ModContent.ProjectileType<SharangaBlastSummon>())
            {
                if (!player.GetModPlayer<InfernalPlayer>().bagOfCharms)
                {
                    if (sotsPlayer.CritCurseFire)
                        projectile.damage = (int)(projectile.damage * 0.75f);
                    else if (sotsPlayer.CritFire)
                        projectile.damage = (int)(projectile.damage * 0.5f);
                }
            }

            if (projectile.type == ModContent.ProjectileType<IcePulseSummon>())
            {
                if (!player.GetModPlayer<InfernalPlayer>().bagOfCharms)
                {
                    if (sotsPlayer.CritCurseFire || sotsPlayer.CritFrost)
                        projectile.damage = (int)(projectile.damage * 0.75f);
                }
            }

            if (projectile.type == ModContent.ProjectileType<CursedThunder>())
            {
                if (!player.GetModPlayer<InfernalPlayer>().bagOfCharms)
                {
                    if (sotsPlayer.CritCurseFire)
                        projectile.damage = (int)(projectile.damage * 0.75f);
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<ThundershockShortbow>())
            {
                target.AddBuff(BuffID.Electrified, 60 * 3);
            }

            if (projectile.type == ModContent.ProjectileType<AncientSteelLantern>())
            {
                target.AddBuff(BuffID.OnFire, 120);
            }

            if (projectile.type == ModContent.ProjectileType<PermafrostSpirit>())
            {
                target.AddBuff(BuffID.Frostburn, 60 * 3);
            }

            if (projectile.type == ModContent.ProjectileType<EarthenSpirit>())
            {
                target.AddBuff(ModContent.BuffType<Crumbling>(), 60);
            }

            if (projectile.type == ModContent.ProjectileType<OtherworldlySpirit>())
            {
                target.AddBuff(BuffID.Electrified, 120);
            }

            if (projectile.type == ModContent.ProjectileType<TidalSpirit>())
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 60);
            }

            if (projectile.type == ModContent.ProjectileType<InfernoSpirit>())
            {
                target.AddBuff(BuffID.OnFire3, 60);
            }

            if (projectile.type == ModContent.ProjectileType<EvilSpirit>())
            {
                target.AddBuff(ModContent.BuffType<BrainRot>(), 60);
            }

            if (projectile.type == ModContent.ProjectileType<VoidspaceCell>())
            {
                target.AddBuff(BuffID.CursedInferno, 120);
            }
        }

        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (projectile.type == ModContent.ProjectileType<InfernoHook>())
            {
                speed = 11f;
            }
            base.GrapplePullSpeed(projectile, player, ref speed);
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (projectile.type == ModContent.ProjectileType<InfernoHook>())
            {
                speed = 17f;
            }
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public sealed class PlasmaShrimpLaserAdjustments : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool fromPlasmaShrimp;
        private Vector2 velocityBeforeAI;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ProjectileType<ShrimpLaser>();
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            fromPlasmaShrimp = false;

            // Detect only ShrimpLasers spawned by the PlasmaShrimp summon/projectile.
            if (source is EntitySource_Parent parentSource &&
                parentSource.Entity is Projectile parentProjectile &&
                parentProjectile.type == ModContent.ProjectileType<PlasmaShrimp>())
            {
                fromPlasmaShrimp = true;

                // Halve damage
                projectile.damage = (int)Math.Max(1, Math.Round(projectile.damage * 0.5));
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(fromPlasmaShrimp);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            fromPlasmaShrimp = bitReader.ReadBit();
        }

        public override bool PreAI(Projectile projectile)
        {
            if (fromPlasmaShrimp)
                velocityBeforeAI = projectile.velocity;

            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            if (!fromPlasmaShrimp)
                return;

            // Only cancel homing while the projectile is still in its normal flight state.
            // Once it has hit something / begun its shutdown behavior, leave it alone.
            if (!projectile.friendly || projectile.timeLeft < 60)
                return;

            // Preserve the original non-homing slowdown behavior from the base AI.
            projectile.velocity = velocityBeforeAI * 0.99f;

            if (projectile.velocity != Vector2.Zero)
                projectile.rotation = projectile.velocity.ToRotation();
        }
    }
}
