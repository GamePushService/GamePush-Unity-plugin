using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GamePush;
using GamePush.Native;
using GamePush.Overlays;
using GamePush.Overlays.Views;
using GamePush.Overlays.Widgets;

namespace GamePushEditor.Overlays
{
    internal enum GP_OverlayPreviewState
    {
        Content,
        Loading,
        Empty,
        Error
    }

    internal enum GP_OverlayPreviewLanguage
    {
        Russian,
        English
    }

    internal static class GP_OverlayPreviewFixtures
    {
        internal static void Populate(GameObject root, GP_OverlayKind kind, GP_OverlaySkin skin,
            GP_OverlayPreviewState state, GP_OverlayPreviewLanguage language)
        {
            var view = root != null ? root.GetComponent<GP_OverlayView>() : null;
            if (view == null)
                return;
            var ru = language == GP_OverlayPreviewLanguage.Russian;

            Set(view.titleLabel, Title(kind, ru));
            if (state != GP_OverlayPreviewState.Content)
            {
                Set(view.statusLabel, state == GP_OverlayPreviewState.Loading ? (ru ? "Загрузка..." : "Loading...")
                    : state == GP_OverlayPreviewState.Empty ? (ru ? "Пока ничего нет" : "Nothing here yet")
                    : (ru ? "Что-то пошло не так" : "Something went wrong"));
                view.statusLabel.gameObject.SetActive(true);
                return;
            }

            if (view.statusLabel != null)
                view.statusLabel.gameObject.SetActive(false);

            switch (kind)
            {
                case GP_OverlayKind.Confirm:
                    PopulateConfirm(root.GetComponent<GP_ConfirmView>(), ru);
                    break;
                case GP_OverlayKind.Achievements:
                    PopulateAchievements(root.GetComponent<GP_AchievementsView>(), skin, ru);
                    break;
                case GP_OverlayKind.Leaderboard:
                    PopulateLeaderboard(root.GetComponent<GP_LeaderboardView>(), skin);
                    break;
                case GP_OverlayKind.Chat:
                    PopulateChat(root.GetComponent<GP_ChatView>(), skin, ru);
                    break;
                case GP_OverlayKind.Document:
                    Set(root.GetComponent<GP_DocumentView>()?.contentLabel, ru
                        ? "1. Общие положения\n\nНастоящий документ описывает правила сервиса GamePush.\n\n" +
                        "2. Использование сервиса\n\nПользователь принимает условия при продолжении работы с игрой.\n\n" +
                        "3. Конфиденциальность\n\nДанные обрабатываются только для работы игровых функций."
                        : "1. General\n\nThis document describes the GamePush service terms.\n\n" +
                        "2. Service use\n\nThe player accepts these terms by continuing to use the game.\n\n" +
                        "3. Privacy\n\nData is processed only to provide game features.");
                    break;
                case GP_OverlayKind.GamesCollections:
                    PopulateList(root.GetComponent<GP_GamesCollectionsView>()?.list, skin.gameCard, 8,
                        PopulateGameCard);
                    break;
                case GP_OverlayKind.Feedbacks:
                    PopulateFeedbacks(root.GetComponent<GP_FeedbacksView>(), skin, ru);
                    break;
                case GP_OverlayKind.AdCountdown:
                    var countdown = root.GetComponent<GP_AdCountdownView>();
                    Set(countdown?.captionLabel, ru ? "Реклама через" : "Ad starts in");
                    Set(countdown?.countdownLabel, ru ? "3 сек." : "3 sec.");
                    SetButton(countdown?.skipButton, ru ? "Пропустить" : "Skip");
                    break;
                case GP_OverlayKind.AdFailed:
                    var failed = root.GetComponent<GP_AdFailedView>();
                    Set(failed?.textLabel, ru ? "Реклама сейчас недоступна" : "The ad is unavailable");
                    SetButton(failed?.okButton, "OK");
                    break;
            }
        }

        static void PopulateConfirm(GP_ConfirmView view, bool ru)
        {
            if (view == null)
                return;
            Set(view.messageLabel, ru ? "Вы уверены, что хотите выполнить это действие?"
                : "Are you sure you want to continue?");
            Set(view.confirmLabel, ru ? "Подтвердить" : "Confirm");
            Set(view.cancelLabel, ru ? "Отмена" : "Cancel");
        }

