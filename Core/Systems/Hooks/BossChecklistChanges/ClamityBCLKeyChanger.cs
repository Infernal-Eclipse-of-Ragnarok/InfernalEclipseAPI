using System.Reflection;
using Clamity;
using Clamity.Content.Biomes.FrozenHell.Items;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace InfernalEclipseAPI.Core.Systems.Hooks.BossChecklistChanges
{
    [JITWhenModsEnabled("Clamity")]
    [ExtendsFromMod("Clamity")]
    public class ClamityBCLKeyChanger : ModSystem 
    {
     /*   private ILHook ilHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("Clamity", out Mod clamity))
                return;

            var wrType = clamity.Code.GetType("Clamity.Commons.SetupWeakReferences");
            if (wrType == null)
                return;

            var method = wrType.GetMethod("SetupBossChecklist", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
                return;

            ilHook = new ILHook(method, EditPyrogenKey);
        }

        public override void Unload()
        {
            ilHook?.Dispose();
            ilHook = null;
        }
     */
        public override void PostSetupContent()
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<FrozenHellstoneTile>(), 11350, 1.35, ClamitySystem.downedWallOfBronze);
            }
        }

    //    private void EditPyrogenKey(ILContext il)
    //    {
    //        var c = new ILCursor(il);
//
    //        // Find "Pyrogen" string
    //        while (c.TryGotoNext(MoveType.After, instr => instr.MatchLdstr("Pyrogen")))
    //        {
    //            // The next instruction should be ldc.r4 8.5 (the float value)
    //            if (c.Next != null && c.Next.MatchLdcR4(8.5f))
    //            {
    //                c.Next.Operand = 8.51f;
    //            }
    //        }
    //    }
    }
}
