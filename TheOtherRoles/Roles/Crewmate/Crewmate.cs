using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class CustomCrewmateRole : CustomRole
    {
        public CustomCrewmateRole() : base()
        {
            Role = RoleTypes.Crewmate;
            NameColor = RoleColors.Crewmate;
            TasksCountTowardProgress = true;
            CanUseKillButton = false;
            CanBeKilled = true;
            CanVent = false;
            AffectedByLightAffectors = true;
            MaxCount = 15;
            TeamType = RoleTeamTypes.Crewmate;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }
    }
}