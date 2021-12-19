using HarmonyLib;
using System.Linq;
using System;
using System.Collections.Generic;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using UnityEngine;

namespace TheOtherRoles
{
    class RoleInfo {
        public Color color;
        public virtual string name { get { return ModTranslation.getString(nameKey); } }
        public virtual string nameColored { get { return Helpers.cs(color, name); } }
        public virtual string introDescription { get { return ModTranslation.getString(nameKey + "IntroDesc"); } }
        public virtual string shortDescription { get { return ModTranslation.getString(nameKey + "ShortDesc"); } }
        public virtual string fullDescription { get { return ModTranslation.getString(nameKey + "FullDesc"); } }
        public virtual string blurb { get { return ModTranslation.getString(nameKey + "Blurb"); } }
        public virtual string roleOptions
        {
            get
            {
                return GameOptionsDataPatch.optionsToString(baseOption, true);
            }
        }
        public bool enabled { get { return baseOption == null || baseOption.enabled; } }
        public RoleId roleId;

        private string nameKey;
        private CustomOption baseOption;

        RoleInfo(string name, Color color, CustomOption baseOption, RoleId roleId) {
            this.color = color;
            this.nameKey = name;
            this.baseOption = baseOption;
            this.roleId = roleId;
        }

        public static RoleInfo jester = new RoleInfo("jester", Jester.color, CustomOptionHolder.jesterSpawnRate, RoleId.Jester);
        public static RoleInfo mayor = new RoleInfo("mayor", Mayor.color, CustomOptionHolder.mayorSpawnRate, RoleId.Mayor);
        public static RoleInfo engineer = new RoleInfo("engineer", Engineer.color, CustomOptionHolder.engineerSpawnRate, RoleId.Engineer);
        public static RoleInfo sheriff = new RoleInfo("sheriff", Sheriff.color, CustomOptionHolder.sheriffSpawnRate, RoleId.Sheriff);
        public static RoleInfo lighter = new RoleInfo("lighter", Lighter.color, CustomOptionHolder.lighterSpawnRate, RoleId.Lighter);
        public static RoleInfo godfather = new RoleInfo("godfather", Godfather.color, CustomOptionHolder.mafiaSpawnRate, RoleId.Godfather);
        public static RoleInfo mafioso = new RoleInfo("mafioso", Mafioso.color, CustomOptionHolder.mafiaSpawnRate, RoleId.Mafioso);
        public static RoleInfo janitor = new RoleInfo("janitor", Janitor.color, CustomOptionHolder.mafiaSpawnRate, RoleId.Janitor);
        public static RoleInfo morphling = new RoleInfo("morphling", Morphling.color, CustomOptionHolder.morphlingSpawnRate, RoleId.Morphling);
        public static RoleInfo camouflager = new RoleInfo("camouflager", Camouflager.color, CustomOptionHolder.camouflagerSpawnRate, RoleId.Camouflager);
        public static RoleInfo vampire = new RoleInfo("vampire", Vampire.color, CustomOptionHolder.vampireSpawnRate, RoleId.Vampire);
        public static RoleInfo eraser = new RoleInfo("eraser", Eraser.color, CustomOptionHolder.eraserSpawnRate, RoleId.Eraser);
        public static RoleInfo trickster = new RoleInfo("trickster", Trickster.color, CustomOptionHolder.tricksterSpawnRate, RoleId.Trickster);
        public static RoleInfo cleaner = new RoleInfo("cleaner", Cleaner.color, CustomOptionHolder.cleanerSpawnRate, RoleId.Cleaner);
        public static RoleInfo warlock = new RoleInfo("warlock", Warlock.color, CustomOptionHolder.warlockSpawnRate, RoleId.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo("bountyHunter", BountyHunter.color, CustomOptionHolder.bountyHunterSpawnRate, RoleId.BountyHunter);
        public static RoleInfo detective = new RoleInfo("detective", Detective.color, CustomOptionHolder.detectiveSpawnRate, RoleId.Detective);
        public static RoleInfo timeMaster = new RoleInfo("timeMaster", TimeMaster.color, CustomOptionHolder.timeMasterSpawnRate, RoleId.TimeMaster);
        public static RoleInfo medic = new RoleInfo("medic", Medic.color, CustomOptionHolder.medicSpawnRate, RoleId.Medic);
        public static RoleInfo niceShifter = new RoleInfo("niceShifter", Shifter.color, CustomOptionHolder.shifterSpawnRate, RoleId.Shifter);
        public static RoleInfo corruptedShifter = new RoleInfo("corruptedShifter", Shifter.color, CustomOptionHolder.shifterSpawnRate, RoleId.Shifter);
        public static RoleInfo niceSwapper = new RoleInfo("niceSwapper", Swapper.color, CustomOptionHolder.swapperSpawnRate, RoleId.Swapper);
        public static RoleInfo evilSwapper = new RoleInfo("evilSwapper", Palette.ImpostorRed, CustomOptionHolder.swapperSpawnRate, RoleId.Swapper);
        public static RoleInfo seer = new RoleInfo("seer", Seer.color, CustomOptionHolder.seerSpawnRate, RoleId.Seer);
        public static RoleInfo hacker = new RoleInfo("hacker", Hacker.color, CustomOptionHolder.hackerSpawnRate, RoleId.Hacker);
        public static RoleInfo niceMini = new RoleInfo("niceMini", Mini.color, CustomOptionHolder.miniSpawnRate, RoleId.Mini);
        public static RoleInfo evilMini = new RoleInfo("evilMini", Palette.ImpostorRed, CustomOptionHolder.miniSpawnRate, RoleId.Mini);
        public static RoleInfo tracker = new RoleInfo("tracker", Tracker.color, CustomOptionHolder.trackerSpawnRate, RoleId.Tracker);
        public static RoleInfo snitch = new RoleInfo("snitch", Snitch.color, CustomOptionHolder.snitchSpawnRate, RoleId.Snitch);
        public static RoleInfo jackal = new RoleInfo("jackal", Jackal.color, CustomOptionHolder.jackalSpawnRate, RoleId.Jackal);
        public static RoleInfo sidekick = new RoleInfo("sidekick", Sidekick.color, CustomOptionHolder.jackalSpawnRate, RoleId.Sidekick);
        public static RoleInfo spy = new RoleInfo("spy", Spy.color, CustomOptionHolder.spySpawnRate, RoleId.Spy);
        public static RoleInfo securityGuard = new RoleInfo("securityGuard", SecurityGuard.color, CustomOptionHolder.securityGuardSpawnRate, RoleId.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo("arsonist", Arsonist.color, CustomOptionHolder.arsonistSpawnRate, RoleId.Arsonist);
        public static RoleInfo niceGuesser = new RoleInfo("niceGuesser", Guesser.color, CustomOptionHolder.guesserSpawnRate, RoleId.NiceGuesser);
        public static RoleInfo evilGuesser = new RoleInfo("evilGuesser", Palette.ImpostorRed, CustomOptionHolder.guesserSpawnRate, RoleId.EvilGuesser);
        public static RoleInfo bait = new RoleInfo("bait", Bait.color, CustomOptionHolder.baitSpawnRate, RoleId.Bait);
        public static RoleInfo madmate = new RoleInfo("madmate", Madmate.color, CustomOptionHolder.madmateSpawnRate, RoleId.Madmate);
        public static RoleInfo impostor = new RoleInfo("impostor", Palette.ImpostorRed,null, RoleId.Impostor);
        public static RoleInfo lawyer = new RoleInfo("lawyer", Lawyer.color, CustomOptionHolder.lawyerSpawnRate, RoleId.Lawyer);
        public static RoleInfo pursuer = new RoleInfo("pursuer", Pursuer.color, CustomOptionHolder.lawyerSpawnRate, RoleId.Pursuer);
        public static RoleInfo crewmate = new RoleInfo("crewmate", Color.white, null, RoleId.Crewmate);
        public static RoleInfo lovers = new RoleInfo("lovers", Lovers.color, CustomOptionHolder.loversSpawnRate, RoleId.Lovers);
        public static RoleInfo gm = new RoleInfo("gm", GM.color, CustomOptionHolder.gmEnabled, RoleId.GM);
        public static RoleInfo opportunist = new RoleInfo("opportunist", Opportunist.color, CustomOptionHolder.opportunistSpawnRate, RoleId.Opportunist);
        public static RoleInfo witch = new RoleInfo("witch", Witch.color, CustomOptionHolder.witchSpawnRate, RoleId.Witch);
        public static RoleInfo vulture = new RoleInfo("vulture", Vulture.color, CustomOptionHolder.vultureSpawnRate, RoleId.Vulture);
        public static RoleInfo medium = new RoleInfo("medium", Medium.color, CustomOptionHolder.mediumSpawnRate, RoleId.Medium);
        public static RoleInfo ninja = new RoleInfo("ninja", Ninja.color, CustomOptionHolder.ninjaSpawnRate, RoleId.Ninja);

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
                witch,
                ninja,
                niceMini,
                evilMini,
                niceGuesser,
                evilGuesser,
                lovers,
                jester,
                arsonist,
                jackal,
                sidekick,
            	vulture,
                pursuer,
                lawyer,
                crewmate,
                niceShifter,
                corruptedShifter,
                mayor,
                engineer,
                sheriff,
                lighter,
                detective,
                timeMaster,
                medic,
                niceSwapper,
                evilSwapper,
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
            if (p == Witch.witch) infos.Add(witch);
            if (p == Detective.detective) infos.Add(detective);
            if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
            if (p == Medic.medic) infos.Add(medic);
            if (p == Shifter.shifter) infos.Add(Shifter.isNeutral ? corruptedShifter : niceShifter);
            if (p == Swapper.swapper) infos.Add(p.Data.Role.IsImpostor ? evilSwapper : niceSwapper);
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
            if (p == Guesser.niceGuesser) infos.Add(niceGuesser);
            if (p == Guesser.evilGuesser) infos.Add(evilGuesser);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Bait.bait) infos.Add(bait);
            if (p == Madmate.madmate) infos.Add(madmate);
            if (p == GM.gm) infos.Add(gm);
            if (p == Opportunist.opportunist) infos.Add(opportunist);
            if (p == Vulture.vulture) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);
            if (p == Lawyer.lawyer) infos.Add(lawyer);
            if (p == Pursuer.pursuer) infos.Add(pursuer);
            if (Ninja.isRole(p)) infos.Add(ninja);

            // Default roles
            if (infos.Count == 0 && p.Data.Role.IsImpostor) infos.Add(impostor); // Just Impostor
            if (infos.Count == 0 && !p.Data.Role.IsImpostor) infos.Add(crewmate); // Just Crewmate

            // Modifier
            if (p.isLovers()) infos.Add(lovers);

            if (excludeRoles != null)
                infos.RemoveAll(x => excludeRoles.Contains(x.roleId));

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, RoleId[] excludeRoles = null) {
            string roleName = "";
            if (p?.Data?.Disconnected != false) return roleName;

            roleName = String.Join(" ", getRoleInfoForPlayer(p, excludeRoles).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p?.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target) roleName += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
            return roleName;
        }
    }
}
