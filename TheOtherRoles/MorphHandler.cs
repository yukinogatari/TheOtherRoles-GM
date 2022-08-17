using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TheOtherRoles
{
    public static class MorphHandler
    {
        public static void morphToPlayer(this PlayerControl pc, PlayerControl target)
        {
            setOutfit(pc, target.Data.DefaultOutfit, target.Visible);
        }

        public static void setOutfit(this PlayerControl pc, GameData.PlayerOutfit outfit, bool visible = true)
        {
            pc.Data.Outfits[PlayerOutfitType.Shapeshifted] = outfit;
            pc.CurrentOutfitType = PlayerOutfitType.Shapeshifted;

            pc.RawSetName(outfit.PlayerName);
            pc.RawSetHat(outfit.HatId, outfit.ColorId);
            pc.RawSetVisor(outfit.VisorId);
            pc.RawSetColor(outfit.ColorId);
            pc.RawSetPet(outfit.PetId, outfit.ColorId);
            Helpers.setSkinWithAnim(pc.MyPhysics, outfit.SkinId, outfit.ColorId);

            if (pc.CurrentPet != null)
            {
                pc.CurrentPet.enabled = false;
                pc.CurrentPet.Visible = false;
                UnityEngine.GameObject.Destroy(pc.CurrentPet);
            }

            HatManager.Instance.StartCoroutine(
                HatManager.Instance.GetPetById(outfit.PetId).CoLoadViewData(new Action<PetBehaviour>((data) =>
                {
                    if (!pc.Data.IsDead)
                    {
                        pc.CurrentPet = UnityEngine.GameObject.Instantiate(data);
                        pc.CurrentPet.transform.position = pc.transform.position;
                        pc.CurrentPet.Source = pc;
                        pc.CurrentPet.Visible = visible;
                        PlayerControl.SetPlayerMaterialColors(outfit.ColorId, pc.CurrentPet.rend);
                    }
                }))
            );
        }

        public static void resetMorph(this PlayerControl pc)
        {
            morphToPlayer(pc, pc);
            pc.CurrentOutfitType = PlayerOutfitType.Default;
        }
    }

}