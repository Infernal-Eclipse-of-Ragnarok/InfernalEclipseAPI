using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using CalamityMod.NPCs;
using CalamityMod.Systems.Collections;
using CalamityMod.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Linq;
using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Content.Buffs.SoulBurn;
using static InfernalEclipseAPI.Core.Systems.InfernalCrossmod;
using Terraria;
using System.Reflection;
using static ThrowerUnification.ModCompatibility;

namespace InfernalEclipseAPI.Common.Globals.GlobalNPCs
{
    public class IEORDebuffDisplay : GlobalNPC
    {
        private static void AddExternalBuff(List<Texture2D> debuffs,NPC npc,string modName,string buffName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod))
                return;

            if (!mod.TryFind<ModBuff>(buffName, out var buff))
                return;

            if (npc.HasBuff(buff.Type))
                debuffs.Add(TextureAssets.Buff[buff.Type].Value);
        }

        private static void RegisterExternalBuff(Mod entropy, string modName, string buffName)
        {
            entropy.Call(
                "RegisterDebuff",
                (Func<NPC, bool>)(npc =>
                {
                    if (!ModLoader.TryGetMod(modName, out Mod mod))
                        return false;

                    if (!mod.TryFind<ModBuff>(buffName, out var buff))
                        return false;

                    return npc.HasBuff(buff.Type);
                }),
                (Func<Texture2D>)(() =>
                {
                    if (!ModLoader.TryGetMod(modName, out Mod mod))
                        return null;

                    if (!mod.TryFind<ModBuff>(buffName, out var buff))
                        return null;

                    return TextureAssets.Buff[buff.Type].Value;
                })
            );
        }

        public static bool RegisteredWithEntropy;

        public override void Load()
        {
            // If Entropy enabled, intergrate with their system
            RegisteredWithEntropy = false;

            if (ModLoader.TryGetMod("CalamityEntropyLegacy", out Mod entropyLegacy))
            {
                try
                {
                    //IEoR
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn>()].Value));
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn2>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn2>()].Value));
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn3>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn3>()].Value));
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn4>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn4>()].Value));
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn5>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn5>()].Value));
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn6>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn6>()].Value));
                    entropyLegacy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn7>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn7>()].Value));

                    //Thorium
                    if (InfernalCrossmod.Thorium.Loaded)
                    {
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "BlightFever");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "BloodyWandDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Charmed");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Corrosion");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "DarkContagionDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "DecayingFlesh");
                        //RegisterExternalBuff(entropyLegacy, "ThoriumMod", "MagickStaffDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Enfeeble");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Freezing");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "FungalGrowth");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Gouge");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "HolyGlare");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "HoneyRecorderDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "IlluminatedNPC");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Insanity");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "LightCurse");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "StrangeSkullDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "LegacyDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "NapalmDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Paralyzed");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Petrify");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "MirroroftheBeholderDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "SchmelzeDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Singed");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "SmitingHammerDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "SpearmintDebuff");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Spored");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Stunned");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Sundered");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "TerrariumBacklash");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Tuned");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "Wither");
                        RegisterExternalBuff(entropyLegacy, "ThoriumMod", "GraniteSurge");
                    }
                    //Catalyst
                    if (InfernalCrossmod.Catalyst.Loaded)
                    {
                        RegisterExternalBuff(entropyLegacy, "CatalystMod", "AstralBlight");
                        RegisterExternalBuff(entropyLegacy, "CatalystMod", "InterstellarCorruption");
                    }
                    //HuntofTheOldGod
                    if (InfernalCrossmod.Hunt.Loaded)
                    {
                        RegisterExternalBuff(entropyLegacy, "CalamityHunt", "FusionBurn");
                        RegisterExternalBuff(entropyLegacy, "CalamityHunt", "Swamped");
                    }
                    //Consolaria
                    if (InfernalCrossmod.Consolaria.Loaded)
                    {
                        RegisterExternalBuff(entropyLegacy, "Consolaria", "Stunned");
                    }
                    //Ragnarok
                    if (InfernalCrossmod.RagnarokMod.Loaded)
                    {
                        RegisterExternalBuff(entropyLegacy, "RagnarokMod", "NightfallenDebuff");
                    }

                    RegisteredWithEntropy = true;
                }
                catch (Exception ex)
                {
                    Mod.Logger.Warn($"Failed to register IEoR debuffs with Entropy: {ex}");
                }
            }
            else if (ModLoader.TryGetMod("CalamityEntropy", out Mod entropy))
            {
                try
                {
                    //IEoR
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn>()].Value));
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn2>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn2>()].Value));
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn3>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn3>()].Value));
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn4>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn4>()].Value));
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn5>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn5>()].Value));
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn6>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn6>()].Value));
                    entropy.Call("RegisterDebuff", (Func<NPC, bool>)(npc => npc.HasBuff<SoulBurn7>()), (Func<Texture2D>)(() => TextureAssets.Buff[ModContent.BuffType<SoulBurn7>()].Value));

                    //Thorium
                    if (InfernalCrossmod.Thorium.Loaded)
                    {
                        RegisterExternalBuff(entropy, "ThoriumMod", "BlightFever");
                        RegisterExternalBuff(entropy, "ThoriumMod", "BloodyWandDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Charmed");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Corrosion");
                        RegisterExternalBuff(entropy, "ThoriumMod", "DarkContagionDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "DecayingFlesh");
                        //RegisterExternalBuff(entropy, "ThoriumMod", "MagickStaffDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Enfeeble");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Freezing");
                        RegisterExternalBuff(entropy, "ThoriumMod", "FungalGrowth");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Gouge");
                        RegisterExternalBuff(entropy, "ThoriumMod", "HolyGlare");
                        RegisterExternalBuff(entropy, "ThoriumMod", "HoneyRecorderDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "IlluminatedNPC");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Insanity");
                        RegisterExternalBuff(entropy, "ThoriumMod", "LightCurse");
                        RegisterExternalBuff(entropy, "ThoriumMod", "StrangeSkullDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "LegacyDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "NapalmDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Paralyzed");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Petrify");
                        RegisterExternalBuff(entropy, "ThoriumMod", "MirroroftheBeholderDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "SchmelzeDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Singed");
                        RegisterExternalBuff(entropy, "ThoriumMod", "SmitingHammerDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "SpearmintDebuff");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Spored");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Stunned");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Sundered");
                        RegisterExternalBuff(entropy, "ThoriumMod", "TerrariumBacklash");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Tuned");
                        RegisterExternalBuff(entropy, "ThoriumMod", "Wither");
                        RegisterExternalBuff(entropy, "ThoriumMod", "GraniteSurge");
                    }
                    //Catalyst
                    if (InfernalCrossmod.Catalyst.Loaded)
                    {
                        RegisterExternalBuff(entropy, "CatalystMod", "AstralBlight");
                        RegisterExternalBuff(entropy, "CatalystMod", "InterstellarCorruption");
                    }
                    //HuntofTheOldGod
                    if (InfernalCrossmod.Hunt.Loaded)
                    {
                        RegisterExternalBuff(entropy, "CalamityHunt", "FusionBurn");
                        RegisterExternalBuff(entropy, "CalamityHunt", "Swamped");
                    }
                    //Consolaria
                    if (InfernalCrossmod.Consolaria.Loaded)
                    {
                        RegisterExternalBuff(entropy, "Consolaria", "Stunned");
                    }
                    //Ragnarok
                    if (InfernalCrossmod.RagnarokMod.Loaded)
                    {
                        RegisterExternalBuff(entropy, "RagnarokMod", "NightfallenDebuff");
                    }

                    RegisteredWithEntropy = true;
                }
                catch (Exception ex)
                {
                    Mod.Logger.Warn($"Failed to register IEoR debuffs with Entropy: {ex}");
                }
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Our system if the call to intergrate fails/if Entropy isn't enabled
            if (npc.type != NPCID.BrainofCthulhu && (npc.type != NPCID.DukeFishron || npc.ai[0] <= 9f) && npc.active && !RegisteredWithEntropy)
            {
                if (CalamityClientConfig.Instance.DebuffDisplay && (npc.boss || BossHealthBarManager.MinibossHPBarList.Contains(npc.type) || BossHealthBarManager.OneToMany.ContainsKey(npc.type) || CalamityNPCSets.ForceDrawDebuffDisplay[npc.type]))
                {
                    List<Texture2D> currentDebuffs = new List<Texture2D>() { };
                    CalamityGlobalNPC cnpc = npc.Calamity();


                    //Calamity debuffs
                    for (int b = 0; b < CalamityGlobalNPC.moddedDebuffTextureList.Count(); b++)
                    {
                        if (CalamityGlobalNPC.moddedDebuffTextureList[b].Item2.Invoke(npc))
                        {
                            currentDebuffs.Add(Request<Texture2D>(CalamityGlobalNPC.moddedDebuffTextureList[b].Item1).Value);
                        }
                    }

                    // Vanilla damage over time debuffs
                    if (cnpc.electrified)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Electrified].Value);
                    if (npc.onFire)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.OnFire].Value);
                    if (npc.poisoned)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Poisoned].Value);
                    if (npc.onFire2)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.CursedInferno].Value);
                    if (npc.onFrostBurn)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Frostburn].Value);
                    if (npc.venom)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Venom].Value);
                    if (npc.shadowFlame)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.ShadowFlame].Value);
                    if (npc.oiled)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Oiled").Value);
                    if (npc.javelined)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.BoneJavelin].Value);
                    if (npc.daybreak)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/Buffs/DamageOverTime/Daybroken").Value);
                    if (npc.celled)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Celled").Value);
                    if (npc.dryadBane)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/DryadsBane").Value);
                    if (npc.dryadWard)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.DryadsWard].Value);
                    if (npc.soulDrain && npc.realLife == -1)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.SoulDrain].Value);
                    if (npc.onFire3) // Hellfire
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Hellfire").Value);
                    if (npc.onFrostBurn2) // Frostbite
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Frostbite").Value);
                    if (npc.tentacleSpiked)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.TentacleSpike].Value);

                    // Vanilla stat debuffs
                    if (npc.confused)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Confused].Value);
                    if (npc.ichor)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Ichor].Value);
                    if (cnpc.webbed)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Webbed].Value);
                    if (npc.midas)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Midas].Value);
                    if (npc.loveStruck)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Lovestruck].Value);
                    if (npc.stinky)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Stinky].Value);
                    if (npc.betsysCurse)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/BetsysCurse").Value);
                    if (npc.dripping)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Wet].Value);
                    if (npc.drippingSlime)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Slimed].Value);
                    if (npc.drippingSparkleSlime)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.GelBalloonBuff].Value);

                    void AddBuffDraw<T>() where T : ModBuff
                    {
                        if (npc.HasBuff<T>())
                        {
                            currentDebuffs.Add(TextureAssets.Buff[ModContent.BuffType<T>()].Value);
                        }
                    }

                    AddBuffDraw<SoulBurn>();
                    AddBuffDraw<SoulBurn2>();
                    AddBuffDraw<SoulBurn3>();
                    AddBuffDraw<SoulBurn4>();
                    AddBuffDraw<SoulBurn5>();
                    AddBuffDraw<SoulBurn6>();
                    AddBuffDraw<SoulBurn7>();

                    if (InfernalCrossmod.Thorium.Loaded)
                    {
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "BlightFever");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "BloodyWandDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Charmed");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Corrosion");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "DarkContagionDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "DecayingFlesh");
                        //AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "MagickStaffDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Enfeeble");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Freezing");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "FungalGrowth");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Gouge");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "HolyGlare");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "HoneyRecorderDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "IlluminatedNPC");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Insanity");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "LightCurse");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "StrangeSkullDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "LegacyDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "NapalmDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Paralyzed");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Petrify");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "MirroroftheBeholderDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "SchmelzeDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Singed");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "SmitingHammerDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "SpearmintDebuff");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Spored");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Stunned");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Sundered");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "TerrariumBacklash");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Tuned");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "Wither");
                        AddExternalBuff(currentDebuffs, npc, "ThoriumMod", "GraniteSurge");
                    }
                    if (InfernalCrossmod.Catalyst.Loaded)
                    {
                        AddExternalBuff(currentDebuffs, npc, "CatalystMod", "AstralBlight");
                        AddExternalBuff(currentDebuffs, npc, "CatalystMod", "InterstellarCorruption");
                    }
                    if (InfernalCrossmod.Hunt.Loaded)
                    {
                        AddExternalBuff(currentDebuffs, npc, "CalamityHunt", "FusionBurn");
                        AddExternalBuff(currentDebuffs, npc, "CalamityHunt", "Swamped");
                    }
                    if (InfernalCrossmod.Consolaria.Loaded)
                    {
                        AddExternalBuff(currentDebuffs, npc, "Consolaria", "Stunned");
                    }
                    //Ragnarok
                    if (InfernalCrossmod.RagnarokMod.Loaded)
                    {
                        AddExternalBuff(currentDebuffs, npc, "RagnarokMod", "NightfallenDebuff");
                    }

                    // Total amount of elements in the buff list
                    int currentDebuffsLength = currentDebuffs.Count();

                    int buffTextureListLength = currentDebuffs.Count;
                    // Total length of a single row in the buff display
                    int totalLength = buffTextureListLength * 14;
                    // Max amount of buffs per row
                    int buffDisplayRowLimit = 5;
                    // The maximum length of a single row in the buff display
                    // Limited to 80 units, because every buff drawn here is half the size of a normal buff, 16 x 16, 16 * 5 = 80 units
                    float drawPosX = totalLength >= 80f ? 40f : (float)(totalLength / 2);
                    // The height of a single frame of the npc
                    float npcHeight = (npc.height * npc.scale) / 2;
                    // Offset the debuff display based on the npc's graphical offset, and 16 units, to create some space between the sprite and the display
                    float drawPosY = npcHeight + npc.gfxOffY + 32f;

                    // Iterate through the buff texture list
                    for (int i = 0; i < currentDebuffs.Count; i++)
                    {
                        // Reset the X position of the display every 5th and non-zero iteration, otherwise decrease the X draw position by 16 units
                        if (i != 0)
                        {
                            if (i % buffDisplayRowLimit == 0)
                                drawPosX = 40f;
                            else
                                drawPosX -= 14f;
                        }

                        // Offset the Y position every row after 5 iterations to limit each displayed row to 5 debuffs
                        float additionalYOffset = 14f * (float)Math.Floor(i * 0.2);

                        var tex = currentDebuffs[i];
                        spriteBatch.Draw(tex, npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), null, Color.White, 0f, default, 0.5f, SpriteEffects.None, 0f);
                    }
                }
            }

            return true;
        }
    }
}
