using CalamityMod.Events;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems;
using InfernumMode.Content.BossIntroScreens;
using InfernumMode.Content.BossIntroScreens.InfernumScreens;
using Microsoft.Xna.Framework;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;

namespace InfernalEclipseAPI.Common.InfernumScreens
{
    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public class BossRushIntroScreen7 : BaseIntroScreen
    {
        private int tier7Id;
        public override TextColorData TextColor => new(completionRatio =>
        {
            float colorFadeInterpolant = Sin(AnimationCompletion * Pi * 4f + completionRatio * Pi * 12f) * 0.5f + 0.5f;
            return Color.Lerp(Color.Orange, Color.OrangeRed, colorFadeInterpolant);
        });

        public override bool TextShouldBeCentered => true;
        public override bool ShouldCoverScreen => false;
        public override bool CaresAboutBossEffectCondition => false;
        public override int AnimationTime => 120;
        public override bool ShouldBeActive()
        {
            Player player = Main.LocalPlayer;
            if (!BossRushEvent.BossRushActive)
            {
                player.GetModPlayer<InfernalPlayer>().tier7IntroPlayed = false;
                return false;
            }

            tier7Id = ModContent.NPCType<NamelessDeityBoss>();
            List<(int, int, Action<int>, int, bool, float, int[], int[])> brEntries = (List<(int, int, Action<int>, int, bool, float, int[], int[])>)InfernalCrossmod.Calamity.Mod.Call("GetBossRushEntries");
            for (int i = 0; i < brEntries.Count; i++)
            {
                if (brEntries[i].Item1 == tier7Id)
                {
                    tier7Id = i;
                    break;
                }
            }

            return BossRushEvent.BossRushActive && !player.GetModPlayer<InfernalPlayer>().tier7IntroPlayed && InfernumMode.InfernumMode.CanUseCustomAIs && BossRushEvent.BossRushStage >= tier7Id;
        }

        public override LocalizedText TextToDisplay => Language.GetText("Mods.InfernalEclipseAPI.InfernumIntegration.BossRushIntroText7");
        public override SoundStyle? SoundToPlayWithTextCreation => null;
        public override SoundStyle? SoundToPlayWithLetterAddition => SoundID.Item100;
        public override bool CanPlaySound => LetterDisplayCompletionRatio(AnimationTimer) >= 1f;
        public override float LetterDisplayCompletionRatio(int animationTimer)
        {
            float completionRatio = Utils.GetLerpValue(TextDelayInterpolant, 0.92f, animationTimer / (float)AnimationTime, true);

            int startOfLargeTextIndex = TextToDisplay.Value.IndexOf('\n');
            int currentIndex = (int)(completionRatio * TextToDisplay.Value.Length);
            if (currentIndex >= startOfLargeTextIndex)
                completionRatio = 1f;

            return completionRatio;
        }
        public override void DoCompletionEffects()
        {
            Main.LocalPlayer.GetModPlayer<InfernalPlayer>().tier7IntroPlayed = true;
            AnimationTimer = 0;
        }
    }
}
