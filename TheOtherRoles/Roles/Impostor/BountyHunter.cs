using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class BountyHunter : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.BountyHunter;

        public static CustomOptionBlank options;
        public static CustomOption bountyHunterBountyDuration;
        public static CustomOption bountyHunterReducedCooldown;
        public static CustomOption bountyHunterPunishmentTime;
        public static CustomOption bountyHunterShowArrow;
        public static CustomOption bountyHunterArrowUpdateIntervall;

        public static float bountyDuration { get { return bountyHunterBountyDuration.getFloat(); } }
        public static bool showArrow { get { return bountyHunterShowArrow.getBool(); } }
        public static float bountyKillCooldown { get { return bountyHunterReducedCooldown.getFloat(); } }
        public static float punishmentTime { get { return bountyHunterPunishmentTime.getFloat(); } }
        public static float arrowUpdateIntervall { get { return bountyHunterArrowUpdateIntervall.getFloat(); } }

        public BountyHunter() : base()
        {
            NameColor = RoleColors.BountyHunter;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            bountyHunterBountyDuration = CustomOption.Create(321, "bountyHunterBountyDuration", 60f, 10f, 180f, 10f, options, format: "unitSeconds");
            bountyHunterReducedCooldown = CustomOption.Create(322, "bountyHunterReducedCooldown", 2.5f, 2.5f, 30f, 2.5f, options, format: "unitSeconds");
            bountyHunterPunishmentTime = CustomOption.Create(323, "bountyHunterPunishmentTime", 20f, 0f, 60f, 2.5f, options, format: "unitSeconds");
            bountyHunterShowArrow = CustomOption.Create(324, "bountyHunterShowArrow", true, options);
            bountyHunterArrowUpdateIntervall = CustomOption.Create(325, "bountyHunterArrowUpdateIntervall", 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow, format: "unitSeconds");
        }
    }
}