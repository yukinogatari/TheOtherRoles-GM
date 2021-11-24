using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [HarmonyPatch]
    public static class ScientistPatch
    {
        public static bool scientistUsing = false;
        public static bool dontConsumeTime {
            get {
                return PlayerControl.LocalPlayer.isRole(RoleTypes.Scientist) && scientistUsing && !CustomRoleSettings.scientistUseVitalsTime.getBool();
            }
        }

        [HarmonyPatch]
        class VitalsMinigameClosePatch
        {
            private static IEnumerable<MethodBase> TargetMethods()
            {
                return typeof(VitalsMinigame).GetMethods().Where(x => x.Name == "Close");
            }

            static void Prefix(VitalsMinigame __instance)
            {
                scientistUsing = false;
            }
        }

        [HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.UseAbility))]
        public class ScientistUseAbilityPatch
        {
            static bool Prefix(ScientistRole __instance)
            {
                if (CustomRoleSettings.scientistUseVitalsTime.getBool() && !MapOptions.canUseVitals)
                    return false;

                scientistUsing = false;
                if (__instance.currentCharge > 0 && !__instance.IsCoolingDown)
                {
                    scientistUsing = true;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.Update))]
        public class ScientistUpdatePatch
        {
            static void Postfix (ScientistRole __instance)
            {
                if (__instance.Player == PlayerControl.LocalPlayer && CustomRoleSettings.scientistUseVitalsTime.getBool() && !MapOptions.canUseVitals)
                {
                    DestroyableSingleton<HudManager>.Instance.AbilityButton.OverrideColor(Palette.DisabledClear);
                    DestroyableSingleton<HudManager>.Instance.AbilityButton.buttonLabelText.color = Palette.DisabledClear;
                }
            }
        }
    }
}