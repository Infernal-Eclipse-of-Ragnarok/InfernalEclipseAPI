using CatalystMod;
using CatalystMod.Items.SummonItems;
using CatalystMod.NPCs;
using InfernalEclipseAPI.Core.Configs;
using InfernumMode.Content.BossIntroScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader.IO;

namespace InfernalEclipseAPI.Core.Systems.Detours
{
    internal sealed class IntroScreenManagerHook : ModSystem
    {
        private Hook drawHook;

        public override void Load()
        {
            MethodInfo drawMethod = typeof(IntroScreenManager).GetMethod(nameof(IntroScreenManager.Draw), LumUtils.UniversalBindingFlags);

            drawHook = new Hook(drawMethod, DrawDetour);
        }

        private static void DrawDetour(Action orig)
        {
            IntroScreenManager.UpdateScreens();

            foreach (BaseIntroScreen introScreen in IntroScreenManager.IntroScreens)
            {
                if (introScreen.ShouldBeActive())
                {
                    if (introScreen.AnimationTimer < introScreen.AnimationTime)
                    {
                        introScreen.Draw(Main.spriteBatch);
                        break;
                    }
                    else if (!introScreen.CaresAboutBossEffectCondition)
                        introScreen.AnimationTimer = 0;
                }
            }
        }

        public override void Unload()
        {
            drawHook?.Dispose();
            drawHook = null;
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.NoxusBoss.Name)]
    [ExtendsFromMod(InfernalCrossmod.NoxusBoss.Name)]
    internal class RevertWoTGBossRushRemoval : ModSystem
    {
        private Hook removeBossRushHook;

        public override void Load()
        {
            // Make sure NoxusBoss is loaded
            if (!ModLoader.TryGetMod("NoxusBoss", out Mod noxusBoss))
                return;

            // Type that contains RemoveBossRushFromChecklist
            Type compatType = noxusBoss.Code?.GetType("NoxusBoss.Core.CrossCompatibility.Inbound.BossChecklist.BossChecklistCompatibilitySystem");

            if (compatType == null)
                return;

            // private static void RemoveBossRushFromChecklist()
            MethodInfo removeBossRushMethod = compatType.GetMethod("RemoveBossRushFromChecklist", BindingFlags.NonPublic | BindingFlags.Static);

            if (removeBossRushMethod == null)
                return;

            // Detour it to a no-op
            removeBossRushHook = new Hook(removeBossRushMethod, RemoveBossRushDetour);
        }

        public override void Unload()
        {
            removeBossRushHook?.Dispose();
            removeBossRushHook = null;
        }

