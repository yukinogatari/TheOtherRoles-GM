using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Shifter : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Shifter;

        public static CustomOptionBlank options;
        public static CustomOption shifterShiftsModifiers;

        public static bool shiftModifiers { get { return shifterShiftsModifiers.getBool(); } }

        public Shifter() : base()
        {
            NameColor = RoleColors.Shifter;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            shifterShiftsModifiers = CustomOption.Create(71, "shifterShiftsModifiers", false, options);
        }
    }
}