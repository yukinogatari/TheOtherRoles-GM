using HarmonyLib;
using System;
using System.IO;
using System.Net.Http;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Roles;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        static void resetNameTagsAndColors() {
            Dictionary<byte, PlayerControl> playersById = Helpers.allPlayersById();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                player.nameText.text = Helpers.hidePlayerName(PlayerControl.LocalPlayer, player) ? "" : player.CurrentOutfit.PlayerName;
                if (PlayerControl.LocalPlayer.role()?.IsImpostor == true && player.role()?.IsImpostor == true) {
                    player.nameText.color = Palette.ImpostorRed;
                } else {
                    player.nameText.color = Color.white;
                }
            }

            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                    PlayerControl playerControl = playersById.ContainsKey((byte)player.TargetPlayerId) ? playersById[(byte)player.TargetPlayerId] : null;
                    if (playerControl != null) {
                        player.NameText.text = playerControl.Data.PlayerName;
                        if (PlayerControl.LocalPlayer.role()?.IsImpostor == true && playerControl.role()?.IsImpostor == true) {
                            player.NameText.color = Palette.ImpostorRed;
                        } else {
                            player.NameText.color = Color.white;
                        }
                    }
                }
            }
            if (PlayerControl.LocalPlayer.role()?.IsImpostor == true) {
                List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().ToList();
                impostors.RemoveAll(x => !x.role()?.IsImpostor == true);
                foreach (PlayerControl player in impostors)
                    player.nameText.color = Palette.ImpostorRed;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        PlayerControl playerControl = Helpers.playerById((byte)player.TargetPlayerId);
                        if (playerControl != null && playerControl.role()?.IsImpostor == true)
                            player.NameText.color =  Palette.ImpostorRed;
                    }
            }

        }

        static void setPlayerNameColor(PlayerControl p, Color? color) {
            if (color == null) return;

            p.nameText.color = (Color)color;
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = (Color)color;
        }

        static void setNameColors()
        {
            if (PlayerControl.LocalPlayer == null) return;

            setPlayerNameColor(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.role()?.NameColor);

            if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Jackal))
            {
                var jackal = PlayerControl.LocalPlayer.role<Jackal>();
                if (jackal != null)
                {
                    foreach (PlayerControl pc in jackal.getFamily())
                    {
                        setPlayerNameColor(pc, RoleColors.Jackal);
                    }
                }
            }

            foreach (PlayerControl pc in RoleHelpers.getPlayersWithRole(CustomRoleTypes.GM))
            {
                setPlayerNameColor(pc, RoleColors.GM);
            }

            // No else if here, as the Impostors need the Spy name to be colored
            if (RoleHelpers.roleExists(CustomRoleTypes.Spy) && PlayerControl.LocalPlayer.role()?.IsImpostor == true)
            {
                foreach (PlayerControl pc in RoleHelpers.getPlayersWithRole(CustomRoleTypes.Spy))
                {
                    setPlayerNameColor(pc, RoleColors.Spy);
                }
            }

            // Crewmate roles with no changes: Mini
            // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter and Mafioso
        }

        static void setNameTags() {
            // Mafia
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.role()?.IsImpostor == true)
            {
                foreach (PlayerControl pc in RoleHelpers.getPlayersWithRole(CustomRoleTypes.Godfather))
                {
                    Mafia gf = pc.role<Mafia>();
                    if (gf == null) continue;

                    gf.godfather.nameText.text = gf.godfather.Data.PlayerName + " (G)";
                    gf.mafioso.nameText.text = gf.mafioso.Data.PlayerName + " (M)";
                    gf.janitor.nameText.text = gf.janitor.Data.PlayerName + " (G)";

                    if (MeetingHud.Instance != null)
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                            if (gf.godfather.PlayerId == player.TargetPlayerId)
                                player.NameText.text = gf.godfather.Data.PlayerName + " (G)";
                            else if (gf.mafioso.PlayerId == player.TargetPlayerId)
                                player.NameText.text = gf.mafioso.Data.PlayerName + " (M)";
                            else if (gf.janitor.PlayerId == player.TargetPlayerId)
                                player.NameText.text = gf.janitor.Data.PlayerName + " (J)";
                }
            }

            // Lovers
            if (PlayerControl.LocalPlayer.isLovers())
            {
                string suffix = Helpers.cs(RoleColors.Lovers, " ♥");
                PlayerControl partner = PlayerControl.LocalPlayer.getPartner();
                PlayerControl.LocalPlayer.nameText.text += suffix;
                partner.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (PlayerControl.LocalPlayer.PlayerId == player.TargetPlayerId || partner.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }
        }

        /*static void updateShielded() {
            if (Medic.shielded == null) return;

            if (Medic.shielded.Data.IsDead || Medic.medic == null || Medic.medic.Data.IsDead) {
                Medic.shielded = null;
            }
        }

        static void timerUpdate() {
            Hacker.hackerTimer -= Time.deltaTime;
            Trickster.lightsOutTimer -= Time.deltaTime;
        }

        public static void miniUpdate() {
            if (Mini.mini == null || Camouflager.camouflageTimer > 0f) return;
                
            float growingProgress = Mini.growingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            string suffix = "";
            if (growingProgress != 1f)
                suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growingProgress * 18) + ")</color>"; 

            Mini.mini.nameText.text += suffix;
            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Mini.mini.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
            }

            if (Morphling.morphling != null && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f)
                Morphling.morphling.nameText.text += suffix;
        }

        static void updateImpostorKillButton(HudManager __instance) {
            if (!PlayerControl.LocalPlayer.getRole().IsImpostor) return;
            bool enabled = true;
            if (Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer)
                enabled = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                enabled = false;
            else if (Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer)
                enabled = false;
            enabled &= __instance.UseButton.isActiveAndEnabled;
            
            __instance.KillButton.gameObject.SetActive(enabled);
            __instance.KillButton.graphic.enabled = enabled;
            //__instance.KillButton.isActive = enabled;
            __instance.KillButton.enabled = enabled;
        }

        static void camouflageAndMorphActions()
        {
            float oldCamouflageTimer = Camouflager.camouflageTimer;
            float oldMorphTimer = Morphling.morphTimer;

            Camouflager.camouflageTimer -= Time.deltaTime;
            Morphling.morphTimer -= Time.deltaTime;

            // Everyone but morphling reset
            if (oldCamouflageTimer > 0f && Camouflager.camouflageTimer <= 0f)
            {
                Camouflager.resetCamouflage();
            }

            // Morphling reset
            if (oldMorphTimer > 0f && Morphling.morphTimer <= 0f)
            {
                Morphling.resetMorph();
            }
        }

        static void GMUpdate(HudManager __instance)
        {
            //__instance.UseButton.enabled = false;
*//*            if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM) && PlayerControl.LocalPlayer.Data.IsDead)
            {
                foreach (UseButton b in DestroyableSingleton<HudManager>.Instance.UseButton.useButtons)
                {
                    //b.gameObject.SetActiveRecursively(false);
                    b.graphic.enabled = false;
                    b.enabled = false;
                    b.text.enabled = false;
                }
            }*//*
        }*/

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

            CustomRoleManager.InitializeButtons();
            CustomButton.HudUpdate();
            resetNameTagsAndColors();
            setNameColors();
            //updateShielded();
            setNameTags();

            // Camouflager and Morphling
            //camouflageAndMorphActions();

            // Impostors
            //updateImpostorKillButton(__instance);
            // Timer updates
            //timerUpdate();
            // Mini
            //miniUpdate();
            // Observer roles
            //GMUpdate(__instance);
        }
    }
}
