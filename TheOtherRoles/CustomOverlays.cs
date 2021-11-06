using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles {
    [Harmony]
    public class CustomOverlays {

        public static Sprite helpButton;
        private static Sprite colorBG;
        private static SpriteRenderer meetingUnderlay;
        private static SpriteRenderer infoUnderlay;
        private static TMPro.TextMeshPro infoOverlayRules;
        private static TMPro.TextMeshPro infoOverlayRoles;
        public static bool overlayShown = false;

        public static void resetOverlays()
        {
            hideBlackBG();
            hideInfoOverlay();
            UnityEngine.Object.Destroy(meetingUnderlay);
            UnityEngine.Object.Destroy(infoUnderlay);
            UnityEngine.Object.Destroy(infoOverlayRules);
            UnityEngine.Object.Destroy(infoOverlayRoles);
            meetingUnderlay = infoUnderlay = null;
            infoOverlayRules = infoOverlayRoles = null;
            overlayShown = false;
        }

        public static void initializeOverlays()
        {
            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager == null) return;

            if (helpButton == null)
            {
                helpButton = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.HelpButton.png", 115f);
            }

            if (colorBG == null)
            {
                colorBG = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.White.png", 100f);
            }

            if (meetingUnderlay == null)
            {
                meetingUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
                meetingUnderlay.transform.localPosition = new Vector3(0f, 0f, 20f);
                meetingUnderlay.gameObject.SetActive(true);
                meetingUnderlay.enabled = false;
            }

            if (infoUnderlay == null)
            {
                infoUnderlay = UnityEngine.Object.Instantiate(meetingUnderlay, hudManager.transform);
                infoUnderlay.transform.localPosition = new Vector3(0f, 0f, -900f);
                infoUnderlay.gameObject.SetActive(true);
                infoUnderlay.enabled = false;
            }

            if (infoOverlayRules == null)
            {
                infoOverlayRules = UnityEngine.Object.Instantiate(hudManager.TaskText, hudManager.transform);
                infoOverlayRules.fontSize = infoOverlayRules.fontSizeMin = infoOverlayRules.fontSizeMax = 1.15f;
                infoOverlayRules.autoSizeTextContainer = false;
                infoOverlayRules.enableWordWrapping = false;
                infoOverlayRules.alignment = TMPro.TextAlignmentOptions.TopLeft;
                infoOverlayRules.transform.position = Vector3.zero;
                infoOverlayRules.transform.localPosition = new Vector3(-2.5f, 1.15f, -910f);
                infoOverlayRules.transform.localScale = Vector3.one;
                infoOverlayRules.color = Palette.White;
                infoOverlayRules.enabled = false;
            }

            if (infoOverlayRoles == null) { 
                infoOverlayRoles = UnityEngine.Object.Instantiate(infoOverlayRules, hudManager.transform);
                infoOverlayRoles.maxVisibleLines = 28;
                infoOverlayRoles.fontSize = infoOverlayRoles.fontSizeMin = infoOverlayRoles.fontSizeMax = 1.15f;
                infoOverlayRoles.outlineWidth += 0.02f;
                infoOverlayRoles.autoSizeTextContainer = false;
                infoOverlayRoles.enableWordWrapping = false;
                infoOverlayRoles.alignment = TMPro.TextAlignmentOptions.TopLeft;
                infoOverlayRoles.transform.position = Vector3.zero;
                infoOverlayRoles.transform.localPosition = infoOverlayRules.transform.localPosition + new Vector3(2.5f, 0.0f, 0.0f);
                infoOverlayRoles.transform.localScale = Vector3.one;
                infoOverlayRoles.color = Palette.White;
                infoOverlayRoles.enabled = false;
            }
        }

        public static void showBlackBG()
        {
            initializeOverlays();
            meetingUnderlay.sprite = colorBG;
            meetingUnderlay.color = Palette.Black;
            meetingUnderlay.transform.localScale = new Vector3(20f, 20f, 1f);
            if (HudManager.Instance == null) return;

            HudManager.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>(t =>
            {
                meetingUnderlay.enabled = t > 0.2f && t < 1f;
            })));
        }

        public static void hideBlackBG()
        {
            if (meetingUnderlay == null) return;
            meetingUnderlay.enabled = false;
        }

        public static void showInfoOverlay()
        {
            if (overlayShown || MapOptions.hideSettings) return;

            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (ShipStatus.Instance == null || PlayerControl.LocalPlayer == null || hudManager == null || HudManager.Instance.isIntroDisplayed || (!PlayerControl.LocalPlayer.CanMove && MeetingHud.Instance == null))
                return;

            if (MapBehaviour.Instance != null)
                MapBehaviour.Instance.Close();
            hudManager.SetHudActive(false);
            
            overlayShown = true;
            initializeOverlays();

            Transform parent;
            if (MeetingHud.Instance != null)
                parent = MeetingHud.Instance.transform;
            else
                parent = hudManager.transform;

            infoUnderlay.transform.parent = parent;
            infoOverlayRules.transform.parent = parent;
            infoOverlayRoles.transform.parent = parent;

            infoUnderlay.sprite = colorBG;
            infoUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
            infoUnderlay.transform.localScale = new Vector3(7.5f, 5f, 1f);
            infoUnderlay.enabled = true;

            TheOtherRolesPlugin.optionsPage = 0;
            GameOptionsData o = PlayerControl.GameOptions;
            List<string> gameOptions = o.ToString().Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList().GetRange(2, 17);
            infoOverlayRules.text = string.Join("\n", gameOptions) + "\n\n" + GameOptionsDataPatch.optionsToString(CustomOptionHolder.specialOptions);
            infoOverlayRules.enabled = true;

            string rolesText = "";
            foreach (RoleInfo r in RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer))
            {
                string roleOptions = r.roleOptions;
                string roleDesc = r.fullDescription;
                rolesText += $"<size=150%>{r.nameColored}</size>" +
                    (roleDesc != "" ? $"\n{r.fullDescription}" : "") + "\n\n" +
                    (roleOptions != "" ? $"{roleOptions}\n\n" : "");
            }

            infoOverlayRoles.text = rolesText;
            infoOverlayRoles.enabled = true;
        }

        public static void hideInfoOverlay()
        {
            if (!overlayShown) return;

            overlayShown = false;
            if (MeetingHud.Instance == null) DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
            if (infoUnderlay != null) infoUnderlay.enabled = false;
            if (infoOverlayRules != null) infoOverlayRules.enabled = false;
            if (infoOverlayRoles != null) infoOverlayRoles.enabled = false;
        }

        public static void toggleInfoOverlay()
        {
            if (overlayShown)
                hideInfoOverlay();
            else
                showInfoOverlay();
        }

        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class CustomOverlayKeybinds
        {
            public static void Postfix(KeyboardJoystick __instance)
            {
                if (Input.GetKeyDown(KeyCode.H) && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    toggleInfoOverlay();
                }
            }
        }
    }
}