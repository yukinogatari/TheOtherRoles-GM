using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Objects;
using UnityEngine;
using TheOtherRoles.Roles;

namespace TheOtherRoles.Patches
{
    // TODO: THIS PRETTY MUCH ALL NEEDS TO BE MOVED TO ITS RESPECTIVE ROLES SECTIONS

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
        }
        // Helpers

        public static PlayerControl setTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null)
        {
            PlayerControl result = null;
            float num = GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];
            if (!ShipStatus.Instance) return result;
            if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
            if (targetingPlayer.Data.IsDead || targetingPlayer.inVent) return result;
            if (targetingPlayer.isRole(CustomRoleTypes.GM)) return result;

            // GM is untargetable by anything
            if (RoleHelpers.roleExists(CustomRoleTypes.GM))
            {
                if (untargetablePlayers == null)
                {
                    untargetablePlayers = new List<PlayerControl>();
                }
                untargetablePlayers.AddRange(RoleHelpers.getPlayersWithRole(CustomRoleTypes.GM));
            }

            Vector2 truePosition = targetingPlayer.GetTruePosition();
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                GameData.PlayerInfo playerInfo = allPlayers[i];
                if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Object.role()?.IsImpostor == true))
                {
                    PlayerControl @object = playerInfo.Object;
                    if (untargetablePlayers != null && untargetablePlayers.Any(x => x == @object))
                    {
                        // if that player is not targetable: skip check
                        continue;
                    }

                    if (@object && (@object.moveable || (@object.inVent && targetPlayersInVents)))
                    {
                        Vector2 vector = @object.GetTruePosition() - truePosition;
                        float magnitude = vector.magnitude;
                        if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                        {
                            result = @object;
                            num = magnitude;
                        }
                    }
                }
            }
            return result;
        }

        public static void setPlayerOutline(PlayerControl target, Color color)
        {
            if (target == null || target.myRend == null) return;

            target.myRend.material.SetFloat("_Outline", 1f);
            target.myRend.material.SetColor("_OutlineColor", color);
        }


        // Update functions


        /*
                static void medicSetTarget()
                {
                    var role = PlayerControl.LocalPlayer.getRole<Medic>();
                    if (role == null) return;

                    role.currentTarget = setTarget();
                    if (!role.usedShield) setPlayerOutline(role.currentTarget, Medic.shieldedColor);
                }

                static void shifterSetTarget()
                {
                    var role = PlayerControl.LocalPlayer.getRole<Shifter>();
                    if (role == null) return;

                    role.currentTarget = setTarget();
                    if (role.futureShift == null) setPlayerOutline(role.currentTarget, RoleColors.Shifter);
                }



                static void sheriffSetTarget()
                {
                    var role = PlayerControl.LocalPlayer.getRole<Sheriff>();
                    if (role == null) return;

                    role.currentTarget = setTarget();
                    setPlayerOutline(role.currentTarget, RoleColors.Sheriff);
                }

                static void trackerSetTarget()
                {
                    var role = PlayerControl.LocalPlayer.getRole<Tracker>();
                    if (role == null) return;

                    role.currentTarget = setTarget();
                    if (!role.usedTracker) setPlayerOutline(role.currentTarget, RoleColors.Tracker);
                }

                static void detectiveUpdateFootPrints()
                {
                    var role = PlayerControl.LocalPlayer.getRole<Detective>();
                    if (role == null) return;

                    role.timer -= Time.fixedDeltaTime;
                    if (role.timer <= 0f) {
                        role.timer = Detective.footprintIntervall;
                        foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                            if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent && !player.isRole(CustomRoleTypes.GM)) {
                                new Footprint(Detective.footprintDuration, Detective.anonymousFootprints, player);
                            }
                        }
                    }
                }

                static void vampireSetTarget() {
                    if (Vampire.vampire == null || Vampire.vampire != PlayerControl.LocalPlayer) return;

                    PlayerControl target = null;
                    if (Spy.spy != null) {
                        if (Spy.impostorsCanKillAnyone) {
                            target = setTarget(false, true);
                        } else {
                            target = setTarget(true, true, new List<PlayerControl>() { Spy.spy });
                        }
                    } else {
                        target = setTarget(true, true);
                    }

                    bool targetNearGarlic = false;
                    if (target != null) {
                        foreach (Garlic garlic in Garlic.garlics) {
                            if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.91f) {
                                targetNearGarlic = true;
                            }
                        }
                    }
                    Vampire.targetNearGarlic = targetNearGarlic;
                    Vampire.currentTarget = target;
                    setPlayerOutline(Vampire.currentTarget, Vampire.color);
                }

                static void jackalSetTarget() {
                    if (Jackal.jackal == null || Jackal.jackal != PlayerControl.LocalPlayer) return;
                    var untargetablePlayers = new List<PlayerControl>();
                    if(Jackal.canCreateSidekickFromImpostor) {
                        // Only exclude sidekick from beeing targeted if the jackal can create sidekicks from impostors
                        if(Sidekick.sidekick != null) untargetablePlayers.Add(Sidekick.sidekick);
                    }
                    if(Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
                    Jackal.currentTarget = setTarget(untargetablePlayers : untargetablePlayers);
                    setPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);
                }

                static void sidekickSetTarget() {
                    if (Sidekick.sidekick == null || Sidekick.sidekick != PlayerControl.LocalPlayer) return;
                    var untargetablePlayers = new List<PlayerControl>();
                    if(Jackal.jackal != null) untargetablePlayers.Add(Jackal.jackal);
                    if(Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Sidekick from targeting the Mini unless it has grown up
                    Sidekick.currentTarget = setTarget(untargetablePlayers : untargetablePlayers);
                    if (Sidekick.canKill) setPlayerOutline(Sidekick.currentTarget, Palette.ImpostorRed);
                }

                static void sidekickCheckPromotion() {
                    // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
                    if (Sidekick.sidekick == null || Sidekick.sidekick != PlayerControl.LocalPlayer) return;
                    if (Sidekick.sidekick.Data.IsDead == true || !Sidekick.promotesToJackal) return;
                    if (Jackal.jackal == null || Jackal.jackal?.Data?.Disconnected == true) {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.sidekickPromotes(); 
                    }
                }

                static void eraserSetTarget() {
                    if (Eraser.eraser == null || Eraser.eraser != PlayerControl.LocalPlayer) return;

                    List<PlayerControl> untargetables = new List<PlayerControl>();
                    if (Spy.spy != null) untargetables.Add(Spy.spy);
                    Eraser.currentTarget = setTarget(onlyCrewmates: !Eraser.canEraseAnyone, untargetablePlayers: Eraser.canEraseAnyone ? new List<PlayerControl>() : untargetables);
                    setPlayerOutline(Eraser.currentTarget, Eraser.color);
                }

                static void engineerUpdate() {
                    if ((Jackal.canSeeEngineerVent && (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Jackal) || PlayerControl.LocalPlayer == Sidekick.sidekick)) || CustomRoleManager.getRole(PlayerControl.LocalPlayer).IsImpostor && ShipStatus.Instance?.AllVents != null) {
                        foreach (Vent vent in ShipStatus.Instance.AllVents) {
                            try {
                                if (vent?.myRend?.material != null) {
                                    if (Engineer.engineer != null && Engineer.engineer.inVent) {
                                        vent.myRend.material.SetFloat("_Outline", 1f);
                                        vent.myRend.material.SetColor("_OutlineColor", Engineer.color);
                                    } else if (vent.myRend.material.GetColor("_AddColor") != Color.red) {
                                        vent.myRend.material.SetFloat("_Outline", 0);
                                    }
                                }
                            } catch {}
                        }
                    }
                }

                static void impostorSetTarget() {
                    if (!CustomRoleManager.getRole(PlayerControl.LocalPlayer).IsImpostor ||!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead) { // !isImpostor || !canMove || isDead
                        DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                        return;
                    }

                    PlayerControl target = null; 
                    if (Spy.spy != null) {
                        if (Spy.impostorsCanKillAnyone) {
                            target = setTarget(false, true);
                        } else {
                            target = setTarget(true, true, new List<PlayerControl>() { Spy.spy });
                        }
                    } else {
                        target = setTarget(true, true);
                    }

                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
                }

                static void warlockSetTarget() {
                    if (Warlock.warlock == null || Warlock.warlock != PlayerControl.LocalPlayer) return;
                    if (Warlock.curseVictim != null && (Warlock.curseVictim.Data.Disconnected || Warlock.curseVictim.Data.IsDead)) {
                        // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                        Warlock.resetCurse();
                    }
                    if (Warlock.curseVictim == null) {
                        Warlock.currentTarget = setTarget();
                        setPlayerOutline(Warlock.currentTarget, Warlock.color);
                    } else {
                        Warlock.curseVictimTarget = setTarget(targetingPlayer: Warlock.curseVictim);
                        setPlayerOutline(Warlock.curseVictimTarget, Warlock.color);
                    }
                }

                static void trackerUpdate() {
                    if (Tracker.arrow?.arrow == null) return;

                    if (Tracker.tracker == null || PlayerControl.LocalPlayer != Tracker.tracker) {
                        Tracker.arrow.arrow.SetActive(false);
                        return;
                    }

                    if (Tracker.tracker != null && Tracker.tracked != null && PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Tracker) && !Tracker.tracker.Data.IsDead) {
                        Tracker.timeUntilUpdate -= Time.fixedDeltaTime;

                        if (Tracker.timeUntilUpdate <= 0f) {
                            bool trackedOnMap = !Tracker.tracked.Data.IsDead;
                            Vector3 position = Tracker.tracked.transform.position;
                            if (!trackedOnMap) { // Check for dead body
                                DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Tracker.tracked.PlayerId);
                                if (body != null) {
                                    trackedOnMap = true;
                                    position = body.transform.position;
                                }
                            }

                            Tracker.arrow.Update(position);
                            Tracker.arrow.arrow.SetActive(trackedOnMap);
                            Tracker.timeUntilUpdate = Tracker.updateInterval;
                        } else {
                            Tracker.arrow.Update();
                        }
                    }
                }

                public static void playerSizeUpdate(PlayerControl p) {
                    // Set default player size
                    CircleCollider2D collider = p.GetComponent<CircleCollider2D>();

                    p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    collider.radius = Mini.defaultColliderRadius;
                    collider.offset = Mini.defaultColliderOffset * Vector2.down;

                    // Set adapted player size to Mini and Morphling
                    if (Mini.mini == null  || Camouflager.camouflageTimer > 0f) return;

                    float growingProgress = Mini.growingProgress();
                    float scale = growingProgress * 0.35f + 0.35f;
                    float correctedColliderRadius = Mini.defaultColliderRadius * 0.7f / scale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

                    if (p == Mini.mini) {
                        p.transform.localScale = new Vector3(scale, scale, 1f);
                        collider.radius = correctedColliderRadius;
                    }
                    if (Morphling.morphling != null && p == Morphling.morphling && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f) {
                        p.transform.localScale = new Vector3(scale, scale, 1f);
                        collider.radius = correctedColliderRadius;
                    }
                }

                public static void securityGuardSetTarget() {
                    if (SecurityGuard.securityGuard == null || SecurityGuard.securityGuard != PlayerControl.LocalPlayer || ShipStatus.Instance == null || ShipStatus.Instance.AllVents == null) return;

                    Vent target = null;
                    Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                    float closestDistance = float.MaxValue;
                    for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++) {
                        Vent vent = ShipStatus.Instance.AllVents[i];
                        if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                        float distance = Vector2.Distance(vent.transform.position, truePosition);
                        if (distance <= vent.UsableDistance && distance < closestDistance) {
                            closestDistance = distance;
                            target = vent;
                        }
                    }
                    SecurityGuard.ventTarget = target;
                }

                static void snitchUpdate()
                {
                    if (Snitch.localArrows == null) return;

                    foreach (Arrow arrow in Snitch.localArrows) arrow.arrow.SetActive(false);

                    if (Snitch.snitch == null || Snitch.snitch.Data.IsDead) return;

                    var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                    int numberOfTasks = playerTotal - playerCompleted;

                    if (numberOfTasks <= Snitch.taskCountForReveal && (CustomRoleManager.getRole(PlayerControl.LocalPlayer).IsImpostor || (Snitch.includeTeamJackal && (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Jackal) || PlayerControl.LocalPlayer == Sidekick.sidekick)))) {
                        if (Snitch.localArrows.Count == 0) Snitch.localArrows.Add(new Arrow(Color.blue));
                        if (Snitch.localArrows.Count != 0 && Snitch.localArrows[0] != null)
                        {
                            Snitch.localArrows[0].arrow.SetActive(true);
                            Snitch.localArrows[0].image.color = Color.blue;
                            Snitch.localArrows[0].Update(Snitch.snitch.transform.position);
                        }
                    }
                    else if (PlayerControl.LocalPlayer == Snitch.snitch && numberOfTasks == 0)
                    {
                        int arrowIndex = 0;
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                        {
                            if (!p.Data.IsDead && (p.Data.Role.IsImpostor || (Snitch.includeTeamJackal && (p.isRole(CustomRoleTypes.Jackal) || p == Sidekick.sidekick)))) {
                                // Update the arrows' color every time bc things go weird when you add a sidekick or someone dies
                                Color c = Snitch.teamJackalUseDifferentArrowColor && (p.isRole(CustomRoleTypes.Jackal) || p == Sidekick.sidekick) ? Jackal.color : Palette.ImpostorRed;

                                if (arrowIndex >= Snitch.localArrows.Count) {
                                    Snitch.localArrows.Add(new Arrow(c));
                                }
                                if (arrowIndex < Snitch.localArrows.Count && Snitch.localArrows[arrowIndex] != null) {
                                    Snitch.localArrows[arrowIndex].image.color = c;
                                    Snitch.localArrows[arrowIndex].arrow.SetActive(true);
                                    Snitch.localArrows[arrowIndex].Update(p.transform.position);
                                }
                                arrowIndex++;
                            }
                        }
                    }
                }

                static void bountyHunterUpdate() {
                    if (BountyHunter.bountyHunter == null || PlayerControl.LocalPlayer != BountyHunter.bountyHunter) return;

                    if (BountyHunter.bountyHunter.Data.IsDead) {
                        if (BountyHunter.arrow != null || BountyHunter.arrow.arrow != null) UnityEngine.Object.Destroy(BountyHunter.arrow.arrow);
                        BountyHunter.arrow = null;
                        if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null) UnityEngine.Object.Destroy(BountyHunter.cooldownText.gameObject);
                        BountyHunter.cooldownText = null;
                        BountyHunter.bounty = null;
                        foreach (PoolablePlayer p in MapOptions.playerIcons.Values) {
                            if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                        }
                        return;
                    }

                    BountyHunter.arrowUpdateTimer -= Time.fixedDeltaTime;
                    BountyHunter.bountyUpdateTimer -= Time.fixedDeltaTime;

                    if (BountyHunter.bounty == null || BountyHunter.bountyUpdateTimer <= 0f) {
                        // Set new bounty
                        BountyHunter.bounty = null;
                        BountyHunter.arrowUpdateTimer = 0f; // Force arrow to update
                        BountyHunter.bountyUpdateTimer = BountyHunter.bountyDuration;
                        var possibleTargets = new List<PlayerControl>();
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                            if (!p.Data.IsDead && !p.Data.Disconnected && !p.Data.Role.IsImpostor && p != Spy.spy && (p != Mini.mini || Mini.isGrownUp()) && !p.isRole(CustomRoleTypes.GM)) possibleTargets.Add(p);
                        }
                        BountyHunter.bounty = possibleTargets[TheOtherRoles.rnd.Next(0, possibleTargets.Count)];
                        if (BountyHunter.bounty == null) return;

                        // Show poolable player
                        if (DestroyableSingleton<HudManager>.Instance != null && DestroyableSingleton<HudManager>.Instance.UseButton != null) {
                            foreach (PoolablePlayer pp in MapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
                            if (MapOptions.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) && MapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                                MapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(true);
                        }
                    }

                    // Update Cooldown Text
                    if (BountyHunter.cooldownText != null) {
                        BountyHunter.cooldownText.text = Mathf.CeilToInt(Mathf.Clamp(BountyHunter.bountyUpdateTimer, 0, BountyHunter.bountyDuration)).ToString();
                    } 

                    // Update Arrow
                    if (BountyHunter.showArrow && BountyHunter.bounty != null) {
                        if (BountyHunter.arrow == null) BountyHunter.arrow = new Arrow(Color.red);
                        if (BountyHunter.arrowUpdateTimer <= 0f) {
                            BountyHunter.arrow.Update(BountyHunter.bounty.transform.position);
                            BountyHunter.arrowUpdateTimer = BountyHunter.arrowUpdateIntervall;
                        }
                        BountyHunter.arrow.Update();
                    }
                }

                static void vultureUpdate() {
                    if (Vulture.vulture == null || PlayerControl.LocalPlayer != Vulture.vulture || Vulture.localArrows == null || !Vulture.showArrows) return;
                    if (Vulture.vulture.Data.IsDead) {
                        foreach (Arrow arrow in Vulture.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                        Vulture.localArrows = new List<Arrow>();
                        return; 
                    }

                    DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                    bool arrowUpdate = Vulture.localArrows.Count != deadBodies.Count();
                    int index = 0;

                    if (arrowUpdate) {
                        foreach (Arrow arrow in Vulture.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                        Vulture.localArrows = new List<Arrow>();
                    }

                    foreach (DeadBody db in deadBodies) {
                        if (arrowUpdate) {
                            Vulture.localArrows.Add(new Arrow(Vulture.color));
                            Vulture.localArrows[index].arrow.SetActive(true);
                        }
                        if (Vulture.localArrows[index] != null) Vulture.localArrows[index].Update(db.transform.position);
                        index++;
                    }
                }

                public static void mediumSetTarget() {
                    if (Medium.medium == null || Medium.medium != PlayerControl.LocalPlayer || Medium.medium.Data.IsDead || Medium.deadBodies == null || ShipStatus.Instance?.AllVents == null) return;

                    DeadPlayer target = null;
                    Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                    float closestDistance = float.MaxValue;
                    float usableDistance = ShipStatus.Instance.AllVents.FirstOrDefault().UsableDistance;
                    foreach ((DeadPlayer dp, Vector3 ps) in Medium.deadBodies) {
                        float distance = Vector2.Distance(ps, truePosition);
                        if (distance <= usableDistance && distance < closestDistance) {
                            closestDistance = distance;
                            target = dp;
                        }
                    }
                    Medium.target = target;
                }


                static void gmUpdate()
                {
                    if (!PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM)) return;

                    bool showIcon = (GM.canWarp || GM.canKill) && MeetingHud.Instance == null;

                    foreach (byte playerID in MapOptions.playerIcons.Keys)
                    {
                        PlayerControl pc = Helpers.playerById(playerID);
                        PoolablePlayer pp = MapOptions.playerIcons[playerID];
                        pp.gameObject.SetActive(showIcon);
                        if (pc.Data.IsDead)
                        {
                            pp.setSemiTransparent(true);
                        } else
                        {
                            pp.setSemiTransparent(false);
                        }
                    }

                    if (TaskPanelBehaviour.InstanceExists)
                    {
                        TaskPanelBehaviour.Instance.enabled = false;
                        TaskPanelBehaviour.Instance.background.enabled = false;
                        TaskPanelBehaviour.Instance.tab.enabled = false;
                        TaskPanelBehaviour.Instance.TaskText.enabled = false;
                        TaskPanelBehaviour.Instance.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>().SetText("");
                        //TaskPanelBehaviour.Instance.transform.localPosition = Vector3.negativeInfinityVector;
                    }

                }

                public static void Postfix(PlayerControl __instance) {
                    if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

                    // Mini and Morphling shrink
                    playerSizeUpdate(__instance);

                    if (PlayerControl.LocalPlayer == __instance) {
                        // Update player outlines
                        setBasePlayerOutlines();

                        // Update Role Description
                        Helpers.refreshRoleDescription(__instance);

                        // Update Player Info
                        updatePlayerInfo();

                        // Time Master
                        bendTimeUpdate();
                        // Morphling
                        morphlingSetTarget();
                        // Medic
                        medicSetTarget();
                        // Shifter
                        shifterSetTarget();
                        // Sheriff
                        sheriffSetTarget();
                        // Detective
                        detectiveUpdateFootPrints();
                        // Tracker
                        trackerSetTarget();
                        // Impostor
                        impostorSetTarget();
                        // Vampire
                        vampireSetTarget();
                        Garlic.UpdateAll();
                        // Eraser
                        eraserSetTarget();
                        // Engineer
                        engineerUpdate();
                        // Tracker
                        trackerUpdate();
                        // Jackal
                        jackalSetTarget();
                        // Sidekick
                        sidekickSetTarget();
                        // Warlock
                        warlockSetTarget();
                        // Check for sidekick promotion on Jackal disconnect
                        sidekickCheckPromotion();
                        // SecurityGuard
                        securityGuardSetTarget();
                        // Arsonist
                        arsonistSetTarget();
                        // Snitch
                        snitchUpdate();
                        // BountyHunter
                        bountyHunterUpdate();
                        // Bait
                        baitUpdate();
                        // GM
                        gmUpdate();
                        // Vulture
                        vultureUpdate();
                        // Medium
                        mediumSetTarget();
                    } 
                }
            }

            [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
            class PlayerPhysicsWalkPlayerToPatch {
                private static Vector2 offset = Vector2.zero;
                public static void Prefix(PlayerPhysics __instance) {
                    bool correctOffset = Camouflager.camouflageTimer <= 0f && (__instance.myPlayer == Mini.mini ||  (Morphling.morphling != null && __instance.myPlayer == Morphling.morphling && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f));
                    if (correctOffset) {
                        float currentScaling = (Mini.growingProgress() + 1) * 0.5f;
                        __instance.myPlayer.Collider.offset = currentScaling * Mini.defaultColliderOffset * Vector2.down;
                    }
                }
            }
        */

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        class PlayerControlCmdReportDeadBodyPatch {
            public static bool Prefix(PlayerControl __instance) {
                Helpers.handleVampireBiteOnBodyReport();

                if (__instance.isRole(CustomRoleTypes.GM))
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.LocalPlayer.CmdReportDeadBody))]
        class BodyReportPatch
        {
            static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo target)
            {
                // Medic or Detective report
                bool isMedicReport = PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Medic) && __instance.AmOwner;
                bool isDetectiveReport = PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Detective) && __instance.AmOwner;
                if (isMedicReport || isDetectiveReport)
                {
                    DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == target?.PlayerId)?.FirstOrDefault();

                    if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                        float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds);
                        string msg = "";

                        if (isMedicReport) {
                            msg = String.Format(ModTranslation.getString("medicReport"), Math.Round(timeSinceDeath / 1000));
                        } else if (isDetectiveReport) {
                            if (timeSinceDeath < Detective.reportNameDuration * 1000) {
                                msg = String.Format(ModTranslation.getString("detectiveReportName"), deadPlayer.killerIfExisting.name);
                            } else if (timeSinceDeath < Detective.reportColorDuration * 1000) {
                                var typeOfColor = Helpers.isLighterColor(deadPlayer.killerIfExisting.CurrentOutfit.ColorId) ? 
                                    ModTranslation.getString("detectiveColorLight") :
                                    ModTranslation.getString("detectiveColorDark");
                                msg = String.Format(ModTranslation.getString("detectiveReportColor"), typeOfColor);
                            } else {
                                msg = ModTranslation.getString("detectiveReportNone");
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(msg))
                        {   
                            if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
                            {
                                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);
                            }
                            if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                DestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
                            }
                        }
                    }
                }  
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public static class MurderPlayerPatch
        {
            public static bool resetToCrewmate = false;
            public static bool resetToDead = false;

            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target)
            {
                // Allow everyone to murder players
                resetToCrewmate = !__instance.Data.Role.IsImpostor;
                resetToDead = __instance.Data.IsDead;
                __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
                __instance.Data.IsDead = false;
            }

            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target)
            {
                // Collect dead player info
                DeadPlayer deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeathReason.Kill, __instance);
                GameHistory.deadPlayers.Add(deadPlayer);

                // Reset killer to crewmate if resetToCrewmate
                if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                if (resetToDead) __instance.Data.IsDead = true;

                // Remove fake tasks when player dies
                if (target.hasFakeTasks())
                    target.clearAllTasks();

                target.OnKilled();
                __instance.OnKill();

                // Update arsonist status
                RoleHelpers.getRoles<Arsonist>().ForEach(x => x.updateStatus());
                /*
                                // Sidekick promotion trigger on murder
                                if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && target.isRole(CustomRoleTypes.Jackal) && Jackal.jackal == PlayerControl.LocalPlayer) {
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.sidekickPromotes();
                                }

                                // Cleaner Button Sync
                                if (Cleaner.cleaner != null && PlayerControl.LocalPlayer == Cleaner.cleaner && __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null) 
                                    HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

                                // Warlock Button Sync
                                if (Warlock.warlock != null && PlayerControl.LocalPlayer == Warlock.warlock && __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null) {
                                    if(Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer) {
                                        HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
                                    }
                                }

                                // Seer show flash and add dead player position
                                if (Seer.seer != null && PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Seer) && !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1) {
                                    DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
                                    DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) => {
                                        var renderer = DestroyableSingleton<HudManager>.Instance.FullScreen;
                                        if (p < 0.5) {
                                            if (renderer != null)
                                                renderer.color = new Color(42f / 255f, 187f / 255f, 245f / 255f, Mathf.Clamp01(p * 2 * 0.75f));
                                        } else {
                                            if (renderer != null)
                                                renderer.color = new Color(42f / 255f, 187f / 255f, 245f / 255f, Mathf.Clamp01((1-p) * 2 * 0.75f));
                                        }
                                        if (p == 1f && renderer != null) renderer.enabled = false;
                                    })));
                                }
                                if (Seer.deadBodyPositions != null) Seer.deadBodyPositions.Add(target.transform.position);

                                // Medium add body
                                if (Medium.deadBodies != null) {
                                    Medium.featureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));
                                }

                                // Mini set adapted kill cooldown
                                if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini && Mini.mini.Data.Role.IsImpostor && Mini.mini == __instance) {
                                    var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
                                    Mini.mini.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
                                }

                                // Set bountyHunter cooldown
                                if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter && __instance == BountyHunter.bountyHunter) {
                                    if (target == BountyHunter.bounty) {
                                        BountyHunter.bountyHunter.SetKillTimer(BountyHunter.bountyKillCooldown);
                                        BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
                                    }
                                    else
                                        BountyHunter.bountyHunter.SetKillTimer(PlayerControl.GameOptions.KillCooldown + BountyHunter.punishmentTime); 
                                }
                */
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
        class PlayerControlSetCoolDownPatch {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)]float time) {
                if (PlayerControl.GameOptions.KillCooldown <= 0f) return false;
                float multiplier = 1f;
                float addition = 0f;
                if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.EvilMini)) multiplier = PlayerControl.LocalPlayer.role<Mini>().isGrownUp ? 0.66f : 2f;
                if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.BountyHunter)) addition = BountyHunter.punishmentTime;

                __instance.killTimer = Mathf.Clamp(time, 0f, PlayerControl.GameOptions.KillCooldown * multiplier + addition);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, PlayerControl.GameOptions.KillCooldown * multiplier + addition);
                return false;
            }
        }

        [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
        class KillAnimationCoPerformKillPatch {
            public static void Prefix(KillAnimation __instance, [HarmonyArgument(0)] ref PlayerControl source, [HarmonyArgument(1)] ref PlayerControl target) {
                foreach (PlayerControl pc in RoleHelpers.getPlayersWithRole(CustomRoleTypes.Vampire)) {
                    if (pc.role<Vampire>().bitten == target) source = target;
                }

                foreach (PlayerControl pc in RoleHelpers.getPlayersWithRole(CustomRoleTypes.Warlock))
                {
                    if (pc.role<Warlock>().curseKillTarget == target)
                    {
                        source = target;
                        pc.role<Warlock>().curseKillTarget = null; // Reset here
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
        public static class ExilePlayerPatch
        {
            public static void Postfix(PlayerControl __instance)
            {
                // Collect dead player info
                DeadPlayer deadPlayer = new DeadPlayer(__instance, DateTime.UtcNow, DeathReason.Exile, null);
                GameHistory.deadPlayers.Add(deadPlayer);

                // Remove fake tasks when player dies
                if (__instance.hasFakeTasks())
                    __instance.clearAllTasks();

                __instance.customRole()?.OnKilled();

                // Lover suicide trigger on exile
                foreach (RoleModifier mod in __instance.getModifiers())
                    mod.OnKilled();

/*                    // Sidekick promotion trigger on exile
                if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && __instance.isRole(CustomRoleTypes.Jackal) && Jackal.jackal == PlayerControl.LocalPlayer) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.sidekickPromotes();
                }*/
            }
        }
        
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckName))]
            class PlayerControlCheckNamePatch
            {
                public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)]string name)
                {
                    TheOtherRolesPlugin.Instance.Log.LogInfo($"Checking name {name}.");
                    if (CustomOptionHolder.uselessOptions.getBool() && CustomOptionHolder.playerNameDupes.getBool())
                    {
                        TheOtherRolesPlugin.Instance.Log.LogInfo($"Dupes allowed for {name}.");
                        __instance.RpcSetName(name);
                        GameData.Instance.UpdateName(__instance.PlayerId, name, false);
                        return false;
                    }

                    return true;
                }
            }
    }
}