using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using CalamityMod;
using InfernalEclipseAPI.Core.DamageClasses;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.DataStructures;
using InfernalEclipseAPI.Core.Systems;
using static InfernalEclipseAPI.Core.Systems.InfernalCrossmod;
using ReLogic.Content;
using System.Collections.Generic;
using InfernalEclipseAPI.Core.Configs;
using Terraria;
using static ThrowerUnification.ModCompatibility;

namespace InfernalEclipseAPI.Common.Projectiles
{
    public class ProjectileBalanceChanges : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private static readonly Dictionary<int, float> ProjectileScales = new();
        private static readonly HashSet<int> CustomDrawProjectiles = new();
        private static readonly Dictionary<int, Asset<Texture2D>> Glowmasks = new();


        private static int clamFireBarrage = -1;
        private static int clamFireBarrageHoming = -1;
        private static int clamFireblast = -1;
        private static int clamFireBombExplosion = -1;
        private static int clamFirethrower = -1;

        private static int jadeLampType = -1;
        private static int goldLampType = -1;

        private static int blazingMineType = -1;
        private static int blazingSpikeType = -1;
        private static int arcLightningType = -1;
        private static int frostSpearType = -1;
        private static int earthenSpiritType = -1;
        private static int thunderRingType = -1;
        private static int irradiatedChainReactorType = -1;
        private static int irradiatedCrushType = -1;
        private static int rippleWaveSummonType = -1;
        private static int infernoLaserType = -1;
        private static int evilSpearType = -1;

        private static int moltenThresherType = -1;
        private static int fallingTwilightType = -1;
        private static int bloodHarvestType = -1;
        private static int trueFallingTwilightType = -1;
        private static int trueBloodHarvestType = -1;
        private static int titanScytheType = -1;
        private static int theBlackScytheType = -1;
        private static int batScytheType = -1;
        private static int batScytheType2 = -1;
        private static int boneBatonType = -1;
        private static int trueHallowedType = -1;
        //private static int windSlashType = -1;
        private static int crimsonType = -1;
        private static int iceType = -1;
        private static int darkType = -1;
        private static int whirlwindType = -1;
        //private static int marbleType = -1;
        private static int terraType = -1;
        private static int morningDewType = -1;
        private static int kinetoType = -1;
        private static int sarsType = -1;
        private static int palmType = -1;
        private static int paperType = -1;
        private static int icyType1 = -1;
        private static int icyType2 = -1;
        private static int icyType3 = -1;

        public override void SetStaticDefaults()
        {
            ProjectileScales.Clear();
            CustomDrawProjectiles.Clear();
            Glowmasks.Clear();

            if (InfernalConfig.Instance.ThoriumBalanceChangess && InfernalCrossmod.Thorium.Loaded)
            {
                CacheThoriumProjectileTypes(InfernalCrossmod.Thorium.Mod);
            }

            if (InfernalCrossmod.Clamity.Loaded)
            {
                Mod clam = InfernalCrossmod.Clamity.Mod;

                clamFireBarrage = FindProjectileType(clam, "FireBarrage");
                clamFireBarrageHoming = FindProjectileType(clam, "FireBarrageHoming");
                clamFireblast = FindProjectileType(clam, "Fireblast");
                clamFireBombExplosion = FindProjectileType(clam, "FireBombExplosion");
                clamFirethrower = FindProjectileType(clam, "Firethrower");
            }

            if (ModLoader.TryGetMod("Consolaria", out Mod consolaria))
            {
                jadeLampType = FindProjectileType(consolaria, "JadeSeal_Lamp");
                goldLampType = FindProjectileType(consolaria, "JadeSeal_GoldenLamp");
            }

            if (InfernalConfig.Instance.SOTSBalanceChanges && InfernalCrossmod.SOTS.Loaded)
            {
                Mod sots = InfernalCrossmod.SOTS.Mod;

                blazingMineType = FindProjectileType(sots, "BlazingMine");
                blazingSpikeType = FindProjectileType(sots, "BlazingSpike");
                arcLightningType = FindProjectileType(sots, "ArcLightning");
                frostSpearType = FindProjectileType(sots, "FrostSpear");
                earthenSpiritType = FindProjectileType(sots, "EarthenSpirit");
                thunderRingType = FindProjectileType(sots, "ThunderRing");
                irradiatedChainReactorType = FindProjectileType(sots, "IrradiatedChainReactor");
                irradiatedCrushType = FindProjectileType(sots, "IrradiatedCrush");
                rippleWaveSummonType = FindProjectileType(sots, "RippleWaveSummon");
                infernoLaserType = FindProjectileType(sots, "InfernoLaser");
                evilSpearType = FindProjectileType(sots, "EvilSpear");
            }
        }

