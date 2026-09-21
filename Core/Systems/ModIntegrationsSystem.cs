using InfernalEclipseAPI.Content.Items.Placeables.MusicBoxes;
using InfernalEclipseAPI.Content.Items.Weapons.Catlight;
using InfernalEclipseAPI.Content.NPCs.LittleCat;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using InfernalEclipseAPI.Core.DamageClasses.MythicClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SOTS.Void;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;
using ThoriumMod.Scenes;

namespace InfernalEclipseAPI.Core.Systems
{
    public class ModIntegrationsSystem : ModSystem
    {
        internal static Mod Infernum;
        internal static Mod SOTS;
        internal static Mod Fargos;

        public override void Load()
        {
            ModLoader.TryGetMod("InfernumMode", out Infernum);
            ModLoader.TryGetMod("SOTS", out SOTS);
            ModLoader.TryGetMod("FargowiltasSouls", out Fargos);
        }

        public override void Unload()
        {
            Infernum = null;
            SOTS = null;
            Fargos = null;
        }

        public override void PostSetupContent()
        {
            MusicDisplaySetup();
            BossChecklistSetup();
            AddInfernumCards();
            ColoredDamageTypesSupport();
            Project_tRUSupport();
            RecipeBrowserSetup();

            if (InfernalCrossmod.Thorium.Loaded)
            {
                AddThoriumMiniBoss.AddMiniBosses();
            }
        }

