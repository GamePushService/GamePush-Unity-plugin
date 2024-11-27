using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public class Console
    {
        public static void Log(string message)
        {
            if (GP_Settings.instance.viewLogs)
            {
                Debug.Log("<color=#04bc04> Game Push: </color> " + message);
            }
        }

        public static void Log(string message, string colorMessage)
        {
            if (GP_Settings.instance.viewLogs)
            {
                Debug.Log("<color=#04bc04> Game Push: </color> " + message + $"<color=#04bc04> {colorMessage} </color>");
            }
        }
    }
}