        private static int FindProjectileType(Mod mod, string name)
        {
            return mod.TryFind(name, out ModProjectile projectile) ? projectile.Type : -1;
        }

        private static void CacheThoriumProjectileTypes(Mod thorium)
        {
            moltenThresherType = FindProjectileType(thorium, "MoltenThresherPro");
            batScytheType = FindProjectileType(thorium, "BatScythePro");
            batScytheType2 = FindProjectileType(thorium, "BatScythePro2");
            fallingTwilightType = FindProjectileType(thorium, "FallingTwilightPro");
            bloodHarvestType = FindProjectileType(thorium, "BloodHarvestPro");
            trueFallingTwilightType = FindProjectileType(thorium, "TrueFallingTwilightPro");
            trueBloodHarvestType = FindProjectileType(thorium, "TrueBloodHarvestPro");
            theBlackScytheType = FindProjectileType(thorium, "TheBlackScythePro");
            titanScytheType = FindProjectileType(thorium, "TitanScythePro");
            boneBatonType = FindProjectileType(thorium, "BoneBatonPro");
            trueHallowedType = FindProjectileType(thorium, "TrueHallowedScythePro");
            crimsonType = FindProjectileType(thorium, "CrimtaneScythePro");
            iceType = FindProjectileType(thorium, "IceShaverPro");
            darkType = FindProjectileType(thorium, "DemoniteScythePro");
            terraType = FindProjectileType(thorium, "TerraScythePro");
            morningDewType = FindProjectileType(thorium, "MorningDewPro");
            kinetoType = FindProjectileType(thorium, "KinetoscythePro2");
            palmType = FindProjectileType(thorium, "StonePalmPro");
            paperType = FindProjectileType(thorium, "PaperExplosivePro2");
            icyType1 = thorium.Find<ModProjectile>("IcyArmorEffect1")?.Type ?? -1;
            icyType2 = thorium.Find<ModProjectile>("IcyArmorEffect2")?.Type ?? -1;
            icyType3 = thorium.Find<ModProjectile>("IcyArmorEffect3")?.Type ?? -1;

            if (ModLoader.TryGetMod("CalamityBardHealer", out Mod calBardHeal))
            {
                whirlwindType = FindProjectileType(calBardHeal, "Whirlwind");
                sarsType = FindProjectileType(calBardHeal, "SARS");
            }

            AddScale(moltenThresherType, 1.5f);
            AddScale(batScytheType, 1.5f);
            AddScale(batScytheType2, 3f);
            AddScale(fallingTwilightType, 1.5f);
            AddScale(bloodHarvestType, 1.5f);
            AddScale(trueFallingTwilightType, 1.5f);
            AddScale(trueBloodHarvestType, 1.5f);
            AddScale(theBlackScytheType, 1.5f);
            AddScale(titanScytheType, 2f);
            AddScale(trueHallowedType, 1.3f);
            AddScale(boneBatonType, 2f);
            AddScale(crimsonType, 1.2f);
            AddScale(iceType, 1.2f);
            AddScale(darkType, 1.1f);
            AddScale(whirlwindType, 1.5f);
            AddScale(terraType, 1.6f);
            AddScale(morningDewType, 1.5f);
            AddScale(kinetoType, 1.5f);
            AddScale(sarsType, 1.5f);
            AddScale(paperType, 3f);
            AddScale(palmType, 2f);
            AddScale(icyType1, 2f);
            AddScale(icyType2, 2f);
            AddScale(icyType3, 2f);

            if (moltenThresherType > 0)
                Glowmasks[moltenThresherType] = ModContent.Request<Texture2D>("ThoriumMod/Projectiles/Scythe/MoltenThresherPro_Glowmask");
        }

        private static void AddScale(int type, float scale, bool customDraw = true)
        {
            if (type <= 0)
                return;

            ProjectileScales[type] = scale;

            if (customDraw)
                CustomDrawProjectiles.Add(type);
        }

