using HarmonyLib;
using System.Linq;
using System;
using System.Collections.Generic;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using UnityEngine;

namespace TheOtherRoles
{
    public class RoleInfo {
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

        public bool enabled { get { return Helpers.RolesEnabled && (this == crewmate || this == impostor || (baseOption != null && baseOption.enabled)); } }
        public RoleType roleType;

        private string nameKey;
        private CustomOption baseOption;

        RoleInfo(string name, Color color, CustomOption baseOption, RoleType roleType) {
            this.color = color;
            this.nameKey = name;
            this.baseOption = baseOption;
            this.roleType = roleType;
        }

        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>();
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
        public static RoleInfo niceShifter;
        public static RoleInfo chainShifter;
        public static RoleInfo niceSwapper;
        public static RoleInfo evilSwapper;
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
        public static RoleInfo niceGuesser;
        public static RoleInfo evilGuesser;
        public static RoleInfo bait;
        public static RoleInfo impostor;
        public static RoleInfo lawyer;
        public static RoleInfo pursuer;
        public static RoleInfo crewmate;
        public static RoleInfo lovers;
        public static RoleInfo gm;
        public static RoleInfo opportunist;
        public static RoleInfo witch;
        public static RoleInfo vulture;
        public static RoleInfo medium;
        public static RoleInfo ninja;
        public static RoleInfo plagueDoctor;
        public static RoleInfo nekoKabocha;
        public static RoleInfo niceWatcher;
        public static RoleInfo evilWatcher;
        public static RoleInfo serialKiller;
        public static RoleInfo fox;
        public static RoleInfo immoralist;
        public static RoleInfo fortuneTeller;
        public static RoleInfo sprinter;
        public static RoleInfo akujo;

