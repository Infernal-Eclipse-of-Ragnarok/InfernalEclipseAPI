using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.Localization;

namespace InfernalEclipseAPI.Content.Cooldowns
{
    public class ParadisePower : CooldownHandler
    {
        public static new string ID => "ParadisePower";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.InfernalEclipseAPI.UI.Cooldowns.{ID}");
        public override string Texture => "InfernalEclipseAPI/Content/Cooldowns/ParadisePower";
        public override Color OutlineColor => Color.White;
        public override Color CooldownStartColor => Color.Sienna;
        public override Color CooldownEndColor => Color.LightSlateGray;
        public override SoundStyle? EndSound => (Main.zenithWorld ? null : new("CalamityMod/Sounds/Item/ArsenalOffCooldown"));
    }
}
