using System.Linq;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    struct MedicShieldInfo
    {

    }

    class MedicShield : RoleModifier
    {
        public PlayerControl medic;

        public MedicShield(PlayerControl? medic = null) : base()
        {
            color = RoleColors.Medic;
            type = RoleModifierTypes.MedicShield;

            if (medic != null)
            {
                this.medic = medic;
            }
        }
    }
}