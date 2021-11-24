using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using TheOtherRoles;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public static class CustomRoleSettings
    {
        public static List<CustomRoleTypes> UnselectableRoles = new List<CustomRoleTypes> {
            CustomRoleTypes.Crewmate,
            CustomRoleTypes.Impostor,
            CustomRoleTypes.Sidekick,
            CustomRoleTypes.EvilGuesser,
            CustomRoleTypes.NiceGuesser,
            CustomRoleTypes.EvilMini,
            CustomRoleTypes.NiceMini,
            CustomRoleTypes.Godfather,
            CustomRoleTypes.Mafioso,
            CustomRoleTypes.Janitor,
            CustomRoleTypes.NoRole
        };

/*        public static List<CustomRoleTypes> SelectableRoles =
            Enum.GetValues(typeof(CustomRoleTypes))
            .Cast<CustomRoleTypes>()
            .Where(x => !UnselectableRoles.Contains(x))
            .ToList();*/

        public static List<CustomRoleTypes> SelectableRoles = new List<CustomRoleTypes>
                {
                    CustomRoleTypes.Mayor,
                    CustomRoleTypes.Sheriff,
                    CustomRoleTypes.Lighter,
                    CustomRoleTypes.Bait,
                    CustomRoleTypes.Madmate,
                    CustomRoleTypes.Jester,
                    CustomRoleTypes.Arsonist,
                    CustomRoleTypes.Opportunist,
                    CustomRoleTypes.Vulture,
                    CustomRoleTypes.Vampire,
                };

        static RoleOptionSetting roleTemplate;
        static StringOption optionTemplate;
        static AdvancedRoleSettingsButton menuTemplate;
        static TMPro.TextMeshPro nameTemplate;
        static TMPro.TextMeshPro bodyTemplate;
        static TMPro.TextMeshPro advancedMenuText;
        static TMPro.TextMeshPro advancedMenuBody;
        static List<CustomOption> currentOptions = new List<CustomOption>();
        static bool advancedMenuShown = false;

        static float optionOffset = 0f;

        public static CustomOptionBlank scientistOptions;
        public static CustomOption scientistUseVitalsTime;

        public static CustomOptionBlank engineerOptions;

        public static CustomOptionBlank gaOptions;
        public static CustomOption gaHideMark;

        public static CustomOptionBlank shapeshifterOptions;
        public static CustomOption shapeshifterShiftAnyone;

        public static void Load()
        {
            scientistOptions = new CustomOptionBlank(null);
            scientistUseVitalsTime = CustomOption.Create(10001, "scientistUseVitalsTime", false, scientistOptions);

            engineerOptions = new CustomOptionBlank(null);
            //engineerSomething = CustomOption.Create(10011, "engineerSomething", false, engineerOptions);

            gaOptions = new CustomOptionBlank(null);
            gaHideMark = CustomOption.Create(10021, "gaHideMark", false, gaOptions);

            shapeshifterOptions = new CustomOptionBlank(null);
            shapeshifterShiftAnyone = CustomOption.Create(10031, "shapeshifterShiftAnyone", false, shapeshifterOptions);
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        class GameSettingMenuPatch
        {
            public static void Postfix(GameSettingMenu __instance)
            {
                roleTemplate = __instance.RolesSettings.AllRoleSettings[0];
                nameTemplate = __instance.RoleName;
                bodyTemplate = __instance.RoleBlurb;
                optionTemplate = UnityEngine.Object.FindObjectsOfType<StringOption>().FirstOrDefault();

                foreach (CustomRoleTypes role in SelectableRoles)
                {
                    if (UnselectableRoles.Contains(role)) continue;

                    var customRole = CustomRoleManager.CreateRole(role);
                    var roleInfo = RoleInfo.getRoleInfo(customRole);
                    var roleSettings = UnityEngine.Object.Instantiate(roleTemplate, roleTemplate.transform.parent);
                    roleSettings.Role = customRole;
                    // Helpers.log($"{(CustomRoleTypes)roleSettings.Role.Role} {roleSettings.Role.NiceName} {roleSettings.Role.Blurb}");

                    __instance.RolesSettings.AllRoleSettings.Add(roleSettings);

                    menuTemplate = __instance.RolesSettings.AllAdvancedSettingTabs[0];
                    var newMenu = new AdvancedRoleSettingsButton();
                    newMenu.Type = roleSettings.Role.Role;
                    newMenu.Tab = UnityEngine.Object.Instantiate(menuTemplate.Tab, menuTemplate.Tab.transform.parent);
                    __instance.RolesSettings.AllAdvancedSettingTabs.Add(newMenu);

                    foreach (var c in newMenu.Tab.GetComponentsInChildren<Component>()) {
                        //Helpers.log($"Component: {c.name}");
                        UnityEngine.Object.Destroy(c);
                    }

                }

                float yoff = __instance.RolesSettings.YOffset;
                var s = __instance.RolesSettings.GetComponentInChildren<Scroller>();
                s.ScrollerY = __instance.RolesSettings.GetComponentInChildren<Scrollbar>();
                s.ScrollerYRange = new FloatRange(-1.5f, 1.5f);
                s.YBounds = new FloatRange(0.0f, -4.0f + yoff * __instance.RolesSettings.AllRoleSettings.Count);

                // check -> ToggleOption
                // numbers -> NumberOption
                // other -> KeyValueOption
                /*                if (tp != null && tp.name == "Role Name")
                                    foreach (UnityEngine.Object sub in tp.GetComponentsInChildren<GameObject>()) Helpers.log($"{sub.name}");*/

                //__instance.GetComponentInChildren<Scroller>().YBounds.max = yoff * __instance.AllRoleSettings.Count + yoff + yoff;
                //Helpers.log($"{__instance.Scroller.YBounds.max}");
                /*            var sc = __instance.GetComponentInChildren<Scroller>();
                            sc.enabled = true;
                            Helpers.log($"{sc.YBounds.max} {sc.allowX} {sc.allowY} {sc.active} {sc.AtTop} {sc.AtBottom} {sc.HandleDrag}");*/
            }
        }

        [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.ShowRoleDetails))]
        class RoleOptionSettingShowRoleDetailsPatch
        {
            public static bool Prefix(RoleOptionSetting __instance)
            {
                if (__instance.Role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    GameSettingMenu parent = __instance.GetComponentInParent<GameSettingMenu>();
                    parent.RoleName.text = __instance.Role.NiceName;
                    parent.RoleBlurb.text = __instance.Role.Blurb;
                    parent.RoleIcon.sprite = DestroyableSingleton<ModManager>.Instance.ModStamp.sprite;

                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ShowAdvancedSettings))]
        class RolesSettingsMenuShowAdvancedSettingsPatch
        {
            public static void Postfix(RolesSettingsMenu __instance, [HarmonyArgument(0)] RoleBehaviour role)
            {
                if (advancedMenuText == null)
                {
                    advancedMenuText = UnityEngine.Object.Instantiate(nameTemplate, __instance.transform);
                    advancedMenuText.transform.localScale = Vector3.one * 0.9f;
                    advancedMenuText.transform.localPosition = new Vector3(-3.1f, 1.05f, -32f);
                    advancedMenuText.autoSizeTextContainer = true;
                    advancedMenuText.text = "";
                    advancedMenuText.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                    advancedMenuText.fontWeight = TMPro.FontWeight.Black;
                    advancedMenuText.color = Palette.Black;
                    advancedMenuText.fontSize = advancedMenuText.fontSizeMax = 5f;
                    advancedMenuText.gameObject.SetActive(false);
                }

                if (advancedMenuBody == null)
                {
                    advancedMenuBody = UnityEngine.Object.Instantiate(bodyTemplate, __instance.transform);
                    advancedMenuBody.transform.localScale = Vector3.one * 1.25f;
                    advancedMenuBody.transform.localPosition = new Vector3(-2.6f, -0.5f, -32f);
                    advancedMenuBody.text = ModTranslation.getString("roleNoSettings");
                    advancedMenuBody.alignment = TMPro.TextAlignmentOptions.Center;
                    advancedMenuBody.fontWeight = TMPro.FontWeight.Black;
                    advancedMenuBody.color = Palette.Black;
                    advancedMenuBody.fontSize = advancedMenuBody.fontSizeMax = 3f;
                    advancedMenuBody.gameObject.SetActive(false);
                }

                advancedMenuShown = true;
                var roleInfo = RoleInfo.getRoleInfo(role);
                currentOptions = roleInfo.baseOption.getChildren();
                //Helpers.log($"{(CustomRoleTypes)role.Role} ShowAdvancedSettings: {advancedMenuText.transform.localPosition.x} {advancedMenuText.transform.localPosition.y} {advancedMenuText.transform.localPosition.z}");

                optionOffset = __instance.YStart;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    advancedMenuText.gameObject.SetActive(true);
                    advancedMenuText.enabled = true;
                    advancedMenuText.text = roleInfo.name;

                    if (currentOptions.Count == 0)
                    {
                        advancedMenuBody.gameObject.SetActive(true);
                        advancedMenuBody.enabled = true;
                    }
                }
                else
                { 
                    switch(role.Role)
                    {
                        case RoleTypes.Shapeshifter:
                        case RoleTypes.GuardianAngel:
                            optionOffset -= 1.5f;
                            break;

                        case RoleTypes.Scientist:
                        case RoleTypes.Engineer:
                            optionOffset -= 1.0f;
                            break;
                    }
                }

                for (int i = 0; i < currentOptions.Count; i++)
                {
                    CustomOption option = currentOptions[i];
                    if (option.optionBehaviour == null)
                    {
                        StringOption stringOption = UnityEngine.Object.Instantiate(optionTemplate, __instance.transform);

                        stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
                        stringOption.TitleText.text = option.getName();
                        stringOption.Value = stringOption.oldValue = option.selection;
                        stringOption.ValueText.text = option.getString();

                        option.optionBehaviour = stringOption;
                    }
                    option.optionBehaviour.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.CloseAdvancedSettings))]
        class RolesSettingsMenuCloseAdvancedSettingsPatch
        {
            public static void Postfix(RolesSettingsMenu __instance)
            {
                advancedMenuShown = false;
                foreach (CustomOption option in currentOptions)
                {
                    option.optionBehaviour?.gameObject?.SetActive(false);
                }
                currentOptions.Clear();
                advancedMenuText.gameObject.SetActive(false);
                advancedMenuBody.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
        class RolesSettingsMenuUpdatePatch
        {
            public static void Postfix(RolesSettingsMenu __instance)
            {
                if (!advancedMenuShown) return;

                float numItems = currentOptions.Count;
                float offset = optionOffset - 1.2f;

                // First calculate the # of items that are visible
                foreach (CustomOption option in currentOptions)
                {
                    if (option?.optionBehaviour != null && option.optionBehaviour.gameObject != null)
                    {
                        bool enabled = true;
                        var parent = option.parent;

                        if (option.isHidden)
                        {
                            enabled = false;
                        }

                        while (parent != null && enabled)
                        {
                            enabled = parent.enabled;
                            parent = parent.parent;
                        }

                        if (!enabled) numItems--;
                    } else
                    {
                        numItems--;
                    }
                }

                float itemScale = Mathf.Min(0.865f, 6.0f / numItems);

                foreach (CustomOption option in currentOptions)
                {
                    if (option?.optionBehaviour != null && option.optionBehaviour.gameObject != null)
                    {
                        bool enabled = true;
                        var parent = option.parent;

                        if (option.isHidden)
                        {
                            enabled = false;
                        }

                        while (parent != null && enabled)
                        {
                            enabled = parent.enabled;
                            parent = parent.parent;
                        }

                        option.optionBehaviour.gameObject.SetActive(enabled);
                        if (enabled)
                        {
                            offset -= 0.5f * itemScale;
                            option.optionBehaviour.transform.localPosition = new Vector3(-2.75f, offset, -16f);
                            option.optionBehaviour.transform.localScale = Vector3.one * itemScale;
                        }
                    }
                }
            }
        }
    }
}