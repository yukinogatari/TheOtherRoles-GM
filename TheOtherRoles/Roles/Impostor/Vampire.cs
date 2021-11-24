using HarmonyLib;
using Hazel;
using System;
using TheOtherRoles.Objects;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using UnityEngine;
using System.Collections.Generic;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Vampire : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Vampire;

        public static CustomOptionBlank options;
        public static CustomOption vampireKillDelay;
        public static CustomOption vampireCooldown;
        public static CustomOption vampireCanKillNearGarlics;

        public static float delay { get { return vampireKillDelay.getFloat(); } }
        public static float cooldown { get { return vampireCooldown.getFloat(); } }
        public static bool canKillNearGarlics { get { return vampireCanKillNearGarlics.getBool(); } }
        public static bool garlicsActive { get { return RoleInfo.vampire.enabled; } }

        public bool targetNearGarlic;

        public PlayerControl bitten;
        public PlayerControl currentTarget;
        public CustomButton killButton;

        public Vampire() : base()
        {
            NameColor = RoleColors.Vampire;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public override bool InitButtons()
        {
            if (!DestroyableSingleton<HudManager>.InstanceExists) return false;
            if (killButton != null) return true;

            killButton = new CustomButton(
                () =>
                {
                    if (Helpers.checkMurderAttempt(Player, currentTarget))
                    {
                        if (targetNearGarlic)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                            writer.Write(Player.PlayerId);
                            writer.Write(currentTarget.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.uncheckedMurderPlayer(Player.PlayerId, currentTarget.PlayerId);

                            killButton.HasEffect = false; // Block effect on this click
                            killButton.Timer = killButton.MaxTimer;
                        }
                        else
                        {
                            bitten = currentTarget;
                            // Notify players about bitten
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                            writer.Write(Player.PlayerId);
                            writer.Write(bitten.PlayerId);
                            writer.Write(0);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.vampireSetBitten(Player.PlayerId, bitten.PlayerId, 0);

                            DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(delay, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.checkMurderAttemptAndKill(Player, bitten);
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                                    writer.Write(Player.PlayerId);
                                    writer.Write(byte.MaxValue);
                                    writer.Write(byte.MaxValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.vampireSetBitten(Player.PlayerId, byte.MaxValue, byte.MaxValue);
                                }
                            })));

                            killButton.HasEffect = true; // Trigger effect on this click
                        }
                    }
                    else
                    {
                        killButton.HasEffect = false; // Block effect if no action was fired
                    }
                },
                () => { return !Player.Data.IsDead; },
                () =>
                {
                    return currentTarget != null && Player.CanMove && (!targetNearGarlic || canKillNearGarlics);
                },
                () =>
                {
                    killButton.Timer = killButton.MaxTimer;
                    killButton.isEffectActive = false;
                    killButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                getButtonSprite(),
                new Vector3(0.0f, 0.0f, 0.0f),
                DestroyableSingleton<HudManager>.Instance.KillButton,
                null,
                KeyCode.Q,
                false,
                0f,
                () =>
                {
                    killButton.Timer = killButton.MaxTimer;
                }
            );
            killButton.MaxTimer = cooldown;
            killButton.EffectDuration = delay;

            return true;
        }

        public override void SetTarget()
        {
            PlayerControl target = null;
            if (RoleHelpers.roleExists(CustomRoleTypes.Spy))
            {
                if (Spy.impostorsCanKillAnyone)
                {
                    target = setTarget(false, true);
                }
                else
                {
                    target = setTarget(true, true, RoleHelpers.getPlayersWithRole(CustomRoleTypes.Spy));
                }
            }
            else
            {
                target = setTarget(true, true);
            }

            targetNearGarlic = false;
            if (target != null)
            {
                foreach (Garlic garlic in Garlic.garlics)
                {
                    if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.91f)
                    {
                        targetNearGarlic = true;
                    }
                }
            }

            currentTarget = target;
            setPlayerOutline(currentTarget, RoleColors.Vampire);
        }

        public override void _RoleUpdate()
        {
            SetTarget();

            if (killButton != null)
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                if (targetNearGarlic && canKillNearGarlics)
                {
                    //killButton.Sprite = DestroyableSingleton<TranslationController>.Instance.GetImage(ImageNames.KillButton);
                    killButton.Sprite = DestroyableSingleton<HudManager>.Instance.KillButton.graphic.sprite;
                    killButton.buttonText = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel);
                }
                else
                {
                    killButton.Sprite = getButtonSprite();
                    killButton.buttonText = ModTranslation.getString("VampireText");
                }
            }
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            vampireKillDelay = CustomOption.Create(41, "vampireKillDelay", 10f, 1f, 20f, 1f, options, format: "unitSeconds");
            vampireCooldown = CustomOption.Create(42, "vampireCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            vampireCanKillNearGarlics = CustomOption.Create(43, "vampireCanKillNearGarlics", true, options);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = ModTranslation.getImage("VampireButton", 115f);
            return buttonSprite;
        }

        private static Sprite garlicButtonSprite;
        public static Sprite getGarlicButtonSprite()
        {
            if (garlicButtonSprite) return garlicButtonSprite;
            garlicButtonSprite = ModTranslation.getImage("GarlicButton", 115f);
            return garlicButtonSprite;
        }
    }
}