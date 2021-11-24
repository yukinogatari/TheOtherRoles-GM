using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Snitch : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Snitch;

        public static CustomOptionBlank options;
        public static CustomOption snitchLeftTasksForReveal;
        public static CustomOption snitchIncludeTeamJackal;
        public static CustomOption snitchTeamJackalUseDifferentArrowColor;

        public static int taskCountForReveal { get { return Mathf.RoundToInt(snitchLeftTasksForReveal.getFloat()); } }
        public static bool includeTeamJackal { get { return snitchIncludeTeamJackal.getBool(); } }
        public static bool teamJackalUseDifferentArrowColor { get { return snitchTeamJackalUseDifferentArrowColor.getBool(); } }

        public Snitch() : base()
        {
            NameColor = RoleColors.Snitch;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            snitchLeftTasksForReveal = CustomOption.Create(211, "snitchLeftTasksForReveal", 1f, 0f, 5f, 1f, options);
            snitchIncludeTeamJackal = CustomOption.Create(212, "snitchIncludeTeamJackal", false, options);
            snitchTeamJackalUseDifferentArrowColor = CustomOption.Create(213, "snitchTeamJackalUseDifferentArrowColor", true, snitchIncludeTeamJackal);
        }
    }
}