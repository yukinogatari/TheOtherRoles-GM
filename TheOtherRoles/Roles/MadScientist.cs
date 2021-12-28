using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class MadScientist : RoleBase<MadScientist>
    {
        public static PlayerControl madScientist;
        private static CustomButton madScientistSyringeButton;
        public static Color color = new Color(00f / 255f, 255f / 255f, 00f / 255f, 1);
        public static PlayerControl currentTarget;
        public static Dictionary<int, PlayerControl> infected;
        public static Dictionary<int, float> progress;
        public static TMPro.TMP_Text text = null;
        public static bool triggerMadScientistWin = false;
        public static bool syringeFlag = false;
        public static bool meetingFlag = false;

        public static Sprite buttonSylinge;
        public static float duration;
        public static float distance;
        public static float infectDistance { get { return CustomOptionHolder.madScientistDistance.getFloat();}}
        public static float infectDuration { get { return CustomOptionHolder.madScientistDuration.getFloat();}}

        public MadScientist()
        {
            RoleType = roleId = RoleId.MadScientist;
        }

        public override void OnMeetingStart() { 
            meetingFlag = true;
        }
        public override void OnMeetingEnd() {
            HudManager.Instance.StartCoroutine(Effects.Lerp(5.0f, new Action<float>((p) => { // 5秒後から感染開始
                if (p == 1f) {
                    meetingFlag = false;
                }
            })));
         }

        public override void FixedUpdate() {
            if (player == PlayerControl.LocalPlayer && !syringeFlag)
            {
                currentTarget = setTarget();
                setPlayerOutline(currentTarget, MadScientist.color);
            }
            Update(player);
            TextUpdate(player);
        }

        private static void Update(PlayerControl player){
            if(PlayerControl.LocalPlayer == player && !MadScientist.meetingFlag) {
                List<PlayerControl> newInfected = new List<PlayerControl>();
                foreach(PlayerControl p1 in PlayerControl.AllPlayerControls){ // 非感染プレイヤーのループ
                    if(p1 == player || p1.Data.IsDead || MadScientist.infected.ContainsKey(p1.Data.PlayerId)) continue;
                    // データが無い場合は作成する
                    if(!MadScientist.progress.ContainsKey(p1.Data.PlayerId)){
                        MadScientist.progress[p1.Data.PlayerId] = 0f;
                    }
                    foreach(int key in MadScientist.infected.Keys){ // 感染プレイヤーのループ
                        if(MadScientist.infected[key].Data.IsDead) continue;
                        float distance = Vector3.Distance(MadScientist.infected[key].transform.position, p1.transform.position);
                        // 障害物判定
                        bool anythingBetween = PhysicsHelpers.AnythingBetween(MadScientist.infected[key].GetTruePosition(), p1.GetTruePosition(), Constants.ShipAndObjectsMask, false);

                        if(distance <= CustomOptionHolder.madScientistDistance.getFloat() && !anythingBetween){
                            MadScientist.progress[p1.Data.PlayerId] += Time.fixedDeltaTime;

							// 他のクライアントに進行状況を通知する
							MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MadScientistUpdateProgress, Hazel.SendOption.Reliable, -1);
							writer.Write(p1.PlayerId);
							writer.Write(MadScientist.progress[p1.Data.PlayerId]);
							AmongUsClient.Instance.FinishRpcImmediately(writer);

                            // 既定値を超えたら感染扱いにする
                            if(MadScientist.progress[p1.Data.PlayerId] >= CustomOptionHolder.madScientistDuration.getFloat()){
                                newInfected.Add(p1);
                            }
                        }

                    }
                }

                // 感染者に追加する
                foreach(PlayerControl p in newInfected){
                    byte targetId = p.Data.PlayerId;
                    MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MadScientistSetInfected, Hazel.SendOption.Reliable, -1); killWriter.Write(targetId);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCProcedure.setInfected(targetId);
                }

                // 勝利条件を満たしたか確認する
                bool winFlag = true;
                foreach(PlayerControl p in PlayerControl.AllPlayerControls){
                    if(p.Data.IsDead) continue;
                    if(p == player) continue;
                    if(!MadScientist.infected.ContainsKey(p.Data.PlayerId)) winFlag = false;
                }
                if(winFlag){
                    MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MadScientistWin, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                    RPCProcedure.madScientistWin();
                }
            }

        }
        public static void TextUpdate(PlayerControl player){
            if((player != null && PlayerControl.LocalPlayer == player) || PlayerControl.LocalPlayer.isDead()){
                if(MadScientist.text == null){
                    var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                    var obj = UnityEngine.Object.Instantiate(HudManager._instance.GameSettings);
                    MadScientist.text = obj.GetComponent<TMPro.TMP_Text>();
                    MadScientist.text.transform.position = new Vector3(HudManager._instance.GameSettings.transform.position.x , position.y - 0.1f, -14f); 
                    MadScientist.text.transform.localScale = new Vector3(1f, 1f, 1f);
                    MadScientist.text.fontSize = 1.5f;
                    MadScientist.text.fontSizeMin = 1.5f;
                    MadScientist.text.fontSizeMax = 1.5f;
                    MadScientist.text.alignment = TMPro.TextAlignmentOptions.BottomLeft;
                    MadScientist.text.transform.parent = HudManager._instance.GameSettings.transform.parent;
                }
                MadScientist.text.gameObject.SetActive(true);
                String text = "[感染状況]\n";
                foreach(PlayerControl p in PlayerControl.AllPlayerControls){
                    if(p.Data.IsDead) continue;
                    if(p == player) continue;
                    if(MadScientist.infected.ContainsKey(p.Data.PlayerId)){
                        text += $"{p.name}: <color=\"red\">感染</color>\n";
                    }else{
                        // データが無い場合は作成する
                        if(!MadScientist.progress.ContainsKey(p.Data.PlayerId)){
                            MadScientist.progress[p.Data.PlayerId] = 0f;
                        }
                        float progress = 100 * MadScientist.progress[p.Data.PlayerId]/CustomOptionHolder.madScientistDuration.getFloat();
                        string prog = progress.ToString("F1");
                        text += $"{p.name}: {prog}%\n";
                    }
                }

                MadScientist.text.text = text;
            }
        }

        public override void OnKill(PlayerControl target) { }
        public override void OnDeath(PlayerControl killer = null) {
            if(killer != null){
                byte targetId = killer.Data.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MadScientistSetInfected, Hazel.SendOption.Reliable, -1); killWriter.Write(targetId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.setInfected(targetId);
            }
        }

        public static void MakeButtons(HudManager hm) {
            madScientistSyringeButton = new CustomButton(
                () => {/*ボタンが押されたとき*/ 
                    byte targetId = MadScientist.currentTarget.Data.PlayerId;
					MadScientist.syringeFlag = true;
                    MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MadScientistSetInfected, Hazel.SendOption.Reliable, -1); killWriter.Write(targetId);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCProcedure.setInfected(targetId);
				},
                () => {/*ボタンが有効になる条件*/ return PlayerControl.LocalPlayer.isRole(RoleId.MadScientist) && !MadScientist.syringeFlag && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => {/*ボタンが使える条件*/ return MadScientist.currentTarget != null && !MadScientist.syringeFlag && PlayerControl.LocalPlayer.CanMove;},
                () => {/*ミーティング終了時*/ },
                MadScientist.getButtonSylinge(),
                new Vector3(0f, 1f, 0),
                hm,
                hm.UseButton,
                KeyCode.F
            );
        }
        public static Sprite getButtonSylinge() {
            if (buttonSylinge) return buttonSylinge;
            buttonSylinge = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Syringe.png", 115f);
            return buttonSylinge;
        }

        public static void SetButtonCooldowns()
        {
        }

        public static void clearAndReload()
        {
            madScientist = null;
            players = new List<MadScientist>();
            infected = new Dictionary<int, PlayerControl>();
            progress = new Dictionary<int, float>();
            syringeFlag = false;
            meetingFlag = false;
            triggerMadScientistWin = false;
        }
    }
}