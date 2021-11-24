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
    public static class CustomRoleManager
    {
        public static Dictionary<Type, CustomRoleTypes> allRoleTypes = new Dictionary<Type, CustomRoleTypes> {
            { typeof(Shifter), CustomRoleTypes.Shifter },
            { typeof(Mayor), CustomRoleTypes.Mayor },
            { typeof(Sheriff), CustomRoleTypes.Sheriff },
            { typeof(Lighter), CustomRoleTypes.Lighter },
            { typeof(Detective), CustomRoleTypes.Detective },
            { typeof(TimeMaster), CustomRoleTypes.TimeMaster },
            { typeof(Medic), CustomRoleTypes.Medic },
            { typeof(Swapper), CustomRoleTypes.Swapper },
            { typeof(Seer), CustomRoleTypes.Seer },
            { typeof(Hacker), CustomRoleTypes.Hacker },
            { typeof(Tracker), CustomRoleTypes.Tracker },
            { typeof(Snitch), CustomRoleTypes.Snitch },
            { typeof(Spy), CustomRoleTypes.Spy },
            { typeof(SecurityGuard), CustomRoleTypes.SecurityGuard },
            { typeof(Bait), CustomRoleTypes.Bait },
            { typeof(Medium), CustomRoleTypes.Medium },

            { typeof(Mafia), CustomRoleTypes.Mafia },
            { typeof(Godfather), CustomRoleTypes.Godfather },
            { typeof(Mafioso), CustomRoleTypes.Mafioso },
            { typeof(Janitor), CustomRoleTypes.Janitor },
            { typeof(Camouflager), CustomRoleTypes.Camouflager },
            { typeof(Vampire), CustomRoleTypes.Vampire },
            { typeof(Eraser), CustomRoleTypes.Eraser },
            { typeof(Trickster), CustomRoleTypes.Trickster },
            { typeof(Cleaner), CustomRoleTypes.Cleaner },
            { typeof(Warlock), CustomRoleTypes.Warlock },
            { typeof(BountyHunter), CustomRoleTypes.BountyHunter },
            { typeof(Madmate), CustomRoleTypes.Madmate },

            { typeof(Mini), CustomRoleTypes.Mini },
            { typeof(NiceMini), CustomRoleTypes.NiceMini },
            { typeof(EvilMini), CustomRoleTypes.EvilMini },
            { typeof(Lovers), CustomRoleTypes.Lovers },
            { typeof(Guesser), CustomRoleTypes.Guesser },
            { typeof(NiceGuesser), CustomRoleTypes.NiceGuesser },
            { typeof(EvilGuesser), CustomRoleTypes.EvilGuesser },
            { typeof(Jester), CustomRoleTypes.Jester },
            { typeof(Arsonist), CustomRoleTypes.Arsonist },
            { typeof(Jackal), CustomRoleTypes.Jackal },
            { typeof(Sidekick), CustomRoleTypes.Sidekick },
            { typeof(Opportunist), CustomRoleTypes.Opportunist },
            { typeof(Vulture), CustomRoleTypes.Vulture },
            { typeof(GM), CustomRoleTypes.GM },
        };

        public static Dictionary<Type, RoleModifierTypes> allModifierTypes = new Dictionary<Type, RoleModifierTypes> {
            { typeof(LoversMod), RoleModifierTypes.Lovers },
            { typeof(MedicShield), RoleModifierTypes.MedicShield },
        };

        public static Dictionary<int, CustomRole> allRoles = new Dictionary<int, CustomRole>();
        public static Dictionary<int, List<RoleModifier>> allModifiers = new Dictionary<int, List<RoleModifier>>();

        public static CustomRole CreateRole(CustomRoleTypes type, PlayerControl? player = null)
        {
            foreach (var t in allRoleTypes)
            {
                if (type == t.Value)
                {
                    CustomRole role = (CustomRole)t.Key.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null).Invoke(null);
                    role.Role = (RoleTypes)type;
                    role.Player = player;
                    return role;
                }
            }
            throw new ArgumentException($"CustomRoleManager.Create could not find a constructor for CustomRoleTypes {type}.");
        }

        public static RoleModifier CreateMod(RoleModifierTypes type, PlayerControl? player = null, dynamic? extraData = null)
        {
            RoleModifier mod;
            foreach (var t in allModifierTypes)
            {
                if (type == t.Value)
                {
                    mod = (RoleModifier)t.Key.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null).Invoke(null);
                    break;
                }
            }

            mod = new GenericModifier();
            mod.Player = player;
            mod.type = type;
            return mod;
        }

        public static void FixedUpdate()
        {
            foreach (CustomRole r in allRoles.Values)
            {
                r._FixedUpdate();
            }
        }

        public static void MeetingStart()
        {
            foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
                pc?.MeetingStart();
        }

        public static void MeetingEnd()
        {
            foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
                pc?.MeetingEnd();
        }

        public static void MeetingStart(this PlayerControl pc)
        {
            pc?.customRole()?.OnMeeting();
            foreach (RoleModifier mod in pc?.getModifiers())
                mod.OnMeeting();
        }

        public static void MeetingEnd(this PlayerControl pc)
        {
            pc?.customRole()?.OnMeetingEnd();
            foreach (RoleModifier mod in pc?.getModifiers())
                mod.OnMeetingEnd();
        }

        public static void OnExiled(this PlayerControl pc)
        {
            pc?.customRole()?.OnExiled();
            foreach (RoleModifier mod in pc.getModifiers())
                mod.OnExiled();
        }

        public static void OnKilled(this PlayerControl pc)
        {
            pc?.customRole()?.OnKilled();
            foreach (RoleModifier mod in pc.getModifiers())
                mod.OnKilled();
        }

        public static void OnKill(this PlayerControl pc)
        {
            pc?.customRole()?.OnKill();
            foreach (RoleModifier mod in pc.getModifiers())
                mod.OnKill();
        }

        public static void Reset()
        {
            allRoles.Clear();
            allModifiers.Clear();
            SetupRoles();
            SetupModifiers();
            InitializeRoles();
            InitializeModifiers();
        }

        public static void InitializeSettings()
        {
            foreach (Type t in allRoleTypes.Keys)
            {
                t.GetMethod("InitSettings", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
            }

            foreach (Type t in allModifierTypes.Keys)
            {
                t.GetMethod("InitSettings", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
            }
        }

        public static void SetupRoles()
        {
            foreach (Type t in allRoleTypes.Keys)
            {
                t.GetMethod("Setup", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
            }
        }

        public static void SetupModifiers()
        {
            foreach (Type t in allModifierTypes.Keys)
            {
                t.GetMethod("Setup", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
            }
        }

        public static void InitializeRoles()
        {
            foreach (CustomRole r in allRoles.Values)
            {
                r.Init();
            }
        }

        public static void InitializeModifiers()
        {
            foreach (var mods in allModifiers.Values)
            {
                foreach (RoleModifier m in mods)
                    m.Init();
            }
        }

        public static void InitializeButtons()
        {
            foreach (CustomRole r in allRoles.Values)
            {
                if (PlayerControl.LocalPlayer == r.Player)
                {
                    try
                    {
                        r.InitButtons();
                    } catch {
                        Helpers.log($"Failed to initialize buttons for {r.Role} / {r.Player.Data.PlayerName}");
                    }
                }
            }
        }

        public static bool DidWin(this PlayerControl pc, GameOverReason gameOverReason)
        {
            return pc.role()?.DidWin(gameOverReason) == true || pc.getModifiers().Any(x => x.DidWin(gameOverReason));
        }

        public static void HandleDisconnect(this PlayerControl pc, DisconnectReasons reason)
        {
            foreach (CustomRole role in allRoles.Values)
                role._HandleDisconnect(pc, reason);

            foreach (var mods in allModifiers.Values)
                foreach (RoleModifier mod in mods)
                    mod._HandleDisconnect(pc, reason);

            allRoles.Remove(pc.PlayerId);
            pc.clearModifiers();
        }
    }
}