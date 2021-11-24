using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Roles;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Patches {

    [HarmonyPatch(typeof(ShipStatus))]
    public class ShipStatusPatch {

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
        public class ShipStatusFixedUpdate
        {
            public static void Postfix(ShipStatus __instance)
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
                Helpers.setBasePlayerOutlines();
                Helpers.refreshRoleDescription(PlayerControl.LocalPlayer);
                Helpers.updatePlayerInfo();
                Helpers.bendTimeUpdate();
                CustomRoleManager.FixedUpdate();
                Garlic.UpdateAll();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        public static bool Prefix(ref float __result, ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player)
        {
            ISystemType systemType = __instance.Systems.ContainsKey(SystemTypes.Electrical) ? __instance.Systems[SystemTypes.Electrical] : null;
            if (systemType == null) return true;
            SwitchSystem switchSystem = systemType.TryCast<SwitchSystem>();
            if (switchSystem == null) return true;

            float num = (float)switchSystem.Value / 255f;
            var pc = player.Object;

            if (player == null || player.IsDead || pc.isRole(CustomRoleTypes.GM)) // IsDead
                __result = __instance.MaxLightRadius;

            else if (player.Object.role()?.IsImpostor == true
                || (pc.isRole(CustomRoleTypes.Jackal) && Jackal.hasImpostorVision)
                || (pc.isRole(CustomRoleTypes.Sidekick) && Sidekick.hasImpostorVision)
                || (pc.isRole(CustomRoleTypes.Spy) && Spy.hasImpostorVision)
                || (pc.isRole(CustomRoleTypes.Madmate) && Madmate.hasImpostorVision) // Impostor, Jackal/Sidekick, Spy, or Madmate with Impostor vision
                )
                __result = __instance.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;

            else if (pc.hasModifier(RoleModifierTypes.LighterLight)) // if player is Lighter and Lighter has his ability active
                __result = Mathf.Lerp(__instance.MaxLightRadius * Lighter.lightsOffVision, __instance.MaxLightRadius * Lighter.lightsOnVision, num);

            else if (RoleHelpers.roleExists(CustomRoleTypes.Trickster) && Trickster.lightsOutTimer > 0f)
            {
                float lerpValue = 1f;
                if (Trickster.lightsOutDuration - Trickster.lightsOutTimer < 0.5f) lerpValue = Mathf.Clamp01((Trickster.lightsOutDuration - Trickster.lightsOutTimer) * 2);
                else if (Trickster.lightsOutTimer < 0.5) lerpValue = Mathf.Clamp01(Trickster.lightsOutTimer * 2);
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue) * PlayerControl.GameOptions.CrewLightMod; // Instant lights out? Maybe add a smooth transition?
            }
            else
            {
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) * PlayerControl.GameOptions.CrewLightMod;
            }

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.IsGameOverDueToDeath))]
        public static void Postfix2(ShipStatus __instance, ref bool __result)
        {
            __result = false;
        }

        private static int originalNumCommonTasksOption = 0;
        private static int originalNumShortTasksOption = 0;
        private static int originalNumLongTasksOption = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool Prefix(ShipStatus __instance)
        {
            if (CustomOptionHolder.uselessOptions.getBool() && CustomOptionHolder.playerColorRandom.getBool() && AmongUsClient.Instance.AmHost)
            {
                List<int> colors = Enumerable.Range(0, Palette.PlayerColors.Count).ToList();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    int i = TheOtherRoles.rnd.Next(0, colors.Count);
                    p.SetColor(colors[i]);
                    p.RpcSetColor((byte)colors[i]);
                    colors.RemoveAt(i);
                }
            }

            var commonTaskCount = __instance.CommonTasks.Count;
            var normalTaskCount = __instance.NormalTasks.Count;
            var longTaskCount = __instance.LongTasks.Count;
            originalNumCommonTasksOption = PlayerControl.GameOptions.NumCommonTasks;
            originalNumShortTasksOption = PlayerControl.GameOptions.NumShortTasks;
            originalNumLongTasksOption = PlayerControl.GameOptions.NumLongTasks;
            if(PlayerControl.GameOptions.NumCommonTasks > commonTaskCount) PlayerControl.GameOptions.NumCommonTasks = commonTaskCount;
            if(PlayerControl.GameOptions.NumShortTasks > normalTaskCount) PlayerControl.GameOptions.NumShortTasks = normalTaskCount;
            if(PlayerControl.GameOptions.NumLongTasks > longTaskCount) PlayerControl.GameOptions.NumLongTasks = longTaskCount;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static void Postfix3(ShipStatus __instance)
        {
            // Restore original settings after the tasks have been selected
            PlayerControl.GameOptions.NumCommonTasks = originalNumCommonTasksOption;
            PlayerControl.GameOptions.NumShortTasks = originalNumShortTasksOption;
            PlayerControl.GameOptions.NumLongTasks = originalNumLongTasksOption;
        }
            
    }

}
