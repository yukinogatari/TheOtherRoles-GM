using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using Hazel;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class NekoKabocha : RoleBase<NekoKabocha>
    {
        public static Color color = Palette.ImpostorRed;

        public static bool revengeCrew { get { return CustomOptionHolder.nekoKabochaRevengeCrew.getBool(); } }
        public static bool revengeNeutral { get { return CustomOptionHolder.nekoKabochaRevengeNeutral.getBool(); } }
        public static bool revengeImpostor { get { return CustomOptionHolder.nekoKabochaRevengeImpostor.getBool(); } }
        public static bool revengeExile { get { return CustomOptionHolder.nekoKabochaRevengeExile.getBool(); } }

        public NekoKabocha()
        {
            RoleType = roleId = RoleType.NekoKabocha;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }
        
        public override void OnDeath(PlayerControl killer = null)
        {
            if (killer != null && killer.isAlive())
            {
                if ((revengeCrew && killer.isCrew()) ||
                    (revengeNeutral && killer.isNeutral()) ||
                    (revengeImpostor && killer.isImpostor()))
                {
                    player.MurderPlayer(killer);
                    finalStatuses[killer.PlayerId] = FinalStatus.Revenge;
                }
            }
            else if (killer == null && revengeExile && PlayerControl.LocalPlayer == player)
            {
                var candidates = PlayerControl.AllPlayerControls.ToArray().Where(x => x != player && x.isAlive()).ToList();
                int targetID = rnd.Next(0, candidates.Count);
                var target = candidates[targetID];

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NekoKabochaExile, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.nekoKabochaExile(target.PlayerId);
            }
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void MakeButtons(HudManager hm) { }
        public static void SetButtonCooldowns() { }

        public static void clearAndReload()
        {
            players = new List<NekoKabocha>();
        }
    }
}