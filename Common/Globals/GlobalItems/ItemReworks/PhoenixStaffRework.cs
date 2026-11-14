using System.Collections.Generic;
using InfernalEclipseAPI.Core.Systems.Compatibility;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Localization;
using ThoriumMod;

namespace InfernalEclipseAPI.Common.GlobalItems.ItemReworks
{
    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class PhoenixStaffRework : GlobalItem
    {
        private Mod thorium
        {
            get
            {
                ModLoader.TryGetMod("ThoriumMod", out Mod thor);
                return thor;
            }
        }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("WHummusMultiModBalancing");
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return thorium != null && entity.type == thorium.Find<ModItem>("PhoenixStaff").Type;
        }

        public override bool AltFunctionUse(Item item, Player player) => true;

        public override void SetDefaults(Item item)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("PhoenixStaff").Type)
            {
                Item.staff[item.type] = true;

                item.damage = 45;
                item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                item.useTime = 6;
                item.useAnimation = 30;
                item.useStyle = ItemUseStyleID.Shoot;
                item.UseSound = SoundID.Item34;
                item.mana = 5;
                item.noMelee = true;
                item.autoReuse = true;
                item.shootSpeed = 1.25f;

                ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
            }
        }


        public override bool CanUseItem(Item item, Player player)
        {
            if (thorium == null) return base.CanUseItem(item, player);

            if (item.type == thorium.Find<ModItem>("PhoenixStaff").Type)
            {
                if (player.altFunctionUse == 2)
                {
                    item.useStyle = ItemUseStyleID.Shoot;
                    item.UseSound = SoundID.Item34;
                    item.noMelee = true;
                    item.autoReuse = true;
                }
                else
                {
                    item.useStyle = ItemUseStyleID.Swing;
                    item.UseSound = SoundID.Item44;
                    item.noMelee = true;
                    item.autoReuse = false;
                }
            }

            return base.CanUseItem(item, player);
        }

        public override float UseTimeMultiplier(Item item, Player player)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("PhoenixStaff").Type)
                return player.altFunctionUse == 2 ? 1f : 5f;
            return 1f;
        }

        public override float UseAnimationMultiplier(Item item, Player player)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("PhoenixStaff").Type)
                return player.altFunctionUse == 2 ? 1f : 1f;
            return 1f;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (thorium == null) return base.Shoot(item, player, source, position, velocity, type, damage, knockback);

            if (item.type == thorium.Find<ModItem>("PhoenixStaff").Type && player.altFunctionUse == 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
                position += muzzleOffset;

                int proj = Projectile.NewProjectile(
                    source,
                    position,
                    velocity * 8f,
                    ProjectileID.Flames,
                    damage,
                    knockback,
                    player.whoAmI
                );

                if (proj >= 0)
                {
                    Main.projectile[proj].ai[0] = 9999f;
                }

                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip, bool InfernalRedActive = false)
        {
            Color InfernalRed = Color.Lerp(
               Color.White,
               new Color(255, 80, 0), // Infernal red/orange
               (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2.0) * 0.5 + 0.5)
            );

            int maxTooltipIndex = -1;
            int maxNumber = -1;

            // Find the TooltipLine with the highest TooltipX name
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.StartsWith("Tooltip"))
                {
                    if (int.TryParse(tooltips[i].Name.Substring(7), out int num) && num > maxNumber)
                    {
                        maxNumber = num;
                        maxTooltipIndex = i;
                    }
                }
            }

            // If found, insert a new TooltipLine right after it with the desired color
            if (maxTooltipIndex != -1)
            {
                int insertIndex = maxTooltipIndex + 1;
                TooltipLine customLine = new TooltipLine(Mod, "StealthTooltip", stealthTooltip);
                if (InfernalRedActive)
                    customLine.OverrideColor = InfernalRed;

                tooltips.Insert(insertIndex, customLine);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("PhoenixStaff")?.Type)
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.PhoenixStaff"));
            }
        }

        public override bool AllowPrefix(Item item, int pre)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("PhoenixStaff").Type)
                return true;
            return base.AllowPrefix(item, pre);
        }

        public override bool CanReforge(Item item)
        {
            if (thorium != null && item.type == thorium.Find<ModItem>("PhoenixStaff").Type)
                return true;
            return base.CanReforge(item);
        }
    }

    [JITWhenModsEnabled(InfernalCrossmod.Thorium.Name)]
    [ExtendsFromMod(InfernalCrossmod.Thorium.Name)]
    public class PhoenixStaffFlameGlobal : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            // Check for flame projectile with our special tag
            if (projectile.type == ProjectileID.Flames && projectile.ai[0] == 9999f)
            {
                projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ProjectileID.Flames && projectile.ai[0] == 9999f)
            {
                // Apply Thorium Holy Glare debuff for 2 seconds (120 ticks)
                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    int holyGlareType = thorium.Find<ModBuff>("HolyGlare")?.Type ?? -1;
                    if (holyGlareType != -1)
                    {
                        target.AddBuff(holyGlareType, 120);
                    }
                }
            }
        }
    }
}
