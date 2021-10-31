using HarmonyLib;
using System;
using Hazel;
using UnityEngine;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.MapOptions;
using System.Collections.Generic;


namespace TheOtherRoles.Patches
{
    public static class UsablesPatch
    {
        public static bool IsBlocked(PlayerTask task, PlayerControl pc)
        {
            if (task == null || pc == null || pc != PlayerControl.LocalPlayer) return false;

            bool isLights = task.TaskType == TaskTypes.FixLights;
            bool isComms = task.TaskType == TaskTypes.FixComms;
            bool isReactor = task.TaskType == TaskTypes.StopCharles || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.ResetReactor;
            bool isO2 = task.TaskType == TaskTypes.RestoreOxy;

            if (Swapper.swapper != null && pc == Swapper.swapper && (isLights || isComms))
            {
                return true;
            }

            if (Madmate.madmate != null && pc == Madmate.madmate && (isLights || (isComms && !Madmate.canFixComm)))
            {
                return true;
            }

            if (pc.isGM() && (isLights || isComms || isReactor || isO2))
            {
                return true;
            }

            return false;
        }

        public static bool IsBlocked(Console console, PlayerControl pc)
        {
            if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
            {
                return false;
            }

            PlayerTask task = console.FindTask(pc);
            return IsBlocked(task, pc);
        }

        public static bool IsBlocked(SystemConsole console, PlayerControl pc)
        {
            if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
            {
                return false;
            }

            string name = console.name;
            bool isSecurity = name == "task_cams" || name == "Surv_Panel" || name == "SurvLogConsole" || name == "SurvConsole";
            bool isVitals = name == "panel_vitals";
            bool isButton = name == "EmergencyButton" || name == "EmergencyConsole" || name == "task_emergency";

            if ((isSecurity && !MapOptions.canUseCameras) || (isVitals && !MapOptions.canUseVitals)) return true;
            return false;
        }

        public static bool IsBlocked(IUsable target, PlayerControl pc)
        {
            if (target == null) return false;

            Console targetConsole = target.TryCast<Console>();
            SystemConsole targetSysConsole = target.TryCast<SystemConsole>();
            MapConsole targetMapConsole = target.TryCast<MapConsole>();
            if ((targetConsole != null && IsBlocked(targetConsole, pc)) ||
                (targetSysConsole != null && IsBlocked(targetSysConsole, pc)) ||
                (targetMapConsole != null && !MapOptions.canUseAdmin))
            {
                return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(Vent), "CanUse")]
        public static class VentCanUsePatch
        {
            public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
            {
                float num = float.MaxValue;
                PlayerControl @object = pc.Object;

                if (MapOptions.disableVents)
                {
                    canUse = couldUse = false;
                    __result = num;
                    return false;
                }

                bool roleCouldUse = false;
                if (Engineer.engineer != null && Engineer.engineer == @object)
                    roleCouldUse = true;
                else if (GM.gm != null && GM.gm == @object)
                    roleCouldUse = true;
                else if (Jackal.canUseVents && Jackal.jackal != null && Jackal.jackal == @object)
                    roleCouldUse = true;
                else if (Sidekick.canUseVents && Sidekick.sidekick != null && Sidekick.sidekick == @object)
                    roleCouldUse = true;
                else if (Spy.canEnterVents && Spy.spy != null && Spy.spy == @object)
                    roleCouldUse = true;
                else if (Madmate.canEnterVents && Madmate.madmate != null && Madmate.madmate == @object)
                    roleCouldUse = true;
                else if (pc.IsImpostor)
                {
                    if (Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer)
                        roleCouldUse = false;
                    else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                        roleCouldUse = false;
                    else
                        roleCouldUse = true;
                }

                var usableDistance = __instance.UsableDistance;
                if (__instance.name.StartsWith("JackInTheBoxVent_"))
                {
                    if (Trickster.trickster != PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.isGM())
                    {
                        // Only the Trickster can use the Jack-In-The-Boxes!
                        canUse = false;
                        couldUse = false;
                        __result = num;
                        return false;
                    }
                    else
                    {
                        // Reduce the usable distance to reduce the risk of gettings stuck while trying to jump into the box if it's placed near objects
                        usableDistance = 0.4f;
                    }
                }
                else if (__instance.name.StartsWith("SealedVent_"))
                {
                    canUse = couldUse = false;
                    __result = num;
                    return false;
                }

                couldUse = (@object.inVent || roleCouldUse) && !pc.IsDead && (@object.CanMove || @object.inVent);
                canUse = couldUse;
                if (canUse)
                {
                    Vector2 truePosition = @object.GetTruePosition();
                    Vector3 position = __instance.transform.position;
                    num = Vector2.Distance(truePosition, position);

                    canUse &= (num <= usableDistance && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipOnlyMask, false));
                }
                __result = num;
                return false;
            }
        }

