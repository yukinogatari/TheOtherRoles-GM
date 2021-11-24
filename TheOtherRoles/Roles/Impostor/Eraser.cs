using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Eraser : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Eraser;

        public static CustomOptionBlank options;
        public static CustomOption eraserCooldown;
        public static CustomOption eraserCooldownIncrease;
        public static CustomOption eraserCanEraseAnyone;

        public static float cooldown { get { return eraserCooldown.getFloat(); } }
        public static float cooldownIncrease { get { return eraserCooldownIncrease.getFloat(); } }
        public static bool canEraseAnyone { get { return eraserCanEraseAnyone.getBool(); } }

        public Eraser() : base()
        {
            NameColor = RoleColors.Eraser;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            eraserCooldown = CustomOption.Create(231, "eraserCooldown", 30f, 5f, 120f, 5f, options, format: "unitSeconds");
            eraserCooldownIncrease = CustomOption.Create(233, "eraserCooldownIncrease", 10f, 0f, 120f, 2.5f, options, format: "unitSeconds");
            eraserCanEraseAnyone = CustomOption.Create(232, "eraserCanEraseAnyone", false, options);
        }
    }
}