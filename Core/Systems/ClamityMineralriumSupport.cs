using Clamity;
using Clamity.Content.Biomes.FrozenHell.Items;

namespace InfernalEclipseAPI.Core.Systems
{
    [JITWhenModsEnabled("Clamity")]
    [ExtendsFromMod("Clamity")]
    public class ClamityMineralriumSupport : ModSystem 
    {
        public override void PostSetupContent()
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<FrozenHellstoneTile>(), 11350, 1.35, ClamitySystem.downedWallOfBronze);
            }
        }
    }
}
