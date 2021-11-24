using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Tracker : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Tracker;

        public static CustomOptionBlank options;
        public static CustomOption trackerUpdateIntervall;
        public static CustomOption trackerResetTargetAfterMeeting;

        public static float updateInterval { get { return trackerUpdateIntervall.getFloat(); } }
        public static bool resetTargetAfterMeeting { get { return trackerResetTargetAfterMeeting.getBool(); } }

        public Tracker() : base()
        {
            NameColor = RoleColors.Tracker;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            trackerUpdateIntervall = CustomOption.Create(201, "trackerUpdateIntervall", 5f, 2.5f, 30f, 2.5f, options, format: "unitSeconds");
            trackerResetTargetAfterMeeting = CustomOption.Create(202, "trackerResetTargetAfterMeeting", false, options);
        }
    }
}