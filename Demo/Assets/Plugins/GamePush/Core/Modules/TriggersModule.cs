using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamePush;
using GamePush.Data;

namespace GamePush.Core
{
    public class TriggersModule 
    {
        private List<TriggerData> _triggersDataList = new();
        private List<PlayerTrigger> _activatedTriggersList = new();
        private Dictionary<string, TriggerData> _triggersMapID = new();
        private Dictionary<string, TriggerData> _triggersMapTag = new();
        private Dictionary<string, PlayerTrigger> _activatedTriggersMap = new();

        #region Events
        
        public event Action<TriggerData> OnTriggerActivate;
        public event Action<TriggerData> OnTriggerClaim;
        public event Action<string> OnTriggerClaimError;

        #endregion
        
        public void Init(AllConfigData config)
        {
            _triggersDataList = config.triggers
                .Concat(config.schedulers.SelectMany(it => it.triggers))
                .Concat(config.events.SelectMany(it => it.triggers))
                .ToList();
            RefreshTriggersMap();

            CoreSDK.Language.OnChangeLanguage += ChangeLanguage;
        }

        private void ChangeLanguage(Language language)
        {
            _triggersDataList.ForEach(trigger =>
            {
                trigger.description = CoreSDK.Language.GetTranslation(trigger.descriptions);
            });
            RefreshTriggersMap();
        }

        public void SetTriggersList(List<PlayerTrigger> triggers)
        {
            _activatedTriggersList = triggers
                .Where(tr => _triggersMapID.ContainsKey(tr.triggerId))
                .ToList();
            RefreshActivatedTriggersMap();
        }

        public void MarkTriggersAsActivated(List<string> ids)
        {
            foreach (var id in ids)
            {
                PlayerTriggerInfo info = GetTriggerInfo(id);
                
                if (info.trigger == null)
                {
                    Logger.Error($"Trigger not found, ID: {id}");
                    continue;
                }
            
                if (!info.isActivated)
                {
                    _activatedTriggersList.Add(new PlayerTrigger { triggerId = id, claimed = false });
                    RefreshActivatedTriggersMap();
                    OnTriggerActivate?.Invoke(info.trigger);
                    
                    Logger.Info($"Trigger activated, ID: {id}, Tag: {info.trigger.tag}");
                }
            }
        }

        public void MarkTriggersAsClaimed(List<string> ids)
        {
            foreach (var id in ids)
            {
                PlayerTriggerInfo info = GetTriggerInfo(id);
                if (info.trigger == null)
                {
                    Logger.Error($"Trigger not found, ID: {id}");
                    continue;
                }
            
                if (!info.isClaimed)
                {
                    _activatedTriggersList = _activatedTriggersList
                        .Select(t => t.triggerId == id ? new PlayerTrigger { triggerId = id, claimed = true } : t)
                        .ToList();
                    RefreshActivatedTriggersMap();
                    OnTriggerClaim?.Invoke(info.trigger);
                    Logger.Info($"Trigger claimed, ID: {id}, Tag: {info.trigger.tag}");
                }
            }
        }

        
        public List<TriggerData> List => new(_triggersDataList);
        public List<PlayerTrigger> ActivatedList => new(_activatedTriggersList);

        public bool IsActivated(string idOrTag) => GetTriggerInfo(idOrTag).isActivated;
        public bool IsClaimed(string idOrTag) => GetTriggerInfo(idOrTag).isClaimed;
        public PlayerTriggerInfo GetTrigger(string idOrTag) => GetTriggerInfo(idOrTag);
        
        public async Task<PlayerTriggerInfo> ClaimAsync(string id = null, string tag = null)
        {
            var idOrTag = id ?? tag;
            var trigger = GetTriggerByIdOrTag(idOrTag);

            try
            {
                if (trigger == null)
                {
                    string error = $"Trigger not found, ID: {idOrTag}";
                    // Logger.Error(error);
                    OnTriggerClaimError?.Invoke(error);
                }

                var result = await ClaimTriggerAsync(trigger.id);
                return result;
            }
            catch (Exception error)
            {
                var err = error.Message;
                OnTriggerClaimError?.Invoke(err);
            }
            
            return null;
        }

        private async Task<PlayerTriggerInfo> ClaimTriggerAsync(string idOrTag)
        {
            var (isActivated, isClaimed) = GetTriggerState(idOrTag);

            if (!isActivated)
            {
                string error = $"Trigger is not activated, ID: {idOrTag}";
                // Logger.Error(error);
                OnTriggerClaimError?.Invoke(error);
            }

            if (isClaimed)
            {
                string error = $"Trigger is already claimed, ID: {idOrTag}";
                // Logger.Error(error);
                OnTriggerClaimError?.Invoke(error);
            }

            CoreSDK.Player.AddClaimedTrigger(idOrTag);
            await CoreSDK.Player.PlayerSync();

            return GetTriggerInfo(idOrTag);
        }

        private TriggerData GetTriggerByIdOrTag(string idOrTag) =>
            _triggersMapID.TryGetValue(idOrTag, out var byId) ? byId :
            _triggersMapTag.TryGetValue(idOrTag, out var byTag) ? byTag :
            null;

        private PlayerTriggerInfo GetTriggerInfo(string idOrTag)
        {
            var trigger = GetTriggerByIdOrTag(idOrTag);
            var info = new PlayerTriggerInfo { trigger = trigger, isActivated = false, isClaimed = false };

            if (trigger != null && _activatedTriggersMap.TryGetValue(trigger.id, out var playerTrigger))
            {
                info.isActivated = true;
                info.isClaimed = playerTrigger.claimed;
            }

            return info;
        }

        private (bool IsActivated, bool IsClaimed) GetTriggerState(string idOrTag)
        {
            return _activatedTriggersMap.TryGetValue(idOrTag, out var playerTrigger)
                ? (true, playerTrigger.claimed)
                : (false, false);
        }

        private void RefreshTriggersMap()
        {
            _triggersMapID = _triggersDataList.ToDictionary(trigger => trigger.id);
            _triggersMapTag = _triggersDataList.ToDictionary(trigger => trigger.tag);
        }

        private void RefreshActivatedTriggersMap()
        {
            _activatedTriggersMap = _activatedTriggersList.ToDictionary(pt => pt.triggerId);
        }
    }
}
