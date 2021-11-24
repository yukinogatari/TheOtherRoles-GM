using HarmonyLib;
using System;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class TimeMaster : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.TimeMaster;

        public static CustomOptionBlank options;
        public static CustomOption timeMasterCooldown;
        public static CustomOption timeMasterRewindTime;
        public static CustomOption timeMasterShieldDuration;

        public static float rewindTime { get { return timeMasterRewindTime.getFloat(); } }
        public static float shieldDuration { get { return timeMasterShieldDuration.getFloat(); } }
        public static float cooldown { get { return timeMasterCooldown.getFloat(); } }
        internal static bool isRewinding;
        internal bool shieldActive;

        public TimeMaster() : base()
        {
            NameColor = RoleColors.TimeMaster;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            timeMasterCooldown = CustomOption.Create(131, "timeMasterCooldown", 30f, 2.5f, 120f, 2.5f, options, format: "unitSeconds");
            timeMasterRewindTime = CustomOption.Create(132, "timeMasterRewindTime", 3f, 1f, 10f, 1f, options, format: "unitSeconds");
            timeMasterShieldDuration = CustomOption.Create(133, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, options, format: "unitSeconds");
        }

        internal void resetTimeMasterButton()
        {
        }
    }
}