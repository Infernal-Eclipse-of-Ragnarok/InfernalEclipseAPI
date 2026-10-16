using InfernalEclipseAPI.Core.Systems.Compatibility;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Core.Players.ThoriumPlayerOverrides
{
    //Wardrobe Hummus
    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class TitanWingsSetFix : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !InfernalCrossmod.Hummus.Loaded;
        }

        public override void UpdateEquips()
        {
            Mod thorium = InfernalCrossmod.Thorium.Mod;
            Mod rework = InfernalCrossmod.ThoriumRework.Mod;

            // Thorium items
            int titanWings = thorium.Find<ModItem>("TitanWings")?.Type ?? -1;
            int titanBreastplate = thorium.Find<ModItem>("TitanBreastplate")?.Type ?? -1;
            int titanGreaves = thorium.Find<ModItem>("TitanGreaves")?.Type ?? -1;

            // ThoriumRework helmets
            int titanHat = rework.Find<ModItem>("TitanHat")?.Type ?? -1;
            int titanHood = rework.Find<ModItem>("TitanHood")?.Type ?? -1;
            int titanVisage = rework.Find<ModItem>("TitanVisage")?.Type ?? -1;
            int titanVisor = rework.Find<ModItem>("TitanVisor")?.Type ?? -1;

            // Equipment slots
            bool hasBreastplate = Player.armor[1].type == titanBreastplate;
            bool hasGreaves = Player.armor[2].type == titanGreaves;

            int helmet = Player.armor[0].type;
            bool hasValidHelmet =
                helmet == titanHat ||
                helmet == titanHood ||
                helmet == titanVisage ||
                helmet == titanVisor;

            bool hasWings = false;


            int start = 3;
            int end = Player.armor.Length - 10;

            for (int i = start; i < end; i++)
            {
                Item accessory = Player.armor[i];

                if (accessory.IsAir || accessory.type != titanWings)
                    continue;

                // Match any of the exhaustion-clearing accessory types
                if (accessory.type == titanWings)
                {
                    hasWings = true; 
                    break;
                }
                else
                {
                    hasWings = false; break;
                }
            }

            if (!hasWings)
            {
                foreach (ModAccessorySlot slot in ModContent.GetContent<ModAccessorySlot>())
                {
                    if (!slot.IsEnabled())
                        continue;

                    Item item = slot.FunctionalItem;

                    if (item == null || item.IsAir)
                        continue;

                    if (item.type == titanWings)
                    {
                        hasWings = true;
                        break;
                    }
                }
            }


            if (hasWings && hasBreastplate && hasGreaves && hasValidHelmet)
            {
                // +15% generic damage
                Player.GetDamage(DamageClass.Generic) += 0.15f;
                Player.GetThoriumPlayer().thoriumEndurance += 0.15f;
            }
        }
    }
}
