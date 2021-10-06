namespace TheOtherRoles
{
    public class MorphData
    {
        public string name = "";
        public uint hat = 0;
        public int color = 6;
        public uint skin = 0;
        public uint pet = 0;
        public bool visible = true;

        public MorphData() { }

        public MorphData(PlayerControl p)
        {
            name = p.name;
            hat = p.Data.HatId;
            color = p.Data.ColorId;
            skin = p.Data.SkinId;
            pet = p.Data.PetId;
            visible = p.Visible;
        }

        public void applyToPlayer(PlayerControl p)
        {
            p.SetName(name);
            p.SetHat(hat, color);
            Helpers.setSkinWithAnim(p.MyPhysics, skin);
            p.SetPet(pet);
            p.CurrentPet.Visible = visible;
            p.SetColor(color);
        }
    }
}