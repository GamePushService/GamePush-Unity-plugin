
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
        None = 0,
        YANDEX = 1,
        VK = 2,
        CRAZY_GAMES = 3,
        GAME_DISTRIBUTION = 4,
        GAME_MONETIZE = 5,
        OK = 6,
        SMARTMARKET = 7,
        GAMEPIX = 8,
        POKI = 9,
        VK_PLAY = 10,
        WG_PLAYGROUND = 11,
        KONGREGATE = 12,
        GOOGLE_PLAY = 13,
        PLAYDECK = 14,
        CUSTOM = 15,
        TELEGRAM
    }

    public static class PlatformTypes
    {
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
    }
}
