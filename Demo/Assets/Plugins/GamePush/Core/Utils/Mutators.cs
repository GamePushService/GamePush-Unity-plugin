using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GamePush.Core
{
    
    public delegate void Mutator(DataMutation mutation);

    public static class MutationHandler
    {
        private static readonly Dictionary<MutationAction, Mutator> PlayerStateMutators = new Dictionary<MutationAction, Mutator>
        {
            { MutationAction.Add, (mutation) => CoreSDK.Player.Add(mutation.key, float.TryParse(mutation.value, out float result) ? result : 0) },
            { MutationAction.Remove, (mutation) => CoreSDK.Player.Add(mutation.key, -Convert.ToSingle(mutation.value)) },
            { MutationAction.Set, (mutation) =>
                {
                    if(float.TryParse(mutation.value, out float result))
                        CoreSDK.Player.Set(mutation.key, result);
                    else
                        CoreSDK.Player.Set(mutation.key, mutation.value.ToString());
                }
            }
        };

        private static readonly Dictionary<MutationType, Dictionary<MutationAction, Mutator>> MutatorsByType = new()
        {
            { MutationType.PlayerField, PlayerStateMutators }
        };

        public static void ApplyMutations(List<DataMutation> mutations)
        {
            foreach (var mutation in mutations)
            {
                if (!MutatorsByType.TryGetValue(mutation.type, out var mutators))
                {
                    throw new Exception($"Unknown mutation type: {mutation.type}");
                }

                if (!mutators.TryGetValue(mutation.action, out var mutator))
                {
                    throw new Exception($"Unknown mutation action: {mutation.action}");
                }

                mutator(mutation);
            }
        }
    }
}