        public static void Init() {
            jester = new RoleInfo("jester", Jester.color, CustomOptionHolder.jesterSpawnRate, RoleType.Jester);
            mayor = new RoleInfo("mayor", Mayor.color, CustomOptionHolder.mayorSpawnRate, RoleType.Mayor);
            engineer = new RoleInfo("engineer", Engineer.color, CustomOptionHolder.engineerSpawnRate, RoleType.Engineer);
            sheriff = new RoleInfo("sheriff", Sheriff.color, CustomOptionHolder.sheriffSpawnRate, RoleType.Sheriff);
            lighter = new RoleInfo("lighter", Lighter.color, CustomOptionHolder.lighterSpawnRate, RoleType.Lighter);
            godfather = new RoleInfo("godfather", Godfather.color, CustomOptionHolder.mafiaSpawnRate, RoleType.Godfather);
            mafioso = new RoleInfo("mafioso", Mafioso.color, CustomOptionHolder.mafiaSpawnRate, RoleType.Mafioso);
            janitor = new RoleInfo("janitor", Janitor.color, CustomOptionHolder.mafiaSpawnRate, RoleType.Janitor);
            morphling = new RoleInfo("morphling", Morphling.color, CustomOptionHolder.morphlingSpawnRate, RoleType.Morphling);
            camouflager = new RoleInfo("camouflager", Camouflager.color, CustomOptionHolder.camouflagerSpawnRate, RoleType.Camouflager);
            vampire = new RoleInfo("vampire", Vampire.color, CustomOptionHolder.vampireSpawnRate, RoleType.Vampire);
            eraser = new RoleInfo("eraser", Eraser.color, CustomOptionHolder.eraserSpawnRate, RoleType.Eraser);
            trickster = new RoleInfo("trickster", Trickster.color, CustomOptionHolder.tricksterSpawnRate, RoleType.Trickster);
            cleaner = new RoleInfo("cleaner", Cleaner.color, CustomOptionHolder.cleanerSpawnRate, RoleType.Cleaner);
            warlock = new RoleInfo("warlock", Warlock.color, CustomOptionHolder.warlockSpawnRate, RoleType.Warlock);
            bountyHunter = new RoleInfo("bountyHunter", BountyHunter.color, CustomOptionHolder.bountyHunterSpawnRate, RoleType.BountyHunter);
            detective = new RoleInfo("detective", Detective.color, CustomOptionHolder.detectiveSpawnRate, RoleType.Detective);
            timeMaster = new RoleInfo("timeMaster", TimeMaster.color, CustomOptionHolder.timeMasterSpawnRate, RoleType.TimeMaster);
            medic = new RoleInfo("medic", Medic.color, CustomOptionHolder.medicSpawnRate, RoleType.Medic);
            niceShifter = new RoleInfo("niceShifter", Shifter.color, CustomOptionHolder.shifterSpawnRate, RoleType.Shifter);
            chainShifter = new RoleInfo("corruptedShifter", Shifter.color, CustomOptionHolder.shifterSpawnRate, RoleType.Shifter);
            niceSwapper = new RoleInfo("niceSwapper", Swapper.color, CustomOptionHolder.swapperSpawnRate, RoleType.Swapper);
            evilSwapper = new RoleInfo("evilSwapper", Palette.ImpostorRed, CustomOptionHolder.swapperSpawnRate, RoleType.Swapper);
            seer = new RoleInfo("seer", Seer.color, CustomOptionHolder.seerSpawnRate, RoleType.Seer);
            hacker = new RoleInfo("hacker", Hacker.color, CustomOptionHolder.hackerSpawnRate, RoleType.Hacker);
            niceMini = new RoleInfo("niceMini", Mini.color, CustomOptionHolder.miniSpawnRate, RoleType.Mini);
            evilMini = new RoleInfo("evilMini", Palette.ImpostorRed, CustomOptionHolder.miniSpawnRate, RoleType.Mini);
            tracker = new RoleInfo("tracker", Tracker.color, CustomOptionHolder.trackerSpawnRate, RoleType.Tracker);
            snitch = new RoleInfo("snitch", Snitch.color, CustomOptionHolder.snitchSpawnRate, RoleType.Snitch);
            jackal = new RoleInfo("jackal", Jackal.color, CustomOptionHolder.jackalSpawnRate, RoleType.Jackal);
            sidekick = new RoleInfo("sidekick", Sidekick.color, CustomOptionHolder.jackalSpawnRate, RoleType.Sidekick);
            spy = new RoleInfo("spy", Spy.color, CustomOptionHolder.spySpawnRate, RoleType.Spy);
            securityGuard = new RoleInfo("securityGuard", SecurityGuard.color, CustomOptionHolder.securityGuardSpawnRate, RoleType.SecurityGuard);
            arsonist = new RoleInfo("arsonist", Arsonist.color, CustomOptionHolder.arsonistSpawnRate, RoleType.Arsonist);
            niceGuesser = new RoleInfo("niceGuesser", Guesser.color, CustomOptionHolder.guesserSpawnRate, RoleType.NiceGuesser);
            evilGuesser = new RoleInfo("evilGuesser", Palette.ImpostorRed, CustomOptionHolder.guesserSpawnRate, RoleType.EvilGuesser);
            bait = new RoleInfo("bait", Bait.color, CustomOptionHolder.baitSpawnRate, RoleType.Bait);
            impostor = new RoleInfo("impostor", Palette.ImpostorRed, null, RoleType.Impostor);
            lawyer = new RoleInfo("lawyer", Lawyer.color, CustomOptionHolder.lawyerSpawnRate, RoleType.Lawyer);
            pursuer = new RoleInfo("pursuer", Pursuer.color, CustomOptionHolder.lawyerSpawnRate, RoleType.Pursuer);
            crewmate = new RoleInfo("crewmate", Color.white, null, RoleType.Crewmate);
            lovers = new RoleInfo("lovers", Lovers.color, CustomOptionHolder.loversSpawnRate, RoleType.Lovers);
            gm = new RoleInfo("gm", GM.color, CustomOptionHolder.gmEnabled, RoleType.GM);
            opportunist = new RoleInfo("opportunist", Opportunist.color, CustomOptionHolder.opportunistSpawnRate, RoleType.Opportunist);
            witch = new RoleInfo("witch", Witch.color, CustomOptionHolder.witchSpawnRate, RoleType.Witch);
            vulture = new RoleInfo("vulture", Vulture.color, CustomOptionHolder.vultureSpawnRate, RoleType.Vulture);
            medium = new RoleInfo("medium", Medium.color, CustomOptionHolder.mediumSpawnRate, RoleType.Medium);
            ninja = new RoleInfo("ninja", Ninja.color, CustomOptionHolder.ninjaSpawnRate, RoleType.Ninja);
            plagueDoctor = new RoleInfo("plagueDoctor", PlagueDoctor.color, CustomOptionHolder.plagueDoctorSpawnRate, RoleType.PlagueDoctor);
            nekoKabocha = new RoleInfo("nekoKabocha", NekoKabocha.color, CustomOptionHolder.nekoKabochaSpawnRate, RoleType.NekoKabocha);
            niceWatcher = new RoleInfo("niceWatcher", Watcher.color, CustomOptionHolder.watcherSpawnRate, RoleType.Watcher);
            evilWatcher = new RoleInfo("evilWatcher", Palette.ImpostorRed, CustomOptionHolder.watcherSpawnRate, RoleType.Watcher);
            serialKiller = new RoleInfo("serialKiller", SerialKiller.color, CustomOptionHolder.serialKillerSpawnRate, RoleType.SerialKiller);
            fox = new RoleInfo("fox", Fox.color, CustomOptionHolder.foxSpawnRate, RoleType.Fox);
            immoralist = new RoleInfo("immoralist", Immoralist.color, CustomOptionHolder.foxSpawnRate, RoleType.Immoralist);
            fortuneTeller = new RoleInfo("fortuneTeller", FortuneTeller.color, CustomOptionHolder.fortuneTellerSpawnRate, RoleType.FortuneTeller);
            sprinter = new RoleInfo("sprinter", Sprinter.color, CustomOptionHolder.sprinterSpawnRate, RoleType.Sprinter);
            akujo = new RoleInfo("akujo", Akujo.color, CustomOptionHolder.akujoSpawnRate, RoleType.Akujo);

            allRoleInfos = new List<RoleInfo>() {
                // Impostor roles
                impostor,
                godfather,
                mafioso,
                janitor,
                evilMini,
                evilGuesser,
                evilSwapper,
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
                nekoKabocha,
                serialKiller,
                evilWatcher,

                // Crew roles
                crewmate,
                niceMini,
                niceGuesser,
                niceShifter,
                mayor,
                engineer,
                sheriff,
                lighter,
                detective,
                timeMaster,
                medic,
                niceSwapper,
                seer,
                hacker,
                tracker,
                snitch,
                spy,
                securityGuard,
                bait,
                medium,
                fortuneTeller,
                niceWatcher,
                sprinter,

                // Neutral/other roles
                lovers,
                jester,
                arsonist,
                jackal,
                sidekick,
                opportunist,
                chainShifter,
                vulture,
                lawyer,
                pursuer,
                plagueDoctor,
                fox,
                immoralist,
                akujo,

                // GM
                gm,
            };
        }

