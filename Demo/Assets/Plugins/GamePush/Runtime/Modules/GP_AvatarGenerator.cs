using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Native;

namespace GamePush
{
    public class GP_AvatarGenerator : GP_Module
    {
        public const int DefaultSize = 64;

        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.AvatarGenerator);

        public static event UnityAction<string> OnChange;


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Current_AvatarGenerator();
        [DllImport("__Internal")]
        private static extern string GP_Generate_Avatar(string hash, int size);
        #endif
        public static string Current()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Current_AvatarGenerator();
#else
            if (!string.IsNullOrEmpty(NativeCore.AvatarGenerator))
                return NativeCore.AvatarGenerator;
            ConsoleLog("CURRENT: dicebear_retro");
            return "dicebear_retro";
#endif
        }

        /// <summary>
        /// Builds a generator URL the same way as gp.generateAvatar(hash, size):
        /// avatarGeneratorTemplate with {{hash}} and {{size}} replaced.
        /// </summary>
        public static string Generate(object hash, int size = DefaultSize)
        {
            var seed = hash != null ? Convert.ToString(hash) : "";
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Generate_Avatar(seed ?? "", size);
#else
            return Apply(NativeCore.AvatarGeneratorTemplate, seed, size);
#endif
        }

        /// <summary>
        /// PlayerProfile / sandbox rule: use the explicit avatar, otherwise generate one from the player id.
        /// </summary>
        public static string ResolveUrl(string avatar, int playerId, int size = DefaultSize)
        {
            if (!string.IsNullOrEmpty(avatar))
                return avatar;
            if (playerId <= 0)
                return "";
            return Generate(playerId, size);
        }

        public static string Apply(string template, object hash, int size)
        {
            if (string.IsNullOrEmpty(template))
                return "";
            var seed = hash != null ? Convert.ToString(hash) ?? "" : "";
            return template
                .Replace("{{hash}}", seed)
                .Replace("{{HASH}}", seed)
                .Replace("{{size}}", size.ToString())
                .Replace("{{SIZE}}", size.ToString());
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Change_AvatarGenerator(string generator);
        #endif
        public static void Change(GeneratorType generator)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Change_AvatarGenerator(generator.ToString());
#else

            ConsoleLog("CHANGE: " + generator);
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
