using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace GamePush.Native
{
    public sealed class NativeMainThread : MonoBehaviour
    {
        static NativeMainThread _instance;
        readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        public static NativeMainThread Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                var go = GameObject.Find("GamePushSDK");
                if (go == null)
                {
                    go = new GameObject("GamePushSDK");
                    DontDestroyOnLoad(go);
                }
                _instance = go.GetComponent<NativeMainThread>() ?? go.AddComponent<NativeMainThread>();
                return _instance;
            }
        }

        public static void Ensure()
        {
            var _ = Instance;
        }

        public static void Run(Action action)
        {
            if (action == null)
                return;
            Instance._queue.Enqueue(action);
        }

        void Update()
        {
            while (_queue.TryDequeue(out var action))
            {
                try { action(); }
                catch (Exception exception) { Debug.LogException(exception); }
            }
            NativePlayer.TickAutoSync(Time.realtimeSinceStartup);
        }
    }
}
