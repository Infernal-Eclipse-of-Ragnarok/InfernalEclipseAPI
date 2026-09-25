using CalamityHunt.Common.DropRules;
using CalamityHunt;
using System.Linq;
using CalamityHunt.Common.Players;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Mounts;
using CatalystMod.NPCs.Boss.Astrageldon;
using Clamity.Content.Bosses.Clamitas.NPCs;
using Clamity.Content.Bosses.Pyrogen.NPCs;
using Clamity.Content.Bosses.WoB.NPCs;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Content.Items.Accessories;
using InfernalEclipseAPI.Content.Items.Materials;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.CalamityAddons;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.CalamityAddons.Clamity;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.CalamityAddons.WoTG;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using NoxusBoss.Assets;
using NoxusBoss.Content.Items;
using NoxusBoss.Content.NPCs.Bosses.Avatar.SecondPhaseForm;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using InfernumSaveSystem = InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Configs;
using CalamityMod.Buffs.StatDebuffs;
using CatalystMod.NPCs;
using InfernalEclipseAPI.Common.GlobalNPCs.LootAdjustments;

namespace InfernalEclipseAPI.Common.GlobalNPCs.InfernalRelics
{
    [ExtendsFromMod("CatalystMod")]
    public class CatalystInfernalRelics : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;

