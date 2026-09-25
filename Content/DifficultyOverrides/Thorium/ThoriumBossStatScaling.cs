using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;
using System.Reflection;
using Terraria.DataStructures;
using ThoriumMod.NPCs.BossBoreanStrider;
using ThoriumMod.NPCs.BossThePrimordials;
using ThoriumMod.Projectiles.Boss;
using System.Linq;
using InfernalEclipseAPI.Core.Systems;
using ThoriumRework.Projectiles;
using InfernalEclipseAPI.Content.Buffs;
using ThoriumMod.Projectiles.Enemy;
using ThoriumMod.NPCs.BossQueenJellyfish;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using ThoriumMod.NPCs.BossViscount;
using CalamityMod;
using Microsoft.Xna.Framework;
using ThoriumRework.Buffs;
using CalamityMod.CalPlayer;
using ThoriumMod.NPCs.BossFallenBeholder;
using ThoriumMod.NPCs.BossForgottenOne;
using ThoriumMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using InfernalEclipseAPI.Core.World;
using ThoriumMod.NPCs.BossGraniteEnergyStorm;
using CalamityMod.Events;
using ThoriumMod.NPCs.BossTheGrandThunderBird;
using ThoriumMod.NPCs.BossBuriedChampion;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Thorium
{
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumBossStatScaling : GlobalNPC
    {
        private static bool GetCalDifficulty(string diff)
        {
            return ModLoader.TryGetMod("CalamityMod", out Mod calamity) &&
                   calamity.Call("GetDifficultyActive", diff) is bool b && b;
        }

        private static bool IsInfernumActive()
        {
            return InfernumSaveSystem.InfernumModeEnabled;
        }

        private static bool GetFargoDifficullty(string diff)
        {
            if (!ModLoader.TryGetMod("FargowiltasSouls", out Mod fargoSouls))
            {
                return false;
            }

            return fargoSouls.Call(diff) is bool active && active;
        }
        private static bool IsWorldLegendary()
        {
            FieldInfo findInfo = typeof(Main).GetField("_currentGameModeInfo", BindingFlags.Static | BindingFlags.NonPublic);
            GameModeData data = (GameModeData)findInfo.GetValue(null);
            return Main.getGoodWorld && data.IsMasterMode;
        }

        public static readonly int[] thorBossMinionTypes =
        [
            ModContent.NPCType<ThoriumMod.NPCs.BossThePrimordials.AquaiusBubble>(),
            ModContent.NPCType<ThoriumMod.NPCs.BossThePrimordials.ImpendingDread>(),
            ModContent.NPCType<UnstableAnger>(),
            ModContent.NPCType<InnerDespair>(),
            ModContent.NPCType<LucidBubble>(),
            ModContent.NPCType<BoreanHopper>(),
            ModContent.NPCType<BoreanMyte>(),
            ModContent.NPCType<DistractingJellyfish>(),
            ModContent.NPCType<SpittingJellyfish>(),
            ModContent.NPCType<ZealousJellyfish>(),
            ModContent.NPCType<ThoriumMod.NPCs.BossViscount.BiteyBaby>(),
            ModContent.NPCType<AbyssalSpawn>()
        ];

        public static bool IsReworkNPC(NPC npc)
        {
            if (!InfernalCrossmod.ThoriumRework.Loaded) return false;

            return ThoriumReworkEntities.IsReworkedThoriumMinion(npc);
        }

        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
        {
            return (npc.boss || thorBossMinionTypes.Contains(npc.type) || IsReworkNPC(npc)) && (npc.ModNPC?.Mod?.Name == "ThoriumMod" || npc.ModNPC?.Mod?.Name == "ThoriumRework");
        }

        public override void SetDefaults(NPC entity)
        {
            if (IsInfernumActive()) 
            {
                if (entity.type == ModContent.NPCType<BoreanStrider>())
                {
                    entity.defense = 100;
                }
                if (entity.type == ModContent.NPCType<BoreanStriderPopped>())
                {
                    entity.defense = 18;
                }
                if (entity.type == ModContent.NPCType<BoreanHopper>())
                {
                    entity.defense = 100;
                }
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                if (entity.type == ModContent.NPCType<Viscount>() || entity.type == ModContent.NPCType<ThoriumMod.NPCs.BossViscount.BiteyBaby>() ||
                    entity.type == ModContent.NPCType<ForgottenOne>() || entity.type == ModContent.NPCType<ForgottenOneCracked>())
                {
                    entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
                }

                if (entity.type == ModContent.NPCType<DreamEater>() || entity.type == ModContent.NPCType<ForgottenOneReleased>())
                {
                    entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
                    entity.GetGlobalNPC<SOTSGlobalNPC>().strongVoidDamge = true;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            //Boss Rush
            if (GetCalDifficulty("bossrush"))
            {
                string name = npc.ModNPC?.Name ?? "";

                //do this 
                if (name.Contains("BoreanStrider"))
                    npc.lifeMax *= 25; //less due to how much it already has

                //ignore the rest if Thorium Bosses Reworked is active as this is already done in that mod.
                if (!ModLoader.TryGetMod("ThoriumRework", out _))
                {
                    if (name.Contains("TheGrandThunderBird"))
                        npc.lifeMax *= 125;
                    else if (name.Contains("QueenJellyfish"))
                        npc.lifeMax *= 115;
                    else if (name.Contains("Viscount"))
                        npc.lifeMax *= 110;
                    else if (name.Contains("StarScouter"))
                        npc.lifeMax *= 105;
                    else if (name.Contains("BuriedChampion") || name.Contains("GraniteEnergyStorm"))
                        npc.lifeMax *= 75;
                    else if (name.Contains("FallenBeholder"))
                        npc.lifeMax *= 65;
                    else if (name.Contains("Lich"))
                        npc.lifeMax *= 30;
                    else if (name.Contains("ForgottenOne"))
                        npc.lifeMax *= 15;
                    else if (name.Contains("SlagFury") || name.Contains("Aquaius") || name.Contains("Omnicide") || name.Contains("DreamEater"))
                        npc.lifeMax *= 2;
                }
            }

            if (IsWorldLegendary())
            {
                npc.lifeMax += (int)(0.1 * npc.lifeMax);
            }
            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                if (npc.ModNPC.Name.Contains("QueenJellyfish") )
                {
                    npc.lifeMax += npc.lifeMax;
                }
                if (npc.type == ModContent.NPCType<FallenBeholder>() || npc.type == ModContent.NPCType<FallenBeholder2>() ||
                    npc.type == ModContent.NPCType<BoreanStrider>() || npc.type == ModContent.NPCType<BoreanStriderPopped>())
                {
                    npc.lifeMax += (int)(npc.lifeMax * 0.5f);
                }
                if (npc.type == ModContent.NPCType<BuriedChampion>())
                {
                    npc.lifeMax += (int)(npc.lifeMax * 0.4f);
                }
                if (npc.type == ModContent.NPCType<ForgottenOne>() || npc.type == ModContent.NPCType<ForgottenOneCracked>() || npc.type == ModContent.NPCType<ForgottenOneReleased>())
                {
                    npc.lifeMax += (int)(npc.lifeMax * 0.25f);
                }
                if (npc.type == ModContent.NPCType<GraniteEnergyStorm>() || npc.ModNPC?.Name?.Contains("StarScouter") == true)
                {
                    npc.lifeMax += (int)(npc.lifeMax * 0.05f);
                }
                string name = npc.ModNPC?.Name ?? "";
                if (name.Contains("SlagFury") || name.Contains("Aquaius") || name.Contains("Omnicide") || name.Contains("DreamEater"))
                    npc.lifeMax += (int)(0.15 * npc.lifeMax);

                npc.lifeMax += (int)(0.35 * npc.lifeMax);
            }
            else
            {
                if (GetCalDifficulty("death"))
                {
                    if (npc.type == ModContent.NPCType<BoreanStrider>() || npc.type == ModContent.NPCType<BoreanStriderPopped>())
                    {
                        npc.lifeMax += (int)(npc.lifeMax * 0.25f);
                    }
                    if (npc.ModNPC.Name.Contains("QueenJellyfish"))
                    {
                        npc.lifeMax += (int)(0.5 * npc.lifeMax);
                    }

                    npc.lifeMax += (int)(0.2 * npc.lifeMax);
                }
                else if (GetCalDifficulty("revengeance"))
                {
                    if (npc.ModNPC.Name.Contains("QueenJellyfish"))
                    {
                        npc.lifeMax += (int)(0.25 * npc.lifeMax);
                    }

                    npc.lifeMax += (int)(0.1 * npc.lifeMax);
                }
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            string name = npc.ModNPC?.Name ?? "";
            float damageMod = 0;

            if (name.Contains("SlagFury") || name.Contains("Aquaius") || name.Contains("Omnicide") || name.Contains("DreamEater"))
                damageMod += 0.75f;
            else if (name.Contains("BoreanStrider") || name.Contains("QueenJellyfish") || name.Contains("Viscount"))
                damageMod += 0.6f;
            else if (name.Contains("BiteyBaby"))
                damageMod += 1f;
            else if (npc.type == ModContent.NPCType<FallenBeholder>() || npc.type == ModContent.NPCType<FallenBeholder2>())
                damageMod += 0.05f;

            if (IsWorldLegendary())
            {
                damageMod += 1.1f;
            }
            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                damageMod += 1.35f;
            }
            else
            {
                if (GetFargoDifficullty("EternityMode"))
                {
                    damageMod += 1.25f;
                }
                else if (GetCalDifficulty("death"))
                {
                    damageMod += 1.1f;
                }
            }

            modifiers.SourceDamage *= damageMod;
        }

        public override void PostAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<ForgottenOne>() && npc.defense != 240)
            {
                npc.defense = 120;
            }
            if (npc.type == ModContent.NPCType<ForgottenOneCracked>() && npc.defense == 40)
            {
                npc.defense = 75;
            }

            if (npc.ModNPC?.Name?.Contains("BoreanStrider") == true)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (!player.dead && npc.WithinRange(player.Center, 1000f))
                        continue;

                    player.AddBuff(ModContent.BuffType<FreezingAura>(), 1);
                }
                return;
            }

            if (BossRushEvent.BossRushActive)
                return;

            if (npc.ModNPC.Name.Contains("FallenBeholder") || npc.ModNPC.Name.Contains("BoreanStrider") || npc.type == ModContent.NPCType<TheGrandThunderBird>() || npc.type == ModContent.NPCType<BuriedChampion>())
                return;

            if (IsWorldLegendary())
            {
                npc.position += npc.velocity * 0.5f;
            }
            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                npc.position += npc.velocity * 0.20f;
            }
            else
            {
                if (GetFargoDifficullty("EternityMode"))
                {
                    npc.position += npc.velocity * 0.10f;
                }
                else if (GetCalDifficulty("death"))
                {
                    npc.position += npc.velocity * 0.05f;
                }
            }
        }
    }

    [JITWhenModsEnabled("ThoriumRework")]
    [ExtendsFromMod("ThoriumRework")]
    public static class ThoriumReworkEntities
    {
        public static bool IsReworkedThoriumMinion(NPC npc)
        {
            int[] reworkType =
            [
            ];

            if (reworkType.Contains(npc.type))
                return true;

            return false;
        }

        public static bool IsReworkedThoriumProjectile(Projectile projectile)
        {
            int[] reworkType =
            [
                ModContent.ProjectileType<ThoriumRework.Projectiles.ImpendingDread>(),
                ModContent.ProjectileType<ImpendingDreadF>(),
                ModContent.ProjectileType<ThoriumRework.Projectiles.LucidRay>(),
                ModContent.ProjectileType<LucidNuke>(),
                ModContent.ProjectileType<ThoriumRework.Projectiles.AquaiusBubble>(),
                ModContent.ProjectileType<AquaiusPunchAttack>(),
                ModContent.ProjectileType<DeathRain>(),
                ModContent.ProjectileType<InfernalRay>(),
                ModContent.ProjectileType<TideDagger>(),
                ModContent.ProjectileType<FalseSun>(),
                ModContent.ProjectileType<EraserLaser>(),
                ModContent.ProjectileType<ExtinctionRay>(),
                ModContent.ProjectileType<NightmareClaw>(),
                ModContent.ProjectileType<NightmareSlash>(),
                ModContent.ProjectileType<DyingReality>(),

                ModContent.ProjectileType<IceShard>(),
                ModContent.ProjectileType<Glacier>(),
                ModContent.ProjectileType<Glacier2>(),

                ModContent.ProjectileType<DancingJellyfish>(),
                ModContent.ProjectileType<ThoriumRework.Projectiles.BubbleBomb>(),
                ModContent.ProjectileType<HighTide>(),
                ModContent.ProjectileType<HighTideWave>(),
                ModContent.ProjectileType<BubbleColumn>(),
                ModContent.ProjectileType<JammingJellyfish>(),
                ModContent.ProjectileType<JellyfishShock>(),

                ModContent.ProjectileType<ThoriumRework.Projectiles.BiteyBaby>(),

                ModContent.ProjectileType<ThoriumRework.Projectiles.SoulSteal>(),
                ModContent.ProjectileType<EradicationRay>(),
                ModContent.ProjectileType<EradicationBeam>(),
                ModContent.ProjectileType<EradicationBeamF>(),
                ModContent.ProjectileType<FalseBeholder>(),
                ModContent.ProjectileType<VoidEye>(),
                ModContent.ProjectileType<VoidEyeF>(),

                ModContent.ProjectileType<AbyssBubble>(),
                ModContent.ProjectileType<AbyssBubble2>(),
                ModContent.ProjectileType<AbyssalApparition>(),
            ];

            foreach (int type in reworkType)
            {
                if (projectile.type == type)
                    return true;
            }

            return false;
        }
    }


    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumBossProjStatScaling : GlobalProjectile
    {
        public static bool IsReworkNPC(Projectile projectile)
        {
            if (!InfernalCrossmod.ThoriumRework.Loaded) return false;

            return ThoriumReworkEntities.IsReworkedThoriumProjectile(projectile);
        }

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                //Primordials
                ModContent.ProjectileType<AquaSplash>(),
                ModContent.ProjectileType<AquaTyphoon>(),
                ModContent.ProjectileType<LucidFury>(),
                ModContent.ProjectileType<LucidMiasma>(),
                ModContent.ProjectileType<LucidPulse>(),
                ModContent.ProjectileType<LucidTyphoon>(),
                ModContent.ProjectileType<FlameFury>(),
                ModContent.ProjectileType<FlameLash>(),
                ModContent.ProjectileType<FlameNova>(),
                ModContent.ProjectileType<FlamePulsePro>(),
                ModContent.ProjectileType<AquaBomb2>(),
                ModContent.ProjectileType<LucidBomb2>(),
                ModContent.ProjectileType<ThoriumMod.Projectiles.Boss.LucidRay>(),
                ModContent.ProjectileType<DeathRaySpawn3>(),
                ModContent.ProjectileType<DeathCircle2>(),
                ModContent.ProjectileType<DeathRay>(),

                //Borean Strider
                ModContent.ProjectileType<BlizzardFang>(),
                ModContent.ProjectileType<BlizzardBoom>(),
                ModContent.ProjectileType<ThoriumMod.Projectiles.Enemy.IceAnomaly>(),

                //Queen Jellyfish
                ModContent.ProjectileType<BubblePulse>(),
                ModContent.ProjectileType<ThoriumMod.Projectiles.Boss.BubbleBomb>(),

                //Viscount
                ModContent.ProjectileType<ViscountBlood>(),
                ModContent.ProjectileType<ViscountRipple>(),
                ModContent.ProjectileType<ViscountRipple2>(),
                ModContent.ProjectileType<ViscountRipple3>(),
                ModContent.ProjectileType<ViscountRockFall>(),
                ModContent.ProjectileType<ViscountRockSummon>(),
                ModContent.ProjectileType<ViscountRockSummon>(),
                ModContent.ProjectileType<ViscountStomp>(),
                ModContent.ProjectileType<ViscountStomp2>(),      
                
                //Fallen Beholder
                ModContent.ProjectileType<BeholderLavaCascade>(),
                ModContent.ProjectileType<BeholderLavaCascade1>(),
                ModContent.ProjectileType<ThoriumMod.Projectiles.Boss.SoulSteal>(),
                ModContent.ProjectileType<BeholderBeam>(),
                ModContent.ProjectileType<DoomBeholderBeam>(),

                //Forgotten One
                ModContent.ProjectileType<ForgottenOneSpit>(),
                ModContent.ProjectileType<ForgottenOneSpit2>(),
                ModContent.ProjectileType<Whirlpool>(),
                ModContent.ProjectileType<AquaRipple>(),
                ModContent.ProjectileType<AbyssalStrike>(),
                ModContent.ProjectileType<AbyssalStrike2>(),
                ModContent.ProjectileType<OldGodSpit>(),
                ModContent.ProjectileType<OldGodSpit2>()
            ];

            foreach (int type in types)
            {
                if (entity.type == type || IsReworkNPC(entity))
                    return true;
            }
            return false;
        }

        private static bool GetCalDifficulty(string diff)
        {
            return ModLoader.TryGetMod("CalamityMod", out Mod calamity) &&
                   calamity.Call("GetDifficultyActive", diff) is bool b && b;
        }

        private static bool IsInfernumActive()
        {
            return InfernumSaveSystem.InfernumModeEnabled;
        }

        private static bool GetFargoDifficullty(string diff)
        {
            if (!ModLoader.TryGetMod("FargowiltasSouls", out Mod fargoSouls))
            {
                return false;
            }

            return fargoSouls.Call(diff) is bool active && active;
        }
        private static bool IsWorldLegendary()
        {
            FieldInfo findInfo = typeof(Main).GetField("_currentGameModeInfo", BindingFlags.Static | BindingFlags.NonPublic);
            GameModeData data = (GameModeData)findInfo.GetValue(null);
            return Main.getGoodWorld && data.IsMasterMode;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                if (entity.ModProjectile.Name.Contains("Blood") || entity.ModProjectile.Name.Contains("BiteyBaby") || entity.ModProjectile.Name.Contains("Ripple") ||
                    (entity.ModProjectile.Name.Contains("Beholder") && !entity.ModProjectile.Name.Contains("Lava")) || entity.ModProjectile.Name.Contains("Soul") ||
                    entity.ModProjectile.Name.Contains("Void") || entity.ModProjectile.Name.Contains("Eradication") || entity.ModProjectile.Name.Contains("OldGodSpit") ||
                    entity.ModProjectile.Name.Contains("AbyssalStrike2"))
                {
                    entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                }
                if (entity.ModProjectile.Name.Contains("Lucid") || entity.ModProjectile.Name.Contains("Nightmare") || entity.ModProjectile.Name.Contains("EraserLaser") || entity.ModProjectile.Name.Contains("DyingReality") || 
                    entity.ModProjectile.Name.Contains("AbyssalApparition"))
                {
                    entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                    entity.GetGlobalProjectile<VoidDamageProjectile>().strongVoidDamge = true;
                }
            }

            if ((entity.ModProjectile.Name.Contains("Viscount") && !entity.ModProjectile.Name.Contains("Rock") && !entity.ModProjectile.Name.Contains("Stomp")) || 
                entity.ModProjectile.Name.Contains("Blizzard") || entity.ModProjectile.Name.Contains("Ice") || entity.ModProjectile.Name.Contains("Glacier") ||
                (entity.ModProjectile.Name.Contains("Beholder") && !entity.ModProjectile.Name.Contains("Beam") && !entity.ModProjectile.Name.Contains("Ray") && !entity.ModProjectile.Name.Contains("False")) ||
                entity.ModProjectile.Name.Contains("VoidEye") || !entity.ModProjectile.Name.Contains("SoulSteal"))
                entity.Calamity().DealsDefenseDamage = false;

            if (entity.ModProjectile.Name.Contains("AbyssBubble"))
                entity.light = 0.25f;

            if (entity.type == ModContent.ProjectileType<ViscountRipple>())
            {
                entity.tileCollide = false;
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.ModProjectile?.Name == "AbyssBubble" && InfernalWorld.RagnarokModeEnabled)
            {
                target.AddBuff(ModContent.BuffType<Bubbled>(), 45);
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 3 * 60);
            }
            if (projectile.type == ModContent.ProjectileType<Whirlpool>())
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 5 * 60);
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            float damageMod = 1f;
            if (projectile.ModProjectile.Name.Contains("BiteyBaby"))
            {
                damageMod += 0.25f;
            }

            if (projectile.ModProjectile.Name.Contains("Nightmare"))
            {
                damageMod += 0.5f;
            }

            if (IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                if (projectile.type == ModContent.ProjectileType<ViscountRipple>())
                    damageMod *= 1.775f;
                else if (projectile.ModProjectile.Name.Contains("Viscount") && !projectile.ModProjectile.Name.Contains("Rock") && !projectile.ModProjectile.Name.Contains("Stomp"))
                    damageMod *= 1.625f;
                else if (projectile.ModProjectile.Name.Contains("ForgottenOne") || projectile.type == ModContent.ProjectileType<AquaRipple>() || projectile.type == ModContent.ProjectileType<Whirlpool>() ||
                    projectile.ModProjectile.Name.Contains("AbyssBubble") || projectile.ModProjectile.Name.Contains("AbyssalStrike") || projectile.ModProjectile.Name.Contains("OldGodSpit"))
                    damageMod *= 1.375f;
                else if (projectile.ModProjectile.Name.Contains("Blizzard") || projectile.ModProjectile.Name.Contains("Ice") || projectile.ModProjectile.Name.Contains("Glacier") ||
                         projectile.ModProjectile.Name.Contains("Beholder") || projectile.ModProjectile.Name.Contains("VoidEye") || projectile.ModProjectile.Name.Contains("SoulSteal") ||
                         projectile.ModProjectile.Name.Contains("EradicationRay") || projectile.ModProjectile.Name.Contains("EradicationBeam") || projectile.ModProjectile.Name.Contains("AbyssalApparition") ||
                         projectile.ModProjectile.Name.Contains("DyingReality"))
                    damageMod *= 1.35f;
                else
                    damageMod *= 2.2f;
            }
            else if (GetFargoDifficullty("EternityMode"))
            {
                if (projectile.ModProjectile.Name.Contains("ForgottenOne") || projectile.type == ModContent.ProjectileType<AquaRipple>() || projectile.type == ModContent.ProjectileType<Whirlpool>() ||
                    projectile.ModProjectile.Name.Contains("AbyssBubble") || projectile.ModProjectile.Name.Contains("AbyssalStrike") || projectile.ModProjectile.Name.Contains("OldGodSpit"))
                    damageMod *= 1.275f;
                else if (projectile.ModProjectile.Name.Contains("Blizzard") || projectile.ModProjectile.Name.Contains("Ice") || projectile.ModProjectile.Name.Contains("Glacier") ||
                         projectile.ModProjectile.Name.Contains("Beholder") || projectile.ModProjectile.Name.Contains("VoidEye") || projectile.ModProjectile.Name.Contains("SoulSteal") ||
                         projectile.ModProjectile.Name.Contains("EradicationRay") || projectile.ModProjectile.Name.Contains("EradicationBeam") || projectile.ModProjectile.Name.Contains("AbyssalApparition") ||
                         (projectile.ModProjectile.Name.Contains("Viscount") && !projectile.ModProjectile.Name.Contains("Rock") && !projectile.ModProjectile.Name.Contains("Stomp")) ||
                         projectile.ModProjectile.Name.Contains("DyingReality"))
                    damageMod *= 1.25f;
                else
                    damageMod *= 1.675f;
            }
            else if (GetCalDifficulty("death"))
            {
                if (projectile.ModProjectile.Name.Contains("ForgottenOne") || projectile.type == ModContent.ProjectileType<AquaRipple>() || projectile.type == ModContent.ProjectileType<Whirlpool>() ||
                    projectile.ModProjectile.Name.Contains("AbyssBubble") || projectile.ModProjectile.Name.Contains("AbyssalStrike") || projectile.ModProjectile.Name.Contains("OldGodSpit"))
                    damageMod *= 1.15f;
                else if (projectile.ModProjectile.Name.Contains("Blizzard") || projectile.ModProjectile.Name.Contains("Ice") || projectile.ModProjectile.Name.Contains("Glacier") ||
                         projectile.ModProjectile.Name.Contains("Beholder") || projectile.ModProjectile.Name.Contains("VoidEye") || projectile.ModProjectile.Name.Contains("SoulSteal") ||
                         projectile.ModProjectile.Name.Contains("EradicationRay") || projectile.ModProjectile.Name.Contains("EradicationBeam") || projectile.ModProjectile.Name.Contains("AbyssalApparition") ||
                         (projectile.ModProjectile.Name.Contains("Viscount") && !projectile.ModProjectile.Name.Contains("Rock") && !projectile.ModProjectile.Name.Contains("Stomp")) ||
                         projectile.ModProjectile.Name.Contains("DyingReality"))
                    damageMod *= 1.1f;
                else
                    damageMod *= 1.15f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ViscountRippleGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private int spawnCollideTimer;
        private int tileCollideTimer;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ProjectileType<ViscountRipple>();
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            spawnCollideTimer = 0;
        }

        public override void PostAI(Projectile projectile)
        {
            projectile.tileCollide = false;

            if (spawnCollideTimer < 30)
                spawnCollideTimer++;
            else if (tileCollideTimer > 0)
                tileCollideTimer--;
            else if (!projectile.tileCollide)
                projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (spawnCollideTimer < 30)
                return false;

            if (tileCollideTimer > 0)
                return false;

            tileCollideTimer = 20;

            return base.OnTileCollide(projectile, oldVelocity);
        }
    }


    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class ViscountBloodAccelerationGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private static int ViscountBloodType = -1;

        /*
        private bool initialized;
        private Vector2 storedDirection;
        private float targetSpeed;
        private float timer;

        private float StartingSpeedMultiplier = 0.005f;
        private float AccelerationTime = 40f;
        */

        public override void Load()
        {
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) &&
                thorium.TryFind<ModProjectile>("ViscountBlood", out var viscountBlood))
            {
                ViscountBloodType = viscountBlood.Type;
            }
        }

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ViscountBloodType;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            projectile.tileCollide = false;
            return;
            /*
            if (Main.netMode != NetmodeID.Server)
                Main.NewText($"ViscountBlood matched: {projectile.type}");
            */

            /*
            timer = 0f;
            initialized = false;

            float originalSpeed = projectile.velocity.Length();
            if (originalSpeed <= 0.001f)
            {
                storedDirection = Vector2.UnitY;
                targetSpeed = 0f;
                return;
            }

            storedDirection = Vector2.Normalize(projectile.velocity);
            targetSpeed = originalSpeed;

            projectile.velocity = storedDirection * (targetSpeed * StartingSpeedMultiplier);
            initialized = true;
            */
        }

        public override void AI(Projectile projectile)
        {
            return;

            /*
            if (!initialized || targetSpeed <= 0f)
                return;

            timer++;

            float progress = Utils.GetLerpValue(0f, AccelerationTime, timer, true);
            progress *= progress;

            float currentSpeed = MathHelper.Lerp(
                targetSpeed * StartingSpeedMultiplier,
                targetSpeed,
                progress
            );

            projectile.velocity = storedDirection * currentSpeed;
            */
        }
    }

    [JITWhenModsEnabled("ThoriumRework")]
    [ExtendsFromMod("ThoriumRework")]
    public class LucidityPlayer : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == ModContent.BuffType<TerminalLucidity>())
            {
                CalamityPlayer mp = player.Calamity();

                if (mp.rage > 0)
                    mp.rage -= 0.05f;

                if (mp.adrenaline > 0)
                    mp.adrenaline -= 0.05f;
            }
        }
    }
}