using InfernumMode.Content.BehaviorOverrides.BossAIs.SlimeGod;

namespace InfernalEclipseAPI.Content.DifficultyOverrides.Calamity.Infernum.SlimeGodOverrides
{
    public class SlowWeirdGel : WeirdGel
    {
        public override string Texture => ModContent.GetInstance<DeceleratingCrimulanGlob>().Texture;
        public override float speedCap => 13;
        public override int activationTime => 10000;
    }
}
