using CalamityMod;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.SlimeGod;
using InfernalEclipseAPI.Content.Items.Lore.Thorium;
using InfernalEclipseAPI.Core.Systems;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod;
using ThoriumMod.Buffs;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.Misc;
using ThoriumMod.NPCs.BossFallenBeholder;
using ThoriumMod.NPCs.BossStarScouter;
using ThoriumMod.NPCs.BossTheGrandThunderBird;
using ThoriumMod.NPCs.BossThePrimordials;
using ThoriumMod.NPCs.Depths;
using ThoriumMod.NPCs;
using MonoMod.Cil;
using System.Reflection;
using ThoriumMod.Items.BossLich;
using ThoriumMod.Items.BossStarScouter;
using Mono.Cecil.Cil;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Materials;
using InfernalEclipseAPI.Content.Items.Placeables.Relics.Thorium;
using ThoriumMod.NPCs.BossBoreanStrider;
using ThoriumMod.NPCs.BossBuriedChampion;
using ThoriumMod.NPCs.BossForgottenOne;
using ThoriumMod.NPCs.BossGraniteEnergyStorm;
using ThoriumMod.NPCs.BossLich;
using ThoriumMod.NPCs.BossMini;
using ThoriumMod.NPCs.BossQueenJellyfish;
using ThoriumMod.NPCs.BossViscount;
using InfernumMode.Core.GlobalInstances.Systems;
using InfernalEclipseAPI.Content.Items.SpawnItems;

