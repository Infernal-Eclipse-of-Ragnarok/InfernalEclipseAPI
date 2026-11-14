using System.Reflection;
using InfernumMode.Content.BehaviorOverrides.BossAIs.SupremeCalamitas;
using MonoMod.RuntimeDetour;
using Terraria.Audio;
using InfernumMode.Assets.Sounds;
using MonoMod.Cil;

namespace InfernalEclipseAPI.Core.Systems.MultiplayerFixes.SupCal
{
    public class BrimstoneFlameOrbFixes : ModSystem
    {
        private ILHook hook;

        public override void Load()
        {
            MethodInfo method = typeof(BrimstoneFlameOrb).GetMethod("AI", BindingFlags.Instance | BindingFlags.Public);
            hook = new ILHook(method, InjectClientLogic);
        }

        public override void Unload()
        {
            hook?.Dispose();
            hook = null;
        }

        private void InjectClientLogic(ILContext il)
        {
            var c = new ILCursor(il);

            // Load "this" for the instance method
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            c.EmitDelegate<Action<BrimstoneFlameOrb>>(self =>
            {
                if (Main.netMode == NetmodeID.MultiplayerClient && self.Projectile.ai[0] == 125f)
                {
                    SoundEngine.PlaySound(InfernumSoundRegistry.WyrmChargeSound, self.Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item163, self.Projectile.Center);
                    self.Projectile.ai[0]++;
                }
            });
        }
    }
}
