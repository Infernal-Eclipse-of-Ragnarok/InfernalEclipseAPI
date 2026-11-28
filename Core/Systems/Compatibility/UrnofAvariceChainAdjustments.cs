using CalamityMod;
using CalamityMod.Items.Placeables.Ores;
using InfernalEclipseAPI.Content.Items.SpawnItems;
using System.Collections.Generic;
using System.Reflection;

namespace InfernalEclipseAPI.Core.Systems.Compatibility
{
    public class UrnofAvariceChainAdjustments : ModSystem
    {
        public override void PostSetupContent()
        {
            RecodeGreedChain();
        }

        private void RecodeGreedChain()
        {
            if (!InfernalCrossmod.Calamity.Mod.TryFind<ModProjectile>("GreedTransmutation", out var transmutation))
                return;

            FieldInfo field = transmutation.GetType().GetField("GreedChain", BindingFlags.Static | BindingFlags.NonPublic);

            if (field?.GetValue(null) is not Dictionary<int, (int Result, Func<bool> Availability)> greedChain)
            {
                Mod.Logger.Error("Failed to retrieve Calamity's GreedChain.");
                return;
            }

            //greedChain.Clear();

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                greedChain[thorium.Find<ModItem>("SmoothCoal").Type] = (thorium.Find<ModItem>("LifeQuartz").Type, () => true);
                greedChain[thorium.Find<ModItem>("LifeQuartz").Type] = (ItemID.LifeCrystal, () => true);
                greedChain[ItemID.GoldOre] = (thorium.Find<ModItem>("ThoriumOre").Type, () => true);
                greedChain[ItemID.PlatinumOre] = (thorium.Find<ModItem>("ThoriumOre").Type, () => true);
                greedChain[thorium.Find<ModItem>("ThoriumOre").Type] = (thorium.Find<ModItem>("Aquaite").Type, () => NPC.downedBoss2);
                greedChain[thorium.Find<ModItem>("Aquaite").Type] = (ModContent.ItemType<AerialiteOre>(), () => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator);
                greedChain[ItemID.CobaltOre] = (thorium.Find<ModItem>("LodeStoneChunk").Type, () => (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "FallenBeholder"));
                greedChain[thorium.Find<ModItem>("LodeStoneChunk").Type] = (ItemID.MythrilOre, () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? NPC.downedMechBossAny : Main.hardMode);
                greedChain[ItemID.PalladiumOre] = (thorium.Find<ModItem>("ValadiumChunk").Type, () => (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "FallenBeholder"));
                greedChain[thorium.Find<ModItem>("ValadiumChunk").Type] = (ItemID.OrichalcumOre, () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? NPC.downedMechBossAny : Main.hardMode);
                greedChain[ItemID.ChlorophyteOre] = (thorium.Find<ModItem>("IllumiteChunk").Type, () => NPC.downedPlantBoss);
                greedChain[thorium.Find<ModItem>("IllumiteChunk").Type] = (ModContent.ItemType<PerennialOre>(), () => NPC.downedPlantBoss); 
            }

            if (ModLoader.TryGetMod("SOTS", out Mod sots))
            {
                greedChain[sots.Find<ModItem>("VibrantOre").Type] = (sots.Find<ModItem>("FrigidIce").Type, () => true);
                greedChain[ModContent.ItemType<HallowedOre>()] = (ItemID.ChlorophyteOre, () => AncientPhylacteryRightClickBlocker.DownedPolaris);
                greedChain[ModContent.ItemType<PerennialOre>()] = (ModContent.ItemType<ScoriaOre>(), () => NPC.downedGolemBoss);
                greedChain[ModContent.ItemType<ScoriaOre>()] = (sots.Find<ModItem>("PhaseOre").Type, () => AncientPhylacteryRightClickBlocker.DownedLux);
                greedChain[sots.Find<ModItem>("PhaseOre").Type] = (ItemID.LunarOre, () => NPC.downedMoonlord);
            }

            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
            {
                greedChain[catalyst.Find<ModItem>("MetanovaOre").Type] = (ModContent.ItemType<UelibloomOre>(), () => DownedBossSystem.downedProvidence);
            }

            if (ModLoader.TryGetMod("Clamity", out Mod clamity))
            {
                greedChain[clamity.Find<ModItem>("FrozenHellstone").Type] = (ModContent.ItemType<AuricOre>(), () => DownedBossSystem.downedCalamitas || DownedBossSystem.downedExoMechs);
            }

            Mod.Logger.Info("[IEoR] Successfully recoded Calamity's GreedChain.");
        }
    }
}
