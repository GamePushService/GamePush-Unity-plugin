using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GamePush.Core
{
    public class ExperimentsModule
    {
        private List<PlayerExperiment> _experiments = new List<PlayerExperiment>();
        private Dictionary<string, string> _playerExperiments = new Dictionary<string, string>();
        
        public void SetExperiments(List<PlayerExperiment> playerExperiments)
        {
            if (playerExperiments.Count == 0)
            {
                return;
            }

            bool isNeedToSendVisitParams = false;
            var visitParams = new Dictionary<string, string>();

            _playerExperiments = playerExperiments
                .Select((experiment, index) =>
                {
                    visitParams[$"{CoreSDK.App.Title().ToUpper()}_AB_{experiment.Experiment}"] = experiment.Cohort;

                    if (!isNeedToSendVisitParams && _experiments.ElementAtOrDefault(index)?.Cohort != experiment.Cohort)
                    {
                        isNeedToSendVisitParams = true;
                    }

                    return experiment;
                })
                .ToDictionary(e => e.Experiment, e => e.Cohort);
            
             _experiments = playerExperiments;

            CoreSDK.Analytics.ExperimentsVisitParams = visitParams;
            if (isNeedToSendVisitParams)
            {
                CoreSDK.Analytics.SetVisitParams(CoreSDK.Analytics.VisitParams);
            }
        }

        public Dictionary<string, string> Map()
        {
            return _playerExperiments;
        }

        public bool Has(string tag, string cohort)
        {
            return _playerExperiments.ContainsKey(tag) && _playerExperiments[tag] == cohort;
        }
    }
}
