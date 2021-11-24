using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Swapper : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Swapper;

        public static CustomOptionBlank options;
        public static CustomOption swapperCanCallEmergency;
        public static CustomOption swapperCanOnlySwapOthers;

        public static bool canCallEmergency { get { return swapperCanCallEmergency.getBool(); } }
        public static bool canOnlySwapOthers { get { return swapperCanOnlySwapOthers.getBool(); } }

        internal byte playerId1;
        internal byte playerId2;

        public Swapper() : base()
        {
            NameColor = RoleColors.Swapper;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            swapperCanCallEmergency = CustomOption.Create(151, "swapperCanCallEmergency", false, options);
            swapperCanOnlySwapOthers = CustomOption.Create(152, "swapperCanOnlySwapOthers", false, options);
        }
    }
}