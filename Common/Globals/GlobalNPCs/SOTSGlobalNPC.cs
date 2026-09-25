using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Boss.BrainOfCthulhu;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Content.Items.Lore.SOTS;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.Platinum;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.SOTS;
using InfernalEclipseAPI.Content.Projectiles;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems;
using InfernumMode;
using InfernumMode.Content.BehaviorOverrides.BossAIs.BoC;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Deerclops;
using InfernumMode.Content.BehaviorOverrides.BossAIs.HiveMind;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Perforators;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SupremeCalamitas;
using InfernumMode.Core.GlobalInstances.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SOTS;
using SOTS.Buffs;
using SOTS.Buffs.Debuffs;
using SOTS.Common.GlobalNPCs;
using SOTS.Helpers;
using SOTS.Items.ChestItems;
using SOTS.Items.Fragments;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Advisor;
using SOTS.NPCs.Boss.Glowmoth;
using SOTS.NPCs.Boss.Lux;
using SOTS.NPCs.Boss.Polaris;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using SOTS.NPCs.Town;
using SOTS.NPCs.TreasureSlimes;
using SOTS.Projectiles.Permafrost;
using SOTS.Void;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod.NPCs.BossGraniteEnergyStorm;

namespace InfernalEclipseAPI.Common.Globals.GlobalNPCs
{
    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class SOTSGlobalNPC : GlobalNPC
    {
        private static readonly List<(Projectile Projectile, EvilGrowth Growth)> ActiveEvilGrowths = new();

        private static ulong lastEvilGrowthCacheUpdate;

        private static HashSet<int> EvilGrowthPullImmuneNPCs;

        private static Asset<Texture2D> EvilArmTexture;
        private static Asset<Texture2D> EvilHandTexture;
        private const int MaxEvilArmSegments = 80;
        private const float MaxEvilArmDrawDistance = 2400f;

        public bool canDoVoidDamage = false;
        public bool strongVoidDamge = false;
        public bool isFlowered;

        private const int MaxAnomalyCurseStacks = 30;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            EvilGrowthPullImmuneNPCs = new HashSet<int>
            {
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailLarge>(),
                ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<DarkHeart>(),
                ModContent.NPCType<GiantClam>(),
                ModContent.NPCType<LightSnuffingHand>(),
                ModContent.NPCType<PutridPinky1>(),
                NPCID.DungeonGuardian,
                ModContent.NPCType<CrimulanPaladin>(),
                ModContent.NPCType<EbonianPaladin>(),
                ModContent.NPCType<SlimeGodCore>(),
                ModContent.NPCType<RavagerHead>(),
                ModContent.NPCType<ProfanedGuardianCommander>(),
                ModContent.NPCType<ProfanedGuardianDefender>(),
                ModContent.NPCType<ProfanedGuardianHealer>(),
                NPCID.DD2Betsy,
                ModContent.NPCType<HorribleHog>()
            };

            EvilArmTexture = ModContent.Request<Texture2D>("SOTS/Projectiles/Evil/EvilArm");
            EvilHandTexture = ModContent.Request<Texture2D>("SOTS/Projectiles/Evil/EvilHand");
        }

        public override void Unload()
        {
            ActiveEvilGrowths.Clear();
            EvilGrowthPullImmuneNPCs = null;
            EvilArmTexture = null;
            EvilHandTexture = null;
        }

        private static List<(Projectile Projectile, EvilGrowth Growth)> GetActiveEvilGrowths()
        {
            if (lastEvilGrowthCacheUpdate == Main.GameUpdateCount)
                return ActiveEvilGrowths;

            lastEvilGrowthCacheUpdate = Main.GameUpdateCount;
            ActiveEvilGrowths.Clear();

            int evilGrowthType = ModContent.ProjectileType<EvilGrowth>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (!projectile.active || projectile.timeLeft >= 8998)
                    continue;

                if (projectile.type != evilGrowthType)
                    continue;

                if (projectile.ModProjectile is EvilGrowth growth)
                    ActiveEvilGrowths.Add((projectile, growth));
            }

