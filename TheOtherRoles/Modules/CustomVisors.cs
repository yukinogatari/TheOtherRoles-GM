using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Il2CppSystem;
using HarmonyLib;
using UnityEngine;
using UnhollowerBaseLib;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TheOtherRoles.Modules
{
    [HarmonyPatch]
    class CustomVisors
    {
        class customVisor
        {
            public string Idle;
            public string Floor;
            public string Name;
            public string Id;
            public bool FromDisk;

        }

        private static Sprite CreateVisorSprite(string path, bool fromDisk = false)
        {
            UnityEngine.Debug.Log(path + "\n");
            Texture2D texture = fromDisk ? Helpers.loadTextureFromDisk(path) : Helpers.loadTextureFromResources(path);
            if (texture == null)
            {
                System.Console.WriteLine("Texture Error:"+path+"\n");
                return null;
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.5f);
            if (sprite == null)
            {
                System.Console.WriteLine("Sprite Error\n");
                return null;
            }
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }
        private static bool existImage(string path, bool fromDisk)
        {
            if (fromDisk)
            {
                return File.Exists(path);
            }
            else
            {
               Assembly assembly = Assembly.GetExecutingAssembly();
                try {
                    var st=assembly.GetManifestResourceInfo(path);
                    return st!=null;
                }
                catch
                {
                    return false;
                }
            }
        }
        private static VisorData CreateVisorData(customVisor cv, bool fromDisk = false, bool testOnly = false)
        {
            VisorData visor = new VisorData();
            System.Console.WriteLine(cv.Idle + "\n");
            var ext = Path.GetExtension(cv.Idle);
            System.Console.WriteLine(ext + "\n");
            var basename = cv.Idle.Substring(0, cv.Idle.Length - ext.Length);
            System.Console.WriteLine(basename + "\n");

            visor.IdleFrame = CreateVisorSprite(cv.Idle, fromDisk);
            int order;
            if (basename.Last()=='L')
            {
                order = 99;
            }
            else if (basename.Last() == 'H')
            {
                order = 99;
            }
            else
            {
                order = 99;
            }

            var IdleLeft = basename  + "_Left" + ext;
            System.Console.WriteLine(IdleLeft + "\n");
            if (existImage(IdleLeft, fromDisk))
            {
                visor.LeftIdleFrame = CreateVisorSprite(IdleLeft, fromDisk);

            }
//            visor.FloorFrame= CreateVisorSprite(cv.Floor, fromDisk);
            visor.name = cv.Name;
            visor.ProductId = cv.Id;
            visor.ChipOffset = new Vector2(0f, 0.0f);
            visor.Free = true;
            visor.NotInStore = true;
            visor.Order = order;
            
            return visor;
        }
        private static List<customVisor> visorList;

        public static void CustomVisorSetup()
        {
            visorList = new List<customVisor>
                {
                new customVisor{
                    Idle ="TheOtherRoles.Resources.VisorTest.siune_heart_galss1.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.siune_heart_galss1.png",
                    Name ="しうねPのハートサングラス",
                    Id   ="visor_siune_sea_glass",
                    FromDisk=false
                }
                ,
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.siune_sun_galss1.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.siune_sun_galss1.png",
                    Name ="しうねPのサングラス",
                    Id   ="visor_siune_sun_glass",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.Visor_cat1.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.Visor_cat1.png",
                    Name ="猫マズル",
                    Id   ="visor_cat",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.visor_flog1.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.visor_flog1.png",
                    Name ="カエルお面",
                    Id   ="visor_flog",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.visorGundam1.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.visorGundam1.png",
                    Name ="機動戦士〇ンダム",
                    Id   ="visor_gundam",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.VisorVR.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.VisorVR.png",
                    Name ="VRゴーグル",
                    Id   ="VisorVR",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.VisorClackL.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.VisorClack.png",
                    Name ="ひび割れ",
                    Id   ="VisorClack",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.VisorSushi.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.VisorSushi.png",
                    Name ="寿司（サーモン）",
                    Id   ="VisorSushi",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.VisorMasamune.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.VisorTemplate.png",
                    Name ="独眼竜",
                    Id   ="VisorMasamune",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.VisorFrontHairL.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.VisorTemplate.png",
                    Name ="キザ髪",
                    Id   ="VisorFrontHair",
                    FromDisk=false
                },
                new customVisor(){
                    Idle ="TheOtherRoles.Resources.VisorTest.VisorTemplate.png",
//                    Floor="TheOtherRoles.Resources.VisorTest.VisorTemplate.png",
                    Name ="テンプレ",
                    Id   ="VisorTemplate",
                    FromDisk=false
                },
            };
            string filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "VisorTest");
            if (Directory.Exists(filePath))
            {
                var files = Directory.GetFiles(filePath);
                foreach (var text in files.Where(x => x.EndsWith(".txt")))
                {
                    var visorSetting = File.ReadAllLines(text);
                    var visor = new customVisor();
                    visor.Idle = Path.Combine(filePath, visorSetting[0]);
                    visor.Floor = Path.Combine(filePath, visorSetting[1]);
                    visor.Name = visorSetting[2];
                    visor.Id = visorSetting[3];
                    visor.FromDisk = true;

                    visorList.Add(visor);
                }

            }

        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
        private static class HatManagerPatch
        {
            private static bool LOADED;
            private static bool RUNNING;

            static void Prefix(HatManager __instance)
            {
                if (RUNNING) return;
                RUNNING = true; // prevent simultanious execution

                try
                {
                    if (!LOADED)
                    {
                        while (visorList.Count!=0)
                        {
                            var cv = visorList[0];
                            visorList.RemoveAt(0);
                            UnityEngine.Debug.Log("visor Add:" + cv.ToString() + "\n");
                            __instance.AllVisors.Add(CreateVisorData(cv,cv.FromDisk));
                        }
                    }
                }
                catch (System.Exception e)
                {
                    if (!LOADED)
                        UnityEngine.Debug.Log("Unable to add Custom Hats\n" + e);
                }
                LOADED = true;
            }
            static void Postfix(HatManager __instance)
            {
                RUNNING = false;
            }
        }
    }
/*
    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetVisor),typeof(VisorData))]
    public class VisorLayerSetVisorPatch
    {
        private static Material hatShader;

        public static void Postfix(VisorLayer __instance)
        {
            if (hatShader == null && DestroyableSingleton<HatManager>.InstanceExists)
            {
                UnityEngine.Debug.Log("Serch AltShader\n");
                foreach (HatBehaviour h in DestroyableSingleton<HatManager>.Instance.AllHats)
                {
                    if (h.AltShader != null)
                    {
                        UnityEngine.Debug.Log(" AltShader found\n");
                        hatShader = h.AltShader;
                        break;
                    }
                }
            }
            if (hatShader != null)
            {
                UnityEngine.Debug.Log("visor Shader Changed\n");
                __instance.Image.material = hatShader;

            }
        }
    }
*/
}
