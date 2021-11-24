using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class GM : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.GM;

        public static CustomOptionBlank options;
        public static CustomOption gmIsHost;
        public static CustomOption gmHasTasks;
        public static CustomOption gmDiesAtStart;
        public static CustomOption gmCanWarp;
        public static CustomOption gmCanKill;

        public static bool isHost { get { return gmIsHost.getBool(); } }
        public static bool diesAtStart { get { return gmDiesAtStart.getBool(); } }
        public static bool hasTasks = false;
        public static bool canSabotage = false;
        public static bool canWarp { get { return gmCanWarp.getBool(); } }
        public static bool canKill { get { return gmCanKill.getBool(); } }

        public GM() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.None;
            NameColor = RoleColors.GM;
            MaxCount = 1;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            gmIsHost = CustomOption.Create(401, "gmIsHost", true, options);
            //gmHasTasks = CustomOption.Create(402, "gmHasTasks", false, options);
            gmCanWarp = CustomOption.Create(405, "gmCanWarp", true, options);
            gmCanKill = CustomOption.Create(406, "gmCanKill", false, options);
            gmDiesAtStart = CustomOption.Create(404, "gmDiesAtStart", true, options);
        }
    }
}