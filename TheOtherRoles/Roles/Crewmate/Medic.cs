using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Medic : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Medic;
        public static Color ShieldColor = new Color32(0, 221, 255, byte.MaxValue);

        public static CustomOptionBlank options;
        public static CustomOption medicShowShielded;
        public static CustomOption medicShowAttemptToShielded;
        public static CustomOption medicSetShieldAfterMeeting;

        public static int showShielded { get { return medicShowShielded.getSelection(); } }
        public static bool showAttemptToShielded { get { return medicShowAttemptToShielded.getBool(); } }
        public static bool setShieldAfterMeeting { get { return medicSetShieldAfterMeeting.getBool(); } }

        public PlayerControl shielded;
        public PlayerControl futureShielded;
        public bool usedShield;

        public Medic() : base()
        {
            NameColor = RoleColors.Medic;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            medicShowShielded = CustomOption.Create(143, "medicShowShielded", new string[] { "medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic" }, options);
            medicShowAttemptToShielded = CustomOption.Create(144, "medicShowAttemptToShielded", false, options);
            medicSetShieldAfterMeeting = CustomOption.Create(145, "medicSetShieldAfterMeeting", false, options);
        }
    }
}