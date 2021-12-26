using System.Linq;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public class Ninja : RoleBase<Ninja> {

        private static CustomButton ninjaButton;

        public static Color color = Palette.ImpostorRed;

        public static float stealthCooldown = 10f;
        public static float stealthDuration = 10f;
        public static float killPenalty = 10f;
        public static float speedBonus = 1.25f;
        public static float fadeTime = 0.5f;
        public static bool canUseVents = false;
        public static bool canBeTargeted = true;

        public bool penalized = false;
        public bool stealthed = false;
        public DateTime stealthedAt = DateTime.UtcNow;

        public Ninja()
        {
            RoleType = roleId = RoleId.Ninja;
            penalized = false;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
        }

        public override void OnMeetingStart()
        {
            stealthed = false;
            ninjaButton.isEffectActive = false;
            ninjaButton.Timer = ninjaButton.MaxTimer = Ninja.stealthCooldown;
        }

        public override void OnMeetingEnd()
        {
            if (player == PlayerControl.LocalPlayer)
            {
                if (penalized)
                {
                    player.SetKillTimerUnchecked(PlayerControl.GameOptions.KillCooldown + killPenalty);
                } else
                {
                    player.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                }
            }
        }

        public static bool isStealthed(PlayerControl player)
        {
            if (isRole(player))
            {
                Ninja n = players.First(x => x.player == player);
                return n.stealthed;
            }
            return false;
        }

        public static float stealthFade(PlayerControl player)
        {
            if (isRole(player) && fadeTime > 0f)
            {
                Ninja n = players.First(x => x.player == player);
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - n.stealthedAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static bool isPenalized(PlayerControl player)
        {
            if (isRole(player))
            {
                Ninja n = players.First(x => x.player == player);
                return n.penalized;
            }
            return false;
        }

        public static void setStealthed(PlayerControl player, bool stealthed = true)
        {
            if (isRole(player))
            {
                Ninja n = players.First(x => x.player == player);
                n.stealthed = stealthed;
                n.stealthedAt = DateTime.UtcNow;
            }
        }

        public override void OnKill(PlayerControl target)
        {
            penalized = stealthed;
            float penalty = penalized ? killPenalty : 0f;
            player.SetKillTimerUnchecked(PlayerControl.GameOptions.KillCooldown + penalty);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NinjaButton.png", 115f);
            return buttonSprite;
        }

        public static void MakeButtons(HudManager hm)
        {
            // Ninja stealth
            ninjaButton = new CustomButton(
                () => {
                    if (ninjaButton.isEffectActive)
                    {
                        ninjaButton.Timer = 0;
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NinjaStealth, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.ninjaStealth(PlayerControl.LocalPlayer.PlayerId, true);
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleId.Ninja) && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => {
                    if (ninjaButton.isEffectActive)
                    {
                        ninjaButton.buttonText = ModTranslation.getString("NinjaUnstealthText");
                    }
                    else
                    {
                        ninjaButton.buttonText = ModTranslation.getString("NinjaText");
                    }
                    return PlayerControl.LocalPlayer.CanMove;
                },
                () => {
                    ninjaButton.Timer = ninjaButton.MaxTimer = Ninja.stealthCooldown;
                },
                Ninja.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                Ninja.stealthDuration,
                () => {
                    ninjaButton.Timer = ninjaButton.MaxTimer = Ninja.stealthCooldown;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NinjaStealth, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.ninjaStealth(PlayerControl.LocalPlayer.PlayerId, false);

                    PlayerControl.LocalPlayer.SetKillTimerUnchecked(Math.Max(PlayerControl.LocalPlayer.killTimer, Ninja.killPenalty));
                }
            );
            ninjaButton.buttonText = ModTranslation.getString("NinjaText");
            ninjaButton.effectCancellable = true;
        }

        public static void SetButtonCooldowns()
        {
            ninjaButton.MaxTimer = Ninja.stealthCooldown;
        }

        public static void clearAndReload()
        {
            players = new List<Ninja>();
            stealthCooldown = CustomOptionHolder.ninjaStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.ninjaStealthDuration.getFloat();
            killPenalty = CustomOptionHolder.ninjaKillPenalty.getFloat();
            speedBonus = CustomOptionHolder.ninjaSpeedBonus.getFloat() / 100f;
            fadeTime = CustomOptionHolder.ninjaFadeTime.getFloat();
            canUseVents = CustomOptionHolder.ninjaCanVent.getBool();
            canBeTargeted = CustomOptionHolder.ninjaCanBeTargeted.getBool();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsNinjaPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.AmOwner && __instance.myPlayer.CanMove && GameData.Instance && isStealthed(__instance.myPlayer))
                {
                    __instance.body.velocity *= speedBonus;
                }

                if (isRole(__instance.myPlayer))
                {
                    var ninja = __instance.myPlayer;
                    if (ninja == null) return;

                    var opacity = (PlayerControl.LocalPlayer.Data.Role.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead) ? 0.1f : 0.0f;

                    if (isStealthed(ninja))
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade(ninja));
                        ninja.myRend.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade(ninja));
                    }

                    // Sometimes it just doesn't work?
                    try
                    {
                        var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
                        if (ninja.MyPhysics?.rend != null)
                        {
                            ninja.MyPhysics.rend.color = color;
                        }
                        ninja.MyPhysics?.Skin?.layer?.material?.SetColor("_Color", color);
                        ninja.HatRenderer?.BackLayer?.material?.SetColor("_Color", color);
                        ninja.HatRenderer?.FrontLayer?.material?.SetColor("_Color", color);
                        ninja.CurrentPet?.rend?.material?.SetColor("_Color", color);
                        ninja.CurrentPet?.shadowRend?.material?.SetColor("_Color", color);
                        if (ninja.VisorSlot != null)
                        {
                            ninja.VisorSlot.color = color;
                        }
                    }
                    catch { }
                }
            }
        }
    }
}