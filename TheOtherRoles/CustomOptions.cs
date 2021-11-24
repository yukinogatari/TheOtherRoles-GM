using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Reflection;
using System.Text;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Roles;

namespace TheOtherRoles {

    public class CustomOptionHolder {
        public static string[] rates = new string[]{"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"};
        public static string[] presets = new string[]{"preset1", "preset2", "preset3", "preset4", "preset5" };

        public static CustomOptionBlank mainOptions;

        public static CustomOption presetSelection;
        public static CustomOption crewmateRolesCountMin;
        public static CustomOption crewmateRolesCountMax;
        public static CustomOption neutralRolesCountMin;
        public static CustomOption neutralRolesCountMax;
        public static CustomOption impostorRolesCountMin;
        public static CustomOption impostorRolesCountMax;

        public static CustomOption specialOptions;
        public static CustomOption maxNumberOfMeetings;
        public static CustomOption blockSkippingInEmergencyMeetings;
        public static CustomOption noVoteIsSelfVote;
        public static CustomOption hidePlayerNames;

        public static CustomOption hideSettings;
        public static CustomOption restrictDevices;
        public static CustomOption restrictAdmin;
        public static CustomOption restrictCameras;
        public static CustomOption restrictVents;

        public static CustomOption uselessOptions;
        public static CustomOption playerColorRandom;
        public static CustomOption playerNameDupes;
        public static CustomOption disableVents;


        internal static Dictionary<RoleTypes, RoleTypes[]> blockedRolePairings = new Dictionary<RoleTypes, RoleTypes[]>();

