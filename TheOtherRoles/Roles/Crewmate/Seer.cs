using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Seer : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Seer;

        public static CustomOptionBlank options;
        public static CustomOption seerMode;
        public static CustomOption seerSoulDuration;
        public static CustomOption seerLimitSoulDuration;

        public static float soulDuration { get { return seerSoulDuration.getFloat(); } }
        public static bool limitSoulDuration { get { return seerLimitSoulDuration.getBool(); } }
        public static int mode { get { return seerMode.getSelection(); } }

        public Seer() : base()
        {
            NameColor = RoleColors.Seer;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            seerMode = CustomOption.Create(161, "seerMode", new string[] { "seerModeBoth", "seerModeFlash", "seerModeSouls" }, options);
            seerLimitSoulDuration = CustomOption.Create(163, "seerLimitSoulDuration", false, options);
            seerSoulDuration = CustomOption.Create(162, "seerSoulDuration", 15f, 0f, 60f, 5f, seerLimitSoulDuration, format: "unitSeconds");
        }
    }
}