using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using System.Collections;
using System.Collections.Generic;
using System;
using UnhollowerBaseLib;

namespace TheOtherRoles {
    [HarmonyPatch]
    public static class TasksHandler {

        [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.FixedUpdate))]
        public static class NormalPlayerTaskPatch
        {
            public static void Postfix(NormalPlayerTask __instance)
            {
                bool showArrows = !MapOptions.hideTaskArrows && !__instance.IsComplete && __instance.TaskStep > 0;
                __instance.Arrow?.gameObject?.SetActive(showArrows);
            }
        }

        [HarmonyPatch(typeof(AirshipUploadTask), nameof(AirshipUploadTask.FixedUpdate))]
        public static class AirshipUploadTaskPatch
        {
            public static void Postfix(AirshipUploadTask __instance)
            {
                bool showArrows = !MapOptions.hideTaskArrows && !__instance.IsComplete && __instance.TaskStep > 0;
                __instance.Arrows?.DoIf(x => x != null, x => x.gameObject?.SetActive(showArrows));
            }
        }

        [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.UpdateArrow))]
        public static class NormalPlayerTaskUpdateArrowPatch
        {
            public static void Postfix(NormalPlayerTask __instance)
            {
                if (MapOptions.hideTaskArrows)
                {
                    __instance.Arrow?.gameObject?.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(AirshipUploadTask), nameof(AirshipUploadTask.UpdateArrow))]
        public static class AirshipUploadTaskUpdateArrowPatch
        {
            public static void Postfix(AirshipUploadTask __instance)
            {
                if (MapOptions.hideTaskArrows)
                {
                    __instance.Arrows?.DoIf(x => x != null, x => x.gameObject?.SetActive(false));
                }
            }
        }

        public static Tuple<int, int> taskInfo(GameData.PlayerInfo playerInfo) {
            int TotalTasks = 0;
            int CompletedTasks = 0;
            if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
                playerInfo.Object &&
                (PlayerControl.GameOptions.GhostsDoTasks || !playerInfo.IsDead) &&
                playerInfo.Role && playerInfo.Role.TasksCountTowardProgress &&
                !(playerInfo.Object.isGM() && !GM.hasTasks) &&
                !(playerInfo.Object.isLovers() && !Lovers.hasTasks) &&
                !playerInfo.Object.hasFakeTasks()
                ) {

                for (int j = 0; j < playerInfo.Tasks.Count; j++) {
                    TotalTasks++;
                    if (playerInfo.Tasks[j].Complete) {
                        CompletedTasks++;
                    }
                }
            }
            return Tuple.Create(CompletedTasks, TotalTasks);
        }

        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private static class GameDataRecomputeTaskCountsPatch {
            private static bool Prefix(GameData __instance) {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                for (int i = 0; i < __instance.AllPlayers.Count; i++) {
                    GameData.PlayerInfo playerInfo = __instance.AllPlayers[i];
                    if (playerInfo.Object &&
                        ((playerInfo.Object?.isLovers() == true && !Lovers.tasksCount) ||
                         (playerInfo.PlayerId == Shifter.shifter?.PlayerId && Shifter.isNeutral) || // Neutral shifter has tasks, but they don't count
                          playerInfo.PlayerId == Lawyer.lawyer?.PlayerId || // Tasks of the Lawyer do not count
                         (playerInfo.PlayerId == Pursuer.pursuer?.PlayerId && Pursuer.pursuer.Data.IsDead) || // Tasks of the Pursuer only count, if he's alive
                          playerInfo.Object?.isRole(RoleType.Fox) == true ||
                         (Madmate.hasTasks && playerInfo.Object?.hasModifier(ModifierType.Madmate) == true)
                        )
                    )
                        continue;
                    var (playerCompleted, playerTotal) = taskInfo(playerInfo);
                    __instance.TotalTasks += playerTotal;
                    __instance.CompletedTasks += playerCompleted;
                }
                return false;
            }
        }


    }
}
