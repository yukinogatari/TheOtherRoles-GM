using HarmonyLib;
using System.Linq;
using System;
using System.Collections.Generic;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;

namespace TheOtherRoles
{
    class RoleInfo {
        public Color color;
        public string name { get { return ModTranslation.getString(nameKey); } }
        public string nameColored { get { return Helpers.cs(color, name); } }
        public string introDescription { get { return ModTranslation.getString(introDescriptionKey); } }
        public string shortDescription { get { return ModTranslation.getString(shortDescriptionKey); } }
        public string fullDescription { get { return ModTranslation.getString(fullDescriptionKey); } }
        public string roleOptions { 
            get {
                return GameOptionsDataPatch.optionsToString(baseOption, true);
            }
        }
        public bool enabled { get { return baseOption == null || baseOption.enabled; } }
        public RoleId roleId;

        private string nameKey;
        private string introDescriptionKey;
        private string shortDescriptionKey;
        private string fullDescriptionKey;
        private CustomOption baseOption;

        RoleInfo(string name, Color color, string introDescription, string shortDescription, string fullDescription, CustomOption baseOption, RoleId roleId) {
            this.color = color;
            this.nameKey = name;
            this.introDescriptionKey = introDescription;
            this.shortDescriptionKey = shortDescription;
            this.fullDescriptionKey = fullDescription;
            this.baseOption = baseOption;
            this.roleId = roleId;
        }

