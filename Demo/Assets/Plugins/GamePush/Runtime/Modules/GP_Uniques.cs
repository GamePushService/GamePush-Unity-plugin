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

        public static event UnityAction OnUniqueValueRegister;
        public static event UnityAction<string> OnUniqueValueRegisterError;
        public static event UnityAction OnUniqueValueCheck;
        public static event UnityAction<string> OnUniqueValueCheckError;
        public static event UnityAction OnUniqueValueDelete;
        public static event UnityAction<string> OnUniqueValueDeleteError;

        private static event Action _onUniqueValueRegister;
        private static event Action<string> _onUniqueValueRegisterError;
        private static event Action _onUniqueValueCheck;
        private static event Action<string> _onUniqueValueCheckError;
        private static event Action _onUniqueValueDelete;
        private static event Action<string> _onUniqueValueDeleteError;


        [DllImport("__Internal")]
        private static extern void GP_UniquesRegister(string tag, string value);
        public static void Register(
            string tag,
            string value,
            Action onUniqueValueRegister = null,
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
            Action onUniqueValueCheck = null,
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
            Action onUniqueValueDelete = null,
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
            OnUniqueValueRegister?.Invoke();
            _onUniqueValueRegister?.Invoke();
        }
        private void CallOnUniqueValueRegisterError(string error)
        {
            OnUniqueValueRegisterError?.Invoke(error);
            _onUniqueValueRegisterError?.Invoke(error);
        }

        private void CallOnUniqueValueCheck(string uniqueValue)
        {
            OnUniqueValueCheck?.Invoke();
            _onUniqueValueCheck?.Invoke();
        }
        private void CallOnUniqueValueCheckError(string error)
        {
            OnUniqueValueCheckError?.Invoke(error);
            _onUniqueValueCheckError?.Invoke(error);
        }

        private void CallOnUniqueValueDelete(string uniqueValue)
        {
            OnUniqueValueDelete?.Invoke();
            _onUniqueValueDelete?.Invoke();
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
