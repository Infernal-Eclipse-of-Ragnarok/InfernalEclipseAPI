using System.Collections.Generic;
using System.Reflection;
using SOTS.Items.Chaos;
using SOTS.Items.Earth;
using SOTS.Items.Permafrost;
using SOTS;
using CalamityMod;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Tiles.Ores;
using static SOTS.Items.Furniture.Functional.MineralariumTE;
using MonoMod.RuntimeDetour;
using SOTS.Items.Furniture.Functional;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Systems.Hooks.ILTileChanges
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class SOTSMineralariumHooks : ModSystem
    {
        private static Hook killTileHook;
        public override void OnModLoad()
        {
            ReplaceOreSpawnConditions();

            // ---------- hook MineralariumTile.KillTile to stop ore breaking ----------
            MethodInfo killTileMI = typeof(MineralariumTile).GetMethod("KillTile", BindingFlags.Public | BindingFlags.Instance);

            if (killTileMI != null)
            {
                killTileHook = new Hook(
                    killTileMI,
                    typeof(SOTSMineralariumHooks).GetMethod(
                        nameof(MineralariumKillTile_Hook),
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
                );
            }
        }

        public override void OnModUnload()
        {
            killTileHook?.Dispose();

            killTileHook = null;
        }

        public override void PostSetupContent()
        {
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<SeaPrism>(), 1500, 0.4, DownedBossSystem.downedDesertScourge);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<AerialiteOre>(), 2700, 0.5, DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<InfernalSuevite>(), 3050, 0.65, NPC.downedMechBossAny);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<CryonicOre>(), 3950, 0.7, DownedBossSystem.downedCryogen && (NPC.downedMechBoss1 && NPC.downedMechBoss2 || NPC.downedMechBoss1 && NPC.downedMechBoss3 || NPC.downedMechBoss2 && NPC.downedMechBoss3));
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<HallowedOre>(), 2100, 1.25, NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<PerennialOre>(), 4100, 1.25, NPC.downedPlantBoss);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<ScoriaOre>(), 4300, 1.25, NPC.downedGolemBoss);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<AstralOre>(), 10000, 1.25, DownedBossSystem.downedAstrumDeus);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<ExodiumOre>(), 11100, 1.25, NPC.downedMoonlord);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<UelibloomOre>(), 11250, 1.3, DownedBossSystem.downedProvidence);
            InfernalCrossmod.SOTS.Mod.Call("AddMineralariumOre", ModContent.TileType<AuricOre>(), 11300, 1.35, DownedBossSystem.downedYharon);
        }

        private delegate void Orig_MineralariumKillTile(MineralariumTile self, int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem);

        private static void MineralariumKillTile_Hook(Orig_MineralariumKillTile orig, MineralariumTile self, int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;
        }

        private static void ReplaceOreSpawnConditions()
        {
            Type oreType = typeof(OreType);

            FieldInfo field = oreType.GetField("OreSpawnConditions", LumUtils.UniversalBindingFlags);

            if (field?.GetValue(null) is not Dictionary<int, OreType.SpawnCondition> dict)
                return;

            dict.Clear();

            dict[TileID.Demonite] = () => NPC.downedBoss1;
            dict[TileID.Crimtane] = () => NPC.downedBoss1;
            dict[ModContent.TileType<VibrantOreTile>()] = () => NPC.downedBoss1;

            dict[TileID.Obsidian] = () => NPC.downedBoss2;
            dict[TileID.Meteorite] = () => NPC.downedBoss2;
            dict[ModContent.TileType<FrigidIceTileSafe>()] = () => NPC.downedBoss2;

            dict[TileID.Hellstone] = () => NPC.downedBoss3 || SOTSWorld.downedAdvisor;

            dict[TileID.Cobalt] = () => Main.hardMode;
            dict[TileID.Palladium] = () => Main.hardMode;

            dict[TileID.Mythril] = () =>
                CalamityServerConfig.Instance.EarlyHardmodeProgressionRework
                    ? NPC.downedMechBossAny
                    : Main.hardMode;

            dict[TileID.Orichalcum] = () =>
                CalamityServerConfig.Instance.EarlyHardmodeProgressionRework
                    ? NPC.downedMechBossAny
                    : Main.hardMode;

            dict[TileID.Adamantite] = () =>
                CalamityServerConfig.Instance.EarlyHardmodeProgressionRework
                    ? (NPC.downedMechBoss1 && NPC.downedMechBoss2) ||
                      (NPC.downedMechBoss1 && NPC.downedMechBoss3) ||
                      (NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    : NPC.downedMechBossAny;

            dict[TileID.Titanium] = () =>
                CalamityServerConfig.Instance.EarlyHardmodeProgressionRework
                    ? (NPC.downedMechBoss1 && NPC.downedMechBoss2) ||
                      (NPC.downedMechBoss1 && NPC.downedMechBoss3) ||
                      (NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    : NPC.downedMechBossAny;

            dict[TileID.Chlorophyte] = () =>
                NPC.downedMechBoss1 &&
                NPC.downedMechBoss2 &&
                NPC.downedMechBoss3;

            dict[ModContent.TileType<PhaseOreTile>()] = () => SOTSWorld.downedLux;

            dict[TileID.LunarOre] = () => NPC.downedMoonlord;
        }
    }
}
