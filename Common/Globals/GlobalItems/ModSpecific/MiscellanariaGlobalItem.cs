using InfernalEclipseAPI.Core.Systems;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific
{
    public class MiscellanariaFirstFractal : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return InfernalCrossmod.YouBoss.Loaded && ModLoader.HasMod("Miscellanaria");
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.FirstFractal;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[ItemID.FirstFractal] = true;
            ItemID.Sets.ShimmerTransformToItem[ItemID.Zenith] = -1;
            ItemID.Sets.ShimmerTransformToItem[ItemID.FirstFractal] = -1;
        }
    }

    public class MiscellanariaCultistBossBag : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModLoader.HasMod("ThoriumMod") && ModLoader.HasMod("Miscellanaria");
    //    public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CultistBossBag;
        public override void PostSetupContent()
        {
            ItemID.Sets.Deprecated[ItemID.CultistBossBag] = true;
        }
    }
}
