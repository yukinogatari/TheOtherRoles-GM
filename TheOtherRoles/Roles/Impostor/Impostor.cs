using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class CustomImpostorRole : CustomRole
    {
        public CustomImpostorRole() : base()
        {
            Role = (RoleTypes)CustomRoleTypes.Impostor;
            NameColor = RoleColors.Impostor;
            TasksCountTowardProgress = false;
            CanUseKillButton = true;
            CanBeKilled = false;
            CanVent = true;
            AffectedByLightAffectors = false;
            MaxCount = 15;
            TeamType = RoleTeamTypes.Impostor;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }
    }
}