        public override void AI(Projectile projectile)
        {
            if (projectile.localAI[2] == 1f)
                return;

            if (!ProjectileScales.TryGetValue(projectile.type, out float scale))
                return;

            Vector2 oldCenter = projectile.Center;

            projectile.scale *= scale;
            projectile.width = (int)(projectile.width * scale);
            projectile.height = (int)(projectile.height * scale);
            projectile.Center = oldCenter;

            projectile.localAI[2] = 1f;
        }

        public void EnsureScaled(Projectile projectile)
        {
            AI(projectile);
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.type == icyType1 || projectile.type == icyType2 || projectile.type == icyType3)
            {
                DrawIcyProjectiles(projectile);
                return false;
            }

            if (!CustomDrawProjectiles.Contains(projectile.type))
                return true;

            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[projectile.type];

            Rectangle sourceRectangle = new(0, frameHeight * projectile.frame, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() * 0.5f;
            Vector2 drawPos = projectile.Center - Main.screenPosition;

            SpriteEffects effects = Main.player[projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, Color.White, projectile.rotation, origin, projectile.scale, effects, 0);

            if (Glowmasks.TryGetValue(projectile.type, out Asset<Texture2D> glowAsset))
            {
                Texture2D glowTexture = glowAsset.Value;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(glowTexture, drawPos, sourceRectangle, Color.White, projectile.rotation, origin, projectile.scale, effects, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

        private static void DrawIcyProjectiles(Projectile projectile)
        {
            Texture2D blur = ModContent.Request<Texture2D>(ModContent.GetModProjectile(projectile.type).Texture + "_Blur", AssetRequestMode.ImmediateLoad).Value;

            Vector2 pos = projectile.Center + new Vector2(0, projectile.gfxOffY) - Main.screenPosition;

            Rectangle frame = Utils.Frame(blur, 1, Main.projFrames[projectile.type], 0, projectile.frame);

            Vector2 origin = frame.Size() / 2f;

            // Draw blur
            Main.EntitySpriteDraw(blur, pos, frame, Color.White * 0.25f, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);

            // Draw main texture
            Texture2D tex = TextureAssets.Projectile[projectile.type].Value;

            Main.EntitySpriteDraw(tex, pos, frame, Color.White * 0.5f, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
        }

        public override void PostDraw(Projectile projectile, Color lightColor) { }

        public override void SetDefaults(Projectile entity)
        {
            #region Vanilla
            if (entity.type == ProjectileID.PewMaticHornShot && InfernalConfig.Instance.VanillaBalanceChanges)
            {
                entity.penetrate = 2;
            }

            if (entity.type == ProjectileID.Trimarang)
            {
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 40;

                // Make sure it's NOT using static ID-based immunity
                entity.usesIDStaticNPCImmunity = false;
            }
            #endregion

            #region Clamity
            if (InfernalConfig.Instance.CalamityBalanceChanges && InfernalCrossmod.Clamity.Loaded)
            {
                if (entity.type == clamFireBarrage)
                    entity.damage = 135;
                else if (entity.type == clamFireBarrageHoming)
                    entity.damage = 130;
                else if (entity.type == clamFireblast)
                    entity.damage = 140;
                else if (entity.type == clamFireBombExplosion)
                    entity.damage = 135;
                else if (entity.type == clamFirethrower)
                    entity.damage = 150;
            }
            #endregion

            #region Thorium
            if (ModLoader.TryGetMod("Thorium", out Mod thorium) && InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (entity.type == thorium.Find<ModProjectile>("SeashellCastanettessPro1").Type)
                {
                    entity.penetrate = 2;
                }

                if (entity.type == thorium.Find<ModProjectile>("Cube").Type)
                {
                    entity.penetrate = 3;
                }

                if (entity.type == thorium.Find<ModProjectile>("GeyserPro2").Type)
                {
                    entity.scale *= 5;
                }

                if (entity.type == thorium.Find<ModProjectile>("GraniteBarrier").Type)
                {
                }

                if (entity.type == thorium.Find<ModProjectile>("PalmCrossPro").Type)
                {
                    entity.scale *= 2;
                }

                if (entity.type == thorium.Find<ModProjectile>("TorpedoPro2").Type)
                {
                    entity.scale *= 2;
                    entity.penetrate = 10;
                }

                if (entity.type == thorium.Find<ModProjectile>("StoneThrowingSpearPro").Type)
                {
                    entity.penetrate = 2;
                }

                if (entity.type == thorium.Find<ModProjectile>("IcyTomahawkPro").Type)
                {
                    entity.penetrate = 5;
                }

                if (entity.type == thorium.Find<ModProjectile>("AquaiteScythePro").Type || entity.type == thorium.Find<ModProjectile>("BloodHarvestPro").Type || entity.type == thorium.Find<ModProjectile>("FallingTwilightPro").Type || entity.type == thorium.Find<ModProjectile>("TrueHallowedScythePro").Type || entity.type == thorium.Find<ModProjectile>("HallowedScythePro").Type)
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = 12;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = 122;
                    }
                }

                if (entity.type == thorium.Find<ModProjectile>("BoneReaperPro").Type)
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = 3;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = 3;
                    }
                }

                if (entity.type == thorium.Find<ModProjectile>("BatScythePro2").Type)
                {
                    //if (entity.usesLocalNPCImmunity)
                    //{
                    //    entity.localNPCHitCooldown = 1;
                    //}

                    //if (entity.usesIDStaticNPCImmunity)
                    //{
                    //    entity.idStaticNPCHitCooldown = 1;
                    //}
                }

                if (entity.type == thorium.Find<ModProjectile>("WyvernSlayerPro2").Type)
                {
                    entity.usesLocalNPCImmunity = true;
                    entity.localNPCHitCooldown = 40;

                    entity.usesIDStaticNPCImmunity = false;
                }
            }
            #endregion

            #region Ragnarok
            /*
            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok) && InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (entity.type == ragnarok.Find<ModProjectile>("GelScythePro2").Type)
                {
                    entity.penetrate = 5;
                    entity.scale = 2;
                }

                //if (entity.type == ragnarok.Find<ModProjectile>("WindSlashPro").Type)
                //{
                //    entity.scale *= 2;
                //}

                if (entity.type == ragnarok.Find<ModProjectile>("ProfanedBellBlast").Type)
                {
                    entity.penetrate = 3;
                    entity.scale = 0.75f;
                }

                if (entity.type == ragnarok.Find<ModProjectile>("ElysianSongPro").Type)
                {
                    entity.penetrate = 20;
                    entity.scale = 1.5f;
                }

                if (entity.type == ragnarok.Find<ModProjectile>("TendrilStrike").Type)
                {
                    entity.scale = 1.5f;
                }

                if (entity.type == ragnarok.Find<ModProjectile>("MarbleScythePro").Type || entity.type == ragnarok.Find<ModProjectile>("ProfanedScythePro").Type)
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = 6;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = 6;
                    }
                }

                if (entity.type == ragnarok.Find<ModProjectile>("ScoriaDualscythePro").Type)
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = 3;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = 3;
                    }
                }

                if (entity.type == ragnarok.Find<ModProjectile>("AuricDamruShock").Type)
                {
                    entity.scale = 2;
                }

                if (entity.type == ragnarok.Find<ModProjectile>("GraspofVoidPro1").Type)
                {
                    entity.penetrate = 6;
                }
            }
            */
            #endregion

            #region Unofficial Calamity Bard and Healer
            if (ModLoader.TryGetMod("CalamityBardHealer", out Mod calBardHeal) && InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (entity.type == calBardHeal.Find<ModProjectile>("ExoSound").Type)
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = -1;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = 1;
                    }
                }

