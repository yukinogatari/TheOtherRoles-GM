using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Vulture : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Vulture;

        public static CustomOptionBlank options;
        public static CustomOption vultureCooldown;
        public static CustomOption vultureNumberToWin;
        public static CustomOption vultureCanUseVents;
        public static CustomOption vultureShowArrows;

        public static float cooldown { get { return vultureCooldown.getFloat(); } }
        public static int numberToWin { get { return Mathf.RoundToInt(vultureNumberToWin.getFloat()); } }
        public static bool canUseVents { get { return vultureCanUseVents.getBool(); } }
        public static bool showArrows { get { return vultureShowArrows.getBool(); } }

        public int eatenBodies = 0;
        public List<Arrow> localArrows = new List<Arrow>();

        public CustomButton eatButton;
        public TMPro.TMP_Text vultureNumCorpsesText;

        public static bool triggerVultureWin = false;
        public static PlayerControl winner;
        private static Sprite buttonSprite;

        public Vulture() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.Vulture;
            NameColor = RoleColors.Vulture;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        ~Vulture()
        {
            foreach (Arrow arrow in localArrows) UnityEngine.Object.Destroy(arrow.arrow);
            UnityEngine.Object.Destroy(vultureNumCorpsesText);
        }

        public static void Setup()
        {
            triggerVultureWin = false;
            winner = null;
        }

        public override bool InitButtons()
        {
            if (!DestroyableSingleton<HudManager>.InstanceExists) return false;
            if (eatButton != null) return true;

            // Vulture Eat
            eatButton = new CustomButton(
                () => {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.cleanBody(playerInfo.PlayerId);

                                    eatButton.Timer = eatButton.MaxTimer;
                                    eatenBodies++;
                                    break;
                                }
                            }
                        }
                    }
                    if (eatenBodies >= numberToWin)
                    {
                        MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VultureWin, Hazel.SendOption.Reliable, -1);
                        winWriter.Write(Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.vultureWin(Player.PlayerId);
                        return;
                    }
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => {
                    if (vultureNumCorpsesText != null)
                        vultureNumCorpsesText.text = String.Format(ModTranslation.getString("vultureCorpses"), numberToWin - eatenBodies);

                    return DestroyableSingleton<HudManager>.Instance.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove;
                },
                () => { eatButton.Timer = eatButton.MaxTimer; },
                getButtonSprite(),
                new Vector3(-1.85f, -1.2f, 0.0f),
                DestroyableSingleton<HudManager>.Instance.KillButton,
                null,
                KeyCode.F
            );
            eatButton.MaxTimer = cooldown;
            eatButton.buttonText = ModTranslation.getString("VultureText");

            vultureNumCorpsesText = GameObject.Instantiate(eatButton.actionButton.cooldownTimerText, eatButton.actionButton.cooldownTimerText.transform.parent);
            vultureNumCorpsesText.text = "";
            vultureNumCorpsesText.enableWordWrapping = false;
            vultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
            vultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);

            return true;
        }

        public override void _RoleUpdate()
        {
            if (localArrows == null || !showArrows) return;
            if (Player.Data.IsDead)
            {
                foreach (Arrow arrow in localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                localArrows = new List<Arrow>();
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = localArrows.Count != deadBodies.Count();
            int index = 0;

            if (arrowUpdate)
            {
                foreach (Arrow arrow in localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                localArrows = new List<Arrow>();
            }

            foreach (DeadBody db in deadBodies)
            {
                if (arrowUpdate)
                {
                    localArrows.Add(new Arrow(RoleColors.Vulture));
                    localArrows[index].arrow.SetActive(true);
                }
                if (localArrows[index] != null) localArrows[index].Update(db.transform.position);
                index++;
            }
        }

        public override bool DidWin(GameOverReason gameOverReason)
        {
            return !Player.Data.IsDead && triggerVultureWin && Player == winner && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            vultureCooldown = CustomOption.Create(341, "vultureCooldown", 15f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            vultureNumberToWin = CustomOption.Create(342, "vultureNumberToWin", 4f, 1f, 12f, 1f, options);
            vultureCanUseVents = CustomOption.Create(343, "vultureCanUseVents", true, options);
            vultureShowArrows = CustomOption.Create(344, "vultureShowArrows", true, options);
        }

        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VultureButton.png", 115f);
            return buttonSprite;
        }
    }
}