using System.Collections.Generic;

namespace TheOtherRoles
{
    public static class MorphHandler
    {
        public static void morphToPlayer(this PlayerControl pc, PlayerControl target)
        {
            setOutfit(pc, target.Data.DefaultOutfit, target.Visible);

            var petId = target.Data.DefaultOutfit.PetId;
            var colorId = target.Data.DefaultOutfit.ColorId;

            if (pc.CurrentPet) UnityEngine.Object.Destroy(pc.CurrentPet.gameObject);
            pc.CurrentPet = UnityEngine.Object.Instantiate<PetBehaviour>(DestroyableSingleton<HatManager>.Instance.GetPetById(petId).PetPrefab);
            pc.CurrentPet.transform.position = pc.transform.position;
            pc.CurrentPet.Source = pc;
            pc.CurrentPet.Visible = pc.Visible;
            PlayerControl.SetPlayerMaterialColors(colorId, pc.CurrentPet.rend);
        }

        public static void setOutfit(this PlayerControl pc, GameData.PlayerOutfit outfit, bool visible = true)
        {
            pc.Data.Outfits[PlayerOutfitType.Shapeshifted] = outfit;
            pc.CurrentOutfitType = PlayerOutfitType.Shapeshifted;

            pc.RawSetName(outfit.PlayerName);
            pc.RawSetHat(outfit.HatId, outfit.ColorId);
            pc.RawSetVisor(outfit.VisorId);
            pc.RawSetColor(outfit.ColorId);
            Helpers.setSkinWithAnim(pc.MyPhysics, outfit.SkinId);
        }

        public static void resetMorph(this PlayerControl pc)
        {
            morphToPlayer(pc, pc);
            pc.CurrentOutfitType = PlayerOutfitType.Default;
        }
    }
}