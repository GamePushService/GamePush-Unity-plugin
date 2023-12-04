using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_AvatarGenerator : MonoBehaviour
    {
        public static event UnityAction<string> OnChange;


        [DllImport("__Internal")]
        private static extern string GP_Current_AvatarGenerator();
        public static string Current()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Current_AvatarGenerator();
#else
            if (GP_ConsoleController.Instance.AvatarGeneratorConsoleLogs)
                Console.Log("AVATAR GENERATOR: ", "CURRENT: dicebear_retro");
            return "dicebear_retro";
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Change_AvatarGenerator(string generator);
        public static void Change(GeneratorType generator)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Change_AvatarGenerator(generator.ToString());
#else
            if (GP_ConsoleController.Instance.AvatarGeneratorConsoleLogs)
                Console.Log("AVATAR GENERATOR: ", "CHANGE: " + generator);
#endif
        }


        private void CallChangeAvatarGenerator(string generator) => OnChange?.Invoke(generator);

    }

    public enum GeneratorType : byte
    {
        dicebear_retro,
        dicebear_identicon,
        dicebear_human,
        dicebear_micah,
        dicebear_bottts,
        icotar,
        robohash_robots,
        robohash_cats,
    }
}