        public static string cs(Color c, string s) {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
 
        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void Load() {

            mainOptions = new CustomOptionBlank(null);

            // Role Options
            presetSelection = CustomOption.Create(0, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, mainOptions, true);

            // Using new id's for the options to not break compatibilty with older versions
            crewmateRolesCountMin = CustomOption.Create(300, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMin"), 0f, 0f, 15f, 1f, mainOptions, true);
            crewmateRolesCountMax = CustomOption.Create(301, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMax"), 0f, 0f, 15f, 1f, mainOptions);
            neutralRolesCountMin = CustomOption.Create(302, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMin"), 0f, 0f, 15f, 1f, mainOptions);
            neutralRolesCountMax = CustomOption.Create(303, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMax"), 0f, 0f, 15f, 1f, mainOptions);
            impostorRolesCountMin = CustomOption.Create(304, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMin"), 0f, 0f, 15f, 1f, mainOptions);
            impostorRolesCountMax = CustomOption.Create(305, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMax"), 0f, 0f, 15f, 1f, mainOptions);

            // Other options
            specialOptions = new CustomOptionBlank(mainOptions);
            maxNumberOfMeetings = CustomOption.Create(3, "maxNumberOfMeetings", 10, 0, 15, 1, specialOptions, true);
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, "blockSkippingInEmergencyMeetings", false, specialOptions);
            noVoteIsSelfVote = CustomOption.Create(5, "noVoteIsSelfVote", false, specialOptions);
            hidePlayerNames = CustomOption.Create(6, "hidePlayerNames", false, specialOptions);
            hideSettings = CustomOption.Create(520, "hideSettings", false, specialOptions);

            restrictDevices = CustomOption.Create(510, "restrictDevices", new string[] { "optionOff", "restrictPerTurn", "restrictPerGame" }, specialOptions);
            restrictAdmin = CustomOption.Create(501, "disableAdmin", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictCameras = CustomOption.Create(502, "disableCameras", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictVents = CustomOption.Create(503, "disableVitals", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");

            uselessOptions = CustomOption.Create(530, "uselessOptions", false, mainOptions, isHeader: true);
            disableVents = CustomOption.Create(504, "disableVents", false, uselessOptions);
            playerColorRandom = CustomOption.Create(521, "playerColorRandom", false, uselessOptions);
            playerNameDupes = CustomOption.Create(522, "playerNameDupes", false, uselessOptions);

            blockedRolePairings.Add((RoleTypes)CustomRoleTypes.Vampire,  new[] { (RoleTypes)CustomRoleTypes.Warlock });
            blockedRolePairings.Add((RoleTypes)CustomRoleTypes.Warlock,  new[] { (RoleTypes)CustomRoleTypes.Vampire });
            blockedRolePairings.Add((RoleTypes)CustomRoleTypes.Spy,      new[] { (RoleTypes)CustomRoleTypes.NiceMini, (RoleTypes)CustomRoleTypes.EvilMini });
            blockedRolePairings.Add((RoleTypes)CustomRoleTypes.NiceMini, new[] { (RoleTypes)CustomRoleTypes.Spy });
            blockedRolePairings.Add((RoleTypes)CustomRoleTypes.EvilMini, new[] { (RoleTypes)CustomRoleTypes.Spy });

            CustomRoleManager.InitializeSettings();
        }
    }

    public class CustomOption {
        public static List<CustomOption> options = new List<CustomOption>();
        public static int preset = 0;

        public int id;
        public string name;
        public string format;
        public System.Object[] selections;

        public int defaultSelection;
        public ConfigEntry<int> entry;
        public int selection;
        public OptionBehaviour optionBehaviour;
        public CustomOption parent;
        public List<CustomOption> children;
        public bool isHeader;
        public bool isHidden;

        public virtual bool enabled
        {
            get
            {
                return this.getBool();
            }
        }

        // Option creation
        public CustomOption()
        {

        }

        public CustomOption(int id, string name,  System.Object[] selections, System.Object defaultValue, CustomOption parent, bool isHeader, bool isHidden, string format) {
            this.id = id;
            this.name = name;
            this.format = format;
            this.selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            this.defaultSelection = index >= 0 ? index : 0;
            this.parent = parent;
            this.isHeader = isHeader;
            this.isHidden = isHidden;

            this.children = new List<CustomOption>();
            if (parent != null) {
                parent.children.Add(this);
            }

            selection = 0;
            if (id > 0) {
                entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", id.ToString(), defaultSelection);
                selection = Mathf.Clamp(entry.Value, 0, selections.Length - 1);
            }
            options.Add(this);
        }

        public static CustomOption Create(int id, string name, string[] selections, CustomOption parent = null, bool isHeader = false, bool isHidden = false, string format = "") {
            return new CustomOption(id, name, selections, "", parent, isHeader, isHidden, format);
        }

        public static CustomOption Create(int id, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false, bool isHidden = false, string format = "") {
            List<float> selections = new List<float>();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, name, selections.Cast<object>().ToArray(), defaultValue, parent, isHeader, isHidden, format);
        }

        public static CustomOption Create(int id, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false, bool isHidden = false, string format = "") {
            return new CustomOption(id, name, new string[]{"optionOff", "optionOn"}, defaultValue ? "optionOn" : "optionOff", parent, isHeader, isHidden, format);
        }

        // Static behaviour

        public static void switchPreset(int newPreset) {
            CustomOption.preset = newPreset;
            foreach (CustomOption option in CustomOption.options) {
                if (option.id <= 0) continue;

                option.entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", option.id.ToString(), option.defaultSelection);
                option.selection = Mathf.Clamp(option.entry.Value, 0, option.selections.Length - 1);
                if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption) {
                    stringOption.oldValue = stringOption.Value = option.selection;
                    stringOption.ValueText.text = option.getString();
                }
            }
        }

        public static void ShareOptionSelections() {
            if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance?.AmHost == false && PlayerControl.LocalPlayer == null) return;
            foreach (CustomOption option in CustomOption.options) {
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareOptionSelection, Hazel.SendOption.Reliable);
                messageWriter.WritePacked((uint)option.id);
                messageWriter.WritePacked((uint)Convert.ToUInt32(option.selection));
                messageWriter.EndMessage();
            }
        }

        // Getter

        public virtual int getSelection() {
            return selection;
        }

        public virtual bool getBool() {
            return selection > 0;
        }

        public virtual float getFloat() {
            return (float)selections[selection];
        }

        public virtual string getString()
        {
            string sel = selections[selection].ToString();
            if (format != "")
            {
                return string.Format(ModTranslation.getString(format), sel);
            }
            return ModTranslation.getString(sel);
        }

        public virtual string getName()
        {
            return ModTranslation.getString(name);
        }

        public virtual List<CustomOption> getChildren()
        {
            List<CustomOption> options = new List<CustomOption>();
            foreach (CustomOption op in children)
            {
                options.Add(op);
                options.AddRange(op.getChildren());
            }
            return options;
        }

        // Option changes

        public virtual void updateSelection(int newSelection) {
            selection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
            if (optionBehaviour != null && optionBehaviour is StringOption stringOption) {
                stringOption.oldValue = stringOption.Value = selection;
                stringOption.ValueText.text = getString();

                if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer) {
                    if (id == 0) switchPreset(selection); // Switch presets
                    else if (entry != null) entry.Value = selection; // Save selection to config

                    ShareOptionSelections();// Share all selections
                }
            }
        }
    }

    public class CustomOptionBlank : CustomOption
    {
        public CustomOptionBlank(CustomOption parent) {
            this.parent = parent;
            this.id = -1;
            this.name = "";
            this.isHeader = false;
            this.isHidden = true;
            this.children = new List<CustomOption>();
            this.selections = new string[] { "" };
            if (parent != null)
            {
                parent.children.Add(this);
            }
            options.Add(this);
        }

        public override bool enabled
        {
            get
            {
                return true;
            }
        }

        public override int getSelection()
        {
            return 0;
        }

        public override bool getBool()
        {
            return true;
        }

        public override float getFloat()
        {
            return 0f;
        }

        public override string getString()
        {
            return "";
        }

        public override void updateSelection(int newSelection)
        {
            return;
        }

    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    class GameOptionsMenuStartPatch {

        public static void Prefix(GameOptionsMenu __instance)
        {
        }

        public static void Postfix(GameOptionsMenu __instance) {
            var template = UnityEngine.Object.FindObjectsOfType<StringOption>().FirstOrDefault();
            if (template == null) return;

            List<OptionBehaviour> allOptions = __instance.Children.ToList();
            List<CustomOption> mainOptions = CustomOptionHolder.mainOptions.getChildren();
            for (int i = 0; i < mainOptions.Count; i++)
            {
                CustomOption option = mainOptions[i];
                if (option.optionBehaviour == null) {
                    StringOption stringOption = UnityEngine.Object.Instantiate(template, template.transform.parent);
                    allOptions.Add(stringOption);

                    stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => {});
                    stringOption.TitleText.text = option.getName();
                    stringOption.Value = stringOption.oldValue = option.selection;
                    stringOption.ValueText.text = option.getString();

                    option.optionBehaviour = stringOption;
                }
                option.optionBehaviour.gameObject.SetActive(true);
            }

            /*            var numTemplate = UnityEngine.Object.FindObjectsOfType<NumberOption>().FirstOrDefault();
                        if (numTemplate)
                        {
                            NumberOption maxPlayers = UnityEngine.Object.Instantiate(numTemplate, numTemplate.transform.parent);
                            maxPlayers.Value = maxPlayers.oldValue = PlayerControl.GameOptions.MaxPlayers;
                            maxPlayers.ValidRange = new FloatRange(4f, 15f);
                            allOptions.Add(maxPlayers);
                        }*/

            // Messing with the # of impostors on the new version gets you kicked from the room orz
            //var numImpostorsOption = allOptions.FirstOrDefault(x => x.name == "NumImpostors").TryCast<NumberOption>();
            //if (numImpostorsOption != null) numImpostorsOption.ValidRange = new FloatRange(0f, 15f);

            var killCoolOption = allOptions.FirstOrDefault(x => x.name == "KillCooldown").TryCast<NumberOption>();
            if (killCoolOption != null) killCoolOption.ValidRange = new FloatRange(2.5f, 60f);

            var commonTasksOption = allOptions.FirstOrDefault(x => x.name == "NumCommonTasks").TryCast<NumberOption>();
            if(commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);

            var shortTasksOption = allOptions.FirstOrDefault(x => x.name == "NumShortTasks").TryCast<NumberOption>();
            if(shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);

            var longTasksOption = allOptions.FirstOrDefault(x => x.name == "NumLongTasks").TryCast<NumberOption>();
            if(longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
            
            __instance.Children = allOptions.ToArray();
        }
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
    public class NumberOptionEnablePatch
    {
        public static bool Prefix(NumberOption __instance)
        {
            if (__instance.Title == StringNames.MatchMaxPlayers)
            {
                __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                __instance.FixedUpdate();
                __instance.Value = (float)PlayerControl.GameOptions.MaxPlayers;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    public class GameOptionsMenuValueChangedPatch
    {
        public static bool Prefix(GameOptionsMenu __instance, OptionBehaviour option)
        {
            if (AmongUsClient.Instance && AmongUsClient.Instance.AmHost && option.Title == StringNames.MatchMaxPlayers)
            {
                PlayerControl.GameOptions.MaxPlayers = option.GetInt();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.OnEnable))]
    public class KeyValueOptionEnablePatch
    {
        public static void Postfix(KeyValueOption __instance)
        {
            GameOptionsData gameOptions = PlayerControl.GameOptions;
            if (__instance.Title == StringNames.GameMapName)
            {
                __instance.Selected = Mathf.Clamp(gameOptions.MapId, 0, Constants.MapNames.Length - 1);
            }

            if (__instance.Values != null && __instance.Values.Count > 0 && __instance.ValueText != null)
                __instance.ValueText.text = __instance.Values[Mathf.Clamp(__instance.Selected, 0, __instance.Values.Count - 1)].Key;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    public class StringOptionEnablePatch {
        public static bool Prefix(StringOption __instance) {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => {});
            __instance.TitleText.text = option.getName();
            __instance.Value = __instance.oldValue = option.selection;
            __instance.ValueText.text = option.getString();
            
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public class StringOptionIncreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.updateSelection(option.selection + 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public class StringOptionDecreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.updateSelection(option.selection - 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public class RpcSyncSettingsPatch
    {
        public static void Postfix()
        {
            CustomOption.ShareOptionSelections();
        }
    }


    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
    class GameOptionsMenuUpdatePatch
    {
        private static float timer = 1f;
        public static void Postfix(GameOptionsMenu __instance) {
            timer += Time.deltaTime;
            if (timer < 0.1f) return;
            timer = 0f;

            float numItems = __instance.Children.Length;

            float offset = -7.00f;
            foreach (CustomOption option in CustomOption.options) {
                if (option?.optionBehaviour != null && option.optionBehaviour.gameObject != null) {
                    bool enabled = true;
                    var parent = option.parent;

                    if (AmongUsClient.Instance?.AmHost == false && CustomOptionHolder.hideSettings.getBool())
                    {
                        enabled = false;
                    }

                    if (option.isHidden)
                    {
                        enabled = false;
                    }

                    while (parent != null && enabled) {
                        enabled = parent.enabled;
                        parent = parent.parent;
                    }

                    option.optionBehaviour.gameObject.SetActive(enabled);
                    if (enabled) {
                        offset -= option.isHeader ? 0.75f : 0.5f;
                        option.optionBehaviour.transform.localPosition = new Vector3(option.optionBehaviour.transform.localPosition.x, offset, option.optionBehaviour.transform.localPosition.z);

                        if (option.isHeader)
                        {
                            numItems += 0.5f;
                        }
                    } else
                    {
                        numItems--;
                    }
                }
            }
            __instance.GetComponentInParent<Scroller>().YBounds.max = -4.0f + numItems * 0.5f;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    class GameSettingMenuPatch
    {
        public static Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.KeyValuePair<string, int>> getMapNames()
        {
            var options = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.KeyValuePair<string, int>>();
            for (int i = 0; i < Constants.MapNames.Length; i++)
            {
                var kvp = new Il2CppSystem.Collections.Generic.KeyValuePair<string, int>();
                kvp.key = Constants.MapNames[i];
                kvp.value = i;
                options.Add(kvp);
            }
            return options;
        }

        public static void Prefix(GameSettingMenu __instance) {
            __instance.HideForOnline = new Transform[]{};
        }

        public static void Postfix(GameSettingMenu __instance) {
            var mapNameTransform = __instance.AllItems.FirstOrDefault(x => x.gameObject.activeSelf && x.name.Equals("MapName", StringComparison.OrdinalIgnoreCase));
            if (mapNameTransform == null) return;
            mapNameTransform.GetComponent<KeyValueOption>().Values = getMapNames();
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.ShouldFlipSkeld))]
    class ConstantsShouldFlipSkeldPatch {
        public static bool Prefix(ref bool __result) {
            if (PlayerControl.GameOptions == null) return true;
            __result = PlayerControl.GameOptions.MapId == 3;
            return false;
        }

        public static bool aprilFools
        {
            get
            {
                try
                {
                    DateTime utcNow = DateTime.UtcNow;
                    DateTime t = new DateTime(utcNow.Year, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    DateTime t2 = t.AddDays(1.0);
                    if (utcNow >= t && utcNow <= t2)
                    {
                        return true;
                    }
                }
                catch
                {
                }
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(FreeWeekendShower), nameof(FreeWeekendShower.Start))]
    class FreeWeekendShowerPatch
    {
        public static bool Prefix()
        {
            return ConstantsShouldFlipSkeldPatch.aprilFools;
        }
    }

    [HarmonyPatch]
    class GameOptionsDataPatch
    {
        public static string tl(string key)
        {
            return ModTranslation.getString(key);
        }

        private static IEnumerable<MethodBase> TargetMethods() {
            return typeof(GameOptionsData).GetMethods().Where(x => x.ReturnType == typeof(string) && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(int));
        }

        public static string optionToString(CustomOption option)
        {
            if (option == null) return "";
            return $"{option.getName()}: {option.getString()}";
        }

        public static string optionsToString(CustomOption option, bool skipFirst = false)
        {
            if (option == null) return "";

            List<string> options = new List<string>();
            if (!option.isHidden && !skipFirst) options.Add(optionToString(option));
            if (option.enabled) { 
                foreach (CustomOption op in option.children)
                {
                    string str = optionsToString(op);
                    if (str != "") options.Add(str);
                }
            }
            return string.Join("\n", options);
        }

        private static void Postfix(GameOptionsData __instance, ref string __result)
        {
            bool hideSettings = AmongUsClient.Instance?.AmHost == false && CustomOptionHolder.hideSettings.getBool();
            if (hideSettings)
            {
                return;
            }

            List<string> pages = new List<string>();
            pages.Add(__result.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList().GetRange(0, 19).Join(delimiter: "\n"));

            StringBuilder entry = new StringBuilder();
            List<string> entries = new List<string>();

            // First add the presets and the role counts
            entries.Add(optionToString(CustomOptionHolder.presetSelection));

            var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), tl("crewmateRoles"));
            var min = CustomOptionHolder.crewmateRolesCountMin.getSelection();
            var max = CustomOptionHolder.crewmateRolesCountMax.getSelection();
            if (min > max) min = max;
            var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), tl("neutralRoles"));
            min = CustomOptionHolder.neutralRolesCountMin.getSelection();
            max = CustomOptionHolder.neutralRolesCountMax.getSelection();
            if (min > max) min = max;
            optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), tl("impostorRoles"));
            min = CustomOptionHolder.impostorRolesCountMin.getSelection();
            max = CustomOptionHolder.impostorRolesCountMax.getSelection();
            if (min > max) min = max;
            optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            entries.Add(entry.ToString().Trim('\r', '\n'));

            void addChildren(CustomOption option, ref StringBuilder entry, bool indent = true)
            {
                if (!option.enabled) return;

                foreach (var child in option.children)
                {
                    if (!child.isHidden)
                        entry.AppendLine((indent ? "    " : "") + optionToString(child));
                    addChildren(child, ref entry, indent);
                }
            }

            var roleData = __instance.RoleOptions;
            foreach (RoleInfo info in RoleInfo.allRoleInfos)
            {
                var key = (RoleTypes)info.type;
                if (roleData.roleRates.ContainsKey(key))
                {
                    var count = roleData.roleRates[key].MaxCount;
                    var chance = roleData.roleRates[key].Chance;
                    if (count > 0 && chance > 0)
                    {
                        entry = new StringBuilder();
                        entry.AppendLine($"{info.nameColored} [<color=#00FF00FF>{count}</color>]: {chance}%");
                        entry.AppendLine(info.roleOptions);
                        entries.Add(entry.ToString().Trim('\n', '\r').Replace("\n", "\n    "));
                    }
                }
            }

            entry = new StringBuilder();
            addChildren(CustomOptionHolder.specialOptions, ref entry, false);
            entries.Add(entry.ToString().Trim('\r', '\n'));

            int maxLines = 28;
            int lineCount = 0;
            string page = "";
            foreach (var e in entries)
            {
                int lines = e.Count(c => c == '\n') + 1;

                if (lineCount + lines > maxLines)
                {
                    pages.Add(page);
                    page = "";
                    lineCount = 0;
                }

                page = page + e + "\n\n";
                lineCount += lines + 1;
            }

            page = page.Trim('\r', '\n');
            if (page != "")
            {
                pages.Add(page);
            }

            int numPages = pages.Count;
            int counter = TheOtherRolesPlugin.optionsPage = TheOtherRolesPlugin.optionsPage % numPages;

            __result = pages[counter].Trim('\r', '\n') + "\n\n" + tl("pressTabForMore") + $" ({counter + 1}/{numPages})";
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.GetAdjustedNumImpostors))]
    public static class GameOptionsGetAdjustedNumImpostorsPatch
    {
        public static bool Prefix(GameOptionsData __instance, ref int __result)
        {
            __result = PlayerControl.GameOptions.NumImpostors;
            return false;
        }
    }

    [HarmonyPatch(typeof(SaveManager), "GameHostOptions", MethodType.Getter)]
    public static class SaveManagerGameHostOptionsPatch
    {
        private static int numImpostors;
        public static void Prefix()
        {
            if (SaveManager.hostOptionsData == null)
            {
                SaveManager.hostOptionsData = SaveManager.LoadGameOptions("gameHostOptions");
            }

            numImpostors = SaveManager.hostOptionsData.NumImpostors;
        }

        public static void Postfix(ref GameOptionsData __result)
        {
            __result.NumImpostors = numImpostors;
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class GameOptionsNextPagePatch
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            if(Input.GetKeyDown(KeyCode.Tab) && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) {
                TheOtherRolesPlugin.optionsPage = TheOtherRolesPlugin.optionsPage + 1;
            }
        }
    }

    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GameSettingsScalePatch {
        public static void Prefix(HudManager __instance) {
            if (__instance.GameSettings != null) __instance.GameSettings.fontSize = 1.2f; 
        }
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Start))]
    public class CreateOptionsPickerPatch
    {
        public static void Postfix(CreateOptionsPicker __instance)
        {
            int numImpostors = Math.Clamp(__instance.GetTargetOptions().NumImpostors, 1, 3);
            __instance.SetImpostorButtons(numImpostors);
        }
    }
}
