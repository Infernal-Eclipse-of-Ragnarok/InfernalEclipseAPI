using System.Reflection;
using System.Security.Policy;
using Consolaria.Content.Items.Armor.Misc;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using InfernalEclipseAPI.Core.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace InfernalEclipseAPI.Core.Players
{
    [JITWhenModsEnabled("Consolaria")]
    [ExtendsFromMod("Consolaria")]
    public class ConsolariaPlayer : ModPlayer
    {
        private bool wasOnGround = true;

        public int hopsLeft = 5;
        public int hopCooldown;
        public override void PreUpdate()
        {
            if (hopCooldown > 0)
                hopCooldown--;
            if (hopCooldown == 0)
            {
                hopsLeft = 5;
                hopCooldown = 60 * 3;
            }
        }

        public override void PostUpdateEquips()
        {
            Type ostaraType = InfernalCrossmod.Consolaria.Mod.Code.GetType("Consolaria.Content.Items.Armor.Misc.OstarasPlayer");
            if (ostaraType == null)
                return;

            MethodInfo getModPlayerMethod = typeof(Player).GetMethod("GetModPlayer", Type.EmptyTypes);
            if (getModPlayerMethod == null)
                return;

            MethodInfo generic = getModPlayerMethod.MakeGenericMethod(ostaraType);
            object ostaraPlayer = generic.Invoke(Player, null);
            if (ostaraPlayer == null)
                return;

            FieldInfo bunnyHopField = ostaraType.GetField("bunnyHop", BindingFlags.Instance | BindingFlags.Public);
            if (bunnyHopField == null)
                return;

            bool hasOstaraBonus = (bool)bunnyHopField.GetValue(ostaraPlayer);
            if (!hasOstaraBonus)
            {
                wasOnGround = Player.velocity.Y == 0;
                return;
            }

            // Apply Frog Leg auto-jump behavior
            Player.autoJump = true;

            // Detect jump start
            bool onGround = Player.velocity.Y == 0;

            if (wasOnGround && !onGround && Player.velocity.Y < 0 && InfernalUtilities.HasArmorSet(Player, ModContent.ItemType<OstaraHat>(), ModContent.ItemType<OstaraJacket>(), ModContent.ItemType<OstaraBoots>()) && hopsLeft > 0) // just left the ground & has armor set
            {
                FirePentagonProjectiles(InfernalCrossmod.Consolaria.Mod);
                hopsLeft--;
                if (hopsLeft == 0)
                {
                    hopCooldown = 60 * 3;
                } 
            }


            wasOnGround = onGround;
        }

        private void FirePentagonProjectiles(Mod consolaria)
        {
            if (!consolaria.TryFind("TurkorKnife", out ModProjectile knifeProj))
                return;

            int projType = knifeProj.Type;

            // Spawn 5 projectiles evenly spaced in a pentagon pattern
            const int numProjectiles = 5;
            const float speed = 7.5f; // adjust projectile speed as needed
            Vector2 center = Player.Center;

            for (int i = 0; i < numProjectiles; i++)
            {
                float angle = MathHelper.TwoPi * i / numProjectiles - MathHelper.PiOver2;
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

                int projID = Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    center,
                    velocity,
                    projType,
                    20,
                    2f,
                    Player.whoAmI
                );

                // Grab the projectile instance and modify it
                Projectile proj = Main.projectile[projID];
                if (proj != null)
                {
                    proj.friendly = true;
                    proj.hostile = false;
                    proj.penetrate = 2;
                    proj.usesLocalNPCImmunity = true;
                    proj.localNPCHitCooldown = 40;

                    float hitboxScale = 2.25f;
                    proj.width = (int)(proj.width * hitboxScale);
                    proj.height = (int)(proj.height * hitboxScale);
                }
            }

            SoundEngine.PlaySound(SoundID.Item71, Player.position);
        }
    }
}