            return ActiveEvilGrowths;
        }

        public override void SetDefaults(NPC entity)
        {
            int[] curseImmune =
            {
                NPCID.WallofFlesh,
                NPCID.WallofFleshEye,
                NPCID.Retinazer,
                NPCID.Spazmatism,
                NPCID.SkeletronPrime,
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeSaw,
                NPCID.PrimeVice,
                ModContent.NPCType<CrimulanPaladin>(),
                ModContent.NPCType<EbonianPaladin>(),
                ModContent.NPCType<SlimeGodCore>(),
                NPCID.TheDestroyer,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCID.Probe

            };

            int[] dealsVoidDamge =
            {
                ModContent.NPCType<PerforatorHive>(),
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailLarge>(),
                ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<HiveMind>(),
                ModContent.NPCType<DarkHeart>(),
                ModContent.NPCType<HiveBlob>(),
                ModContent.NPCType<CrimulanPaladin>(),
                ModContent.NPCType<EbonianPaladin>(),
                ModContent.NPCType<SlimeGodCore>(),
            };

            if (curseImmune.Contains(entity.type) && WorldSaveSystem.InfernumModeEnabled)
            {
                entity.buffImmune[ModContent.BuffType<CurseVision>()] = true;
                entity.buffImmune[ModContent.BuffType<PharaohsCurse>()] = true;
            }

            if (dealsVoidDamge.Contains(entity.type))
            {
                canDoVoidDamage = true;
            }

            if (entity.type == NPCID.Golem || entity.type == NPCID.GolemHead || entity.type == NPCID.GolemHeadFree)
            {
                entity.buffImmune[ModContent.BuffType<Assassination>()] = true;
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (CurseImmuneThoriumBosses.curseImmune.Contains(entity.type) && WorldSaveSystem.InfernumModeEnabled)
                {
                    entity.buffImmune[ModContent.BuffType<CurseVision>()] = true;
                    entity.buffImmune[ModContent.BuffType<PharaohsCurse>()] = true;
                }
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.boss)
            {
                foreach (Player player in Main.player)
                {
                    if (player.active && !player.dead)
                    {
                        player.ClearBuff(ModContent.BuffType<Embattle>());
                    }

                    //Shatter Shard clear on boss spawn
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (!proj.active)
                            continue;

                        if (proj.type == ModContent.ProjectileType<ShatterShard>())
                        {
                            proj.Kill();
                        }
                    }
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (InfernalConfig.Instance.SOTSBalanceChanges || !npc.active) return base.PreAI(npc);

            if (npc.type == ModContent.NPCType<MutagenTreasureSlime>() && !NPC.downedBoss2)
                npc.active = false;

            if (npc.type != ModContent.NPCType<SubspaceSerpentHead>() && npc.type != ModContent.NPCType<Lux>()) return base.PreAI(npc);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.dead || !player.active || !npc.WithinRange(player.Center, 10000f))
                    continue;

                if (player.mount?.Type == ModContent.MountType<DraedonGamerChairMount>())
                    player.mount.Dismount(player);
                if (InfernalCrossmod.Clamity.Loaded)
                {
                    if (player.mount?.Type == InfernalCrossmod.Clamity.Mod.Find<ModMount>("PlagueChairMount").Type)
                        player.mount.Dismount(player);
                }

                player.DoInfiniteFlightCheck(Color.LimeGreen);
            }

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (!npc.active)
                return;

            DebuffNPC debuffNPC = npc.GetGlobalNPC<DebuffNPC>();

            if (InfernalCrossmod.NoxusBoss.Loaded)
            {
                if (npc.type == InfernalCrossmod.NoxusBoss.Mod.Find<ModNPC>("NamelessDeityBoss").Type)
                {
                    MakeSOTSCurseImmune(npc);
                }
            }

            if (npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsTail>())
            {
                MakeSOTSCurseImmune(npc);
            }

            if (debuffNPC.AnomalyCurse > MaxAnomalyCurseStacks)
                debuffNPC.AnomalyCurse = MaxAnomalyCurseStacks;

            if (npc.type == NPCID.SkeletronHead || npc.type == NPCID.SkeletronHand ||
                npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeLaser || npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeVice ||
                npc.type == NPCID.WallofFlesh || npc.type == NPCID.WallofFleshEye)
                debuffNPC.BleedingCurse = 0;

            if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];

                    if (player.active &&
                        !player.dead &&
                        npc.Distance(player.Center) < 12000f &&
                        !BossRushEvent.BossRushActive &&
                        player.GetModPlayer<InfernalPlayer>().teleportRespawnKilldown <= 0)
                    {
                        player.AddBuff(ModContent.BuffType<StarboundHorrification>(), 60);
                    }
                }
            }

            // Gives polaris the cryogen shader
            if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
            {
                foreach (Player p in Main.ActivePlayers)
                {
                    p.Calamity().monolithCryogenShader = 30;
                }
            }

            if (npc.immortal || npc.realLife != -1)
                return;

            float num8 = 0f;
            bool flag3 = false;

            foreach ((Projectile projectile, EvilGrowth growth) in GetActiveEvilGrowths())
            {
                bool flag5 = growth.effected[npc.whoAmI];
                bool flag6 = false;
                int num14 = -1;

                if (!flag5)
                    continue;

                flag3 = true;

                if (num8 <= 1f)
                {
                    SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(Main.player[projectile.owner]);

                    if (sotsPlayer.halfLifeRegen < 3)
                        sotsPlayer.halfLifeRegen += 3;

                    sotsPlayer.halfLifeRegen++;

                    if (npc.boss)
                        sotsPlayer.halfLifeRegen++;
                }

                if (num14 == npc.whoAmI)
                {
                    if (flag6)
                        num8++;
                }
                else
                {
                    float num15 = 0.5f;
                    float num16 = 0.025f;

                    if (!flag6 && !npc.boss)
                        num16 = 0.04f;

                    Vector2 vector2_23 = new Vector2(projectile.Center.X, projectile.position.Y - 8f) - new Vector2(npc.Center.X, npc.position.Y + npc.height);

                    float num19 = vector2_23.Length() * num16 * (npc.boss ? 0.01f : 1f);

                    if (!flag6)
                        num15 = 0.65f;

                    Vector2 vector2_25 = vector2_23.SafeNormalize(Vector2.Zero) * (num15 + num19);

                    if (!npc.boss && npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail && !EvilGrowthPullImmuneNPCs.Contains(npc.type))
                    {
                        npc.position += vector2_25;
                    }
                }
            }

            isFlowered = num8 >= 1f;

            float num26 = 1f;

            if (flag3)
            {
                if (!npc.boss && npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail && !EvilGrowthPullImmuneNPCs.Contains(npc.type))
                {
                    num26 *= 0.2f;
                }
                else
                {
                    num26 *= 0.875f;
                }
            }

            npc.position -= npc.velocity * (1f - num26);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (isFlowered)
                npc.lifeRegen -= 8;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo info)
        {
            if (canDoVoidDamage)
            {
                int damage = 1 + npc.damage / (strongVoidDamge ? 3 : 6);
                VoidPlayer.VoidDamage(Mod, target, damage);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            NerfDendroChain(npc, ref modifiers);
            NerfBlazingCurse(npc, ref modifiers);
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            NerfDendroChain(npc, ref modifiers);
            NerfBlazingCurse(npc, ref modifiers);
        }

        private static void MakeSOTSCurseImmune(NPC npc)
        {
            DebuffNPC debuffNPC = npc.GetGlobalNPC<DebuffNPC>();

            debuffNPC.PlatinumCurse = 0;
            debuffNPC.HarvestCurse = 0;
            debuffNPC.DestableCurse = 0;
            debuffNPC.BlazingCurse = 0;
            debuffNPC.AnomalyCurse = 0;
            debuffNPC.BlightCurse = 0;
            debuffNPC.CrystalCurse = 0;
            debuffNPC.DamageCurse = 0;
            debuffNPC.VoidspaceCurse = 0;
        }

        private static void NerfBlazingCurse(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges) return;

            DebuffNPC debuffNPC = npc.GetGlobalNPC<DebuffNPC>();

            if (debuffNPC.BlazingCurse > 0)
            {
                float orig = 1f + 0.03f * debuffNPC.BlazingCurse + 0.005f * debuffNPC.BlazingCurse;
                float nerfed = 1f + 0.01f * debuffNPC.BlazingCurse + 0.005f * debuffNPC.BlazingCurse;

                modifiers.SourceDamage /= orig;
                modifiers.SourceDamage *= nerfed;
            }
        }

        private static void NerfDendroChain(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges || npc.immortal) return;

            if (!npc.HasBuff(ModContent.BuffType<Shattered>()))
            {
                if (npc.HasBuff<DendroChain>()) 
                {
                    modifiers.Defense.Flat += 15;
                }
            }
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type == ModContent.NPCType<Archaeologist>() && !NPC.downedDeerclops)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != null && !items[i].IsAir &&
                        items[i].type == ModContent.ItemType<SOTS.Items.ChestItems.GlazeBow>())
                    {
                        items[i].TurnToAir();
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<CrimsonTreasureSlime>() || npc.type == ModContent.NPCType<CorruptionTreasureSlime>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlightedGel>(), 1, 7, 13));
            }

            if (npc.type == ModContent.NPCType<Cryogen>())
            {
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<FragmentOfPermafrost>(), 1, 12, 18));
            }

            #region Lore Items
            if (npc.type == ModContent.NPCType<TheAdvisorHead>())
            {
                bool firstAdvisorKill() => !SOTS.SOTSWorld.downedAdvisor;
                npcLoot.AddConditionalPerPlayer(firstAdvisorKill, ModContent.ItemType<LoreAdvisor>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<Glowmoth>())
            {
                bool firstGlowmothKill() => !SOTS.SOTSWorld.downedGlowmoth;
                npcLoot.AddConditionalPerPlayer(firstGlowmothKill, ModContent.ItemType<LoreGlowmoth>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<Lux>())
            {
                bool firstLuxKill() => !SOTS.SOTSWorld.downedLux;
                npcLoot.AddConditionalPerPlayer(firstLuxKill, ModContent.ItemType<LoreLux>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<SOTS.NPCs.Boss.Curse.PharaohsCurse>())
            {
                bool firstCurseKill() => !SOTS.SOTSWorld.downedCurse;
                npcLoot.AddConditionalPerPlayer(firstCurseKill, ModContent.ItemType<LorePharaoh>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
            {
                bool firstPolarisKill() => !SOTS.SOTSWorld.downedAmalgamation;
                npcLoot.AddConditionalPerPlayer(firstPolarisKill, ModContent.ItemType<LorePolaris>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
            {
                bool firstPutridKill() => !SOTS.SOTSWorld.downedPinky;
                npcLoot.AddConditionalPerPlayer(firstPutridKill, ModContent.ItemType<LorePutrid>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
            {
                bool firstSupspaceKill() => !SOTS.SOTSWorld.downedSubspace;
                npcLoot.AddConditionalPerPlayer(firstSupspaceKill, ModContent.ItemType<LoreSerpent>(), desc: DropHelper.FirstKillText);
            }
            #endregion

            #region Infernal Relics
            static bool isInfernum() => WorldSaveSystem.InfernumModeEnabled;
            Mod sots = ModLoader.GetMod("SOTS");
            if (npc.type == ModContent.NPCType<Glowmoth>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<GlowmothRelic>());
            }
            if (npc.type == ModContent.NPCType<SOTS.NPCs.Boss.Curse.PharaohsCurse>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PharohsCurseRelic>());
            }
            if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PutridPinkyRelic>());
            }
            if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PutridPinkyRelic>());
            }
            if (npc.type == sots.Find<ModNPC>("Excavator").Type)
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<ExcavatorRelic>());
            }
            if (npc.type == ModContent.NPCType<TheAdvisorHead>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<AdvisorRelic>());
            }
            if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PolarisRelic>());
            }
            if (npc.type == ModContent.NPCType<Lux>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<LuxRelic>());
            }
            if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<SubspaceSerpentRelic>());
                npcLoot.AddIf(() => !NPC.downedMoonlord, ModContent.ItemType<SubspaceSerpentRelicPlatinum>());
            }
            #endregion
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            foreach ((Projectile projectile, EvilGrowth growth) in GetActiveEvilGrowths())
            {
                if (!growth.effected[npc.whoAmI] || npc.realLife != -1)
                    continue;

                Texture2D armTexture = EvilArmTexture.Value;
                Texture2D handTexture = EvilHandTexture.Value;

                float drawScale = projectile.scale * (projectile.timeLeft / 150f);

                if (drawScale <= 0.001f)
                    continue;

                Vector2 toNpc = npc.Center - projectile.Center;
                float distance = toNpc.Length();

                if (distance > MaxEvilArmDrawDistance)
                    distance = MaxEvilArmDrawDistance;

                int segmentCount = Math.Clamp(
                    (int)(distance / Math.Max(1f, armTexture.Width * drawScale)),
                    1,
                    MaxEvilArmSegments
                );

                Vector2 clampedToNpc = toNpc.SafeNormalize(Vector2.UnitY) * distance;
                float rotation = toNpc.ToRotation();

                for (int j = 0; j < segmentCount; j++)
                {
                    float completion = j / (float)segmentCount;
                    Vector2 drawPos = npc.Center - clampedToNpc * completion - screenPos;

                    if (j == 0)
                    {
                        spriteBatch.Draw(
                            handTexture,
                            drawPos,
                            null,
                            ColorHelper.EvilColor,
                            rotation + PiOver2,
                            handTexture.Size() * 0.5f,
                            drawScale * 1.4f,
                            SpriteEffects.None,
                            0f
                        );
                    }
                    else
                    {
                        spriteBatch.Draw(
                            armTexture,
                            drawPos,
                            null,
                            ColorHelper.EvilColor,
                            rotation,
                            armTexture.Size() * 0.5f,
                            drawScale,
                            SpriteEffects.None,
                            0f
                        );
                    }
                }
            }

            return true;
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class VoidDamageProjectile : GlobalProjectile
    {
        public bool canDoVoidDamage = false;
        public bool strongVoidDamge = false;
        public bool strongerVoidDamage = false;

        public override bool InstancePerEntity => true;

        public override void SetDefaults(Projectile entity)
        {
            int[] dealsVoidDamge =
            [
                //Perforators
                ModContent.ProjectileType<IchorShot>(),
                ModContent.ProjectileType<IchorBlob>(),
                ModContent.ProjectileType<IchorBlast>(),
                ModContent.ProjectileType<IchorBolt>(),
                ModContent.ProjectileType<IchorShower>(),
                ModContent.ProjectileType<IchorSpit>(),
                ModContent.ProjectileType<Crimera>(),

                //Hive Mind
                ModContent.ProjectileType<ShaderainHostile>(),
                ModContent.ProjectileType<VileClot>(),
                ModContent.ProjectileType<EaterOfSouls>(),
                ModContent.ProjectileType<ShadeFire>(),

                //Slime God
                ModContent.ProjectileType<DeceleratingCrimulanGlob>(),
                ModContent.ProjectileType<GroundSlimeGlob>(),
                ModContent.ProjectileType<DeceleratingEbonianGlob>(),

                //SCal
                ModContent.ProjectileType<SupremeCataclysmFist>(),
                ModContent.ProjectileType<SupremeCatastropheSlash>(),
                ModContent.ProjectileType<SupremeCataclysmFistOld>(),
                ModContent.ProjectileType<CatastropheSlash>()
            ];

            foreach (int type in dealsVoidDamge)
            {
                if (entity.type == type)
                    canDoVoidDamage = true;

                if (entity.type == ModContent.ProjectileType<SupremeCataclysmFist>() || entity.type == ModContent.ProjectileType<SupremeCatastropheSlash>() || entity.type == ModContent.ProjectileType<SupremeCataclysmFistOld>() || entity.type == ModContent.ProjectileType<CatastropheSlash>())
                    strongerVoidDamage = true;
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (canDoVoidDamage)
            {
                int damage = 1 + projectile.damage / (strongerVoidDamage ? 2 : strongVoidDamge ? 3 : 6);
                VoidPlayer.VoidDamage(Mod, target, damage);
            }
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public static class CurseImmuneThoriumBosses
    {
        public static int[] curseImmune =
        {
            ModContent.NPCType<GraniteEnergyStorm>()
        };
    }
}