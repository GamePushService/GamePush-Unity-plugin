using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using GamePush.Native;
using UnityEngine.Rendering;
using GamePush.Data;

namespace GamePush
{
    public class GP_Init : GP_Module
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_UnityReady();
#if GP_NATIVE_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_NativePlayer_Snapshot();
#endif
#endif

        public static bool isReady = false;

        public static Task Ready;
        public static event Action OnReady;
        public static event Action OnError;

#if UNITY_WEBGL && !UNITY_EDITOR && GP_NATIVE_WEBGL
        bool _jsPlayerReady;
        bool _jsPlayerFailed;
#endif

        private void OnEnable()
        {
            OnReady += Autocalls;
        }

        private void OnDisable()
        {
            OnReady -= Autocalls;
        }

        private void Autocalls() => StartCoroutine(AutocallsCoroutine());

        private void Start()
        {
#if UNITY_EDITOR
            if (GP_Play2Web.Enabled)
            {
                StartCoroutine(WaitForPlay2Web());
                return;
            }
            GP_Logger.Info("Init", "SDK ready");
            FinishReady();
#elif UNITY_WEBGL && !UNITY_EDITOR && GP_NATIVE_WEBGL
            StartCoroutine(InitWebGlNative());
#elif UNITY_WEBGL && !UNITY_EDITOR
            GP_UnityReady();
#else
            StartCoroutine(InitNative());
#endif
        }

        IEnumerator InitNative()
        {
            var task = NativeCore.Initialize();
            while (!task.IsCompleted)
                yield return null;
            if (task.IsFaulted || !task.Result)
            {
                GP_Logger.Error("Init", "Native SDK error: " + NativeCore.LastError);
                FinishError();
                yield break;
            }
            GP_Logger.Info("Init", "Native SDK ready");
            FinishReady();
        }

#if UNITY_WEBGL && !UNITY_EDITOR && GP_NATIVE_WEBGL
        IEnumerator InitWebGlNative()
        {
            GP_Logger.Info("Init", "WebGL native: wait JS player");
            GP_UnityReady();
            var elapsed = 0f;
            while (!_jsPlayerReady && !_jsPlayerFailed && elapsed < 30f)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            if (_jsPlayerFailed)
            {
                GP_Logger.Error("Init", "JS SDK error");
                yield break;
            }
            if (!_jsPlayerReady)
            {
                GP_Logger.Error("Init", "JS SDK timeout");
                FinishError();
                yield break;
            }
            GP_Logger.Info("Init", "JS player ready");

            NativeJsSession session = null;
            try
            {
                session = NativeJsSession.Parse(GP_NativePlayer_Snapshot());
            }
            catch (Exception exception)
            {
                GP_Logger.Error("Init", "JS player snapshot: " + exception.Message);
            }
            if (session == null || session.PlayerId <= 0)
            {
                GP_Logger.Error("Init", "JS player id missing");
                FinishError();
                yield break;
            }
            GP_Logger.Info("Init", "snapshot id=" + session.PlayerId);

            var task = NativeCore.Initialize(session);
            while (!task.IsCompleted)
                yield return null;
            if (task.IsFaulted || !task.Result)
            {
                GP_Logger.Error("Init", "Native SDK error: " + NativeCore.LastError);
                FinishError();
                yield break;
            }
            GP_Logger.Info("Init", "Native SDK ready");
            FinishReady();
        }
#endif

#if UNITY_EDITOR
        IEnumerator WaitForPlay2Web()
        {
            var elapsed = 0f;
            while (!GP_Play2Web.IsReady && elapsed < 30f)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (GP_Play2Web.IsReady)
            {
                NativeMainThread.Ensure();
                GP_Logger.Info("Init", "Play2Web SDK ready");
                FinishReady();
            }
            else
            {
                GP_Logger.Error("Init", "Play2Web SDK timeout");
                FinishError();
            }
        }
#endif

        IEnumerator AutocallsCoroutine()
        {
            while (!SplashScreen.isFinished)
                yield return null;

            if (ProjectData.SHOW_STICKY_ON_START)
                GP_Ads.ShowSticky();
        }

        public void CallOnSDKReady()
        {
#if UNITY_WEBGL && !UNITY_EDITOR && GP_NATIVE_WEBGL
            if (!isReady)
            {
                _jsPlayerReady = true;
                return;
            }
#endif
            FinishReady();
        }

        public void CallOnSDKError()
        {
#if UNITY_WEBGL && !UNITY_EDITOR && GP_NATIVE_WEBGL
            _jsPlayerFailed = true;
#endif
            FinishError();
        }

        void FinishReady()
        {
            isReady = true;
            OnReady?.Invoke();
        }

        void FinishError()
        {
            OnError?.Invoke();
        }
    }
}
