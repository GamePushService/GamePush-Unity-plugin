using System.IO;
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace GamePush.BuildTools
{
    public static class GP_ParityBuild
    {
        private const string OutputDirectory = "Build/WebGLParity";
        private static readonly string[] Scenes =
        {
            "Assets/GP_Examples/Scenes/ExamplesScene.unity"
        };

        [MenuItem("GamePush/Build/Parity WebGL")]
        public static void BuildParityWebGL()
        {
            Build();
        }

        public static void CI_BuildParityWebGL()
        {
            Build();
        }

        private static void Build()
        {
            Directory.CreateDirectory(OutputDirectory);

            PlayerSettings.WebGL.decompressionFallback = false;
            string originalTemplate = PlayerSettings.WebGL.template;
            PlayerSettings.WebGL.template = "PROJECT:GamePush";

            try
            {
                BuildPlayerOptions options = new BuildPlayerOptions
                {
                    scenes = Scenes,
                    locationPathName = OutputDirectory,
                    target = BuildTarget.WebGL,
                    options = BuildOptions.None
                };

                BuildReport report = BuildPipeline.BuildPlayer(options);

                if (report.summary.result != BuildResult.Succeeded)
                    throw new Exception("Parity WebGL build failed: " + report.summary.result);
            }
            finally
            {
                PlayerSettings.WebGL.template = originalTemplate;
            }
        }
    }
}