        static void PopulateAchievements(GP_AchievementsView view, GP_OverlaySkin skin, bool ru)
        {
            if (view == null)
                return;
            Set(view.counterLabel, (ru ? "Разблокировано: " : "Unlocked: ") + "<color=#" + skin.AccentHex +
                ">3</color> / 8");
            if (view.counterLabel != null)
                view.counterLabel.richText = true;
            var labels = ru ? new[] { "Все", "Прогресс", "Испытания" }
                : new[] { "All", "Progress", "Challenges" };
            var counts = new[] { "3 / 8", "2 / 4", "1 / 4" };
            PopulateGroups(view.groupRail, view.groupButtonTemplate, labels, counts);
            PopulateGroups(view.compactGroupRail, view.compactGroupButtonTemplate, labels, counts);
            PopulateList(view.list, skin.achievementRow, 6, (row, index) =>
            {
                var achievement = row.GetComponent<GP_AchievementRow>();
                if (achievement == null)
                    return;
                achievement.Bind(new AchievementsFetch
                {
                    name = index % 2 == 0 ? "Первый шаг" : "Исследователь",
                    description = ru ? "Выполните условие достижения" : "Complete the achievement",
                    icon = "",
                    maxProgress = index < 3 ? 1 : 10,
                    lockedVisible = true,
                    lockedDescriptionVisible = true
                }, new AchievementsFetchPlayer
                {
                    progress = index < 3 ? 1 : 3,
                    unlocked = index < 3
                }, index);
            });
        }

        static void PopulateGroups(RectTransform rail, Button template, string[] labels, string[] counts = null)
        {
            if (rail == null || template == null)
                return;
            for (var i = 0; i < labels.Length; i++)
            {
                var button = Object.Instantiate(template, rail);
                button.gameObject.SetActive(true);
                var chip = button.GetComponent<GP_OverlayChip>();
                if (chip != null)
                    chip.Bind(labels[i], counts != null && i < counts.Length ? counts[i] : "", i == 0);
                else
                    SetButton(button, labels[i]);
            }
        }

        static void PopulateChat(GP_ChatView view, GP_OverlaySkin skin, bool ru)
        {
            if (view == null)
                return;
            PopulateList(view.messageList, skin.messageRow, 5, (row, index) =>
            {
                SetNamed(row, "Author", index % 2 == 0 ? "Алекс" : "Лира");
                SetNamed(row, "Time", "14:" + (32 + index));
                SetNamed(row, "Text", index % 2 == 0 ? "Кто сегодня играет?" : "Я с вами, собираю группу.");
            });
            PopulateList(view.memberList, skin.memberRow, 6, (row, index) =>
            {
                SetNamed(row, "Name", index % 2 == 0 ? "Алекс" : "Лира");
                var member = row.GetComponent<GP_MemberRow>();
                if (member != null && member.onlineDot != null)
                {
                    GP_OverlayTone.Paint(member.onlineDot, skin,
                        index < 4 ? GP_OverlayColorRole.Accent : GP_OverlayColorRole.TextMuted);
                    GP_LayoutSquare.Lock(member.onlineDot, 14f);
                }
            });
            var wide = view.layoutMode == null || view.layoutMode.Mode == GP_LayoutMode.Wide;
            if (view.compactTabs != null)
                view.compactTabs.SetActive(!wide);
            if (view.messagesPanel != null)
                view.messagesPanel.SetActive(true);
            if (view.membersPanel != null)
                view.membersPanel.SetActive(wide);
            if (view.composer != null)
                view.composer.gameObject.SetActive(true);
            if (view.messagesTab != null)
            {
                var chip = view.messagesTab.GetComponent<GP_OverlayChip>();
                chip?.Bind(ru ? "Сообщения" : "Messages", null, true);
            }
            if (view.membersTab != null)
            {
                var chip = view.membersTab.GetComponent<GP_OverlayChip>();
                chip?.Bind(ru ? "Участники" : "Members", null, false);
            }
            SetButton(view.sendButton, ru ? "Отправить" : "Send");
        }

