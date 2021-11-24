using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Arsonist : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Arsonist;

        public static CustomOptionBlank options;
        public static CustomOption arsonistCooldown;
        public static CustomOption arsonistDuration;

        public static float cooldown { get { return arsonistCooldown.getFloat(); } }
        public static float duration { get { return arsonistDuration.getFloat(); } }

        public CustomButton douseButton;
        public CustomButton igniteButton;
        public static bool triggerArsonistWin = false;
        public bool dousedEveryone = false;

        public PlayerControl currentTarget;
        public PlayerControl douseTarget;
        public List<PlayerControl> dousedPlayers = new List<PlayerControl>();

        private static Sprite douseSprite;
        private static Sprite igniteSprite;
        public static PlayerControl winner;

        public Arsonist() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.Arsonist;
            NameColor = RoleColors.Arsonist;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void Setup()
        {
            triggerArsonistWin = false;
            winner = null;
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            arsonistCooldown = CustomOption.Create(291, "arsonistCooldown", 12.5f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            arsonistDuration = CustomOption.Create(292, "arsonistDuration", 3f, 0f, 10f, 1f, options, format: "unitSeconds");
        }

        public override bool DidWin(GameOverReason gameOverReason)
        {
            return !Player.Data.IsDead && dousedEveryoneAlive() && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        }

        public override bool InitButtons()
        {
            if (!DestroyableSingleton<HudManager>.InstanceExists) return false;
            if (douseButton != null && igniteButton != null) return true;

            douseButton = new CustomButton(
                () => {
                    if (currentTarget != null)
                    {
                        douseTarget = currentTarget;
                        douseButton.HasEffect = true;
                    }
                },
                () => { return !dousedEveryone && !Player.Data.IsDead; },
                () => {
                    if (douseButton.isEffectActive && douseTarget != currentTarget)
                    {
                        douseTarget = null;
                        douseButton.Timer = 0f;
                        douseButton.isEffectActive = false;
                    }

                    return Player.CanMove && (dousedEveryone || currentTarget != null);
                },
                () => {
                    douseButton.Timer = douseButton.MaxTimer;
                    douseButton.isEffectActive = false;
                    douseTarget = null;
                    updateStatus();
                },
                getDouseSprite(),
                new Vector3(-1.85f, -1.2f, 0.0f),
                DestroyableSingleton<HudManager>.Instance.KillButton,
                null,
                KeyCode.F,
                true,
                Arsonist.duration,
                () => {
                    if (douseTarget != null) dousedPlayers.Add(douseTarget);
                    douseTarget = null;
                    updateStatus();
                    douseButton.Timer = dousedEveryone ? 0 : douseButton.MaxTimer;

                    foreach (PlayerControl p in dousedPlayers)
                    {
                        if (MapOptions.playerIcons.ContainsKey(p.PlayerId))
                        {
                            MapOptions.playerIcons[p.PlayerId].setSemiTransparent(false);
                        }
                    }
                }
            );
            douseButton.MaxTimer = cooldown;
            douseButton.EffectDuration = duration;
            douseButton.buttonText = ModTranslation.getString("DouseText");

            igniteButton = new CustomButton(
                () => {
                    if (dousedEveryone)
                    {
                        MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ArsonistWin, Hazel.SendOption.Reliable, -1);
                        winWriter.Write(Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.arsonistWin(Player.PlayerId);
                    }
                },
                () => { return dousedEveryone && !Player.Data.IsDead; },
                () => { return dousedEveryone && PlayerControl.LocalPlayer.CanMove; },
                () => { },
                getIgniteSprite(),
                new Vector3(-1.85f, -1.2f, 0.0f),
                DestroyableSingleton<HudManager>.Instance.KillButton,
                null,
                KeyCode.F
            );
            igniteButton.MaxTimer = igniteButton.Timer = 0f;
            igniteButton.buttonText = ModTranslation.getString("IgniteText");

            return true;
        }

        public override void SetTarget()
        {
            List<PlayerControl> untargetables;
            if (douseTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != douseTarget.PlayerId).ToList();
            else
                untargetables = dousedPlayers;
            currentTarget = setTarget(untargetablePlayers: untargetables);
            if (currentTarget != null) setPlayerOutline(currentTarget, RoleColors.Arsonist);
        }

        public override void _RoleUpdate()
        {
            SetTarget();
        }

        public static Sprite getIgniteSprite()
        {
            if (igniteSprite) return igniteSprite;
            igniteSprite = ModTranslation.getImage("IgniteButton", 115f);
            return igniteSprite;
        }

        public static Sprite getDouseSprite()
        {
            if (douseSprite) return douseSprite;
            douseSprite = ModTranslation.getImage("DouseButton", 115f);
            return douseSprite;
        }

        public void updateIcons()
        {
            if (PlayerControl.LocalPlayer != Player) return;

            int visibleCounter = 0;
            Vector3 bottomLeft = DestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition;
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

        public void updateStatus()
        {
            dousedEveryone = dousedEveryoneAlive();
        }

        public bool dousedEveryoneAlive()
        {
            return PlayerControl.AllPlayerControls.ToArray().All(x => { return x == Player || x.Data.IsDead || x.Data.Disconnected || x.isRole(CustomRoleTypes.GM) || dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
        }
    }
}