        public static RoleInfo jester = new RoleInfo("jester", Jester.color, "jesterIntroDesc", "jesterShortDesc", "jesterFullDesc", CustomOptionHolder.jesterSpawnRate, RoleId.Jester);
        public static RoleInfo mayor = new RoleInfo("mayor", Mayor.color, "mayorIntroDesc", "mayorShortDesc", "mayorFullDesc", CustomOptionHolder.mayorSpawnRate, RoleId.Mayor);
        public static RoleInfo engineer = new RoleInfo("engineer", Engineer.color, "engineerIntroDesc", "engineerShortDesc", "engineerFullDesc", CustomOptionHolder.engineerSpawnRate, RoleId.Engineer);
        public static RoleInfo sheriff = new RoleInfo("sheriff", Sheriff.color, "sheriffIntroDesc", "sheriffShortDesc", "sheriffFullDesc", CustomOptionHolder.sheriffSpawnRate, RoleId.Sheriff);
        public static RoleInfo lighter = new RoleInfo("lighter", Lighter.color, "lighterIntroDesc", "lighterShortDesc", "lighterFullDesc", CustomOptionHolder.lighterSpawnRate, RoleId.Lighter);
        public static RoleInfo godfather = new RoleInfo("godfather", Godfather.color, "godfatherIntroDesc", "godfatherShortDesc", "godfatherFullDesc", CustomOptionHolder.mafiaSpawnRate, RoleId.Godfather);
        public static RoleInfo mafioso = new RoleInfo("mafioso", Mafioso.color, "mafiosoIntroDesc", "mafiosoShortDesc", "mafiosoFullDesc", CustomOptionHolder.mafiaSpawnRate, RoleId.Mafioso);
        public static RoleInfo janitor = new RoleInfo("janitor", Janitor.color, "janitorIntroDesc", "janitorShortDesc", "janitorFullDesc", CustomOptionHolder.mafiaSpawnRate, RoleId.Janitor);
        public static RoleInfo morphling = new RoleInfo("morphling", Morphling.color, "morphlingIntroDesc", "morphlingShortDesc", "morphlingFullDesc", CustomOptionHolder.morphlingSpawnRate, RoleId.Morphling);
        public static RoleInfo camouflager = new RoleInfo("camouflager", Camouflager.color, "camouflagerIntroDesc", "camouflagerShortDesc", "camouflagerFullDesc", CustomOptionHolder.camouflagerSpawnRate, RoleId.Camouflager);
        public static RoleInfo vampire = new RoleInfo("vampire", Vampire.color, "vampireIntroDesc", "vampireShortDesc", "vampireFullDesc", CustomOptionHolder.vampireSpawnRate, RoleId.Vampire);
        public static RoleInfo eraser = new RoleInfo("eraser", Eraser.color, "eraserIntroDesc", "eraserShortDesc", "eraserFullDesc", CustomOptionHolder.eraserSpawnRate, RoleId.Eraser);
        public static RoleInfo trickster = new RoleInfo("trickster", Trickster.color, "tricksterIntroDesc", "tricksterShortDesc", "tricksterFullDesc", CustomOptionHolder.tricksterSpawnRate, RoleId.Trickster);
        public static RoleInfo cleaner = new RoleInfo("cleaner", Cleaner.color, "cleanerIntroDesc", "cleanerShortDesc", "cleanerFullDesc", CustomOptionHolder.cleanerSpawnRate, RoleId.Cleaner);
        public static RoleInfo warlock = new RoleInfo("warlock", Warlock.color, "warlockIntroDesc", "warlockShortDesc", "warlockFullDesc", CustomOptionHolder.warlockSpawnRate, RoleId.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo("bountyHunter", BountyHunter.color, "bountyHunterIntroDesc", "bountyHunterShortDesc", "bountyHunterFullDesc", CustomOptionHolder.bountyHunterSpawnRate, RoleId.BountyHunter);
        public static RoleInfo detective = new RoleInfo("detective", Detective.color, "detectiveIntroDesc", "detectiveShortDesc", "detectiveFullDesc", CustomOptionHolder.detectiveSpawnRate, RoleId.Detective);
        public static RoleInfo timeMaster = new RoleInfo("timeMaster", TimeMaster.color, "timeMasterIntroDesc", "timeMasterShortDesc", "timeMasterFullDesc", CustomOptionHolder.timeMasterSpawnRate, RoleId.TimeMaster);
        public static RoleInfo medic = new RoleInfo("medic", Medic.color, "medicIntroDesc", "medicShortDesc", "medicFullDesc", CustomOptionHolder.medicSpawnRate, RoleId.Medic);
        public static RoleInfo shifter = new RoleInfo("shifter", Shifter.color, "shifterIntroDesc", "shifterShortDesc", "shifterFullDesc", CustomOptionHolder.shifterSpawnRate, RoleId.Shifter);
        public static RoleInfo swapper = new RoleInfo("swapper", Swapper.color, "swapperIntroDesc", "swapperShortDesc", "swapperFullDesc", CustomOptionHolder.swapperSpawnRate, RoleId.Swapper);
        public static RoleInfo seer = new RoleInfo("seer", Seer.color, "seerIntroDesc", "seerShortDesc", "seerFullDesc", CustomOptionHolder.seerSpawnRate, RoleId.Seer);
        public static RoleInfo hacker = new RoleInfo("hacker", Hacker.color, "hackerIntroDesc", "hackerShortDesc", "hackerFullDesc", CustomOptionHolder.hackerSpawnRate, RoleId.Hacker);
        public static RoleInfo niceMini = new RoleInfo("niceMini", Mini.color, "niceMiniIntroDesc", "niceMiniShortDesc", "niceMiniFullDesc", CustomOptionHolder.miniSpawnRate, RoleId.Mini);
        public static RoleInfo evilMini = new RoleInfo("evilMini", Palette.ImpostorRed, "evilMiniIntroDesc", "evilMiniShortDesc", "evilMiniFullDesc", CustomOptionHolder.miniSpawnRate, RoleId.Mini);
        public static RoleInfo tracker = new RoleInfo("tracker", Tracker.color, "trackerIntroDesc", "trackerShortDesc", "trackerFullDesc", CustomOptionHolder.trackerSpawnRate, RoleId.Tracker);
        public static RoleInfo snitch = new RoleInfo("snitch", Snitch.color, "snitchIntroDesc", "snitchShortDesc", "snitchFullDesc", CustomOptionHolder.snitchSpawnRate, RoleId.Snitch);
        public static RoleInfo jackal = new RoleInfo("jackal", Jackal.color, "jackalIntroDesc", "jackalShortDesc", "jackalFullDesc", CustomOptionHolder.jackalSpawnRate, RoleId.Jackal);
        public static RoleInfo sidekick = new RoleInfo("sidekick", Sidekick.color, "sidekickIntroDesc", "sidekickShortDesc", "sidekickFullDesc", CustomOptionHolder.jackalSpawnRate, RoleId.Sidekick);
        public static RoleInfo spy = new RoleInfo("spy", Spy.color, "spyIntroDesc", "spyShortDesc", "spyFullDesc", CustomOptionHolder.spySpawnRate, RoleId.Spy);
        public static RoleInfo securityGuard = new RoleInfo("securityGuard", SecurityGuard.color, "securityGuardIntroDesc", "securityGuardShortDesc", "securityGuardFullDesc", CustomOptionHolder.securityGuardSpawnRate, RoleId.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo("arsonist", Arsonist.color, "arsonistIntroDesc", "arsonistShortDesc", "arsonistFullDesc", CustomOptionHolder.arsonistSpawnRate, RoleId.Arsonist);
        public static RoleInfo goodGuesser = new RoleInfo("goodGuesser", Guesser.color, "goodGuesserIntroDesc", "goodGuesserShortDesc", "goodGuesserFullDesc", CustomOptionHolder.guesserSpawnRate, RoleId.Guesser);
        public static RoleInfo badGuesser = new RoleInfo("badGuesser", Palette.ImpostorRed, "badGuesserIntroDesc", "badGuesserShortDesc", "badGuesserFullDesc", CustomOptionHolder.guesserSpawnRate, RoleId.Guesser);
        public static RoleInfo bait = new RoleInfo("bait", Bait.color, "baitIntroDesc", "baitShortDesc", "baitFullDesc", CustomOptionHolder.baitSpawnRate, RoleId.Bait);
        public static RoleInfo madmate = new RoleInfo("madmate", Madmate.color, "madmateIntroDesc", "madmateShortDesc", "madmateFullDesc", CustomOptionHolder.madmateSpawnRate, RoleId.Madmate);
        public static RoleInfo impostor = new RoleInfo("impostor", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "impostorIntroDesc"), "impostorShortDesc", "impostorFullDesc", null, RoleId.Impostor);
        public static RoleInfo crewmate = new RoleInfo("crewmate", Color.white, "crewmateIntroDesc", "crewmateShortDesc", "crewmateFullDesc", null, RoleId.Crewmate);
        public static RoleInfo lovers = new RoleInfo("lovers", Lovers.color, "loversIntroDesc", "loversShortDesc", "loversFullDesc", CustomOptionHolder.loversSpawnRate, RoleId.Lover);
        public static RoleInfo gm = new RoleInfo("gm", GM.color, "gmIntroDesc", "gmShortDesc", "gmFullDesc", CustomOptionHolder.gmEnabled, RoleId.GM);
        public static RoleInfo opportunist = new RoleInfo("opportunist", Opportunist.color, "oppIntroDesc", "oppShortDesc", "oppFullDesc", CustomOptionHolder.opportunistSpawnRate, RoleId.Opportunist);
        public static RoleInfo vulture = new RoleInfo("vulture", Vulture.color, "vultureIntroDesc", "vultureShortDesc", "vultureFullDesc", CustomOptionHolder.vultureSpawnRate, RoleId.Vulture);
        public static RoleInfo medium = new RoleInfo("medium", Medium.color, "mediumIntroDesc", "mediumShortDesc", "mediumFullDesc", CustomOptionHolder.mediumSpawnRate, RoleId.Medium);

        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
                impostor,
                godfather,
                mafioso,
                janitor,
                morphling,
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
                lovers,
                jester,
                arsonist,
                jackal,
                sidekick,
            	vulture,
                crewmate,
                shifter,
                mayor,
                engineer,
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

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, RoleId[] excludeRoles = null) {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            // Special roles
            if (p == Jester.jester) infos.Add(jester);
            if (p == Mayor.mayor) infos.Add(mayor);
            if (p == Engineer.engineer) infos.Add(engineer);
            if (p == Sheriff.sheriff) infos.Add(sheriff);
            if (p == Lighter.lighter) infos.Add(lighter);
            if (p == Godfather.godfather) infos.Add(godfather);
            if (p == Mafioso.mafioso) infos.Add(mafioso);
            if (p == Janitor.janitor) infos.Add(janitor);
            if (p == Morphling.morphling) infos.Add(morphling);
            if (p == Camouflager.camouflager) infos.Add(camouflager);
            if (p == Vampire.vampire) infos.Add(vampire);
            if (p == Eraser.eraser) infos.Add(eraser);
            if (p == Trickster.trickster) infos.Add(trickster);
            if (p == Cleaner.cleaner) infos.Add(cleaner);
            if (p == Warlock.warlock) infos.Add(warlock);
            if (p == Detective.detective) infos.Add(detective);
            if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
            if (p == Medic.medic) infos.Add(medic);
            if (p == Shifter.shifter) infos.Add(shifter);
            if (p == Swapper.swapper) infos.Add(swapper);
            if (p == Seer.seer) infos.Add(seer);
            if (p == Hacker.hacker) infos.Add(hacker);
            if (p == Mini.mini) infos.Add(p.Data.Role.IsImpostor ? evilMini : niceMini);
            if (p == Tracker.tracker) infos.Add(tracker);
            if (p == Snitch.snitch) infos.Add(snitch);
            if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
            if (p == Sidekick.sidekick) infos.Add(sidekick);
            if (p == Spy.spy) infos.Add(spy);
            if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
            if (p == Arsonist.arsonist) infos.Add(arsonist);
            if (p == Guesser.guesser) infos.Add(p.Data.Role.IsImpostor ? badGuesser : goodGuesser);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Bait.bait) infos.Add(bait);
            if (p == Madmate.madmate) infos.Add(madmate);
            if (p == GM.gm) infos.Add(gm);
            if (p == Opportunist.opportunist) infos.Add(opportunist);
            if (p == Vulture.vulture) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);

            // Default roles
            if (infos.Count == 0 && p.Data.Role.IsImpostor) infos.Add(impostor); // Just Impostor
            if (infos.Count == 0 && !p.Data.Role.IsImpostor) infos.Add(crewmate); // Just Crewmate

            // Modifier
            if (p == Lovers.lover1|| p == Lovers.lover2) infos.Add(lovers);

            if (excludeRoles != null)
                infos.RemoveAll(x => excludeRoles.Contains(x.roleId));

            return infos;
        }

        public static String GetRole(PlayerControl p) {
            string roleName;
            roleName = String.Join("", getRoleInfoForPlayer(p, new RoleId[] { RoleId.Lover }).Select(x => x.name).ToArray());
            return roleName;
        }
    }
}
