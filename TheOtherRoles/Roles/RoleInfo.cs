using HarmonyLib;
using System.Linq;
using System;
using System.Collections.Generic;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    public class RoleInfo {
        public virtual Color color { get; set; }
        public virtual string name { get { return ModTranslation.getString(nameKey); } }
        public virtual string nameColored { get { return Helpers.cs(color, name); } }
        public virtual string introDescription { get { return ModTranslation.getString(nameKey + "IntroDesc"); } }
        public virtual string shortDescription { get { return ModTranslation.getString(nameKey + "ShortDesc"); } }
        public virtual string fullDescription { get { return ModTranslation.getString(nameKey + "FullDesc"); } }
        public virtual string blurb { get { return ModTranslation.getString(nameKey + "Blurb"); } }
        public virtual string roleOptions { 
            get {
                return GameOptionsDataPatch.optionsToString(baseOption, true);
            }
        }

        public virtual int count
        {
            get
            {
                return PlayerControl.GameOptions.RoleOptions.GetNumPerGame((RoleTypes)type);
            }
        }

        public virtual float chance
        {
            get
            {
                return PlayerControl.GameOptions.RoleOptions.GetChancePerGame((RoleTypes)type);
            }
        }

        public virtual bool enabled
        {
            get
            {
                return count > 0 && chance > 0;
            }
        }

        public CustomRoleTypes type;

        private string nameKey;
        public CustomOption baseOption;

        public RoleInfo() { }

        public RoleInfo(string name, Color color, CustomOption baseOption, CustomRoleTypes type) {
            this.color = color;
            this.nameKey = name;
            this.baseOption = baseOption;
            this.type = type;
        }

        public static RoleInfo jester = new RoleInfo("jester", RoleColors.Jester, Jester.options, CustomRoleTypes.Jester);
        public static RoleInfo mayor = new RoleInfo("mayor", RoleColors.Mayor, Mayor.options, CustomRoleTypes.Mayor);
        public static RoleInfo sheriff = new RoleInfo("sheriff", RoleColors.Sheriff, Sheriff.options, CustomRoleTypes.Sheriff);
        public static RoleInfo lighter = new RoleInfo("lighter", RoleColors.Lighter, Lighter.options, CustomRoleTypes.Lighter);
        public static RoleInfo mafia = new RoleInfo("mafia", RoleColors.Mafia, Mafia.options, CustomRoleTypes.Mafia);
        public static RoleInfo godfather = new RoleInfo("godfather", RoleColors.Godfather, Mafia.options, CustomRoleTypes.Godfather);
        public static RoleInfo mafioso = new RoleInfo("mafioso", RoleColors.Mafioso, Mafia.options, CustomRoleTypes.Mafioso);
        public static RoleInfo janitor = new RoleInfo("janitor", RoleColors.Janitor, Mafia.options, CustomRoleTypes.Janitor);
        public static RoleInfo camouflager = new RoleInfo("camouflager", RoleColors.Camouflager, Camouflager.options, CustomRoleTypes.Camouflager);
        public static RoleInfo vampire = new RoleInfo("vampire", RoleColors.Vampire, Vampire.options, CustomRoleTypes.Vampire);
        public static RoleInfo eraser = new RoleInfo("eraser", RoleColors.Eraser, Eraser.options, CustomRoleTypes.Eraser);
        public static RoleInfo trickster = new RoleInfo("trickster", RoleColors.Trickster, Trickster.options, CustomRoleTypes.Trickster);
        public static RoleInfo cleaner = new RoleInfo("cleaner", RoleColors.Cleaner, Cleaner.options, CustomRoleTypes.Cleaner);
        public static RoleInfo warlock = new RoleInfo("warlock", RoleColors.Warlock, Warlock.options, CustomRoleTypes.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo("bountyHunter", RoleColors.BountyHunter, BountyHunter.options, CustomRoleTypes.BountyHunter);
        public static RoleInfo detective = new RoleInfo("detective", RoleColors.Detective, Detective.options, CustomRoleTypes.Detective);
        public static RoleInfo timeMaster = new RoleInfo("timeMaster", RoleColors.TimeMaster, TimeMaster.options, CustomRoleTypes.TimeMaster);
        public static RoleInfo medic = new RoleInfo("medic", RoleColors.Medic, Medic.options, CustomRoleTypes.Medic);
        public static RoleInfo shifter = new RoleInfo("shifter", RoleColors.Shifter, Shifter.options, CustomRoleTypes.Shifter);
        public static RoleInfo swapper = new RoleInfo("swapper", RoleColors.Swapper, Swapper.options, CustomRoleTypes.Swapper);
        public static RoleInfo seer = new RoleInfo("seer", RoleColors.Seer, Seer.options, CustomRoleTypes.Seer);
        public static RoleInfo hacker = new RoleInfo("hacker", RoleColors.Hacker, Hacker.options, CustomRoleTypes.Hacker);
        public static RoleInfo mini = new RoleInfo("mini", RoleColors.Mini, Mini.options, CustomRoleTypes.Mini);
        public static RoleInfo niceMini = new RoleInfo("niceMini", RoleColors.Mini, Mini.options, CustomRoleTypes.NiceMini);
        public static RoleInfo evilMini = new RoleInfo("evilMini", Palette.ImpostorRed, Mini.options, CustomRoleTypes.EvilMini);
        public static RoleInfo tracker = new RoleInfo("tracker", RoleColors.Tracker, Tracker.options, CustomRoleTypes.Tracker);
        public static RoleInfo snitch = new RoleInfo("snitch", RoleColors.Snitch, Snitch.options, CustomRoleTypes.Snitch);
        public static RoleInfo jackal = new RoleInfo("jackal", RoleColors.Jackal, Jackal.options, CustomRoleTypes.Jackal);
        public static RoleInfo sidekick = new RoleInfo("sidekick", RoleColors.Sidekick, Jackal.options, CustomRoleTypes.Sidekick);
        public static RoleInfo spy = new RoleInfo("spy", RoleColors.Spy, Spy.options, CustomRoleTypes.Spy);
        public static RoleInfo securityGuard = new RoleInfo("securityGuard", RoleColors.SecurityGuard, SecurityGuard.options, CustomRoleTypes.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo("arsonist", RoleColors.Arsonist, Arsonist.options, CustomRoleTypes.Arsonist);
        public static RoleInfo guesser = new RoleInfo("guesser", RoleColors.Guesser, Guesser.options, CustomRoleTypes.Guesser);
        public static RoleInfo niceGuesser = new RoleInfo("goodGuesser", RoleColors.Guesser, Guesser.options, CustomRoleTypes.NiceGuesser);
        public static RoleInfo evilGuesser = new RoleInfo("badGuesser", Palette.ImpostorRed, Guesser.options, CustomRoleTypes.EvilGuesser);
        public static RoleInfo bait = new RoleInfo("bait", RoleColors.Bait, Bait.options, CustomRoleTypes.Bait);
        public static RoleInfo madmate = new RoleInfo("madmate", RoleColors.Madmate, Madmate.options, CustomRoleTypes.Madmate);
        public static RoleInfo lovers = new RoleInfo("lovers", RoleColors.Lovers, Lovers.options, CustomRoleTypes.Lovers);
        public static RoleInfo gm = new RoleInfo("gm", RoleColors.GM, GM.options, CustomRoleTypes.GM);
        public static RoleInfo opportunist = new RoleInfo("opportunist", RoleColors.Opportunist, Opportunist.options, CustomRoleTypes.Opportunist);
        public static RoleInfo vulture = new RoleInfo("vulture", RoleColors.Vulture, Vulture.options, CustomRoleTypes.Vulture);
        public static RoleInfo medium = new RoleInfo("medium", RoleColors.Medium, Medium.options, CustomRoleTypes.Medium);

        public static OfficialRoleInfo impostor = new OfficialRoleInfo(RoleTypes.Impostor);
        public static OfficialRoleInfo crewmate = new OfficialRoleInfo(RoleTypes.Crewmate);
        public static OfficialRoleInfo scientist = new OfficialRoleInfo(RoleTypes.Scientist);
        public static OfficialRoleInfo engineer = new OfficialRoleInfo(RoleTypes.Engineer);
        public static OfficialRoleInfo guardianAngel = new OfficialRoleInfo(RoleTypes.GuardianAngel);
        public static OfficialRoleInfo shapeshifter = new OfficialRoleInfo(RoleTypes.Shapeshifter);

        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
            // Official roles
            //crewmate,
            //impostor,
            scientist,
            engineer,
            guardianAngel,
            shapeshifter,


            // Crew roles
            shifter,
            mayor,
            sheriff,
            lighter,
            detective,
            timeMaster,
            medic,
            swapper,
            seer,
            hacker,
            tracker,
            snitch,
            spy,
            securityGuard,
            bait,
            medium,


            // Impostor roles
            mafia,
            godfather,
            mafioso,
            janitor,
            camouflager,
            vampire,
            eraser,
            trickster,
            cleaner,
            warlock,
            bountyHunter,
            madmate,


            // Other roles
            mini,
            niceMini,
            evilMini,
            lovers,
            guesser,
            niceGuesser,
            evilGuesser,
            jester,
            arsonist,
            jackal,
            sidekick,
            opportunist,
            vulture,


            // GM
            gm,
        };


        public static RoleInfo getRoleInfo(RoleBehaviour role)
        {
            foreach (RoleInfo info in allRoleInfos)
            {
                if (info.type == (CustomRoleTypes)role.Role)
                {
                    return info;
                }
            }

            return new OfficialRoleInfo(role);
        }

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, CustomRoleTypes[] excludeRoles = null) {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            infos.Add(getRoleInfo(p.role()));

            // Special roles
            // if ((Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);

            // Modifier
            if (p.hasModifier(RoleModifierTypes.Lovers)) infos.Add(lovers);

            if (excludeRoles != null)
                infos.RemoveAll(x => excludeRoles.Contains(x.type));

            return infos;
        }

        public static String GetRole(PlayerControl p) {
            return p.role()?.NiceName;
        }
    }

    public class OfficialRoleInfo : RoleInfo
    {
        public RoleBehaviour role;

        public override Color color {
            get {
                switch ((RoleTypes)type)
                {
                    case RoleTypes.Crewmate:
                    case RoleTypes.Scientist:
                    case RoleTypes.Engineer:
                    case RoleTypes.GuardianAngel:
                        return Palette.White;

                    case RoleTypes.Impostor:
                    case RoleTypes.Shapeshifter:
                    default:
                        return Palette.ImpostorRed;
                }
            }
        }

        public override string name {
            get {
                return TranslationController.Instance.GetString((RoleTypes)type);
            }
        }

        public override string nameColored { get { return Helpers.cs(color, name); } }
        public override string introDescription
        {
            get
            {
                switch ((RoleTypes)type)
                {
                    case RoleTypes.Scientist:
                        return TranslationController.Instance.GetString(StringNames.ScientistBlurbMed);
                    case RoleTypes.Engineer:
                        return TranslationController.Instance.GetString(StringNames.EngineerBlurbMed);
                    case RoleTypes.GuardianAngel:
                        return TranslationController.Instance.GetString(StringNames.GuardianAngelBlurbMed);
                    case RoleTypes.Shapeshifter:
                        return TranslationController.Instance.GetString(StringNames.ShapeshifterBlurbMed);
                    default:
                        return "";
                }
            }
        }

        public override string shortDescription
        {
            get
            {
                switch ((RoleTypes)type)
                {
                    case RoleTypes.Scientist:
                        return TranslationController.Instance.GetString(StringNames.ScientistBlurbMed);
                    case RoleTypes.Engineer:
                        return TranslationController.Instance.GetString(StringNames.EngineerBlurbMed);
                    case RoleTypes.GuardianAngel:
                        return TranslationController.Instance.GetString(StringNames.GuardianAngelBlurbMed);
                    case RoleTypes.Shapeshifter:
                        return TranslationController.Instance.GetString(StringNames.ShapeshifterBlurbMed);
                    default:
                        return "";
                }
            }
        }

        public override string fullDescription {
            get {
                string desc = "";
                switch ((RoleTypes)type)
                {
                    case RoleTypes.Scientist:
                        desc = TranslationController.Instance.GetString(StringNames.ScientistBlurbLong);
                        break;
                    case RoleTypes.Engineer:
                        desc = TranslationController.Instance.GetString(StringNames.EngineerBlurbLong);
                        break;
                    case RoleTypes.GuardianAngel:
                        desc = TranslationController.Instance.GetString(StringNames.GuardianAngelBlurbLong);
                        break;
                    case RoleTypes.Shapeshifter:
                        desc = TranslationController.Instance.GetString(StringNames.ShapeshifterBlurbLong);
                        break;
                }

                return Helpers.WordWrap(desc, 37).Join(delimiter: "\n");
            }
        }

        public override string blurb { get { return TranslationController.Instance.GetString(role.BlurbName); } }

        public override string roleOptions
        {
            get
            {
                string options = "";
                switch ((RoleTypes)type)
                {
                    case RoleTypes.Scientist:
                        options = @$"
{TranslationController.Instance.GetString(StringNames.ScientistCooldown)}: {PlayerControl.GameOptions.RoleOptions.ScientistCooldown}
{TranslationController.Instance.GetString(StringNames.ScientistBatteryCharge)}: {PlayerControl.GameOptions.RoleOptions.ScientistBatteryCharge}
";
                        break;

                    case RoleTypes.Engineer:
                        options = @$"
{TranslationController.Instance.GetString(StringNames.EngineerCooldown)}: {PlayerControl.GameOptions.RoleOptions.EngineerCooldown}
{TranslationController.Instance.GetString(StringNames.EngineerInVentCooldown)}: {PlayerControl.GameOptions.RoleOptions.EngineerInVentMaxTime}
";
                        break;

                    case RoleTypes.GuardianAngel:
                        options = @$"
{TranslationController.Instance.GetString(StringNames.GuardianAngelCooldown)}: {PlayerControl.GameOptions.RoleOptions.GuardianAngelCooldown}
{TranslationController.Instance.GetString(StringNames.GuardianAngelDuration)}: {PlayerControl.GameOptions.RoleOptions.ProtectionDurationSeconds}
{TranslationController.Instance.GetString(StringNames.GuardianAngelImpostorSeeProtect)}: {PlayerControl.GameOptions.RoleOptions.ImpostorsCanSeeProtect}
";
                        break;

                    case RoleTypes.Shapeshifter:
                        options = @$"
{TranslationController.Instance.GetString(StringNames.ShapeshifterDuration)}: {PlayerControl.GameOptions.RoleOptions.ShapeshifterDuration}
{TranslationController.Instance.GetString(StringNames.ShapeshifterCooldown)}: {PlayerControl.GameOptions.RoleOptions.ShapeshifterCooldown}
{TranslationController.Instance.GetString(StringNames.ShapeshifterLeaveSkin)}: {PlayerControl.GameOptions.RoleOptions.ShapeshifterLeaveSkin}
";
                        break;
                }

                options = options + GameOptionsDataPatch.optionsToString(baseOption, true);
                return options.Trim('\n', '\r');
            }
        }

        public OfficialRoleInfo(RoleTypes type)
        {
            this.setType(type);
        }

        public OfficialRoleInfo(RoleBehaviour role)
        {
            this.setType(role.Role);
            this.role = role;
        }

        public void setType(RoleTypes type)
        {
            this.type = (CustomRoleTypes)type;

            switch (type)
            {
                case RoleTypes.Scientist:
                    this.baseOption = CustomRoleSettings.scientistOptions;
                    break;

                case RoleTypes.Engineer:
                    this.baseOption = CustomRoleSettings.engineerOptions;
                    break;

                case RoleTypes.GuardianAngel:
                    this.baseOption = CustomRoleSettings.gaOptions;
                    break;

                case RoleTypes.Shapeshifter:
                    this.baseOption = CustomRoleSettings.shapeshifterOptions;
                    break;
            }
        }
    }
}
