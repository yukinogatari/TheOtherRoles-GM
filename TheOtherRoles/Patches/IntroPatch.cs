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
            if (PlayerControl.LocalPlayer != null && DestroyableSingleton<HudManager>.Instance != null)
            {
                Vector3 bottomLeft = new Vector3(-DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    GameData.PlayerInfo data = p.Data;
                    PoolablePlayer player = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, DestroyableSingleton<HudManager>.Instance.transform);
                    PlayerControl.SetPlayerMaterialColors(p.CurrentOutfit.ColorId, player.Body);
                    player.Skin.SetSkin(p.CurrentOutfit.SkinId, true);
                    player.HatSlot.SetHat(p.CurrentOutfit.HatId, p.CurrentOutfit.ColorId);
                    PlayerControl.SetPetImage(p.CurrentOutfit.PetId, p.CurrentOutfit.ColorId, player.PetSlot);
                    player.NameText.text = data.PlayerName;
                    player.SetFlipX(true);
                    MapOptions.playerIcons[p.PlayerId] = player;
                    MorphData.morphData[p.PlayerId] = new MorphData(p);

                    if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.BountyHunter))
                    {
                        player.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0);
                        player.transform.localScale = Vector3.one * 0.4f;
                        player.gameObject.SetActive(false);
                    }
                    else if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM))
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

            // TODO: fix this
            // Force Bounty Hunter to load a new Bounty when the Intro is over
            /*            if (BountyHunter.bounty != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter) {
                            BountyHunter.bountyUpdateTimer = 0f;
                            if (DestroyableSingleton<HudManager>.Instance != null) {
                                Vector3 bottomLeft = new Vector3(-DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) + new Vector3(-0.25f, 1f, 0);
                                BountyHunter.cooldownText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(DestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, DestroyableSingleton<HudManager>.Instance.transform);
                                BountyHunter.cooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                                BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                                BountyHunter.cooldownText.gameObject.SetActive(true);
                            }
                        }

                        */

            RoleHelpers.getRoles<Arsonist>().ForEach(x => x.updateIcons());

            if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM))
            {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActiveRecursively(false);
                DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
                DestroyableSingleton<HudManager>.Instance.ReportButton.graphic.enabled = false;
                DestroyableSingleton<HudManager>.Instance.ReportButton.enabled = false;
                DestroyableSingleton<HudManager>.Instance.ReportButton.graphic.sprite = null;
                DestroyableSingleton<HudManager>.Instance.ReportButton.buttonLabelText.enabled = false;
                DestroyableSingleton<HudManager>.Instance.ReportButton.buttonLabelText.SetText("");

                DestroyableSingleton<HudManager>.Instance.roomTracker.gameObject.SetActiveRecursively(false);
                DestroyableSingleton<HudManager>.Instance.roomTracker.text.enabled = false;
                DestroyableSingleton<HudManager>.Instance.roomTracker.text.SetText("");
                DestroyableSingleton<HudManager>.Instance.roomTracker.enabled = false;

                if (!GM.hasTasks)
                    PlayerControl.LocalPlayer.clearAllTasks();
            }
        }
    }

    [HarmonyPatch]
    class IntroPatch {
        public static void setupIntroTeam(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            // Intro solo teams
            if (PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Jester) || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Jackal) || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Arsonist) || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM) || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Opportunist) || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Vulture)) {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }

            // Don't show the GM
            if (!PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM))
            {
                var newTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (PlayerControl p in yourTeam)
                {
                    if (!p.isRole(CustomRoleTypes.GM))
                        newTeam.Add(p);
                }
                yourTeam = newTeam;
            }

            // Add the Spy to the Impostor team (for the Impostors)
            if (PlayerControl.LocalPlayer.role()?.IsImpostor == true && RoleHelpers.roleExists(CustomRoleTypes.Spy)) {
                List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
                var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (PlayerControl p in players) {
                    if (p.isRole(CustomRoleTypes.Spy) || p.role()?.IsImpostor == true)
                        fakeImpostorTeam.Add(p);
                }
                yourTeam = fakeImpostorTeam;
            }
        }

        public static void setupIntroMessage(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
            RoleInfo roleInfo = infos.Where(info => info.type != CustomRoleTypes.Lovers).FirstOrDefault();

            RoleBehaviour r = PlayerControl.LocalPlayer.role();
            if (r.TeamType >= (RoleTeamTypes)CustomRoleTeamTypes.Jackal) {
                __instance.TeamTitle.text = r.NiceName;
                __instance.TeamTitle.color = r.NameColor;
                __instance.BackgroundBar.material.color = r.NameColor;
                __instance.ImpostorText.gameObject.SetActive(false);
            }

            if (PlayerControl.LocalPlayer.hasModifier(RoleModifierTypes.Lovers))
            {
                var loversText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(__instance.ImpostorText, __instance.ImpostorText.transform.parent);
                loversText.transform.localPosition += Vector3.down * 3f;
                PlayerControl otherLover = PlayerControl.LocalPlayer.getPartner();
                loversText.text = Helpers.cs(RoleColors.Lovers, String.Format(ModTranslation.getString("loversFlavor"), otherLover?.Data?.PlayerName ?? ""));
            }
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
                RoleBehaviour role = PlayerControl.LocalPlayer.role();
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

