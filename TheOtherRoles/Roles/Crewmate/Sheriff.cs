using HarmonyLib;
using Hazel;
using System;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using UnityEngine;
using UnityEngine.UI;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Sheriff : CustomCrewmateRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Sheriff;

        public static CustomOptionBlank options;
        public static CustomOption sheriffCooldown;
        public static CustomOption sheriffNumShots;
        public static CustomOption sheriffCanKillNeutrals;
        public static CustomOption sheriffMisfireKillsTarget;

        public int numShots = 2;
        public static int maxShots { get { return (int)sheriffNumShots.getFloat(); } }
        public static float cooldown { get { return sheriffCooldown.getFloat(); } }

        public static bool canKillNeutrals { get { return sheriffCanKillNeutrals.getBool(); } }
        public static bool misfireKillsTarget { get { return sheriffMisfireKillsTarget.getBool(); } }
        public static bool spyCanDieToSheriff { get { return Spy.spyCanDieToSheriff.getBool(); } }
        public static bool madmateCanDieToSheriff { get { return Madmate.madmateCanDieToSheriff.getBool(); } }

        public CustomButton killButton;
        public TMPro.TMP_Text sheriffNumShotsText;
        public PlayerControl currentTarget;

        public Sheriff() : base()
        {
            NameColor = RoleColors.Sheriff;
            CanUseKillButton = true;
            MaxCount = 15;
        }

        ~Sheriff()
        {
            UnityEngine.Object.Destroy(sheriffNumShotsText);
        }

        public override void Init()
        {
            base.Init();
            numShots = maxShots;
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            sheriffCooldown = CustomOption.Create(101, "sheriffCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            sheriffNumShots = CustomOption.Create(103, "sheriffNumShots", 2f, 1f, 15f, 1f, options, format: "unitShots");
            sheriffMisfireKillsTarget = CustomOption.Create(104, "sheriffMisfireKillsTarget", false, options);
            sheriffCanKillNeutrals = CustomOption.Create(102, "sheriffCanKillNeutrals", false, options);
        }

        public override bool InitButtons()
        {
            if (!DestroyableSingleton<HudManager>.InstanceExists) return false;
            if (killButton != null) return true;

            killButton = new CustomButton(
                () =>
                {
                    if (numShots <= 0 || currentTarget == null)
                    {
                        return;
                    }

                    bool misfire = false;
                    byte sheriffId = Player.PlayerId;
                    byte targetId = currentTarget.PlayerId;

                    if (currentTarget.hasModifier(RoleModifierTypes.MedicShield))
                    {
                        MessageWriter attemptWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                        attemptWriter.Write(sheriffId);
                        attemptWriter.Write(targetId);
                        AmongUsClient.Instance.FinishRpcImmediately(attemptWriter);
                        RPCProcedure.shieldedMurderAttempt(sheriffId, targetId);
                        return;
                    }

                    if ((currentTarget.Data.Role.IsImpostor && !(currentTarget.isRole(CustomRoleTypes.Mini) && currentTarget.role<Mini>()?.isGrownUp != true)) ||
                        (spyCanDieToSheriff && currentTarget.isRole(CustomRoleTypes.Spy)) ||
                        (madmateCanDieToSheriff && currentTarget.isRole(CustomRoleTypes.Madmate)) ||
                        (canKillNeutrals && currentTarget.isNeutral()) ||
                        (currentTarget.isRole(CustomRoleTypes.Jackal) || currentTarget.isRole(CustomRoleTypes.Sidekick)))
                    {
                        misfire = false;
                    }
                    else
                    {
                        misfire = true;
                    }

                    MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SheriffKill, Hazel.SendOption.Reliable, -1);
                    killWriter.Write(sheriffId);
                    killWriter.Write(targetId);
                    killWriter.Write(misfire);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCProcedure.sheriffKill(sheriffId, targetId, misfire);

                    HudManager.Instance.KillButton.SetCoolDown(cooldown, cooldown);
                    currentTarget = null;
                    numShots--;
                },
                () =>
                {
                    return numShots > 0 && !Player.Data.IsDead;
                },
                () =>
                {
                    if (sheriffNumShotsText != null)
                    {
                        if (numShots > 0)
                            sheriffNumShotsText.text = String.Format(ModTranslation.getString("sheriffShots"), numShots);
                        else
                            sheriffNumShotsText.text = "";
                    }
                    return currentTarget != null && PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {
                    killButton.Timer = killButton.MaxTimer;
                },
                null,
                new Vector3(-0.10f, -0.20f, 0.0f),
                DestroyableSingleton<HudManager>.Instance.KillButton,
                null,
                KeyCode.Q
            );
            killButton.MaxTimer = cooldown;

            sheriffNumShotsText = GameObject.Instantiate(killButton.actionButton.cooldownTimerText, killButton.actionButton.cooldownTimerText.transform.parent);
            sheriffNumShotsText.text = "";
            sheriffNumShotsText.enableWordWrapping = false;
            sheriffNumShotsText.transform.localScale = Vector3.one * 0.5f;
            sheriffNumShotsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            return true;
        }

        public override void SetTarget()
        {
            currentTarget = setTarget();
            setPlayerOutline(currentTarget, RoleColors.Sheriff);
        }

        public override void _RoleUpdate()
        {
            SetTarget();

            bool hasShots = numShots > 0;
            if (sheriffNumShotsText != null)
            {
                sheriffNumShotsText.enabled = hasShots;
                sheriffNumShotsText.gameObject.SetActive(true);
                sheriffNumShotsText.text = hasShots ? String.Format(ModTranslation.getString("sheriffShots"), numShots) : "";
            }
        }
    }
}

