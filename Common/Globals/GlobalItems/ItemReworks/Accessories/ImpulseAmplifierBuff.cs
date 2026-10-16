using ThoriumMod;
using Microsoft.Xna.Framework;
using InfernalEclipseAPI.Core.Systems.Compatibility;

namespace InfernalEclipseAPI.Common.Globals.GlobalItems.ItemReworks.Accessories
{
    [JITWhenModsEnabled(InfernalCrossmod.ThoriumRework.Name)]
    [ExtendsFromMod(InfernalCrossmod.ThoriumRework.Name)]
    public class ImpulseAmplifierBuff : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool stormTriggered = false;

        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
        {

            object callResult = InfernalCrossmod.Thorium.Mod.Call("IsBardProjectile", projectile);

            if (callResult != null)
            {
                try
                {
                    dynamic tuple = callResult;
                    bool isBard = tuple.Item1;
                    byte bardType = tuple.Item2;

                    // Electronic instruments = type 5
                    return isBard && bardType == 5;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public override void OnHitNPC(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (stormTriggered) return;
            if (!proj.friendly || proj.owner < 0 || proj.owner > 255) return;

            Player player = Main.player[proj.owner];

            // Get the ThoriumPlayer type dynamically
            Type thoriumPlayerType = InfernalCrossmod.ThoriumRework.Mod.Code.GetType("ThoriumRework.ThoriumPlayer");
            if (thoriumPlayerType == null) return;

            // Use reflection to get the ModPlayer instance from Player
            ModPlayer thoriumPlayerInstance = null;
            var modPlayersField = typeof(Player).GetField("modPlayers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (modPlayersField != null)
            {
                var modPlayersArray = modPlayersField.GetValue(player) as ModPlayer[];
                if (modPlayersArray != null)
                {
                    foreach (var mp in modPlayersArray)
                    {
                        if (mp.GetType() == thoriumPlayerType)
                        {
                            thoriumPlayerInstance = mp;
                            break;
                        }
                    }
                }
            }

            if (thoriumPlayerInstance == null) return;

            // Access the impulseAmp field dynamically
            bool impulseAmpActive = false;
            try
            {
                var fieldInfo = thoriumPlayerType.GetField("impulseAmp");
                if (fieldInfo != null)
                    impulseAmpActive = (bool)fieldInfo.GetValue(thoriumPlayerInstance);
            }
            catch
            {
                return;
            }

            if (!impulseAmpActive) return;

            // --- Apply Electrified to the original target ---
            target.AddBuff(BuffID.Electrified, 60);
            stormTriggered = true;

            float searchRadius = 150f;
            NPC closest = null;
            float closestDist = searchRadius;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.CanBeChasedBy()) continue;
                if (npc.whoAmI == target.whoAmI) continue;

                float d = Vector2.Distance(npc.Center, target.Center);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = npc;
                }
            }

            if (closest == null) return;

            Vector2 dir = closest.Center - proj.Center;
            dir.Normalize();
            dir *= 12f;

            int stormDamage = Math.Max(damageDone / 10, 1);
            int projectileID = 732; // Lightning projectile

            Projectile newProj = Projectile.NewProjectileDirect(
                proj.GetSource_FromThis(),
                proj.Center,
                dir,
                projectileID,
                stormDamage,
                1.5f,
                proj.owner
            );

            newProj.friendly = true;
            newProj.DamageType = ThoriumDamageBase<BardDamage>.Instance;

            newProj.usesLocalNPCImmunity = true;
            newProj.usesIDStaticNPCImmunity = false;
            newProj.localNPCHitCooldown = 20;
            newProj.localNPCImmunity[target.whoAmI] = 20;

            newProj.GetGlobalProjectile<ImpulseAmplifierTag>().triggeredByImpulse = true;
        }
    }

    public class ImpulseAmplifierTag : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool triggeredByImpulse = false;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (triggeredByImpulse)
            {
                target.AddBuff(BuffID.Electrified, 60);
            }
        }
    }
}
