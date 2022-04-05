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

        public static float stealthCooldown { get { return CustomOptionHolder.ninjaStealthCooldown.getFloat(); } }
        public static float stealthDuration { get { return CustomOptionHolder.ninjaStealthDuration.getFloat(); } }
        public static float killPenalty { get { return CustomOptionHolder.ninjaKillPenalty.getFloat(); } }
        public static float speedBonus { get { return CustomOptionHolder.ninjaSpeedBonus.getFloat() / 100f; } }
        public static float fadeTime { get { return CustomOptionHolder.ninjaFadeTime.getFloat(); } }
        public static bool canUseVents { get { return CustomOptionHolder.ninjaCanVent.getBool(); } }
        public static bool canBeTargeted { get { return CustomOptionHolder.ninjaCanBeTargeted.getBool(); } }

        public bool penalized = false;
        public bool stealthed = false;
        public DateTime stealthedAt = DateTime.UtcNow;

        public Ninja()
        {
            RoleType = roleId = RoleType.Ninja;
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
                }
                else
                {
                    player.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                }
            }
        }

        public override void ResetRole()
        {
            penalized = false;
            stealthed = false;
            setOpacity(player, 1.0f);
            ninjaButton.isEffectActive = false;
            ninjaButton.Timer = ninjaButton.MaxTimer = Ninja.stealthCooldown;
        }

        public override void FixedUpdate() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static bool isStealthed(PlayerControl player)
        {
            if (isRole(player) && player.isAlive())
            {
                Ninja n = players.First(x => x.player == player);
                return n.stealthed;
            }
            return false;
        }

        public static float stealthFade(PlayerControl player)
        {
            if (isRole(player) && fadeTime > 0f && player.isAlive())
            {
                Ninja n = players.First(x => x.player == player);
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - n.stealthedAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static bool isPenalized(PlayerControl player)
        {
            if (isRole(player) && player.isAlive())
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
            if (PlayerControl.LocalPlayer == player)
                player.SetKillTimerUnchecked(PlayerControl.GameOptions.KillCooldown + penalty);
        }

        public override void OnDeath(PlayerControl killer)
        {
            stealthed = false;
            ninjaButton.isEffectActive = false;
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
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Ninja) && !PlayerControl.LocalPlayer.Data.IsDead; },
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

        public static void Clear()
        {
            players = new List<Ninja>();
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            // Sometimes it just doesn't work?
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (player.MyPhysics?.rend != null)
                    player.MyPhysics.rend.color = color;

                if (player.MyPhysics?.Skin?.layer != null)
                    player.MyPhysics.Skin.layer.color = color;

                if (player.HatRenderer != null)
                    player.HatRenderer.color = color;

                if (player.CurrentPet?.rend != null)
                    player.CurrentPet.rend.color = color;

                if (player.CurrentPet?.shadowRend != null)
                    player.CurrentPet.shadowRend.color = color;

                if (player.VisorSlot != null)
                    player.VisorSlot.color = color;
            }
            catch { }
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
                    if (ninja == null || ninja.isDead()) return;

                    bool canSee = 
                        PlayerControl.LocalPlayer.isImpostor() ||
                        PlayerControl.LocalPlayer.isDead() ||
                        (Lighter.canSeeNinja && PlayerControl.LocalPlayer.isRole(RoleType.Lighter) && Lighter.isLightActive(PlayerControl.LocalPlayer));

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (isStealthed(ninja))
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade(ninja));
                        ninja.MyRend.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade(ninja));
                    }

                    setOpacity(ninja, opacity);
                }
            }
        }
    }
}