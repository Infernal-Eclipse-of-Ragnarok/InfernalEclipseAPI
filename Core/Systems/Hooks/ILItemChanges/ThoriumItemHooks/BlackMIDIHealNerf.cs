using System.Reflection;
using MonoMod.RuntimeDetour;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseAPI.Core.Systems.Hooks.ILItemChanges.ThoriumItemHooks
{
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
}
