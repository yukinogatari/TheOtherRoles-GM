using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.ShowMap))]
    class OpenMapPatch
    {
        static bool Prefix(HudManager __instance, [HarmonyArgument(0)] Il2CppSystem.Action<MapBehaviour> mapAction)
        {
            return true;
        }
    }
}