        public static string tl(string key)
        {
            return ModTranslation.getString(key);
        }

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, RoleType[] excludeRoles = null, bool includeHidden = false) {
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
            if (p.isRole(RoleType.Shifter)) infos.Add(Shifter.isNeutral ? chainShifter : niceShifter);
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
            if (p.isRole(RoleType.FortuneTeller))
            {
                if (includeHidden || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    infos.Add(fortuneTeller);
                }
                else
                {
                    var info = FortuneTeller.isCompletedNumTasks(p) ? fortuneTeller: crewmate;
                    infos.Add(info);
                }
            }
            if (p.isRole(RoleType.Sprinter)) infos.Add(sprinter);
            if (p.isRole(RoleType.Akujo)) infos.Add(akujo);

            // Default roles
            if (infos.Count == 0 && p.Data.Role.IsImpostor) infos.Add(impostor); // Just Impostor
            if (infos.Count == 0 && !p.Data.Role.IsImpostor) infos.Add(crewmate); // Just Crewmate

            // Modifier
            if (p.isLovers()) infos.Add(lovers);

            if (excludeRoles != null)
                infos.RemoveAll(x => excludeRoles.Contains(x.roleType));

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, RoleType[] excludeRoles = null, bool includeHidden = false) {
            if (p?.Data?.Disconnected != false) return "";

            var roleInfo = getRoleInfoForPlayer(p, excludeRoles, includeHidden);
            string roleText = String.Join(" ", roleInfo.Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p?.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target) roleText += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
            roleText = p.modifyRoleText(roleText, roleInfo, useColors, includeHidden);

            return roleText;
        }
    }
}
