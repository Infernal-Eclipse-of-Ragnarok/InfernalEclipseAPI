using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee.MaceFlails;
using CalamityMod.Projectiles.Melee.Yoyos;
using InfernalEclipseAPI.Content.Buffs.Tag;
using InfernalEclipseAPI.Core.Systems;
using ThoriumMod;

namespace InfernalEclipseAPI.Common.GlobalProjectiles
{
    //Wardrobe Hummus
    public class ProjectileDebuffs : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ProjectileID.HoundiusShootius && !InfernalCrossmod.Hummus.Loaded)
            {
                target.AddBuff(BuffID.Electrified, 120);
            }

            if (projectile.type == ProjectileID.JungleYoyo) // Amazon
            {
                target.AddBuff(BuffID.Poisoned, 60);
            }

            if (projectile.type == ModContent.ProjectileType<AirSpinnerYoyo>())
            {
                target.AddBuff(ModContent.BuffType<WindChilled>(), 60);
            }

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) && !InfernalCrossmod.Hummus.Loaded)
            {
                if (projectile.type == thorium.Find<ModProjectile>("ThunderTalonPro").Type)
                {
                    target.AddBuff(BuffID.Electrified, 180);
                }

                if (projectile.type == (thorium.Find<ModProjectile>("VoltHatchetPro")?.Type ?? -1))
                {
                    target.AddBuff(BuffID.Electrified, 60);
                }

                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    if (projectile.type == thorium.Find<ModProjectile>("DrenchedPro").Type)
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 60);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("AquaPelterPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 60);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("GeyserPro2")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("AquaiteKnifePro")?.Type ?? -1) || projectile.type == (thorium.Find<ModProjectile>("AquaiteKnifePro2")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 60);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("AquamarineWineGlassPro2")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("AquaiteScythePro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("IllustriousPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 120);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("IridescentPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 120);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("PearlPikePro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 120);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("ScubaCurvaPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 120);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("ScubaCurvaPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 120);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("BlobhornCoralStaffPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("SeaFoamScepterPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("SerpentsCryPro")?.Type ?? -1) || projectile.type == (thorium.Find<ModProjectile>("SerpentsCryPro2")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("RiptideDebuff")?.Type ?? -1, 120);
                    }

                    if (projectile.type == ModContent.ProjectileType<UrchinMaceProj>() || projectile.type == (calamity.Find<ModProjectile>("RedtideWhirlpool")?.Type ?? -1))
                    {
                        target.AddBuff(BuffID.Poisoned, 120);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("SandweaversTiaraPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("ArmorCrunch")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("DemonBloodSpearImage")?.Type ?? -1) ||
                        projectile.type == (thorium.Find<ModProjectile>("DemonBloodSpearPro")?.Type ?? -1) ||
                        projectile.type == (thorium.Find<ModProjectile>("DemonBloodSwordPro")?.Type ?? -1) ||
                        projectile.type == (thorium.Find<ModProjectile>("DemonBloodStaffPro")?.Type ?? -1))
                    {
                        target.AddBuff(calamity.Find<ModBuff>("BurningBlood")?.Type ?? -1, 180);
                    }

                    if (projectile.type == (thorium.Find<ModProjectile>("DemonBloodStaffPro2")?.Type ?? -1))
                    {
                        target.AddBuff(BuffID.Ichor, 60);
                    }

                    if (ModLoader.TryGetMod("ThoriumRework", out Mod helheim))
                    {
                        if (projectile.type == (helheim.Find<ModProjectile>("DemonBloodSword")?.Type ?? -1))
                        {
                            target.AddBuff(calamity.Find<ModBuff>("BurningBlood")?.Type ?? -1, 180);
                        }
                    }
                }
            }

            if (ModLoader.TryGetMod("CalamityBardHealer", out Mod bardhealer) && ModLoader.TryGetMod("CalamityMod", out Mod calamity2) && !InfernalCrossmod.Hummus.Loaded)
            {
                if (projectile.type == (bardhealer.Find<ModProjectile>("ExoSound")?.Type ?? -1))
                {
                    Player obj = Main.player[projectile.owner];

                    // Nerfed healing
                    int healAmount = damageDone / 200;
                    obj.statLife += healAmount;
                    obj.HealEffect(healAmount, true);

                    // Thorium inspiration
                    if (Utils.NextBool(Main.rand, 3))
                    {
                        HealInspiration.Init(obj);
                    }

                    // Apply Calamity debuff
                    int buffType = calamity2.Find<ModBuff>("MiracleBlight")?.Type ?? 0;
                    if (buffType > 0)
                    {
                        target.AddBuff(buffType, 240, false);
                    }
                }

                if (projectile.type == (bardhealer.Find<ModProjectile>("InfestedCastanet")?.Type ?? -1) && ModLoader.TryGetMod("ThoriumMod", out Mod thorium3))
                {
                    // Apply Calamity debuff
                    int buffType = thorium3.Find<ModBuff>("FungalGrowth")?.Type ?? 0;
                    if (buffType > 0)
                    {
                        target.AddBuff(buffType, 180, false);
                    }
                }
            }

            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst) && !InfernalCrossmod.Hummus.Loaded)
            {
                if (projectile.type == (catalyst.Find<ModProjectile>("CoralCrusherProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<CoralCrusherTag>(), 240);
                }

                if (projectile.type == (catalyst.Find<ModProjectile>("PrismBreakProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<PrismBreakTag>(), 240);
                }
                if (projectile.type == (catalyst.Find<ModProjectile>("CongeledCorruptProjectile")?.Type ?? -1)
                    || projectile.type == (catalyst.Find<ModProjectile>("CongeledCrimsonProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<CongeledDuoWhipTag>(), 240);
                }

                if (projectile.type == (catalyst.Find<ModProjectile>("UnderbiteSkull")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<UnderBiteTag>(), 240);
                }

                if (projectile.type == (catalyst.Find<ModProjectile>("SandstoneReignsProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<SandstoneReignsTag>(), 240);
                }

                if (projectile.type == (catalyst.Find<ModProjectile>("ResonantStrikerProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<ResonantStrikerTag>(), 240);
                }

                if (projectile.type == (catalyst.Find<ModProjectile>("BlossomsBlessingProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<BlossomsBlessingTag>(), 240);
                }

                if (projectile.type == (catalyst.Find<ModProjectile>("UnrelentingTormentProjectile")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<UnrelentingTormentTag>(), 240);
                }
                if (projectile.type == (catalyst.Find<ModProjectile>("CatharsisProjectile")?.Type ?? -1)
                || projectile.type == (catalyst.Find<ModProjectile>("HighCatharsisDown")?.Type ?? -1)
                    || projectile.type == (catalyst.Find<ModProjectile>("HighCatharsisUp")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<CatharsisTag>(), 240);
                }
            }

            if (ModLoader.TryGetMod("WulfrumExpansion", out Mod overdrive))
            {
                if (projectile.type == (overdrive.Find<ModProjectile>("ArcScepterBeam")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<StaticDischarge>(), 180);
                }

                if (projectile.type == (overdrive.Find<ModProjectile>("FriendlyWulfrumLaser")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<StaticDischarge>(), 60);
                }

                if (projectile.type == (overdrive.Find<ModProjectile>("WulfrumShrapnel")?.Type ?? -1))
                {
                    target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.HeavyBleeding>(), 60);
                }
            }
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public static class HealInspiration
    {
        public static void Init(Player obj)
        {
            obj.GetModPlayer<ThoriumPlayer>().HealInspiration(1);
        }
    }
}