        private static void RemoveBossRushDetour(Action orig)
        {
        }
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class SolynCampsiteFix : ModSystem
    {
        internal static ILHook SurveyHook;

        public override void Load()
        {
            if (!ModLoader.HasMod("WOTGCampsiteFix"))
                TryApplyPatch();
        }

        public override void Unload()
        {
            SurveyHook?.Dispose();
            SurveyHook = null;
        }

        public override void PostSetupContent()
        {
            if (!ModLoader.HasMod("WOTGCampsiteFix"))
                TryApplyPatch();
        }

        internal void TryApplyPatch()
        {
            if (!ModLoader.TryGetMod("NoxusBoss", out Mod noxusBoss))
            {
                InfernalEclipseAPI.Instance.Logger.Info("[IEoR]: NoxusBoss not found — patch not applied.");
                return;
            }

            if (SurveyHook != null)
            {
                try
                {
                    SurveyHook.Dispose();
                }
                catch
                {
                }

                SurveyHook = null;
            }

            MethodInfo patchTarget = FindPatchTarget(noxusBoss);

            if (patchTarget is null)
            {
                InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: Could not find a patchable WOTG campsite method. Patch not applied.");
                return;
            }

            try
            {
                SurveyHook = new ILHook(patchTarget, PatchSolidTileCalls);

                InfernalEclipseAPI.Instance.Logger.Info($"[IEoR]: Patched {patchTarget.DeclaringType?.Name}.{patchTarget.Name} — flagpole crash protection active.");
            }
            catch (Exception ex)
            {
                InfernalEclipseAPI.Instance.Logger.Error("[IEoR]: ILHook failed: " + ex.Message);
            }
        }

        private static MethodInfo FindPatchTarget(Mod noxusBoss)
        {
            Type surveyType = noxusBoss.Code?.GetType("NoxusBoss.Core.World.WorldGeneration.SolynCampsiteSurvey");

            if (surveyType is not null)
            {
                foreach (MethodInfo method in surveyType.GetMethods(LumUtils.UniversalBindingFlags))
                {
                    if (method.Name == "TryToFindSpotInRange")
                        return method;
                }
            }

            return noxusBoss.Code?.GetType("NoxusBoss.Core.World.WorldGeneration.SolynCampsiteWorldGen")?.GetMethod("FlatTerrainExists", LumUtils.UniversalBindingFlags);
        }

        private static void PatchSolidTileCalls(ILContext il)
        {
            ILCursor cursor = new(il);

            MethodInfo solidTile3 = typeof(WorldGen).GetMethod(nameof(WorldGen.SolidTile), LumUtils.UniversalBindingFlags, null,
                new[]
                {
                    typeof(int),
                    typeof(int),
                    typeof(bool)
                },
                null
            );

            MethodInfo solidTile2 = typeof(WorldGen).GetMethod(nameof(WorldGen.SolidTile), LumUtils.UniversalBindingFlags,
                null,
                new[]
                {
                    typeof(int),
                    typeof(int)
                },
                null
            );

            if (solidTile3 is not null)
            {
                cursor.Index = 0;

                while (cursor.TryGotoNext(MoveType.Before, i => i.MatchCall(solidTile3)))
                {
                    cursor.Remove();
                    cursor.EmitDelegate<Func<int, int, bool, bool>>(SafeSolidTile3);
                }
            }

            if (solidTile2 is not null)
            {
                cursor.Index = 0;

                while (cursor.TryGotoNext(MoveType.Before, i => i.MatchCall(solidTile2)))
                {
                    cursor.Remove();
                    cursor.EmitDelegate<Func<int, int, bool>>(SafeSolidTile2);
                }
            }
        }

        private static bool SafeSolidTile3(int i, int j, bool noDoors) => i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && WorldGen.SolidTile(i, j, noDoors);
        private static bool SafeSolidTile2(int i, int j) => i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && WorldGen.SolidTile(i, j, false);
    }

