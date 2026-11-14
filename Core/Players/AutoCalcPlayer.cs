using System.Collections.Generic;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityMod.Projectiles.Ranged;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using NoxusBoss.Content.NPCs.Bosses.Avatar.FirstPhaseForm;
using NoxusBoss.Content.NPCs.Bosses.Avatar.SecondPhaseForm;
using NoxusBoss.Content.NPCs.Bosses.Draedon;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity;

namespace InfernalEclipseAPI.Core.Players
{
    public class AutoCalcPlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod("CalamityAmmo");
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            Mod calAmmo = ModLoader.GetMod("CalamityAmmo");

            List<int> excludedProjs =
            [
                ModContent.ProjectileType<ContagionBow>(),
                ModContent.ProjectileType<ContagionArrow>(),
                ModContent.ProjectileType<ContagionBall>()
            ];

            if (InfernalUtilities.HasAccessoryEquipped(Player, calAmmo.Find<ModItem>("AutoCalculationCoil").Type) && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (excludedProjs.Contains(proj.type))
                {
                    if (target.boss)
                        modifiers.FinalDamage /= 5;
                    else //just in case
                    {
                        if (InfernalCrossmod.NoxusBoss.Loaded)
                            WrathBossNerfs.NerfDuringWrathBosses(target, ref modifiers);

                        if (ModLoader.HasMod("CalamityHunt"))
                            HotGBossNerfs.NerfDuringHotGBosses(target, ref modifiers);
                    }
                }
            }
        }
    }

    [JITWhenModsEnabled("NoxusBoss")]
    [ExtendsFromMod("NoxusBoss")]
    public static class WrathBossNerfs
    {
        public static void NerfDuringWrathBosses(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == ModContent.NPCType<NamelessDeityBoss>() || target.type == ModContent.NPCType<MarsBody>() || target.type == ModContent.NPCType<AvatarRift>() || target.type == ModContent.NPCType<AvatarOfEmptiness>())
            {
                modifiers.FinalDamage /= 5;
            }
        }
    }

    [JITWhenModsEnabled("CalamityHunt")]
    [ExtendsFromMod("CalamityHunt")]
    public static class HotGBossNerfs
    {
        public static void NerfDuringHotGBosses(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == ModContent.NPCType<Goozma>() || target.type == ModContent.NPCType<EbonianBehemuck>() || target.type == ModContent.NPCType<CrimulanGlopstrosity>() || target.type == ModContent.NPCType<DivineGargooptuar>() || target.type == ModContent.NPCType<StellarGeliath>() || target.type == ModContent.NPCType<Goozmite>())
            {
                modifiers.FinalDamage /= 5;
            }
        }
    }
}
