using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Reflection;
using System.Text;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles {

    public class CustomOptionHolder {
        public static string[] rates = new string[]{"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"};
        public static string[] presets = new string[]{"preset1", "preset2", "preset3", "preset4", "preset5" };

        public static CustomOption presetSelection;
        public static CustomOption crewmateRolesCountMin;
        public static CustomOption crewmateRolesCountMax;
        public static CustomOption neutralRolesCountMin;
        public static CustomOption neutralRolesCountMax;
        public static CustomOption impostorRolesCountMin;
        public static CustomOption impostorRolesCountMax;

        public static CustomOption mafiaSpawnRate;
        public static CustomOption janitorCooldown;

        public static CustomOption morphlingSpawnRate;
        public static CustomOption morphlingCooldown;
        public static CustomOption morphlingDuration;

        public static CustomOption camouflagerSpawnRate;
        public static CustomOption camouflagerCooldown;
        public static CustomOption camouflagerDuration;

        public static CustomOption vampireSpawnRate;
        public static CustomOption vampireKillDelay;
        public static CustomOption vampireCooldown;
        public static CustomOption vampireCanKillNearGarlics;

        public static CustomOption eraserSpawnRate;
        public static CustomOption eraserCooldown;
        public static CustomOption eraserCanEraseAnyone;

        public static CustomOption miniSpawnRate;
        public static CustomOption miniGrowingUpDuration;

        public static CustomOption loversSpawnRate;
        public static CustomOption loversImpLoverRate;
        public static CustomOption loversBothDie;
        public static CustomOption loversCanHaveAnotherRole;
        public static CustomOption loversWinWithCrew;
        public static CustomOption loversTasksCount;

        public static CustomOption guesserSpawnRate;
        public static CustomOption guesserIsImpGuesserRate;
        public static CustomOption guesserNumberOfShots;

        public static CustomOption jesterSpawnRate;
        public static CustomOption jesterCanCallEmergency;
        public static CustomOption jesterCanSabotage;

        public static CustomOption arsonistSpawnRate;
        public static CustomOption arsonistCooldown;
        public static CustomOption arsonistDuration;

        public static CustomOption jackalSpawnRate;
        public static CustomOption jackalKillCooldown;
        public static CustomOption jackalCreateSidekickCooldown;
        public static CustomOption jackalCanUseVents;
        public static CustomOption jackalCanCreateSidekick;
        public static CustomOption sidekickPromotesToJackal;
        public static CustomOption sidekickCanKill;
        public static CustomOption sidekickCanUseVents;
        public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
        public static CustomOption jackalCanCreateSidekickFromImpostor;
        public static CustomOption jackalAndSidekickHaveImpostorVision;

        public static CustomOption bountyHunterSpawnRate;
        public static CustomOption bountyHunterBountyDuration;
        public static CustomOption bountyHunterReducedCooldown;
        public static CustomOption bountyHunterPunishmentTime;
        public static CustomOption bountyHunterShowArrow;
        public static CustomOption bountyHunterArrowUpdateIntervall;

        public static CustomOption shifterSpawnRate;
        public static CustomOption shifterShiftsModifiers;

        public static CustomOption mayorSpawnRate;

        public static CustomOption engineerSpawnRate;

        public static CustomOption sheriffSpawnRate;
        public static CustomOption sheriffCooldown;
        public static CustomOption sheriffNumShots;
        public static CustomOption sheriffCanKillNeutrals;

        public static CustomOption lighterSpawnRate;
        public static CustomOption lighterModeLightsOnVision;
        public static CustomOption lighterModeLightsOffVision;
        public static CustomOption lighterCooldown;
        public static CustomOption lighterDuration;

        public static CustomOption detectiveSpawnRate;
        public static CustomOption detectiveAnonymousFootprints;
        public static CustomOption detectiveFootprintIntervall;
        public static CustomOption detectiveFootprintDuration;
        public static CustomOption detectiveReportNameDuration;
        public static CustomOption detectiveReportColorDuration;

        public static CustomOption timeMasterSpawnRate;
        public static CustomOption timeMasterCooldown;
        public static CustomOption timeMasterRewindTime;
        public static CustomOption timeMasterShieldDuration;

        public static CustomOption medicSpawnRate;
        public static CustomOption medicShowShielded;
        public static CustomOption medicShowAttemptToShielded;
        public static CustomOption medicSetShieldAfterMeeting;

        public static CustomOption swapperSpawnRate;
        public static CustomOption swapperCanCallEmergency;
        public static CustomOption swapperCanOnlySwapOthers;

        public static CustomOption seerSpawnRate;
        public static CustomOption seerMode;
        public static CustomOption seerSoulDuration;
        public static CustomOption seerLimitSoulDuration;

        public static CustomOption hackerSpawnRate;
        public static CustomOption hackerCooldown;
        public static CustomOption hackerHackeringDuration;
        public static CustomOption hackerOnlyColorType;

        public static CustomOption trackerSpawnRate;
        public static CustomOption trackerUpdateIntervall;
        public static CustomOption trackerResetTargetAfterMeeting;

        public static CustomOption snitchSpawnRate;
        public static CustomOption snitchLeftTasksForReveal;
        public static CustomOption snitchIncludeTeamJackal;
        public static CustomOption snitchTeamJackalUseDifferentArrowColor;

        public static CustomOption spySpawnRate;
        public static CustomOption spyCanDieToSheriff;
        public static CustomOption spyImpostorsCanKillAnyone;
        public static CustomOption spyCanEnterVents;
        public static CustomOption spyHasImpostorVision;

        public static CustomOption tricksterSpawnRate;
        public static CustomOption tricksterPlaceBoxCooldown;
        public static CustomOption tricksterLightsOutCooldown;
        public static CustomOption tricksterLightsOutDuration;

        public static CustomOption cleanerSpawnRate;
        public static CustomOption cleanerCooldown;
        
        public static CustomOption warlockSpawnRate;
        public static CustomOption warlockCooldown;
        public static CustomOption warlockRootTime;

        public static CustomOption securityGuardSpawnRate;
        public static CustomOption securityGuardCooldown;
        public static CustomOption securityGuardTotalScrews;
        public static CustomOption securityGuardCamPrice;
        public static CustomOption securityGuardVentPrice;

        public static CustomOption baitSpawnRate;
        public static CustomOption baitHighlightAllVents;
        public static CustomOption baitReportDelay;

        public static CustomOption madmateSpawnRate;
        public static CustomOption madmateCanDieToSheriff;
        public static CustomOption madmateCanEnterVents;
        public static CustomOption madmateHasImpostorVision;
        public static CustomOption madmateCanSabotage;
        public static CustomOption madmateCanFixComm;

        public static CustomOption opportunistSpawnRate;

        public static CustomOption gmEnabled;
        public static CustomOption gmIsHost;
        public static CustomOption gmHasTasks;
        public static CustomOption gmDiesAtStart;
        public static CustomOption gmHideSettings;
        public static CustomOption gmCanWarp;
        public static CustomOption gmCanKill;

        public static CustomOption specialOptions;
        public static CustomOption maxNumberOfMeetings;
        public static CustomOption blockSkippingInEmergencyMeetings;
        public static CustomOption noVoteIsSelfVote;
        public static CustomOption hidePlayerNames;

        public static CustomOption disableAdmin;
        public static CustomOption disableCameras;
        public static CustomOption disableVitals;
        public static CustomOption disableVents;

        internal static Dictionary<byte, byte[]> blockedRolePairings = new Dictionary<byte, byte[]>();

        public static string cs(Color c, string s) {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
 
        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void Load() {

            // Role Options
            presetSelection = CustomOption.Create(0, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, null, true);

            // Using new id's for the options to not break compatibilty with older versions
            crewmateRolesCountMin = CustomOption.Create(300, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMin"), 0f, 0f, 15f, 1f, null, true);
            crewmateRolesCountMax = CustomOption.Create(301, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMax"), 0f, 0f, 15f, 1f);
            neutralRolesCountMin = CustomOption.Create(302, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMin"), 0f, 0f, 15f, 1f);
            neutralRolesCountMax = CustomOption.Create(303, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMax"), 0f, 0f, 15f, 1f);
            impostorRolesCountMin = CustomOption.Create(304, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMin"), 0f, 0f, 15f, 1f);
            impostorRolesCountMax = CustomOption.Create(305, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMax"), 0f, 0f, 15f, 1f);


            gmEnabled = CustomOption.Create(400, cs(GM.color, "gmEnabled"), false, null, true);
            gmIsHost = CustomOption.Create(401, "gmIsHost", true, gmEnabled);
            //gmHasTasks = CustomOption.Create(402, "gmHasTasks", false, gmEnabled);
            gmCanWarp = CustomOption.Create(405, "gmCanWarp", true, gmEnabled);
            gmCanKill = CustomOption.Create(406, "gmCanKill", false, gmEnabled);
            gmHideSettings = CustomOption.Create(403, "gmHideSettings", true, gmEnabled);
            gmDiesAtStart = CustomOption.Create(404, "gmDiesAtStart", true, gmEnabled);


            mafiaSpawnRate = CustomOption.Create(10, cs(Janitor.color, "mafiaSpawnRate"), rates, null, true);
            janitorCooldown = CustomOption.Create(11, "janitorCooldown", 30f, 10f, 60f, 2.5f, mafiaSpawnRate, format : "unitSeconds");

            morphlingSpawnRate = CustomOption.Create(20, cs(Morphling.color, "morphlingSpawnRate"), rates, null, true);
            morphlingCooldown = CustomOption.Create(21, "morphlingCooldown", 30f, 10f, 60f, 2.5f, morphlingSpawnRate, format: "unitSeconds");
            morphlingDuration = CustomOption.Create(22, "morphlingDuration", 10f, 1f, 20f, 0.5f, morphlingSpawnRate, format: "unitSeconds");

            camouflagerSpawnRate = CustomOption.Create(30, cs(Camouflager.color, "camouflagerSpawnRate"), rates, null, true);
            camouflagerCooldown = CustomOption.Create(31, "camouflagerCooldown", 30f, 10f, 60f, 2.5f, camouflagerSpawnRate, format: "unitSeconds");
            camouflagerDuration = CustomOption.Create(32, "camouflagerDuration", 10f, 1f, 20f, 0.5f, camouflagerSpawnRate, format: "unitSeconds");

            vampireSpawnRate = CustomOption.Create(40, cs(Vampire.color, "vampireSpawnRate"), rates, null, true);
            vampireKillDelay = CustomOption.Create(41, "vampireKillDelay", 10f, 1f, 20f, 1f, vampireSpawnRate, format: "unitSeconds");
            vampireCooldown = CustomOption.Create(42, "vampireCooldown", 30f, 10f, 60f, 2.5f, vampireSpawnRate, format: "unitSeconds");
            vampireCanKillNearGarlics = CustomOption.Create(43, "vampireCanKillNearGarlics", true, vampireSpawnRate);

            eraserSpawnRate = CustomOption.Create(230, cs(Eraser.color, "eraserSpawnRate"), rates, null, true);
            eraserCooldown = CustomOption.Create(231, "eraserCooldown", 30f, 10f, 120f, 5f, eraserSpawnRate, format: "unitSeconds");
            eraserCanEraseAnyone = CustomOption.Create(232, "eraserCanEraseAnyone", false, eraserSpawnRate);

            tricksterSpawnRate = CustomOption.Create(250, cs(Trickster.color, "tricksterSpawnRate"), rates, null, true);
            tricksterPlaceBoxCooldown = CustomOption.Create(251, "tricksterPlaceBoxCooldown", 10f, 0f, 30f, 2.5f, tricksterSpawnRate, format: "unitSeconds");
            tricksterLightsOutCooldown = CustomOption.Create(252, "tricksterLightsOutCooldown", 30f, 10f, 60f, 5f, tricksterSpawnRate, format: "unitSeconds");
            tricksterLightsOutDuration = CustomOption.Create(253, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, tricksterSpawnRate, format: "unitSeconds");

            cleanerSpawnRate = CustomOption.Create(260, cs(Cleaner.color, "cleanerSpawnRate"), rates, null, true);
            cleanerCooldown = CustomOption.Create(261, "cleanerCooldown", 30f, 10f, 60f, 2.5f, cleanerSpawnRate, format: "unitSeconds");

            warlockSpawnRate = CustomOption.Create(270, cs(Warlock.color, "warlockSpawnRate"), rates, null, true);
            warlockCooldown = CustomOption.Create(271, "warlockCooldown", 30f, 10f, 60f, 2.5f, warlockSpawnRate, format: "unitSeconds");
            warlockRootTime = CustomOption.Create(272, "warlockRootTime", 5f, 0f, 15f, 1f, warlockSpawnRate, format: "unitSeconds");

            bountyHunterSpawnRate = CustomOption.Create(320, cs(BountyHunter.color, "bountyHunterSpawnRate"), rates, null, true);
            bountyHunterBountyDuration = CustomOption.Create(321, "bountyHunterBountyDuration", 60f, 10f, 180f, 10f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterReducedCooldown = CustomOption.Create(322, "bountyHunterReducedCooldown", 2.5f, 0f, 30f, 2.5f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterPunishmentTime = CustomOption.Create(323, "bountyHunterPunishmentTime", 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterShowArrow = CustomOption.Create(324, "bountyHunterShowArrow", true, bountyHunterSpawnRate);
            bountyHunterArrowUpdateIntervall = CustomOption.Create(325, "bountyHunterArrowUpdateIntervall", 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow, format: "unitSeconds");


            miniSpawnRate = CustomOption.Create(180, cs(Mini.color, "miniSpawnRate"), rates, null, true);
            miniGrowingUpDuration = CustomOption.Create(181, "miniGrowingUpDuration", 400f, 100f, 1500f, 100f, miniSpawnRate, format: "unitSeconds");

            loversSpawnRate = CustomOption.Create(50, cs(Lovers.color, "loversSpawnRate"), rates, null, true);
            loversImpLoverRate = CustomOption.Create(51, "loversImpLoverRate", rates, loversSpawnRate);
            loversBothDie = CustomOption.Create(52, "loversBothDie", true, loversSpawnRate);
            loversCanHaveAnotherRole = CustomOption.Create(53, "loversCanHaveAnotherRole", true, loversSpawnRate);
            loversWinWithCrew = CustomOption.Create(54, "loversWinWithCrew", false, loversSpawnRate);
            loversTasksCount = CustomOption.Create(55, "loversTasksCount", false, loversSpawnRate);

            guesserSpawnRate = CustomOption.Create(310, cs(Guesser.color, "guesserSpawnRate"), rates, null, true);
            guesserIsImpGuesserRate = CustomOption.Create(311, "guesserIsImpGuesserRate", rates, guesserSpawnRate);
            guesserNumberOfShots = CustomOption.Create(312, "guesserNumberOfShots", 2f, 1f, 15f, 1f, guesserSpawnRate, format: "unitShots");

            jesterSpawnRate = CustomOption.Create(60, cs(Jester.color, "jesterSpawnRate"), rates, null, true);
            jesterCanCallEmergency = CustomOption.Create(61, "jesterCanCallEmergency", true, jesterSpawnRate);
            jesterCanSabotage = CustomOption.Create(62, "jesterCanSabotage", true, jesterSpawnRate);

            arsonistSpawnRate = CustomOption.Create(290, cs(Arsonist.color, "arsonistSpawnRate"), rates, null, true);
            arsonistCooldown = CustomOption.Create(291, "arsonistCooldown", 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate, format: "unitSeconds");
            arsonistDuration = CustomOption.Create(292, "arsonistDuration", 3f, 1f, 10f, 1f, arsonistSpawnRate, format: "unitSeconds");

            jackalSpawnRate = CustomOption.Create(220, cs(Jackal.color, "jackalSpawnRate"), rates, null, true);
            jackalKillCooldown = CustomOption.Create(221, "jackalKillCooldown", 30f, 10f, 60f, 2.5f, jackalSpawnRate, format: "unitSeconds");
            jackalCanUseVents = CustomOption.Create(223, "jackalCanUseVents", true, jackalSpawnRate);
            jackalAndSidekickHaveImpostorVision = CustomOption.Create(430, "jackalAndSidekickHaveImpostorVision", false, jackalSpawnRate);
            jackalCanCreateSidekick = CustomOption.Create(224, "jackalCanCreateSidekick", false, jackalSpawnRate);
            jackalCreateSidekickCooldown = CustomOption.Create(222, "jackalCreateSidekickCooldown", 30f, 10f, 60f, 2.5f, jackalCanCreateSidekick, format: "unitSeconds");
            sidekickPromotesToJackal = CustomOption.Create(225, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
            sidekickCanKill = CustomOption.Create(226, "sidekickCanKill", false, jackalCanCreateSidekick);
            sidekickCanUseVents = CustomOption.Create(227, "sidekickCanUseVents", true, jackalCanCreateSidekick);
            jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(228, "jackalPromotedFromSidekickCanCreateSidekick", true, jackalCanCreateSidekick);
            jackalCanCreateSidekickFromImpostor = CustomOption.Create(229, "jackalCanCreateSidekickFromImpostor", true, jackalCanCreateSidekick);

            shifterSpawnRate = CustomOption.Create(70, cs(Shifter.color, "shifterSpawnRate"), rates, null, true);
            shifterShiftsModifiers = CustomOption.Create(71, "shifterShiftsModifiers", false, shifterSpawnRate);

            mayorSpawnRate = CustomOption.Create(80, cs(Mayor.color, "mayorSpawnRate"), rates, null, true);

            engineerSpawnRate = CustomOption.Create(90, cs(Engineer.color, "engineerSpawnRate"), rates, null, true);

            sheriffSpawnRate = CustomOption.Create(100, cs(Sheriff.color, "sheriffSpawnRate"), rates, null, true);
            sheriffCooldown = CustomOption.Create(101, "sheriffCooldown", 30f, 10f, 60f, 2.5f, sheriffSpawnRate, format: "unitSeconds");
            sheriffNumShots = CustomOption.Create(103, "sheriffNumShots", 2f, 1f, 15f, 1f, sheriffSpawnRate, format: "unitShots");
            sheriffCanKillNeutrals = CustomOption.Create(102, "sheriffCanKillNeutrals", false, sheriffSpawnRate);


            lighterSpawnRate = CustomOption.Create(110, cs(Lighter.color, "lighterSpawnRate"), rates, null, true);
            lighterModeLightsOnVision = CustomOption.Create(111, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterModeLightsOffVision = CustomOption.Create(112, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterCooldown = CustomOption.Create(113, "lighterCooldown", 30f, 5f, 120f, 5f, lighterSpawnRate, format: "unitSeconds");
            lighterDuration = CustomOption.Create(114, "lighterDuration", 5f, 2.5f, 60f, 2.5f, lighterSpawnRate, format: "unitSeconds");

            detectiveSpawnRate = CustomOption.Create(120, cs(Detective.color, "detectiveSpawnRate"), rates, null, true);
            detectiveAnonymousFootprints = CustomOption.Create(121, "detectiveAnonymousFootprints", false, detectiveSpawnRate);
            detectiveFootprintIntervall = CustomOption.Create(122, "detectiveFootprintIntervall", 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, format: "unitSeconds");
            detectiveFootprintDuration = CustomOption.Create(123, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, format: "unitSeconds");
            detectiveReportNameDuration = CustomOption.Create(124, "detectiveReportNameDuration", 0, 0, 60, 2.5f, detectiveSpawnRate, format: "unitSeconds");
            detectiveReportColorDuration = CustomOption.Create(125, "detectiveReportColorDuration", 20, 0, 120, 2.5f, detectiveSpawnRate, format: "unitSeconds");

            timeMasterSpawnRate = CustomOption.Create(130, cs(TimeMaster.color, "timeMasterSpawnRate"), rates, null, true);
            timeMasterCooldown = CustomOption.Create(131, "timeMasterCooldown", 30f, 10f, 120f, 2.5f, timeMasterSpawnRate, format: "unitSeconds");
            timeMasterRewindTime = CustomOption.Create(132, "timeMasterRewindTime", 3f, 1f, 10f, 1f, timeMasterSpawnRate, format: "unitSeconds");
            timeMasterShieldDuration = CustomOption.Create(133, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, timeMasterSpawnRate, format: "unitSeconds");

            medicSpawnRate = CustomOption.Create(140, cs(Medic.color, "medicSpawnRate"), rates, null, true);
            medicShowShielded = CustomOption.Create(143, "medicShowShielded", new string[] { "medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic" }, medicSpawnRate);
            medicShowAttemptToShielded = CustomOption.Create(144, "medicShowAttemptToShielded", false, medicSpawnRate);
            medicSetShieldAfterMeeting = CustomOption.Create(145, "medicSetShieldAfterMeeting", false, medicSpawnRate);

            swapperSpawnRate = CustomOption.Create(150, cs(Swapper.color, "swapperSpawnRate"), rates, null, true);
            swapperCanCallEmergency = CustomOption.Create(151, "swapperCanCallEmergency", false, swapperSpawnRate);
            swapperCanOnlySwapOthers = CustomOption.Create(152, "swapperCanOnlySwapOthers", false, swapperSpawnRate);

            seerSpawnRate = CustomOption.Create(160, cs(Seer.color, "seerSpawnRate"), rates, null, true);
            seerMode = CustomOption.Create(161, "seerMode", new string[] { "seerModeBoth", "seerModeFlash", "seerModeSouls" }, seerSpawnRate);
            seerLimitSoulDuration = CustomOption.Create(163, "seerLimitSoulDuration", false, seerSpawnRate);
            seerSoulDuration = CustomOption.Create(162, "seerSoulDuration", 15f, 0f, 60f, 5f, seerLimitSoulDuration, format: "unitSeconds");

            hackerSpawnRate = CustomOption.Create(170, cs(Hacker.color, "hackerSpawnRate"), rates, null, true);
            hackerCooldown = CustomOption.Create(171, "hackerCooldown", 30f, 0f, 60f, 5f, hackerSpawnRate, format: "unitSeconds");
            hackerHackeringDuration = CustomOption.Create(172, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate, format: "unitSeconds");
            hackerOnlyColorType = CustomOption.Create(173, "hackerOnlyColorType", false, hackerSpawnRate);

            trackerSpawnRate = CustomOption.Create(200, cs(Tracker.color, "trackerSpawnRate"), rates, null, true);
            trackerUpdateIntervall = CustomOption.Create(201, "trackerUpdateIntervall", 5f, 2.5f, 30f, 2.5f, trackerSpawnRate, format: "unitSeconds");
            trackerResetTargetAfterMeeting = CustomOption.Create(202, "trackerResetTargetAfterMeeting", false, trackerSpawnRate);

            snitchSpawnRate = CustomOption.Create(210, cs(Snitch.color, "snitchSpawnRate"), rates, null, true);
            snitchLeftTasksForReveal = CustomOption.Create(211, "snitchLeftTasksForReveal", 1f, 0f, 5f, 1f, snitchSpawnRate);
            snitchIncludeTeamJackal = CustomOption.Create(212, "snitchIncludeTeamJackal", false, snitchSpawnRate);
            snitchTeamJackalUseDifferentArrowColor = CustomOption.Create(213, "snitchTeamJackalUseDifferentArrowColor", true, snitchIncludeTeamJackal);

            spySpawnRate = CustomOption.Create(240, cs(Spy.color, "spySpawnRate"), rates, null, true);
            spyCanDieToSheriff = CustomOption.Create(241, "spyCanDieToSheriff", false, spySpawnRate);
            spyImpostorsCanKillAnyone = CustomOption.Create(242, "spyImpostorsCanKillAnyone", true, spySpawnRate);
            spyCanEnterVents = CustomOption.Create(243, "spyCanEnterVents", false, spySpawnRate);
            spyHasImpostorVision = CustomOption.Create(244, "spyHasImpostorVision", false, spySpawnRate);

            securityGuardSpawnRate = CustomOption.Create(280, cs(SecurityGuard.color, "securityGuardSpawnRate"), rates, null, true);
            securityGuardCooldown = CustomOption.Create(281, "securityGuardCooldown", 30f, 10f, 60f, 2.5f, securityGuardSpawnRate, format: "unitSeconds");
            securityGuardTotalScrews = CustomOption.Create(282, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardCamPrice = CustomOption.Create(283, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardVentPrice = CustomOption.Create(284, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");

            baitSpawnRate = CustomOption.Create(330, cs(Bait.color, "baitSpawnRate"), rates, null, true);
            baitHighlightAllVents = CustomOption.Create(331, "baitHighlightAllVents", false, baitSpawnRate);
            baitReportDelay = CustomOption.Create(332, "baitReportDelay", 0f, 0f, 10f, 1f, baitSpawnRate, format: "unitSeconds");

            madmateSpawnRate = CustomOption.Create(360, cs(Madmate.color, "madmateSpawnRate"), rates, null, true);
            madmateCanDieToSheriff = CustomOption.Create(361, "madmateCanDieToSheriff", false, madmateSpawnRate);
            madmateCanEnterVents = CustomOption.Create(362, "madmateCanEnterVents", false, madmateSpawnRate);
            madmateHasImpostorVision = CustomOption.Create(363, "madmateHasImpostorVision", false, madmateSpawnRate);
            madmateCanSabotage = CustomOption.Create(364, "madmateCanSabotage", false, madmateSpawnRate);
            madmateCanFixComm = CustomOption.Create(365, "madmateCanFixComm", true, madmateSpawnRate);

            opportunistSpawnRate = CustomOption.Create(366, cs(Opportunist.color, "opportunistSpawnRate"), rates, null, true);

            // Other options
            specialOptions = new CustomOptionBlank(null);
            maxNumberOfMeetings = CustomOption.Create(3, "maxNumberOfMeetings", 10, 0, 15, 1, specialOptions, true);
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, "blockSkippingInEmergencyMeetings", false, specialOptions);
            noVoteIsSelfVote = CustomOption.Create(5, "noVoteIsSelfVote", false, specialOptions);
            hidePlayerNames = CustomOption.Create(6, "hidePlayerNames", false, specialOptions);

            disableAdmin = CustomOption.Create(501, "disableAdmin", false, specialOptions);
            disableCameras = CustomOption.Create(502, "disableCameras", false, specialOptions);
            disableVitals = CustomOption.Create(503, "disableVitals", false, specialOptions);
            disableVents = CustomOption.Create(504, "disableVents", false, specialOptions);

            blockedRolePairings.Add((byte)RoleId.Vampire, new [] { (byte)RoleId.Warlock});
            blockedRolePairings.Add((byte)RoleId.Warlock, new [] { (byte)RoleId.Vampire});
            blockedRolePairings.Add((byte)RoleId.Spy, new [] { (byte)RoleId.Mini});
            blockedRolePairings.Add((byte)RoleId.Mini, new [] { (byte)RoleId.Spy});
            
        }
    }

    public class CustomOption {
        public static List<CustomOption> options = new List<CustomOption>();
        public static int preset = 0;

        public int id;
        public string name;
        public string format;
        public System.Object[] selections;

        public int defaultSelection;
        public ConfigEntry<int> entry;
        public int selection;
        public OptionBehaviour optionBehaviour;
        public CustomOption parent;
        public List<CustomOption> children;
        public bool isHeader;
        public bool isHidden;

        public virtual bool enabled
        {
            get
            {
                return this.getBool();
            }
        }

        // Option creation
        public CustomOption()
        {

        }

        public CustomOption(int id, string name,  System.Object[] selections, System.Object defaultValue, CustomOption parent, bool isHeader, bool isHidden, string format) {
            this.id = id;
            this.name = name;
            this.format = format;
            this.selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            this.defaultSelection = index >= 0 ? index : 0;
            this.parent = parent;
            this.isHeader = isHeader;
            this.isHidden = isHidden;

            this.children = new List<CustomOption>();
            if (parent != null) {
                parent.children.Add(this);
            }

            selection = 0;
            if (id > 0) {
                entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", id.ToString(), defaultSelection);
                selection = Mathf.Clamp(entry.Value, 0, selections.Length - 1);
            }
            options.Add(this);
        }

        public static CustomOption Create(int id, string name, string[] selections, CustomOption parent = null, bool isHeader = false, bool isHidden = false, string format = "") {
            return new CustomOption(id, name, selections, "", parent, isHeader, isHidden, format);
        }

        public static CustomOption Create(int id, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false, bool isHidden = false, string format = "") {
            List<float> selections = new List<float>();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, name, selections.Cast<object>().ToArray(), defaultValue, parent, isHeader, isHidden, format);
        }

        public static CustomOption Create(int id, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false, bool isHidden = false, string format = "") {
            return new CustomOption(id, name, new string[]{"optionOff", "optionOn"}, defaultValue ? "optionOn" : "optionOff", parent, isHeader, isHidden, format);
        }

        // Static behaviour

        public static void switchPreset(int newPreset) {
            CustomOption.preset = newPreset;
            foreach (CustomOption option in CustomOption.options) {
                if (option.id <= 0) continue;

                option.entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", option.id.ToString(), option.defaultSelection);
                option.selection = Mathf.Clamp(option.entry.Value, 0, option.selections.Length - 1);
                if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption) {
                    stringOption.oldValue = stringOption.Value = option.selection;
                    stringOption.ValueText.text = option.getString();
                }
            }
        }

        public static void ShareOptionSelections() {
            if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance?.AmHost == false && PlayerControl.LocalPlayer == null) return;
            foreach (CustomOption option in CustomOption.options) {
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareOptionSelection, Hazel.SendOption.Reliable);
                messageWriter.WritePacked((uint)option.id);
                messageWriter.WritePacked((uint)Convert.ToUInt32(option.selection));
                messageWriter.EndMessage();
            }
        }

        // Getter

        public virtual int getSelection() {
            return selection;
        }

        public virtual bool getBool() {
            return selection > 0;
        }

        public virtual float getFloat() {
            return (float)selections[selection];
        }

        public virtual string getString()
        {
            string sel = selections[selection].ToString();
            if (format != "")
            {
                return string.Format(ModTranslation.getString(format), sel);
            }
            return sel;
        }

        // Option changes

        public virtual void updateSelection(int newSelection) {
            selection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
            if (optionBehaviour != null && optionBehaviour is StringOption stringOption) {
                stringOption.oldValue = stringOption.Value = selection;
                stringOption.ValueText.text = ModTranslation.getString(getString());

                if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer) {
                    if (id == 0) switchPreset(selection); // Switch presets
                    else if (entry != null) entry.Value = selection; // Save selection to config

                    ShareOptionSelections();// Share all selections
                }
            }
        }
    }

    public class CustomOptionBlank : CustomOption
    {
        public CustomOptionBlank(CustomOption parent) {
            this.parent = parent;
            this.id = -1;
            this.name = "";
            this.isHeader = false;
            this.isHidden = true;
            this.children = new List<CustomOption>();
            this.selections = new string[] { "" };
            options.Add(this);
        }

        public override int getSelection()
        {
            return 0;
        }

        public override bool getBool()
        {
            return true;
        }

        public override float getFloat()
        {
            return 0f;
        }

        public override string getString()
        {
            return "";
        }

        public override void updateSelection(int newSelection)
        {
            return;
        }

    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    class GameOptionsMenuStartPatch {
        public static void Postfix(GameOptionsMenu __instance) {
            var template = UnityEngine.Object.FindObjectsOfType<StringOption>().FirstOrDefault();
            if (template == null) return;

            List<OptionBehaviour> allOptions = __instance.Children.ToList();
            for (int i = 0; i < CustomOption.options.Count; i++) {
                CustomOption option = CustomOption.options[i];
                if (option.optionBehaviour == null) {
                    StringOption stringOption = UnityEngine.Object.Instantiate(template, template.transform.parent);
                    allOptions.Add(stringOption);

                    stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => {});
                    stringOption.TitleText.text = ModTranslation.getString(option.name);
                    stringOption.Value = stringOption.oldValue = option.selection;
                    stringOption.ValueText.text = ModTranslation.getString(option.getString());

                    option.optionBehaviour = stringOption;
                }
                option.optionBehaviour.gameObject.SetActive(true);
            }

            var numImpostorsOption = allOptions.FirstOrDefault(x => x.name == "NumImpostors").TryCast<NumberOption>();
            if (numImpostorsOption != null) numImpostorsOption.ValidRange = new FloatRange(0f, 15f);

            var commonTasksOption = allOptions.FirstOrDefault(x => x.name == "NumCommonTasks").TryCast<NumberOption>();
            if(commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);

            var shortTasksOption = allOptions.FirstOrDefault(x => x.name == "NumShortTasks").TryCast<NumberOption>();
            if(shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);

            var longTasksOption = allOptions.FirstOrDefault(x => x.name == "NumLongTasks").TryCast<NumberOption>();
            if(longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
            
            __instance.Children = allOptions.ToArray();
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    public class StringOptionEnablePatch {
        public static bool Prefix(StringOption __instance) {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => {});
            __instance.TitleText.text = ModTranslation.getString(option.name);
            __instance.Value = __instance.oldValue = option.selection;
            __instance.ValueText.text = ModTranslation.getString(option.getString());
            
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public class StringOptionIncreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.updateSelection(option.selection + 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public class StringOptionDecreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.updateSelection(option.selection - 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public class RpcSyncSettingsPatch
    {
        public static void Postfix()
        {
            CustomOption.ShareOptionSelections();
        }
    }


    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
    class GameOptionsMenuUpdatePatch
    {
        private static float timer = 1f;
        public static void Postfix(GameOptionsMenu __instance) {
            timer += Time.deltaTime;
            if (timer < 0.1f) return;
            timer = 0f;

            float numItems = __instance.Children.Length;

            float offset = -7.85f;
            foreach (CustomOption option in CustomOption.options) {
                if (option?.optionBehaviour != null && option.optionBehaviour.gameObject != null) {
                    bool enabled = true;
                    var parent = option.parent;

                    if (AmongUsClient.Instance?.AmHost == false && CustomOptionHolder.gmEnabled.getBool() && CustomOptionHolder.gmHideSettings.getBool())
                    {
                        enabled = false;
                    }

                    if (option.isHidden)
                    {
                        enabled = false;
                    }

                    while (parent != null && enabled) {
                        enabled = parent.enabled;
                        parent = parent.parent;
                    }

                    option.optionBehaviour.gameObject.SetActive(enabled);
                    if (enabled) {
                        offset -= option.isHeader ? 0.75f : 0.5f;
                        option.optionBehaviour.transform.localPosition = new Vector3(option.optionBehaviour.transform.localPosition.x, offset, option.optionBehaviour.transform.localPosition.z);

                        if (option.isHeader)
                        {
                            numItems += 0.5f;
                        }
                    } else
                    {
                        numItems--;
                    }
                }
            }
            __instance.GetComponentInParent<Scroller>().YBounds.max = -3.0F + numItems * 0.5F;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), "OnEnable")]
    class GameSettingMenuPatch {
        public static void Prefix(GameSettingMenu __instance) {
            __instance.HideForOnline = new Transform[]{};
        }

        public static void Postfix(GameSettingMenu __instance) {
            var mapNameTransform = __instance.AllItems.FirstOrDefault(x => x.gameObject.activeSelf && x.name.Equals("MapName", StringComparison.OrdinalIgnoreCase));
            if (mapNameTransform == null) return;

            var options = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.KeyValuePair<string, int>>();
            for (int i = 0; i < GameOptionsData.MapNames.Length; i++) {
                var kvp = new Il2CppSystem.Collections.Generic.KeyValuePair<string, int>();
                kvp.key = GameOptionsData.MapNames[i];
                kvp.value = i;
                options.Add(kvp);
            }
            mapNameTransform.GetComponent<KeyValueOption>().Values = options;
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.ShouldFlipSkeld))]
    class ConstantsShouldFlipSkeldPatch {
        public static bool Prefix(ref bool __result) {
            if (PlayerControl.GameOptions == null) return true;
            __result = PlayerControl.GameOptions.MapId == 3;
            return false;
        }
    }

    [HarmonyPatch] 
    class GameOptionsDataPatch
    {
        public static string tl(string key)
        {
            return ModTranslation.getString(key);
        }

        private static IEnumerable<MethodBase> TargetMethods() {
            return typeof(GameOptionsData).GetMethods().Where(x => x.ReturnType == typeof(string) && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(int));
        }

        private static string optionToString(CustomOption option)
        {
            if (option == null) return "";
            return $"{tl(option.name)}: {tl(option.getString())}";
        }

        private static void Postfix(ref string __result)
        {

            bool hideSettings = AmongUsClient.Instance?.AmHost == false && CustomOptionHolder.gmEnabled.getBool() == true && CustomOptionHolder.gmHideSettings.getBool();
            if (hideSettings)
            {
                return;
            }

            List<string> pages = new List<string>();
            pages.Add(__result);

            StringBuilder entry = new StringBuilder();
            List<string> entries = new List<string>();

            // First add the presets and the role counts
            entries.Add(optionToString(CustomOptionHolder.presetSelection));

            var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), tl("crewmateRoles"));
            var min = CustomOptionHolder.crewmateRolesCountMin.getSelection();
            var max = CustomOptionHolder.crewmateRolesCountMax.getSelection();
            if (min > max) min = max;
            var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), tl("neutralRoles"));
            min = CustomOptionHolder.neutralRolesCountMin.getSelection();
            max = CustomOptionHolder.neutralRolesCountMax.getSelection();
            if (min > max) min = max;
            optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), tl("impostorRoles"));
            min = CustomOptionHolder.impostorRolesCountMin.getSelection();
            max = CustomOptionHolder.impostorRolesCountMax.getSelection();
            if (min > max) min = max;
            optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            entries.Add(entry.ToString().Trim('\r', '\n'));

            void addChildren(CustomOption option, ref StringBuilder entry, bool indent = true)
            {
                if (!option.enabled) return;

                foreach (var child in option.children)
                {
                    if (!child.isHidden)
                        entry.AppendLine((indent ? "    " : "") + optionToString(child));
                    addChildren(child, ref entry, indent);
                }
            }

            foreach (CustomOption option in CustomOption.options) {
                if ((option == CustomOptionHolder.presetSelection) ||
                    (option == CustomOptionHolder.crewmateRolesCountMin) ||
                    (option == CustomOptionHolder.crewmateRolesCountMax) ||
                    (option == CustomOptionHolder.neutralRolesCountMin) ||
                    (option == CustomOptionHolder.neutralRolesCountMax) ||
                    (option == CustomOptionHolder.impostorRolesCountMin) ||
                    (option == CustomOptionHolder.impostorRolesCountMax))
                {
                    continue;
                }

                if (option.parent == null) {
                    if (!option.enabled)
                    {
                        continue;
                    }

                    entry = new StringBuilder();
                    if (!option.isHidden)
                        entry.AppendLine(optionToString(option));

                    addChildren(option, ref entry, !option.isHidden);
                    entries.Add(entry.ToString().Trim('\r', '\n'));
                }
            }

            int maxLines = 37;
            int lineCount = 0;
            string page = "";
            foreach (var e in entries)
            {
                int lines = e.Count(c => c == '\n') + 1;

                if (lineCount + lines > maxLines)
                {
                    pages.Add(page);
                    page = "";
                    lineCount = 0;
                }

                page = page + e + "\n\n";
                lineCount += lines + 1;
            }

            page = page.Trim('\r', '\n');
            if (page != "")
            {
                pages.Add(page);
            }

            int numPages = pages.Count;
            int counter = TheOtherRolesPlugin.optionsPage = TheOtherRolesPlugin.optionsPage % numPages;

            __result = pages[counter].Trim('\r', '\n') + "\n\n" + tl("pressTabForMore") + $" ({counter + 1}/{numPages})";
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class GameOptionsNextPagePatch
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            if(Input.GetKeyDown(KeyCode.Tab) && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) {
                TheOtherRolesPlugin.optionsPage = TheOtherRolesPlugin.optionsPage + 1;
            }
        }
    }

    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GameSettingsScalePatch {
        public static void Prefix(HudManager __instance) {
            if (__instance.GameSettings != null) __instance.GameSettings.fontSize = 1.2f; 
        }
    }
}
