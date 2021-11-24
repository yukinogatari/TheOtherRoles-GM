using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [HarmonyPatch]
    public static class GuardianAngelPatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudPatch
        {
            static void Postfix(MeetingHud __instance)
            {
                bool hideMark = CustomRoleSettings.gaHideMark.getBool();
                if (hideMark)
                {
                    foreach (PlayerVoteArea pva in __instance.playerStates)
                    {
                        pva.GAIcon.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}