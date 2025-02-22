using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static System.Timers.Timer;

namespace GamePush.Core
{
    public abstract class AbstractAnalyticsAdapter
    {
        public virtual Task Hit(string url) => Task.CompletedTask;
        public virtual Task Goal(string eventName, object value) => Task.CompletedTask;
        public virtual void SetVisitParams(Dictionary<string, string> parameters) { }
    }
    
    public class AnalyticsModule
    {
        private readonly List<AbstractAnalyticsAdapter> _counters = new();
        public Dictionary<string, string> VisitParams = new();
        public Dictionary<string, string> ExperimentsVisitParams = new();
        public Dictionary<string, string> SegmentsVisitParams = new();
        
        private int? _visitParamsTimeoutId;

        public async Task AddCounter(AbstractAnalyticsAdapter counter)
        {
            _counters.Add(counter);
        }

        public void ReplaceCounters(List<AbstractAnalyticsAdapter> counters)
        {
            _counters.Clear();
            _counters.AddRange(counters);
        }

        public void Hit(string url)
        {
            foreach (var counter in _counters)
            {
                counter.Hit(url);
            }
        }

        public void Goal(string eventName, object value)
        {
            foreach (var counter in _counters)
            {
                counter.Goal(eventName, value);
            }
        }

        public void SetVisitParams(Dictionary<string, string> visitParams)
        {
            VisitParams = visitParams;

            if (_visitParamsTimeoutId.HasValue)
            {
                return;
            }

            _visitParamsTimeoutId = Task.Delay(100).ContinueWith(t =>
            {
                _visitParamsTimeoutId = null;
                var visitParams = 
                    ExperimentsVisitParams
                    .Concat(SegmentsVisitParams)
                    .Concat(VisitParams)
                    .ToDictionary(k => k.Key, v => v.Value);
                
                foreach (var c in _counters)
                {
                    c.SetVisitParams(visitParams);
                }
            }).Id;
        }
        
    }
}
