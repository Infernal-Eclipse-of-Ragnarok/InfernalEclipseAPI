using InfernumMode.Content.BehaviorOverrides.BossAIs.BrimstoneElemental;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class FastDeathray : SlimeDeathray
    {
        public override float MaxAcceleration => 0.033f;
        public override float ReversalAccel => 0.0005f;
        public override float RegularAccel => 0.00025f;
        public override string Texture => ModContent.GetInstance<BrimstoneDeathray>().Texture;
        public override float Lifetime => 800;
        public override float ProjectileGaps => 225;
        public override int ProjectileReleaseRate => 80;
    }
}
