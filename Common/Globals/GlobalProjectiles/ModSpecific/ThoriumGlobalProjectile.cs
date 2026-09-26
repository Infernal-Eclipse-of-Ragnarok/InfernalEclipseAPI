using ThoriumMod;
using ThoriumMod.Projectiles;
using CalamityMod.Projectiles.Melee;
using CalamityMod;
using Terraria;
using Terraria.DataStructures;
using InfernalEclipseAPI.Core.Systems;
using ThoriumMod.Projectiles.Thrower;
using System.Collections.Generic;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Configs;

namespace InfernalEclipseAPI.Common.Globals.GlobalProjectiles.ModSpecific
{
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool triggeredGuaranteedProc;

        // Auto-detected Thorium-style flails
        private static readonly HashSet<int> FlailProjectiles = new();

        // Manual additions
        private static readonly HashSet<int> ManualFlailProjectiles = new();
        private static readonly HashSet<int> GuaranteedOnlyProjectiles = new();

        public override void Load()
        {
            FlailProjectiles.Clear();
            ManualFlailProjectiles.Clear();

            // AUTO THORIUM-STYLE FLAIL DETECTION

            foreach (Item item in ContentSamples.ItemsByType.Values)
            {
                if (
                    item.maxStack == 1 &&
                    item.damage > 0 &&
                    item.channel &&
                    item.noUseGraphic &&
                    item.useStyle == ItemUseStyleID.Shoot &&
                    item.shoot > ProjectileID.None &&
                    item.CountsAsClass(DamageClass.Melee)
                )
                {
                    Projectile proj = ContentSamples.ProjectilesByType[item.shoot];

                    if (proj.aiStyle == ProjAIStyleID.Flail)
                    {
                        FlailProjectiles.Add(proj.type);
                    }
                }
            }

            // MANUAL PROJECTILE ADDITIONS

            AddManualProjectile("CalamityMod", "UrchinMaceProj");
            AddManualProjectile("CalamityMod", "YateveoBloomMace");
            AddManualProjectile("CalamityMod", "BallOFuguProj");
            AddManualProjectile("CalamityMod", "ClamCrusherFlail");
            AddManualProjectile("CalamityMod", "TumbleweedFlail");
            AddManualProjectile("CalamityMod", "CrescentMoonFlail");
            AddManualProjectile("CalamityMod", "DragonPowFlail");
            AddManualProjectile("CalamityMod", "PulseDragonProjectile");
            AddManualProjectile("CalamityMod", "RemsRevengeProj");

            AddManualProjectile("Clamity", "ClamitasCrusherProjectile");

            AddManualProjectile("SOTS", "Shattershine");
            AddManualProjectile("SOTS", "AtenProj");
            AddManualProjectile("SOTS", "NorthStar");

            GuaranteedOnlyProjectiles.Add(ProjectileID.Flairon);
        }

