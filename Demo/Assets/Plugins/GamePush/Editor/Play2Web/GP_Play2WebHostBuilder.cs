#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    // Builds the standalone overlay host on demand and caches it in Temp. Shipping a prebuilt
    // binary would mean a platform executable in version control; Windows uses csc.exe +
    // WebView2, macOS uses swiftc + system WebKit.
    static class GP_Play2WebHostBuilder
    {
        static string HostDir => Path.Combine(
            Directory.GetParent(Application.dataPath)?.FullName ?? "",
            "Temp", "GamePushPlay2Web", "host");

        public static string EnsureBuilt()
        {
#if UNITY_EDITOR_WIN
            return EnsureBuiltWin();
#elif UNITY_EDITOR_OSX
            return EnsureBuiltMac();
#else
            return null;
#endif
        }

#if UNITY_EDITOR_WIN
        const string SourceAsset = "Assets/Plugins/GamePush/Editor/Play2Web/gp-play2web-hostapp.cs.txt";
        const string CoreDll = "Microsoft.Web.WebView2.Core.dll";
        const string LoaderDll = "WebView2Loader.dll";

        static string ExePath => Path.Combine(HostDir, "gp-play2web-host.exe");

        static string EnsureBuiltWin()
        {
            var source = Path.GetFullPath(SourceAsset);
            if (!File.Exists(source))
            {
                UnityEngine.Debug.LogError("[Play2Web] Overlay host source missing: " + SourceAsset);
                return null;
            }

            var pluginDir = Path.GetFullPath(Path.Combine(
                Application.dataPath, "Plugins", "GamePush", "Editor", "WebView2"));
            var core = Path.Combine(pluginDir, CoreDll);
            var loader = Path.Combine(pluginDir, LoaderDll);
            if (!File.Exists(core) || !File.Exists(loader))
            {
                UnityEngine.Debug.LogError("[Play2Web] WebView2 assemblies missing in " + pluginDir);
                return null;
            }

            Directory.CreateDirectory(HostDir);
            File.Copy(core, Path.Combine(HostDir, CoreDll), true);
            File.Copy(loader, Path.Combine(HostDir, LoaderDll), true);

            if (File.Exists(ExePath) && File.GetLastWriteTimeUtc(ExePath) > File.GetLastWriteTimeUtc(source))
                return ExePath;

            return CompileWin(source);
        }

        static string CompileWin(string source)
        {
            var csc = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "Microsoft.NET", "Framework64", "v4.0.30319", "csc.exe");
            if (!File.Exists(csc))
            {
                UnityEngine.Debug.LogError("[Play2Web] .NET Framework C# compiler not found at " + csc);
                return null;
            }

            var sourceCopy = Path.Combine(HostDir, "gp-play2web-hostapp.cs");
            File.Copy(source, sourceCopy, true);

            var arguments =
                "/nologo /target:winexe /optimize+ /platform:x64 " +
                $"/out:\"{ExePath}\" " +
                "/reference:System.dll /reference:System.Core.dll " +
                "/reference:System.Drawing.dll /reference:System.Windows.Forms.dll " +
                $"/reference:\"{Path.Combine(HostDir, CoreDll)}\" " +
                $"\"{sourceCopy}\"";

            var info = new ProcessStartInfo
            {
                FileName = csc,
                Arguments = arguments,
                WorkingDirectory = HostDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(info))
            {
                if (process == null)
                {
                    UnityEngine.Debug.LogError("[Play2Web] Could not run the C# compiler.");
                    return null;
                }

                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit(60000);

                if (process.ExitCode != 0 || !File.Exists(ExePath))
                {
                    UnityEngine.Debug.LogError("[Play2Web] Overlay host build failed:\n" + stdout + stderr);
                    return null;
                }
            }

            GP_Play2WebWindow.PushLog("Overlay host compiled");
            return ExePath;
        }
#endif

