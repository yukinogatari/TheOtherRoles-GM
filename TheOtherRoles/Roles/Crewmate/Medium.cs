using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Medium : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Medium;

        public static CustomOptionBlank options;
        public static CustomOption mediumCooldown;
        public static CustomOption mediumDuration;
        public static CustomOption mediumOneTimeUse;

        public static float cooldown { get { return mediumCooldown.getFloat(); } }
        public static float duration { get { return mediumDuration.getFloat(); } }
        public static bool oneTimeUse { get { return mediumOneTimeUse.getBool(); } }

        public Medium() : base()
        {
            NameColor = RoleColors.Medium;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            mediumCooldown = CustomOption.Create(371, "mediumCooldown", 30f, 5f, 120f, 5f, options, format: "unitSeconds");
            mediumDuration = CustomOption.Create(372, "mediumDuration", 3f, 0f, 15f, 1f, options, format: "unitSeconds");
            mediumOneTimeUse = CustomOption.Create(373, "mediumOneTimeUse", false, options);
        }
    }
}