namespace InfernalEclipseAPI.Common.GlobalNPCs
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ThoriumGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            int oozed = ModContent.BuffType<Oozed>();
            int stunned = ModContent.BuffType<Stunned>();

            // Perforator worms
            if (
                npc.type == ModContent.NPCType<PerforatorHeadSmall>() ||
                npc.type == ModContent.NPCType<PerforatorBodySmall>() ||
                npc.type == ModContent.NPCType<PerforatorTailSmall>() ||

                npc.type == ModContent.NPCType<PerforatorHeadMedium>() ||
                npc.type == ModContent.NPCType<PerforatorBodyMedium>() ||
                npc.type == ModContent.NPCType<PerforatorTailMedium>() ||

                npc.type == ModContent.NPCType<PerforatorHeadLarge>() ||
                npc.type == ModContent.NPCType<PerforatorBodyLarge>() ||
                npc.type == ModContent.NPCType<PerforatorTailLarge>() ||

                // Slime God’s summoned slimes
                npc.type == ModContent.NPCType<CrimulanPaladin>() ||
                npc.type == ModContent.NPCType<EbonianPaladin>() ||

                npc.type == ModContent.NPCType<SplitCrimulanPaladin>() ||
                npc.type == ModContent.NPCType<SplitEbonianPaladin>() ||

                //Profaned Guardians
                npc.type == ModContent.NPCType<ProfanedGuardianCommander>() ||
                npc.type == ModContent.NPCType<ProfanedGuardianDefender>() ||
                npc.type == ModContent.NPCType<ProfanedGuardianHealer>()
                )
            {
                npc.buffImmune[oozed] = true;
                npc.buffImmune[stunned] = true;
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<Lich>() || npc.type == ModContent.NPCType<LichHeadless>())
            {
                Main.GraveyardVisualIntensity = 1f;
            }
            return base.PreAI(npc);
        }

        public override bool PreKill(NPC npc)
        {
            if (npc.type == ModContent.NPCType<Lich>() || npc.type == ModContent.NPCType<LichHeadless>())
            {
                Main.GraveyardVisualIntensity = 0f;
            }
            return base.PreKill(npc);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<GigaClam>())
            {
                npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<MolluskHusk>(), 2, 1, 1);
            }

            int[] meteoriteEnemies =
            {
                ModContent.NPCType<UFO>(),
                ModContent.NPCType<MartianScout>(),
                ModContent.NPCType<MartianSentry>(),
            };

            if (npc.type == NPCID.BloodNautilus)
                npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<BloodCell>(), 1, 15, 20);

            if (ModLoader.HasMod("SOTS"))
            {
                foreach (int npcID in meteoriteEnemies)
                {
                    if (npc.type == npcID)
                    {
                        npcLoot.AddIf(() => AncientPhylacteryRightClickBlocker.DownedAdvisor, ModLoader.GetMod("SOTS").Find<ModItem>("TwilightShard").Type, 3);
                    }
                }

                if (npc.type == ModContent.NPCType<StarScouter>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModLoader.GetMod("SOTS").Find<ModItem>("TwilightShard").Type, 1, 5, 10));
                }
            }

            #region Infernal Relics & Lore
            bool isInfernum() => WorldSaveSystem.InfernumModeEnabled;
            if (npc.type == ModContent.NPCType<TheGrandThunderBird>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<GrandThunderBirdRelic>());

                bool firstBirdKill() => !ThoriumWorld.downedTheGrandThunderBird;
                npcLoot.AddConditionalPerPlayer(firstBirdKill, ModContent.ItemType<LoreThunderBird>(), desc: DropHelper.FirstKillText);
            }
            if (npc.type == ModContent.NPCType<PatchWerk>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PatchWerkRelic>());
            }
            if (npc.type == ModContent.NPCType<QueenJellyfish>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<QueenJellyfishRelic>());
            }
            if (npc.type == ModContent.NPCType<Viscount>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<ViscountRelic>());
            }
            if (npc.type == ModContent.NPCType<CorpseBloom>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<CorpseBloomRelic>());
            }
            if (npc.type == ModContent.NPCType<Illusionist>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<IllusionistRelic>());
            }
            if (npc.type == ModContent.NPCType<GraniteEnergyStorm>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<GraniteEnergyStormRelic>());
            }
            if (npc.type == ModContent.NPCType<BuriedChampion>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<BurriedChampionRelic>());
            }
            if (npc.type == ModContent.NPCType<StarScouter>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<StarScouterRelic>());
            }
            if (npc.type == ModContent.NPCType<BoreanStriderPopped>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<BoreanStriderRelic>());
            }
            if (npc.type == ModContent.NPCType<FallenBeholder>() || npc.type == ModContent.NPCType<FallenBeholder2>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<FallenBeholderRelic>());
            }
            if (npc.type == ModContent.NPCType<LichHeadless>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<LichRelic>());
            }
            if (npc.type == ModContent.NPCType<ForgottenOneReleased>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<ForgottenOneRelic>());
            }
            if (npc.type == ModContent.NPCType<DreamEater>())
            {
                npcLoot.AddIf(isInfernum, ModContent.ItemType<PrimordialsRelic>());

                bool firstPrimordialKill() => !ThoriumWorld.downedThePrimordials;
                npcLoot.AddConditionalPerPlayer(firstPrimordialKill, ModContent.ItemType<LorePrimordials>(), desc: DropHelper.FirstKillText);
                npcLoot.AddConditionalPerPlayer(firstPrimordialKill, ModContent.ItemType<LoreRagnarok>(), desc: DropHelper.FirstKillText);
            }
            #endregion

            #region Biome Keys
            if (npc.type == ModContent.NPCType<SubmergedMimic>())
            {
                npcLoot.Add(ModContent.ItemType<AquaticDepthsBiomeKey>(), 10);
            }

            if (npc.type == ModContent.NPCType<HellBringerMimic>())
            {
                npcLoot.Add(ModContent.ItemType<UnderworldBiomeKey>(), 10);
            }
            #endregion
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ThoriumLootBagAdjustments : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<StarScouterTreasureBag>() && ModLoader.HasMod("SOTS"))
            {
                itemLoot.AddIf(() => AncientPhylacteryRightClickBlocker.DownedAdvisor, ModLoader.GetMod("SOTS").Find<ModItem>("TwilightShard").Type, 1, 7, 14);
            }

            if (item.type == ModContent.ItemType<LichTreasureBag>() && InfernalCrossmod.ThoriumRework.Loaded && !InfernalCrossmod.Hummus.Loaded)
            {
                if (InfernalCrossmod.ThoriumRework.Mod.TryFind("SoulSnatcher", out ModItem soulSnacter))
                {
                    var rule = new CommonDropNotScalingWithLuck(soulSnacter.Type, 5, 1, 1)
                    {
                        chanceNumerator = 2 // 2/5 = 40%
                    };
                    itemLoot.Add(rule);
                }
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class PrimordialPotionPatch : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModLoader.HasMod("ThoriumRework");

        public override void Load()
        {
            var primodialMethod = typeof(PrimordialBase).GetMethod(nameof(PrimordialBase.BossLoot), LumUtils.UniversalBindingFlags, null, new Type[] { typeof(int).MakeByRefType() }, null);
            var dreamEaterMethod = typeof(DreamEater).GetMethod(nameof(DreamEater.BossLoot), LumUtils.UniversalBindingFlags, null, new Type[] { typeof(int).MakeByRefType() }, null);

            if (primodialMethod != null)
                MonoModHooks.Modify(primodialMethod, IL_BossLoot);

            if (dreamEaterMethod != null)
                MonoModHooks.Modify(dreamEaterMethod, IL_BossLoot);
        }

        private void IL_BossLoot(ILContext il)
        {
            var c = new ILCursor(il);

            while (c.TryGotoNext(i => i.OpCode == OpCodes.Ldc_I4 && (int)i.Operand == 3544))
            {
                c.Remove();
                c.Emit(OpCodes.Call, typeof(ModContent).GetMethod(nameof(ModContent.ItemType)).MakeGenericMethod(typeof(OmegaHealingPotion)));
            }
        }
    }
}
