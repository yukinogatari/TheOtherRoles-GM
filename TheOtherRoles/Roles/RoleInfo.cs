using HarmonyLib;
using System.Linq;
using System;
using System.Collections.Generic;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    class RoleInfo {
        public Color color;
        public string name { get { return ModTranslation.getString(nameKey); } }
        public string nameColored { get { return Helpers.cs(color, name); } }
        public string introDescription { get { return ModTranslation.getString(nameKey + "IntroDesc"); } }
        public string shortDescription { get { return ModTranslation.getString(nameKey + "ShortDesc"); } }
        public string fullDescription { get { return ModTranslation.getString(nameKey + "FullDesc"); } }
        public string blurb { get { return ModTranslation.getString(nameKey + "Blurb"); } }
        public string roleOptions { 
            get {
                return GameOptionsDataPatch.optionsToString(baseOption, true);
            }
        }
        public bool enabled { get { return baseOption == null || baseOption.enabled; } }
        public CustomRoleTypes roleId;

        private string nameKey;
        private CustomOption baseOption;

        RoleInfo(string name, Color color, CustomOption baseOption, CustomRoleTypes roleId) {
            this.color = color;
            this.nameKey = name;
            this.baseOption = baseOption;
            this.roleId = roleId;
        }

        public static RoleInfo jester = new RoleInfo("jester", RoleColors.Jester, CustomOptionHolder.jesterSpawnRate, CustomRoleTypes.Jester);
        public static RoleInfo mayor = new RoleInfo("mayor", RoleColors.Mayor, CustomOptionHolder.mayorSpawnRate, CustomRoleTypes.Mayor);
        public static RoleInfo sheriff = new RoleInfo("sheriff", RoleColors.Sheriff, CustomOptionHolder.sheriffSpawnRate, CustomRoleTypes.Sheriff);
        public static RoleInfo lighter = new RoleInfo("lighter", RoleColors.Lighter, CustomOptionHolder.lighterSpawnRate, CustomRoleTypes.Lighter);
        public static RoleInfo godfather = new RoleInfo("godfather", RoleColors.Godfather, CustomOptionHolder.mafiaSpawnRate, CustomRoleTypes.Godfather);
        public static RoleInfo mafioso = new RoleInfo("mafioso", RoleColors.Mafioso, CustomOptionHolder.mafiaSpawnRate, CustomRoleTypes.Mafioso);
        public static RoleInfo janitor = new RoleInfo("janitor", RoleColors.Janitor, CustomOptionHolder.mafiaSpawnRate, CustomRoleTypes.Janitor);
        public static RoleInfo camouflager = new RoleInfo("camouflager", RoleColors.Camouflager, CustomOptionHolder.camouflagerSpawnRate, CustomRoleTypes.Camouflager);
        public static RoleInfo vampire = new RoleInfo("vampire", RoleColors.Vampire, CustomOptionHolder.vampireSpawnRate, CustomRoleTypes.Vampire);
        public static RoleInfo eraser = new RoleInfo("eraser", RoleColors.Eraser, CustomOptionHolder.eraserSpawnRate, CustomRoleTypes.Eraser);
        public static RoleInfo trickster = new RoleInfo("trickster", RoleColors.Trickster, CustomOptionHolder.tricksterSpawnRate, CustomRoleTypes.Trickster);
        public static RoleInfo cleaner = new RoleInfo("cleaner", RoleColors.Cleaner, CustomOptionHolder.cleanerSpawnRate, CustomRoleTypes.Cleaner);
        public static RoleInfo warlock = new RoleInfo("warlock", RoleColors.Warlock, CustomOptionHolder.warlockSpawnRate, CustomRoleTypes.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo("bountyHunter", RoleColors.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate, CustomRoleTypes.BountyHunter);
        public static RoleInfo detective = new RoleInfo("detective", RoleColors.Detective, CustomOptionHolder.detectiveSpawnRate, CustomRoleTypes.Detective);
        public static RoleInfo timeMaster = new RoleInfo("timeMaster", RoleColors.TimeMaster, CustomOptionHolder.timeMasterSpawnRate, CustomRoleTypes.TimeMaster);
        public static RoleInfo medic = new RoleInfo("medic", RoleColors.Medic, CustomOptionHolder.medicSpawnRate, CustomRoleTypes.Medic);
        public static RoleInfo shifter = new RoleInfo("shifter", RoleColors.Shifter, CustomOptionHolder.shifterSpawnRate, CustomRoleTypes.Shifter);
        public static RoleInfo swapper = new RoleInfo("swapper", RoleColors.Swapper, CustomOptionHolder.swapperSpawnRate, CustomRoleTypes.Swapper);
        public static RoleInfo seer = new RoleInfo("seer", RoleColors.Seer, CustomOptionHolder.seerSpawnRate, CustomRoleTypes.Seer);
        public static RoleInfo hacker = new RoleInfo("hacker", RoleColors.Hacker, CustomOptionHolder.hackerSpawnRate, CustomRoleTypes.Hacker);
        public static RoleInfo mini = new RoleInfo("mini", RoleColors.Mini, CustomOptionHolder.miniSpawnRate, CustomRoleTypes.Mini);
        public static RoleInfo niceMini = new RoleInfo("niceMini", RoleColors.Mini, CustomOptionHolder.miniSpawnRate, CustomRoleTypes.NiceMini);
        public static RoleInfo evilMini = new RoleInfo("evilMini", Palette.ImpostorRed, CustomOptionHolder.miniSpawnRate, CustomRoleTypes.EvilMini);
        public static RoleInfo tracker = new RoleInfo("tracker", RoleColors.Tracker, CustomOptionHolder.trackerSpawnRate, CustomRoleTypes.Tracker);
        public static RoleInfo snitch = new RoleInfo("snitch", RoleColors.Snitch, CustomOptionHolder.snitchSpawnRate, CustomRoleTypes.Snitch);
        public static RoleInfo jackal = new RoleInfo("jackal", RoleColors.Jackal, CustomOptionHolder.jackalSpawnRate, CustomRoleTypes.Jackal);
        public static RoleInfo sidekick = new RoleInfo("sidekick", RoleColors.Sidekick, CustomOptionHolder.jackalSpawnRate, CustomRoleTypes.Sidekick);
        public static RoleInfo spy = new RoleInfo("spy", RoleColors.Spy, CustomOptionHolder.spySpawnRate, CustomRoleTypes.Spy);
        public static RoleInfo securityGuard = new RoleInfo("securityGuard", RoleColors.SecurityGuard, CustomOptionHolder.securityGuardSpawnRate, CustomRoleTypes.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo("arsonist", RoleColors.Arsonist, CustomOptionHolder.arsonistSpawnRate, CustomRoleTypes.Arsonist);
        public static RoleInfo guesser = new RoleInfo("goodGuesser", RoleColors.Guesser, CustomOptionHolder.guesserSpawnRate, CustomRoleTypes.Guesser);
        public static RoleInfo goodGuesser = new RoleInfo("goodGuesser", RoleColors.Guesser, CustomOptionHolder.guesserSpawnRate, CustomRoleTypes.NiceGuesser);
        public static RoleInfo badGuesser = new RoleInfo("badGuesser", Palette.ImpostorRed, CustomOptionHolder.guesserSpawnRate, CustomRoleTypes.EvilGuesser);
        public static RoleInfo bait = new RoleInfo("bait", RoleColors.Bait, CustomOptionHolder.baitSpawnRate, CustomRoleTypes.Bait);
        public static RoleInfo madmate = new RoleInfo("madmate", RoleColors.Madmate, CustomOptionHolder.madmateSpawnRate, CustomRoleTypes.Madmate);
        public static RoleInfo impostor = new RoleInfo("impostor", Palette.ImpostorRed, null, CustomRoleTypes.Impostor);
        public static RoleInfo crewmate = new RoleInfo("crewmate", Color.white, null, CustomRoleTypes.Crewmate);
        public static RoleInfo lover = new RoleInfo("lover", RoleColors.Lovers, CustomOptionHolder.loversSpawnRate, CustomRoleTypes.Lovers);
        public static RoleInfo gm = new RoleInfo("gm", RoleColors.GM, CustomOptionHolder.gmEnabled, CustomRoleTypes.GM);
        public static RoleInfo opportunist = new RoleInfo("opportunist", RoleColors.Opportunist, CustomOptionHolder.opportunistSpawnRate, CustomRoleTypes.Opportunist);
        public static RoleInfo vulture = new RoleInfo("vulture", RoleColors.Vulture, CustomOptionHolder.vultureSpawnRate, CustomRoleTypes.Vulture);
        public static RoleInfo medium = new RoleInfo("medium", RoleColors.Medium, CustomOptionHolder.mediumSpawnRate, CustomRoleTypes.Medium);

        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
                impostor,
                godfather,
                mafioso,
                janitor,
                camouflager,
                vampire,
                eraser,
                trickster,
                cleaner,
                warlock,
                bountyHunter,
                niceMini,
                evilMini,
                goodGuesser,
                badGuesser,
                lover,
                jester,
                arsonist,
                jackal,
                sidekick,
            	vulture,
                crewmate,
                shifter,
                mayor,
                sheriff,
                lighter,
                detective,
                timeMaster,
                medic,
                swapper,
                seer,
                hacker,
                tracker,
                snitch,
                spy,
                securityGuard,
                bait,
                madmate,
                gm,
                opportunist,
	            medium
            };

        public static string tl(string key)
        {
            return ModTranslation.getString(key);
        }

        public static RoleInfo getRoleInfo(RoleTypes roleId)
        {
            if (roleId == RoleTypes.Impostor)
                return impostor;
            if (roleId == RoleTypes.Crewmate)
                return crewmate;

            foreach (RoleInfo info in allRoleInfos)
                if (info.roleId == (CustomRoleTypes)roleId) return info;

            return crewmate;
        }

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, CustomRoleTypes[] excludeRoles = null) {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            var roleType = p.Data.Role.Role;

            infos.Add(getRoleInfo(roleType));

            // Special roles
            // if ((Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);

            // Modifier
            if (p.hasModifier(RoleModifierTypes.Lovers)) infos.Add(lover);

            if (excludeRoles != null)
                infos.RemoveAll(x => excludeRoles.Contains(x.roleId));

            return infos;
        }

        public static String GetRole(PlayerControl p) {
            return p.Data.Role.NiceName;
        }
    }
}
