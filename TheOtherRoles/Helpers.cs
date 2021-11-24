using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Collections;
using UnhollowerBaseLib;
using UnityEngine;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Modules;
using HarmonyLib;
using Hazel;
using TheOtherRoles.Roles;

namespace TheOtherRoles {
    public static class Helpers
    {
        public static bool ShowButtons { 
            get { 
                return !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                      !MeetingHud.Instance &&
                      !ExileController.Instance;
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

        public static void setSkinWithAnim(PlayerPhysics playerPhysics, uint SkinId)
        {
            SkinData nextSkin = DestroyableSingleton<HatManager>.Instance.AllSkins[(int)SkinId];
            AnimationClip clip = null;
            var spriteAnim = playerPhysics.Skin.animator;
            var anim = spriteAnim.m_animator;
            var skinLayer = playerPhysics.Skin;

            var currentPhysicsAnim = playerPhysics.Animator.GetCurrentAnimation();
            if (currentPhysicsAnim == playerPhysics.RunAnim) clip = nextSkin.RunAnim;
            else if (currentPhysicsAnim == playerPhysics.SpawnAnim) clip = nextSkin.SpawnAnim;
            else if (currentPhysicsAnim == playerPhysics.EnterVentAnim) clip = nextSkin.EnterVentAnim;
            else if (currentPhysicsAnim == playerPhysics.ExitVentAnim) clip = nextSkin.ExitVentAnim;
            else if (currentPhysicsAnim == playerPhysics.IdleAnim) clip = nextSkin.IdleAnim;
            else clip = nextSkin.IdleAnim;

            float progress = playerPhysics.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            skinLayer.skin = nextSkin;

            spriteAnim.Play(clip, 1f);
            anim.Play("a", 0, progress % 1);
            anim.Update(0f);
        }

        public static Sprite loadSpriteFromResources(string path, float pixelsPerUnit) {
            try {
                Texture2D texture = loadTextureFromResources(path);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            } catch {
                Helpers.log("Error loading sprite from path: " + path);
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
                Helpers.log("Error loading texture from resources: " + path);
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
                Helpers.log("Error loading texture from disk: " + path);
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


        public static PlayerControl playerById(int id)
        {
            return playerById((byte)id);
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

        // TODO: MURDER

        public static void handleVampireBiteOnBodyReport()
        {
            foreach (Vampire v in RoleHelpers.getRoles<Vampire>())
            {
                // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
                Helpers.checkMurderAttemptAndKill(v.Player, v.bitten, true);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                writer.Write(byte.MaxValue);
                writer.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.vampireSetBitten(v.Player.PlayerId, byte.MaxValue, byte.MaxValue);
            }
        }


        // TODO: FIX
        public static void setBasePlayerOutlines()
        {
            // only mess with this for custom roles
            if (PlayerControl.LocalPlayer.role()?.Role < (RoleTypes)CustomRoleTypes.Crewmate) return;

            foreach (PlayerControl target in PlayerControl.AllPlayerControls)
            {
                if (target == null || target.myRend == null) continue;

                bool hasVisibleShield = false;
/*                if (Camouflager.camouflageTimer <= 0f && Medic.shielded != null && ((target == Medic.shielded && !isMorphedMorphling) || (isMorphedMorphling && Morphling.morphTarget == Medic.shielded)))
                {
                    hasVisibleShield = Medic.showShielded == 0 // Everyone
                        || (Medic.showShielded == 1 && (PlayerControl.LocalPlayer == Medic.shielded || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Medic))) // Shielded + Medic
                        || (Medic.showShielded == 2 && PlayerControl.LocalPlayer.isRole(CustomRoleTypes.Medic)); // Medic only
                }*/

                if (hasVisibleShield)
                {
                    target.myRend.material.SetFloat("_Outline", 1f);
                    target.myRend.material.SetColor("_OutlineColor", Medic.ShieldColor);
                }
                else
                {
                    target.myRend.material.SetFloat("_Outline", 0f);
                }
            }
        }

        // TODO: FIX
        public static void bendTimeUpdate()
        {
            if (TimeMaster.isRewinding)
            {
                if (localPlayerPositions.Count > 0)
                {
                    // Set position
                    var next = localPlayerPositions[0];
                    if (next.Item2 == true)
                    {
                        // Exit current vent if necessary
                        if (PlayerControl.LocalPlayer.inVent)
                        {
                            foreach (Vent vent in ShipStatus.Instance.AllVents)
                            {
                                bool canUse;
                                bool couldUse;
                                vent.CanUse(PlayerControl.LocalPlayer.Data, out canUse, out couldUse);
                                if (canUse)
                                {
                                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(vent.Id);
                                    vent.SetButtons(false);
                                }
                            }
                        }
                        // Set position
                        PlayerControl.LocalPlayer.transform.position = next.Item1;
                    }
                    else if (localPlayerPositions.Any(x => x.Item2 == true))
                    {
                        PlayerControl.LocalPlayer.transform.position = next.Item1;
                    }

                    localPlayerPositions.RemoveAt(0);

                    if (localPlayerPositions.Count > 1) localPlayerPositions.RemoveAt(0); // Skip every second position to rewinde twice as fast, but never skip the last position
                }
                else
                {
                    TimeMaster.isRewinding = false;
                    PlayerControl.LocalPlayer.moveable = true;
                }
            }
            else
            {
                while (localPlayerPositions.Count >= Mathf.Round(TimeMaster.rewindTime / Time.fixedDeltaTime)) localPlayerPositions.RemoveAt(localPlayerPositions.Count - 1);
                localPlayerPositions.Insert(0, new Tuple<Vector3, bool>(PlayerControl.LocalPlayer.transform.position, PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
            }
        }

        public static void updatePlayerInfo()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

            bool commsActive = PlayerControl.LocalPlayer.role()?.CommsSabotaged == true;
/*            foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
            {
                if (t.TaskType == TaskTypes.FixComms)
                {
                    commsActive = true;
                    break;
                }
            }*/

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p != PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !p.isRole(CustomRoleTypes.GM) && !PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM)) continue;

                Transform playerInfoTransform = p.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo == null)
                {
                    playerInfo = UnityEngine.Object.Instantiate(p.nameText, p.nameText.transform.parent);
                    playerInfo.fontSize *= 0.75f;
                    playerInfo.gameObject.name = "Info";
                }

                // Set the position every time bc it sometimes ends up in the wrong place due to camoflauge
                playerInfo.transform.localPosition = p.nameText.transform.localPosition + Vector3.up * 0.5f;

                PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.20f;
                    meetingInfo.fontSize *= 0.63f;
                    meetingInfo.autoSizeTextContainer = true;
                    meetingInfo.gameObject.name = "Info";
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                string roleNames = RoleInfo.getRoleInfoForPlayer(p).Select(x => x.nameColored).Join(delimiter: " ");

                var completedStr = commsActive ? "?" : tasksCompleted.ToString();
                string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({completedStr}/{tasksTotal})</color>" : "";

                string playerInfoText = "";
                string meetingInfoText = "";
                if (p == PlayerControl.LocalPlayer)
                {
                    playerInfoText = $"{roleNames}";
                    if (DestroyableSingleton<TaskPanelBehaviour>.InstanceExists)
                    {
                        TMPro.TextMeshPro tabText = DestroyableSingleton<TaskPanelBehaviour>.Instance.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                        tabText.SetText($"Tasks {taskInfo}");
                    }
                    meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                }
                else if (MapOptions.ghostsSeeRoles && MapOptions.ghostsSeeTasks)
                {
                    playerInfoText = $"{roleNames} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (MapOptions.ghostsSeeTasks)
                {
                    playerInfoText = $"{taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (MapOptions.ghostsSeeRoles)
                {
                    playerInfoText = $"{roleNames}";
                    meetingInfoText = playerInfoText;
                }
                else if (p.isRole(CustomRoleTypes.GM) || PlayerControl.LocalPlayer.isRole(CustomRoleTypes.GM))
                {
                    playerInfoText = $"{roleNames} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }

                meetingInfoText = meetingInfoText.Replace("ラバーズ", "♥");
                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible);
                if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
            }
        }

        public static void refreshRoleDescription(PlayerControl player) {
            if (player == null) return;

            // only mess with this for custom roles
            if (player.role()?.Role < (RoleTypes)CustomRoleTypes.Crewmate) return;

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

                if (roleInfo.type == CustomRoleTypes.Jackal) {
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
        }

        public static bool isLighterColor(int colorId) {
            return CustomColors.lighterColors.Contains(colorId);
        }

        public static bool isCustomServer() {
            if (DestroyableSingleton<ServerManager>.Instance == null) return false;
            StringNames n = DestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
            return n != StringNames.ServerNA && n != StringNames.ServerEU && n != StringNames.ServerAS;
        }

        public static bool isNeutral(this PlayerControl player)
        {
            return (player != null && player.role()?.TeamType != RoleTeamTypes.Crewmate && player.role()?.TeamType != RoleTeamTypes.Impostor);
        }

        public static bool isCrew(this PlayerControl player)
        {
            return player != null && !player.role()?.IsImpostor == true && !player.isNeutral();
        }

        public static bool hasFakeTasks(this PlayerControl player) {
            return player.isNeutral() ||
                   player.isRole(CustomRoleTypes.Madmate) ||
                   (player.isLovers() && Lovers.separateTeam && !Lovers.tasksCount);
        }

        public static bool isGM(this PlayerControl player)
        {
            return player != null && player.isRole(CustomRoleTypes.GM);
        }

        public static bool canBeErased(this PlayerControl player) {
            return player.isRole(CustomRoleTypes.Jackal) || player.isRole(CustomRoleTypes.Sidekick);
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

        public static bool hidePlayerName(PlayerControl source, PlayerControl target) {
            // TODO: HANDLE CAMO
            //if (Camouflager.camouflageTimer > 0f) return true; // No names are visible
            if (!MapOptions.hidePlayerNames) return false; // All names are visible
            if (source == null || target == null) return true;
            if (source == target) return false; // Player sees his own name
            if (source.role()?.IsImpostor == true && (target.role()?.IsImpostor == true || target.isRole(CustomRoleTypes.Spy))) return false; // Members of team Impostors see the names of Impostors/Spies
            if (source.isRelated(target)) return false; // Members of team Lovers see the names of each other
            return true;
        }

        public static bool checkMurderAttempt(PlayerControl killer, PlayerControl target, bool isMeetingStart = false)
        {
            // Modified vanilla checks
            if (AmongUsClient.Instance.IsGameOver) return false;
            if (killer == null || killer.Data == null || killer.Data.IsDead || killer.Data.Disconnected) return false; // Allow non Impostor kills compared to vanilla code
            if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return false; // Allow killing players in vents compared to vanilla code

            // Block impostor shielded kill
            if (target.hasModifier(RoleModifierTypes.MedicShield))
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shieldedMurderAttempt(killer.PlayerId, target.PlayerId);
                return false;
            }

            // Block impostor not fully grown mini kill
            else if (target.isRole(CustomRoleTypes.Mini) && !target.role<Mini>().isGrownUp)
            {
                return false;
            }

            // Block Time Master with time shield
            else if (target.isRole(CustomRoleTypes.TimeMaster) && !target.role<TimeMaster>().shieldActive)
            {
                if (!isMeetingStart)
                { // Only rewind the attempt was not called because a meeting startet 
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.timeMasterRewindTime(target.PlayerId);
                }
                return false;
            }
            return true;
        }

        public static bool checkMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false)
        {
            // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
            // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible

            bool attemptIsSuccessful = checkMurderAttempt(killer, target, isMeetingStart);
            if (attemptIsSuccessful)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.uncheckedMurderPlayer(killer.PlayerId, target.PlayerId);
            }
            return attemptIsSuccessful;
        }

        public static void shareGameVersion()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable, -1);
            writer.Write((byte)TheOtherRolesPlugin.Version.Major);
            writer.Write((byte)TheOtherRolesPlugin.Version.Minor);
            writer.Write((byte)TheOtherRolesPlugin.Version.Build);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TheOtherRolesPlugin.Version.Revision < 0 ? 0xFF : TheOtherRolesPlugin.Version.Revision));
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.versionHandshake(TheOtherRolesPlugin.Version.Major, TheOtherRolesPlugin.Version.Minor, TheOtherRolesPlugin.Version.Build, TheOtherRolesPlugin.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static List<string> WordWrap(string text, int maxLineLength)
        {
            var list = new List<string>();

            int currentIndex;
            var lastWrap = 0;
            var whitespace = new[] { ' ', '\r', '\n', '\t' };
            do
            {
                currentIndex = lastWrap + maxLineLength > text.Length ? text.Length : (text.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t', '。', '、', '…', 'は', 'が', 'に', 'を', 'の' }, Math.Min(text.Length - 1, lastWrap + maxLineLength)) + 1);
                if (currentIndex <= lastWrap)
                    currentIndex = Math.Min(lastWrap + maxLineLength, text.Length);
                list.Add(text.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
                lastWrap = currentIndex;
            } while (currentIndex < text.Length);

            return list;
        }
    }
}
