using System.Collections.Generic;
using System.Reflection;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Enums;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.UI.DraedonSummoning;
using InfernalEclipseAPI.Content.Projectiles;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.World;
using InfernumMode.Content.BehaviorOverrides.BossAIs.AstrumDeus;
using InfernumMode.Content.BehaviorOverrides.BossAIs.BoC;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Deerclops;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Draedon.Ares;
using InfernumMode.Content.BehaviorOverrides.BossAIs.DukeFishron;
using InfernumMode.Content.BehaviorOverrides.BossAIs.EyeOfCthulhu;
using InfernumMode.Content.BehaviorOverrides.BossAIs.GreatSandShark;
using InfernumMode.Content.BehaviorOverrides.BossAIs.KingSlime;
using InfernumMode.Content.BehaviorOverrides.BossAIs.PlaguebringerGoliath;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Polterghast;
using InfernumMode.Content.BehaviorOverrides.BossAIs.ProfanedGuardians;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SupremeCalamitas;
using InfernumMode.Core.GlobalInstances.Systems;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria.Audio;
using Terraria.DataStructures;
using static CalamityMod.Events.BossRushEvent;
using static Terraria.ModLoader.ModContent;

namespace InfernalEclipseAPI.Core.Systems.BossRush
{
    public class InfernalBossRush : ModSystem
    {
        private Hook handleTeleportsHook;

        public override void Load()
        {
            MethodInfo target = typeof(BossRushChangesSystem).GetMethod("HandleTeleports", BindingFlags.Public | BindingFlags.Static);

            MethodInfo replacement = typeof(InfernalBossRush).GetMethod(nameof(HandleTeleports_NoOp), BindingFlags.NonPublic | BindingFlags.Static);

            if (target != null && replacement != null)
                handleTeleportsHook = new Hook(target, replacement);
        }

        public override void Unload()
        {
            handleTeleportsHook?.Dispose();
            handleTeleportsHook = null;
        }

        // Signature matches MonoMod Hook pattern for a static void method with no parameters.
        private static void HandleTeleports_NoOp(Action orig)
        {
            // Do nothing, effectively disabling the original method.
        }

        public override void PostSetupContent()
        {
            Bosses = [];

            // Tier 1
            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                Bosses.Add(new Boss(SoulsNPC("TrojanSquirrel"), spawnContext: type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                    if (!InfernalConfig.Instance.ForceFullXerocDialogue)
                        DownedBossSystem.startedBossRushAtLeastOnce = true;
                }, permittedNPCs: [SoulsNPC("TrojanSquirrelArms"), SoulsNPC("TrojanSquirrelHead")]));

                if (InfernalCrossmod.Thorium.Loaded && (!InfernalCrossmod.ThoriumRework.Loaded || InfernalCrossmod.ThoriumRework.Mod?.Version > Version.Parse("1.6.1")))
                {
                    Bosses.Add(new Boss(ThoriumNPC("TheGrandThunderBird"), TimeChangeContext.Day, permittedNPCs: new int[] { ThoriumNPC("StormHatchling"), ThoriumNPC("TheGrandThunderBird") }));
                }

