using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class FortuneTeller : RoleBase<FortuneTeller>
    {
        public enum DivineResults
        {
            BlackWhite,
            Team,
            Role,
        }

        public static Color color = new Color32(175, 198, 241, byte.MaxValue);
        public static int numTasks { get { return (int)CustomOptionHolder.fortuneTellerNumTasks.getFloat(); } }
        public static DivineResults divineResult { get { return (DivineResults)CustomOptionHolder.fortuneTellerResults.getSelection(); } }
        public static float duration { get { return CustomOptionHolder.fortuneTellerDuration.getFloat(); } }
        public static float distance { get { return CustomOptionHolder.fortuneTellerDistance.getFloat(); } }

        public static bool endGameFlag = false;
        public static bool meetingFlag = false;

        public Dictionary<byte, float> progress = new Dictionary<byte, float>();
        public Dictionary<byte, bool> playerStatus = new Dictionary<byte, bool>();
        public bool divinedFlag = false;
        public int numUsed = 0;


        public FortuneTeller()
        {
            RoleType = roleId = RoleType.FortuneTeller;
        }

        public override void OnMeetingStart()
        {
            meetingFlag = true;
        }

        public override void OnMeetingEnd()
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(5.0f, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    meetingFlag = false;
                }
            })));

            foreach (var p in PlayerControl.AllPlayerControls)
            {
                playerStatus[p.PlayerId] = p.isAlive();
            }
        }

        public override void OnKill(PlayerControl target) { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
        public override void OnDeath(PlayerControl killer = null) { }

        public override void FixedUpdate()
        {
            fortuneTellerUpdate();
            impostorArrowUpdate();
        }

        public static bool isCompletedNumTasks(PlayerControl p)
        {
            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
            return tasksCompleted >= numTasks;
        }

        public static void setDivinedFlag(PlayerControl player, bool flag)
        {
            if (isRole(player))
            {
                FortuneTeller n = players.First(x => x.player == player);
                n.divinedFlag = flag;
            }
        }

        public bool canDivine(byte index)
        {
            bool status = true;
            if (playerStatus.ContainsKey(index))
            {
                status = playerStatus[index];
            }
            return (progress.ContainsKey(index) && progress[index] >= duration) || !status;
        }

        public static List<CustomButton> fortuneTellerButtons;

        public static void MakeButtons(HudManager hm)
        {
            fortuneTellerButtons = new List<CustomButton>();

            Vector3 fortuneTellerCalcPos(byte index)
            {
                int adjIndex = index < PlayerControl.LocalPlayer.PlayerId ? index : index - 1;
                return new Vector3(-0.25f, -0.15f, 0) + Vector3.right * adjIndex * 0.55f;
            }

            Action fortuneTellerButtonOnClick(byte index)
            {
                return () =>
                {
                    if (PlayerControl.LocalPlayer.CanMove && local.numUsed < 1 && local.canDivine(index))
                    {
                        PlayerControl p = Helpers.playerById(index);
                        local.divine(p);
                    }
                };
            };

            Func<bool> fortuneTellerHasButton(byte index)
            {
                return () =>
                {
                    return PlayerControl.LocalPlayer.isRole(RoleType.FortuneTeller);
                    //var p = PlayerControl.LocalPlayer;
                    //if (!p.isRole(RoleType.FortuneTeller)) return false;
                };
            }

            void setButtonPos(byte index)
            {
                Vector3 pos = fortuneTellerCalcPos(index);
                Vector3 scale = new Vector3(0.4f, 0.5f, 1.0f);

                Vector3 iconBase = hm.UseButton.transform.localPosition;
                iconBase.x *= -1;
                if (fortuneTellerButtons[index].PositionOffset != pos)
                {
                    fortuneTellerButtons[index].PositionOffset = pos;
                    fortuneTellerButtons[index].LocalScale = scale;
                    MapOptions.playerIcons[index].transform.localPosition = iconBase + pos;
                }
            }

            void setIconPos(byte index, bool transparent)
            {
                MapOptions.playerIcons[index].transform.localScale = Vector3.one * 0.25f;
                MapOptions.playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
                MapOptions.playerIcons[index].setSemiTransparent(transparent);
            }

            Func<bool> fortuneTellerCouldUse(byte index)
            {
                return () =>
                {
                    //　占い師以外の場合、リソースがない場合はボタンを表示しない
                    if (!MapOptions.playerIcons.ContainsKey(index) ||
                        !PlayerControl.LocalPlayer.isRole(RoleType.FortuneTeller) ||
                        PlayerControl.LocalPlayer.isDead() ||
                        PlayerControl.LocalPlayer.PlayerId == index ||
                        !isCompletedNumTasks(PlayerControl.LocalPlayer) ||
                        local.numUsed >= 1)
                    {
                        if (MapOptions.playerIcons.ContainsKey(index))
                            MapOptions.playerIcons[index].gameObject.SetActive(false);
                        if (fortuneTellerButtons.Count > index)
                            fortuneTellerButtons[index].setActive(false);

                        return false;
                    }

                    // ボタンの位置を変更
                    setButtonPos(index);

                    // ボタンにテキストを設定
                    bool status = true;
                    if (local.playerStatus.ContainsKey(index))
                    {
                        status = local.playerStatus[index];
                    }

                    if (status)
                    {
                        var progress = local.progress.ContainsKey(index) ? local.progress[index] : 0f;
                        fortuneTellerButtons[index].buttonText = $"{progress:0.0}/{duration:0.0}";
                    }
                    else
                    {
                        fortuneTellerButtons[index].buttonText = ModTranslation.getString("fortuneTellerDead");
                    }

                    // アイコンの位置と透明度を変更
                    setIconPos(index, !local.canDivine(index));

                    MapOptions.playerIcons[index].gameObject.SetActive(Helpers.ShowButtons && PlayerControl.LocalPlayer.CanMove);
                    fortuneTellerButtons[index].setActive(Helpers.ShowButtons && PlayerControl.LocalPlayer.CanMove);

                    return PlayerControl.LocalPlayer.CanMove && local.numUsed < 1 && local.canDivine(index);
                };
            }


            for (byte i = 0; i < 15; i++)
            {
                CustomButton fortuneTellerButton = new CustomButton(
                    // Action OnClick
                    fortuneTellerButtonOnClick(i),
                    // bool HasButton
                    fortuneTellerHasButton(i),
                    // bool CouldUse
                    fortuneTellerCouldUse(i),
                    // Action OnMeetingEnds
                    () => { },
                    // sprite
                    null,
                    // position
                    Vector3.zero,
                    // hudmanager
                    hm,
                    hm.AbilityButton,
                    // keyboard shortcut
                    KeyCode.None,
                    true
                );
                fortuneTellerButton.Timer = 0.0f;
                fortuneTellerButton.MaxTimer = 0.0f;

                fortuneTellerButtons.Add(fortuneTellerButton);
            }

        }

        private void fortuneTellerUpdate()
        {
            if (player == PlayerControl.LocalPlayer && !meetingFlag)
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (!progress.ContainsKey(p.PlayerId)) progress[p.PlayerId] = 0f;
                    if (p.isDead()) continue;
                    var fortuneTeller = PlayerControl.LocalPlayer;
                    float distance = Vector3.Distance(p.transform.position, fortuneTeller.transform.position);
                    // 障害物判定
                    bool anythingBetween = PhysicsHelpers.AnythingBetween(p.GetTruePosition(), fortuneTeller.GetTruePosition(), Constants.ShipAndObjectsMask, false);
                    if (!anythingBetween && distance <= FortuneTeller.distance && progress[p.PlayerId] < duration)
                    {
                        progress[p.PlayerId] += Time.fixedDeltaTime;
                    }
                }
            }
        }

        public static List<Arrow> arrows = new List<Arrow>();
        public static float updateTimer = 0f;

        public void impostorArrowUpdate()
        {
            if (PlayerControl.LocalPlayer.isImpostor())
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

                    foreach (var p in players)
                    {
                        if (p.player.isDead()) continue;
                        if (!p.divinedFlag) continue;

                        Arrow arrow = new Arrow(FortuneTeller.color);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.player.transform.position);
                        arrows.Add(arrow);
                    }

                    // タイマーに時間をセット
                    updateTimer = 1f;
                }
                else
                {
                    arrows.Do(x => x.Update());
                }
            }
        }

        public static void Clear()
        {
            players = new List<FortuneTeller>();
            arrows = new List<Arrow>();
            meetingFlag = true;
        }

        public void divine(PlayerControl p)
        {
            string msgBase = "";
            string msgInfo = "";
            Color color = Color.white;

            if (divineResult == DivineResults.BlackWhite) {
                if (p.isCrew())
                {
                    msgBase = "divineMessageIsCrew";
                    color = Color.white;
                }
                else
                {
                    msgBase = "divineMessageIsntCrew";
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Team) {
                msgBase = "divineMessageTeam";
                if (p.isCrew())
                {
                    msgInfo = ModTranslation.getString("divineCrew");
                    color = Color.white;
                }
                else if (p.isNeutral())
                {
                    msgInfo = ModTranslation.getString("divineNeutral");
                    color = Color.yellow;
                }
                else
                {
                    msgInfo = ModTranslation.getString("divineImpostor");
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Role) { 
                msgBase = "divineMessageRole";
                msgInfo = String.Join(" ", RoleInfo.getRoleInfoForPlayer(p).Select(x => Helpers.cs(x.color, x.name)).ToArray());
            }

            string msg = string.Format(ModTranslation.getString(msgBase), p.name, msgInfo);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                fortuneTellerMessage(msg, 5f, color);
            }

            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
            numUsed += 1;

            // 占いを実行したことで発火される処理を他クライアントに通知
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FortuneTellerUsedDivine, Hazel.SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(p.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.fortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
        }

        private static TMPro.TMP_Text text;
        public static void fortuneTellerMessage(string message, float duration, Color color)
        {
            RoomTracker roomTracker = HudManager.Instance?.roomTracker;
            if (roomTracker != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(HudManager.Instance.transform);
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                gameObject.transform.localScale *= 1.5f;

                text = gameObject.GetComponent<TMPro.TMP_Text>();
                text.text = message;
                text.color = color;

                HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
                {
                    if (p == 1f && text != null && text.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(text.gameObject);
                    }
                })));
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatch
        {
            public static void Prefix(IntroCutscene __instance)
            {
                HudManager.Instance.StartCoroutine(Effects.Lerp(16.2f, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        meetingFlag = false;
                    }
                })));
            }
        }
    }

}