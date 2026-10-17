using System.Linq;
using System.Reflection;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using InfernalEclipseAPI.Common.Globals.GlobalNPCs;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.World;
using InfernumMode.Core.GlobalInstances.Systems;
using MonoMod.Cil;
using Terraria.DataStructures;
using InfernumActive = InfernalEclipseAPI.Content.DifficultyOverrides.hellActive;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity
{
    [JITWhenModsEnabled("CalamityHunt")]
    [ExtendsFromMod("CalamityHunt")]
    public class CalHuntBossStatScaling : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            int[] validNPCs =
            {
                ModContent.NPCType<CrimulanGlopstrosity>(),
                ModContent.NPCType<DivineGargooptuar>(),
                ModContent.NPCType<EbonianBehemuck>(),
                ModContent.NPCType<Goozma>(),
                ModContent.NPCType<Goozmite>(),
                ModContent.NPCType<StellarGeliath>()
            };

            return validNPCs.Contains(npc.type);
        }

        public override void SetDefaults(NPC entity)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                entity.GetGlobalNPC<SOTSGlobalNPC>().canDoVoidDamage = true;
                //entity.GetGlobalNPC<SOTSGlobalNPC>().strongVoidDamge = true;
            }
        }

        private bool scaled = false;

        public override bool PreAI(NPC npc)
        {
            if (npc.type != ModContent.NPCType<Goozmite>() && !scaled)
            {
                bool flag = false;
                int num1 = 0, num2 = 0;

                object result = InfernalCrossmod.Calamity.Mod.Call("GetDifficultyActive", "BossRush");
                if (result is bool b)
                {
                    flag = b;
                    num1 = 1;
                }

                num2 = flag ? 1 : 0;
                if ((num1 & num2) != 0)
                {
                    ModNPC modNPC14 = npc.ModNPC;
                    if ((modNPC14 != null ? modNPC14.Name.Contains("Goozma") ? 1 : 0 : 0) != 0)
                    {
                        npc.lifeMax += npc.lifeMax / 4;
                    }
                }

                if (InfernumActive.InfernumActive)
                {
                    npc.lifeMax += (int)((double)1.35 * npc.lifeMax);
                }

                npc.life = npc.lifeMax;
                scaled = true;
            }

            if (InfernalWorld.RagnarokModeEnabled && npc.active && npc.type == ModContent.NPCType<EbonianBehemuck>())
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead)
                    {
                        if (player.mount.Active)
                        {
                            player.mount.Dismount(player);
                        }
                    }
                }
            }

            return base.PreAI(npc);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (InfernumActive.InfernumActive)
            {
                modifiers.SourceDamage *= 1.35f;
            }
        }

        public override void PostAI(NPC npc)
        {
            if (InfernumActive.InfernumActive)
            {
                npc.position += npc.velocity * 0.35f;
            }

            if (InfernalWorld.RagnarokModeEnabled)
            {
                if (npc.type == ModContent.NPCType<CrimulanGlopstrosity>())
                {
                    foreach (Player player in Main.player)
                    {
                        if (player.active && !player.dead && npc.WithinRange(player.Center, 5000f))
                        {
                            player.AddBuff(ModContent.BuffType<CrimulanAura>(), 1);
                        }
                    }
                    return;
                }

                if (npc.type == ModContent.NPCType<StellarGeliath>())
                {
                    foreach (Player player in Main.player)
                    {
                        if (player.active && !player.dead && npc.WithinRange(player.Center, 5000f))
                        {
                            player.AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 1);
                        }
                    }
                    return;
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.type == ModContent.NPCType<Goozma>() && InfernalWorld.RagnarokModeEnabled)
            {
                InfernalGlobalNPC.ClearRageAndAdrenaline();
            }
            return base.CheckDead(npc);
        }
    }

    [JITWhenModsEnabled("CalamityHunt")]
    [ExtendsFromMod("CalamityHunt")]
    public class HuntBossProjStatScaling : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            int[] types =
            [
                ModContent.ProjectileType<BloatedBlast>(),
                ModContent.ProjectileType<ConstellationLine>(),
                ModContent.ProjectileType<ConstellationStar>(),
                ModContent.ProjectileType<CrimulanShockwave>(),
                ModContent.ProjectileType<CrimulanSmasher>(),
                ModContent.ProjectileType<EbonianBehemuckClone>(),
                ModContent.ProjectileType<EbonstonePillar>(),
                ModContent.ProjectileType<EbonstoneTooth>(),
                ModContent.ProjectileType<FoulSlime>(),
                ModContent.ProjectileType<FusionRay>(),
                ModContent.ProjectileType<GelCrystalShard>(),
                ModContent.ProjectileType<GooLightning>(),
                ModContent.ProjectileType<HolyExplosion>(),
                ModContent.ProjectileType<InterstellarFlame>(),
                ModContent.ProjectileType<PixieBall>(),
                ModContent.ProjectileType<PrismDestroyer>(),
                ModContent.ProjectileType<RainbowBall>(),
                ModContent.ProjectileType<RainbowLaser>(),
                ModContent.ProjectileType<SlimeBomb>(),
                ModContent.ProjectileType<SlimeShot>(),
                ModContent.ProjectileType<StellarBlackHole>(),
                ModContent.ProjectileType<StellarDebris>(),
                ModContent.ProjectileType<StellarGelatine>(),
                ModContent.ProjectileType<ThrowableChunk>(),
                ModContent.ProjectileType<ToxicSludge>()
            ];

            foreach (int type in types)
            {
                if (entity.type == type)
                    return true;
            }

            return false;
        }

        private static bool IsInfernumActive()
        {
            return WorldSaveSystem.InfernumModeEnabled;
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
                entity.GetGlobalProjectile<VoidDamageProjectile>().canDoVoidDamage = true;
                //entity.GetGlobalProjectile<VoidDamageProjectile>().strongVoidDamge = true;
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            float damageMod = 1f;

            if (IsWorldLegendary())
            {
                damageMod *= 1.35f;
            }

            if (IsInfernumActive() || GetFargoDifficullty("MasochistMode"))
            {
                damageMod *= 1.35f;
            }

            modifiers.SourceDamage *= damageMod;
        }
    }

    [JITWhenModsEnabled("CalamityHunt")]
    [ExtendsFromMod("CalamityHunt")]
    public class GoozmaP2PatchSystem : ModSystem
    {
        public override void Load()
        {
            if (!ModLoader.TryGetMod("CalamityHunt", out Mod hunt))
                return;

            PatchAllP2LifeMaxReads(hunt.Code);
        }

        public static void PatchAllP2LifeMaxReads(Assembly huntAsm)
        {
            PatchAllInstanceFieldReads(
                huntAsm,
                "CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Goozma",
                "P2LifeMax"
            );
        }

        private static void PatchAllInstanceFieldReads(Assembly asm, string declaringTypeFullName, string fieldName)
        {
            Type declaringType = asm.GetType(declaringTypeFullName);
            if (declaringType == null)
                return;

            FieldInfo field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                return;

            int fieldToken = field.MetadataToken;

            foreach (Type t in asm.GetTypes())
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (m.IsAbstract || m.GetMethodBody() == null)
                        continue;
                    if (m.ContainsGenericParameters || m.IsGenericMethodDefinition)
                        continue;

                    if (!MethodReadsInstanceField(m, fieldToken))
                        continue;

                    MonoModHooks.Modify(m, il =>
                    {
                        var c = new ILCursor(il);

                        // Replace every:
                        //   ldfld int32 Goozma::P2LifeMax
                        // with:
                        //   ldfld int32 Goozma::P2LifeMax
                        //   call AdjustP2(int) -> int   (via delegate emit)
                        while (c.TryGotoNext(i => i.MatchLdfld(field)))
                        {
                            c.Index++; // move AFTER ldfld (stack: [baseP2:int])

                            c.EmitDelegate(AdjustP2LifeMax);
                        }
                    });
                }
        }

        private static int AdjustP2LifeMax(int baseP2LifeMax)
        {
            // Preserve original behavior if something is already odd.
            if (baseP2LifeMax <= 0)
                return 1;

            float mult = BossRushEvent.BossRushActive ? 3f : 2.35f;

            // Clamp so we never end up at 0 due to rounding or weirdness.
            int result = (int)Round(baseP2LifeMax * mult);
            return result < 1 ? 1 : result;
        }

        private static bool MethodReadsInstanceField(MethodInfo m, int fieldMetadataToken)
        {
            try
            {
                byte[] il = m.GetMethodBody()?.GetILAsByteArray();
                if (il == null || il.Length < 5)
                    return false;

                for (int i = 0; i <= il.Length - 5; i++)
                {
                    // ldfld (0x7B)
                    if (il[i] != 0x7B)
                        continue;

                    int token = il[i + 1] | il[i + 2] << 8 | il[i + 3] << 16 | il[i + 4] << 24;
                    if (token == fieldMetadataToken)
                        return true;
                }
            }
            catch { }

            return false;
        }
    }
}