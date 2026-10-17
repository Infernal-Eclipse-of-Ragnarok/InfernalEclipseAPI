using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Armor.Silva;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernumMode.Content.Cooldowns;
using InfernumMode.Content.Items.Accessories;
using InfernumMode.Content.Items.Weapons.Melee;
using InfernumMode.Core.GlobalInstances.Players;
using SOTS.Buffs.Debuffs;
using SOTS.Void;
using ThoriumMod;
using ThoriumMod.Buffs;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Content.Buffs
{
    public class BrimstoneDesperation : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/DamageOverTime/VulnerabilityHex";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.AddBuff(BuffID.ChaosState, 2);

            if (InfernalCrossmod.Consolaria.Loaded)
            {
                player.ClearBuff(InfernalCrossmod.Consolaria.Mod.Find<ModBuff>("Drunk").Type);
            }

            if (!player.name.ToLower().Contains("jareto15"))
            {
                player.GetModPlayer<InfernumPlayer>().SetValue<bool>("EggShieldActive", false);
                player.GetModPlayer<InfernumPlayer>().SetValue<int>("CurrentEggShieldHits", 0);
            }
            if (!player.name.ToLower().Contains("myra"))
                player.GetModPlayer<InfernumPlayer>().SetValue<bool>("BrimstoneCrescentForcefieldIsActive", false);

            player.GetModPlayer<InfernumPlayer>().SetValue<bool>("SealocketMechanicalEffectsApply", false);

            player.AddCooldown(PermafrostConcoction.ID, CalamityUtils.SecondsToFrames(180));
            player.AddCooldown(SilvaRevive.ID, SilvaArmor.ReviveCooldown);
            player.AddCooldown(NebulousCore.ID, CalamityUtils.SecondsToFrames(90));

            player.AddCooldown(GlobalDodge.ID, CalamityUtils.SecondsToFrames(180));
            player.AddCooldown(CalamityMod.Cooldowns.ChaosState.ID, CalamityUtils.SecondsToFrames(180));

            player.AddCooldown(EggShieldRecharge.ID, CallUponTheEggs.EggShieldCooldown);
            player.AddCooldown(SealocketForcefieldRecharge.ID, CalamityUtils.SecondsToFrames(CherishedSealocket.ForcefieldRechargeSeconds));
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public static class ThoriumEffectHandler
    {
        public static void DisableThoriumEffects(Player player, bool disableChaosState = true)
        {
            ThoriumPlayer mp = player.GetThoriumPlayer();

            player.AddBuff(ModContent.BuffType<RevivalExhaustion>(), 2);

            mp.debuffRevivalExhaustion = true;

            mp.accAbyssalShell = false;
            mp.accFlawlessChrysalis = false;

            player.ClearBuff(ModContent.BuffType<PhylacteryBuff>());

            if (disableChaosState)
            {
                GrantThoriumChaosState(player);
            }
        }

        public static void GrantThoriumChaosState(Player player)
        {
            player.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.ChaosState>(), 2);
        }
    }

    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public static class SOTSEffectHandler
    {
        public static void DisableSOTSEffects(Player player)
        {
            player.AddBuff(ModContent.BuffType<ChaosState2>(), 2);
        }
    }
}