#if UNITY_EDITOR_OSX
        const string MacSourceAsset = "Assets/Plugins/GamePush/Editor/Play2Web/gp-play2web-hostapp.swift.txt";
        const string CltDialogKey = "GamePush.Play2Web.CltDialog";

        static string MacExePath => Path.Combine(HostDir, "gp-play2web-host");

        static string EnsureBuiltMac()
        {
            var source = Path.GetFullPath(MacSourceAsset);
            if (!File.Exists(source))
            {
                UnityEngine.Debug.LogError("[Play2Web] Overlay host source missing: " + MacSourceAsset);
                return null;
            }

            Directory.CreateDirectory(HostDir);
            if (File.Exists(MacExePath) && File.GetLastWriteTimeUtc(MacExePath) > File.GetLastWriteTimeUtc(source))
                return MacExePath;

            return CompileMac(source);
        }

        static string CompileMac(string source)
        {
            if (string.IsNullOrEmpty(FindSwiftc()))
            {
                UnityEngine.Debug.LogError(
                    "[Play2Web] Xcode Command Line Tools not found. Install them with: xcode-select --install");
                if (!SessionState.GetBool(CltDialogKey, false))
                {
                    SessionState.SetBool(CltDialogKey, true);
                    EditorUtility.DisplayDialog(
                        "Play2Web",
                        "Play2Web on macOS needs Xcode Command Line Tools to compile the WKWebView overlay.\n\nInstall them with:\nxcode-select --install",
                        "OK");
                }
                return null;
            }

            File.Copy(source, Path.Combine(HostDir, "gp-play2web-hostapp.swift"), true);
            File.WriteAllText(Path.Combine(HostDir, "Info.plist"), MacInfoPlist);

            const string compileArgs =
                "swiftc -O -framework AppKit -framework WebKit " +
                "-o gp-play2web-host " +
                "-Xlinker -sectcreate -Xlinker __TEXT -Xlinker __info_plist -Xlinker Info.plist " +
                "gp-play2web-hostapp.swift";
            if (!RunTool("/usr/bin/xcrun", compileArgs, HostDir, 120000, "Overlay host build failed"))
                return null;

            const string signArgs =
                "--force --sign - --identifier com.gamepush.play2web.host gp-play2web-host";
            if (!RunTool("/usr/bin/codesign", signArgs, HostDir, 30000, "Overlay host codesign failed"))
                return null;

            GP_Play2WebWindow.PushLog("Overlay host compiled");
            return MacExePath;
        }

        static string FindSwiftc()
        {
            try
            {
                var info = new ProcessStartInfo
                {
                    FileName = "/usr/bin/xcrun",
                    Arguments = "--find swiftc",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var process = Process.Start(info))
                {
                    if (process == null)
                        return null;
                    var stdout = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit(15000);
                    if (process.ExitCode == 0 && File.Exists(stdout))
                        return stdout;
                }
            }
            catch
            {
                // xcrun missing or developer path unset
            }
            return null;
        }

        static bool RunTool(string fileName, string arguments, string workDir, int timeoutMs, string failLabel)
        {
            var info = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using (var process = Process.Start(info))
                {
                    if (process == null)
                    {
                        UnityEngine.Debug.LogError("[Play2Web] Could not run " + fileName);
                        return false;
                    }

                    var stdout = process.StandardOutput.ReadToEnd();
                    var stderr = process.StandardError.ReadToEnd();
                    if (!process.WaitForExit(timeoutMs))
                    {
                        try { process.Kill(); }
                        catch { /* ignore */ }
                        UnityEngine.Debug.LogError("[Play2Web] " + failLabel + " (timeout)");
                        return false;
                    }

                    if (process.ExitCode != 0)
                    {
                        UnityEngine.Debug.LogError("[Play2Web] " + failLabel + ":\n" + stdout + stderr);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[Play2Web] " + failLabel + ": " + ex.Message);
                return false;
            }

            return File.Exists(MacExePath);
        }

        const string MacInfoPlist =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n" +
            "<plist version=\"1.0\">\n" +
            "<dict>\n" +
            "  <key>CFBundleIdentifier</key>\n" +
            "  <string>com.gamepush.play2web.host</string>\n" +
            "  <key>CFBundleName</key>\n" +
            "  <string>gp-play2web-host</string>\n" +
            "  <key>CFBundlePackageType</key>\n" +
            "  <string>APPL</string>\n" +
            "  <key>CFBundleExecutable</key>\n" +
            "  <string>gp-play2web-host</string>\n" +
            "  <key>LSUIElement</key>\n" +
            "  <true/>\n" +
            "  <key>NSHighResolutionCapable</key>\n" +
            "  <true/>\n" +
            "  <key>LSMinimumSystemVersion</key>\n" +
            "  <string>11.0</string>\n" +
            "  <key>NSAppTransportSecurity</key>\n" +
            "  <dict>\n" +
            "    <key>NSAllowsLocalNetworking</key>\n" +
            "    <true/>\n" +
            "    <key>NSAllowsArbitraryLoads</key>\n" +
            "    <true/>\n" +
            "  </dict>\n" +
            "</dict>\n" +
            "</plist>\n";
#endif
    }
}
#endif
