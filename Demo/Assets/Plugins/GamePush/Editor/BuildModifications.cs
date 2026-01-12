using System.IO;
using System.IO.Compression;
using GamePush;
using GamePush.Data;
using UnityEditor;
using UnityEditor.Callbacks;


namespace GamePushEditor
{
    public static class BuildModifications
    {
        [PostProcessBuild]
        public static void ModifyBuildDo(BuildTarget target, string pathToBuiltProject)
        {
            if(PluginData.COMPRESS_BUILD)
                Archiving(pathToBuiltProject);
        }
    
        private static void Archiving(string pathToBuiltProject)
        {
            string number = "";

            if (!File.Exists(pathToBuiltProject + ".zip"))
            {
                Do();
            }
            else
            {
                for (int i = 1; i < 100; i++)
                {
                    if (!File.Exists(pathToBuiltProject + "_" + i + ".zip"))
                    {
                        number = "_" + i;
                        Do();
                        break;
                    }
                }
            }

            void Do()
            {
                ZipFile.CreateFromDirectory(pathToBuiltProject, pathToBuiltProject + number + ".zip");
            }
        }
    }
}