        private static void MusicDisplaySetup()
        {
            ModLoader.TryGetMod("MusicDisplay", out Mod musicDisplay);
            if (musicDisplay is null)
            {
                return;
            }

            //IEoR Original Music
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/TeardropsofDragonfire"), "Teardrops of Dragonfire", "by PFungi", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/Seahorse"), "Seahorse Scrutiny (Rebirth)", "by Brimston Premium", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/InfernalThunderBird"), "Infernal Thunder Bird", "by Techhy", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/MutantMajesty"), "Mutant Majesty", "by Techhy", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/VampiricVitality"), "Carotid Crypt", "by Techhy & PFungi", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/Stonewarden"), "Stonewarden", "by Techhy", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/BereftVassal"), "Dead Kingdom's Champion", "by Techhy", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot(InfernalEclipseAPI.ProvidenceNightPath), "Unholy Finality", "by Brimston Premium", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/LastBattle"), "Last Battle (Ballos Mix)", "by DM DOKURO", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/LastBattleDesperation"), "Last Battle (Ballos Mix)", "by DM DOKURO", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/LastBattleDesperationCut"), "Last Battle (Ballos Mix)", "by DM DOKURO", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/Arsenal"), "Arsenal", "by TheTrester", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/Interlude04"), "Calamity before the cynosure", "by theforge129", "Infernal Eclipse of Ragnarok");

            //Boss Rush Themes
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/tier5"), "Omiscience Of Gods", "by TheTrester", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/tier6"), "Onslaught of Beasts (Encore Mix)", "by CDMusic", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/TWISTEDGARDENRemix"), "TWISTED GARDEN [Remix]", "by Kuudray", "Infernal Eclipse of Ragnarok");
            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/EnsembleofFools(EncoreMix)"), "Ensemble of Fools (Encore Mix)", "by CDMusic", "Infernal Eclipse of Ragnarok");

            // Currently unused
            //musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("InfernalEclipseAPI/Assets/Music/TheRealityoftheProphey"), "The Reality of the Prophecy", "theforge129", "Infernal Eclipse of Ragnarok");
            

            if (ModLoader.TryGetMod("YouBoss", out _))
            {
                musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("YouBoss/Assets/Sounds/Music/You"), "FINAL FRACTAL", "by ENNWAY", "You");
            }

            if (ModLoader.TryGetMod("NoxusBoss", out _))
            {
                musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("NoxusBoss/Assets/Sounds/Music/Mars"), "RAMifications", "by moonburn", "Calamity: Wrath of the Gods");
                musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("NoxusBoss/Assets/Sounds/Music/AvatarOfEmptinessP2"), "PARADISE PARASITE (Avatar)", "by ENNWAY", "Calamity: Wrath of the Gods");
                musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("NoxusBoss/Assets/Sounds/Music/AvatarOfEmptinessP3"), "PARADISE PARASITE (Paradise)", "by ENNWAY", "Calamity: Wrath of the Gods");
                musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("NoxusBoss/Assets/Sounds/Music/NamelessDeity"), "TWISTED GARDEN", "by ENNWAY & HeartPlusUp!", "Calamity: Wrath of the Gods");
            }
        }
        private static void RecipeBrowserSetup()
        {
            if (ModLoader.TryGetMod("RecipeBrowser", out Mod mod))
            {
                const string path = "InfernalEclipseAPI/Assets/Images/UI/RecipeBrowser/ClassIcon";
                mod.Call("AddItemCategory", Language.GetTextValue("Mods.InfernalEclipseAPI.UI.RecipeBrowserCategories.Legendary"), "Weapons", ModContent.Request<Texture2D>(path + "Legendary"), (Predicate<Item>)(item => item.DamageType == ModContent.GetInstance<LegendaryMagic>() || item.DamageType == ModContent.GetInstance<LegendaryMelee>() || item.DamageType == ModContent.GetInstance<LegendaryRanged>() || item.DamageType == ModContent.GetInstance<LegendarySummonMeleeSpeed>() || item.type == ModContent.ItemType<Catlight>()));
                mod.Call("AddItemCategory", Language.GetTextValue("Mods.InfernalEclipseAPI.UI.RecipeBrowserCategories.Mythical"), "Weapons", ModContent.Request<Texture2D>(path + "Mythic"), (Predicate<Item>)(item => item.DamageType == ModContent.GetInstance<MythicMagic>() || item.DamageType == ModContent.GetInstance<MythicMelee>() || item.DamageType == ModContent.GetInstance<MythicRanged>() || item.DamageType == ModContent.GetInstance<MythicSummon>()));
            }
        }
        private static void BossChecklistSetup()
        {
            Mod mod1;
            Mod CalamityMod;
            ModLoader.TryGetMod("CalamityMod", out CalamityMod);
            if (!ModLoader.TryGetMod("BossChecklist", out mod1) || mod1.Version < new Version(1, 6))
                return;

            if (ModLoader.HasMod("InfernumModeMusic"))
            {
                mod1.Call(new object[3]
                {
                "AddToBossCollection",
                "CalamityMod Exo Mechs",
                new List<int>()
                {
                    ModContent.ItemType<CatastrophicFabricationsMusicBox>()
                }
                });
            }
        }

        internal void AddInfernumCards()
        {
            if (Infernum is null) return;

            MakeCard(ModContent.NPCType<LittleCat>(), (horz, anim) => Color.Lerp(Color.MediumPurple, Color.DarkViolet, anim), "LittleCat", SoundID.NPCHit4, SoundID.ScaryScream);

            if (SOTS != null)
            {
                MakeCard(SOTS.Find<ModNPC>("Polaris").Type, (horz, anim) => Color.Lerp(Color.Aquamarine, Color.Red, anim), "Polaris", SoundID.NPCHit4, new SoundStyle("InfernumMode/Assets/Sounds/Custom/ExoMechs/ThanatosTransition"));
                MakeCard(SOTS.Find<ModNPC>("NewPolaris").Type, (horz, anim) => Color.Lerp(Color.Aquamarine, Color.Red, anim), "NewPolaris", SoundID.NPCHit4, new SoundStyle("InfernumMode/Assets/Sounds/Custom/ExoMechs/ThanatosTransition"));
            }
            if (Fargos != null && InfernalConfig.Instance.DontEnableThis)
            {
                MakeCard(Fargos.Find<ModNPC>("TrojanSquirrel").Type, (horz, anim) => Color.Lerp(Color.Brown, Color.SaddleBrown, anim), "TrojanSquirrel", SoundID.NPCHit4, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("CursedCoffin").Type, (horz, anim) => Color.Lerp(Color.SandyBrown, Color.Yellow, anim), "CursedCoffin", SoundID.MenuTick, SoundID.Roar);
                MakeCard(Fargos.Find<ModNPC>("DeviBoss").Type, (horz, anim) => Color.Lerp(Color.IndianRed, Color.Pink, anim), "DeviBoss", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("BanishedBaron").Type, (horz, anim) => Color.Lerp(Color.SandyBrown, Color.Aqua, anim), "BanishedBaron", SoundID.NPCHit4, new SoundStyle("InfernumMode/Assets/Sounds/Custom/ExoMechs/ThanatosTransition"));
                MakeCard(Fargos.Find<ModNPC>("LifeChallenger").Type, (horz, anim) => Color.Lerp(Color.LightYellow, Color.LightGoldenrodYellow, anim), "LifeChallenger", SoundID.Pixie, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("TimberChampion").Type, (horz, anim) => Color.Lerp(Color.ForestGreen, Color.DarkSlateGray, anim), "TimberChampion", SoundID.NPCHit4, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("TerraChampion").Type, (horz, anim) => Color.Lerp(Color.OliveDrab, Color.Brown, anim), "TerraChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("EarthChampion").Type, (horz, anim) => Color.Lerp(Color.DarkGreen, Color.DarkSlateGray, anim), "EarthChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("NatureChampion").Type, (horz, anim) => Color.Lerp(Color.ForestGreen, Color.DarkOliveGreen, anim), "NatureChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("LifeChampion").Type, (horz, anim) => Color.Lerp(Color.DeepPink, Color.LightGoldenrodYellow, anim), "LifeChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("ShadowChampion").Type, (horz, anim) => Color.Lerp(Color.DarkGray, Color.Black, anim), "DeathChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("SpiritChampion").Type, (horz, anim) => Color.Lerp(Color.MediumPurple, Color.Gold, anim), "SpiritChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("WillChampion").Type, (horz, anim) => Color.Lerp(Color.Gold, Color.Goldenrod, anim), "WillChampion", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("CosmosChampion").Type, (horz, anim) => Color.Lerp(Color.DeepPink, Color.LightGoldenrodYellow, anim), "Eridanus", SoundID.MenuTick, SoundID.Item14);
                MakeCard(Fargos.Find<ModNPC>("AbomBoss").Type, (horz, anim) => Color.Lerp(Color.Purple, Color.Orange, anim), "AbomBoss", SoundID.MenuTick, InfernumMode.Assets.Sounds.InfernumSoundRegistry.ModeToggleLaugh);
                MakeCard(Fargos.Find<ModNPC>("MutantBoss").Type, (horz, anim) => Color.Lerp(Color.LightBlue, Color.Cyan, anim), "Mutant", SoundID.DD2_BetsyFireballShot, SoundID.ScaryScream);
            }

            if (ModLoader.TryGetMod("YouBoss", out Mod you))
            {
                MakeCard(you.Find<ModNPC>("TerraBladeBoss").Type, (horz, anim) => Color.Lerp(Color.LimeGreen, Color.Green, anim), "You", SoundID.MenuTick, new SoundStyle("InfernalEclipseAPI/Assets/Sounds/Custom/TerraBlade/Split"));
            }

            if (ModLoader.TryGetMod("NoxusBoss", out Mod noxus))
            {
                int marsType = noxus.Find<ModNPC>("MarsBody").Type;
                string textKey = "Mods.InfernalEclipseAPI.InfernumIntegration.Mars";
                LocalizedText introText = Language.GetOrRegister(textKey);

                // Initialize the intro screen
                object intro = Infernum.Call(
                    "InitializeIntroScreen",
                    introText,
                    150,                            // display time
                    true,                           // center text?
                    (() => NPC.AnyNPCs(marsType)),
                    (Func<float, float, Color>)((completionRatio, _) =>
                    {
                        // Base gradient between pink and yellow
                        Color baseColor = Color.Lerp(Color.HotPink, Color.Yellow, completionRatio);

                        // Pulsing white shine emphasis near the end
                        float pulse = MathF.Sin(completionRatio * MathF.PI * 4f + Main.GlobalTimeWrappedHourly * 5f) * 0.5f + 0.5f;
                        float t = Terraria.Utils.GetLerpValue(0.8f, 1f, pulse, true);

                        return Color.Lerp(baseColor, Color.White, t);
                    })
                );

                // Scale, sounds, and behavior
                Infernum.Call("SetupTextScale", intro, 1.15f);

                var spawn = new SoundStyle("InfernumMode/Assets/Sounds/Custom/ExoMechs/ThanatosTransition", SoundType.Sound)
                {
                    Volume = 1.5f,
                    PitchVariance = 0.05f,
                    MaxInstances = 1
                };

                Infernum.Call(
                    "SetupMainSound",
                    intro,
                    (Func<int, int, float, float, bool>)((t, _, __, ___) => true),
                    () => spawn
                );

                Infernum.Call("SetupLetterAdditionSound", intro, () => SoundID.NPCHit4);
                Infernum.Call("SetupLetterDisplayCompletionRatio", intro, (Func<int, float>)(count => count / 10f));

                // Register and optional completion effects
                Infernum.Call("RegisterIntroScreen", intro);
                Infernum.Call("SetupCompletionEffects", intro, () => { });

                // Add Avatar of Emptiness card
                int avatarType = noxus.Find<ModNPC>("AvatarOfEmptiness").Type;
                string avatarTextKey = "Mods.InfernalEclipseAPI.InfernumIntegration.AvatarOfEmptiness";
                LocalizedText avatarIntroText = Language.GetOrRegister(avatarTextKey);

                // Initialize the intro screen
                object avatarIntro = Infernum.Call(
                    "InitializeIntroScreen",
                    avatarIntroText,
                    900,                            // display time (700 delay + 200 text)
                    true,                           // center text?
                    (Func<bool>)(() => NPC.AnyNPCs(avatarType)),
                    (Func<float, float, Color>)((completionRatio, _) =>
                    {
                        // Base gradient from blood maroon red to dark purple blue
                        Color baseColor = Color.Lerp(Color.Maroon, Color.DarkSlateBlue, completionRatio);

                        // Flash to black effect
                        float flashPulse = MathF.Sin(completionRatio * MathF.PI * 4f + Main.GlobalTimeWrappedHourly * 8f) * 0.5f + 0.5f;
                        float flashIntensity = MathF.Pow(flashPulse, 4f) * 0.7f;

                        return Color.Lerp(baseColor, Color.Gray, flashIntensity);
                    })
                );

                // Scale, sounds, and behavior
                Infernum.Call("SetupTextScale", avatarIntro, 1.15f);

                var avatarSpawn = new SoundStyle("InfernumMode/Assets/Sounds/Custom/ExoMechs/ThanatosTransition", SoundType.Sound)
                {
                    Volume = 1.5f,
                    PitchVariance = 0.05f,
                    MaxInstances = 1
                };

                Infernum.Call(
                    "SetupMainSound",
                    avatarIntro,
                    (Func<int, int, float, float, bool>)((t, _, __, ___) => t == 0),
                    (() => avatarSpawn)
                );

                Infernum.Call("SetupLetterAdditionSound", avatarIntro, () => SoundID.NPCHit4);
                Infernum.Call("SetupLetterDisplayCompletionRatio", avatarIntro, (Func<int, float>)(animationTimer =>
                {
                    // Wait 750 frames before starting text display
                    if (animationTimer < 750) return 0f;

                    return (animationTimer - 750) / 10f;
                }));

                // Register and optional completion effects
                Infernum.Call("RegisterIntroScreen", avatarIntro);
                Infernum.Call("SetupCompletionEffects", avatarIntro, () => { });
            }
            if (ModLoader.TryGetMod("Clamity", out Mod clam))
            {
                MakeCard(clam.Find<ModNPC>("ClamitasBoss").Type, (horz, anim) => Color.Lerp(Color.RosyBrown, Color.Red, anim), "Clamitas", SoundID.Item100, new SoundStyle("CalamityMod/Sounds/Custom/Providence/ProvidenceSpawn"));
            }
            if (ModLoader.TryGetMod("HypnosMod", out Mod hypnos))
            {
                
                MakeCard(hypnos.Find<ModNPC>("HypnosBoss").Type, (horz, anim) => Color.Lerp(Color.DeepPink, Color.MistyRose, anim), "HypnosBoss", SoundID.NPCHit4, new SoundStyle("InfernumMode/Assets/Sounds/Custom/ExoMechs/ThanatosTransition"), 240);
            }
            if (ModLoader.TryGetMod("CalamityAddon", out Mod warMachine))
            {
                MakeCard(warMachine.Find<ModNPC>("WulfrumMothership").Type, (horz, anim) => Color.Lerp(Color.YellowGreen, Color.GreenYellow, anim), "WulfrumMothership", SoundID.NPCHit4, new("CalamityMod/Sounds/Custom/RoverDriveActivate"), 240);
            }
            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst) && !ModLoader.TryGetMod("CnI", out _))
            {
                int astrageldonType = catalyst.Find<ModNPC>("Astrageldon").Type;
                string textKey = "Mods.InfernalEclipseAPI.InfernumIntegration.Astrageldon";
                LocalizedText introText = Language.GetOrRegister(textKey);

                // Initialize the intro screen
                object intro = Infernum.Call(
                    "InitializeIntroScreen",
                    introText,
                    150,                            // display time
                    true,                           // center text?
                    () => NPC.AnyNPCs(astrageldonType),
                    (Func<float, float, Color>)((completionRatio, _) =>
                    {
                        // Base gradient between purple and pink
                        Color baseColor = Color.Lerp(Color.MediumPurple, Color.HotPink, completionRatio);

                        // Pulsing yellow emphasis near the end
                        float pulse = MathF.Sin(completionRatio * MathF.PI * 4f + Main.GlobalTimeWrappedHourly * 5f) * 0.5f + 0.5f;
                        float t = Terraria.Utils.GetLerpValue(0.8f, 1f, pulse, true);

                        return Color.Lerp(baseColor, Color.Yellow, t);
                    })
                );

                // Scale, sounds, and behavior
                Infernum.Call("SetupTextScale", intro, 1.15f);

                var spawn = new SoundStyle($"{catalyst.Name}/Assets/Sounds/AstrageldonSpawn", SoundType.Sound)
                {
                    Volume = 1.5f,
                    PitchVariance = 0.05f,
                    MaxInstances = 1
                };

                Infernum.Call(
                    "SetupMainSound",
                    intro,
                    (Func<int, int, float, float, bool>)((t, _, __, ___) => t == 0),
                    () => spawn
                );

                Infernum.Call("SetupLetterAdditionSound", intro, () => SoundID.MenuTick);
                Infernum.Call("SetupLetterDisplayCompletionRatio", intro, (Func<int, float>)(count => count / 10f));

                // Register and optional completion effects
                Infernum.Call("RegisterIntroScreen", intro);
                Infernum.Call("SetupCompletionEffects", intro, () => { });
            }
        }
        internal void MakeCard(int type, Func<float, float, Color> color, string title, SoundStyle tickSound, SoundStyle endSound, int time = 300, float size = 1f)
        {
            MakeCard(() => NPC.AnyNPCs(type), color, title, tickSound, endSound, time, size);
        }
        internal void MakeCard(Func<bool> condition, Func<float, float, Color> color, string title, SoundStyle tickSound, SoundStyle endSound, int time = 300, float size = 1f)
        {
            // Initialize the base instance for the intro card. Alternative effects may be added separately.
            Func<float, float, Color> textColorSelectionDelegate = color;
            object instance = Infernum.Call("InitializeIntroScreen", Mod.GetLocalization("InfernumIntegration." + title), time, true, condition, textColorSelectionDelegate);
            Infernum.Call("SetupLetterDisplayCompletionRatio", instance, new Func<int, float>(animationTimer => MathHelper.Clamp(animationTimer / (float)time * 1.36f, 0f, 1f)));

            // dnc but needed or else it errors
            Action onCompletionDelegate = () => { };
            Infernum.Call("SetupCompletionEffects", instance, onCompletionDelegate);

            // Letter addition sound.
            Func<SoundStyle> chooseLetterSoundDelegate = () => tickSound;
            Infernum.Call("SetupLetterAdditionSound", instance, chooseLetterSoundDelegate);

            // Main sound.
            Func<SoundStyle> chooseMainSoundDelegate = () => endSound;
            Func<int, int, float, float, bool> why = (_, _2, _3, _4) => true;
            Infernum.Call("SetupMainSound", instance, why, chooseMainSoundDelegate);

            // Text scale.
            Infernum.Call("SetupTextScale", instance, size);

            // Register the intro card.
            Infernum.Call("RegisterIntroScreen", instance);
        }

        public static void ColoredDamageTypesSupport()
        {
            if (ModLoader.TryGetMod("ColoredDamageTypes", out Mod coloredDamageTypes))
            {
                if (InfernalCrossmod.SOTS.Loaded)
                {
                    SOTSIntegrationsSystem.VoidColoredDamageTypes(coloredDamageTypes);
                }

                Color legendaryColor = new Color(255, 215, 0); // Gold
                Vector3 hslVector = Main.rgbToHsl(legendaryColor);
                hslVector.Y = MathHelper.Lerp(hslVector.Y, 1f, 0.6f);
                Color legendaryCritColor = Main.hslToRgb(hslVector);

                coloredDamageTypes.Call("AddDamageType", LegendaryMelee.Instance, legendaryColor, legendaryColor, legendaryCritColor);
                coloredDamageTypes.Call("AddDamageType", LegendaryRanged.Instance, legendaryColor, legendaryColor, legendaryCritColor);
                coloredDamageTypes.Call("AddDamageType", LegendaryMagic.Instance, legendaryColor, legendaryColor, legendaryCritColor);
                coloredDamageTypes.Call("AddDamageType", LegendarySummonMeleeSpeed.Instance, legendaryColor, legendaryColor, legendaryCritColor);

                Color mythicColor = Color.Cyan;
                Vector3 hslVector2 = Main.rgbToHsl(mythicColor);
                hslVector2.Y = MathHelper.Lerp(hslVector2.Y, 1f, 0.6f);
                Color mythicCritColor = Main.hslToRgb(hslVector2);

                coloredDamageTypes.Call("AddDamageType", MythicMelee.Instance, mythicColor, mythicColor, mythicCritColor);
                coloredDamageTypes.Call("AddDamageType", MythicMagic.Instance, mythicColor, mythicColor, mythicCritColor);
                coloredDamageTypes.Call("AddDamageType", MythicRanged.Instance, mythicColor, mythicColor, mythicCritColor);
                coloredDamageTypes.Call("AddDamageType", MythicSummon.Instance, mythicColor, mythicColor, mythicCritColor);
            }
        }

        public static void Project_tRUSupport()
        {
            if (ModLoader.TryGetMod("CalamityRuTranslate", out Mod tru))
            {
                Mod eclipse = ModLoader.GetMod("InfernalEclipseAPI");

                tru.Call("AddFeminineItems", eclipse, new string[]
                {
                    "NovaBomb",
                    "StellarSabre",
                    "SplitFirebrand"
                });

                tru.Call("AddNeuterItems", eclipse, new string[]
                {
                    "CelestialIllumination",
                    "RingofTix",
                    "TheChickenWing",
                    "ShatteredSubcommunity",
                    "BlixerCore"
                });

                tru.Call("AddPluralItems", eclipse, new string[]
                {
                    "The454CasullandTheJackal"
                });
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public static class AddThoriumMiniBoss
    {
        public static void AddMiniBosses()
        {
            void newMiniBoss(int npc)
            {
                EventSceneBase<MiniBossScene>.Instance.NPCs.Add(npc);
            }

            if (InfernalCrossmod.SOTS.Loaded)
            {
                int findSOTSNPC(string name)
                {
                    return InfernalCrossmod.SOTS.Mod.Find<ModNPC>(name).Type;
                }

                EventSceneBase<MiniBossScene>.Instance.NPCs.Add(InfernalCrossmod.SOTS.Mod.Find<ModNPC>("PutridPinky1").Type);
                newMiniBoss(findSOTSNPC("ChaosConstruct"));
                newMiniBoss(findSOTSNPC("ChaosSpirit"));
                newMiniBoss(findSOTSNPC("EarthenConstruct"));
                newMiniBoss(findSOTSNPC("EarthenSpirit"));
                newMiniBoss(findSOTSNPC("EvilConstruct"));
                newMiniBoss(findSOTSNPC("EvilSpirit"));
                newMiniBoss(findSOTSNPC("InfernoConstruct"));
                newMiniBoss(findSOTSNPC("InfernoSpirit"));
                newMiniBoss(findSOTSNPC("NatureConstruct"));
                newMiniBoss(findSOTSNPC("NatureSpirit"));
                newMiniBoss(findSOTSNPC("PermafrostConstruct"));
                newMiniBoss(findSOTSNPC("PermafrostSpirit"));
                newMiniBoss(findSOTSNPC("TidalConstruct"));
                newMiniBoss(findSOTSNPC("TidalSpirit"));
                newMiniBoss(findSOTSNPC("Bridgeburner"));
            }
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public static class SOTSIntegrationsSystem
    {
        public static void VoidColoredDamageTypes(Mod cDT)
        {
            cDT.Call("AddDamageType", ModContent.GetInstance<VoidGeneric>(), new Color(255, 255, 255), new Color(255, 255, 255), new Color(205, 205, 205));
            cDT.Call("AddDamageType", ModContent.GetInstance<VoidMelee>(), new Color(70, 0, 0), new Color(30, 0, 0), new Color(70, 0, 0));
            cDT.Call("AddDamageType", ModContent.GetInstance<VoidRanged>(), new Color(0, 70, 0), new Color(0, 30, 0), new Color(0, 70, 0));
            cDT.Call("AddDamageType", ModContent.GetInstance<VoidMagic>(), new Color(0, 0, 70), new Color(0, 0, 30), new Color(0, 0, 70));
        }
    }
}