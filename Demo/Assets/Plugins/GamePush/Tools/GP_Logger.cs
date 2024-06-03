using UnityEngine;
using System.Collections;

namespace GamePush
{
    public class GP_Logger
    {
        public static void Log(string message)
        {
            Debug.Log("<color=#04bc04> Game Push: </color> " + message);
        }

        public static void Log(string message, string colorMessage)
        {
            Debug.Log("<color=#04bc04> Game Push: </color> " + message + $"<color=#04bc04> {colorMessage} </color>");
        }

        public static void Warn(string message)
        {
            Debug.Log("<color=#04bc04> Game Push: </color> " + $"<color=#E3F137> {message} </color>");
        }

        public static void Error(string message)
        {
            Debug.Log("<color=#04bc04> Game Push: </color> " + $"<color=#FF5733> {message} </color>");
        }
    }
}

