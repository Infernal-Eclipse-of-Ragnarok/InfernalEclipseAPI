using System.Reflection;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace InfernalEclipseAPI.Core.Systems.Hooks.ILItemChanges.ThoriumItemHooks.CalBardHealerItemHooks
{
    [JITWhenModsEnabled(InfernalCrossmod.CalBardHealer.Name)]
    [ExtendsFromMod(InfernalCrossmod.CalBardHealer.Name)]
    public class ExoSoundNerf : ModSystem
    {
        public override void Load()
        {
            if (!ModLoader.TryGetMod("CalamityBardHealer", out var mod) || InfernalCrossmod.Hummus.Loaded) return;

            var exoSoundType = mod.Code.GetType("CalamityBardHealer.Projectiles.ExoSound");
            if (exoSoundType == null) return;

            var method = exoSoundType.GetMethod("BardOnHitNPC", BindingFlags.Public | BindingFlags.Instance);
            if (method == null) return;

            MonoModHooks.Modify(method, IL_BardOnHitNPC);
        }

        private void IL_BardOnHitNPC(ILContext il)
        {
            var c = new ILCursor(il);

            while (il.Instrs.Count > 0)
            {
                c.Index = 0;
                c.Remove();
            }

            c.Emit(OpCodes.Ret);
        }
    }
}
