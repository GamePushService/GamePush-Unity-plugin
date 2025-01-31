
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
        PARTNER
    }

    public static class PlatformTypes
    {
        public const string NONE = "NONE";
        public const string YANDEX = "YANDEX";
        public const string VK = "VK";
        public const string CRAZY_GAMES = "CRAZY_GAMES";
        public const string GAME_DISTRIBUTION = "GAME_DISTRIBUTION";
        public const string GAME_MONETIZE = "GAME_MONETIZE";
        public const string OK = "OK";
        public const string SMARTMARKET = "SMARTMARKET";
        public const string GAMEPIX = "GAMEPIX";
        public const string POKI = "POKI";
        public const string VK_PLAY = "VK_PLAY";
        public const string WG_PLAYGROUND = "WG_PLAYGROUND";
        public const string KONGREGATE = "KONGREGATE";
        public const string GOOGLE_PLAY = "GOOGLE_PLAY";
        public const string PLAYDECK = "PLAYDECK";
        public const string CUSTOM = "CUSTOM";
        public const string TELEGRAM = "TELEGRAM";
        public const string ANDROID = "ANDROID";
        public const string APP_GALLERY = "APP_GALLERY";
        public const string GALAXY_STORE = "GALAXY_STORE";
        public const string ONE_STORE = "ONE_STORE";
        public const string AMAZON_APPSTORE = "AMAZON_APPSTORE";
        public const string XIAOMI_GETAPPS = "XIAOMI_GETAPPS";
        public const string APTOIDE = "APTOIDE";
        public const string RUSTORE = "RUSTORE";
        public const string CUSTOM_ANDROID = "CUSTOM_ANDROID";
        public const string FOTOSTRANA = "FOTOSTRANA";
        public const string Y8 = "Y8";
        public const string BEELINE = "BEELINE";
        public const string PARTNER = "PARTNER";
        


        public static Platform ConvertToEnum(string platform)
        {
            return platform switch
            {
                YANDEX => Platform.YANDEX,
                VK => Platform.VK,
                OK => Platform.OK,
                CRAZY_GAMES => Platform.CRAZY_GAMES,
                GAME_DISTRIBUTION => Platform.GAME_DISTRIBUTION,
                GAME_MONETIZE => Platform.GAME_MONETIZE,
                SMARTMARKET => Platform.SMARTMARKET,
                GAMEPIX => Platform.GAMEPIX,
                POKI => Platform.POKI,
                VK_PLAY => Platform.VK_PLAY,
                WG_PLAYGROUND => Platform.WG_PLAYGROUND,
                KONGREGATE => Platform.KONGREGATE,
                TELEGRAM => Platform.TELEGRAM,
                PLAYDECK => Platform.PLAYDECK,
                CUSTOM => Platform.CUSTOM,
                ANDROID => Platform.ANDROID,
                GOOGLE_PLAY => Platform.GOOGLE_PLAY,
                APP_GALLERY => Platform.APP_GALLERY,
                GALAXY_STORE => Platform.GALAXY_STORE,
                ONE_STORE => Platform.ONE_STORE,
                AMAZON_APPSTORE => Platform.AMAZON_APPSTORE,
                XIAOMI_GETAPPS => Platform.XIAOMI_GETAPPS,
                APTOIDE => Platform.APTOIDE,
                RUSTORE => Platform.RUSTORE,
                CUSTOM_ANDROID => Platform.CUSTOM_ANDROID,
                FOTOSTRANA => Platform.FOTOSTRANA,
                Y8 => Platform.Y8,
                BEELINE => Platform.BEELINE,
                PARTNER => Platform.PARTNER,
                _ => Platform.NONE
            };

        }
    }


}
