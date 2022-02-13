using HarmonyLib;
using System;
using System.IO;
using System.Net.Http;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        static void resetNameTagsAndColors() {
            Dictionary<byte, PlayerControl> playersById = Helpers.allPlayersById();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                player.nameText.text = Helpers.hidePlayerName(PlayerControl.LocalPlayer, player) ? "" : player.CurrentOutfit.PlayerName;
                if (PlayerControl.LocalPlayer.isImpostor() && player.isImpostor()) {
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
                        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && playerControl.Data.Role.IsImpostor) {
                            player.NameText.color = Palette.ImpostorRed;
                        } else {
                            player.NameText.color = Color.white;
                        }
                    }
                }
            }

            if (PlayerControl.LocalPlayer.isImpostor()) {
                List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.isImpostor()).ToList();
                foreach (PlayerControl player in impostors)
                    player.nameText.color = Palette.ImpostorRed;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        PlayerControl playerControl = Helpers.playerById((byte)player.TargetPlayerId);
                        if (playerControl != null && playerControl.Data.Role.IsImpostor)
                            player.NameText.color =  Palette.ImpostorRed;
                    }
            }

        }

        static void setPlayerNameColor(PlayerControl p, Color color) {
            p.nameText.color = color;
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = color;
        }

        static void setNameColors() {
            if (PlayerControl.LocalPlayer.isRole(RoleType.Jester))
                setPlayerNameColor(PlayerControl.LocalPlayer, Jester.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Mayor))
                setPlayerNameColor(PlayerControl.LocalPlayer, Mayor.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Engineer))
                setPlayerNameColor(PlayerControl.LocalPlayer, Engineer.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Sheriff))
                setPlayerNameColor(PlayerControl.LocalPlayer, Sheriff.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Lighter))
                setPlayerNameColor(PlayerControl.LocalPlayer, Lighter.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Detective))
                setPlayerNameColor(PlayerControl.LocalPlayer, Detective.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.TimeMaster))
                setPlayerNameColor(PlayerControl.LocalPlayer, TimeMaster.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Medic))
                setPlayerNameColor(PlayerControl.LocalPlayer, Medic.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Shifter))
                setPlayerNameColor(PlayerControl.LocalPlayer, Shifter.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Swapper))
                setPlayerNameColor(PlayerControl.LocalPlayer, Swapper.swapper.Data.Role.IsImpostor ? Palette.ImpostorRed : Swapper.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Seer))
                setPlayerNameColor(PlayerControl.LocalPlayer, Seer.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Hacker))
                setPlayerNameColor(PlayerControl.LocalPlayer, Hacker.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Tracker))
                setPlayerNameColor(PlayerControl.LocalPlayer, Tracker.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Snitch))
                setPlayerNameColor(PlayerControl.LocalPlayer, Snitch.color);
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Jackal))
            {
                // Jackal can see his sidekick
                setPlayerNameColor(PlayerControl.LocalPlayer, Jackal.color);
                if (Sidekick.sidekick != null)
                {
                    setPlayerNameColor(Sidekick.sidekick, Jackal.color);
                }
                if (Jackal.fakeSidekick != null)
                {
                    setPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
                }
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Spy))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Spy.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.SecurityGuard))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, SecurityGuard.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Arsonist))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Arsonist.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.NiceGuesser))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Guesser.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.EvilGuesser))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Palette.ImpostorRed);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Bait))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Bait.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Madmate))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Madmate.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Opportunist))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Opportunist.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Vulture))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Vulture.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Medium))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Medium.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Lawyer))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Lawyer.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.Pursuer))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Pursuer.color);
            }
            else if (PlayerControl.LocalPlayer.isRole(RoleType.PlagueDoctor))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, PlagueDoctor.color);
            }

            if (GM.gm != null) {
                setPlayerNameColor(GM.gm, GM.color);
            }

            // No else if here, as a Lover of team Jackal needs the colors
            if (PlayerControl.LocalPlayer.isRole(RoleType.Sidekick)) {
                // Sidekick can see the jackal
                setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
                if (Jackal.jackal != null) {
                    setPlayerNameColor(Jackal.jackal, Jackal.color);
                }
            }

            // No else if here, as the Impostors need the Spy name to be colored
            if (Spy.spy != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                setPlayerNameColor(Spy.spy, Spy.color);
            }

            // Crewmate roles with no changes: Mini
            // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
        }

        static void setNameTags() {
            // Mafia
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.nameText.text == "") continue;
                    if (Godfather.godfather != null && Godfather.godfather == player)
                        player.nameText.text = player.Data.PlayerName + " (G)";
                    else if (Mafioso.mafioso != null && Mafioso.mafioso == player)
                        player.nameText.text = player.Data.PlayerName + " (M)";
                    else if (Janitor.janitor != null && Janitor.janitor == player)
                        player.nameText.text = player.Data.PlayerName + " (J)";
                }
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Godfather.godfather != null && Godfather.godfather.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Godfather.godfather.Data.PlayerName + " (G)";
                        else if (Mafioso.mafioso != null && Mafioso.mafioso.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Mafioso.mafioso.Data.PlayerName + " (M)";
                        else if (Janitor.janitor != null && Janitor.janitor.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Janitor.janitor.Data.PlayerName + " (J)";
            }

            bool meetingShow = MeetingHud.Instance != null && 
                (MeetingHud.Instance.state == MeetingHud.VoteStates.Voted ||
                 MeetingHud.Instance.state == MeetingHud.VoteStates.NotVoted ||
                 MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion);
            
            // Lovers
            if (PlayerControl.LocalPlayer.isLovers() && PlayerControl.LocalPlayer.isAlive()) {
                string suffix = Lovers.getIcon(PlayerControl.LocalPlayer);
                var lover1 = PlayerControl.LocalPlayer;
                var lover2 = PlayerControl.LocalPlayer.getPartner();

                lover1.nameText.text += suffix;
                if (!Helpers.hidePlayerName(lover2))
                    lover2.nameText.text += suffix;

                if (meetingShow)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (lover1.PlayerId == player.TargetPlayerId || lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }
            else if (MapOptions.ghostsSeeRoles && PlayerControl.LocalPlayer.isDead())
            {
                foreach (var couple in Lovers.couples)
                {
                    string suffix = Lovers.getIcon(couple.lover1);
                    couple.lover1.nameText.text += suffix;
                    couple.lover2.nameText.text += suffix;

                    if (meetingShow)
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                            if (couple.lover1.PlayerId == player.TargetPlayerId || couple.lover2.PlayerId == player.TargetPlayerId)
                                player.NameText.text += suffix;
                }
            }

            // Lawyer
            bool localIsLawyer = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.lawyer == PlayerControl.LocalPlayer;
            bool localIsKnowingTarget = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == PlayerControl.LocalPlayer;
            if (localIsLawyer || (localIsKnowingTarget && !Lawyer.lawyer.Data.IsDead)) {
                string suffix = Helpers.cs(Lawyer.color, " §");
                if (!Helpers.hidePlayerName(Lawyer.target))
                    Lawyer.target.nameText.text += suffix;

                if (meetingShow)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == Lawyer.target.PlayerId)
                            player.NameText.text += suffix;
            }

            // Hacker and Detective
            if (PlayerControl.LocalPlayer != null && MapOptions.showLighterDarker) {
                if (meetingShow) {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        var target = Helpers.playerById(player.TargetPlayerId);
                        if (target != null)  player.NameText.text += $" ({(Helpers.isLighterColor(target.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("detectiveLightLabel") : ModTranslation.getString("detectiveDarkLabel"))})";
                    }
                }
            }
        }

        static void updateShielded() {
            if (Medic.shielded == null) return;

            if (Medic.shielded.Data.IsDead || Medic.medic == null || Medic.medic.Data.IsDead) {
                Medic.shielded = null;
            }
        }

        static void timerUpdate() {
            Hacker.hackerTimer -= Time.deltaTime;
            Trickster.lightsOutTimer -= Time.deltaTime;
            Tracker.corpsesTrackingTimer -= Time.deltaTime;
        }

        public static void miniUpdate() {
            if (Mini.mini == null || Camouflager.camouflageTimer > 0f) return;
                
            float growingProgress = Mini.growingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            string suffix = "";
            if (growingProgress != 1f)
                suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growingProgress * 18) + ")</color>"; 

            if (!Helpers.hidePlayerName(Mini.mini))
                Mini.mini.nameText.text += suffix;

            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Mini.mini.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
            }

            if (Morphling.morphling != null && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f && !Helpers.hidePlayerName(Morphling.morphling))
                Morphling.morphling.nameText.text += suffix;
        }

        static void updateImpostorKillButton(HudManager __instance) {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor || MeetingHud.Instance) return;
            bool enabled = Helpers.ShowButtons;
            if (Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer)
                enabled &= false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                enabled &= false;
            else if (Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer)
                enabled &= false;
            
            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();
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

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

            CustomButton.HudUpdate();
            resetNameTagsAndColors();
            setNameColors();
            updateShielded();
            setNameTags();

            // Camouflager and Morphling
            camouflageAndMorphActions();

            // Impostors
            updateImpostorKillButton(__instance);
            // Timer updates
            timerUpdate();
            // Mini
            miniUpdate();
        }
    }
}
