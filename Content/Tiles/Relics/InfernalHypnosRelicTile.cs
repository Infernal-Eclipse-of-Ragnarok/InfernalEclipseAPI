using InfernalEclipseAPI.Content.Items.Placeables.Relics;
using InfernumMode.Content.Tiles.Relics;

namespace InfernalEclipseAPI.Content.Tiles.Relics
{
    public class InfernalHypnosRelicTile : BaseInfernumBossRelic
    {
        public override int DropItemID => ModContent.ItemType<InfernalHypnosRelic>();

        public override string RelicTextureName => "InfernalEclipseAPI/Content/Tiles/Relics/InfernalHypnosRelicTile";
    }
}
