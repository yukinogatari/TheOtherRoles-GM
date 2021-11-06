  
using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hazel;
using UnhollowerBaseLib;
using System;
using System.Text;

namespace TheOtherRoles.Patches {
    enum CustomGameOverReason {
        LoversWin = 10,
        TeamJackalWin = 11,
        MiniLose = 12,
        JesterWin = 13,
        ArsonistWin = 14
    }

    enum WinCondition {
        Default,
        LoversTeamWin,
        LoversSoloWin,
        JesterWin,
        JackalWin,
        MiniLose,
        ArsonistWin,
        OpportunistWin
    }

    enum FinalStatus {
        Alive,
        Torched,
        Exiled,
        Dead,
        Suicide,
        Misfire,
        Disconnected
    }

    static class AdditionalTempData {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition winCondition = WinCondition.Default;
        public static List<WinCondition> extraConditions = new List<WinCondition>();
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();
        public static bool isGM = false;
        public static GameOverReason gameOverReason;

        public static void clear() {
            playerRoles.Clear();
            extraConditions.Clear();
            winCondition = WinCondition.Default;
            isGM = false;
        }

        internal class PlayerRoleInfo {
            public string PlayerName { get; set; }
            public List<RoleInfo> Roles { get; set; }
            public int TasksCompleted  { get; set; }
            public int TasksTotal  { get; set; }
            public FinalStatus Status { get; set; }
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch {
        
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)]ref GameOverReason reason, [HarmonyArgument(1)]bool showAd) {
            Camouflager.resetCamouflage();
            Morphling.resetMorph();

            AdditionalTempData.gameOverReason = reason;
            if ((int)reason >= 10) reason = GameOverReason.ImpostorByKill;
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)]ref GameOverReason reason, [HarmonyArgument(1)]bool showAd) {
            var gameOverReason = AdditionalTempData.gameOverReason;

            AdditionalTempData.clear();

            //foreach (var pc in PlayerControl.AllPlayerControls)
            foreach (var p in GameData.Instance.AllPlayers)
            {
                //var p = pc.Data;
                var roles = RoleInfo.getRoleInfoForPlayer(p.Object);
                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p);
                var finalStatus = 
                    p.Disconnected == true ? FinalStatus.Disconnected :
                    suicidedPlayers.Contains(p.PlayerId) ? FinalStatus.Suicide :
                    misfiredPlayers.Contains(p.PlayerId) ? FinalStatus.Misfire :
                    exiledPlayers.Contains(p.PlayerId) ? FinalStatus.Exiled :
                    p.IsDead == true ? FinalStatus.Dead :
                    gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin && Arsonist.arsonist != p.Object ? FinalStatus.Torched :
                    gameOverReason == GameOverReason.ImpostorBySabotage && !p.IsImpostor ? FinalStatus.Dead :
                    FinalStatus.Alive;

                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo()
                {
                    PlayerName = p.PlayerName,
                    Roles = roles,
                    TasksTotal = tasksTotal,
                    TasksCompleted = gameOverReason == GameOverReason.HumansByTask ? tasksTotal : tasksCompleted,
                    Status = finalStatus,
                });
            }

            AdditionalTempData.isGM = CustomOptionHolder.gmEnabled.getBool() && PlayerControl.LocalPlayer.isGM();

            // Remove Jester, Arsonist, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new List<PlayerControl>();
            if (Jester.jester != null) notWinners.Add(Jester.jester);
            if (Sidekick.sidekick != null) notWinners.Add(Sidekick.sidekick);
            if (Jackal.jackal != null) notWinners.Add(Jackal.jackal);
            if (Arsonist.arsonist != null) notWinners.Add(Arsonist.arsonist);
            if (Madmate.madmate != null) notWinners.Add(Madmate.madmate);
            if (Opportunist.opportunist != null) notWinners.Add(Opportunist.opportunist);
            notWinners.AddRange(Jackal.formerJackals);

            // GM can't win at all, and we're treating lovers as a separate class
            if (GM.gm != null) notWinners.Add(GM.gm);

            if (Lovers.separateTeam)
            {
                if (Lovers.lover1 != null) notWinners.Add(Lovers.lover1);
                if (Lovers.lover2 != null) notWinners.Add(Lovers.lover2);
            }

            List<WinningPlayerData> winnersToRemove = new List<WinningPlayerData>();
            foreach (WinningPlayerData winner in TempData.winners) {
                if (notWinners.Any(x => x.Data.PlayerName == winner.Name)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) TempData.winners.Remove(winner);


            bool jesterWin = Jester.jester != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            bool arsonistWin = Arsonist.arsonist != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            bool miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
            bool loversWin = Lovers.existingAndAlive() && !(Lovers.separateTeam && gameOverReason == GameOverReason.HumansByTask);
            /*bool loversWin = Lovers.existingAndAlive() && 
                (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin || 
                    (TempData.DidHumansWin(gameOverReason) && !Lovers.existingWithKiller() && Lovers.canWinWithCrew)
                ); // Either they win if they are among the last 3 players, or they win if they are both Crewmates and both alive and the Crew wins (Team Imp/Jackal Lovers can only win solo wins)*/
            bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin && ((Jackal.jackal != null && !Jackal.jackal.Data.IsDead) || (Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead));
            bool opportunistWin = Opportunist.opportunist != null && !Opportunist.opportunist.Data.IsDead;


            // Mini lose
            if (miniLose)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Mini.mini.Data);
                wpd.IsYou = false; // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
                TempData.winners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.MiniLose;
            }

            // Jester win
            else if (jesterWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Jester.jester.Data);
                TempData.winners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.JesterWin;
            }

            // Arsonist win
            else if (arsonistWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Arsonist.arsonist.Data);
                TempData.winners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            }

            // Lovers win conditions
            else if (loversWin)
            {
                // Double win for lovers, crewmates also win
                if (TempData.DidHumansWin(gameOverReason) && !Lovers.separateTeam && !Lovers.existingWithKiller())
                {
                    AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
                    AdditionalTempData.extraConditions.Add(WinCondition.LoversTeamWin);
                }
                // Lovers solo win
                else
                {
                    AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                    TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                    TempData.winners.Add(new WinningPlayerData(Lovers.lover1.Data));
                    TempData.winners.Add(new WinningPlayerData(Lovers.lover2.Data));
                }
            }

            // Jackal win condition (should be implemented using a proper GameOverReason in the future)
            else if (teamJackalWin) {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.JackalWin;
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Jackal.jackal.Data);
                wpd.IsImpostor = false; 
                TempData.winners.Add(wpd);
                // If there is a sidekick. The sidekick also wins
                if (Sidekick.sidekick != null) {
                    WinningPlayerData wpdSidekick = new WinningPlayerData(Sidekick.sidekick.Data);
                    wpdSidekick.IsImpostor = false; 
                    TempData.winners.Add(wpdSidekick);
                }
                foreach(var player in Jackal.formerJackals) {
                    WinningPlayerData wpdFormerJackal = new WinningPlayerData(player.Data);
                    wpdFormerJackal.IsImpostor = false; 
                    TempData.winners.Add(wpdFormerJackal);
                }
            } else if (Madmate.madmate != null) {
                // Madmate wins if team impostors wins
                foreach (WinningPlayerData winner in TempData.winners) {
                    if (winner.IsImpostor) {
                        WinningPlayerData wpd = new WinningPlayerData(Madmate.madmate.Data);
                        TempData.winners.Add(wpd);
                        break;
                    }
                }
            }

            if (opportunistWin) {
                AdditionalTempData.extraConditions.Add(WinCondition.OpportunistWin);
                bool toAdd = true;
                foreach (WinningPlayerData winner in TempData.winners)
                {
                    if (winner.Name == Opportunist.opportunist.Data.PlayerName)
                    {
                        toAdd = false;
                        break;
                    }
                }

                if (toAdd) {
                    TempData.winners.Add(new WinningPlayerData(Opportunist.opportunist.Data));
                }
            }

            foreach (WinningPlayerData wpd in TempData.winners)
            {
                wpd.IsDead = wpd.IsDead || AdditionalTempData.playerRoles.Any(x => x.PlayerName == wpd.Name && x.Status != FinalStatus.Alive);
            }

            // Reset Settings
            RPCProcedure.resetVariables();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch
    {

        public static void Postfix(EndGameManager __instance) {
            GameObject bonusTextObject = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusTextObject.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
            bonusTextObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusTextObject.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            if (AdditionalTempData.isGM) {
                __instance.WinText.text = ModTranslation.getString("gmGameOver");
                __instance.WinText.color = GM.color;
            }

            string bonusText = "";

            if (AdditionalTempData.winCondition == WinCondition.JesterWin) {
                bonusText = "jesterWin";
                textRenderer.color = Jester.color;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.ArsonistWin) {
                bonusText = "arsonistWin";
                textRenderer.color = Arsonist.color;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.LoversTeamWin) {
                bonusText = "crewWin";
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.LoversSoloWin) {
                bonusText = "loversWin";
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.JackalWin) {
                bonusText = "jackalWin";
                textRenderer.color = Jackal.color;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.MiniLose) {
                bonusText = "miniDied";
                textRenderer.color = Mini.color;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
            } 
            else if (AdditionalTempData.gameOverReason == GameOverReason.HumansByTask || AdditionalTempData.gameOverReason == GameOverReason.HumansByVote) {
                bonusText = "crewWin";
                textRenderer.color = Palette.White;
            }
            else if (AdditionalTempData.gameOverReason == GameOverReason.ImpostorByKill || AdditionalTempData.gameOverReason == GameOverReason.ImpostorBySabotage || AdditionalTempData.gameOverReason == GameOverReason.ImpostorByVote) {
                bonusText = "impostorWin";
                textRenderer.color = Palette.ImpostorRed;
            }

            string extraText = "";
            foreach (WinCondition w in AdditionalTempData.extraConditions)
            {
                switch (w)
                {
                    case WinCondition.OpportunistWin:
                        extraText += ModTranslation.getString("opportunistExtra");
                        break;
                    case WinCondition.LoversTeamWin:
                        extraText += ModTranslation.getString("loversExtra");
                        break;
                    default:
                        break;
                }
            }

            if (extraText.Length > 0)
            {
                textRenderer.text = string.Format(ModTranslation.getString(bonusText + "Extra"), extraText);
            } 
            else
            {
                textRenderer.text = ModTranslation.getString(bonusText);
            }

            if (MapOptions.showRoleSummary) {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f); 
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                roleSummaryText.AppendLine(ModTranslation.getString("roleSummaryText"));
                AdditionalTempData.playerRoles.Sort((x, y) => {
                    RoleInfo roleX = x.Roles.FirstOrDefault();
                    RoleInfo roleY = y.Roles.FirstOrDefault();
                    RoleId idX = roleX == null ? RoleId.NoRole : roleX.roleId;
                    RoleId idY = roleY == null ? RoleId.NoRole : roleY.roleId;

                    if (x.Status == y.Status) {
                        if (idX == idY)
                        {
                            return x.PlayerName.CompareTo(y.PlayerName);
                        }
                        return idX.CompareTo(idY);
                    }
                    return x.Status.CompareTo(y.Status);

                });
                foreach(var data in AdditionalTempData.playerRoles) {
                    var roles = string.Join(" ", data.Roles.Select(x => Helpers.cs(x.color, x.name)));
                    var taskInfo = data.TasksTotal > 0 ? $"<color=#FAD934FF>{data.TasksCompleted}/{data.TasksTotal}</color>" : "";
                    string aliveDead = "";
                    switch (data.Status)
                    {
                        case FinalStatus.Alive: aliveDead = ModTranslation.getString("roleSummaryAlive"); break;
                        case FinalStatus.Torched: aliveDead = ModTranslation.getString("roleSummaryTorched"); break;
                        case FinalStatus.Exiled: aliveDead = ModTranslation.getString("roleSummaryExiled"); break;
                        case FinalStatus.Dead: aliveDead = ModTranslation.getString("roleSummaryDead"); break;
                        case FinalStatus.Suicide: aliveDead = ModTranslation.getString("roleSummarySuicide"); break;
                        case FinalStatus.Misfire: aliveDead = ModTranslation.getString("roleSummaryMisfire"); break;
                        case FinalStatus.Disconnected: aliveDead = ModTranslation.getString("roleSummaryDC"); break;
                        default: aliveDead = ""; break;
                    }

                    roleSummaryText.AppendLine($"{data.PlayerName}<pos=18.5%>{taskInfo}<pos=25%>{aliveDead}<pos=34%>{roles}");
                }
                TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
                roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.outlineWidth *= 1.2f;
                roleSummaryTextMesh.fontSizeMin = 1.25f;
                roleSummaryTextMesh.fontSizeMax = 1.25f;
                roleSummaryTextMesh.fontSize = 1.25f;
                
                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
            AdditionalTempData.clear();
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))] 
    class CheckEndCriteriaPatch {
        public static bool Prefix(ShipStatus __instance) {
            if (!GameData.Instance) return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) return true; // InstanceExists | Don't check Custom Criteria when in Tutorial
            if (HudManager.Instance.isIntroDisplayed) return false;

            var statistics = new PlayerStatistics(__instance);
            if (CheckAndEndGameForMiniLose(__instance)) return false;
            if (CheckAndEndGameForJesterWin(__instance)) return false;
            if (CheckAndEndGameForArsonistWin(__instance)) return false;
            if (CheckAndEndGameForSabotageWin(__instance)) return false;
            if (CheckAndEndGameForTaskWin(__instance)) return false;
            if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
            if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
            if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
            return false;
        }

        private static bool CheckAndEndGameForMiniLose(ShipStatus __instance) {
            if (Mini.triggerMiniLose) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.MiniLose, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJesterWin(ShipStatus __instance) {
            if (Jester.triggerJesterWin) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance) {
            if (Arsonist.triggerArsonistWin) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance) {
            if (__instance.Systems == null) return false;
            ISystemType systemType = __instance.Systems.ContainsKey(SystemTypes.LifeSupp) ? __instance.Systems[SystemTypes.LifeSupp] : null;
            if (systemType != null) {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f) {
                    EndGameForSabotage(__instance);
                    lifeSuppSystemType.Countdown = 10000f;
                    return true;
                }
            }
            ISystemType systemType2 = __instance.Systems.ContainsKey(SystemTypes.Reactor) ? __instance.Systems[SystemTypes.Reactor] : null;
            if (systemType2 == null) {
                systemType2 = __instance.Systems.ContainsKey(SystemTypes.Laboratory) ? __instance.Systems[SystemTypes.Laboratory] : null;
            }
            if (systemType2 != null) {
                ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSystem != null && criticalSystem.Countdown < 0f) {
                    EndGameForSabotage(__instance);
                    criticalSystem.ClearSabotage();
                    return true;
                }
            }
            return false;
        }

        private static bool CheckAndEndGameForTaskWin(ShipStatus __instance) {
            if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame(GameOverReason.HumansByTask, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.TeamLoversAlive == 2 && statistics.TotalAlive <= 3) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.LoversWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJackalWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive && statistics.TeamImpostorsAlive == 0 && !(statistics.TeamJackalLovers < 2 && statistics.TeamLoversAlive == 2)) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive && statistics.TeamJackalAlive == 0 && !(statistics.TeamImpostorLovers < 2 && statistics.TeamLoversAlive == 2)) {
                __instance.enabled = false;
                GameOverReason endReason;
                switch (TempData.LastDeathReason) {
                    case DeathReason.Exile:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                    case DeathReason.Kill:
                        endReason = GameOverReason.ImpostorByKill;
                        break;
                    default:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                }
                ShipStatus.RpcEndGame(endReason, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForCrewmateWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.TeamCrew > 0 && statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0) {
                __instance.enabled = false;
                ShipStatus.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            return false;
        }

        private static void EndGameForSabotage(ShipStatus __instance) {
            __instance.enabled = false;
            ShipStatus.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
            return;
        }

    }

    internal class PlayerStatistics {
        public int TeamImpostorsAlive {get;set;}
        public int TeamJackalAlive {get;set; }
        public int TeamLoversAlive { get; set; }
        public int TeamCrew { get; set; }
        public int NeutralAlive { get; set; }
        public int TotalAlive {get;set;}
        public int TeamImpostorLovers {get;set;}
        public int TeamJackalLovers {get;set;}

        public PlayerStatistics(ShipStatus __instance) {
            GetPlayerCounts();
        }

        private bool isLover(GameData.PlayerInfo p) {
            return (Lovers.lover1 != null && Lovers.lover1.PlayerId == p.PlayerId) || (Lovers.lover2 != null && Lovers.lover2.PlayerId == p.PlayerId);
        }

        private void GetPlayerCounts() {
            int numJackalAlive = 0;
            int numImpostorsAlive = 0;
            int numLoversAlive = 0;
            int numTotalAlive = 0;
            int impLovers = 0;
            int jackalLovers = 0;
            int numNeutralAlive = 0;
            int numCrew = 0;

            for (int i = 0; i < GameData.Instance.PlayerCount; i++)
            {
                GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
                if (playerInfo.Object.isCrew()) numCrew++;
                if (!playerInfo.Disconnected)
                {
                    if (!playerInfo.IsDead && !playerInfo.Object.isGM())
                    {
                        numTotalAlive++;

                        bool lover = isLover(playerInfo);
                        if (lover) numLoversAlive++;

                        if (playerInfo.IsImpostor) {
                            numImpostorsAlive++;
                            if (lover) impLovers++;
                        }
                        if (Jackal.jackal != null && Jackal.jackal.PlayerId == playerInfo.PlayerId) {
                            numJackalAlive++;
                            if (lover) jackalLovers++;
                        }
                        if (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == playerInfo.PlayerId) {
                            numJackalAlive++;
                            if (lover) jackalLovers++;
                        }

                        if (playerInfo.Object.isNeutral()) numNeutralAlive++;
                    }
                }
            }

            // In the special case of Mafia being enabled, but only the janitor's left alive,
            // count it as zero impostors alive bc they can't actually do anything.
            if (Godfather.godfather?.Data.IsDead == true && Mafioso.mafioso?.Data.IsDead == true && Janitor.janitor?.Data.IsDead == false)
            {
                numImpostorsAlive = 0;
            }

            TeamCrew = numCrew;
            TeamJackalAlive = numJackalAlive;
            TeamImpostorsAlive = numImpostorsAlive;
            TeamLoversAlive = numLoversAlive;
            NeutralAlive = numNeutralAlive;
            TotalAlive = numTotalAlive;
            TeamImpostorLovers = impLovers;
            TeamJackalLovers = jackalLovers;
        }
    }
}
