namespace InfernalEclipseAPI.Core.DamageClasses.LegendaryClass
{
    public class LegendarySummon : DamageClass
    {
        internal static LegendarySummon Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            return damageClass == Summon;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            return damageClass == Summon || damageClass == Generic ? StatInheritanceData.Full : new StatInheritanceData(0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return damageClass == Summon ? true : false;
        }
    }
}
