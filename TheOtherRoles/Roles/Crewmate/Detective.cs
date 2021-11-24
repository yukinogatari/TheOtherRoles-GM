using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Detective : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Detective;

        public static CustomOptionBlank options;
        public static CustomOption detectiveAnonymousFootprints;
        public static CustomOption detectiveFootprintIntervall;
        public static CustomOption detectiveFootprintDuration;
        public static CustomOption detectiveReportNameDuration;
        public static CustomOption detectiveReportColorDuration;

        public static float footprintIntervall { get { return detectiveFootprintIntervall.getFloat(); } }
        public static float footprintDuration { get { return detectiveFootprintDuration.getFloat(); } }
        public static bool anonymousFootprints { get { return detectiveAnonymousFootprints.getBool(); } }
        public static float reportNameDuration { get { return detectiveReportNameDuration.getFloat(); } }
        public static float reportColorDuration { get { return detectiveReportColorDuration.getFloat(); } }

        public float timer = 6.2f;

        public Detective() : base()
        {
            NameColor = RoleColors.Detective;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            detectiveAnonymousFootprints = CustomOption.Create(121, "detectiveAnonymousFootprints", false, options);
            detectiveFootprintIntervall = CustomOption.Create(122, "detectiveFootprintIntervall", 0.5f, 0.25f, 10f, 0.25f, options, format: "unitSeconds");
            detectiveFootprintDuration = CustomOption.Create(123, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, options, format: "unitSeconds");
            detectiveReportNameDuration = CustomOption.Create(124, "detectiveReportNameDuration", 0, 0, 60, 2.5f, options, format: "unitSeconds");
            detectiveReportColorDuration = CustomOption.Create(125, "detectiveReportColorDuration", 20, 0, 120, 2.5f, options, format: "unitSeconds");
        }
    }
}