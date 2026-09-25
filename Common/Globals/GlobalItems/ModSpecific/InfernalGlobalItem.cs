using CalamityMod;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.TileEntities;
using CalamityMod.Tiles.DraedonSummoner;
using InfernalEclipseAPI.Content.Items.Accessories;
using InfernalEclipseAPI.Content.Items.Armor.Vanity;
using InfernalEclipseAPI.Content.Items.Lore.InfernalEclipse;
using InfernalEclipseAPI.Content.Items.Lore.Thorium;
using InfernalEclipseAPI.Content.Items.Other;
using InfernalEclipseAPI.Content.Items.Placeables.MusicBoxes;
using InfernalEclipseAPI.Content.Items.Placeables.Paintings;
using InfernalEclipseAPI.Content.Items.Weapons.Catlight;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.Utils;
using InfernalEclipseAPI.Core.World;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee;
using InfernumMode.Content.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ModSpecific
{
    public class InfernalGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public bool hasEnchantmentShader;

        public override void SetDefaults(Item item)
        {
            if (item.type == ModContent.ItemType<TrustyOldRod>())
            {
                item.fishingPole = 0;
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (InfernalCrossmod.FargosMutant.Loaded)
            {
                if (item.type == InfernalCrossmod.FargosMutant.Mod.Find<ModItem>("SuspiciousSkull").Type && !NPC.downedBoss3)
                    return false;
            }

            if (item.type == ModContent.ItemType<Wayfinder>() && player.Calamity().ZoneAbyss && !DownedBossSystem.downedYharon)
                return false;

            if (item.type == ModContent.ItemType<TrustyOldRod>())
                return false;

            return base.CanUseItem(item, player);
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (ModLoader.TryGetMod("YouBoss", out Mod you))
            {
                if (you.TryFind("FirstFractal", out ModItem firstFractal))
                {
                    if (item.type == firstFractal.Type)
                    {
                        if (player.mount.Active)
                        {
                            player.mount.Dismount(player);
                        }
                        player.RemoveAllGrapplingHooks();
                    }
                }
            }

            return base.UseItem(item, player);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            bool isConsumed = base.ConsumeItem(item, player);

            if (isConsumed)
            {
                if (item.type == ModContent.ItemType<AuricQuantumCoolingCell>() || item.type == ModContent.ItemType<AdvancedDisplay>() || item.type == ModContent.ItemType<DecryptionComputer>() || item.type == ModContent.ItemType<VoltageRegulationSystem>() || item.type == ModContent.ItemType<LongRangedSensorArray>())
                {
                    Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
                    Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
                    float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

                    if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.HasTile && tile.TileType == ModContent.TileType<CodebreakerTile>())
                    {
                        TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(placeTileCoords.X, placeTileCoords.Y, CodebreakerTile.Width, CodebreakerTile.Height, CodebreakerTile.SheetSquare);
                        if (codebreakerTileEntity.ContainsAdvancedDisplay && codebreakerTileEntity.ContainsCoolingCell && codebreakerTileEntity.ContainsVoltageRegulationSystem && codebreakerTileEntity.ContainsDecryptionComputer && codebreakerTileEntity.ContainsSensorArray)
                        {
                            InfernalWorld.codebreakerCompleted = true;
                        }
                    }
                }
            }

            return isConsumed;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.type == ModContent.ItemType<DreadmineStaff>())
            {
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.owner == player.whoAmI && (projectile.type == ModContent.ProjectileType<DreadmineTurret>() || projectile.type == ModContent.ProjectileType<Dreadmine>()))
                    {
                        projectile.Kill();
                    }
                }
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<StarterBag>())
            {
                if (ModLoader.TryGetMod("CalamityAmmo", out Mod calAmmo))
                {
                    itemLoot.Remove(ItemDropRule.Common(calAmmo.Find<ModItem>("HardTackChest").Type));
                }

                if (ModLoader.TryGetMod("ThoriumMod", out var thoriumMod) && !ModLoader.TryGetMod("WHummusMultiModBalancing", out _))
                {
                    if (thoriumMod.TryFind("Tambourine", out ModItem tambourineItem))
                    {
                        itemLoot.Add(ItemDropRule.Common(tambourineItem.Type));
                    }

                    if (thoriumMod.TryFind("Pill", out ModItem pillsItem))
                    {
                        itemLoot.Add(ItemDropRule.Common(pillsItem.Type, 1, 200, 200));
                    }
                }

                if (ModLoader.HasMod("SOTS"))
                {
                    itemLoot.Add(ItemDropRule.Common(InfernalCrossmod.SOTS.Mod.Find<ModItem>("WorldgenScanner").Type));
                }

                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MenuMusicBox>()));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulDrivenHeadphonesEclipse>()));

                itemLoot.Add(ItemDropRule.ByCondition(new SoltanPlayerCondition(), ModContent.ItemType<LoreDylan>()));

                itemLoot.Add(ItemDropRule.ByCondition(new DylanPlayerCondition(), ModContent.ItemType<SoltanBullyingSlip>()));

                itemLoot.Add(ItemDropRule.ByCondition(new CheesePlayerCondition(), ModContent.ItemType<DeathWhistle>()));

                itemLoot.Add(ItemDropRule.ByCondition(new AkiraPlayerCondition(), ModContent.ItemType<PhantomMask>()));
                itemLoot.Add(ItemDropRule.ByCondition(new AkiraPlayerCondition(), ModContent.ItemType<PhantomSuitCoat>()));
                itemLoot.Add(ItemDropRule.ByCondition(new AkiraPlayerCondition(), ModContent.ItemType<PhantomSuitPants>()));

                itemLoot.Add(ItemDropRule.ByCondition(new CatPlayerCondition(), ModContent.ItemType<Catlight>()));

                itemLoot.Add(ItemDropRule.ByCondition(new DevListPlayerCondition(), ModContent.ItemType<InfernalTwilight>()));
                itemLoot.Add(ItemDropRule.ByCondition(new ChallengeListPlayerCondition(), ModContent.ItemType<InfernalArsenalPainting>()));
            }
        }

        public override void PostUpdate(Item item)
        {
            if (item.type == ItemID.GoldenKey && (!NPC.downedBoss3))
                item.TurnToAir();
        }

        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (item.type == ItemID.TinkerersWorkshop)
            {
                InfernalWorld.craftedWorkshop = true;
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (item.type == InfernalCrossmod.Thorium.Mod.Find<ModItem>("Mjolnir").Type)
                {
                    Player p = Main.LocalPlayer;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        int hammerLore = Item.NewItem(p.GetSource_Misc("IEoR_MjolnirCrafted"), (int)p.position.X, (int)p.position.Y, p.width, p.height, ModContent.ItemType<LoreMjolnir>());
                        NetMessage.SendData(MessageID.InstancedItem, Main.myPlayer, -1, null, hammerLore);
                    }
                }
            }

            base.OnCreated(item, context);
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.TinkerersWorkshop)
            {
                player.GetModPlayer<InfernalPlayer>().workshopHasBeenOwned = true;
                InfernalWorld.craftedWorkshop = true;
            }

            if (item.type == ModContent.ItemType<LoreAnniversaryOne>())
            {
                player.GetModPlayer<InfernalPlayer>().aniversaryYearOneLoreObtained = true;
            }

            return base.OnPickup(item, player);
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            if (InfernalCrossmod.Consolaria.Loaded)
            {
                if (item.type == InfernalCrossmod.Consolaria.Mod.Find<ModItem>("Wiesnbrau").Type)
                {
                    player.AddBuff(BuffID.PotionSickness, player.pStone ? 30 * 60 : 45 * 60);
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<TrustyOldRod>())
            {
                InfernalUtilities.AddDisabledItemTag(tooltips);
            }

            if (item.type == ModContent.ItemType<DreadmineStaff>())
            {
                InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Dreadmine"), Color.Lerp(Color.White, new Color(255, 80, 0), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)));
            }

            if (InfernalCrossmod.FargosMutant.Loaded)
            {
                if (item.type == InfernalCrossmod.FargosMutant.Mod.Find<ModItem>("SuspiciousSkull").Type && !NPC.downedBoss3)
                    InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.MutantSkeletron"), Color.Lerp(Color.White, new Color(255, 80, 0), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)));
            }

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                if (item.type == InfernalCrossmod.Consolaria.Mod.Find<ModItem>("Wiesnbrau").Type)
                {
                    InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.Wiesnbrau"), Color.Lerp(Color.White, new Color(255, 80, 0), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)));
                }
            }

            if (ModLoader.TryGetMod("CalamitySimpleWhipAddon", out Mod simpleWhipAddon) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                int findWhipItem(string name) => simpleWhipAddon.Find<ModItem>(name).Type;

                int[] bleachedAcessories =
                {
                    findWhipItem("BleachedNucleogenesis"),
                    findWhipItem("BleachedStatisCurse"),
                    findWhipItem("BleachedStarTaintedGenerator"),
                    findWhipItem("BleachedStarbusterCore"),
                    findWhipItem("BleachedNuclearFuelRod"),
                    findWhipItem("BleachedTheFirstShadowflame"),
                    findWhipItem("BleachedJellyChargedBattery"),
                    findWhipItem("BleachedVoltaicJelly"),

                    //findWhipItem("BuddyEmblem")
                };

                foreach (int bleachedItem in bleachedAcessories)
                {
                    if (item.type == bleachedItem)
                        InfernalUtilities.AddDisabledItemTag(tooltips);
                }
            }
        }

        #region Enchantment Shader
        private static Asset<Texture2D> glintTex;
        private static Effect glintFx;

        private static void EnsureAssetsLoaded()
        {
            if (glintTex == null || !glintTex.IsLoaded)
                glintTex = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Assets/Textures/Enchanted", AssetRequestMode.ImmediateLoad);

            if (glintFx == null)
                glintFx = ModContent.Request<Effect>("InfernalEclipseWeaponsDLC/Assets/Effects/Transform", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin,float scale)
        {
            if (!hasEnchantmentShader)
                return true;

            EnsureAssetsLoaded();

            glintFx.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            glintFx.CurrentTechnique.Passes["EnchantedPass"].Apply();
            Main.instance.GraphicsDevice.Textures[1] = glintTex.Value;

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, glintFx, Main.UIScaleMatrix);

            return true; // let vanilla draw the item with our active effect
        }

        public override void PostDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor,  Color itemColor, Vector2 origin, float scale)
        {
            if (!hasEnchantmentShader)
                return;

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch sb, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (!hasEnchantmentShader)
                return true;

            EnsureAssetsLoaded();

            glintFx.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            glintFx.CurrentTechnique.Passes["EnchantedPass"].Apply();
            Main.instance.GraphicsDevice.Textures[1] = glintTex.Value;

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState,  sb.GraphicsDevice.SamplerStates[0], Main.spriteBatch.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, glintFx, Main.GameViewMatrix.TransformationMatrix);

            return true; // let vanilla draw the world item with our active effect
        }

        public override void PostDrawInWorld(Item item, SpriteBatch sb, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (!hasEnchantmentShader)
                return;

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion
    }

    public class DevListPlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            foreach (Player player in Main.ActivePlayers)
            {
                foreach (string name in InfernalTwilight.devList)
                {
                    if (player.name.ToLower().Contains(name))
                        return true;
                }
                if (player.name.ToLower().Contains("nuggets") || player.name.ToLower().Contains("hummus"))
                    return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }

    public class ChallengeListPlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            foreach (Player player in Main.ActivePlayers)
            {
                foreach (string name in InfernalArsenalPainting.bossRushList)
                {
                    if (player.name.ToLower().Contains(name))
                        return true;
                }
                foreach (string name in InfernalArsenalPainting.lowPercentList)
                {
                    if (player.name.ToLower().Contains(name))
                        return true;
                }
                foreach (string name in InfernalArsenalPainting.notHitList)
                {
                    if (player.name.ToLower().Contains(name))
                        return true;
                }
                foreach (string name in InfernalArsenalPainting.whipsList)
                {
                    if (player.name.ToLower().Contains(name))
                        return true;
                }
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }

    public class SoltanPlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && (player.name == "Bloxxer" || player.name == "Dylan"))
                    return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }

    public class DylanPlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.name == "Dylan")
                    return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }


    public class CheesePlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.name == "lifenuggets")
                    return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }

    public class AkiraPlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.name == "Akira")
                    return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }

    public class CatPlayerCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Loop through all players in the world
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.name == "StarlightCat")
                    return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => "A certain person must be present...";
    }
}
