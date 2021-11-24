using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Spy : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Spy;

        public static CustomOptionBlank options;
        public static CustomOption spyCanDieToSheriff;
        public static CustomOption spyImpostorsCanKillAnyone;
        public static CustomOption spyCanEnterVents;
        public static CustomOption spyHasImpostorVision;

        public static bool impostorsCanKillAnyone { get { return spyImpostorsCanKillAnyone.getBool(); } }
        public static bool canEnterVents { get { return spyCanEnterVents.getBool(); } }
        public static bool hasImpostorVision { get { return spyHasImpostorVision.getBool(); } }

        public Spy() : base()
        {
            NameColor = RoleColors.Spy;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            spyCanDieToSheriff = CustomOption.Create(241, "spyCanDieToSheriff", false, options);
            spyImpostorsCanKillAnyone = CustomOption.Create(242, "spyImpostorsCanKillAnyone", true, options);
            spyCanEnterVents = CustomOption.Create(243, "spyCanEnterVents", false, options);
            spyHasImpostorVision = CustomOption.Create(244, "spyHasImpostorVision", false, options);
        }
    }
}