        static void PopulateFeedbacks(GP_FeedbacksView view, GP_OverlaySkin skin, bool ru)
        {
            if (view == null)
                return;
            PopulateList(view.feedbackList, skin.feedbackRow, 5, (row, index) =>
            {
                SetNamed(row, "Text", index == 0 ? "Не отображается уровень" : "Обращение игрока");
                SetNamed(row, "Status", index < 2 ? "Новое" : "В работе");
                SetNamed(row, "Date", "21.05.2024");
                var feedback = row.GetComponent<GP_FeedbackRow>();
                if (feedback != null && feedback.selectedBar != null)
                    feedback.selectedBar.gameObject.SetActive(index == 0);
            });
            PopulateList(view.threadList, skin.messageRow, 4, (row, index) =>
            {
                SetNamed(row, "Author", index % 2 == 0 ? "Игрок" : "GamePush");
                SetNamed(row, "Time", "14:" + (20 + index));
                SetNamed(row, "Text", index % 2 == 0 ? "После запуска уровень не появляется."
                    : "Спасибо! Уточните платформу и версию игры.");
            });
            Set(view.threadTitle, "Не отображается уровень");
            SetButton(view.sendButton, ru ? "Отправить" : "Send");
            var wide = view.layoutMode == null || view.layoutMode.Mode == GP_LayoutMode.Wide;
            if (view.listPanel != null)
                view.listPanel.SetActive(true);
            if (view.threadPanel != null)
                view.threadPanel.SetActive(wide);
            if (view.backButton != null)
                view.backButton.gameObject.SetActive(false);
        }

        static NativeLeaderboardField[] PreviewLeaderboardFields()
        {
            return new[]
            {
                new NativeLeaderboardField { key = "score", name = "score" },
                new NativeLeaderboardField { key = "gold", name = "gold" }
            };
        }

        static void PopulateLeaderboard(GP_LeaderboardView view, GP_OverlaySkin skin)
        {
            var fields = PreviewLeaderboardFields();
            if (view != null && view.headerRow != null)
            {
                view.headerRow.gameObject.SetActive(true);
                view.headerRow.BindHeader(fields);
            }
            PopulateList(view != null ? view.list : null, skin.leaderboardRow, 8, PopulateLeaderboardRow);
        }

        static void PopulateLeaderboardRow(GameObject row, int index)
        {
            var component = row.GetComponent<GP_LeaderboardRow>();
            if (component == null)
                return;
            var fields = PreviewLeaderboardFields();
            var json = "{\"score\":" + (12500 - index * 620) + ",\"gold\":" + (40 - index) +
                       ",\"name\":\"Player\",\"avatar\":\"\"}";
            component.Bind(new NativeLeaderboardEntry
            {
                id = 100 + index,
                position = index + 1,
                name = index == 2 ? "Вы" : (index == 3 ? "Игрок с очень длинным ником" : "Игрок " + (index + 1)),
                avatar = "",
                score = 12500 - index * 620,
                json = json
            }, fields, index, index == 2, GP_LayoutMode.Wide);
        }

        static void PopulateGameCard(GameObject row, int index)
        {
            SetNamed(row, "Name", "Игра " + (index + 1));
            SetNamed(row, "Play", "Играть");
        }

        static void PopulateList(GP_OverlayList list, GameObject prefab, int count,
            System.Action<GameObject, int> populate)
        {
            if (list == null || prefab == null)
                return;
            list.Bind(count, populate, prefab);
        }

        static void SetButton(Button button, string value)
        {
            if (button != null)
                Set(button.GetComponentInChildren<TMP_Text>(true), value);
        }

        static void SetNamed(GameObject root, string name, string value)
        {
            if (root == null)
                return;
            foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.name == name)
                    Set(text, value);
            }
        }

        static void Set(TMP_Text text, string value)
        {
            if (text != null)
                text.text = value ?? "";
        }

        static string Title(GP_OverlayKind kind, bool ru)
        {
            switch (kind)
            {
                case GP_OverlayKind.Achievements: return ru ? "Достижения" : "Achievements";
                case GP_OverlayKind.Leaderboard: return ru ? "Таблица лидеров" : "Leaderboard";
                case GP_OverlayKind.Chat: return ru ? "Чат" : "Chat";
                case GP_OverlayKind.Document: return ru ? "Документ" : "Document";
                case GP_OverlayKind.GamesCollections: return ru ? "Игры" : "Games";
                case GP_OverlayKind.Feedbacks: return ru ? "Обратная связь" : "Feedback";
                case GP_OverlayKind.Confirm: return ru ? "Подтверждение" : "Confirm";
                default: return "";
            }
        }
    }
}
