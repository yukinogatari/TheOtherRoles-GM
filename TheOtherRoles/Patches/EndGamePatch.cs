  
using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using static TheOtherRoles.GameHistory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hazel;
using UnhollowerBaseLib;
using System;
using System.Text;

namespace TheOtherRoles.Patches
{
    enum CustomGameOverReason
    {
        LoversWin = 10,
        TeamJackalWin = 11,
        MiniLose = 12,
        JesterWin = 13,
        ArsonistWin = 14,
        VultureWin = 15,
        LawyerSoloWin = 16,
        PlagueDoctorWin = 17,
        FoxWin = 18,
        AkujoWin = 19,
    }

    enum WinCondition
    {
        Default,
        LoversTeamWin,
        LoversSoloWin,
        JesterWin,
        JackalWin,
        MiniLose,
        ArsonistWin,
        OpportunistWin,
        VultureWin,
        LawyerSoloWin,
        AdditionalLawyerBonusWin,
        AdditionalLawyerStolenWin,
        AdditionalAlivePursuerWin,
        PlagueDoctorWin,
        FoxWin,
        AkujoWin,

        EveryoneDied,
    }

    enum FinalStatus
    {
        Alive,
        Torched,
        Spelled,
        Sabotage,
        Exiled,
        Dead,
        Suicide,
        Misfire,
        Revenge,
        Diseased,
        Divined,
        Loneliness,
        GMExecuted,
        Disconnected
    }

    static class AdditionalTempData
    {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition winCondition = WinCondition.Default;
        public static List<WinCondition> additionalWinConditions = new List<WinCondition>();
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();
        public static bool isGM = false;
        public static GameOverReason gameOverReason;

        public static Dictionary<int, PlayerControl> plagueDoctorInfected = new Dictionary<int, PlayerControl>();
        public static Dictionary<int, float> plagueDoctorProgress = new Dictionary<int, float>();

        public static void clear()
        {
            playerRoles.Clear();
            additionalWinConditions.Clear();
            winCondition = WinCondition.Default;
            isGM = false;
        }

        internal class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public string NameSuffix { get; set; }
            public List<RoleInfo> Roles { get; set; }
            public string RoleString { get; set; }
            public int TasksCompleted { get; set; }
            public int TasksTotal { get; set; }
            public FinalStatus Status { get; set; }
            public int PlayerId { get; set; }
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch
    {

        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            Camouflager.resetCamouflage();
            Morphling.resetMorph();

            AdditionalTempData.gameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            var gameOverReason = AdditionalTempData.gameOverReason;

            // 狐の勝利条件を満たしたか確認する
            bool isFoxAlive = Fox.isFoxAlive();
            bool isFoxCompletedTasks = Fox.isFoxCompletedTasks(); // 生存中の狐が1匹でもタスクを全て終えていること
            if (isFoxAlive && isFoxCompletedTasks) {
                // タスク勝利の場合はオプションの設定次第
                if (gameOverReason == GameOverReason.HumansByTask && !Fox.crewWinsByTasks)
                {
                    gameOverReason = (GameOverReason)CustomGameOverReason.FoxWin;
                }

                // 第三陣営の勝利以外の場合に狐が生存していたら狐の勝ち
                else if (gameOverReason != (GameOverReason)CustomGameOverReason.PlagueDoctorWin &&
                    gameOverReason != (GameOverReason)CustomGameOverReason.ArsonistWin &&
                    gameOverReason != (GameOverReason)CustomGameOverReason.JesterWin &&
                    gameOverReason != (GameOverReason)CustomGameOverReason.VultureWin &&
                    gameOverReason != (GameOverReason)CustomGameOverReason.AkujoWin &&
                    gameOverReason != (GameOverReason)GameOverReason.HumansByTask)
                {
                    gameOverReason = (GameOverReason)CustomGameOverReason.FoxWin;
                }
            }
            AdditionalTempData.clear();

            //foreach (var pc in PlayerControl.AllPlayerControls)
            var excludeRoles = new RoleType[] { RoleType.Lovers };
            foreach (var p in GameData.Instance.AllPlayers)
            {
                //var p = pc.Data;
                var roles = RoleInfo.getRoleInfoForPlayer(p.Object, excludeRoles, includeHidden: true);
                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p);
                var finalStatus = finalStatuses[p.PlayerId] =
                    p.Disconnected == true ? FinalStatus.Disconnected :
                    finalStatuses.ContainsKey(p.PlayerId) ? finalStatuses[p.PlayerId] :
                    p.IsDead == true ? FinalStatus.Dead :
                    gameOverReason == GameOverReason.ImpostorBySabotage && !p.Role.IsImpostor ? FinalStatus.Sabotage :
                    FinalStatus.Alive;
                var suffix = p.Object.modifyNameText("");


                if (gameOverReason == GameOverReason.HumansByTask && p.Object.isCrew()) tasksCompleted = tasksTotal;

                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo()
                {
                    PlayerName = p.PlayerName,
                    PlayerId = p.PlayerId,
                    NameSuffix = suffix,
                    Roles = roles,
                    RoleString = RoleInfo.GetRolesString(p.Object, true, excludeRoles, true),
                    TasksTotal = tasksTotal,
                    TasksCompleted = tasksCompleted,
                    Status = finalStatus,
                });
            }