    [JITWhenModsEnabled("CatalystMod")]
    [ExtendsFromMod("CatalystMod")]
    public class AstralCommunicatorAnytime : ModSystem
    {
        // Target methods (cached via reflection)
        private static readonly MethodBase _communicatorCanUseItem = typeof(AstralCommunicator).GetMethod(nameof(AstralCommunicator.CanUseItem));
        private static readonly MethodBase _communicatorTooltip = typeof(AstralCommunicator).GetMethod(nameof(AstralCommunicator.ModifyTooltips));
        private static readonly MethodBase _communicatorPreDrawInventory = typeof(AstralCommunicator).GetMethod(nameof(AstralCommunicator.PreDrawInInventory));
        private static readonly MethodBase _communicatorPreDrawWorld = typeof(AstralCommunicator).GetMethod(nameof(AstralCommunicator.PreDrawInWorld));
        private static readonly MethodBase _playerUpdateEquips = typeof(CatalystPlayer).GetMethod(nameof(CatalystPlayer.PostUpdateEquips));
        private static readonly MethodBase _playerLoadData = typeof(CatalystPlayer).GetMethod(nameof(CatalystPlayer.LoadData));
        private static readonly MethodBase _npcPreKill = typeof(CatalystNPC).GetMethod(nameof(CatalystNPC.PreKill));
        private static readonly PropertyInfo _summonAstrageldonProp = typeof(CatalystPlayer).GetProperty("SummonAstrageldon", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly MethodInfo _summonAstrageldonSetter = _summonAstrageldonProp?.GetSetMethod(true);

        public override void Load()
        {
            // Hooks (MonoMod.RuntimeDetour.HookGen style)
            MonoModHooks.Add(_communicatorCanUseItem, CanUseCheck);
            MonoModHooks.Add(_communicatorTooltip, TooltipCheck);
            MonoModHooks.Add(_communicatorPreDrawInventory, InventoryDraw);
            MonoModHooks.Add(_communicatorPreDrawWorld, (hook_CommunicatorPreDrawWorld)WorldDraw);
            MonoModHooks.Add(_playerUpdateEquips, EquipsCheck);
            MonoModHooks.Add(_playerLoadData, ForceData);
            MonoModHooks.Add(_npcPreKill, PreKillCheck);
        }

        // --- Hook implementations ---
        // Allow use even if Moon Lord isn't downed (temporarily spoof the flag to false).
        private static bool CanUseCheck(orig_CommunicatorCanUseItem orig, AstralCommunicator self, Player player)
        {
            bool prev = NPC.downedMoonlord;
            NPC.downedMoonlord = false;
            try
            {
                return orig(self, player);
            }
            finally
            {
                NPC.downedMoonlord = prev;
            }
        }

        // Force tooltip path that assumes Astrageldon is downed (temporarily spoof to true).
        private static void TooltipCheck(orig_CommunicatorTooltip orig, AstralCommunicator self, List<TooltipLine> tooltips)
        {
            bool prev = WorldDefeats.downedAstrageldon;
            WorldDefeats.downedAstrageldon = true;
            try
            {
                orig(self, tooltips);
            }
            finally
            {
                WorldDefeats.downedAstrageldon = prev;
            }
        }

        // Inventory draw should behave as if Moon Lord is not downed.
        private static bool InventoryDraw(orig_CommunicatorPreDrawInventory orig, AstralCommunicator self, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            bool prev = NPC.downedMoonlord;
            NPC.downedMoonlord = false;
            try
            {
                return orig(self, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            }
            finally
            {
                NPC.downedMoonlord = prev;
            }
        }

        // World draw should behave as if Moon Lord is not downed.
        private static bool WorldDraw(orig_CommunicatorPreDrawWorld orig, AstralCommunicator self, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool prev = NPC.downedMoonlord;
            NPC.downedMoonlord = false;
            try
            {
                return orig(self, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
            }
            finally
            {
                NPC.downedMoonlord = prev;
            }
        }

        // Player equips update should ignore Moon Lord downed gate.
        private static void EquipsCheck(orig_PlayerEquips orig, CatalystPlayer self)
        {
            bool prev = NPC.downedMoonlord;
            NPC.downedMoonlord = false;
            try
            {
                orig(self);
            }
            finally
            {
                NPC.downedMoonlord = prev;
            }
        }

        // When loading data, force SummonAstrageldon = true via the (possibly non-public) setter.
        private static void ForceData(orig_PlayerLoadData orig, CatalystPlayer self, TagCompound tag)
        {
            orig(self, tag);
            _summonAstrageldonSetter?.Invoke(self, new object[] { true });
        }

        // PreKill should behave as if Moon Lord IS downed (to allow drops/logic gated behind it).
        private static bool PreKillCheck(orig_NPCPreKill orig, CatalystNPC self, NPC npc)
        {
            bool prev = NPC.downedMoonlord;
            NPC.downedMoonlord = true;
            try
            {
                return orig(self, npc);
            }
            finally
            {
                NPC.downedMoonlord = prev;
            }
        }

        // --- Orig delegate signatures expected by HookGen ---
        private delegate bool orig_CommunicatorCanUseItem(AstralCommunicator self, Player player);
        private delegate void orig_CommunicatorTooltip(AstralCommunicator self, List<TooltipLine> tooltips);
        private delegate bool orig_CommunicatorPreDrawInventory(AstralCommunicator self, SpriteBatch spriteBatch,  Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale);
        private delegate bool orig_CommunicatorPreDrawWorld( AstralCommunicator self, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI);
        private delegate bool hook_CommunicatorPreDrawWorld(orig_CommunicatorPreDrawWorld orig, AstralCommunicator self, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI);
        private delegate void orig_PlayerEquips(CatalystPlayer self);
        private delegate void orig_PlayerLoadData(CatalystPlayer self, TagCompound tag);
        private delegate bool orig_NPCPreKill(CatalystNPC self, NPC npc);
    }
}
