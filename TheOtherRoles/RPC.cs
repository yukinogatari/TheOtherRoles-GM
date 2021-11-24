using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using TheOtherRoles.Roles;
using UnityEngine;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.HudManagerStartPatch;
using static TheOtherRoles.MapOptions;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles
{
    enum CustomRPC
    {
        // Main Controls

        ResetVaribles = 100,
        ShareOptionSelection,
        ForceEnd,
        SetRole,
        VersionHandshake,
        UseUncheckedVent,
        UncheckedMurderPlayer,
        UncheckedCmdReportDeadBody,

        // Role functionality

        EngineerFixLights = 131,
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
        VampireTryKill,
        PlaceGarlic,
        JackalKill,
        SidekickKill,
        JackalCreatesSidekick,
        SidekickPromotes,
        ErasePlayerRoles,
        SetFutureErased,
        SetFutureShifted,
        SetFutureShielded,
        PlaceJackInTheBox,
        LightsOut,
        WarlockCurseKill,
        PlaceCamera,
        SealVent,
        ArsonistWin,
        GuesserShoot,
        GMKill,
        GMRevive,
        UseAdminTime,
        UseCameraTime,
        UseVitalsTime,
        VultureWin,
        
        // role/modifier related
        SetRoleModifier = 200,
        RemoveRoleModifier,
        SetLoversModifier,
    }

    public static class RPCProcedure {

        // Main Controls

        public static void resetVariables() {
            Garlic.clearGarlics();
            JackInTheBox.clearJackInTheBoxes();
            clearAndReloadMapOptions();
            clearAndReloadRoles();
            clearGameHistory();
            setCustomButtonCooldowns();
            MorphData.resetMorphData();
            AdminPatch.ResetData();
            CameraPatch.ResetData();
            VitalsPatch.ResetData();
            MapBehaviorPatch.resetIcons();
            CustomRoleManager.Reset();
        }

        public static void shareOptionSelection(uint id, uint selection) {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.id == (int)id);
            option?.updateSelection((int)selection);
        }

        public static void forceEnd() {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.role()?.IsImpostor == true)
                {
                    player.RemoveInfected();
                    player.MurderPlayer(player);
                    player.Data.IsDead = true;
                }
            }
        }


        public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId) {
            System.Version ver;
            if (revision < 0) 
                ver = new System.Version(major, minor, build);
            else 
                ver = new System.Version(major, minor, build, revision);

            GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static void useUncheckedVent(int ventId, byte playerId, byte isEnter) {
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

        public static void uncheckedMurderPlayer(byte sourceId, byte targetId) {
            PlayerControl source = Helpers.playerById(sourceId);
            PlayerControl target = Helpers.playerById(targetId);
            if (source != null && target != null) source.MurderPlayer(target);
        }

        public static void uncheckedCmdReportDeadBody(byte sourceId, byte targetId) {
            PlayerControl source = Helpers.playerById(sourceId);
            PlayerControl target = Helpers.playerById(targetId);
            if (source != null && target != null) source.ReportDeadBody(target.Data);
        }

        // Role functionality
        public static void sheriffKill(byte sheriffId, byte targetId, bool misfire)
        {
            PlayerControl sheriff = Helpers.playerById(sheriffId);
            PlayerControl target  = Helpers.playerById(targetId);
            if (misfire)
            {
                sheriff.MurderPlayer(sheriff);
                misfiredPlayers.Add(sheriffId);

                if (!Sheriff.misfireKillsTarget) return;
                misfiredPlayers.Add(targetId);
            }

            sheriff.MurderPlayer(target);
        }

        public static void shieldedMurderAttempt(byte killerId, byte targetId)
        {
            PlayerControl killer = Helpers.playerById(killerId);
            PlayerControl target = Helpers.playerById(targetId);

            if (PlayerControl.LocalPlayer == target && target.hasModifier(RoleModifierTypes.MedicShield) && Medic.showAttemptToShielded)
            {
                MedicShield shield = (MedicShield)target.getModifier(RoleModifierTypes.MedicShield);
                if (!shield.medic.Data.IsDead)
                {
                    DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
                    DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
                    {
                        var renderer = DestroyableSingleton<HudManager>.Instance.FullScreen;
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

            // TODO: IMPLEMENT
            /*            if (Medic.shielded != null && Medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded && DestroyableSingleton<HudManager>.Instance?.FullScreen != null)
                        {
                            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
                            DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
                            {
                                var renderer = DestroyableSingleton<HudManager>.Instance.FullScreen;
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
                        }*/
        }

        public static void timeMasterRewindTime(byte playerId)
        {
            var timeMaster = Helpers.playerById(playerId);
            if (timeMaster.isRole(CustomRoleTypes.TimeMaster))
            {
                var role = timeMaster.role<TimeMaster>();
                role.shieldActive = false; // Shield is no longer active when rewinding
                if (timeMaster == PlayerControl.LocalPlayer)
                {
                    role.resetTimeMasterButton();
                }
                DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
                DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) =>
                {
                    if (p == 1f) DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
                })));

                if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.TimeMaster)) return; // Time Master himself does not rewind
                if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM)) return; // GM does not rewind

                TimeMaster.isRewinding = true;

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
                if (Minigame.Instance)
                    Minigame.Instance.ForceClose();
                PlayerControl.LocalPlayer.moveable = false;
            }
        }

        public static void timeMasterShield(byte playerId)
        {
            var timeMaster = Helpers.playerById(playerId);
            if (timeMaster.isRole(CustomRoleTypes.TimeMaster))
            {
                var role = timeMaster.role<TimeMaster>();
                role.shieldActive = true;
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration, new Action<float>((p) =>
                {
                    if (p == 1f) role.shieldActive = false;
                })));
            }
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

        public static void vampireSetBitten(byte playerId, byte targetId, byte reset)
        {
            PlayerControl vamp = Helpers.playerById(playerId);
            if (!vamp.isRole(CustomRoleTypes.Vampire)) return;

            Vampire role = vamp.role<Vampire>();
            if (reset != 0)
            {
                role.bitten = null;
                return;
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == targetId && !player.Data.IsDead)
                {
                    Helpers.log($"vampire {playerId} bit {targetId}");
                    role.bitten = player;
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

        /*
                public static void engineerFixLights()
                {
                    SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
                }

                public static void engineerUsedRepair()
                {
                    Engineer.usedRepair = true;
                }

                public static void medicSetShielded(byte shieldedId)
                {
                    Medic.usedShield = true;
                    Medic.shielded = Helpers.playerById(shieldedId);
                    Medic.futureShielded = null;
                }

                public static void shifterShift(byte targetId)
                {
                    // TODO: SHIFTER IS GOING TO HAVE TO BE REWRITTEN ENTIRELY
                    *//*PlayerControl oldShifter = Shifter.shifter;
                    PlayerControl player = Helpers.playerById(targetId);
                    if (player == null || oldShifter == null) return;

                    Shifter.futureShift = null;
                    Shifter.clearAndReload();

                    if (player.isRole(CustomRoleTypes.GM))
                    {
                        return;
                    }

                    // Suicide (exile) when impostor or impostor variants
                    if (player.Data.Role.IsImpostor || player.isNeutral() || player == Madmate.madmate) {
                        oldShifter.Exiled();
                        return;
                    }

                    if (Shifter.shiftModifiers) {
                        // Switch shield
                        if (Medic.shielded != null && Medic.shielded == player) {
                            Medic.shielded = oldShifter;
                        } else if (Medic.shielded != null && Medic.shielded == oldShifter) {
                            Medic.shielded = player;
                        }
                        // Shift Lovers Role
                        if (Lovers.lover1 != null && oldShifter == Lovers.lover1) Lovers.lover1 = player;
                        else if (Lovers.lover1 != null && player == Lovers.lover1) Lovers.lover1 = oldShifter;

                        if (Lovers.lover2 != null && oldShifter == Lovers.lover2) Lovers.lover2 = player;
                        else if (Lovers.lover2 != null && player == Lovers.lover2) Lovers.lover2 = oldShifter;
                    }

                    // Shift role
                    if (Mayor.mayor != null && Mayor.mayor == player)
                        Mayor.mayor = oldShifter;
                    if (Engineer.engineer != null && Engineer.engineer == player)
                        Engineer.engineer = oldShifter;
                    if (Sheriff.sheriff != null && Sheriff.sheriff == player)
                        Sheriff.sheriff = oldShifter;
                    if (Lighter.lighter != null && Lighter.lighter == player)
                        Lighter.lighter = oldShifter;
                    if (Detective.detective != null && Detective.detective == player)
                        Detective.detective = oldShifter;
                    if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player)
                        TimeMaster.timeMaster = oldShifter;
                    if (Medic.medic != null && Medic.medic == player)
                        Medic.medic = oldShifter;
                    if (Swapper.swapper != null && Swapper.swapper == player)
                        Swapper.swapper = oldShifter;
                    if (Seer.seer != null && Seer.seer == player)
                        Seer.seer = oldShifter;
                    if (Hacker.hacker != null && Hacker.hacker == player)
                        Hacker.hacker = oldShifter;
                    if (Mini.mini != null && Mini.mini == player)
                        Mini.mini = oldShifter;
                    if (Tracker.tracker != null && Tracker.tracker == player)
                        Tracker.tracker = oldShifter;
                    if (Snitch.snitch != null && Snitch.snitch == player)
                        Snitch.snitch = oldShifter;
                    if (Spy.spy != null && Spy.spy == player)
                        Spy.spy = oldShifter;
                    if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player)
                        SecurityGuard.securityGuard = oldShifter;
                    if (Guesser.guesser != null && Guesser.guesser == player)
                        Guesser.guesser = oldShifter;
                    if (Bait.bait != null && Bait.bait == player) {
                        Bait.bait = oldShifter;
                        if (Bait.bait.Data.IsDead) Bait.reported = true;
                    }
                    if (Medium.medium != null && Medium.medium == player)
                        Medium.medium = oldShifter;

                    // Set cooldowns to max for both players
                    if (PlayerControl.LocalPlayer == oldShifter || PlayerControl.LocalPlayer == player)
                        CustomButton.ResetAllCooldowns();*//*
                }

                public static void swapperSwap(byte playerId1, byte playerId2) {
                    if (MeetingHud.Instance) {
                        Swapper.playerId1 = playerId1;
                        Swapper.playerId2 = playerId2;
                    }
                }

                public static void morphlingMorph(byte playerId) {  
                    PlayerControl target = Helpers.playerById(playerId);
                    if (Morphling.morphling == null || target == null) return;
                    Morphling.startMorph(target);
                }

                public static void camouflagerCamouflage() {
                    if (Camouflager.camouflager == null) return;
                    Camouflager.startCamouflage();
                }

                public static void trackerUsedTracker(byte targetId) {
                    Tracker.usedTracker = true;
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        if (player.PlayerId == targetId)
                            Tracker.tracked = player;
                }

                public static void jackalKill(byte targetId) {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == targetId)
                        {
                            Jackal.jackal.MurderPlayer(player);
                            return;
                        }
                    }
                }

                public static void sidekickKill(byte targetId) {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == targetId)
                        {
                            Sidekick.sidekick.MurderPlayer(player);
                            return;
                        }
                    }
                }

                public static void jackalCreatesSidekick(byte targetId) {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == targetId)
                        {
                            if (!Jackal.canCreateSidekickFromImpostor && player.Data.Role.IsImpostor) {
                                Jackal.fakeSidekick = player;
                            } else {
                                player.RemoveInfected();
                                erasePlayerRoles(player.PlayerId, true);
                                Sidekick.sidekick = player;
                            }
                            Jackal.canCreateSidekick = false;
                            return;
                        }
                    }
                }

                public static void sidekickPromotes() {
                    Jackal.removeCurrentJackal();
                    Jackal.jackal = Sidekick.sidekick;
                    Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
                    Sidekick.clearAndReload();
                    return;
                }

                // TODO: REWRITE THIS
                public static void erasePlayerRoles(byte playerId, bool ignoreLovers = false) {
                    *//*
                    PlayerControl player = Helpers.playerById(playerId);
                    if (player == null) return;

                    // Don't give a former neutral role tasks because that destroys the balance.
                    if (player.isNeutral())
                        player.clearAllTasks();

                    // Crewmate roles
                    if (player.isRole(CustomRoleTypes.Mayor)) Mayor.clearAndReload();
                    if (player == Engineer.engineer) Engineer.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Sheriff)) Sheriff.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Lighter)) Lighter.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Detective)) Detective.clearAndReload();
                    if (player.isRole(CustomRoleTypes.TimeMaster)) TimeMaster.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Medic)) Medic.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Shifter)) Shifter.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Seer)) Seer.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Hacker)) Hacker.clearAndReload();
                    if (player == Mini.mini) Mini.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Tracker)) Tracker.clearAndReload();
                    if (player == Snitch.snitch) Snitch.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Swapper)) Swapper.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Spy)) Spy.clearAndReload();
                    if (player == SecurityGuard.securityGuard) SecurityGuard.clearAndReload();
                    if (player == Bait.bait) Bait.clearAndReload();
                    if (player == Madmate.madmate) Madmate.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Opportunist)) Opportunist.clearAndReload();
                    if (player == Medium.medium) Medium.clearAndReload();

                    // Impostor roles
                    if (player == Morphling.morphling) Morphling.clearAndReload();
                    if (player == Camouflager.camouflager) Camouflager.clearAndReload();
                    if (player == Godfather.godfather) Godfather.clearAndReload();
                    if (player == Mafioso.mafioso) Mafioso.clearAndReload();
                    if (player == Janitor.janitor) Janitor.clearAndReload();
                    if (player == Vampire.vampire) Vampire.clearAndReload();
                    if (player == Eraser.eraser) Eraser.clearAndReload();
                    if (player == Trickster.trickster) Trickster.clearAndReload();
                    if (player == Cleaner.cleaner) Cleaner.clearAndReload();
                    if (player == Warlock.warlock) Warlock.clearAndReload();

                    // Other roles
                    if (player.isRole(CustomRoleTypes.Jester)) Jester.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Arsonist)) Arsonist.clearAndReload();
                    if (player == Guesser.guesser) Guesser.clearAndReload();
                    if (!ignoreLovers && (player == Lovers.lover1 || player == Lovers.lover2)) { // The whole Lover couple is being erased
                        Lovers.clearAndReload(); 
                    }
                    if (player.isRole(CustomRoleTypes.Jackal)) { // Promote Sidekick and hence override the the Jackal or erase Jackal
                        if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead) {
                            RPCProcedure.sidekickPromotes();
                        } else {
                            Jackal.clearAndReload();
                        }
                    }
                    if (player == Sidekick.sidekick) Sidekick.clearAndReload();
                    if (player == BountyHunter.bountyHunter) BountyHunter.clearAndReload();
                    if (player.isRole(CustomRoleTypes.Vulture)) Vulture.clearAndReload();
                    *//*
                }

                public static void setFutureErased(byte playerId) {
                    PlayerControl player = Helpers.playerById(playerId);
                    if (Eraser.futureErased == null) 
                        Eraser.futureErased = new List<PlayerControl>();
                    if (player != null) {
                        Eraser.futureErased.Add(player);
                    }
                }

                public static void setFutureShifted(byte playerId) {
                    Shifter.futureShift = Helpers.playerById(playerId);
                }

                public static void setFutureShielded(byte playerId) {
                    Medic.futureShielded = Helpers.playerById(playerId);
                    Medic.usedShield = true;
                }

                public static void placeJackInTheBox(byte[] buff) {
                    Vector3 position = Vector3.zero;
                    position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
                    position.y = BitConverter.ToSingle(buff, 1*sizeof(float));
                    new JackInTheBox(position);
                }

                public static void lightsOut() {
                    Trickster.lightsOutTimer = Trickster.lightsOutDuration;
                    // If the local player is impostor indicate lights out
                    if(CustomRoleManager.getRole(PlayerControl.LocalPlayer).IsImpostor) {
                        new CustomMessage("Lights are out", Trickster.lightsOutDuration);
                    }
                }

                public static void warlockCurseKill(byte targetId) {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                        if (player.PlayerId == targetId) {
                            Warlock.curseKillTarget = player;
                            Warlock.warlock.MurderPlayer(player);
                            return;
                        }
                    }
                }

                public static void placeCamera(byte[] buff, byte roomId) {
                    var referenceCamera = UnityEngine.Object.FindObjectOfType<SurvCamera>(); 
                    if (referenceCamera == null) return; // Mira HQ

                    SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
                    SecurityGuard.placedCameras++;

                    Vector3 position = Vector3.zero;
                    position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
                    position.y = BitConverter.ToSingle(buff, 1*sizeof(float));

                    SystemTypes roomType = (SystemTypes)roomId;

                    var camera = UnityEngine.Object.Instantiate<SurvCamera>(referenceCamera);
                    camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
                    camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
                    camera.Offset = new Vector3(0f, 0f, camera.Offset.z);

                    switch(roomType)
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

                    if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard) {
                        camera.gameObject.SetActive(true);
                        camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    } else {
                        camera.gameObject.SetActive(false);
                    }
                    MapOptions.camerasToAdd.Add(camera);
                }

                public static void sealVent(int ventId) {
                    Vent vent = ShipStatus.Instance.AllVents.FirstOrDefault((x) => x != null && x.Id == ventId);
                    if (vent == null) return;

                    SecurityGuard.remainingScrews -= SecurityGuard.ventPrice;
                    if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard) {
                        PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>(); 
                        animator?.Stop();
                        vent.EnterVentAnim = vent.ExitVentAnim = null;
                        vent.myRend.sprite = animator == null ? SecurityGuard.getStaticVentSealedSprite() : SecurityGuard.getAnimatedVentSealedSprite();
                        vent.myRend.color = new Color(1f, 1f, 1f, 0.5f);
                        vent.name = "FutureSealedVent_" + vent.name;
                    }

                    MapOptions.ventsToSeal.Add(vent);
                }

                public static void guesserShoot(byte playerId) {
                    PlayerControl target = Helpers.playerById(playerId);
                    if (target == null) return;
                    target.Exiled();
                    PlayerControl partner = target.getPartner(); // Lover check
                    byte partnerId = partner != null ? partner.PlayerId : playerId;
                    Guesser.remainingShots = Mathf.Max(0, Guesser.remainingShots - 1);
                    if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);
                    if (MeetingHud.Instance) {
                        foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates) {
                            if (pva.TargetPlayerId == playerId || pva.TargetPlayerId == partnerId) {
                                pva.SetDead(pva.DidReport, true);
                                pva.Overlay.gameObject.SetActive(true);
                            }
                        }
                        if (AmongUsClient.Instance.AmHost) 
                            MeetingHud.Instance.CheckForEndVoting();
                    }
                    if (DestroyableSingleton<HudManager>.Instance != null && Guesser.guesser != null)
                        if (PlayerControl.LocalPlayer == target) 
                            DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(Guesser.guesser.Data, target.Data);
                        else if (partner != null && PlayerControl.LocalPlayer == partner) 
                            DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(partner.Data, partner.Data);
                }
        */

        public static void arsonistWin(byte playerId)
        {
            Arsonist.triggerArsonistWin = true;
            Arsonist.winner = Helpers.playerById(playerId);
        }

        public static void vultureWin(byte playerId)
        {
            Vulture.triggerVultureWin = true;
            Vulture.winner = Helpers.playerById(playerId);
        }

        public static void GMKill(byte targetId)
        {
            PlayerControl target = Helpers.playerById(targetId);

            if (target == null) return;
            target.MyPhysics.ExitAllVents();
            target.Exiled();

            PlayerControl partner = target.getPartner(); // Lover check
            partner?.MyPhysics.ExitAllVents();

            PlayerControl gm = RoleHelpers.getPlayersWithRole(CustomRoleTypes.GM).FirstOrDefault();
            if (DestroyableSingleton<HudManager>.Instance != null)
            {
                if (PlayerControl.LocalPlayer == target)
                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(gm.Data, target.Data);
                else if (partner != null && PlayerControl.LocalPlayer == partner)
                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(gm.Data, partner.Data);
            }
        }

        public static void GMRevive(byte targetId)
        {
            PlayerControl target = Helpers.playerById(targetId);
            if (target == null) return;
            target.Revive();
            target.getPartner()?.Revive(); // Lover check

            if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM))
            {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
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

        internal static void setRoleModifier(byte playerId, int roleMod)
        {
            Helpers.playerById(playerId).addModifier((RoleModifierTypes)roleMod);
        }

        internal static void removeRoleModifier(byte playerId, int roleMod)
        {
            Helpers.playerById(playerId).removeModifier((RoleModifierTypes)roleMod);
        }
    }   

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class RPCHandlerPatch
    {
        static void Postfix([HarmonyArgument(0)]byte callId, [HarmonyArgument(1)]MessageReader reader)
        {
            byte packetId = callId;
            switch (packetId) {

                case (byte)CustomRPC.ResetVaribles:
                    RPCProcedure.resetVariables();
                    break;
                case (byte)CustomRPC.ShareOptionSelection:
                    uint id = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    RPCProcedure.shareOptionSelection(id, selection);
                    break;
                case (byte)CustomRPC.ForceEnd:
                    RPCProcedure.forceEnd();
                    break;
                case (byte)CustomRPC.SetRoleModifier:
                    RPCProcedure.setRoleModifier(reader.ReadByte(), reader.ReadInt32());
                    break;
                case (byte)CustomRPC.RemoveRoleModifier:
                    RPCProcedure.removeRoleModifier(reader.ReadByte(), reader.ReadInt32());
                    break;
                case (byte)CustomRPC.SetLoversModifier:
                    // TODO: IMPLEMENT THIS
                    break;

                case (byte)CustomRPC.VersionHandshake:
                    byte major = reader.ReadByte();
                    byte minor = reader.ReadByte();
                    byte patch = reader.ReadByte();
                    int versionOwnerId = reader.ReadPackedInt32();
                    byte revision = 0xFF;
                    Guid guid;
                    if (reader.Length - reader.Position >= 17) { // enough bytes left to read
                        revision = reader.ReadByte();
                        // GUID
                        byte[] gbytes = reader.ReadBytes(16);
                        guid = new Guid(gbytes);
                    } else {
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
                    RPCProcedure.uncheckedMurderPlayer(source, target);
                    break;
                case (byte)CustomRPC.UncheckedCmdReportDeadBody:
                    byte reportSource = reader.ReadByte();
                    byte reportTarget = reader.ReadByte();
                    RPCProcedure.uncheckedCmdReportDeadBody(reportSource, reportTarget);
                    break;

                // Role functionality
                case (byte)CustomRPC.CleanBody:
                    RPCProcedure.cleanBody(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SheriffKill:
                    RPCProcedure.sheriffKill(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.ShieldedMurderAttempt:
                    RPCProcedure.shieldedMurderAttempt(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.TimeMasterRewindTime:
                    RPCProcedure.timeMasterRewindTime(reader.ReadByte());
                    break;
                case (byte)CustomRPC.TimeMasterShield:
                    RPCProcedure.timeMasterShield(reader.ReadByte());
                    break;
                case (byte)CustomRPC.VampireSetBitten:
                    byte playerId = reader.ReadByte();
                    byte bittenId = reader.ReadByte();
                    byte reset = reader.ReadByte();
                    RPCProcedure.vampireSetBitten(playerId, bittenId, reset);
                    break;
                case (byte)CustomRPC.PlaceGarlic:
                    RPCProcedure.placeGarlic(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.ArsonistWin:
                    RPCProcedure.arsonistWin(reader.ReadByte());
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
                case (byte)CustomRPC.VultureWin:
                    RPCProcedure.vultureWin(reader.ReadByte());
                    break;
                    /*
                                    case (byte)CustomRPC.EngineerUsedRepair:
                                        RPCProcedure.engineerUsedRepair();
                                        break;
                                    case (byte)CustomRPC.MedicSetShielded:
                                        RPCProcedure.medicSetShielded(reader.ReadByte());
                                        break;
                                    case (byte)CustomRPC.ShifterShift:
                                        RPCProcedure.shifterShift(reader.ReadByte());
                                        break;
                                    case (byte)CustomRPC.SwapperSwap:
                                        byte playerId1 = reader.ReadByte();
                                        byte playerId2 = reader.ReadByte();
                                        RPCProcedure.swapperSwap(playerId1, playerId2);
                                        break;
                                    case (byte)CustomRPC.CamouflagerCamouflage:
                                        RPCProcedure.camouflagerCamouflage();
                                        break;
                                    case (byte)CustomRPC.VampireTryKill:
                                        RPCProcedure.vampireTryKill();
                                        break;
                                    case (byte)CustomRPC.TrackerUsedTracker:
                                        RPCProcedure.trackerUsedTracker(reader.ReadByte());
                                        break;
                                    case (byte)CustomRPC.JackalKill:
                                        RPCProcedure.jackalKill(reader.ReadByte());
                                        break;
                                    case (byte)CustomRPC.SidekickKill:
                                        RPCProcedure.sidekickKill(reader.ReadByte());
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
                                    case (byte)CustomRPC.WarlockCurseKill:
                                        RPCProcedure.warlockCurseKill(reader.ReadByte());
                                        break;
                                    case (byte)CustomRPC.PlaceCamera:
                                        RPCProcedure.placeCamera(reader.ReadBytesAndSize(), reader.ReadByte());
                                        break;
                                    case (byte)CustomRPC.SealVent:
                                        RPCProcedure.sealVent(reader.ReadPackedInt32());
                                        break;
                                    case (byte)CustomRPC.GuesserShoot:
                                        RPCProcedure.guesserShoot(reader.ReadByte());
                                        break;*/
            }
        }
    }
}
