using HarmonyLib;
using System;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Roles;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class IntroCutsceneOnDestroyPatch
    {
        public static void Prefix(IntroCutscene __instance) {
            // Generate and initialize player icons
            if (PlayerControl.LocalPlayer != null && HudManager.Instance != null)
            {
                CustomOverlays.initializeOverlays();
                Vector3 bottomLeft = new Vector3(-HudManager.Instance.UseButton.transform.localPosition.x, HudManager.Instance.UseButton.transform.localPosition.y, HudManager.Instance.UseButton.transform.localPosition.z);
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    GameData.PlayerInfo data = p.Data;
                    PoolablePlayer player = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, HudManager.Instance.transform);
                    PlayerControl.SetPlayerMaterialColors(p.CurrentOutfit.ColorId, player.Body);
                    player.Skin.SetSkin(p.CurrentOutfit.SkinId, true);
                    player.HatSlot.SetHat(p.CurrentOutfit.HatId, p.CurrentOutfit.ColorId);
                    PlayerControl.SetPetImage(p.CurrentOutfit.PetId, p.CurrentOutfit.ColorId, player.PetSlot);
                    player.NameText.text = data.PlayerName;
                    player.SetFlipX(true);
                    MapOptions.playerIcons[p.PlayerId] = player;
                    MorphData.morphData[p.PlayerId] = new MorphData(p);

                    if (PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
                    {
                        player.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0);
                        player.transform.localScale = Vector3.one * 0.4f;
                        player.gameObject.SetActive(false);
                    }
                    else if (PlayerControl.LocalPlayer == GM.gm)
                    {
                        player.transform.localPosition = Vector3.zero;
                        player.transform.localScale = Vector3.one * 0.3f;
                        player.setSemiTransparent(false);
                        player.gameObject.SetActive(false);
                    }
                    else
                    {
                        player.gameObject.SetActive(false);
                    }
                }
            }

            // Force Bounty Hunter to load a new Bounty when the Intro is over
            if (BountyHunter.bounty != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter) {
                BountyHunter.bountyUpdateTimer = 0f;
                if (HudManager.Instance != null) {
                    Vector3 bottomLeft = new Vector3(-HudManager.Instance.UseButton.transform.localPosition.x, HudManager.Instance.UseButton.transform.localPosition.y, HudManager.Instance.UseButton.transform.localPosition.z) + new Vector3(-0.25f, 1f, 0);
                    BountyHunter.cooldownText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.transform);
                    BountyHunter.cooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                    BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                    BountyHunter.cooldownText.gameObject.SetActive(true);
                }
            }

            Arsonist.updateIcons();

            if (PlayerControl.LocalPlayer == GM.gm && !GM.hasTasks)
            {
                PlayerControl.LocalPlayer.clearAllTasks();
            }

            if (PlayerControl.LocalPlayer.isGM())
            {
                HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                HudManager.Instance.ReportButton.gameObject.SetActiveRecursively(false);
                HudManager.Instance.ReportButton.SetActive(false);
                HudManager.Instance.ReportButton.graphic.enabled = false;
                HudManager.Instance.ReportButton.enabled = false;
                HudManager.Instance.ReportButton.graphic.sprite = null;
                HudManager.Instance.ReportButton.buttonLabelText.enabled = false;
                HudManager.Instance.ReportButton.buttonLabelText.SetText("");

                HudManager.Instance.roomTracker.gameObject.SetActiveRecursively(false);
                HudManager.Instance.roomTracker.text.enabled = false;
                HudManager.Instance.roomTracker.text.SetText("");
                HudManager.Instance.roomTracker.enabled = false;
            }
        }
    }

    [HarmonyPatch]
    class IntroPatch {
        public static void setupIntroTeam(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            // Intro solo teams
            if (PlayerControl.LocalPlayer == Jester.jester || PlayerControl.LocalPlayer == Jackal.jackal || PlayerControl.LocalPlayer == Arsonist.arsonist || PlayerControl.LocalPlayer == GM.gm || PlayerControl.LocalPlayer == Opportunist.opportunist || PlayerControl.LocalPlayer == Vulture.vulture) {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }

            // Don't show the GM
            if (!PlayerControl.LocalPlayer.isGM())
            {
                var newTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (PlayerControl p in yourTeam)
                {
                    if (p != GM.gm)
                        newTeam.Add(p);
                }
                yourTeam = newTeam;
            }

            // Add the Spy to the Impostor team (for the Impostors)
            if (Spy.spy != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
                var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (PlayerControl p in players) {
                    if (p == Spy.spy || p.Data.Role.IsImpostor)
                        fakeImpostorTeam.Add(p);
                }
                yourTeam = fakeImpostorTeam;
            }
        }

        public static void setupIntroMessage(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
            RoleInfo roleInfo = infos.Where(info => info.roleId != CustomRoleTypes.Lovers).FirstOrDefault();

            RoleBehaviour r = PlayerControl.LocalPlayer.Data.Role;
            RoleInfo info = RoleInfo.getRoleInfo(PlayerControl.LocalPlayer.Data.Role.Role);

            if (r != null) {
                __instance.TeamTitle.text = r.TeamType.ToString();
                __instance.ImpostorText.gameObject.SetActive(true);

                if (r.TeamType != RoleTeamTypes.Crewmate && r.TeamType != RoleTeamTypes.Impostor) {
                    // For native Crewmate or Impostor do not modify the colors
                    __instance.TeamTitle.color = r.NameColor;
                    __instance.BackgroundBar.material.color = r.NameColor;
                }
            }

/*            if (infos.Any(info => info.roleId == CustomRoleTypes.Lover)) {
                var loversText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(__instance.ImpostorText, __instance.ImpostorText.transform.parent);
                loversText.transform.localPosition += Vector3.down * 3f;
                PlayerControl otherLover = PlayerControl.LocalPlayer == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                loversText.text = Helpers.cs(Lovers.color, String.Format(ModTranslation.getString("loversFlavor"), otherLover?.Data?.PlayerName ?? ""));
            }*/
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                setupIntroTeam(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                setupIntroMessage(__instance, ref yourTeam);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                setupIntroTeam(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                setupIntroMessage(__instance, ref yourTeam);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.SetUpRoleText))]
        class SetUpRoleTextPatch
        {
            public static bool Prefix(IntroCutscene __instance)
            {
                var role = PlayerControl.LocalPlayer.Data.Role;
                if (role.Role >= (RoleTypes)CustomRoleTypes.Crewmate)
                {
                    __instance.YouAreText.color = role.NameColor;
                    __instance.RoleText.color = role.NameColor;
                    __instance.RoleBlurbText.color = role.NameColor;

                    __instance.RoleText.text = role.NiceName;
                    __instance.RoleBlurbText.text = role.BlurbMed;
                    return false;
                }

                return true;
            }
        }
    }
}

