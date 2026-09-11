using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GamePush.Data;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    static class GP_Play2WebTemplateComposer
    {
        const string BridgeScript = "gp-unity-bridge.js";

        public static string OutputRoot => Path.Combine(
            Directory.GetParent(Application.dataPath)?.FullName ?? "",
            "Temp", "GamePushPlay2Web", "www");

        public static string Compose(bool wipe = true)
        {
            var output = OutputRoot;
            if (wipe && Directory.Exists(output))
            {
                try { Directory.Delete(output, true); }
                catch (IOException ex) { Debug.LogWarning("[Play2Web] Could not wipe www: " + ex.Message); }
            }
            Directory.CreateDirectory(output);

            var templateDir = ResolveTemplateDirectory();
            if (!string.IsNullOrEmpty(templateDir) && Directory.Exists(templateDir))
                CopyDirectory(templateDir, output);

            var templateData = Path.Combine(output, "TemplateData");
            Directory.CreateDirectory(templateData);

            var bootSrc = Path.Combine("Assets", "Plugins", "GamePush", "Editor", "Play2Web", "gp-play2web-boot.js.txt");
            var bootAbs = Path.GetFullPath(bootSrc);
            if (File.Exists(bootAbs))
                File.Copy(bootAbs, Path.Combine(templateData, "gp-play2web-boot.js"), true);

            // The bridge ships as a WebGL preamble and as a prebuilt bundle in the template. Only
            // the preamble is kept up to date, so the page gets that one: the bundle predates the
            // multiplayer API and would silently drop every Multiplayer_* call.
            var bridgeAbs = Path.GetFullPath(Path.Combine(
                "Assets", "Plugins", "GamePush", "JS", "_gpUnity.jspre"));
            if (File.Exists(bridgeAbs))
                File.Copy(bridgeAbs, Path.Combine(templateData, BridgeScript), true);
            else
                Debug.LogWarning("[Play2Web] GamePush bridge source not found: " + bridgeAbs);

            File.WriteAllText(Path.Combine(output, "index.html"), BuildIndex(templateDir), Encoding.UTF8);
            return output;
        }

        static string BuildIndex(string templateDir)
        {
            var product = PlayerSettings.productName;
            var templateHtml = "";
            if (!string.IsNullOrEmpty(templateDir))
            {
                var index = Path.Combine(templateDir, "index.html");
                if (File.Exists(index))
                    templateHtml = File.ReadAllText(index);
            }

            var cssHref = File.Exists(Path.Combine(OutputRoot, "TemplateData", "style.css"))
                ? "TemplateData/style.css"
                : "";

            var styleBlock = "";
            if (!string.IsNullOrEmpty(templateHtml))
            {
                var styleMatch = Regex.Match(templateHtml, @"<style[\s\S]*?</style>", RegexOptions.IgnoreCase);
                if (styleMatch.Success)
                    styleBlock = styleMatch.Value;
            }

            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{Escape(product)} Play2Web</title>
  {(string.IsNullOrEmpty(cssHref) ? "" : $"<link rel=\"stylesheet\" href=\"{cssHref}\">")}
  {styleBlock}
  <style>
    html, body, #play2web-root {{
      margin:0; padding:0; width:100%; height:100%; overflow:hidden;
      background:{GamePushEditor.Play2Web.GP_Play2WebOverlay.PageBackgroundCss} !important;
    }}
    canvas, #unity-canvas, #unity-container {{
      display:none !important;
    }}
    #play2web-status {{
      display:none;
      position:fixed; left:8px; bottom:8px; z-index:10;
      font:12px/1.4 ui-monospace, Consolas, monospace;
      color:#cfe; background:rgba(0,0,0,.55); padding:6px 10px; border-radius:6px;
      pointer-events:none;
    }}
  </style>
</head>
<body>
  <div id=""play2web-root""></div>
  <div id=""play2web-status"">Play2Web connecting…</div>
  <script>
    window.__GP_PLAY2WEB__ = {{
      projectId: {Json(ProjectData.ID)},
      token: {Json(ProjectData.TOKEN)},
      showPreloaderAd: {Json(ProjectData.SHOW_STICKY_ON_START ? "False" : "False")}
    }};
  </script>
  <script src=""TemplateData/{BridgeScript}""></script>
  <script src=""TemplateData/gp-play2web-boot.js""></script>
</body>
</html>";
        }

        static string ResolveTemplateDirectory()
        {
            var template = PlayerSettings.WebGL.template;
            if (string.IsNullOrEmpty(template))
                template = "PROJECT:GamePush";
            if (template.StartsWith("PROJECT:", StringComparison.OrdinalIgnoreCase))
            {
                var name = template.Substring("PROJECT:".Length);
                var path = Path.Combine(Application.dataPath, "WebGLTemplates", name);
                if (Directory.Exists(path))
                    return path;
            }
            var fallback = Path.Combine(Application.dataPath, "WebGLTemplates", "GamePush");
            return Directory.Exists(fallback) ? fallback : "";
        }

        static void CopyDirectory(string src, string dst)
        {
            Directory.CreateDirectory(dst);
            foreach (var file in Directory.GetFiles(src))
            {
                var name = Path.GetFileName(file);
                if (name.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) || name == "index.html")
                    continue;
                File.Copy(file, Path.Combine(dst, name), true);
            }
            foreach (var dir in Directory.GetDirectories(src))
            {
                var name = Path.GetFileName(dir);
                if (name == "Build")
                    continue;
                CopyDirectory(dir, Path.Combine(dst, name));
            }
        }

        static string Escape(string value)
        {
            return (value ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        static string Json(string value)
        {
            return "\"" + (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }
    }
}
