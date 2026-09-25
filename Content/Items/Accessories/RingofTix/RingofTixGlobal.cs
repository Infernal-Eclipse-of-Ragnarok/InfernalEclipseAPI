using System.Collections.Generic;
using System.IO;
using InfernalEclipseAPI.Core.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SOTS;
using SOTS.Items.Gems;
using SOTS.Void;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace InfernalEclipseAPI.Content.Items.Accessories.RingofTix
{
    [ExtendsFromMod("SOTS")]
    public class RingofTixGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<RingofTix>();

        public override bool InstancePerEntity => true;
        private const float OrbitSpeed = -0.8f;
        private bool[] GemDisabled = new bool[7];
        private static Texture2D challengerGems;
        private static Texture2D invertedChallengerGems;
        private static readonly Color[] GemColors = new[]
        {
            new Color(186,104,200), // Amethyst
            new Color(255,200, 80), // Topaz
            new Color( 86,120,255), // Sapphire
            new Color( 46,204,113), // Emerald
            new Color(220, 50, 50), // Ruby
            new Color(180,220,255), // Diamond
            new Color(255,175,  0), // Amber
        };

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return (equippedItem.type != ModContent.ItemType<ChallengerRing>() && incomingItem.type != ModContent.ItemType<ChallengerRing>());
        }

        // -------- Save/Load/Net --------
        public override void NetSend(Item item, BinaryWriter writer)
        {
            for (int i = 0; i < 7; i++) writer.Write(GemDisabled[i]);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            for (int i = 0; i < 7; i++) GemDisabled[i] = reader.ReadBoolean();
        }

        public override void SaveData(Item item, TagCompound tag) => tag["Gems"] = GemDisabled;

        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.TryGet("Gems", out bool[] flags) && flags is { Length: 7 }) GemDisabled = flags;
        }

        // -------- Assets --------
        private static void EnsureGemTexturesLoaded()
        {
            if (Main.dedServ) return;
            challengerGems ??= ModContent.Request<Texture2D>("SOTS/Items/Gems/ChallengerGems", AssetRequestMode.ImmediateLoad).Value;
            invertedChallengerGems ??= ModContent.Request<Texture2D>("SOTS/Items/Gems/InvertedChallengerGems", AssetRequestMode.ImmediateLoad).Value;
        }

        private static Texture2D TryGetGlowTexture(Item item)
        {
            if (Main.dedServ) return null;
            if (item.ModItem is ModItem mi)
                return ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj", AssetRequestMode.ImmediateLoad).Value;
            return null;
        }

        // -------- Orbit visuals (inventory/world) --------
        private void DrawOrbitals(SpriteBatch sb, Vector2 pos, Color drawColor, float scale, float rotation, bool front)
        {
            EnsureGemTexturesLoaded();

            for (int idx = 0; idx < 7; idx++)
            {
                float a = MathHelper.WrapAngle(MathHelper.ToRadians(SOTSWorld.GlobalCounter * OrbitSpeed) + (float)(idx * MathHelper.TwoPi / 7.0));
                bool isFront = a > 0f && a < MathHelper.Pi;
                if (isFront != front) continue;

                Vector2 radial = new Vector2(18f, 0f).RotatedBy(a);
                float off = a - MathHelper.PiOver2;
                const float band = 0.448798954f;

                float onBand = 1f - Math.Abs(off) / band;
                float drawRot = radial.ToRotation() + MathHelper.PiOver2 + rotation;

                float len = radial.Length();
                float squash = 1f + (len == 0f ? 0f : radial.Y / len) / 5f;

                radial.Y *= 0.625f;
                Vector2 gemPos = radial.RotatedBy(rotation - MathHelper.PiOver4);
                Rectangle frame = new Rectangle(0, 16 * idx, 16, 16);

                Texture2D sheet = GemDisabled[idx] ? invertedChallengerGems : challengerGems;

                float d = MathHelper.Clamp(onBand, 0.25f, 1f);
                float pulse = (float)Math.Sqrt(d);

                for (int s = 0; s < 8; s++)
                {
                    Vector2 spark = new Vector2(0f, 3f * scale * pulse).RotatedBy(MathHelper.ToRadians(s * 45 + SOTSWorld.GlobalCounter * 2));
                    sb.Draw(challengerGems, pos + gemPos + spark, frame, new Color(200, 200, 200, 0) * d, drawRot, new Vector2(8f, 8f), scale * 0.6f * squash, SpriteEffects.None, 0f);
                }

                sb.Draw(sheet, pos + gemPos, frame, drawColor, drawRot, new Vector2(8f, 8f), scale * 0.6f * squash, SpriteEffects.None, 0f);
            }
        }

        // -------- Inventory draw --------
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawOrbitals(spriteBatch, position, drawColor, scale, 0f, front: false);

            Texture2D glow = TryGetGlowTexture(item);
            if (glow != null)
            {
                Vector2 shifted = position - new Vector2(2f, 2f) * scale;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 bump = new Vector2(0f, 2f).RotatedBy(MathHelper.ToRadians(i * 60 + SOTSWorld.GlobalCounter * 2)) * scale;
                    Color c = new Color(70 - i * 7, 45 - i * 2, 40 + i * 4, 250);
                    spriteBatch.Draw(glow, shifted + bump, null, c * (1f - item.alpha / 255f) * 1.2f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D glow = TryGetGlowTexture(item);
            if (glow != null)
                spriteBatch.Draw(glow, position - new Vector2(2f, 2f) * scale, null, Color.Lerp(drawColor, Color.White, 0.5f), 0f, origin, scale, SpriteEffects.None, 0f);

            DrawOrbitals(spriteBatch, position, drawColor, scale, 0f, front: true);
        }

        // -------- World draw --------
        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawOrbitals(spriteBatch, item.Center - Main.screenPosition, lightColor, scale, rotation, front: false);

            Texture2D glow = TryGetGlowTexture(item);
            if (glow != null)
            {
                var tex = TextureAssets.Item[item.type].Value;
                Vector2 origin = tex.Size() * 0.5f;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 bump = new Vector2(0f, 2f).RotatedBy(MathHelper.ToRadians(i * 60 + SOTSWorld.GlobalCounter * 2));
                    Color c = new Color(70 - i * 7, 45 - i * 2, 40 + i * 4, 250);
                    spriteBatch.Draw(glow, item.Center - Main.screenPosition + bump, null, c * (1f - item.alpha / 255f) * 1.2f, rotation, origin, scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glow = TryGetGlowTexture(item);
            if (glow != null)
            {
                var tex = TextureAssets.Item[item.type].Value;
                Vector2 origin = tex.Size() * 0.5f;
                spriteBatch.Draw(glow, item.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), rotation, origin, scale, SpriteEffects.None, 0f);
            }

            DrawOrbitals(spriteBatch, item.Center - Main.screenPosition, lightColor, scale, rotation, front: true);
        }

        // -------- Tooltip content (dynamic) --------
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Inject gem description lines (custom), then our PreDraw will render them.
            // First remove any old ones to avoid duplicates on re-open.
            tooltips.RemoveAll(t => t.Mod == Mod.Name && t.Name.StartsWith("GemTip", StringComparison.Ordinal));

            int insertAt = tooltips.FindLastIndex(t => t.Mod == Mod.Name && (t.Name == "SOTSTip" || t.Name == "NormalTip"));
            if (insertAt < 0) insertAt = tooltips.Count - 1;

            int selected = SimulateSelectedGem();
            for (int i = 0; i < 7; i++)
            {
                string key = GemDisabled[i] ? $"I{i}" : $"G{i}";
                string text;

                if (i == 5) // Diamond needs parameters
                {
                    if (key == "G5")
                    {
                        int prevDef = SOTSPlayer.ModPlayer(Main.LocalPlayer).previousDefense;
                        text = Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix." + key, prevDef.ToString(), (prevDef * 0.66).ToString());
                    }
                    else
                    {
                        text = Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix." + key, Main.LocalPlayer.GetModPlayer<InfernalPlayer>().defenseGain, Main.LocalPlayer.GetModPlayer<InfernalPlayer>().defenseGain);
                    }
                }
                else
                {
                    text = Language.GetTextValue("Mods.InfernalEclipseAPI.Items.RingofTix." + key);
                }

                if (i == selected) text = text.Replace("\n", "\n> ");

                // store text so if our PreDraw is ever bypassed, vanilla still shows something
                var line = new TooltipLine(Mod, $"GemTip{i}", text)
                {
                    OverrideColor = Color.Lerp(GemDisabled[i] ? Color.LightGray : Color.White, GemColors[i], 0.4f)
                };

                tooltips.Insert(Math.Min(++insertAt, tooltips.Count), line);
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            // Only custom-draw our gem lines. Let everything else render normally.
            if (line.Mod != Mod.Name || !line.Name.StartsWith("GemTip", StringComparison.Ordinal))
                return true;

            int idx = int.Parse(line.Name.AsSpan("GemTip".Length));
            bool isSelected = SimulateSelectedGem() == idx;
            bool isInverse = GemDisabled[idx];

            int xShift = isSelected ? -19 : 0;

            Color onColor = line.OverrideColor ?? line.Color;
            Color textColor = isInverse ? Color.Black : onColor;
            Color shadow = isInverse ? onColor : Color.Black;

            // shadow
            ChatManager.DrawColorCodedStringShadow(
                Main.spriteBatch, line.Font, line.Text,
                new Vector2(line.X + xShift, line.Y),
                shadow, line.Rotation, line.Origin, line.BaseScale,
                line.MaxWidth, line.Spread);

            // text (snippets aware)
            var snippets = ChatManager.ParseMessage(line.Text, textColor).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            int _;
            ChatManager.DrawColorCodedString(
                Main.spriteBatch, line.Font, snippets,
                new Vector2(line.X + xShift, line.Y),
                textColor, line.Rotation, line.Origin, line.BaseScale,
                out _, line.MaxWidth, false);

            // we drew it
            return false;
        }

        // -------- Click handlers --------
        public override bool CanRightClick(Item item) => item.favorited;

        public override void RightClick(Item item, Player player)
        {
            int idx = SimulateSelectedGem();
            GemDisabled[idx] = !GemDisabled[idx];
        }

        public override bool ConsumeItem(Item item, Player player) => false;

        // -------- Helpers --------
        private int SimulateSelectedGem()
        {
            for (int i = 0; i < 7; i++)
            {
                float a = MathHelper.WrapAngle(MathHelper.ToRadians(SOTSWorld.GlobalCounter * OrbitSpeed) + (float)(i * MathHelper.TwoPi / 7.0));
                float off = a - MathHelper.PiOver2;
                const float band = 0.448798954f;
                if (off > -band && off < band) return i;
            }
            return 0;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            VoidPlayer voidPlayer = player.VoidPlayer();
            SOTSPlayer sotsPlayer = player.SOTSPlayer();
            if (GemDisabled[0])
            {
                player.blockRange += 3;
                player.tileSpeed += 0.2f;
                player.wallSpeed += 0.2f;
                player.moveSpeed += 0.2f;
            }
            else
                sotsPlayer.AmethystRing = true;
            if (GemDisabled[1])
                sotsPlayer.InverseTopazRing = true;
            else
                sotsPlayer.TopazRing = true;
            if (GemDisabled[2])
            {
                voidPlayer.GainHealthOnVoidUse += 0.1f;
                voidPlayer.GainVoidOnHurt += 0.1f;
            }
            else
                ++voidPlayer.VoidGenerateMoney;
            if (GemDisabled[3])
            {
                player.endurance += 0.15f;
                player.GetDamage(DamageClass.Generic) -= 0.15f;
            }
            else
                sotsPlayer.EmeraldRing = true;
            if (GemDisabled[4])
            {
                player.AddBuff(BuffID.WellFed, 60, true, false);
                voidPlayer.VoidFoodGainMultiplier -= 0.75f;
            }
            else
                sotsPlayer.RubyRing = true;
            if (GemDisabled[5])
                sotsPlayer.InverseDiamondRing = true;
            else
                sotsPlayer.DiamondRing = true;
            if (GemDisabled[6])
                sotsPlayer.InverseAmberRing = true;
            else
                sotsPlayer.AmberRing = true;
        }
    }
}
