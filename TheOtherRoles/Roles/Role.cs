using System.Net;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using TheOtherRoles.Objects;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using TheOtherRoles.Patches;
using System.Reflection;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public static class RoleData
    {
        public static Dictionary<Type, RoleId> allRoleTypes = new Dictionary<Type, RoleId>
        {
            { typeof(RoleBase<Sheriff>), RoleId.Sheriff },
            { typeof(RoleBase<Lighter>), RoleId.Lighter },
            { typeof(RoleBase<Ninja>), RoleId.Ninja },
            { typeof(RoleBase<SerialKiller>), RoleId.SerialKiller },
            { typeof(RoleBase<Madmate>), RoleId.Madmate },
            { typeof(RoleBase<Opportunist>), RoleId.Opportunist },
            { typeof(RoleBase<PlagueDoctor>), RoleId.PlagueDoctor },
            { typeof(RoleBase<Fox>), RoleId.Fox},
            { typeof(RoleBase<Immoralist>), RoleId.Immoralist},
        };
    }

    public abstract class Role
    {
        public static List<Role> allRoles = new List<Role>();
        public PlayerControl player;
        public RoleId roleId;

        public abstract void OnMeetingStart();
        public abstract void OnMeetingEnd();
        public abstract void FixedUpdate();
        public abstract void OnKill(PlayerControl target);
        public abstract void OnDeath(PlayerControl killer = null);
        public abstract void HandleDisconnect(PlayerControl player, DisconnectReasons reason);
        public virtual void ResetRole() { }

        public static void ClearAll()
        {
            allRoles = new List<Role>();
        }
    }

    [HarmonyPatch]
    public abstract class RoleBase<T> : Role where T : RoleBase<T>, new()
    {
        public static List<T> players = new List<T>();
        public static RoleId RoleType;

        public void Init(PlayerControl player)
        {
            this.player = player;
            players.Add((T)this);
            allRoles.Add(this);
        }

        public static T local
        {
            get
            {
                return players.FirstOrDefault(x => x.player == PlayerControl.LocalPlayer);
            }
        }

        public static List<PlayerControl> allPlayers
        {
            get
            {
                return players.Select(x => x.player).ToList();
            }
        }

        public static List<PlayerControl> livingPlayers
        {
            get
            {
                return players.Select(x => x.player).Where(x => x.isAlive()).ToList();
            }
        }

        public static List<PlayerControl> deadPlayers
        {
            get
            {
                return players.Select(x => x.player).Where(x => !x.isAlive()).ToList();
            }
        }

        public static bool exists
        {
            get { return players.Count > 0; }
        }

        public static T getRole(PlayerControl player = null)
        {
            player = player ?? PlayerControl.LocalPlayer;
            return players.FirstOrDefault(x => x.player == player);
        }

        public static bool isRole(PlayerControl player)
        {
            return players.Any(x => x.player == player);
        }

        public static void setRole(PlayerControl player)
        {
            if (!isRole(player))
            {
                T role = new T();
                role.Init(player);
            }
        }

        public static void eraseRole(PlayerControl player)
        {
            players.DoIf(x => x.player == player, x => x.ResetRole());
            players.RemoveAll(x => x.player == player && x.roleId == RoleType);
            allRoles.RemoveAll(x => x.player == player && x.roleId == RoleType);
        }

        public static void swapRole(PlayerControl p1, PlayerControl p2)
        {
            var index = players.FindIndex(x => x.player == p1);
            if (index >= 0)
            {
                players[index].player = p2;
            }
        }
    }

    public static class RoleHelpers
    {
        public static bool isRole(this PlayerControl player, RoleId role)
        {
            foreach (var t in RoleData.allRoleTypes)
            {
                if (role == t.Value)
                {
                    return (bool)t.Key.GetMethod("isRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { player });
                }
            }

            switch (role)
            {
                case RoleId.Jester:
                    return Jester.jester == player;
                case RoleId.Mayor:
                    return Mayor.mayor == player;
                case RoleId.Engineer:
                    return Engineer.engineer == player;
                case RoleId.Godfather:
                    return Godfather.godfather == player;
                case RoleId.Mafioso:
                    return Mafioso.mafioso == player;
                case RoleId.Janitor:
                    return Janitor.janitor == player;
                case RoleId.Detective:
                    return Detective.detective == player;
                case RoleId.TimeMaster:
                    return TimeMaster.timeMaster == player;
                case RoleId.Medic:
                    return Medic.medic == player;
                case RoleId.Shifter:
                    return Shifter.shifter == player;
                case RoleId.Swapper:
                    return Swapper.swapper == player;
                case RoleId.Seer:
                    return Seer.seer == player;
                case RoleId.Morphling:
                    return Morphling.morphling == player;
                case RoleId.Camouflager:
                    return Camouflager.camouflager == player;
                case RoleId.Hacker:
                    return Hacker.hacker == player;
                case RoleId.Mini:
                    return Mini.mini == player;
                case RoleId.Tracker:
                    return Tracker.tracker == player;
                case RoleId.Vampire:
                    return Vampire.vampire == player;
                case RoleId.Snitch:
                    return Snitch.snitch == player;
                case RoleId.Jackal:
                    return Jackal.jackal == player;
                case RoleId.Sidekick:
                    return Sidekick.sidekick == player;
                case RoleId.Eraser:
                    return Eraser.eraser == player;
                case RoleId.Spy:
                    return Spy.spy == player;
                case RoleId.Trickster:
                    return Trickster.trickster == player;
                case RoleId.Cleaner:
                    return Cleaner.cleaner == player;
                case RoleId.Warlock:
                    return Warlock.warlock == player;
                case RoleId.SecurityGuard:
                    return SecurityGuard.securityGuard == player;
                case RoleId.Arsonist:
                    return Arsonist.arsonist == player;
                case RoleId.EvilGuesser:
                    return Guesser.evilGuesser == player;
                case RoleId.NiceGuesser:
                    return Guesser.niceGuesser == player;
                case RoleId.BountyHunter:
                    return BountyHunter.bountyHunter == player;
                case RoleId.Bait:
                    return Bait.bait == player;
                case RoleId.GM:
                    return GM.gm == player;
                case RoleId.Vulture:
                    return Vulture.vulture == player;
                case RoleId.Medium:
                    return Medium.medium == player;
                case RoleId.Witch:
                    return Witch.witch == player;
                case RoleId.Lawyer:
                    return Lawyer.lawyer == player;
                case RoleId.Pursuer:
                    return Pursuer.pursuer == player;
                default:
                    TheOtherRolesPlugin.Logger.LogError($"isRole: no method found for role type {role}");
                    return false;
            }
        }

        public static void setRole(this PlayerControl player, RoleId role)
        {
            foreach (var t in RoleData.allRoleTypes)
            {
                if (role == t.Value)
                {
                    t.Key.GetMethod("setRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { player });
                    return;
                }
            }

            switch (role)
            {
                case RoleId.Jester:
                    Jester.jester = player;
                    break;
                case RoleId.Mayor:
                    Mayor.mayor = player;
                    break;
                case RoleId.Engineer:
                    Engineer.engineer = player;
                    break;
                case RoleId.Godfather:
                    Godfather.godfather = player;
                    break;
                case RoleId.Mafioso:
                    Mafioso.mafioso = player;
                    break;
                case RoleId.Janitor:
                    Janitor.janitor = player;
                    break;
                case RoleId.Detective:
                    Detective.detective = player;
                    break;
                case RoleId.TimeMaster:
                    TimeMaster.timeMaster = player;
                    break;
                case RoleId.Medic:
                    Medic.medic = player;
                    break;
                case RoleId.Shifter:
                    Shifter.shifter = player;
                    break;
                case RoleId.Swapper:
                    Swapper.swapper = player;
                    break;
                case RoleId.Seer:
                    Seer.seer = player;
                    break;
                case RoleId.Morphling:
                    Morphling.morphling = player;
                    break;
                case RoleId.Camouflager:
                    Camouflager.camouflager = player;
                    break;
                case RoleId.Hacker:
                    Hacker.hacker = player;
                    break;
                case RoleId.Mini:
                    Mini.mini = player;
                    break;
                case RoleId.Tracker:
                    Tracker.tracker = player;
                    break;
                case RoleId.Vampire:
                    Vampire.vampire = player;
                    break;
                case RoleId.Snitch:
                    Snitch.snitch = player;
                    break;
                case RoleId.Jackal:
                    Jackal.jackal = player;
                    break;
                case RoleId.Sidekick:
                    Sidekick.sidekick = player;
                    break;
                case RoleId.Eraser:
                    Eraser.eraser = player;
                    break;
                case RoleId.Spy:
                    Spy.spy = player;
                    break;
                case RoleId.Trickster:
                    Trickster.trickster = player;
                    break;
                case RoleId.Cleaner:
                    Cleaner.cleaner = player;
                    break;
                case RoleId.Warlock:
                    Warlock.warlock = player;
                    break;
                case RoleId.SecurityGuard:
                    SecurityGuard.securityGuard = player;
                    break;
                case RoleId.Arsonist:
                    Arsonist.arsonist = player;
                    break;
                case RoleId.EvilGuesser:
                    Guesser.evilGuesser = player;
                    break;
                case RoleId.NiceGuesser:
                    Guesser.niceGuesser = player;
                    break;
                case RoleId.BountyHunter:
                    BountyHunter.bountyHunter = player;
                    break;
                case RoleId.Bait:
                    Bait.bait = player;
                    break;
                case RoleId.GM:
                    GM.gm = player;
                    break;
                case RoleId.Vulture:
                    Vulture.vulture = player;
                    break;
                case RoleId.Medium:
                    Medium.medium = player;
                    break;
                case RoleId.Witch:
                    Witch.witch = player;
                    break;
                case RoleId.Lawyer:
                    Lawyer.lawyer = player;
                    break;
                case RoleId.Pursuer:
                    Pursuer.pursuer = player;
                    break;
                default:
                    TheOtherRolesPlugin.Logger.LogError($"setRole: no method found for role type {role}");
                    return;
            }
        }

        public static void eraseRole(this PlayerControl player, RoleId role)
        {
            if (isRole(player, role))
            {
                foreach (var t in RoleData.allRoleTypes)
                {
                    if (role == t.Value)
                    {
                        t.Key.GetMethod("eraseRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { player });
                        return;
                    }
                }
                TheOtherRolesPlugin.Logger.LogError($"eraseRole: no method found for role type {role}");
            }
        }

        public static void eraseAllRoles(this PlayerControl player)
        {
            foreach (var t in RoleData.allRoleTypes)
            {
                t.Key.GetMethod("eraseRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { player });
            }

            // Crewmate roles
            if (player.isRole(RoleId.Mayor)) Mayor.clearAndReload();
            if (player.isRole(RoleId.Engineer)) Engineer.clearAndReload();
            if (player.isRole(RoleId.Detective)) Detective.clearAndReload();
            if (player.isRole(RoleId.TimeMaster)) TimeMaster.clearAndReload();
            if (player.isRole(RoleId.Medic)) Medic.clearAndReload();
            if (player.isRole(RoleId.Shifter)) Shifter.clearAndReload();
            if (player.isRole(RoleId.Seer)) Seer.clearAndReload();
            if (player.isRole(RoleId.Hacker)) Hacker.clearAndReload();
            if (player.isRole(RoleId.Mini)) Mini.clearAndReload();
            if (player.isRole(RoleId.Tracker)) Tracker.clearAndReload();
            if (player.isRole(RoleId.Snitch)) Snitch.clearAndReload();
            if (player.isRole(RoleId.Swapper)) Swapper.clearAndReload();
            if (player.isRole(RoleId.Spy)) Spy.clearAndReload();
            if (player.isRole(RoleId.SecurityGuard)) SecurityGuard.clearAndReload();
            if (player.isRole(RoleId.Bait)) Bait.clearAndReload();
            if (player.isRole(RoleId.Medium)) Medium.clearAndReload();

            // Impostor roles
            if (player.isRole(RoleId.Morphling)) Morphling.clearAndReload();
            if (player.isRole(RoleId.Camouflager)) Camouflager.clearAndReload();
            if (player.isRole(RoleId.Godfather)) Godfather.clearAndReload();
            if (player.isRole(RoleId.Mafioso)) Mafioso.clearAndReload();
            if (player.isRole(RoleId.Janitor)) Janitor.clearAndReload();
            if (player.isRole(RoleId.Vampire)) Vampire.clearAndReload();
            if (player.isRole(RoleId.Eraser)) Eraser.clearAndReload();
            if (player.isRole(RoleId.Trickster)) Trickster.clearAndReload();
            if (player.isRole(RoleId.Cleaner)) Cleaner.clearAndReload();
            if (player.isRole(RoleId.Warlock)) Warlock.clearAndReload();
            if (player.isRole(RoleId.Witch)) Witch.clearAndReload();

            // Other roles
            if (player.isRole(RoleId.Jester)) Jester.clearAndReload();
            if (player.isRole(RoleId.Arsonist)) Arsonist.clearAndReload();
            if (player.isRole(RoleId.Sidekick)) Sidekick.clearAndReload();
            if (player.isRole(RoleId.BountyHunter)) BountyHunter.clearAndReload();
            if (player.isRole(RoleId.Vulture)) Vulture.clearAndReload();
            if (player.isRole(RoleId.Lawyer)) Lawyer.clearAndReload();
            if (player.isRole(RoleId.Pursuer)) Pursuer.clearAndReload();
            if (Guesser.isGuesser(player.PlayerId)) Guesser.clear(player.PlayerId);


            if (player.isRole(RoleId.Jackal))
            { // Promote Sidekick and hence override the the Jackal or erase Jackal
                if (Sidekick.promotesToJackal && Sidekick.sidekick != null && Sidekick.sidekick.isAlive())
                {
                    RPCProcedure.sidekickPromotes();
                }
                else
                {
                    Jackal.clearAndReload();
                }
            }
        }

        public static void OnKill(this PlayerControl player, PlayerControl target)
        {
            foreach (var r in Role.allRoles)
            {
                if (r.player == player)
                {
                    r.OnKill(target);
                }
            }
        }

        public static void OnDeath(this PlayerControl player, PlayerControl killer)
        {
            foreach (var r in Role.allRoles)
            {
                if (r.player == player)
                {
                    r.OnDeath(killer);
                }
            }

            // Lover suicide trigger on exile/death
            if (player.isLovers())
                Lovers.killLovers(player, killer);
        }
    }
}