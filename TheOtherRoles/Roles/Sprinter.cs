using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using System;
using Hazel;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Sprinter : RoleBase<Sprinter>
    {
        private static CustomButton sprintButton;
        public static Color color = new Color32(128, 128, 255, byte.MaxValue);
        public static float sprintCooldown { get { return CustomOptionHolder.sprinterCooldown.getFloat(); } }
        public static float sprintDuration { get { return CustomOptionHolder.sprinterDuration.getFloat(); } }
        public static float speedBonus { get { return CustomOptionHolder.sprinterSpeedBonus.getFloat() / 100f; } }

        private bool sprinting = false;

        public Sprinter()
        {
            sprinting = false;
            RoleType = roleId = RoleType.Sprinter;
        }

        public override void OnMeetingStart()
        {
            sprinting = false;
            sprintButton.isEffectActive = false;
            sprintButton.Timer = sprintButton.MaxTimer = sprintCooldown;
        }

        public override void OnMeetingEnd() { }

        public override void ResetRole()
        {
            sprinting = false;
            sprintButton.isEffectActive = false;
            sprintButton.Timer = sprintButton.MaxTimer = sprintCooldown;
        }

        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }

        public override void OnDeath(PlayerControl killer)
        {
            sprintButton.isEffectActive = false;
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SprintButton.png", 115f);
            return buttonSprite;
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void MakeButtons(HudManager hm)
        {
            sprintButton = new CustomButton(
                () => {
                    if (sprintButton.isEffectActive)
                    {
                        sprintButton.Timer = 0;
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SprinterSprint, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.sprinterSprint(PlayerControl.LocalPlayer.PlayerId, true);
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Sprinter) && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => {
                    if (sprintButton.isEffectActive)
                    {
                        sprintButton.buttonText = ModTranslation.getString("SprinterStopText");
                    }
                    else
                    {
                        sprintButton.buttonText = ModTranslation.getString("SprinterText");
                    }
                    return PlayerControl.LocalPlayer.CanMove;
                },
                () => {
                    sprintButton.Timer = sprintButton.MaxTimer = sprintCooldown;
                },
                getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.AbilityButton,
                KeyCode.F,
                true,
                sprintDuration,
                () => {
                    sprintButton.Timer = sprintButton.MaxTimer = sprintCooldown;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SprinterSprint, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.sprinterSprint(PlayerControl.LocalPlayer.PlayerId, false);
                }
            );
            sprintButton.buttonText = ModTranslation.getString("SprinterText");
            sprintButton.effectCancellable = true;
        }

        public static void SetButtonCooldowns()
        {
            sprintButton.MaxTimer = sprintCooldown;
        }

        public static bool isSprinting(PlayerControl player)
        {
            if (isRole(player) && player.isAlive())
            {
                var p = players.First(x => x.player == player);
                return p.sprinting;
            }
            return false;
        }

        public static void setSprinting(PlayerControl player, bool sprinting = true)
        {
            if (isRole(player))
            {
                var p = players.First(x => x.player == player);
                p.sprinting = sprinting;
            }
        }

        public static void Clear()
        {
            players = new List<Sprinter>();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsSprinterPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.AmOwner && __instance.myPlayer.CanMove && GameData.Instance && isSprinting(__instance.myPlayer))
                {
                    __instance.body.velocity *= speedBonus;
                }
            }
        }
    }
}