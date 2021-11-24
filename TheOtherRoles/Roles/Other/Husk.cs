using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Husk : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Husk;

        public static CustomOptionBlank options;
        public static CustomOption jesterCanCallEmergency;
        public static CustomOption jesterCanSabotage;

        public static bool canCallEmergency { get { return jesterCanCallEmergency.getBool(); } }
        public static bool canSabotage { get { return jesterCanSabotage.getBool(); } }

        public Husk() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.None;
            NameColor = RoleColors.Jester;
            MaxCount = 0;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public bool DidWin(GameOverReason gameOverReason)
        {
            return false;
        }

        public static void InitSettings()
        {
        }
    }
}