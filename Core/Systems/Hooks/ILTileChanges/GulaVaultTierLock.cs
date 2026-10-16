using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Core.Systems.Hooks.ILTileChanges
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class GulaVaultTierLock : ModSystem
    {
        private static Hook containerTypeRightClickHook;
        private static int gulaVaultTileType = -1;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out Mod sots))
                return;

            // Get GulaVaultTile tile type
            gulaVaultTileType = sots.Find<ModTile>("GulaVaultTile").Type;

            // Get SOTS.Items.Furniture.ContainerType type + its RightClick method
            Type containerType = sots.Code.GetType("SOTS.Items.Furniture.ContainerType");
            if (containerType is null)
                return;

            MethodInfo rightClickMethod = containerType.GetMethod("RightClick", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (rightClickMethod is null)
                return;

            containerTypeRightClickHook = new Hook(rightClickMethod, ContainerType_RightClick_Detour);
        }

        public override void Unload()
        {
            containerTypeRightClickHook?.Dispose();
            containerTypeRightClickHook = null;
            gulaVaultTileType = -1;
        }

        // Signature must match: bool RightClick(ModTile self, int i, int j)
        private delegate bool RightClickOrigDelegate(ModTile self, int i, int j);

        private static bool ContainerType_RightClick_Detour(
            RightClickOrigDelegate orig,
            ModTile self,
            int i,
            int j)
        {
            try
            {
                if (gulaVaultTileType != -1)
                {
                    Tile tile = Framing.GetTileSafely(i, j);

                    if (tile.HasTile && tile.TileType == gulaVaultTileType && !NPC.downedBoss2 && InfernalConfig.Instance.SOTSBalanceChanges)
                    {
                        // Gula vault clicked before EoW/BoC: block opening.
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.WelcomeMessage.GulaVaultMessage"), 200, 160, 60);
                            SoundEngine.PlaySound(SoundID.Tink, new Vector2(i * 16, j * 16));
                        }

                        return true;
                    }
                }

                return orig(self, i, j);
            }
            catch
            {
                return orig(self, i, j);
            }
        }
    }
}