            AdditionalTempData.isGM = CustomOptionHolder.gmEnabled.getBool() && PlayerControl.LocalPlayer.isGM();
            AdditionalTempData.plagueDoctorInfected = PlagueDoctor.infected;
            AdditionalTempData.plagueDoctorProgress = PlagueDoctor.progress;


            // Remove Jester, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new List<PlayerControl>();
            if (Jester.jester != null) notWinners.Add(Jester.jester);
            if (Sidekick.sidekick != null) notWinners.Add(Sidekick.sidekick);
            if (Jackal.jackal != null) notWinners.Add(Jackal.jackal);
            if (Arsonist.arsonist != null) notWinners.Add(Arsonist.arsonist);
            if (Vulture.vulture != null) notWinners.Add(Vulture.vulture);
            if (Lawyer.lawyer != null) notWinners.Add(Lawyer.lawyer);
            if (Pursuer.pursuer != null) notWinners.Add(Pursuer.pursuer);

            notWinners.AddRange(Jackal.formerJackals);
            notWinners.AddRange(Madmate.allPlayers);
            notWinners.AddRange(Opportunist.allPlayers);
            notWinners.AddRange(PlagueDoctor.allPlayers);
            notWinners.AddRange(Fox.allPlayers);
            notWinners.AddRange(Immoralist.allPlayers);
            notWinners.AddRange(Akujo.allPlayers);
            notWinners.AddRange(AkujoHonmei.allPlayers);

            // Neutral shifter can't win
            if (Shifter.shifter != null && Shifter.isNeutral) notWinners.Add(Shifter.shifter);

            // GM can't win at all, and we're treating lovers as a separate class
            if (GM.gm != null) notWinners.Add(GM.gm);

            if (Lovers.separateTeam)
            {
                foreach (var couple in Lovers.couples)
                {
                    notWinners.Add(couple.lover1);
                    notWinners.Add(couple.lover2);
                }
            }

            List<WinningPlayerData> winnersToRemove = new List<WinningPlayerData>();
            foreach (WinningPlayerData winner in TempData.winners)
            {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) TempData.winners.Remove(winner);

            bool saboWin = gameOverReason == GameOverReason.ImpostorBySabotage;

            bool jesterWin = Jester.jester != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            bool arsonistWin = Arsonist.arsonist != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            bool miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
            bool loversWin = Lovers.anyAlive() && !(Lovers.separateTeam && gameOverReason == GameOverReason.HumansByTask);
            bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin && ((Jackal.jackal != null && Jackal.jackal.isAlive()) || (Sidekick.sidekick != null && !Sidekick.sidekick.isAlive()));
            bool vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
            bool lawyerSoloWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.LawyerSoloWin;
            bool plagueDoctorWin = PlagueDoctor.exists && gameOverReason == (GameOverReason)CustomGameOverReason.PlagueDoctorWin;
            bool foxWin = Fox.exists && gameOverReason == (GameOverReason)CustomGameOverReason.FoxWin;
            bool everyoneDead = AdditionalTempData.playerRoles.All(x => x.Status != FinalStatus.Alive);
            bool akujoWin = Akujo.numAlive > 0 && gameOverReason != GameOverReason.HumansByTask;

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

