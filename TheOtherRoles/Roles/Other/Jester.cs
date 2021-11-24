using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Jester : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Jester;

        public static CustomOptionBlank options;
        public static CustomOption jesterCanCallEmergency;
        public static CustomOption jesterCanSabotage;
        public static bool triggerJesterWin = false;
        public static PlayerControl winner;

        public static bool canCallEmergency { get { return jesterCanCallEmergency.getBool(); } }
        public static bool canSabotage { get { return jesterCanSabotage.getBool(); } }
        public bool wasEjected = false;

        public Jester() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.Jester;
            NameColor = RoleColors.Jester;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void Setup()
        {
            triggerJesterWin = false;
            winner = null;
        }

        public override void _RoleUpdate()
        {
            var hm = DestroyableSingleton<HudManager>.Instance;

            hm?.SabotageButton?.gameObject?.SetActive(canSabotage);
            hm?.SabotageButton?.ToggleVisible(canSabotage && Helpers.ShowButtons);
        }

        public override void OnExiled()
        {
            triggerJesterWin = true;
            winner = Player;
        }

        public override bool DidWin(GameOverReason gameOverReason)
        {
            return triggerJesterWin && Player == winner && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            jesterCanCallEmergency = CustomOption.Create(61, "jesterCanCallEmergency", true, options);
            jesterCanSabotage = CustomOption.Create(62, "jesterCanSabotage", true, options);
        }
    }
}