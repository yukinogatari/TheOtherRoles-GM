using System.Net;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using TheOtherRoles.Objects;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Patches;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public static class TheOtherRoles
    {
        public static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);

        public enum Teams
        {
            Crew,
            Impostor,
            Jackal,
            Jester,
            Arsonist,
            Lovers,
            Opportunist,
            Vulture,

            None = int.MaxValue,
        }

        public static void clearAndReloadRoles()
        {
            Jester.clearAndReload();
            Mayor.clearAndReload();
            Engineer.clearAndReload();
            Sheriff.Clear();
            Lighter.Clear();
            Godfather.clearAndReload();
            Mafioso.clearAndReload();
            Janitor.clearAndReload();
            Detective.clearAndReload();
            TimeMaster.clearAndReload();
            Medic.clearAndReload();
            Seer.clearAndReload();
            Hacker.clearAndReload();
            Mini.clearAndReload();
            Tracker.clearAndReload();
            Vampire.clearAndReload();
            Snitch.clearAndReload();
            Jackal.clearAndReload();
            Sidekick.clearAndReload();
            Eraser.clearAndReload();
            Spy.clearAndReload();
            Trickster.clearAndReload();
            Cleaner.clearAndReload();
            Warlock.clearAndReload();
            SecurityGuard.clearAndReload();
            Arsonist.clearAndReload();
            Guesser.clearAndReload();
            BountyHunter.clearAndReload();
            Bait.clearAndReload();
            Vulture.clearAndReload();
            Medium.clearAndReload();
            Lawyer.clearAndReload();
            Pursuer.clearAndReload();
            Witch.clearAndReload();
            TheOtherRolesGM.clearAndReloadRoles();
        }

        public static class Jester
        {
            public static PlayerControl jester;
            public static Color color = new Color32(236, 98, 165, byte.MaxValue);

            public static bool triggerJesterWin = false;
            public static bool canCallEmergency = true;
            public static bool canSabotage = true;

            public static void clearAndReload()
            {
                jester = null;
                triggerJesterWin = false;
                canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
                canSabotage = CustomOptionHolder.jesterCanSabotage.getBool();
            }
        }

        public static class Mayor
        {
            public static PlayerControl mayor;
            public static Color color = new Color32(32, 77, 66, byte.MaxValue);
            public static int numVotes = 2;

            public static void clearAndReload()
            {
                mayor = null;
                numVotes = (int)CustomOptionHolder.mayorNumVotes.getFloat();
            }
        }

        public static class Engineer
        {
            public static PlayerControl engineer;
            public static Color color = new Color32(0, 40, 245, byte.MaxValue);
            private static Sprite buttonSprite;

            public static int remainingFixes = 1;           
            public static bool highlightForImpostors = true;
            public static bool highlightForTeamJackal = true; 

            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("RepairButton", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                engineer = null;
                remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.getFloat());
                highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.getBool();
                highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.getBool();
            }
        }

        public static class Godfather
        {
            public static PlayerControl godfather;
            public static Color color = Palette.ImpostorRed;

            public static void clearAndReload()
            {
                godfather = null;
            }
        }

        public static class Mafioso
        {
            public static PlayerControl mafioso;
            public static Color color = Palette.ImpostorRed;

            public static void clearAndReload()
            {
                mafioso = null;
            }
        }


        public static class Janitor
        {
            public static PlayerControl janitor;
            public static Color color = Palette.ImpostorRed;

            public static float cooldown = 30f;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("CleanButton", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                janitor = null;
                cooldown = CustomOptionHolder.janitorCooldown.getFloat();
            }
        }

        public static class Detective
        {
            public static PlayerControl detective;
            public static Color color = new Color32(45, 106, 165, byte.MaxValue);

            public static float footprintIntervall = 1f;
            public static float footprintDuration = 1f;
            public static bool anonymousFootprints = false;
            public static float reportNameDuration = 0f;
            public static float reportColorDuration = 20f;
            public static float timer = 6.2f;

            public static void clearAndReload()
            {
                detective = null;
                anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
                footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
                footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
                reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
                reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
                timer = 6.2f;
            }
        }

        public static class TimeMaster
        {
            public static PlayerControl timeMaster;
            public static Color color = new Color32(112, 142, 239, byte.MaxValue);

            public static bool reviveDuringRewind = false;
            public static float rewindTime = 3f;
            public static float shieldDuration = 3f;
            public static float cooldown = 30f;

            public static bool shieldActive = false;
            public static bool isRewinding = false;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("TimeShieldButton", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                timeMaster = null;
                isRewinding = false;
                shieldActive = false;
                rewindTime = CustomOptionHolder.timeMasterRewindTime.getFloat();
                shieldDuration = CustomOptionHolder.timeMasterShieldDuration.getFloat();
                cooldown = CustomOptionHolder.timeMasterCooldown.getFloat();
            }
        }

        public static class Medic
        {
            public static PlayerControl medic;
            public static PlayerControl shielded;
            public static PlayerControl futureShielded;

            public static Color color = new Color32(126, 251, 194, byte.MaxValue);
            public static bool usedShield;

            public static int showShielded = 0;
            public static bool showAttemptToShielded = false;
            public static bool showAttemptToMedic = false;
            public static bool setShieldAfterMeeting = false;

            public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
            public static PlayerControl currentTarget;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("ShieldButton", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                medic = null;
                shielded = null;
                futureShielded = null;
                currentTarget = null;
                usedShield = false;
                showShielded = CustomOptionHolder.medicShowShielded.getSelection();
                showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.getBool();
                showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.getBool();
                setShieldAfterMeeting = CustomOptionHolder.medicSetShieldAfterMeeting.getBool();
            }
        }

        public static class Seer
        {
            public static PlayerControl seer;
            public static Color color = new Color32(97, 178, 108, byte.MaxValue);
            public static List<Vector3> deadBodyPositions = new List<Vector3>();

            public static float soulDuration = 15f;
            public static bool limitSoulDuration = false;
            public static int mode = 0;

            private static Sprite soulSprite;
            public static Sprite getSoulSprite()
            {
                if (soulSprite) return soulSprite;
                soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
                return soulSprite;
            }

            public static void clearAndReload()
            {
                seer = null;
                deadBodyPositions = new List<Vector3>();
                limitSoulDuration = CustomOptionHolder.seerLimitSoulDuration.getBool();
                soulDuration = CustomOptionHolder.seerSoulDuration.getFloat();
                mode = CustomOptionHolder.seerMode.getSelection();
            }
        }

        public static class Hacker
        {
            public static PlayerControl hacker;
            public static Minigame vitals = null;
        public static Minigame doorLog = null;
            public static Color color = new Color32(117, 250, 76, byte.MaxValue);

            public static float cooldown = 30f;
            public static float duration = 10f;
            public static float toolsNumber = 5f;
            public static bool onlyColorType = false;
            public static float hackerTimer = 0f;
            public static int rechargeTasksNumber = 2;
            public static int rechargedTasks = 2;
            public static int chargesVitals = 1;
            public static int chargesAdminTable = 1;

            private static Sprite buttonSprite;
            private static Sprite vitalsSprite;
        private static Sprite logSprite;
            private static Sprite adminSprite;

            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("HackerButton", 115f);
                return buttonSprite;
            }

            public static Sprite getVitalsSprite() {
                if (vitalsSprite) return vitalsSprite;
                vitalsSprite = HudManager.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
                return vitalsSprite;
        }

        public static Sprite getLogSprite() {
            if (logSprite) return logSprite;
            logSprite = HudManager.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
            }

            public static Sprite getAdminSprite() {
                byte mapId = PlayerControl.GameOptions.MapId;
                UseButtonSettings button = HudManager.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
                if (mapId == 0 || mapId == 3) button = HudManager.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
                else if (mapId == 1) button = HudManager.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
                else if (mapId == 4) button = HudManager.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
                adminSprite = button.Image;
                return adminSprite;
            }

            public static void clearAndReload()
            {
                hacker = null;
                vitals = null;
            doorLog = null;
                hackerTimer = 0f;
                adminSprite = null;
                cooldown = CustomOptionHolder.hackerCooldown.getFloat();
                duration = CustomOptionHolder.hackerHackeringDuration.getFloat();
                onlyColorType = CustomOptionHolder.hackerOnlyColorType.getBool();
                toolsNumber = CustomOptionHolder.hackerToolsNumber.getFloat();
                rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
                rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
                chargesVitals = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
                chargesAdminTable = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
            }
        }

        public static class Mini
        {
            public static PlayerControl mini;
            public static Color color = Color.white;
            public const float defaultColliderRadius = 0.2233912f;
            public const float defaultColliderOffset = 0.3636057f;

            public static float growingUpDuration = 400f;
            public static DateTime timeOfGrowthStart = DateTime.UtcNow;
            public static bool triggerMiniLose = false;

            public static void clearAndReload()
            {
                mini = null;
                triggerMiniLose = false;
                growingUpDuration = CustomOptionHolder.miniGrowingUpDuration.getFloat();
                timeOfGrowthStart = DateTime.UtcNow;
            }

            public static float growingProgress()
            {
                if (timeOfGrowthStart == null) return 0f;

                float timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
                return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
            }

            public static bool isGrownUp()
            {
                return growingProgress() == 1f;
            }
        }

        public static class Tracker
        {
            public static PlayerControl tracker;
            public static Color color = new Color32(100, 58, 220, byte.MaxValue);
        public static List<Arrow> localArrows = new List<Arrow>();

            public static float updateIntervall = 5f;
            public static bool resetTargetAfterMeeting = false;
        public static bool canTrackCorpses = false;
        public static float corpsesTrackingCooldown = 30f;
        public static float corpsesTrackingDuration = 5f;
        public static float corpsesTrackingTimer = 0f;
        public static List<Vector3> deadBodyPositions = new List<Vector3>();

            public static PlayerControl currentTarget;
            public static PlayerControl tracked;
            public static bool usedTracker = false;
            public static float timeUntilUpdate = 0f;
            public static Arrow arrow = new Arrow(Color.blue);

        private static Sprite trackCorpsesButtonSprite;
        public static Sprite getTrackCorpsesButtonSprite()
        {
            if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
            trackCorpsesButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PathfindButton.png", 115f);
            return trackCorpsesButtonSprite;
        }

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("TrackerButton", 115f);
                return buttonSprite;
            }

            public static void resetTracked()
            {
                currentTarget = tracked = null;
                usedTracker = false;
                if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                arrow = new Arrow(Color.blue);
                if (arrow.arrow != null) arrow.arrow.SetActive(false);
            }

            public static void clearAndReload()
            {
                tracker = null;
                resetTracked();
                timeUntilUpdate = 0f;
                updateIntervall = CustomOptionHolder.trackerUpdateIntervall.getFloat();
                resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.getBool();
            if (localArrows != null) {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            deadBodyPositions = new List<Vector3>();
            corpsesTrackingTimer = 0f;
            corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.getFloat();
            corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.getFloat();
            canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.getBool();
            }
        }

        public static class Vampire
        {
            public static PlayerControl vampire;
            public static Color color = Palette.ImpostorRed;

            public static float delay = 10f;
            public static float cooldown = 30f;
            public static bool canKillNearGarlics = true;
            public static bool localPlacedGarlic = false;
            public static bool garlicsActive = true;

            public static PlayerControl currentTarget;
            public static PlayerControl bitten;
            public static bool targetNearGarlic = false;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("VampireButton", 115f);
                return buttonSprite;
            }

            private static Sprite garlicButtonSprite;
            public static Sprite getGarlicButtonSprite()
            {
                if (garlicButtonSprite) return garlicButtonSprite;
                garlicButtonSprite = ModTranslation.getImage("GarlicButton", 115f);
                return garlicButtonSprite;
            }

            public static void clearAndReload()
            {
                vampire = null;
                bitten = null;
                targetNearGarlic = false;
                localPlacedGarlic = false;
                currentTarget = null;
                garlicsActive = CustomOptionHolder.vampireSpawnRate.getSelection() > 0;
                delay = CustomOptionHolder.vampireKillDelay.getFloat();
                cooldown = CustomOptionHolder.vampireCooldown.getFloat();
                canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.getBool();
            }
        }

        public static class Snitch
        {
            public static PlayerControl snitch;
            public static Color color = new Color32(184, 251, 79, byte.MaxValue);

            public static List<Arrow> localArrows = new List<Arrow>();
            public static int taskCountForReveal = 1;
            public static bool includeTeamJackal = false;
            public static bool teamJackalUseDifferentArrowColor = true;


            public static void clearAndReload()
            {
                if (localArrows != null)
                {
                    foreach (Arrow arrow in localArrows)
                        if (arrow?.arrow != null)
                            UnityEngine.Object.Destroy(arrow.arrow);
                }
                localArrows = new List<Arrow>();
                taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
                includeTeamJackal = CustomOptionHolder.snitchIncludeTeamJackal.getBool();
                teamJackalUseDifferentArrowColor = CustomOptionHolder.snitchTeamJackalUseDifferentArrowColor.getBool();
                snitch = null;
            }
        }

        public static class Jackal
        {
            public static PlayerControl jackal;
            public static Color color = new Color32(0, 180, 235, byte.MaxValue);
            public static PlayerControl fakeSidekick;
            public static PlayerControl currentTarget;
            public static List<PlayerControl> formerJackals = new List<PlayerControl>();

            public static float cooldown = 30f;
            public static float createSidekickCooldown = 30f;
            public static bool canUseVents = true;
            public static bool canCreateSidekick = true;
            public static Sprite buttonSprite;
            public static bool jackalPromotedFromSidekickCanCreateSidekick = true;
            public static bool canCreateSidekickFromImpostor = true;
            public static bool canCreateSidekickFromFox = true;
            public static bool hasImpostorVision = false;

            public static Sprite getSidekickButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("SidekickButton", 115f);
                return buttonSprite;
            }

            public static void removeCurrentJackal()
            {
                if (!formerJackals.Any(x => x.PlayerId == jackal.PlayerId)) formerJackals.Add(jackal);
                jackal = null;
                currentTarget = null;
                fakeSidekick = null;
                cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
                createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
            }

            public static void clearAndReload()
            {
                jackal = null;
                currentTarget = null;
                fakeSidekick = null;
                cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
                createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
                canUseVents = CustomOptionHolder.jackalCanUseVents.getBool();
                canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.getBool();
                jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.getBool();
                canCreateSidekickFromImpostor = CustomOptionHolder.jackalCanCreateSidekickFromImpostor.getBool();
                formerJackals.Clear();
                hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            }

        }

        public static class Sidekick
        {
            public static PlayerControl sidekick;
            public static Color color = new Color32(0, 180, 235, byte.MaxValue);

            public static PlayerControl currentTarget;

            public static float cooldown = 30f;
            public static bool canUseVents = true;
            public static bool canKill = true;
            public static bool promotesToJackal = true;
            public static bool hasImpostorVision = false;

            public static void clearAndReload()
            {
                sidekick = null;
                currentTarget = null;
                cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
                canUseVents = CustomOptionHolder.sidekickCanUseVents.getBool();
                canKill = CustomOptionHolder.sidekickCanKill.getBool();
                promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.getBool();
                hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            }
        }

        public static class Eraser
        {
            public static PlayerControl eraser;
            public static Color color = Palette.ImpostorRed;

            public static List<PlayerControl> futureErased = new List<PlayerControl>();
            public static PlayerControl currentTarget;
            public static float cooldown = 30f;
            public static float cooldownIncrease = 10f;
            public static bool canEraseAnyone = false;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("EraserButton", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                eraser = null;
                futureErased = new List<PlayerControl>();
                currentTarget = null;
                cooldown = CustomOptionHolder.eraserCooldown.getFloat();
                cooldownIncrease = CustomOptionHolder.eraserCooldownIncrease.getFloat();
                canEraseAnyone = CustomOptionHolder.eraserCanEraseAnyone.getBool();
            }
        }

        public static class Spy
        {
            public static PlayerControl spy;
            public static Color color = Palette.ImpostorRed;

            public static bool impostorsCanKillAnyone = true;
            public static bool canEnterVents = false;
            public static bool hasImpostorVision = false;

            public static void clearAndReload()
            {
                spy = null;
                impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.getBool();
                canEnterVents = CustomOptionHolder.spyCanEnterVents.getBool();
                hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.getBool();
            }
        }

        public static class Trickster
        {
            public static PlayerControl trickster;
            public static Color color = Palette.ImpostorRed;
            public static float placeBoxCooldown = 30f;
            public static float lightsOutCooldown = 30f;
            public static float lightsOutDuration = 10f;
            public static float lightsOutTimer = 0f;

            private static Sprite placeBoxButtonSprite;
            private static Sprite lightOutButtonSprite;
            private static Sprite tricksterVentButtonSprite;

            public static Sprite getPlaceBoxButtonSprite()
            {
                if (placeBoxButtonSprite) return placeBoxButtonSprite;
                placeBoxButtonSprite = ModTranslation.getImage("PlaceJackInTheBoxButton", 115f);
                return placeBoxButtonSprite;
            }

            public static Sprite getLightsOutButtonSprite()
            {
                if (lightOutButtonSprite) return lightOutButtonSprite;
                lightOutButtonSprite = ModTranslation.getImage("LightsOutButton", 115f);
                return lightOutButtonSprite;
            }

            public static Sprite getTricksterVentButtonSprite()
            {
                if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
                tricksterVentButtonSprite = ModTranslation.getImage("TricksterVentButton", 115f);
                return tricksterVentButtonSprite;
            }

            public static void clearAndReload()
            {
                trickster = null;
                lightsOutTimer = 0f;
                placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.getFloat();
                lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.getFloat();
                lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.getFloat();
                JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
            }

        }

        public static class Cleaner
        {
            public static PlayerControl cleaner;
            public static Color color = Palette.ImpostorRed;

            public static float cooldown = 30f;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = ModTranslation.getImage("CleanButton", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                cleaner = null;
                cooldown = CustomOptionHolder.cleanerCooldown.getFloat();
            }
        }

        public static class Warlock
        {

            public static PlayerControl warlock;
            public static Color color = Palette.ImpostorRed;

            public static PlayerControl currentTarget;
            public static PlayerControl curseVictim;
            public static PlayerControl curseVictimTarget;

            public static float cooldown = 30f;
            public static float rootTime = 5f;

            private static Sprite curseButtonSprite;
            private static Sprite curseKillButtonSprite;

            public static Sprite getCurseButtonSprite()
            {
                if (curseButtonSprite) return curseButtonSprite;
                curseButtonSprite = ModTranslation.getImage("CurseButton", 115f);
                return curseButtonSprite;
            }

            public static Sprite getCurseKillButtonSprite()
            {
                if (curseKillButtonSprite) return curseKillButtonSprite;
                curseKillButtonSprite = ModTranslation.getImage("CurseKillButton", 115f);
                return curseKillButtonSprite;
            }

            public static void clearAndReload()
            {
                warlock = null;
                currentTarget = null;
                curseVictim = null;
                curseVictimTarget = null;
                cooldown = CustomOptionHolder.warlockCooldown.getFloat();
                rootTime = CustomOptionHolder.warlockRootTime.getFloat();
            }

            public static void resetCurse()
            {
                HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
                HudManagerStartPatch.warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
            HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                currentTarget = null;
                curseVictim = null;
                curseVictimTarget = null;
            }
        }

        public static class SecurityGuard
        {
            public static PlayerControl securityGuard;
            public static Color color = new Color32(195, 178, 95, byte.MaxValue);

            public static float cooldown = 30f;
            public static int remainingScrews = 7;
            public static int totalScrews = 7;
            public static int ventPrice = 1;
            public static int camPrice = 2;
            public static int placedCameras = 0;
            public static Vent ventTarget = null;

            private static Sprite closeVentButtonSprite;
            public static Sprite getCloseVentButtonSprite()
            {
                if (closeVentButtonSprite) return closeVentButtonSprite;
                closeVentButtonSprite = ModTranslation.getImage("CloseVentButton", 115f);
                return closeVentButtonSprite;
            }

            private static Sprite placeCameraButtonSprite;
            public static Sprite getPlaceCameraButtonSprite()
            {
                if (placeCameraButtonSprite) return placeCameraButtonSprite;
                placeCameraButtonSprite = ModTranslation.getImage("PlaceCameraButton", 115f);
                return placeCameraButtonSprite;
            }

            private static Sprite animatedVentSealedSprite;
            public static Sprite getAnimatedVentSealedSprite()
            {
                if (animatedVentSealedSprite) return animatedVentSealedSprite;
                animatedVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AnimatedVentSealed.png", 160f);
                return animatedVentSealedSprite;
            }

            private static Sprite staticVentSealedSprite;
            public static Sprite getStaticVentSealedSprite()
            {
                if (staticVentSealedSprite) return staticVentSealedSprite;
                staticVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.StaticVentSealed.png", 160f);
                return staticVentSealedSprite;
            }

            public static void clearAndReload()
            {
                securityGuard = null;
                ventTarget = null;
                placedCameras = 0;
                cooldown = CustomOptionHolder.securityGuardCooldown.getFloat();
                totalScrews = remainingScrews = Mathf.RoundToInt(CustomOptionHolder.securityGuardTotalScrews.getFloat());
                camPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamPrice.getFloat());
                ventPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardVentPrice.getFloat());
            }
        }

        public static class Arsonist
        {
            public static PlayerControl arsonist;
            public static Color color = new Color32(238, 112, 46, byte.MaxValue);

            public static float cooldown = 30f;
            public static float duration = 3f;
            public static bool triggerArsonistWin = false;
            public static bool dousedEveryone = false;

            public static PlayerControl currentTarget;
            public static PlayerControl douseTarget;
            public static List<PlayerControl> dousedPlayers = new List<PlayerControl>();

            private static Sprite douseSprite;
            public static Sprite getDouseSprite()
            {
                if (douseSprite) return douseSprite;
                douseSprite = ModTranslation.getImage("DouseButton", 115f);
                return douseSprite;
            }

            private static Sprite igniteSprite;
            public static Sprite getIgniteSprite()
            {
                if (igniteSprite) return igniteSprite;
                igniteSprite = ModTranslation.getImage("IgniteButton", 115f);
                return igniteSprite;
            }

            public static bool dousedEveryoneAlive()
            {
                return PlayerControl.AllPlayerControls.ToArray().All(x => { return x == Arsonist.arsonist || x.Data.IsDead || x.Data.Disconnected || x.isGM() || Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
            }

            public static void updateStatus()
            {
                if (Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer) { 
                    dousedEveryone = dousedEveryoneAlive();
                }
            }

            public static void updateIcons()
            {
                foreach (PoolablePlayer pp in MapOptions.playerIcons.Values)
                {
                    pp.gameObject.SetActive(false);
                }

                if (Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer)
                {
                    int visibleCounter = 0;
                    Vector3 bottomLeft = HudManager.Instance.UseButton.transform.localPosition;
                    bottomLeft.x *= -1;
                    bottomLeft += new Vector3(-0.25f, -0.25f, 0);

                    foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                    {
                        if (p.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                        if (!MapOptions.playerIcons.ContainsKey(p.PlayerId)) continue;

                        if (p.Data.IsDead || p.Data.Disconnected)
                        {
                            MapOptions.playerIcons[p.PlayerId].gameObject.SetActive(false);
                        }
                        else
                        {
                            MapOptions.playerIcons[p.PlayerId].gameObject.SetActive(true);
                            MapOptions.playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.25f;
                            MapOptions.playerIcons[p.PlayerId].transform.localPosition = bottomLeft + Vector3.right * visibleCounter * 0.45f;
                            visibleCounter++;
                        }
                        bool isDoused = dousedPlayers.Any(x => x.PlayerId == p.PlayerId);
                        MapOptions.playerIcons[p.PlayerId].setSemiTransparent(!isDoused);
                    }
                }
            }

            public static void clearAndReload()
            {
                arsonist = null;
                currentTarget = null;
                douseTarget = null;
                triggerArsonistWin = false;
                dousedEveryone = false;
                dousedPlayers = new List<PlayerControl>();
                foreach (PoolablePlayer p in MapOptions.playerIcons.Values)
                {
                    if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                }
                cooldown = CustomOptionHolder.arsonistCooldown.getFloat();
                duration = CustomOptionHolder.arsonistDuration.getFloat();
            }
        }

        public static class Guesser
        {
            public static PlayerControl niceGuesser;
            public static PlayerControl evilGuesser;
            public static Color color = new Color32(255, 255, 0, byte.MaxValue);
            private static Sprite targetSprite;

            public static int remainingShotsEvilGuesser = 2;
            public static int remainingShotsNiceGuesser = 2;
            public static bool onlyAvailableRoles = true;
        	public static bool hasMultipleShotsPerMeeting = false;
            public static bool showInfoInGhostChat = true;
            public static bool killsThroughShield = true;
            public static bool evilGuesserCanGuessSpy = true;

            public static Sprite getTargetSprite()
            {
                if (targetSprite) return targetSprite;
                targetSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TargetIcon.png", 150f);
                return targetSprite;
            }

            public static bool isGuesser (byte playerId) {
                if ((niceGuesser != null && niceGuesser.PlayerId == playerId) || (evilGuesser != null && evilGuesser.PlayerId == playerId)) return true;
                return false;
            }

            public static void clear (byte playerId) {
                if (niceGuesser != null && niceGuesser.PlayerId == playerId) niceGuesser = null;
                else if (evilGuesser != null && evilGuesser.PlayerId == playerId) evilGuesser = null;
            }

            public static int remainingShots(byte playerId, bool shoot = false) {
                int remainingShots = remainingShotsEvilGuesser;
                if (niceGuesser != null && niceGuesser.PlayerId == playerId) {
                    remainingShots = remainingShotsNiceGuesser;
                    if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
                } else if (shoot) {
                    remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
                }
                return remainingShots;
            }

            public static void clearAndReload()
            {
                niceGuesser = null;
                evilGuesser = null;

                remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
                remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
                onlyAvailableRoles = CustomOptionHolder.guesserOnlyAvailableRoles.getBool();
            	hasMultipleShotsPerMeeting = CustomOptionHolder.guesserHasMultipleShotsPerMeeting.getBool();
                showInfoInGhostChat = CustomOptionHolder.guesserShowInfoInGhostChat.getBool();
                killsThroughShield = CustomOptionHolder.guesserKillsThroughShield.getBool();
                evilGuesserCanGuessSpy = CustomOptionHolder.guesserEvilCanKillSpy.getBool();
            }
        }

        public static class BountyHunter
        {
            public static PlayerControl bountyHunter;
            public static Color color = Palette.ImpostorRed;

            public static Arrow arrow;
            public static float bountyDuration = 30f;
            public static bool showArrow = true;
            public static float bountyKillCooldown = 0f;
            public static float punishmentTime = 15f;
            public static float arrowUpdateIntervall = 10f;

            public static float arrowUpdateTimer = 0f;
            public static float bountyUpdateTimer = 0f;
            public static PlayerControl bounty;
            public static TMPro.TextMeshPro cooldownText;

            public static void clearAndReload()
            {
                arrow = new Arrow(color);
                bountyHunter = null;
                bounty = null;
                arrowUpdateTimer = 0f;
                bountyUpdateTimer = 0f;
                if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                arrow = null;
                if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
                cooldownText = null;
                foreach (PoolablePlayer p in MapOptions.playerIcons.Values)
                {
                    if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                }


                bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.getFloat();
                bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.getFloat();
                punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.getFloat();
                showArrow = CustomOptionHolder.bountyHunterShowArrow.getBool();
                arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.getFloat();
            }
        }

        public static class Bait
        {
            public static PlayerControl bait;
            public static Color color = new Color32(0, 247, 255, byte.MaxValue);

            public static bool highlightAllVents = false;
            public static float reportDelay = 0f;
        	public static bool showKillFlash = true;

            public static bool reported = false;

            public static void clearAndReload()
            {
                bait = null;
                reported = false;
                highlightAllVents = CustomOptionHolder.baitHighlightAllVents.getBool();
                reportDelay = CustomOptionHolder.baitReportDelay.getFloat();
				showKillFlash = CustomOptionHolder.baitShowKillFlash.getBool();
            }
        }
    }

    public static class Vulture {
        public static PlayerControl vulture;
        public static Color color = new Color32(139, 69, 19, byte.MaxValue);
        public static List<Arrow> localArrows = new List<Arrow>();
        public static float cooldown = 30f;
        public static int vultureNumberToWin = 4;
        public static int eatenBodies = 0;
        public static bool triggerVultureWin = false;
        public static bool canUseVents = true;
        public static bool showArrows = true;
        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VultureButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            vulture = null;
            vultureNumberToWin = Mathf.RoundToInt(CustomOptionHolder.vultureNumberToWin.getFloat());
            eatenBodies = 0;
            cooldown = CustomOptionHolder.vultureCooldown.getFloat();
            triggerVultureWin = false;
            canUseVents = CustomOptionHolder.vultureCanUseVents.getBool();
            showArrows = CustomOptionHolder.vultureShowArrows.getBool();
            if (localArrows != null) {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            localArrows = new List<Arrow>();
        }
    }


    public static class Medium {
        public static PlayerControl medium;
        public static DeadPlayer target;
        public static DeadPlayer soulTarget;
        public static Color color = new Color32(98, 120, 115, byte.MaxValue);
        public static List<Tuple<DeadPlayer, Vector3>> deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public static List<Tuple<DeadPlayer, Vector3>> featureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public static List<SpriteRenderer> souls = new List<SpriteRenderer>();
        public static DateTime meetingStartTime = DateTime.UtcNow;

        public static float cooldown = 30f;
        public static float duration = 3f;
        public static bool oneTimeUse = false;

        private static Sprite soulSprite;
        public static Sprite getSoulSprite() {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
            return soulSprite;
        }

        private static Sprite question;
        public static Sprite getQuestionSprite() {
            if (question) return question;
            question = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MediumButton.png", 115f);
            return question;
        }

        public static void clearAndReload() {
            medium = null;
            target = null;
            soulTarget = null;
            deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            featureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            souls = new List<SpriteRenderer>();
            meetingStartTime = DateTime.UtcNow;
            cooldown = CustomOptionHolder.mediumCooldown.getFloat();
            duration = CustomOptionHolder.mediumDuration.getFloat();
            oneTimeUse = CustomOptionHolder.mediumOneTimeUse.getBool();
        }
    }

    public static class Lawyer {
        public static PlayerControl lawyer;
        public static PlayerControl target;
        public static Color color = new Color32(134, 153, 25, byte.MaxValue);
        public static Sprite targetSprite;
        public static bool triggerLawyerWin = false;
        public static int meetings = 0;

        public static bool targetKnows = false;
        public static float vision = 1f;
        public static bool winsAfterMeetings = false;
        public static int neededMeetings = 4;
        public static bool lawyerKnowsRole = false;

        public static Sprite getTargetSprite() {
            if (targetSprite) return targetSprite;
            targetSprite = Helpers.loadSpriteFromResources("", 150f);
            return targetSprite;
        }

        public static void clearAndReload() {
            lawyer = null;
            target = null;
            triggerLawyerWin = false;
            meetings = 0;

            targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
            winsAfterMeetings = CustomOptionHolder.lawyerWinsAfterMeetings.getBool();
            neededMeetings = Mathf.RoundToInt(CustomOptionHolder.lawyerNeededMeetings.getFloat());
            vision = CustomOptionHolder.lawyerVision.getFloat();
            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
        }
    }

    public static class Pursuer {
        public static PlayerControl pursuer;
        public static PlayerControl target;
        public static Color color = Lawyer.color;
        public static List<PlayerControl> blankedList = new List<PlayerControl>();
        public static int blanks = 0;
        public static Sprite blank;
        public static bool notAckedExiled = false;

        public static float cooldown = 30f;
        public static int blanksNumber = 5;

        public static Sprite getTargetSprite() {
            if (blank) return blank;
            blank = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PursuerButton.png", 115f);
            return blank;
        }

        public static void clearAndReload() {
            pursuer = null;
            target = null;
            blankedList = new List<PlayerControl>();
            blanks = 0;
            notAckedExiled = false;

            cooldown = CustomOptionHolder.pursuerCooldown.getFloat();
            blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.getFloat());
        }
    }

    public static class Witch {
        public static PlayerControl witch;
        public static Color color = Palette.ImpostorRed;

        public static List<PlayerControl> futureSpelled = new List<PlayerControl>();
        public static PlayerControl currentTarget;
        public static PlayerControl spellCastingTarget;
        public static float cooldown = 30f;
        public static float spellCastingDuration = 2f;
        public static float cooldownAddition = 10f;
        public static float currentCooldownAddition = 0f;
        public static bool canSpellAnyone = false;
        public static bool triggerBothCooldowns = true;
        public static bool witchVoteSavesTargets = true;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite spelledOverlaySprite;
        public static Sprite getSpelledOverlaySprite() {
            if (spelledOverlaySprite) return spelledOverlaySprite;
            spelledOverlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButtonMeeting.png", 225f);
            return spelledOverlaySprite;
        }


        public static void clearAndReload() {
            witch = null;
            futureSpelled = new List<PlayerControl>();
            currentTarget = spellCastingTarget = null;
            cooldown = CustomOptionHolder.witchCooldown.getFloat();
            cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.getFloat();
            currentCooldownAddition = CustomOptionHolder.witchCooldown.getFloat();
            canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.getBool();
            spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.getFloat();
            triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.getBool();
            witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.getBool();
        }
    }
}
