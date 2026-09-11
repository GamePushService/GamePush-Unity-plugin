using System.IO;

namespace GamePushEditor.Adapters
{
    static class GP_EditorFiles
    {
        public static bool WriteIfChanged(string path, string text)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            text = text ?? "";
            if (File.Exists(path))
            {
                var existing = File.ReadAllText(path);
                if (Normalize(existing) == Normalize(text))
                    return false;
            }
            else
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, text);
            return true;
        }

        static string Normalize(string value)
        {
            return (value ?? "").Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}
