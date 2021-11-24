using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Mayor : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Mayor;

        public static CustomOptionBlank options;
        public static CustomOption mayorNumVotes;

        public static int numVotes { get { return (int)mayorNumVotes.getFloat(); } }

        public Mayor() : base()
        {
            NameColor = RoleColors.Mayor;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            mayorNumVotes = CustomOption.Create(81, "mayorNumVotes", 2f, 2f, 10f, 1f, options);
        }

        public static void Setup()
        {
        }
    }
}