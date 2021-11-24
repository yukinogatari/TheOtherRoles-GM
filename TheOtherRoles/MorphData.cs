using System.Collections.Generic;

namespace TheOtherRoles
{
    public class MorphData
    {
        public static Dictionary<byte, MorphData> morphData = new Dictionary<byte, MorphData>();

        public string name = "";
        public string hat = "";
        public string visor = "";
        public int color = 6;
        public string skin = "";
        public string pet = "";
        public bool visible = true;

        public static void resetMorphData()
        {
            morphData = new Dictionary<byte, MorphData>();
        }

        public MorphData() { }

        public MorphData(PlayerControl p)
        {
            name = p.name;
            hat = p.Data.DefaultOutfit.HatId;
            color = p.Data.DefaultOutfit.ColorId;
            skin = p.Data.DefaultOutfit.SkinId;
            visor = p.Data.DefaultOutfit.VisorId;
            pet = p.Data.DefaultOutfit.PetId;
            visible = p.Visible;
        }

        public void applyToPlayer(PlayerControl p)
        {
            p.SetName(name);
            p.SetHat(hat, color);
            Helpers.setSkinWithAnim(p.MyPhysics, skin);
            p.SetPet(pet);
            p.SetVisor(visor);
            p.CurrentPet.Visible = visible && !p.Data.IsDead;
            p.SetColor(color);
        }
    }
}