using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    enum RoleModifierTypes
    {
        Lovers = 0,

        None = int.MaxValue,
    }

    [Harmony]
    abstract class RoleModifier
    {
        public static List<Type> allTypes = new List<Type> {
            typeof(Lovers),
        };

        public static List<RoleModifier> playerModifiers = new List<RoleModifier>();

        public Color color = Palette.White;
        public RoleModifierTypes type = RoleModifierTypes.None;
        public int playerId;

        public RoleModifier(int playerId) {
            this.playerId = playerId;
            RoleModifier.playerModifiers.Add(this);
        }

        public abstract bool didWin(GameOverReason gameOverReason);

        public virtual void OnDeath() { }

        public virtual void _HandleDisconnect(PlayerControl pc, DisconnectReasons reason) { }

        public static void HandleDisconnect(PlayerControl pc, DisconnectReasons reason)
        {
            foreach (RoleModifier mod in playerModifiers)
                mod._HandleDisconnect(pc, reason);

            clearModifiers(pc.PlayerId);
        }

        public static List<RoleModifier> getModifiers(int playerId)
        {
            return playerModifiers.FindAll(x => x.playerId == playerId);
        }

        public static void clearModifiers(int playerId)
        {
            playerModifiers.RemoveAll(x => x.playerId == playerId);
        }

        public static void clearModifiers()
        {
            playerModifiers.Clear();
        }

        public static void InitializeAll()
        {
            foreach (Type t in allTypes)
            {
                t.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
            }
        }
    }

    static class ModifierExtensions
    {
        public static RoleModifier addModifier(this PlayerControl pc, RoleModifierTypes m, dynamic? extraData = null)
        {
            RoleModifier mod = null;
            switch(m)
            {
                case RoleModifierTypes.Lovers:
                    mod = new Lovers(pc.PlayerId, extraData);
                    break;

                default:
                    break;
            }

            return mod;
        }

        public static bool hasModifier(this PlayerControl pc, RoleModifierTypes mod)
        {
            return pc.getModifiers().Any(x => x.type == mod);
        }

        public static RoleModifier getModifier(this PlayerControl pc, RoleModifierTypes mod)
        {
            return pc.getModifiers().FirstOrDefault(x => x.type == mod);
        }

        public static List<RoleModifier> getModifiers(this PlayerControl pc)
        {
            return RoleModifier.getModifiers(pc.PlayerId);
        }

        // TODO: IMPLEMENT THIS
        public static void RpcSetRoleModifier(this PlayerControl pc, RoleModifierTypes mod) { }
    }
}