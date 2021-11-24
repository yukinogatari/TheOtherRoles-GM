using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Camouflager : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Camouflager;
        public static GameData.PlayerOutfit camouflage;

        public static CustomOptionBlank options;
        public static CustomOption camouflagerCooldown;
        public static CustomOption camouflagerDuration;
        public static CustomOption camouflagerRandomColors;

        public static float cooldown { get { return camouflagerCooldown.getFloat(); } }
        public static float duration { get { return camouflagerDuration.getFloat(); } }
        public static bool randomColors { get { return camouflagerRandomColors.getBool(); } }

        public Camouflager() : base()
        {
            NameColor = RoleColors.Camouflager;
            MaxCount = 15;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            camouflagerCooldown = CustomOption.Create(31, "camouflagerCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            camouflagerDuration = CustomOption.Create(32, "camouflagerDuration", 10f, 1f, 20f, 0.5f, options, format: "unitSeconds");
            camouflagerRandomColors = CustomOption.Create(33, "camouflagerRandomColors", false, options);
        }

        public static void Setup()
        {
            camouflage = new GameData.PlayerOutfit();
            camouflage.ColorId = 6;
            camouflage.HatId = "";
            camouflage.PetId = "";
            camouflage.SkinId = "";
            camouflage.VisorId = "";
            camouflage.PlayerName = "";
        }

        public static void resetCamouflage()
        {
            if (randomColors)
            {
                camouflage.ColorId = TheOtherRoles.rnd.Next(0, Palette.PlayerColors.Length);
            }
            else
            {
                camouflage.ColorId = 6;
            }

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                p.Data.SetOutfit(PlayerOutfitType.Shapeshifted, camouflage);
            }
        }
    }
}