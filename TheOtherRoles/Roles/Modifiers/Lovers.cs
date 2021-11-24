using HarmonyLib;
using System.Linq;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    // Just so we can put this on the options menu.
    // The actual body of the Lovers functionality is in the role modifier.
    class Lovers : CustomRole
    {
        public static CustomOptionBlank options;
        public static CustomOption loversImpLoverRate;
        public static CustomOption loversBothDie;
        public static CustomOption loversCanHaveAnotherRole;
        public static CustomOption loversSeparateTeam;
        public static CustomOption loversTasksCount;

        public static bool bothDie { get { return loversBothDie.getBool(); } }

        // Making this closer to the au.libhalt.net version of Lovers
        public static bool separateTeam { get { return loversSeparateTeam.getBool(); } }
        public static bool tasksCount { get { return loversTasksCount.getBool(); } }

        public Lovers() : base()
        {
            // max 15 players = max 7 pairs
            MaxCount = 7;
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            loversImpLoverRate = CustomOption.Create(51, "loversImpLoverRate", CustomOptionHolder.rates, options);
            loversBothDie = CustomOption.Create(52, "loversBothDie", true, options);
            loversCanHaveAnotherRole = CustomOption.Create(53, "loversCanHaveAnotherRole", true, options);
            loversSeparateTeam = CustomOption.Create(56, "loversSeparateTeam", true, options);
            loversTasksCount = CustomOption.Create(55, "loversTasksCount", false, loversSeparateTeam);
        }
    }

    class LoversMod : RoleModifier
    {
        public PlayerControl Partner;
        public LoversMod partnerMod;

        // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
        public bool notAckedExiledIsLover = false;

        public LoversMod(PlayerControl? partner = null) : base()
        {
            color = RoleColors.Lovers;
            type = RoleModifierTypes.Lovers;

            if (partner != null)
            {
                this.Partner = partner;
                this.partnerMod = (LoversMod)partner.addModifier(RoleModifierTypes.Lovers);
                this.partnerMod.partnerMod = this;
                this.partnerMod.Partner = this.Player;
            }
        }

        public static bool hasTasks
        {
            get
            {
                return !Lovers.separateTeam || Lovers.tasksCount;
            }
        }

        public override void OnKilled() {
            if (Lovers.bothDie && !Partner.Data.IsDead)
            {
                if (GameHistory.exiledPlayers.Contains(Player.PlayerId)) Partner.Exiled();
                else Partner.MurderPlayer(Partner);
                GameHistory.suicidedPlayers.Add(Partner.PlayerId);
            }

            if (Lovers.separateTeam && Lovers.tasksCount)
                Player.clearAllTasks();
        }

        public override void OnMeetingEnd()
        {
            base.OnMeetingEnd();
            notAckedExiledIsLover = false;
        }

        public override void _HandleDisconnect(PlayerControl pc, DisconnectReasons reason) {
            if (pc.PlayerId == Player.PlayerId || pc.PlayerId == Partner.PlayerId)
            {
                Player.removeModifier(RoleModifierTypes.Lovers);
                Partner.removeModifier(RoleModifierTypes.Lovers);
            }
        }

        public bool bothAlive()
        {
            return !Player.Data.IsDead && !Partner.Data.IsDead && !Player.Data.Disconnected && !Partner.Data.Disconnected;
        }

        public bool killerPair()
        {
            var pcRole = Player.role();
            var ptRole = Partner.role();

            var killingTeams = new[] {
                RoleTeamTypes.Impostor,
                (RoleTeamTypes)CustomRoleTeamTypes.Jackal,
            };

            return killingTeams.Contains(pcRole.TeamType) || killingTeams.Contains(ptRole.TeamType);
        }

        public override bool DidWin(GameOverReason gameOverReason)
        {
            if (Lovers.separateTeam)
            {
                return gameOverReason != GameOverReason.HumansByTask && bothAlive();
            }
            else
            {
                return !killerPair() && !Lovers.separateTeam &&
                    (gameOverReason == GameOverReason.HumansByTask || gameOverReason == GameOverReason.HumansByVote);
            }
        }
    }

    static class LoversExtensions
    {
        public static bool isLovers(this PlayerControl player)
        {
            return player != null && player.hasModifier(RoleModifierTypes.Lovers);
        }

        public static PlayerControl getPartner(this PlayerControl player)
        {
            if (player == null)
                return null;

            return ((LoversMod)player.getModifier(RoleModifierTypes.Lovers))?.Partner;
        }

        public static bool hasAliveKillingLover(this PlayerControl player)
        {
            LoversMod mod = (LoversMod)player.getModifier(RoleModifierTypes.Lovers);
            return player.isLovers() && mod != null && mod.bothAlive() && mod.killerPair();
        }

        public static bool loversBothAlive(this PlayerControl player)
        {
            LoversMod mod = (LoversMod)player.getModifier(RoleModifierTypes.Lovers);
            return player.isLovers() && mod != null && mod.bothAlive();
        }

        // TODO: IMPLEMENT THIS RPC PROPERLY
        public static void RpcSetLoversModifier(this PlayerControl pc, PlayerControl partner) {
            pc.SetLoversModifier(partner);
        }

        public static void SetLoversModifier(this PlayerControl pc, PlayerControl partner)
        {
            pc.addModifier(RoleModifierTypes.Lovers, partner);
        }
    }
}