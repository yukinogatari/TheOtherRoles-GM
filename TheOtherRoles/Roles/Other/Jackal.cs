using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    [Harmony]
    class Jackal : CustomRole
    {
        public static CustomRoleTypes RoleType = CustomRoleTypes.Jackal;

        public static CustomOptionBlank options;
        public static CustomOption jackalKillCooldown;
        public static CustomOption jackalCreateSidekickCooldown;
        public static CustomOption jackalCanUseVents;
        public static CustomOption jackalCanCreateSidekick;
        public static CustomOption sidekickPromotesToJackal;
        public static CustomOption sidekickCanKill;
        public static CustomOption sidekickCanUseVents;
        public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
        public static CustomOption jackalCanCreateSidekickFromImpostor;
        public static CustomOption jackalAndSidekickHaveImpostorVision;
        public static CustomOption jackalCanSeeEngineerVent;

        public static float cooldown { get { return jackalKillCooldown.getFloat(); } }
        public static float createSidekickCooldown { get { return jackalCreateSidekickCooldown.getFloat(); } }
        public static bool canUseVents { get { return jackalCanUseVents.getBool(); } }
        public static bool canCreateSidekick { get { return jackalCanCreateSidekick.getBool(); } }
        public static bool promotedJackalCanCreateSidekick { get { return jackalPromotedFromSidekickCanCreateSidekick.getBool(); } }
        public static bool canCreateSidekickFromImpostor { get { return jackalCanCreateSidekickFromImpostor.getBool(); } }
        public static bool hasImpostorVision { get { return jackalAndSidekickHaveImpostorVision.getBool(); } }
        public static bool canSeeEngineerVent { get { return jackalCanSeeEngineerVent.getBool(); } }

        public static Sprite buttonSprite;

        public PlayerControl parent;
        public PlayerControl sidekick;
        public PlayerControl fakeSidekick;

        public Jackal() : base()
        {
            TeamType = (RoleTeamTypes)CustomRoleTeamTypes.Jackal;
            NameColor = RoleColors.Jackal;
            MaxCount = 1;
            //Ability.Image = TheOtherRoles.getBlankIcon();
        }

        public static void InitSettings()
        {
            options = new CustomOptionBlank(null);
            jackalKillCooldown = CustomOption.Create(221, "jackalKillCooldown", 30f, 2.5f, 60f, 2.5f, options, format: "unitSeconds");
            jackalCanUseVents = CustomOption.Create(223, "jackalCanUseVents", true, options);
            jackalAndSidekickHaveImpostorVision = CustomOption.Create(430, "jackalAndSidekickHaveImpostorVision", false, options);
            jackalCanCreateSidekick = CustomOption.Create(224, "jackalCanCreateSidekick", false, options);
            jackalCreateSidekickCooldown = CustomOption.Create(222, "jackalCreateSidekickCooldown", 30f, 2.5f, 60f, 2.5f, jackalCanCreateSidekick, format: "unitSeconds");
            sidekickPromotesToJackal = CustomOption.Create(225, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
            sidekickCanKill = CustomOption.Create(226, "sidekickCanKill", false, jackalCanCreateSidekick);
            sidekickCanUseVents = CustomOption.Create(227, "sidekickCanUseVents", true, jackalCanCreateSidekick);
            jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(228, "jackalPromotedFromSidekickCanCreateSidekick", true, jackalCanCreateSidekick);
            jackalCanCreateSidekickFromImpostor = CustomOption.Create(229, "jackalCanCreateSidekickFromImpostor", true, jackalCanCreateSidekick);
            jackalCanSeeEngineerVent = CustomOption.Create(431, "jackalCanSeeEngineerVent", false, options);
        }

        public List<PlayerControl> getFamily()
        {
            List<PlayerControl> family = new List<PlayerControl>();
            family.AddRange(getParents());
            family.AddRange(getChildren());
            return family;
        }

        public List<PlayerControl> getParents()
        {
            List<PlayerControl> parents = new List<PlayerControl>();
            if (parent != null)
            {
                parents.Add(parent);
                parents.AddRange(parent.role<Jackal>().getParents());
            }
            return parents;
        }

        public List<PlayerControl> getChildren()
        {
            List<PlayerControl> children = new List<PlayerControl>();
            if (sidekick != null)
            {
                children.Add(sidekick);
                children.AddRange(sidekick.role<Jackal>().getChildren());
            }
            else if (fakeSidekick != null)
            {
                children.Add(fakeSidekick);
            }
            return children;
        }
    }

    class Sidekick : Jackal
    {
        public static bool sidekickCanVent { get { return sidekickCanUseVents.getBool(); } }
        public static bool canKill { get { return sidekickCanKill.getBool(); } }
        public static bool promotesToJackal { get { return sidekickPromotesToJackal.getBool(); } }

        public Sidekick() : base() { }
    }
}