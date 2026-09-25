using System.Collections.Generic;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseAPI.Content.Items.Lore.InfernalEclipse;
using InfernalEclipseAPI.Core.Players;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Security.Policy;
using Terraria;

namespace InfernalEclipseAPI.Content.Items.Accessories.RingofTix
{
    public class RingofTix : ModItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.accessory = true;
            Item.defense = 12;
            Item.lifeRegen = 1;
            Item.Calamity().devItem = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            InfernalPlayer infernalPlayer = player.GetModPlayer<InfernalPlayer>();

            modPlayer.harpyRing = true;
            player.moveSpeed += 0.1f;

            modPlayer.darkSunRing = true;

            player.arrowDamage += 0.05f;
            infernalPlayer.tixThumbRing = true;

            player.GetDamage(DamageClass.Generic).Flat += 4;
            player.GetArmorPenetration(DamageClass.Generic) += 5;
            player.GetDamage(DamageClass.Generic) += 0.06f;
            player.GetCritChance(DamageClass.Generic) += 4;

            player.lifeRegen += 3;
            player.luck += 0.25f;

            InfernalPlayer infernal = player.GetModPlayer<InfernalPlayer>();
            infernal.HarvestMoonBuff = true;

            if (ModLoader.TryGetMod("VervCaves", out Mod vervCaves))
            {
                vervCaves.Find<ModItem>("MushroomRing").UpdateAccessory(player, hideVisual);
            }

            //Challenger Ring handled in GlobalItem
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Always show the normal stats tooltip.
            tooltips.Add(new TooltipLine(Mod, "NormalTip",
                Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix.NormalTooltip")));

            // Show SOTS-specific usage text only if SOTS is enabled.
            if (ModLoader.HasMod("SOTS"))
                tooltips.Add(new TooltipLine(Mod, "CaveTip", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix.SOTSTooltip")));

            if (ModLoader.HasMod("VervCaves"))
                tooltips.Add(new TooltipLine(Mod, "SOTSTip", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix.VervCaveTooltip")));

            tooltips.Add(new(Mod, "Flavour", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix.FlavorText")));

            Color color = CalamityUtils.ColorSwap(Color.OrangeRed, Color.DarkRed, 2f);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Soltan"))}");
                line5.OverrideColor = color;
                tooltips.Add(line5);
            }
        }

        public override void AddRecipes()
        {
            Recipe tixRing = Recipe.Create(ModContent.ItemType<RingofTix>());
            if (ModLoader.TryGetMod("VervCaves", out Mod vervCaves))
            {
                tixRing.AddIngredient(vervCaves.Find<ModItem>("MushroomRing"));
            }
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                tixRing.AddIngredient(thorium.Find<ModItem>("ThumbRing"));
                tixRing.AddIngredient(thorium.Find<ModItem>("TheRing"));
            }
            if (ModLoader.TryGetMod("BlueMoon", out Mod moons))
                tixRing.AddIngredient(moons.Find<ModItem>("MoonsRing"));
            tixRing.AddIngredient<HarpyRing>();
            if (ModLoader.TryGetMod("SOTS", out Mod sots))
                tixRing.AddIngredient(sots.Find<ModItem>("ChallengerRing"));
            tixRing.AddIngredient<DarkSunRing>();
            tixRing.AddIngredient<ShadowspecBar>(5);
            tixRing.AddIngredient<LoreDylan>();
            tixRing.AddTile<DraedonsForge>();
            tixRing.Register();
        }
    }

    public class TixGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            InfernalPlayer mp = player.GetModPlayer<InfernalPlayer>();
            if (mp.HarvestMoonBuff) npc.value += 2f;
        }
    }
}
