using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public enum RoleModifierTypes
    {
        Lovers = 0,

        MedicShield = 100,
        LighterLight = 101,

        None = int.MaxValue,
    }

    [Harmony]
    public abstract class RoleModifier
    {
        public Color color = Palette.White;
        public RoleModifierTypes type = RoleModifierTypes.None;
        public PlayerControl Player;

        public RoleModifier() {
        }

        public virtual void Init(dynamic? extraData = null) { }

        public virtual bool DidWin(GameOverReason gameOverReason) { return false; }

        public virtual void OnKill() { }
        public virtual void OnKilled() { }
        public virtual void OnExiled() { }
        public virtual void OnMeeting() { }
        public virtual void OnMeetingEnd() { }

        public virtual void _HandleDisconnect(PlayerControl pc, DisconnectReasons reason) { }
    }

    public class GenericModifier : RoleModifier
    {

    }

    static class ModifierExtensions
    {
        public static RoleModifier addModifier(this PlayerControl pc, RoleModifierTypes type)
        {
            RoleModifier mod = CustomRoleManager.CreateMod(type, pc);
            addModifier(pc, mod);
            return mod;
        }

        public static void addModifier(this PlayerControl pc, RoleModifier mod)
        {
            if (!CustomRoleManager.allModifiers.ContainsKey(pc.PlayerId))
            {
                CustomRoleManager.allModifiers[pc.PlayerId] = new List<RoleModifier>();
            }
            CustomRoleManager.allModifiers[pc.PlayerId].Add(mod);
        }

        public static void removeModifier(this PlayerControl pc, RoleModifierTypes type)
        {
            if (CustomRoleManager.allModifiers.ContainsKey(pc.PlayerId))
            {
                CustomRoleManager.allModifiers[pc.PlayerId].RemoveAll(x => x.type == type);
            }
        }

        public static RoleModifier getModifier(this PlayerControl pc, RoleModifierTypes mod)
        {
            return pc.getModifiers()?.FirstOrDefault(x => x.type == mod);
        }

        public static List<RoleModifier> getModifiers(this PlayerControl pc)
        {
            return CustomRoleManager.allModifiers.ContainsKey(pc.PlayerId) ? CustomRoleManager.allModifiers[pc.PlayerId] : new List<RoleModifier>();
        }

        public static void clearModifiers(this PlayerControl pc)
        {
            CustomRoleManager.allModifiers.Remove(pc.PlayerId);
        }

        public static void clearModifiers()
        {
            CustomRoleManager.allModifiers.Clear();
        }

        public static RoleModifier addModifier(this PlayerControl pc, RoleModifierTypes m, dynamic? extraData = null)
        {
            RoleModifier mod = addModifier(pc, m);
            mod.Init(extraData);
            return mod;
        }

        public static bool hasModifier(this PlayerControl pc, RoleModifierTypes mod)
        {
            return pc.getModifiers()?.Any(x => x.type == mod) == true;
        }

        public static RoleModifier RpcSetRoleModifier(this PlayerControl pc, RoleModifierTypes mod)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRoleModifier, Hazel.SendOption.Reliable, -1);
            writer.Write(pc.PlayerId);
            writer.Write((int)mod);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return pc.addModifier(mod);
        }

        public static void RpcRemoveRoleModifier(this PlayerControl pc, RoleModifierTypes mod)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RemoveRoleModifier, Hazel.SendOption.Reliable, -1);
            writer.Write(pc.PlayerId);
            writer.Write((int)mod);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            pc.removeModifier(mod);
        }
    }
}