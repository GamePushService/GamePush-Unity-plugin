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
            { MutationAction.Add, (mutation) =>
                {
                    if(int.TryParse(mutation.value, out int resultInt))
                        CoreSDK.Player.Add(mutation.key, resultInt);
                    else if(float.TryParse(mutation.value, out float resultFloat))
                        CoreSDK.Player.Add(mutation.key, resultFloat);
                }
            },
            { MutationAction.Remove, (mutation) =>
                {
                    if(int.TryParse(mutation.value, out int resultInt))
                        CoreSDK.Player.Add(mutation.key, -resultInt);
                    else if(float.TryParse(mutation.value, out float resultFloat))
                        CoreSDK.Player.Add(mutation.key, -resultFloat);
                }
            },
            { MutationAction.Set, (mutation) =>
                {
                    if(int.TryParse(mutation.value, out int resultInt))
                        CoreSDK.Player.Set(mutation.key, resultInt);
                    else if(float.TryParse(mutation.value, out float resultFloat))
                        CoreSDK.Player.Set(mutation.key, resultFloat);
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