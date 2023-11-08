using UnityEngine;

namespace GP_Utilities.Console
{
    public class GP_ConsoleController : MonoBehaviour
    {
        public static GP_ConsoleController Instance;
        private void Awake() => Instance = this;

        [SerializeField] public bool AchievementsConsoleLogs = true;
        [SerializeField] public bool AdsConsoleLogs = true;
        [SerializeField] public bool AnalyticsConsoleLogs = true;
        [SerializeField] public bool AppConsoleLogs = true;
        [SerializeField] public bool AvatarGeneratorConsoleLogs = true;
        [SerializeField] public bool ChannelConsoleLogs = true;
        [SerializeField] public bool DeviceConsoleLogs = true;
        [SerializeField] public bool DocumentsConsoleLogs = true;
        [SerializeField] public bool FilesConsoleLogs = true;
        [SerializeField] public bool FullscreenConsoleLogs = true;
        [SerializeField] public bool GameConsoleLogs = true;
        [SerializeField] public bool GamesCollectionsConsoleLogs = true;
        [SerializeField] public bool LanguageConsoleLogs = true;
        [SerializeField] public bool LeaderboardConsoleLogs = true;
        [SerializeField] public bool LeaderboardScopedConsoleLogs = true;
        [SerializeField] public bool PaymentsConsoleLogs = true;
        [SerializeField] public bool PlatformConsoleLogs = true;
        [SerializeField] public bool PlayerConsoleLogs = true;
        [SerializeField] public bool PlayersConsoleLogs = true;
        [SerializeField] public bool ServerConsoleLogs = true;
        [SerializeField] public bool SocialsConsoleLogs = true;
        [SerializeField] public bool SystemConsoleLogs = true;
        [SerializeField] public bool VariablesConsoleLogs = true;


        public void SwitchAll(bool value)
        {
            AchievementsConsoleLogs = value;
            AdsConsoleLogs = value;
            AnalyticsConsoleLogs = value;
            AppConsoleLogs = value;
            AvatarGeneratorConsoleLogs = value;
            ChannelConsoleLogs = value;
            DeviceConsoleLogs = value;
            DocumentsConsoleLogs = value;
            FilesConsoleLogs = value;
            FullscreenConsoleLogs = value;
            GameConsoleLogs = value;
            GamesCollectionsConsoleLogs = value;
            LanguageConsoleLogs = value;
            LeaderboardConsoleLogs = value;
            LeaderboardScopedConsoleLogs = value;
            PaymentsConsoleLogs = value;
            PlatformConsoleLogs = value;
            PlayerConsoleLogs = value;
            PlayersConsoleLogs = value;
            ServerConsoleLogs = value;
            SocialsConsoleLogs = value;
            SystemConsoleLogs = value;
            VariablesConsoleLogs = value;
        }
    }


}