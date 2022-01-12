using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Madmate : RoleBase<Madmate>
    {
        public static Color color = Palette.ImpostorRed;

        public static bool canEnterVents { get { return CustomOptionHolder.madmateCanEnterVents.getBool(); } }
        public static bool hasImpostorVision { get { return CustomOptionHolder.madmateHasImpostorVision.getBool(); } }
        public static bool canSabotage { get { return CustomOptionHolder.madmateCanSabotage.getBool(); } }
        public static bool canFixComm { get { return CustomOptionHolder.madmateCanFixComm.getBool(); } }

        public Madmate()
        {
            RoleType = roleId = RoleType.Madmate;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }
        public override void OnDeath(PlayerControl killer = null) { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void clearAndReload()
        {
            players = new List<Madmate>();
        }
    }
}