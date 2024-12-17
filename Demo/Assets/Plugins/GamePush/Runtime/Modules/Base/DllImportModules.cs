
using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public static class InternalDllImport
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        public static extern void GP_Achievements_Open();
#endif

    }
}
