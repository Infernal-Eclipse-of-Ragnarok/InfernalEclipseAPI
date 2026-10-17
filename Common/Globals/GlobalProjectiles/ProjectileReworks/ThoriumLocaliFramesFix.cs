using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.GlobalProjectiles.ProjectileReworks
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class ThoriumLocaliFramesFix : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void SetDefaults(Projectile projectile)
        {
            Mod thorium = InfernalCrossmod.Thorium.Mod;

            if (projectile.ModProjectile is null)
                return;

            if (projectile.ModProjectile.Mod != thorium || projectile.ModProjectile.Mod.Name != "ThoriumRework")
                return;

            int pro1Type = thorium.Find<ModProjectile>("CactusNeedlePro")?.Type ?? -1;
            int pro2Type = thorium.Find<ModProjectile>("BatScythePro2")?.Type ?? -1;
            int pro3type = thorium.Find<ModProjectile>("EchoWave")?.Type ?? -1;
            int pro4type = thorium.Find<ModProjectile>("PyroExplosion2")?.Type ?? -1;
            int pro5type = thorium.Find<ModProjectile>("PyroBurst")?.Type ?? -1;
            int pro6type = thorium.Find<ModProjectile>("CalaverasPro")?.Type ?? -1;
            int pro7type = thorium.Find<ModProjectile>("CalaverasPro2")?.Type ?? -1;
            int pro8type = thorium.Find<ModProjectile>("DragonsWailPro")?.Type ?? -1;
            int pro9type = thorium.Find<ModProjectile>("ScholarsHarpPro")?.Type ?? -1;
            int pro10type = thorium.Find<ModProjectile>("AcousticWave")?.Type ?? -1;
            int pro11type = thorium.Find<ModProjectile>("RiffWeaverPro")?.Type ?? -1;
            int pro12type = thorium.Find<ModProjectile>("RiffWeaverPro2")?.Type ?? -1;
            int pro13type = thorium.Find<ModProjectile>("RiffWeaverPro3")?.Type ?? -1;
            int pro14type = thorium.Find<ModProjectile>("HellBellPro")?.Type ?? -1;
            int pro15type = thorium.Find<ModProjectile>("TerraScythePro")?.Type ?? -1;
            int pro16type = thorium.Find<ModProjectile>("ZTerraScythePro")?.Type ?? -1;
            int pro17type = thorium.Find<ModProjectile>("NaiadShivPro")?.Type ?? -1;
            int pro18type = thorium.Find<ModProjectile>("GaussFlingerPro")?.Type ?? -1;
            int pro19type = thorium.Find<ModProjectile>("KinetoscythePro")?.Type ?? -1;
            int pro20type = thorium.Find<ModProjectile>("KinetoscythePro2")?.Type ?? -1;
            int pro21type = thorium.Find<ModProjectile>("OmenPro")?.Type ?? -1;
            int pro22type = thorium.Find<ModProjectile>("PaperExplosivePro")?.Type ?? -1;
            int pro23type = thorium.Find<ModProjectile>("PaperExplosivePro2")?.Type ?? -1;
            int pro25type = thorium.Find<ModProjectile>("CorruptlingStaffPro")?.Type ?? -1;

            if (projectile.type == pro1Type || projectile.type == pro13type || projectile.type == pro3type || projectile.type == pro4type || projectile.type == pro5type || projectile.type == pro21type)
            {
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 20;
                projectile.usesIDStaticNPCImmunity = false;
            }

            if (projectile.type == pro2Type || projectile.type == pro16type || projectile.type == pro19type || projectile.type == pro20type || projectile.type == pro23type)
            {
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.usesIDStaticNPCImmunity = false;
            }

            if (projectile.type == pro6type || projectile.type == pro7type || projectile.type == pro8type || projectile.type == pro9type || projectile.type == pro10type || projectile.type == pro11type || projectile.type == pro12type || projectile.type == pro14type || 
                projectile.type == pro15type || projectile.type == pro17type || projectile.type == pro18type || projectile.type == pro22type || projectile.type == pro25type)
            {
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 40;
                projectile.usesIDStaticNPCImmunity = false;
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                Mod thoriumRework = InfernalCrossmod.ThoriumRework.Mod;

                if (thoriumRework.TryFind("ValadiumHeavyScytheWave", out ModProjectile valScytheWave))
                {
                    if (projectile.type == valScytheWave.Type)
                    {
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = 40;

                        //Make sure it's NOT using static ID-based immunity
                        projectile.usesIDStaticNPCImmunity = false;
                    }
                }

                if (thoriumRework.TryFind("ValadiumHeavyScythe", out ModProjectile valScythe))
                {
                    if (projectile.type == valScythe.Type)
                    {
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = 60;

                        //Make sure it's NOT using static ID-based immunity
                        projectile.usesIDStaticNPCImmunity = false;
                    }
                }

                if (projectile.type == thorium.Find<ModProjectile>("GraniteCharge").Type)
                {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = 60;

                    //Make sure it's NOT using static ID-based immunity
                    projectile.usesIDStaticNPCImmunity = false;
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Mod thorium = InfernalCrossmod.Thorium.Mod;

            int pyroExplosion = thorium.Find<ModProjectile>("PyroExplosion2")?.Type ?? -1;
            int pyroBurst = thorium.Find<ModProjectile>("PyroBurst")?.Type ?? -1;

            if (projectile.type != pyroExplosion && projectile.type != pyroBurst)
                return;

            // Kill Thorium's forced global iframes
            target.immune[projectile.owner] = 0;

            // Force local NPC immunity
            projectile.usesLocalNPCImmunity = true;
            projectile.usesIDStaticNPCImmunity = false;

            projectile.localNPCHitCooldown = 20;
        }
    }
}
