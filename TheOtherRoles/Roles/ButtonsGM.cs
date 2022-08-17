using HarmonyLib;
using Hazel;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Patches;
using System.Reflection;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public static class ButtonsGM
    {
        private static List<CustomButton> gmButtons;
        private static List<CustomButton> gmKillButtons;
        private static CustomButton gmZoomIn;
        private static CustomButton gmZoomOut;

        public static void setCustomButtonCooldowns()
        {
            Ninja.SetButtonCooldowns();
            Sheriff.SetButtonCooldowns();
            PlagueDoctor.SetButtonCooldowns();
            Lighter.SetButtonCooldowns();
            SerialKiller.SetButtonCooldowns();
            Immoralist.SetButtonCooldowns();
            Sprinter.SetButtonCooldowns();
            Akujo.SetButtonCooldowns();

            foreach (CustomButton gmButton in gmButtons)
            {
                gmButton.MaxTimer = 0.0f;
            }
            foreach (CustomButton gmButton in gmKillButtons)
            {
                gmButton.MaxTimer = 0.0f;
            }

            gmZoomIn.MaxTimer = 0.0f;
            gmZoomOut.MaxTimer = 0.0f;
        }

        public static void makeButtons(HudManager hm)
        {
            Ninja.MakeButtons(hm);
            Sheriff.MakeButtons(hm);
            PlagueDoctor.MakeButtons(hm);
            Lighter.MakeButtons(hm);
            SerialKiller.MakeButtons(hm);
            Fox.MakeButtons(hm);
            Immoralist.MakeButtons(hm);
            FortuneTeller.MakeButtons(hm);
            Sprinter.MakeButtons(hm);
            Akujo.MakeButtons(hm);

            gmButtons = new List<CustomButton>();
            gmKillButtons = new List<CustomButton>();

            Vector3 gmCalcPos(byte index)
            {
                return new Vector3(-0.25f, -0.25f, 1.0f) + Vector3.right * index * 0.55f;
            }

            Action gmButtonOnClick(byte index)
            {
                return () =>
                {
                    PlayerControl target = Helpers.playerById(index);
                    if (!MapOptions.playerIcons.ContainsKey(index) || target.Data.Disconnected)
                    {
                        return;
                    }

                    if (GM.gm.transform.position != target.transform.position)
                    {
                        GM.gm.transform.position = target.transform.position;
                    }
                };
            };

            Action gmKillButtonOnClick(byte index)
            {
                return () =>
                {
                    PlayerControl target = Helpers.playerById(index);
                    if (!MapOptions.playerIcons.ContainsKey(index) || target.Data.Disconnected)
                    {
                        return;
                    }

                    if (!target.Data.IsDead)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GMKill, Hazel.SendOption.Reliable, -1);
                        writer.Write(index);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.GMKill(index);
                    }
                    else
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GMRevive, Hazel.SendOption.Reliable, -1);
                        writer.Write(index);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.GMRevive(index);
                    }
                };
            };

            Func<bool> gmHasButton(byte index)
            {
                return () =>
                {
                    if ((GM.gm == null || PlayerControl.LocalPlayer != GM.gm) ||
                        (!MapOptions.playerIcons.ContainsKey(index)) ||
                        (!GM.canWarp) ||
                        (Helpers.playerById(index).Data.Disconnected))
                    {
                        return false;
                    }

                    return true;
                };
            }

            Func<bool> gmHasKillButton(byte index)
            {
                return () =>
                {
                    if ((GM.gm == null || PlayerControl.LocalPlayer != GM.gm) ||
                        (!MapOptions.playerIcons.ContainsKey(index)) ||
                        (!GM.canKill) ||
                        (Helpers.playerById(index).Data.Disconnected))
                    {
                        return false;
                    }

                    return true;
                };
            }

            Func<bool> gmCouldUse(byte index)
            {
                return () =>
                {
                    if (!MapOptions.playerIcons.ContainsKey(index) || !GM.canWarp)
                    {
                        return false;
                    }

                    Vector3 pos = gmCalcPos(index);
                    Vector3 scale = new Vector3(0.4f, 0.8f, 1.0f);

                    Vector3 iconBase = hm.UseButton.transform.localPosition;
                    iconBase.x *= -1;
                    if (gmButtons[index].PositionOffset != pos)
                    {
                        gmButtons[index].PositionOffset = pos;
                        gmButtons[index].LocalScale = scale;
                        MapOptions.playerIcons[index].transform.localPosition = iconBase + pos;
                        //TheOtherRolesPlugin.Instance.Log.LogInfo($"Updated {index}: {pos.x}, {pos.y}, {pos.z}");
                    }

                    //MapOptions.playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
                    return PlayerControl.LocalPlayer.CanMove;
                };
            }

            Func<bool> gmCouldKill(byte index)
            {
                return () =>
                {
                    if (!MapOptions.playerIcons.ContainsKey(index) || !GM.canKill)
                    {
                        return false;
                    }

                    Vector3 pos = gmCalcPos(index) + Vector3.up * 0.55f;
                    Vector3 scale = new Vector3(0.4f, 0.25f, 1.0f);
                    if (gmKillButtons[index].PositionOffset != pos)
                    {
                        gmKillButtons[index].PositionOffset = pos;
                        gmKillButtons[index].LocalScale = scale;
                    }

                    PlayerControl target = Helpers.playerById(index);
                    if (target.Data.IsDead)
                    {
                        gmKillButtons[index].buttonText = ModTranslation.getString("gmRevive");
                    }
                    else
                    {
                        gmKillButtons[index].buttonText = ModTranslation.getString("gmKill");
                    }

                    //MapOptions.playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
                    return true;
                };
            }

            for (byte i = 0; i < 15; i++)
            {
                //TheOtherRolesPlugin.Instance.Log.LogInfo($"Added {i}");

                CustomButton gmButton = new CustomButton(
                    // Action OnClick
                    gmButtonOnClick(i),
                    // bool HasButton
                    gmHasButton(i),
                    // bool CouldUse
                    gmCouldUse(i),
                    // Action OnMeetingEnds
                    () => { },
                    // sprite
                    null,
                    // position
                    Vector3.zero,
                    // hudmanager
                    hm,
                    hm.UseButton,
                    // keyboard shortcut
                    null,
                    true
                );
                gmButton.Timer = 0.0f;
                gmButton.MaxTimer = 0.0f;
                gmButton.showButtonText = false;
                gmButtons.Add(gmButton);

                CustomButton gmKillButton = new CustomButton(
                    // Action OnClick
                    gmKillButtonOnClick(i),
                    // bool HasButton
                    gmHasKillButton(i),
                    // bool CouldUse
                    gmCouldKill(i),
                    // Action OnMeetingEnds
                    () => { },
                    // sprite
                    null,
                    // position
                    Vector3.zero,
                    // hudmanager
                    hm,
                    hm.KillButton,
                    // keyboard shortcut
                    null,
                    true
                );
                gmKillButton.Timer = 0.0f;
                gmKillButton.MaxTimer = 0.0f;
                gmKillButton.showButtonText = true;

                var buttonPos = gmKillButton.actionButton.buttonLabelText.transform.localPosition;
                gmKillButton.actionButton.buttonLabelText.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y + 0.6f, -500f);
                gmKillButton.actionButton.buttonLabelText.transform.localScale = new Vector3(1.5f, 1.8f, 1.0f);

                gmKillButtons.Add(gmKillButton);
            }

            gmZoomOut = new CustomButton(
                () => {

                    if (Camera.main.orthographicSize < 18.0f)
                    {
                        Camera.main.orthographicSize *= 1.5f;
                        hm.UICamera.orthographicSize *= 1.5f;
                    }

                    if (hm.transform.localScale.x < 6.0f)
                    {
                        hm.transform.localScale *= 1.5f;
                    }

                    /*TheOtherRolesPlugin.Instance.Log.LogInfo($"Camera zoom {Camera.main.orthographicSize} / {TaskPanelBehaviour.Instance.transform.localPosition.x}");*/
                },
                () => { return !(GM.gm == null || PlayerControl.LocalPlayer != GM.gm); },
                () => { return true; },
                () => { },
                GM.getZoomOutSprite(),
                // position
                Vector3.zero + Vector3.up * 3.75f + Vector3.right * 0.55f,
                // hudmanager
                hm,
                hm.UseButton,
                // keyboard shortcut
                KeyCode.PageDown,
                false
            );
            gmZoomOut.Timer = 0.0f;
            gmZoomOut.MaxTimer = 0.0f;
            gmZoomOut.showButtonText = false;
            gmZoomOut.LocalScale = Vector3.one * 0.275f;

            gmZoomIn = new CustomButton(
                () => {

                    if (Camera.main.orthographicSize > 3.0f)
                    {
                        Camera.main.orthographicSize /= 1.5f;
                        hm.UICamera.orthographicSize /= 1.5f;
                    }

                    if (hm.transform.localScale.x > 1.0f)
                    {
                        hm.transform.localScale /= 1.5f;
                    }

                    /*TheOtherRolesPlugin.Instance.Log.LogInfo($"Camera zoom {Camera.main.orthographicSize} / {TaskPanelBehaviour.Instance.transform.localPosition.x}");*/
                },
                () => { return !(GM.gm == null || PlayerControl.LocalPlayer != GM.gm); },
                () => { return true; },
                () => { },
                GM.getZoomInSprite(),
                // position
                Vector3.zero + Vector3.up * 3.75f + Vector3.right * 0.2f,
                // hudmanager
                hm,
                hm.UseButton,
                // keyboard shortcut
                KeyCode.PageUp,
                false
            );
            gmZoomIn.Timer = 0.0f;
            gmZoomIn.MaxTimer = 0.0f;
            gmZoomIn.showButtonText = false;
            gmZoomIn.LocalScale = Vector3.one * 0.275f;
        }
    }
}