using CalamityMod;
using CalamityMod.CalPlayer;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Common.ModPlayers;
using SOTS.Items.Wings;
using SOTS.NPCs.Boss;
using SOTS.Void;
using Terraria.GameContent.Events;
using Terraria.Localization;

namespace InfernalEclipseAPI.Core.Players.SOTSPlayerOverrides
{
    [JITWhenModsEnabled(InfernalCrossmod.SOTS.Name)]
    [ExtendsFromMod(InfernalCrossmod.SOTS.Name)]
    public class SOTSPlayerAdjustments : ModPlayer
    {
        public string bossMessage = "";
        public bool royalJelly;
        public bool glowSpores;
        public bool sandwich;
        public bool glowJelly;
        public bool alchemistsCharm;
        public bool bladeWings;

        public override void ResetEffects()
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(Player);
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(Player);

            if (sotsPlayer.VigorDashes > 25)
            {
                sotsPlayer.VigorDashes = 25;
            }

            float ieorPermVoidRegenSpeedIncrese = 0f;

            if (Player.GetModPlayer<InfernalPlayer>().singularityCore)
            {
                ieorPermVoidRegenSpeedIncrese += 0.05f;
            }

            ieorPermVoidRegenSpeedIncrese += Player.GetModPlayer<InfernalPlayer>().ruinousPlasmaInjection * 0.01f;

            voidPlayer.voidRegenSpeed += ieorPermVoidRegenSpeedIncrese;

            if (Player.GetModPlayer<MachinaBoosterPlayer>().creativeFlight)
            {
                voidPlayer.flatVoidRegen -= bladeWings ? 16 : 28;
            }

            royalJelly = sandwich = glowJelly = alchemistsCharm = bladeWings = false;
        }

        public override void UpdateEquips()
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(Player);
            InfernalPlayer mp = Player.GetModPlayer<InfernalPlayer>();

            if (Player.SOTSPlayer().InverseDiamondRing)
            {
                mp.InverseDiamondRing = Player.SOTSPlayer().InverseDiamondRing;
                Player.SOTSPlayer().InverseDiamondRing = false;
            }
            if (Player.SOTSPlayer().InverseAmberRing)
            {
                mp.InverseAmberRing = Player.SOTSPlayer().InverseAmberRing;
                Player.SOTSPlayer().InverseAmberRing = false;
            }

            Player player = this.Player;

            // Best class total multiplier.
            // 1.00f = no bonus, 1.35f = +35%, etc.
            StatModifier bestClassDamage = player.GetBestClassDamage();
            float totalBestDamageMult = bestClassDamage.ApplyTo(1f);

            float damageBonus = bestClassDamage.Additive * bestClassDamage.Multiplicative - 1f;
            if (damageBonus < 0f)
                damageBonus = 0f;

            // Convert 1/3 of the bonus into defense.
            // Example: +30% damage -> 10 defense
            mp.defenseGain = (int)(damageBonus * 100f / 3f);

            if (mp.InverseDiamondRing)
            {
                player.statDefense += mp.defenseGain;

                player.GetDamage(DamageClass.Generic) -= mp.defenseGain * 0.01f;
            }
            mp.InverseDiamondRing = false;


            if (alchemistsCharm)
            {
                sotsPlayer.additionalHeal += 100;
                sotsPlayer.additionalPotionMana += 100;
            }
            else if (glowJelly)
            {
                sotsPlayer.additionalHeal += 40;
                sotsPlayer.additionalPotionMana += 40;
            }
            else if (glowSpores)
            {
                sotsPlayer.additionalPotionMana += 40;
            }

            if (sandwich)
            {
                sotsPlayer.additionalHeal += 40;
            }
            else if (royalJelly && !glowJelly && !alchemistsCharm)
            {
                sotsPlayer.additionalHeal += 40;
            }
        }

        public override void PostUpdateEquips()
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(Player);

            /*
            if (InfernalPlayer.PlayerHasPurity(Player))
            {
                sotsPlayer.InverseDiamondRing = false;
            }

            if (sotsPlayer.InverseDiamondRing)
            {
                Player.ClearBuff(ModContent.BuffType<GrapeBeerBuff>());
                Player.Calamity().grapeBeer = false;
            }

            if (Player.Calamity().grapeBeer)
            {
                sotsPlayer.CritBonusDamage = (int)(sotsPlayer.CritBonusDamage * 0.25f);
                sotsPlayer.CritBonusMultiplier *= 0.75f;
                sotsPlayer.CritCurseFire = false;
                sotsPlayer.CritFire = false;
                sotsPlayer.CritFrost = false;
            }
            */
        }

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<InfernalPlayer>().voidMagePrevention > 0)
                Player.SOTSPlayer().VMincubator = false;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            VoidMagePreventedOnHit();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            VoidMagePreventedOnHit();
        }

        private void VoidMagePreventedOnHit()
        {
            InfernalPlayer mp = Player.GetModPlayer<InfernalPlayer>();
            if (mp.voidMagePrevention > 0 && mp.incubatorTextTime == 0)
            {
                Main.NewText(Language.GetTextValue("Mods.InfernalEclipseAPI.TimeFreezePrevention." + bossMessage));
                mp.incubatorTextTime = 60 * 60;
            }
        }
        public override void OnRespawn()
        {
            if (InfernalConfig.Instance.MaxVoidOnRespawn)
            {
                VoidPlayer voidPlayer = Player.GetModPlayer<VoidPlayer>();
                voidPlayer.voidMeter = voidPlayer.voidMeterMax2;
            }
        }
    }
}
