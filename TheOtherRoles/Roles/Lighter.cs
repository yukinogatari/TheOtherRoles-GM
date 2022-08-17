using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Lighter : RoleBase<Lighter>
    {
        private static CustomButton lighterButton;

        public static Color color = new Color32(238, 229, 190, byte.MaxValue);

        public static float lighterModeLightsOnVision { get { return CustomOptionHolder.lighterModeLightsOnVision.getFloat(); } }
        public static float lighterModeLightsOffVision { get { return CustomOptionHolder.lighterModeLightsOffVision.getFloat(); } }
        public static bool canSeeNinja { get { return CustomOptionHolder.lighterCanSeeNinja.getBool(); } }

        public static float cooldown { get { return CustomOptionHolder.lighterCooldown.getFloat(); } }
        public static float duration { get { return CustomOptionHolder.lighterDuration.getFloat(); } }

        public bool lightActive = false;

        public Lighter()
        {
            RoleType = roleId = RoleType.Lighter;
            lightActive = false;
        }

        public static bool isLightActive(PlayerControl player)
        {
            if (isRole(player) && player.isAlive())
            {
                Lighter r = players.First(x => x.player == player);
                return r.lightActive;
            }
            return false;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }
        public override void OnDeath(PlayerControl killer = null) { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void MakeButtons(HudManager hm)
        {
            // Lighter light
            lighterButton = new CustomButton(
                () =>
                {
                    local.lightActive = true;
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Lighter) && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    local.lightActive = false;

                    lighterButton.Timer = lighterButton.MaxTimer;
                    lighterButton.isEffectActive = false;
                    lighterButton.actionButton.graphic.color = Palette.EnabledColor;
                },
                Lighter.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.UseButton,
                KeyCode.F,
                true,
                duration,
                () => {
                    local.lightActive = false;
                    lighterButton.Timer = lighterButton.MaxTimer;
                }
            );
            lighterButton.buttonText = ModTranslation.getString("LighterText");
        }

        public static void SetButtonCooldowns()
        {
            lighterButton.MaxTimer = cooldown;
            lighterButton.EffectDuration = duration;
        }

        public static void Clear()
        {
            players = new List<Lighter>();
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = ModTranslation.getImage("LighterButton", 115f);
            return buttonSprite;
        }
    }
}