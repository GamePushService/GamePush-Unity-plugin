using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Reactions : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Reactions);

        public static event UnityAction<ReactionResult> OnSet;
        public static event UnityAction<string> OnSetError;
        public static event UnityAction<ReactionResult> OnUnset;
        public static event UnityAction<string> OnUnsetError;
        public static event UnityAction<ReactionResult> OnReactionSetEvent;
        public static event UnityAction<ReactionResult> OnReactionUnsetEvent;

        private static event Action<ReactionResult> _onSet;
        private static event Action<string> _onSetError;
        private static event Action<ReactionResult> _onUnset;
        private static event Action<string> _onUnsetError;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Reactions_Set(string entityType, string entityId, string reactionType);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Reactions_Unset(string entityType, string entityId, string reactionType);
        #endif

        public static void Set(string entityType, string entityId, string reactionType, Action<ReactionResult> onSet = null, Action<string> onError = null)
        {
            _onSet = onSet;
            _onSetError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Reactions_Set(entityType ?? "", entityId ?? "", reactionType ?? "");
#else
            ConsoleLog("SET: " + entityType + " " + entityId + " " + reactionType);
            ReactionResult stub = new ReactionResult
            {
                entityType = entityType,
                entityId = entityId,
                reactionType = reactionType,
                counter = 1
            };
            OnSet?.Invoke(stub);
            _onSet?.Invoke(stub);
#endif
        }

        public static void Unset(string entityType, string entityId, string reactionType, Action<ReactionResult> onUnset = null, Action<string> onError = null)
        {
            _onUnset = onUnset;
            _onUnsetError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Reactions_Unset(entityType ?? "", entityId ?? "", reactionType ?? "");
#else
            ConsoleLog("UNSET: " + entityType + " " + entityId + " " + reactionType);
            ReactionResult stub = new ReactionResult
            {
                entityType = entityType,
                entityId = entityId,
                reactionType = reactionType,
                counter = 0
            };
            OnUnset?.Invoke(stub);
            _onUnset?.Invoke(stub);
#endif
        }

        private void CallReactionsSet(string data)
        {
            ReactionResult result = Parse(data);
            OnSet?.Invoke(result);
            _onSet?.Invoke(result);
        }

        private void CallReactionsSetError(string error)
        {
            OnSetError?.Invoke(error);
            _onSetError?.Invoke(error);
        }

        private void CallReactionsUnset(string data)
        {
            ReactionResult result = Parse(data);
            OnUnset?.Invoke(result);
            _onUnset?.Invoke(result);
        }

        private void CallReactionsUnsetError(string error)
        {
            OnUnsetError?.Invoke(error);
            _onUnsetError?.Invoke(error);
        }

        private void CallReactionsSetEvent(string data)
        {
            OnReactionSetEvent?.Invoke(Parse(data));
        }

        private void CallReactionsUnsetEvent(string data)
        {
            OnReactionUnsetEvent?.Invoke(Parse(data));
        }

        private static ReactionResult Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new ReactionResult();
            return JsonUtility.FromJson<ReactionResult>(data) ?? new ReactionResult();
        }
    }

    [Serializable]
    public class ReactionResult
    {
        public string entityType;
        public string entityId;
        public string reactionType;
        public int counter;
    }

    [Serializable]
    public class ReactionCount
    {
        public string type;
        public int count;
    }

    [Serializable]
    public class PlayerReaction
    {
        public string reactionType;
    }
}
