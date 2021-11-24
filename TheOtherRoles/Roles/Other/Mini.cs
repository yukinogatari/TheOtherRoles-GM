using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Mini : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Mini;

        public static CustomOptionBlank options;
        public static CustomOption miniGrowingUpDuration;
        public static CustomOption miniIsImpRate;

        public static float growingUpDuration { get { return miniGrowingUpDuration.getFloat(); } }
        public bool isGrownUp = false;
        public static bool triggerMiniLose = false;
        public static PlayerControl exiled;

        public Mini() : base()
        {
            NameColor = RoleColors.Mini;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void Setup()
        {
            triggerMiniLose = false;
            exiled = null;
        }

        public override void OnExiled()
        {
            if (!isGrownUp && !IsImpostor)
            {
                triggerMiniLose = true;
                exiled = Player;
            }
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            miniGrowingUpDuration = CustomOption.Create(181, "miniGrowingUpDuration", 400f, 100f, 1500f, 100f, options, format: "unitSeconds");
            miniIsImpRate = CustomOption.Create(182, "miniIsImpRate", CustomOptionHolder.rates, options);
        }
    }

    class EvilMini : Mini
    {
        public EvilMini() : base()
        {
            NameColor = RoleColors.EvilMini;
            TeamType = RoleTeamTypes.Impostor;
        }
    }

    class NiceMini : Mini
    {
        public NiceMini() : base()
        {
            NameColor = RoleColors.NiceMini;
            TeamType = RoleTeamTypes.Crewmate;
        }
    }
}