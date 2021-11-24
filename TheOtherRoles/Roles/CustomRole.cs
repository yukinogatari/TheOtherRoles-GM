using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using TheOtherRoles;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Patches;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles
{
    public enum CustomRoleTypes
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


        Neutral = 150,
        Mini,
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
        Husk,

        // don't put anything below this
        NoRole = ushort.MaxValue
    }

    public enum CustomRoleTeamTypes
    {
        /*
        Crewmate = 0,
        Impostor = 1
        */

        Neutral = 100,
        Jackal,
        Jester,
        Arsonist,
        Lovers,
        Opportunist,
        Vulture,

        None = ushort.MaxValue,
    }

    [HarmonyPatch]
    public class CustomRole : RoleBehaviour
    {
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

        public virtual void Init() { 
            if (Player != null)
            {
                Player.Data.Role.NameColor = NameColor;
            }
        }

        public virtual bool InitButtons() { return true; }
        public virtual void SetTarget() { }
        public virtual void OnExiled() { }
        public virtual void OnKilled() { }
        public virtual void OnKill() { }
        public virtual void OnMeeting() { }
        public virtual void OnMeetingEnd() { }
        public virtual new bool DidWin(GameOverReason gameOverReason)
        {
            return false;
        }

        public void _FixedUpdate() {
            if (PlayerControl.LocalPlayer == Player)
            {
                _RoleUpdate();
            }
        }

        public virtual void _RoleUpdate() { }

        public virtual void _HandleDisconnect(PlayerControl pc, DisconnectReasons reason) { }

        [HarmonyPatch(typeof(RoleBehaviour), "IsImpostor", MethodType.Getter)]
        class RoleBehaviorIsImpostor
        {
            public static bool Prefix(RoleBehaviour __instance, ref bool __result)
            {
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = role.TeamType == RoleTeamTypes.Impostor;
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
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RoleBehaviour), "IsAffectedByComms", MethodType.Getter)]
        class RoleBehaviorIsAffectedByComms
        {
            public static bool Prefix(RoleBehaviour __instance, ref bool __result)
            {
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = role.IsAffectedByComms;
                    return false;
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
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = RoleInfo.getRoleInfo(role).name;
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
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = RoleInfo.getRoleInfo(role).blurb;
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
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = RoleInfo.getRoleInfo(role).introDescription;
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
                RoleBehaviour role = __instance.Player.customRole() ?? __instance;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __result = RoleInfo.getRoleInfo(role).fullDescription;
                    return false;
                }
                return true;
            }
        }
    }

    public static class RoleHelpers
    {
        public static bool roleExists(RoleTypes type)
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p.isRole(type)) return true;
            }
            return false;
        }

        public static bool roleExists(CustomRoleTypes type) { return roleExists((RoleTypes)type); }

        public static List<PlayerControl> getPlayersWithRole(RoleTypes type)
        {
            List<PlayerControl> players = new List<PlayerControl>();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p.isRole(type)) players.Add(p);
            }
            return players;
        }

        public static List<PlayerControl> getPlayersWithRole(CustomRoleTypes type) { return getPlayersWithRole((RoleTypes)type); }

        public static List<PlayerControl> getPlayersWithModifier(RoleModifierTypes type)
        {
            List<PlayerControl> players = new List<PlayerControl>();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p.hasModifier(type)) players.Add(p);
            }
            return players;
        }

        public static List<T> getRoles<T>() where T : CustomRole
        {
            return CustomRoleManager.allRoles.Values.Where(x => x is T).Cast<T>().ToList();
        }
    }

    public static class RoleExtension
    {

        public static RoleBehaviour role(this PlayerControl pc)
        {
            return customRole(pc) ?? pc?.Data?.Role;
        }

        public static CustomRole customRole(this PlayerControl pc)
        {
            if (pc == null) return null;
            return CustomRoleManager.allRoles.ContainsKey(pc.PlayerId) ? CustomRoleManager.allRoles[pc.PlayerId] : null;
        }

        public static T role<T>(this PlayerControl pc) where T : CustomRole
        {
            return customRole(pc) as T;
        }

        public static CustomRole setRole(this PlayerControl pc, CustomRoleTypes type)
        {
            CustomRole role = CustomRoleManager.CreateRole(type, pc);
            setRole(pc, role);
            return role;
        }

        public static void setRole(this PlayerControl pc, CustomRole role)
        {
            role.Player = pc;

            if (role.IsImpostor)
            {
                pc.Data.Role = new ImpostorRole();
            }
            else
            {
                pc.Data.Role = new CrewmateRole();
            }

            pc.Data.Role.Initialize(pc);
            pc.roleAssigned = true;
            CustomRoleManager.allRoles[pc.PlayerId] = role;
        }

        public static bool isRole(this PlayerControl pc, RoleTypes type)
        {
            return role(pc)?.Role == type;
        }

        public static bool isRole(this PlayerControl pc, CustomRoleTypes type)
        {
            if (pc == null || role(pc) == null) return false;

            CustomRoleTypes playerRole = (CustomRoleTypes)role(pc).Role;

            // Handle some special cases where we're looking for a class of roles that have more specific types.
            if (type == CustomRoleTypes.Mini && (playerRole == CustomRoleTypes.EvilMini || playerRole == CustomRoleTypes.NiceMini))
                return true;

            if (type == CustomRoleTypes.Guesser && (playerRole == CustomRoleTypes.EvilGuesser || playerRole == CustomRoleTypes.NiceGuesser))
                return true;

            if (type == CustomRoleTypes.Mafia && (playerRole == CustomRoleTypes.Godfather || playerRole == CustomRoleTypes.Mafioso || playerRole == CustomRoleTypes.Janitor))
                return true;

            if (type == CustomRoleTypes.Lovers && pc.hasModifier(RoleModifierTypes.Lovers))
                return true;

            return playerRole == type;
        }

        public static bool isRelated(this PlayerControl pc, PlayerControl target)
        {
            return (pc.hasModifier(RoleModifierTypes.Lovers) && (target == pc.getPartner())) ||
                   ((pc.isRole(CustomRoleTypes.Jackal) || pc.isRole(CustomRoleTypes.Sidekick)) && (pc.role<Jackal>().getFamily().Contains(target)));
        }
    }
}