using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public enum SoundType {All, SFX, Music}
    public class GP_Sounds : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Sounds);
        
        private static Dictionary<SoundType, bool> muted = new Dictionary<SoundType, bool>()
        {
            { SoundType.All, false},
            { SoundType.Music, false},
            { SoundType.SFX, false}
        };
        
        #region Events

        public static event UnityAction OnMute;
        public static event UnityAction OnMuteSFX;
        public static event UnityAction OnMuteMusic;
        
        public static event UnityAction OnUnmute;
        public static event UnityAction OnUnmuteSFX;
        public static event UnityAction OnUnmuteMusic;

        #endregion

        #region DLL Imports
#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_Sounds_IsMuted();
        [DllImport("__Internal")]
        private static extern string GP_Sounds_IsMusicMuted();
        [DllImport("__Internal")]
        private static extern string GP_Sounds_IsSFXMuted();

        [DllImport("__Internal")]
        private static extern void GP_Sounds_Mute();
        [DllImport("__Internal")]
        private static extern void GP_Sounds_MuteSFX();
        [DllImport("__Internal")]
        private static extern void GP_Sounds_MuteMusic();
        [DllImport("__Internal")]
        private static extern void GP_Sounds_Unmute();
        [DllImport("__Internal")]
        private static extern void GP_Sounds_UnmuteSFX();
        [DllImport("__Internal")]
        private static extern void GP_Sounds_UnmuteMusic();
#endif
        #endregion

        public static bool IsMuted(SoundType soundType = SoundType.All)
        {
           
#if !UNITY_EDITOR && UNITY_WEBGL
            return soundType switch
            {
                SoundType.All => GP_Sounds_IsMuted() == "true",
                SoundType.Music => GP_Sounds_IsMusicMuted() == "true",
                SoundType.SFX => GP_Sounds_IsSFXMuted() == "true",
                _ => false
            };
#else
            bool isMuted = muted[soundType];
            ConsoleLog($"Is {soundType} muted: " + isMuted);
            return isMuted;
#endif
        }
        
        public static void Mute(SoundType soundType = SoundType.All)
        {
            ConsoleLog($"Mute {soundType}");
            
#if !UNITY_EDITOR && UNITY_WEBGL
            switch (soundType)
            {
                case SoundType.All: GP_Sounds_Mute(); break;
                case SoundType.Music: GP_Sounds_MuteMusic(); break;
                case SoundType.SFX: GP_Sounds_MuteSFX(); break;
            }
#else
            muted[soundType] = true;
            switch (soundType)
            {
                case SoundType.All: OnMute?.Invoke(); break;
                case SoundType.Music: OnMuteMusic?.Invoke(); break;
                case SoundType.SFX: OnMuteSFX?.Invoke(); break;
            }
#endif
        }
        public static void Unmute(SoundType soundType = SoundType.All)
        {
            ConsoleLog($"Unmute {soundType}");
#if !UNITY_EDITOR && UNITY_WEBGL
            switch (soundType)
            {
                case SoundType.All: GP_Sounds_Unmute(); break;
                case SoundType.Music: GP_Sounds_UnmuteMusic(); break;
                case SoundType.SFX: GP_Sounds_UnmuteSFX(); break;
            }
#else
            muted[soundType] = false;
            switch (soundType)
            {
                case SoundType.All: OnUnmute?.Invoke(); break;
                case SoundType.Music: OnUnmuteMusic?.Invoke(); break;
                case SoundType.SFX: OnUnmuteSFX?.Invoke(); break;
            }
#endif
        }
        
        #region Callbacks
        private void CallOnSoundsMute() => OnMute?.Invoke();
        private void CallOnSoundsMuteSFX() => OnMuteSFX?.Invoke();
        private void CallOnSoundsMuteMusic() => OnMuteMusic?.Invoke();
        private void CallOnSoundsUnmute() => OnUnmute?.Invoke();
        private void CallOnSoundsUnmuteSFX() => OnUnmuteSFX?.Invoke();
        private void CallOnSoundsUnmuteMusic() => OnUnmuteMusic?.Invoke();

        #endregion
        
        
    }
}