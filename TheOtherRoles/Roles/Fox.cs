using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Patches;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Fox : RoleBase<Fox>
    {
        public static Color color = new Color32(167, 87, 168, byte.MaxValue);
        private static CustomButton foxButton;
        private static CustomButton foxRepairButton;
        private static CustomButton foxImmoralistButton;
        public static List<Arrow> arrows = new List<Arrow>();
        public static float updateTimer = 0f;

        public static bool canFixReactorAndO2 { get { return CustomOptionHolder.foxCanFixReactorAndO2.getBool(); } }
        public static float arrowUpdateInterval = 0.5f;
        public static bool crewWinsByTasks { get { return CustomOptionHolder.foxCrewWinsByTasks.getBool(); } }
        public static float stealthCooldown { get { return CustomOptionHolder.foxStealthCooldown.getFloat(); } }
        public static float stealthDuration { get { return CustomOptionHolder.foxStealthDuration.getFloat(); } }
        public static int numCommonTasks { get { return CustomOptionHolder.foxTasks.commonTasks; } }
        public static int numLongTasks { get { return CustomOptionHolder.foxTasks.longTasks; } }
        public static int numShortTasks { get { return CustomOptionHolder.foxTasks.shortTasks; } }

        public bool stealthed = false;
        public DateTime stealthedAt = DateTime.UtcNow;
        public static float fadeTime = 1f;

        public static int optNumRepair { get { return (int)CustomOptionHolder.foxNumRepair.getFloat(); } }
        public static int numRepair = 0;

        public static bool canCreateImmoralist { get { return CustomOptionHolder.foxCanCreateImmoralist.getBool(); } }
        public static PlayerControl currentTarget;
        public static PlayerControl immoralist;
        public static List<byte> exiledFox = new List<byte>();


        public Fox()
        {
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
            RoleType = roleId = RoleType.Fox;
            numRepair = optNumRepair;
            immoralist = null;
            currentTarget = null;
            exiledFox = new List<byte>();
        }

        public override void OnMeetingStart()
        {
            stealthed = false;
            foxButton.isEffectActive = false;
            foxButton.Timer = foxButton.MaxTimer = stealthCooldown;
        }

        public override void OnMeetingEnd() { }
        public override void OnKill(PlayerControl target) { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
        public override void OnDeath(PlayerControl killer = null)
        {
            exiledFox.Add(player.PlayerId);
            player.clearAllTasks();
            if (!Fox.isFoxAlive())
            {
                foreach (var immoralist in Immoralist.allPlayers)
                {
                    if (immoralist.isAlive())
                    {
                        if (killer == null)
                        {
                            immoralist.Exiled();
                        }
                        else
                        {
                            immoralist.MurderPlayer(immoralist);
                        }
                        finalStatuses[immoralist.PlayerId] = FinalStatus.Suicide;
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            if (player == PlayerControl.LocalPlayer) {
                arrowUpdate();
                if (player.isAlive())
                {
                    List<PlayerControl> untargetablePlayers = new List<PlayerControl>();
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (p.isImpostor() || p.isRole(RoleType.Jackal) || p.isRole(RoleType.Sheriff))
                        {
                            untargetablePlayers.Add(p);
                        }
                    }
                    currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
                    setPlayerOutline(currentTarget, Fox.color);
                }
            }
        }

        public static void Clear()
        {
            players = new List<Fox>();
            foreach (Arrow arrow in arrows)
            {
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            }
            arrows = new List<Arrow>();
            Immoralist.Clear();
        }

        private static Sprite hideButtonSprite;
        private static Sprite repairButtonSprite;
        private static Sprite immoralistButtonSprite;

        public static Sprite getHideButtonSprite()
        {
            if (hideButtonSprite) return hideButtonSprite;
            hideButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FoxHideButton.png", 115f);
            return hideButtonSprite;
        }

        public static Sprite getRepairButtonSprite()
        {
            if (repairButtonSprite) return repairButtonSprite;
            repairButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
            return repairButtonSprite;
        }

        public static Sprite getImmoralistButtonSprite()
        {
            if (immoralistButtonSprite) return immoralistButtonSprite;
            immoralistButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FoxImmoralistButton.png", 115f);
            return immoralistButtonSprite;
        }

        public static float stealthFade(PlayerControl player)
        {
            if (isRole(player) && fadeTime > 0f && player.isAlive())
            {
                Fox n = players.First(x => x.player == player);
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - n.stealthedAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static bool isStealthed(PlayerControl player)
        {
            if (isRole(player) && player.isAlive())
            {
                Fox n = players.First(x => x.player == player);
                return n.stealthed;
            }
            return false;
        }

        public static void setStealthed(PlayerControl player, bool stealthed = true)
        {
            if (isRole(player))
            {
                Fox n = players.First(x => x.player == player);
                n.stealthed = stealthed;
                n.stealthedAt = DateTime.UtcNow;
            }
        }


        public static void MakeButtons(HudManager hm)
        {
            // Fox stealth
            foxButton = new CustomButton(
                () =>
                {
                    if (foxButton.isEffectActive)
                    {
                        foxButton.Timer = 0;
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FoxStealth, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.foxStealth(PlayerControl.LocalPlayer.PlayerId, true);
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Fox) && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    if (foxButton.isEffectActive)
                    {
                        foxButton.buttonText = ModTranslation.getString("FoxUnstealthText");
                    }
                    else
                    {
                        foxButton.buttonText = ModTranslation.getString("FoxStealthText");
                    }
                    return PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {
                    foxButton.Timer = foxButton.MaxTimer = Fox.stealthCooldown;
                },
                Fox.getHideButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.AbilityButton,
                KeyCode.F,
                true,
                Fox.stealthDuration,
                () =>
                {
                    foxButton.Timer = foxButton.MaxTimer = Fox.stealthCooldown;
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FoxStealth, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.foxStealth(PlayerControl.LocalPlayer.PlayerId, false);
                }
            );
            foxButton.buttonText = ModTranslation.getString("FoxStealthText");
            foxButton.effectCancellable = true;

            foxRepairButton = new CustomButton(
                () =>
                {
                    bool sabotageActive = false;
                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                        if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles)
                            sabotageActive = true;
                    if (!sabotageActive) return;

                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                    {
                        if (task.TaskType == TaskTypes.FixLights)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EngineerFixLights, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.engineerFixLights();
                        }
                        else if (task.TaskType == TaskTypes.RestoreOxy)
                        {
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                        }
                        else if (task.TaskType == TaskTypes.ResetReactor)
                        {
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                        }
                        else if (task.TaskType == TaskTypes.ResetSeismic)
                        {
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
                        }
                        else if (task.TaskType == TaskTypes.FixComms)
                        {
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                        }
                        else if (task.TaskType == TaskTypes.StopCharles)
                        {
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
                        }
                    }
                    numRepair -= 1;
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Fox) && PlayerControl.LocalPlayer.isAlive() && numRepair > 0; },
                () =>
                {
                    bool sabotageActive = false;
                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                        if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles)
                            sabotageActive = true;
                    return sabotageActive && numRepair > 0 && PlayerControl.LocalPlayer.CanMove;
                },
                () => { },
                Fox.getRepairButtonSprite(),
                new Vector3(-0.9f, 1f, 0),
                hm,
                hm.AbilityButton,
                KeyCode.G
            );
            foxRepairButton.buttonText = ModTranslation.getString("FoxRepairText"); ;

            foxImmoralistButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FoxCreatesImmoralist, Hazel.SendOption.Reliable, -1);
                    writer.Write(currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.foxCreatesImmoralist(currentTarget.PlayerId);
                },
                () => { return !Immoralist.exists && canCreateImmoralist && PlayerControl.LocalPlayer.isRole(RoleType.Fox) && PlayerControl.LocalPlayer.isAlive(); },
                () => { return canCreateImmoralist && Fox.currentTarget != null && PlayerControl.LocalPlayer.CanMove; },
                () => { foxImmoralistButton.Timer = 20; },
                getImmoralistButtonSprite(),
                new Vector3(-1.8f, 1f, 0),
                hm,
                hm.AbilityButton,
                KeyCode.I
            );
            foxImmoralistButton.buttonText = ModTranslation.getString("FoxImmoralistText");
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

                // Arrorw一覧
                arrows = new List<Arrow>();

                // インポスターの位置を示すArrorwを描画
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.isDead()) continue;
                    Arrow arrow;
                    // float distance = Vector2.Distance(p.transform.position, PlayerControl.LocalPlayer.transform.position);
                    if (p.Data.Role.IsImpostor || p.isRole(RoleType.Jackal) || p.isRole(RoleType.Sheriff))
                    {
                        if (p.Data.Role.IsImpostor)
                        {
                            arrow = new Arrow(Palette.ImpostorRed);
                        }
                        else if (p.isRole(RoleType.Jackal))
                        {
                            arrow = new Arrow(Jackal.color);
                        }
                        else if (p.isRole(RoleType.Sheriff))
                        {
                            arrow = new Arrow(Palette.White);
                        }
                        else
                        {
                            arrow = new Arrow(Palette.Black);
                        }
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

        public static bool isFoxAlive()
        {
            bool isAlive = false;
            foreach (var fox in Fox.allPlayers)
            {
                if (fox.isAlive() && !exiledFox.Contains(fox.PlayerId))
                {
                    isAlive = true;
                }
            }
            return isAlive;
        }

        public static bool isFoxCompletedTasks()
        {
            // 生存中の狐が1匹でもタスクを終了しているかを確認
            bool isCompleted = false;
            foreach (var fox in allPlayers)
            {
                if (fox.isAlive())
                {
                    if (tasksComplete(fox))
                    {
                        isCompleted = true;
                        break;
                    }
                }
            }
            return isCompleted;
        }

        private static bool tasksComplete(PlayerControl p)
        {
            int counter = 0;
            int totalTasks = numCommonTasks + numLongTasks + numShortTasks;
            if (totalTasks == 0) return true;
            foreach (var task in p.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter >= totalTasks;
        }

        public void assignTasks()
        {
            player.generateAndAssignTasks(numCommonTasks, numShortTasks, numLongTasks);
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch
        {
            public static void Postfix(ShipStatus __instance)
            {
                if (PlayerControl.LocalPlayer.isRole(RoleType.Fox))
                {
                    local.assignTasks();
                }
            }
        }

        // 透明化
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsFoxPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (isRole(__instance.myPlayer))
                {
                    var fox = __instance.myPlayer;
                    if (fox == null || fox.isDead()) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.isRole(RoleType.Fox) ||
                        PlayerControl.LocalPlayer.isRole(RoleType.Immoralist) ||
                        PlayerControl.LocalPlayer.isDead() ||
                        (PlayerControl.LocalPlayer.isRole(RoleType.Lighter) && Lighter.isLightActive(PlayerControl.LocalPlayer));

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (isStealthed(fox))
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade(fox));
                        fox.MyRend.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade(fox));
                    }

                    Ninja.setOpacity(fox, opacity);
                }
            }
        }
    }
}