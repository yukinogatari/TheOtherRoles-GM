using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using System;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Madmate : ModifierBase<Madmate>
    {
        public static Color color = Palette.ImpostorRed;

        public enum MadmateType
        {
            Simple = 0,
            WithRole = 1,
            Random = 2,
        }

        public enum MadmateAbility
        {
            None = 0,
            Fanatic = 1,
            Kuroneko = 2,
            Obstructor = 3,
        }

        public static bool canEnterVents { get { return CustomOptionHolder.madmateCanEnterVents.getBool(); } }
        public static bool hasImpostorVision { get { return CustomOptionHolder.madmateHasImpostorVision.getBool(); } }
        public static bool canSabotage { get { return CustomOptionHolder.madmateCanSabotage.getBool(); } }
        public static bool canFixComm { get { return CustomOptionHolder.madmateCanFixComm.getBool(); } }

        public static MadmateType madmateType { get { return (MadmateType)CustomOptionHolder.madmateType.getSelection(); } }
        public static MadmateAbility madmateAbility { get { return (MadmateAbility)CustomOptionHolder.madmateAbility.getSelection(); } }
        public static RoleType fixedRole { get { return CustomOptionHolder.madmateFixedRole.role; } }

        public static int numCommonTasks { get { return CustomOptionHolder.madmateTasks.commonTasks; } }
        public static int numLongTasks { get { return CustomOptionHolder.madmateTasks.longTasks; } }
        public static int numShortTasks { get { return CustomOptionHolder.madmateTasks.shortTasks; } }

        public static bool hasTasks { get { return madmateAbility == MadmateAbility.Fanatic; } }

        public static string prefix
        {
            get
            {
                return ModTranslation.getString("madmatePrefix");
            }
        }

        public static string fullName
        {
            get
            {
                return ModTranslation.getString("madmate");
            }
        }

        public static List<RoleType> validRoles = new List<RoleType>
        {
            RoleType.NoRole, // NoRole = off
            RoleType.Shifter,
            RoleType.Mayor,
            RoleType.Engineer,
            RoleType.Sheriff,
            RoleType.Lighter,
            RoleType.Detective,
            RoleType.TimeMaster,
            RoleType.Medic,
            RoleType.Swapper,
            RoleType.Seer,
            RoleType.Hacker,
            RoleType.Tracker,
            RoleType.SecurityGuard,
            RoleType.Bait,
            RoleType.Medium,
            RoleType.Mini,
            RoleType.NiceGuesser,
            RoleType.Watcher,
            RoleType.Sprinter,
        };

        public static List<PlayerControl> candidates
        {
            get
            {
                List<PlayerControl> crewHasRole = new List<PlayerControl>();
                List<PlayerControl> crewNoRole = new List<PlayerControl>();
                List<PlayerControl> validCrewmates = new List<PlayerControl>();

                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.isCrew() && !hasModifier(x)).ToList())
                {
                    var info = RoleInfo.getRoleInfoForPlayer(player, includeHidden: true);
                    if (info.Contains(RoleInfo.crewmate))
                    {
                        crewNoRole.Add(player);
                        validCrewmates.Add(player);
                    }
                    else if (info.Any(x => validRoles.Contains(x.roleType)))
                    {
                        if (fixedRole == RoleType.NoRole || info.Any(x => x.roleType == fixedRole))
                            crewHasRole.Add(player);

                        validCrewmates.Add(player);
                    }
                }

                if (madmateType == MadmateType.Simple) return crewNoRole;
                else if (madmateType == MadmateType.WithRole && crewHasRole.Count > 0) return crewHasRole;
                else if (madmateType == MadmateType.Random) return validCrewmates;
                return validCrewmates;
            }
        }

        public Madmate()
        {
            ModType = modId = ModifierType.Madmate;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }

        public override void OnDeath(PlayerControl killer = null)
        {
            player.clearAllTasks();
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void MakeButtons(HudManager hm) { }
        public static void SetButtonCooldowns() { }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch
        {
            public static void Postfix(ShipStatus __instance)
            {
                if (hasTasks && hasModifier(PlayerControl.LocalPlayer))
                {
                    local.assignTasks();
                }
            }
        }

        public void assignTasks()
        {
            player.generateAndAssignTasks(numCommonTasks, numShortTasks, numLongTasks);
        }

        public static bool knowsImpostors(PlayerControl player)
        {
            return hasTasks && hasModifier(player) && tasksComplete(player);
        }

        public static bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = numCommonTasks + numLongTasks + numShortTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter >= totalTasks;
        }

        public override string modifyNameText(string nameText)
        {
            return nameText;
        }

        public override string modifyRoleText(string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false)
        {
            if (roleInfo.Contains(RoleInfo.crewmate))
            {
                roleText = useColors ? Helpers.cs(Madmate.color, Madmate.fullName) : Madmate.fullName;
            }
            else
            {
                string prefix = useColors ? Helpers.cs(Madmate.color, Madmate.prefix) : Madmate.prefix;
                roleText = String.Join(" ", roleInfo.Select(x => useColors ? Helpers.cs(Madmate.color, x.name) : x.name).ToArray());
                roleText = prefix + roleText;
            }
            return roleText;
        }

        public static void Clear()
        {
            players = new List<Madmate>();
        }
    }
}