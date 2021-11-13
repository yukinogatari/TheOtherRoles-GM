using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    enum CustomGameOverReason
    {
        /*HumansByVote = 0,
        HumansByTask = 1,
        ImpostorByVote = 2,
        ImpostorByKill = 3,
        ImpostorBySabotage = 4,
        ImpostorDisconnect = 5,
        HumansDisconnect = 6,*/

        LoversWin = 10,
        TeamJackalWin = 11,
        MiniLose = 12,
        JesterWin = 13,
        ArsonistWin = 14,
        VultureWin = 15
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
        VultureWin
    }

    [Harmony]
    class WinHandler
    {
        // public virtual bool DidWin(GameOverReason gameOverReason) => default;
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.DidWin))]
        public static class DidWinPatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref GameOverReason gameOverReason, ref bool __result)
            {
                switch (gameOverReason)
                {
                    case GameOverReason.HumansByVote:
                    case GameOverReason.HumansByTask:
                        if (__instance.TeamType == RoleTeamTypes.Crewmate)
                            __result = true;

                        break;
                    case GameOverReason.ImpostorByVote:
                        break;
                    case GameOverReason.ImpostorByKill:
                        break;
                    case GameOverReason.ImpostorBySabotage:
                        break;
                    case GameOverReason.ImpostorDisconnect:
                        break;
                    case GameOverReason.HumansDisconnect:
                        break;
                    case (GameOverReason)CustomGameOverReason.LoversWin:
                        break;
                    case (GameOverReason)CustomGameOverReason.TeamJackalWin:
                        break;
                    case (GameOverReason)CustomGameOverReason.MiniLose:
                        break;
                    case (GameOverReason)CustomGameOverReason.JesterWin:
                        break;
                    case (GameOverReason)CustomGameOverReason.ArsonistWin:
                        break;
                    case (GameOverReason)CustomGameOverReason.VultureWin:
                        break;
                    default:
                        break;
                }
                return true;
            }
        }
    }
}