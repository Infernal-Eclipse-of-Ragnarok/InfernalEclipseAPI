using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SOTS.Buffs;
using SOTS.Void;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Consumables
{
    public class AngryPudding : ModItem
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
            Item.width = 58 / 2;
            Item.height = 36 / 2;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 15;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = 1550000;

            Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            bool sotsCanUse = true;
            if (InfernalCrossmod.SOTS.Loaded)
                sotsCanUse = VoidFoodHelper.CanUse(player);

            return sotsCanUse && player.Calamity().rage != player.Calamity().rageMax && !player.HasBuff<RageMode>() && !player.HasBuff(BuffID.PotionSickness);
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 30 * 60 : 45 * 60);
            player.AddBuff(InfernalCrossmod.NoxusBoss.Loaded ? InfernalCrossmod.NoxusBoss.Mod.Find<ModBuff>("StarstrikinglySatiated").Type : BuffID.WellFed3, 36000);
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            ++Item.stack;
            Activate(player);

            if (player?.active == true)
                player.AddBuff(ModContent.BuffType<VoidSickness2>(), 600);
        }

        public void Activate(Player player)
        {
            OnActivation(player);
            --Item.stack;
        }

        public static int GetVoidAmt() => 100;

        public static int GetSatiateDuration() => 5;

        public static void OnActivation(Player player)
        {
            if (InfernalCrossmod.SOTS.Loaded)
            {
                VoidFoodHelper.RefillEffect(player, GetVoidAmt());
            }
            player.Calamity().rage = player.Calamity().rageMax;
            player.AddBuff(ModContent.BuffType<RageMode>(), 2);

            if (player.whoAmI == Main.myPlayer)
                SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/AbilitySounds/RageActivate"));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new(Mod, nameof(AngryPudding), Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.DynamicTooltip", InfernalCrossmod.SOTS.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.VoidTooltip") + "\n" : "", InfernalCrossmod.NoxusBoss.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.Stellar") : Language.GetTextValue("Mods.InfernalEclipseAPI.Items.AngryPudding.Major")));
            tooltips.Add(tooltipLine);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Pudding"))}\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Contributor")}");
                line5.OverrideColor = new Color(50, 205, 50);
                tooltips.Add(line5);
            }
            else
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Contributor"));
                line5.OverrideColor = new Color(50, 205, 50);
                tooltips.Add(line5);
            }
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

        public override void AddRecipes()
        {
            if (!ModLoader.HasMod("SOTS") || !ModLoader.HasMod("CalamityHunt"))
            {
                Recipe recipe = CreateRecipe(ModLoader.TryGetMod("CalamityHunt", out Mod calHunt) ? 1 : 5);

                recipe.AddIngredient(calHunt != null ? calHunt.Find<ModItem>("ChromaticMass").Type : ModContent.ItemType<ShadowspecBar>());
                recipe.AddTile(TileID.CookingPots);
                recipe.Register();
            }
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public static class VoidFoodHelper
    {
        public static bool CanUse(Player player)
        {
            if (player.HasBuff(ModContent.BuffType<VoidSickness2>()))
            {
                int buffIndex = player.FindBuffIndex(ModContent.BuffType<VoidSickness2>());

                if (buffIndex != -1 && player.buffTime[buffIndex] >= 10 * 60)
                {
                    if (player.GetModPlayer<InfernalPlayer>().voidSicknessTextCooldown <= 0)
                    {
                        CombatText.NewText(player.Hitbox, Color.Lerp(Color.Red, Color.Magenta, 0.5f), Language.GetTextValue("Mods.InfernalEclipseAPI.UI.NoVoidConsumable"), true);
                        player.GetModPlayer<InfernalPlayer>().voidSicknessTextCooldown = 60 * 5;
                    }
                    return false;
                }
            }

            return !VoidPlayer.ModPlayer(player).frozenVoid && !player.HasBuff(ModContent.BuffType<Satiated>());
        }

        public static void RefillEffect(Player player, int amt)
        {
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            voidPlayer.voidMeter += amt * voidPlayer.VoidFoodGainMultiplier;
            VoidPlayer.VoidEffect(player, (int)(amt * voidPlayer.VoidFoodGainMultiplier));
        }
    }
}