                if (ModLoader.HasMod("CatalystMod"))
                {
                    if (entity.type == calBardHeal.Find<ModProjectile>("StarBirth").Type)
                    {
                        //entity.scale *= 0.3f;
                    }
                }

                if (InfernalCrossmod.Hunt.Loaded)
                {
                    if (entity.type == calBardHeal.Find<ModProjectile>("TimesOldTrail").Type)
                    {
                        entity.usesLocalNPCImmunity = false;

                        entity.usesIDStaticNPCImmunity = true;
                        entity.idStaticNPCHitCooldown = 4;
                        entity.timeLeft = 300;
                    }
                }
            }
            #endregion

            #region Thorium Helhiem
            if (ModLoader.TryGetMod("ThoriumRework", out Mod thorRework) && InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (GetProj(entity, thorRework, "DemonBloodSword") ||
                    GetProj(entity, thorRework, "DragonTooth") ||
                    GetProj(entity, thorRework, "DreadRazor") || 
                    GetProj(entity, thorRework, "IllumiteBlade") ||
                    GetProj(entity, thorRework, "LodeStoneClaymore") ||
                    GetProj(entity, thorRework, "SoulRender") || 
                    GetProj(entity, thorRework, "TerrariumSaber") ||
                    GetProj(entity, thorRework, "TitanSword") ||
                    GetProj(entity, thorRework, "ToothOfTheConsumer") ||
                    GetProj(entity, thorRework, "BeholderBlade") ||
                    GetProj(entity, thorRework, "GraniteCharge"))
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = 60;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = 60;
                    }
                }

                if (GetProj(entity, thorRework, "GrandThunder"))
                {
                    if (entity.usesLocalNPCImmunity)
                    {
                        entity.localNPCHitCooldown = -1;
                    }

                    if (entity.usesIDStaticNPCImmunity)
                    {
                        entity.idStaticNPCHitCooldown = -1;
                    }
                }

                if (GetProj(entity, thorRework, "ValadiumHeavyScytheWave"))
                {
                    entity.penetrate = 5;
                }

                if (GetProj(entity, thorRework, "GraniteCharge"))
                {
                    entity.penetrate = 1;
                }
            }
            #endregion

            #region Secrets of the Shadows
            if (ModLoader.TryGetMod("SOTS", out Mod sots) && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (GetProj(entity, sots, "BetrayersSlash"))
                {
                    entity.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }

                if (GetProj(entity, sots, "SootBall"))
                {
                    entity.usesIDStaticNPCImmunity = false;

                    entity.localNPCHitCooldown = entity.idStaticNPCHitCooldown;
                    entity.usesLocalNPCImmunity = true;
                }

                if (GetProj(entity, sots, "AncientSteelBullet"))
                {
                    entity.usesIDStaticNPCImmunity = false;

                    entity.localNPCHitCooldown = entity.idStaticNPCHitCooldown;
                    entity.usesLocalNPCImmunity = true;
                }
            }
            #endregion

            #region Unofficial SOTS Bard, Healer, and Thrower
            if (ModLoader.TryGetMod("SOTSBardHealer", out Mod sotsBH) && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (GetProj(entity, sotsBH, "DualStylophonePro"))
                {
                    entity.penetrate = 3;
                }

                if (GetProj(entity, sotsBH, "GoopwoodWiggle"))
                {
                    entity.localNPCHitCooldown = 30;
                    entity.usesLocalNPCImmunity = true;
                }

                if (GetProj(entity, sotsBH, "GoopwoodSplit") || GetProj(entity, sotsBH, "ForbiddenMaelstrom") || GetProj(entity, sotsBH, "Serpentbite"))
                {
                    if (InfernalConfig.Instance.SOTSThrowerToRogue) entity.DamageType = ModContent.GetInstance<VoidRogue>();
                }
            }
            #endregion

            #region Consolaria
            if (InfernalCrossmod.Consolaria.Loaded && InfernalConfig.Instance.ConsolariaBalanceChanges)
            {
                if (GetProj(entity, InfernalCrossmod.Consolaria.Mod, "TonbogiriSpear"))
                {
                    entity.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                }
            }
            #endregion

            #region Calamity Overdrive
            if (InfernalCrossmod.Overdrive.Loaded && InfernalConfig.Instance.CalamityBalanceChanges)
            {
                if (GetProj(entity, InfernalCrossmod.Overdrive.Mod, "FriendlyWulfrumLaser"))
                {
                    entity.DamageType = RangedDamageClass.Ranged;
                    entity.usesIDStaticNPCImmunity = false;

                    entity.localNPCHitCooldown = 10;
                    entity.usesLocalNPCImmunity = true;
                }
            }
            #endregion
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.ModProjectile != null)
            {
                if (projectile.ModProjectile.Mod.Name == "ThoriumMod" && InfernalConfig.Instance.ThoriumBalanceChangess && !Hummus.Loaded)
                {
                    if (projectile.ModProjectile.Name == "TideDagger")
                    {
                        projectile.damage /= 10;
                    }

                    if (projectile.ModProjectile.Name == "InfernoLordsFocusPro")
                    {
                        if (projectile.owner == Main.myPlayer && Main.LocalPlayer.ownedProjectileCounts[projectile.type] >= 7)
                        {
                            projectile.damage = 0;
                            projectile.active = false;
                        }
                        else
                            projectile.damage /= 3;
                    }

                    if (projectile.ModProjectile.Name == "WyvernSlayerPro2")
                    {
                        projectile.damage /= 2;
                    }
                }

                if (projectile.ModProjectile.Mod.Name == "CalamityAmmo" && InfernalConfig.Instance.CalamityBalanceChanges) 
                {
                    if (projectile.ModProjectile.Name == "Shroomere")
                    {
                        projectile.damage /= 6;
                    }

                    if (projectile.ModProjectile.Name == "Crabulon_Spore" || projectile.ModProjectile.Name == "Spore1" || projectile.ModProjectile.Name == "Spore2" || projectile.ModProjectile.Name == "Spore3")
                    {
                        projectile.damage /= 2;
                    }

                    if (projectile.ModProjectile.Name == "SoulBullet_Proj")
                    {
                        projectile.damage /= 2;
                    }
                }

                if (projectile.ModProjectile.Mod.Name == "CalamitySimpleWhipAddon" && InfernalConfig.Instance.CalamityBalanceChanges)
                {
                    /*
                    if (projectile.ModProjectile.Name == "AncientBondsProj")
                    {
                        projectile.WhipSettings.RangeMultiplier = 0.6f;
                    }

                    if (projectile.ModProjectile.Name == "MilkywayProj")
                    {
                        projectile.WhipSettings.RangeMultiplier = 2.7f;
                    }

                    if (projectile.ModProjectile.Name == "ChorusofExecrationProj")
                    {
                        projectile.WhipSettings.RangeMultiplier = 2.8f;
                    }
                    */
                }

                if (InfernalConfig.Instance.SOTSBalanceChanges) 
                {
                    if (projectile.ModProjectile.Mod.Name == "SOTS") 
                    {
                        if (projectile.ModProjectile.Name == "RevolutionBoltDay")
                        {
                            projectile.damage /= 3;
                        }

                        if (projectile.ModProjectile.Name == "RevolutionBolt")
                        {
                            projectile.damage /= 3;
                        }
                    }

                    if (projectile.ModProjectile.Mod.Name == "SOTSBardHealer") 
                    {
                        if (projectile.ModProjectile.Name == "TurboSlicerThrown")
                        {
                            projectile.damage *= 2;
                        }
                    }
                }

                if (InfernalCrossmod.Consolaria.Loaded && projectile.owner >= 0)
                {
                    if (projectile.type == jadeLampType)
                        KillPlayerProjectiles(projectile.owner, goldLampType);
                    else if (projectile.type == goldLampType)
                        KillPlayerProjectiles(projectile.owner, jadeLampType);
                }
            }
        }

        private static void KillPlayerProjectiles(int owner, int projType)
        {
            if (projType <= 0) return;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == owner && proj.type == projType)
                {
                    proj.Kill();
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int type = projectile.type;

            #region Secrets of the Shadows
            if (InfernalCrossmod.SOTS.Loaded && InfernalConfig.Instance.SOTSBalanceChanges)
            {
                if (type == blazingMineType || type == blazingSpikeType)
                    target.AddBuff(BuffID.OnFire, 120);
                else if (type == arcLightningType)
                    target.AddBuff(BuffID.Electrified, 180);
                else if (type == frostSpearType)
                    target.AddBuff(BuffID.Frostburn, 180);
                else if (type == earthenSpiritType)
                    target.AddBuff(ModContent.BuffType<Crumbling>(), 60);
                else if (type == thunderRingType)
                    target.AddBuff(BuffID.Electrified, 120);
                else if (type == irradiatedChainReactorType || type == irradiatedCrushType)
                    target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
                else if (type == rippleWaveSummonType)
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 60);
                else if (type == infernoLaserType)
                    target.AddBuff(BuffID.OnFire3, 60);
                else if (type == evilSpearType)
                    target.AddBuff(ModContent.BuffType<BrainRot>(), 60);
            }
            #endregion

            #region Thorium
            if (InfernalCrossmod.Thorium.Loaded && InfernalConfig.Instance.ThoriumBalanceChangess)
            {
                if (type == InfernalCrossmod.Thorium.Mod.Find<ModProjectile>("WyvernSlayerPro2").Type)
                {
                    // Kill Thorium's forced global iframes
                    target.immune[projectile.owner] = 0;

                    // Force local NPC immunity
                    projectile.usesLocalNPCImmunity = true;
                    projectile.usesIDStaticNPCImmunity = false;

                    // Adjust this to taste
                    projectile.localNPCHitCooldown = 40;
                }
            }
            #endregion
        }

        private static bool GetProj(Projectile entity, Mod mod, string item)
        {
            mod.TryFind(item, out ModProjectile projectile);
            if (projectile == null)
            {
                return false;
            }
            if (entity.type == projectile.Type)
            {
                return true;
            }
            return false;
        }
    }
}
