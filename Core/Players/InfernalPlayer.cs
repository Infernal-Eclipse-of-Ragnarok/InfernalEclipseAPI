using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Content.Items.PermanentBoosters;
using InfernalEclipseAPI.Content.Items.Weapons.Legendary.Lycanroc;
using InfernalEclipseAPI.Content.Projectiles;
using InfernalEclipseAPI.Content.UI.Notificatons;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.DamageClasses;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using InfernumMode;
using InfernumMode.Common.DataStructures;
using InfernumMode.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace InfernalEclipseAPI.Core.Players
{
    public class InfernalPlayer : ModPlayer
    {
        // Stored variables
        private Vector2 previousPos;
        private bool wasUsingItem;
        public int BoostDirection;

        // Accessory effect reworks
        public bool CritNightmare;
        public bool bagOfCharms;
        public bool batPoop;
        public bool tixThumbRing;
        public bool bloodstainedCoin;
        public bool putridCoin;
        public bool eyeOfChaos;
        public bool snakeEyes;
        public bool chaosBadge;
        public bool focusReticle;
        public bool LazyCrafterAmulet;
        public bool statShareAll;
        public bool scalingArmorPenetration;
        public bool flightArmor;
        public bool Earthdrive;
        public bool InverseDiamondRing;
        public bool InverseAmberRing;
        public bool gutWrench;

        // Accessory Effects
        public bool exoSights;
        public bool blixerCoreSummon;
        public bool blixerLaserMode;

        // Consumables & Player States
        public int voidMagePrevention;

        public bool workshopHasBeenOwned;
        public bool aniversaryYearOneLoreObtained = false;

        public bool singularityCore;
        public int ruinousPlasmaInjection;
        public bool exoBaguette;

        // Steal Values
        public float manaSteal = Main.expertMode ? 40f : 50f;
        public float voidSteal = Main.expertMode ? 45f : 55f;
        public float inspirationSteal = Main.expertMode ? 5f : 10f;

        // Timers and Cooldowns
        private int horrifiedTimer = 0;
        private int jamTimer = 0;
        private int batCoinTimer = 0;
        public int resonatorTimer = 0;
        public int incubatorTextTime = 0;
        public int BoostPressTimer;
        public int namelessDialogueCooldown;
        private int nightmareArmCD;
        public int voidSicknessTextCooldown;
        public int teleportRespawnKilldown;
        public int boostCooldownTime;
        public int RingofRestCooldown;
        public int CloverCharmCooldown;

        // Boss Rush Intro Screens
        public bool tier1IntroPlayed;
        public bool tier2IntroPlayed;
        public bool tier3IntroPlayed;
        public bool tier4IntroPlayed;
        public bool tier5IntroPlayed;
        public bool tier6IntroPlayed;
        public bool tier7IntroPlayed;

        public override void Initialize()
        {
            workshopHasBeenOwned = false;
            singularityCore = false;
            ruinousPlasmaInjection = 0;
            exoBaguette = false;
        }

        public override void OnEnterWorld()
        {
            if (!InfernalWorld.craftedWorkshop && workshopHasBeenOwned)
            {
                InfernalWorld.craftedWorkshop = true;
            }

            /*
            if ((InfernalConfig.Instance.SolynCampsiteFixes || !ModLoader.HasMod("WOTGCampsiteFix")) && InfernalConfig.Instance.DeveloperMode)
                InGameNotificationsTracker.AddNotification(new SolynCampsiteFixApplicationNotification());
            */

            if (ModLoader.HasMod("ContinentOfJourney"))
            {
                if (HomewardConfig.Instance.DisplayHomewardWorldEntryMessages)
                {
                    InGameNotificationsTracker.AddNotification(new HomewardJourneyNotification());
                }
            }

            if (ModLoader.HasMod("FargowiltasSouls"))
                InGameNotificationsTracker.AddNotification(new FargosSoulsNotification());

            if (Main.getGoodWorld)
                InGameNotificationsTracker.AddNotification(new ForTheWorthyNotification());

            if (ModLoader.HasMod("CWRMod"))
                InGameNotificationsTracker.AddNotification(new OverhaulNotification());

            if (ModLoader.HasMod("Remnants"))
                InGameNotificationsTracker.AddNotification(new RemnantsNotification());

            if (ModLoader.HasMod("CalamityMinus"))
                InGameNotificationsTracker.AddNotification(new MinusNotification());

            if (ModLoader.HasMod("CalBalChange"))
                InGameNotificationsTracker.AddNotification(new CalBalanceNotification());

            if (ModLoader.HasMod("InfernumMasterPatch"))
                InGameNotificationsTracker.AddNotification(new MasterPatchNotification());

            if (InfernalConfig.Instance.ForceFullXerocDialogue)
            {
                DownedBossSystem.startedBossRushAtLeastOnce = false;
            }

            if (!InfernalConfig.Instance.DisplayWorldEntryMessages) return;

            Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.Welcome"), 95, 06, 06);

            if (InfernalWorld.RagnarokModeEnabled)
            {
                Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.InfernumActive"), 95, 06, 06);
                SoundEngine.PlaySound(InfernumMode.Assets.Sounds.InfernumSoundRegistry.ModeToggleLaugh, Player.Center);
            }

            if (ModLoader.HasMod("ThoriumMod"))
            {
                if (!InfernalCrossmod.ThoriumRework.Loaded)
                {
                    InGameNotificationsTracker.AddNotification(new HelheimNotification());
                }

                if (!InfernalCrossmod.RagnarokMod.Loaded)
                {
                    InGameNotificationsTracker.AddNotification(new RagnarokModNotification());
                }
                else if (!InfernalConfig.Instance.AutomatedConfigSetup)
                {
                    Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.RagnarokBalance"), 255, 255, 0);

                    if (InfernalCrossmod.ThoriumRework.Loaded)
                    {
                        Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.RagnarokRework"), 255, 255, 0);
                    }
                }
            }
        }

        public override void PlayerConnect()
        {
            if (!InfernalWorld.craftedWorkshop && workshopHasBeenOwned)
            {
                InfernalWorld.craftedWorkshop = true;
            }

            if (InfernalConfig.Instance.ForceFullXerocDialogue)
            {
                DownedBossSystem.startedBossRushAtLeastOnce = false;
            }

            if (!InfernalConfig.Instance.DisplayWorldEntryMessages) return;

            Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.MPConnect"), 95, 06, 06);
        }

        public override void SaveData(TagCompound tag)
        {
            if (workshopHasBeenOwned)
            {
                tag.Add("workshopHasBeenOwned", true);
            }

            if (aniversaryYearOneLoreObtained)
            {
                tag.Add("aniversaryYearOneLoreObtained", true);
            }

            var boost = new List<string>();
            boost.AddWithCondition("singularityCore", singularityCore);

            tag["IEORboost"] = boost;

            tag.Add("ruinousPlasmaInjection", ruinousPlasmaInjection);
            tag.Add("exoBaguette", exoBaguette);
        }

        public override void LoadData(TagCompound tag)
        {
            workshopHasBeenOwned = tag.Get<bool>("workshopHasBeenOwned");

            aniversaryYearOneLoreObtained = tag.Get<bool>("aniversaryYearOneLoreObtained");

            var boost = tag.GetList<string>("IEORboost");
            singularityCore = boost.Contains("singularityCore");

            ruinousPlasmaInjection = tag.Get<int>("ruinousPlasmaInjection");
            exoBaguette = tag.Get<bool>("exoBaguette");
        }

        public override bool CanUseItem(Item item)
        {
            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (Player.HasBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("Bubbled").Type))
                    return false;
            }

            if (ModLoader.TryGetMod("XDContentMod", out Mod heartbeat))
            {
                if (Player.mount.Active)
                {
                    if (Player.mount?.Type == heartbeat.Find<ModMount>("TapTapMinivan").Type || Player.mount?.Type == heartbeat.Find<ModMount>("LuxuryConvertible").Type || Player.mount?.Type == heartbeat.Find<ModMount>("DiDiCar").Type || Player.mount?.Type == heartbeat.Find<ModMount>("DiDiBike").Type || Player.mount?.Type == heartbeat.Find<ModMount>("KFCDeliveryScooter").Type)
                    {
                        return false;
                    }
                }
            }

            return base.CanUseItem(item);
        }

        public override void ResetEffects()
        {
            if (!Player.HasBuff(ModContent.BuffType<StarboundHorrification>()))
                horrifiedTimer = 0;

            if (!Player.HasBuff(ModContent.BuffType<WarpJammed>()))
                jamTimer = 0;

            if (teleportRespawnKilldown > 0)
                teleportRespawnKilldown--;

            if (namelessDialogueCooldown > 0)
                namelessDialogueCooldown--;

            if (CloverCharmCooldown > 0)
                CloverCharmCooldown--;

            if (incubatorTextTime > 0)
                incubatorTextTime--;

            if (voidMagePrevention > 0)
                voidMagePrevention--;

            if (boostCooldownTime > 0)
                boostCooldownTime--;

            if (voidSicknessTextCooldown > 0)
                voidSicknessTextCooldown--;

            if (manaSteal < (Main.expertMode ? 40f : 50f))
                manaSteal++;

            if (voidSteal < (Main.expertMode ? 45f : 55f))
            {
                voidSteal++;
            }

            if (voidSteal > (Main.expertMode ? 45f : 55f))
                voidSteal = (Main.expertMode ? 45f : 55f);

            if (inspirationSteal < (Main.expertMode ? 5f : 6f))
                inspirationSteal++;

            if (batPoop)
            {
                batCoinTimer++;
                if (batCoinTimer == 60 * 5)
                {
                    Player.QuickSpawnItem(Player.GetSource_Misc("IEoR_PoopCoin"), ItemID.GoldCoin, Main.rand.Next(1, 6));
                    batCoinTimer = 0;
                }
            }
            else
                batCoinTimer = 0;

            if (resonatorTimer > 0)
                resonatorTimer--;

            if (resonatorTimer == 1)
            {
                for (int i = 0; i < 24; i++)
                {
                    Vector2 pos = Player.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-24f, 8f));
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-0.6f, -0.1f));

                    int d = Dust.NewDust(pos, 0, 0, DustID.Cloud, vel.X, vel.Y, 150, default, Main.rand.NextFloat(1.0f, 1.6f));
                    Main.dust[d].noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.MaxMana, Player.Center);
            }
            if (namelessDialogueCooldown <= 0)
                InfernalWorld.namelessDeveloperDiagloguePlayed = false;

            soltanBullying = false;
            HarvestMoonBuff = false;
            scalingArmorPenetration = false;
            statShareAll = false;
            LazyCrafterAmulet = false;
            batPoop = false;
            bloodstainedCoin = false;
            putridCoin = false;
            eyeOfChaos = false;
            snakeEyes = false;
            chaosBadge = false;
            focusReticle = false;
            exoSights = false;
            flightArmor = false;
            CritNightmare = false;
            gutWrench = false;
            bagOfCharms = false;
            blixerCoreSummon = false;
            InverseAmberRing = false;
        }

        public override void PreUpdate()
        {
            if (BoostPressTimer > 0)
                BoostPressTimer--;

            if (Player.ZoneLihzhardTemple && !NPC.downedPlantBoss)
            {
                Player.statLife -= 2;
                if (Player.statLife <= 0)
                    Player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{Player.name} fell to the jungles curse...")), 0, 0);
                Player.AddBuff(BuffID.PotionSickness, 60);
            }

            if (InfernalCrossmod.Thorium.Loaded) 
            {
                if (Player.IsUnderwater() && NPC.AnyNPCs(InfernalCrossmod.Thorium.Mod.Find<ModNPC>("QueenJellyfish").Type))
                {
                    Player.AddBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("Bubbled").Type, 60);
                    Player.AddBuff(BuffID.Electrified, 60);
                }

                if (Player.HasBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("RealityBearer").Type) && BossRushEvent.BossRushActive)
                    Player.ClearBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("RealityBearer").Type);
            }
        }

        public int defenseGain;

        public override void UpdateDead()
        {
            Player.ClearBuff(ModContent.BuffType<WarpJammed>());
            Player.ClearBuff(ModContent.BuffType<StarboundHorrification>());

            teleportRespawnKilldown = 30;
        }

        public override void PostUpdate()
        {
            if (PlayerHasPurity(Player) && Player.HeldItem.type == ModContent.ItemType<PrismaticBreaker>())
            {
                Player.GetDamage<GenericDamageClass>() /= 1.4f;
            }

            if (Player.GetModPlayer<TheConcoctionPlayer>().swinesWrathCounter > 0)
                Player.GetModPlayer<TheConcoctionPlayer>().swinesWrathCounter--;

            if (Player.HasBuff(ModContent.BuffType<SwinesWrathBuff>()))
            {
                if (InfernalCrossmod.Thorium.Loaded)
                {
                    ThoriumEffectHandler.DisableThoriumEffects(Player, false);
                }
            }

            if (Player.HasBuff(ModContent.BuffType<StarboundHorrification>()))
            {
                horrifiedTimer++;

                // Give a 1 second grace period after first applying the buff
                if (horrifiedTimer < 60)
                {
                    previousPos = Player.position;
                    wasUsingItem = Player.itemAnimation > 0;
                    return;
                }

                float distanceMoved = Vector2.Distance(Player.position, previousPos);
                bool usedTeleportItem = !wasUsingItem && Player.itemAnimation > 0 &&
                    (Player.HeldItem.type == ItemID.MagicMirror ||
                     Player.HeldItem.type == ItemID.IceMirror ||
                     Player.HeldItem.type == ItemID.RecallPotion ||
                     Player.HeldItem.type == ItemID.WormholePotion ||
                     Player.HeldItem.type == ItemID.PotionOfReturn ||
                     Player.HeldItem.type == ItemID.CellPhone ||
                     Player.HeldItem.type == ItemID.Shellphone ||
                     Player.HeldItem.type == ItemID.ShellphoneHell ||
                     Player.HeldItem.type == ItemID.ShellphoneOcean ||
                     Player.HeldItem.type == ItemID.ShellphoneSpawn ||
                     Player.HeldItem.type == ItemID.DemonConch ||
                     Player.HeldItem.type == ItemID.MagicConch ||
                     Player.HeldItem.type == ItemID.TeleportationPotion);

                if (distanceMoved > 1000f || usedTeleportItem)
                {
                    SoundEngine.PlaySound(InfernumMode.Assets.Sounds.InfernumSoundRegistry.ModeToggleLaugh, Player.Center);
                    Player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{Player.name} tried to escape the cosmic entity.")), 9999.0, 0);
                }

                previousPos = Player.position;
                wasUsingItem = Player.itemAnimation > 0;
            }

            if (Player.HasBuff(ModContent.BuffType<WarpJammed>()))
            {
                jamTimer++;

                // Give a 3 second grace period after first applying the buff
                if (jamTimer < 180)
                {
                    previousPos = Player.position;
                    wasUsingItem = Player.itemAnimation > 0;
                    return;
                }

                float distanceMoved = Vector2.Distance(Player.position, previousPos);
                bool usedTeleportItem = !wasUsingItem && Player.itemAnimation > 0 &&
                    (Player.HeldItem.type == ItemID.MagicMirror ||
                     Player.HeldItem.type == ItemID.IceMirror ||
                     Player.HeldItem.type == ItemID.RecallPotion ||
                     Player.HeldItem.type == ItemID.WormholePotion ||
                     Player.HeldItem.type == ItemID.PotionOfReturn ||
                     Player.HeldItem.type == ItemID.CellPhone ||
                     Player.HeldItem.type == ItemID.Shellphone ||
                     Player.HeldItem.type == ItemID.ShellphoneHell ||
                     Player.HeldItem.type == ItemID.ShellphoneOcean ||
                     Player.HeldItem.type == ItemID.ShellphoneSpawn ||
                     Player.HeldItem.type == ItemID.DemonConch ||
                     Player.HeldItem.type == ItemID.MagicConch ||
                     Player.HeldItem.type == ItemID.TeleportationPotion);

                if (distanceMoved > 1000f || usedTeleportItem)
                {
                    SoundEngine.PlaySound(CalamityMod.Sounds.CommonCalamitySounds.ExoPlasmaShootSound, Player.Center);
                    Player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{Player.name} tried to escape draedon's creations.")), 9999.0, 0);
                }

                previousPos = Player.position;
                wasUsingItem = Player.itemAnimation > 0;
            }

            if (!NPC.downedBoss3 && !Main.hardMode && InfernalConfig.Instance.BossKillCheckOnOres && Player.HasBuff(BuffID.Bewitched))
                Player.ClearBuff(BuffID.Bewitched);

            if (RingofRestCooldown > 0)
                RingofRestCooldown--;

            if (nightmareArmCD > 0)
                nightmareArmCD--;
            else
                nightmareArmCD = 0;

            if (BossRushEvent.BossRushActive)
            {
                if (Player.statLifeMax2 > 1111)
                    Player.statLifeMax2 = 1111;

                if (Player.statLife > 1111)
                    Player.statLife = 1111;
            }
        }

        public bool soltanBullying = false;
        public bool HarvestMoonBuff = false;

        public override void PostUpdateMiscEffects()
        {
            CalamityPlayer calPlayer = Player.Calamity();

            int wyrm = CalamityGlobalNPC.adultEidolonWyrmHead;
            if (InfernalWorld.RagnarokModeEnabled && wyrm >= 0 && wyrm < Main.maxNPCs)
            {
                NPC Wyrm = Main.npc[wyrm];
                if (Wyrm != null && Wyrm.active && Wyrm.type == ModContent.NPCType<PrimordialWyrmHead>() && Wyrm.WithinRange(Player.Center, 40000f))
                {
                    Player.SetAbyssLightLevels();

                    if (calPlayer.ZoneAbyss && Player.whoAmI == Main.myPlayer)
                    {
                        Point tilePosition = Player.Center.ToTileCoordinates();

                        double abyssSurface = Main.remixWorld ? SulphurousSea.YStart : Main.rockLayer - Main.maxTilesY * 0.05;

                        double totalAbyssDepth = Main.remixWorld ? SulphurousSea.YStart : Main.maxTilesY - 250D - abyssSurface;

                        double playerAbyssDepth = Main.remixWorld ? totalAbyssDepth - tilePosition.Y : tilePosition.Y - abyssSurface;

                        double depthRatio = playerAbyssDepth / totalAbyssDepth;

                        calPlayer.darknessIntensity = calPlayer.abyssDarkness + (float)depthRatio * 3f;

                        if (!Player.headcovered)
                            ScreenObstruction.screenObstruction = Lerp(ScreenObstruction.screenObstruction, 0f, 0.1f);
                    }
                }
            }

            if (InfernalWorld.RagnarokModeEnabled)
            {
                // Condition 1: In the Dungeon before Skeletron is defeated
                bool unclearedDungeon = Player.ZoneDungeon && !NPC.downedBoss3 && !Main.hardMode;

                // Condition 2: In the Jungle Temple before Plantera is defeated
                bool unclearedTemple = Player.ZoneLihzhardTemple && !NPC.downedPlantBoss;

                // Apply Creative Shock if in either restricted area
                if (unclearedDungeon || unclearedTemple)
                {
                    Player.AddBuff(BuffID.NoBuilding, 2);
                    Player.noBuilding = true;
                }
            }

            CheckIfMouseItemIsSpellbook();

            if (soltanBullying)
            {
                float emptySummonSlots = Player.maxMinions - Player.slotsMinions;
                Player.GetDamage(DamageClass.Generic) += (float)(0.02 * emptySummonSlots);
                Player.GetDamage(DamageClass.Summon) -= (float)(0.02 * emptySummonSlots);

                ref StatModifier summon = ref Player.GetDamage(DamageClass.Summon);
                summon -= (float)(0.1 * Player.slotsMinions);
            }

            if (BossRushEvent.BossRushActive)
            {
                if (!(Main.LocalPlayer.name == "Dominic" || Main.LocalPlayer.name == "Lucille"))
                {
                    if (Player.Infernum().GetValue<bool>("CyberneticImmortalityIsActive"))
                    {
                        Referenced<bool> cyberneticImmortality = Player.Infernum().GetRefValue<bool>("CyberneticImmortalityIsActive");
                        cyberneticImmortality.Value = !cyberneticImmortality.Value;
                    }

                    if (Player.Infernum().GetValue<bool>("PhysicsDefianceIsEnabled"))
                    {
                        Player.Infernum().SetValue<bool>("PhysicsDefianceIsEnabled", false);
                    }
                }
            }

            if (Player.HasBuff<LowGround>() || Player.HasBuff<CrimulanAura>())
            {
                Player.buffImmune[BuffID.Featherfall] = true;
                Player.ClearBuff(BuffID.Featherfall);
                Player.slowFall = false;

                if (InfernalCrossmod.QoLC.Loaded)
                {
                    InfernalCrossmod.QoLC.RemoveQoLCompendiumInfiniteBuff(Player, BuffID.Featherfall);
                }
            }
        }

        public static bool PlayerHasPurity(Player player)
        {
            int purityType = ModContent.ItemType<Purity>();

            for (int i = 3; i < 10 + player.extraAccessorySlots; i++)
            {
                Item item = player.armor[i];
                if (item != null && item.type == purityType)
                    return true;
            }
            return false;
        }

        public void CheckIfMouseItemIsSpellbook()
        {
            if (!ModLoader.HasMod("NoxusBoss")) return;

            if (Main.myPlayer != Player.whoAmI)
                return;

            bool shouldSync = false;

            // ActiveItem doesn't need to be checked as the other possibility involves
            // the item in question already being in the inventory.
            if (Main.mouseItem != null && !Main.mouseItem.IsAir)
            {
                if (Main.mouseItem.type == ModContent.ItemType<SolynsSpellbook>() && !InfernalRecipeUnlockHandler.HasFoundSolynSpellbook)
                {
                    InfernalRecipeUnlockHandler.HasFoundSolynSpellbook = true;
                    shouldSync = true;
                }

            }

            if (shouldSync)
                CalamityNetcode.SyncWorld();
        }

        public override void PostUpdateEquips()
        {
            if (exoSights || focusReticle)
            {
                Player.GetCritChance(DamageClass.Generic) += 15f;
            }
            else if (eyeOfChaos)
            {
                Player.GetCritChance(DamageClass.Generic) += 12f;
            }
            else
            {
                if (snakeEyes)
                {
                    Player.GetCritChance(DamageClass.Generic) += 3f;
                }

                if (chaosBadge)
                {
                    Player.GetCritChance(DamageClass.Generic) += 5f;
                }
            }

            if (LazyCrafterAmulet)
            {
                Player.adjTile[TileID.WorkBenches] = true;
                Player.adjTile[TileID.Furnaces] = true;
                Player.adjTile[TileID.Anvils] = true;
                Player.adjTile[TileID.Bottles] = true;
                Player.adjTile[TileID.Tables] = true;
            }

            if (statShareAll)
            {
                if (!PlayerHasPurity(Player))
                {
                    var meleeDamage = Player.GetDamage(DamageClass.Melee);
                    float meleeAdd = (meleeDamage.Additive - 1f) * 0.1f;
                    float meleeFlat = meleeDamage.Flat * 0.1f;
                    float meleeMult = ((meleeDamage.Multiplicative - 1f) * 0.1f) + 1f;
                    float meleeBase = meleeDamage.Base * 0.1f;

                    var rangedDamage = Player.GetDamage(DamageClass.Ranged);
                    float rangedAdd = (rangedDamage.Additive - 1f) * 0.1f;
                    float rangedFlat = rangedDamage.Flat * 0.1f;
                    float rangedMult = ((rangedDamage.Multiplicative - 1f) * 0.1f) + 1f;
                    float rangedBase = rangedDamage.Base * 0.1f;

                    var magicDamage = Player.GetDamage(DamageClass.Magic);
                    float magicAdd = (magicDamage.Additive - 1f) * 0.1f;
                    float magicFlat = magicDamage.Flat * 0.1f;
                    float magicMult = ((magicDamage.Multiplicative - 1f) * 0.1f) + 1f;
                    float magicBase = magicDamage.Base * 0.1f;

                    var summonDamage = Player.GetDamage(DamageClass.Summon);
                    float summonAdd = (summonDamage.Additive - 1f) * 0.1f;
                    float summonFlat = summonDamage.Flat * 0.1f;
                    float summonMult = ((summonDamage.Multiplicative - 1f) * 0.1f) + 1f;
                    float summonBase = summonDamage.Base * 0.1f;

                    if (meleeAdd > 0f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var melee = ref Player.GetDamage(DamageClass.Melee);
                        generic += meleeAdd;
                        melee -= meleeAdd;
                    }

                    if (meleeFlat > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Flat += meleeFlat;
                        Player.GetDamage(DamageClass.Melee).Flat -= meleeFlat;
                    }

                    if (meleeMult > 1f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var melee = ref Player.GetDamage(DamageClass.Melee);
                        generic *= meleeMult;
                        melee /= meleeMult;
                    }

                    if (meleeBase > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Base += meleeBase;
                        Player.GetDamage(DamageClass.Melee).Base -= meleeBase;
                    }

                    if (rangedAdd > 0f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var ranged = ref Player.GetDamage(DamageClass.Ranged);
                        generic += rangedAdd;
                        ranged -= rangedAdd;
                    }

                    if (rangedFlat > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Flat += rangedFlat;
                        Player.GetDamage(DamageClass.Ranged).Flat -= rangedFlat;
                    }

                    if (rangedMult > 1f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var ranged = ref Player.GetDamage(DamageClass.Ranged);
                        generic *= rangedMult;
                        ranged /= rangedMult;
                    }

                    if (rangedBase > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Base += rangedBase;
                        Player.GetDamage(DamageClass.Ranged).Base -= rangedBase;
                    }

                    if (magicAdd > 0f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var magic = ref Player.GetDamage(DamageClass.Magic);
                        generic += magicAdd;
                        magic -= magicAdd;
                    }

                    if (magicFlat > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Flat += magicFlat;
                        Player.GetDamage(DamageClass.Magic).Flat -= magicFlat;
                    }

                    if (magicMult > 1f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var magic = ref Player.GetDamage(DamageClass.Magic);
                        generic *= magicMult;
                        magic /= magicMult;
                    }

                    if (magicBase > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Base += magicBase;
                        Player.GetDamage(DamageClass.Magic).Base -= magicBase;
                    }

                    if (summonAdd > 0f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var summon = ref Player.GetDamage(DamageClass.Summon);
                        generic += summonAdd;
                        summon -= summonAdd;
                    }

                    if (summonFlat > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Flat += summonFlat;
                        Player.GetDamage(DamageClass.Summon).Flat -= summonFlat;
                    }

                    if (summonMult > 1f)
                    {
                        ref var generic = ref Player.GetDamage(DamageClass.Generic);
                        ref var summon = ref Player.GetDamage(DamageClass.Summon);
                        generic *= summonMult;
                        summon /= summonMult;
                    }

                    if (summonBase > 0f)
                    {
                        Player.GetDamage(DamageClass.Generic).Base += summonBase;
                        Player.GetDamage(DamageClass.Summon).Base -= summonBase;
                    }
                }
            }

            if (Earthdrive)
            {
                float meleeSpeedBonus = Player.GetAttackSpeed(DamageClass.Melee) - 1f;
                float miningSpeedBonus = 1f - Player.pickSpeed;
                if (meleeSpeedBonus > 0.0)
                    Player.pickSpeed -= meleeSpeedBonus;
                if (miningSpeedBonus > 0.0)
                {
                    if (miningSpeedBonus > 0.1f)
                        miningSpeedBonus = 0.1f;
                    Player.GetAttackSpeed(DamageClass.Melee) += miningSpeedBonus;
                }
            }
            Earthdrive = false;

            if (Player.HasBuff(ModContent.BuffType<BrimstoneDesperation>()))
            {
                CalamityPlayer mp = Player.Calamity();

                mp.chaliceOfTheBloodGod = false;
                mp.chaliceHeartStyle = false;
                mp.draedonsHeart = false;

                if (InfernalCrossmod.Thorium.Loaded)
                {
                    ThoriumEffectHandler.DisableThoriumEffects(Player);
                }
            }
        }

        private bool oceanBufferModified = false;

        public override void PostUpdateBuffs()
        {
            if (exoBaguette)
                Player.Calamity().alcoholPoisonMax += ExoBaguette.alcoholCapBoost;

            if (InfernalCrossmod.SOTS.Loaded)
            {
                int idx = Player.FindBuffIndex(ModContent.BuffType<VoidSickness2>());
                if (idx == -1)
                    return;

                float time = Player.buffTime[idx];

                Player.GetDamage(DamageClass.Generic) -= (float)(0.25 * (time / 300f));

                if (time >= 10 * 60)
                {
                    Player.buffImmune[InfernalCrossmod.SOTS.Mod.Find<ModBuff>("VoidAccess").Type] = true;
                }
                else
                {
                    Player.buffImmune[InfernalCrossmod.SOTS.Mod.Find<ModBuff>("VoidAccess").Type] = false;
                }
            }

            if (InfernalCrossmod.Thorium.Loaded && InfernalConfig.Instance.ThoriumBalanceChangess && !InfernalCrossmod.Hummus.Loaded)
            {
                if (ModContent.TryFind<ModBuff>("ThoriumMod", "OceansBufferExhaust", out var buff))
                {
                    for (int i = 0; i < Player.buffType.Length; i++)
                    {
                        if (Player.buffType[i] == buff.Type && Player.buffTime[i] > 0)
                        {
                            if (!oceanBufferModified)
                            {
                                Player.buffTime[i] = (int)(Player.buffTime[i] * 2.5f);
                                oceanBufferModified = true;
                            }
                            break; // stop looping once we found the buff
                        }
                    }
                }
                else
                {
                    oceanBufferModified = false; // reset if buff is gone
                }
            }

            /*
            if (Player.whoAmI == Main.myPlayer)
            {
                float baseRecoveryRate = Main.expertMode ? 0.3f : 0.4f;
                float lifeStealRecoveryRateReduction = Main.expertMode ? 0.2f : 0.25f;
                float lifeStealCap = Main.expertMode ? 40f : 50f;

                float lifeStealRecoveryRate = baseRecoveryRate - lifeStealRecoveryRateReduction;

                if (manaSteal < lifeStealCap)
                {
                    int timeLeft = (int)MathF.Ceiling(Math.Max(0f, lifeStealCap - manaSteal));

                    // If it should display at all, don't let it become 0 from float truncation.
                    if (timeLeft <= 0)
                        timeLeft = 1;

                    if (Player.Calamity().cooldowns.TryGetValue(ManaSteal.ID, out var cooldown))
                    {
                        cooldown.timeLeft = timeLeft;
                    }
                    else
                    {
                        Player.AddCooldown(ManaSteal.ID, timeLeft).timeLeft = timeLeft;
                    }
                }
            }
            */
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (InfernalEclipseAPI.SubpaceBoostHotkey.JustPressed && boostCooldownTime <= 15)
            {
                BoostPressTimer = 2;
                BoostDirection =
                    Player.controlRight ? 1 :
                    Player.controlLeft ? -1 :
                    Player.direction;

                if (boostCooldownTime >= 5)
                    boostCooldownTime = 225;
                else
                    boostCooldownTime = 135;
            }
        }

        public void ConvertSummonMeleeToMelee(Item item, ref StatModifier damage)
        {
            if (item.DamageType == ModContent.GetInstance<MeleeWhip>())
                item.DamageType = DamageClass.SummonMeleeSpeed;

            if (!soltanBullying || item is null || item.IsAir)
                return;

            var summonMeleeSpeed = ModContent.GetInstance<SummonMeleeSpeedDamageClass>();
            if (!item.CountsAsClass(summonMeleeSpeed))
                return;

            // Replace the item's damage scaling with Melee scaling:
            /*
            float meleeScale = player.GetTotalDamage(DamageClass.Melee).ApplyTo(1f);
            float sourceScale = player.GetTotalDamage(summonMeleeSpeed).ApplyTo(1f);
            float ratio = meleeScale / MathF.Max(sourceScale, 1e-6f);
            */
            item.DamageType = ModContent.GetInstance<MeleeWhip>();

            //damage *= ratio;       // mimic Melee scaling
            damage *= 1.25f;       // extra 10% while SoltanBullying
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            ConvertSummonMeleeToMelee(item, ref damage);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (scalingArmorPenetration)
            {
                if (!BypassesScalingArmorPen(target.type))
                {
                    modifiers.DefenseEffectiveness *= Main.hardMode ? 0.9f : 0.95f;
                }
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                if (target.type == InfernalCrossmod.Thorium.Mod.Find<ModNPC>("BoreanHopper").Type)
                {
                    modifiers.FinalDamage *= 0.2f;
                }
            }
        }

        private static bool BypassesScalingArmorPen(int type)
        {
            if (type == ModContent.NPCType<Providence>())
                return true;

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Mod thor = InfernalCrossmod.Thorium.Mod;

                if (type == thor.Find<ModNPC>("BoreanStrider").Type ||
                    type == thor.Find<ModNPC>("BoreanStriderPopped").Type ||
                    type == thor.Find<ModNPC>("BoreanHopper").Type ||
                    type == thor.Find<ModNPC>("BoreanStrider").Type ||
                    type == thor.Find<ModNPC>("ForgottenOne").Type ||
                    type == thor.Find<ModNPC>("ForgottenOneCracked").Type ||
                    type == thor.Find<ModNPC>("ForgottenOneReleased").Type)
                {
                    return true;
                }
            }

            if (InfernalCrossmod.Clamity.Loaded)
            {
                Mod clam = InfernalCrossmod.Clamity.Mod;

                if (type == clam.Find<ModNPC>("ClamitasBoss").Type)
                    return true;
            }

            return false;
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if ((target.type == ModContent.NPCType<AstrumDeusHead>() || target.type == ModContent.NPCType<AstrumDeusBody>() || target.type == ModContent.NPCType<AstrumDeusTail>()) && !NPC.downedAncientCultist)
            {
                modifiers.FinalDamage *= 0.1f;
            }

            if ((target.type == ModContent.NPCType<PrimordialWyrmBody>() || target.type == ModContent.NPCType<PrimordialWyrmHead>() || target.type == ModContent.NPCType<PrimordialWyrmTail>() || target.type == ModContent.NPCType<PrimordialWyrmBodyAlt>()) && !DownedBossSystem.downedYharon)
            {
                modifiers.FinalDamage *= 0.1f;
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if ((proj.type == ModContent.ProjectileType<CelestusProj>() || proj.type == ModContent.ProjectileType<CelestusMiniScythe>()) &&
                (target.type == ModContent.NPCType<SepulcherHead>() || target.type == ModContent.NPCType<SepulcherBody>() || target.type == ModContent.NPCType<SepulcherTail>()))
            {
                modifiers.FinalDamage *= 0.001f;
            }

            if ((target.type == ModContent.NPCType<AstrumDeusHead>() || target.type == ModContent.NPCType<AstrumDeusBody>() || target.type == ModContent.NPCType<AstrumDeusTail>()) && !NPC.downedAncientCultist)
            {
                modifiers.FinalDamage *= 0.1f;
            }

            if ((target.type == ModContent.NPCType<PrimordialWyrmBody>() || target.type == ModContent.NPCType<PrimordialWyrmHead>() || target.type == ModContent.NPCType<PrimordialWyrmTail>() || target.type == ModContent.NPCType<PrimordialWyrmBodyAlt>()) && !DownedBossSystem.downedYharon)
            {
                modifiers.FinalDamage *= 0.1f;
            }

            if (target.type == ModContent.NPCType<Yharon>() && target.life < target.lifeMax / 4 && (proj.type == ModContent.ProjectileType<GalaxySmasherHammer>() || proj.type == ModContent.ProjectileType<GalaxySmasherBlast>() || proj.type == ModContent.ProjectileType<GalaxySmasherEcho>() || proj.type == ModContent.ProjectileType<GalaxySmasherMini>()))
            {
                modifiers.FinalDamage /= 2;
            }

            if (target.type == NPCID.TheDestroyer || target.type == NPCID.TheDestroyerBody || target.type == NPCID.TheDestroyerTail)
            {
                if (proj.type == ModContent.ProjectileType<SubmarineShockerProj>())
                    modifiers.FinalDamage *= 0.2f;
                
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    if (proj.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModProjectile>("BeholderBlade").Type || proj.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModProjectile>("Void").Type)
                        modifiers.FinalDamage /= 2;
                }
            }

            if (target.type == ModContent.NPCType<AquaticScourgeHead>() || target.type == ModContent.NPCType<AquaticScourgeBody>() || target.type == ModContent.NPCType<AquaticScourgeTail>())
            {
                if (InfernalCrossmod.ThoriumRework.Loaded)
                {
                    if (proj.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModProjectile>("BeholderBlade").Type || proj.type == InfernalCrossmod.ThoriumRework.Mod.Find<ModProjectile>("Void").Type)
                        modifiers.FinalDamage /= 2;
                }
            }

            if (proj.type == ModContent.ProjectileType<DukesDecapitatorProj>() || proj.type == ModContent.ProjectileType<DukesDecapitatorBubble>())
            {

                if ((target.type == ModContent.NPCType<PlaguebringerGoliath>() || target.type == ModContent.NPCType<RavagerBody>() || target.type == ModContent.NPCType<RavagerClawLeft>() || target.type == ModContent.NPCType<RavagerClawRight>() 
                  || target.type == ModContent.NPCType<RavagerHead>() ||target.type == ModContent.NPCType<RavagerHead2>() || target.type == ModContent.NPCType<RavagerLegLeft>() || target.type == ModContent.NPCType<RavagerLegRight>()))
                {
                    modifiers.FinalDamage *= 0.1f;
                }

                if (InfernalCrossmod.SOTS.Loaded)
                {
                    if ((target.type == InfernalCrossmod.SOTS.Mod.Find<ModNPC>("Lux").Type))
                    {
                        modifiers.FinalDamage *= 0.5f;
                    }
                }
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                //Desert Scourge DR
                if ((target.type == ModContent.NPCType<DesertScourgeBody>() || target.type == ModContent.NPCType<DesertScourgeHead>() || target.type == ModContent.NPCType<DesertScourgeTail>()))
                {
                    if (proj.type == InfernalCrossmod.Thorium.Mod.Find<ModProjectile>("ButterflyStaffPro").Type)
                    {
                        modifiers.FinalDamage *= 0.5f;
                    }
                }
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (InverseAmberRing)
            {
                InflictRandomDebuff(target);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.whoAmI != Main.myPlayer) return;

            if (!proj.npcProj && !proj.trap && proj.friendly)
            {
                LycanrocGlobalProjectile lgp = proj.GetGlobalProjectile<LycanrocGlobalProjectile>();
                if (lgp.appliesArmorCrunch)
                {
                    target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
                }

                if (lgp.appliesCrumbling)
                {
                    target.AddBuff(ModContent.BuffType<Crumbling>(), 180);
                }
            }

            if (tixThumbRing && proj.arrow && hit.Crit)
                target.AddBuff(BuffID.ShadowFlame, 60, false);

            if (InverseAmberRing)
            {
                InflictRandomDebuff(target);
            }

            if (hit.Crit && CritNightmare && proj != null && proj.type != ModContent.ProjectileType<EvilGrowth>() && proj.type != ModContent.ProjectileType<EvilStrike>() && nightmareArmCD <= 0)
            {
                nightmareArmCD = 360;
                if (Main.myPlayer == Player.whoAmI)
                    Projectile.NewProjectile(new EntitySource_OnHit(Player, target), target.Center, Vector2.Zero, ModContent.ProjectileType<EvilGrowth>(), (int)(Main.hardMode ? hit.SourceDamage * 0.1 : hit.SourceDamage * 0.05), 0f, Player.whoAmI, 0f, target.whoAmI);
            }
        }

        private static void InflictRandomDebuff(NPC target)
        {
            int debuffRarity = Main.rand.Next(21);

            if (debuffRarity < 5)
            {
                int debuff = Main.rand.Next(8);

                switch (debuff)
                {
                    case 0:
                        target.AddBuff(BuffID.Poisoned, 180, false);
                        break;
                    case 2:
                        target.AddBuff(BuffID.Frostburn, 180, false);
                        break;
                    case 3:
                        goto default;
                    case 4:
                        target.AddBuff(BuffID.Confused, 90, false);
                        break;
                    case 5:
                        target.AddBuff(BuffID.Ichor, 120, false);
                        break;
                    case 6:
                        if (InfernalCrossmod.Thorium.Loaded)
                        {
                            target.AddBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("Charmed").Type, 180, false);
                            break;
                        }
                        else goto default;
                    case 7:
                        if (InfernalCrossmod.Thorium.Loaded)
                        {
                            if (!target.boss)
                            {
                                target.AddBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("Stunned").Type, 30, false);
                                break;
                            }
                            else goto default;
                        }
                        else goto default;
                    default:
                        target.AddBuff(BuffID.OnFire3, 180, false);
                        break;
                }
            }
            else if (debuffRarity >= 5 && debuffRarity < 20)
            {
                int debuff = Main.rand.Next(8);

                switch (debuff)
                {
                    case 0:
                        target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180, false);
                        break;
                    case 2:
                        target.AddBuff(BuffID.ShadowFlame, 180, false);
                        break;
                    case 3:
                        target.AddBuff(ModContent.BuffType<Plague>(), 180, false);
                        break;
                    case 4:
                        target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180, false);
                        break;
                    case 5:
                        target.AddBuff(ModContent.BuffType<CrushDepth>(), 180, false);
                        break;
                    case 6:
                        target.AddBuff(ModContent.BuffType<Crumbling>(), 120, false);
                        break;
                    case 7:
                        target.AddBuff(BuffID.CursedInferno, 180, false);
                        break;
                }
            }
            else
            {
                int debuff = Main.rand.Next(11);

                switch (debuff)
                {
                    case 0:
                        target.AddBuff(ModContent.BuffType<MiracleBlight>(), 120, false);
                        break;
                    case 2:
                        target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, false);
                        break;
                    case 3:
                        target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 120, false);
                        break;
                    case 4:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120, false);
                        break;
                    case 5:
                        target.AddBuff(ModContent.BuffType<Laceration>(), 180, false);
                        break;
                    case 6:
                        target.AddBuff(ModContent.BuffType<ElementalMix>(), 60, false);
                        break;
                    case 7:
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 180, false);
                        break;
                    case 8:
                        target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180, false);
                        break;
                    case 9:
                        target.AddBuff(ModContent.BuffType<Dragonfire>(), 180, false);
                        break;
                    case 10:
                        target.AddBuff(ModContent.BuffType<Voidfrost>(), 180, false);
                        break;
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            TryCoinDebuff();
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Terraria.Player.HurtModifiers modifiers)
        {
            //Reverts vanilla trap damage nerf
            ref StatModifier sourceDamage = ref modifiers.SourceDamage;

            if (proj.type == ProjectileID.Explosives)
            {
                sourceDamage /= Main.expertMode ? 0.225f : 0.35f;
            }
            else if (proj.type == ProjectileID.RollingCactus || proj.type == ProjectileID.RollingCactusSpike)
            {
                sourceDamage /= Main.expertMode ? 0.3f : 0.5f;
            }

            if (!Main.expertMode)
                return;

            if (proj.type == ProjectileID.Boulder || proj.type == ProjectileID.MiniBoulder)
            {
                sourceDamage /= 0.65f;
            }
            else if (proj.type == ProjectileID.SpikyBallTrap || proj.type == ProjectileID.FlamethrowerTrap || proj.type == ProjectileID.PoisonDartTrap)
            {
                sourceDamage /= 0.625f;
            }
            else if (proj.type == ProjectileID.SpearTrap)
            {
                sourceDamage /= 0.6f;
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            TryCoinDebuff();
        }

        private void TryCoinDebuff()
        {
            if (bloodstainedCoin || putridCoin)
            {
                if (Main.rand.Next(4) != 0)
                {
                    if (putridCoin)
                        Player.AddBuff(BuffID.Poisoned, 1020, false);
                    if (bloodstainedCoin)
                        Player.AddBuff(BuffID.Bleeding, 1020, false);
                }
            }
        }
    }

    public class SoltanGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            int owner = projectile.owner;
            if (owner < 0 || owner >= Main.maxPlayers)
                return;

            Player p = Main.player[owner];
            var mp = p.GetModPlayer<InfernalPlayer>();
            if (!mp.soltanBullying)
                return;

            var summonMeleeSpeed = ModContent.GetInstance<SummonMeleeSpeedDamageClass>();
            if (projectile.DamageType == summonMeleeSpeed)
            {
                // Make the projectile actually "be" Melee so Melee-only effects can see it.
                projectile.DamageType = DamageClass.Melee;
                projectile.netUpdate = true;
            }
        }
    }
}
