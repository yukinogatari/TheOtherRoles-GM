using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Collections;
using UnhollowerBaseLib;
using UnityEngine;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Modules;
using HarmonyLib;
using Hazel;
using TheOtherRoles.Patches;

namespace TheOtherRoles {

    public enum MurderAttemptResult {
        PerformKill,
        SuppressKill,
        BlankKill
    }
	
    public static class Helpers
    {
        public static bool ShowButtons
        {
            get
            {
                return !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                      !MeetingHud.Instance &&
                      !ExileController.Instance;
            }
        }

        public static bool ShowMeetingText
        {
            get
            {
                return MeetingHud.Instance != null &&
                    (MeetingHud.Instance.state == MeetingHud.VoteStates.Voted ||
                     MeetingHud.Instance.state == MeetingHud.VoteStates.NotVoted ||
                     MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion);
            }
        }

        public static bool GameStarted
        {
            get
            {
                return AmongUsClient.Instance != null && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;
            }
        }

        public static bool RolesEnabled
        {
            get
            {
                return CustomOptionHolder.activateRoles.getBool();
            }
        }

        public static bool RefundVotes
        {
            get
            {
                return CustomOptionHolder.refundVotesOnDeath.getBool();
            }
        }

        public static void destroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : UnityEngine.Object
        {
            if (items == null) return;
            foreach (T item in items)
            {
                UnityEngine.Object.Destroy(item);
            }
        }

        public static void destroyList<T>(List<T> items) where T : UnityEngine.Object
        {
            if (items == null) return;
            foreach (T item in items)
            {
                UnityEngine.Object.Destroy(item);
            }
        }

        public static void log(string msg)
        {
            TheOtherRolesPlugin.Instance.Log.LogInfo(msg);
        }

        public static List<byte> generateTasks(int numCommon, int numShort, int numLong)
        {
            if (numCommon + numShort + numLong <= 0)
            {
                numShort = 1;
            }

            var tasks = new Il2CppSystem.Collections.Generic.List<byte>();
            var hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();

            var commonTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in ShipStatus.Instance.CommonTasks.OrderBy(x => rnd.Next())) commonTasks.Add(task);

            var shortTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in ShipStatus.Instance.NormalTasks.OrderBy(x => rnd.Next())) shortTasks.Add(task);

