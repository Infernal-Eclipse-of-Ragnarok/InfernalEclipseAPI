using System.Linq;
using InfernalEclipseAPI.Content.Items.Weapons.BossRush.Swordofthe14thGlitch;
using InfernalEclipseAPI.Content.Items.Weapons.Melee.SwordoftheCorrupted;
using InfernalEclipseAPI.Content.Items.Weapons.Melee.SwordoftheFirst;
using InfernalEclipseAPI.Core.DamageClasses;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.Items.Weapons.Rogue;
using SOTS.FakePlayer;
using SOTS.Void;
using SOTSBardHealer.DamageTypes;
using Terraria.GameContent.Prefixes;
using Terraria.Utilities;
using ThoriumMod;

namespace InfernalEclipseAPI.Core.Utils
{
    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public static partial class SOTSItemUtils
    {
        public static void InitializeFakePlayerBlacklist()
        {
            var list = (FakePlayerHelper.FakePlayerItemBlacklist ?? Array.Empty<int>()).ToList();

            list.Add(ModContent.ItemType<StormMaidensRetribution>());

            list.Add(ModContent.ItemType<Swordofthe1stGlitch>());
            list.Add(ModContent.ItemType<Swordofthe13thGlitch>());
            list.Add(ModContent.ItemType<Swordofthe14thGlitch>());

            if (InfernalCrossmod.Thorium.Loaded)
            {
                Mod thorium = InfernalCrossmod.Thorium.Mod;

                list.Add(thorium.Find<ModItem>("TerrariansLastKnife").Type);
                list.Add(thorium.Find<ModItem>("BlackMIDI").Type);
                list.Add(thorium.Find<ModItem>("NorthernLight").Type);
                list.Add(thorium.Find<ModItem>("QuasarsFlare").Type);
            }

            if (InfernalCrossmod.YouBoss.Loaded)
            {
                list.Add(InfernalCrossmod.YouBoss.Mod.Find<ModItem>("FirstFractal").Type);
            }

            FakePlayerHelper.FakePlayerItemBlacklist = list.ToArray();
        }

        internal static int GetReworkedReforge(Item item, UnifiedRandom rand, int currentPrefix)
        {
            Mod SOTS = ModLoader.GetMod("SOTS");
            int GetSOTSPrefix(string name)
            {
                bool found = SOTS.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }

            int prefix = -1;

            //ACCESSORIES
            //TODO: FULLY REPLACE CAL ACCESSORY REFORGE TIERS WITH SOTS ONES IF PEOPLE START TO COMPLAIN

            //VOID MELEE & VOID WHIPS
            if (item.CountsAsClass<VoidMelee>())
            {
                int[][] meleeReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Light, PrefixID.Heavy, PrefixID.Light, PrefixID.Forceful, PrefixID.Strong },
                    /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky, GetSOTSPrefix("Chthonic") },
                    /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp, GetSOTSPrefix("Potent") },
                    /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                    /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly, GetSOTSPrefix("Precarious") },
                    /* 5 */ new int[] { PrefixID.Legendary, GetSOTSPrefix("Omnipotent") }
                };
                prefix = IteratePrefix(rand, meleeReforgeTiers, currentPrefix);
            }

            //VOID RANGED
            else if (item.CountsAsClass<VoidRanged>())
            {
                int[][] rangedReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Powerful, PrefixID.Forceful, PrefixID.Strong },
                    /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Intimidating,  GetSOTSPrefix("Chthonic") },
                    /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Hasty, PrefixID.Staunch, PrefixID.Unpleasant, GetSOTSPrefix("Potent") },
                    /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Sighted },
                    /* 4 */ new int[] { PrefixID.Godly, PrefixID.Rapid, /* ranged Deadly */ PrefixID.Deadly, /* universal Deadly */ PrefixID.Deadly2, GetSOTSPrefix("Precarious") },
                    /* 5 */ new int[] { PrefixID.Unreal, GetSOTSPrefix("Omnipotent") }
                };
                prefix = IteratePrefix(rand, rangedReforgeTiers, currentPrefix);
            }

            //VOID MAGE
            else if (item.CountsAsClass<VoidMagic>())
            {
                int[][] magicReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Furious, PrefixID.Forceful, PrefixID.Strong },
                    /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic, GetSOTSPrefix("Chthonic") },
                    /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Adept, PrefixID.Celestial, PrefixID.Unpleasant, GetSOTSPrefix("Potent") },
                    /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic },
                    /* 4 */ new int[] { PrefixID.Godly, PrefixID.Masterful, PrefixID.Deadly2, GetSOTSPrefix("Precarious") },
                    /* 5 */ new int[] { PrefixID.Mythical, GetSOTSPrefix("Omnipotent") }
                };
                prefix = IteratePrefix(rand, magicReforgeTiers, currentPrefix);
            }

            //VOID SUMMONER
            else if (item.CountsAsClass<VoidSummon>())
            {
                int[][] summonReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { PrefixID.Nimble, PrefixID.Furious },
                    /* 1 */ new int[] { PrefixID.Forceful, PrefixID.Strong, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic, GetSOTSPrefix("Chthonic") },
                    /* 2 */ new int[] { PrefixID.Hurtful, PrefixID.Adept, PrefixID.Celestial, GetSOTSPrefix("Potent") },
                    /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic, PrefixID.Deadly2 },
                    /* 4 */ new int[] { PrefixID.Masterful, PrefixID.Godly, GetSOTSPrefix("Precarious") },
                    /* 5 */ new int[] { PrefixID.Mythical, PrefixID.Ruthless, GetSOTSPrefix("Omnipotent") } // you may want mythical early game for the knockback.
                };
                prefix = IteratePrefix(rand, summonReforgeTiers, currentPrefix);
            }
            return prefix;
        }

        private static int GetPrefixTier(int[][] tiers, int currentPrefix)
        {
            for (int checkingTier = 0; checkingTier < tiers.Length; ++checkingTier)
            {
                int[] tierList = tiers[checkingTier];
                for (int i = 0; i < tierList.Length; ++i)
                    if (tierList[i] == currentPrefix)
                        return checkingTier;
            }

            // If an invalid or modded prefix is detected, return -1.
            // This will give a random tier 0 prefix (the "next tier"), starting fresh with a low-tier vanilla or Calamity prefix.
            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix)
        {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            // If max tier: give max tier reforges forever
            // Otherwise: go up by 1 tier with every reforge, guaranteed
            int newTier = currentTier == reforgeTiers.Length - 1 ? currentTier : currentTier + 1;
            return rand.Next(reforgeTiers[newTier]);
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public static partial class ThoriumItemUtils
    {
        internal static int GetReworkedReforge(Item item, UnifiedRandom rand, int currentPrefix)
        {
            Mod ThoriumMod = ModLoader.GetMod("ThoriumMod");
            int GetThorPrefix(string name)
            {
                bool found = ThoriumMod.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }

            int prefix = -1;

            //HEALER
            if (item.CountsAsClass(ThoriumDamageBase<HealerDamage>.Instance) || item.CountsAsClass(ThoriumDamageBase<HealerToolDamageHybrid>.Instance))
            {
                //Heavy Scythes from BardHealer mods
                if (item.ModItem.MeleePrefix() && (item.ModItem.Mod.Name == "CalamityBardHealer" || item.ModItem.Mod.Name == "SOTSBardHealer" || item.ModItem.Mod.Name == "SpookyBardHealer"))
                {
                   int[][] meleeReforgeTiers = new int[][]
                   {
                        /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Light, PrefixID.Heavy, PrefixID.Light, PrefixID.Forceful, PrefixID.Strong },
                        /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky },
                        /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp },
                        /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                        /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly },
                        /* 5 */ new int[] { PrefixID.Legendary }
                   };
                    prefix = IteratePrefix(rand, meleeReforgeTiers, currentPrefix);
                }
                else //everything else
                {
                    int[][] magicReforgeTiers = new int[][]
                    {
                        /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Furious, PrefixID.Forceful, PrefixID.Strong },
                        /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic },
                        /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Adept, PrefixID.Celestial, PrefixID.Unpleasant },
                        /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic },
                        /* 4 */ new int[] { PrefixID.Godly, PrefixID.Masterful, PrefixID.Deadly2 },
                        /* 5 */ new int[] { PrefixID.Mythical }
                    };
                    prefix = IteratePrefix(rand, magicReforgeTiers, currentPrefix);
                }
            }

            //BARD
            else if (item.CountsAsClass<BardDamage>())
            {
                int[][] bardReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { GetThorPrefix("Muted"), GetThorPrefix("OffKey"), GetThorPrefix("Rambling") },
                    /* 1 */ new int[] { GetThorPrefix("Buzzing"), GetThorPrefix("Refined"), GetThorPrefix("Loud") },
                    /* 2 */ new int[] { GetThorPrefix("Supersonic"), GetThorPrefix("Vibrant"), GetThorPrefix("Euphonic"), GetThorPrefix("Inspiring") },
                    /* 3 */ new int[] { GetThorPrefix("Melodic") },
                    /* 4 */ new int[] { GetThorPrefix("Fabled") }
                };
                prefix = IteratePrefix(rand, bardReforgeTiers, currentPrefix);
            }

            return prefix;
        }

        private static int GetPrefixTier(int[][] tiers, int currentPrefix)
        {
            for (int checkingTier = 0; checkingTier < tiers.Length; ++checkingTier)
            {
                int[] tierList = tiers[checkingTier];
                for (int i = 0; i < tierList.Length; ++i)
                    if (tierList[i] == currentPrefix)
                        return checkingTier;
            }

            // If an invalid or modded prefix is detected, return -1.
            // This will give a random tier 0 prefix (the "next tier"), starting fresh with a low-tier vanilla or Calamity prefix.
            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix)
        {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            // If max tier: give max tier reforges forever
            // Otherwise: go up by 1 tier with every reforge, guaranteed
            int newTier = currentTier == reforgeTiers.Length - 1 ? currentTier : currentTier + 1;
            return rand.Next(reforgeTiers[newTier]);
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.SOTSBardHealer.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTSBardHealer.Name)]
    public static partial class SOTSBardHealerItemUtils
    {
        internal static int GetReworkedReforge(Item item, UnifiedRandom rand, int currentPrefix)
        {
            Mod CalamityMod = ModLoader.GetMod("CalamityMod");
            int GetCalPrefix(string name)
            {
                bool found = CalamityMod.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }
            Mod ThoriumMod = ModLoader.GetMod("ThoriumMod");
            int GetThorPrefix(string name)
            {
                bool found = ThoriumMod.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }
            Mod SOTSBH = ModLoader.GetMod("SOTSBardHealer");
            int GetSOTSBHPrefix(string name)
            {
                bool found = SOTSBH.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }

            int prefix = -1;

            //VOID ROGUE
            if (item.CountsAsClass<VoidThrowing>() || item.CountsAsClass<VoidRogue>())
            {
                int[][] rogueReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Forceful, PrefixID.Strong, GetCalPrefix("Radical"), GetCalPrefix("Pointy") },
                    /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, GetCalPrefix("Sharp"), GetCalPrefix("Glorious"), GetSOTSBHPrefix("Chthonic") },
                    /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Unpleasant, GetCalPrefix("Feathered"), GetCalPrefix("Sleek"), GetCalPrefix("Hefty"), GetSOTSBHPrefix("Potent") },
                    /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, GetCalPrefix("Mighty"), GetCalPrefix("Serrated") },
                    /* 4 */ new int[] { PrefixID.Godly, PrefixID.Deadly2, GetCalPrefix("Vicious"), GetCalPrefix("Lethal"), GetSOTSBHPrefix("Precarious") },
                    /* 5 */ new int[] { GetCalPrefix("Flawless"), PrefixID.Unreal, GetSOTSBHPrefix("Omnipotent") }
                };
                prefix = IteratePrefix(rand, rogueReforgeTiers, currentPrefix);

            }

            //VOID HEALER
            else if (item.CountsAsClass<VoidRadiant>())
            {
                //Heavy Scythes from BardHealer mods
                if (item.ModItem.MeleePrefix() && (item.ModItem.Mod.Name == "CalamityBardHealer" || item.ModItem.Mod.Name == "SOTSBardHealer" || item.ModItem.Mod.Name == "SpookyBardHealer"))
                {
                    int[][] meleeReforgeTiers = new int[][]
                    {
                        /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Light, PrefixID.Heavy, PrefixID.Light, PrefixID.Forceful, PrefixID.Strong },
                        /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky, GetSOTSBHPrefix("Chthonic") },
                        /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp, GetSOTSBHPrefix("Potent") },
                        /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                        /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly, GetSOTSBHPrefix("Precarious") },
                        /* 5 */ new int[] { PrefixID.Legendary, GetSOTSBHPrefix("Omnipotent") }
                    };
                    prefix = IteratePrefix(rand, meleeReforgeTiers, currentPrefix);
                }
                else //everything else
                {
                    int[][] magicReforgeTiers = new int[][]
                    {
                        /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Furious, PrefixID.Forceful, PrefixID.Strong },
                        /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic, GetSOTSBHPrefix("Chthonic") },
                        /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Adept, PrefixID.Celestial, PrefixID.Unpleasant, GetSOTSBHPrefix("Potent") },
                        /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic },
                        /* 4 */ new int[] { PrefixID.Godly, PrefixID.Masterful, PrefixID.Deadly2, GetSOTSBHPrefix("Precarious") },
                        /* 5 */ new int[] { PrefixID.Mythical, GetSOTSBHPrefix("Omnipotent") }
                    };
                    prefix = IteratePrefix(rand, magicReforgeTiers, currentPrefix);
                }
            }

            //VOID BARD
            else if (item.CountsAsClass<VoidSymphonic>())
            {
                int[][] bardReforgeTiers = new int[][]
                {
                    /* 0 */ new int[] { GetThorPrefix("Muted"), GetThorPrefix("OffKey"), GetThorPrefix("Rambling") },
                    /* 1 */ new int[] { GetThorPrefix("Buzzing"), GetThorPrefix("Refined"), GetThorPrefix("Loud"), GetSOTSBHPrefix("Chthonic") },
                    /* 2 */ new int[] { GetThorPrefix("Supersonic"), GetThorPrefix("Vibrant"), GetThorPrefix("Euphonic"), GetThorPrefix("Inspiring"), GetSOTSBHPrefix("Potent") },
                    /* 3 */ new int[] { GetThorPrefix("Melodic"), GetSOTSBHPrefix("Precarious") },
                    /* 4 */ new int[] { GetThorPrefix("Fabled"), GetSOTSBHPrefix("Omnipotent") }
                };
                prefix = IteratePrefix(rand, bardReforgeTiers, currentPrefix);
            }


            return prefix;
        }

        private static int GetPrefixTier(int[][] tiers, int currentPrefix)
        {
            for (int checkingTier = 0; checkingTier < tiers.Length; ++checkingTier)
            {
                int[] tierList = tiers[checkingTier];
                for (int i = 0; i < tierList.Length; ++i)
                    if (tierList[i] == currentPrefix)
                        return checkingTier;
            }

            // If an invalid or modded prefix is detected, return -1.
            // This will give a random tier 0 prefix (the "next tier"), starting fresh with a low-tier vanilla or Calamity prefix.
            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix)
        {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            // If max tier: give max tier reforges forever
            // Otherwise: go up by 1 tier with every reforge, guaranteed
            int newTier = currentTier == reforgeTiers.Length - 1 ? currentTier : currentTier + 1;
            return rand.Next(reforgeTiers[newTier]);
        }
    }
}
