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
    public class Madmate : Role<Madmate>
    {
        public static Color color = Palette.ImpostorRed;

        public static bool canEnterVents = false;
        public static bool hasImpostorVision = false;
        public static bool canSabotage = false;
        public static bool canFixComm = true;

        public Madmate() {
            roleType = RoleId.Madmate;
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