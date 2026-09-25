using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.StellarSabre
{
    public class StellarStar : ModProjectile
    {
        public override string GlowTexture => "InfernalEclipseAPI/Assets/Glow";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = LegendaryMelee.Instance;
            Projectile.penetrate = (NPC.downedMoonlord ? 4 : NPC.downedGolemBoss ? 2 : NPC.downedPlantBoss ? 1 : -1);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[2] == -1)
                Projectile.penetrate = 2;
        }

        public override void AI()
        {
            const float PassThroughRadius = 40f;   // distance from target to count as "inside"
            const int PassThroughFrames = 8;       // frames to keep flying straight after passing

            // Acquire target once, based on closest NPC to mouse within range
            if (Projectile.ai[1] == 0f && Main.myPlayer == Projectile.owner)
            {
                float maxTargetDistance = 960f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(this) && npc.Distance(Main.MouseWorld) < maxTargetDistance)
                    {
                        maxTargetDistance = npc.Distance(Main.MouseWorld);
                        Projectile.ai[1] = i + 1; // store index+1 so 0 means "no target"
                    }
                }

                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.localAI[0] = 0f; // phase
                    Projectile.localAI[1] = 0f; // previous distance
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                }
            }
            // Home towards stored target if Golem has been defeated.
            else if (Projectile.ai[1] > 0f && NPC.downedGolemBoss && Projectile.ai[2] != -1)
            {
                int targetIndex = (int)Projectile.ai[1] - 1;
                if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = 0f;
                    Projectile.localAI[0] = 0f;
                    Projectile.localAI[1] = 0f;
                }
                else
                {
                    NPC target = Main.npc[targetIndex];

                    if (!target.CanBeChasedBy(this))
                    {
                        Projectile.ai[0] = 0f;
                        Projectile.ai[1] = 0f;
                        Projectile.localAI[0] = 0f;
                        Projectile.localAI[1] = 0f;
                    }
                    else
                    {
                        int phase = (int)Projectile.localAI[0]; // 0 = homing, 1 = pass-through
                        Vector2 toTarget = target.Center - Projectile.Center;
                        float distance = toTarget.Length();
                        float prevDistance = Projectile.localAI[1];

                        if (phase == 0)
                        {
                            // Homing
                            Projectile.ai[0]++;

                            Vector2 currentDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                            Vector2 targetDir = toTarget.SafeNormalize(Vector2.UnitX);

                            // Slightly faster curve after the first few hits: decrease homingTime if you want
                            float homingTime = 90f;
                            float lerpFactor = MathHelper.Min(Projectile.ai[0] / homingTime, 1f);

                            Vector2 newDir = Vector2.Normalize(Vector2.Lerp(currentDir, targetDir, lerpFactor));
                            float speed = Projectile.velocity.Length();
                            Projectile.velocity = newDir * speed;

                            // Detect passing through: we were very close, now moving away again
                            if (prevDistance > 0f && prevDistance < PassThroughRadius && distance > prevDistance)
                            {
                                phase = 1;
                                Projectile.localAI[0] = 1f; // go into pass-through
                                Projectile.ai[0] = 0f;      // reuse as timer
                            }
                        }
                        else if (phase == 1)
                        {
                            // Pass-through: keep going straight for a short duration
                            Projectile.ai[0]++;

                            if (Projectile.ai[0] >= PassThroughFrames)
                            {
                                // Back to homing; this lets the behaviour repeat indefinitely
                                phase = 0;
                                Projectile.localAI[0] = 0f;
                                Projectile.ai[0] = 0f;
                            }
                        }

                        // Store distance so next frame can detect crossing
                        Projectile.localAI[1] = distance;
                    }
                }
            }

            // Dust type (fallback to vanilla dust if CatalystMod is missing)
            int dustType = 6;
            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
                dustType = catalyst.Find<ModDust>("MonoDust2").Type;

            if (Projectile.timeLeft % Projectile.MaxUpdates == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (!Main.rand.NextBool(3))
                    {
                        float velocityFactor = i * 0.5f;

                        Vector2 dustPos =
                            Projectile.Center +
                            Main.rand.NextVector2Circular(Projectile.width / 2f, Projectile.height / 2f) +
                            Projectile.velocity * velocityFactor;

                        Dust dust = Dust.NewDustPerfect(dustPos, dustType, Projectile.velocity * velocityFactor);
                        dust.noGravity = true;
                        dust.velocity *= 0.1f;
                        dust.scale = 0.9f;
                        dust.fadeIn = 0.1f;
                        dust.alpha = 100;
                        dust.color = !Main.rand.NextBool(3)
                            ? new Color(255, 233, 2, 50)
                            : new Color(220, 95, 210, 50);

                        if (Projectile.oldVelocity != Vector2.Zero)
                            dust.velocity -= Projectile.oldVelocity.SafeNormalize(Vector2.UnitX);
                    }
                }
            }

            Projectile.rotation += Projectile.velocity.X < 0f ? -1f : 1f;
        }

        private bool hasSplit = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Astral Infection (Mechs+)
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120); // 2 seconds
        }

        public override void OnKill(int timeLeft)
        {
            // SPLIT ON DEATH (Plantera+)
            if (NPC.downedPlantBoss && Projectile.owner == Main.myPlayer && Projectile.ai[2] != -1 & !hasSplit)
            {
                int numSplits = NPC.downedMoonlord ? 3 : 2;
                float spread = MathHelper.ToRadians(5);

                for (int i = 0; i < numSplits; i++)
                {
                    float rotation = MathHelper.Lerp(-spread, spread, (float)i / (numSplits - 1));
                    Vector2 newVelocity = Projectile.velocity.RotatedBy(rotation) * 0.85f;
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        newVelocity,
                        Type,
                        Projectile.damage / 2,
                        Projectile.knockBack * 0.8f,
                        Projectile.owner,
                        ai2: -1
                    );
                }

                hasSplit = true;
            }

            // EXPLODE ON DEATH (Cultist+)
            if (NPC.downedAncientCultist)
            {
                int explosionRadius = 48;
                int explosionDamage = Projectile.damage / 2;
                float explosionKnockback = Projectile.knockBack;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < explosionRadius)
                    {
                        NPC.HitInfo explosionInfo = new NPC.HitInfo()
                        {
                            Damage = explosionDamage,
                            Knockback = explosionKnockback,
                            HitDirection = npc.Center.X > Projectile.Center.X ? 1 : -1,
                            Crit = false
                        };
                        npc.StrikeNPC(explosionInfo, false); // The second param is fromNet, set to false for local
                    }
                }

                // Visual: fireworks dust
                for (int k = 0; k < 16; k++)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworkFountain_Yellow);
            }

            /*
            // STARFALL (Moon Lord+)
            if (NPC.downedMoonlord && Projectile.owner == Main.myPlayer)
            {
                Vector2 fallVelocity = new Vector2(0, 14f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    new Vector2(Projectile.Center.X, Projectile.Center.Y - 300),
                    fallVelocity,
                    ProjectileID.Starfury,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
            */
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = MathHelper.Min(15f, Projectile.timeLeft) / 15f;

            Texture2D glowTex = ModContent.Request<Texture2D>(GlowTexture, AssetRequestMode.ImmediateLoad).Value;
            Vector2 glowOrigin = glowTex.Size() / 2f;

            int trailLength = Projectile.oldPos.Length;
            for (int i = 0; i < trailLength; i++)
            {
                float t = 1f - i / (float)trailLength;

                Color trailColor = new Color(100, 33, 100, 0) * t * fade;
                float trailScale = Projectile.scale * t * fade;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                Main.spriteBatch.Draw(
                    glowTex,
                    trailPos,
                    null,
                    trailColor,
                    Projectile.rotation,
                    glowOrigin,
                    trailScale,
                    SpriteEffects.None,
                    0f
                );
            }

            Texture2D mainTex = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            Vector2 mainOrigin = mainTex.Size() / 2f;

            lightColor = new Color(255, 233, 2, 0) * fade;

            Main.EntitySpriteDraw(
                mainTex,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                mainOrigin,
                Projectile.scale * fade,
                SpriteEffects.None,
                0f
            );
            return false;
        }
    }
}
