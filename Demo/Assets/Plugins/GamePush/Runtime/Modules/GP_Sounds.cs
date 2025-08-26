using System.Runtime.InteropServices;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Sounds : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Sounds);

        #region Events

        public static event UnityAction OnMute;
        public static event UnityAction OnMuteSFX;
        public static event UnityAction OnMuteMusic;
        
        public static event UnityAction OnUnmute;
        public static event UnityAction OnUnmuteSFX;
        public static event UnityAction OnUnmuteMusic;

        #endregion

        #region DLL Imports

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

        #endregion
    }
}