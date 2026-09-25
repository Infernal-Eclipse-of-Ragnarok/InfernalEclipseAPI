using System.Reflection;
using InfernalEclipseAPI.Core.Players;
using InfernalEclipseAPI.Core.Systems;
using Terraria.DataStructures;
using Terraria.Localization;
using ThoriumMod.Items.Terrarium;
using ThoriumMod.NPCs;
using ThoriumMod.Utilities;

namespace InfernalEclipseAPI.Content.RogueThrower
{
    //Exhaustion removal code by Wardrobe Hummus
    //White Dwarf set cooldown by Akira
    [JITWhenModsEnabled("ThoriumMod")]
    [ExtendsFromMod("ThoriumMod")]
    public class RogueThrowerPlayer : ModPlayer
    {
        private int volume2Type = -1;
        private int volume3Type = -1;
        private int soul1Type = -1;
        private int soul2Type = -1;
        private int soul3Type = -1;
        private bool initialized = false;

        private int? previousHeldItemType;
        private int? previousHeldItemPrefix = null;
        private bool? previousHeldItemOriginalExhaustion;

        public int whiteDwarfCooldown;
        public int ShinobiSigilCooldown;
        public int bronzeSetCooldown;
        public int dragonSetCooldown;
        public int demonBloodDodgeCooldown;

        public bool subspaceLocketThorClassNerf;

        public override void ResetEffects()
        {
            if (whiteDwarfCooldown > 0)
                whiteDwarfCooldown--;

            if (ShinobiSigilCooldown > 0)
                ShinobiSigilCooldown--;

            if (bronzeSetCooldown > 0)
                bronzeSetCooldown--;

            if (dragonSetCooldown > 0)
                dragonSetCooldown--;

            if (demonBloodDodgeCooldown > 0)
                demonBloodDodgeCooldown--;
        }

        private void EnsureInitialized()
        {
            if (initialized)
                return;

            if (ModContent.TryFind("ThoriumMod/ThrowingGuideVolume2", out ModItem modItem1))
                volume2Type = modItem1.Type;

            if (ModContent.TryFind("ThoriumMod/ThrowingGuideVolume3", out ModItem modItem2))
                volume3Type = modItem2.Type;

            if (ModContent.TryFind("FargowiltasCrossmod/VagabondsSoul", out ModItem modItem4))
                soul1Type = modItem4.Type;

            if (ModContent.TryFind("FargowiltasSouls/UniverseSoul", out ModItem modItem5))
                soul2Type = modItem5.Type;

            if (ModContent.TryFind("FargowiltasSouls/EternitySoul", out ModItem modItem6))
                soul3Type = modItem6.Type;

            initialized = true;
        }

        public override void UpdateEquips()
        {
            EnsureInitialized();

            bool accessoryEquipped = HasExhaustionClearingAccessoryEquipped();

            Item heldItem = Player.HeldItem;

            bool heldItemChanged =
                previousHeldItemType == null ||
                previousHeldItemPrefix == null ||
                heldItem.type != previousHeldItemType ||
                heldItem.prefix != previousHeldItemPrefix;

            if (heldItemChanged)
            {
                RestoreExhaustion();

                previousHeldItemType = heldItem.type;
                previousHeldItemPrefix = heldItem.prefix;
                previousHeldItemOriginalExhaustion = null;
            }

            if (accessoryEquipped)
            {
                ApplyExhaustionDisabling();
            }
            else
            {
                RestoreExhaustion();
            }

            if (InfernalCrossmod.ThoriumRework.Loaded)
            {
                if (Player.armor[0].type == ModContent.ItemType<TerrariumHelmet>() &&
                    Player.armor[1].type == ModContent.ItemType<TerrariumBreastPlate>() &&
                    Player.armor[2].type == ModContent.ItemType<TerrariumGreaves>())
                {
                    for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
                    {
                        if (Player.armor[i].type == ModContent.ItemType<TerrariumWings>())
                        {
                            Player.wingTimeMax *= 2;
                            break;
                        }
                    }
                }
            }
        }

        private void ApplyExhaustionDisabling()
        {
            Item heldItem = Player.HeldItem;
            if (heldItem.IsAir || heldItem.ModItem == null)
                return;

            ModItem modItem = heldItem.ModItem;

            if (modItem.Mod?.Name == "ThoriumMod" || modItem.Mod?.Name == "Consolaria" || modItem.Mod?.Name == "SOTSBardHealer")
            {
                var field = modItem.GetType().GetField("isThrowerNon", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    bool currentValue = (bool)field.GetValue(modItem);

                    if (previousHeldItemOriginalExhaustion == null)
                    {
                        previousHeldItemOriginalExhaustion = currentValue;
                    }

                    if (currentValue)
                    {
                        field.SetValue(modItem, false); // Disable exhaustion
                    }
                }
            }
        }

