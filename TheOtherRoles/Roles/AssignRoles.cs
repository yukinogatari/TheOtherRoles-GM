using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;
using System;
using static TheOtherRoles.TheOtherRoles;


namespace TheOtherRoles.Roles
{

    /* RoleManager.SelectRoles
     * 
     * AssignRolesForTeam(GameData.PlayerInfo, this.RoleOptions, RoleTeamTypes.Impostor, maxImpostors, defaultRole: RoleTypes.Impostor)
     * AssignRolesForTeam(GameData.PlayerInfo, this.RoleOptions, RoleTeamTypes.Crewmate, maxInt, defaultRole: RoleTypes.Crewmate)
     * 
     */

    /* RoleManager.AssignRolesForTeam(List<GameData.PlayerInfo> players, RoleOptionsData opts, RoleTeamTypes team, int teamMax, RoleTypes? defaultRole)
     * 
     * 
     * 
     */

    class AssignRoles
    {
        public static List<CustomRoleTypes> SpecialRoles = new List<CustomRoleTypes> {
            CustomRoleTypes.Mafia,
            CustomRoleTypes.Godfather,
            CustomRoleTypes.Mafioso,
            CustomRoleTypes.Janitor,
            CustomRoleTypes.Sidekick,
            CustomRoleTypes.Lovers,
            CustomRoleTypes.Guesser,
            CustomRoleTypes.EvilGuesser,
            CustomRoleTypes.NiceGuesser,
            CustomRoleTypes.Mini,
            CustomRoleTypes.EvilMini,
            CustomRoleTypes.NiceMini,
            CustomRoleTypes.GM,
        };

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SetRole))]
        class RoleManagerSetRolePatch
        {
            public static bool Prefix(RoleManager __instance, ref PlayerControl targetPlayer, ref RoleTypes roleType)
            {
                if (__instance == null || targetPlayer == null) return false;
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && !DestroyableSingleton<TutorialManager>.InstanceExists) return false;

                if (roleType >= (RoleTypes)CustomRoleTypes.Crewmate && targetPlayer != null)
                {
                    Helpers.log($"RoleManager.SetRole: {(CustomRoleTypes)roleType} to {targetPlayer.Data.PlayerName}");
                    CustomRole role = targetPlayer.setRole((CustomRoleTypes)roleType);
                    roleType = role.IsImpostor ? RoleTypes.Impostor : RoleTypes.Crewmate;
                }
                else
                {
                    Helpers.log($"RoleManager.SetRole: {roleType} to {targetPlayer.Data.PlayerName}");
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.TryAssignRoleOnDeath))]
        class RoleManagerTryAssignRoleOnDeathePatch
        {
            public static bool Prefix(RoleManager __instance, ref PlayerControl player)
            {
                if (player.role()?.TeamType != RoleTeamTypes.Crewmate) return false;
                if (player.isRole(CustomRoleTypes.Madmate) && !Madmate.canBeGA) return false;

                return true;
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        class RoleManagerSelectRolesPatch
        {
            public static bool Prefix(RoleManager __instance)
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return false;

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ResetVaribles, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.resetVariables();

                List<PlayerControl> players = PlayerControl.AllPlayerControls?.ToArray()?.ToList();

                int numPlayers = players.Count;
                int numImpostors = PlayerControl.GameOptions.GetAdjustedNumImpostors(numPlayers);

                // First figure out what our max # of roles we can assign is
                var crewmateMin = CustomOptionHolder.crewmateRolesCountMin.getSelection();
                var crewmateMax = CustomOptionHolder.crewmateRolesCountMax.getSelection();
                var neutralMin = CustomOptionHolder.neutralRolesCountMin.getSelection();
                var neutralMax = CustomOptionHolder.neutralRolesCountMax.getSelection();
                var impostorMin = CustomOptionHolder.impostorRolesCountMin.getSelection();
                var impostorMax = CustomOptionHolder.impostorRolesCountMax.getSelection();

                // Make sure min is less or equal to max
                if (crewmateMin > crewmateMax) crewmateMin = crewmateMax;
                if (neutralMin > neutralMax) neutralMin = neutralMax;
                if (impostorMin > impostorMax) impostorMin = impostorMax;

                // Get the maximum allowed count of each role type based on the minimum and maximum option
                int numCrewRoles = rnd.Next(crewmateMin, crewmateMax + 1);
                int numNeutralRoles = rnd.Next(neutralMin, neutralMax + 1);
                int numImpRoles = rnd.Next(impostorMin, impostorMax + 1);

                RoleAssignmentData data = new RoleAssignmentData
                {
                    crewmates = new List<PlayerControl>(),
                    impostors = new List<PlayerControl>(),
                    neutrals = new List<PlayerControl>(),
                    players = players,

                    specialRoles = new List<Tuple<RoleTypes, int>>(),

                    impGuaranteed = new List<Tuple<RoleTypes, int>>(),
                    crewGuaranteed = new List<Tuple<RoleTypes, int>>(),
                    neutralGuaranteed = new List<Tuple<RoleTypes, int>>(),

                    impChance = new List<Tuple<RoleTypes, int>>(),
                    crewChance = new List<Tuple<RoleTypes, int>>(),
                    neutralChance = new List<Tuple<RoleTypes, int>>(),
                };

                Helpers.log($"{data.maxImpostorRoles} / {data.maxNeutralRoles} / {data.maxCrewRoles}");

                RoleOptionsData roleOptions = PlayerControl.GameOptions.RoleOptions;
                foreach (var key in roleOptions.roleRates.Keys)
                {
                    var roleRate = roleOptions.roleRates[key];
                    int chance = roleRate.Chance;
                    List<Tuple<RoleTypes, int>> roles = 
                        Enumerable.Repeat(new Tuple<RoleTypes, int>(key, chance), roleRate.MaxCount)
                        .Where(x => chance >= rnd.Next(1, 101))
                        .ToList();

                    if (RoleManager.IsGhostRole(key)) continue;
                    if (chance == 0) continue;
                    if (SpecialRoles.Contains((CustomRoleTypes)key))
                    {
                        data.specialRoles.AddRange(roles);
                        continue;
                    }

                    if (getTeam(key) == RoleTeamTypes.Impostor)
                    {
                        if (chance == 100)
                            data.impGuaranteed.AddRange(roles);
                        else
                            data.impChance.AddRange(roles);
                    }
                    else if (getTeam(key) == RoleTeamTypes.Crewmate)
                    {
                        if (chance == 100)
                            data.crewGuaranteed.AddRange(roles);
                        else
                            data.crewChance.AddRange(roles);
                    }
                    else
                    {
                        if (chance == 100)
                            data.neutralGuaranteed.AddRange(roles);
                        else
                            data.neutralChance.AddRange(roles);
                    }
                }

                // having a GM in the pool reduces the number of players that can be assigned roles by one
                if (data.specialRoles.Any(x => x.Item1 == (RoleTypes)CustomRoleTypes.GM)) numPlayers--;

                // Potentially lower the actual maximum to the assignable players
                data.maxImpostorRoles = Mathf.Min(numImpostors, numImpRoles);
                data.maxNeutralRoles = Mathf.Min(numPlayers - numImpostors, numNeutralRoles);
                data.maxCrewRoles = Mathf.Min(numPlayers - numImpostors, numCrewRoles);

                assignSpecialRoles(data, ref numImpostors); // Assign special roles like mafia and lovers first as they assign a role to multiple players and the chances are independent of the ticket system
                selectFactionForFactionIndependentRoles(data);
                assignEnsuredRoles(data); // Assign roles that should always be in the game next
                assignChanceRoles(data); // Assign roles that may or may not be in the game last
                assignUnassigned(data, ref numImpostors);
                //Helpers.log($"{players.Count}");
                //Helpers.log($"{players.Count} {opts.roleRates.Count} {team} {teamMax} {defaultRole}");
                //Helpers.log($"{players} {opts} {team} {teamMax}");
                return false;
            }
        }

        public static RoleTeamTypes getTeam(RoleTypes role)
        {
            foreach (RoleBehaviour r in RoleManager.Instance.AllRoles)
            {
                if (r.Role == role) return r.TeamType;
            }

            if (role >= (RoleTypes)CustomRoleTypes.Neutral)
            {
                return (RoleTeamTypes)CustomRoleTeamTypes.Neutral;
            }

            if (role >= (RoleTypes)CustomRoleTypes.Impostor)
            {
                return RoleTeamTypes.Impostor;
            }

            if (role >= (RoleTypes)CustomRoleTypes.Crewmate)
            {
                return RoleTeamTypes.Crewmate;
            }

            return RoleTeamTypes.Crewmate;
        }

        private static void assignUnassigned(RoleAssignmentData data, ref int numImpostors)
        {
            int imps = 0;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                if (p.Data.Role.IsImpostor) imps++;

            Helpers.log($"leftover imps");
            for (int i = 0; i < numImpostors - imps; i++)
            {
                var p = setRoleToRandomPlayer(RoleTypes.Impostor, data.players);
                data.addImpostor(p);
                data.maxImpostorRoles++;
            }

            Helpers.log($"leftover crews");
            while (data.players.Count > 0)
            {
                var p = setRoleToRandomPlayer(RoleTypes.Crewmate, data.players);
                data.addCrew(p);
                data.maxCrewRoles++;
            }

            // If lovers can double up with other roles, assign them at the very end,
            // after 
            if (!Lovers.loversCanHaveAnotherRole.getBool()) assignLovers(data, ref numImpostors);
        }


        private static void assignLovers(RoleAssignmentData data, ref int numImpostors) {

            bool onlyRole = !Lovers.loversCanHaveAnotherRole.getBool();
            bool imp = rnd.Next(1, 101) <= Lovers.loversImpLoverRate.getSelection() * 10;

            var players = onlyRole ? data.players : PlayerControl.AllPlayerControls.ToArray().ToList();

            // Assign Lovers
            foreach (var role in data.specialRoles.FindAll(x => x.Item1 == (RoleTypes)CustomRoleTypes.Lovers))
            {
                if (rnd.Next(1, 101) <= role.Item2)
                {
                    int id1 = 0;
                    int id2 = 0;
                    PlayerControl lover1 = null;
                    PlayerControl lover2 = null;
                    if (onlyRole)
                    {
                        id1 = id2 = rnd.Next(0, data.players.Count);
                        while (id1 == id2)
                        {
                            id2 = rnd.Next(0, data.players.Count);
                        }

                        lover1 = players[id1];
                        lover2 = players[id2];

                        lover1.RpcSetRole(RoleTypes.Crewmate);
                        lover2.RpcSetRole(imp ? RoleTypes.Impostor : RoleTypes.Crewmate);
                        lover1.SetLoversModifier(lover2);

                        if (imp) numImpostors--;

                        data.players.RemoveAll(x => x.PlayerId == lover1.PlayerId || x.PlayerId == lover2.PlayerId);
                    }
                    else
                    {
                        lover1 = data.crewmates[rnd.Next(0, data.crewmates.Count)];
                        lover2 = lover1;
                        if (imp)
                        {
                            lover2 = data.impostors[rnd.Next(0, data.impostors.Count)];
                        }
                        else
                        {
                            id1 = lover1.PlayerId;
                            id2 = id1;
                            while (id1 == id2)
                            {
                                id2 = rnd.Next(0, data.crewmates.Count + data.neutrals.Count);
                            }

                            if (id2 < data.crewmates.Count)
                            {
                                lover2 = data.crewmates[id2];
                            }
                            else
                            {
                                lover2 = data.neutrals[id2 - data.crewmates.Count];
                            }
                        }
                        lover1.SetLoversModifier(lover2);
                    }

                    players.RemoveAll(x => x.PlayerId == lover1.PlayerId || x.PlayerId == lover2.PlayerId);
                }
            }
        }

        // TODO: handle special roles
        private static void assignSpecialRoles(RoleAssignmentData data, ref int numImpostors)
        {
            Helpers.log($"assigning special roles");

            // Assign the GM
            foreach (var role in data.specialRoles.FindAll(x => x.Item1 == (RoleTypes)CustomRoleTypes.GM))
            {
                if (role == null) continue;
                PlayerControl host = AmongUsClient.Instance?.GetHost().Character;
                PlayerControl gm;
                if (GM.gmIsHost.getBool() == true && host != null)
                {
                    gm = host;
                    gm.RpcSetRole((RoleTypes)CustomRoleTypes.GM);
                }
                else
                {
                    gm = setRoleToRandomPlayer((RoleTypes)CustomRoleTypes.GM, data.players);
                }

                if (GM.gmDiesAtStart.getBool())
                    gm.Exiled();

                data.players.RemoveAll(x => x.PlayerId == gm.PlayerId);
            }

            Helpers.log($"assigning lovers");

            // If Lovers is an exclusive role, assign it at the beginning.
            if (Lovers.loversCanHaveAnotherRole.getBool()) assignLovers(data, ref numImpostors);

            // Assign any simple impostor/crew roles, players that won't have anything at all.
            int simpleCrew = data.players.Count - numImpostors - data.maxCrewRoles - data.maxNeutralRoles;
            for (int i = 0; i < simpleCrew; i++)
            {
                Helpers.log("guaranteed crew");
                var p = setRoleToRandomPlayer(RoleTypes.Crewmate, data.players);
                data.addCrew(p);
                data.maxCrewRoles++;
            }

            int simpleImp = numImpostors - data.maxImpostorRoles;
            for (int i = 0; i < simpleImp; i++)
            {
                Helpers.log("guaranteed imp");
                var p = setRoleToRandomPlayer(RoleTypes.Impostor, data.players);
                data.addImpostor(p);
                data.maxImpostorRoles++;
            }

            // Assign Mafia
            foreach (var role in data.specialRoles.FindAll(x => x.Item1 == (RoleTypes)CustomRoleTypes.Mafia))
            {
                if (data.impostorRolesLeft >= 3 && (rnd.Next(1, 101) <= role.Item2))
                {
                    PlayerControl godfather = setRoleToRandomPlayer((RoleTypes)CustomRoleTypes.Godfather, data.players);
                    PlayerControl janitor = setRoleToRandomPlayer((RoleTypes)CustomRoleTypes.Janitor, data.players);
                    PlayerControl mafioso = setRoleToRandomPlayer((RoleTypes)CustomRoleTypes.Mafioso, data.players);

                    godfather.role<Mafia>().setMafia(godfather, mafioso, janitor);
                    mafioso.role<Mafia>().setMafia(godfather, mafioso, janitor);
                    janitor.role<Mafia>().setMafia(godfather, mafioso, janitor);

                    data.addImpostor(godfather);
                    data.addImpostor(janitor);
                    data.addImpostor(mafioso);
                }
            }
        }

        private static void selectFactionForFactionIndependentRoles(RoleAssignmentData data)
        {
            Helpers.log($"choosing factions");

            foreach (var role in data.specialRoles.FindAll(x => x.Item1 == (RoleTypes)CustomRoleTypes.Mini))
            {
                var chance = role.Item2;
                if (data.impostorRolesLeft > 0 && rnd.Next(1, 101) <= Mini.miniIsImpRate.getSelection() * 10)
                {
                    var val = new Tuple<RoleTypes, int>((RoleTypes)CustomRoleTypes.EvilMini, chance);
                    if (chance == 100) data.impGuaranteed.Add(val);
                    else data.impChance.Add(val);
                }
                else if (data.crewRolesLeft > 0)
                {
                    var val = new Tuple<RoleTypes, int>((RoleTypes)CustomRoleTypes.NiceMini, chance);
                    if (chance == 100) data.crewGuaranteed.Add(val);
                    else data.crewChance.Add(val);
                }
            }

            foreach (var role in data.specialRoles.FindAll(x => x.Item1 == (RoleTypes)CustomRoleTypes.Guesser))
            {
                var chance = role.Item2;
                if (data.impostorRolesLeft > 0 && rnd.Next(1, 101) <= Mini.miniIsImpRate.getSelection() * 10)
                {
                    var val = new Tuple<RoleTypes, int>((RoleTypes)CustomRoleTypes.EvilGuesser, chance);
                    if (chance == 100) data.impGuaranteed.Add(val);
                    else data.impChance.Add(val);
                }
                else if (data.crewRolesLeft > 0)
                {
                    var val = new Tuple<RoleTypes, int>((RoleTypes)CustomRoleTypes.NiceGuesser, chance);
                    if (chance == 100) data.crewGuaranteed.Add(val);
                    else data.crewChance.Add(val);
                }
            }
        }

        private static void assignEnsuredRoles(RoleAssignmentData data)
        {
            Helpers.log($"assigning guaranteed roles");

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (data.players.Count > 0 &&
                  ((data.impostorRolesLeft > 0 && data.impGuaranteed.Count > 0) ||
                   (data.neutralRolesLeft > 0  && data.neutralGuaranteed.Count > 0) ||
                   (data.crewRolesLeft > 0     && data.crewGuaranteed.Count > 0)))
            {

                List<Tuple<RoleTypes, int>> rolesToAssign = new List<Tuple<RoleTypes, int>>();

                if (data.impostorRolesLeft > 0) rolesToAssign.AddRange(data.impGuaranteed);
                if (data.neutralRolesLeft > 0)  rolesToAssign.AddRange(data.neutralGuaranteed);
                if (data.crewRolesLeft > 0)     rolesToAssign.AddRange(data.crewGuaranteed);

                // Randomly select one of the available roles and assign it to a random player,
                // then remove the role (and any potentially blocked role pairings) from the pool(s)
                var index = rnd.Next(0, rolesToAssign.Count);
                var role = rolesToAssign[index];
                var team = getTeam(role.Item1);
                var p = setRoleToRandomPlayer(role.Item1, data.players);

                if (team == RoleTeamTypes.Crewmate)
                {
                    data.addCrew(p);
                    data.crewGuaranteed.Remove(role);
                }
                else if (team == RoleTeamTypes.Impostor)
                {
                    data.addImpostor(p);
                    data.impGuaranteed.Remove(role);
                }
                else
                {
                    data.addNeutral(p);
                    data.neutralGuaranteed.Remove(role);
                }

                if (CustomOptionHolder.blockedRolePairings.ContainsKey(role.Item1)) {
                    foreach (var blockedRole in CustomOptionHolder.blockedRolePairings[role.Item1]) {
                        data.removeRole(blockedRole);
                    }
                }
            }
        }


        private static void assignChanceRoles(RoleAssignmentData data) {
            Helpers.log($"assigning chance roles");
            // Get all roles where the chance to occur is set greater than 0% but not 100% and build a ticket pool based on their weight
            List<RoleTypes> impostorTickets = data.impChance    .Select(x => Enumerable.Repeat(x.Item1, x.Item2)).SelectMany(x => x).ToList();
            List<RoleTypes> neutralTickets  = data.neutralChance.Select(x => Enumerable.Repeat(x.Item1, x.Item2)).SelectMany(x => x).ToList();
            List<RoleTypes> crewmateTickets = data.crewChance   .Select(x => Enumerable.Repeat(x.Item1, x.Item2)).SelectMany(x => x).ToList();

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (data.players.Count > 0 &&
                  ((data.impostorRolesLeft > 0 && impostorTickets.Count > 0) ||
                   (data.neutralRolesLeft > 0 && neutralTickets.Count > 0) ||
                   (data.crewRolesLeft > 0 && crewmateTickets.Count > 0))) {

                // regenerate tickets every iteration to properly handle multiple instances of roles
                impostorTickets = data.impChance   .Select(x => Enumerable.Repeat(x.Item1, x.Item2)).SelectMany(x => x).ToList();
                neutralTickets = data.neutralChance.Select(x => Enumerable.Repeat(x.Item1, x.Item2)).SelectMany(x => x).ToList();
                crewmateTickets = data.crewChance  .Select(x => Enumerable.Repeat(x.Item1, x.Item2)).SelectMany(x => x).ToList();

                List<RoleTypes> rolesToAssign = new List<RoleTypes>();

                if (data.impostorRolesLeft > 0) rolesToAssign.AddRange(impostorTickets);
                if (data.neutralRolesLeft > 0) rolesToAssign.AddRange(neutralTickets);
                if (data.crewRolesLeft > 0) rolesToAssign.AddRange(crewmateTickets);

                if (rolesToAssign.Count <= 0) break;

                // Randomly select a pool of role tickets to assign a role from next (Crewmate role, Neutral role or Impostor role) 
                // then select one of the roles from the selected pool to a player 
                // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)

                var index = rnd.Next(0, rolesToAssign.Count);
                var role = rolesToAssign[index];
                var team = getTeam(role);
                var p = setRoleToRandomPlayer(role, data.players);

                if (team == RoleTeamTypes.Crewmate)
                {
                    data.addCrew(p);
                    data.crewChance.RemoveAt(data.crewChance.FindIndex(x => x.Item1 == role));
                }
                else if (team == RoleTeamTypes.Impostor)
                {
                    data.addImpostor(p);
                    data.impChance.RemoveAt(data.impChance.FindIndex(x => x.Item1 == role));
                }
                else
                {
                    data.addNeutral(p);
                    data.neutralChance.RemoveAt(data.neutralChance.FindIndex(x => x.Item1 == role));
                }

                if (CustomOptionHolder.blockedRolePairings.ContainsKey(role))
                {
                    foreach (var blockedRole in CustomOptionHolder.blockedRolePairings[role])
                    {
                        data.removeRole(blockedRole);
                    }
                }
            }
        }


        private static PlayerControl setRoleToRandomPlayer(RoleTypes roleId, List<PlayerControl> playerList, byte flag = 0) {
            PlayerControl player = playerList[rnd.Next(0, playerList.Count)];
            player.RpcSetRole(roleId);
            return player;
        }
        private static PlayerControl setModifierToRandomPlayer(RoleModifierTypes mod, List<PlayerControl> playerList, byte flag = 0)
        {
            PlayerControl player = playerList[rnd.Next(0, playerList.Count)];
            player.RpcSetRoleModifier(mod);
            return player;
        }



        private class RoleAssignmentData {
            public List<PlayerControl> players { get; set; }
            public List<PlayerControl> impostors { get; set; }
            public List<PlayerControl> neutrals { get; set; }
            public List<PlayerControl> crewmates { get; set; }

            public void addCrew(PlayerControl p)
            {
                crewmates.Add(p);
                players.RemoveAll(x => x.PlayerId == p.PlayerId);
            }

            public void addImpostor(PlayerControl p)
            {
                impostors.Add(p);
                players.RemoveAll(x => x.PlayerId == p.PlayerId);
            }

            public void addNeutral(PlayerControl p)
            {
                neutrals.Add(p);
                players.RemoveAll(x => x.PlayerId == p.PlayerId);
            }

            public void removeRole(RoleTypes role)
            {
                specialRoles.RemoveAll(x => x.Item1 == role);
                impGuaranteed.RemoveAll(x => x.Item1 == role);
                crewGuaranteed.RemoveAll(x => x.Item1 == role);
                neutralGuaranteed.RemoveAll(x => x.Item1 == role);
                impChance.RemoveAll(x => x.Item1 == role);
                crewChance.RemoveAll(x => x.Item1 == role);
                neutralChance.RemoveAll(x => x.Item1 == role);
            }

            public int impostorRolesLeft { get { return maxImpostorRoles - impostors.Count;  } }
            public int neutralRolesLeft { get { return maxNeutralRoles - neutrals.Count - crewmates.Count; } }
            public int crewRolesLeft { get { return maxCrewRoles - crewmates.Count - neutrals.Count; } }

            public int maxImpostorRoles { get; set; }
            public int maxNeutralRoles { get; set; }
            public int maxCrewRoles { get; set; }

            public List<Tuple<RoleTypes, int>> specialRoles { get; set; }
            public List<Tuple<RoleTypes, int>> impGuaranteed { get; set; }
            public List<Tuple<RoleTypes, int>> crewGuaranteed { get; set; }
            public List<Tuple<RoleTypes, int>> neutralGuaranteed { get; set; }
            public List<Tuple<RoleTypes, int>> impChance { get; set; }
            public List<Tuple<RoleTypes, int>> crewChance { get; set; }
            public List<Tuple<RoleTypes, int>> neutralChance { get; set; }
        }
    }
}