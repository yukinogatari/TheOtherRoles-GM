using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Warlock : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Warlock;

        public static CustomOptionBlank options;
        public static CustomOption warlockCooldown;
        public static CustomOption warlockRootTime;

        public static float cooldown { get { return warlockCooldown.getFloat(); } }
        public static float rootTime { get { return warlockRootTime.getFloat(); } }

        internal PlayerControl curseKillTarget;

        public Warlock() : base()
        {
            NameColor = RoleColors.Warlock;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            warlockCooldown = CustomOption.Create(271, "warlockCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            warlockRootTime = CustomOption.Create(272, "warlockRootTime", 5f, 0f, 15f, 1f, options, format: "unitSeconds");
        }
    }
}