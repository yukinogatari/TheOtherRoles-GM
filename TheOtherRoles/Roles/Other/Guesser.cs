using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Guesser : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Guesser;

        public static CustomOptionBlank options;
        public static CustomOption guesserIsImpRate;
        public static CustomOption guesserNumberOfShots;
        public static CustomOption guesserOnlyAvailableRoles;
        public static CustomOption guesserHasMultipleShotsPerMeeting;

        public int remainingShots = 2;
        public static int totalShots { get { return Mathf.RoundToInt(guesserNumberOfShots.getFloat()); } }
        public static bool onlyAvailableRoles { get { return guesserOnlyAvailableRoles.getBool(); } }
        public static bool hasMultipleShotsPerMeeting { get { return guesserHasMultipleShotsPerMeeting.getBool(); } }

        public Guesser() : base()
        {
            NameColor = RoleColors.Guesser;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            guesserIsImpRate = CustomOption.Create(311, "guesserIsImpGuesserRate", CustomOptionHolder.rates, options);
            guesserNumberOfShots = CustomOption.Create(312, "guesserNumberOfShots", 2f, 1f, 15f, 1f, options, format: "unitShots");
            guesserHasMultipleShotsPerMeeting = CustomOption.Create(314, "guesserHasMultipleShotsPerMeeting", false, options);
            guesserOnlyAvailableRoles = CustomOption.Create(313, "guesserOnlyAvailableRoles", true, options);
        }
    }

    class EvilGuesser : Guesser
    {
        public EvilGuesser() : base()
        {
            NameColor = RoleColors.EvilGuesser;
            TeamType = RoleTeamTypes.Impostor;
        }
    }

    class NiceGuesser : Guesser
    {
        public NiceGuesser() : base()
        {
            NameColor = RoleColors.NiceGuesser;
            TeamType = RoleTeamTypes.Crewmate;
        }
    }
}