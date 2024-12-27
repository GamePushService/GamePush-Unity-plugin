using System;
using System.Collections.Generic;

namespace GamePush.Core
{
    public class AchievementsModule
    {
        public event Action OnAchievementsOpen;
        public event Action OnAchievementsClose;

        public event Action<List<AchievementsFetch>> OnAchievementsFetch;
        public event Action OnAchievementsFetchError;

        public event Action<List<AchievementsFetchGroups>> OnAchievementsFetchGroups;
        public event Action<List<AchievementsFetchPlayer>> OnAchievementsFetchPlayer;

        public event Action<string> OnAchievementsUnlock;
        public event Action<string> OnAchievementsUnlockError;

        public event Action<string> OnAchievementsProgress;
        public event Action OnAchievementsProgressError;

        public event Action OnShowAcievementUnlock;
        public event Action OnShowAcievementProgress;

        public event Action OnShowAcievementsList;

        public void Open(Action onOpen = null, Action onClose = null)
        {

        }

        public void Fetch()
        {

        }

        public void Unlock(string idOrTag)
        {

        }

        public bool Has(string idOrTag)
        {
            return false;
        }

        public void SetProgress(string idOrTag, int progress)
        {
            
        }

        public int GetProgress(string idOrTag)
        {
            return 0;
        }
    }
}
