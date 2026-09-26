using InfernalEclipseAPI.Core.Configs;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.Misc;
using ThoriumMod.NPCs;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Tiles;
using static Daybreak.Common.Features.Hooks.GlobalItemHooks;

namespace InfernalEclipseAPI.Core.Systems.Detours
{
    internal class ProjectileHealingHooks : ModSystem
    {
        public override void Load()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out var thorium) || !InfernalConfig.Instance.ThoriumBalanceChangess || InfernalCrossmod.Hummus.Loaded)
                return;

            // --- Patch the BrainCoralPro projectile ---
            var projType = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.BrainCoralPro");
            if (projType != null)
            {
                var aiMethod = projType.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (aiMethod != null)
                    MonoModHooks.Modify(aiMethod, IL_SetBrainCoralHeal);
            }

            var projType2 = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.CelestialWandPro");
            if (projType2 != null)
            {
                var aiMethod = projType2.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (aiMethod != null)
                    MonoModHooks.Modify(aiMethod, IL_SetCelestialWandHeal);
            }

            var projType3 = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.ChiLanternHeal");
            if (projType3 != null)
            {
                var aiMethod = projType3.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (aiMethod != null)
                    MonoModHooks.Modify(aiMethod, IL_SetChiLanternHeal);
            }

            var projType4 = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.EaterOfPainPro");
            if (projType4 != null)
            {
                var aiMethod = projType4.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (aiMethod != null)
                    MonoModHooks.Modify(aiMethod, IL_SetEaterOfPainHeal);
            }

            var gauzeProType = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.GauzePro");
            if (gauzeProType != null)
            {
                var gauzeProjAIMethod = gauzeProType.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (gauzeProjAIMethod != null)
                    MonoModHooks.Modify(gauzeProjAIMethod, IL_SetGauzeHealToFive);
            }

            var gigaNeedleItemType = thorium.Code.GetType("ThoriumMod.Items.HealerItems.TheGigaNeedle");
            if (gigaNeedleItemType != null)
            {
                var setDefaults = gigaNeedleItemType.GetMethod("SetDefaults", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (setDefaults != null)
                    MonoModHooks.Modify(setDefaults, IL_ReplaceGigaNeedleHealAmountLoad);
            }

            var gigaNeedleProType = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.TheGigaNeedlePro");
            if (gigaNeedleProType != null)
            {
                var aiMethod = gigaNeedleProType.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (aiMethod != null)
                    MonoModHooks.Modify(aiMethod, IL_ReplaceGigaNeedleHealAmountLoad);
            }

            var projType7 = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.RecoveryWandPro");
            if (projType7 != null)
            {
                var aiMethod = projType7.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (aiMethod == null)
                    MonoModHooks.Modify(aiMethod, IL_PatchRecoveryWandHeal);
            }

            var projType9 = thorium.Code.GetType("ThoriumMod.Projectiles.Healer.TheGoodBookPro");
            if (projType9 != null)
            {
                var aiMethod = projType.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (aiMethod != null)
                    MonoModHooks.Modify(aiMethod, IL_SetGoodBookHeal);
            }
        }

        #region Hooks
        private void IL_SetGoodBookHeal(ILContext il)
        {
            var c = new ILCursor(il);

            // Find the integer constant '2' used in ThoriumHeal call
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_2))
            {
                // Replace base heal 2 - 4
                c.Next.OpCode = OpCodes.Ldc_I4_4;
            }
        }

        private void IL_PatchRecoveryWandHeal(ILContext il)
        {
            var c = new ILCursor(il);

            // --- Patch base heal: 2 - 5 ---
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_2))
            {
                c.Next.OpCode = OpCodes.Ldc_I4_5;
            }

            // --- Patch out-of-combat bonus: 4 - 10 ---
            c.Index = 0; // Reset cursor to start
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_4))
            {
                // If the original delegate adds 4, replace with 10
                c.Next.OpCode = OpCodes.Ldc_I4_S;
                c.Next.Operand = (sbyte)10;
            }
        }

        private void IL_SetBrainCoralHeal(ILContext il)
        {
            var c = new ILCursor(il);

            // Look for the first Ldc_I4_3 and replace it with 5
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_3))
            {
                c.Next.OpCode = OpCodes.Ldc_I4_5;
            }
        }

        private void IL_SetCelestialWandHeal(ILContext il)
        {
            var c = new ILCursor(il);

            // Look for the integer 5 pushed before ThoriumHeal
            if (c.TryGotoNext(i => i.MatchLdcI4(5)))
            {
                c.Next.OpCode = OpCodes.Ldc_I4_1;
            }
            else
            {
                ModContent.GetInstance<Mod>().Logger.Warn(
                    "CelestialWandHealingPatch: failed to find heal constant 5.");
            }
        }

        private void IL_SetChiLanternHeal(ILContext il)
        {
            var c = new ILCursor(il);

            // Look for the call to ThoriumHeal
            if (c.TryGotoNext(i =>
                   i.OpCode == OpCodes.Call &&
                   i.Operand is MethodReference m &&
                   m.Name.Contains("ThoriumHeal")))
            {
                // Step back to the first integer pushed (the heal amount)
                int startIndex = c.Index;
                for (int i = startIndex - 1; i >= 0; i--)
                {
                    if (il.Instrs[i].OpCode == OpCodes.Ldc_I4_3) // originally 3
                    {
                        il.Instrs[i].OpCode = OpCodes.Ldc_I4_5; // change to 5
                        break;
                    }
                }
            }
        }

        private void IL_SetEaterOfPainHeal(ILContext il)
        {
            var c = new ILCursor(il);

            // Look for the first constant 2 and replace with 3
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_2))
            {
                c.Next.OpCode = OpCodes.Ldc_I4_3;
            }
        }

        private void IL_SetGauzeHealToFive(ILContext il)
        {
            var c = new ILCursor(il);

            // Look for the first integer constant '2' (Ldc_I4_2)
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4_2))
            {
                // Replace it with 5
                c.Next.OpCode = OpCodes.Ldc_I4_5;
            }
        }

        private static bool IsGigaNeedleHealAmountLoad(Instruction i)
        {
            return i.OpCode == OpCodes.Ldsfld &&
                   i.Operand is FieldReference fr &&
                   fr.DeclaringType.FullName == "ThoriumMod.Items.HealerItems.TheGigaNeedle" &&
                   fr.Name == "HealAmount";
        }

        private void IL_ReplaceGigaNeedleHealAmountLoad(ILContext il)
        {
            var c = new ILCursor(il);

            while (c.TryGotoNext(IsGigaNeedleHealAmountLoad))
            {
                c.Next.OpCode = OpCodes.Ldc_I4_6;
                c.Next.Operand = null;
            }
        }
        #endregion
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class SheathNerfHooks : ModSystem
    {
        private Hook leatherSheathHook;
        private Hook titanSheathHook;

        public override void Load()
        {
            Mod thorium = InfernalCrossmod.Thorium.Mod;

            var leatherSheathType = thorium.Code.GetType("ThoriumMod.Core.Sheaths.LeatherSheathData");
            if (leatherSheathType != null)
            {
                var getter = leatherSheathType.GetProperty(
                    "DamageMultiplier",
                    BindingFlags.Instance | BindingFlags.Public)
                    ?.GetGetMethod();

                if (getter != null)
                    leatherSheathHook = new Hook(getter, LeatherSheathDamageMult);
            }

            var titanSheathType = thorium.Code.GetType("ThoriumMod.Core.Sheaths.TitanSlayerSheathData");
            if (titanSheathType != null)
            {

                var getter = titanSheathType.GetProperty(
                    "DamageMultiplier",
                    BindingFlags.Instance | BindingFlags.Public)
                    ?.GetGetMethod();

                if (getter != null)
                    titanSheathHook = new Hook(getter, TitanSheathDamageMult);
            }
        }

        private float LeatherSheathDamageMult(object self)
        {
            return 6f;
        }

        private float TitanSheathDamageMult(object self)
        {
            return 14f;
        }

        public override void Unload()
        {
            leatherSheathHook?.Dispose();
            leatherSheathHook = null;

            titanSheathHook?.Dispose();
            titanSheathHook = null;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class TerrariansLastKnifeHealingCooldown : ModSystem
    {
        private static Hook lastKnifeOnHit = null;

        public override void OnModLoad()
        {
            if (!InfernalConfig.Instance.ThoriumBalanceChangess)
                return;

            Type terrariansLastKnife = InfernalCrossmod.Thorium.Mod.Code.GetType("ThoriumMod.Items.Donate.TerrariansLastKnife");
            MethodInfo orig = terrariansLastKnife.GetMethod("OnHitNPC", BindingFlags.Public | BindingFlags.Instance);
            lastKnifeOnHit = new Hook(orig, OnHitDetour);
        }

        public override void OnModUnload()
        {
            lastKnifeOnHit?.Dispose();
            lastKnifeOnHit = null;
        }

        private static void OnHitDetour(Action<ThoriumItem, Player, NPC, NPC.HitInfo, int> orig, ThoriumItem self, Player player, NPC target, NPC.HitInfo hitInfo, int damageDone)
        {
            int heal = (int)Math.Round(hitInfo.Damage * 0.04);
            if (heal > 100)
                heal = 100;

            if (player.altFunctionUse == 2 || !IsHostile(target) || heal <= 0 || player.moonLeech || player.lifeSteal <= 0f || target.lifeMax <= 5)
                return;

            player.lifeSteal -= heal;
            player.statLife += heal;
            player.HealEffect(heal);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            for (int index1 = 0; index1 < 15; ++index1)
            {
                int index2 = Dust.NewDust(player.position, player.width, player.height, DustID.LifeDrain, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity *= 0.75f;
                int num1 = Main.rand.Next(-55, 56);
                int num2 = Main.rand.Next(-55, 56);
                Main.dust[index2].position.X += (float)num1;
                Main.dust[index2].position.Y += (float)num2;
                Main.dust[index2].velocity.X = (float)(-(double)num1 * 0.075000002980232239);
                Main.dust[index2].velocity.Y = (float)(-(double)num2 * 0.075000002980232239);
            }
        }

        private static bool IsHostile(NPC npc, object attacker = null, bool ignoreDontTakeDamage = false)
        {
            return !npc.friendly && npc.lifeMax > 5 && npc.chaseable && !npc.dontTakeDamage | ignoreDontTakeDamage && !npc.immortal;
        }
    }

    //WH
    public class MoltenThresherPostDrawIL : ModSystem
    {
        private ILHook thresherPostDrawIL;

        public override void Load()
        {
            // Only run if Thorium is loaded
            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium) || ModLoader.TryGetMod("WHummusMultiModBalancing", out _))
                return;

            // Get Thorium's MoltenThresherPro type
            var projType = thorium.Code?.GetType("ThoriumMod.Projectiles.Scythe.MoltenThresherPro");
            if (projType == null)
                return;

            // Get the PostDraw method (instance, any visibility)
            var method = projType.GetMethod("PostDraw", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
                return;

            // Hook: insert an immediate 'ret' at the start so PostDraw does nothing
            thresherPostDrawIL = new ILHook(method, (il) =>
            {
                var c = new ILCursor(il);
                c.Goto(0);           // beginning of the method
                c.Emit(OpCodes.Ret); // return; (void method, safe to do)
            });
        }

        public override void Unload()
        {
            thresherPostDrawIL?.Dispose();
            thresherPostDrawIL = null;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class CapeoftheSurvivorNerfSystem : ModSystem
    {
        private ILHook _consumableDodgeIL;
        private ILHook _postUpdateEquipsIL;

        public override void Load()
        {
            var tp = typeof(ThoriumMod.ThoriumPlayer);

            // 1) Disable the “<= 1 damage” Cape dodge.
            var miConsumableDodge = tp.GetMethod("ConsumableDodge", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            _consumableDodgeIL = new ILHook(miConsumableDodge, PatchConsumableDodge);

            // 2) Reduce Cape DR gain so it caps at 0.15 (600 * 0.00025f).
            var miPostUpdateEquips = tp.GetMethod("PostUpdateEquips", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            _postUpdateEquipsIL = new ILHook(miPostUpdateEquips, PatchPostUpdateEquips);
        }

        public override void Unload()
        {
            _consumableDodgeIL?.Dispose();
            _postUpdateEquipsIL?.Dispose();
        }

        public override void PostSetupContent()
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<SmoothCoal>(), 1240, 0.275);
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<ThoriumOre>(), 1500, 0.4);
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<ThoriumMod.Tiles.LifeQuartz>(), 1550, 0.45);
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<Aquaite>(), 2000, 0.5, NPC.downedBoss2);
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<LodeStone>(), 2750, 0.55, ThoriumWorld.downedFallenBeholder);
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<ValadiumChunk>(), 2750, 0.55, ThoriumWorld.downedFallenBeholder);
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<IllumiteChunk>(), 4100, 1.25, NPC.downedPlantBoss);

                InfernalCrossmod.SOTS.Mod?.Call("AddHydroponicsHerb", ModContent.ItemType<ThoriumMod.Items.Depths.MarineKelp>(), ModContent.TileType<MarineKelp>(), 0, DustID.GrassBlades, 0f, 0f, 0f);
            }
        }

        private static void PatchConsumableDodge(ILContext il)
        {
            var c = new ILCursor(il);

            // Find the call to CapeoftheSurvivorDodge and back up to the constant "1" in the compare (Damage <= 1).
            if (c.TryGotoNext(MoveType.Before, i => i.MatchCallvirt(typeof(ThoriumMod.ThoriumPlayer).GetMethod("CapeoftheSurvivorDodge",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))))
            {
                if (c.TryGotoPrev(i => i.MatchLdcI4(1)))
                {
                    // Change `<= 1` to `<= 0` (effectively never triggers on normal hits)
                    c.Next.Operand = 0;
                }
            }
        }

        private static void PatchPostUpdateEquips(ILContext il)
        {
            var c = new ILCursor(il);

            // Replace the Cape DR increment coefficient: 0.000334f -> 0.00025f (so 600 ticks => 0.15 DR).
            if (c.TryGotoNext(i => i.MatchLdcR4(0.000334f)))
            {
                c.Next.Operand = 0.00025f;
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    internal sealed class BlackMIDIHealNerf : ModSystem
    {
        public static MethodInfo BlackMIDIProOnHitNPCMethod = typeof(BlackMIDIPro).GetMethod("BardOnHitNPC", LumUtils.UniversalBindingFlags);
        public delegate void Orig_BlackMIDIProOnHitNPCMethod(BlackMIDIPro self, NPC target, NPC.HitInfo hit, int damageDone);
        private static Hook BlackMIDIHealCooldown_Detour_Hook;

        public override void OnModLoad()
        {
            if (BlackMIDIProOnHitNPCMethod != null)
            {
                BlackMIDIHealCooldown_Detour_Hook = new(BlackMIDIProOnHitNPCMethod, OnHitNPC_Detour);
                BlackMIDIHealCooldown_Detour_Hook?.Apply();
            }
            else InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: " + this + " returned null on getting MethodInfo");
        }

        public void OnHitNPC_Detour(Orig_BlackMIDIProOnHitNPCMethod orig, BlackMIDIPro self, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[self.Projectile.owner];

            if (owner.lifeSteal <= 0f)
                return;

            orig(self, target, hit, damageDone);

            int healed = (int)(damageDone * 0.05);

            float cooldownMult = 0.1f;

            if (healed > 0)
                owner.lifeSteal -= healed * cooldownMult;

            if (owner.lifeSteal < -70f)
                owner.lifeSteal = -70f;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class HexingTalismanChanges : ModSystem
    {
        private static ILHook tooltipHook;
        private static ILHook updateLifeRegenHook;

        public override void Load()
        {
            MethodInfo tooltipGetter = typeof(HexingTalisman).GetProperty(nameof(HexingTalisman.Tooltip), LumUtils.UniversalBindingFlags)?.GetGetMethod();

            if (tooltipGetter is not null)
                tooltipHook = new ILHook(tooltipGetter, ModifyTooltip);

            MethodInfo method = typeof(ThoriumGlobalNPC).GetMethod(nameof(ThoriumGlobalNPC.UpdateLifeRegen), LumUtils.UniversalBindingFlags);

            if (method is not null)
                updateLifeRegenHook = new ILHook(method, ModifyUpdateLifeRegen);
        }

        private static void ModifyTooltip(ILContext il)
        {
            ILCursor cursor = new(il);

            // Original: HexingTalisman.DotDamage
            // Replace the value pushed onto the stack with 100.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<HexingTalisman>(nameof(HexingTalisman.DotDamage))))
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error("Could not locate HexingTalisman.DotDamage in Tooltip getter.");
                return;
            }

            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4, 100);
        }

        private static void ModifyUpdateLifeRegen(ILContext il)
        {
            ILCursor cursor = new(il);

            // Find the Hexing Talisman section specifically.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<ThoriumGlobalNPC>(nameof(ThoriumGlobalNPC.debuffNPCHexingTalisman))))
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error("Could not locate Hexing Talisman section in ThoriumGlobalNPC.UpdateLifeRegen.");
                return;
            }

            // Original:
            // npc.lifeRegen = (int)(npc.lifeRegen * 3.0);
            // New:
            // npc.lifeRegen = (int)(npc.lifeRegen * 2.0);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR8(3.0)))
            {
                ModContent.GetInstance<InfernalEclipseAPI>().Logger.Error("Could not locate Hexing Talisman's 3x DoT multiplier.");
                return;
            }

            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R8, 2.0);
        }

        public override void Unload()
        {
            tooltipHook?.Dispose();
            tooltipHook = null;

            updateLifeRegenHook?.Dispose();
            updateLifeRegenHook = null;
        }
    }
}
