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
        public string name;
        public string introDescription;
        public string shortDescription;
        public RoleId roleId;

        RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId) {
            this.color = color;
            this.name = name;
            this.introDescription = introDescription;
            this.shortDescription = shortDescription;
            this.roleId = roleId;
        }

        public static RoleInfo jester;
        public static RoleInfo mayor;
        public static RoleInfo engineer;
        public static RoleInfo sheriff;
        public static RoleInfo lighter;
        public static RoleInfo godfather;
        public static RoleInfo mafioso;
        public static RoleInfo janitor;
        public static RoleInfo morphling;
        public static RoleInfo camouflager;
        public static RoleInfo vampire;
        public static RoleInfo eraser;
        public static RoleInfo trickster;
        public static RoleInfo cleaner;
        public static RoleInfo warlock;
        public static RoleInfo bountyHunter;
        public static RoleInfo detective;
        public static RoleInfo timeMaster;
        public static RoleInfo medic;
        public static RoleInfo shifter;
        public static RoleInfo swapper;
        public static RoleInfo seer;
        public static RoleInfo hacker;
        public static RoleInfo niceMini;
        public static RoleInfo evilMini;
        public static RoleInfo tracker;
        public static RoleInfo snitch;
        public static RoleInfo jackal;
        public static RoleInfo sidekick;
        public static RoleInfo spy;
        public static RoleInfo securityGuard;
        public static RoleInfo arsonist;
        public static RoleInfo goodGuesser;
        public static RoleInfo badGuesser;
        public static RoleInfo bait;
        public static RoleInfo madmate;
        public static RoleInfo impostor;
        public static RoleInfo crewmate;
        public static RoleInfo lover;
        public static RoleInfo gm;
        public static RoleInfo opportunist;

        public static List<RoleInfo> allRoleInfos;

        public static string tl(string key)
        {
            return ModTranslation.getString(key);
        }

        public static void Load() {
            jester = new RoleInfo(tl("jester"), Jester.color, tl("jesterIntroDesc"), tl("jesterShortDesc"), RoleId.Jester);
            mayor = new RoleInfo(tl("mayor"), Mayor.color, tl("mayorIntroDesc"), tl("mayorShortDesc"), RoleId.Mayor);
            engineer = new RoleInfo(tl("engineer"), Engineer.color, tl("engineerIntroDesc"), tl("engineerShortDesc"), RoleId.Engineer);
            sheriff = new RoleInfo(tl("sheriff"), Sheriff.color, tl("sheriffIntroDesc"), tl("sheriffShortDesc"), RoleId.Sheriff);
            lighter = new RoleInfo(tl("lighter"), Lighter.color, tl("lighterIntroDesc"), tl("lighterShortDesc"), RoleId.Lighter);
            godfather = new RoleInfo(tl("godfather"), Godfather.color, tl("godfatherIntroDesc"), tl("godfatherShortDesc"), RoleId.Godfather);
            mafioso = new RoleInfo(tl("mafioso"), Mafioso.color, tl("mafiosoIntroDesc"), tl("mafiosoShortDesc"), RoleId.Mafioso);
            janitor = new RoleInfo(tl("janitor"), Janitor.color, tl("janitorIntroDesc"), tl("janitorShortDesc"), RoleId.Janitor);
            morphling = new RoleInfo(tl("morphling"), Morphling.color, tl("morphlingIntroDesc"), tl("morphlingShortDesc"), RoleId.Morphling);
            camouflager = new RoleInfo(tl("camouflager"), Camouflager.color, tl("camouflagerIntroDesc"), tl("camouflagerShortDesc"), RoleId.Camouflager);
            vampire = new RoleInfo(tl("vampire"), Vampire.color, tl("vampireIntroDesc"), tl("vampireShortDesc"), RoleId.Vampire);
            eraser = new RoleInfo(tl("eraser"), Eraser.color, tl("eraserIntroDesc"), tl("eraserShortDesc"), RoleId.Eraser);
            trickster = new RoleInfo(tl("trickster"), Trickster.color, tl("tricksterIntroDesc"), tl("tricksterShortDesc"), RoleId.Trickster);
            cleaner = new RoleInfo(tl("cleaner"), Cleaner.color, tl("cleanerIntroDesc"), tl("cleanerShortDesc"), RoleId.Cleaner);
            warlock = new RoleInfo(tl("warlock"), Warlock.color, tl("warlockIntroDesc"), tl("warlockShortDesc"), RoleId.Warlock);
            bountyHunter = new RoleInfo(tl("bountyHunter"), BountyHunter.color, tl("bountyHunterIntroDesc"), tl("bountyHunterShortDesc"), RoleId.BountyHunter);
            detective = new RoleInfo(tl("detective"), Detective.color, tl("detectiveIntroDesc"), tl("detectiveShortDesc"), RoleId.Detective);
            timeMaster = new RoleInfo(tl("timeMaster"), TimeMaster.color, tl("timeMasterIntroDesc"), tl("timeMasterShortDesc"), RoleId.TimeMaster);
            medic = new RoleInfo(tl("medic"), Medic.color, tl("medicIntroDesc"), tl("medicShortDesc"), RoleId.Medic);
            shifter = new RoleInfo(tl("shifter"), Shifter.color, tl("shifterIntroDesc"), tl("shifterShortDesc"), RoleId.Shifter);
            swapper = new RoleInfo(tl("swapper"), Swapper.color, tl("swapperIntroDesc"), tl("swapperShortDesc"), RoleId.Swapper);
            seer = new RoleInfo(tl("seer"), Seer.color, tl("seerIntroDesc"), tl("seerShortDesc"), RoleId.Seer);
            hacker = new RoleInfo(tl("hacker"), Hacker.color, tl("hackerIntroDesc"), tl("hackerShortDesc"), RoleId.Hacker);
            niceMini = new RoleInfo(tl("niceMini"), Mini.color, tl("niceMiniIntroDesc"), tl("niceMiniShortDesc"), RoleId.Mini);
            evilMini = new RoleInfo(tl("evilMini"), Palette.ImpostorRed, tl("evilMiniIntroDesc"), tl("evilMiniShortDesc"), RoleId.Mini);
            tracker = new RoleInfo(tl("tracker"), Tracker.color, tl("trackerIntroDesc"), tl("trackerShortDesc"), RoleId.Tracker);
            snitch = new RoleInfo(tl("snitch"), Snitch.color, tl("snitchIntroDesc"), tl("snitchShortDesc"), RoleId.Snitch);
            jackal = new RoleInfo(tl("jackal"), Jackal.color, tl("jackalIntroDesc"), tl("jackalShortDesc"), RoleId.Jackal);
            sidekick = new RoleInfo(tl("sidekick"), Sidekick.color, tl("sidekickIntroDesc"), tl("sidekickShortDesc"), RoleId.Sidekick);
            spy = new RoleInfo(tl("spy"), Spy.color, tl("spyIntroDesc"), tl("spyShortDesc"), RoleId.Spy);
            securityGuard = new RoleInfo(tl("securityGuard"), SecurityGuard.color, tl("securityGuardIntroDesc"), tl("securityGuardShortDesc"), RoleId.SecurityGuard);
            arsonist = new RoleInfo(tl("arsonist"), Arsonist.color, tl("arsonistIntroDesc"), tl("arsonistShortDesc"), RoleId.Arsonist);
            goodGuesser = new RoleInfo(tl("goodGuesser"), Guesser.color, tl("goodGuesserIntroDesc"), tl("goodGuesserShortDesc"), RoleId.Guesser);
            badGuesser = new RoleInfo(tl("badGuesser"), Palette.ImpostorRed, tl("badGuesserIntroDesc"), tl("badGuesserShortDesc"), RoleId.Guesser);
            bait = new RoleInfo(tl("bait"), Bait.color, tl("baitIntroDesc"), tl("baitShortDesc"), RoleId.Bait);
            madmate = new RoleInfo(tl("madmate"), Madmate.color, tl("madmateIntroDesc"), tl("madmateShortDesc"), RoleId.Madmate);
            impostor = new RoleInfo(tl("impostor"), Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, tl("impostorIntroDesc")), tl("impostorShortDesc"), RoleId.Impostor);
            crewmate = new RoleInfo(tl("crewmate"), Color.white, tl("crewmateIntroDesc"), tl("crewmateShortDesc"), RoleId.Crewmate);
            lover = new RoleInfo(tl("lover"), Lovers.color, tl("loverIntroDesc"), tl("loverShortDesc"), RoleId.Lover);
            gm = new RoleInfo(tl("gm"), GM.color, tl("gmIntroDesc"), tl("gmShortDesc"), RoleId.GM);
            opportunist = new RoleInfo(tl("opportunist"), Opportunist.color, tl("oppIntroDesc"), tl("oppShortDesc"), RoleId.Opportunist);

            allRoleInfos = new List<RoleInfo>() {
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
                lover,
                jester,
                arsonist,
                jackal,
                sidekick,
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
                opportunist
            };
        }

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p) {
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
            if (p == Mini.mini) infos.Add(p.Data.IsImpostor ? evilMini : niceMini);
            if (p == Tracker.tracker) infos.Add(tracker);
            if (p == Snitch.snitch) infos.Add(snitch);
            if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
            if (p == Sidekick.sidekick) infos.Add(sidekick);
            if (p == Spy.spy) infos.Add(spy);
            if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
            if (p == Arsonist.arsonist) infos.Add(arsonist);
            if (p == Guesser.guesser) infos.Add(p.Data.IsImpostor ? badGuesser : goodGuesser);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Bait.bait) infos.Add(bait);
            if (p == Madmate.madmate) infos.Add(madmate);
            if (p == GM.gm) infos.Add(gm);
            if (p == Opportunist.opportunist) infos.Add(opportunist);

            // Default roles
            if (infos.Count == 0 && p.Data.IsImpostor) infos.Add(impostor); // Just Impostor
            if (infos.Count == 0 && !p.Data.IsImpostor) infos.Add(crewmate); // Just Crewmate

            // Modifier
            if (p == Lovers.lover1|| p == Lovers.lover2) infos.Add(lover);

            return infos;
        }
    }
}
