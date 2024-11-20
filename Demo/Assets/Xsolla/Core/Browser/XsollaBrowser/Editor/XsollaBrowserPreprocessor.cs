using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.Core.Browser
{
    public class XsollaBrowserPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platformGroup != BuildTargetGroup.Standalone || !XsollaSettings.InAppBrowserEnabled)
                return;
            
            //Check IL2CPP — browser supports only Mono.
            if (GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP)
                XDebug.LogError(@"WARNING: In-App Browser does not support IL2CPP scripting backend and will result in Win32Exception upon launch.
				Please change scripting backend to Mono or disable In-App Browser.");
        }

        private ScriptingImplementation GetCurrentScriptingBackend()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

#if UNITY_6000
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            return PlayerSettings.GetScriptingBackend(namedBuildTarget);
#else
            return PlayerSettings.GetScriptingBackend(targetGroup);
#endif
        }
    }
}