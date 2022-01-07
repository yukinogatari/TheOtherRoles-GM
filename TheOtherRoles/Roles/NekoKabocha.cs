using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class NekoKabocha : RoleBase<NekoKabocha>
    {
        public static Color color = Palette.ImpostorRed;

        public static bool revengeCrew { get { return CustomOptionHolder.nekoKabochaRevengeCrew.getBool(); } }
        public static bool revengeNeutral { get { return CustomOptionHolder.nekoKabochaRevengeNeutral.getBool(); } }
        public static bool revengeImpostor { get { return CustomOptionHolder.nekoKabochaRevengeImpostor.getBool(); } }
        public static bool revengeExile { get { return CustomOptionHolder.nekoKabochaRevengeExile.getBool(); } }

        public NekoKabocha()
        {
            RoleType = roleId = RoleId.NekoKabocha;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }
        
        public override void OnDeath(PlayerControl killer = null)
        {
            if (killer != null)
            {
                if ((revengeCrew && killer.isCrew()) ||
                    (revengeNeutral && killer.isNeutral()) ||
                    (revengeImpostor && killer.isImpostor()))
                {
                    player.MurderPlayer(killer);
                    finalStatuses[killer.PlayerId] = FinalStatus.Revenge;
                }
            }
            else if (killer == null && revengeExile)
            {
                var candidates = PlayerControl.AllPlayerControls.ToArray().Where(x => x != player && x.isAlive()).ToList();
                int targetID = rnd.Next(0, candidates.Count);
                var target = candidates[targetID];

                target.Exiled();
                finalStatuses[target.PlayerId] = FinalStatus.Revenge;
            }
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void MakeButtons(HudManager hm) { }
        public static void SetButtonCooldowns() { }

        public static void clearAndReload()
        {
            players = new List<NekoKabocha>();
        }
    }
}