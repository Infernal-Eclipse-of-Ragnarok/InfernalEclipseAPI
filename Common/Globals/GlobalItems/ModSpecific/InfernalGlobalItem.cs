using CalamityMod;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
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
            /*
            if (item.type == ModContent.ItemType<Moonshine>() && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                item.value = Item.buyPrice(0, 1, 0, 0);
            }

            if (item.type == ModContent.ItemType<GrapeBeer>() && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                item.value = Item.buyPrice(0, 0, 3, 0);
            }
            */

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
            }

            /*
            if (InfernalConfig.Instance.CalamityExpertAccessories) 
            {
                if (item.type == ModContent.ItemType<AquaticScourgeBag>())
                {
                    itemLoot.RemoveWhere(aqua => aqua is CommonDrop commonDrop1 && commonDrop1.itemId == ModContent.ItemType<AquaticEmblem>(), true);
                    itemLoot.RemoveWhere(spine => spine is CommonDrop commonDrop2 && commonDrop2.itemId == ModContent.ItemType<CorrosiveSpine>(), true);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<CorrosiveSpine>(), 1, 1, 1);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<AquaticEmblem>(), DropHelper.BagWeaponDropRateFraction, 1, 1);
                }
                if (item.type == ModContent.ItemType<CryogenBag>())
                {
                    itemLoot.RemoveWhere(flare => flare is CommonDrop commonDrop3 && commonDrop3.itemId == ModContent.ItemType<FrostFlare>(), true);
                    itemLoot.RemoveWhere(soul => soul is CommonDrop commonDrop4 && commonDrop4.itemId == ModContent.ItemType<SoulofCryogen>(), true);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<FrostFlare>(), 1, 1, 1);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<SoulofCryogen>(), DropHelper.BagWeaponDropRateFraction, 1, 1);
                }
                if (item.type == ModContent.ItemType<DesertScourgeBag>())
                {
                    itemLoot.RemoveWhere(cloak => cloak is CommonDrop commonDrop5 && commonDrop5.itemId == ModContent.ItemType<SandCloak>(), true);
                    itemLoot.RemoveWhere(crest => crest is CommonDrop commonDrop6 && commonDrop6.itemId == ModContent.ItemType<OceanCrest>(), true);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<SandCloak>(), 1, 1, 1);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<OceanCrest>(), DropHelper.BagWeaponDropRateFraction, 1, 1);
                }
                if (item.type == ModContent.ItemType<RavagerBag>())
                {
                    itemLoot.RemoveWhere(totem => totem is CommonDrop commonDrop7 && commonDrop7.itemId == ModContent.ItemType<FleshTotem>(), true);
                    itemLoot.RemoveWhere(core => core is CommonDrop commonDrop8 && commonDrop8.itemId == ModContent.ItemType<BloodflareCore>(), true);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<FleshTotem>(), 1, 1, 1);
                    ((ILoot)(object)itemLoot).Add(ModContent.ItemType<BloodflareCore>(), DropHelper.BagWeaponDropRateFraction, 1, 1);
                }
                if (item.type == ItemID.BossBagBetsy)
                {
                    itemLoot.RemoveWhere(wing => wing is CommonDrop commonDrop && commonDrop.itemId == ItemID.BetsyWings, true);
                    ((ILoot)(object)itemLoot).Add(3883, 1, 1, 1);
                }
                if (item.type == ModContent.ItemType<LeviathanBag>())
                {
                    itemLoot.RemoveWhere(Community => Community is CommonDrop commonDrop && commonDrop.itemId == ModContent.ItemType<TheCommunity>(), true);
                    ((ILoot)(object)itemLoot).DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<TheCommunity>(), 10, 1, 1, false);
                }
                if (item.type == ModContent.ItemType<CalamitasCloneBag>())
                {
                    itemLoot.RemoveWhere(regen => regen is CommonDrop commonDrop && commonDrop.itemId == ModContent.ItemType<Regenator>(), true);
                    ((ILoot)(object)itemLoot).DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<Regenator>(), 10, 1, 1, false);
                }
                if (item.type == ItemID.QueenBeeBossBag)
                {
                    itemLoot.RemoveWhere(Bee => Bee is CommonDrop commonDrop9 && commonDrop9.itemId == ModContent.ItemType<TheBee>(), true);
                    ((ILoot)(object)itemLoot).DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<TheBee>(), 10, 1, 1, false);
                }

                if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
                {
                    if (item.type == calHunt.Find<ModItem>("TreasureTrunk").Type)
                    {
                        itemLoot.RemoveWhere(Tend => Tend is CommonDrop commonDrop && commonDrop.itemId == calHunt.Find<ModItem>("TendrilCursorAttachment").Type, true);
                        ((ILoot)(object)itemLoot).DefineConditionalDropSet(DropHelper.RevAndMaster).Add(calHunt.Find<ModItem>("TendrilCursorAttachment").Type, 10, 1, 1, false);
                    }
                }

                if (InfernalCrossmod.Clamity.Loaded)
                {
                    if (item.type == InfernalCrossmod.Clamity.Mod.Find<ModItem>("PyrogenBag").Type)
                    {
                        itemLoot.RemoveWhere(flare => flare is CommonDrop commonDrop3 && commonDrop3.itemId == InfernalCrossmod.Clamity.Mod.Find<ModItem>("HellFlare").Type, true);
                        ((ILoot)(object)itemLoot).Add(InfernalCrossmod.Clamity.Mod.Find<ModItem>("HellFlare").Type);
                    }
                }
            }
            */
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
            /*
            if ((item.type == ModContent.ItemType<GrapeBeer>() || item.type == ModContent.ItemType<Moonshine>()) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                InfernalUtilities.ReplaceTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GrapeBeer.Orig"), InfernalCrossmod.SOTS.Loaded ? Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GrapeBeer.Nerf") + "\n" + Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GrapeBeer.SOTSAdditional") : Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.GrapeBeer.Nerf"));
                InfernalUtilities.AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.TwoAlchs"), Color.Lerp(Color.White, new Color(255, 80, 0), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)));
            }
            */

            if (item.type == ModContent.ItemType<TrustyOldRod>())
            {
                InfernalUtilities.AddDisabledItemTag(tooltips);
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
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                foreach (string name in InfernalTwilight.devList)
                {
                    if (player.active && player.name.ToLower().Contains(name))
                        return true;
                }
                if (player.active && (player.name.ToLower().Contains("nuggets") || player.name.ToLower().Contains("hummus")))
                    return true;
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
