using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    class MapBehaviourFixedUpdatePatch
    {
        static void Postfix(MapBehaviour __instance)
        {
            if (PlayerControl.LocalPlayer.isGM())
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p == null || p.isGM()) continue;

                    byte id = p.PlayerId;
                    if (!GM.MapIcons.ContainsKey(id))
                    {
                        GM.MapIcons[id] = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        p.SetPlayerMaterialColors(GM.MapIcons[id]);
                    }

                    Vector3 vector = p.transform.position;
                    vector /= ShipStatus.Instance.MapScale;
                    vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                    vector.z = -1f;
                    GM.MapIcons[id].transform.localPosition = vector;

                    // Set dead players as transparent.
                    float alpha = p.Data.IsDead ? 0.75f : 1f;
                    Color color = GM.MapIcons[id].color;
                    Color newColor = new Color(color.r, color.g, color.b, alpha);
                    if (color != newColor)
                    {
                        GM.MapIcons[id].color = newColor;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
    class MapBehaviourShowNormalMapPatch
    {
        static void Postfix(MapBehaviour __instance)
        {
            if (PlayerControl.LocalPlayer.isGM())
            {
                __instance.taskOverlay.Hide();
                foreach (byte id in GM.MapIcons.Keys)
                {
                    GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(id);
                    PlayerControl.SetPlayerMaterialColors(playerById.ColorId, GM.MapIcons[id]);
                    GM.MapIcons[id].enabled = true;
                }
            }
        }
    }
}