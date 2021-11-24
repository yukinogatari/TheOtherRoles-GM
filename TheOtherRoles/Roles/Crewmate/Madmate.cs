using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Madmate : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Madmate;

        public static CustomOptionBlank options;
        public static CustomOption madmateCanDieToSheriff;
        public static CustomOption madmateCanEnterVents;
        public static CustomOption madmateHasImpostorVision;
        public static CustomOption madmateCanSabotage;
        public static CustomOption madmateCanFixComm;
        public static CustomOption madmateCanBeGA;

        public static bool canEnterVents { get { return madmateCanEnterVents.getBool(); } }
        public static bool hasImpostorVision { get { return madmateHasImpostorVision.getBool(); } }
        public static bool canSabotage { get { return madmateCanSabotage.getBool(); } }
        public static bool canFixComm { get { return madmateCanFixComm.getBool(); } }
        public static bool canBeGA { get { return madmateCanBeGA.getBool(); } }

        public Madmate() : base()
        {
            NameColor = RoleColors.Madmate;
            TasksCountTowardProgress = false;
            MaxCount = 15;
            CanVent = true;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public override void Init()
        {
            base.Init();
        }

        public override void _RoleUpdate()
        {
            var hm = DestroyableSingleton<HudManager>.Instance;

            CanVent = canEnterVents;
            hm?.ImpostorVentButton?.gameObject?.SetActive(canEnterVents);
            hm?.ImpostorVentButton?.ToggleVisible(canEnterVents && Helpers.ShowButtons);

            hm?.SabotageButton?.gameObject?.SetActive(canSabotage);
            hm?.SabotageButton?.ToggleVisible(canSabotage && Helpers.ShowButtons);
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            madmateCanDieToSheriff = CustomOption.Create(361, "madmateCanDieToSheriff", false, options);
            madmateCanEnterVents = CustomOption.Create(362, "madmateCanEnterVents", false, options);
            madmateHasImpostorVision = CustomOption.Create(363, "madmateHasImpostorVision", false, options);
            madmateCanSabotage = CustomOption.Create(364, "madmateCanSabotage", false, options);
            madmateCanFixComm = CustomOption.Create(365, "madmateCanFixComm", true, options);
            madmateCanBeGA = CustomOption.Create(366, "madmateCanBeGA", true, options);
        }
    }
}