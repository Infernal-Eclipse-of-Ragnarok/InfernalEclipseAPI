using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.Items;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.Items.SummonItems;
using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Helpers;
using SOTS.Items;
using SOTS.Items.Fragments;
using SOTS.NPCs.Boss.Lux;
using SOTS.NPCs.Constructs;
using SOTS.Projectiles;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.SpawnItems
{
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    public class ChaosLure : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("SecretsoftheSouls");
        }
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6, false));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<TrufflePlatter>()];
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(ticksperframe: 4, frameCount: 10));
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player) => player.ZoneHallow && player.ZoneOverworldHeight && Main.hardMode && (ModLoader.HasMod("Fargowiltas") || (!NPC.AnyNPCs(ModContent.NPCType<ChaosConstruct>()) && !NPC.AnyNPCs(ModContent.NPCType<ChaosSpirit>()) && !NPC.AnyNPCs(ModContent.NPCType<Lux>()))) && !BossRushEvent.BossRushActive;

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center + new Vector2(0.0f, -32f), new Vector2(0.0f, -4f), ModContent.ProjectileType<ConstructFinder>(), 0, 0.0f, Main.myPlayer, 0.0f, 0.0f, 0.0f);

                SOTSUtils.PlaySound(SoundID.Item122, player.position.X, player.position.Y, 0.8f, 0.1f);
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ChaosConstruct>());
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<ChaosConstruct>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ElectromagneticLure>()
                .AddIngredient<FragmentOfChaos>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
