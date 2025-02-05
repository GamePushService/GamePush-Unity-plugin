using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamePush.Tools;

namespace GamePush.Core
{
    public class EventsModule
    {
        private List<EventData> _eventsList;
        private List<PlayerEvent> _playerEvents;
        public event Action<PlayerEvent> OnEventJoin;
        public event Action<string> OnEventJoinError;

        public void Init(List<EventData> events)
        {
            Logger.Log(events.ToString());
            _eventsList = events.Select(e => new EventData
            {
                id = e.id,
                tag = e.tag,
                name = LanguageTypes.GetTranslation(CoreSDK.currentLang, e.names) == "" ? e.names.en : LanguageTypes.GetTranslation(CoreSDK.currentLang, e.names),
                description = LanguageTypes.GetTranslation(CoreSDK.currentLang, e.descriptions) == "" ? e.descriptions.en : LanguageTypes.GetTranslation(CoreSDK.currentLang, e.descriptions),
                iconSmall = UtilityImage.ResizeImage(e.icon, 48, 48, false),
                icon = UtilityImage.ResizeImage(e.icon, 256, 256, false),
                timeLeft =  SetTimeLeft(e),
                isActive = SetIsActive(e)
            }).ToList();

            CoreSDK.Language.OnChangeLanguage += ChangeTranslation;
        }

        

        private int SetTimeLeft(EventData e)
        {
            var timeEnd = DateTime.TryParse(e.dateEnd, out var end) ? end : DateTime.MaxValue;
            var timeLeft = (timeEnd - CoreSDK.GetServerTime()).TotalSeconds;
            return timeLeft > 0 ? (int)timeLeft : 0;
        }
        private bool SetIsActive(EventData e)
        {
            var timeStart = DateTime.TryParse(e.dateStart, out var start) ? start : DateTime.MinValue;
            return CoreSDK.GetServerTime() >= timeStart && e.timeLeft > 0;
        }
        
        public void SetEvents(List<EventData> events)
        {
            
        }

        private void ChangeTranslation(Language lang)
        {
            foreach (var eventItem in _eventsList)
            {
                eventItem.name = LanguageTypes.GetTranslation(lang, eventItem.names) == "" ? eventItem.names.en : LanguageTypes.GetTranslation(lang, eventItem.names);
                eventItem.description = LanguageTypes.GetTranslation(lang, eventItem.descriptions) == "" ? eventItem.descriptions.en : LanguageTypes.GetTranslation(lang, eventItem.descriptions);
            }
        }
        
        
        
        private PlayerEventInfo GetEventInfo(string eventId)
        {
            var info = new PlayerEventInfo();
            var ev = _eventsList.FirstOrDefault(e => e.tag == eventId || e.id.ToString() == eventId);
            if (ev == null) return info;

            info.Event = ev;
            info.PlayerEvent = _playerEvents.FirstOrDefault(pe => pe.eventId == ev.id);

            return info;
        }
        public async Task<PlayerEventInfo> Join(string idOrTag)
        {
            
            var playerEventInfo = GetEventInfo(idOrTag);
            var ev = playerEventInfo.Event;
            var playerEvent = playerEventInfo.PlayerEvent;

            if (ev == null)
            {
                // HandleError(new Exception("EventNotFoundError"));
            }
            if (playerEvent != null)
            {
                // HandleError(new Exception("AlreadyJoinedError"));
            }

            try
            {

                
                var playerEventResult = await DataFetcher.Events.JoinEvent(new PlayerJoinEventInput(ev.id));

                playerEventResult = new PlayerEvent();
                if (_playerEvents.All(pe => pe.eventId != playerEventResult.eventId))
                {
                    _playerEvents.Add(playerEventResult);
                }

                var result = new PlayerEventInfo { Event = ev, PlayerEvent = playerEventResult };

                OnEventJoin?.Invoke(playerEventResult);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message);
            }

            return playerEventInfo;
        }

        public EventData[] List()
        {
            return _eventsList.ToArray();
        }

        public PlayerEvent[] ActiveList()
        {
            return _playerEvents.ToArray();
        }

        public EventInfo GetEvent(string idOrTag)
    {
        var playerEventInfo = GetEventInfo(idOrTag);
        var ev = playerEventInfo.Event;
        var playerEvent = playerEventInfo.PlayerEvent;
        var info = new EventInfo
        {
            Event = ev,
            Stats = playerEvent?.stats ?? new EventStats { activeDays = 0, activeDaysConsecutive = 0 },
            IsJoined = ev?.isActive == true && playerEvent != null,
            Rewards = new List<RewardData>(),
            Achievements = new List<AchievementData>(),
            Products = new List<ProductData>()
        };

        if (ev == null) return info;

        foreach (var trigger in ev.triggers)
        {
            foreach (var bonus in trigger.bonuses)
            {
                switch (bonus.type)
                {
                    case BonusType.Reward:
                        var reward = CoreSDK.Rewards.GetReward(bonus.id);
                        if (reward != null)
                        {
                            info.Rewards.Add(new RewardData { /* Копирование данных */ });
                        }
                        break;
                    case BonusType.Achievement:
                        var achievement = CoreSDK.Achievements.GetAchievement(bonus.id);
                        if (achievement != null)
                        {
                            info.Achievements.Add(new AchievementData { /* Копирование данных */ });
                        }
                        break;
                    case BonusType.Product:
                        var product = CoreSDK.Payments.GetProduct(bonus.id);
                        if (product != null)
                        {
                            info.Products.Add(new ProductData { /* Копирование данных */ });
                        }
                        break;
                }
            }
        }

        return info;
    }

    public bool IsActive(string eventId) => GetEventInfo(eventId).Event?.isActive == true;
    public bool IsJoined(string eventId) => GetEventInfo(eventId).Event?.isActive == true && GetEventInfo(eventId).PlayerEvent != null;

        
        
    }
}
