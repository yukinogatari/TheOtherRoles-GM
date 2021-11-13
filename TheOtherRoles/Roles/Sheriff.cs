using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Sheriff : CustomCrewRole
    {
        public static Color color = new Color32(248, 205, 70, byte.MaxValue);
        public static RoleTypes RoleType = (RoleTypes)CustomRoleTypes.Sheriff;

        public Sheriff() : base()
        {
            Role = RoleType;
            NameColor = color;
            TasksCountTowardProgress = true;
            CanUseKillButton = true;
            CanBeKilled = true;
            CanVent = true;
            AffectedByLightAffectors = true;
            MaxCount = 15;
            TeamType = RoleTeamTypes.Crewmate;
            Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void Initialize()
        {
        }

        // public virtual bool CanUse(IUsable console) => default;
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.CanUse))]
        public static class CanUsePatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref IUsable console, ref bool __result)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void Deinitialize(PlayerControl targetPlayer) { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.Deinitialize))]
        public static class DeinitializePatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref PlayerControl targetPlayer)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void SpawnTaskHeader(PlayerControl playerControl) { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.SpawnTaskHeader))]
        public static class SpawnTaskHeaderPatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref PlayerControl targetPlayer)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void UseAbility() { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.UseAbility))]
        public static class UseAbilityPatch
        {
            public static bool Prefix(RoleBehaviour __instance)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void OnMeetingStart() { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.OnMeetingStart))]
        public static class OnMeetingStartPatch
        {
            public static bool Prefix(RoleBehaviour __instance)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void OnVotingComplete() { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.OnVotingComplete))]
        public static class OnVotingCompletePatch
        {
            public static bool Prefix(RoleBehaviour __instance)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void Initialize(PlayerControl player) { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.Initialize))]
        public static class InitializePatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref PlayerControl targetPlayer)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void SetUsableTarget(IUsable target) { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.SetUsableTarget))]
        public static class SetUsableTargetPatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref IUsable target)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void SetPlayerTarget(PlayerControl target) { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.SetPlayerTarget))]
        public static class SetPlayerTargetPatch
        {
            public static bool Prefix(RoleBehaviour __instance, ref PlayerControl target)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

        // public virtual void SetCooldown() { }
        [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.SetCooldown))]
        public static class SetCooldownPatch
        {
            public static bool Prefix(RoleBehaviour __instance)
            {
                if (__instance.Role == RoleType)
                {

                }
                return true;
            }
        }

    }
}