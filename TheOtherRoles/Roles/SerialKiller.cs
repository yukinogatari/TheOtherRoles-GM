
using System.Linq;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class SerialKiller : RoleBase<SerialKiller> {

        private static CustomButton serialKillerButton;

        public static Color color = Palette.ImpostorRed;

        public static float killCooldown { get { return CustomOptionHolder.serialKillerKillCooldown.getFloat(); } }
        public static float suicideTimer { get { return CustomOptionHolder.serialKillerSuicideTimer.getFloat(); } }
        public static bool isCountDown = false;


        public SerialKiller()
        {
        }

        public override void OnMeetingStart()
        {
            if (player == PlayerControl.LocalPlayer)
            {
            }
        }

        public override void OnMeetingEnd() { }

        public override void FixedUpdate() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }


        public override void OnKill(PlayerControl target)
        {
            player.SetKillTimerUnchecked(killCooldown);
            serialKillerButton.Timer = suicideTimer;
            isCountDown = true;
        }

        public override void OnDeath(PlayerControl killer) { }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseKillButton.png", 115f);
            return buttonSprite;
        }

        public static void MakeButtons(HudManager hm)
        {
            // SerialKiller Suicide CountDown
            serialKillerButton = new CustomButton(
                () => {},
                () => {return PlayerControl.LocalPlayer.isRole(RoleId.SerialKiller) && !PlayerControl.LocalPlayer.Data.IsDead && isCountDown;},
                () => {return false;},
                () => {serialKillerButton.Timer = suicideTimer;},
                SerialKiller.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.AbilityButton,
                KeyCode.F,
                true,
                1f,
                () => {SerialKiller.suicide();}
            );
            serialKillerButton.buttonText = ModTranslation.getString("SerialKillerText");
            serialKillerButton.isEffectActive = true;
        }

        public static void suicide(){
                byte targetId = PlayerControl.LocalPlayer.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SerialKillerSuicide, Hazel.SendOption.Reliable, -1); killWriter.Write(targetId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.serialKillerSuicide(targetId);
        }

        public static void SetButtonCooldowns()
        {
            serialKillerButton.MaxTimer = SerialKiller.suicideTimer;
        }

        public static void Clear()
        {
            players = new List<SerialKiller>();
            isCountDown = false;
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
        class ExileControllerReEnableGameplayPatch {
            public static void Postfix(ExileController __instance) {
                PlayerControl player = PlayerControl.LocalPlayer;
                if(player.isRole(RoleId.SerialKiller))
                {
                    player.SetKillTimerUnchecked(killCooldown);
                    serialKillerButton.Timer = suicideTimer;
                }
            }
        }
    }
}