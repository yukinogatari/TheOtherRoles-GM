using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Opportunist : RoleBase<Opportunist>
    {
        public static Color color = new Color32(0, 255, 00, byte.MaxValue);

        public Opportunist()
        {
            RoleType = roleId = RoleId.Opportunist;
        }

        public static void clearAndReload()
        {
            players = new List<Opportunist>();
        }
    }
}