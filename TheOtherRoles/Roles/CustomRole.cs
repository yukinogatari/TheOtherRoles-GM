using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheOtherRoles;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{

    enum CustomRoleTypes
    {
        /*
        Crewmate = 0,
        Impostor = 1,
        Scientist = 2,
        Engineer = 3,
        GuardianAngel = 4,
        Shapeshifter = 5,
        */

        Crewmate = 50,
        Shifter,
        Mayor,
        Sheriff,
        Lighter,
        Detective,
        TimeMaster,
        Medic,
        Swapper,
        Seer,
        Hacker,
        Tracker,
        Snitch,
        Spy,
        SecurityGuard,
        Bait,
        Medium,


        Impostor = 100,
        Mafia,
        Godfather,
        Mafioso,
        Janitor,
        Camouflager,
        Vampire,
        Eraser,
        Trickster,
        Cleaner,
        Warlock,
        BountyHunter,
        Madmate,


        Mini = 150,
        NiceMini,
        EvilMini,
        Lovers,
        Guesser,
        NiceGuesser,
        EvilGuesser,
        Jester,
        Arsonist,
        Jackal,
        Sidekick,
        Opportunist,
        Vulture,


        GM = 200,


        // don't put anything below this
        NoRole = ushort.MaxValue
    }

    enum CustomRoleTeamTypes
    {
        /*
        Crewmate = 0,
        Impostor = 1
        */

        Jackal = 100,
        Jester,
        Arsonist,
        Lovers,
        Opportunist,
        Vulture,

        None = ushort.MaxValue,
    }

    /* RoleManager.SelectRoles
     * 
     * AssignRolesForTeam(GameData.PlayerInfo, this.RoleOptions, RoleTeamTypes.Impostor, maxImpostors, defaultRole: RoleTypes.Impostor)
     * AssignRolesForTeam(GameData.PlayerInfo, this.RoleOptions, RoleTeamTypes.Crewmate, maxInt, defaultRole: RoleTypes.Crewmate)
     * 
     */

    /* RoleManager.AssignRolesForTeam(List<GameData.PlayerInfo> players, RoleOptionsData opts, RoleTeamTypes team, int teamMax, RoleTypes? defaultRole)
     * 
     * 
     * 
     */

    class CustomRole : RoleBehaviour
    {
        public static List<Type> allTypes = new List<Type> {
            typeof(Shifter),
            typeof(Mayor),
            typeof(Sheriff),
            typeof(Lighter),
            typeof(Detective),
            typeof(TimeMaster),
            typeof(Medic),
            typeof(Swapper),
            typeof(Seer),
            typeof(Hacker),
            typeof(Tracker),
            typeof(Snitch),
            typeof(Spy),
            typeof(SecurityGuard),
            typeof(Bait),
            typeof(Medium),

            typeof(Godfather),
            typeof(Mafioso),
            typeof(Janitor),
            typeof(Camouflager),
            typeof(Vampire),
            typeof(Eraser),
            typeof(Trickster),
            typeof(Cleaner),
            typeof(Warlock),
            typeof(BountyHunter),
            typeof(Madmate),

            typeof(Mini),
            typeof(Lovers),
            typeof(Guesser),
            typeof(Jester),
            typeof(Arsonist),
            typeof(Jackal),
            typeof(Sidekick),
            typeof(Opportunist),
            typeof(Vulture),
            typeof(GM),
        };

        // public bool IsImpostor { get => { return TeamType == RoleTeamTypes.Impostor; } }
        // public bool IsSimpleRole { get => { return Role == RoleTypes.Crewmate || Role == RoleTypes.Impostor; } }
        // public bool IsAffectedByComms { get => { is comms active & not RoleTeamTypes.Impostor } }
        // public Color TeamColor { get => { ImpostorRed if RoleTeamTypes.Impostor, or CrewmateBlue if RoleTeamTypes.Crewmate } }

        // public string NiceName { get => { TranslationController.GetString(StringName)  } }
        // public string Blurb { get => { TranslationController.GetString(BlurbName) } }
        // public string BlurbMed { get => { TranslationController.GetString(BlurbNameLong) } }
        // public string BlurbLong { get => { TranslationController.GetString(StringName) } }

        // protected bool CommsSabotaged { get => { is comms active } }

        public CustomRole() : base()
        {
            NameColor = RoleColors.None;
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.None;
            Role = (RoleTypes)CustomRoleTypes.NoRole;
            MaxCount = 0;
        }

        public static void InitializeAll()
        {
            foreach (Type t in allTypes)
            {
                t.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
            }
        }
    }

    class CustomCrewRole : CustomRole
    {
        public CustomCrewRole() : base()
        {
            NameColor = new Color32(0, 0, 0, byte.MaxValue);
            TeamType = RoleTeamTypes.Crewmate;
            Role = RoleTypes.Crewmate;
            MaxCount = 15;
        }
    }

    class CustomImpostorRole : CustomRole
    {
        public CustomImpostorRole() : base()
        {
            NameColor = Palette.ImpostorRed;
            TeamType = RoleTeamTypes.Impostor;
            Role = RoleTypes.Impostor;
            MaxCount = 15;
        }
    }

    [HarmonyPatch(typeof(RoleBehaviour), "IsImpostor", MethodType.Getter)]
    class RoleBehaviorIsImpostor
    {
        public static bool Prefix(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                __result = __instance.TeamType == RoleTeamTypes.Impostor;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RoleBehaviour), "IsSimpleRole", MethodType.Getter)]
    class RoleBehaviorIsSimpleRole
    {
        public static bool Prefix(RoleBehaviour __instance, ref bool __result)
        {
            return true;
        }
    }

    [HarmonyPatch(typeof(RoleBehaviour), "IsAffectedByComms", MethodType.Getter)]
    class RoleBehaviorIsAffectedByComms
    {
        public static bool Prefix(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                //return false;
            }
            return true;
        }
    }

    // For some reason the game crashes if we try to mess with TeamColor