        [HarmonyPatch(typeof(Vent), "Use")]
        public static class VentUsePatch
        {
            public static bool Prefix(Vent __instance)
            {
                bool canUse;
                bool couldUse;
                __instance.CanUse(PlayerControl.LocalPlayer.Data, out canUse, out couldUse);
                bool canMoveInVents = true;
                if (!canUse) return false; // No need to execute the native method as using is disallowed anyways
                if (Spy.spy == PlayerControl.LocalPlayer || Madmate.madmate == PlayerControl.LocalPlayer)
                {
                    canMoveInVents = false;
                }
                bool isEnter = !PlayerControl.LocalPlayer.inVent;

                if (__instance.name.StartsWith("JackInTheBoxVent_"))
                {
                    __instance.SetButtons(isEnter && canMoveInVents);
                    MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UseUncheckedVent, Hazel.SendOption.Reliable);
                    writer.WritePacked(__instance.Id);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(isEnter ? byte.MaxValue : (byte)0);
                    writer.EndMessage();
                    RPCProcedure.useUncheckedVent(__instance.Id, PlayerControl.LocalPlayer.PlayerId, isEnter ? byte.MaxValue : (byte)0);
                    return false;
                }

                if (isEnter)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(__instance.Id);
                }
                else
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(__instance.Id);
                }
                __instance.SetButtons(isEnter && canMoveInVents);
                return false;
            }
        }

        [HarmonyPatch(typeof(UseButtonManager), nameof(UseButtonManager.SetTarget))]
        class UseButtonSetTargetPatch
        {
            static bool Prefix(UseButtonManager __instance, [HarmonyArgument(0)] IUsable target)
            {
                __instance.currentTarget = target;

                bool enabled = true;
                UseButton newButton = null;
                PlayerControl pc = PlayerControl.LocalPlayer;

                // Trickster render special vent button
                if (__instance.currentTarget != null && Trickster.trickster != null && Trickster.trickster == pc)
                {
                    Vent possibleVent = __instance.currentTarget.TryCast<Vent>();
                    if (possibleVent != null && possibleVent.gameObject != null)
                    {
                        var useButton = __instance.currentButtonShown;
                        if (possibleVent.gameObject.name.StartsWith("JackInTheBoxVent_"))
                        {
                            newButton = Trickster.getTricksterVentButton();
                        }
                    }
                }

                // Jester sabotage
                if (Jester.canSabotage && Jester.jester != null && Jester.jester == pc && pc.CanMove)
                {
                    if (!Jester.jester.Data.IsDead && __instance.currentTarget == null)
                    { // no target, so sabotage
                        newButton = __instance.otherButtons[ImageNames.SabotageButton];
                    }
                }

                // Madmate sabotage
                if (Madmate.canSabotage && Madmate.madmate != null && Madmate.madmate == pc && pc.CanMove)
                {
                    if (!Madmate.madmate.Data.IsDead && __instance.currentTarget == null)
                    { // no target, so sabotage
                        newButton = __instance.otherButtons[ImageNames.SabotageButton];
                    }
                }

                // Mafia sabotage button render patch
                bool blockSabotageJanitor = (Janitor.janitor != null && Janitor.janitor == pc);
                bool blockSabotageMafioso = (Mafioso.mafioso != null && Mafioso.mafioso == pc && Godfather.godfather != null && !Godfather.godfather.Data.IsDead);
                if (__instance.currentTarget == null && (blockSabotageJanitor || blockSabotageMafioso))
                {
                    /*                var useButton = __instance.currentButtonShown;
                                    useButton.graphic.sprite = DestroyableSingleton<TranslationController>.Instance.GetImage(ImageNames.UseButton);
                                    useButton.graphic.color = UseButtonManager.DisabledColor;
                                    useButton.text.enabled = false;*/
                    newButton = __instance.otherButtons[ImageNames.UseButton];
                    enabled = false;
                }

                
                if (IsBlocked(target, pc))
                {
                    enabled = false;
                    newButton = GM.getBlockedButton();
                }

                if (newButton != null)
                {
                    newButton.graphic.color = enabled ? UseButtonManager.EnabledColor : UseButtonManager.DisabledColor;
                    //CooldownHelpers.SetCooldownNormalizedUvs(newButton.graphic);
                    //newButton.graphic.material.SetFloat("_Percent", target.PercentCool);

                    __instance.currentButtonShown.Hide();
                    __instance.currentButtonShown = newButton;
                    __instance.currentButtonShown.Show();

                    return false;
                }

                return true;

            }
        }

        [HarmonyPatch(typeof(UseButtonManager), nameof(UseButtonManager.DoClick))]
        class UseButtonDoClickPatch
        {
            static bool Prefix(UseButtonManager __instance)
            {
                if (__instance.currentTarget != null) return true;

                if (IsBlocked(__instance.currentTarget, PlayerControl.LocalPlayer)) return false;

                // Jester sabotage
                if (Jester.canSabotage && Jester.jester != null && Jester.jester == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Action<MapBehaviour> action = m => m.ShowInfectedMap();
                    DestroyableSingleton<HudManager>.Instance.ShowMap(action);
                    return false;
                }
                // Madmate sabotage
                if (Madmate.canSabotage && Madmate.madmate != null && Madmate.madmate == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Action<MapBehaviour> action = m => m.ShowInfectedMap();
                    DestroyableSingleton<HudManager>.Instance.ShowMap(action);
                    return false;
                }
                // Mafia sabotage button click patch
                bool blockSabotageJanitor = (Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer);
                bool blockSabotageMafioso = (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.godfather != null && !Godfather.godfather.Data.IsDead);
                if (blockSabotageJanitor || blockSabotageMafioso) return false;

                return true;
            }
        }

        [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
        class EmergencyMinigameUpdatePatch
        {
            static void Postfix(EmergencyMinigame __instance)
            {
                var roleCanCallEmergency = true;
                var statusText = "";

                // Deactivate emergency button for GM
                if (PlayerControl.LocalPlayer.isGM())
                {
                    roleCanCallEmergency = false;
                    statusText = ModTranslation.getString("gmMeetingButton");
                }

                // Deactivate emergency button for Swapper
                if (Swapper.swapper != null && Swapper.swapper == PlayerControl.LocalPlayer && !Swapper.canCallEmergency)
                {
                    roleCanCallEmergency = false;
                    statusText = ModTranslation.getString("swapperMeetingButton");
                }
                // Potentially deactivate emergency button for Jester
                if (Jester.jester != null && Jester.jester == PlayerControl.LocalPlayer && !Jester.canCallEmergency)
                {
                    roleCanCallEmergency = false;
                    statusText = ModTranslation.getString("jesterMeetingButton");
                }

                if (!roleCanCallEmergency)
                {
                    __instance.StatusText.text = statusText;
                    __instance.NumberText.text = string.Empty;
                    __instance.ClosedLid.gameObject.SetActive(true);
                    __instance.OpenLid.gameObject.SetActive(false);
                    __instance.ButtonActive = false;
                    return;
                }
                // Handle max number of meetings
                if (__instance.state == 1)
                {
                    int localRemaining = PlayerControl.LocalPlayer.RemainingEmergencies;
                    int teamRemaining = Mathf.Max(0, maxNumberOfMeetings - meetingsCount);
                    int remaining = Mathf.Min(localRemaining, (Mayor.mayor != null && Mayor.mayor == PlayerControl.LocalPlayer) ? 1 : teamRemaining);

                    __instance.StatusText.text = "<size=100%>" + String.Format(ModTranslation.getString("meetingStatus"), PlayerControl.LocalPlayer.name) + "</size>";
                    __instance.NumberText.text = String.Format(ModTranslation.getString("meetingCount"), localRemaining.ToString(), teamRemaining.ToString());
                    __instance.ButtonActive = remaining > 0;
                    __instance.ClosedLid.gameObject.SetActive(!__instance.ButtonActive);
                    __instance.OpenLid.gameObject.SetActive(__instance.ButtonActive);
                    return;
                }
            }
        }


        public static class ConsolePatch
        {
            [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
            public static class ConsoleCanUsePatch
            {

                public static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
                {
                    canUse = couldUse = false;
                    __result = float.MaxValue;

                    //if (IsBlocked(__instance, pc.Object)) return false;
                    if (__instance.AllowImpostor) return true;
                    if (!pc.Object.hasFakeTasks()) return true;

                    return false;
                }
            }

            [HarmonyPatch(typeof(Console), nameof(Console.Use))]
            public static class ConsoleUsePatch
            {
                public static bool Prefix(Console __instance)
                {
                    return !IsBlocked(__instance, PlayerControl.LocalPlayer);
                }
            }
        }

        public class SystemConsolePatch
        {
            [HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.CanUse))]
            public static class SystemConsoleCanUsePatch
            {
                public static bool Prefix(ref float __result, SystemConsole __instance, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
                {
                    canUse = couldUse = false;
                    __result = float.MaxValue;
                    //if (IsBlocked(__instance, pc.Object)) return false;

                    return true;
                }
            }

            [HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.Use))]
            public static class SystemConsoleUsePatch
            {
                public static bool Prefix(SystemConsole __instance)
                {
                    return !IsBlocked(__instance, PlayerControl.LocalPlayer);
                }
            }
        }


    }
}