        private static void AddManualProjectile(string modName, string projectileName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod))
                return;

            if (mod.TryFind(projectileName, out ModProjectile projectile))
            {
                ManualFlailProjectiles.Add(projectile.Type);
            }
        }

        public override void SetDefaults(Projectile entity)
        {
            if (entity.type == ModContent.ProjectileType<IceLancePro>() ||
                entity.type == ModContent.ProjectileType<SandStoneSpearPro>() ||
                entity.type == ModContent.ProjectileType<ForkPro>() ||
                entity.type == ModContent.ProjectileType<CoralPolearmPro>() ||
                entity.type == ModContent.ProjectileType<CoralPolearmPro2>() ||
                entity.type == ModContent.ProjectileType<CoralPolearmPro3>() ||
                //entity.type == ModContent.ProjectileType<HarpyTalonPro>() ||
                entity.type == ModContent.ProjectileType<PearlPikePro>() ||
                entity.type == ModContent.ProjectileType<MoonlightPro>() ||
                entity.type == ModContent.ProjectileType<MoonlightPro2>() ||
                entity.type == ModContent.ProjectileType<EnergyStormPartisanPro>() ||
                entity.type == ModContent.ProjectileType<FleshSkewerPro>() ||
                entity.type == ModContent.ProjectileType<HellishHalberdPro>() ||
                entity.type == ModContent.ProjectileType<HellishHalberdPro2>() ||
                entity.type == ModContent.ProjectileType<ValadiumSpearPro>() ||
                entity.type == ModContent.ProjectileType<BloodGloryPro1>() ||
                entity.type == ModContent.ProjectileType<BloodGloryPro3>()
            )
            {
                entity.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            }

            if (!InfernalConfig.Instance.ChanageWeaponClasses) return;

            if (entity.type == ModContent.ProjectileType<AncientFirePro>() || entity.type == ModContent.ProjectileType<AncientFirePro2>() || entity.type == ModContent.ProjectileType<BurningMeteor>())
            {
                entity.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            }

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) && !InfernalCrossmod.Hummus.Loaded)
            {
                int silverSpearTipProj = thorium.Find<ModProjectile>("SpearExtra")?.Type ?? -1;

                if (entity.type == silverSpearTipProj)
                {
                    if (entity.ModProjectile is ThoriumProjectile thoriumProj)
                    {
                        entity.aiStyle = 0;
                    }
                }
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.Name.Contains("Super Plasma Cannon Pro"))
            {
                if (projectile.damage > 500)
                    projectile.damage = 500;
            }

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) && !InfernalCrossmod.Hummus.Loaded)
            {
                int spiritBenderProj = thorium.Find<ModProjectile>("SpiritBendersStaffPro")?.Type ?? -1;

                if (projectile.type == spiritBenderProj)
                {
                    if (projectile.ModProjectile is ThoriumProjectile thoriumProj)
                    {
                        thoriumProj.lifeStealHealer = 0;
                    }
                }

                {
                    int silverSpearTipProj = thorium.Find<ModProjectile>("SpearExtra")?.Type ?? -1;
                    int moltenSpearTipProj = thorium.Find<ModProjectile>("SpearExtraFlame")?.Type ?? -1;
                    int crystalSpearTipProj = thorium.Find<ModProjectile>("SpearExtraCrystal")?.Type ?? -1;

                    if (projectile.type == silverSpearTipProj || projectile.type == moltenSpearTipProj || projectile.type == crystalSpearTipProj)
                    {
                        if (projectile.ModProjectile is ThoriumProjectile thoriumProj)
                        {
                            projectile.ai[2] = -1f;

                            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                                return;

                            if (SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] > 0)
                            {
                                projectile.active = false;
                                return;
                            }

                            SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] = 10;
                        }
                    }

                    if (projectile.type == silverSpearTipProj)
                    {
                        if (projectile.velocity != Vector2.Zero)
                            projectile.velocity =
                                projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
                    }
                    else if (projectile.type == moltenSpearTipProj)
                    {
                        if (projectile.velocity != Vector2.Zero)
                            projectile.velocity =
                                projectile.velocity.SafeNormalize(Vector2.Zero) * 10f;
                    }
                    else if (projectile.type == crystalSpearTipProj)
                    {
                        if (projectile.velocity != Vector2.Zero)
                            projectile.velocity =
                                projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
                    }
                }

                if (ModLoader.TryGetMod("ThoriumRework", out Mod helheim))
                {
                    int sporeSpearTipProj = helheim.Find<ModProjectile>("SpearExtraSpore")?.Type ?? -1;
                    int electronSpearTipProj = helheim.Find<ModProjectile>("SpearExtraElectronWave")?.Type ?? -1;

                    if (projectile.type == electronSpearTipProj)
                    {
                        if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                            return;

                        if (SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] > 0)
                        {
                            projectile.active = false;
                            return;
                        }

                        SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] = 10;
                    }

                    if (projectile.type == sporeSpearTipProj)
                    {
                        if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                            return;

                        if (SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] > 0 && projectile.ai[1] == 0)
                        {
                            projectile.active = false;
                            return;
                        }

                        SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] = 10;
                    }

                    if (projectile.type == electronSpearTipProj)
                    {
                        if (projectile.velocity != Vector2.Zero)
                            projectile.velocity =
                                projectile.velocity.SafeNormalize(Vector2.Zero) * 14f;
                    }
                }

                //Infernal Arsenal
                if (ModLoader.TryGetMod("InfernalEclipseWeaponsDLC", out Mod arsenal))
                {
                    int arcticSpearTipProj = arsenal.Find<ModProjectile>("CryonicSpearTip")?.Type ?? -1;
                    int searingSpearTipProj = arsenal.Find<ModProjectile>("HydrogenSulfideProj")?.Type ?? -1;

                    if (projectile.type == arcticSpearTipProj || projectile.type == searingSpearTipProj)
                    {
                        if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                            return;

                        if (SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] > 0)
                        {
                            projectile.active = false;
                            return;
                        }

                        SpearTipCooldown.SpearTipSpawnCooldown[projectile.owner] = 10;
                    }

                    if (projectile.type == arcticSpearTipProj)
                    {
                        if (projectile.velocity != Vector2.Zero)
                            projectile.velocity =
                                projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
                    }
                    if (projectile.type == searingSpearTipProj)
                    {
                        if (projectile.velocity != Vector2.Zero)
                            projectile.velocity =
                                projectile.velocity.SafeNormalize(Vector2.Zero) * 14f;
                    }
                }

                if (projectile.type == ModContent.ProjectileType<ShadowPurgeCaltropPro>())
                {
                    List<Projectile> caltrops = new();

                    // Gather all active caltrops for this player
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile p = Main.projectile[i];

                        if (!p.active)
                            continue;

                        if (p.owner != projectile.owner)
                            continue;

                        if (p.type != ModContent.ProjectileType<ShadowPurgeCaltropPro>())
                            continue;

                        caltrops.Add(p);
                    }

                    // If we exceed 10, remove the one with lowest timeLeft
                    if (caltrops.Count > 10)
                    {
                        Projectile lowest = null;

                        foreach (var p in caltrops)
                        {
                            if (lowest == null || p.timeLeft < lowest.timeLeft)
                                lowest = p;
                        }

                        if (lowest != null && lowest.active)
                        {
                            lowest.Kill();
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FlailProjectiles.Contains(projectile.type) || ManualFlailProjectiles.Contains(projectile.type) || GuaranteedOnlyProjectiles.Contains(projectile.type))
            {
                Player player = Main.player[projectile.owner];

                ThoriumPlayer thoriumPlayer =
                    player.GetModPlayer<ThoriumPlayer>();

                Vector2 launchVelocity = projectile.velocity * 0.5f;

                if (launchVelocity == Vector2.Zero)
                {
                    launchVelocity = Main.MouseWorld - player.Center;
                    launchVelocity.Normalize();
                    launchVelocity *= 6f;
                }

                IEntitySource source = projectile.GetSource_OnHit(target);

                // GUARANTEED FIRST PROC
                if (!triggeredGuaranteedProc)
                {
                    SpawnFlailCores(projectile, thoriumPlayer, source, launchVelocity);
                    triggeredGuaranteedProc = true;
                }

                // EXTRA 25% PROC FOR MANUAL ADDITIONS LIKE VANILLA THORIUM
                if (ManualFlailProjectiles.Contains(projectile.type) && Main.rand.NextBool(4))
                {
                    SpawnFlailCores(projectile, thoriumPlayer, source, launchVelocity);
                }
            }
        }

        private static void SpawnFlailCores(Projectile projectile, ThoriumPlayer thoriumPlayer, IEntitySource source, Vector2 launchVelocity)
        {
            // Iron Flail Core
            if (thoriumPlayer.accIronFlailCore)
            {
                SoundEngine.PlaySound(SoundID.Item1, projectile.Center);

                Projectile.NewProjectile(
                    source,
                    projectile.Center,
                    launchVelocity,
                    ModContent.ProjectileType<IronFlailCorePro>(),
                    (int)(projectile.damage * 0.35),
                    projectile.knockBack,
                    projectile.owner);
            }

            // Cursed Flail Core
            if (thoriumPlayer.accCursedFlailCore)
            {
                SoundEngine.PlaySound(SoundID.Item1, projectile.Center);

                Projectile.NewProjectile(
                    source,
                    projectile.Center,
                    launchVelocity,
                    ModContent.ProjectileType<CursedFlailCorePro>(),
                    (int)(projectile.damage * 0.65),
                    projectile.knockBack,
                    projectile.owner);
            }

            // Vile Flail Core
            if (thoriumPlayer.accVileFlailCore)
            {
                SoundEngine.PlaySound(SoundID.Item1, projectile.Center);

                Projectile.NewProjectile(
                    source,
                    projectile.Center,
                    launchVelocity,
                    ModContent.ProjectileType<VileFlailCorePro>(),
                    (int)(projectile.damage * 0.65),
                    projectile.knockBack,
                    projectile.owner);
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<OmniArrow3>())
            {
                modifiers.DisableCrit();
            }
        }
    }

    public class SpearTipCooldown : ModSystem
    {
        public static int[] SpearTipSpawnCooldown = new int[Main.maxPlayers];

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (SpearTipSpawnCooldown[i] > 0)
                    SpearTipSpawnCooldown[i]--;
            }
        }
    }
}
