using CalamityMod.Items.Materials;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Materials
{
    public class EssenceOfFlame : EssenceofEleum
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalCrossmod.Clamity.Loaded;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.9f * num, 0.4f * num, 0.5f * num);
        }
    }
}
