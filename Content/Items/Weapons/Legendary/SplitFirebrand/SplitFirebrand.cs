using Microsoft.Xna.Framework;
using System.Collections.Generic;
using InfernumMode.Content.Rarities.InfernumRarities;
using CalamityMod.Items.Materials;
using Terraria.Localization;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using CalamityMod;
using Microsoft.Xna.Framework.Input;
using Terraria.DataStructures;
using InfernumMode;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ThoriumMod.Empowerments;
using CalamityMod.Cooldowns;
using Terraria.Audio;
using static InfernalEclipseWeaponsDLC.Content.Items.Weapons.Other.AbsoluteTVRemote;
using InfernalEclipseAPI.Core.Configs;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.SplitFirebrand
{
    public class SplitFirebrand : ModItem
    {
        public class FireWaveCooldown : CooldownHandler
        {
            public static new string ID => "FireWaveCooldown";
            public override bool ShouldDisplay => true;
            public override bool SavedWithPlayer => true;
            public override bool PersistsThroughDeath => true;
            public override LocalizedText DisplayName => Language.GetText("Mods.InfernalEclipseAPI.Cooldowns.FireWave");
            public override string Texture => GetTexture();
            public override Color OutlineColor => Color.DarkRed;
            public override Color CooldownStartColor => Color.Red;
            public override Color CooldownEndColor => Color.Maroon;

            public override void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
            {
                Texture2D value = ModContent.Request<Texture2D>(Texture).Value;
                Texture2D value2 = ModContent.Request<Texture2D>(OutlineTexture).Value;
                Texture2D value3 = ModContent.Request<Texture2D>(ChargeBarTexture).Value;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
                ApplyBarShaders(opacity);
                spriteBatch.Draw(value3, position, null, Color.White * opacity, 0f, value3.Size() * 0.5f, scale, SpriteEffects.None, 0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                spriteBatch.Draw(value2, position, null, OutlineColor * opacity, 0f, value2.Size() * 0.5f, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(value, position, null, Color.White * opacity, 0f, value.Size() * 0.5f, scale / 2, SpriteEffects.None, 0f);
            }

            public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
            {
                Texture2D value = ModContent.Request<Texture2D>(Texture).Value;
                Texture2D value2 = ModContent.Request<Texture2D>(OutlineTexture).Value;
                Texture2D value3 = ModContent.Request<Texture2D>(OverlayTexture).Value;
                Color outlineColor = OutlineColor;
                spriteBatch.Draw(value2, position, null, outlineColor * opacity, 0f, value2.Size() * 0.5f, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(value, position, null, Color.White * opacity, 0f, value.Size() * 0.5f, scale / 2, SpriteEffects.None, 0f);
                int num = (int)Math.Ceiling((float)value3.Height * (1f - instance.Completion));
                spriteBatch.Draw(sourceRectangle: new Rectangle(0, num, value3.Width, value3.Height - num), texture: value3, position: position + Vector2.UnitY * num * scale, color: outlineColor * opacity * 0.9f, rotation: 0f, origin: value.Size() * 0.25f, scale: scale, effects: SpriteEffects.None, layerDepth: 0f);
            }

            private string GetTexture()
            {
                if (NPC.downedMoonlord)
                {
                    return "InfernalEclipseAPI/Content/Items/Weapons/Legendary/SplitFirebrand/SplitFirebrandCrescendo";
                }
                else
                {
                    return "InfernalEclipseAPI/Content/Items/Weapons/Legendary/SplitFirebrand/SplitFirebrand";
                }
            }
        }

        static Asset<Texture2D> inventoryTexture;
        private bool fireWave = false;

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = LegendarySummonMeleeSpeed.Instance;
            Item.width = 66;
            Item.height = 66;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<InfernumProfanedRarity>();
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SplitFirebrandProjectile>();
            Item.shootSpeed = 14f;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.Infernum_Tooltips().DeveloperItem = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (NPC.downedMoonlord)
                damage += 3.75f;
            else if (NPC.downedGolemBoss)
                damage += 2.75f;
            else if (NPC.downedPlantBoss)
                damage += 2.75f;
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                damage += 2.6f;
            else if (Main.hardMode)
                damage += 1.45f;
            else if (NPC.downedBoss3)
                damage += 0.25f;
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            if (NPC.downedMoonlord)
                knockback += 3;
            else if (NPC.downedGolemBoss)
                knockback += 2;
            else if (NPC.downedPlantBoss)
                knockback += 1.5f;
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                knockback += 1;
            else if (Main.hardMode)
                knockback += 0.5f;
            else if (NPC.downedBoss3)
                knockback += 0.25f;
        }
        public override bool MeleePrefix() => true;


        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 || NPC.downedPlantBoss || NPC.downedGolemBoss || NPC.downedMoonlord)
            {
                // Prevent triggering during normal item usage
                if (player.itemAnimation > 0)
                    return;

                if (InfernalEclipseAPI.ItemAbility.JustPressed && !player.Calamity().cooldowns.ContainsKey(FireWaveCooldown.ID))
                {
                    player.AddCooldown(FireWaveCooldown.ID, 3600);
                    SoundEngine.PlaySound(SoundID.Item88);

                    Item.useStyle = ItemUseStyleID.Shoot;
                    Item.UseSound = SoundID.Item116;

                    fireWave = true;
                    player.controlUseItem = true;
                    player.ItemCheck();
                }
            }
        }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer = player.GetModPlayer<SplitFirebrandPlayer>();

            if (player.itemAnimation < player.itemAnimationMax - 1)
                return false;

            float ai2 = NPC.downedMoonlord ? 1f : 0f;

            if (fireWave)
            {
                Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    ModContent.ProjectileType<SplitFirebrandFlailProjectile>(),
                    (int)(damage * 1.5f),
                    knockback,
                    player.whoAmI,
                    0f,
                    0f,
                    ai2
                    );

                for (int i = 0; i < 12; i++)
                {
                    Vector2 flameVelocity =
                        velocity.RotatedByRandom(MathHelper.ToRadians(15f))
                        * Main.rand.NextFloat(0.8f, 1.2f);

                    int proj = Projectile.NewProjectile(
                        source,
                        position,
                        flameVelocity,
                        ProjectileID.Flames,
                        damage,
                        0f,
                        player.whoAmI
                    );

                    Main.projectile[proj].DamageType = Item.DamageType;
                }

                fireWave = false;

                return false;
            }
            else if (modPlayer.comboCounter != 2)
            {
                Projectile.NewProjectile(
                source,
                position,
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI,
                0f,
                0f,
                ai2
                );
            }
            else
            {
                Projectile.NewProjectile(
                source,
                position,
                velocity,
                ModContent.ProjectileType<SplitFirebrandFlailProjectile>(),
                (int)(damage * 1.5f),
                knockback,
                player.whoAmI,
                0f,
                0f,
                ai2
                );
            }

            modPlayer.comboCounter++;
            if (modPlayer.comboCounter > 2)
                modPlayer.comboCounter = 0;

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = player.GetModPlayer<SplitFirebrandPlayer>();

            if (modPlayer.comboCounter != 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item152;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.UseSound = SoundID.Item116;
            }

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HellstoneBar, 15);
            recipe.AddIngredient(ModContent.ItemType<AncientBoneDust>(), 3);
            recipe.AddIngredient(ItemID.Leather, 3);
            recipe.AddIngredient(ItemID.RedString);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Microsoft.Xna.Framework.Color lerpedColor = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.White, new Microsoft.Xna.Framework.Color(30, 144, 255), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5));

            TooltipLine line = new(Mod, "TagDebuff", GetTagDebuffTooltip());
            line.OverrideColor = Microsoft.Xna.Framework.Color.White;
            tooltips.Add(line);

            TooltipLine line4 = new(Mod, "Lore", GetLoreTooltip());
            tooltips.Add(line4);

            if (!NPC.downedMoonlord)
            {
                TooltipLine line3 = new(Mod, "Progression2", Language.GetTextValue("Mods.InfernalEclipseAPI.LegendaryTooltip.Base"));
                line3.OverrideColor = lerpedColor;
                tooltips.Add(line3);
            }

            TooltipLine line2 = new(Mod, "Progression", GetProgressionTooltip());
            line2.OverrideColor = lerpedColor;
            tooltips.Add(line2);

            Microsoft.Xna.Framework.Color color = CalamityUtils.ColorSwap(Microsoft.Xna.Framework.Color.OrangeRed, Microsoft.Xna.Framework.Color.DarkRed, 2f);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.Soltan"))}");
                line5.OverrideColor = color;
                tooltips.Add(line5);
            }

            string hotkey = InfernalEclipseAPI.ItemAbility.TooltipHotkeyString();

            foreach (TooltipLine tooltip in tooltips)
            {
                if (line.Text.Contains("{0}"))
                    line.Text = line.Text.Replace("{0}", hotkey);
            }
        }

        private string GetProgressionTooltip()
        {
            if (NPC.downedMoonlord)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.Full");
            if (NPC.downedGolemBoss)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.MoonLord");
            if (NPC.downedPlantBoss)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.Golem");
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.Plantera");
            if (Main.hardMode)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.Mechs");
            if (NPC.downedBoss3)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.WoF");
            return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Progression.Skeletron");
        }

        private string GetTagDebuffTooltip()
        {
            if (NPC.downedMoonlord)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.Full");
            if (NPC.downedGolemBoss)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.MoonLord");
            if (NPC.downedPlantBoss)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.Golem");
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.Plantera");
            if (Main.hardMode)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.Mechs");
            if (NPC.downedBoss3)
                return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.WoF");
            return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.TagDebuff.Skeletron");
        }

        private string GetLoreTooltip()
        {
            if (NPC.downedMoonlord)
                return "";
            if (NPC.downedGolemBoss)
                return "";
            if (NPC.downedPlantBoss)
                return "";
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                return "";
            if (Main.hardMode)
                return "";
            if (NPC.downedBoss3)
                return "";
            return Language.GetTextValue("Mods.InfernalEclipseAPI.Items.SplitFirebrand.Lore");
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Microsoft.Xna.Framework.Rectangle frame, Microsoft.Xna.Framework.Color drawColor, Microsoft.Xna.Framework.Color itemColor, Vector2 origin, float scale)
        {
            DrawTexture(spriteBatch, position, drawColor, scale);
            return false;
        }

        public override bool PreDrawInWorld( SpriteBatch spriteBatch, Microsoft.Xna.Framework.Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawTexture(
                spriteBatch,
                Item.Center - Main.screenPosition,
                lightColor,
                scale);

            return false;
        }

        private void DrawTexture(SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
        {
            Texture2D tex =
                NPC.downedMoonlord
                ? ModContent.Request<Texture2D>(
                    "InfernalEclipseAPI/Content/Items/Weapons/Legendary/SplitFirebrand/SplitFirebrandCrescendo"
                  ).Value
                : ModContent.Request<Texture2D>(
                    "InfernalEclipseAPI/Content/Items/Weapons/Legendary/SplitFirebrand/SplitFirebrand"
                  ).Value;

            Vector2 origin = tex.Size() * 0.5f;

            spriteBatch.Draw(
                tex,
                position,
                null,
                color,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public class SplitFirebrandPlayer : ModPlayer
    {
        public int comboCounter;

        public override void ResetEffects()
        {
            
        }
    }
}
