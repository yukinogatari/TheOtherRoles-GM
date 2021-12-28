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
            RoleType = roleId = RoleId.Madmate;
        }

        public static void clearAndReload()
        {
            players = new List<Madmate>();
        }
    }
}