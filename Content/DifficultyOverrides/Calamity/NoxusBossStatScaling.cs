using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.World;
using InfernumMode.Core.GlobalInstances.Systems;
using NoxusBoss.Content.NPCs.Bosses.Avatar.Projectiles;
using NoxusBoss.Content.NPCs.Bosses.Avatar.SecondPhaseForm;
using NoxusBoss.Content.NPCs.Bosses.Draedon;
using NoxusBoss.Content.NPCs.Bosses.Draedon.Projectiles;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity.Projectiles;
using Terraria.DataStructures;
using InfernumActive = InfernalEclipseAPI.Content.DifficultyOverrides.hellActive;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity
{
    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class NoxusBossStatScaling : GlobalNPC
    {
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return (npc.boss || npc.type == ModContent.NPCType<TrappingHolographicForcefield>()) && ((ModType)npc.ModNPC)?.Mod.Name == "NoxusBoss";
        }

        public override void SetDefaults(NPC entity)
        {
            if (entity.type == ModContent.NPCType<NamelessDeityBoss>())
            {
                entity.Calamity().canBreakPlayerDefense = true;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            Mod mod;
            bool flag = false;
            int num1 = 0, num2 = 0;

            if (ModLoader.TryGetMod("CalamityMod", out mod))
            {
                object result = mod.Call("GetDifficultyActive", "BossRush");
                if (result is bool b)
                {
                    flag = b;
                    num1 = 1;
                }
            }
            num2 = flag ? 1 : 0;
            if ((num1 & num2) != 0)
            {
                ModNPC modNpc1 = npc.ModNPC;
                if ((modNpc1 != null ? modNpc1.Name.Contains("MarsBody") ? 1 : 0 : 0) != 0)
                {
                    npc.lifeMax += (int)(npc.lifeMax * 0.25f);
                }
                else
                {
                    ModNPC modNpc3 = npc.ModNPC;
                    if ((modNpc3 != null ? modNpc3.Name.Contains("NamelessDeityBoss") ? 1 : 0 : 0) != 0)
                    {
                        npc.lifeMax += (int)(npc.lifeMax * 0.25f);
                    }
                }
            }

            if (InfernumActive.InfernumActive)
            {
                if (npc.type == ModContent.NPCType<AvatarOfEmptiness>())
                {
                    npc.lifeMax += (int)(0.20 * npc.lifeMax);
                }
                else if (npc.type == ModContent.NPCType<NamelessDeityBoss>())
                {
                    npc.lifeMax += (int)(0.4 * npc.lifeMax);
                }
                else
                    npc.lifeMax += (int)((double).35 * npc.lifeMax);
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (InfernumActive.InfernumActive)
            {
                modifiers.SourceDamage *= (npc.type == ModContent.NPCType<NamelessDeityBoss>() ? 1.4f : 1.35f);
            }
        }

        public override void PostAI(NPC npc)
        {
            if (InfernumActive.InfernumActive)
            {
                npc.position += npc.velocity * 0.35f;
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    public class WrathProjStatScaling : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private static bool IsInfernumActive() => WorldSaveSystem.InfernumModeEnabled;

        private static bool IsWorldLegendary()
        {
            FieldInfo findInfo = typeof(Main).GetField("_currentGameModeInfo", BindingFlags.Static | BindingFlags.NonPublic);
            GameModeData data = (GameModeData)findInfo.GetValue(null);
            return Main.getGoodWorld && data.IsMasterMode;
        }

        private readonly int[] marsTypes =
        {
            ModContent.ProjectileType<MarsMissile>(),
            ModContent.ProjectileType<RailGunCannonDeathray>(),
            ModContent.ProjectileType<SmallTeslaArc>(),
            ModContent.ProjectileType<TeslaField>(),
            ModContent.ProjectileType<UnstableMatter>(),
            ModContent.ProjectileType<ExoelectricDisintegrationRay>(),
        };

        private readonly int[] AoETypes =
        {
            ModContent.ProjectileType<DarkWave>(),
            ModContent.ProjectileType<PaleComet>(),
            ModContent.ProjectileType<DarkGas>(),
            ModContent.ProjectileType<OtherworldlyThorn>(),
            ModContent.ProjectileType<DarkPortal>(),
            ModContent.ProjectileType<AcceleratingRubble>(),
            ModContent.ProjectileType<PortalArmStrike>(),
            ModContent.ProjectileType<DisgustingStar>(),
            ModContent.ProjectileType<DeadStar>(),
            ModContent.ProjectileType<DeadStarIron>(),
            ModContent.ProjectileType<LilyStar>(),
            ModContent.ProjectileType<VoidBlot>(),
            ModContent.ProjectileType<StolenPlanetoid>(),
            ModContent.ProjectileType<MoltenBlob>(),
            ModContent.ProjectileType<RedirectingRubble>(),
            ModContent.ProjectileType<BloodBlob>(),
            ModContent.ProjectileType<BackgroundBloodBlob>(),
            ModContent.ProjectileType<ArcticBlast>(),
            ModContent.ProjectileType<Snowflake>(),
            ModContent.ProjectileType<FrostColumn>(),
            ModContent.ProjectileType<ConvergingSnowEnergy>(),
            ModContent.ProjectileType<FreezingWave>(),
            ModContent.ProjectileType<DimensionTwistedComet>(),
            ModContent.ProjectileType<AntimatterBlast>(),
            ModContent.ProjectileType<StellarRemnant>(),
            ModContent.ProjectileType<AnnihilationSphere>(),
            ModContent.ProjectileType<FallingMeleeWeapon>(),
            ModContent.ProjectileType<BloodTorrent>()
        };

        private readonly int[] NDTypes =
        {
            ModContent.ProjectileType<ArcingStarburst>(),
            ModContent.ProjectileType<LightDagger>(),
            ModContent.ProjectileType<TelegraphedScreenSlice>(),
            ModContent.ProjectileType<LightLaserElectricityArc>(),
            ModContent.ProjectileType<TelegraphedPortalLaserbeam>(),
            ModContent.ProjectileType<LightLaserCamellia>(),
            ModContent.ProjectileType<PrimordialStardust>(),
            ModContent.ProjectileType<Starburst>(),
            ModContent.ProjectileType<ControlledStar>(),
            ModContent.ProjectileType<TelegraphedStarLaserbeam>(),
            ModContent.ProjectileType<BlackHoleHostile>(),
            ModContent.ProjectileType<ConvergingSupernovaEnergy>(),
            ModContent.ProjectileType<PurifyingMatter>(),
            ModContent.ProjectileType<CelestialDreamcatcher>(),
            ModContent.ProjectileType<CodeLightningArc>(),
            ModContent.ProjectileType<CodeCrack>(),
            ModContent.ProjectileType<BigNamelessPunchImpact>(),
            ModContent.ProjectileType<SlowSolarSpark>(),
            ModContent.ProjectileType<ExplodingStar>(),
            ModContent.ProjectileType<SwordConstellation>(),
            ModContent.ProjectileType<StarPatternedStarburst>(),
            ModContent.ProjectileType<VergilScreenSlice>(),
            ModContent.ProjectileType<LightSlash>(),
            ModContent.ProjectileType<SuperCosmicBeam>(),
            ModContent.ProjectileType<FallingGalaxy>()
        };

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            foreach (int type in marsTypes)
            {
                if (entity.type == type)
                    return true;
            }

            foreach (int type in AoETypes)
            {
                if (entity.type == type)
                    return true;
            }

            foreach (int type in NDTypes)
            {
                if (entity.type == type)
                    return true;
            }

            return false;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                if (entity.type == ModContent.ProjectileType<UnstableMatter>() || entity.type == ModContent.ProjectileType<ExoelectricDisintegrationRay>() ||
                    entity.type == ModContent.ProjectileType<DarkWave>() || entity.type == ModContent.ProjectileType<DarkGas>() || entity.type == ModContent.ProjectileType<OtherworldlyThorn>() ||
                    entity.type == ModContent.ProjectileType<DarkPortal>() || entity.type == ModContent.ProjectileType<PortalArmStrike>() || entity.type == ModContent.ProjectileType<DisgustingStar>() ||
                    entity.type == ModContent.ProjectileType<LilyStar>() || entity.type == ModContent.ProjectileType<VoidBlot>() || entity.type == ModContent.ProjectileType<DimensionTwistedComet>() ||
                    entity.type == ModContent.ProjectileType<AntimatterBlast>() || entity.type == ModContent.ProjectileType<StellarRemnant>() || entity.type == ModContent.ProjectileType<AnnihilationSphere>() ||
                    entity.type == ModContent.ProjectileType<FallingMeleeWeapon>() || entity.type == ModContent.ProjectileType<BloodTorrent>() || entity.type == ModContent.ProjectileType<BloodBlob>() ||
                    entity.type == ModContent.ProjectileType<BackgroundBloodBlob>())
                {
                    entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                    entity.GetGlobalProjectile<VoidDamageProjectile>().strongVoidDamge = true;
                }
                else
                {
                    foreach (int type in NDTypes)
                    {
                        if (entity.type == type)
                        {
                            entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;

                            if (entity.type == ModContent.ProjectileType<BlackHoleHostile>() || entity.type == ModContent.ProjectileType<ConvergingSupernovaEnergy>())
                                entity.GetGlobalProjectile<VoidDamageProjectile>().strongerVoidDamage = true;
                            else
                                entity.GetGlobalProjectile<VoidDamageProjectile>().strongVoidDamge = true;
                        }
                    }
                }
            }

            if (entity.type == ModContent.ProjectileType<AcceleratingRubble>() || entity.type == ModContent.ProjectileType<DeadStar>() || entity.type == ModContent.ProjectileType<DeadStarIron>() ||
                entity.type == ModContent.ProjectileType<StolenPlanetoid>() || entity.type == ModContent.ProjectileType<RedirectingRubble>() || entity.type == ModContent.ProjectileType<SwordConstellation>() ||
                entity.type == ModContent.ProjectileType<StellarRemnant>() || entity.type == ModContent.ProjectileType<FallingMeleeWeapon>() || entity.type == ModContent.ProjectileType<TelegraphedScreenSlice>() ||
                entity.type == ModContent.ProjectileType<MarsMissile>() || entity.type == ModContent.ProjectileType<TelegraphedStarLaserbeam>() || entity.type == ModContent.ProjectileType<LightDagger>() ||
                entity.type == ModContent.ProjectileType<VergilScreenSlice>() || entity.type == ModContent.ProjectileType<LightSlash>() || entity.type == ModContent.ProjectileType<FallingGalaxy>())
            {
                entity.Calamity().DealsDefenseDamage = true;
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            float damageMod = 1f;

            if (IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (InfernalWorld.RagnarokModeEnabled)
            {
                if (NDTypes.Contains(projectile.type))
                    damageMod *= 1.4f;
                else
                    damageMod *= 1.35f;
            }
            else if (IsInfernumActive())
            {
                if (NDTypes.Contains(projectile.type))
                    damageMod *= 1.3f;
                else
                    damageMod *= 1.25f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }
}