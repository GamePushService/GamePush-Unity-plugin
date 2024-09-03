
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
}
