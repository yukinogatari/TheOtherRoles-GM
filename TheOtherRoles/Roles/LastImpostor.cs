using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class LastImpostor : RoleBase<LastImpostor>
    {
        public static Color color = Palette.ImpostorRed;
        public static bool isEnable {get {return CustomOptionHolder.lastImpostorEnable.getBool();}}
        public static int killCounter = 0;
        public static int maxKillCounter {get {return (int)CustomOptionHolder.lastImpostorNumKills.getFloat();}}
        public static int numUsed = 0;
        public static int remainingShots = 0;
        public static int selectedFunction {get {return CustomOptionHolder.lastImpostorFunctions.getSelection();}}
        public static bool resultIsCrewOrNot {get {return CustomOptionHolder.lastImpostorResultIsCrewOrNot.getBool();}}

        public LastImpostor()
        {
            RoleType = roleId = RoleId.LastImpostor;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { 
            killCounter += 1;
        }
        public override void OnDeath(PlayerControl killer = null) { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static List<CustomButton> lastImpostorButtons = new List<CustomButton>();
        static Dictionary<byte, PoolablePlayer> playerIcons = new Dictionary<byte, PoolablePlayer>();
        public static void MakeButtons(HudManager hm)
        {
            lastImpostorButtons = new List<CustomButton>();

            Vector3 lastImpostorCalcPos(byte index)
            {
                //return new Vector3(-0.25f, -0.25f, 0) + Vector3.right * index * 0.55f;
                return new Vector3(-0.25f, -0.15f, 0) + Vector3.right * index * 0.55f;
            }

            Action lastImpostorButtonOnClick(byte index)
            {
                return () =>
                {
                    if(selectedFunction == 1) return;
                    PlayerControl p = Helpers.playerById(index);
                    LastImpostor.divine(p);
                };
            };

            Func<bool> lastImpostorHasButton(byte index)
            {
                return () =>
                {
                    if(selectedFunction == 1) return false;
                    var p = PlayerControl.LocalPlayer;
                    if(!p.isRole(RoleId.LastImpostor)) return false;
                    if (p.isRole(RoleId.LastImpostor) && p.CanMove && p.isAlive() & p.PlayerId != index
                        && MapOptions.playerIcons.ContainsKey(index) && numUsed < 1 && isCounterMax())
                    {
                        return true;
                    } 
                    else
                    {
                        if(playerIcons.ContainsKey(index))
                        {
                            playerIcons[index].gameObject.SetActive(false);
                            if(PlayerControl.LocalPlayer.isRole(RoleId.BountyHunter))
                                setBountyIconPos(Vector3.zero);
                        }
                        if(lastImpostorButtons.Count > index)
                        {
                            lastImpostorButtons[index].setActive(false);
                        }
                        return false;
                    }
                };
            }

            void setButtonPos(byte index)
            {
                Vector3 pos = lastImpostorCalcPos(index);
                Vector3 scale = new Vector3(0.4f, 0.8f, 1.0f);

                Vector3 iconBase = hm.UseButton.transform.localPosition;
                iconBase.x *= -1;
                if (lastImpostorButtons[index].PositionOffset != pos)
                {
                    lastImpostorButtons[index].PositionOffset = pos;
                    lastImpostorButtons[index].LocalScale = scale;
                    playerIcons[index].transform.localPosition = iconBase + pos;
                }
            }

            void setIconStatus(byte index, bool transparent)
            {
                playerIcons[index].transform.localScale = Vector3.one * 0.25f;
                playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
                playerIcons[index].setSemiTransparent(transparent);
            }

            void setBountyIconPos(Vector3 offset){
                Vector3 bottomLeft = new Vector3(-HudManager.Instance.UseButton.transform.localPosition.x, HudManager.Instance.UseButton.transform.localPosition.y, HudManager.Instance.UseButton.transform.localPosition.z);
                PoolablePlayer icon = MapOptions.playerIcons[BountyHunter.bounty.PlayerId];
                icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0) + offset;
                BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, -1f) + offset;
            }

            Func<bool> lastImpostorCouldUse(byte index)
            {
                return () =>
                {
                    if(selectedFunction == 1) return false;

                    //　ラストインポスター以外の場合、リソースがない場合はボタンを表示しない
                    var p = Helpers.playerById(index);
                    if (!playerIcons.ContainsKey(index) ||
                        !PlayerControl.LocalPlayer.isRole(RoleId.LastImpostor) ||
                        !isCounterMax()) 
                    {
                        return false;
                    }

                    // ボタンの位置を変更
                    setButtonPos(index);

                    // ボタンにテキストを設定
                    lastImpostorButtons[index].buttonText = PlayerControl.LocalPlayer.isAlive() ? "生存": "死亡";

                    // アイコンの位置と透明度を変更
                    setIconStatus(index, false);

                    // Bounty Hunterの場合賞金首の位置をずらして表示する
                    if(PlayerControl.LocalPlayer.isRole(RoleId.BountyHunter))
                    {
                        Vector3 offset = new Vector3(0f, 1f, 0f);
                        setBountyIconPos(offset);
                    }
                    
                    return PlayerControl.LocalPlayer.CanMove && numUsed < 1;
                };
            }


            for (byte i = 0; i < 15; i++)
            {
                CustomButton lastImpostorButton = new CustomButton(
                    // Action OnClick
                    lastImpostorButtonOnClick(i),
                    // bool HasButton
                    lastImpostorHasButton(i),
                    // bool CouldUse
                    lastImpostorCouldUse(i),
                    // Action OnMeetingEnds
                    () => { },
                    // sprite
                    null,
                    // position
                    Vector3.zero,
                    // hudmanager
                    hm,
                    // keyboard shortcut
                    null,
                    KeyCode.None,
                    true
                );
                lastImpostorButton.Timer = 0.0f;
                lastImpostorButton.MaxTimer = 0.0f;

                lastImpostorButtons.Add(lastImpostorButton);
            }

        }
        public static void SetButtonCooldowns() { }

        public static void Clear()
        {
            players = new List<LastImpostor>();
            killCounter = 0;
            numUsed = 0;
            remainingShots = (int)CustomOptionHolder.lastImpostorNumShots.getFloat();
            playerIcons = new Dictionary<byte, PoolablePlayer>();
        }
        public static bool isCounterMax(){
            if(maxKillCounter <= killCounter) return true;
            return false;
        }

        public static void promoteToLastImpostor()
        {
            if(!isEnable) return;

            var impList = new List<PlayerControl>();
            foreach(var p in PlayerControl.AllPlayerControls)
            {
                if(p.isImpostor() && p.isAlive()) impList.Add(p);
            }
            if(impList.Count == 1)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ImpostorPromotesToLastImpostor, Hazel.SendOption.Reliable, -1);
                writer.Write(impList[0].PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.impostorPromotesToLastImpostor(impList[0].PlayerId);
            }
        }
        public static void divine(PlayerControl p)
        {
            FortuneTeller.divine(p, resultIsCrewOrNot);
            numUsed += 1;
        }
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatch
        {
            public static void Prefix(IntroCutscene __instance) {
                if (PlayerControl.LocalPlayer != null && HudManager.Instance != null)
                {
                    Vector3 bottomLeft = new Vector3(-HudManager.Instance.UseButton.transform.localPosition.x, HudManager.Instance.UseButton.transform.localPosition.y, HudManager.Instance.UseButton.transform.localPosition.z);
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                        GameData.PlayerInfo data = p.Data;
                        PoolablePlayer player = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, HudManager.Instance.transform);
                        player.UpdateFromPlayerOutfit(p.Data.DefaultOutfit, p.Data.IsDead);
                        player.SetFlipX(true);
                        player.PetSlot.gameObject.SetActive(false);
                        player.NameText.text = p.Data.DefaultOutfit.PlayerName;
                        player.gameObject.SetActive(false);
                        playerIcons[p.PlayerId] = player;
                    }
                }
            }
        }
    }
}