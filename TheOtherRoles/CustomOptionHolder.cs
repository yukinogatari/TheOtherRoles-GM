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
using static TheOtherRoles.TheOtherRolesGM;

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

        public static CustomRoleOption mafiaSpawnRate;
        public static CustomOption mafiosoCanSabotage;
        public static CustomOption mafiosoCanRepair;
        public static CustomOption mafiosoCanVent;
        public static CustomOption janitorCooldown;
        public static CustomOption janitorCanSabotage;
        public static CustomOption janitorCanRepair;
        public static CustomOption janitorCanVent;

        public static CustomRoleOption morphlingSpawnRate;
        public static CustomOption morphlingCooldown;
        public static CustomOption morphlingDuration;

        public static CustomRoleOption camouflagerSpawnRate;
        public static CustomOption camouflagerCooldown;
        public static CustomOption camouflagerDuration;
        public static CustomOption camouflagerRandomColors;

        public static CustomRoleOption vampireSpawnRate;
        public static CustomOption vampireKillDelay;
        public static CustomOption vampireCooldown;
        public static CustomOption vampireCanKillNearGarlics;

        public static CustomRoleOption eraserSpawnRate;
        public static CustomOption eraserCooldown;
        public static CustomOption eraserCooldownIncrease;
        public static CustomOption eraserCanEraseAnyone;

        public static CustomRoleOption miniSpawnRate;
        public static CustomOption miniGrowingUpDuration;
        public static CustomOption miniIsImpRate;

        public static CustomOption loversSpawnRate;
        public static CustomOption loversNumCouples;
        public static CustomOption loversImpLoverRate;
        public static CustomOption loversBothDie;
        public static CustomOption loversCanHaveAnotherRole;
        public static CustomOption loversSeparateTeam;
        public static CustomOption loversTasksCount;
        public static CustomOption loversEnableChat;

        public static CustomRoleOption guesserSpawnRate;
        public static CustomOption guesserIsImpGuesserRate;
        public static CustomOption guesserNumberOfShots;
        public static CustomOption guesserOnlyAvailableRoles;
        public static CustomOption guesserHasMultipleShotsPerMeeting;
        public static CustomOption guesserShowInfoInGhostChat;
        public static CustomOption guesserKillsThroughShield;
        public static CustomOption guesserEvilCanKillSpy;
        public static CustomOption guesserSpawnBothRate;
        public static CustomOption guesserCantGuessSnitchIfTaksDone;

        public static CustomRoleOption jesterSpawnRate;
        public static CustomOption jesterCanCallEmergency;
        public static CustomOption jesterCanSabotage;
        public static CustomOption jesterHasImpostorVision;

        public static CustomRoleOption arsonistSpawnRate;
        public static CustomOption arsonistCooldown;
        public static CustomOption arsonistDuration;
        public static CustomOption arsonistCanBeLovers;

        public static CustomRoleOption jackalSpawnRate;
        public static CustomOption jackalKillCooldown;
        public static CustomOption jackalCreateSidekickCooldown;
        public static CustomOption jackalCanUseVents;
        public static CustomOption jackalCanCreateSidekick;
        public static CustomOption sidekickPromotesToJackal;
        public static CustomOption sidekickCanKill;
        public static CustomOption sidekickCanUseVents;
        public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
        public static CustomOption jackalCanCreateSidekickFromImpostor;
        public static CustomOption jackalCanCreateSidekickFromFox;
        public static CustomOption jackalAndSidekickHaveImpostorVision;
        public static CustomOption jackalCanSeeEngineerVent;

        public static CustomRoleOption bountyHunterSpawnRate;
        public static CustomOption bountyHunterBountyDuration;
        public static CustomOption bountyHunterReducedCooldown;
        public static CustomOption bountyHunterPunishmentTime;
        public static CustomOption bountyHunterShowArrow;
        public static CustomOption bountyHunterArrowUpdateIntervall;

        public static CustomRoleOption witchSpawnRate;
        public static CustomOption witchCooldown;
        public static CustomOption witchAdditionalCooldown;
        public static CustomOption witchCanSpellAnyone;
        public static CustomOption witchSpellCastingDuration;
        public static CustomOption witchTriggerBothCooldowns;
        public static CustomOption witchVoteSavesTargets;

        public static CustomRoleOption shifterSpawnRate;
        public static CustomOption shifterIsNeutralRate;
        public static CustomOption shifterShiftsModifiers;
        public static CustomOption shifterPastShifters;

        public static CustomRoleOption fortuneTellerSpawnRate;
        public static CustomOption fortuneTellerNumTasks;
        public static CustomOption fortuneTellerResults;
        public static CustomOption fortuneTellerDistance;
        public static CustomOption fortuneTellerDuration;

        public static CustomRoleOption mayorSpawnRate;
        public static CustomOption mayorNumVotes;

        public static CustomRoleOption engineerSpawnRate;
        public static CustomOption engineerNumberOfFixes;
        public static CustomOption engineerHighlightForImpostors;
        public static CustomOption engineerHighlightForTeamJackal;

        public static CustomRoleOption sheriffSpawnRate;
        public static CustomOption sheriffCooldown;
        public static CustomOption sheriffNumShots;
        public static CustomOption sheriffCanKillNeutrals;
        public static CustomOption sheriffMisfireKillsTarget;

        public static CustomRoleOption lighterSpawnRate;
        public static CustomOption lighterModeLightsOnVision;
        public static CustomOption lighterModeLightsOffVision;
        public static CustomOption lighterCooldown;
        public static CustomOption lighterDuration;
        public static CustomOption lighterCanSeeNinja;

        public static CustomRoleOption detectiveSpawnRate;
        public static CustomOption detectiveAnonymousFootprints;
        public static CustomOption detectiveFootprintIntervall;
        public static CustomOption detectiveFootprintDuration;
        public static CustomOption detectiveReportNameDuration;
        public static CustomOption detectiveReportColorDuration;

        public static CustomRoleOption timeMasterSpawnRate;
        public static CustomOption timeMasterCooldown;
        public static CustomOption timeMasterRewindTime;
        public static CustomOption timeMasterShieldDuration;

        public static CustomRoleOption medicSpawnRate;
        public static CustomOption medicShowShielded;
        public static CustomOption medicShowAttemptToShielded;
        public static CustomOption medicSetShieldAfterMeeting;
        public static CustomOption medicShowAttemptToMedic;

        public static CustomRoleOption swapperSpawnRate;
        public static CustomOption swapperIsImpRate;
        public static CustomOption swapperCanCallEmergency;
        public static CustomOption swapperCanOnlySwapOthers;
        public static CustomOption swapperNumSwaps;

        public static CustomRoleOption seerSpawnRate;
        public static CustomOption seerMode;
        public static CustomOption seerSoulDuration;
        public static CustomOption seerLimitSoulDuration;

        public static CustomRoleOption hackerSpawnRate;
        public static CustomOption hackerCooldown;
        public static CustomOption hackerHackeringDuration;
        public static CustomOption hackerOnlyColorType;
        public static CustomOption hackerToolsNumber;
        public static CustomOption hackerRechargeTasksNumber;
        public static CustomOption hackerNoMove;

        public static CustomRoleOption trackerSpawnRate;
        public static CustomOption trackerUpdateIntervall;
        public static CustomOption trackerResetTargetAfterMeeting;
        public static CustomOption trackerCanTrackCorpses;
        public static CustomOption trackerCorpsesTrackingCooldown;
        public static CustomOption trackerCorpsesTrackingDuration;

        public static CustomRoleOption snitchSpawnRate;
        public static CustomOption snitchLeftTasksForReveal;
        public static CustomOption snitchIncludeTeamJackal;
        public static CustomOption snitchTeamJackalUseDifferentArrowColor;

        public static CustomRoleOption spySpawnRate;
        public static CustomOption spyCanDieToSheriff;
        public static CustomOption spyImpostorsCanKillAnyone;
        public static CustomOption spyCanEnterVents;
        public static CustomOption spyHasImpostorVision;

        public static CustomRoleOption tricksterSpawnRate;
        public static CustomOption tricksterPlaceBoxCooldown;
        public static CustomOption tricksterLightsOutCooldown;
        public static CustomOption tricksterLightsOutDuration;

        public static CustomRoleOption cleanerSpawnRate;
        public static CustomOption cleanerCooldown;
        
        public static CustomRoleOption warlockSpawnRate;
        public static CustomOption warlockCooldown;
        public static CustomOption warlockRootTime;

        public static CustomRoleOption securityGuardSpawnRate;
        public static CustomOption securityGuardCooldown;
        public static CustomOption securityGuardTotalScrews;
        public static CustomOption securityGuardCamPrice;
        public static CustomOption securityGuardVentPrice;
        public static CustomOption securityGuardCamDuration;
        public static CustomOption securityGuardCamMaxCharges;
        public static CustomOption securityGuardCamRechargeTasksNumber;
        public static CustomOption securityGuardNoMove;

        public static CustomRoleOption baitSpawnRate;
        public static CustomOption baitHighlightAllVents;
        public static CustomOption baitReportDelay;
        public static CustomOption baitShowKillFlash;
		
		public static CustomRoleOption vultureSpawnRate;
        public static CustomOption vultureCooldown;
        public static CustomOption vultureNumberToWin;
        public static CustomOption vultureCanUseVents;
        public static CustomOption vultureShowArrows;

        public static CustomRoleOption mediumSpawnRate;
        public static CustomOption mediumCooldown;
        public static CustomOption mediumDuration;
        public static CustomOption mediumOneTimeUse;

        public static CustomRoleOption lawyerSpawnRate;
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

		public static CustomOption allowParallelMedBayScans;

        public static CustomOption dynamicMap;
        public static CustomOption dynamicMapEnableSkeld;
        public static CustomOption dynamicMapEnableMira;
        public static CustomOption dynamicMapEnablePolus;
        public static CustomOption dynamicMapEnableDleks;
        public static CustomOption dynamicMapEnableAirShip;

        // GM Edition options
        public static CustomRoleOption madmateSpawnRate;
        public static CustomOption madmateCanDieToSheriff;
        public static CustomOption madmateCanEnterVents;
        public static CustomOption madmateHasImpostorVision;
        public static CustomOption madmateCanSabotage;
        public static CustomOption madmateCanFixComm;
        public static CustomOption madmateType;
        public static CustomRoleSelectionOption madmateFixedRole;
        public static CustomOption madmateAbility;
        public static CustomTasksOption madmateTasks;

        public static CustomRoleOption opportunistSpawnRate;

        public static CustomRoleOption ninjaSpawnRate;
        public static CustomOption ninjaStealthCooldown;
        public static CustomOption ninjaStealthDuration;
        public static CustomOption ninjaKillPenalty;
        public static CustomOption ninjaSpeedBonus;
        public static CustomOption ninjaFadeTime;
        public static CustomOption ninjaCanVent;
        public static CustomOption ninjaCanBeTargeted;

        public static CustomOption gmEnabled;
        public static CustomOption gmIsHost;
        public static CustomOption gmHasTasks;
        public static CustomOption gmDiesAtStart;
        public static CustomOption gmCanWarp;
        public static CustomOption gmCanKill;

        public static CustomRoleOption plagueDoctorSpawnRate;
        public static CustomOption plagueDoctorInfectCooldown;
        public static CustomOption plagueDoctorNumInfections;
        public static CustomOption plagueDoctorDistance;
        public static CustomOption plagueDoctorDuration;
        public static CustomOption plagueDoctorImmunityTime;
        public static CustomOption plagueDoctorInfectKiller;
        public static CustomOption plagueDoctorResetMeeting;
        public static CustomOption plagueDoctorWinDead;

        public static CustomRoleOption nekoKabochaSpawnRate;
        public static CustomOption nekoKabochaRevengeCrew;
        public static CustomOption nekoKabochaRevengeNeutral;
        public static CustomOption nekoKabochaRevengeImpostor;
        public static CustomOption nekoKabochaRevengeExile;

        public static CustomDualRoleOption watcherSpawnRate;

        public static CustomOption hideSettings;
        public static CustomOption restrictDevices;
        public static CustomOption restrictAdmin;
        public static CustomOption restrictCameras;
        public static CustomOption restrictVents;

        public static CustomOption hideOutOfSightNametags;
        public static CustomOption refundVotesOnDeath;

        public static CustomOption uselessOptions;
        public static CustomOption playerColorRandom;
        public static CustomOption playerNameDupes;
        public static CustomOption disableVents;

        public static CustomRoleOption serialKillerSpawnRate;
        public static CustomOption serialKillerKillCooldown;
        public static CustomOption serialKillerSuicideTimer;
        public static CustomOption serialKillerResetTimer;

        public static CustomRoleOption foxSpawnRate;
        public static CustomOption foxCanFixReactorAndO2;
        public static CustomOption foxCanCreateImmoralist;
        public static CustomOption foxNumRepair;
        public static CustomOption foxCrewWinsByTasks;
        public static CustomOption foxStealthCooldown;
        public static CustomOption foxStealthDuration;
        public static CustomTasksOption foxTasks;

        public static CustomRoleOption sprinterSpawnRate;
        public static CustomOption sprinterCooldown;
        public static CustomOption sprinterDuration;
        public static CustomOption sprinterSpeedBonus;

        public static CustomRoleOption akujoSpawnRate;
        public static CustomOption akujoTimeLimit;
        public static CustomOption akujoKnowsRoles;
        public static CustomOption akujoNumKeeps;

        public static CustomOption enabledHorseMode;

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


            mafiaSpawnRate = new CustomRoleOption(10, "mafia", Janitor.color, 1);
            mafiosoCanSabotage = CustomOption.Create(12, "mafiosoCanSabotage", false, mafiaSpawnRate);
            mafiosoCanRepair = CustomOption.Create(13, "mafiosoCanRepair", false, mafiaSpawnRate);
            mafiosoCanVent = CustomOption.Create(14, "mafiosoCanVent", false, mafiaSpawnRate);
            janitorCooldown = CustomOption.Create(11, "janitorCooldown", 30f, 2.5f, 60f, 2.5f, mafiaSpawnRate, format: "unitSeconds");
            janitorCanSabotage = CustomOption.Create(15, "janitorCanSabotage", false, mafiaSpawnRate);
            janitorCanRepair = CustomOption.Create(16, "janitorCanRepair", false, mafiaSpawnRate);
            janitorCanVent = CustomOption.Create(17, "janitorCanVent", false, mafiaSpawnRate);

            morphlingSpawnRate = new CustomRoleOption(20, "morphling", Morphling.color, 1);
            morphlingCooldown = CustomOption.Create(21, "morphlingCooldown", 30f, 2.5f, 60f, 2.5f, morphlingSpawnRate, format: "unitSeconds");
            morphlingDuration = CustomOption.Create(22, "morphlingDuration", 10f, 1f, 20f, 0.5f, morphlingSpawnRate, format: "unitSeconds");

            camouflagerSpawnRate = new CustomRoleOption(30, "camouflager", Camouflager.color, 1);
            camouflagerCooldown = CustomOption.Create(31, "camouflagerCooldown", 30f, 2.5f, 60f, 2.5f, camouflagerSpawnRate, format: "unitSeconds");
            camouflagerDuration = CustomOption.Create(32, "camouflagerDuration", 10f, 1f, 20f, 0.5f, camouflagerSpawnRate, format: "unitSeconds");
            camouflagerRandomColors = CustomOption.Create(33, "camouflagerRandomColors", false, camouflagerSpawnRate);

            vampireSpawnRate = new CustomRoleOption(40, "vampire", Vampire.color, 1);
            vampireKillDelay = CustomOption.Create(41, "vampireKillDelay", 10f, 1f, 20f, 1f, vampireSpawnRate, format: "unitSeconds");
            vampireCooldown = CustomOption.Create(42, "vampireCooldown", 30f, 2.5f, 60f, 2.5f, vampireSpawnRate, format: "unitSeconds");
            vampireCanKillNearGarlics = CustomOption.Create(43, "vampireCanKillNearGarlics", true, vampireSpawnRate);

            eraserSpawnRate = new CustomRoleOption(230, "eraser", Eraser.color, 1);
            eraserCooldown = CustomOption.Create(231, "eraserCooldown", 30f, 5f, 120f, 5f, eraserSpawnRate, format: "unitSeconds");
            eraserCooldownIncrease = CustomOption.Create(233, "eraserCooldownIncrease", 10f, 0f, 120f, 2.5f, eraserSpawnRate, format: "unitSeconds");
            eraserCanEraseAnyone = CustomOption.Create(232, "eraserCanEraseAnyone", false, eraserSpawnRate);

            tricksterSpawnRate = new CustomRoleOption(250, "trickster", Trickster.color, 1);
            tricksterPlaceBoxCooldown = CustomOption.Create(251, "tricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, tricksterSpawnRate, format: "unitSeconds");
            tricksterLightsOutCooldown = CustomOption.Create(252, "tricksterLightsOutCooldown", 30f, 5f, 60f, 5f, tricksterSpawnRate, format: "unitSeconds");
            tricksterLightsOutDuration = CustomOption.Create(253, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, tricksterSpawnRate, format: "unitSeconds");

            cleanerSpawnRate = new CustomRoleOption(260, "cleaner", Cleaner.color, 1);
            cleanerCooldown = CustomOption.Create(261, "cleanerCooldown", 30f, 2.5f, 60f, 2.5f, cleanerSpawnRate, format: "unitSeconds");

            warlockSpawnRate = new CustomRoleOption(270, "warlock", Warlock.color, 1);
            warlockCooldown = CustomOption.Create(271, "warlockCooldown", 30f, 2.5f, 60f, 2.5f, warlockSpawnRate, format: "unitSeconds");
            warlockRootTime = CustomOption.Create(272, "warlockRootTime", 5f, 0f, 15f, 1f, warlockSpawnRate, format: "unitSeconds");

            bountyHunterSpawnRate = new CustomRoleOption(320, "bountyHunter", BountyHunter.color, 1);
            bountyHunterBountyDuration = CustomOption.Create(321, "bountyHunterBountyDuration", 60f, 10f, 180f, 10f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterReducedCooldown = CustomOption.Create(322, "bountyHunterReducedCooldown", 2.5f, 2.5f, 30f, 2.5f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterPunishmentTime = CustomOption.Create(323, "bountyHunterPunishmentTime", 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate, format: "unitSeconds");
            bountyHunterShowArrow = CustomOption.Create(324, "bountyHunterShowArrow", true, bountyHunterSpawnRate);
            bountyHunterArrowUpdateIntervall = CustomOption.Create(325, "bountyHunterArrowUpdateIntervall", 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow, format: "unitSeconds");

            witchSpawnRate = new CustomRoleOption(390, "witch", Witch.color, 1);
            witchCooldown = CustomOption.Create(391, "witchSpellCooldown", 30f, 2.5f, 120f, 2.5f, witchSpawnRate, format: "unitSeconds");
            witchAdditionalCooldown = CustomOption.Create(392, "witchAdditionalCooldown", 10f, 0f, 60f, 5f, witchSpawnRate, format: "unitSeconds");
            witchCanSpellAnyone = CustomOption.Create(393, "witchCanSpellAnyone", false, witchSpawnRate);
            witchSpellCastingDuration = CustomOption.Create(394, "witchSpellDuration", 1f, 0f, 10f, 1f, witchSpawnRate, format: "unitSeconds");
            witchTriggerBothCooldowns = CustomOption.Create(395, "witchTriggerBoth", true, witchSpawnRate);
            witchVoteSavesTargets = CustomOption.Create(396, "witchSaveTargets", true, witchSpawnRate);


            ninjaSpawnRate = new CustomRoleOption(1000, "ninja", Ninja.color, 3);
            ninjaStealthCooldown = CustomOption.Create(1002, "ninjaStealthCooldown", 30f, 2.5f, 60f, 2.5f, ninjaSpawnRate, format: "unitSeconds");
            ninjaStealthDuration = CustomOption.Create(1003, "ninjaStealthDuration", 15f, 2.5f, 60f, 2.5f, ninjaSpawnRate, format: "unitSeconds");
            ninjaFadeTime = CustomOption.Create(1004, "ninjaFadeTime", 0.5f, 0.0f, 2.5f, 0.5f, ninjaSpawnRate, format: "unitSeconds");
            ninjaKillPenalty = CustomOption.Create(1005, "ninjaKillPenalty", 10f, 0f, 60f, 2.5f, ninjaSpawnRate, format: "unitSeconds");
            ninjaSpeedBonus = CustomOption.Create(1006, "ninjaSpeedBonus", 125f, 50f, 200f, 5f, ninjaSpawnRate, format: "unitPercent");
            ninjaCanBeTargeted = CustomOption.Create(1007, "ninjaCanBeTargeted", true, ninjaSpawnRate);
            ninjaCanVent = CustomOption.Create(1008, "ninjaCanVent", false, ninjaSpawnRate);

            serialKillerSpawnRate = new CustomRoleOption(1010, "serialKiller", SerialKiller.color, 3);
            serialKillerKillCooldown = CustomOption.Create(1012, "serialKillerKillCooldown", 15f, 2.5f, 60f, 2.5f, serialKillerSpawnRate, format: "unitSeconds");
            serialKillerSuicideTimer = CustomOption.Create(1013, "serialKillerSuicideTimer", 40f, 2.5f, 60f, 2.5f, serialKillerSpawnRate, format: "unitSeconds");
            serialKillerResetTimer = CustomOption.Create(1014, "serialKillerResetTimer", true, serialKillerSpawnRate);

            nekoKabochaSpawnRate = new CustomRoleOption(1020, "nekoKabocha", NekoKabocha.color, 3);
            nekoKabochaRevengeCrew = CustomOption.Create(1021, "nekoKabochaRevengeCrew", true, nekoKabochaSpawnRate);
            nekoKabochaRevengeNeutral = CustomOption.Create(1022, "nekoKabochaRevengeNeutral", true, nekoKabochaSpawnRate);
            nekoKabochaRevengeImpostor = CustomOption.Create(1023, "nekoKabochaRevengeImpostor", true, nekoKabochaSpawnRate);
            nekoKabochaRevengeExile = CustomOption.Create(1024, "nekoKabochaRevengeExile", false, nekoKabochaSpawnRate);


            madmateSpawnRate = new CustomRoleOption(360, "madmate", Madmate.color);
            madmateType = CustomOption.Create(366, "madmateType", new string[] { "madmateDefault", "madmateWithRole", "madmateRandom" }, madmateSpawnRate);
            madmateFixedRole = new CustomRoleSelectionOption(369, "madmateFixedRole", Madmate.validRoles, madmateType);
            madmateAbility = CustomOption.Create(367, "madmateAbility", new string[] { "madmateNone", "madmateFanatic" }, madmateSpawnRate);
            madmateTasks = new CustomTasksOption(368, 1, 1, 3, madmateAbility);
            madmateCanDieToSheriff = CustomOption.Create(361, "madmateCanDieToSheriff", false, madmateSpawnRate);
            madmateCanEnterVents = CustomOption.Create(362, "madmateCanEnterVents", false, madmateSpawnRate);
            madmateHasImpostorVision = CustomOption.Create(363, "madmateHasImpostorVision", false, madmateSpawnRate);
            madmateCanSabotage = CustomOption.Create(364, "madmateCanSabotage", false, madmateSpawnRate);
            madmateCanFixComm = CustomOption.Create(365, "madmateCanFixComm", true, madmateSpawnRate);

            miniSpawnRate = new CustomRoleOption(180, "mini", Mini.color, 1);
            miniIsImpRate = CustomOption.Create(182, "miniIsImpRate", rates, miniSpawnRate);
            miniGrowingUpDuration = CustomOption.Create(181, "miniGrowingUpDuration", 400f, 100f, 1500f, 100f, miniSpawnRate, format: "unitSeconds");

            loversSpawnRate = new CustomRoleOption(50, "lovers", Lovers.color, 1);
            loversImpLoverRate = CustomOption.Create(51, "loversImpLoverRate", rates, loversSpawnRate);
            loversNumCouples = CustomOption.Create(57, "loversNumCouples", 1f, 1f, 7f, 1f, loversSpawnRate, format: "unitCouples");
            loversBothDie = CustomOption.Create(52, "loversBothDie", true, loversSpawnRate);
            loversCanHaveAnotherRole = CustomOption.Create(53, "loversCanHaveAnotherRole", true, loversSpawnRate);
            loversSeparateTeam = CustomOption.Create(56, "loversSeparateTeam", true, loversSpawnRate);
            loversTasksCount = CustomOption.Create(55, "loversTasksCount", false, loversSpawnRate);
			loversEnableChat = CustomOption.Create(54, "loversEnableChat", true, loversSpawnRate);

            guesserSpawnRate = new CustomRoleOption(310, "guesser", Guesser.color, 1);
            guesserIsImpGuesserRate = CustomOption.Create(311, "guesserIsImpGuesserRate", rates, guesserSpawnRate);
            guesserSpawnBothRate = CustomOption.Create(317, "guesserSpawnBothRate", rates, guesserSpawnRate);
            guesserNumberOfShots = CustomOption.Create(312, "guesserNumberOfShots", 2f, 1f, 15f, 1f, guesserSpawnRate, format: "unitShots");
            guesserOnlyAvailableRoles = CustomOption.Create(313, "guesserOnlyAvailableRoles", true, guesserSpawnRate);
            guesserHasMultipleShotsPerMeeting = CustomOption.Create(314, "guesserHasMultipleShotsPerMeeting", false, guesserSpawnRate);
            guesserShowInfoInGhostChat = CustomOption.Create(315, "guesserToGhostChat", true, guesserSpawnRate);
            guesserKillsThroughShield = CustomOption.Create(316, "guesserPierceShield", true, guesserSpawnRate);
            guesserEvilCanKillSpy = CustomOption.Create(318, "guesserEvilCanKillSpy", true, guesserSpawnRate);
			guesserCantGuessSnitchIfTaksDone = CustomOption.Create(319, "guesserCantGuessSnitchIfTaksDone", true, guesserSpawnRate);

            swapperSpawnRate = new CustomRoleOption(150, "swapper", Swapper.color, 1);
            swapperIsImpRate = CustomOption.Create(153, "swapperIsImpRate", rates, swapperSpawnRate);
            swapperNumSwaps = CustomOption.Create(154, "swapperNumSwaps", 2f, 1f, 15f, 1f, swapperSpawnRate, format: "unitTimes");
            swapperCanCallEmergency = CustomOption.Create(151, "swapperCanCallEmergency", false, swapperSpawnRate);
            swapperCanOnlySwapOthers = CustomOption.Create(152, "swapperCanOnlySwapOthers", false, swapperSpawnRate);

            jesterSpawnRate = new CustomRoleOption(60, "jester", Jester.color, 1);
            jesterCanCallEmergency = CustomOption.Create(61, "jesterCanCallEmergency", true, jesterSpawnRate);
			jesterCanSabotage = CustomOption.Create(62, "jesterCanSabotage", true, jesterSpawnRate);
            jesterHasImpostorVision = CustomOption.Create(63, "jesterHasImpostorVision", false, jesterSpawnRate);

            arsonistSpawnRate = new CustomRoleOption(290, "arsonist", Arsonist.color, 1);
            arsonistCooldown = CustomOption.Create(291, "arsonistCooldown", 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate, format: "unitSeconds");
            arsonistDuration = CustomOption.Create(292, "arsonistDuration", 3f, 0f, 10f, 1f, arsonistSpawnRate, format: "unitSeconds");
            arsonistCanBeLovers = CustomOption.Create(293, "arsonistCanBeLovers", false, arsonistSpawnRate);

            opportunistSpawnRate = new CustomRoleOption(380, "opportunist", Opportunist.color);

            jackalSpawnRate = new CustomRoleOption(220, "jackal", Jackal.color, 1);
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
            jackalCanCreateSidekickFromFox = CustomOption.Create(431, "jackalCanCreateSidekickFromFox", true, jackalCanCreateSidekick);

            vultureSpawnRate = new CustomRoleOption(340, "vulture", Vulture.color, 1);
            vultureCooldown = CustomOption.Create(341, "vultureCooldown", 15f, 2.5f, 60f, 2.5f, vultureSpawnRate, format: "unitSeconds");
            vultureNumberToWin = CustomOption.Create(342, "vultureNumberToWin", 4f, 1f, 12f, 1f, vultureSpawnRate);
            vultureCanUseVents = CustomOption.Create(343, "vultureCanUseVents", true, vultureSpawnRate);
            vultureShowArrows = CustomOption.Create(344, "vultureShowArrows", true, vultureSpawnRate);

            lawyerSpawnRate = new CustomRoleOption(350, "lawyer", Lawyer.color, 1);
            lawyerTargetKnows = CustomOption.Create(351, "lawyerTargetKnows", true, lawyerSpawnRate);
            lawyerWinsAfterMeetings = CustomOption.Create(352, "lawyerWinsMeeting", false, lawyerSpawnRate);
            lawyerNeededMeetings = CustomOption.Create(353, "lawyerMeetingsNeeded", 5f, 1f, 15f, 1f, lawyerWinsAfterMeetings);
            lawyerVision = CustomOption.Create(354, "lawyerVision", 1f, 0.25f, 3f, 0.25f, lawyerSpawnRate, format: "unitMultiplier");
            lawyerKnowsRole = CustomOption.Create(355, "lawyerKnowsRole", false, lawyerSpawnRate);
            pursuerCooldown = CustomOption.Create(356, "pursuerBlankCool", 30f, 2.5f, 60f, 2.5f, lawyerSpawnRate, format: "unitSeconds");
            pursuerBlanksNumber = CustomOption.Create(357, "pursuerNumBlanks", 5f, 0f, 20f, 1f, lawyerSpawnRate, format: "unitShots");

            shifterSpawnRate = new CustomRoleOption(70, "shifter", Shifter.color, 1);
            shifterIsNeutralRate = CustomOption.Create(72, "shifterIsNeutralRate", rates, shifterSpawnRate);
            shifterShiftsModifiers = CustomOption.Create(71, "shifterShiftsModifiers", false, shifterSpawnRate);
            shifterPastShifters = CustomOption.Create(73, "shifterPastShifters", false, shifterSpawnRate);

            plagueDoctorSpawnRate = new CustomRoleOption(900, "plagueDoctor", PlagueDoctor.color, 1);
            plagueDoctorInfectCooldown = CustomOption.Create(901, "plagueDoctorInfectCooldown", 10f, 2.5f, 60f, 2.5f, plagueDoctorSpawnRate, format: "unitSeconds");
            plagueDoctorNumInfections = CustomOption.Create(902, "plagueDoctorNumInfections", 1f, 1f, 15, 1f, plagueDoctorSpawnRate, format: "unitPlayers");
            plagueDoctorDistance = CustomOption.Create(903, "plagueDoctorDistance", 1.0f, 0.25f, 5.0f, 0.25f, plagueDoctorSpawnRate, format: "unitMeters");
            plagueDoctorDuration = CustomOption.Create(904, "plagueDoctorDuration", 5f, 1f, 30f, 1f, plagueDoctorSpawnRate, format: "unitSeconds");
            plagueDoctorImmunityTime = CustomOption.Create(905, "plagueDoctorImmunityTime", 10f, 1f, 30f, 1f, plagueDoctorSpawnRate, format: "unitSeconds");
            //plagueDoctorResetMeeting = CustomOption.Create(907, "plagueDoctorResetMeeting", false, plagueDoctorSpawnRate);
            plagueDoctorInfectKiller = CustomOption.Create(906, "plagueDoctorInfectKiller", true, plagueDoctorSpawnRate);
            plagueDoctorWinDead = CustomOption.Create(908, "plagueDoctorWinDead", true, plagueDoctorSpawnRate);


            watcherSpawnRate = new CustomDualRoleOption(1040, "watcher", Watcher.color, RoleType.Watcher, 15);


            akujoSpawnRate = new CustomRoleOption(1060, "akujo", Akujo.color, 7, roleEnabled: false);
            akujoTimeLimit = CustomOption.Create(1061, "akujoTimeLimit", 300f, 30f, 1200f, 30f, akujoSpawnRate, format: "unitSeconds");
            akujoKnowsRoles = CustomOption.Create(1062, "akujoKnowsRoles", false, akujoSpawnRate);
            akujoNumKeeps = CustomOption.Create(1063, "akujoNumKeeps", 1f, 1f, 15f, 1f, akujoSpawnRate, format: "unitPlayers");


            foxSpawnRate = new CustomRoleOption(910, "fox", Fox.color, 1);
            foxCanFixReactorAndO2 = CustomOption.Create(911, "foxCanFixReactorAndO2", false, foxSpawnRate);
            foxCrewWinsByTasks = CustomOption.Create(912, "foxCrewWinsByTasks", true, foxSpawnRate);
            foxTasks = new CustomTasksOption(913, 1, 1, 3, foxSpawnRate);
            foxStealthCooldown = CustomOption.Create(916, "foxStealthCooldown", 15f, 1f, 30f, 1f, foxSpawnRate, format: "unitSeconds");
            foxStealthDuration = CustomOption.Create(917, "foxStealthDuration", 15f, 1f, 30f, 1f, foxSpawnRate, format: "unitSeconds");
            foxCanCreateImmoralist = CustomOption.Create(918, "foxCanCreateImmoralist", true, foxSpawnRate);
            foxNumRepair = CustomOption.Create(919, "foxNumRepair", 1f, 0f, 5f, 1f, foxSpawnRate, format: "unitTimes");


            fortuneTellerSpawnRate = new CustomRoleOption(940, "fortuneTeller", FortuneTeller.color, 15);
            fortuneTellerNumTasks = CustomOption.Create(941, "fortuneTellerNumTasks", 4f, 0f, 25f, 1f, fortuneTellerSpawnRate);
            fortuneTellerResults = CustomOption.Create(942, "fortuneTellerResults ", new string[] { "fortuneTellerResultCrew", "fortuneTellerResultTeam", "fortuneTellerResultRole" }, fortuneTellerSpawnRate);
            fortuneTellerDuration = CustomOption.Create(943, "fortuneTellerDuration ", 20f, 1f, 50f, 0.5f, fortuneTellerSpawnRate, format: "unitSeconds");
            fortuneTellerDistance = CustomOption.Create(944, "fortuneTellerDistance ", 2.5f, 1f, 10f, 0.5f, fortuneTellerSpawnRate, format: "unitMeters");


            mayorSpawnRate = new CustomRoleOption(80, "mayor", Mayor.color, 1);
            mayorNumVotes = CustomOption.Create(81, "mayorNumVotes", 2f, 2f, 10f, 1f, mayorSpawnRate, format: "unitVotes");

            engineerSpawnRate = new CustomRoleOption(90, "engineer", Engineer.color, 1);
            engineerNumberOfFixes = CustomOption.Create(91, "engineerNumFixes", 1f, 0f, 3f, 1f, engineerSpawnRate);
            engineerHighlightForImpostors = CustomOption.Create(92, "engineerImpostorsSeeVent", true, engineerSpawnRate);
            engineerHighlightForTeamJackal = CustomOption.Create(93, "engineerJackalSeeVent", true, engineerSpawnRate);

            sheriffSpawnRate = new CustomRoleOption(100, "sheriff", Sheriff.color, 15);
            sheriffCooldown = CustomOption.Create(101, "sheriffCooldown", 30f, 2.5f, 60f, 2.5f, sheriffSpawnRate, format: "unitSeconds");
            sheriffNumShots = CustomOption.Create(103, "sheriffNumShots", 2f, 1f, 15f, 1f, sheriffSpawnRate, format: "unitShots");
            sheriffMisfireKillsTarget = CustomOption.Create(104, "sheriffMisfireKillsTarget", false, sheriffSpawnRate);
            sheriffCanKillNeutrals = CustomOption.Create(102, "sheriffCanKillNeutrals", false, sheriffSpawnRate);

            lighterSpawnRate = new CustomRoleOption(110, "lighter", Lighter.color, 15);
            lighterModeLightsOnVision = CustomOption.Create(111, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterModeLightsOffVision = CustomOption.Create(112, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterCooldown = CustomOption.Create(113, "lighterCooldown", 30f, 5f, 120f, 5f, lighterSpawnRate, format: "unitSeconds");
            lighterDuration = CustomOption.Create(114, "lighterDuration", 5f, 2.5f, 60f, 2.5f, lighterSpawnRate, format: "unitSeconds");
            lighterCanSeeNinja = CustomOption.Create(115, "lighterCanSeeNinja", true, lighterSpawnRate);

            detectiveSpawnRate = new CustomRoleOption(120, "detective", Detective.color, 1);
            detectiveAnonymousFootprints = CustomOption.Create(121, "detectiveAnonymousFootprints", false, detectiveSpawnRate);
            detectiveFootprintIntervall = CustomOption.Create(122, "detectiveFootprintIntervall", 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, format: "unitSeconds");
            detectiveFootprintDuration = CustomOption.Create(123, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, format: "unitSeconds");
            detectiveReportNameDuration = CustomOption.Create(124, "detectiveReportNameDuration", 0, 0, 60, 2.5f, detectiveSpawnRate, format: "unitSeconds");
            detectiveReportColorDuration = CustomOption.Create(125, "detectiveReportColorDuration", 20, 0, 120, 2.5f, detectiveSpawnRate, format: "unitSeconds");

            timeMasterSpawnRate = new CustomRoleOption(130, "timeMaster", TimeMaster.color, 1);
            timeMasterCooldown = CustomOption.Create(131, "timeMasterCooldown", 30f, 2.5f, 120f, 2.5f, timeMasterSpawnRate, format: "unitSeconds");
            timeMasterRewindTime = CustomOption.Create(132, "timeMasterRewindTime", 3f, 1f, 10f, 1f, timeMasterSpawnRate, format: "unitSeconds");
            timeMasterShieldDuration = CustomOption.Create(133, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, timeMasterSpawnRate, format: "unitSeconds");

            medicSpawnRate = new CustomRoleOption(140, "medic", Medic.color, 1);
            medicShowShielded = CustomOption.Create(143, "medicShowShielded", new string[] { "medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic" }, medicSpawnRate);
            medicShowAttemptToShielded = CustomOption.Create(144, "medicShowAttemptToShielded", false, medicSpawnRate);
            medicSetShieldAfterMeeting = CustomOption.Create(145, "medicSetShieldAfterMeeting", false, medicSpawnRate);
            medicShowAttemptToMedic = CustomOption.Create(146, "medicSeesMurderAttempt", false, medicSpawnRate);

            seerSpawnRate = new CustomRoleOption(160, "seer", Seer.color, 1);
            seerMode = CustomOption.Create(161, "seerMode", new string[] { "seerModeBoth", "seerModeFlash", "seerModeSouls" }, seerSpawnRate);
            seerLimitSoulDuration = CustomOption.Create(163, "seerLimitSoulDuration", false, seerSpawnRate);
            seerSoulDuration = CustomOption.Create(162, "seerSoulDuration", 15f, 0f, 120f, 5f, seerLimitSoulDuration, format: "unitSeconds");

            hackerSpawnRate = new CustomRoleOption(170, "hacker", Hacker.color, 1);
            hackerCooldown = CustomOption.Create(171, "hackerCooldown", 30f, 5f, 60f, 5f, hackerSpawnRate, format: "unitSeconds");
            hackerHackeringDuration = CustomOption.Create(172, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate, format: "unitSeconds");
            hackerOnlyColorType = CustomOption.Create(173, "hackerOnlyColorType", false, hackerSpawnRate);
            hackerToolsNumber = CustomOption.Create(174, "hackerToolsNumber", 5f, 1f, 30f, 1f, hackerSpawnRate);
            hackerRechargeTasksNumber = CustomOption.Create(175, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, hackerSpawnRate);
            hackerNoMove = CustomOption.Create(176, "hackerNoMove", true, hackerSpawnRate);

            trackerSpawnRate = new CustomRoleOption(200, "tracker", Tracker.color, 1);
            trackerUpdateIntervall = CustomOption.Create(201, "Tracker Update Intervall", 5f, 1f, 30f, 1f, trackerSpawnRate);
            trackerResetTargetAfterMeeting = CustomOption.Create(202, "trackerResetTargetAfterMeeting", false, trackerSpawnRate);
            trackerCanTrackCorpses = CustomOption.Create(203, "trackerTrackCorpses", true, trackerSpawnRate);
            trackerCorpsesTrackingCooldown = CustomOption.Create(204, "trackerCorpseCooldown", 30f, 0f, 120f, 5f, trackerCanTrackCorpses, format: "unitSeconds");
            trackerCorpsesTrackingDuration = CustomOption.Create(205, "trackerCorpseDuration", 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses, format: "unitSeconds");

            snitchSpawnRate = new CustomRoleOption(210, "snitch", Snitch.color, 1);
            snitchLeftTasksForReveal = CustomOption.Create(211, "snitchLeftTasksForReveal", 1f, 0f, 5f, 1f, snitchSpawnRate);
            snitchIncludeTeamJackal = CustomOption.Create(212, "snitchIncludeTeamJackal", false, snitchSpawnRate);
            snitchTeamJackalUseDifferentArrowColor = CustomOption.Create(213, "snitchTeamJackalUseDifferentArrowColor", true, snitchIncludeTeamJackal);

            spySpawnRate = new CustomRoleOption(240, "spy", Spy.color, 1);
            spyCanDieToSheriff = CustomOption.Create(241, "spyCanDieToSheriff", false, spySpawnRate);
            spyImpostorsCanKillAnyone = CustomOption.Create(242, "spyImpostorsCanKillAnyone", true, spySpawnRate);
            spyCanEnterVents = CustomOption.Create(243, "spyCanEnterVents", false, spySpawnRate);
            spyHasImpostorVision = CustomOption.Create(244, "spyHasImpostorVision", false, spySpawnRate);

            securityGuardSpawnRate = new CustomRoleOption(280, "securityGuard", SecurityGuard.color, 1);
            securityGuardCooldown = CustomOption.Create(281, "securityGuardCooldown", 30f, 2.5f, 60f, 2.5f, securityGuardSpawnRate, format: "unitSeconds");
            securityGuardTotalScrews = CustomOption.Create(282, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardCamPrice = CustomOption.Create(283, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardVentPrice = CustomOption.Create(284, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate, format: "unitScrews");
            securityGuardCamDuration = CustomOption.Create(285, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate, format: "unitSeconds");
            securityGuardCamMaxCharges = CustomOption.Create(286, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
            securityGuardCamRechargeTasksNumber = CustomOption.Create(287, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
            securityGuardNoMove = CustomOption.Create(288, "securityGuardNoMove", true, securityGuardSpawnRate);

            baitSpawnRate = new CustomRoleOption(330, "bait", Bait.color, 1);
            baitHighlightAllVents = CustomOption.Create(331, "baitHighlightAllVents", false, baitSpawnRate);
            baitReportDelay = CustomOption.Create(332, "baitReportDelay", 0f, 0f, 10f, 1f, baitSpawnRate, format: "unitSeconds");
            baitShowKillFlash = CustomOption.Create(333, "baitShowKillFlash", true, baitSpawnRate);

            mediumSpawnRate = new CustomRoleOption(370, "medium", Medium.color, 1);
            mediumCooldown = CustomOption.Create(371, "mediumCooldown", 30f, 5f, 120f, 5f, mediumSpawnRate, format: "unitSeconds");
            mediumDuration = CustomOption.Create(372, "mediumDuration", 3f, 0f, 15f, 1f, mediumSpawnRate, format: "unitSeconds");
            mediumOneTimeUse = CustomOption.Create(373, "mediumOneTimeUse", false, mediumSpawnRate);

            sprinterSpawnRate = new CustomRoleOption(1050, "sprinter", Sprinter.color, 15);
            sprinterCooldown = CustomOption.Create(1051, "sprinterCooldown", 30f, 2.5f, 60f, 2.5f, sprinterSpawnRate, format: "unitSeconds");
            sprinterDuration = CustomOption.Create(1052, "sprinterDuration", 15f, 2.5f, 60f, 2.5f, sprinterSpawnRate, format: "unitSeconds");
            sprinterSpeedBonus = CustomOption.Create(1053, "sprinterSpeedBonus", 125f, 50f, 200f, 5f, sprinterSpawnRate, format: "unitPercent");

            // Other options
            specialOptions = new CustomOptionBlank(null);
            maxNumberOfMeetings = CustomOption.Create(3, "maxNumberOfMeetings", 10, 0, 15, 1, specialOptions, true);
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, "blockSkippingInEmergencyMeetings", false, specialOptions);
            noVoteIsSelfVote = CustomOption.Create(5, "noVoteIsSelfVote", false, specialOptions);
            hideOutOfSightNametags = CustomOption.Create(550, "hideOutOfSightNametags", false, specialOptions);
            refundVotesOnDeath = CustomOption.Create(551, "refundVotesOnDeath", true, specialOptions);
            allowParallelMedBayScans = CustomOption.Create(540, "parallelMedbayScans", false, specialOptions);
            hideSettings = CustomOption.Create(520, "hideSettings", false, specialOptions);
            enabledHorseMode = CustomOption.Create(552, "enabledHorseMode", false, specialOptions);

            restrictDevices = CustomOption.Create(510, "restrictDevices", new string[] { "optionOff", "restrictPerTurn", "restrictPerGame" }, specialOptions);
            restrictAdmin = CustomOption.Create(501, "disableAdmin", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictCameras = CustomOption.Create(502, "disableCameras", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictVents = CustomOption.Create(503, "disableVitals", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");

            uselessOptions = CustomOption.Create(530, "uselessOptions", false, null, isHeader: true);
            dynamicMap = CustomOption.Create(8, "playRandomMaps", false, uselessOptions);
            dynamicMapEnableSkeld = CustomOption.Create(531, "dynamicMapEnableSkeld", true, dynamicMap, false);
            dynamicMapEnableMira = CustomOption.Create(532, "dynamicMapEnableMira", true, dynamicMap, false);
            dynamicMapEnablePolus = CustomOption.Create(533, "dynamicMapEnablePolus", true, dynamicMap, false);
            dynamicMapEnableAirShip = CustomOption.Create(534, "dynamicMapEnableAirShip", true, dynamicMap, false);
            //dynamicMapEnableDleks = CustomOption.Create(535, "dynamicMapEnableDleks", false, dynamicMap, false);
			
            disableVents = CustomOption.Create(504, "disableVents", false, uselessOptions);
            hidePlayerNames = CustomOption.Create(6, "hidePlayerNames", false, uselessOptions);
            playerNameDupes = CustomOption.Create(522, "playerNameDupes", false, uselessOptions);
            playerColorRandom = CustomOption.Create(521, "playerColorRandom", false, uselessOptions);

            blockedRolePairings.Add((byte)RoleType.Vampire, new [] { (byte)RoleType.Warlock});
            blockedRolePairings.Add((byte)RoleType.Warlock, new [] { (byte)RoleType.Vampire});
            blockedRolePairings.Add((byte)RoleType.Spy, new [] { (byte)RoleType.Mini});
            blockedRolePairings.Add((byte)RoleType.Mini, new [] { (byte)RoleType.Spy});
            blockedRolePairings.Add((byte)RoleType.Vulture, new [] { (byte)RoleType.Cleaner});
            blockedRolePairings.Add((byte)RoleType.Cleaner, new [] { (byte)RoleType.Vulture});

            // RoleInfo is dependent on our options, so make sure not to initialize it until
            // *after* all the options have been created (lmao programming sucks ass)
            RoleInfo.Init();
        }
    }

}
