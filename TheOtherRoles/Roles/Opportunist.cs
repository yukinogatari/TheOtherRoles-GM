using System.Net;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using TheOtherRoles.Objects;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using TheOtherRoles.Patches;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Opportunist : Role<Opportunist>
    {
        public static Color color = new Color32(0, 255, 00, byte.MaxValue);

        public Opportunist() {
            roleType = RoleId.Opportunist;
        }

        public static void clearAndReload()
        {
            players = new List<Opportunist>();
        }
    }
}