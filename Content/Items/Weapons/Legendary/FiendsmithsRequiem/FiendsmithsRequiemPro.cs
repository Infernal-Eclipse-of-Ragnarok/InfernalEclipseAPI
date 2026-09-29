using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using InfernalEclipseAPI.Content.Buffs;
using InfernalEclipseAPI.Core.Configs;
using InfernalEclipseAPI.Core.DamageClasses.LegendaryClass;
using InfernalEclipseAPI.Core.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace InfernalEclipseAPI.Content.Items.Weapons.Legendary.FiendsmithsRequiem
{
    public class FiendsmithsRequiemPro : ModProjectile
    {
        public static float TargetRange = DownedBossSystem.downedProvidence ? 1250f : DownedBossSystem.downedAstrumDeus ? 1000f : 750f;

        public int AttackState;
        public int TargetIndex = -1;

        public NPC CurrentTarget
        {
            get
            {
                if (!Main.npc.IndexInRange(TargetIndex))
                    return null;

                NPC npc = Main.npc[TargetIndex];

                if (!npc.active || !npc.CanBeChasedBy())
                    return null;

                return npc;
            }
        }

        private const int LightningBarrageDuration = 180; // 3 seconds.
        private const int LightningShootRate = 6;

        private int lightningBarrageTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
            Projectile.penetrate = -1;

            Projectile.DamageType = LegendarySummon.Instance;

            Projectile.width = 104;
            Projectile.height = 128;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;

            Projectile.timeLeft = 8;
        }

        public override bool? CanDamage() => false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackState);
            writer.Write(TargetIndex);
            writer.Write(lightningBarrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackState = reader.ReadInt32();
            TargetIndex = reader.ReadInt32();
            lightningBarrageTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (!player.HasBuff<FiendsmithsRequiemBuff>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            if (player.GetModPlayer<InfernalPlayer>().fiendsmithParadise)
                Projectile.minionSlots = 4f;
            else
                Projectile.minionSlots = 2f;

            EnsureChainsawsExist();

            Vector2 followPosition = player.Center;

            followPosition.X -= (15f + player.width * 0.5f) * player.direction;

            followPosition.Y -= 25f;

            NPC target = FindTarget(player);

            int oldAttackState = AttackState;
            int oldTargetIndex = TargetIndex;

            if (lightningBarrageTimer > 0)
            {
                DoLightningBarrage(player, target);
                lightningBarrageTimer--;
                return;
            }

            if (target != null && (player.HeldItem.CountsAsClass<SummonDamageClass>() || player.HeldItem.CountsAsClass<AverageDamageClass>()) && player.ItemAnimationActive)
            {
                AttackState = 1;
                TargetIndex = target.whoAmI;

                AttackTarget(player, target);
            }
            else
            {
                AttackState = 0;
                TargetIndex = -1;

                IdleMovement(player, followPosition);
            }

            if (oldAttackState != AttackState || oldTargetIndex != TargetIndex)
            {
                Projectile.netUpdate = true;
            }

            if (Projectile.Distance(player.Center) > 2000f)
            {
                Projectile.Center = followPosition;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
        }

        private void IdleMovement(Player player, Vector2 followPosition)
        {
            float bobOffset = Sin(Main.GlobalTimeWrappedHourly * 3f) * 8f;

            Vector2 idlePosition = followPosition;

            idlePosition.Y += bobOffset;

            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePosition, 0.05f);

            Projectile.velocity *= 0.5f;

            Projectile.direction = Projectile.spriteDirection = player.direction;
        }

        private void AttackTarget(Player player, NPC target)
        {
            int direction = (target.Center - player.Center).X > 0f ? 1 : -1;

            const float behindDistance = -200f;

            float bobOffset = Sin(Main.GlobalTimeWrappedHourly * 3f) * 3f;

            Vector2 desiredPosition = target.Center + new Vector2(direction * (target.width * 0.5f + behindDistance), bobOffset);

            Vector2 moveDirection = desiredPosition - Projectile.Center;

            float distance = moveDirection.Length();

            if (distance > 4f)
            {
                // Move quickly enough to actually reach distant targets.
                float speed = Clamp(distance * 0.15f, 12f, 32f);

                Vector2 movement = moveDirection.SafeNormalize(Vector2.Zero) * speed;

                // Prevent overshooting once we're close.
                if (movement.LengthSquared() > moveDirection.LengthSquared())
                {
                    movement = moveDirection;
                }

                Projectile.Center += movement;
            }
            else
            {
                Projectile.Center = desiredPosition;
            }

            Projectile.velocity = Vector2.Zero;

            // Teleport if absurdly far away.
            if (distance > 1200f)
            {
                Projectile.Center = desiredPosition;
            }

            // Requiem faces back toward the target.
            Projectile.direction = Projectile.spriteDirection = -direction;
        }

        public void StartLightningBarrage()
        {
            if (lightningBarrageTimer > 0)
                return;

            lightningBarrageTimer = LightningBarrageDuration;
            Projectile.netUpdate = true;
        }

        private void DoLightningBarrage(Player player, NPC target)
        {
            // Put the coffin in front of the player.
            Vector2 desiredPosition =
                player.Center +
                new Vector2(player.direction * 100f, -25f);

            float bobOffset =
                Sin(Main.GlobalTimeWrappedHourly * 4f) * 5f;

            desiredPosition.Y += bobOffset;

            Projectile.Center =
                Vector2.Lerp(
                    Projectile.Center,
                    desiredPosition,
                    0.15f
                );

            Projectile.velocity = Vector2.Zero;

            // Face toward the target when one exists.
            if (target != null)
            {
                Projectile.direction =
                    Projectile.spriteDirection =
                    target.Center.X >= Projectile.Center.X
                        ? 1
                        : -1;
            }
            else
            {
                Projectile.direction =
                    Projectile.spriteDirection =
                    player.direction;
            }

            // Fire throughout the 3-second barrage.
            if (lightningBarrageTimer % LightningShootRate != 0)
                return;

            if (target == null)
                return;

            if (Projectile.owner != Main.myPlayer)
                return;

            Vector2 targetPosition =
                Main.rand.NextVector2FromRectangle(target.Hitbox);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FiendsmithsParadise>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner,
                targetPosition.X,
                targetPosition.Y
            );

            SoundStyle lightning = SoundID.DD2_LightningBugZap with
            {
                MaxInstances = 0,
                PitchVariance = 0.1f
            };

            SoundEngine.PlaySound(
                lightning.WithPitchOffset(1f),
                Projectile.Center
            );
        }

        private void EnsureChainsawsExist()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            bool hasLeftChainsaw = false;
            bool hasRightChainsaw = false;

            int chainsawType = ModContent.ProjectileType<FiendsmithChainsaw>();

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner != Projectile.owner)
                    continue;

                if (projectile.type != chainsawType)
                    continue;

                // ai[0] contains this Requiem's whoAmI.
                if ((int)projectile.ai[0] != Projectile.whoAmI)
                    continue;

                if (projectile.ai[1] < 0f)
                    hasLeftChainsaw = true;

                else if (projectile.ai[1] > 0f)
                    hasRightChainsaw = true;
            }

            if (!hasLeftChainsaw)
                SpawnChainsaw(-1f);

            if (!hasRightChainsaw)
                SpawnChainsaw(1f);
        }

        private void SpawnChainsaw(float side)
        {
            float spawnOffset = Projectile.width * 0.5f;

            Vector2 spawnPosition = Projectile.Center + Vector2.UnitX * side * spawnOffset;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<FiendsmithChainsaw>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, side);
        }

        private NPC FindTarget(Player player)
        {
            NPC target = null;

            float closestDistance = TargetRange;

            // Respect manually selected minion targets first.
            if (player.HasMinionAttackTargetNPC)
            {
                NPC manualTarget = Main.npc[player.MinionAttackTargetNPC];

                if (manualTarget.CanBeChasedBy(Projectile))
                {
                    float distance = Vector2.Distance(player.Center, manualTarget.Center);

                    if (distance <= TargetRange)
                        return manualTarget;
                }
            }

            // Otherwise find the closest valid NPC.
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile))
                    continue;

                float distance = Vector2.Distance(player.Center, npc.Center);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = npc;
                }
            }

            return target;
        }
    }

    public class FiendsmithChainsaw : ModProjectile
    {
        private const float SegmentOverlap = 4f;

        // Duration of one individual arm swing.
        private const int SwingDuration = 24;

        // Swing extension/retraction timing.
        private const float ExtensionStart = 0.08f;
        private const float ExtensionFull = 0.28f;
        private const float RetractionStart = 0.72f;
        private const float RetractionEnd = 0.95f;

        // Size of the whip arc.
        private const float SwingArc = 1.15f;

        // Amount the joint bends during the swing.
        private const float ElbowBend = 42f;

        // Each arm attaches to a different side of Requiem.
        private const float AttachmentXFactor = 0.32f;
        private const float AttachmentYOffset = 4f;

        // Thickness of the damaging arm segments.
        private const float SegmentCollisionWidth = 16f;

        private const int MiddleSegmentCount = 2;

        // Both arms run the same 48-tick cycle.
        // 0-23  = left arm
        // 24-47 = right arm
        private int swingTimer;

        private bool currentlySwinging;

        // 0 = hidden.
        // 1 = fully deployed.
        private float extensionProgress;

        // Used for drawing.
        private Vector2 baseCenter;
        private Vector2 middleCenter;
        private Vector2 middleCenter2;

        private float baseRotation;
        private float middleRotation;

        private float baseDrawScale;
        private float middleDrawScale;

        // Used for collision.
        // attachment -> elbow -> chainsaw
        private Vector2 armAttachmentPosition;
        private Vector2 armElbowPosition;

        // ai[0] = parent Requiem whoAmI.
        private int ParentIndex => (int)Projectile.ai[0];

        // ai[1]:
        // -1 = left arm
        // +1 = right arm
        private float Side => Projectile.ai[1];

        private bool IsLeftArm => Side < 0f;

        private Projectile Parent
        {
            get
            {
                if (!Main.projectile.IndexInRange(ParentIndex))
                    return null;

                Projectile parent = Main.projectile[ParentIndex];

                if (!parent.active || parent.owner != Projectile.owner || parent.type != ModContent.ProjectileType<FiendsmithsRequiemPro>())
                {
                    return null;
                }

                return parent;
            }
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = LegendarySummon.Instance;

            Projectile.netImportant = true;

            Projectile.width = 64;
            Projectile.height = 32;

            Projectile.timeLeft = 2;

            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;

            // The entire articulated arm is one projectile, so the blade and both segments share the same hit cooldown.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanDamage() => currentlySwinging && extensionProgress >= 0.50f ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!currentlySwinging || extensionProgress < 0.50f)
            {
                return false;
            }

            float collisionWidth = SegmentCollisionWidth * Projectile.scale;

            float collisionPoint = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), armAttachmentPosition, armElbowPosition, collisionWidth, ref collisionPoint))
            {
                return true;
            }

            collisionPoint = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), armElbowPosition, Projectile.Center, collisionWidth, ref collisionPoint))
            {
                return true;
            }

            return projHitbox.Intersects(targetHitbox);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(swingTimer);
            writer.Write(currentlySwinging);
            writer.Write(extensionProgress);
            writer.Write(baseRotation);
            writer.Write(middleRotation);
            writer.Write(baseDrawScale);
            writer.Write(middleDrawScale);
            writer.Write(armAttachmentPosition.X);
            writer.Write(armAttachmentPosition.Y);
            writer.Write(armElbowPosition.X);
            writer.Write(armElbowPosition.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swingTimer = reader.ReadInt32();
            currentlySwinging = reader.ReadBoolean();
            extensionProgress = reader.ReadSingle();
            baseRotation = reader.ReadSingle();
            middleRotation = reader.ReadSingle();
            baseDrawScale = reader.ReadSingle();
            middleDrawScale = reader.ReadSingle();
            armAttachmentPosition.X = reader.ReadSingle();
            armAttachmentPosition.Y = reader.ReadSingle();
            armElbowPosition.X = reader.ReadSingle();
            armElbowPosition.Y = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile parent = Parent;

            if (parent == null)
            {
                Projectile.Kill();
                return;
            }

            if (parent.ModProjectile is not FiendsmithsRequiemPro requiem)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.velocity = Vector2.Zero;

            NPC target = requiem.CurrentTarget;

            Texture2D baseTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Weapons/Legendary/FiendsmithsRequiem/ChainsawSegmentBase").Value;

            Texture2D middleTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Weapons/Legendary/FiendsmithsRequiem/ChainsawSegmentMiddle").Value;

            Texture2D bladeTexture = TextureAssets.Projectile[Type].Value;

            float baseLength = baseTexture.Width * Projectile.scale - SegmentOverlap;

            float middleLength = middleTexture.Width * Projectile.scale - SegmentOverlap;

            float bladeHalfLength = bladeTexture.Width * Projectile.scale * 0.5f;

            // Each arm gets its own physical attachment point.
            Vector2 attachmentPosition = GetAttachmentPosition(parent);

            bool attacking = requiem.AttackState == 1 && target != null;

            if (attacking)
            {
                // Immediately start/restart the cycle.
                if (swingTimer <= 0)
                {
                    swingTimer = SwingDuration * 2;
                }

                UpdateCombo(parent, target, attachmentPosition, baseLength, middleLength, bladeHalfLength);

                swingTimer--;

                // Continue indefinitely LEFT -> RIGHT -> LEFT -> RIGHT until the parent leaves attack state.
                if (swingTimer <= 0)
                {
                    swingTimer =
                        SwingDuration * 2;
                }
            }
            else
            {
                swingTimer = 0;

                currentlySwinging = false;
                extensionProgress = 0f;

                HideInsideParent(attachmentPosition);
            }

            Projectile.rotation = middleRotation;

            Vector2 bladeDirection = middleRotation.ToRotationVector2();

            Projectile.spriteDirection = bladeDirection.X >= 0f ? 1 : -1;
        }

        private void UpdateCombo(Projectile parent, NPC target, Vector2 attachmentPosition, float fullBaseLength, float fullMiddleLength, float bladeHalfLength)
        {
            int elapsed = SwingDuration * 2 - swingTimer;

            // First half = left arm.
            // Second half = right arm.
            bool leftArmTurn = elapsed < SwingDuration;

            currentlySwinging = IsLeftArm ? leftArmTurn : !leftArmTurn;

            if (!currentlySwinging)
            {
                extensionProgress = 0f;

                HideInsideParent(attachmentPosition);

                return;
            }

            int localTimer = leftArmTurn ? elapsed : elapsed - SwingDuration;

            if (localTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, attachmentPosition);
            }

            float swingCompletion = Clamp(localTimer / (float)SwingDuration, 0f, 1f);

            PerformWhipSwing(parent, target, attachmentPosition, fullBaseLength, fullMiddleLength, bladeHalfLength, swingCompletion);
        }

        private void PerformWhipSwing(Projectile parent, NPC target, Vector2 attachmentPosition, float fullBaseLength, float fullMiddleLength, float bladeHalfLength, float swingCompletion)
        {
            float extend = Utils.GetLerpValue(ExtensionStart, ExtensionFull, swingCompletion, true);
            float retract = Utils.GetLerpValue(RetractionEnd, RetractionStart, swingCompletion, true);

            extensionProgress = MathF.Min(extend, retract);

            float smoothExtension = SmoothStep(0f, 1f, extensionProgress);
            Vector2 directionToTarget = (target.Center - attachmentPosition).SafeNormalize(new Vector2(parent.spriteDirection, 0f));
            float targetRotation = directionToTarget.ToRotation();
            float easedSwing = SmoothStep(0f, 1f, swingCompletion);
            bool parentBelowTarget = parent.Center.Y > target.Center.Y;
            float comboDirection = IsLeftArm ? 1f : -1f;

            float startAngle = SwingArc * comboDirection;
            float endAngle = -SwingArc * comboDirection;
            float swingOffset = Lerp(startAngle, endAngle, easedSwing);
            float swingRotation = targetRotation + swingOffset;
            Vector2 swingDirection = swingRotation.ToRotationVector2();

            float totalReach = (fullBaseLength + fullMiddleLength * MiddleSegmentCount + bladeHalfLength - SegmentOverlap * 3f) * smoothExtension;
            Vector2 bladePosition =  attachmentPosition + swingDirection * totalReach;

            // Strongest bend at the middle of the swing.
            float bendEnvelope = Sin(swingCompletion * Pi);

            // Use the same direction multiplier as the swing.
            // This is important because when the combo reverses due to Requiem being underneath the target, the elbow curvature needs to reverse with it.
            float bendDirection = comboDirection;

            Vector2 perpendicular = swingDirection.RotatedBy(PiOver2);
            Vector2 elbowPosition = Vector2.Lerp(attachmentPosition, bladePosition, 0.5f) + perpendicular * ElbowBend * bendEnvelope * bendDirection * smoothExtension;

            // Damage collision follows: attachment -> elbow -> chainsaw
            armAttachmentPosition = attachmentPosition;
            armElbowPosition = elbowPosition;

            Vector2 baseDirection = (elbowPosition - attachmentPosition).SafeNormalize(swingDirection);
            float currentBaseLength = Vector2.Distance(attachmentPosition, elbowPosition);

            baseRotation = baseDirection.ToRotation();
            baseCenter = attachmentPosition + baseDirection * (currentBaseLength * 0.5f);
            baseDrawScale = fullBaseLength > 0f ? currentBaseLength / fullBaseLength : 0f;

            Vector2 middleDirection =
                (bladePosition - elbowPosition)
                .SafeNormalize(swingDirection);

            float currentMiddleSpan =
                Vector2.Distance(
                    elbowPosition,
                    bladePosition
                );

            // Divide the middle portion into two equal physical segments.
            float currentMiddleLength =
                currentMiddleSpan * 0.5f;

            Vector2 middleJoint =
                elbowPosition +
                middleDirection *
                currentMiddleLength;

            middleRotation =
                middleDirection.ToRotation();

            // First middle segment.
            middleCenter =
                elbowPosition +
                middleDirection *
                (currentMiddleLength * 0.5f);

            // Second middle segment.
            middleCenter2 =
                middleJoint +
                middleDirection *
                (currentMiddleLength * 0.5f);

            // Both are full-sized when the arm is fully extended.
            middleDrawScale =
                fullMiddleLength > 0f
                    ? currentMiddleLength / fullMiddleLength
                    : 0f;

            Projectile.Center = bladePosition;

            Projectile.rotation = middleRotation;

            Projectile.velocity = Vector2.Zero;
        }

        private void HideInsideParent(Vector2 attachmentPosition)
        {
            Projectile.Center = attachmentPosition;
            Projectile.velocity = Vector2.Zero;

            baseCenter = attachmentPosition;
            middleCenter = attachmentPosition;
            middleCenter2 = attachmentPosition;

            armAttachmentPosition = attachmentPosition;
            armElbowPosition = attachmentPosition;

            baseDrawScale = 0f;
            middleDrawScale = 0f;
        }

        private Vector2 GetAttachmentPosition(Projectile parent)
        {
            // -1 and +1 place the two arms on opposite physical sides of Requiem's body.
            float horizontalOffset = parent.width * AttachmentXFactor;

            return parent.Center + new Vector2(Side * horizontalOffset, AttachmentYOffset);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (DownedBossSystem.downedProvidence)
            {
                target.AddBuff(ModContent.BuffType<Laceration>(), 60 * 3);
            }
            else if (DownedBossSystem.downedRavager)
            {
                target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 60 * 3);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (extensionProgress <= 0.001f)
                return false;

            if (Parent == null)
                return false;

            Texture2D baseTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Weapons/Legendary/FiendsmithsRequiem/ChainsawSegmentBase").Value;
            Texture2D middleTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Weapons/Legendary/FiendsmithsRequiem/ChainsawSegmentMiddle").Value;
            Texture2D bladeTexture = TextureAssets.Projectile[Type].Value;

            Vector2 baseScale = new Vector2(MathF.Max(baseDrawScale, 0f), 1f) * Projectile.scale;
            Vector2 middleScale = new Vector2(MathF.Max(middleDrawScale, 0f), 1f) * Projectile.scale;
            Vector2 baseDirection = baseRotation.ToRotationVector2();
            Vector2 middleDirection = middleRotation.ToRotationVector2();

            SpriteEffects baseEffects = baseDirection.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;
            SpriteEffects middleEffects = middleDirection.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;
            SpriteEffects bladeEffects = middleDirection.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

            if (baseDrawScale > 0.001f)
            {
                Main.EntitySpriteDraw(baseTexture, baseCenter - Main.screenPosition, null, Projectile.GetAlpha(Lighting.GetColor(baseCenter.ToTileCoordinates())), baseRotation, baseTexture.Size() * 0.5f, baseScale, baseEffects, 0f);
            }

            if (middleDrawScale > 0.001f)
            {
                // Middle segment #1.
                Main.EntitySpriteDraw(
                    middleTexture,
                    middleCenter - Main.screenPosition,
                    null,
                    Projectile.GetAlpha(
                        Lighting.GetColor(
                            middleCenter.ToTileCoordinates()
                        )
                    ),
                    middleRotation,
                    middleTexture.Size() * 0.5f,
                    middleScale,
                    middleEffects,
                    0f
                );

                // Middle segment #2.
                Main.EntitySpriteDraw(
                    middleTexture,
                    middleCenter2 - Main.screenPosition,
                    null,
                    Projectile.GetAlpha(
                        Lighting.GetColor(
                            middleCenter2.ToTileCoordinates()
                        )
                    ),
                    middleRotation,
                    middleTexture.Size() * 0.5f,
                    middleScale,
                    middleEffects,
                    0f
                );
            }

            float bladeProgress = Utils.GetLerpValue(0f, 0.25f, extensionProgress, true);

            if (bladeProgress > 0f)
            {
                Main.EntitySpriteDraw(bladeTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Lighting.GetColor( Projectile.Center.ToTileCoordinates())) * bladeProgress, Projectile.rotation, bladeTexture.Size() * 0.5f, Projectile.scale * bladeProgress, bladeEffects, 0f);
            }

            return false;
        }
    }
}