using HarmonyLib;
using Hazel;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using TheOtherRoles.Roles;

namespace TheOtherRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    static class HudManagerStartPatch
    {
        private static CustomButton garlicButton;
        public static bool localPlacedGarlic = false;

        public static void setCustomButtonCooldowns()
        {
            //garlicButton.MaxTimer = 0f;
        }

        public static void Postfix(HudManager __instance)
        {
            localPlacedGarlic = false;
            /*garlicButton = new CustomButton(
                () =>
                {
                    localPlacedGarlic = true;
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceGarlic, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeGarlic(buff);
                },
                () => { return !localPlacedGarlic && !PlayerControl.LocalPlayer.Data.IsDead && Vampire.garlicsActive && !PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM); },
                () => { return PlayerControl.LocalPlayer.CanMove && !localPlacedGarlic; },
                () => { localPlacedGarlic = false; },
                Vampire.getGarlicButtonSprite(),
                Vector3.zero,
                DestroyableSingleton<HudManager>.Instance.UseButton,
                null,
                null,
                true
            );
            garlicButton.Timer = 0f;
            garlicButton.MaxTimer = 0f;
            garlicButton.buttonText = ModTranslation.getString("GarlicText");

            // Set the default (or settings from the previous game) timers/durations when spawning the buttons
            setCustomButtonCooldowns();*/
        }
    }
}
/*private static CustomButton engineerRepairButton;
private static CustomButton janitorCleanButton;
private static CustomButton sheriffKillButton;
private static CustomButton timeMasterShieldButton;
private static CustomButton medicShieldButton;
private static CustomButton shifterShiftButton;
private static CustomButton morphlingButton;
private static CustomButton camouflagerButton;
private static CustomButton hackerButton;
private static CustomButton trackerButton;
private static CustomButton vampireKillButton;
private static CustomButton garlicButton;
private static CustomButton jackalKillButton;
private static CustomButton sidekickKillButton;
private static CustomButton jackalSidekickButton;
private static CustomButton lighterButton;
private static CustomButton eraserButton;
private static CustomButton placeJackInTheBoxButton;        
private static CustomButton lightsOutButton;
private static List<CustomButton> gmButtons;
private static List<CustomButton> gmKillButtons;
private static CustomButton gmZoomIn;
private static CustomButton gmZoomOut;
//public static CustomButton showInfoOverlay;
public static CustomButton cleanerCleanButton;
public static CustomButton warlockCurseButton;
public static CustomButton securityGuardButton;
public static CustomButton arsonistButton;
public static CustomButton arsonistIgniteButton;
public static CustomButton vultureEatButton;
public static CustomButton mediumButton;
public static TMPro.TMP_Text securityGuardButtonScrewsText;
public static TMPro.TMP_Text sheriffNumShotsText;
public static TMPro.TMP_Text vultureNumCorpsesText;

public static void setCustomButtonCooldowns() {
    engineerRepairButton.MaxTimer = 0f;
    janitorCleanButton.MaxTimer = Janitor.cooldown;
    sheriffKillButton.MaxTimer = Sheriff.cooldown;
    timeMasterShieldButton.MaxTimer = TimeMaster.cooldown;
    medicShieldButton.MaxTimer = 0f;
    shifterShiftButton.MaxTimer = 0f;
    morphlingButton.MaxTimer = Morphling.cooldown;
    camouflagerButton.MaxTimer = Camouflager.cooldown;
    hackerButton.MaxTimer = Hacker.cooldown;
    vampireKillButton.MaxTimer = Vampire.cooldown;
    trackerButton.MaxTimer = 0f;
    garlicButton.MaxTimer = 0f;
    jackalKillButton.MaxTimer = Jackal.cooldown;
    sidekickKillButton.MaxTimer = Sidekick.cooldown;
    jackalSidekickButton.MaxTimer = Jackal.createSidekickCooldown;
    lighterButton.MaxTimer = Lighter.cooldown;
    eraserButton.MaxTimer = Eraser.cooldown;
    placeJackInTheBoxButton.MaxTimer = Trickster.placeBoxCooldown;
    lightsOutButton.MaxTimer = Trickster.lightsOutCooldown;
    cleanerCleanButton.MaxTimer = Cleaner.cooldown;
    warlockCurseButton.MaxTimer = Warlock.cooldown;
    securityGuardButton.MaxTimer = SecurityGuard.cooldown;
    arsonistButton.MaxTimer = Arsonist.cooldown;
    vultureEatButton.MaxTimer = Vulture.cooldown;
    mediumButton.MaxTimer = Medium.cooldown;

    timeMasterShieldButton.EffectDuration = TimeMaster.shieldDuration;
    hackerButton.EffectDuration = Hacker.duration;
    vampireKillButton.EffectDuration = Vampire.delay;
    lighterButton.EffectDuration = Lighter.duration; 
    camouflagerButton.EffectDuration = Camouflager.duration;
    morphlingButton.EffectDuration = Morphling.duration;
    lightsOutButton.EffectDuration = Trickster.lightsOutDuration;
    arsonistButton.EffectDuration = Arsonist.duration;
    mediumButton.EffectDuration = Medium.duration;

    // Already set the timer to the max, as the button is enabled during the game and not available at the start
    lightsOutButton.Timer = lightsOutButton.MaxTimer;

    foreach (CustomButton gmButton in gmButtons)
    {
        gmButton.MaxTimer = 0.0f;
    }
    foreach (CustomButton gmButton in gmKillButtons)
    {
        gmButton.MaxTimer = 0.0f;
    }

    gmZoomIn.MaxTimer = 0.0f;
    gmZoomOut.MaxTimer = 0.0f;

    arsonistIgniteButton.MaxTimer = 0f;
    arsonistIgniteButton.Timer = 0f;
}

public static void resetTimeMasterButton() {
    timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
    timeMasterShieldButton.isEffectActive = false;
    timeMasterShieldButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
}

public static void Postfix(HudManager __instance)
{
    // Engineer Repair
    engineerRepairButton = new CustomButton(
        () => {
            engineerRepairButton.Timer = 0f;

            MessageWriter usedRepairWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EngineerUsedRepair, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(usedRepairWriter);
            RPCProcedure.engineerUsedRepair();

            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks) {
                if (task.TaskType == TaskTypes.FixLights) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EngineerFixLights, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.engineerFixLights();
                } else if (task.TaskType == TaskTypes.RestoreOxy) {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                } else if (task.TaskType == TaskTypes.ResetReactor) {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                } else if (task.TaskType == TaskTypes.ResetSeismic) {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
                } else if (task.TaskType == TaskTypes.FixComms) {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                } else if (task.TaskType == TaskTypes.StopCharles) {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
                }
            }
        },
        () => { return Engineer.engineer != null && Engineer.engineer == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => {
            bool sabotageActive = false;
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles)
                    sabotageActive = true;
            return sabotageActive && !Engineer.usedRepair && PlayerControl.LocalPlayer.CanMove;
        },
        () => {},
        Engineer.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );

    // Janitor Clean
    janitorCleanButton = new CustomButton(
        () => {
            foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask)) {
                if (collider2D.tag == "DeadBody")
                {
                    DeadBody component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                        Vector2 truePosition2 = component.TruePosition;
                        if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                        {
                            GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                            writer.Write(playerInfo.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.cleanBody(playerInfo.PlayerId);
                            janitorCleanButton.Timer = janitorCleanButton.MaxTimer;

                            break;
                        }
                    }
                }
            }
        },
        () => { return Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor  && PlayerControl.LocalPlayer.CanMove; },
        () => { janitorCleanButton.Timer = janitorCleanButton.MaxTimer; },
        Janitor.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );


    // Time Master Rewind Time
    timeMasterShieldButton = new CustomButton(
        () => {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TimeMasterShield, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.timeMasterShield();
        },
        () => { return TimeMaster.timeMaster != null && TimeMaster.timeMaster == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return PlayerControl.LocalPlayer.CanMove; },
        () => {
            timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
            timeMasterShieldButton.isEffectActive = false;
            timeMasterShieldButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
        },
        TimeMaster.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q, 
        true,
        TimeMaster.shieldDuration,
        () => { timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer; }
    );

    // Medic Shield
    medicShieldButton = new CustomButton(
        () => {
            medicShieldButton.Timer = 0f;

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, Medic.setShieldAfterMeeting ? (byte)CustomRPC.SetFutureShielded : (byte)CustomRPC.MedicSetShielded, Hazel.SendOption.Reliable, -1);
            writer.Write(Medic.currentTarget.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            if (Medic.setShieldAfterMeeting)
                RPCProcedure.setFutureShielded(Medic.currentTarget.PlayerId);
            else
                RPCProcedure.medicSetShielded(Medic.currentTarget.PlayerId);
        },
        () => { return Medic.medic != null && Medic.medic == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return !Medic.usedShield && Medic.currentTarget && PlayerControl.LocalPlayer.CanMove; },
        () => {},
        Medic.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );


    // Shifter shift
    shifterShiftButton = new CustomButton(
        () => {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFutureShifted, Hazel.SendOption.Reliable, -1);
            writer.Write(Shifter.currentTarget.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
        },
        () => { return Shifter.shifter != null && Shifter.shifter == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return Shifter.currentTarget && Shifter.futureShift == null && PlayerControl.LocalPlayer.CanMove; },
        () => { },
        Shifter.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );

    // Morphling morph
    morphlingButton = new CustomButton(
        () => {
            if (Morphling.sampledTarget != null) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MorphlingMorph, Hazel.SendOption.Reliable, -1);
                writer.Write(Morphling.sampledTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.morphlingMorph(Morphling.sampledTarget.PlayerId);
                Morphling.sampledTarget = null;
                morphlingButton.EffectDuration = Morphling.duration;
            } else if (Morphling.currentTarget != null) {
                Morphling.sampledTarget = Morphling.currentTarget;
                morphlingButton.Sprite = Morphling.getMorphSprite();
                morphlingButton.EffectDuration = 1f;
            }
        },
        () => { return Morphling.morphling != null && Morphling.morphling == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return (Morphling.currentTarget || Morphling.sampledTarget) && PlayerControl.LocalPlayer.CanMove; },
        () => { 
            morphlingButton.Timer = morphlingButton.MaxTimer;
            morphlingButton.Sprite = Morphling.getSampleSprite();
            morphlingButton.isEffectActive = false;
            morphlingButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
            Morphling.sampledTarget = null;
        },
        Morphling.getSampleSprite(),
         new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F,
        true,
        Morphling.duration,
        () => {
            if (Morphling.sampledTarget == null) {
                morphlingButton.Timer = morphlingButton.MaxTimer;
                morphlingButton.Sprite = Morphling.getSampleSprite();
            }
        }
    );

    // Camouflager camouflage
    camouflagerButton = new CustomButton(
        () => {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CamouflagerCamouflage, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.camouflagerCamouflage();
        },
        () => { return Camouflager.camouflager != null && Camouflager.camouflager == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return PlayerControl.LocalPlayer.CanMove; },
        () => {
            camouflagerButton.Timer = camouflagerButton.MaxTimer;
            camouflagerButton.isEffectActive = false;
            camouflagerButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
        },
        Camouflager.getButtonSprite(),
         new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F,
        true,
        Camouflager.duration,
        () => { camouflagerButton.Timer = camouflagerButton.MaxTimer; }
    );

    // Hacker button
    hackerButton = new CustomButton(
        () => {
            Hacker.hackerTimer = Hacker.duration;
        },
        () => { return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return PlayerControl.LocalPlayer.CanMove; },
        () => {
            hackerButton.Timer = hackerButton.MaxTimer;
            hackerButton.isEffectActive = false;
            hackerButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
        },
        Hacker.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q,
        true,
        0f,
        () => {
            hackerButton.Timer = hackerButton.MaxTimer;
        }
    );

    // Tracker button
    trackerButton = new CustomButton(
        () => {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TrackerUsedTracker, Hazel.SendOption.Reliable, -1);
            writer.Write(Tracker.currentTarget.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.trackerUsedTracker(Tracker.currentTarget.PlayerId);
        },
        () => { return Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return PlayerControl.LocalPlayer.CanMove && Tracker.currentTarget != null && !Tracker.usedTracker; },
        () => { if(Tracker.resetTargetAfterMeeting) Tracker.resetTracked(); },
        Tracker.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );

    vampireKillButton = new CustomButton(
        () => {
            if (Helpers.checkMurderAttempt(Vampire.vampire, Vampire.currentTarget)) {
                if (Vampire.targetNearGarlic) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                    writer.Write(Vampire.vampire.PlayerId);
                    writer.Write(Vampire.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.uncheckedMurderPlayer(Vampire.vampire.PlayerId, Vampire.currentTarget.PlayerId);

                    vampireKillButton.HasEffect = false; // Block effect on this click
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                } else {
                    Vampire.bitten = Vampire.currentTarget;
                    // Notify players about bitten
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                    writer.Write(Vampire.vampire.PlayerId);
                    writer.Write(Vampire.bitten.PlayerId);
                    writer.Write(0);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.vampireSetBitten(Vampire.vampire.PlayerId, Vampire.bitten.PlayerId, 0);

                    DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.delay, new Action<float>((p) => { // Delayed action
                        if (p == 1f) {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten);
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                                    writer.Write(Vampire.vampire.PlayerId);
                                    writer.Write(byte.MaxValue);
                                    writer.Write(byte.MaxValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.vampireSetBitten(Vampire.vampire.PlayerId, byte.MaxValue, byte.MaxValue);
                        }
                    })));

                    vampireKillButton.HasEffect = true; // Trigger effect on this click
                }
            } else {
                vampireKillButton.HasEffect = false; // Block effect if no action was fired
            }
        },
        () => { return Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () =>
        {
            if (Vampire.targetNearGarlic && Vampire.canKillNearGarlics)
            {
                vampireKillButton.killButton.graphic.sprite = __instance.KillButton.graphic.sprite;
                vampireKillButton.showButtonText = true;
            }
            else { 
                vampireKillButton.killButton.graphic.sprite = Vampire.getButtonSprite();
                vampireKillButton.showButtonText = false;
            }
            return Vampire.currentTarget != null && PlayerControl.LocalPlayer.CanMove && (!Vampire.targetNearGarlic || Vampire.canKillNearGarlics);
        },
        () => {
            vampireKillButton.Timer = vampireKillButton.MaxTimer;
            vampireKillButton.isEffectActive = false;
            vampireKillButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
        },
        Vampire.getButtonSprite(),
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q,
        false,
        0f,
        () => {
            vampireKillButton.Timer = vampireKillButton.MaxTimer;
        }
    );


    // Jackal Sidekick Button
    jackalSidekickButton = new CustomButton(
        () => {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.JackalCreatesSidekick, Hazel.SendOption.Reliable, -1);
            writer.Write(Jackal.currentTarget.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.jackalCreatesSidekick(Jackal.currentTarget.PlayerId);
        },
        () => { return Jackal.canCreateSidekick && Jackal.jackal != null && Jackal.jackal == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return Jackal.canCreateSidekick && Jackal.currentTarget != null && PlayerControl.LocalPlayer.CanMove; },
        () => { jackalSidekickButton.Timer = jackalSidekickButton.MaxTimer;},
        Jackal.getSidekickButtonSprite(),
        new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F
    );

    // Jackal Kill
    jackalKillButton = new CustomButton(
        () => {
            if (!Helpers.checkMurderAttemptAndKill(Jackal.jackal, Jackal.currentTarget)) return;
            jackalKillButton.Timer = jackalKillButton.MaxTimer; 
            Jackal.currentTarget = null;
        },
        () => { return Jackal.jackal != null && Jackal.jackal == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return Jackal.currentTarget && PlayerControl.LocalPlayer.CanMove; },
        () => { jackalKillButton.Timer = jackalKillButton.MaxTimer;},
        __instance.KillButton.graphic.sprite,
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );

    // Sidekick Kill
    sidekickKillButton = new CustomButton(
        () => {
            if (!Helpers.checkMurderAttemptAndKill(Sidekick.sidekick, Sidekick.currentTarget)) return;

            sidekickKillButton.Timer = sidekickKillButton.MaxTimer; 
            Sidekick.currentTarget = null;
        },
        () => { return Sidekick.canKill && Sidekick.sidekick != null && Sidekick.sidekick == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return Sidekick.currentTarget && PlayerControl.LocalPlayer.CanMove; },
        () => { sidekickKillButton.Timer = sidekickKillButton.MaxTimer;},
        __instance.KillButton.graphic.sprite,
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );


    // Eraser erase button
    eraserButton = new CustomButton(
        () => {
            eraserButton.MaxTimer += Eraser.cooldownIncrease;
            eraserButton.Timer = eraserButton.MaxTimer;

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFutureErased, Hazel.SendOption.Reliable, -1);
            writer.Write(Eraser.currentTarget.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setFutureErased(Eraser.currentTarget.PlayerId);
        },
        () => { return Eraser.eraser != null && Eraser.eraser == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return PlayerControl.LocalPlayer.CanMove && Eraser.currentTarget != null; },
        () => { eraserButton.Timer = eraserButton.MaxTimer; },
        Eraser.getButtonSprite(),
        new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F
    );

    placeJackInTheBoxButton = new CustomButton(
        () => {
            placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

            var pos = PlayerControl.LocalPlayer.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0*sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1*sizeof(float), sizeof(float));

            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceJackInTheBox, Hazel.SendOption.Reliable);
            writer.WriteBytesAndSize(buff);
            writer.EndMessage();
            RPCProcedure.placeJackInTheBox(buff); 
        },
        () => { return Trickster.trickster != null && Trickster.trickster == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !JackInTheBox.hasJackInTheBoxLimitReached(); },
        () => { return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.hasJackInTheBoxLimitReached(); },
        () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;},
        Trickster.getPlaceBoxButtonSprite(),
        new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F
    );

    lightsOutButton = new CustomButton(
        () => {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LightsOut, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.lightsOut(); 
        },
        () => { return Trickster.trickster != null && Trickster.trickster == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents; },
        () => { return PlayerControl.LocalPlayer.CanMove && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents; },
        () => { 
            lightsOutButton.Timer = lightsOutButton.MaxTimer;
            lightsOutButton.isEffectActive = false;
            lightsOutButton.killButton.cooldownTimerText.color = Palette.EnabledColor;
        },
        Trickster.getLightsOutButtonSprite(),
         new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F,
        true,
        Trickster.lightsOutDuration,
        () => { lightsOutButton.Timer = lightsOutButton.MaxTimer; }
    );
    // Cleaner Clean
    cleanerCleanButton = new CustomButton(
        () => {
            foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask)) {
                if (collider2D.tag == "DeadBody")
                {
                    DeadBody component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                        Vector2 truePosition2 = component.TruePosition;
                        if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                        {
                            GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                            writer.Write(playerInfo.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.cleanBody(playerInfo.PlayerId);

                            Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                            break;
                        }
                    }
                }
            }
        },
        () => { return Cleaner.cleaner != null && Cleaner.cleaner == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; },
        () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
        Cleaner.getButtonSprite(),
        new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F
    );

    // Warlock curse
    warlockCurseButton = new CustomButton(
        () => {
            if (Warlock.curseVictim == null) {
                // Apply Curse
                Warlock.curseVictim = Warlock.currentTarget;
                warlockCurseButton.Sprite = Warlock.getCurseKillButtonSprite();
                warlockCurseButton.Timer = 1f;
            } else if (Warlock.curseVictim != null && Warlock.curseVictimTarget != null && Helpers.checkMurderAttempt(Warlock.warlock, Warlock.curseVictimTarget)) {
                // Curse Kill
                Warlock.curseKillTarget = Warlock.curseVictimTarget;

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WarlockCurseKill, Hazel.SendOption.Reliable, -1);
                writer.Write(Warlock.curseKillTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.warlockCurseKill(Warlock.curseKillTarget.PlayerId);

                Warlock.curseVictim = null;
                Warlock.curseVictimTarget = null;
                warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                Warlock.warlock.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;

                if(Warlock.rootTime > 0) {
                    PlayerControl.LocalPlayer.moveable = false;
                    PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                    DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.rootTime, new Action<float>((p) => { // Delayed action
                        if (p == 1f) {
                            PlayerControl.LocalPlayer.moveable = true;
                        }
                    })));
                }
            }
        },
        () => { return Warlock.warlock != null && Warlock.warlock == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => { return ((Warlock.curseVictim == null && Warlock.currentTarget != null) || (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)) && PlayerControl.LocalPlayer.CanMove; },
        () => { 
            warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
            warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
            Warlock.curseVictim = null;
            Warlock.curseVictimTarget = null;
        },
        Warlock.getCurseButtonSprite(),
        new Vector3(-1.3f, 1.3f, 0f),
        __instance,
        KeyCode.F
    );

    // Security Guard button
    securityGuardButton = new CustomButton(
        () => {
            if (SecurityGuard.ventTarget != null) { // Seal vent
                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SealVent, Hazel.SendOption.Reliable);
                writer.WritePacked(SecurityGuard.ventTarget.Id);
                writer.EndMessage();
                RPCProcedure.sealVent(SecurityGuard.ventTarget.Id);
                SecurityGuard.ventTarget = null;
            } else if (PlayerControl.GameOptions.MapId != 1 && MapOptions.couldUseCameras) { // Place camera if there's no vent and it's not MiraHQ
                var pos = PlayerControl.LocalPlayer.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0*sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1*sizeof(float), sizeof(float));

                byte roomId;
                try {
                    roomId = (byte)DestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom.RoomId;
                } catch {
                    roomId = 255;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceCamera, Hazel.SendOption.Reliable);
                writer.WriteBytesAndSize(buff);
                writer.Write(roomId);
                writer.EndMessage();
                RPCProcedure.placeCamera(buff, roomId); 
            }
            securityGuardButton.Timer = securityGuardButton.MaxTimer;
        },
        () => { return SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews >= Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice); },
        () => {
            securityGuardButton.killButton.graphic.sprite = (SecurityGuard.ventTarget == null && PlayerControl.GameOptions.MapId != 1) ? SecurityGuard.getPlaceCameraButtonSprite() : SecurityGuard.getCloseVentButtonSprite(); 
            if (securityGuardButtonScrewsText != null) securityGuardButtonScrewsText.text = $"{SecurityGuard.remainingScrews}/{SecurityGuard.totalScrews}";

            if (SecurityGuard.ventTarget != null)
                return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice && PlayerControl.LocalPlayer.CanMove;
            return PlayerControl.GameOptions.MapId != 1 && MapOptions.couldUseCameras && SecurityGuard.remainingScrews >= SecurityGuard.camPrice && PlayerControl.LocalPlayer.CanMove;
        },
        () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
        SecurityGuard.getPlaceCameraButtonSprite(),
        new Vector3(-1.3f, 0f, 0f),
        __instance,
        KeyCode.Q
    );

    // Security Guard button screws counter
    securityGuardButtonScrewsText = GameObject.Instantiate(securityGuardButton.killButton.cooldownTimerText, securityGuardButton.killButton.cooldownTimerText.transform.parent);
    securityGuardButtonScrewsText.text = "";
    securityGuardButtonScrewsText.enableWordWrapping = false;
    securityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
    securityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

    // Arsonist button

    gmButtons = new List<CustomButton>();
    gmKillButtons = new List<CustomButton>();

    Vector3 gmCalcPos(byte index)
    {
        return new Vector3(-0.25f, -0.25f, 0) + Vector3.right * index * 0.55f;
    }

    Action gmButtonOnClick(byte index)
    {
        return () =>
        {
            if (!MapOptions.playerIcons.ContainsKey(index))
            {
                return;
            }

            PlayerControl target = Helpers.playerById(index);
            //TheOtherRolesPlugin.Instance.Log.LogInfo($"Clicked {index}: {target.transform.position.x}, {target.transform.position.y}");
            //TheOtherRolesPlugin.Instance.Log.LogInfo($"GM pos: {GM.gm.transform.position.x}, {GM.gm.transform.position.y}");

            if (GM.gm.transform.position != target.transform.position)
            {
                GM.gm.transform.position = target.transform.position;
            }
        };
    };

    Action gmKillButtonOnClick(byte index)
    {
        return () =>
        {
            if (!MapOptions.playerIcons.ContainsKey(index))
            {
                return;
            }

            PlayerControl target = Helpers.playerById(index);
            if (!target.Data.IsDead)
            {
                //TheOtherRolesPlugin.Instance.Log.LogInfo($"Murdered {index}");

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GMKill, Hazel.SendOption.Reliable, -1);
                writer.Write(index);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.GMKill(index);
            } else
            {
                //TheOtherRolesPlugin.Instance.Log.LogInfo($"Revived {index}");

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GMRevive, Hazel.SendOption.Reliable, -1);
                writer.Write(index);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.GMRevive(index);
            }
        };
    };

    Func<bool> gmHasButton(byte index)
    {
        return () =>
        {
            if ((GM.gm == null || PlayerControl.LocalPlayer != GM.gm) ||
                (!MapOptions.playerIcons.ContainsKey(index)) ||
                (!GM.canWarp))
            {
                return false;
            }

            return true;
        };
    }

    Func<bool> gmHasKillButton(byte index)
    {
        return () =>
        {
            if ((GM.gm == null || PlayerControl.LocalPlayer != GM.gm) ||
                (!MapOptions.playerIcons.ContainsKey(index)) ||
                (!GM.canKill))
            {
                return false;
            }

            return true;
        };
    }

    Func<bool> gmCouldUse(byte index)
    {
        return () =>
        {
            if (!MapOptions.playerIcons.ContainsKey(index) || !GM.canWarp)
            {
                return false;
            }

            Vector3 pos = gmCalcPos(index);
            Vector3 scale = new Vector3(0.4f, 0.8f, 1.0f);

            Vector3 iconBase = __instance.UseButton.transform.localPosition;
            iconBase.x *= -1;
            if (gmButtons[index].PositionOffset != pos)
            {
                gmButtons[index].PositionOffset = pos;
                gmButtons[index].LocalScale = scale;
                MapOptions.playerIcons[index].transform.localPosition = iconBase + pos;
                //TheOtherRolesPlugin.Instance.Log.LogInfo($"Updated {index}: {pos.x}, {pos.y}, {pos.z}");
            }

            //MapOptions.playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
            return PlayerControl.LocalPlayer.CanMove;
        };
    }

    Func<bool> gmCouldKill(byte index)
    {
        return () =>
        {
            if (!MapOptions.playerIcons.ContainsKey(index) || !GM.canKill)
            {
                return false;
            }

            Vector3 pos = gmCalcPos(index) + Vector3.up * 0.55f;
            Vector3 scale = new Vector3(0.4f, 0.25f, 1.0f);
            if (gmKillButtons[index].PositionOffset != pos)
            {
                gmKillButtons[index].PositionOffset = pos;
                gmKillButtons[index].LocalScale = scale;
            }

            PlayerControl target = Helpers.playerById(index);
            if (target.Data.IsDead)
            {
                gmKillButtons[index].killButton.buttonLabelText.SetText(ModTranslation.getString("gmRevive"));
            } else
            {
                gmKillButtons[index].killButton.buttonLabelText.SetText(ModTranslation.getString("gmKill"));
            }

            //MapOptions.playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
            return true;
        };
    }

    for (byte i = 0; i < 15; i++)
    {
        //TheOtherRolesPlugin.Instance.Log.LogInfo($"Added {i}");

        CustomButton gmButton = new CustomButton(
            // Action OnClick
            gmButtonOnClick(i),
            // bool HasButton
            gmHasButton(i),
            // bool CouldUse
            gmCouldUse(i),
            // Action OnMeetingEnds
            () => { },
            // sprite
            null,
            // position
            Vector3.zero,
            // hudmanager
            __instance,
            // keyboard shortcut
            null,
            true
        );
        gmButton.Timer = 0.0f;
        gmButton.MaxTimer = 0.0f;
        gmButtons.Add(gmButton);

        CustomButton gmKillButton = new CustomButton(
            // Action OnClick
            gmKillButtonOnClick(i),
            // bool HasButton
            gmHasKillButton(i),
            // bool CouldUse
            gmCouldKill(i),
            // Action OnMeetingEnds
            () => { },
            // sprite
            null,
            // position
            Vector3.zero,
            // hudmanager
            __instance,
            // keyboard shortcut
            null,
            true
        );
        gmKillButton.Timer = 0.0f;
        gmKillButton.MaxTimer = 0.0f;
        gmKillButton.showButtonText = true;
        gmKillButtons.Add(gmKillButton);
    }

    gmZoomOut = new CustomButton(
        () => {

            if (Camera.main.orthographicSize < 18.0f)
            {
                Camera.main.orthographicSize *= 1.5f;
                __instance.UICamera.orthographicSize *= 1.5f;
            }

            if (__instance.transform.localScale.x < 6.0f)
            {
                __instance.transform.localScale *= 1.5f;
            }

            *//*TheOtherRolesPlugin.Instance.Log.LogInfo($"Camera zoom {Camera.main.orthographicSize} / {TaskPanelBehaviour.Instance.transform.localPosition.x}");*//*
        },
        () => { return !(GM.gm == null || PlayerControl.LocalPlayer != GM.gm); },
        () => { return true; },
        () => { },
        GM.getZoomOutSprite(),
        // position
        Vector3.zero + Vector3.up * 3.55f + Vector3.right * 0.55f,
        // hudmanager
        __instance,
        // keyboard shortcut
        KeyCode.PageDown,
        false
    );
    gmZoomOut.Timer = 0.0f;
    gmZoomOut.MaxTimer = 0.0f;
    gmZoomOut.LocalScale = Vector3.one * 0.275f;

    gmZoomIn = new CustomButton(
        () => {

            if (Camera.main.orthographicSize > 3.0f)
            {
                Camera.main.orthographicSize /= 1.5f;
                __instance.UICamera.orthographicSize /= 1.5f;
            }

            if (__instance.transform.localScale.x > 1.0f)
            {
                __instance.transform.localScale /= 1.5f;
            }

            *//*TheOtherRolesPlugin.Instance.Log.LogInfo($"Camera zoom {Camera.main.orthographicSize} / {TaskPanelBehaviour.Instance.transform.localPosition.x}");*//*
        },
        () => { return !(GM.gm == null || PlayerControl.LocalPlayer != GM.gm); },
        () => { return true; },
        () => { },
        GM.getZoomInSprite(),
        // position
        Vector3.zero + Vector3.up * 3.55f + Vector3.right * 0.2f,
        // hudmanager
        __instance,
        // keyboard shortcut
        KeyCode.PageUp,
        false
    );
    gmZoomIn.Timer = 0.0f;
    gmZoomIn.MaxTimer = 0.0f;
    gmZoomIn.LocalScale = Vector3.one * 0.275f;

*//*            CustomOverlays.initializeOverlays();
            Vector3 overlayPos = Vector3.zero; // Vector3.down * 0.75f;
            showInfoOverlay = new CustomButton(
                // OnClick
                () => { CustomOverlays.toggleInfoOverlay();  },
                // HasButton
                () => { return !PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM) && !MapOptions.hideSettings; },
                // CouldUse
                () => { return !PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM) && !MapOptions.hideSettings; },
                // OnMeetingEnds
                () => { },
                // Sprite
                CustomOverlays.helpButton,
                // Position
                overlayPos,
                // HudManager
                __instance,
                // Keyboard Shortcut
                null,
                // Mirror
                false);
            showInfoOverlay.Timer = 0.0f;
            showInfoOverlay.MaxTimer = 0.0f;
            showInfoOverlay.LocalScale = Vector3.one;
            //showInfoOverlay.killButtonManager.transform.parent = __instance.MapButton.transform;*//*

            // Medium button
            mediumButton = new CustomButton(
                () => {
                    if (Medium.target != null) {
                        Medium.soulTarget = Medium.target;
                        mediumButton.HasEffect = true;
                    }
                },
                () => { return Medium.medium != null && Medium.medium == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => {
                    if (mediumButton.isEffectActive && Medium.target != Medium.soulTarget) {
                        Medium.soulTarget = null;
                        mediumButton.Timer = 0f;
                        mediumButton.isEffectActive = false;
                    }
                    return Medium.target != null && PlayerControl.LocalPlayer.CanMove;
                },
                () => {
                    mediumButton.Timer = mediumButton.MaxTimer;
                    mediumButton.isEffectActive = false;
                    Medium.soulTarget = null;
                },
                Medium.getQuestionSprite(),
                new Vector3(-1.3f, 0f, 0f),
                __instance,
                KeyCode.Q,
                true,
                Medium.duration,
                () => {
                    mediumButton.Timer = mediumButton.MaxTimer;
                    if (Medium.target == null || Medium.target.player == null) return;
                    string msg = "";

                    int randomNumber = Medium.target.killerIfExisting?.PlayerId == Mini.mini?.PlayerId ? TheOtherRoles.rnd.Next(3) : TheOtherRoles.rnd.Next(4);
                    var typeOfColor = Helpers.isLighterColor(Medium.target.killerIfExisting.Data.DefaultOutfit.ColorId) ?
                        ModTranslation.getString("detectiveColorLight") :
                        ModTranslation.getString("detectiveColorDark");
                    float timeSinceDeath = ((float)(Medium.meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds);
                    string name = " (" + Medium.target.player.Data.PlayerName + ")";

                    if (randomNumber == 0)      msg = string.Format(ModTranslation.getString("mediumQuestion1"), RoleInfo.GetRole(Medium.target.player)) + name;
                    else if (randomNumber == 1) msg = string.Format(ModTranslation.getString("mediumQuestion2"), typeOfColor) + name;
                    else if (randomNumber == 2) msg = string.Format(ModTranslation.getString("mediumQuestion3"), Math.Round(timeSinceDeath / 1000)) + name;
                    else                        msg = string.Format(ModTranslation.getString("mediumQuestion4"), RoleInfo.GetRole(Medium.target.killerIfExisting)) + name; ; // Excludes mini 

                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");

                    // Remove soul
                    if (Medium.oneTimeUse) {
                        float closestDistance = float.MaxValue;
                        SpriteRenderer target = null;

                        foreach ((DeadPlayer db, Vector3 ps) in Medium.deadBodies) {
                            if (db == Medium.target) {
                                Tuple<DeadPlayer, Vector3> deadBody = Tuple.Create(db, ps);
                                Medium.deadBodies.Remove(deadBody);
                                break;
                            }

                        }
                        foreach (SpriteRenderer rend in Medium.souls) {
                            float distance = Vector2.Distance(rend.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                            if (distance < closestDistance) {
                                closestDistance = distance;
                                target = rend;
                            }
                        }

                        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>((p) => {
                            if (target != null) {
                                var tmp = target.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                target.color = tmp;
                            }
                            if (p == 1f && target != null && target.gameObject != null) UnityEngine.Object.Destroy(target.gameObject);
                        })));

                        Medium.souls.Remove(target);
                    }
                }
            );

            // Set the default (or settings from the previous game) timers/durations when spawning the buttons
            setCustomButtonCooldowns();
        }
    }*/