/*
    // Sheriff Kill
    sheriffKillButton = new CustomButton(
        () => {
            if (Sheriff.numShots <= 0)
            {
                return;
            }

            if (Medic.shielded != null && Medic.shielded == Sheriff.currentTarget) {
                MessageWriter attemptWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(attemptWriter);
                RPCProcedure.shieldedMurderAttempt();
                return;
            }

            bool misfire = false;
            byte targetId = Sheriff.currentTarget.PlayerId; ;
            if ((Sheriff.currentTarget.Data.Role.IsImpostor && (Sheriff.currentTarget != Mini.mini || Mini.isGrownUp())) ||
                (Sheriff.spyCanDieToSheriff && Spy.spy == Sheriff.currentTarget) ||
                (Sheriff.madmateCanDieToSheriff && Madmate.madmate == Sheriff.currentTarget) ||
                (Sheriff.canKillNeutrals && Sheriff.currentTarget.isNeutral()) ||
                (Jackal.jackal == Sheriff.currentTarget || Sidekick.sidekick == Sheriff.currentTarget)) {
                //targetId = Sheriff.currentTarget.PlayerId;
                misfire = false;
            }
            else {
                //targetId = PlayerControl.LocalPlayer.PlayerId;
                misfire = true;
            }
            MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SheriffKill, Hazel.SendOption.Reliable, -1);
            killWriter.Write(targetId);
            killWriter.Write(misfire);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.sheriffKill(targetId, misfire);

            sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
            Sheriff.currentTarget = null;
            Sheriff.numShots--;
        },
        () => { return Sheriff.sheriff != null && Sheriff.sheriff == PlayerControl.LocalPlayer && Sheriff.numShots > 0 && !PlayerControl.LocalPlayer.Data.IsDead; },
        () => {
            if (sheriffNumShotsText != null)
            {
                if (Sheriff.numShots > 0)
                    sheriffNumShotsText.text = String.Format(ModTranslation.getString("sheriffShots"), Sheriff.numShots);
                else
                    sheriffNumShotsText.text = "";
            }
            return Sheriff.currentTarget && PlayerControl.LocalPlayer.CanMove;
        },
        () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer; },
        __instance.KillButton.graphic.sprite,
        new Vector3(-1.3f, 0, 0),
        __instance,
        KeyCode.Q
    );

    sheriffNumShotsText = GameObject.Instantiate(sheriffKillButton.killButton.cooldownTimerText, sheriffKillButton.killButton.cooldownTimerText.transform.parent);
    sheriffNumShotsText.text = "";
    sheriffNumShotsText.enableWordWrapping = false;
    sheriffNumShotsText.transform.localScale = Vector3.one * 0.5f;
    sheriffNumShotsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
*/