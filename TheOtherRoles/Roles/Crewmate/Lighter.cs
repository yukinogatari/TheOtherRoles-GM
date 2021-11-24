using HarmonyLib;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Lighter : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Lighter;

        public static CustomOptionBlank options;
        public static CustomOption lighterModeLightsOnVision;
        public static CustomOption lighterModeLightsOffVision;
        public static CustomOption lighterCooldown;
        public static CustomOption lighterDuration;

        private static Sprite buttonSprite;
        public CustomButton lighterButton;

        public static float lightsOnVision { get { return lighterModeLightsOnVision.getFloat(); } }
        public static float lightsOffVision { get { return lighterModeLightsOffVision.getFloat(); } }

        public static float cooldown { get { return lighterCooldown.getFloat(); } }
        public static float duration { get { return lighterDuration.getFloat(); } }

        public Lighter() : base()
        {
            NameColor = RoleColors.Lighter;
            MaxCount = 15;
        }

        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = ModTranslation.getImage("LighterButton", 115f);
            return buttonSprite;
        }

        public override bool InitButtons()
        {
            if (!DestroyableSingleton<HudManager>.InstanceExists) return false;
            if (lighterButton != null) return true;

            InitializeAbilityButton();
            lighterButton = new CustomButton(
                () => {
                    PlayerControl.LocalPlayer.RpcSetRoleModifier(RoleModifierTypes.LighterLight);
                },
                () => {
                    return !PlayerControl.LocalPlayer.Data.IsDead;
                },
                () => {
                    return PlayerControl.LocalPlayer.CanMove;
                },
                () => {
                    lighterButton.Timer = lighterButton.MaxTimer;
                    lighterButton.isEffectActive = false;
                    lighterButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Lighter.getButtonSprite(),
                new Vector3(-1.85f, -1.2f, 0.0f),
                DestroyableSingleton<HudManager>.Instance.KillButton,
                DestroyableSingleton<HudManager>.Instance.UseButton,
                KeyCode.F,
                true,
                Lighter.duration,
                () => {
                    lighterButton.Timer = lighterButton.MaxTimer;
                    PlayerControl.LocalPlayer.RpcRemoveRoleModifier(RoleModifierTypes.LighterLight);
                }
            );
            lighterButton.MaxTimer = cooldown;
            lighterButton.buttonText = ModTranslation.getString("LighterText");

            return true;
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            lighterModeLightsOnVision = CustomOption.Create(111, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, options, format: "unitMultiplier");
            lighterModeLightsOffVision = CustomOption.Create(112, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, options, format: "unitMultiplier");
            lighterCooldown = CustomOption.Create(113, "lighterCooldown", 30f, 5f, 120f, 5f, options, format: "unitSeconds");
            lighterDuration = CustomOption.Create(114, "lighterDuration", 5f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
        }

        public override void _RoleUpdate()
        {
        }
    }
}