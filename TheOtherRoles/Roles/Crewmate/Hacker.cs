using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Hacker : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Hacker;

        public static CustomOptionBlank options;
        public static CustomOption hackerCooldown;
        public static CustomOption hackerHackeringDuration;
        public static CustomOption hackerOnlyColorType;

        public static float cooldown { get { return hackerCooldown.getFloat(); } }
        public static float duration { get { return hackerHackeringDuration.getFloat(); } }
        public static bool onlyColorType { get { return hackerOnlyColorType.getBool(); } }

        public float hackerTimer = 0f;

        public Hacker() : base()
        {
            NameColor = RoleColors.Hacker;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            hackerCooldown = CustomOption.Create(171, "hackerCooldown", 30f, 5f, 60f, 5f, options, format: "unitSeconds");
            hackerHackeringDuration = CustomOption.Create(172, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            hackerOnlyColorType = CustomOption.Create(173, "hackerOnlyColorType", false, options);
        }
    }
}