            var longTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in ShipStatus.Instance.LongTasks.OrderBy(x => rnd.Next())) longTasks.Add(task);

            int start = 0;
            ShipStatus.Instance.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasks);

            start = 0;
            ShipStatus.Instance.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasks);

            start = 0;
            ShipStatus.Instance.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasks);

            return tasks.ToArray().ToList();
        }

        public static void generateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
        {
            if (player == null) return;

            List<byte> taskTypeIds = generateTasks(numCommon, numShort, numLong);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedSetTasks, Hazel.SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            writer.WriteBytesAndSize(taskTypeIds.ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.uncheckedSetTasks(player.PlayerId, taskTypeIds.ToArray());
        }

        public static void setSkinWithAnim(PlayerPhysics playerPhysics, string SkinId, int ColorId)
        {
            SkinViewData nextSkin = HatManager.Instance.GetSkinById(SkinId).viewData.viewData;
            AnimationClip clip = null;
            var spriteAnim = playerPhysics.Skin.animator;
            var anim = spriteAnim.m_animator;

            var currentPhysicsAnim = playerPhysics.Animator.GetCurrentAnimation();
            if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.RunAnim) clip = nextSkin.RunAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.SpawnAnim) clip = nextSkin.SpawnAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.EnterVentAnim) clip = nextSkin.EnterVentAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.ExitVentAnim) clip = nextSkin.ExitVentAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.IdleAnim) clip = nextSkin.IdleAnim;
            else clip = nextSkin.IdleAnim;

            float progress = playerPhysics.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            playerPhysics.myPlayer.RawSetSkin(SkinId, ColorId);

            spriteAnim.Play(clip, 1f);
            anim.Play("a", 0, progress % 1);
            anim.Update(0f);
        }

        public static Sprite loadSpriteFromResources(string path, float pixelsPerUnit) {
            try {
                Texture2D texture = loadTextureFromResources(path);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            } catch {
                System.Console.WriteLine("Error loading sprite from path: " + path);
            }
            return null;
        }

        public static Texture2D loadTextureFromResources(string path) {
            try {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var byteTexture = new byte[stream.Length];
                var read = stream.Read(byteTexture, 0, (int) stream.Length);
                LoadImage(texture, byteTexture, false);
                return texture;
            } catch {
                System.Console.WriteLine("Error loading texture from resources: " + path);
            }
            return null;
        }

        public static Texture2D loadTextureFromDisk(string path) {
            try {          
                if (File.Exists(path))     {
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                    byte[] byteTexture = File.ReadAllBytes(path);
                    LoadImage(texture, byteTexture, false);
                    return texture;
                }
            } catch {
                System.Console.WriteLine("Error loading texture from disk: " + path);
            }
            return null;
        }

        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable) {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2cppArray = (Il2CppStructArray<byte>) data;
            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }

        public static PlayerControl playerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;
            return null;
        }
        
        public static Dictionary<byte, PlayerControl> allPlayersById()
        {
            Dictionary<byte, PlayerControl> res = new Dictionary<byte, PlayerControl>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                res.Add(player.PlayerId, player);
            return res;
        }

        public static void handleVampireBiteOnBodyReport() {
            // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
            Helpers.checkMuderAttemptAndKill(Vampire.vampire, Vampire.bitten, true, false);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
            writer.Write(byte.MaxValue);
            writer.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
        }

        public static void refreshRoleDescription(PlayerControl player) {
            if (player == null) return;

            List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(player); 

            var toRemove = new List<PlayerTask>();
            foreach (PlayerTask t in player.myTasks) {
                var textTask = t.gameObject.GetComponent<ImportantTextTask>();
                if (textTask != null) {
                    var info = infos.FirstOrDefault(x => textTask.Text.StartsWith(x.name));
                    if (info != null)
                        infos.Remove(info); // TextTask for this RoleInfo does not have to be added, as it already exists
                    else
                        toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
                }
            }   

            foreach (PlayerTask t in toRemove) {
                t.OnRemove();
                player.myTasks.Remove(t);
                UnityEngine.Object.Destroy(t.gameObject);
            }

            // Add TextTask for remaining RoleInfos
            foreach (RoleInfo roleInfo in infos) {
                var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);

                if (roleInfo.roleType == RoleType.Jackal) {
                    if (Jackal.canCreateSidekick)
                    {
                        task.Text = cs(roleInfo.color, $"{roleInfo.name}: " + ModTranslation.getString("jackalWithSidekick"));
                    } 
                    else
                    {
                        task.Text = cs(roleInfo.color, $"{roleInfo.name}: " + ModTranslation.getString("jackalShortDesc"));
                    }
                } else {
                    task.Text = cs(roleInfo.color, $"{roleInfo.name}: {roleInfo.shortDescription}");  
                }

                player.myTasks.Insert(0, task);
            }

            if (player.hasModifier(ModifierType.Madmate))
            {
                var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = cs(Madmate.color, $"{Madmate.fullName}: " + ModTranslation.getString("madmateShortDesc"));
                player.myTasks.Insert(0, task);
            }
        }

        public static bool isLighterColor(int colorId) {
            return CustomColors.lighterColors.Contains(colorId);
        }

        public static bool isCustomServer() {
            if (DestroyableSingleton<ServerManager>.Instance == null) return false;
            StringNames n = DestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
            return n != StringNames.ServerNA && n != StringNames.ServerEU && n != StringNames.ServerAS;
        }

        public static bool isDead(this PlayerControl player)
        {
            return player == null || player?.Data?.IsDead == true || player?.Data?.Disconnected == true ||
                  (finalStatuses != null && finalStatuses.ContainsKey(player.PlayerId) && finalStatuses[player.PlayerId] != FinalStatus.Alive);
        }

        public static bool isAlive(this PlayerControl player)
        {
            return !isDead(player);
        }

        public static bool isNeutral(this PlayerControl player)
        {
            return (player != null &&
                   (player.isRole(RoleType.Jackal) ||
                    player.isRole(RoleType.Sidekick) ||
                    Jackal.formerJackals.Contains(player) ||
                    player.isRole(RoleType.Arsonist) ||
                    player.isRole(RoleType.Jester) ||
                    player.isRole(RoleType.Opportunist) ||
                    player.isRole(RoleType.PlagueDoctor) ||
                    player.isRole(RoleType.Fox) ||
                    player.isRole(RoleType.Immoralist) ||
                    player.isRole(RoleType.Vulture) ||
                    player.isRole(RoleType.Lawyer) ||
                    player.isRole(RoleType.Pursuer) ||
                    player.isRole(RoleType.Akujo) ||
                    player.hasModifier(ModifierType.AkujoHonmei) ||
                    (player.isRole(RoleType.Shifter) && Shifter.isNeutral)));
        }

        public static bool isCrew(this PlayerControl player)
        {
            return player != null && !player.isImpostor() && !player.isNeutral() && !player.isGM();
        }

        public static bool isImpostor(this PlayerControl player)
        {
            return player != null && player.Data.Role.IsImpostor;
        }

        public static bool hasFakeTasks(this PlayerControl player) {
            return (player.isNeutral() && !player.neutralHasTasks()) ||
                   (player.hasModifier(ModifierType.Madmate) && !Madmate.hasTasks) ||
                   (player.isLovers() && Lovers.separateTeam && !Lovers.tasksCount) ||
                   (player.hasModifier(ModifierType.AkujoHonmei) && !player.isImpostor() && !player.neutralHasTasks());
        }

        public static bool neutralHasTasks(this PlayerControl player)
        {
            return player.isNeutral() && 
                (player.isRole(RoleType.Lawyer) ||
                 player.isRole(RoleType.Pursuer) ||
                 player.isRole(RoleType.Shifter) ||
                 player.isRole(RoleType.Fox));
        }

        public static bool isGM(this PlayerControl player)
        {
            return GM.gm != null && player == GM.gm;
        }

        public static bool isLovers(this PlayerControl player)
        {
            return player != null && Lovers.isLovers(player);
        }

        public static bool isAkujoLover(this PlayerControl player)
        {
            return player != null &&
                (player.isRole(RoleType.Akujo) ||
                 player.hasModifier(ModifierType.AkujoHonmei) ||
                 player.hasModifier(ModifierType.AkujoKeep));
        }

        public static bool isAkujoPartners(this PlayerControl player, PlayerControl partner)
        {
            foreach (var akujo in Akujo.players)
            {
                if ((akujo.player == player && akujo.isPartner(partner)) ||
                    (akujo.player == partner && akujo.isPartner(player)))
                {
                    return true;
                }
            }

            return false;
        }

        public static PlayerControl getPartner(this PlayerControl player)
        {
            return Lovers.getPartner(player);
        }

        public static bool canBeErased(this PlayerControl player) {
            return (player != Jackal.jackal && player != Sidekick.sidekick && !Jackal.formerJackals.Contains(player));
        }

        public static void clearAllTasks(this PlayerControl player) {
            if (player == null) return;
            for (int i = 0; i < player.myTasks.Count; i++) {
                PlayerTask playerTask = player.myTasks[i];
                playerTask.OnRemove();
                UnityEngine.Object.Destroy(playerTask.gameObject);
            }
            player.myTasks.Clear();
            
            if (player.Data != null && player.Data.Tasks != null)
                player.Data.Tasks.Clear();
        }

        public static void setSemiTransparent(this PoolablePlayer player, bool value) {
            float alpha = value ? 0.25f : 1f;
            foreach (SpriteRenderer r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
                r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
            player.NameText.color = new Color(player.NameText.color.r, player.NameText.color.g, player.NameText.color.b, alpha);
        }

        public static string GetString(this TranslationController t, StringNames key, params Il2CppSystem.Object[] parts) {
            return t.GetString(key, parts);
        }

        public static string cs(Color c, string s) {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
 
        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie) {
            tie = true;
            KeyValuePair<byte, int> result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (KeyValuePair<byte, int> keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }
            }
            return result;
        }

        public static bool hidePlayerName(PlayerControl target)
        {
            return hidePlayerName(PlayerControl.LocalPlayer, target);
        }

        public static bool hidePlayerName(PlayerControl source, PlayerControl target)
        {
            if (source == target) return false;
            if (source == null || target == null) return true;
            if (source.isDead()) return false;
            if (target.isDead()) return true;
            if (Camouflager.camouflageTimer > 0f) return true; // No names are visible
            if (!source.isImpostor() && Ninja.isStealthed(target)) return true; // Hide ninja nametags from non-impostors
            if (!source.isRole(RoleType.Fox) && !source.Data.IsDead && Fox.isStealthed(target)) return true;
            if (MapOptions.hideOutOfSightNametags && GameStarted && ShipStatus.Instance != null && source.transform != null && target.transform != null)
            {
                float distMod = 1.025f;
                float distance = Vector3.Distance(source.transform.position, target.transform.position);
                bool anythingBetween = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false);

                if (distance > ShipStatus.Instance.CalculateLightRadius(source.Data) * distMod || anythingBetween) return true;
            }
            if (!MapOptions.hidePlayerNames) return false; // All names are visible
            if (source.isImpostor() && (target.isImpostor() || target.isRole(RoleType.Spy))) return false; // Members of team Impostors see the names of Impostors/Spies
            if (source.getPartner() == target) return false; // Members of team Lovers see the names of each other
            if ((source.isRole(RoleType.Jackal) || source.isRole(RoleType.Sidekick)) && (target.isRole(RoleType.Jackal) || target.isRole(RoleType.Sidekick) || target == Jackal.fakeSidekick)) return false; // Members of team Jackal see the names of each other
            return true;
        }

        public static void setDefaultLook(this PlayerControl target) {
            target.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void setLook(this PlayerControl target, String playerName, int colorId, string hatId, string visorId, string skinId, string petId) {
            target.RawSetColor(colorId);
            target.RawSetVisor(visorId);
            target.RawSetHat(hatId, colorId);
            target.RawSetName(hidePlayerName(PlayerControl.LocalPlayer, target) ? "" : playerName);

            SkinViewData nextSkin = DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId).viewData.viewData;
            PlayerPhysics playerPhysics = target.MyPhysics;
            AnimationClip clip = null;
            var spriteAnim = playerPhysics.Skin.animator;
            var currentPhysicsAnim = playerPhysics.Animator.GetCurrentAnimation();
            if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.RunAnim) clip = nextSkin.RunAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.SpawnAnim) clip = nextSkin.SpawnAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.EnterVentAnim) clip = nextSkin.EnterVentAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.ExitVentAnim) clip = nextSkin.ExitVentAnim;
            else if (currentPhysicsAnim == playerPhysics.CurrentAnimationGroup.IdleAnim) clip = nextSkin.IdleAnim;
            else clip = nextSkin.IdleAnim;
            float progress = playerPhysics.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            playerPhysics.Skin.skin = nextSkin;
            spriteAnim.Play(clip, 1f);
            spriteAnim.m_animator.Play("a", 0, progress % 1);
            spriteAnim.m_animator.Update(0f);

            if (target.CurrentPet) UnityEngine.Object.Destroy(target.CurrentPet.gameObject);
            target.CurrentPet = UnityEngine.Object.Instantiate<PetBehaviour>(DestroyableSingleton<HatManager>.Instance.GetPetById(petId).viewData.viewData);
            target.CurrentPet.transform.position = target.transform.position;
            target.CurrentPet.Source = target;
            target.CurrentPet.Visible = target.Visible;
            PlayerControl.SetPlayerMaterialColors(colorId, target.CurrentPet.rend);
        }

        public static void showFlash(Color color, float duration = 1f)
        {
            if (HudManager.Instance == null || HudManager.Instance.FullScreen == null) return;
            HudManager.Instance.FullScreen.gameObject.SetActive(true);
            HudManager.Instance.FullScreen.enabled = true;
            HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) => {
                var renderer = HudManager.Instance.FullScreen;

                if (p < 0.5)
                {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * 0.75f));
                }
                else
                {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
                }
                if (p == 1f && renderer != null) renderer.enabled = false;
            })));
        }

        public static bool roleCanUseVents(this PlayerControl player) {
            bool roleCouldUse = false;
            if (player.isRole(RoleType.Engineer))
                roleCouldUse = true;
            else if (Jackal.canUseVents && player.isRole(RoleType.Jackal))
                roleCouldUse = true;
            else if (Sidekick.canUseVents && player.isRole(RoleType.Sidekick))
                roleCouldUse = true;
            else if (Spy.canEnterVents && player.isRole(RoleType.Spy))
                roleCouldUse = true;
            else if (Madmate.canEnterVents && player.hasModifier(ModifierType.Madmate))
                roleCouldUse = true;
            else if (Vulture.canUseVents && player.isRole(RoleType.Vulture))
                roleCouldUse = true;
            else if (player.Data?.Role != null && player.Data.Role.CanVent)
            {
                if (!Janitor.canVent && player.isRole(RoleType.Janitor))
                    roleCouldUse = false;
                else if (!Mafioso.canVent && player.isRole(RoleType.Mafioso))
                    roleCouldUse = false;
                else if (!Ninja.canUseVents && player.isRole(RoleType.Ninja))
                    roleCouldUse = false;
                else
                    roleCouldUse = true;
            }
            return roleCouldUse;
        }

        public static bool roleCanSabotage(this PlayerControl player)
        {
            bool roleCouldUse = false;
            if (Madmate.canSabotage && player.hasModifier(ModifierType.Madmate))
                roleCouldUse = true;
            else if (Jester.canSabotage && player.isRole(RoleType.Jester))
                roleCouldUse = true;
            else if (!Mafioso.canSabotage && player.isRole(RoleType.Mafioso))
                roleCouldUse = false;
            else if (!Janitor.canSabotage && player.isRole(RoleType.Janitor))
                roleCouldUse = false;
            else if (player.Data?.Role != null && player.Data.Role.IsImpostor)
                roleCouldUse = true;

            return roleCouldUse;
        }

        public static MurderAttemptResult checkMuderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false) {
            // Modified vanilla checks
            if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
            if (killer == null || killer.Data == null || killer.Data.IsDead || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
            if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code

            // Handle blank shot
            if (Pursuer.blankedList.Any(x => x.PlayerId == killer.PlayerId)) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBlanked, Hazel.SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                writer.Write((byte)0);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setBlanked(killer.PlayerId, 0);

                return MurderAttemptResult.BlankKill;
            }

            // Block impostor shielded kill
            if (Medic.shielded != null && Medic.shielded == target) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shieldedMurderAttempt();
                return MurderAttemptResult.SuppressKill;
            }

            // Block impostor not fully grown mini kill
            else if (Mini.mini != null && target.isRole(RoleType.Mini) && !Mini.isGrownUp()) {
                return MurderAttemptResult.SuppressKill;
            }

            // Block Time Master with time shield kill
            else if (TimeMaster.shieldActive && TimeMaster.timeMaster != null && TimeMaster.timeMaster == target) {
                if (!blockRewind) { // Only rewind the attempt was not called because a meeting startet 
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.timeMasterRewindTime();
                }
                return MurderAttemptResult.SuppressKill;
            }
            return MurderAttemptResult.PerformKill;
        }

        public static MurderAttemptResult checkMuderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true)  {
            // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
            // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible

            MurderAttemptResult murder = checkMuderAttempt(killer, target, isMeetingStart);
            if (murder == MurderAttemptResult.PerformKill) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                writer.Write(target.PlayerId);
                writer.Write(showAnimation ? Byte.MaxValue : 0);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.uncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? Byte.MaxValue : (byte)0);
            }
            return murder;            
        }
    
        public static void shareGameVersion() {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable, -1);
            writer.WritePacked(TheOtherRolesPlugin.Version.Major);
            writer.WritePacked(TheOtherRolesPlugin.Version.Minor);
            writer.WritePacked(TheOtherRolesPlugin.Version.Build);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TheOtherRolesPlugin.Version.Revision < 0 ? 0xFF : TheOtherRolesPlugin.Version.Revision));
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.versionHandshake(TheOtherRolesPlugin.Version.Major, TheOtherRolesPlugin.Version.Minor, TheOtherRolesPlugin.Version.Build, TheOtherRolesPlugin.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static List<PlayerControl> getKillerTeamMembers(PlayerControl player) {
            List<PlayerControl> team = new List<PlayerControl>();
            foreach(PlayerControl p in PlayerControl.AllPlayerControls) {
                if (player.Data.Role.IsImpostor && p.Data.Role.IsImpostor && player.PlayerId != p.PlayerId && team.All(x => x.PlayerId != p.PlayerId)) team.Add(p);
                else if (player.isRole(RoleType.Jackal) && p.isRole(RoleType.Sidekick)) team.Add(p); 
                else if (player.isRole(RoleType.Sidekick) && p.isRole(RoleType.Jackal)) team.Add(p);
            }
            
            return team;
        }

        public static void shuffle<T>(this IList<T> self, int startAt = 0)
        {
            for (int i = startAt; i < self.Count - 1; i++) {
                T value = self[i];
                int index = UnityEngine.Random.Range(i, self.Count);
                self[i] = self[index];
                self[index] = value;
            }
        }

        public static void shuffle<T>(this System.Random r, IList<T> self)
        {
            for (int i = 0; i < self.Count; i++) {
                T value = self[i];
                int index = r.Next(self.Count);
                self[i] = self[index];
                self[index] = value;
            }
        }
    }
}
