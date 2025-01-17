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

        public static event UnityAction<List<AchievementData>> OnAchievementsFetch;
        public static event UnityAction OnAchievementsFetchError;

        public static event UnityAction<List<AchievementsGroupData>> OnAchievementsFetchGroups;
        public static event UnityAction<List<PlayerAchievement>> OnAchievementsFetchPlayer;

        public static event UnityAction<string> OnAchievementsUnlock;
        public static event UnityAction<string> OnAchievementsUnlockError;

        public static event UnityAction<string> OnAchievementsProgress;
        public static event UnityAction<string> OnAchievementsProgressError;

        private static event Action _onAchievementsOpen;
        private static event Action _onAchievementsClose;

        private static event Action<string> _onAchievementsUnlock;
        private static event Action<string> _onAchievementsUnlockError;

        private static event Action<string> _onAchievementsProgress;
        private static event Action<string> _onAchievementsProgressError;
        #endregion

        #region Callbacks
        private void CallAchievementsOpen() { _onAchievementsOpen?.Invoke(); OnAchievementsOpen?.Invoke(); }
        private void CallAchievementsClose() { _onAchievementsClose?.Invoke(); OnAchievementsClose?.Invoke(); }

        private void CallAchievementsFetchList(List<AchievementData> achievementsData) => OnAchievementsFetch?.Invoke(achievementsData);
        private void CallAchievementsFetch(string achievementsData) => OnAchievementsFetch?.Invoke(UtilityJSON.GetList<AchievementData>(achievementsData));
        private void CallAchievementsFetchError() => OnAchievementsFetchError?.Invoke();

        private void CallAchievementsFetchGroupsList(List<AchievementsGroupData> achievementsData) => OnAchievementsFetchGroups?.Invoke(achievementsData);
        private void CallPlayerAchievementsFetchList(List<PlayerAchievement> achievementsData) => OnAchievementsFetchPlayer?.Invoke(achievementsData);
        private void CallAchievementsFetchGroups(string achievementsGroupsData) => OnAchievementsFetchGroups?.Invoke(UtilityJSON.GetList<AchievementsGroupData>(achievementsGroupsData));
        private void CallPlayerAchievementsFetch(string achievementsPlayerData) => OnAchievementsFetchPlayer?.Invoke(UtilityJSON.GetList<PlayerAchievement>(achievementsPlayerData));

        private void CallAchievementsUnlock(string achievement) { _onAchievementsUnlock?.Invoke(achievement); OnAchievementsUnlock?.Invoke(achievement); }
        private void CallAchievementsUnlockError(string error) { _onAchievementsUnlockError?.Invoke(error); OnAchievementsUnlockError?.Invoke(error); }

        private void CallAchievementsProgress(string idOrTag) { OnAchievementsProgress?.Invoke(idOrTag); _onAchievementsProgress?.Invoke(idOrTag); }
        private void CallAchievementsProgressError() => CallAchievementsProgressError("");
        private void CallAchievementsProgressError(string error) { OnAchievementsProgressError?.Invoke(error); _onAchievementsProgressError?.Invoke(error); }
        #endregion

        private void OnEnable()
        {
            CoreSDK.Achievements.OnAchievementsOpen += () => CallAchievementsOpen();
            CoreSDK.Achievements.OnAchievementsClose += () => CallAchievementsOpen();

            CoreSDK.Achievements.OnAchievementsFetch += (List<AchievementData> achievementsData) => CallAchievementsFetchList(achievementsData);
            CoreSDK.Achievements.OnAchievementsFetchError += () => CallAchievementsFetchError();

            CoreSDK.Achievements.OnAchievementsFetchGroups += (List<AchievementsGroupData> achievementsData) => CallAchievementsFetchGroupsList(achievementsData);
            CoreSDK.Achievements.OnAchievementsFetchPlayer += (List<PlayerAchievement> achievementsData) => CallPlayerAchievementsFetchList(achievementsData);

            CoreSDK.Achievements.OnAchievementsUnlock += (string achievement) => CallAchievementsUnlock(achievement);
            CoreSDK.Achievements.OnAchievementsUnlockError += (string error) => CallAchievementsUnlockError(error);

            CoreSDK.Achievements.OnAchievementsSetProgress += (string idOrTag) => CallAchievementsProgress(idOrTag);
            CoreSDK.Achievements.OnAchievementsSetProgressError += (string error) => CallAchievementsProgressError(error);
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
            CoreSDK.Achievements.Open();
#endif
        }


        public static void Fetch()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Fetch();
#else
            CoreSDK.Achievements.Fetch();
#endif
        }


        public static void Unlock(string idOrTag, Action<string> onUnlock = null, Action<string> onUnlockError = null)
        {
            _onAchievementsUnlock = onUnlock;
            _onAchievementsUnlockError = onUnlockError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Unlock(idOrTag);
#else
            CoreSDK.Achievements.Unlock(idOrTag);
#endif
        }

        public static void SetProgress(string idOrTag, int progress, Action<string> onProgress = null, Action<string> onProgressError = null)
        {
            _onAchievementsProgress = onProgress;
            _onAchievementsProgressError = onProgressError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_SetProgress(idOrTag,progress);
#else
            CoreSDK.Achievements.SetProgress(idOrTag, progress);
#endif
        }


        public static bool Has(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_Achievements_Has(idOrTag) == "true";
#else
           return CoreSDK.Achievements.Has(idOrTag);
#endif
        }

        public static int GetProgress(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_Achievements_GetProgress(idOrTag);
#else
           return CoreSDK.Achievements.GetProgress(idOrTag);
#endif
        }
    }
}
