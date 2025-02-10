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
            if (events == null || events.Count == 0)
            {
                Logger.Warn("Events list is empty");
                _eventsList = new List<EventData>();
                _playerEvents = new List<PlayerEvent>();
                return;
            }
                
            _eventsList = events.Select(e => new EventData
            {
                id = e.id,
                tag = e.tag,
                name = LanguageTypes.GetTranslation(CoreSDK.currentLang, e.names) == "" ? e.names.en : LanguageTypes.GetTranslation(CoreSDK.currentLang, e.names),
                description = LanguageTypes.GetTranslation(CoreSDK.currentLang, e.descriptions) == "" ? e.descriptions.en : LanguageTypes.GetTranslation(CoreSDK.currentLang, e.descriptions),
                iconSmall = UtilityImage.ResizeImage(e.icon, 48, 48, false),
                icon = UtilityImage.ResizeImage(e.icon, 256, 256, false),
                triggers = e.triggers,
                dateStart = e.dateStart,
                dateEnd = e.dateEnd,
                timeLeft =  GetTimeLeft(e),
                isActive = GetIsActive(e),
                isAutoJoin = e.isAutoJoin,
            }).ToList();

            CoreSDK.Language.OnChangeLanguage += ChangeTranslation;

            _playerEvents = new List<PlayerEvent>();
        }
        
        private double GetTimeLeft(EventData e)
        {
            var timeEnd = DateTime.TryParse(e.dateEnd, out var end) ? end : DateTime.MaxValue;
            var timeLeft = (timeEnd - CoreSDK.GetServerTime()).TotalSeconds;
            return timeLeft > 0 ? (double)timeLeft : 0;
        }
        private bool GetIsActive(EventData e)
        {
            var timeStart = DateTime.TryParse(e.dateStart, out var start) ? start : DateTime.MinValue;
            return CoreSDK.GetServerTime() >= timeStart && GetTimeLeft(e) > 0;
        }
        
        public void SetPlayerEvents(List<PlayerEvent> events)
        {
            if (events == null || events.Count == 0) return;
            
            _playerEvents = events.Where(ps => _eventsList.Any(s => s.id == ps.eventId))
                .ToList();
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
            // Logger.Log("Get event info:" + eventId);
            var info = new PlayerEventInfo();
            
            var ev = _eventsList.FirstOrDefault(e => e.tag == eventId || e.id.ToString() == eventId);
            if (ev == null) return info;
            ev.timeLeft = GetTimeLeft(ev);
            ev.isActive = GetIsActive(ev);
            info.Event = ev;
            
            var playerEv= _playerEvents.FirstOrDefault(pe => pe.eventId == ev.id);
            if (playerEv == null) return info;
            info.PlayerEvent = playerEv;
            
            return info;
        }
        public async Task<PlayerEventInfo> Join(string idOrTag)
        {
            var playerEventInfo = GetEventInfo(idOrTag);
           
            var ev = playerEventInfo.Event;
            // Logger.Log("Join event:" + ev.id);
            var playerEvent = playerEventInfo.PlayerEvent;

            if (ev == null)
            {
                Logger.Error("EventNotFoundError");
                OnEventJoinError?.Invoke(idOrTag);
            }
            if (playerEvent != null)
            {
                Logger.Error("AlreadyJoinedError");
                OnEventJoinError?.Invoke(idOrTag);
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
                OnEventJoinError?.Invoke(idOrTag);
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

            if(ev.triggers == null || ev.triggers.Length == 0) return info;
            
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
                                info.Rewards.Add(reward);
                            }
                            break;
                        case BonusType.Achievement:
                            var achievement = CoreSDK.Achievements.GetAchievement(bonus.id);
                            if (achievement != null)
                            {
                                info.Achievements.Add(achievement.ToAchievementData());
                            }
                            break;
                        case BonusType.Product:
                            var product = CoreSDK.Payments.GetProduct(bonus.id);
                            if (product != null)
                            {
                                info.Products.Add(product);
                            }
                            break;
                    }
                }
            }

            return info;
        }

    public bool IsActive(string eventId) => GetEventInfo(eventId).Event?.isActive == true;
    public bool IsJoined(string eventId) => 
        GetEventInfo(eventId).Event?.isActive == true && GetEventInfo(eventId).PlayerEvent != null;

        
        
    }
}