            if (npc.type == ModContent.NPCType<Astrageldon>() && !ModLoader.TryGetMod("CnI", out _))
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<AstrageldonRelic>());

                foreach (IItemDropRule rule in npcLoot.Get())
                {
                    if (rule is CommonDrop drop && drop.itemId == ModContent.ItemType<CatalystMod.Items.Placeable.Furniture.BossTrophies.AstrageldonRelic>())
                    {
                        npcLoot.Remove(drop);
                    }
                }
                npcLoot.AddIf(() => !NPC.downedMoonlord, ModContent.ItemType<CatalystMod.Items.Placeable.Furniture.BossTrophies.AstrageldonRelic>());

                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<AstrageldonRelicGold>()));
                npcLoot.Add(ItemDropRule.ByCondition(new RevengenceMode(), ModContent.ItemType<AstrageldonRelicGold>(), 1, 1, 1, 1));
            }
        }
    }

    [JITWhenModsEnabled("CalamityHunt")]
    [ExtendsFromMod("CalamityHunt")]
    public class HuntInfernalRelics : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;
            if (ModLoader.TryGetMod("CalamityHunt", out Mod hunt))
            {
                if (npc.type == hunt.Find<ModNPC>("Goozma").Type)
                {
                    npcLoot.AddIf(isInfernum, ModContent.ItemType<GoozmaRelic>());

                    int relic = ModContent.ItemType<CalamityHunt.Content.Items.Placeable.GoozmaInfernumRelic>();

                    npcLoot.RemoveWhere(rule =>
                    {
                        if (rule is CommonDrop directDrop && directDrop.itemId == relic)
                            return true;

                        if (rule is LeadingConditionRule leading)
                        {
                            return leading.ChainedRules.Any(chain =>
                                chain.RuleToChain is CommonDrop drop &&
                                drop.itemId == relic
                            );
                        }

                        return false;
                    });
                }
            }
        }
    }

    [ExtendsFromMod("CalamityHunt")]
    public static class StressMeterOverrides
    {
        public static void DisableStress(Player player)
        {
            player.GetModPlayer<SplendorJamPlayer>().active = false;
            player.GetModPlayer<SplendorJamPlayer>().stress = 0;
            player.GetModPlayer<SplendorJamPlayer>().stressedOut = false;
        }
    }

    [ExtendsFromMod("NoxusBoss")]
    public class WrathInfernalRelics : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (!InfernalConfig.Instance.CalamityBalanceChanges || !npc.active) return base.PreAI(npc);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    if (npc.ModNPC is NamelessDeityBoss ND)
                    {
                        if (player.mount?.Type == ModContent.MountType<DraedonGamerChairMount>())
                        {
                            player.mount.Dismount(player);
                            SoundEngine.PlaySound(GennedAssets.Sounds.NamelessDeity.Chuckle, player.Center);
                        }

                        if (InfernalCrossmod.Clamity.Loaded)
                        {
                            if (player.mount?.Type == InfernalCrossmod.Clamity.Mod.Find<ModMount>("PlagueChairMount").Type)
                            {
                                player.mount.Dismount(player);
                                SoundEngine.PlaySound(GennedAssets.Sounds.NamelessDeity.Chuckle, player.Center);
                            }
                        }

                        if (ND.CurrentPhase > 0)
                        {
                            if (ModLoader.HasMod("CalamityHunt"))
                            {
                                StressMeterOverrides.DisableStress(player);
                            }

                            CalamityPlayer mp = player.Calamity();
                            mp.rage = 0;
                            mp.rageModeActive = false;
                            mp.adrenaline = 0;
                            mp.adrenalineModeActive = false;

                            if (InfernalWorld.RagnarokModeEnabled)
                            {
                                if (!player.ghost)
                                    player.AddBuff(ModContent.BuffType<BrimstoneDesperation>(), 2);

                                if (!npc.buffImmune[ModContent.BuffType<Enraged>()])
                                    npc.buffImmune[ModContent.BuffType<Enraged>()] = true;

                                player.buffImmune[ModContent.BuffType<Enraged>()] = true;
                                player.Calamity().enraged = false;
                            }
                            else
                                npc.buffImmune[ModContent.BuffType<Enraged>()] = false;
                        }
                    }

                    if (npc.type == ModContent.NPCType<AvatarOfEmptiness>())
                    {
                        if (InfernumSaveSystem.InfernumModeEnabled)
                        {
                            if (player.mount.Active)
                            {
                                player.mount.Dismount(player);
                                CombatText.NewText(player.Hitbox, Color.BlueViolet, "Can't do that right now!", true);
                            }
                        }

                        if (npc.HasValidTarget)
                        {
                            if (player.Distance(Main.player[npc.target].Center) >= 10000f)
                            {
                                player.position = Main.player[npc.target].Center;
                                SoundEngine.PlaySound(BossRushEvent.TeleportSound with { Volume = 1.6f }, player.Center);
                            }
                        }
                    }
                }
            }
            return base.PreAI(npc);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;
            if (npc.type == ModContent.NPCType<AvatarOfEmptiness>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<AvatarOfEmptinessRelic>());
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MetallicChunk>(), 1, 4, 9));
            }
            if (npc.type == ModContent.NPCType<NamelessDeityBoss>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<NamelessDeityRelic>());
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoltanBullyingSlip>(), 1));
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<PrimordialOrchid>(), 1, 10, 15));

                npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ModContent.ItemType<CheatPermissionSlip>());
            }
        }
    }

    [ExtendsFromMod("NoxusBoss")]
    public class WrathTreasureBags : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            Mod noxusBoss = ModLoader.GetMod("NoxusBoss");
            if (item.type == noxusBoss.Find<ModItem>("AvatarTreasureBag").Type)
                itemLoot.Add(ModContent.ItemType<MetallicChunk>(), 1, 4, 9);
            if (item.type == noxusBoss.Find<ModItem>("NamelessDeityTreasureBag").Type)
            {
                itemLoot.Add(ModContent.ItemType<PrimordialOrchid>(), 1, 10, 15);

                itemLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ModContent.ItemType<CheatPermissionSlip>());
            }
        }
    }

    [ExtendsFromMod("Clamity")]
    public class ClamityInfernalRelic : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;
            if (npc.type == ModContent.NPCType<PyrogenBoss>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PyrogenRelic>());
            }
            if (npc.type == ModContent.NPCType<ClamitasBoss>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<ClamitasRelic>());
            }
            if (npc.type == ModContent.NPCType<WallOfBronze>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<WallofBronzeRelic>());
            }
        }
    }

    public class WarMachineInfernalRelic : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            bool isInfernum() => InfernumSaveSystem.InfernumModeEnabled;
            if (ModLoader.TryGetMod("CalamityAddon", out Mod warMachine))
            {
                if (npc.type == warMachine.Find<ModNPC>("WulfrumMothership").Type)
                {
                    npcLoot.AddIf(isInfernum, ModContent.ItemType<WulfrumMothershipRelic>());
                }
            }
        }
    }
}