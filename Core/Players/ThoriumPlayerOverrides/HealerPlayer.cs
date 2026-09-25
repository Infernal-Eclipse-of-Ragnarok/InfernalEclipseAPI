using InfernalEclipseAPI.Content.Buffs;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria.ModLoader.IO;

namespace InfernalEclipseAPI.Core.Players
{
    //Original code provided by Wardrobe Hummus
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class HealerPlayer : ModPlayer
    {
        private int scytheChargeCooldown;
        private bool hadDreamCatcher;

        private bool initialized = false;
        public HashSet<int> fifthScytheTypes = new();

        //public bool accessoryEquipped = false;

        //private int contractCooldownTimer = 0;
        //private bool restoreContractAfterCooldown = false;

        //private int executionersContract = -1;
        //private int sealedContract = -1;
        //private bool ContractInitialized = false;

        public int renewCooldown;
        public int starBirthCooldown;

        public bool buffBubbleBulwarkWandCooldown;

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (buffBubbleBulwarkWandCooldown)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<BubbleShock>(), 5400);
            }
            return base.ConsumableDodge(info);
        }


        public override void ResetEffects()
        {
            if (renewCooldown > 0)
                renewCooldown--;
            if (starBirthCooldown > 0)
                starBirthCooldown--;
            buffBubbleBulwarkWandCooldown = false;
        }

        // Dynamic callsite storage
        private static class DynamicSetters
        {
            public static CallSite<Func<CallSite, object, bool, object>> SetCanGiveScytheCharge;
        }

        public override void Initialize()
        {
            scytheChargeCooldown = 0;
            initialized = false;
        }

        public override void PostUpdate()
        {
            Player player = Player;

            if (!initialized)
            {
                initialized = true;
                LoadProjectileTypes();
            }

            if (scytheChargeCooldown > 0)
                scytheChargeCooldown--;

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile != null || !ModLoader.HasMod("WHummusMultiModBalancing"))
                    continue;

                if (fifthScytheTypes.Contains(projectile.type))
                {
                    object modProjectile = projectile.ModProjectile;

                    DynamicSetters.SetCanGiveScytheCharge ??= CallSite<Func<CallSite, object, bool, object>>.Create(
                        Binder.SetMember(
                            CSharpBinderFlags.None,
                            "CanGiveScytheCharge",
                            typeof(HealerPlayer),
                            new[]
                            {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                            }
                        )
                    );

                    DynamicSetters.SetCanGiveScytheCharge.Target(
                        DynamicSetters.SetCanGiveScytheCharge,
                        modProjectile,
                        scytheChargeCooldown <= 0
                    );
                }
            }

            bool hasDreamCatcher = player.HasBuff(ModContent.BuffType<ThoriumMod.Buffs.Healer.DreamCatcherBuff>());
            bool hasExhaustion = player.HasBuff(ModContent.BuffType<ThoriumMod.Buffs.RevivalExhaustion>());

            if (hadDreamCatcher && !hasDreamCatcher && hasExhaustion)
            {
                int setLife = (int)(player.statLifeMax2 * 0.2f);
                player.statLife = Math.Max(1, setLife);

                player.HealEffect(player.statLife);

                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) && thorium.TryFind("Mortality", out ModBuff mortality))
                {
                    player.AddBuff(mortality.Type, 600);
                }
            }

            hadDreamCatcher = hasDreamCatcher;
        }

        private void LoadProjectileTypes()
        {
            void TryAdd(Mod mod, string name)
            {
                if (mod != null && mod.TryFind(name, out ModProjectile proj))
                    fifthScytheTypes.Add(proj.Type);
            }

            //ModLoader.TryGetMod("RagnarokMod", out Mod ragnarokMod);
            ModLoader.TryGetMod("ThoriumMod", out Mod thoriumMod);

            // Ragnarok
            //TryAdd(ragnarokMod, "ScoriaDualscythePro");
            //TryAdd(ragnarokMod, "ProfanedScythePro");
            //TryAdd(ragnarokMod, "MarbleScythePro");

            // Thorium
            string[] thoriumProjs = {
                "AquaiteScythePro", "MoltenThresherPro", "BatScythePro", "BoneReaperPro", "BloodHarvestPro", "FallingTwilightPro",
                "HallowedScythePro", "TrueHallowedScythePro", "TitanScythePro", "MorningDewPro",
                "DreadTearerPro", //"TheBlackScythePro", 
                "LustrousBatonPro"
            };

            foreach (string name in thoriumProjs)
                TryAdd(thoriumMod, name);
        }

        public bool CanTriggerChargeEffect() => scytheChargeCooldown <= 0;

        public void TriggerScytheCharge(bool fromServer = false)
        {
            scytheChargeCooldown = 2;

            if (Main.netMode == NetmodeID.MultiplayerClient && !fromServer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)0);
                packet.Write((byte)Player.whoAmI);
                packet.Send();
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["scytheChargeCooldown"] = scytheChargeCooldown;
        }

        public override void LoadData(TagCompound tag)
        {
            scytheChargeCooldown = tag.GetInt("scytheChargeCooldown");
        }
    }
}
