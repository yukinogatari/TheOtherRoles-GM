using HarmonyLib;
using System;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [HarmonyPatch]
    public static class ShapeshifterPatch
    {
        [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
        class ShapeshifterMinigamePatch
        {
            public static void Postfix(ShapeshifterMinigame __instance)
            {
                if (!CustomRoleSettings.shapeshifterShiftAnyone.getBool()) return;

                foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
                {
                    if (PlayerControl.LocalPlayer != pc && !pc.Data.IsDead && pc.Data.Role.IsImpostor)
                    {
                        int count = __instance.potentialVictims.Count;
                        ShapeshifterPanel panel = UnityEngine.Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                        panel.SetPlayer(count, pc.Data, new Action(() => {
                            __instance.Shapeshift(pc);
                            __instance.Close();
                        }));
                        panel.gameObject.SetActive(true);

                        float xpos = __instance.XStart + ((count % 3) * __instance.XOffset);
                        float ypos = __instance.YStart + ((count / 3) * __instance.YOffset);
                        panel.transform.localPosition = new Vector3(xpos, ypos, panel.transform.localPosition.z);

                        __instance.potentialVictims.Add(panel);
                    }
                }
            }
        }
    }
}