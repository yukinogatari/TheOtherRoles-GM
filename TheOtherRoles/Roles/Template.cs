using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Template : RoleBase<Template>
    {
        public static Color color = Palette.CrewmateBlue;

        public Template()
        {
            RoleType = roleId = RoleId.NoRole;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill() { }
        public override void OnDeath() { }

        public static void MakeButtons(HudManager hm) { }
        public static void SetButtonCooldowns() { }

        public static void clearAndReload()
        {
            players = new List<Template>();
        }
    }
}