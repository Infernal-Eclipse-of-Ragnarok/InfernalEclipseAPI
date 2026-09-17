using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.SlimeGod;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using SOTS;
using SOTS.Items.ChestItems;
using SOTS.Items.Void;
using SOTS.Projectiles;
using SOTS.Projectiles.BiomeChest;
using SOTS.Void;
using System.Collections.Generic;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Core.Systems.Detours
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class FloweringBudHook : ModSystem
    {
        private Hook floweringBudOnHitHook;

        public override void Load()
        {
            MethodInfo method = typeof(FloweringBud).GetMethod(nameof(FloweringBud.OnHitNPC), LumUtils.UniversalBindingFlags);

            floweringBudOnHitHook = new Hook(method, FloweringBud_OnHitNPC);
        }

        private delegate void Orig_OnHitNPC(FloweringBud self, NPC target, NPC.HitInfo hit, int damageDone);

        private static void FloweringBud_OnHitNPC(Orig_OnHitNPC orig, FloweringBud self, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss)
            {
                // Preserve the immunity reset from the original hit, but do NOT latch the FloweringBud onto the boss.
                target.immune[self.Projectile.owner] = 0;
                return;
            }

            orig(self, target, hit, damageDone);
        }

        public override void Unload()
        {
            floweringBudOnHitHook?.Dispose();
            floweringBudOnHitHook = null;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class ChallengerRingNerfs : ModSystem
    {
        private static Hook diamandRingEffectHook;

        public override void Load()
        {
            Mod sots = ModLoader.GetMod("SOTS");
            var sotsPlayerType = sots.Code.GetType("SOTS.SOTSPlayer");
            var updateEquips = sotsPlayerType.GetMethod("UpdateEquips", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            diamandRingEffectHook = new Hook(updateEquips, UpdateEquips_Detour);
        }

        public override void Unload()
        {
            diamandRingEffectHook?.Dispose();
            diamandRingEffectHook = null;
        }

        private static void UpdateEquips_Detour(Action<SOTSPlayer> orig, SOTSPlayer self)
        {
            Player player = self.Player;

            int def = player.statDefense;
            if (def > 15)
                def = 15;

            self.previousDefense = def;

            if (self.DiamondRing)
            {
                player.statDefense -= (int)(def * 0.66f);

                player.GetDamage(DamageClass.Generic) += def * 0.01f;
            }

            self.DiamondRing = false;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class ElementalAmuletNerfs : ModSystem
    {
        private Hook getBonusesHook;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("SecretsOfTheSouls");
        }

        public override void Load()
        {
            var type = InfernalCrossmod.SOTS.Mod.Code.GetType("SOTS.Items.AbandonedVillage.VisionAmulet");
            var mi = type?.GetMethod("GetBonuses", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (mi == null)
            {
                InfernalEclipseAPI.Instance.Logger.Warn("Failed to find Vision Amulet's GetBonuses method for hooking. Elemental Amulet nerfs will not be applied.");
                return;
            }

            getBonusesHook = new Hook(mi, GetBonuses_Hook);
        }

        public override void Unload()
        {
            getBonusesHook?.Dispose();
            getBonusesHook = null;
        }

        private static void GetBonuses_Hook(Player player, int gem, int frame)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            InfernalPlayer infernalPlayer = player.GetModPlayer<InfernalPlayer>();

            switch (gem)
            {
                case 0:
                    player.endurance += 0.1f;
                    break;
                case 1:
                    ++player.maxMinions;
                    ++player.maxTurrets;
                    break;
                case 2:
                    sotsPlayer.attackSpeedMod += 0.08f;
                    break;
                case 3:
                    player.GetCritChance(DamageClass.Generic) += Main.hardMode ? 8f : 4f;
                    break;
                case 4:
                    sotsPlayer.CritBonusMultiplier += Main.hardMode ? 0.12f : 0.08f;
                    break;
                case 5:
                    sotsPlayer.additionalHeal += 40;
                    player.lifeRegen += 2;
                    break;
                case 6:
                    player.statLifeMax2 += 20;
                    player.GetDamage(DamageClass.Generic) += Main.hardMode ? 0.1f : 0.05f;
                    break;
                case 7:
                    voidPlayer.voidRegenSpeed += 0.2f;
                    break;
            }

            switch (frame)
            {
                case 0:
                    player.discountAvailable = true;
                    break;
                case 1:
                    player.manaCost -= Main.hardMode ? 0.2f : 0.15f;
                    break;
                case 2:
                    player.jumpSpeedBoost += 2f;
                    player.moveSpeed += 0.08f;
                    player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
                    break;
                case 3:
                    if (Main.hardMode)
                        sotsPlayer.LazyCrafterAmulet = true;
                    else
                        infernalPlayer.LazyCrafterAmulet = true;

                    sotsPlayer.additionalPotionMana += 40;
                    player.statManaMax2 += 40;
                    break;
                case 4:
                    infernalPlayer.statShareAll = true;
                    break;
                case 5:
                    infernalPlayer.scalingArmorPenetration = true;
                    break;
                case 6:
                    voidPlayer.voidGainMultiplier += 0.2f;
                    player.GetDamage<VoidGeneric>() += 0.1f;
                    break;
            }
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class GelWingsRebalanceHook : ModSystem
    {
        private Hook verticalSpeedsHook;
        private Hook updateAccessoryHook;

        private static WingStats NewStats = new WingStats(60, 7.5f, 2f);

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out var sots) || !InfernalConfig.Instance.SOTSBalanceChanges)
                return;

            Type gelWingsType = sots.Code.GetType("SOTS.Items.Slime.GelWings", throwOnError: true);
            if (gelWingsType is null)
                return;

            MethodInfo vertical = gelWingsType.GetMethod("VerticalWingSpeeds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            MethodInfo updateAcc = gelWingsType.GetMethod("UpdateAccessory", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //if (vertical != null)
                //verticalSpeedsHook = new Hook(vertical, (VerticalWingSpeedsDetour)Detour_VerticalWingSpeeds);

            if (updateAcc != null)
                updateAccessoryHook = new Hook(updateAcc, (UpdateAccessoryDetour)Detour_UpdateAccessory);
        }

        public override void Unload()
        {
            verticalSpeedsHook?.Dispose();
            verticalSpeedsHook = null;

            updateAccessoryHook?.Dispose();
            updateAccessoryHook = null;
        }

        public override void PostSetupContent()
        {
            if (!InfernalConfig.Instance.SOTSBalanceChanges)
                return;

            ModItem gelItem = ModContent.Find<ModItem>("SOTS", "GelWings");
            if (gelItem?.Item is null || gelItem.Item.wingSlot < 0)
                return;

            ArmorIDs.Wing.Sets.Stats[gelItem.Item.wingSlot] = NewStats;
        }

        // ---------------- HOOKS ----------------
        private delegate void Orig_VerticalWingSpeeds( ModItem self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        private delegate void VerticalWingSpeedsDetour(Orig_VerticalWingSpeeds orig, ModItem self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);

        private static void Detour_VerticalWingSpeeds(Orig_VerticalWingSpeeds orig, ModItem self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            // Call original (optional), then override.
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);

            // Skyline defaults: 0.5 / 0.1 / 0.5 / 1.5 / 0.1
            ascentWhenFalling = 0.42f;
            ascentWhenRising = 0.08f;
            maxCanAscendMultiplier = 0.45f;
            maxAscentMultiplier = 1.40f;
            constantAscend = 0.085f;
        }

        private delegate void Orig_UpdateAccessory(ModItem self, Player player, bool hideVisual);
        private delegate void UpdateAccessoryDetour(Orig_UpdateAccessory orig, ModItem self, Player player, bool hideVisual);
        private static void Detour_UpdateAccessory(Orig_UpdateAccessory orig, ModItem self, Player player, bool hideVisual)
        {
            orig(self, player, hideVisual);

            if (self?.Item != null && self.Item.wingSlot >= 0)
                player.wingTimeMax = ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot].FlyTime;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class HarmonyBlacklistHook : ModSystem
    {
        private static ILHook _hook;

        private static bool ShouldEarlyReturn(Player player)
        {
            if (InfernalCrossmod.Thorium.Loaded)
            {
                Mod thor = InfernalCrossmod.Thorium.Mod;

                int[] thorBlacklist =
                [
                    thor.Find<ModBuff>("ScytheofUndoingBuff").Type
                ];

                for (int i = 0; i < player.buffType.Length; i++)
                {
                    int buffType = player.buffType[i];
                    if (buffType <= 0)
                        continue;

                    for (int j = 0; j < thorBlacklist.Length; j++)
                    {
                        if (buffType == thorBlacklist[j])
                            return true;
                    }
                }
            }

            return false;
        }

        public override void Load()
        {
            if (!ModLoader.HasMod("SOTS"))
                return;

            try
            {
                Assembly sotsAsm = ModLoader.GetMod("SOTS").Code;

                Type sotsPlayerType = sotsAsm.GetType("SOTS.SOTSPlayer", throwOnError: false);
                if (sotsPlayerType is null)
                    return;

                MethodInfo target = sotsPlayerType.GetMethod("IncreaseBuffDurations",BindingFlags.Public | BindingFlags.Static, binder: null,
                    types: new[]
                    {
                        typeof(Terraria.Player),
                        typeof(int),
                        typeof(float),
                        typeof(int),
                        typeof(bool),
                        typeof(bool),
                        typeof(bool),
                    },
                    modifiers: null);

                if (target is null)
                    return;

                _hook = new ILHook(target, InjectEarlyReturn);
            }
            catch
            {
                _hook = null;
            }
        }

        public override void Unload()
        {
            _hook?.Dispose();
            _hook = null;
        }

        private static void InjectEarlyReturn(ILContext il)
        {
            var c = new ILCursor(il);
            c.Goto(0);

            Instruction continueOriginal = c.Next;

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, typeof(HarmonyBlacklistHook).GetMethod(nameof(ShouldEarlyReturn), BindingFlags.NonPublic | BindingFlags.Static));
            c.Emit(OpCodes.Brfalse_S, continueOriginal);
            c.Emit(OpCodes.Ret);
        }
    }

    //Wardrobe Hummus
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class PlatinumDartBlacklistHook : ModSystem
    {
        public override void Load()
        {
            if (!ModLoader.TryGetMod(InfernalCrossmod.SOTS.Name, out var sots))
                return;

            var dartType = sots.Code.GetType("SOTS.Projectiles.Ores.PlatinumDart");
            if (dartType == null)
                return;

            var onHit = dartType.GetMethod("OnHitNPC", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (onHit != null)
                MonoModHooks.Modify(onHit, IL_BlockBossLatch);
        }

        private static void IL_BlockBossLatch(ILContext il)
        {
            var c = new ILCursor(il);

            /*
             * We want to replace:
             *   latch = true;
             *
             * With:
             *   if (!CanLatch(target)) return;
             *   latch = true;
             */

            if (!c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_1, i => i.OpCode == OpCodes.Stfld && i.Operand is FieldReference fr && fr.Name == "latch"))
                return;

            // Move cursor BEFORE ldc.i4.1
            c.Index--;

            // Load NPC target (arg1)
            c.Emit(OpCodes.Ldarg_1);

            // Call CanLatch via delegate (SAFE)
            c.EmitDelegate<Func<NPC, bool>>(npc =>
            {
                return CanLatch(npc);
            });

            // If false -> return from OnHitNPC
            c.Emit(OpCodes.Brtrue_S, c.Next); // allow latch
            c.Emit(OpCodes.Ret);
        }

        private static readonly HashSet<int> NPCBlacklist = new()
        {
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,

            NPCID.TheDestroyer,
            NPCID.TheDestroyerBody,
            NPCID.TheDestroyerTail,

            // Perforators
            ModContent.NPCType<PerforatorHeadSmall>(),
            ModContent.NPCType<PerforatorBodySmall>(),
            ModContent.NPCType<PerforatorTailSmall>(),

            ModContent.NPCType<PerforatorHeadMedium>(),
            ModContent.NPCType<PerforatorBodyMedium>(),
            ModContent.NPCType<PerforatorTailMedium>(),

            ModContent.NPCType<PerforatorHeadLarge>(),
            ModContent.NPCType<PerforatorBodyLarge>(),
            ModContent.NPCType<PerforatorTailLarge>(),

            // Slime God
            ModContent.NPCType<CrimulanPaladin>(),
            ModContent.NPCType<EbonianPaladin>(),
            ModContent.NPCType<SplitCrimulanPaladin>(),
            ModContent.NPCType<SplitEbonianPaladin>(),

            // Profaned Guardians
            ModContent.NPCType<ProfanedGuardianCommander>(),
            ModContent.NPCType<ProfanedGuardianCommander>(),
            ModContent.NPCType<ProfanedGuardianHealer>(),
        };


        /// <summary>
        /// Determines whether Platinum Dart is allowed to latch onto the NPC.
        /// </summary>
        private static bool CanLatch(NPC npc)
        {
            if (npc == null)
                return false;

            // Vanilla + modded boss flag
            if (npc.boss)
                return false;

            // Explicit blacklist
            if (NPCBlacklist.Contains(npc.type))
                return false;

            return true;
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public sealed class RubyRingNerfs : ModSystem
    {
        private Hook onPickupHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out Mod sots))
                return;

            Type sotsItemType = sots.Code.GetType("SOTS.SOTSItem");
            if (sotsItemType is null)
                return;

            MethodInfo onPickupMethod = sotsItemType.GetMethod("OnPickup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(Item), typeof(Player) }, null);

            if (onPickupMethod is null)
                return;

            onPickupHook = new Hook(onPickupMethod, SOTSOnPickup_Override);
        }

        public override void Unload()
        {
            onPickupHook?.Dispose();
            onPickupHook = null;
        }

        private delegate bool Orig_OnPickup(object self, Item item, Player player);

        private static bool SOTSOnPickup_Override(Orig_OnPickup orig, object self, Item item, Player player)
        {
            // Do nothing.
            // GlobalItem.OnPickup's default behavior is effectively "allow pickup",
            // so just return true instead of calling the original SOTS logic.
            return true;
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class ShatteredDreamsNerfSystem : ModSystem
    {
        private Hook castWishingStarHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out Mod sots))
                return;

            Type sotsPlayerType = sots.Code.GetType("SOTS.SOTSPlayer");
            MethodInfo castWishingStarMethod = sotsPlayerType?.GetMethod("CastWishingStar", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (castWishingStarMethod != null)
                castWishingStarHook = new Hook(castWishingStarMethod, CastWishingStar_Hook);
        }

        public override void Unload()
        {
            castWishingStarHook?.Dispose();
            castWishingStarHook = null;
        }

        private delegate void Orig_CastWishingStar(Player player, Vector2 target, int damage);

        private static void CastWishingStar_Hook(Orig_CastWishingStar orig, Player player, Vector2 target, int damage)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);

            if (!WishingStar.IsAlternate)
                orig(player, target, damage);

            if (player.ownedProjectileCounts[ModContent.ProjectileType<WishingStarProj>()] < 3)
                orig(player, target, damage);
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class SubspaceBoosterDashAdjustment : ModSystem
    {
        private static Hook _hook;
        private static Type _subspaceBoostersType;
        private static FieldInfo _hasActivateField;
        private static FieldInfo _doAccelField;
        private static MethodInfo _fireBoostPlayerMethod;

        private static int _subspaceBoosterProjType = -1;

        private delegate void Orig_UpdateAccessory(object self, Player player, bool hideVisual);

        public override void Load()
        {
            if (!ModLoader.HasMod("SOTS"))
                return;

            var sots = ModLoader.GetMod("SOTS");
            _subspaceBoostersType = sots?.Code?.GetType("SOTS.Items.SubspaceBoosters");
            if (_subspaceBoostersType is null)
                return;

            var updateAccessory = _subspaceBoostersType.GetMethod("UpdateAccessory", BindingFlags.Instance | BindingFlags.Public);

            if (updateAccessory is null)
                return;

            _hasActivateField = _subspaceBoostersType.GetField("hasActivate", BindingFlags.Instance | BindingFlags.NonPublic);
            _doAccelField = _subspaceBoostersType.GetField("doAccel", BindingFlags.Instance | BindingFlags.NonPublic);
            _fireBoostPlayerMethod = _subspaceBoostersType.GetMethod("FireBoostPlayer", BindingFlags.Instance | BindingFlags.Public);

            if (_hasActivateField is null || _doAccelField is null || _fireBoostPlayerMethod is null)
                return;

            if (ModContent.TryFind<ModProjectile>("SOTS", "SubspaceBoosterProj", out var proj))
                _subspaceBoosterProjType = proj.Type;

            _hook = new Hook(updateAccessory, Detour_UpdateAccessory);
        }

        public override void Unload()
        {
            _hook?.Dispose();
            _hook = null;

            _subspaceBoostersType = null;
            _hasActivateField = null;
            _doAccelField = null;
            _fireBoostPlayerMethod = null;

            _subspaceBoosterProjType = -1;
        }

        private static void Detour_UpdateAccessory(Orig_UpdateAccessory orig, object self, Player player, bool hideVisual)
        {
            player.buffImmune[67] = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 600;
            player.rocketBoots = player.vanityRocketBoots = 4;
            player.iceSkate = true;
            player.moveSpeed += 0.2f;
            player.accRunSpeed = 7f;

            int num1 = 0;
            try
            {
                bool fireBoost = (bool)_fireBoostPlayerMethod.Invoke(self, new object[] { player });
                num1 = fireBoost ? 1 : 0;
            }
            catch
            {
                num1 = 0;
            }

            bool flag = false;
            int num2 = 0;

            var mp = player.GetModPlayer<InfernalPlayer>();
            if (mp is not null && mp.BoostPressTimer > 0)
            {
                num2 = mp.BoostDirection;
                if (num2 == 0)
                    num2 = player.direction;

                // clamp to -1/1
                num2 = num2 > 0 ? 1 : -1;

                flag = true;
            }

            if (Main.myPlayer == player.whoAmI)
            {
                int hasActivate = GetHasActivate(self);

                if (hasActivate == -1 && flag && player.velocity.Y == 0f)
                {
                    if (_subspaceBoosterProjType != -1)
                    {
                        Projectile.NewProjectile(player.GetSource_Misc("SOTS:SubspaceBoosterMultiplayerSync"), player.Center, Vector2.Zero, _subspaceBoosterProjType, 0, 0f, Main.myPlayer, num2, 0f, 0f);
                    }

                    SetHasActivate(self, 60);
                    hasActivate = 60;
                }

                if (hasActivate > -1)
                    SetHasActivate(self, hasActivate - 1);
            }

            bool doAccel = GetDoAccel(self);

            if (num1 == 0 || !doAccel)
                return;

            player.accRunSpeed = 0f;
            player.velocity *= 1.04166675f;
        }

        private static int GetHasActivate(object self)
        {
            try
            {
                return (int)_hasActivateField.GetValue(self);
            }
            catch
            {
                return -1;
            }
        }

        private static void SetHasActivate(object self, int value)
        {
            try
            {
                _hasActivateField.SetValue(self, value);
            }
            catch
            {
            }
        }

        private static bool GetDoAccel(object self)
        {
            try
            {
                return (bool)_doAccelField.GetValue(self);
            }
            catch
            {
                return false;
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class VoidSicknessDetour : ModSystem
    {
        private static Hook onConsumeHook;
        private Hook sealedUpdateInventoryHook;

        public override void Load()
        {
            if (!InfernalCrossmod.SOTS.Loaded)
                return;

            MethodInfo m = typeof(VoidConsumable).GetMethod("OnConsumeItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (m is null)
                throw new MissingMethodException("SOTS.Items.Void.VoidConsumable.OnConsumeItem(Player) not found.");

            onConsumeHook = new Hook(m, OnConsumeItem_Detour);

            MethodInfo sealedUpdateInventoryMethod = typeof(VoidConsumable).GetMethod(nameof(VoidConsumable.SealedUpdateInventory), BindingFlags.Instance | BindingFlags.Public);

            if (sealedUpdateInventoryMethod is null)
                throw new Exception("Failed to find VoidConsumable.SealedUpdateInventory for detour.");

            sealedUpdateInventoryHook = new Hook(sealedUpdateInventoryMethod, HookSealedUpdateInventory);
        }

        public override void Unload()
        {
            onConsumeHook?.Dispose();
            onConsumeHook = null;

            sealedUpdateInventoryHook?.Dispose();
            sealedUpdateInventoryHook = null;

        }

        private static void OnConsumeItem_Detour(Action<VoidConsumable, Player> orig, VoidConsumable self, Player player)
        {
            orig(self, player);

            if (player?.active == true)
            {
                if (self.Type == ModContent.ItemType<Taco>())
                    player.AddBuff(ModContent.BuffType<VoidSickness2>(), 600);
                else
                    player.AddBuff(ModContent.BuffType<VoidSickness2>(), 300);
            }
        }

        private delegate void Orig_SealedUpdateInventory(VoidConsumable self, Player player);
        private static void HookSealedUpdateInventory(Orig_SealedUpdateInventory orig, VoidConsumable self, Player player)
        {
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            int num1 = 0;
            int num2 = voidPlayer.voidMeterMax2 - voidPlayer.lootingSouls - voidPlayer.VoidMinionConsumption - self.GetVoidAmt();

            if (player.HasBuff(ModContent.BuffType<VoidSickness2>()))
            {
                int buffIndex = player.FindBuffIndex(ModContent.BuffType<VoidSickness2>());

                if (buffIndex != -1 && player.buffTime[buffIndex] >=  10 * 60)
                {
                    if (player.GetModPlayer<InfernalPlayer>().voidSicknessTextCooldown <= 0)
                    {
                        CombatText.NewText(player.Hitbox, Color.Lerp(Color.Red, Color.Magenta, 0.5f), Language.GetTextValue("Mods.InfernalEclipseAPI.UI.NoVoidConsumable"), true);
                        player.GetModPlayer<InfernalPlayer>().voidSicknessTextCooldown = 60 * 5;
                    }
                    return;
                }
            }

            while (player?.active == true && voidPlayer.voidMeter <= num1 && num2 >= 0 && self.Item.stack > 0 && self.CanUseItem(player))
            {
                self.Activate(player);
                if (self.Type == ModContent.ItemType<Taco>())
                    player.AddBuff(ModContent.BuffType<VoidSickness2>(), 600);
                else
                    player.AddBuff(ModContent.BuffType<VoidSickness2>(), 300);
            }
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public sealed class VorpalKnifeChaosState : ModSystem
    {
        private Hook altFunctionUseHook;
        private Hook teleportPlayerHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out Mod sots))
                return;

            Type vorpalKnifeType = sots.Code.GetType("SOTS.Items.Invidia.VorpalKnife");
            Type vorpalThrowType = sots.Code.GetType("SOTS.Projectiles.Blades.VorpalThrow");

            if (vorpalKnifeType is null || vorpalThrowType is null)
                return;

            MethodInfo altFunctionUse = vorpalKnifeType.GetMethod("AltFunctionUse", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            MethodInfo teleportPlayer = vorpalThrowType.GetMethod("TeleportPlayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (altFunctionUse is not null)
                altFunctionUseHook = new Hook(altFunctionUse, BlockChaosTeleportUse);

            if (teleportPlayer is not null)
                teleportPlayerHook = new Hook(teleportPlayer, ApplyChaosStateOnTeleport);
        }

        public override void Unload()
        {
            altFunctionUseHook?.Dispose();
            teleportPlayerHook?.Dispose();

            altFunctionUseHook = null;
            teleportPlayerHook = null;
        }

        private delegate bool Orig_AltFunctionUse(object self, Player player);

        private static bool BlockChaosTeleportUse(Orig_AltFunctionUse orig, object self, Player player)
        {
            if (player.HasBuff(BuffID.ChaosState))
                return false;

            return orig(self, player);
        }

        private delegate void Orig_TeleportPlayer(object self, Player player);

        private static void ApplyChaosStateOnTeleport(Orig_TeleportPlayer orig, object self, Player player)
        {
            if (player.HasBuff(BuffID.ChaosState))
                return;

            orig(self, player);

            player.AddBuff(BuffID.ChaosState, 20 * 60);
        }
    }
}