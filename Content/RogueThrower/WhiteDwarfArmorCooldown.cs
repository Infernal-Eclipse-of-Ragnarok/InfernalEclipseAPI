using System.Security.Policy;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using RagnarokMod.Utils;
using Terraria.Audio;
using Terraria.DataStructures;
using ThoriumMod;
using ThoriumMod.Buffs.Thrower;
using ThoriumMod.Projectiles.Thrower;

namespace InfernalEclipseAPI.Content.RogueThrower
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class WhiteDwarfArmorCooldown : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            IEntitySource sourceOnHit = projectile.GetSource_OnHit(target, null);
            Player player = Main.player[projectile.owner];

            var thoriumPlayer = player.GetModPlayer<ThoriumPlayer>();
            var cdPlayer = player.GetModPlayer<RogueThrowerPlayer>();

            bool ragWhiteDwarf = false;
            if (InfernalCrossmod.RagnarokMod.Loaded)
            {
                ragWhiteDwarf = GetRagnarokModPlayerField.RagnarokWhiteDwarfActive(player);
            }

            if ((thoriumPlayer.setWhiteDwarf || ragWhiteDwarf) && hit.Crit)
            {
                if (cdPlayer.whiteDwarfCooldown > 0)
                {
                    thoriumPlayer.setWhiteDwarf = false;

                    /*
                    if (InfernalCrossmod.RagnarokMod.Loaded)
                        GetRagnarokModPlayerField.SetWhiteDwarf(player, false);
                    */
                }
                else
                {
                    int cooldown = 60; // Default cooldown

                    /*
                    // Check if White Dwarf Thrusters accessory is equipped
                    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium) &&
                        thorium.TryFind<ModItem>("WhiteDwarfThrusters", out var thrusterItem))
                    {
                        int whiteDwarfThrustersType = thrusterItem.Type;

                        for (int i = 3; i < player.armor.Length; i++) // accessories start at index 3
                        {
                            Item accessory = player.armor[i];

                            if (!accessory.IsAir && accessory.type == whiteDwarfThrustersType)
                            {
                                cooldown = 60; // Reduced cooldown
                                break;
                            }
                        }
                    }
                    */

                    cdPlayer.whiteDwarfCooldown = cooldown;
                }
            }

            if (thoriumPlayer.accShinobiSigil)
            {
                thoriumPlayer.accShinobiSigil = false;
                if (hit.Crit)
                {
                    int shinobiSigilPro = ModContent.ProjectileType<ShinobiSigilPro>();
                    if (projectile.type != shinobiSigilPro)
                    {
                        ++thoriumPlayer.accShinobiSigilCrit;
                        if (thoriumPlayer.accShinobiSigilCrit >= 2)
                        {
                            thoriumPlayer.accShinobiSigilCrit = 0;
                            if (!(cdPlayer.ShinobiSigilCooldown > 0))
                            {
                                SoundEngine.PlaySound(SoundID.Item103, projectile.Center, null);
                                player.AddBuff(ModContent.BuffType<ThrowingSpeed>(), 150, true, false);
                                int shinobiDamge = (int)(projectile.damage * 0.25);
                                Projectile.NewProjectile(sourceOnHit, ((Entity)target).Center.X, ((Entity)target).Center.Y, 0.0f, -1f, shinobiSigilPro, shinobiDamge, 1f, projectile.owner, 0.0f, 0.0f, 0.0f);
                                Projectile.NewProjectile(sourceOnHit, ((Entity)target).Center.X, ((Entity)target).Center.Y, 0.75f, 1f, shinobiSigilPro, shinobiDamge, 1f, projectile.owner, 0.0f, 0.0f, 0.0f);
                                Projectile.NewProjectile(sourceOnHit, ((Entity)target).Center.X, ((Entity)target).Center.Y, -1f, -0.75f, shinobiSigilPro, shinobiDamge, 1f, projectile.owner, 0.0f, 0.0f, 0.0f);
                                Projectile.NewProjectile(sourceOnHit, ((Entity)target).Center.X, ((Entity)target).Center.Y, 1f, -0.75f, shinobiSigilPro, shinobiDamge, 1f, projectile.owner, 0.0f, 0.0f, 0.0f);
                                Projectile.NewProjectile(sourceOnHit, ((Entity)target).Center.X, ((Entity)target).Center.Y, -0.75f, 1f, shinobiSigilPro, shinobiDamge, 1f, projectile.owner, 0.0f, 0.0f, 0.0f);

                                cdPlayer.ShinobiSigilCooldown = 120;
                            }
                        }
                    }
                }
                else
                    thoriumPlayer.accShinobiSigilCrit = 0;
            }
        }
    }

    [JITWhenModsEnabled("RagnarokMod")]
    [ExtendsFromMod("RagnarokMod")]
    public static class GetRagnarokModPlayerField
    {
        public static bool RagnarokWhiteDwarfActive(Player player) => player.GetRagnarokModPlayer().WhiteDwarf;

        public static void SetWhiteDwarf(Player player, bool value)
        {
            player.GetRagnarokModPlayer().WhiteDwarf = value;
        }
    }
}
