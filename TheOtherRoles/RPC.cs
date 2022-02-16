using HarmonyLib;
using Hazel;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using static TheOtherRoles.HudManagerStartPatch;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.MapOptions;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace TheOtherRoles
{
    public enum RoleType
    {
        Crewmate = 0,
        Shifter,
        Mayor,
        Engineer,
        Sheriff,
        Lighter,
        Detective,
        TimeMaster,
        Medic,
        Swapper,
        Seer,
        Hacker,
        Tracker,
        Snitch,
        Spy,
        SecurityGuard,
        Bait,
        Medium,


        Impostor = 100,
        Godfather,
        Mafioso,
        Janitor,
        Morphling,
        Camouflager,
        Vampire,
        Eraser,
        Trickster,
        Cleaner,
        Warlock,
        BountyHunter,
        Witch,
        Ninja,
        NekoKabocha,
        Madmate,
        SerialKiller,


        Mini = 150,
        Lovers,
        EvilGuesser,
        NiceGuesser,
        Jester,
        Arsonist,
        Jackal,
        Sidekick,
        Opportunist,
        Vulture,
        Lawyer,
        Pursuer,
        PlagueDoctor,


        GM = 200,


        // don't put anything below this
        NoRole = int.MaxValue
    }

    enum CustomRPC
    {
        // Main Controls

        ResetVaribles = 60,
        ShareOptions,
        ForceEnd,
        SetRole,
        SetLovers,
        VersionHandshake,
        UseUncheckedVent,
        UncheckedMurderPlayer,
        UncheckedCmdReportDeadBody,
        OverrideNativeRole,
        UncheckedExilePlayer,
        DynamicMapOption,

        // Role functionality

        EngineerFixLights = 91,
        EngineerUsedRepair,
        CleanBody,
        SheriffKill,
        MedicSetShielded,
        ShieldedMurderAttempt,
        TimeMasterShield,
        TimeMasterRewindTime,
        ShifterShift,
        SwapperSwap,
        MorphlingMorph,
        CamouflagerCamouflage,
        TrackerUsedTracker,
        VampireSetBitten,
        PlaceGarlic,
        JackalCreatesSidekick,
        SidekickPromotes,
        ErasePlayerRoles,
        SetFutureErased,
        SetFutureShifted,
        SetFutureShielded,
        SetFutureSpelled,
        WitchSpellCast,
        PlaceJackInTheBox,
        LightsOut,
        PlaceCamera,
        SealVent,
        ArsonistWin,
        GuesserShoot,
        VultureWin,
        LawyerWin,
        LawyerSetTarget,
        LawyerPromotesToPursuer,
        SetBlanked,

        // GM Edition functionality
        NinjaStealth,
        SetShifterType,
        GMKill,
        GMRevive,
        UseAdminTime,
        UseCameraTime,
        UseVitalsTime,
        ArsonistDouse,
        VultureEat,
        PlagueDoctorWin,
        PlagueDoctorSetInfected,
        PlagueDoctorUpdateProgress,
        NekoKabochaExile,
        SerialKillerSuicide,
        SwapperAnimate,
    }

    public static class RPCProcedure
    {

        // Main Controls

        public static void resetVariables()
        {
            Garlic.clearGarlics();
            JackInTheBox.clearJackInTheBoxes();
            MapOptions.clearAndReloadMapOptions();
            TheOtherRoles.clearAndReloadRoles();
            GameHistory.clearGameHistory();
            setCustomButtonCooldowns();
            AdminPatch.ResetData();
            CameraPatch.ResetData();
            VitalsPatch.ResetData();
            MapBehaviorPatch.resetIcons();
            CustomOverlays.resetOverlays();

            KillAnimationCoPerformKillPatch.hideNextAnimation = false;
        }

        public static void ShareOptions(int numberOfOptions, MessageReader reader)
        {
            try
            {
                for (int i = 0; i < numberOfOptions; i++)
                {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption option = CustomOption.options.FirstOrDefault(option => option.id == (int)optionId);
                    option.updateSelection((int)selection);
                }
            }
            catch (Exception e)
            {
                TheOtherRolesPlugin.Logger.LogError("Error while deserializing options: " + e.Message);
            }
        }

        public static void forceEnd()
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Role.IsImpostor)
                {
                    player.RemoveInfected();
                    player.MurderPlayer(player);
                    player.Data.IsDead = true;
                }
            }
        }

        public static void setRole(byte roleId, byte playerId, byte flag)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == playerId)
                {
                    player.setRole((RoleType)roleId);
                }
            }
        }

        public static void setLovers(byte playerId1, byte playerId2)
        {
            Lovers.addCouple(Helpers.playerById(playerId1), Helpers.playerById(playerId2));
        }

        public static void overrideNativeRole(byte playerId, byte roleType)
        {
            var player = Helpers.playerById(playerId);
            player.roleAssigned = false;
            DestroyableSingleton<RoleManager>.Instance.SetRole(player, (RoleTypes)roleType);
        }

        public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            System.Version ver;
            if (revision < 0)
                ver = new System.Version(major, minor, build);
            else
                ver = new System.Version(major, minor, build, revision);
            GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static void useUncheckedVent(int ventId, byte playerId, byte isEnter)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (player == null) return;
            // Fill dummy MessageReader and call MyPhysics.HandleRpc as the corountines cannot be accessed
            MessageReader reader = new MessageReader();
            byte[] bytes = BitConverter.GetBytes(ventId);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            reader.Buffer = bytes;
            reader.Length = bytes.Length;

            JackInTheBox.startAnimation(ventId);
            player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
        }

        public static void uncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation)
        {
            PlayerControl source = Helpers.playerById(sourceId);
            PlayerControl target = Helpers.playerById(targetId);
            if (source != null && target != null)
            {
                if (showAnimation == 0) KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                source.MurderPlayer(target);
            }
        }

        public static void uncheckedCmdReportDeadBody(byte sourceId, byte targetId)
        {
            PlayerControl source = Helpers.playerById(sourceId);
            PlayerControl target = Helpers.playerById(targetId);
            if (source != null && target != null) source.ReportDeadBody(target.Data);
        }

        public static void uncheckedExilePlayer(byte targetId)
        {
            PlayerControl target = Helpers.playerById(targetId);
            if (target != null) target.Exiled();
        }

        public static void dynamicMapOption(byte mapId) {
            PlayerControl.GameOptions.MapId = mapId;
        }

        // Role functionality

        public static void engineerFixLights()
        {
            SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
        }

        public static void engineerUsedRepair()
        {
            Engineer.remainingFixes--;
        }

        public static void cleanBody(byte playerId)
        {
            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++)
            {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId)
                {
                    UnityEngine.Object.Destroy(array[i].gameObject);
                }
            }
        }

        public static void sheriffKill(byte sheriffId, byte targetId, bool misfire)
        {
            PlayerControl sheriff = Helpers.playerById(sheriffId);
            PlayerControl target = Helpers.playerById(targetId);
            if (sheriff == null || target == null) return;

            Sheriff role = Sheriff.getRole(sheriff);
            if (role != null)
                role.numShots--;

            if (misfire)
            {
                sheriff.MurderPlayer(sheriff);
                finalStatuses[sheriffId] = FinalStatus.Misfire;

                if (!Sheriff.misfireKillsTarget) return;
                finalStatuses[targetId] = FinalStatus.Misfire;
            }

            sheriff.MurderPlayer(target);
        }

        public static void timeMasterRewindTime()
        {
            TimeMaster.shieldActive = false; // Shield is no longer active when rewinding
            if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == PlayerControl.LocalPlayer)
            {
                resetTimeMasterButton();
            }
            HudManager.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            HudManager.Instance.FullScreen.enabled = true;
            HudManager.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) =>
            {
                if (p == 1f) HudManager.Instance.FullScreen.enabled = false;
            })));

            if (TimeMaster.timeMaster == null || PlayerControl.LocalPlayer == TimeMaster.timeMaster) return; // Time Master himself does not rewind
            if (PlayerControl.LocalPlayer.isGM()) return; // GM does not rewind

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            PlayerControl.LocalPlayer.moveable = false;
        }

        public static void timeMasterShield()
        {
            TimeMaster.shieldActive = true;
            HudManager.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration, new Action<float>((p) =>
            {
                if (p == 1f) TimeMaster.shieldActive = false;
            })));
        }

        public static void medicSetShielded(byte shieldedId)
        {
            Medic.usedShield = true;
            Medic.shielded = Helpers.playerById(shieldedId);
            Medic.futureShielded = null;
        }

        public static void shieldedMurderAttempt()
        {
            if (Medic.shielded == null || Medic.medic == null) return;

            bool isShieldedAndShow = Medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded;
            bool isMedicAndShow = Medic.medic == PlayerControl.LocalPlayer && Medic.showAttemptToMedic;

            if ((isShieldedAndShow || isMedicAndShow) && HudManager.Instance?.FullScreen != null)
            {
                HudManager.Instance.FullScreen.enabled = true;
                HudManager.Instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
                {
                    var renderer = HudManager.Instance.FullScreen;
                    Color c = Palette.ImpostorRed;
                    if (p < 0.5)
                    {
                        if (renderer != null)
                            renderer.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(p * 2 * 0.75f));
                    }
                    else
                    {
                        if (renderer != null)
                            renderer.color = new Color(c.r, c.g, c.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
                    }
                    if (p == 1f && renderer != null) renderer.enabled = false;
                })));
            }
        }

        public static void shifterShift(byte targetId)
        {
            PlayerControl oldShifter = Shifter.shifter;
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            if (!Shifter.isNeutral)
                Shifter.clearAndReload();

            if (player == GM.gm)
            {
                return;
            }

            // Suicide (exile) when impostor or impostor variants
            if (!Shifter.isNeutral && (player.Data.Role.IsImpostor || player.isNeutral() || player.isRole(RoleType.Madmate)))
            {
                oldShifter.Exiled();
                finalStatuses[oldShifter.PlayerId] = FinalStatus.Suicide;
                return;
            }

            if (Shifter.shiftModifiers)
            {
                // Switch shield
                if (Medic.shielded != null && Medic.shielded == player)
                {
                    Medic.shielded = oldShifter;
                }
                else if (Medic.shielded != null && Medic.shielded == oldShifter)
                {
                    Medic.shielded = player;
                }

                Lovers.swapLovers(oldShifter, player);
            }

            // Shift role
            var targetRole = RoleInfo.getRoleInfoForPlayer(player, new RoleType[] { RoleType.Lovers });
            if (targetRole.Count > 0)
            {
                switch (targetRole[0].roleId)
                {
                    case RoleType.Mayor:
                        Mayor.mayor = oldShifter;
                        break;

                    case RoleType.Engineer:
                        Engineer.engineer = oldShifter;
                        break;

                    case RoleType.Sheriff:
                        Sheriff.swapRole(player, oldShifter);
                        break;

                    case RoleType.Lighter:
                        Lighter.swapRole(player, oldShifter);
                        break;

                    case RoleType.Detective:
                        Detective.detective = oldShifter;
                        break;

                    case RoleType.TimeMaster:
                        TimeMaster.timeMaster = oldShifter;
                        break;

                    case RoleType.Medic:
                        Medic.medic = oldShifter;
                        break;

                    case RoleType.Swapper:
                        Swapper.swapper = oldShifter;
                        break;

                    case RoleType.Seer:
                        Seer.seer = oldShifter;
                        break;

                    case RoleType.Hacker:
                        Hacker.hacker = oldShifter;
                        break;

                    case RoleType.Tracker:
                        Tracker.tracker = oldShifter;
                        break;

                    case RoleType.Snitch:
                        Snitch.snitch = oldShifter;
                        break;

                    case RoleType.Spy:
                        Spy.spy = oldShifter;
                        break;

                    case RoleType.SecurityGuard:
                        SecurityGuard.securityGuard = oldShifter;
                        break;

                    case RoleType.Bait:
                        Bait.bait = oldShifter;
                        if (Bait.bait.Data.IsDead) Bait.reported = true;
                        break;

                    case RoleType.Medium:
                        Medium.medium = oldShifter;
                        break;

                    case RoleType.Impostor:
                        break;

                    case RoleType.Godfather:
                        Godfather.godfather = oldShifter;
                        break;

                    case RoleType.Mafioso:
                        Mafioso.mafioso = oldShifter;
                        break;

                    case RoleType.Janitor:
                        Janitor.janitor = oldShifter;
                        break;

                    case RoleType.Morphling:
                        Morphling.morphling = oldShifter;
                        break;

                    case RoleType.Camouflager:
                        Camouflager.camouflager = oldShifter;
                        break;

                    case RoleType.Vampire:
                        Vampire.vampire = oldShifter;
                        break;

                    case RoleType.Eraser:
                        Eraser.eraser = oldShifter;
                        break;

                    case RoleType.Trickster:
                        Trickster.trickster = oldShifter;
                        break;

                    case RoleType.Cleaner:
                        Cleaner.cleaner = oldShifter;
                        break;

                    case RoleType.Warlock:
                        Warlock.warlock = oldShifter;
                        break;

                    case RoleType.BountyHunter:
                        BountyHunter.bountyHunter = oldShifter;
                        break;

                    case RoleType.Witch:
                        Witch.witch = oldShifter;
                        break;

                    case RoleType.Madmate:
                        Madmate.swapRole(player, oldShifter);
                        break;

                    case RoleType.Mini:
                        Mini.mini = oldShifter;
                        break;

                    case RoleType.EvilGuesser:
                        Guesser.evilGuesser = oldShifter;
                        break;

                    case RoleType.NiceGuesser:
                        Guesser.niceGuesser = oldShifter;
                        break;

                    case RoleType.Jester:
                        Jester.jester = oldShifter;
                        break;

                    case RoleType.Arsonist:
                        Arsonist.arsonist = oldShifter;
                        break;

                    case RoleType.Jackal:
                        Jackal.jackal = oldShifter;
                        break;

                    case RoleType.Sidekick:
                        Sidekick.sidekick = oldShifter;
                        break;

                    case RoleType.Opportunist:
                        Opportunist.swapRole(player, oldShifter);
                        break;

                    case RoleType.Vulture:
                        Vulture.vulture = oldShifter;
                        break;

                    case RoleType.Lawyer:
                        Lawyer.lawyer = oldShifter;
                        break;

                    case RoleType.Pursuer:
                        Pursuer.pursuer = oldShifter;
                        break;

                    case RoleType.Ninja:
                        Ninja.swapRole(player, oldShifter);
                        break;

                    case RoleType.PlagueDoctor:
                        PlagueDoctor.swapRole(player, oldShifter);
                        break;
                    case RoleType.SerialKiller:
                        SerialKiller.swapRole(player, oldShifter);
                        break;
                }
            }

            if (Shifter.isNeutral)
            {
                Shifter.shifter = player;
                Shifter.pastShifters.Add(oldShifter.PlayerId);

                if (player.Data.Role.IsImpostor)
                {
                    DestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                    DestroyableSingleton<RoleManager>.Instance.SetRole(oldShifter, RoleTypes.Impostor);
                }
            }

            // Set cooldowns to max for both players
            if (PlayerControl.LocalPlayer == oldShifter || PlayerControl.LocalPlayer == player)
                CustomButton.ResetAllCooldowns();
        }

        public static void swapperSwap(byte playerId1, byte playerId2)
        {
            if (MeetingHud.Instance)
            {
                Swapper.playerId1 = playerId1;
                Swapper.playerId2 = playerId2;
            }
        }

        public static void swapperAnimate()
        {
            MeetingHudPatch.animateSwap = true;
        }

        public static void morphlingMorph(byte playerId)
        {
            PlayerControl target = Helpers.playerById(playerId);
            if (Morphling.morphling == null || target == null) return;
            Morphling.startMorph(target);
        }

        public static void camouflagerCamouflage()
        {
            if (Camouflager.camouflager == null) return;
            Camouflager.startCamouflage();
        }

        public static void vampireSetBitten(byte targetId, byte performReset)
        {
            if (performReset != 0)
            {
                Vampire.bitten = null;
                return;
            }

            if (Vampire.vampire == null) return;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == targetId && !player.Data.IsDead)
                {
                    Vampire.bitten = player;
                }
            }
        }

        public static void placeGarlic(byte[] buff)
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Garlic(position);
        }

        public static void trackerUsedTracker(byte targetId)
        {
            Tracker.usedTracker = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == targetId)
                    Tracker.tracked = player;
        }

        public static void jackalCreatesSidekick(byte targetId)
        {
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null) return;

            if (!Jackal.canCreateSidekickFromImpostor && player.Data.Role.IsImpostor) {
                Jackal.fakeSidekick = player;
            } else {
                DestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                erasePlayerRoles(player.PlayerId, true);
                Sidekick.sidekick = player;
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true; 
            }
            Jackal.canCreateSidekick = false;
        }

        public static void sidekickPromotes()
        {
            Jackal.removeCurrentJackal();
            Jackal.jackal = Sidekick.sidekick;
            Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
            Sidekick.clearAndReload();
            return;
        }

        public static void erasePlayerRoles(byte playerId, bool ignoreLovers = false)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (player == null) return;

            // Don't give a former neutral role tasks because that destroys the balance.
            if (player.isNeutral())
                player.clearAllTasks();

            player.eraseAllRoles();

            if (!ignoreLovers && player.isLovers())
            { // The whole Lover couple is being erased
                Lovers.eraseCouple(player);
            }
        }

        public static void setFutureErased(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (Eraser.futureErased == null)
                Eraser.futureErased = new List<PlayerControl>();
            if (player != null)
            {
                Eraser.futureErased.Add(player);
            }
        }

        public static void setFutureShifted(byte playerId)
        {
            if (Shifter.isNeutral && !Shifter.shiftPastShifters && Shifter.pastShifters.Contains(playerId))
                return;
            Shifter.futureShift = Helpers.playerById(playerId);
        }

        public static void setFutureShielded(byte playerId)
        {
            Medic.futureShielded = Helpers.playerById(playerId);
            Medic.usedShield = true;
        }

        public static void setFutureSpelled(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (Witch.futureSpelled == null)
                Witch.futureSpelled = new List<PlayerControl>();
            if (player != null)
            {
                Witch.futureSpelled.Add(player);
            }
        }


        public static void placeJackInTheBox(byte[] buff)
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new JackInTheBox(position);
        }

        public static void lightsOut()
        {
            Trickster.lightsOutTimer = Trickster.lightsOutDuration;
            // If the local player is impostor indicate lights out
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                new CustomMessage("tricksterLightsOutText", Trickster.lightsOutDuration);
            }
        }

        public static void placeCamera(byte[] buff, byte roomId)
        {
            var referenceCamera = UnityEngine.Object.FindObjectOfType<SurvCamera>();
            if (referenceCamera == null) return; // Mira HQ

            SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
            SecurityGuard.placedCameras++;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

            SystemTypes roomType = (SystemTypes)roomId;

            var camera = UnityEngine.Object.Instantiate<SurvCamera>(referenceCamera);
            camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
            camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
            camera.Offset = new Vector3(0f, 0f, camera.Offset.z);

            switch (roomType)
            {
                case SystemTypes.Hallway: camera.NewName = StringNames.Hallway; break;
                case SystemTypes.Storage: camera.NewName = StringNames.Storage; break;
                case SystemTypes.Cafeteria: camera.NewName = StringNames.Cafeteria; break;
                case SystemTypes.Reactor: camera.NewName = StringNames.Reactor; break;
                case SystemTypes.UpperEngine: camera.NewName = StringNames.UpperEngine; break;
                case SystemTypes.Nav: camera.NewName = StringNames.Nav; break;
                case SystemTypes.Admin: camera.NewName = StringNames.Admin; break;
                case SystemTypes.Electrical: camera.NewName = StringNames.Electrical; break;
                case SystemTypes.LifeSupp: camera.NewName = StringNames.LifeSupp; break;
                case SystemTypes.Shields: camera.NewName = StringNames.Shields; break;
                case SystemTypes.MedBay: camera.NewName = StringNames.MedBay; break;
                case SystemTypes.Security: camera.NewName = StringNames.Security; break;
                case SystemTypes.Weapons: camera.NewName = StringNames.Weapons; break;
                case SystemTypes.LowerEngine: camera.NewName = StringNames.LowerEngine; break;
                case SystemTypes.Comms: camera.NewName = StringNames.Comms; break;
                case SystemTypes.Decontamination: camera.NewName = StringNames.Decontamination; break;
                case SystemTypes.Launchpad: camera.NewName = StringNames.Launchpad; break;
                case SystemTypes.LockerRoom: camera.NewName = StringNames.LockerRoom; break;
                case SystemTypes.Laboratory: camera.NewName = StringNames.Laboratory; break;
                case SystemTypes.Balcony: camera.NewName = StringNames.Balcony; break;
                case SystemTypes.Office: camera.NewName = StringNames.Office; break;
                case SystemTypes.Greenhouse: camera.NewName = StringNames.Greenhouse; break;
                case SystemTypes.Dropship: camera.NewName = StringNames.Dropship; break;
                case SystemTypes.Decontamination2: camera.NewName = StringNames.Decontamination2; break;
                case SystemTypes.Outside: camera.NewName = StringNames.Outside; break;
                case SystemTypes.Specimens: camera.NewName = StringNames.Specimens; break;
                case SystemTypes.BoilerRoom: camera.NewName = StringNames.BoilerRoom; break;
                case SystemTypes.VaultRoom: camera.NewName = StringNames.VaultRoom; break;
                case SystemTypes.Cockpit: camera.NewName = StringNames.Cockpit; break;
                case SystemTypes.Armory: camera.NewName = StringNames.Armory; break;
                case SystemTypes.Kitchen: camera.NewName = StringNames.Kitchen; break;
                case SystemTypes.ViewingDeck: camera.NewName = StringNames.ViewingDeck; break;
                case SystemTypes.HallOfPortraits: camera.NewName = StringNames.HallOfPortraits; break;
                case SystemTypes.CargoBay: camera.NewName = StringNames.CargoBay; break;
                case SystemTypes.Ventilation: camera.NewName = StringNames.Ventilation; break;
                case SystemTypes.Showers: camera.NewName = StringNames.Showers; break;
                case SystemTypes.Engine: camera.NewName = StringNames.Engine; break;
                case SystemTypes.Brig: camera.NewName = StringNames.Brig; break;
                case SystemTypes.MeetingRoom: camera.NewName = StringNames.MeetingRoom; break;
                case SystemTypes.Records: camera.NewName = StringNames.Records; break;
                case SystemTypes.Lounge: camera.NewName = StringNames.Lounge; break;
                case SystemTypes.GapRoom: camera.NewName = StringNames.GapRoom; break;
                case SystemTypes.MainHall: camera.NewName = StringNames.MainHall; break;
                case SystemTypes.Medical: camera.NewName = StringNames.Medical; break;
                default: camera.NewName = StringNames.ExitButton; break;
            }

            if (PlayerControl.GameOptions.MapId == 2 || PlayerControl.GameOptions.MapId == 4) camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

            if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard)
            {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            }
            else
            {
                camera.gameObject.SetActive(false);
            }
            MapOptions.camerasToAdd.Add(camera);
        }

        public static void sealVent(int ventId)
        {
            Vent vent = ShipStatus.Instance.AllVents.FirstOrDefault((x) => x != null && x.Id == ventId);
            if (vent == null) return;

            SecurityGuard.remainingScrews -= SecurityGuard.ventPrice;
            if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard)
            {
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
                animator?.Stop();
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                vent.myRend.sprite = animator == null ? SecurityGuard.getStaticVentSealedSprite() : SecurityGuard.getAnimatedVentSealedSprite();
                vent.myRend.color = new Color(1f, 1f, 1f, 0.5f);
                vent.name = "FutureSealedVent_" + vent.name;
            }

            MapOptions.ventsToSeal.Add(vent);
        }

        public static void arsonistDouse(byte playerId)
        {
            Arsonist.dousedPlayers.Add(Helpers.playerById(playerId));
        }

        public static void arsonistWin()
        {
            Arsonist.triggerArsonistWin = true;
            var livingPlayers = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.isRole(RoleType.Arsonist) && p.isAlive());
            foreach (PlayerControl p in livingPlayers)
            {
                p.Exiled();
                finalStatuses[p.PlayerId] = FinalStatus.Torched;
            }
        }

        public static void vultureEat(byte playerId)
        {
            cleanBody(playerId);
            Vulture.eatenBodies++;
        }

        public static void vultureWin()
        {
            Vulture.triggerVultureWin = true;
        }

        public static void lawyerWin()
        {
            Lawyer.triggerLawyerWin = true;
        }

        public static void lawyerSetTarget(byte playerId)
        {
            Lawyer.target = Helpers.playerById(playerId);
        }

        public static void lawyerPromotesToPursuer()
        {
            PlayerControl player = Lawyer.lawyer;
            PlayerControl client = Lawyer.target;
            Lawyer.clearAndReload();
            Pursuer.pursuer = player;

            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId && client != null)
            {
                Transform playerInfoTransform = client.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo != null) playerInfo.text = "";
            }
        }

        public static void guesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleId)
        {
            PlayerControl dyingTarget = Helpers.playerById(dyingTargetId);
            if (dyingTarget == null) return;
            dyingTarget.Exiled();
            PlayerControl dyingLoverPartner = Lovers.bothDie ? dyingTarget.getPartner() : null; // Lover check
            byte partnerId = dyingLoverPartner != null ? dyingLoverPartner.PlayerId : dyingTargetId;

            Guesser.remainingShots(killerId, true);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
            if (MeetingHud.Instance)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.TargetPlayerId == dyingTargetId || pva.TargetPlayerId == partnerId)
                    {
                        pva.SetDead(pva.DidReport, true);
                        pva.Overlay.gameObject.SetActive(true);
                    }

                    //Give players back their vote if target is shot dead
                    if (pva.VotedFor != dyingTargetId || pva.VotedFor != partnerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();

                }
                if (AmongUsClient.Instance.AmHost)
                    MeetingHud.Instance.CheckForEndVoting();
            }
            PlayerControl guesser = Helpers.playerById(killerId);
            if (HudManager.Instance != null && guesser != null)
                if (PlayerControl.LocalPlayer == dyingTarget)
                    HudManager.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
                else if (dyingLoverPartner != null && PlayerControl.LocalPlayer == dyingLoverPartner)
                    HudManager.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data, dyingLoverPartner.Data);

            PlayerControl guessedTarget = Helpers.playerById(guessedTargetId);
            if (Guesser.showInfoInGhostChat && PlayerControl.LocalPlayer.Data.IsDead && guessedTarget != null)
            {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                string msg = string.Format(ModTranslation.getString("guesserGuessChat"), roleInfo.name, guessedTarget.Data.PlayerName);
                if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(guesser, msg);
                if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                    DestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
            }
        }

        public static void setBlanked(byte playerId, byte value)
        {
            PlayerControl target = Helpers.playerById(playerId);
            if (target == null) return;
            Pursuer.blankedList.RemoveAll(x => x.PlayerId == playerId);
            if (value > 0) Pursuer.blankedList.Add(target);
        }

        public static void witchSpellCast(byte playerId)
        {
            uncheckedExilePlayer(playerId);
            finalStatuses[playerId] = FinalStatus.Spelled;
        }

        public static void setShifterType(bool isNeutral)
        {
            Shifter.isNeutral = isNeutral;
        }

        public static void ninjaStealth(byte playerId, bool stealthed)
        {
            PlayerControl player = Helpers.playerById(playerId);
            Ninja.setStealthed(player, stealthed);
        }

        public static void GMKill(byte targetId)
        {
            PlayerControl target = Helpers.playerById(targetId);

            if (target == null) return;
            target.MyPhysics.ExitAllVents();
            target.Exiled();
            GMUpdateMeeting(targetId, true);
            finalStatuses[target.PlayerId] = FinalStatus.GMExecuted;

            PlayerControl partner = target.getPartner(); // Lover check
            if (partner != null)
            {
                partner?.MyPhysics.ExitAllVents();
                GMUpdateMeeting(partner.PlayerId, true);
                finalStatuses[partner.PlayerId] = FinalStatus.GMExecuted;
            }

            if (HudManager.Instance != null && GM.gm != null)
            {
                if (PlayerControl.LocalPlayer == target)
                    HudManager.Instance.KillOverlay.ShowKillAnimation(GM.gm.Data, target.Data);
                else if (partner != null && PlayerControl.LocalPlayer == partner)
                    HudManager.Instance.KillOverlay.ShowKillAnimation(GM.gm.Data, partner.Data);
            }
        }

        public static void GMRevive(byte targetId)
        {
            PlayerControl target = Helpers.playerById(targetId);
            if (target == null) return;
            target.Revive();
            GMUpdateMeeting(targetId, false);
            finalStatuses[target.PlayerId] = FinalStatus.Alive;

            PlayerControl partner = target.getPartner(); // Lover check
            if (partner != null)
            {
                partner.Revive();
                GMUpdateMeeting(partner.PlayerId, false);
                finalStatuses[partner.PlayerId] = FinalStatus.Alive;
            }

            if (PlayerControl.LocalPlayer.isGM())
            {
                HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
            }
        }

        public static void GMUpdateMeeting(byte targetId, bool dead)
        {
            if (MeetingHud.Instance)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.TargetPlayerId == targetId)
                    {
                        pva.SetDead(pva.DidReport, dead);
                        pva.Overlay.gameObject.SetActive(dead);
                    }
                }
                if (AmongUsClient.Instance.AmHost)
                    MeetingHud.Instance.CheckForEndVoting();
            }
        }

        public static void UseAdminTime(float time)
        {
            MapOptions.restrictAdminTime -= time;
        }

        public static void UseCameraTime(float time)
        {
            MapOptions.restrictCamerasTime -= time;
        }

        public static void UseVitalsTime(float time)
        {
            MapOptions.restrictVitalsTime -= time;
        }

        public static void plagueDoctorWin()
        {
            PlagueDoctor.triggerPlagueDoctorWin = true;
            var livingPlayers = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.isRole(RoleType.PlagueDoctor) && p.isAlive());
            foreach (PlayerControl p in livingPlayers)
            {
                // Check again so we don't re-kill any lovers
                if (p.isAlive())
                    p.Exiled();
                finalStatuses[p.PlayerId] = FinalStatus.Diseased;
            }
        }

        public static void plagueDoctorInfected(byte targetId)
        {
            var p = Helpers.playerById(targetId);
            if (!PlagueDoctor.infected.ContainsKey(targetId))
            {
                PlagueDoctor.infected[targetId] = p;
            }
        }

        public static void plagueDoctorProgress(byte targetId, float progress)
        {
            PlagueDoctor.progress[targetId] = progress;
        }

        public static void nekoKabochaExile(byte playerId)
        {
            uncheckedExilePlayer(playerId);
            finalStatuses[playerId] = FinalStatus.Revenge;
        }

        public static void serialKillerSuicide(byte serialKillerId)
        {
            PlayerControl serialKiller = Helpers.playerById(serialKillerId);
            if (serialKiller == null) return;
            serialKiller.MurderPlayer(serialKiller);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        class RPCHandlerPatch
        {
            static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte packetId = callId;
                switch (packetId)
                {

                    // Main Controls

                    case (byte)CustomRPC.ResetVaribles:
                        RPCProcedure.resetVariables();
                        break;
                    case (byte)CustomRPC.ShareOptions:
                        RPCProcedure.ShareOptions((int)reader.ReadPackedUInt32(), reader);
                        break;
                    case (byte)CustomRPC.ForceEnd:
                        RPCProcedure.forceEnd();
                        break;
                    case (byte)CustomRPC.SetRole:
                        byte roleId = reader.ReadByte();
                        byte playerId = reader.ReadByte();
                        byte flag = reader.ReadByte();
                        RPCProcedure.setRole(roleId, playerId, flag);
                        break;
                    case (byte)CustomRPC.SetLovers:
                        RPCProcedure.setLovers(reader.ReadByte(), reader.ReadByte());
                        break;
                    case (byte)CustomRPC.OverrideNativeRole:
                        RPCProcedure.overrideNativeRole(reader.ReadByte(), reader.ReadByte());
                        break;
                    case (byte)CustomRPC.VersionHandshake:
                        int major = reader.ReadPackedInt32();
                        int minor = reader.ReadPackedInt32();
                        int patch = reader.ReadPackedInt32();
                        int versionOwnerId = reader.ReadPackedInt32();
                        byte revision = 0xFF;
                        Guid guid;
                        if (reader.Length - reader.Position >= 17)
                        { // enough bytes left to read
                            revision = reader.ReadByte();
                            // GUID
                            byte[] gbytes = reader.ReadBytes(16);
                            guid = new Guid(gbytes);
                        }
                        else
                        {
                            guid = new Guid(new byte[16]);
                        }
                        RPCProcedure.versionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                        break;
                    case (byte)CustomRPC.UseUncheckedVent:
                        int ventId = reader.ReadPackedInt32();
                        byte ventingPlayer = reader.ReadByte();
                        byte isEnter = reader.ReadByte();
                        RPCProcedure.useUncheckedVent(ventId, ventingPlayer, isEnter);
                        break;
                    case (byte)CustomRPC.UncheckedMurderPlayer:
                        byte source = reader.ReadByte();
                        byte target = reader.ReadByte();
                        byte showAnimation = reader.ReadByte();
                        RPCProcedure.uncheckedMurderPlayer(source, target, showAnimation);
                        break;
                    case (byte)CustomRPC.UncheckedExilePlayer:
                        byte exileTarget = reader.ReadByte();
                        RPCProcedure.uncheckedExilePlayer(exileTarget);
                        break;
                    case (byte)CustomRPC.UncheckedCmdReportDeadBody:
                        byte reportSource = reader.ReadByte();
                        byte reportTarget = reader.ReadByte();
                        RPCProcedure.uncheckedCmdReportDeadBody(reportSource, reportTarget);
                        break;
                    case (byte)CustomRPC.DynamicMapOption:
	                    byte mapId = reader.ReadByte();
	                    RPCProcedure.dynamicMapOption(mapId);
	                    break;

                    // Role functionality

                    case (byte)CustomRPC.EngineerFixLights:
                        RPCProcedure.engineerFixLights();
                        break;
                    case (byte)CustomRPC.EngineerUsedRepair:
                        RPCProcedure.engineerUsedRepair();
                        break;
                    case (byte)CustomRPC.CleanBody:
                        RPCProcedure.cleanBody(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SheriffKill:
                        RPCProcedure.sheriffKill(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean());
                        break;
                    case (byte)CustomRPC.TimeMasterRewindTime:
                        RPCProcedure.timeMasterRewindTime();
                        break;
                    case (byte)CustomRPC.TimeMasterShield:
                        RPCProcedure.timeMasterShield();
                        break;
                    case (byte)CustomRPC.MedicSetShielded:
                        RPCProcedure.medicSetShielded(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.ShieldedMurderAttempt:
                        RPCProcedure.shieldedMurderAttempt();
                        break;
                    case (byte)CustomRPC.ShifterShift:
                        RPCProcedure.shifterShift(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SwapperSwap:
                        byte playerId1 = reader.ReadByte();
                        byte playerId2 = reader.ReadByte();
                        RPCProcedure.swapperSwap(playerId1, playerId2);
                        break;
                    case (byte)CustomRPC.MorphlingMorph:
                        RPCProcedure.morphlingMorph(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.CamouflagerCamouflage:
                        RPCProcedure.camouflagerCamouflage();
                        break;
                    case (byte)CustomRPC.VampireSetBitten:
                        byte bittenId = reader.ReadByte();
                        byte reset = reader.ReadByte();
                        RPCProcedure.vampireSetBitten(bittenId, reset);
                        break;
                    case (byte)CustomRPC.PlaceGarlic:
                        RPCProcedure.placeGarlic(reader.ReadBytesAndSize());
                        break;
                    case (byte)CustomRPC.TrackerUsedTracker:
                        RPCProcedure.trackerUsedTracker(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.JackalCreatesSidekick:
                        RPCProcedure.jackalCreatesSidekick(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SidekickPromotes:
                        RPCProcedure.sidekickPromotes();
                        break;
                    case (byte)CustomRPC.ErasePlayerRoles:
                        RPCProcedure.erasePlayerRoles(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SetFutureErased:
                        RPCProcedure.setFutureErased(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SetFutureShifted:
                        RPCProcedure.setFutureShifted(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SetFutureShielded:
                        RPCProcedure.setFutureShielded(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.PlaceJackInTheBox:
                        RPCProcedure.placeJackInTheBox(reader.ReadBytesAndSize());
                        break;
                    case (byte)CustomRPC.LightsOut:
                        RPCProcedure.lightsOut();
                        break;
                    case (byte)CustomRPC.PlaceCamera:
                        RPCProcedure.placeCamera(reader.ReadBytesAndSize(), reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SealVent:
                        RPCProcedure.sealVent(reader.ReadPackedInt32());
                        break;
                    case (byte)CustomRPC.ArsonistWin:
                        RPCProcedure.arsonistWin();
                        break;
                    case (byte)CustomRPC.GuesserShoot:
                        byte killerId = reader.ReadByte();
                        byte dyingTarget = reader.ReadByte();
                        byte guessedTarget = reader.ReadByte();
                        byte guessedRoleId = reader.ReadByte();
                        RPCProcedure.guesserShoot(killerId, dyingTarget, guessedTarget, guessedRoleId);
                        break;
                    case (byte)CustomRPC.VultureWin:
                        RPCProcedure.vultureWin();
                        break;
                    case (byte)CustomRPC.LawyerWin:
                        RPCProcedure.lawyerWin();
                        break;
                    case (byte)CustomRPC.LawyerSetTarget:
                        RPCProcedure.lawyerSetTarget(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.LawyerPromotesToPursuer:
                        RPCProcedure.lawyerPromotesToPursuer();
                        break;
                    case (byte)CustomRPC.SetBlanked:
                        var pid = reader.ReadByte();
                        var blankedValue = reader.ReadByte();
                        RPCProcedure.setBlanked(pid, blankedValue);
                        break;
                    case (byte)CustomRPC.SetFutureSpelled:
                        RPCProcedure.setFutureSpelled(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.WitchSpellCast:
                        RPCProcedure.witchSpellCast(reader.ReadByte());
                        break;

                    // GM functionality
                    case (byte)CustomRPC.SetShifterType:
                        RPCProcedure.setShifterType(reader.ReadBoolean());
                        break;
                    case (byte)CustomRPC.NinjaStealth:
                        RPCProcedure.ninjaStealth(reader.ReadByte(), reader.ReadBoolean());
                        break;
                    case (byte)CustomRPC.ArsonistDouse:
                        RPCProcedure.arsonistDouse(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.VultureEat:
                        RPCProcedure.vultureEat(reader.ReadByte());
                        break;

                    case (byte)CustomRPC.GMKill:
                        RPCProcedure.GMKill(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.GMRevive:
                        RPCProcedure.GMRevive(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.UseAdminTime:
                        RPCProcedure.UseAdminTime(reader.ReadSingle());
                        break;
                    case (byte)CustomRPC.UseCameraTime:
                        RPCProcedure.UseCameraTime(reader.ReadSingle());
                        break;
                    case (byte)CustomRPC.UseVitalsTime:
                        RPCProcedure.UseVitalsTime(reader.ReadSingle());
                        break;
                    case (byte)CustomRPC.PlagueDoctorWin:
                        RPCProcedure.plagueDoctorWin();
                        break;
                    case (byte)CustomRPC.PlagueDoctorSetInfected:
                        RPCProcedure.plagueDoctorInfected(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.PlagueDoctorUpdateProgress:
                        byte progressTarget = reader.ReadByte();
                        byte[] progressByte = reader.ReadBytes(4);
                        float progress = System.BitConverter.ToSingle(progressByte, 0);
                        RPCProcedure.plagueDoctorProgress(progressTarget, progress);
                        break;
                    case (byte)CustomRPC.NekoKabochaExile:
                        RPCProcedure.nekoKabochaExile(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SerialKillerSuicide:
                        RPCProcedure.serialKillerSuicide(reader.ReadByte());
                        break;
                    case (byte)CustomRPC.SwapperAnimate:
                        RPCProcedure.swapperAnimate();
                        break;
                }
            }
        }
    }
}