/*    [HarmonyPatch(typeof(RoleBehaviour), "TeamColor", MethodType.Getter)]
    class RoleBehaviorTeamColor
    {
        public static void Postfix(RoleBehaviour __instance, ref Color __result)
        {
            __result = Palette.ImpostorRed;
        }
    }*/

  
    [HarmonyPatch(typeof(RoleBehaviour), "NiceName", MethodType.Getter)]
    class RoleBehaviorNiceName
    {
        public static bool Prefix(RoleBehaviour __instance, ref string __result)
        {
            if (__instance.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                __result = RoleInfo.getRoleInfo(__instance.Role).nameColored;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RoleBehaviour), "Blurb", MethodType.Getter)]
    class RoleBehaviorBlurb
    {
        public static bool Prefix(RoleBehaviour __instance, ref string __result)
        {
            if (__instance.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                __result = RoleInfo.getRoleInfo(__instance.Role).blurb;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RoleBehaviour), "BlurbMed", MethodType.Getter)]
    class RoleBehaviorBlurbMed
    {
        public static bool Prefix(RoleBehaviour __instance, ref string __result)
        {
            if (__instance.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                __result = RoleInfo.getRoleInfo(__instance.Role).introDescription;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RoleBehaviour), "BlurbLong", MethodType.Getter)]
    class RoleBehaviorBlurbLong
    {
        public static bool Prefix(RoleBehaviour __instance, ref string __result)
        {
            if (__instance.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                __result = RoleInfo.getRoleInfo(__instance.Role).fullDescription;
                return false;
            }
            return true;
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
                parent.RoleIcon.sprite = TheOtherRoles.getBlankIcon();

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ShowAdvancedSettings))]
    class RoleOptionSettingUpdatePatch
    {
        public static void Postfix(RolesSettingsMenu __instance, [HarmonyArgument(0)]RoleBehaviour role)
        {
            if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {

                //Helpers.log($"{role.Role} ShowAdvancedSettings");
            }
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
    class RoleSettingMenuUpdatePatch
    {
        public static void Postfix(RolesSettingsMenu __instance)
        {
            var s = __instance.GetComponentInChildren<Scroller>();
            if (s != null)
            {
                //Helpers.log($"{s.transform.position.y}/{s.transform.localPosition.y} {s.Inner.localPosition.y} => {s.GetScrollPercY()}");
            }
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Update))]
    class GameSettingMenuUpdatePatch
    {
        public static void Postfix(GameSettingMenu __instance)
        {
            //var s = __instance.Scroller;
            //Helpers.log($"{s.YBounds.min}~{s.YBounds.max} / {s.ScrollerYRange.min}~{s.ScrollerYRange.max} / {s.Inner.localPosition.y}");
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    class GameSettingMenuPatch
    {
        public static List<CustomRoleTypes> HiddenRoles = new List<CustomRoleTypes> {
            CustomRoleTypes.Crewmate,
            CustomRoleTypes.Impostor,
            CustomRoleTypes.Sidekick,
            CustomRoleTypes.Guesser,
            CustomRoleTypes.EvilGuesser,
            CustomRoleTypes.NiceGuesser,
            CustomRoleTypes.Mini,
            CustomRoleTypes.EvilMini,
            CustomRoleTypes.NiceMini,
            CustomRoleTypes.Mafia,
            CustomRoleTypes.Godfather,
            CustomRoleTypes.Mafioso,
            CustomRoleTypes.Janitor,
            CustomRoleTypes.NoRole
        };

        public static void Postfix(GameSettingMenu __instance)
        {
            var template = __instance.RolesSettings.AllRoleSettings[0];

            foreach (CustomRoleTypes role in Enum.GetValues(typeof(CustomRoleTypes)))
            {
                if (HiddenRoles.Contains(role)) continue;

                var roleSettings = UnityEngine.Object.Instantiate(template, template.transform.parent);
                roleSettings.Role = new Sheriff();
                __instance.RolesSettings.AllRoleSettings.Add(roleSettings);

                var menuTemplate = __instance.RolesSettings.AllAdvancedSettingTabs[0];
                var newMenu = new AdvancedRoleSettingsButton();
                newMenu.Type = roleSettings.Role.Role;
                newMenu.Tab = UnityEngine.Object.Instantiate(menuTemplate.Tab, menuTemplate.Tab.transform.parent);
                __instance.RolesSettings.AllAdvancedSettingTabs.Add(newMenu);

                foreach (var c in newMenu.Tab.GetComponentsInChildren<Component>())
                    UnityEngine.Object.Destroy(c);
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

}