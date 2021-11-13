

using System.Linq;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    class Lovers : RoleModifier
    {
        public static bool bothDie = true;

        // Making this closer to the au.libhalt.net version of Lovers
        public static bool separateTeam = true;
        public static bool tasksCount = false;

        public int partnerId;
        public PlayerControl player;
        public PlayerControl partner;
        public Lovers partnerMod;

        // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
        public bool notAckedExiledIsLover = false;

        public Lovers(int playerId, int? partnerId = null) : base(playerId)
        {
            color = new Color32(232, 57, 185, byte.MaxValue);
            type = RoleModifierTypes.Lovers;

            if (partnerId != null)
            {
                this.partnerId = (int)partnerId;

                this.player = Helpers.playerById(playerId);
                this.partner = Helpers.playerById(this.partnerId);

                this.partnerMod = new Lovers(this.partnerId);
                this.partnerMod.partnerMod = this;
                this.partnerMod.partner = this.player;
            }
        }
        public static bool hasTasks
        {
            get
            {
                return !separateTeam || tasksCount;
            }
        }

        public override void OnDeath() {
            if (Lovers.bothDie && !partner.Data.IsDead)
            {
                if (exiledPlayers.Contains(playerId)) partner.Exiled();
                else partner.MurderPlayer(partner);
                suicidedPlayers.Add(partnerId);
            }

            if (Lovers.separateTeam && Lovers.tasksCount)
                player.clearAllTasks();
        }

        public override void _HandleDisconnect(PlayerControl pc, DisconnectReasons reason) {
            if (pc.PlayerId == playerId || pc.PlayerId == partnerId)
            {
                RoleModifier.playerModifiers.RemoveAll(x => x.type == RoleModifierTypes.Lovers && (x.playerId == playerId || x.playerId == partnerId));
            }
        }

        public bool bothAlive()
        {
            return !player.Data.IsDead && !partner.Data.IsDead && !player.Data.Disconnected && !partner.Data.Disconnected;
        }

        public bool killerPair()
        {
            var pcRole = player.Data.Role;
            var ptRole = partner.Data.Role;

            var killingTeams = new[] {
                RoleTeamTypes.Impostor,
                (RoleTeamTypes)CustomRoleTeamTypes.Jackal,
            };

            return killingTeams.Contains(pcRole.TeamType) || killingTeams.Contains(ptRole.TeamType);
        }

        public override bool didWin(GameOverReason gameOverReason)
        {
            if (Lovers.separateTeam)
            {

            }
            return true;
        }

        public static void Initialize()
        {
            bothDie = CustomOptionHolder.loversBothDie.getBool();
            separateTeam = CustomOptionHolder.loversSeparateTeam.getBool();
            tasksCount = CustomOptionHolder.loversTasksCount.getBool();
        }
    }

    static class LoversExtensions
    {
        public static bool isLovers(this PlayerControl player)
        {
            return player != null && player.hasModifier(RoleModifierTypes.Lovers);
        }

        public static bool hasAliveKillingLover(this PlayerControl player)
        {
            Lovers mod = (Lovers)player.getModifier(RoleModifierTypes.Lovers);
            return player.isLovers() && mod != null && mod.bothAlive() && mod.killerPair();
        }

        // TODO: IMPLEMENT THIS RPC PROPERLY
        public static void RpcSetLoversModifier(this PlayerControl pc, PlayerControl partner) {
            pc.SetLoversModifier(partner.PlayerId);
        }

        public static void SetLoversModifier(this PlayerControl pc, int partnerId)
        {
            pc.addModifier(RoleModifierTypes.Lovers, partnerId);
        }
    }
}