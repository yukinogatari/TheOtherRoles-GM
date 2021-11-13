using System.Collections.Generic;

namespace TheOtherRoles
{
    public class MorphData
    {
        public static Dictionary<byte, MorphData> morphData = new Dictionary<byte, MorphData>();

        public string name = "";
        public string hat = "";
        public int color = 6;
        public string skin = "0";
        public string pet = "0";
        public bool visible = true;

        public static void resetMorphData()
        {
            morphData = new Dictionary<byte, MorphData>();
        }

        public MorphData() { }

        public MorphData(PlayerControl p)
        {
            name = p.name;
            hat = p.CurrentOutfit.HatId;
            color = p.CurrentOutfit.ColorId;
            skin = p.CurrentOutfit.SkinId;
            pet = p.CurrentOutfit.PetId;
            visible = p.Visible;
        }

        public void applyToPlayer(PlayerControl p)
        {
            p.SetName(name);
            p.SetHat(hat, color);
            //Helpers.setSkinWithAnim(p.MyPhysics, skin); TODO: PROBABLY USE NEW OUTFIT SYSTEM
            p.SetPet(pet);
            p.CurrentPet.Visible = visible && !p.Data.IsDead;
            p.SetColor(color);
        }
    }
}