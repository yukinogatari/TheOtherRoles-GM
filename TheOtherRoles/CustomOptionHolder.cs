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
        public static CustomOption activateRoles;
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
        public static CustomOption camouflagerRandomColors;

        public static CustomOption vampireSpawnRate;
        public static CustomOption vampireKillDelay;
        public static CustomOption vampireCooldown;
        public static CustomOption vampireCanKillNearGarlics;

        public static CustomOption eraserSpawnRate;
        public static CustomOption eraserCooldown;
        public static CustomOption eraserCooldownIncrease;
        public static CustomOption eraserCanEraseAnyone;

        public static CustomOption miniSpawnRate;
        public static CustomOption miniGrowingUpDuration;
        public static CustomOption miniIsImpRate;

        public static CustomOption loversSpawnRate;
        public static CustomOption loversImpLoverRate;
        public static CustomOption loversBothDie;
        public static CustomOption loversCanHaveAnotherRole;
        public static CustomOption loversSeparateTeam;
        public static CustomOption loversTasksCount;

        public static CustomOption guesserSpawnRate;
        public static CustomOption guesserIsImpGuesserRate;
        public static CustomOption guesserNumberOfShots;
        public static CustomOption guesserOnlyAvailableRoles;
        public static CustomOption guesserHasMultipleShotsPerMeeting;
        public static CustomOption guesserShowInfoInGhostChat;
        public static CustomOption guesserKillsThroughShield;

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
        public static CustomOption jackalCanSeeEngineerVent;

        public static CustomOption bountyHunterSpawnRate;
        public static CustomOption bountyHunterBountyDuration;
        public static CustomOption bountyHunterReducedCooldown;
        public static CustomOption bountyHunterPunishmentTime;
        public static CustomOption bountyHunterShowArrow;
        public static CustomOption bountyHunterArrowUpdateIntervall;

        public static CustomOption witchSpawnRate;
        public static CustomOption witchCooldown;
        public static CustomOption witchAdditionalCooldown;
        public static CustomOption witchCanSpellAnyone;
        public static CustomOption witchSpellCastingDuration;
        public static CustomOption witchTriggerBothCooldowns;
        public static CustomOption witchVoteSavesTargets;

        public static CustomOption shifterSpawnRate;
        public static CustomOption shifterShiftsModifiers;

        public static CustomOption mayorSpawnRate;
        public static CustomOption mayorNumVotes;

        public static CustomOption engineerSpawnRate;
        public static CustomOption engineerNumberOfFixes;
        public static CustomOption engineerHighlightForImpostors;
        public static CustomOption engineerHighlightForTeamJackal;

        public static CustomOption sheriffSpawnRate;
        public static CustomOption sheriffCooldown;
        public static CustomOption sheriffNumShots;
        public static CustomOption sheriffCanKillNeutrals;
        public static CustomOption sheriffMisfireKillsTarget;

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
        public static CustomOption medicShowAttemptToMedic;

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
        public static CustomOption trackerCanTrackCorpses;
        public static CustomOption trackerCorpsesTrackingCooldown;
        public static CustomOption trackerCorpsesTrackingDuration;

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
        public static CustomOption gmCanWarp;
        public static CustomOption gmCanKill;
		
		public static CustomOption vultureSpawnRate;
        public static CustomOption vultureCooldown;
        public static CustomOption vultureNumberToWin;
        public static CustomOption vultureCanUseVents;
        public static CustomOption vultureShowArrows;

        public static CustomOption mediumSpawnRate;
        public static CustomOption mediumCooldown;
        public static CustomOption mediumDuration;
        public static CustomOption mediumOneTimeUse;

        public static CustomOption lawyerSpawnRate;
        public static CustomOption lawyerTargetKnows;
        public static CustomOption lawyerWinsAfterMeetings;
        public static CustomOption lawyerNeededMeetings;
        public static CustomOption lawyerVision;
        public static CustomOption lawyerKnowsRole;
        public static CustomOption pursuerCooldown;
        public static CustomOption pursuerBlanksNumber;

        public static CustomOption specialOptions;
        public static CustomOption maxNumberOfMeetings;
        public static CustomOption blockSkippingInEmergencyMeetings;
        public static CustomOption noVoteIsSelfVote;
        public static CustomOption hidePlayerNames;

        public static CustomOption hideSettings;
        public static CustomOption restrictDevices;
        public static CustomOption restrictAdmin;
        public static CustomOption restrictCameras;
        public static CustomOption restrictVents;

        public static CustomOption uselessOptions;
        public static CustomOption playerColorRandom;
        public static CustomOption playerNameDupes;
        public static CustomOption disableVents;
		public static CustomOption allowParallelMedBayScans;
        public static CustomOption dynamicMap;

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
            activateRoles = CustomOption.Create(7, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "blockOriginal"), true, null, true);

            presetSelection = CustomOption.Create(0, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, null, true);

            // Using new id's for the options to not break compatibilty with older versions
            crewmateRolesCountMin = CustomOption.Create(300, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMin"), 0f, 0f, 15f, 1f, null, true);
            crewmateRolesCountMax = CustomOption.Create(301, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMax"), 0f, 0f, 15f, 1f);
            neutralRolesCountMin = CustomOption.Create(302, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMin"), 0f, 0f, 15f, 1f);
            neutralRolesCountMax = CustomOption.Create(303, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMax"), 0f, 0f, 15f, 1f);
            impostorRolesCountMin = CustomOption.Create(304, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMin"), 0f, 0f, 15f, 1f);
            impostorRolesCountMax = CustomOption.Create(305, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMax"), 0f, 0f, 15f, 1f);


            gmEnabled = CustomOption.Create(400, cs(GM.color, "gm"), false, null, true);
            gmIsHost = CustomOption.Create(401, "gmIsHost", true, gmEnabled);
            //gmHasTasks = CustomOption.Create(402, "gmHasTasks", false, gmEnabled);
            gmCanWarp = CustomOption.Create(405, "gmCanWarp", true, gmEnabled);
            gmCanKill = CustomOption.Create(406, "gmCanKill", false, gmEnabled);
            gmDiesAtStart = CustomOption.Create(404, "gmDiesAtStart", true, gmEnabled);


            mafiaSpawnRate = CustomOption.Create(10, cs(Janitor.color, "mafia"), rates, null, true);
            janitorCooldown = CustomOption.Create(11, "janitorCooldown", 30f, 2.5f, 60f, 2.5f, mafiaSpawnRate, format : "unitSeconds");

            morphlingSpawnRate = CustomOption.Create(20, cs(Morphling.color, "morphling"), rates, null, true);
            morphlingCooldown = CustomOption.Create(21, "morphlingCooldown", 30f, 2.5f, 60f, 2.5f, morphlingSpawnRate, format: "unitSeconds");
            morphlingDuration = CustomOption.Create(22, "morphlingDuration", 10f, 1f, 20f, 0.5f, morphlingSpawnRate, format: "unitSeconds");

            camouflagerSpawnRate = CustomOption.Create(30, cs(Camouflager.color, "camouflager"), rates, null, true);
            camouflagerCooldown = CustomOption.Create(31, "camouflagerCooldown", 30f, 2.5f, 60f, 2.5f, camouflagerSpawnRate, format: "unitSeconds");
            camouflagerDuration = CustomOption.Create(32, "camouflagerDuration", 10f, 1f, 20f, 0.5f, camouflagerSpawnRate, format: "unitSeconds");
            camouflagerRandomColors = CustomOption.Create(33, "camouflagerRandomColors", false, camouflagerSpawnRate);

            vampireSpawnRate = CustomOption.Create(40, cs(Vampire.color, "vampire"), rates, null, true);
            vampireKillDelay = CustomOption.Create(41, "vampireKillDelay", 10f, 1f, 20f, 1f, vampireSpawnRate, format: "unitSeconds");
            vampireCooldown = CustomOption.Create(42, "vampireCooldown", 30f, 2.5f, 60f, 2.5f, vampireSpawnRate, format: "unitSeconds");
            vampireCanKillNearGarlics = CustomOption.Create(43, "vampireCanKillNearGarlics", true, vampireSpawnRate);

            eraserSpawnRate = CustomOption.Create(230, cs(Eraser.color, "eraser"), rates, null, true);
            eraserCooldown = CustomOption.Create(231, "eraserCooldown", 30f, 5f, 120f, 5f, eraserSpawnRate, format: "unitSeconds");
            eraserCooldownIncrease = CustomOption.Create(233, "eraserCooldownIncrease", 10f, 0f, 120f, 2.5f, eraserSpawnRate, format: "unitSeconds");
            eraserCanEraseAnyone = CustomOption.Create(232, "eraserCanEraseAnyone", false, eraserSpawnRate);

            tricksterSpawnRate = CustomOption.Create(250, cs(Trickster.color, "trickster"), rates, null, true);
            tricksterPlaceBoxCooldown = CustomOption.Create(251, "tricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, tricksterSpawnRate, format: "unitSeconds");
            tricksterLightsOutCooldown = CustomOption.Create(252, "tricksterLightsOutCooldown", 30f, 5f, 60f, 5f, tricksterSpawnRate, format: "unitSeconds");
            tricksterLightsOutDuration = CustomOption.Create(253, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, tricksterSpawnRate, format: "unitSeconds");

            cleanerSpawnRate = CustomOption.Create(260, cs(Cleaner.color, "cleaner"), rates, null, true);
            cleanerCooldown = CustomOption.Create(261, "cleanerCooldown", 30f, 2.5f, 60f, 2.5f, cleanerSpawnRate, format: "unitSeconds");

            warlockSpawnRate = CustomOption.Create(270, cs(Warlock.color, "warlock"), rates, null, true);
            warlockCooldown = CustomOption.Create(271, "warlockCooldown", 30f, 2.5f, 60f, 2.5f, warlockSpawnRate, format: "unitSeconds");
            warlockRootTime = CustomOption.Create(272, "warlockRootTime", 5f, 0f, 15f, 1f, warlockSpawnRate, format: "unitSeconds");

            bountyHunterSpawnRate = CustomOption.Create(320, cs(BountyHunter.color, "bountyHunter"), rates, null, true);
            bountyHunterBountyDuration = CustomOption.Create(321, "bountyHunterBountyDuration", 60f, 10f, 180f, 10f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterReducedCooldown = CustomOption.Create(322, "bountyHunterReducedCooldown", 2.5f, 2.5f, 30f, 2.5f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterPunishmentTime = CustomOption.Create(323, "bountyHunterPunishmentTime", 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterShowArrow = CustomOption.Create(324, "bountyHunterShowArrow", true, bountyHunterSpawnRate);
            bountyHunterArrowUpdateIntervall = CustomOption.Create(325, "bountyHunterArrowUpdateIntervall", 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow, format: "unitSeconds");

            witchSpawnRate = CustomOption.Create(390, cs(Witch.color, "witch"), rates, null, true);
            witchCooldown = CustomOption.Create(391, "witchSpellCooldown", 30f, 2.5f, 120f, 2.5f, witchSpawnRate, format: "unitSeconds");
            witchAdditionalCooldown = CustomOption.Create(392, "witchAdditionalCooldown", 10f, 0f, 60f, 5f, witchSpawnRate, format: "unitSeconds");
            witchCanSpellAnyone = CustomOption.Create(393, "witchCanSpellAnyone", false, witchSpawnRate);
            witchSpellCastingDuration = CustomOption.Create(394, "witchSpellDuration", 1f, 0f, 10f, 1f, witchSpawnRate, format: "unitSeconds");
            witchTriggerBothCooldowns = CustomOption.Create(395, "witchTriggerBoth", true, witchSpawnRate);
            witchVoteSavesTargets = CustomOption.Create(396, "witchSaveTargets", true, witchSpawnRate);

            madmateSpawnRate = CustomOption.Create(360, cs(Madmate.color, "madmate"), rates, null, true);
            madmateCanDieToSheriff = CustomOption.Create(361, "madmateCanDieToSheriff", false, madmateSpawnRate);
            madmateCanEnterVents = CustomOption.Create(362, "madmateCanEnterVents", false, madmateSpawnRate);
            madmateHasImpostorVision = CustomOption.Create(363, "madmateHasImpostorVision", false, madmateSpawnRate);
            madmateCanSabotage = CustomOption.Create(364, "madmateCanSabotage", false, madmateSpawnRate);
            madmateCanFixComm = CustomOption.Create(365, "madmateCanFixComm", true, madmateSpawnRate);


            miniSpawnRate = CustomOption.Create(180, cs(Mini.color, "mini"), rates, null, true);
            miniIsImpRate = CustomOption.Create(182, "miniIsImpRate", rates, miniSpawnRate);
            miniGrowingUpDuration = CustomOption.Create(181, "miniGrowingUpDuration", 400f, 100f, 1500f, 100f, miniSpawnRate, format: "unitSeconds");

            loversSpawnRate = CustomOption.Create(50, cs(Lovers.color, "lovers"), rates, null, true);
            loversImpLoverRate = CustomOption.Create(51, "loversImpLoverRate", rates, loversSpawnRate);
            loversBothDie = CustomOption.Create(52, "loversBothDie", true, loversSpawnRate);
            loversCanHaveAnotherRole = CustomOption.Create(53, "loversCanHaveAnotherRole", true, loversSpawnRate);
            loversSeparateTeam = CustomOption.Create(56, "loversSeparateTeam", true, loversSpawnRate);
            loversTasksCount = CustomOption.Create(55, "loversTasksCount", false, loversSeparateTeam);

            guesserSpawnRate = CustomOption.Create(310, cs(Guesser.color, "guesser"), rates, null, true);
            guesserIsImpGuesserRate = CustomOption.Create(311, "guesserIsImpGuesserRate", rates, guesserSpawnRate);
            guesserNumberOfShots = CustomOption.Create(312, "guesserNumberOfShots", 2f, 1f, 15f, 1f, guesserSpawnRate, format: "unitShots");
            guesserOnlyAvailableRoles = CustomOption.Create(313, "guesserOnlyAvailableRoles", true, guesserSpawnRate);
            guesserHasMultipleShotsPerMeeting = CustomOption.Create(314, "guesserHasMultipleShotsPerMeeting", false, guesserSpawnRate);
            guesserShowInfoInGhostChat = CustomOption.Create(315, "guesserToGhostChat", true, guesserSpawnRate);
            guesserKillsThroughShield  = CustomOption.Create(316, "guesserPierceShield", true, guesserSpawnRate);

            jesterSpawnRate = CustomOption.Create(60, cs(Jester.color, "jester"), rates, null, true);
            jesterCanCallEmergency = CustomOption.Create(61, "jesterCanCallEmergency", true, jesterSpawnRate);
            jesterCanSabotage = CustomOption.Create(62, "jesterCanSabotage", true, jesterSpawnRate);

            arsonistSpawnRate = CustomOption.Create(290, cs(Arsonist.color, "arsonist"), rates, null, true);
            arsonistCooldown = CustomOption.Create(291, "arsonistCooldown", 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate, format: "unitSeconds");
            arsonistDuration = CustomOption.Create(292, "arsonistDuration", 3f, 0f, 10f, 1f, arsonistSpawnRate, format: "unitSeconds");

            opportunistSpawnRate = CustomOption.Create(380, cs(Opportunist.color, "opportunist"), rates, null, true);

            jackalSpawnRate = CustomOption.Create(220, cs(Jackal.color, "jackal"), rates, null, true);
            jackalKillCooldown = CustomOption.Create(221, "jackalKillCooldown", 30f, 2.5f, 60f, 2.5f, jackalSpawnRate, format: "unitSeconds");
            jackalCanUseVents = CustomOption.Create(223, "jackalCanUseVents", true, jackalSpawnRate);
            jackalAndSidekickHaveImpostorVision = CustomOption.Create(430, "jackalAndSidekickHaveImpostorVision", false, jackalSpawnRate);
            jackalCanCreateSidekick = CustomOption.Create(224, "jackalCanCreateSidekick", false, jackalSpawnRate);
            jackalCreateSidekickCooldown = CustomOption.Create(222, "jackalCreateSidekickCooldown", 30f, 2.5f, 60f, 2.5f, jackalCanCreateSidekick, format: "unitSeconds");
            sidekickPromotesToJackal = CustomOption.Create(225, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
            sidekickCanKill = CustomOption.Create(226, "sidekickCanKill", false, jackalCanCreateSidekick);
            sidekickCanUseVents = CustomOption.Create(227, "sidekickCanUseVents", true, jackalCanCreateSidekick);
            jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(228, "jackalPromotedFromSidekickCanCreateSidekick", true, jackalCanCreateSidekick);
            jackalCanCreateSidekickFromImpostor = CustomOption.Create(229, "jackalCanCreateSidekickFromImpostor", true, jackalCanCreateSidekick);

            vultureSpawnRate = CustomOption.Create(340, cs(Vulture.color, "vulture"), rates, null, true);
            vultureCooldown = CustomOption.Create(341, "vultureCooldown", 15f, 2.5f, 60f, 2.5f, vultureSpawnRate, format: "unitSeconds");
            vultureNumberToWin = CustomOption.Create(342, "vultureNumberToWin", 4f, 1f, 12f, 1f, vultureSpawnRate);
            vultureCanUseVents = CustomOption.Create(343, "vultureCanUseVents", true, vultureSpawnRate);
            vultureShowArrows = CustomOption.Create(344, "vultureShowArrows", true, vultureSpawnRate);

            lawyerSpawnRate = CustomOption.Create(350, cs(Lawyer.color, "lawyer"), rates, null, true);
            lawyerTargetKnows = CustomOption.Create(351, "lawyerTargetKnows", true, lawyerSpawnRate);
            lawyerWinsAfterMeetings = CustomOption.Create(352, "lawyerWinsMeeting", false, lawyerSpawnRate);
            lawyerNeededMeetings = CustomOption.Create(353, "lawyerMeetingsNeeded", 5f, 1f, 15f, 1f, lawyerWinsAfterMeetings);
            lawyerVision = CustomOption.Create(354, "lawyerVision", 1f, 0.25f, 3f, 0.25f, lawyerSpawnRate, format: "unitMultiplier");
            lawyerKnowsRole = CustomOption.Create(355, "lawyerKnowsRole", false, lawyerSpawnRate);
            pursuerCooldown = CustomOption.Create(356, "pursuerBlankCool", 30f, 2.5f, 60f, 2.5f, lawyerSpawnRate, format: "unitSeconds");
            pursuerBlanksNumber = CustomOption.Create(357, "pursuerNumBlanks", 5f, 0f, 20f, 1f, lawyerSpawnRate, format: "unitShots");

            shifterSpawnRate = CustomOption.Create(70, cs(Shifter.color, "shifter"), rates, null, true);
            shifterShiftsModifiers = CustomOption.Create(71, "shifterShiftsModifiers", false, shifterSpawnRate);

            mayorSpawnRate = CustomOption.Create(80, cs(Mayor.color, "mayor"), rates, null, true);
            mayorNumVotes = CustomOption.Create(81, "mayorNumVotes", 2f, 2f, 10f, 1f, mayorSpawnRate);

            engineerSpawnRate = CustomOption.Create(90, cs(Engineer.color, "engineer"), rates, null, true);
            engineerNumberOfFixes = CustomOption.Create(91, "engineerNumFixes", 1f, 0f, 3f, 1f, engineerSpawnRate);
            engineerHighlightForImpostors = CustomOption.Create(92, "engineerImpostorsSeeVent", true, engineerSpawnRate);
            engineerHighlightForTeamJackal = CustomOption.Create(93, "engineerJackalSeeVent", true, engineerSpawnRate);

            sheriffSpawnRate = CustomOption.Create(100, cs(Sheriff.color, "sheriff"), rates, null, true);
            sheriffCooldown = CustomOption.Create(101, "sheriffCooldown", 30f, 2.5f, 60f, 2.5f, sheriffSpawnRate, format: "unitSeconds");
            sheriffNumShots = CustomOption.Create(103, "sheriffNumShots", 2f, 1f, 15f, 1f, sheriffSpawnRate, format: "unitShots");
            sheriffMisfireKillsTarget = CustomOption.Create(104, "sheriffMisfireKillsTarget", false, sheriffSpawnRate);
            sheriffCanKillNeutrals = CustomOption.Create(102, "sheriffCanKillNeutrals", false, sheriffSpawnRate);

            lighterSpawnRate = CustomOption.Create(110, cs(Lighter.color, "lighter"), rates, null, true);
            lighterModeLightsOnVision = CustomOption.Create(111, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterModeLightsOffVision = CustomOption.Create(112, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterCooldown = CustomOption.Create(113, "lighterCooldown", 30f, 5f, 120f, 5f, lighterSpawnRate, format: "unitSeconds");
            lighterDuration = CustomOption.Create(114, "lighterDuration", 5f, 2.5f, 60f, 2.5f, lighterSpawnRate, format: "unitSeconds");

            detectiveSpawnRate = CustomOption.Create(120, cs(Detective.color, "detective"), rates, null, true);
            detectiveAnonymousFootprints = CustomOption.Create(121, "detectiveAnonymousFootprints", false, detectiveSpawnRate);
            detectiveFootprintIntervall = CustomOption.Create(122, "detectiveFootprintIntervall", 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, format: "unitSeconds");
            detectiveFootprintDuration = CustomOption.Create(123, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, format: "unitSeconds");
            detectiveReportNameDuration = CustomOption.Create(124, "detectiveReportNameDuration", 0, 0, 60, 2.5f, detectiveSpawnRate, format: "unitSeconds");
            detectiveReportColorDuration = CustomOption.Create(125, "detectiveReportColorDuration", 20, 0, 120, 2.5f, detectiveSpawnRate, format: "unitSeconds");

            timeMasterSpawnRate = CustomOption.Create(130, cs(TimeMaster.color, "timeMaster"), rates, null, true);
            timeMasterCooldown = CustomOption.Create(131, "timeMasterCooldown", 30f, 2.5f, 120f, 2.5f, timeMasterSpawnRate, format: "unitSeconds");
            timeMasterRewindTime = CustomOption.Create(132, "timeMasterRewindTime", 3f, 1f, 10f, 1f, timeMasterSpawnRate, format: "unitSeconds");
            timeMasterShieldDuration = CustomOption.Create(133, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, timeMasterSpawnRate, format: "unitSeconds");

            medicSpawnRate = CustomOption.Create(140, cs(Medic.color, "medic"), rates, null, true);
            medicShowShielded = CustomOption.Create(143, "medicShowShielded", new string[] { "medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic" }, medicSpawnRate);
            medicShowAttemptToShielded = CustomOption.Create(144, "medicShowAttemptToShielded", false, medicSpawnRate);
            medicSetShieldAfterMeeting = CustomOption.Create(145, "medicSetShieldAfterMeeting", false, medicSpawnRate);
            medicShowAttemptToMedic = CustomOption.Create(146, "medicSeesMurderAttempt", false, medicSpawnRate);

            swapperSpawnRate = CustomOption.Create(150, cs(Swapper.color, "swapper"), rates, null, true);
            swapperCanCallEmergency = CustomOption.Create(151, "swapperCanCallEmergency", false, swapperSpawnRate);
            swapperCanOnlySwapOthers = CustomOption.Create(152, "swapperCanOnlySwapOthers", false, swapperSpawnRate);

            seerSpawnRate = CustomOption.Create(160, cs(Seer.color, "seer"), rates, null, true);
            seerMode = CustomOption.Create(161, "seerMode", new string[] { "seerModeBoth", "seerModeFlash", "seerModeSouls" }, seerSpawnRate);
            seerLimitSoulDuration = CustomOption.Create(163, "seerLimitSoulDuration", false, seerSpawnRate);
            seerSoulDuration = CustomOption.Create(162, "seerSoulDuration", 15f, 0f, 60f, 5f, seerLimitSoulDuration, format: "unitSeconds");

            hackerSpawnRate = CustomOption.Create(170, cs(Hacker.color, "hacker"), rates, null, true);
            hackerCooldown = CustomOption.Create(171, "hackerCooldown", 30f, 5f, 60f, 5f, hackerSpawnRate, format: "unitSeconds");
            hackerHackeringDuration = CustomOption.Create(172, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate, format: "unitSeconds");
            hackerOnlyColorType = CustomOption.Create(173, "hackerOnlyColorType", false, hackerSpawnRate);

            trackerSpawnRate = CustomOption.Create(200, cs(Tracker.color, "tracker"), rates, null, true);
            trackerUpdateIntervall = CustomOption.Create(201, "trackerUpdateIntervall", 5f, 2.5f, 30f, 2.5f, trackerSpawnRate, format: "unitSeconds");
            trackerResetTargetAfterMeeting = CustomOption.Create(202, "trackerResetTargetAfterMeeting", false, trackerSpawnRate);
            trackerCanTrackCorpses = CustomOption.Create(203, "trackerTrackCorpses", true, trackerSpawnRate);
            trackerCorpsesTrackingCooldown = CustomOption.Create(204, "trackerCorpseCooldown", 30f, 0f, 120f, 5f, trackerCanTrackCorpses, format: "unitSeconds");
            trackerCorpsesTrackingDuration = CustomOption.Create(205, "trackerCorpseDuration", 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses, format: "unitSeconds");
                           
            snitchSpawnRate = CustomOption.Create(210, cs(Snitch.color, "snitch"), rates, null, true);
            snitchLeftTasksForReveal = CustomOption.Create(211, "snitchLeftTasksForReveal", 1f, 0f, 5f, 1f, snitchSpawnRate);
            snitchIncludeTeamJackal = CustomOption.Create(212, "snitchIncludeTeamJackal", false, snitchSpawnRate);
            snitchTeamJackalUseDifferentArrowColor = CustomOption.Create(213, "snitchTeamJackalUseDifferentArrowColor", true, snitchIncludeTeamJackal);

            spySpawnRate = CustomOption.Create(240, cs(Spy.color, "spy"), rates, null, true);
            spyCanDieToSheriff = CustomOption.Create(241, "spyCanDieToSheriff", false, spySpawnRate);
            spyImpostorsCanKillAnyone = CustomOption.Create(242, "spyImpostorsCanKillAnyone", true, spySpawnRate);
            spyCanEnterVents = CustomOption.Create(243, "spyCanEnterVents", false, spySpawnRate);
            spyHasImpostorVision = CustomOption.Create(244, "spyHasImpostorVision", false, spySpawnRate);

            securityGuardSpawnRate = CustomOption.Create(280, cs(SecurityGuard.color, "securityGuard"), rates, null, true);
            securityGuardCooldown = CustomOption.Create(281, "securityGuardCooldown", 30f, 2.5f, 60f, 2.5f, securityGuardSpawnRate, format: "unitSeconds");
            securityGuardTotalScrews = CustomOption.Create(282, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardCamPrice = CustomOption.Create(283, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardVentPrice = CustomOption.Create(284, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");

            baitSpawnRate = CustomOption.Create(330, cs(Bait.color, "bait"), rates, null, true);
            baitHighlightAllVents = CustomOption.Create(331, "baitHighlightAllVents", false, baitSpawnRate);
            baitReportDelay = CustomOption.Create(332, "baitReportDelay", 0f, 0f, 10f, 1f, baitSpawnRate, format: "unitSeconds");

            mediumSpawnRate = CustomOption.Create(370, cs(Medium.color, "medium"), rates, null, true);
            mediumCooldown = CustomOption.Create(371, "mediumCooldown", 30f, 5f, 120f, 5f, mediumSpawnRate, format: "unitSeconds");
            mediumDuration = CustomOption.Create(372, "mediumDuration", 3f, 0f, 15f, 1f, mediumSpawnRate, format: "unitSeconds");
            mediumOneTimeUse = CustomOption.Create(373, "mediumOneTimeUse", false, mediumSpawnRate);

            // Other options
            specialOptions = new CustomOptionBlank(null);
            maxNumberOfMeetings = CustomOption.Create(3, "maxNumberOfMeetings", 10, 0, 15, 1, specialOptions, true);
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, "blockSkippingInEmergencyMeetings", false, specialOptions);
            noVoteIsSelfVote = CustomOption.Create(5, "noVoteIsSelfVote", false, specialOptions);
            allowParallelMedBayScans = CustomOption.Create(540, "parallelMedbayScans", false, specialOptions);
            hideSettings = CustomOption.Create(520, "hideSettings", false, specialOptions);

            restrictDevices = CustomOption.Create(510, "restrictDevices", new string[] { "optionOff", "restrictPerTurn", "restrictPerGame" }, specialOptions);
            restrictAdmin = CustomOption.Create(501, "disableAdmin", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictCameras = CustomOption.Create(502, "disableCameras", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictVents = CustomOption.Create(503, "disableVitals", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");

            uselessOptions = CustomOption.Create(530, "uselessOptions", false, null, isHeader: true);
            dynamicMap = CustomOption.Create(8, "playRandomMaps", false, uselessOptions);
            disableVents = CustomOption.Create(504, "disableVents", false, uselessOptions);
            hidePlayerNames = CustomOption.Create(6, "hidePlayerNames", false, uselessOptions);
            playerNameDupes = CustomOption.Create(522, "playerNameDupes", false, uselessOptions);
            playerColorRandom = CustomOption.Create(521, "playerColorRandom", false, uselessOptions);

            blockedRolePairings.Add((byte)RoleId.Vampire, new [] { (byte)RoleId.Warlock});
            blockedRolePairings.Add((byte)RoleId.Warlock, new [] { (byte)RoleId.Vampire});
            blockedRolePairings.Add((byte)RoleId.Spy, new [] { (byte)RoleId.Mini});
            blockedRolePairings.Add((byte)RoleId.Mini, new [] { (byte)RoleId.Spy});
            
        }
    }

}
