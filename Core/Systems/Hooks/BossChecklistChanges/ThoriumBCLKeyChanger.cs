using MonoMod.Cil;
using System.Reflection;
using MonoMod.RuntimeDetour;

public class ThoriumBCLKeyChanger : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return false;
    }
    private ILHook ilHook;

    public override void Load()
    {
        if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            return;

        var bcSupportType = thorium.Code.GetType("ThoriumMod.ModSupport.ModSupportModules.BossChecklistSupport");
        if (bcSupportType == null)
            return;

        var logBossesMethod = bcSupportType.GetMethod("DoBossChecklistSupport_LogBosses",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (logBossesMethod == null)
            return;

        ilHook = new ILHook(logBossesMethod, IL_EditBossChecklistKeys);
    }

    public override void Unload()
    {
        ilHook?.Dispose();
        ilHook = null;
    }

    private void IL_EditBossChecklistKeys(ILContext il)
    {
        var c = new ILCursor(il);

        // Search for the string "ThePrimordials" as a marker.
        if (ModLoader.TryGetMod("ThoriumRework", out _))
        {
            while (c.TryGotoNext(MoveType.After, i => i.MatchLdstr("ThePrimordials")))
            {
                // The very next instruction should be ldc.r4 19.5 (float literal).
                if (c.Next != null && c.Next.MatchLdcR4(19.5f))
                {
                    c.Next.Operand = 21.5f; // Change the operand to 21.5f.
                }
            }
        }

        // Reset cursor to start for Star Scouter search
        c.Index = 0;

        // Update Star Scouter progression: 6.9 -> 6.91
        while (c.TryGotoNext(MoveType.After, i => i.MatchLdstr("StarScouter")))
        {
            if (c.Next != null && c.Next.MatchLdcR4(6.9f))
            {
                c.Next.Operand = 6.91f;
            }
        }
    }
}