                Bosses.Add(new Boss(NPCID.KingSlime,
                    permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime, NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime,
                                                NPCType<KingSlimeJewelRuby>(), NPCType<Ninja>() }));
            }
            else if (InfernalCrossmod.Thorium.Loaded && (!InfernalCrossmod.ThoriumRework.Loaded || InfernalCrossmod.ThoriumRework.Mod?.Version > Version.Parse("1.6.1")))
            {
                Bosses.Add(new Boss(ThoriumNPC("TheGrandThunderBird"), TimeChangeContext.Day, spawnContext: type => 
                { 
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type); 
                
                    if (!InfernalConfig.Instance.ForceFullXerocDialogue)
                        DownedBossSystem.startedBossRushAtLeastOnce = true;
                }, permittedNPCs: new int[]
                    { ThoriumNPC("StormHatchling"), ThoriumNPC("TheGrandThunderBird") }));

                Bosses.Add(new Boss(NPCID.KingSlime, 
                    permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime, NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime,
                                                NPCType<KingSlimeJewelRuby>(), NPCType<Ninja>() }));
            }
            else
            {
                Bosses.Add(new Boss(NPCID.KingSlime, spawnContext: type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                    if (!InfernalConfig.Instance.ForceFullXerocDialogue)
                        DownedBossSystem.startedBossRushAtLeastOnce = true;
                }, permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime, NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime,
                                                NPCType<KingSlimeJewelRuby>(), NPCType<Ninja>() }));
            }

            if (ModLoader.TryGetMod("CalamityAddon", out Mod warMachine))
            {
                Bosses.Add(new Boss(warMachine.Find<ModNPC>("WulfrumMothership").Type, TimeChangeContext.Day,
                    permittedNPCs: new int[] { NPCType<WulfrumDrone>(), NPCType<WulfrumHovercraft>(), NPCType<WulfrumRover>(), NPCType<WulfrumGyrator>(),
                        warMachine.Find<ModNPC>("WulfrumTank").Type, warMachine.Find<ModNPC>("WulfrumWormBody").Type, warMachine.Find<ModNPC>("WulfrumWormHead").Type, warMachine.Find<ModNPC>("WulfrumWormTail").Type}));
            }

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                Bosses.Add(new Boss(ConsolariaNPC("Lepus"), permittedNPCs: new int[] { ConsolariaNPC("DisasterBunny") }));
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("QueenJellyfish")));
            }

            Bosses.Add(new Boss(NPCID.EyeofCthulhu, TimeChangeContext.Night, permittedNPCs: [NPCID.ServantofCthulhu, NPCType<ExplodingServant>()]));

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                //Cursed Coffin
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("Viscount"), specialSpawnCountdown: 0, permittedNPCs: new int[] { ThoriumNPC("BiteyBaby") }));
            }

            Bosses.Add(new Boss(NPCID.EaterofWorldsHead, permittedNPCs: [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.VileSpit]));

            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("GoblinChariot"), permittedNPCs: [HomewardNPC("GoblinChariot"), HomewardNPC("Bombie")]));
            }

            Bosses.Add(new Boss(NPCID.WallofFlesh, spawnContext: type =>
            {
                BossRushTeleports.HellTeleportAll();
                Player player = Main.player[ClosestPlayerToWorldCenter];
                NPC.SpawnWOF(player.position);
            }, permittedNPCs: [NPCID.WallofFleshEye, NPCID.LeechHead, NPCID.LeechBody, NPCID.LeechTail, NPCID.TheHungry, NPCID.TheHungryII]));

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("BoreanStrider"), TimeChangeContext.Night,
                permittedNPCs: new int[] { ThoriumNPC("BoreanStrider"), ThoriumNPC("BoreanStriderPopped"), ThoriumNPC("BoreanHopper"), ThoriumNPC("BoreanMyte") }));
            }

            Bosses.Add(new Boss(NPCID.BloodNautilus, TimeChangeContext.Night, permittedNPCs: new int[] { NPCID.EyeballFlyingFish, NPCID.VampireBat }));

            Bosses.Add(new Boss(ModContent.NPCType<PerforatorHive>(), permittedNPCs: [ ModContent.NPCType<PerforatorHeadLarge>(), ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorTailLarge>(),
                    ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>(), ModContent.NPCType<PerforatorHeadSmall>(),
                    ModContent.NPCType<PerforatorBodySmall>() ,ModContent.NPCType<PerforatorTailSmall>() ]));

            Bosses.Add(new Boss(NPCID.QueenBee, permittedNPCs: [NPCID.HornetFatty, NPCID.HornetHoney, NPCID.HornetStingy]));

            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("BigDipper"), permittedNPCs: [HomewardNPC("NodePointer")]));
            }

            Bosses.Add(new Boss(NPCID.QueenSlimeBoss));

            Bosses.Add(new Boss(ModContent.NPCType<AstrumAureus>(), TimeChangeContext.Night, permittedNPCs: ModContent.NPCType<AureusSpawn>()));

            Bosses.Add(new Boss(ModContent.NPCType<Crabulon>(), spawnContext: type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];
                int thePefectOne = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                Main.npc[thePefectOne].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(thePefectOne);
            }, specialSpawnCountdown: 300, permittedNPCs: [ModContent.NPCType<CrabShroom>()]));

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                Bosses.Add(new Boss(ConsolariaNPC("TurkortheUngrateful"), permittedNPCs: new int[] { ConsolariaNPC("TurkortheUngratefulHead"), ConsolariaNPC("TurkorNeck") }));
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("Glowmoth"), permittedNPCs: new int[] { SOTSNPC("GlowmothMinion")}));
            }

            Bosses.Add(new Boss(ModContent.NPCType<AquaticScourgeHead>(), permittedNPCs: [ ModContent.NPCType<AquaticScourgeBody>(), ModContent.NPCType<AquaticScourgeBodyAlt>(),
                    ModContent.NPCType<AquaticScourgeTail>() ]));

            Bosses.Add(new Boss(ModContent.NPCType<DesertScourgeHead>(), spawnContext: type =>
            {
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertScourgeHead>());
            }, permittedNPCs: [ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>()]));

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("PutridPinkyPhase2"), permittedNPCs: new int[] { SOTSNPC("PutridHook") }));
            }

            Bosses.Add(new Boss(ModContent.NPCType<ProfanedGuardianCommander>(), TimeChangeContext.Day, type =>
            {
                BossRushTeleports.GaurdiansTeleportAll();
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            }, permittedNPCs: [ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.NPCType<EtherealHand>(), ModContent.NPCType<HealerShieldCrystal>()]));

            // Tier 2
            Bosses.Add(new Boss(ModContent.NPCType<StormWeaverHead>(), TimeChangeContext.Day, permittedNPCs: [ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>(),]));

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("FallenBeholder"), permittedNPCs: new int[] { ThoriumNPC("Beholder"), ThoriumNPC("FallenBeholder"), ThoriumNPC("FallenBeholder2") }));
            }

            Bosses.Add(new Boss(ModContent.NPCType<BrimstoneElemental>(), permittedNPCs: ModContent.NPCType<Brimling>()));

            Bosses.Add(new Boss(ModContent.NPCType<Anahita>(), TimeChangeContext.Day, permittedNPCs: [ ModContent.NPCType<Leviathan>(),
                    ModContent.NPCType<AnahitasIceShield>(), NPCID.DetonatingBubble, ModContent.NPCType<RedirectingBubble>() ]));

            if (InfernalCrossmod.Clamity.Loaded)
            {
                Bosses.Add(new Boss(ClamityNPC("PyrogenBoss"), permittedNPCs: new int[] { ClamityNPC("PyrogenShield") }));
            }

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                //Banished Baron
            }

            Bosses.Add(new Boss(ModContent.NPCType<RavagerBody>(), spawnContext: type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];

                SoundEngine.PlaySound(SoundID.ScaryScream, player.Center);
                int ravager = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                Main.npc[ravager].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(ravager);
            }, usesSpecialSound: true, permittedNPCs: [ ModContent.NPCType<FlamePillar>(), ModContent.NPCType<RockPillar>(), ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>(),
                    ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>() ]));

            Bosses.Add(new Boss(ModContent.NPCType<HiveMind>(), spawnContext: type =>
            {
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            }, permittedNPCs: [ModContent.NPCType<DankCreeper>(), ModContent.NPCType<DarkHeart>(), ModContent.NPCType<HiveBlob>()]));

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("Excavator"), TimeChangeContext.Night, permittedNPCs: new int[] { SOTSNPC("ExcavatorBody"), SOTSNPC("ExcavatorBody2"), SOTSNPC("ExcavatorTail"), SOTSNPC("ExcavatorDrillTail") }));
            }

            Bosses.Add(new Boss(NPCID.DukeFishron, spawnContext: type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];
                int dukeFishron = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                Main.npc[dukeFishron].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(dukeFishron);
            }, permittedNPCs: [NPCID.DetonatingBubble, NPCID.Sharkron, NPCID.Sharkron2, ModContent.NPCType<RedirectingBubble>()]));

            Bosses.Add(new Boss(ModContent.NPCType<Cryogen>()));

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                Bosses.Add(new Boss(ConsolariaNPC("Ocram"), TimeChangeContext.Night, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                    Main.bloodMoon = true;
                }, permittedNPCs: new int[] { ConsolariaNPC("ServantofOcram") }));
            }

            Bosses.Add(new Boss(NPCID.BrainofCthulhu, permittedNPCs: [NPCID.Creeper, ModContent.NPCType<BrainIllusion>()]));

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("StarScouter"), permittedNPCs: [ ThoriumNPC("CryoCore"), ThoriumNPC("BioCore"), ThoriumNPC("PyroCore") ]));
            }

            Bosses.Add(new Boss(NPCID.Deerclops, permittedNPCs: [ModContent.NPCType<LightSnuffingHand>()]));

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("PharaohsCurse"), spawnContext: type =>
                {
                    BossRushTeleports.PyramidTeleportAll();

                    byte closest = Player.FindClosest(new Vector2(Main.maxTilesX * 8f, Main.maxTilesY * 8f), 0, 0);
                    NPC.SpawnOnPlayer(closest, type);
                    Main.npc[NPC.FindFirstNPC(type)].Center = Main.player[closest].Center;
                }, permittedNPCs: [SOTSNPC("SmallGas")]));
            }

            if (InfernalCrossmod.Clamity.Loaded)
            {
                Bosses.Add(new Boss(ClamityNPC("ClamitasBoss")));
            }

            Bosses.Add(new Boss(ModContent.NPCType<Signus>(), specialSpawnCountdown: 360));

            Bosses.Add(new Boss(ModContent.NPCType<Dragonfolly>(), TimeChangeContext.Day, permittedNPCs: [ModContent.NPCType<DraconicSwarmer>(), NPCID.Spazmatism, NPCID.Retinazer]));

            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("TheLifebringerHead"), permittedNPCs: [HomewardNPC("TheLifebringerBody"), HomewardNPC("TheLifebringerTail"), HomewardNPC("PillarOfMercy"), HomewardNPC("PillarOfMildness"), HomewardNPC("PillarOfSeverity"), HomewardNPC("Sephirah"),
                                                                                        HomewardNPC("SuperProbe"), HomewardNPC("TheLifebringer_Minion")]));

                Bosses.Add(new Boss(HomewardNPC("TheMaterealizer"), permittedNPCs: [HomewardNPC("TheMaterealizer_Minion")]));

                //Ordeals
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("TheAdvisorHead"), spawnContext: type =>
                {
                    byte closest = Player.FindClosest(new Vector2(Main.maxTilesX * 8f, Main.maxTilesY * 8f), 0, 0);
                    if (!NPC.AnyNPCs(type))
                        NPC.SpawnOnPlayer(closest, type);
                    Main.npc[NPC.FindFirstNPC(type)].Center = Main.player[closest].Center;
                }));
            }

            Bosses.Add(new Boss(ModContent.NPCType<SlimeGodCore>(), permittedNPCs: [ ModContent.NPCType<SlimeGodCore>(), ModContent.NPCType<EbonianPaladin>(), ModContent.NPCType<CrimulanPaladin>(), ModContent.NPCType<SplitCrimulanPaladin>(),
                    ModContent.NPCType<SplitEbonianPaladin>(), ModContent.NPCType<SplitBigSlime>() ]));

            Bosses.Add(new Boss(NPCID.SkeletronHead, TimeChangeContext.Night, type =>
            {
                BossRushTeleports.BringPlayersBackToSpawn();

                Player player = Main.player[ClosestPlayerToWorldCenter];
                int sans = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                Main.npc[sans].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(sans);
            }, permittedNPCs: NPCID.SkeletronHand));

            // Tier 3
            if (HomewardLoaded())
            {
                //Puppet Opera

                Bosses.Add(new Boss(HomewardNPC("MarquisMoonsquid"), permittedNPCs: [HomewardNPC("MarquisMoonsquid_Minion")]));
            }

            if (InfernalCrossmod.Thorium.Loaded && (!InfernalCrossmod.ThoriumRework.Loaded || InfernalCrossmod.ThoriumRework.Mod?.Version > Version.Parse("1.6.1")))
            {
                Bosses.Add(new Boss(ThoriumNPC("BuriedChampion"), permittedNPCs: [ThoriumNPC("FallenChampion2")]));

                Bosses.Add(new Boss(ThoriumNPC("GraniteEnergyStorm"), permittedNPCs: [ThoriumNPC("CoalescedEnergy"), ThoriumNPC("EncroachingEnergy"), ThoriumNPC("EnergyConduit")]));

                Bosses.Add(new Boss(ThoriumNPC("Lich"), TimeChangeContext.Night, permittedNPCs: [ThoriumNPC("LichHeadless")]));
            }

            Bosses.Add(new Boss(NPCID.Plantera, permittedNPCs: [NPCID.PlanterasTentacle, NPCID.PlanterasHook, NPCID.Spore]));

            Bosses.Add(new Boss(NPCID.TheDestroyer, TimeChangeContext.Night, specialSpawnCountdown: 300, permittedNPCs: [NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Probe]));

            Bosses.Add(new Boss(ModContent.NPCType<PlaguebringerGoliath>(), permittedNPCs: [ ModContent.NPCType<BuilderDroneSmall>(), ModContent.NPCType<BuilderDroneBig>(), ModContent.NPCType<SmallDrone>(),
                    ModContent.NPCType<PlagueMine>(), ModContent.NPCType<ExplosivePlagueCharger>() ]));

            Bosses.Add(new Boss(ModContent.NPCType<AstrumDeusHead>(), TimeChangeContext.Night, type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];

                SoundEngine.PlaySound(AstrumDeusHead.SpawnSound, player.Center);
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>(), ModContent.NPCType<DeusSpawn>()]));

            // Filler. 
            Bosses.Add(new Boss(NPCID.CultistBoss));

            Bosses.Add(new Boss(NPCType<BereftVassal>(), spawnContext: type =>
            {
                SoundStyle roar = InfernumMode.Assets.Sounds.InfernumSoundRegistry.VassalHornSound;
                int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                Player player = Main.player[whomst];
                SoundEngine.PlaySound(roar, player.Center);
                NPC.SpawnOnPlayer(whomst, NPCType<BereftVassal>());
            }, usesSpecialSound: true, permittedNPCs: [NPCType<GreatSandShark>()]));

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("SubspaceSerpentHead"), spawnContext: type =>
                {
                    BossRushTeleports.HellTeleportAll();
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, permittedNPCs: [SOTSNPC("SubspaceSerpentBody"), SOTSNPC("SubspaceSerpentTail"), SOTSNPC("SubspaceEye")]));
            }

            Bosses.Add(new Boss(NPCID.CultistBoss, spawnContext: type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];
                int doctorLooneyTunes = NPC.NewNPC(new EntitySource_WorldEvent(), (int)player.Center.X, (int)player.Center.Y - 400, type, 1);
                Main.npc[doctorLooneyTunes].direction = Main.npc[doctorLooneyTunes].spriteDirection = Math.Sign(player.Center.X - player.Center.X - 90f);
                Main.npc[doctorLooneyTunes].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(doctorLooneyTunes);
            }, permittedNPCs: [ NPCID.CultistBossClone, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4,
                    NPCID.CultistDragonTail, NPCID.AncientCultistSquidhead, NPCID.AncientLight, NPCID.AncientDoom ]));

            Bosses.Add(new Boss(NPCID.SkeletronPrime, TimeChangeContext.Night, permittedNPCs: [NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser, NPCID.Probe]));

            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("PriestessRod"), permittedNPCs: [HomewardNPC("PriestessRod_Minion")]));
            }

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                if (InfernalCrossmod.FargosSouls.Mod.Version < Version.Parse("1.8"))
                {
                    Bosses.Add(new Boss(SoulsNPC("LifeChallenger"), TimeChangeContext.Day));
                }
                else
                    Bosses.Add(new Boss(SoulsNPC("Lifelight"), TimeChangeContext.Day));
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("NewPolaris")));
            }

            Bosses.Add(new Boss(ModContent.NPCType<OldDuke>(), spawnContext: type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];
                int boomerDuke = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                Main.npc[boomerDuke].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(boomerDuke);
            }, permittedNPCs: [ModContent.NPCType<OldDukeToothBall>(), ModContent.NPCType<SulphurousSharkron>()]));

            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("ScarabBelief"), permittedNPCs: [HomewardNPC("ScarabBelief_Minion")]));
            }

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                Bosses.Add(new Boss(SoulsNPC("DeviBoss")));
            }

            Bosses.Add(new Boss(NPCID.Golem, TimeChangeContext.Day, type =>
            {
                int sans = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].position.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].position.Y - 400f), type, 1);
                Main.npc[sans].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(sans);
            }, permittedNPCs: [NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree]));

            // Tier 4
            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("Diver"), permittedNPCs: [HomewardNPC("Diver_Minion"), HomewardNPC("AbyssPortal")]));

                //Bosses.Add(new Boss(HomewardNPC("TheMotherbrain"), permittedNPCs: [HomewardNPC("TheMotherbrain_Minion")]));
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("ForgottenOne"), TimeChangeContext.Day, permittedNPCs: [ThoriumNPC("AbyssalSpawn"), ThoriumNPC("ForgottenOneCracked"), ThoriumNPC("ForgottenOneReleased")]));
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                Bosses.Add(new Boss(SOTSNPC("Lux"), permittedNPCs: [SOTSNPC("FakeLux"), SOTSNPC("Collector2")]));
            }

            Bosses.Add(new Boss(NPCID.HallowBoss, spawnContext: type =>
            {
                int drawcodeGoddess = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400f), type, 1);
                Main.npc[drawcodeGoddess].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(drawcodeGoddess);
            }, toChangeTimeTo: TimeChangeContext.Night));

            Bosses.Add(new Boss(NPCID.DD2Betsy, spawnContext: type =>
            {
                if (!NPC.AnyNPCs(NPCID.DD2Betsy))
                {
                    SoundStyle roar = SoundID.DD2_BetsyScream;
                    int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                    Player player = Main.player[whomst];
                    SoundEngine.PlaySound(roar, player.Center);
                    NPC.SpawnOnPlayer(whomst, NPCID.DD2Betsy);
                }
            }, specialSpawnCountdown: 400, usesSpecialSound: true));

            Bosses.Add(new Boss(NPCID.Spazmatism, TimeChangeContext.Night, type =>
            {
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Spazmatism);
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Retinazer);
            }, permittedNPCs: [NPCID.Retinazer]));

            if (InfernalCrossmod.YouBoss.Loaded && InfernalConfig.Instance.TerraBladeBossInBossRush)
            {
                Bosses.Add(new Boss(InfernalCrossmod.YouBoss.Mod.Find<ModNPC>("TerraBladeBoss").Type, TimeChangeContext.Night, type =>
                {
                    SoundStyle roar = new SoundStyle("InfernalEclipseAPI/Assets/Sounds/Custom/TerraBlade/Dash");
                    int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                    Player player = Main.player[whomst];
                    SoundEngine.PlaySound(roar, player.Center);
                    NPC.SpawnOnPlayer(whomst, InfernalCrossmod.YouBoss.Mod.Find<ModNPC>("TerraBladeBoss").Type);
                }, usesSpecialSound: true));
            }

            Bosses.Add(new Boss(ModContent.NPCType<Polterghast>(), TimeChangeContext.Day, permittedNPCs: [ModContent.NPCType<PhantomFuckYou>(), ModContent.NPCType<PolterghastHook>(), ModContent.NPCType<PolterPhantom>(), ModContent.NPCType<PolterghastLeg>()]));

            if (InfernalCrossmod.Catalyst.Loaded)
            {
                Bosses.Add(new Boss(InfernalCrossmod.Catalyst.Mod.Find<ModNPC>("Astrageldon").Type, TimeChangeContext.Night, permittedNPCs: [InfernalCrossmod.Catalyst.Mod.Find<ModNPC>("NovaSlime").Type, InfernalCrossmod.Catalyst.Mod.Find<ModNPC>("NovaSlimer").Type]));
            }

            Bosses.Add(new Boss(NPCID.MoonLordCore, spawnContext: type =>
            {
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            }, usesSpecialSound: true, permittedNPCs: [NPCID.MoonLordLeechBlob, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye]));

            if (HomewardLoaded())
            {
                //Wall of Shadow

                Bosses.Add(new Boss(HomewardNPC("SlimeGod"), permittedNPCs: [HomewardNPC("BeamSlime"), HomewardNPC("FlyingSpikeSlime"), HomewardNPC("IcicleSlime"), HomewardNPC("NitroSlime"), HomewardNPC("PropellerSlime"), HomewardNPC("SlimeJupiter"), HomewardNPC("SlimeMars"),
                                                                                HomewardNPC("SlimeMercury"), HomewardNPC("SlimeNeptune"), HomewardNPC("SlimePluto"), HomewardNPC("SlimeSaturn"), HomewardNPC("SlimeTerra"), HomewardNPC("SlimeUranus"), HomewardNPC("SlimeVenus"),
                                                                                HomewardNPC("SnowflakeSlime")]));

                Bosses.Add(new Boss(HomewardNPC("TheOverwatcher"), permittedNPCs: [HomewardNPC("TheOverwatcher_Minion"), HomewardNPC("Overseer")]));
            }

            Bosses.Add(new Boss(ModContent.NPCType<CeaselessVoid>(), spawnContext: type =>
            {
                BossRushTeleports.TeleportToForbiddenArchive();
                Player player = Main.player[ClosestPlayerToWorldCenter];
                int ceaselessVoid = NPC.NewNPC(new EntitySource_WorldEvent(), (int)player.Center.X, (int)(player.position.Y + 300f), type, 1);
                CalamityUtils.BossAwakenMessage(ceaselessVoid);
            }, permittedNPCs: [ModContent.NPCType<DarkEnergy>()]));

            Bosses.Add(new Boss(ModContent.NPCType<CalamitasClone>(), TimeChangeContext.Night, specialSpawnCountdown: 420, dimnessFactor: 0.6f, permittedNPCs: [ ModContent.NPCType<Cataclysm>(), ModContent.NPCType<Catastrophe>(),
                         ModContent.NPCType<SoulSeeker>() ]));

            // Tier 5
            Bosses.Add(new Boss(ModContent.NPCType<DevourerofGodsHead>(), TimeChangeContext.Day, type =>
            {
                Player player = Main.player[ClosestPlayerToWorldCenter];
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    if (Main.player[playerIndex].active)
                    {
                        Player player2 = Main.player[playerIndex];
                        if (player2.FindBuffIndex(ModContent.BuffType<DoGExtremeGravity>()) > -1)
                            player2.ClearBuff(ModContent.BuffType<DoGExtremeGravity>());
                    }
                }

                SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, player.Center);
                NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>()]));

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Bosses.Add(new Boss(ThoriumNPC("DreamEater"), TimeChangeContext.Night, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ThoriumNPC("SlagFury"));
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ThoriumNPC("Aquaius"));
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ThoriumNPC("Omnicide"));
                }, 0, permittedNPCs: [ThoriumNPC("SlagFury"), ThoriumNPC("Aquaius"), ThoriumNPC("Omnicide"), ThoriumNPC("DreamEater")]));
            }

            Bosses.Add(new Boss(ModContent.NPCType<Yharon>(), TimeChangeContext.Day));

            if (InfernalCrossmod.Clamity.Loaded)
            {
                Bosses.Add(new Boss(ClamityNPC("WallOfBronze"), spawnContext: type =>
                {
                    BossRushTeleports.HellTeleportAll();
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, permittedNPCs: [ClamityNPC("WallOfBronzeClaw"), ClamityNPC("WallOfBronzeLaser"), ClamityNPC("WallOfBronzeTorret")]));
            }

            Bosses.Add(new Boss(NPCType<PrimordialWyrmHead>(), TimeChangeContext.Night, type =>
            {
                SoundStyle roar = CalamityMod.Sounds.CommonCalamitySounds.WyrmScreamSound;
                int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                Player player = Main.player[whomst];
                SoundEngine.PlaySound(roar, player.Center);
                NPC.SpawnOnPlayer(whomst, NPCType<PrimordialWyrmHead>());
            }, permittedNPCs: new int[] { NPCType<PrimordialWyrmHead>(), NPCType<PrimordialWyrmBody>(), NPCType<PrimordialWyrmBodyAlt>(), NPCType<PrimordialWyrmTail>() }));

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                Bosses.Add(new Boss(SoulsNPC("CosmosChampion"), spawnContext: type =>
                {
                    int erd = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
                    Main.npc[erd].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(erd);
                }));
            }

            if (HomewardLoaded())
            {
                Bosses.Add(new Boss(HomewardNPC("WorldsEndEverlastingFallingWhale"), spawnContext: type =>
                {
                    BossRushTeleports.OceanTeleportAll();
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, permittedNPCs: [HomewardNPC("AtlasMoth"), HomewardNPC("Griffin"), HomewardNPC("Loong_Body"), HomewardNPC("Loong_Head"), HomewardNPC("Mammoth"),
                                                                                                        HomewardNPC("Python_Body"), HomewardNPC("Python_Head"), HomewardNPC("Turtle"), HomewardNPC("PhantomOfDeath_1"), HomewardNPC("PhantomOfDeath_2")]));
            }

            Bosses.Add(new Boss(ModContent.NPCType<Providence>(), TimeChangeContext.Day, type =>
            {
                BossRushTeleports.TeleportToProvidence();
                Player player = Main.player[ClosestPlayerToWorldCenter];

                SoundEngine.PlaySound(Providence.SpawnSound, player.Center);
                int prov = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), type, 1);
                Main.npc[prov].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(prov);
            }, specialSpawnCountdown: 240, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<ProvSpawnOffense>(), ModContent.NPCType<ProvSpawnHealer>(), ModContent.NPCType<ProvSpawnDefense>()]));

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                Bosses.Add(new Boss(SoulsNPC("AbomBoss")));
            }

            Bosses.Add(new Boss(ModContent.NPCType<Draedon>(), spawnContext: type =>
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Draedon>()))
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(CodebreakerUI.SummonSound, player.Center);
                    Vector2 spawnPos = player.Center + new Vector2(-8f, -100f);
                    int draedon = NPC.NewNPC(new EntitySource_WorldEvent("CalamityMod_BossRush"), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<Draedon>());
                    Main.npc[draedon].timeLeft *= 20;
                }
            }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<Artemis>(), ModContent.NPCType<Apollo>(), ModContent.NPCType<AresBody>(),
                    ModContent.NPCType<AresLaserCannon>(), ModContent.NPCType<AresTeslaCannon>(), ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.NPCType<AresGaussNuke>(), ModContent.NPCType<AresPulseCannon>(), ModContent.NPCType<AresEnergyKatana>(),
                    ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<ThanatosBody1>(), ModContent.NPCType<ThanatosBody2>(), ModContent.NPCType<ThanatosTail>() }));

            if (ModLoader.TryGetMod("HypnosMod", out Mod hypnos))
            {
                Bosses.Add(new Boss(hypnos.Find<ModNPC>("HypnosBoss").Type, TimeChangeContext.Night, permittedNPCs: new int[] { hypnos.Find<ModNPC>("AergiaNeuron").Type, hypnos.Find<ModNPC>("HypnosPlug").Type }));
            }

            Bosses.Add(new Boss(ModContent.NPCType<SupremeCalamitas>(), spawnContext: type =>
            {
                SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42f, 84f), type);
            }, dimnessFactor: 0.5f, permittedNPCs: [ ModContent.NPCType<SepulcherArm>(), ModContent.NPCType<SepulcherHead>(), ModContent.NPCType<SepulcherBody>(),
                    ModContent.NPCType<SepulcherBodyEnergyBall>(), ModContent.NPCType<SepulcherTail>(),
                    ModContent.NPCType<SoulSeekerSupreme>(), ModContent.NPCType<BrimstoneHeart>(), ModContent.NPCType<SupremeCataclysm>(),
                    ModContent.NPCType<SupremeCatastrophe>(), ModContent.NPCType<ShadowDemon>() ]));

            // Tier 6
            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                Bosses.Add(new Boss(calHunt.Find<ModNPC>("Goozma").Type, TimeChangeContext.Night, type =>
                {
                    SoundStyle roar = new("CalamityHunt/Assets/Sounds/Goozma/GoozmaAwaken");
                    int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                    Player guy = Main.player[whomst];
                    SoundEngine.PlaySound(roar, guy.Center);
                    NPC.SpawnOnPlayer(whomst, calHunt.Find<ModNPC>("Goozma").Type);
                }, permittedNPCs: [calHunt.Find<ModNPC>("EbonianBehemuck").Type, calHunt.Find<ModNPC>("CrimulanGlopstrosity").Type, calHunt.Find<ModNPC>("DivineGargooptuar").Type, calHunt.Find<ModNPC>("DivineGargooptuar").Type, calHunt.Find<ModNPC>("Goozmite").Type]));
            }

            if (HomewardLoaded())
            {
                //Bosses.Add(new Boss(HomewardNPC("TheSon"), permittedNPCs: [HomewardNPC("TheSon_Minion")]));
            }

            if (InfernalCrossmod.NoxusBoss.Loaded && InfernalConfig.Instance.WrathoftheGodsBossesInBossRush)
            {
                Bosses.Add(new Boss(WrathNPC("MarsBody"), TimeChangeContext.Night, permittedNPCs: [WrathNPC("BattleSolyn"), WrathNPC("TrappingHolographicForcefield")]));
            }

            if (InfernalCrossmod.FargosSouls.Loaded)
            {
                Bosses.Add(new Boss(SoulsNPC("MutantBoss"), permittedNPCs: [SoulsNPC("MutantIllusion")]));
            }

            if (InfernalCrossmod.NoxusBoss.Loaded && InfernalConfig.Instance.WrathoftheGodsBossesInBossRush)
            {
                Bosses.Add(new Boss(WrathNPC("AvatarOfEmptiness"), TimeChangeContext.Night, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, WrathNPC("AvatarOfEmptiness"));
                }, permittedNPCs: new int[] { WrathNPC("BattleSolyn"), WrathNPC("NamelessDeityBoss"), WrathNPC("AvatarRift"), WrathNPC("AvatarOfEmptiness") }));

                Bosses.Add(new Boss(WrathNPC("NamelessDeityBoss"), TimeChangeContext.Night, specialSpawnCountdown: 270, usesSpecialSound: true));
            }

            // Death IDs after defeat
            if (InfernalCrossmod.Thorium.Loaded)
            {
                BossIDsAfterDeath[ThoriumNPC("BoreanStrider")] = new int[] { ThoriumNPC("BoreanStriderPopped") };
                BossIDsAfterDeath[ThoriumNPC("FallenBeholder")] = new int[] { ThoriumNPC("FallenBeholder"), ThoriumNPC("FallenBeholder2") };
                BossIDsAfterDeath[ThoriumNPC("Lich")] = new int[] { ThoriumNPC("Lich"), ThoriumNPC("LichHeadless") };
                BossIDsAfterDeath[ThoriumNPC("ForgottenOne")] = [ThoriumNPC("ForgottenOneReleased")];
            }
            BossIDsAfterDeath[ModContent.NPCType<Apollo>()] =
            [
                ModContent.NPCType<Apollo>(),
                ModContent.NPCType<Artemis>(),
                ModContent.NPCType<AresBody>(),
                ModContent.NPCType<AresLaserCannon>(),
                ModContent.NPCType<AresPlasmaFlamethrower>(),
                ModContent.NPCType<AresTeslaCannon>(),
                ModContent.NPCType<AresGaussNuke>(),
                ModContent.NPCType<AresPulseCannon>(),
                ModContent.NPCType<ThanatosHead>(),
                ModContent.NPCType<ThanatosBody1>(),
                ModContent.NPCType<ThanatosBody2>(),
                ModContent.NPCType<ThanatosTail>(),
            ];

            // Death Effects
            BossDeathEffects = new Dictionary<int, Action<NPC>>
            {
                [NPCID.WallofFlesh] = npc => { BossRushTeleports.BringPlayersBackToSpawn(); },
                [NPCType<ProfanedGuardianCommander>()] = npc =>
                {
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierOneComplete);
                    CreateTierAnimation(2);
                    BossRushTeleports.BringPlayersBackToSpawn();
                },
                [NPCID.SkeletronHead] = npc =>
                {
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierTwoComplete);

                    ActiveEntityIterator<Player>.Enumerator enumerator = Main.ActivePlayers.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Player current = enumerator.Current;
                        if (!current.dead)
                        {
                            int num = Projectile.NewProjectile(new EntitySource_WorldEvent(), current.Center, Vector2.Zero, ModContent.ProjectileType<BossRushTierAnimation>(), 0, 0f, current.whoAmI);
                            if (Main.projectile.IndexInRange(num))
                            {
                                Main.projectile[num].ai[0] = 3;
                            }
                        }
                    }
                },
                [NPCID.Golem] = npc =>
                {
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierThreeComplete);
                    CreateTierAnimation(4);
                },
                [NPCType<CeaselessVoid>()] = npc => { BossRushTeleports.BringPlayersBackToSpawn(); },
                [NPCType<CalamitasClone>()] = npc =>
                {
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierFourComplete);
                    CreateTierAnimation(5);
                },
                [NPCType<Providence>()] = npc => { BossRushTeleports.BringPlayersBackToSpawn(); }
            };

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                BossDeathEffects.Add(ConsolariaNPC("Ocram"), npc => { Main.bloodMoon = false; });
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                BossDeathEffects.Add(SOTSNPC("PharaohsCurse"), npc =>
                {
                    BossRushTeleports.BringPlayersBackToSpawn();
                });
                BossDeathEffects.Add(SOTSNPC("SubspaceSerpentHead"), npc =>
                {
                    BossRushTeleports.BringPlayersBackToSpawn();
                });
            }

            if (InfernalCrossmod.Clamity.Loaded)
            {
                BossDeathEffects.Add(ClamityNPC("WallOfBronze"), npc =>
                {
                    BossRushTeleports.BringPlayersBackToSpawn();
                });
            }

            if ((InfernalCrossmod.NoxusBoss.Loaded && InfernalConfig.Instance.WrathoftheGodsBossesInBossRush)|| calHunt != null || HomewardLoaded() || InfernalCrossmod.FargosSouls.Loaded)
            {
                BossDeathEffects.Add(NPCType<SupremeCalamitas>(), npc =>
                {
                    CustomBossRushDialogue.StartDialogue(IEoRBossRushDialoguePhase.TierFiveComplete);
                    InfernalWorld.tier5Downed = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        foreach (Player p in Main.ActivePlayers)
                        {
                            if (p.dead)
                                continue;

                            int animation = Projectile.NewProjectile(new EntitySource_WorldEvent(), p.Center, Vector2.Zero, ProjectileType<BossRushTier6Animation>(), 0, 0f, p.whoAmI);
                        }
                    }
                });

                if (InfernalCrossmod.NoxusBoss.Loaded && InfernalConfig.Instance.WrathoftheGodsBossesInBossRush)
                {
                    BossDeathEffects.Add(WrathNPC("AvatarOfEmptiness"), npc => { InfernalWorld.tier6Downed = true; });

                    BossDeathEffects.Add(WrathNPC("NamelessDeityBoss"), npc =>
                    {
                        if (InfernalConfig.Instance.ForceFullXerocDialogue)
                        {
                            //Always play end dialgoue
                            BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.End);
                        }
                        else
                        {
                            BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End);
                        }
                        CalamityUtils.KillAllHostileProjectiles();
                        HostileProjectileKillCounter = 3;
                    });
                }
                else if (InfernalCrossmod.FargosSouls.Loaded)
                {
                    BossDeathEffects.Add(SoulsNPC("MutantBoss"), npc =>
                    {
                        InfernalWorld.tier6Downed = true;
                        if (InfernalConfig.Instance.ForceFullXerocDialogue)
                        {
                            //Always play end dialgoue
                            BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.End);
                        }
                        else
                        {
                            BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End);
                        }
                        CalamityUtils.KillAllHostileProjectiles();
                        HostileProjectileKillCounter = 3;
                    });
                }
                /*
                else if (HomewardLoaded())
                {
                    BossDeathEffects.Add(HomewardNPC("TheSon"), npc =>
                    {
                        if (InfernalConfig.Instance.ForceFullXerocDialogue)
                        {
                            //Always play end dialgoue
                            BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.End);
                        }
                        else
                        {
                            BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End);
                        }
                        CalamityUtils.KillAllHostileProjectiles();
                        HostileProjectileKillCounter = 3;
                    });
                }
                */
                else if (calHunt != null)
                {
                    InfernalWorld.tier6Downed = true;
                    BossDeathEffects.Add(calHunt.Find<ModNPC>("Goozma").Type, npc =>
                    {
                        if (InfernalConfig.Instance.ForceFullXerocDialogue)
                        {
                            //Always play end dialgoue
                            BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.End);
                        }
                        else
                        {
                            BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End);
                        }
                        CalamityUtils.KillAllHostileProjectiles();
                        HostileProjectileKillCounter = 3;
                    });
                }
            }
            else
            {
                BossDeathEffects.Add(NPCType<SupremeCalamitas>(), npc =>
                {
                    InfernalWorld.tier5Downed = true;
                    InfernalWorld.tier6Downed = true;
                    if (InfernalConfig.Instance.ForceFullXerocDialogue)
                    {
                        //Always play end dialgoue
                        BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.End);
                    }
                    else
                    {
                        BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End);
                    }
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;
                });
            }
        }
        public static int ThoriumNPC(string name)
        {
            return InfernalCrossmod.Thorium.Mod.Find<ModNPC>(name).Type;
        }
        public static int ConsolariaNPC(string name)
        {
            return InfernalCrossmod.Consolaria.Mod.Find<ModNPC>(name).Type;
        }

        public static int SOTSNPC(string name)
        {
            return InfernalCrossmod.SOTS.Mod.Find<ModNPC>(name).Type;
        }

        public static int ClamityNPC(string name)
        {
            return InfernalCrossmod.Clamity.Mod.Find<ModNPC>(name).Type;
        }

        public static int WrathNPC(string name)
        {
            return InfernalCrossmod.NoxusBoss.Mod.Find<ModNPC>(name).Type;
        }

        public static int SoulsNPC(string name)
        {
            return InfernalCrossmod.FargosSouls.Mod.Find<ModNPC>(name).Type;
        }

        public static int HomewardNPC(string name)
        {
            return ModLoader.GetMod("ContinentOfJourney").Find<ModNPC>(name).Type;
        }

        public static bool HomewardLoaded()
        {
            bool inBossRush = false;
            if (ModLoader.HasMod("ContinentOfJourney"))
                if (HomewardConfig.Instance.HomewardInBossRush)
                    inBossRush = true;

            return inBossRush;
        }
    }
}