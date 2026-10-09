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
            var newestInput = File.GetLastWriteTimeUtc(source);
            var builder = Path.GetFullPath("Assets/Plugins/GamePush/Editor/Play2Web/GP_Play2WebHostBuilder.cs");
            if (File.Exists(builder))
            {
                var builderTime = File.GetLastWriteTimeUtc(builder);
                if (builderTime > newestInput)
                    newestInput = builderTime;
            }
            if (File.Exists(MacExePath) && File.GetLastWriteTimeUtc(MacExePath) > newestInput)
                return MacExePath;

            return CompileMac(source);
        }

        static string CompileMac(string source)
        {
            var swiftc = FindSwiftc();
            if (string.IsNullOrEmpty(swiftc))
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

            var sdk = QueryXcrun("--sdk macosx --show-sdk-path", 15000);
            var cache = Path.Combine(HostDir, "swift-module-cache");
            try
            {
                if (Directory.Exists(cache))
                    Directory.Delete(cache, true);
            }
            catch
            {
                // stale cache is optional; a poisoned one is worse than none
            }
            Directory.CreateDirectory(cache);

            var compileArgs =
                "--sdk macosx swiftc -O " +
                (string.IsNullOrEmpty(sdk) ? "" : "-sdk \"" + sdk + "\" ") +
                "-module-cache-path \"" + cache + "\" " +
                "-Xcc -fmodules-cache-path=\"" + cache + "\" " +
                SwiftBridgingOverlayArgs(swiftc) +
                "-framework AppKit -framework WebKit " +
                "-o gp-play2web-host " +
                "-Xlinker -sectcreate -Xlinker __TEXT -Xlinker __info_plist -Xlinker Info.plist " +
                "gp-play2web-hostapp.swift";
            if (!RunTool("/usr/bin/xcrun", compileArgs, HostDir, 120000, "Overlay host build failed", sdk))
                return null;

            const string signArgs =
                "--force --sign - --identifier com.gamepush.play2web.host gp-play2web-host";
            if (!RunTool("/usr/bin/codesign", signArgs, HostDir, 30000, "Overlay host codesign failed", sdk))
                return null;

            GP_Play2WebWindow.PushLog("Overlay host compiled");
            return MacExePath;
        }

        static string FindSwiftc()
        {
            var path = QueryXcrun("--sdk macosx --find swiftc", 15000);
            return !string.IsNullOrEmpty(path) && File.Exists(path) ? path : null;
        }

        // CLT 16.x renamed include/swift/module.modulemap to bridging.modulemap. An incomplete
        // upgrade leaves both, and clang then errors on redefinition of SwiftBridging while
        // importing AppKit. Hide the leftover map without touching the system install.
        static string SwiftBridgingOverlayArgs(string swiftc)
        {
            try
            {
                var usrBin = Path.GetDirectoryName(swiftc);
                if (string.IsNullOrEmpty(usrBin))
                    return "";
                var includeSwift = Path.GetFullPath(Path.Combine(usrBin, "..", "include", "swift"));
                var leftover = Path.Combine(includeSwift, "module.modulemap");
                var current = Path.Combine(includeSwift, "bridging.modulemap");
                if (!File.Exists(leftover) || !File.Exists(current))
                    return "";

                var emptyMap = Path.Combine(HostDir, "empty.modulemap");
                File.WriteAllText(emptyMap, "// Play2Web: leftover CLT SwiftBridging map\n");
                var overlay = Path.Combine(HostDir, "swift-vfs.yaml");
                File.WriteAllText(overlay,
                    "version: 0\n" +
                    "roots:\n" +
                    "  - name: " + YamlQuote(includeSwift) + "\n" +
                    "    type: directory\n" +
                    "    contents:\n" +
                    "      - name: module.modulemap\n" +
                    "        type: file\n" +
                    "        external-contents: " + YamlQuote(emptyMap) + "\n");
                return "-vfsoverlay \"" + overlay + "\" -Xcc -ivfsoverlay -Xcc \"" + overlay + "\" ";
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("[Play2Web] Could not apply SwiftBridging workaround: " + ex.Message);
                return "";
            }
        }

        static string YamlQuote(string value)
        {
            return "'" + (value ?? "").Replace("'", "''") + "'";
        }

        static string PreferredDeveloperDir()
        {
            const string xcode = "/Applications/Xcode.app/Contents/Developer";
            if (Directory.Exists(Path.Combine(xcode, "usr", "bin")))
                return xcode;
            const string clt = "/Library/Developer/CommandLineTools";
            return Directory.Exists(clt) ? clt : null;
        }

        static void ApplyMacBuildEnvironment(ProcessStartInfo info, string sdkRoot)
        {
            var env = info.EnvironmentVariables;
            env.Remove("CPATH");
            env.Remove("C_INCLUDE_PATH");
            env.Remove("CPLUS_INCLUDE_PATH");
            env.Remove("OBJC_INCLUDE_PATH");
            var developerDir = PreferredDeveloperDir();
            if (!string.IsNullOrEmpty(developerDir))
                env["DEVELOPER_DIR"] = developerDir;
            if (!string.IsNullOrEmpty(sdkRoot))
                env["SDKROOT"] = sdkRoot;
        }

        static string QueryXcrun(string arguments, int timeoutMs)
        {
            try
            {
                var info = new ProcessStartInfo
                {
                    FileName = "/usr/bin/xcrun",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                ApplyMacBuildEnvironment(info, null);
                using (var process = Process.Start(info))
                {
                    if (process == null)
                        return null;
                    var stdout = process.StandardOutput.ReadToEnd().Trim();
                    if (!process.WaitForExit(timeoutMs))
                    {
                        try { process.Kill(); }
                        catch { /* ignore */ }
                        return null;
                    }
                    return process.ExitCode == 0 ? stdout : null;
                }
            }
            catch
            {
                return null;
            }
        }

        static bool RunTool(
            string fileName, string arguments, string workDir, int timeoutMs, string failLabel, string sdkRoot)
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
            ApplyMacBuildEnvironment(info, sdkRoot);

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
                        UnityEngine.Debug.LogError("[Play2Web] " + failLabel + ":\n" +
                            TrimToolOutput(stdout + stderr) +
                            SwiftBridgingHint(stdout + stderr));
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

        static string SwiftBridgingHint(string output)
        {
            if (string.IsNullOrEmpty(output) || output.IndexOf("SwiftBridging", StringComparison.Ordinal) < 0)
                return "";
            return "\n[Play2Web] Duplicate SwiftBridging module maps in Command Line Tools. " +
                   "If this persists, reinstall them:\n" +
                   "sudo rm -rf /Library/Developer/CommandLineTools && xcode-select --install";
        }

        static string TrimToolOutput(string text)
        {
            const int max = 4000;
            if (string.IsNullOrEmpty(text) || text.Length <= max)
                return text;
            return text.Substring(0, max) + "\n… (truncated)";
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
