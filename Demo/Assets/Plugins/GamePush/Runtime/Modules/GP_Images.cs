using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Images : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void GP_Images_Fetch();
        public static void Fetch()
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Fetch();
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("VARIABLES: ", "FETCH");
#endif
        }

        private void CallImagesFetchSuccess(string result)
        {
            Console.Log("Result: " + result);
        }
    }
}
