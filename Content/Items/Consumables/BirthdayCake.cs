using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria.Localization;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Content.Items.Consumables
{
    public class BirthdayCake : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            ItemID.Sets.FoodParticleColors[Item.type] = new Color[]
            {
                new(180, 112, 82),
                new(205, 133, 81),
                new(255, 139, 190),
                new(255, 224, 96)
            };

            //ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38 / 2;
            Item.height = 42 / 2;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 15;
            Item.maxStack = 1;
            Item.consumable = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;

            Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.Calamity().adrenaline != player.Calamity().adrenalineMax && !player.HasBuff<AdrenalineMode>() && !player.HasBuff(BuffID.PotionSickness) && !CalamityUtils.AnyBossNPCS();
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 30 * 60 : 45 * 60);
            player.AddBuff(InfernalCrossmod.NoxusBoss.Loaded ? InfernalCrossmod.NoxusBoss.Mod.Find<ModBuff>("StarstrikinglySatiated").Type : BuffID.WellFed3, 36000);
            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            ++Item.stack;
            Activate(player);
        }

        public void Activate(Player player)
        {
            OnActivation(player);
            --Item.stack;
        }

        public static void OnActivation(Player player)
        {
            player.Calamity().adrenaline = player.Calamity().adrenalineMax;
            player.AddBuff(ModContent.BuffType<AdrenalineMode>(), player.Calamity().AdrenalineDuration);

            if (player.whoAmI == Main.myPlayer)
                SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/AbilitySounds/AdrenalineActivate"));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new(Mod, nameof(BirthdayCake), Language.GetTextValue("Mods.InfernalEclipseAPI.Items.BirthdayCake.DynamicTooltip", InfernalCrossmod.NoxusBoss.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.Stellar") : Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.Major")));
            tooltips.Add(tooltipLine);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 0.5f;
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float offset = 10f;

            if (player.direction == -1)
                offset = -offset;

            player.itemLocation.X += offset; player.itemLocation.X += 10f;

            base.UseStyle(player, heldItemFrame);
        }
    }
}