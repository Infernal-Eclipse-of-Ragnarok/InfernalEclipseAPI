using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Content.Items.Consumables;
using InfernalEclipseAPI.Content.Items.Materials;
using InfernalEclipseAPI.Content.Items.Placeables.MusicBoxes;
using InfernalEclipseAPI.Content.Items.Placeables.Paintings;
using InfernalEclipseAPI.Content.Items.SpawnItems;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Players.ThoriumPlayerOverrides.ThoriumMulticlassNerf;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.Utils;
using InfernalEclipseAPI.Core.World;
using InfernumMode.Content.BehaviorOverrides.BossAIs.ProfanedGuardians;
using InfernumMode.Core.GlobalInstances.Systems;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod.Items.Depths;

namespace InfernalEclipseAPI.Common.GlobalNPCs
{
    public class InfernalGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static int cnidrion = -1;

        public override void SetDefaults(NPC entity)
        {
            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (entity.buffImmune[BuffID.Confused] && entity.boss)
                {
                    entity.buffImmune[InfernalCrossmod.Thorium.Mod.Find<ModBuff>("Stunned").Type] = true;
                }
            }

            switch (entity.type)
            {
                case NPCID.DetonatingBubble:
                    if (WorldSaveSystem.InfernumModeEnabled && Main.masterMode && CalamityWorld.revenge && NPC.AnyNPCs(NPCID.DukeFishron))
                        entity.dontTakeDamage = false;
                    break;
                default:
                    break;
            }
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type == NPCID.Painter)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null || items[i].IsAir)
                    {
                        items[i] = new Item(ModContent.ItemType<InfernalArsenalPainting>());
                        break;
                    }
                }
            }

            if (npc.type == NPCID.PartyGirl && BirthdayParty.GenuineParty)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null || items[i].IsAir)
                    {
                        items[i] = new Item(ModContent.ItemType<BirthdayCake>());
                        break;
                    }
                }
            }

            if (npc.type == NPCID.Princess)
            {
                if (InfernalWorld.codebreakerCompleted)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null || items[i].IsAir)
                        {
                            items[i] = new Item(ModContent.ItemType<CodebreakerMusicBox>());
                            break;
                        }
                    }
                }

                if (DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs) 
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null || items[i].IsAir)
                        {
                            items[i] = new Item(ModContent.ItemType<InterludeFourMusicBox>());
                            break;
                        }
                    }
                }
            }

            if (npc.type == NPCID.GoblinTinkerer && InfernalConfig.Instance.BossKillCheckOnOres)
            {
                // Replace Tinkerer's Workshop by filtering the Entries list
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    if (item != null && !item.IsAir && item.type == ItemID.TinkerersWorkshop)
                    {
                        // Replace workshop with blueprint
                        items[i] = new Item(ModContent.ItemType<TinkerersRepairBlueprints>());
                    }
                }

                bool someoneHasOwnedWorkshop = false;

                static bool HasWorkshop(Item[] arr)
                {
                    if (arr is null) return false;
                    foreach (var it in arr)
                        if (!it.IsAir && it.type == ItemID.TinkerersWorkshop)
                            return true;
                    return false;
                }

                foreach (var player in Main.ActivePlayers)
                {
                    if (HasWorkshop(player.inventory) || HasWorkshop(player.armor) ||
                        HasWorkshop(player.bank?.item) || HasWorkshop(player.bank2?.item) ||
                        HasWorkshop(player.bank3?.item) || HasWorkshop(player.bank4?.item))
                    {
                        player.GetModPlayer<InfernalPlayer>().workshopHasBeenOwned = true;
                    }

                    if (player.GetModPlayer<InfernalPlayer>().workshopHasBeenOwned)
                    {
                        someoneHasOwnedWorkshop = true;
                        break;
                    }
                }

                if (someoneHasOwnedWorkshop || InfernalWorld.craftedWorkshop || Main.hardMode) // start selling it again after its been obtained at least once; and will always sell it again in hardmode
                {
                    // Find first empty slot
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null || items[i].IsAir)
                        {
                            items[i] = new Item(ItemID.TinkerersWorkshop);
                            break;
                        }
                    }
                }
            }

            if (npc.type == ModContent.NPCType<ShadySalesman>())
            {
                for (int i = 0; i < items.Length; i++)
                {
                    Item item = items[i];

                    if (item.type != ModContent.ItemType<TrustyOldRod>())
                        continue;

                    item.TurnToAir();
                }
            }

            if (ModLoader.TryGetMod("CalamityAmmo", out Mod calamityAmmo) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                int hydroArrow = calamityAmmo.Find<ModItem>("HydrothermicArrow").Type;
                int hydroBullet = calamityAmmo.Find<ModItem>("HydrothermicBullet").Type;
                int divineArrow = calamityAmmo.Find<ModItem>("DivineArrow").Type;
                int divineBullet = calamityAmmo.Find<ModItem>("DivineBullet").Type;
                int weakAstralBullet = calamityAmmo.Find<ModItem>("WeakAstralBullet").Type;
                int astralBullet = calamityAmmo.Find<ModItem>("AstralBullet").Type;
                int dazzlingAstralBullet = calamityAmmo.Find<ModItem>("DazzlingAstralBullet").Type;

                // Remove matching entries
                for (int i = 0; i < items.Length; i++)
                {
                    Item item = items[i];

                    if (item == null || item.IsAir)
                        continue;

                    if (item.type == hydroArrow || item.type == hydroBullet || item.type == divineArrow || item.type == divineBullet
                        || item.type == weakAstralBullet || item.type == astralBullet || item.type == dazzlingAstralBullet)
                        item.TurnToAir();
                }
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                if (npc.type == InfernalCrossmod.Thorium.Mod.Find<ModNPC>("ConfusedZombie").Type)
                {
                    /*
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null || items[i].IsAir)
                        {
                            if (InfernalCrossmod.SOTS.Loaded)
                            {
                                if (AncientPhylacteryRightClickBlocker.DownedExcavator)
                                {
                                    items[i] = new Item(InfernalCrossmod.SOTS.Mod.Find<ModItem>("SeismicStation").Type);
                                    break;
                                }
                            }
                        }
                    }
                    */

                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null || items[i].IsAir)
                        {
                            if (InfernalCrossmod.SOTS.Loaded)
                            {
                                if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                                {
                                    items[i] = new Item(InfernalCrossmod.SOTS.Mod.Find<ModItem>("FrostArtifact").Type);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.boss) 
            {
                if (InfernalWorld.RagnarokModeEnabled && !BossRushEvent.BossRushActive)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (!player.dead)
                        {
                            ClearRageAndAdrenaline(player);

                            if (InfernalCrossmod.Thorium.Loaded)
                            {
                                if (InfernalCrossmod.RagnarokMod.Loaded)
                                {
                                    RagnarokRiffSystemInteraction.ClearRiffs(player);
                                }

                                // clear locally
                                ThoriumHelpers.ClearAllEmpowerments(player);

                                // and tell others
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    ModPacket p = Mod.GetPacket();
                                    p.Write((byte)InfernalEclipseMessageType.ThoriumEmpowerment);
                                    p.Write((byte)ThoriumEmpowermentMsg.ClearEmpowerments);
                                    p.Write((byte)player.whoAmI);
                                    p.Send();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ClearRageAndAdrenaline()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead)
                {
                    ClearRageAndAdrenaline(player);
                }
            }
        }

        public static void ClearRageAndAdrenaline(Player player)
        {
            player.ClearBuff(ModContent.BuffType<RageMode>());
            player.ClearBuff(ModContent.BuffType<AdrenalineMode>());

            CalamityPlayer mp = player.Calamity();
            mp.rage = 0;
            mp.rageModeActive = false;
            mp.adrenaline = 0;
            mp.adrenalineModeActive = false;
        }


        public override bool PreAI(NPC npc)
        {
            if (!npc.active) return base.PreAI(npc);

            if (InfernalWorld.RagnarokModeEnabled)
            {
                if (npc.type == NPCID.Deerclops)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (!player.dead && !npc.WithinRange(player.Center, 1000f))
                            continue;

                        player.AddBuff(ModContent.BuffType<FreezingAura>(), 2);
                    }
                }

                if (npc.type == NPCID.Golem)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (player.dead || !npc.WithinRange(player.Center, 10000f))
                            continue;

                        player.AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
                        player.RemoveAllGrapplingHooks();
                    }
                }

                if (npc.type == ModContent.NPCType<HealerShieldCrystal>())
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (player.dead || !npc.WithinRange(player.Center, 10000f))
                            continue;

                        player.AddBuff(ModContent.BuffType<HormonalBlockade>(), 2);
                    }
                }
            }

            if (InfernalCrossmod.Thorium.Loaded)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.dead || !npc.WithinRange(player.Center, 10000f))
                        continue;

                    if (npc.ModNPC?.Mod.Name != "ThoriumMod" && npc.boss)
                    {
                        player.ClearBuff(InfernalCrossmod.Thorium.Mod.Find<ModBuff>("SpiritualistBuff").Type);
                    }
                }
            }

            return base.PreAI(npc);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.boss && npc.type != NPCID.TorchGod) //anything that is considered a boss will have a 1/100 chance to drop our dev painting directly
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<InfernalTwilight>(), ThankYouPainting.DropInt));
            }

            if (npc.type == ModContent.NPCType<SoulSlurper>() && InfernalConfig.Instance.BossKillCheckOnOres)
            {
                int slurperPole = ModContent.ItemType<SlurperPole>();

                npcLoot.RemoveWhere(rule => rule is CommonDrop cd && cd.itemId == slurperPole || rule is ItemDropWithConditionRule iwc && iwc.itemId == slurperPole);

                foreach (var rule in npcLoot.Get())
                    PruneFromChains(rule, slurperPole);

                npcLoot.Add(ItemDropRule.ByCondition(new EvilBossDownedCondition(), slurperPole, 30));
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (InfernalCrossmod.SOTS.Loaded)
            {
                if (player.ZoneHallow && Main.hardMode && !TwoMechsDowned())
                {
                    pool.Remove(InfernalCrossmod.SOTS.Mod.Find<ModNPC>("HallowTreasureSlime").Type);
                }
            }
        }

        public static bool TwoMechsDowned()
        {
            return (NPC.downedMechBoss1 && NPC.downedMechBoss2) || (NPC.downedMechBoss1 && NPC.downedMechBoss3) || (NPC.downedMechBoss2 && NPC.downedMechBoss3);
        }

        private static void PruneFromChains(IItemDropRule rule, int itemId)
        {
            if (rule.ChainedRules is null || rule.ChainedRules.Count == 0)
                return;

            rule.ChainedRules.RemoveAll(c => c.RuleToChain is CommonDrop cd && cd.itemId == itemId || c.RuleToChain is ItemDropWithConditionRule iwc && iwc.itemId == itemId);

            foreach (var chain in rule.ChainedRules.ToList())
                PruneFromChains(chain.RuleToChain, itemId);
        }

        internal sealed class EvilBossDownedCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss2;
            public bool CanShowItemDropInUI() => true;
            public string GetConditionDescription() => "Downed Evil Boss";
        }
    }

}
