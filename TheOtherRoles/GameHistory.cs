using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles {
    public class DeadPlayer
    {
        public PlayerControl player;
        public DateTime timeOfDeath;
        public DeathReason deathReason;
        public PlayerControl killerIfExisting;

        public DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting) {
            this.player = player;
            this.timeOfDeath = timeOfDeath;
            this.deathReason = deathReason;
            this.killerIfExisting = killerIfExisting;
        }
    }

    static class GameHistory {
        public static List<Tuple<Vector3, bool>> localPlayerPositions = new List<Tuple<Vector3, bool>>();
        public static List<DeadPlayer> deadPlayers = new List<DeadPlayer>();
        public static List<int> exiledPlayers = new List<int>();
        public static List<int> suicidedPlayers = new List<int>();
        public static List<int> misfiredPlayers = new List<int>();

        public static void clearGameHistory() {
            localPlayerPositions.Clear();
            deadPlayers.Clear();
            exiledPlayers.Clear();
            suicidedPlayers.Clear();
            misfiredPlayers.Clear();
        }
    }
}