using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Achievements : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Achievements);

        #region Events
        public static event UnityAction OnAchievementsOpen;
        public static event UnityAction OnAchievementsClose;

        public static event UnityAction<List<AchievementsFetch>> OnAchievementsFetch;
        public static event UnityAction OnAchievementsFetchError;

        public static event UnityAction<List<AchievementsFetchGroups>> OnAchievementsFetchGroups;
        public static event UnityAction<List<AchievementsFetchPlayer>> OnAchievementsFetchPlayer;

        public static event UnityAction<string> OnAchievementsUnlock;
        public static event UnityAction<string> OnAchievementsUnlockError;

        public static event UnityAction<string> OnAchievementsProgress;
        public static event UnityAction OnAchievementsProgressError;

        private static event Action _onAchievementsOpen;
        private static event Action _onAchievementsClose;

        private static event Action<string> _onAchievementsUnlock;
        private static event Action<string> _onAchievementsUnlockError;

        private static event Action<string> _onAchievementsProgress;
        private static event Action _onAchievementsProgressError;
        #endregion

        #region Callbacks
        private void CallAchievementsOpen() { _onAchievementsOpen?.Invoke(); OnAchievementsOpen?.Invoke(); }
        private void CallAchievementsClose() { _onAchievementsClose?.Invoke(); OnAchievementsClose?.Invoke(); }

        private void CallAchievementsFetchList(List<AchievementsFetch> achievementsData) => OnAchievementsFetch?.Invoke(achievementsData);
        private void CallAchievementsFetch(string achievementsData) => OnAchievementsFetch?.Invoke(UtilityJSON.GetList<AchievementsFetch>(achievementsData));
        private void CallAchievementsFetchError() => OnAchievementsFetchError?.Invoke();

        private void CallAchievementsFetchGroupsList(List<AchievementsFetchGroups> achievementsData) => OnAchievementsFetchGroups?.Invoke(achievementsData);
        private void CallPlayerAchievementsFetchList(List<AchievementsFetchPlayer> achievementsData) => OnAchievementsFetchPlayer?.Invoke(achievementsData);
        private void CallAchievementsFetchGroups(string achievementsGroupsData) => OnAchievementsFetchGroups?.Invoke(UtilityJSON.GetList<AchievementsFetchGroups>(achievementsGroupsData));
        private void CallPlayerAchievementsFetch(string achievementsPlayerData) => OnAchievementsFetchPlayer?.Invoke(UtilityJSON.GetList<AchievementsFetchPlayer>(achievementsPlayerData));

        private void CallAchievementsUnlock(string achievement) { _onAchievementsUnlock?.Invoke(achievement); OnAchievementsUnlock?.Invoke(achievement); }
        private void CallAchievementsUnlockError(string error) { _onAchievementsUnlockError?.Invoke(error); OnAchievementsUnlockError?.Invoke(error); }

        private void CallAchievementsProgress(string idOrTag) { OnAchievementsProgress?.Invoke(idOrTag); _onAchievementsProgress?.Invoke(idOrTag); }
        private void CallAchievementsProgressError() { OnAchievementsProgressError?.Invoke(); _onAchievementsProgressError?.Invoke(); }
        #endregion

        private void OnEnable()
        {
            CoreSDK.achievements.OnAchievementsOpen += () => CallAchievementsOpen();
            CoreSDK.achievements.OnAchievementsClose += () => CallAchievementsOpen();

            CoreSDK.achievements.OnAchievementsFetch += (List<AchievementsFetch> achievementsData) => CallAchievementsFetchList(achievementsData);
            CoreSDK.achievements.OnAchievementsFetchError += () => CallAchievementsFetchError();

            CoreSDK.achievements.OnAchievementsFetchGroups += (List<AchievementsFetchGroups> achievementsData) => CallAchievementsFetchGroupsList(achievementsData);
            CoreSDK.achievements.OnAchievementsFetchPlayer += (List<AchievementsFetchPlayer> achievementsData) => CallPlayerAchievementsFetchList(achievementsData);

            CoreSDK.achievements.OnAchievementsUnlock += (string achievement) => CallAchievementsUnlock(achievement);
            CoreSDK.achievements.OnAchievementsUnlockError += (string error) => CallAchievementsUnlockError(error);

            CoreSDK.achievements.OnAchievementsProgress += (string idOrTag) => CallAchievementsProgress(idOrTag);
            CoreSDK.achievements.OnAchievementsProgressError += () => CallAchievementsProgressError();
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Achievements_Open();

        [DllImport("__Internal")]
        private static extern void GP_Achievements_Fetch();

        [DllImport("__Internal")]
        private static extern void GP_Achievements_Unlock(string idOrTag);

        [DllImport("__Internal")]
        private static extern void GP_Achievements_SetProgress(string idOrTag, int progress);

        [DllImport("__Internal")]
        private static extern string GP_Achievements_Has(string idOrTag);

        [DllImport("__Internal")]
        private static extern int GP_Achievements_GetProgress(string idOrTag);

#endif

        public static void Open(Action onOpen = null, Action onClose = null)
        {
            _onAchievementsOpen = onOpen;
            _onAchievementsClose = onClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Open();
#else
            CoreSDK.achievements.Open();
#endif
        }


        public static void Fetch()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Fetch();
#else
            CoreSDK.achievements.Fetch();
#endif
        }


        public static void Unlock(string idOrTag, Action<string> onUnlock = null, Action<string> onUnlockError = null)
        {
            _onAchievementsUnlock = onUnlock;
            _onAchievementsUnlockError = onUnlockError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Unlock(idOrTag);
#else
            CoreSDK.achievements.Unlock(idOrTag);
#endif
        }

        public static void SetProgress(string idOrTag, int progress, Action<string> onProgress = null, Action onProgressError = null)
        {
            _onAchievementsProgress = onProgress;
            _onAchievementsProgressError = onProgressError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_SetProgress(idOrTag,progress);
#else
            CoreSDK.achievements.SetProgress(idOrTag, progress);
#endif
        }


        public static bool Has(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_Achievements_Has(idOrTag) == "true";
#else
           return CoreSDK.achievements.Has(idOrTag);
#endif
        }

        public static int GetProgress(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_Achievements_GetProgress(idOrTag);
#else
           return CoreSDK.achievements.GetProgress(idOrTag);
#endif
        }
    }
}
