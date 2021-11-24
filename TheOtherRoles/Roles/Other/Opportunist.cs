using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Opportunist : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Opportunist;

        public static CustomOptionBlank options;

        public Opportunist() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.Opportunist;
            NameColor = RoleColors.Opportunist;
            TasksCountTowardProgress = false;
            CanUseKillButton = false;
            CanVent = false;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
        }

        public static void Setup()
        {
        }
    }
}