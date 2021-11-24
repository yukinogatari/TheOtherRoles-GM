using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using System.Collections;
using System.Collections.Generic;
using System;
using UnhollowerBaseLib;
using TheOtherRoles.Roles;

namespace TheOtherRoles {
    [HarmonyPatch]
    public static class TasksHandler {

        [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.FixedUpdate))]
        public static class NormalPlayerTaskPatch
        {
            public static void Postfix(NormalPlayerTask __instance)
            {
                if (__instance.IsComplete && __instance.Arrow?.isActiveAndEnabled == true)
                    __instance.Arrow?.gameObject?.SetActive(false);
            }
        }

        public static Tuple<int, int> taskInfo(GameData.PlayerInfo playerInfo) {
            if (playerInfo == null) return Tuple.Create(0, 0);

            int TotalTasks = 0;
            int CompletedTasks = 0;
            if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
                playerInfo.Object &&
                (PlayerControl.GameOptions?.GhostsDoTasks == true || !playerInfo.IsDead) &&
                !playerInfo.Object.role()?.IsImpostor == true &&
                !(playerInfo.Object.isRole(CustomRoleTypes.GM) && !GM.hasTasks) &&
                !(playerInfo.Object.isLovers() && !LoversMod.hasTasks) &&
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
                if (__instance == null) return false;

                // TODO: fix?
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                for (int i = 0; i < __instance.AllPlayers?.Count; i++)
                {
                    GameData.PlayerInfo playerInfo = __instance.AllPlayers[i];
                    if (playerInfo.Object?.isLovers() == true && !LoversMod.hasTasks)
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
