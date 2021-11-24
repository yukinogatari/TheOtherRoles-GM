using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Mafia : CustomImpostorRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Mafia;
        public PlayerControl godfather;
        public PlayerControl mafioso;
        public PlayerControl janitor;

        public static CustomOptionBlank options;
        public static CustomOption janitorCooldown;

        public static float cooldown { get { return janitorCooldown.getFloat(); } }

        public Mafia() : base()
        {
            NameColor = RoleColors.Mafia;
            MaxCount = 1;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            janitorCooldown = CustomOption.Create(11, "janitorCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
        }

        public void setMafia(PlayerControl godfather, PlayerControl mafioso, PlayerControl janitor)
        {
            this.godfather = godfather;
            this.mafioso = mafioso;
            this.janitor = janitor;
        }
    }

    class Godfather : Mafia
    {

    }

    class Mafioso : Mafia
    {

    }

    class Janitor : Mafia
    {

    }
}