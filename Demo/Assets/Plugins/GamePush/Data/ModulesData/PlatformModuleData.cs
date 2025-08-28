
using System;

namespace GamePush
{

    [System.Serializable]
    public static class ProgressSaveFormat
    {
        public const string Local = "LOCAL";
        public const string Platform = "PLATFORM";
        public const string Cloud = "CLOUD";
    }

    public enum Platform : byte
    {
        NONE = 0,
        YANDEX,
        VK,
        CRAZY_GAMES,
        GAME_DISTRIBUTION,
        GAME_MONETIZE,
        OK,
        SMARTMARKET,
        GAMEPIX,
        POKI,
        VK_PLAY,
        WG_PLAYGROUND,
        KONGREGATE,
        GOOGLE_PLAY,
        PLAYDECK,
        CUSTOM,
        TELEGRAM,
        ANDROID,
        APP_GALLERY,
        GALAXY_STORE,
        ONE_STORE,
        AMAZON_APPSTORE,
        XIAOMI_GETAPPS,
        APTOIDE,
        RUSTORE,
        CUSTOM_ANDROID,
        FOTOSTRANA,
        Y8,
        BEELINE,
        PARTNER,
        PLAYDIA,
        YOUTUBE,
        XIAOMI_GAMECENTER,
        ARCADIUM
    }

    public static class PlatformTypes
    {
        public static Platform ConvertToEnum(string platform)
        {
            if (Enum.TryParse(platform, out Platform result))
            {
                return result;
            }
            return Platform.NONE;
        }
    }
}
