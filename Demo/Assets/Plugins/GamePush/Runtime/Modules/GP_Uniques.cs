using System.Runtime.InteropServices;
using UnityEngine;
using System;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Uniques : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Uniques);

        public static event UnityAction<UniquesData> OnUniqueValueRegister;
        public static event UnityAction<string> OnUniqueValueRegisterError;
        public static event UnityAction<UniquesData> OnUniqueValueCheck;
        public static event UnityAction<string> OnUniqueValueCheckError;
        public static event UnityAction<string> OnUniqueValueDelete;
        public static event UnityAction<string> OnUniqueValueDeleteError;

        private static event Action<UniquesData> _onUniqueValueRegister;
        private static event Action<string> _onUniqueValueRegisterError;
        private static event Action<UniquesData> _onUniqueValueCheck;
        private static event Action<string> _onUniqueValueCheckError;
        private static event Action<string> _onUniqueValueDelete;
        private static event Action<string> _onUniqueValueDeleteError;

        private void OnEnable()
        {
            CoreSDK.uniques.OnUniqueValueRegister += (UniquesData data) => CallOnUniqueCheck(data);
            CoreSDK.uniques.OnUniqueValueCheck += (UniquesData data) => CallOnUniqueCheck(data);
            CoreSDK.uniques.OnUniqueValueDelete += (UniquesData data) => CallOnUniqueDelete(data);

            CoreSDK.uniques.OnUniqueValueRegisterError += (string error) => CallOnUniqueValueRegisterError(error);
            CoreSDK.uniques.OnUniqueValueCheckError += (string error) => CallOnUniqueValueCheckError(error);
            CoreSDK.uniques.OnUniqueValueDeleteError += (string error) => CallOnUniqueValueDeleteError(error);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_UniquesRegister(string tag, string value);
        [DllImport("__Internal")]
        private static extern string GP_UniquesGet(string tag);
        [DllImport("__Internal")]
        private static extern string GP_UniquesList();
        [DllImport("__Internal")]
        private static extern void GP_UniquesCheck(string tag, string value);
        [DllImport("__Internal")]
        private static extern void GP_UniquesDelete(string tag);
#endif

        public static void Register(
            string tag,
            string value,
            Action<UniquesData> onUniqueValueRegister = null,
            Action<string> onUniqueValueRegisterError = null)
        {
            _onUniqueValueRegister = onUniqueValueRegister;
            _onUniqueValueRegisterError = onUniqueValueRegisterError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesRegister(tag, value);
#else
            CoreSDK.uniques.Register(tag, value);
#endif
        }

        public static string Get(string tag)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            //string json = GP_UniquesGet(tag);
            //return UtilityJSON.Get<UniquesData>(json);
            return GP_UniquesGet(tag);
#else
            return CoreSDK.uniques.Get(tag);
#endif
        }

        public static UniquesData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string json = GP_UniquesList();
            return UtilityJSON.GetArray<UniquesData>(json);
#else
            return CoreSDK.uniques.List();
#endif
        }

        public static void Check(
            string tag,
            string value,
            Action<UniquesData> onUniqueValueCheck = null,
            Action<string> onUniqueValueCheckError = null)
        {
            _onUniqueValueCheck = onUniqueValueCheck;
            _onUniqueValueCheckError = onUniqueValueCheckError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesCheck(tag, value);
#else
            CoreSDK.uniques.Check(tag, value);
#endif
        }

        public static void Delete(
            string tag,
            Action<string> onUniqueValueDelete = null,
            Action<string> onUniqueValueDeleteError = null)
        {
            _onUniqueValueDelete = onUniqueValueDelete;
            _onUniqueValueDeleteError = onUniqueValueDeleteError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesDelete(tag);
#else
            CoreSDK.uniques.Delete(tag);
#endif
        }

        private void CallOnUniqueValueRegister(string uniqueValue)
        {
            UniquesData data = UtilityJSON.Get<UniquesData>(uniqueValue);
            CallOnUniqueRegister(data);
        }

        private void CallOnUniqueRegister(UniquesData data)
        {
            OnUniqueValueRegister?.Invoke(data);
            _onUniqueValueRegister?.Invoke(data);
        }

        private void CallOnUniqueValueRegisterError(string error)
        {
            OnUniqueValueRegisterError?.Invoke(error);
            _onUniqueValueRegisterError?.Invoke(error);
        }

        private void CallOnUniqueValueCheck(string uniqueValue)
        {
            UniquesData data = UtilityJSON.Get<UniquesData>(uniqueValue);
            CallOnUniqueCheck(data);
        }

        private void CallOnUniqueCheck(UniquesData data)
        {
            OnUniqueValueCheck?.Invoke(data);
            _onUniqueValueCheck?.Invoke(data);
        }

        private void CallOnUniqueValueCheckError(string error)
        {
            OnUniqueValueCheckError?.Invoke(error);
            _onUniqueValueCheckError?.Invoke(error);
        }

        private void CallOnUniqueValueDelete(string uniqueValue)
        {
            UniquesData data = UtilityJSON.Get<UniquesData>(uniqueValue);
            CallOnUniqueDelete(data);
        }

        private void CallOnUniqueDelete(UniquesData data)
        {
            OnUniqueValueDelete?.Invoke(data.tag);
            _onUniqueValueDelete?.Invoke(data.tag);
        }

        private void CallOnUniqueValueDeleteError(string error)
        {
            GP_Logger.Log("Delete", error);
            OnUniqueValueDeleteError?.Invoke(error);
            _onUniqueValueDeleteError?.Invoke(error);
        }
    }
}
