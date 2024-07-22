using System.Runtime.InteropServices;
using UnityEngine;
using System;
using UnityEngine.Events;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Uniques : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Uniques);

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


        [DllImport("__Internal")]
        private static extern void GP_UniquesRegister(string tag, string value);
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
            ConsoleLog("Register");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_UniquesGet(string tag);
        public static string Get(string tag)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            //string json = GP_UniquesGet(tag);
            //return UtilityJSON.Get<UniquesData>(json);
            return GP_UniquesGet(tag);
#else
            ConsoleLog("Get");
            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_UniquesList();
        public static UniquesData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string json = GP_UniquesList();
            return UtilityJSON.GetArray<UniquesData>(json);
#else
            ConsoleLog("List");
            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_UniquesCheck(string tag, string value);
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
            ConsoleLog("List");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_UniquesDelete(string tag);
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
            ConsoleLog("Delete");
#endif
        }

        private void CallOnUniqueValueRegister(string uniqueValue)
        {
            UniquesData data = UtilityJSON.Get<UniquesData>(uniqueValue);
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


    [System.Serializable]
    public class UniquesData
    {
        public string tag;
        public string value;
    }
}
