using UnityEngine;

namespace GamePush.Overlays.Widgets
{
    /// <summary>
    /// Chrome captions for the overlays. Language follows the same rule as NativeCore:
    /// Russian system language gets RU, everything else gets EN.
    /// </summary>
    public static class GP_OverlayStrings
    {
        public static bool IsRussian => Application.systemLanguage == SystemLanguage.Russian;

        static string Pick(string ru, string en) => IsRussian ? ru : en;

        public static string Loading => Pick("Загрузка...", "Loading...");
        public static string Empty => Pick("Пусто", "Nothing here yet");
        public static string Error => Pick("Не удалось загрузить", "Failed to load");
        public static string Retry => Pick("Повторить", "Retry");
        public static string Close => Pick("Закрыть", "Close");
        public static string Back => Pick("Назад", "Back");
        public static string Ok => Pick("ОК", "OK");
        public static string Cancel => Pick("Отмена", "Cancel");
        public static string Yes => Pick("Да", "Yes");
        public static string No => Pick("Нет", "No");
        public static string Send => Pick("Отправить", "Send");
        public static string LoadMore => Pick("Показать ещё", "Load more");

        public static string Achievements => Pick("Достижения", "Achievements");
        public static string Leaderboard => Pick("Таблица лидеров", "Leaderboard");
        public static string Chat => Pick("Чат", "Chat");
        public static string Feed => Pick("Лента", "Feed");
        public static string Document => Pick("Документ", "Document");
        public static string Games => Pick("Игры", "Games");
        public static string Feedbacks => Pick("Обращения", "Feedback");
        public static string Members => Pick("Участники", "Members");
        public static string Messages => Pick("Сообщения", "Messages");
        public static string All => Pick("Все", "All");
        public static string Muted => Pick("Мут", "Mute");
        public static string Confirm => Pick("Подтверждение", "Confirm");

        public static string Locked => Pick("Закрыто", "Locked");
        public static string Unlocked => Pick("Получено", "Unlocked");
        public static string UnlockedProgress(int unlocked, int total, string accentHex) =>
            Pick("Разблокировано: ", "Unlocked: ") + "<color=#" + accentHex + ">" + unlocked + "</color> / " +
            total;
        public static string HiddenAchievement => Pick("Секретное достижение", "Hidden achievement");
        public static string Position => Pick("Место", "Rank");
        public static string Score => Pick("Очки", "Score");
        public static string You => Pick("Вы", "You");
        public static string PlayerNumber(int id) => Pick("Игрок #", "Player #") + id;
        public static string MessagePlaceholder => Pick("Сообщение...", "Message...");
        public static string NewFeedback => Pick("Новое обращение", "New request");
        public static string Play => Pick("Играть", "Play");
        public static string AdSoon => Pick("Реклама через", "Ad in");
        public static string AdUnavailable => Pick("Реклама сейчас недоступна", "No ads available right now");
        public static string Skip => Pick("Пропустить", "Skip");

        public static string Seconds(float value) => Mathf.CeilToInt(Mathf.Max(0f, value)).ToString();
    }
}
