using CalamityMod;
using InfernalEclipseAPI.Common.GlobalNPCs.NPCDebuffs;
using InfernalEclipseAPI.Core.Systems;
using InfernalEclipseAPI.Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace InfernalEclipseAPI.Content.Items.Other
{
    public class EclipseHeadphonesProj : ModProjectile
    {
        public class MusicUIIcon
        {
            public bool RequiresWoTG
            {
                get;
                set;
            }

            public bool RequiresYou
            {
                get;
                set;
            }

            public bool RequiresThorium
            {
                get;
                set;
            }

            public string HoverText
            {
                get;
                set;
            }

            public float Scale
            {
                get;
                set;
            } = 1f;

            public string TrackName
            {
                get;
                set;
            }

            public Asset<Texture2D> BossIconTexture
            {
                get;
                set;
            }

            public Func<Color> HoverTextColor
            {
                get;
                set;
            }

            public Func<bool> UnlockCondition
            {
                get;
                set;
            }

            public bool Draw(Player player, Vector2 center, float indexRatio, float opacity, out Vector2 textDrawPosition, out Color textColor, out string text)
            {
                Texture2D background = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Other/HeadphoneIconBackground").Value;
                Texture2D icon = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Other/MusicIcons").Value;

                // Acquire drawing information.
                bool unlockedTrack = UnlockCondition() || player.GetModPlayer<CustomMusicPlayer>().UnlockAllMusic;
                float scale = Scale * opacity * 0.8f;
                float indexAngle = MathHelper.TwoPi * indexRatio - MathHelper.PiOver2;
                Vector2 drawPosition = center + indexAngle.ToRotationVector2() * 155f;

                Vector2 backgroundOrigin = background.Size() * 0.5f;
                Rectangle iconFrame = icon.Frame(1, 3, 0, (int)(indexRatio * 10f) % 3);
                Vector2 iconOrigin = iconFrame.Size() * 0.5f;
                Color iconColor = Main.hslToRgb((indexRatio + Main.GlobalTimeWrappedHourly * 0.33f) % 1f, 1f, 0.5f);
                Rectangle drawArea = Utils.CenteredRectangle(drawPosition, background.Size() * scale);

                // Determine if the mouse is hovering over the icon.
                // If it is, it should display the hover text and increase in size.
                bool hoveringOverBackground = drawArea.Contains(Main.MouseScreen.ToPoint());
                Scale = MathHelper.Clamp(Scale + hoveringOverBackground.ToDirectionInt() * 0.04f, 1f, 1.2f);

                // Draw the icon.
                Main.spriteBatch.Draw(background, drawPosition, null, Color.White * opacity, 0f, backgroundOrigin, scale, 0, 0f);
                Vector2 headIconScale = Vector2.One * 30f / BossIconTexture.Value.Size() * scale;
                Main.spriteBatch.Draw(BossIconTexture.Value, drawPosition, null, (unlockedTrack ? Color.White : Color.Black) * opacity, 0f, BossIconTexture.Value.Size() * 0.5f, headIconScale, 0, 0f);

                if (unlockedTrack)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    Main.spriteBatch.Draw(icon, drawPosition - indexAngle.ToRotationVector2() * 16f, iconFrame, iconColor * opacity, 0f, iconOrigin, scale, 0, 0f);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                }

                // Draw the text above the icon.
                text = string.Empty;
                textColor = Color.White;
                textDrawPosition = Vector2.Zero;
                if (hoveringOverBackground && opacity > 0f)
                {
                    text = HoverText;
                    textColor = HoverTextColor();
                    if (!unlockedTrack)
                        textColor = Color.Gray;

                    textDrawPosition = drawPosition + Vector2.UnitY * 44f;
                }

                // Handle click behaviors.
                bool clicked = opacity > 0f && Main.mouseLeft && Main.mouseLeftRelease && hoveringOverBackground;
                if (hoveringOverBackground)
                    Main.blockMouse = true;

                if (clicked && unlockedTrack)
                {
                    if (Main.myPlayer == player.whoAmI && player.GetModPlayer<CustomMusicPlayer>().CurrentTrackName != TrackName)
                    {
                        LocalizedText chatText = Language.GetText("Mods.InfernalEclipseAPI.HeadphonesText.1").WithFormatArgs(HoverText.Split(" - ")[0]);
                        Main.NewText(chatText, HoverTextColor());
                        player.GetModPlayer<CustomMusicPlayer>().CurrentTrackName = TrackName;
                    }
                    return true;
                }
                return false;
            }
        }

        public enum HeadphonesMatrixState : byte
        {
            PutOn,
            TakeOff,
            HandleUIState
        }

        public bool IsRenderingUI
        {
            get;
            set;
        }

        public float UIOptionsOpacity
        {
            get;
            set;
        }

        public HeadphonesMatrixState CurrentState
        {
            get;
            set;
        }

        public Player Owner => Main.player[Projectile.owner];

        public List<MusicUIIcon> UIStates = new()
        {
            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.TitleScreen"),
                TrackName = "TeardropsofDragonfire",
                HoverTextColor = () => Color.Lerp(Color.White, new Color(255, 80, 0), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)),
                UnlockCondition = () => true,
                BossIconTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/icon_small")
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Seahorse"),
                TrackName = "Seahorse",
                HoverTextColor = () => Color.Lerp(Color.SandyBrown, Color.SaddleBrown, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.3f + 0.35f),
                UnlockCondition = () => InfernalWorld.cnidrionDowned,
                BossIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/IlmerisSpark")
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.ThunderBird"),
                TrackName = "InfernalThunderBird",
                HoverTextColor = () => new(205, 150, 255),
                UnlockCondition = () => InfernalCrossmod.Thorium.Loaded ? (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "TheGrandThunderBird") : false,
                BossIconTexture = InfernalCrossmod.Thorium.Loaded ? InfernalCrossmod.Thorium.Mod.Assets.Request<Texture2D>("NPCs/BossTheGrandThunderBird/TheGrandThunderBird_Head_Boss") : TextureAssets.MagicPixel,
                RequiresThorium = true
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.QueenJellyfish"),
                TrackName = "MutantMajesty",
                HoverTextColor = () => new(205, 150, 255),
                UnlockCondition = () => InfernalCrossmod.Thorium.Loaded ? (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "QueenJellyfish") : false,
                BossIconTexture = InfernalCrossmod.Thorium.Loaded ? InfernalCrossmod.Thorium.Mod.Assets.Request<Texture2D>("NPCs/BossQueenJellyfish/QueenJellyfish_Head_Boss") : TextureAssets.MagicPixel,
                RequiresThorium = true
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Viscount"),
                TrackName = "VampiricVitality",
                HoverTextColor = () => new(205, 150, 255),
                UnlockCondition = () => InfernalCrossmod.Thorium.Loaded ? (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "Viscount") : false,
                BossIconTexture = InfernalCrossmod.Thorium.Loaded ? InfernalCrossmod.Thorium.Mod.Assets.Request<Texture2D>("NPCs/BossViscount/Viscount_Head_Boss") : TextureAssets.MagicPixel,
                RequiresThorium = true
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.GraniteEnergyStorm"),
                TrackName = "Stonewarden",
                HoverTextColor = () => new(205, 150, 255),
                UnlockCondition = () => InfernalCrossmod.Thorium.Loaded ? (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "GraniteEnergyStorm") : false,
                BossIconTexture = InfernalCrossmod.Thorium.Loaded ? InfernalCrossmod.Thorium.Mod.Assets.Request<Texture2D>("NPCs/BossGraniteEnergyStorm/GraniteEnergyStorm_Head_Boss") : TextureAssets.MagicPixel,
                RequiresThorium = true
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.BereftVassal"),
                TrackName = "BereftVassal",
                HoverTextColor = () => Color.Lerp(Color.Cyan, Color.Yellow, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.3f + 0.35f),
                UnlockCondition = () => (bool)ModLoader.GetMod("InfernumMode").Call("CanPlaySoulHeadphonesMusic", "BereftVassal"),
                BossIconTexture = ModContent.Request<Texture2D>("InfernumMode/Content/BehaviorOverrides/BossAIs/GreatSandShark/BereftVassal_Head_Boss"),
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.ProvidenceNight"),
                TrackName = "ProvidenceNight",
                HoverTextColor = () => Color.Lerp(Color.Turquoise, new Color(255, 191, 73), (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.3f + 0.35f),
                UnlockCondition = () => InfernalWorld.providenceNightDowned,
                BossIconTexture = ModContent.Request<Texture2D>("InfernumMode/Content/Items/Accessories/Purity"),
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.You"),
                TrackName = "FINALFRACTAL",
                HoverTextColor = () =>  Color.Yellow,
                UnlockCondition = () => InfernalCrossmod.YouBoss.Loaded ? TerraBladeDebuff.DownedYou : false,
                BossIconTexture = TextureAssets.Item[ItemID.TerraBlade],
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.DoG"),
                TrackName = "LastBattle",
                HoverTextColor = () => Color.Lerp(Color.Cyan, Color.Fuchsia, (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)),
                UnlockCondition = () => DownedBossSystem.downedDoG,
                BossIconTexture = ModContent.Request<Texture2D>("InfernumMode/Content/BehaviorOverrides/BossAIs/DoG/DoGP2HeadMapIcon")
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Ragnarok"),
                TrackName = "RealitysEnd",
                HoverTextColor = () => new(205, 150, 255),
                UnlockCondition = () => InfernalCrossmod.Thorium.Loaded ? (bool)InfernalCrossmod.Thorium.Mod.Call("GetDownedBoss", "ThePrimordials") : false,
                BossIconTexture = InfernalCrossmod.Thorium.Loaded ? InfernalCrossmod.Thorium.Mod.Assets.Request<Texture2D>("NPCs/BossThePrimordials/DreamEater_Head_Boss") : TextureAssets.MagicPixel,
                RequiresThorium = true
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Arsenal"),
                TrackName = "Arsenal",
                HoverTextColor = () => new(155, 255, 255),
                UnlockCondition = () => InfernalWorld.codebreakerCompleted,
                BossIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/Vanity/DraedonMask")
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Interlude4"),
                TrackName = "Interlude04",
                HoverTextColor = () => Color.DarkRed,
                UnlockCondition = () => DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas,
                BossIconTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Other/Interlude4Icon")
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Tier5"),
                TrackName = "tier5",
                HoverTextColor = () => Color.DarkGoldenrod,
                UnlockCondition = () => InfernalWorld.tier5Downed,
                BossIconTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Other/Tier5Icon"),
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Tier6"),
                TrackName = "tier6",
                HoverTextColor = () => Color.DarkGoldenrod,
                UnlockCondition = () => InfernalWorld.tier6Downed,
                BossIconTexture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Other/Tier6Icon"),
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.TierNameless"),
                TrackName = "TWISTEDGARDENRemix",
                HoverTextColor = () => Color.White,
                UnlockCondition = () => DownedBossSystem.downedBossRush,
                BossIconTexture = !InfernalCrossmod.NoxusBoss.Loaded ? TextureAssets.MagicPixel : ModContent.Request<Texture2D>("NoxusBoss/Assets/Textures/Content/NPCs/Bosses/NamelessDeity/NamelessDeityBoss_Head_Boss"),
                RequiresWoTG = true
            },

            new()
            {
                HoverText = Language.GetTextValue($"Mods.InfernalEclipseAPI.BossName.Encore"),
                TrackName = "EnsembleofFools(EncoreMix)",
                HoverTextColor = () => Color.YellowGreen,
                UnlockCondition = () => DownedBossSystem.downedBossRush,
                BossIconTexture = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/BossRushIcon"),
            },
        };

        public ref float Time => ref Projectile.ai[0];

        public ref float DisappearCountdown => ref Projectile.ai[1];

        public override string Texture => "InfernalEclipseAPI/Content/Items/Other/SoulDrivenHeadphonesEclipse_Head";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 7200;
            Projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)CurrentState);
            writer.Write(Projectile.Opacity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentState = (HeadphonesMatrixState)reader.ReadByte();
            Projectile.Opacity = reader.ReadSingle();
        }

        public override void AI()
        {
            Item heldItem = Main.mouseItem.IsAir ? Owner.HeldItem : Main.mouseItem;

            // Die if no longer holding the click button or otherwise cannot use the item.
            bool shouldDie = !Owner.channel || Owner.dead || !Owner.active || Owner.noItems || Owner.CCed || heldItem is null;
            if (IsRenderingUI)
            {
                shouldDie = Owner.dead || !Owner.active || heldItem is null;
                if (DisappearCountdown > 0f)
                {
                    DisappearCountdown--;
                    if (DisappearCountdown <= 0f)
                        shouldDie = true;
                }
            }

            if (Main.myPlayer == Projectile.owner && shouldDie)
            {
                Projectile.Kill();
                return;
            }

            // Stick to the owner.
            Projectile.Center = Owner.MountedCenter;
            AdjustPlayerValues();

            switch (CurrentState)
            {
                // Put the headphones on.
                case HeadphonesMatrixState.PutOn:
                    if (Owner.GetModPlayer<CustomMusicPlayer>().UsingHeadphones)
                        CurrentState = HeadphonesMatrixState.TakeOff;
                    DoBehavior_PutOnHeadphones();
                    break;

                // Take the headphones off.
                case HeadphonesMatrixState.TakeOff:
                    DoBehavior_TakeOffHeadphones();
                    break;

                // Make the UI appear.
                case HeadphonesMatrixState.HandleUIState:
                    DoBehavior_HandleUIState();
                    break;
            }

            Time++;
        }

        public void DoBehavior_PutOnHeadphones()
        {
            // Make the owner put the headphones on.
            float animationCompletion = (float)Math.Pow(Utils.GetLerpValue(0f, 40f, Time, true), 0.56);

            // Update the player's arm directions to make it look as though they're holding the headphones.
            float frontArmRotation = Owner.direction * animationCompletion * -MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation - Owner.direction * 1.72f);
            Projectile.Center = Owner.Center + Vector2.UnitX.RotatedBy(frontArmRotation) * Owner.direction * 16f;

            if (animationCompletion >= 1f)
            {
                CurrentState = HeadphonesMatrixState.HandleUIState;
                Owner.SetCompositeArmFront(false, Player.CompositeArmStretchAmount.Full, 0f);
                Owner.GetModPlayer<CustomMusicPlayer>().UsingHeadphones = true;
                Projectile.Opacity = 0f;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_TakeOffHeadphones()
        {
            // Make the owner put the headphones on.
            float animationCompletion = (float)Math.Pow(Utils.GetLerpValue(0f, 55f, Time, true), 0.72);

            // Update the player's arm directions to make it look as though they're holding the headphones.
            float frontArmRotation = Owner.direction * (1f - animationCompletion) * -MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation - Owner.direction * 1.72f);
            Projectile.Center = Owner.Center + Vector2.UnitX.RotatedBy(frontArmRotation) * Owner.direction * 16f;

            Owner.GetModPlayer<CustomMusicPlayer>().UsingHeadphones = false;
            if (animationCompletion >= 1f)
                Projectile.Kill();
        }

        public void DoBehavior_HandleUIState()
        {
            // Use the UI effect.
            IsRenderingUI = true;
            UIOptionsOpacity = MathHelper.Clamp(UIOptionsOpacity + 0.045f, 0f, 1f);
        }

        public void AdjustPlayerValues()
        {
            Projectile.timeLeft = 2;
            Projectile.spriteDirection = Owner.direction;
            Projectile.Center = Owner.Center;
            Owner.heldProj = Projectile.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Just draw the matrix as usual without the hologram or UI effect if the calling client isn't the one using the item, since they shouldn't be able to interact
            // with someone else's UI.
            if (Main.myPlayer != Projectile.owner)
                return true;

            DrawUI();
            return true;
        }

        public void DrawUI()
        {
            if (!IsRenderingUI || UIOptionsOpacity <= 0f)
                return;

            float opacity = UIOptionsOpacity;
            if (DisappearCountdown >= 1f)
                opacity *= Utils.GetLerpValue(1f, 36f, DisappearCountdown, true);

            string text = string.Empty;
            Color textColor = Color.Transparent;
            Vector2 textDrawPosition = Vector2.Zero;

            List<MusicUIIcon> icons = UIStates.Where(ui =>
            {
                if (ui.RequiresWoTG && !InfernalCrossmod.NoxusBoss.Loaded)
                    return false;
                if (ui.RequiresYou && !InfernalCrossmod.YouBoss.Loaded)
                    return false;
                if (ui.RequiresThorium && !InfernalCrossmod.Thorium.Loaded)
                    return false;

                return true;
            }).ToList();
            for (int i = 0; i < icons.Count; i++)
            {
                float indexCompletion = i / (float)icons.Count;
                if (icons.Count <= 1)
                    indexCompletion = 0.5f;

                bool clicked = icons[i].Draw(Owner, Projectile.Center - Vector2.UnitY * 210f - Main.screenPosition, indexCompletion, opacity, out Vector2 localTextDrawPosition, out Color localTextColor, out string localText) && opacity >= 1f;
                if (!string.IsNullOrEmpty(localText) && opacity >= 1f)
                {
                    text = localText;
                    textColor = localTextColor;
                    textDrawPosition = localTextDrawPosition;
                }

                if (DisappearCountdown == 0f && clicked)
                {
                    SoundEngine.PlaySound(SoundID.Item129);
                    Main.blockMouse = false;
                    break;
                }
            }

            // Draw text above everything else if necessary.
            if (!string.IsNullOrEmpty(text))
            {
                var font = FontAssets.MouseText.Value;
                Vector2 textArea = font.MeasureString(text);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, textDrawPosition - Vector2.UnitX * textArea * 0.5f, textColor, 0f, textArea * new Vector2(0f, 0.5f), Vector2.One);
            }

            if (Main.mouseLeft && Main.mouseLeftRelease && !Main.blockMouse)
                DisappearCountdown = 36f;
        }

        public override bool? CanDamage() => false;
    }

    public class CustomMusicPlayer : ModPlayer
    {
        public bool UsingHeadphones
        {
            get;
            set;
        }

        public string CurrentTrackName
        {
            get;
            set;
        }

        public bool UnlockAllMusic
        {
            get;
            set;
        }

        public float HeadRotationTime
        {
            get;
            set;
        }

        public bool ListeningToMusic
        {
            get
            {
                return UsingHeadphones && !string.IsNullOrEmpty(CurrentTrackName);
            }
        }

        public override void PreUpdate()
        {
            if (!UsingHeadphones)
                CurrentTrackName = string.Empty;

            // Create music particles if a track is playing.
            if (Main.myPlayer == Player.whoAmI && Main.rand.NextBool(16) && ListeningToMusic)
            {
                int musicNoteID = Main.rand.Next(ProjectileID.EighthNote, ProjectileID.TiedEighthNote + 1);
                Vector2 noteSpawnPosition = Player.Top + new Vector2(Main.rand.NextFloatDirection() * 16f, Main.rand.NextFloat(12f));

                int note = Projectile.NewProjectile(Player.GetSource_FromThis(), noteSpawnPosition, -Vector2.UnitY.RotatedByRandom(0.7f), musicNoteID, 0, 0f, Player.whoAmI);
                if (Main.projectile.IndexInRange(note))
                    Main.projectile[note].scale = 0.5f;
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            HeadRotationTime = (float)ModLoader.GetMod("InfernumMode").Call("BopHeadToMusic", Player, HeadRotationTime);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["UsingHeadphonesIEoR"] = UsingHeadphones;
            tag["UnlockAllMusicIEoR"] = UnlockAllMusic;
            tag["CurrentTrackNameIEoR"] = CurrentTrackName;
        }

        public override void LoadData(TagCompound tag)
        {
            UsingHeadphones = tag.GetBool("UsingHeadphonesIEoR");
            UnlockAllMusic = tag.GetBool("UnlockAllMusicIEoR");
            CurrentTrackName = tag.GetString("CurrentTrackNameIEoR");
        }
    }

    public class HeadphonesMusicSystem : ModSceneEffect
    {
        public override int Music
        {
            get
            {
                string trackName = Main.LocalPlayer.GetModPlayer<CustomMusicPlayer>().CurrentTrackName;

                switch (trackName)
                {
                    case "FINALFRACTAL":
                        if (!InfernalCrossmod.YouBoss.Loaded) return -1;
                        return MusicLoader.GetMusicSlot("YouBoss/Assets/Sounds/Music/You");
                    case "RealitysEnd":
                        if (!InfernalCrossmod.Thorium.Loaded) return -1;
                        return MusicLoader.GetMusicSlot(InfernalCrossmod.Thorium.Mod, "Sounds/Music/Realitys_End");
                    default:
                        return MusicLoader.GetMusicSlot(Mod, $"Assets/Music/{trackName}");
                }
            }
        }

        public override bool IsSceneEffectActive(Player player)
        {
            string trackName = player.GetModPlayer<CustomMusicPlayer>().CurrentTrackName;

            return player.GetModPlayer<CustomMusicPlayer>().UsingHeadphones && !string.IsNullOrEmpty(trackName);
        }

        public override SceneEffectPriority Priority => (SceneEffectPriority)20;
    }

    public class SoulDrivenHeadphonesEclipseLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.GetModPlayer<CustomMusicPlayer>().UsingHeadphones)
                return drawInfo.shadow <= 0f;
            return false;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var drawPlayer = drawInfo.drawPlayer;
            Texture2D texture = ModContent.Request<Texture2D>("InfernalEclipseAPI/Content/Items/Other/SoulDrivenHeadphonesEclipse_Head").Value;

            // It is imperative drawInfo.Position and not drawPlayer.position is used, or else the layer will break on the player select and the map (in the case of a head layer).
            Vector2 headDrawPosition = drawInfo.Position - Main.screenPosition;

            // Center everything.
            headDrawPosition += new Vector2((drawPlayer.width - drawPlayer.bodyFrame.Width) / 2f, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f);

            // Floor the result to remove subpixel jittering.
            headDrawPosition += drawPlayer.headPosition + drawInfo.headVect;
            headDrawPosition += new Vector2(drawPlayer.direction * -4f, -6f).RotatedBy(drawPlayer.headRotation);
            headDrawPosition = new Vector2((int)headDrawPosition.X, (int)headDrawPosition.Y);

            drawInfo.DrawDataCache.Add(new(texture, headDrawPosition, null, drawInfo.colorArmorHead, drawPlayer.headRotation, texture.Size() * 0.5f, 1f, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0));
        }
    }
}