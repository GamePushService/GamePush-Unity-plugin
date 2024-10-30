
namespace GamePush
{
    public enum Language : byte
    {
        English,
        Russian,
        Turkish,
        French,
        Italian,
        German,
        Spanish,
        Chineese,
        Portuguese,
        Korean,
        Japanese,
        Arab,
        Hindi,
        Indonesian,
    }

    public static class LanguageTypes
    {
        public const string English = "en";
        public const string Russian = "ru";
        public const string Turkish = "tr";
        public const string French = "fr";
        public const string Italian = "it";
        public const string German = "de";
        public const string Spanish = "es";
        public const string Chineese = "zh";
        public const string Portuguese = "pt";
        public const string Korean = "ko";
        public const string Japanese = "ja";
        public const string Arab = "ar";
        public const string Hindi = "hi";
        public const string Indonesian = "id";

        public static Language ConvertToEnum(string lang)
        {
            return lang switch
            {
                English => Language.English,
                Russian => Language.Russian,
                Turkish => Language.Turkish,
                French => Language.French,
                Italian => Language.Italian,
                German => Language.German,
                Spanish => Language.Spanish,
                Chineese => Language.Chineese,
                Portuguese => Language.Portuguese,
                Korean => Language.Korean,
                Japanese => Language.Japanese,
                Arab => Language.Arab,
                Hindi => Language.Hindi,
                Indonesian => Language.Indonesian,
                _ => Language.English
            };
        }

        public static string ConvertToString(Language lang)
        {
            return lang switch
            {
                Language.English => English,
                Language.Russian => Russian,
                Language.Turkish => Turkish,
                Language.French => French,
                Language.Italian => Italian,
                Language.German => German,
                Language.Spanish => Spanish,
                Language.Chineese => Chineese,
                Language.Portuguese => Portuguese,
                Language.Korean => Korean,
                Language.Japanese => Japanese,
                Language.Arab => Arab,
                Language.Hindi => Hindi,
                Language.Indonesian => Indonesian,
                _ => English
            };
        }
    }
}