        private void RestoreExhaustion()
        {
            if (previousHeldItemOriginalExhaustion == null)
                return;

            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (!item.IsAir &&
                    item.type == previousHeldItemType &&
                    item.prefix == previousHeldItemPrefix &&
                    item.ModItem != null &&
                    item.ModItem.Mod?.Name == "ThoriumMod")
                {
                    var field = item.ModItem.GetType().GetField("isThrowerNon", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(item.ModItem, previousHeldItemOriginalExhaustion);
                    }

                    break;
                }
            }

            previousHeldItemOriginalExhaustion = null;
        }

        public override void PostUpdateEquips()
        {
            Mod thorium; 
            ModLoader.TryGetMod("ThoriumMod", out thorium);
            if (thorium == null || InfernalCrossmod.Hummus.Loaded || InfernalCrossmod.RagnarokMod.Loaded) return;

            int whiteDwarfHelm = thorium.Find<ModItem>("WhiteDwarfMask").Type;
            int whiteDwarfPlate = thorium.Find<ModItem>("WhiteDwarfGuard").Type;
            int whiteDwarfGreaves = thorium.Find<ModItem>("WhiteDwarfGreaves").Type;

            if (Player.armor[0].type == whiteDwarfHelm &&
                Player.armor[1].type == whiteDwarfPlate &&
                Player.armor[2].type == whiteDwarfGreaves)
            {
                Player.setBonus += $"\n{Language.GetTextValue("Mods.InfernalEclipseAPI.ItemTooltip.WhiteDwarfSetBonus")}";
            }

            if (dragonSetCooldown > 0)
                Player.GetThoriumPlayer().dragonSet = false;

            if (subspaceLocketThorClassNerf)
            {
                Player.GetThoriumPlayer().healBonus /= 2;
                Player.GetThoriumPlayer().inspirationRegenBonus /= 2;
            }
        }

        public bool HasExhaustionClearingAccessoryEquipped()
        {
            EnsureInitialized();

            int start = 3;
            int end = Player.armor.Length - 10;

            for (int i = start; i < end; i++)
            {
                if (i >= Player.armor.Length)
                    continue;

                Item accessory = Player.armor[i];

                if (accessory == null || accessory.IsAir)
                    continue;

                // Match any of the exhaustion-clearing accessory types
                if (accessory.type == volume2Type ||
                    accessory.type == volume3Type ||

                    accessory.type == soul1Type ||
                    accessory.type == soul2Type ||
                    accessory.type == soul3Type)
                {
                    return true;
                }
            }

            foreach (ModAccessorySlot slot in ModContent.GetContent<ModAccessorySlot>())
            {
                if (!slot.IsEnabled())
                    continue;

                Item accessory = slot.FunctionalItem;

                if (accessory == null || accessory.IsAir)
                    continue;

                if (accessory.type == volume2Type ||
                    accessory.type == volume3Type ||
                    accessory.type == soul1Type ||
                    accessory.type == soul2Type ||
                    accessory.type == soul3Type)
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ThoriumGlobalNPC thorGlobalNPC = target.GetGlobalNPC<ThoriumGlobalNPC>();
            bool isHostile = !target.friendly && target.lifeMax > 5 && target.chaseable && (!target.dontTakeDamage) && !target.immortal;
            IEntitySource sourceOnHit = proj.GetSource_OnHit(target);

            if (Player.GetModPlayer<InfernalPlayer>().gutWrench & isHostile && target.value > 0.0)
            {
                int lifeHealed = Math.Min(damageDone / 100, Player.statLifeMax2 / 2 - Player.statLife);
                if (Player.lifeSteal > 0)
                {
                    Player.HealLife(lifeHealed);

                    if (Player.lifeSteal - lifeHealed * 10f < -35)
                        Player.lifeSteal = -35;
                    else
                        Player.lifeSteal -= lifeHealed * 10f;
                }
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ThoriumGlobalNPC thorGlobalNPC = target.GetGlobalNPC<ThoriumGlobalNPC>();
            bool isHostile = !target.friendly && target.lifeMax > 5 && target.chaseable && (!target.dontTakeDamage) && !target.immortal;
            IEntitySource sourceOnHit = Player.GetSource_OnHit(target);

            if (Player.GetModPlayer<InfernalPlayer>().gutWrench & isHostile && target.value > 0.0)
            {
                int lifeHealed = Math.Min(damageDone / 100, Player.statLifeMax2 / 2 - Player.statLife);
                if (Player.lifeSteal > 0)
                {
                    Player.HealLife(lifeHealed);

                    if (Player.lifeSteal - lifeHealed * 10f < -35)
                        Player.lifeSteal = -35;
                    else
                        Player.lifeSteal -= lifeHealed * 10f;
                }
            }
        }
    }
}
