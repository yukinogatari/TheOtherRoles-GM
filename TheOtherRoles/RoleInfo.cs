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
        public RoleType roleId;

        private string nameKey;
        private CustomOption baseOption;

        RoleInfo(string name, Color color, CustomOption baseOption, RoleType roleId) {
            this.color = color;
            this.nameKey = name;
            this.baseOption = baseOption;
            this.roleId = roleId;
        }

        public static RoleInfo jester = new RoleInfo("jester", Jester.color, CustomOptionHolder.jesterSpawnRate, RoleType.Jester);
        public static RoleInfo mayor = new RoleInfo("mayor", Mayor.color, CustomOptionHolder.mayorSpawnRate, RoleType.Mayor);
        public static RoleInfo engineer = new RoleInfo("engineer", Engineer.color, CustomOptionHolder.engineerSpawnRate, RoleType.Engineer);
        public static RoleInfo sheriff = new RoleInfo("sheriff", Sheriff.color, CustomOptionHolder.sheriffSpawnRate, RoleType.Sheriff);
        public static RoleInfo lighter = new RoleInfo("lighter", Lighter.color, CustomOptionHolder.lighterSpawnRate, RoleType.Lighter);
        public static RoleInfo godfather = new RoleInfo("godfather", Godfather.color, CustomOptionHolder.mafiaSpawnRate, RoleType.Godfather);
        public static RoleInfo mafioso = new RoleInfo("mafioso", Mafioso.color, CustomOptionHolder.mafiaSpawnRate, RoleType.Mafioso);
        public static RoleInfo janitor = new RoleInfo("janitor", Janitor.color, CustomOptionHolder.mafiaSpawnRate, RoleType.Janitor);
        public static RoleInfo morphling = new RoleInfo("morphling", Morphling.color, CustomOptionHolder.morphlingSpawnRate, RoleType.Morphling);
        public static RoleInfo camouflager = new RoleInfo("camouflager", Camouflager.color, CustomOptionHolder.camouflagerSpawnRate, RoleType.Camouflager);
        public static RoleInfo vampire = new RoleInfo("vampire", Vampire.color, CustomOptionHolder.vampireSpawnRate, RoleType.Vampire);
        public static RoleInfo eraser = new RoleInfo("eraser", Eraser.color, CustomOptionHolder.eraserSpawnRate, RoleType.Eraser);
        public static RoleInfo trickster = new RoleInfo("trickster", Trickster.color, CustomOptionHolder.tricksterSpawnRate, RoleType.Trickster);
        public static RoleInfo cleaner = new RoleInfo("cleaner", Cleaner.color, CustomOptionHolder.cleanerSpawnRate, RoleType.Cleaner);
        public static RoleInfo warlock = new RoleInfo("warlock", Warlock.color, CustomOptionHolder.warlockSpawnRate, RoleType.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo("bountyHunter", BountyHunter.color, CustomOptionHolder.bountyHunterSpawnRate, RoleType.BountyHunter);
        public static RoleInfo detective = new RoleInfo("detective", Detective.color, CustomOptionHolder.detectiveSpawnRate, RoleType.Detective);
        public static RoleInfo timeMaster = new RoleInfo("timeMaster", TimeMaster.color, CustomOptionHolder.timeMasterSpawnRate, RoleType.TimeMaster);
        public static RoleInfo medic = new RoleInfo("medic", Medic.color, CustomOptionHolder.medicSpawnRate, RoleType.Medic);
        public static RoleInfo niceShifter = new RoleInfo("niceShifter", Shifter.color, CustomOptionHolder.shifterSpawnRate, RoleType.Shifter);
        public static RoleInfo corruptedShifter = new RoleInfo("corruptedShifter", Shifter.color, CustomOptionHolder.shifterSpawnRate, RoleType.Shifter);
        public static RoleInfo niceSwapper = new RoleInfo("niceSwapper", Swapper.color, CustomOptionHolder.swapperSpawnRate, RoleType.Swapper);
        public static RoleInfo evilSwapper = new RoleInfo("evilSwapper", Palette.ImpostorRed, CustomOptionHolder.swapperSpawnRate, RoleType.Swapper);
        public static RoleInfo seer = new RoleInfo("seer", Seer.color, CustomOptionHolder.seerSpawnRate, RoleType.Seer);
        public static RoleInfo hacker = new RoleInfo("hacker", Hacker.color, CustomOptionHolder.hackerSpawnRate, RoleType.Hacker);
        public static RoleInfo niceMini = new RoleInfo("niceMini", Mini.color, CustomOptionHolder.miniSpawnRate, RoleType.Mini);
        public static RoleInfo evilMini = new RoleInfo("evilMini", Palette.ImpostorRed, CustomOptionHolder.miniSpawnRate, RoleType.Mini);
        public static RoleInfo tracker = new RoleInfo("tracker", Tracker.color, CustomOptionHolder.trackerSpawnRate, RoleType.Tracker);
        public static RoleInfo snitch = new RoleInfo("snitch", Snitch.color, CustomOptionHolder.snitchSpawnRate, RoleType.Snitch);
        public static RoleInfo jackal = new RoleInfo("jackal", Jackal.color, CustomOptionHolder.jackalSpawnRate, RoleType.Jackal);
        public static RoleInfo sidekick = new RoleInfo("sidekick", Sidekick.color, CustomOptionHolder.jackalSpawnRate, RoleType.Sidekick);
        public static RoleInfo spy = new RoleInfo("spy", Spy.color, CustomOptionHolder.spySpawnRate, RoleType.Spy);
        public static RoleInfo securityGuard = new RoleInfo("securityGuard", SecurityGuard.color, CustomOptionHolder.securityGuardSpawnRate, RoleType.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo("arsonist", Arsonist.color, CustomOptionHolder.arsonistSpawnRate, RoleType.Arsonist);
        public static RoleInfo niceGuesser = new RoleInfo("niceGuesser", Guesser.color, CustomOptionHolder.guesserSpawnRate, RoleType.NiceGuesser);
        public static RoleInfo evilGuesser = new RoleInfo("evilGuesser", Palette.ImpostorRed, CustomOptionHolder.guesserSpawnRate, RoleType.EvilGuesser);
        public static RoleInfo bait = new RoleInfo("bait", Bait.color, CustomOptionHolder.baitSpawnRate, RoleType.Bait);
        public static RoleInfo madmate = new RoleInfo("madmate", Madmate.color, CustomOptionHolder.madmateSpawnRate, RoleType.Madmate);
        public static RoleInfo impostor = new RoleInfo("impostor", Palette.ImpostorRed,null, RoleType.Impostor);
        public static RoleInfo lawyer = new RoleInfo("lawyer", Lawyer.color, CustomOptionHolder.lawyerSpawnRate, RoleType.Lawyer);
        public static RoleInfo pursuer = new RoleInfo("pursuer", Pursuer.color, CustomOptionHolder.lawyerSpawnRate, RoleType.Pursuer);
        public static RoleInfo crewmate = new RoleInfo("crewmate", Color.white, null, RoleType.Crewmate);
        public static RoleInfo lovers = new RoleInfo("lovers", Lovers.color, CustomOptionHolder.loversSpawnRate, RoleType.Lovers);
        public static RoleInfo gm = new RoleInfo("gm", GM.color, CustomOptionHolder.gmEnabled, RoleType.GM);
        public static RoleInfo opportunist = new RoleInfo("opportunist", Opportunist.color, CustomOptionHolder.opportunistSpawnRate, RoleType.Opportunist);
        public static RoleInfo witch = new RoleInfo("witch", Witch.color, CustomOptionHolder.witchSpawnRate, RoleType.Witch);
        public static RoleInfo vulture = new RoleInfo("vulture", Vulture.color, CustomOptionHolder.vultureSpawnRate, RoleType.Vulture);
        public static RoleInfo medium = new RoleInfo("medium", Medium.color, CustomOptionHolder.mediumSpawnRate, RoleType.Medium);
        public static RoleInfo ninja = new RoleInfo("ninja", Ninja.color, CustomOptionHolder.ninjaSpawnRate, RoleType.Ninja);
        public static RoleInfo plagueDoctor = new RoleInfo("plagueDoctor", PlagueDoctor.color, CustomOptionHolder.plagueDoctorSpawnRate, RoleType.PlagueDoctor);
        public static RoleInfo nekoKabocha = new RoleInfo("nekoKabocha", NekoKabocha.color, CustomOptionHolder.nekoKabochaSpawnRate, RoleType.NekoKabocha);
        public static RoleInfo niceWatcher = new RoleInfo("niceWatcher", Watcher.color, CustomOptionHolder.watcherSpawnRate, RoleType.Watcher);
        public static RoleInfo evilWatcher = new RoleInfo("evilWatcher", Palette.ImpostorRed, CustomOptionHolder.watcherSpawnRate, RoleType.Watcher);
        public static RoleInfo serialKiller = new RoleInfo("serialKiller", SerialKiller.color, CustomOptionHolder.serialKillerSpawnRate, RoleType.SerialKiller);
        public static RoleInfo fox = new RoleInfo("fox", Fox.color, CustomOptionHolder.foxSpawnRate, RoleType.Fox);
        public static RoleInfo immoralist = new RoleInfo("immoralist", Immoralist.color, CustomOptionHolder.foxSpawnRate, RoleType.Immoralist);
        public static RoleInfo fortuneTeller = new RoleInfo("fortuneTeller", FortuneTeller.color, CustomOptionHolder.fortuneTellerSpawnRate, RoleType.FortuneTeller);

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
                serialKiller,
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
	            medium,
                plagueDoctor,
                nekoKabocha,
                niceWatcher,
                evilWatcher,
                fox,
                immoralist,
                fortuneTeller
            };

        public static string tl(string key)
        {
            return ModTranslation.getString(key);
        }

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, RoleType[] excludeRoles = null) {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            // Special roles
            if (p.isRole(RoleType.Jester)) infos.Add(jester);
            if (p.isRole(RoleType.Mayor)) infos.Add(mayor);
            if (p.isRole(RoleType.Engineer)) infos.Add(engineer);
            if (p.isRole(RoleType.Sheriff)) infos.Add(sheriff);
            if (p.isRole(RoleType.Lighter)) infos.Add(lighter);
            if (p.isRole(RoleType.Godfather)) infos.Add(godfather);
            if (p.isRole(RoleType.Mafioso)) infos.Add(mafioso);
            if (p.isRole(RoleType.Janitor)) infos.Add(janitor);
            if (p.isRole(RoleType.Morphling)) infos.Add(morphling);
            if (p.isRole(RoleType.Camouflager)) infos.Add(camouflager);
            if (p.isRole(RoleType.Vampire)) infos.Add(vampire);
            if (p.isRole(RoleType.Eraser)) infos.Add(eraser);
            if (p.isRole(RoleType.Trickster)) infos.Add(trickster);
            if (p.isRole(RoleType.Cleaner)) infos.Add(cleaner);
            if (p.isRole(RoleType.Warlock)) infos.Add(warlock);
            if (p.isRole(RoleType.Witch)) infos.Add(witch);
            if (p.isRole(RoleType.Detective)) infos.Add(detective);
            if (p.isRole(RoleType.TimeMaster)) infos.Add(timeMaster);
            if (p.isRole(RoleType.Medic)) infos.Add(medic);
            if (p.isRole(RoleType.Shifter)) infos.Add(Shifter.isNeutral ? corruptedShifter : niceShifter);
            if (p.isRole(RoleType.Swapper)) infos.Add(p.Data.Role.IsImpostor ? evilSwapper : niceSwapper);
            if (p.isRole(RoleType.Seer)) infos.Add(seer);
            if (p.isRole(RoleType.Hacker)) infos.Add(hacker);
            if (p.isRole(RoleType.Mini)) infos.Add(p.Data.Role.IsImpostor ? evilMini : niceMini);
            if (p.isRole(RoleType.Tracker)) infos.Add(tracker);
            if (p.isRole(RoleType.Snitch)) infos.Add(snitch);
            if (p.isRole(RoleType.Jackal) || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
            if (p.isRole(RoleType.Sidekick)) infos.Add(sidekick);
            if (p.isRole(RoleType.Spy)) infos.Add(spy);
            if (p.isRole(RoleType.SecurityGuard)) infos.Add(securityGuard);
            if (p.isRole(RoleType.Arsonist)) infos.Add(arsonist);
            if (p.isRole(RoleType.NiceGuesser)) infos.Add(niceGuesser);
            if (p.isRole(RoleType.EvilGuesser)) infos.Add(evilGuesser);
            if (p.isRole(RoleType.BountyHunter)) infos.Add(bountyHunter);
            if (p.isRole(RoleType.Bait)) infos.Add(bait);
            if (p.isRole(RoleType.Madmate)) infos.Add(madmate);
            if (p.isRole(RoleType.GM)) infos.Add(gm);
            if (p.isRole(RoleType.Opportunist)) infos.Add(opportunist);
            if (p.isRole(RoleType.Vulture)) infos.Add(vulture);
            if (p.isRole(RoleType.Medium)) infos.Add(medium);
            if (p.isRole(RoleType.Lawyer)) infos.Add(lawyer);
            if (p.isRole(RoleType.Pursuer)) infos.Add(pursuer);
            if (p.isRole(RoleType.Ninja)) infos.Add(ninja);
            if (p.isRole(RoleType.PlagueDoctor)) infos.Add(plagueDoctor);
            if (p.isRole(RoleType.NekoKabocha)) infos.Add(nekoKabocha);
            if (p.isRole(RoleType.SerialKiller)) infos.Add(serialKiller);
            if (p.isRole(RoleType.Watcher))
            {
                if (p.isImpostor()) infos.Add(evilWatcher);
                else infos.Add(niceWatcher);
            }
            if (p.isRole(RoleType.Fox)) infos.Add(fox);
            if (p.isRole(RoleType.Immoralist)) infos.Add(immoralist);
            if(p.isRole(RoleType.FortuneTeller))
            {
                if(PlayerControl.LocalPlayer.Data.IsDead || FortuneTeller.endGameFlag)
                {
                    infos.Add(fortuneTeller);
                }
                else
                {
                    var info = FortuneTeller.isCompletedNumTasks(p) ? fortuneTeller: crewmate;
                    infos.Add(info);
                }
            }


            // Default roles
            if (infos.Count == 0 && p.Data.Role.IsImpostor) infos.Add(impostor); // Just Impostor
            if (infos.Count == 0 && !p.Data.Role.IsImpostor) infos.Add(crewmate); // Just Crewmate

            // Modifier
            if (p.isLovers()) infos.Add(lovers);

            if (excludeRoles != null)
                infos.RemoveAll(x => excludeRoles.Contains(x.roleId));

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, RoleType[] excludeRoles = null) {
            string roleName = "";
            if (p?.Data?.Disconnected != false) return roleName;

            roleName = String.Join(" ", getRoleInfoForPlayer(p, excludeRoles).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p?.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target) roleName += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
            return roleName;
        }
    }
}