            else if (plagueDoctorWin)
            {
                foreach (var pd in PlagueDoctor.players)
                {
                    TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                    WinningPlayerData wpd = new WinningPlayerData(pd.player.Data);
                    TempData.winners.Add(wpd);
                    AdditionalTempData.winCondition = WinCondition.PlagueDoctorWin;
                }
            }

            else if (everyoneDead)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                AdditionalTempData.winCondition = WinCondition.EveryoneDied;
            }

            // Vulture win
            else if (vultureWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Vulture.vulture.Data);
                TempData.winners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.VultureWin;
            }

            // Akujo win conditions
            else if (akujoWin)
            {
                AdditionalTempData.winCondition = WinCondition.AkujoWin;
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (var akujo in Akujo.players)
                {
                    if (akujo.player.isAlive() && akujo.honmei?.player != null && akujo.honmei.player.isAlive())
                    {
                        TempData.winners.Add(new WinningPlayerData(akujo.player.Data));
                        TempData.winners.Add(new WinningPlayerData(akujo.honmei.player.Data));
                    }
                }
            }

            // Lovers win conditions
            else if (loversWin)
            {
                // Double win for lovers, crewmates also win
                if (TempData.DidHumansWin(gameOverReason) && !Lovers.separateTeam && Lovers.anyNonKillingCouples())
                {
                    AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.LoversTeamWin);
                }
                // Lovers solo win
                else
                {
                    AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                    TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();

                    foreach (var couple in Lovers.couples)
                    {
                        if (couple.existingAndAlive)
                        {
                            TempData.winners.Add(new WinningPlayerData(couple.lover1.Data));
                            TempData.winners.Add(new WinningPlayerData(couple.lover2.Data));
                        }
                    }
                }
            }

            // Jackal win condition (should be implemented using a proper GameOverReason in the future)
            else if (teamJackalWin)
            {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.JackalWin;
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Jackal.jackal.Data);
                wpd.IsImpostor = false;
                TempData.winners.Add(wpd);
                // If there is a sidekick. The sidekick also wins
                if (Sidekick.sidekick != null)
                {
                    WinningPlayerData wpdSidekick = new WinningPlayerData(Sidekick.sidekick.Data);
                    wpdSidekick.IsImpostor = false;
                    TempData.winners.Add(wpdSidekick);
                }
                foreach (var player in Jackal.formerJackals)
                {
                    WinningPlayerData wpdFormerJackal = new WinningPlayerData(player.Data);
                    wpdFormerJackal.IsImpostor = false;
                    TempData.winners.Add(wpdFormerJackal);
                }
            }
            // Lawyer solo win 
            else if (lawyerSoloWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new WinningPlayerData(Lawyer.lawyer.Data);
                TempData.winners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.LawyerSoloWin;
            }
            else if (foxWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (var fox in Fox.players)
                {
                    WinningPlayerData wpd = new WinningPlayerData(fox.player.Data);
                    TempData.winners.Add(wpd);
                }
                foreach (var immoralist in Immoralist.players)
                {
                    WinningPlayerData wpd = new WinningPlayerData(immoralist.player.Data);
                    TempData.winners.Add(wpd);
                }
                AdditionalTempData.winCondition = WinCondition.FoxWin;
            }

            // Madmate win with impostors
            if (Madmate.exists && TempData.winners.ToArray().Any(x => x.IsImpostor))
            {
                foreach (var p in Madmate.allPlayers)
                {
                    WinningPlayerData wpd = new WinningPlayerData(p.Data);
                    TempData.winners.Add(wpd);
                }
            }

            // Possible Additional winner: Lawyer
            if (!lawyerSoloWin && Lawyer.lawyer != null && Lawyer.target != null && Lawyer.target.isAlive())
            {
                WinningPlayerData winningClient = null;
                foreach (WinningPlayerData winner in TempData.winners)
                {
                    if (winner.PlayerName == Lawyer.target.Data.PlayerName)
                        winningClient = winner;
                }

                if (winningClient != null)
                { // The Lawyer wins if the client is winning (and alive, but if he wasn't the Lawyer shouldn't exist anymore)
                    if (!TempData.winners.ToArray().Any(x => x.PlayerName == Lawyer.lawyer.Data.PlayerName))
                        TempData.winners.Add(new WinningPlayerData(Lawyer.lawyer.Data));
                    if (Lawyer.lawyer.isAlive())
                    { // The Lawyer steals the clients win
                        TempData.winners.Remove(winningClient);
                        AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerStolenWin);
                    }
                    else
                    { // The Lawyer wins together with the client
                        AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerBonusWin);
                    }
                }
            }

            // Extra win conditions for non-impostor roles
            if (!saboWin)
            {
                bool oppWin = false;
                foreach (var p in Opportunist.livingPlayers)
                {
                    if (!TempData.winners.ToArray().Any(x => x.PlayerName == p.Data.PlayerName))
                        TempData.winners.Add(new WinningPlayerData(p.Data));
                    oppWin = true;
                }
                if (oppWin)
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.OpportunistWin);

                // Possible Additional winner: Pursuer
                if (Pursuer.pursuer != null && Pursuer.pursuer.isAlive())
                {
                    if (!TempData.winners.ToArray().Any(x => x.PlayerName == Pursuer.pursuer.Data.PlayerName))
                        TempData.winners.Add(new WinningPlayerData(Pursuer.pursuer.Data));
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAlivePursuerWin);
                }
            }

            foreach (WinningPlayerData wpd in TempData.winners)
            {
                wpd.IsDead = wpd.IsDead || AdditionalTempData.playerRoles.Any(x => x.PlayerName == wpd.PlayerName && x.Status != FinalStatus.Alive);
            }

            // Reset Settings
            RPCProcedure.resetVariables();
        }

        public class EndGameNavigationPatch
        {
            public static TMPro.TMP_Text textRenderer;

            [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowProgression))]
            public class ShowProgressionPatch
            {
                public static void Prefix()
                {
                    if (textRenderer != null)
                    {
                        textRenderer.gameObject.SetActive(false);
                    }
                }
            }

            [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
            public class EndGameManagerSetUpPatch
            {
                public static void Postfix(EndGameManager __instance)
                {
                    // Delete and readd PoolablePlayers always showing the name and role of the player
                    foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
                    {
                        UnityEngine.Object.Destroy(pb.gameObject);
                    }
                    int num = Mathf.CeilToInt(7.5f);
                    List<WinningPlayerData> list = TempData.winners.ToArray().ToList().OrderBy(delegate (WinningPlayerData b)
                    {
                        if (!b.IsYou)
                        {
                            return 0;
                        }
                        return -1;
                    }).ToList<WinningPlayerData>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        WinningPlayerData winningPlayerData2 = list[i];
                        int num2 = (i % 2 == 0) ? -1 : 1;
                        int num3 = (i + 1) / 2;
                        float num4 = (float)num3 / (float)num;
                        float num5 = Mathf.Lerp(1f, 0.75f, num4);
                        float num6 = (float)((i == 0) ? -8 : -1);
                        PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
                        poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
                        float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
                        Vector3 vector = new Vector3(num7, num7, 1f);
                        poolablePlayer.transform.localScale = vector;
                        poolablePlayer.UpdateFromPlayerOutfit(winningPlayerData2, winningPlayerData2.IsDead);
                        if (winningPlayerData2.IsDead)
                        {
                            poolablePlayer.CurrentBodySprite.BodySprite.sprite = poolablePlayer.CurrentBodySprite.GhostSprite;
                            poolablePlayer.SetDeadFlipX(i % 2 == 0);
                        }
                        else
                        {
                            poolablePlayer.SetFlipX(i % 2 == 0);
                        }

                        poolablePlayer.NameText.color = Color.white;
                        poolablePlayer.NameText.lineSpacing *= 0.7f;
                        poolablePlayer.NameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
                        poolablePlayer.NameText.transform.localPosition = new Vector3(poolablePlayer.NameText.transform.localPosition.x, poolablePlayer.NameText.transform.localPosition.y, -15f);
                        poolablePlayer.NameText.text = winningPlayerData2.PlayerName;

                        foreach (var data in AdditionalTempData.playerRoles)
                        {
                            if (data.PlayerName != winningPlayerData2.PlayerName) continue;
                            poolablePlayer.NameText.text += data.NameSuffix + $"\n<size=80%>{data.RoleString}</size>";
                        }
                    }

                    // Additional code
                    GameObject bonusTextObject = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                    bonusTextObject.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
                    bonusTextObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    textRenderer = bonusTextObject.GetComponent<TMPro.TMP_Text>();
                    textRenderer.text = "";

                    if (AdditionalTempData.isGM)
                    {
                        __instance.WinText.text = ModTranslation.getString("gmGameOver");
                        __instance.WinText.color = GM.color;
                    }

                    string bonusText = "";

                    if (AdditionalTempData.winCondition == WinCondition.JesterWin)
                    {
                        bonusText = "jesterWin";
                        textRenderer.color = Jester.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Jester.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.ArsonistWin)
                    {
                        bonusText = "arsonistWin";
                        textRenderer.color = Arsonist.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Arsonist.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.VultureWin)
                    {
                        bonusText = "vultureWin";
                        textRenderer.color = Vulture.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Vulture.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.LawyerSoloWin)
                    {
                        bonusText = "lawyerWin";
                        textRenderer.color = Lawyer.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Lawyer.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.PlagueDoctorWin)
                    {
                        bonusText = "plagueDoctorWin";
                        textRenderer.color = PlagueDoctor.color;
                        __instance.BackgroundBar.material.SetColor("_Color", PlagueDoctor.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.FoxWin)
                    {
                        bonusText = "foxWin";
                        textRenderer.color = Fox.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Fox.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.LoversTeamWin)
                    {
                        bonusText = "crewWin";
                        textRenderer.color = Lovers.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.LoversSoloWin)
                    {
                        bonusText = "loversWin";
                        textRenderer.color = Lovers.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.AkujoWin)
                    {
                        bonusText = "akujoWin";
                        textRenderer.color = Akujo.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Akujo.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.JackalWin)
                    {
                        bonusText = "jackalWin";
                        textRenderer.color = Jackal.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Jackal.color);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.EveryoneDied)
                    {
                        bonusText = "everyoneDied";
                        textRenderer.color = Palette.DisabledGrey;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                    }
                    else if (AdditionalTempData.winCondition == WinCondition.MiniLose)
                    {
                        bonusText = "miniDied";
                        textRenderer.color = Mini.color;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                    }
                    else if (AdditionalTempData.gameOverReason == GameOverReason.HumansByTask || AdditionalTempData.gameOverReason == GameOverReason.HumansByVote)
                    {
                        bonusText = "crewWin";
                        textRenderer.color = Palette.White;
                    }
                    else if (AdditionalTempData.gameOverReason == GameOverReason.ImpostorByKill || AdditionalTempData.gameOverReason == GameOverReason.ImpostorBySabotage || AdditionalTempData.gameOverReason == GameOverReason.ImpostorByVote)
                    {
                        bonusText = "impostorWin";
                        textRenderer.color = Palette.ImpostorRed;
                    }

                    string extraText = "";
                    foreach (WinCondition w in AdditionalTempData.additionalWinConditions)
                    {
                        switch (w)
                        {
                            case WinCondition.OpportunistWin:
                                extraText += ModTranslation.getString("opportunistExtra");
                                break;
                            case WinCondition.LoversTeamWin:
                                extraText += ModTranslation.getString("loversExtra");
                                break;
                            case WinCondition.AdditionalAlivePursuerWin:
                                extraText += ModTranslation.getString("pursuerExtra");
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

                    foreach (WinCondition cond in AdditionalTempData.additionalWinConditions)
                    {
                        switch (cond)
                        {
                            case WinCondition.AdditionalLawyerStolenWin:
                                textRenderer.text += $"\n{Helpers.cs(Lawyer.color, ModTranslation.getString("lawyerExtraStolen"))}";
                                break;
                            case WinCondition.AdditionalLawyerBonusWin:
                                textRenderer.text += $"\n{Helpers.cs(Lawyer.color, ModTranslation.getString("lawyerExtraBonus"))}";
                                break;
                        }
                    }

                    if (MapOptions.showRoleSummary)
                    {
                        var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                        GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                        roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                        roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                        var roleSummaryText = new StringBuilder();
                        roleSummaryText.AppendLine(ModTranslation.getString("roleSummaryText"));
                        AdditionalTempData.playerRoles.Sort((x, y) =>
                        {
                            RoleInfo roleX = x.Roles.FirstOrDefault();
                            RoleInfo roleY = y.Roles.FirstOrDefault();
                            RoleType idX = roleX == null ? RoleType.NoRole : roleX.roleType;
                            RoleType idY = roleY == null ? RoleType.NoRole : roleY.roleType;

                            if (x.Status == y.Status)
                            {
                                if (idX == idY)
                                {
                                    return x.PlayerName.CompareTo(y.PlayerName);
                                }
                                return idX.CompareTo(idY);
                            }
                            return x.Status.CompareTo(y.Status);

                        });

                        bool plagueExists = AdditionalTempData.playerRoles.Any(x => x.Roles.Contains(RoleInfo.plagueDoctor));
                        foreach (var data in AdditionalTempData.playerRoles)
                        {
                            var taskInfo = data.TasksTotal > 0 ? $"<color=#FAD934FF>{data.TasksCompleted}/{data.TasksTotal}</color>" : "";
                            string aliveDead = ModTranslation.getString("roleSummary" + data.Status.ToString(), def: "-");
                            string result = $"{data.PlayerName + data.NameSuffix}<pos=18.5%>{taskInfo}<pos=25%>{aliveDead}<pos=34%>{data.RoleString}";
                            if (plagueExists && !data.Roles.Contains(RoleInfo.plagueDoctor))
                            {
                                result += "<pos=52.5%>";
                                if (AdditionalTempData.plagueDoctorInfected.ContainsKey(data.PlayerId))
                                {
                                    result += Helpers.cs(Color.red, ModTranslation.getString("plagueDoctorInfectedText"));
                                }
                                else
                                {
                                    float progress = AdditionalTempData.plagueDoctorProgress.ContainsKey(data.PlayerId) ? AdditionalTempData.plagueDoctorProgress[data.PlayerId] : 0f;
                                    result += PlagueDoctor.getProgressString(progress);
                                }
                            }
                            roleSummaryText.AppendLine(result);
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
            class CheckEndCriteriaPatch
            {
                public static bool Prefix(ShipStatus __instance)
                {
                    if (!GameData.Instance) return false;
                    if (DestroyableSingleton<TutorialManager>.InstanceExists) return true; // InstanceExists | Don't check Custom Criteria when in Tutorial
                    if (HudManager.Instance.IsIntroDisplayed) return false;

                    var statistics = new PlayerStatistics(__instance);
                    if (CheckAndEndGameForMiniLose(__instance)) return false;
                    if (CheckAndEndGameForJesterWin(__instance)) return false;
                    if (CheckAndEndGameForLawyerMeetingWin(__instance)) return false;
                    if (CheckAndEndGameForArsonistWin(__instance)) return false;
                    if (CheckAndEndGameForVultureWin(__instance)) return false;
                    if (CheckAndEndGameForPlagueDoctorWin(__instance)) return false;
                    if (CheckAndEndGameForSabotageWin(__instance)) return false;
                    if (CheckAndEndGameForTaskWin(__instance)) return false;
                    if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
                    if (CheckAndEndGameForAkujoWin(__instance, statistics)) return false;
                    if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
                    if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
                    if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
                    return false;
                }

                private static bool CheckAndEndGameForMiniLose(ShipStatus __instance)
                {
                    if (Mini.triggerMiniLose)
                    {
                        UncheckedEndGame(CustomGameOverReason.MiniLose);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForJesterWin(ShipStatus __instance)
                {
                    if (Jester.triggerJesterWin)
                    {
                        UncheckedEndGame(CustomGameOverReason.JesterWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance)
                {
                    if (Arsonist.triggerArsonistWin)
                    {
                        UncheckedEndGame(CustomGameOverReason.ArsonistWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForVultureWin(ShipStatus __instance)
                {
                    if (Vulture.triggerVultureWin)
                    {
                        UncheckedEndGame(CustomGameOverReason.VultureWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForLawyerMeetingWin(ShipStatus __instance)
                {
                    if (Lawyer.triggerLawyerWin)
                    {
                        UncheckedEndGame(CustomGameOverReason.LawyerSoloWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForPlagueDoctorWin(ShipStatus __instance)
                {
                    if (PlagueDoctor.triggerPlagueDoctorWin)
                    {
                        UncheckedEndGame(CustomGameOverReason.PlagueDoctorWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance)
                {
                    if (__instance.Systems == null) return false;
                    ISystemType systemType = __instance.Systems.ContainsKey(SystemTypes.LifeSupp) ? __instance.Systems[SystemTypes.LifeSupp] : null;
                    if (systemType != null)
                    {
                        LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                        if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
                        {
                            EndGameForSabotage(__instance);
                            lifeSuppSystemType.Countdown = 10000f;
                            return true;
                        }
                    }
                    ISystemType systemType2 = __instance.Systems.ContainsKey(SystemTypes.Reactor) ? __instance.Systems[SystemTypes.Reactor] : null;
                    if (systemType2 == null)
                    {
                        systemType2 = __instance.Systems.ContainsKey(SystemTypes.Laboratory) ? __instance.Systems[SystemTypes.Laboratory] : null;
                    }
                    if (systemType2 != null)
                    {
                        ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                        if (criticalSystem != null && criticalSystem.Countdown < 0f)
                        {
                            EndGameForSabotage(__instance);
                            criticalSystem.ClearSabotage();
                            return true;
                        }
                    }
                    return false;
                }

                private static bool CheckAndEndGameForTaskWin(ShipStatus __instance)
                {
                    if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
                    {
                        UncheckedEndGame(GameOverReason.HumansByTask);
                        return true;
                    }

                    if (Fox.exists && !Fox.crewWinsByTasks)
                    {
                        // 狐生存かつタスク完了時に生存中のクルーがタスクを全て終わらせたら勝ち
                        // 死んだプレイヤーが意図的にタスクを終了させないのを防止するため
                        bool isFoxAlive = Fox.isFoxAlive();
                        bool isFoxCompletedtasks = Fox.isFoxCompletedTasks();
                        int numDeadPlayerUncompletedTasks = 0;
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            foreach (var task in player.Data.Tasks)
                            {
                                if (player.Data.IsDead && player.isCrew())
                                {
                                    if (!task.Complete)
                                    {
                                        numDeadPlayerUncompletedTasks++;
                                    }
                                }
                            }
                        }

                        if (isFoxCompletedtasks && isFoxAlive && GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks + numDeadPlayerUncompletedTasks)
                        {
                            UncheckedEndGame(GameOverReason.HumansByTask);
                            return true;
                        }
                    }

                    return false;
                }

                private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics)
                {
                    if (statistics.CouplesAlive == 1 && statistics.TotalAlive <= 3)
                    {
                        UncheckedEndGame(CustomGameOverReason.LoversWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForAkujoWin(ShipStatus __instance, PlayerStatistics statistics)
                {
                    // if we have a majority, akujo wins, same as lovers
                    if (Akujo.numAlive == 1 && statistics.TotalAlive <= 3)
                    {
                        UncheckedEndGame(CustomGameOverReason.AkujoWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForJackalWin(ShipStatus __instance, PlayerStatistics statistics)
                {
                    if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive - statistics.FoxAlive &&
                        statistics.TeamImpostorsAlive == 0 && 
                        (statistics.TeamJackalLovers == 0 || statistics.TeamJackalLovers >= statistics.CouplesAlive * 2)
                       )
                    {
                        UncheckedEndGame(CustomGameOverReason.TeamJackalWin);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics)
                {
                    if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive - statistics.FoxAlive &&
                        statistics.TeamJackalAlive == 0 &&
                        (statistics.TeamImpostorLovers == 0 || statistics.TeamImpostorLovers >= statistics.CouplesAlive * 2)
                       )
                    {
                        GameOverReason endReason;
                        switch (TempData.LastDeathReason)
                        {
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
                        UncheckedEndGame(endReason);
                        return true;
                    }
                    return false;
                }

                private static bool CheckAndEndGameForCrewmateWin(ShipStatus __instance, PlayerStatistics statistics)
                {
                    if (statistics.TeamCrew > 0 && statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0)
                    {
                        UncheckedEndGame(GameOverReason.HumansByVote);
                        return true;
                    }
                    return false;
                }

                private static void EndGameForSabotage(ShipStatus __instance)
                {
                    UncheckedEndGame(GameOverReason.ImpostorBySabotage);
                    return;
                }

                private static void UncheckedEndGame(GameOverReason reason)
                {
                    ShipStatus.RpcEndGame(reason, false);
                }

                private static void UncheckedEndGame(CustomGameOverReason reason)
                {
                    UncheckedEndGame((GameOverReason)reason);
                }
            }

            internal class PlayerStatistics
            {
                public int TeamImpostorsAlive { get; set; }
                public int TeamJackalAlive { get; set; }
                public int TeamLoversAlive { get; set; }
                public int CouplesAlive { get; set; }
                public int TeamCrew { get; set; }
                public int NeutralAlive { get; set; }
                public int TotalAlive { get; set; }
                public int TeamImpostorLovers { get; set; }
                public int TeamJackalLovers { get; set; }
                public int FoxAlive { get; set; }

                public PlayerStatistics(ShipStatus __instance)
                {
                    GetPlayerCounts();
                }

                private bool isLover(GameData.PlayerInfo p)
                {
                    foreach (var couple in Lovers.couples)
                    {
                        if (p.PlayerId == couple.lover1.PlayerId || p.PlayerId == couple.lover2.PlayerId) return true;
                    }
                    return false;
                }

                private void GetPlayerCounts()
                {
                    int numJackalAlive = 0;
                    int numImpostorsAlive = 0;
                    int numTotalAlive = 0;
                    int numNeutralAlive = 0;
                    int numCrew = 0;

                    int numLoversAlive = 0;
                    int numCouplesAlive = 0;
                    int impLovers = 0;
                    int jackalLovers = 0;

                    for (int i = 0; i < GameData.Instance.PlayerCount; i++)
                    {
                        GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
                        if (!playerInfo.Disconnected)
                        {
                            if (playerInfo.Object.isCrew()) numCrew++;
                            if (!playerInfo.IsDead && !playerInfo.Object.isGM())
                            {
                                numTotalAlive++;

                                bool lover = isLover(playerInfo);
                                if (lover) numLoversAlive++;

                                if (playerInfo.Role.IsImpostor)
                                {
                                    numImpostorsAlive++;
                                    if (lover) impLovers++;
                                }
                                if (Jackal.jackal != null && Jackal.jackal.PlayerId == playerInfo.PlayerId)
                                {
                                    numJackalAlive++;
                                    if (lover) jackalLovers++;
                                }
                                if (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == playerInfo.PlayerId)
                                {
                                    numJackalAlive++;
                                    if (lover) jackalLovers++;
                                }

                                if (playerInfo.Object.isNeutral()) numNeutralAlive++;
                            }
                        }
                    }

                    foreach (var couple in Lovers.couples)
                    {
                        if (couple.alive) numCouplesAlive++;
                    }

                    // In the special case of Mafia being enabled, but only the janitor's left alive,
                    // count it as zero impostors alive bc they can't actually do anything.
                    if (Godfather.godfather?.isDead() == true && Mafioso.mafioso?.isDead() == true && Janitor.janitor?.isDead() == false)
                    {
                        numImpostorsAlive = 0;
                    }

                    TeamCrew = numCrew;
                    TeamJackalAlive = numJackalAlive;
                    TeamImpostorsAlive = numImpostorsAlive;
                    TeamLoversAlive = numLoversAlive;
                    NeutralAlive = numNeutralAlive;
                    TotalAlive = numTotalAlive;
                    CouplesAlive = numCouplesAlive;
                    TeamImpostorLovers = impLovers;
                    TeamJackalLovers = jackalLovers;
                    FoxAlive = Fox.livingPlayers.Count;
                }
            }
        }
    }
}