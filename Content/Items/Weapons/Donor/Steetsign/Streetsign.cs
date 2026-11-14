using System.Collections.Generic;
using System.Reflection;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.UI.CalamitasEnchants;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Items.Weapons.Donor.Steetsign
{
    public class Streetsign : ModItem
    {
        private static bool VanillaShoot;
        private static readonly MethodInfo MiItemCheckShoot = typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public override void SetDefaults()
        {
            Item.damage = 3000;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();

            Item.width = Item.height = 118;

            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 6f;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.UseSound = SoundID.DD2_MonkStaffSwing;

            Item.Calamity().donorItem = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);

            StreetsignPlayer mp = player.GetModPlayer<StreetsignPlayer>();
            if (mp.improbabilityCharge <= 100 && mp.improbabilityTime <= 0)
                mp.improbabilityCharge += 2;


            base.OnHitNPC(player, target, hit, damageDone);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            var state = player.GetModPlayer<BladeSwingState>();

            if (player.itemAnimation == player.itemAnimationMax)
            {
                state.swingDirection *= -1;
                state.pendingShoot = true;
            }

            state.useDirection = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
            player.direction = state.useDirection;

            state.useRotation = player.Center.AngleTo(Main.MouseWorld);

            float x = 1f - (float)player.itemAnimation / player.itemAnimationMax;
            float lerp = x < 0.5f ? 4f * x * x * x : 1f - (float)Math.Pow(-2f * x + 2f, 3) / 2f;

            float arcMult = 1f;
            if (player.itemAnimationMax > 20)
                arcMult += 0.4f * (player.itemAnimationMax - 20) / 30f;
            arcMult = MathHelper.Clamp(arcMult, 1f, 1.3f);

            float arcStart = -110f * arcMult;
            float arcEnd = 90f * arcMult;

            player.itemRotation =
                state.useRotation
                + MathHelper.ToRadians(state.useDirection == 1 ? 45 : 135)
                + MathHelper.ToRadians(
                    MathHelper.Lerp(arcStart, arcEnd, state.swingDirection == 1 ? lerp : 1f - lerp)
                    * state.useDirection);

            if (player.gravDir == -1f)
                player.itemRotation = -player.itemRotation;

            if (state.pendingShoot && player.itemAnimation == player.itemAnimationMax)
            {
                state.pendingShoot = false;
                VanillaShoot = true;
                MiItemCheckShoot?.Invoke(player, new object[] { player.whoAmI, Item, player.GetWeaponDamage(Item) });
                VanillaShoot = false;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,
                player.itemRotation + MathHelper.ToRadians(-135f * state.useDirection));
            player.itemLocation = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full,
                player.itemRotation + MathHelper.ToRadians(-135f * state.useDirection));

            base.UseStyle(player, heldItemFrame);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip", Language.GetTextValue("Mods.InfernalEclipseAPI.Items.Streetsign.DynamicTooltip", InfernalEclipseAPI.ItemAbility.TooltipHotkeyString())));

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dedicated.rose"))}");
                line5.OverrideColor = new(196, 35, 44);
                tooltips.Add(line5);
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<StreetsignPlayer>();

            StreetsignDrawHelper.DrawChargeBarInInventory(spriteBatch, position, frame, modPlayer.improbabilityCharge, scale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sign)
                .AddIngredient(InfernalCrossmod.Clamity.Loaded ? InfernalCrossmod.Clamity.Mod.Find<ModItem>("EndobsidianBar").Type : ModContent.ItemType<AuricBar>(), 5)
                .AddIngredient<AshesofAnnihilation>(3)
                .AddTile<SCalAltar>()
                .Register();
        }
    }

    public sealed class BladeSwingState : ModPlayer
    {
        public int swingDirection = 1;
        public int useDirection = 1;
        public float useRotation = 0f;
        public bool pendingShoot = false;

        public override void ResetEffects()
        {
        }
    }

    public class StreetsignPlayer : ModPlayer
    {
        public int improbabilityTime;
        public int improbabilityCharge;
        public bool fullyCharged;
        public bool improabilityActive;

        private const int SpamIntervalTicks = 4;
        private const int SpamCountPerBurst = 2;
        private const float ScreenMargin = 60f;

        private int spamTimer;
        private int spamIndex;

        private static readonly string[] SpamLines =
        {
            "REALITY COMPROMISED",
            "THE CLOWN HAS BEEN ENGAGED.",
            "VENGEANCE.",
            "SUFFER."
        };

        public override void PostUpdate()
        {
            if (fullyCharged)
            {
                Lighting.AddLight(Player.Center, 0.2f, 0.4f, 1.0f);
                if (Main.rand.NextBool(5))
                    Dust.NewDust(Player.Center, 10, 10, DustID.Clentaminator_Red, 0, 0, 150, Color.Red, 0.75f);
            }

            if (improbabilityCharge > 0 && Player.HeldItem.type != ModContent.ItemType<Streetsign>())
                improbabilityCharge--;

            if (improbabilityCharge > 100)
                improbabilityCharge = 100;

            if (improbabilityCharge == 100)
                fullyCharged = true;
            else
                fullyCharged = false;

            if (improbabilityTime > 0)
            {
                improbabilityTime--;

                if (!improabilityActive)
                    improabilityActive = true;
            }
            else
            {
                if (improabilityActive)
                    improabilityActive = false;
            }

            if (!improabilityActive || improbabilityTime <= 0)
            {
                spamTimer = 0;
                spamIndex = 0;
                return;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 v = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Dust d = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(24f, 24f),
                    DustID.Clentaminator_Red, v, 150, Color.Red, 1.1f);
                d.noGravity = true;
            }

            // Only spawn on the local client.
            if (Player.whoAmI != Main.myPlayer)
                return;

            spamTimer++;
            if (spamTimer < SpamIntervalTicks)
                return;

            spamTimer = 0;

            // Spam multiple lines per burst for density.
            for (int k = 0; k < SpamCountPerBurst; k++)
                SpawnCyclingCombatText();
        }

        private void SpawnCyclingCombatText()
        {
            // Cycle text
            string text = SpamLines[spamIndex];
            spamIndex++;
            if (spamIndex >= SpamLines.Length)
                spamIndex = 0;

            // Pick a random on-screen position (screen space -> world space)
            float x = Main.rand.NextFloat(ScreenMargin, Main.screenWidth - ScreenMargin);
            float y = Main.rand.NextFloat(ScreenMargin, Main.screenHeight - ScreenMargin);

            Vector2 worldPos = Main.screenPosition + new Vector2(x, y);

            // CombatText.NewText wants a world-space Rectangle.
            Rectangle r = new Rectangle((int)worldPos.X, (int)worldPos.Y, 2, 2);

            // Color + “dramatic” for punch
            Color c = new Color(196, 35, 44);

            // Optional: vary slightly for chaos
            if (Main.rand.NextBool(3))
                c = Color.Lerp(c, Color.DarkRed, 0.5f);

            CombatText.NewText(r, c, text, dramatic: true);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (InfernalEclipseAPI.ItemAbility.JustPressed && fullyCharged)
            {
                improbabilityCharge = 0;
                fullyCharged = false;
                SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/AbilitySounds/DemonshadeEnrage"));
                improbabilityTime = 60 * 60; // 1 minute
            }
        }

        public override float UseAnimationMultiplier(Item item)
        {
            if (improabilityActive)
                return 0.9f;
            else
                return base.UseTimeMultiplier(item);
        }

        public override float UseTimeMultiplier(Item item)
        {
            if (improabilityActive)
                return 0.9f;
            else
                return base.UseTimeMultiplier(item);
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if (!improabilityActive)
                return;

            crit += 10f; // +10% crit chance
        }

        public override void PostUpdateRunSpeeds()
        {
            if (!improabilityActive)
                return;

            // 5% movement speed increase
            Player.moveSpeed += 0.05f;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Player.HeldItem.type == ModContent.ItemType<Streetsign>())
                StreetsignCharge();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Player.HeldItem.type == ModContent.ItemType<Streetsign>())
                StreetsignCharge();
        }

        private void StreetsignCharge()
        {
            if (improbabilityTime > 0) return;

            if (improbabilityCharge <= 100)
                improbabilityCharge += 5;

            if (improbabilityCharge >= 100)
            {
                fullyCharged = true;
            }
        }
    }

    public class StreetsignChargeLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var modPlayer = player.GetModPlayer<StreetsignPlayer>();

            if (player.HeldItem.ModItem is not Streetsign) return;
            if (modPlayer.improbabilityCharge <= 0) return;

            Texture2D barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack", AssetRequestMode.ImmediateLoad).Value;
            Texture2D barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront", AssetRequestMode.ImmediateLoad).Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            float barScale = 0.95f;
            Vector2 drawPos = player.Top + Vector2.UnitY * -16f - Main.screenPosition;

            Rectangle frameCrop = new Rectangle(0, 0, (int)(modPlayer.improbabilityCharge / 100f * barFG.Width), barFG.Height);

            float t = (float)((Math.Sin(Main.GlobalTimeWrappedHourly * 2f) + 1f) / 2f);
            Color color = Color.Lerp(new Color(196, 35, 44), Color.DarkRed, t);

            Main.spriteBatch.Draw(barBG, drawPos, null, color, 0f, barOrigin, barScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, barOrigin, barScale, SpriteEffects.None, 0f);
        }
    }

    public class StreetsignGlitchPlayerLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
            => new AfterParent(PlayerDrawLayers.Torso);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player p = drawInfo.drawPlayer;
            if (!p.active)
                return;

            var mp = p.GetModPlayer<StreetsignPlayer>();
            if (!mp.improabilityActive || mp.improbabilityTime <= 0)
                return;

            float intensity = MathHelper.Clamp(mp.improbabilityTime / 3600f, 0f, 1f);

            Lighting.AddLight(p.Center, 0.9f * intensity, 0.1f * intensity, 0.1f * intensity);

            int ghostCount = 3;
            float jitter = 6f * intensity;
            Color ghostColor = new Color(196, 35, 44) * (0.35f * intensity);

            int baseCount = drawInfo.DrawDataCache.Count;

            // Identify held-item texture once. If no held item, treat as none.
            Texture2D heldTex = null;
            if (p.HeldItem != null && !p.HeldItem.IsAir)
                heldTex = TextureAssets.Item[p.HeldItem.type].Value;

            // Also exclude anything extremely close to the held item position to avoid catching arm/held-item fragments.
            Vector2 heldScreenPos = p.itemLocation - Main.screenPosition;
            float heldExcludeDistSq = 42f * 42f; // tune 32-56

            bool ShouldExcludeFromPlayerGhosts(in DrawData dd)
            {
                if (dd.texture == null)
                    return false;

                // Hard exclude: exact held item texture draw(s).
                if (heldTex != null && dd.texture == heldTex)
                    return true;

                // Soft exclude: anything right around itemLocation (prevents cloning arm/item bits together).
                if (Vector2.DistanceSquared(dd.position, heldScreenPos) <= heldExcludeDistSq)
                    return true;

                return false;
            }

            for (int g = 0; g < ghostCount; g++)
            {
                Vector2 offset = new Vector2(
                    Main.rand.NextFloat(-jitter, jitter),
                    Main.rand.NextFloat(-jitter, jitter));

                for (int i = 0; i < baseCount; i++)
                {
                    DrawData dd = drawInfo.DrawDataCache[i];

                    if (ShouldExcludeFromPlayerGhosts(dd))
                        continue;

                    dd.position += offset;
                    dd.color = ghostColor;
                    drawInfo.DrawDataCache.Add(dd);
                }
            }
        }
    }

    public class StreetsignGlitchHeldItemLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player p = drawInfo.drawPlayer;
            if (!p.active)
                return;

            var mp = p.GetModPlayer<StreetsignPlayer>();
            if (!mp.improabilityActive || mp.improbabilityTime <= 0)
                return;

            if (p.HeldItem?.ModItem is not Streetsign)
                return;

            Texture2D heldTex = TextureAssets.Item[p.HeldItem.type].Value;

            int baseCount = drawInfo.DrawDataCache.Count;

            List<int> indices = new();
            for (int i = 0; i < baseCount; i++)
            {
                if (drawInfo.DrawDataCache[i].texture == heldTex)
                    indices.Add(i);
            }

            if (indices.Count == 0)
                return;

            float intensity = MathHelper.Clamp(mp.improbabilityTime / 3600f, 0f, 1f);

            int copies = 4;
            float jitter = 6f * intensity;
            Color glowColor = new Color(196, 35, 44) * (0.40f * intensity);

            for (int c = 0; c < copies; c++)
            {
                Vector2 offset = new Vector2(
                    Main.rand.NextFloat(-jitter, jitter),
                    Main.rand.NextFloat(-jitter, jitter));

                foreach (int idx in indices)
                {
                    DrawData ghost = drawInfo.DrawDataCache[idx];
                    ghost.position += offset;
                    ghost.color = glowColor;
                    drawInfo.DrawDataCache.Add(ghost);
                }
            }
        }
    }

    public static class StreetsignDrawHelper
    {
        public static void DrawChargeBarInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, float charge, float scale)
        {
            if (charge <= 0f) return;

            Texture2D barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack", AssetRequestMode.ImmediateLoad).Value;
            Texture2D barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront", AssetRequestMode.ImmediateLoad).Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;

            // Position below the item slot
            float yOffset = 40f;
            Vector2 drawPos = position + Vector2.UnitY * scale * (frame.Height - yOffset);

            // Crop the foreground based on charge
            Rectangle frameCrop = new Rectangle(0, 0, (int)(charge / 100f * barFG.Width), barFG.Height);

            // Smooth lerp color
            float t = (float)((Math.Sin(Main.GlobalTimeWrappedHourly * 2f) + 1f) / 2f);
            Color color = Color.Lerp(new Color(196, 35, 44), Color.DarkRed, t);

            float barScale = 2.75f;

            // Draw background
            spriteBatch.Draw(barBG, drawPos, null, color, 0f, barOrigin, scale * barScale, SpriteEffects.None, 0f);
            // Draw foreground
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, barOrigin, scale * barScale, SpriteEffects.None, 0f);
        }
    }

    public class StrretSignOverlayColor : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            if (player is null || !player.active)
                return;

            var mp = player.GetModPlayer<StreetsignPlayer>();

            // FIX: must be NOT active -> return
            if (!mp.improabilityActive || mp.improbabilityTime <= 0)
                return;

            // 3600 ticks = 60s. Fade out over the duration.
            float intensity = MathHelper.Clamp(mp.improbabilityTime / 3600f, 0f, 1f);

            Color overlayColor = new Color(196, 35, 44) * (0.35f * intensity);
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // IMPORTANT: PostDrawInterface is already inside a Begin/End. Just draw.
            spriteBatch.Draw(pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), overlayColor);
        }
    }
}
