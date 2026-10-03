using System;
using System.IO;
using UnityEditor;

namespace GamePushEditor.Adapters
{
    static class GP_AndroidTooling
    {
        const string ResolveMenu = "Assets/External Dependency Manager/Android Resolver/Resolve";

        public static string JavaHome => Environment.GetEnvironmentVariable("JAVA_HOME");

        public static string UnityJdkPath()
        {
            string embedded = Path.Combine(EditorApplication.applicationContentsPath,
                "PlaybackEngines", "AndroidPlayer", "OpenJDK");
            return File.Exists(JavaExecutable(embedded)) ? embedded : null;
        }

        public static bool AndroidModuleInstalled =>
            Directory.Exists(Path.Combine(EditorApplication.applicationContentsPath,
                "PlaybackEngines", "AndroidPlayer"));

        public static bool HasWorkingJava()
        {
            string home = JavaHome;
            return !string.IsNullOrEmpty(home) && File.Exists(JavaExecutable(home));
        }

        public static bool UseUnityJdkForThisSession()
        {
            string jdk = UnityJdkPath();
            if (string.IsNullOrEmpty(jdk))
                return false;
            Environment.SetEnvironmentVariable("JAVA_HOME", jdk);
            string bin = Path.Combine(jdk, "bin");
            string path = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (path.IndexOf(bin, StringComparison.OrdinalIgnoreCase) < 0)
                Environment.SetEnvironmentVariable("PATH", bin + Path.PathSeparator + path);
            return true;
        }

        public static bool TryResolveNow()
        {
            if (!HasWorkingJava() && !UseUnityJdkForThisSession())
                return false;
            return EditorApplication.ExecuteMenuItem(ResolveMenu);
        }

        public static bool EnsureJavaOrExplain(out string error)
        {
            error = null;
            if (HasWorkingJava())
                return true;
            if (!AndroidModuleInstalled)
            {
                error = "Android Build Support is not installed for this Unity version. In Unity Hub → this editor → Add modules → Android Build Support (SDK, NDK, OpenJDK).";
                return false;
            }

            if (string.IsNullOrEmpty(UnityJdkPath()))
            {
                error = "Unity's OpenJDK was not found. Install the OpenJDK module with Android Build Support, or set JAVA_HOME to a JDK 11+.";
                return false;
            }

            if (EditorUtility.DisplayDialog("GamePush",
                    "Android dependency resolve needs Java. This PC has no JAVA_HOME.\n\nUse Unity's bundled OpenJDK for this Editor session?",
                    "Use Unity JDK", "Cancel"))
            {
                UseUnityJdkForThisSession();
                return true;
            }

            error = "JAVA_HOME is not set. Set it to a JDK 11+, or click Use Unity JDK in Tools/GamePush → Android.";
            return false;
        }

        static string JavaExecutable(string jdk)
        {
            string name = Environment.OSVersion.Platform == PlatformID.Win32NT ? "java.exe" : "java";
            return Path.Combine(jdk, "bin", name);
        }
    }
}
