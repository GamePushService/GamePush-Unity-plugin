using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Events;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Achievements : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Achievements);

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


        [DllImport("__Internal")]
        private static extern void GP_Achievements_Open();
        public static void Open(Action onOpen = null, Action onClose = null)
        {
            _onAchievementsOpen = onOpen;
            _onAchievementsClose = onClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Open();
#else
            ConsoleLog("OPEN");
            OnAchievementsOpen?.Invoke();
            _onAchievementsOpen?.Invoke();
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Achievements_Fetch();
        public static void Fetch()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Fetch();
#else
            ConsoleLog("FETCH");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Achievements_Unlock(string idOrTag);
        public static void Unlock(string idOrTag, Action<string> onUnlock = null, Action<string> onUnlockError = null)
        {
            _onAchievementsUnlock = onUnlock;
            _onAchievementsUnlockError = onUnlockError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_Unlock(idOrTag);
#else
            ConsoleLog("UNLOCK: " + idOrTag);
            OnAchievementsUnlock?.Invoke(idOrTag);
            _onAchievementsUnlock?.Invoke(idOrTag);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Achievements_SetProgress(string idOrTag, int progress);
        public static void SetProgress(string idOrTag, int progress, Action<string> onProgress = null, Action onProgressError = null)
        {
            _onAchievementsProgress = onProgress;
            _onAchievementsProgressError = onProgressError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Achievements_SetProgress(idOrTag,progress);
#else
            ConsoleLog("PROGRESS: " + idOrTag + " : " + progress);

            OnAchievementsProgress?.Invoke(idOrTag);
            _onAchievementsProgress?.Invoke(idOrTag);
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Achievements_Has(string idOrTag);
        public static bool Has(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_Achievements_Has(idOrTag) == "true";
#else
            ConsoleLog("HAS: " + idOrTag + " : TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern int GP_Achievements_GetProgress(string idOrTag);
        public static int GetProgress(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_Achievements_GetProgress(idOrTag);
#else
            ConsoleLog("GET PROGRESS: " + idOrTag + " : 100");
            return 100;
#endif
        }


        private void CallAchievementsFetch(string achievementsData) => OnAchievementsFetch?.Invoke(UtilityJSON.GetList<AchievementsFetch>(achievementsData));
        private void CallAchievementsFetchError() => OnAchievementsFetchError?.Invoke();

        private void CallAchievementsFetchGroups(string achievementsGroupsData) => OnAchievementsFetchGroups?.Invoke(UtilityJSON.GetList<AchievementsFetchGroups>(achievementsGroupsData));
        private void CallPlayerAchievementsFetch(string achievementsPlayerData) => OnAchievementsFetchPlayer?.Invoke(UtilityJSON.GetList<AchievementsFetchPlayer>(achievementsPlayerData));

        private void CallAchievementsOpen() { _onAchievementsOpen?.Invoke(); OnAchievementsOpen?.Invoke(); }
        private void CallAchievementsClose() { _onAchievementsClose?.Invoke(); OnAchievementsClose?.Invoke(); }

        private void CallAchievementsUnlock(string achievement) { _onAchievementsUnlock?.Invoke(achievement); OnAchievementsUnlock?.Invoke(achievement); }
        private void CallAchievementsUnlockError(string error) { _onAchievementsUnlockError?.Invoke(error); OnAchievementsUnlockError?.Invoke(error); }

        private void CallAchievementsProgress(string idOrTag) { OnAchievementsProgress?.Invoke(idOrTag); _onAchievementsProgress?.Invoke(idOrTag); }
        private void CallAchievementsProgressError() { OnAchievementsProgressError?.Invoke(); _onAchievementsProgressError?.Invoke(); }

    }

    [System.Serializable]
    public class AchievementsFetch
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public string lockedIcon;
        public string lockedIconSmall;
        public string rare;
        public int maxProgress;
        public int progressStep;
        public bool lockedVisible;
        public bool lockedDescriptionVisible;
    }

    [System.Serializable]
    public class AchievementsFetchGroups
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public int[] achievements;
    }

    [System.Serializable]
    public class AchievementsFetchPlayer
    {
        public int achievementId;
        public string createdAt;
        public int progress;
        public bool unlocked;
    }
}
