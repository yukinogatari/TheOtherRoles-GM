using HarmonyLib;
using System;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Trickster : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Trickster;

        public static CustomOptionBlank options;
        public static CustomOption tricksterPlaceBoxCooldown;
        public static CustomOption tricksterLightsOutCooldown;
        public static CustomOption tricksterLightsOutDuration;

        public static float placeBoxCooldown { get { return tricksterPlaceBoxCooldown.getFloat(); } }
        public static float lightsOutCooldown { get { return tricksterLightsOutCooldown.getFloat(); } }
        public static float lightsOutDuration { get { return tricksterLightsOutDuration.getFloat(); } }

        // no point in having multiple lights outs happening at the same time
        public static float lightsOutTimer = 0f;
        public static bool lightsOutActive = false;

        public Trickster() : base()
        {
            NameColor = RoleColors.Trickster;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            tricksterPlaceBoxCooldown = CustomOption.Create(251, "tricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, options, format: "unitSeconds");
            tricksterLightsOutCooldown = CustomOption.Create(252, "tricksterLightsOutCooldown", 30f, 5f, 60f, 5f, options, format: "unitSeconds");
            tricksterLightsOutDuration = CustomOption.Create(253, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, options, format: "unitSeconds");
        }

        public static Sprite getTricksterVentButtonSprite()
        {
            throw new NotImplementedException();
        }
    }
}