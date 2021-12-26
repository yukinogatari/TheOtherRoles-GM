using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Madmate : RoleBase<Madmate>
    {
        public static Color color = Palette.ImpostorRed;

        public static bool canEnterVents = false;
        public static bool hasImpostorVision = false;
        public static bool canSabotage = false;
        public static bool canFixComm = true;

        public Madmate()
        {
            RoleType = roleId = RoleId.Madmate;
        }

        public static void clearAndReload()
        {
            players = new List<Madmate>();
            canEnterVents = CustomOptionHolder.madmateCanEnterVents.getBool();
            hasImpostorVision = CustomOptionHolder.madmateHasImpostorVision.getBool();
            canSabotage = CustomOptionHolder.madmateCanSabotage.getBool();
            canFixComm = CustomOptionHolder.madmateCanFixComm.getBool();
        }
    }
}