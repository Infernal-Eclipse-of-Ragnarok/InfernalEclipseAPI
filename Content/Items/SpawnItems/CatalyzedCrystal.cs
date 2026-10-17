using CalamityMod.Events;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using SOTS.Items.Celestial;
using SOTS.NPCs.Boss;
using Terraria.Audio;

namespace InfernalEclipseAPI.Content.Items.SpawnItems
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class CatalyzedCrystal : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("SecretsoftheSouls");
        }
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 13;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.rare = ItemRarityID.Purple;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 9999;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player) => player.ZoneUnderworldHeight && (ModLoader.HasMod("Fargowiltas") || !NPC.AnyNPCs(ModContent.NPCType<SubspaceSerpentHead>())) && !BossRushEvent.BossRushActive;

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SubspaceSerpentHead>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<SubspaceSerpentHead>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CatalystBomb>())
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
