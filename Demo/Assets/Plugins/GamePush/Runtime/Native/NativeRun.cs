using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePush.Native
{
    /// <summary>
    /// Shared plumbing for the native module implementations: guards on SDK readiness,
    /// unwraps GraphQL "Problem" payloads and routes failures back to the main thread.
    /// </summary>
    public static class NativeRun
    {
        public static void Go(Func<Task> work, Action onError, string scope)
        {
            Go(work, _ => onError?.Invoke(), scope);
        }

        public static void Go(Func<Task> work, Action<string> onError, string scope)
        {
            async void Execute()
            {
                try
                {
                    if (NativeCore.Client == null || !NativeCore.Ready)
                        throw new Exception("sdk_not_ready");
                    if (NativePlayer.Id <= 0)
                        throw new Exception("player_not_found");
                    await work();
                }
                catch (Exception exception)
                {
                    var message = exception.Message ?? "unknown_error";
                    Debug.LogWarning("[GamePush Native] " + scope + ": " + message);
                    NativeMainThread.Run(() => onError?.Invoke(message));
                }
            }

            Execute();
        }

        /// <summary>
        /// Returns the payload of the aliased "result" field. GraphQLClient already unwraps
        /// single-field responses, so the payload may arrive with or without the wrapper.
        /// </summary>
        public static string Result(string json) => GpJson.GetObject(json, "result") ?? json;

        public static void ThrowIfProblem(string json)
        {
            var result = Result(json);
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                throw new Exception(GpJson.TryGetString(result, "message", out var msg) ? msg : "problem");
        }
    }
}
