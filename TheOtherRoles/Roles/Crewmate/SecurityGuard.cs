using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class SecurityGuard : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.SecurityGuard;

        public static CustomOptionBlank options;
        public static CustomOption securityGuardCooldown;
        public static CustomOption securityGuardTotalScrews;
        public static CustomOption securityGuardCamPrice;
        public static CustomOption securityGuardVentPrice;

        public static float cooldown { get { return securityGuardCooldown.getFloat(); } }
        public static int remainingScrews = 7;
        public static int totalScrews { get { return Mathf.RoundToInt(securityGuardTotalScrews.getFloat()); } }
        public static int ventPrice { get { return Mathf.RoundToInt(securityGuardCamPrice.getFloat()); } }
        public static int camPrice { get { return Mathf.RoundToInt(securityGuardVentPrice.getFloat()); } }

        public SecurityGuard() : base()
        {
            NameColor = RoleColors.SecurityGuard;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            securityGuardCooldown = CustomOption.Create(281, "securityGuardCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            securityGuardTotalScrews = CustomOption.Create(282, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, options, format: "unitScrews");
            securityGuardCamPrice = CustomOption.Create(283, "securityGuardCamPrice", 2f, 1f, 15f, 1f, options, format: "unitScrews");
            securityGuardVentPrice = CustomOption.Create(284, "securityGuardVentPrice", 1f, 1f, 15f, 1f, options, format: "unitScrews");
        }
    }
}