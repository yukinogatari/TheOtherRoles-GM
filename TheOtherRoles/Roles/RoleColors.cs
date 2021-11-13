
using UnityEngine;

namespace TheOtherRoles.Roles
{
    public static class RoleColors
    {
        // Crewmate colors
        public static Color Crewmate = Palette.White;
        public static Color Shifter = new Color32(102, 102, 102, byte.MaxValue);
        public static Color Mayor = new Color32(32, 77, 66, byte.MaxValue);
        public static Color Sheriff = new Color32(248, 205, 70, byte.MaxValue);
        public static Color Lighter = new Color32(238, 229, 190, byte.MaxValue);
        public static Color Detective = new Color32(45, 106, 165, byte.MaxValue);
        public static Color TimeMaster = new Color32(112, 142, 239, byte.MaxValue);
        public static Color Medic = new Color32(126, 251, 194, byte.MaxValue);
        public static Color Swapper = new Color32(134, 55, 86, byte.MaxValue);
        public static Color Seer = new Color32(97, 178, 108, byte.MaxValue);
        public static Color Hacker = new Color32(117, 250, 76, byte.MaxValue);
        public static Color Tracker = new Color32(100, 58, 220, byte.MaxValue);
        public static Color Snitch = new Color32(184, 251, 79, byte.MaxValue);
        public static Color Spy = Palette.ImpostorRed;
        public static Color SecurityGuard = new Color32(195, 178, 95, byte.MaxValue);
        public static Color Bait = new Color32(0, 247, 255, byte.MaxValue);
        public static Color Medium = new Color32(98, 120, 115, byte.MaxValue);

        // Impostor colors (lol)
        public static Color Impostor = Palette.ImpostorRed;
        public static Color Mafia = Palette.ImpostorRed;
        public static Color Godfather = Palette.ImpostorRed;
        public static Color Mafioso = Palette.ImpostorRed;
        public static Color Janitor = Palette.ImpostorRed;
        public static Color Camouflager = Palette.ImpostorRed;
        public static Color Vampire = Palette.ImpostorRed;
        public static Color Eraser = Palette.ImpostorRed;
        public static Color Trickster = Palette.ImpostorRed;
        public static Color Cleaner = Palette.ImpostorRed;
        public static Color Warlock = Palette.ImpostorRed;
        public static Color BountyHunter = Palette.ImpostorRed;
        public static Color Madmate = Palette.ImpostorRed;

        // Other team colors
        public static Color Mini = Color.white;
        public static Color NiceMini = Palette.White;
        public static Color EvilMini = Palette.ImpostorRed;
        public static Color Lovers = new Color32(232, 57, 185, byte.MaxValue);
        public static Color Guesser = new Color32(255, 255, 0, byte.MaxValue);
        public static Color NiceGuesser = new Color32(255, 255, 0, byte.MaxValue);
        public static Color EvilGuesser = Palette.ImpostorRed;
        public static Color Jester = new Color32(236, 98, 165, byte.MaxValue);
        public static Color Arsonist = new Color32(238, 112, 46, byte.MaxValue);
        public static Color Jackal = new Color32(0, 180, 235, byte.MaxValue);
        public static Color Sidekick = new Color32(0, 180, 235, byte.MaxValue);
        public static Color Opportunist = new Color32(0, 255, 00, byte.MaxValue);
        public static Color Vulture = new Color32(139, 69, 19, byte.MaxValue);

        public static Color GM = new Color32(255, 91, 112, byte.MaxValue);

        // etc
        public static Color None = Palette.White;
    }

    public static class TeamColors
    {
        public static Color Crewmate = RoleColors.Crewmate;
        public static Color Impostor = RoleColors.Impostor;
        public static Color Jackal = RoleColors.Jackal;
        public static Color Jester = RoleColors.Jester;
        public static Color Arsonist = RoleColors.Arsonist;
        public static Color Lovers = RoleColors.Lovers;
        public static Color Opportunist = RoleColors.Opportunist;
        public static Color Vulture = RoleColors.Vulture;
    }
}