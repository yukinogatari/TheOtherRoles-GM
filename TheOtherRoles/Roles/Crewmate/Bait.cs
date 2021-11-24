using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Bait : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Bait;

        public static CustomOptionBlank options;
        public static CustomOption baitHighlightAllVents;
        public static CustomOption baitReportDelay;

        public static bool highlightAllVents { get { return baitHighlightAllVents.getBool(); } }
        public static float reportDelay { get { return baitReportDelay.getFloat(); } }

        public bool reported = false;
        public float timeSinceKilled = 0f;

        public Bait() : base()
        {
            NameColor = RoleColors.Bait;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public override void _RoleUpdate()
        {
            base._RoleUpdate();

            // Bait report
            if (Player.Data.IsDead && !reported)
            {
                timeSinceKilled += Time.fixedDeltaTime;
                DeadPlayer deadPlayer = GameHistory.deadPlayers?.Where(x => x.player?.PlayerId == Player.PlayerId)?.FirstOrDefault();
                if (deadPlayer.killerIfExisting != null && timeSinceKilled >= reportDelay)
                {

                    Helpers.handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                    RPCProcedure.uncheckedCmdReportDeadBody(deadPlayer.killerIfExisting.PlayerId, Player.PlayerId);

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
                    writer.Write(deadPlayer.killerIfExisting.PlayerId);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    reported = true;
                }
            }

            // Bait Vents
            if (ShipStatus.Instance?.AllVents != null)
            {
                var ventsWithPlayers = new List<int>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.inVent)
                    {
                        Vent target = ShipStatus.Instance.AllVents.OrderBy(x => Vector2.Distance(x.transform.position, player.GetTruePosition())).FirstOrDefault();
                        if (target != null) ventsWithPlayers.Add(target.Id);
                    }
                }

                foreach (Vent vent in ShipStatus.Instance.AllVents)
                {
                    if (vent.myRend == null || vent.myRend.material == null) continue;
                    if (ventsWithPlayers.Contains(vent.Id) || (ventsWithPlayers.Count > 0 && Bait.highlightAllVents))
                    {
                        vent.myRend.material.SetFloat("_Outline", 1f);
                        vent.myRend.material.SetColor("_OutlineColor", Color.yellow);
                    }
                    else
                    {
                        vent.myRend.material.SetFloat("_Outline", 0);
                    }
                }
            }
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            baitHighlightAllVents = CustomOption.Create(331, "baitHighlightAllVents", false, options);
            baitReportDelay = CustomOption.Create(332, "baitReportDelay", 0f, 0f, 10f, 1f, options, format: "unitSeconds");
        }
    }
}