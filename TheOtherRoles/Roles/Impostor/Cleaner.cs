using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Cleaner : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Cleaner;

        public static CustomOptionBlank options;
        public static CustomOption cleanerCooldown;

        public static float cooldown { get { return cleanerCooldown.getFloat(); } }

        public Cleaner() : base()
        {
            NameColor = RoleColors.Cleaner;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            cleanerCooldown = CustomOption.Create(261, "cleanerCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
        }
    }
}