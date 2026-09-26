using CalamityMod.Projectiles.Healing;
using MonoMod.Cil;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Core.Systems.Hooks.ILItemChanges
{
    internal class LifestealCooldownIntergrationHooks : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => !InfernalCrossmod.Hummus.Loaded;

        private static readonly string[] DisabledOnHitProjectiles =
        {
            "FleshSkewerPro",
            "VeinBusterPro",
            "FleshMacePro",
            "BloodClotStaffPro",
            "LeechBoltPro",
            "SpiritBlastWandPro2",
            //Spirit Bender is done in set defaults instead since it's on hit is fine
            "BloodTransfusionPro",
        };

        private static readonly string[] DisabledOnHitItems =
        {
            "LifeQuartzClaymore",
        };

        private static readonly string[] DisabledReworkOnHitProjectiles =
        {
            "ToothOfTheConsumer",
        };

        public override void Load()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                return;

            foreach (string projectileName in DisabledOnHitProjectiles)
            {
                ModProjectile projectile = thorium.Find<ModProjectile>(projectileName);

                if (projectile == null)
                    continue;

                MethodInfo onHitNPC = projectile.GetType().GetMethod("OnHitNPC", LumUtils.UniversalBindingFlags);

                if (onHitNPC == null)
                    continue;

                MonoModHooks.Modify(onHitNPC, RemoveOnHitNPC);
            }

            foreach (string itemName in DisabledOnHitItems)
            {
                ModItem item = thorium.Find<ModItem>(itemName);

                if (item == null)
                    continue;

                MethodInfo onHitNPC = item.GetType().GetMethod("OnHitNPC", LumUtils.UniversalBindingFlags);

                if (onHitNPC == null)
                    continue;

                MonoModHooks.Modify(onHitNPC, RemoveOnHitNPC);
            }

            if (!ModLoader.TryGetMod("ThoriumRework", out Mod thoriumRework))
                return;

            foreach (string projectileName in DisabledReworkOnHitProjectiles)
            {
                ModProjectile projectile = thoriumRework.Find<ModProjectile>(projectileName);

                if (projectile == null)
                    continue;

                MethodInfo onHitNPC = projectile.GetType().GetMethod("OnHitNPC", LumUtils.UniversalBindingFlags);

                if (onHitNPC == null)
                    continue;

                MonoModHooks.Modify(onHitNPC, RemoveOnHitNPC);
            }

            // Disable Helheim's separate GlobalProjectile lifesteal.
            DisableHelheimFleshSkewerLifesteal(thoriumRework);
        }

        private static void RemoveOnHitNPC(ILContext il)
        {
            ILCursor cursor = new(il);
            cursor.EmitRet();
        }

        //FleshSkewer specifically thanks Helheim
        private static void DisableHelheimFleshSkewerLifesteal(Mod thoriumRework)
        {
            Type projectileChangesType = thoriumRework.Code.GetType("ThoriumRework.ProjectileChanges");

            if (projectileChangesType == null)
                return;

            MethodInfo onHitNPC = projectileChangesType.GetMethod("OnHitNPC", LumUtils.UniversalBindingFlags);

            if (onHitNPC == null)
                return;

            MonoModHooks.Modify(onHitNPC, DisableHelheimFleshSkewerLifestealIL);
        }

        private static void DisableHelheimFleshSkewerLifestealIL(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Find the FleshSkewerPro comparison used by Helheim's special lifesteal block and make it impossible to match.
            if (!cursor.TryGotoNext(MoveType.Before, instruction => instruction.MatchLdstr("FleshSkewerPro")))
            {
                return;
            }

            cursor.Next.Operand = "__WHummus_DisabledFleshSkewerLifesteal__";
        }
    }

    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class LifestealCooldownAdditionsThorium : GlobalProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => !InfernalCrossmod.Hummus.Loaded;

        public override bool InstancePerEntity => true;

        private bool firstHit = true;

        private static int toothOfTheConsumerType = -1;
        private static int fleshSkewerType = -1;
        private static int veinBusterType = -1;
        private static int fleshMaceType = -1;
        private static int bloodClotStaffType = -1;
        private static int leechBoltType = -1;
        private static int spiritBlastType = -1;
        private static int SpiritBenderType = -1;
        private static int BloodTransfusionType = -1;

        private static int fleshBowItemType = -1;
        public bool fleshBowTag = false;

        public override void SetStaticDefaults()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out Mod thorium)) 
                return;

            fleshSkewerType = thorium.Find<ModProjectile>("FleshSkewerPro")?.Type ?? -1;
            veinBusterType = thorium.Find<ModProjectile>("VeinBusterPro")?.Type ?? -1;
            fleshMaceType = thorium.Find<ModProjectile>("FleshMacePro")?.Type ?? -1;
            bloodClotStaffType = thorium.Find<ModProjectile>("BloodClotStaffPro")?.Type ?? -1;
            leechBoltType = thorium.Find<ModProjectile>("LeechBoltPro")?.Type ?? -1;
            spiritBlastType = thorium.Find<ModProjectile>("SpiritBlastWandPro2")?.Type ?? -1;
            SpiritBenderType = thorium.Find<ModProjectile>("SpiritBendersStaffPro")?.Type ?? -1;
            BloodTransfusionType = thorium.Find<ModProjectile>("BloodTransfusionPro")?.Type ?? -1;

            fleshBowItemType = thorium.Find<ModItem>("FleshBow")?.Type ?? -1;

            if (!ModLoader.TryGetMod("ThoriumRework", out Mod thoriumRework)) 
                return;

            toothOfTheConsumerType = thoriumRework.Find<ModProjectile>("ToothOfTheConsumer")?.Type ?? -1;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            //Disabling Leechbolt's healing
            if (projectile.type == leechBoltType)
            {
                projectile.ai[0] = 1f;
            }

            //FleshBow stuff
            if (fleshBowItemType == -1)
                return;

            if (source is EntitySource_ItemUse_WithAmmo itemSource && itemSource.Item.type == fleshBowItemType)
            {
                fleshBowTag = true;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];

            if (Main.myPlayer != projectile.owner)
                return;

            if (!target.CanBeChasedBy(projectile))
                return;

            //Blood Clot Staff making it not infinitely pierce
            if (projectile.type == bloodClotStaffType)
            {
                projectile.timeLeft = 4;
            }

            //Flesh with crit mechanics
            if (projectile.type == toothOfTheConsumerType || projectile.type == fleshSkewerType)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                if (firstHit)
                {
                    float healAmount = damageDone / 20;
                    if (healAmount > player.lifeSteal)
                    {
                        healAmount = player.lifeSteal;
                    }
                    if (player.lifeSteal < 0)
                    {
                        healAmount += player.lifeSteal;

                        if (healAmount <= 0)
                        {
                            return;
                        }

                        if (healAmount > player.statLifeMax2 - player.statLife)
                        {
                            healAmount = player.statLifeMax2 - player.statLife;
                        }
                    }

                    if (hit.Crit)
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ProjectileID.VampireHeal, 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                        player.lifeSteal -= healAmount;
                        firstHit = false;
                    }
                    else if (Terraria.Utils.NextBool(Main.rand, 3))
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ProjectileID.VampireHeal, 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                        player.lifeSteal -= healAmount;
                        firstHit = false;

                        if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                        {
                            player.AddBuff(thorium.Find<ModBuff>("LifeTransfusion").Type, 300, true, false
                            );
                        }
                    }

                    firstHit = false;
                }
            }

            //Flesh without crit mechanics
            else if (projectile.type == veinBusterType || projectile.type == bloodClotStaffType)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                if (firstHit)
                {
                    float healAmount = damageDone / 20;
                    if (healAmount > player.lifeSteal)
                    {
                        healAmount = player.lifeSteal;
                    }
                    if (player.lifeSteal < 0)
                    {
                        healAmount += player.lifeSteal;

                        if (healAmount <= 0)
                        {
                            return;
                        }

                        if (healAmount > player.statLifeMax2 - player.statLife)
                        {
                            healAmount = player.statLifeMax2 - player.statLife;
                        }
                    }

                    if (Terraria.Utils.NextBool(Main.rand, 3))
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ProjectileID.VampireHeal, 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                        player.lifeSteal -= healAmount;
                        firstHit = false;

                        if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                        {
                            player.AddBuff(thorium.Find<ModBuff>("LifeTransfusion").Type, 300, true, false);
                        }
                    }
                }
            }

            //Flesh without first hit and crit mechanics
            else if (projectile.type == fleshMaceType || fleshBowTag)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                float healAmount = damageDone / 20;
                if (healAmount > player.lifeSteal)
                {
                    healAmount = player.lifeSteal;
                }
                if (player.lifeSteal < 0)
                {
                    healAmount += player.lifeSteal;

                    if (healAmount <= 0)
                    {
                        return;
                    }

                    if (healAmount > player.statLifeMax2 - player.statLife)
                    {
                        healAmount = player.statLifeMax2 - player.statLife;
                    }
                }

                if (Terraria.Utils.NextBool(Main.rand, 3))
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ProjectileID.VampireHeal, 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                    player.lifeSteal -= healAmount;

                    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                    {
                        player.AddBuff(thorium.Find<ModBuff>("LifeTransfusion").Type, 300, true, false);
                    }
                }
            }

            //Leech Bolt
            else if (projectile.type == leechBoltType)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                int bonusHealing = 0;

                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    object result = thorium.Call("GetHealerHealBonus", player);

                    if (result is int healBonus)
                    {
                        bonusHealing = healBonus;
                    }
                }

                float healAmount = 1 + bonusHealing;

                if (healAmount > player.lifeSteal)
                {
                    healAmount = player.lifeSteal;
                }
                if (player.lifeSteal < 0)
                {
                    healAmount += player.lifeSteal;

                    if (healAmount <= 0)
                    {
                        return;
                    }

                    if (healAmount > player.statLifeMax2 - player.statLife)
                    {
                        healAmount = player.statLifeMax2 - player.statLife;
                    }
                }

                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ProjectileID.VampireHeal, 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                player.lifeSteal -= healAmount;
            }

            // Spirit Blast Wand
            else if (projectile.type == spiritBlastType)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                if (firstHit)
                {
                    float healAmount = 1;

                    if (healAmount > player.lifeSteal)
                    {
                        healAmount = player.lifeSteal;
                    }
                    if (player.lifeSteal < 0)
                    {
                        healAmount += player.lifeSteal;

                        if (healAmount <= 0)
                        {
                            return;
                        }

                        if (healAmount > player.statLifeMax2 - player.statLife)
                        {
                            healAmount = player.statLifeMax2 - player.statLife;
                        }
                    }

                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ModContent.ProjectileType<FungalHeal>(), 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                    player.lifeSteal -= healAmount;

                    firstHit = false;
                }
            }

            //SpiritBenderStaff
            else if (projectile.type == SpiritBenderType)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                int bonusHealing = 0;

                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    object result = thorium.Call("GetHealerHealBonus", player);

                    if (result is int healBonus)
                    {
                        bonusHealing = healBonus;
                    }
                }

                float healAmount = 5 + bonusHealing;

                if (healAmount > player.lifeSteal)
                {
                    healAmount = player.lifeSteal;
                }
                if (player.lifeSteal < 0)
                {
                    healAmount += player.lifeSteal;

                    if (healAmount <= 0)
                    {
                        return;
                    }

                    if (healAmount > player.statLifeMax2 - player.statLife)
                    {
                        healAmount = player.statLifeMax2 - player.statLife;
                    }
                }

                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, ModContent.ProjectileType<FungalHeal>(), 0, 0f, projectile.owner, player.whoAmI, healAmount, 0f);
                player.lifeSteal -= (healAmount / 1.5f);
            }

            //Blood Transfusion
            else if (projectile.type == BloodTransfusionType)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    return;
                }

                int bonusHealing = 0;

                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    object result = thorium.Call("GetHealerHealBonus", player);

                    if (result is int healBonus)
                    {
                        bonusHealing = healBonus;
                    }
                }

                float healAmount = 4 + bonusHealing;

                //Original code
                if (player.statLife < player.statLifeMax2 && !target.friendly && target.lifeMax > 5 && target.chaseable && !target.dontTakeDamage && !target.immortal)
                {
                    if (player.lifeSteal > 0)
                    {
                        Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Microsoft.Xna.Framework.Vector2.Zero, ModContent.ProjectileType<BloodTransfusionProReturn>(), 0, 0f, projectile.owner, 0f, 0f, 0f);
                        player.lifeSteal -= healAmount;
                    }
                }
                if (player.GetThoriumPlayer().darkAura)
                {
                    target.AddBuff(BuffID.ShadowFlame, 90, false);
                }
                for (int k = 0; k < 10; k++)
                {
                    Dust dust = Dust.NewDustDirect(((Entity)target).position, (projectile).width, (projectile).height, DustID.LifeDrain, Terraria.Utils.NextFloat(Main.rand, -4f, 4f), Terraria.Utils.NextFloat(Main.rand, -4f, 4f), 0, default, 1f);
                    dust.noGravity = true;
                    if (player.GetThoriumPlayer().darkAura)
                    {
                        dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
                    }
                }
            }
        }
    }
}