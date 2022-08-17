using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Immoralist : RoleBase<Immoralist>
    {
        public static Color color = Fox.color;
        private static CustomButton immoralistButton;

        public static List<Arrow> arrows = new List<Arrow>();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 1f;

        public Immoralist()
        {
            RoleType = roleId = RoleType.Immoralist;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer.isRole(RoleType.Immoralist))
            {
                arrowUpdate();
            }
        }

        public override void OnKill(PlayerControl target) { }

        public override void OnDeath(PlayerControl killer = null)
        {
            player.clearAllTasks();
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void SetButtonCooldowns() { }

        public static void Clear()
        {
            foreach(Arrow arrow in arrows)
            {
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            }
            arrows = new List<Arrow>();
            players = new List<Immoralist>();
        }

        public static void suicide()
        {
            byte targetId = PlayerControl.LocalPlayer.PlayerId;
            MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SerialKillerSuicide, Hazel.SendOption.Reliable, -1); killWriter.Write(targetId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.serialKillerSuicide(targetId);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
            return buttonSprite;
        }

        public static void MakeButtons(HudManager hm)
        {
            // Fox stealth
            immoralistButton = new CustomButton(
                () =>
                {
                    suicide();
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Immoralist) && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return true; },
                () =>
                {
                    immoralistButton.Timer = immoralistButton.MaxTimer = 20f;
                },
                getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                false,
                0,
                () => { }
            );
            immoralistButton.buttonText = ModTranslation.getString("immoralistSuicideText");
            immoralistButton.effectCancellable = true;
        }

        static void arrowUpdate()
        {
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {
                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrow一覧
                arrows = new List<Arrow>();

                // 狐の位置を示すArrowを描画
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    if (p.isRole(RoleType.Fox))
                    {
                        arrow = new Arrow(Fox.color);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
            else
            {
                arrows.Do(x => x.Update());
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public static class MurderPlayerPatch
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                PlayerControl player = PlayerControl.LocalPlayer;
                if (player.isRole(RoleType.Immoralist) && player.isAlive())
                {
                    HudManager.Instance.FullScreen.enabled = true;
                    HudManager.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
                    {
                        var renderer = HudManager.Instance.FullScreen;
                        if (p < 0.5)
                        {
                            if (renderer != null)
                                renderer.color = new Color(42f / 255f, 187f / 255f, 245f / 255f, Mathf.Clamp01(p * 2 * 0.75f));
                        }
                        else
                        {
                            if (renderer != null)
                                renderer.color = new Color(42f / 255f, 187f / 255f, 245f / 255f, Mathf.Clamp01((1 - p) * 2 * 0.75f));
                        }
                        if (p == 1f && renderer != null) renderer.enabled = false;
                    })));
                }
            }
        }
    }
}