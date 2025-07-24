using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DfineryPlugin.Editor
{
    static class DfineryPathUtil
    {
        private static string _packageRoot; 

        public static string GetPackageRoot()
        {
            if (_packageRoot != null) return _packageRoot;

            var guid = AssetDatabase.FindAssets("DfineryUnityPostProcessor t:Script").FirstOrDefault();
            if (string.IsNullOrEmpty(guid))
                throw new Exception("Cannot locate DfineryUnityPostProcessor script via AssetDatabase.");

            var scriptPath = AssetDatabase.GUIDToAssetPath(guid);
            var editorDir = Path.GetDirectoryName(scriptPath);
            var root = Path.GetFullPath(Path.Combine(editorDir, ".."));

            _packageRoot = root.Replace("\\", "/");
            return _packageRoot;
        }

        public static string FindFileInPackage(string fileName)
        {
            var guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(fileName));
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (Path.GetFileName(p).Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    return Path.GetFullPath(p);
            }
            throw new FileNotFoundException($"Cannot find {fileName} in project.");
        }
    }

    public class DfineryUnityPostProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        // ---------------- Android ----------------
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            try
            {
                var settings = Resources.Load<DfineryUnitySettings>("DfineryUnitySettings");
                if (settings == null || string.IsNullOrEmpty(settings.serviceId))
                {
                    Debug.LogWarning("[DfineryUnity] Service ID not set. Skipping meta-data injection for Android.");
                    return;
                }

                string manifestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
                if (!File.Exists(manifestPath))
                {
                    Debug.LogError("[DfineryUnity] AndroidManifest.xml not found at: " + manifestPath);
                    return;
                }

                var doc = new XmlDocument();
                doc.Load(manifestPath);

                var ns = "http://schemas.android.com/apk/res/android";
                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("android", ns);

                var applicationNode = doc.SelectSingleNode("/manifest/application");
                if (applicationNode == null)
                {
                    Debug.LogError("[DfineryUnity] Couldn't find <application> node in AndroidManifest.");
                    return;
                }

                AddOrUpdateMetaData(doc, applicationNode, ns, "com.igaworks.dfinery.unity.serviceId", settings.serviceId);

                if (!string.IsNullOrEmpty(settings.androidNotificationIconName))
                    AddOrUpdateMetaData(doc, applicationNode, ns, "com.igaworks.dfinery.unity.androidNotificationIconName", settings.androidNotificationIconName);

                if (!string.IsNullOrEmpty(settings.androidNotificationChannelId))
                    AddOrUpdateMetaData(doc, applicationNode, ns, "com.igaworks.dfinery.unity.androidNotificationChannelId", settings.androidNotificationChannelId);

                if (!string.IsNullOrEmpty(settings.androidNotificationAccentColor))
                    AddOrUpdateMetaData(doc, applicationNode, ns, "com.igaworks.dfinery.unity.androidNotificationAccentColor", settings.androidNotificationAccentColor);

                AddOrUpdateMetaData(doc, applicationNode, ns, "com.igaworks.dfinery.unity.androidLogLevel", ((int)settings.AndroidLogLevel).ToString());
                AddOrUpdateMetaData(doc, applicationNode, ns, "com.igaworks.dfinery.unity.androidLogEnable", settings.androidLogEnabled.ToString());

                doc.Save(manifestPath);
                Debug.Log("[DfineryUnity] Successfully updated AndroidManifest.xml with Dfinery settings.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DfineryUnity] Error processing AndroidManifest.xml: {e.Message}");
            }
        }

        private void AddOrUpdateMetaData(XmlDocument doc, XmlNode parent, string ns, string name, string value)
        {
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("android", ns);

            var existingNode = parent.SelectSingleNode($".//meta-data[@android:name='{name}']", nsManager);

            if (existingNode != null)
            {
                if (existingNode.Attributes["value", ns]?.Value != value)
                {
                    existingNode.Attributes["value", ns].Value = value;
                    Debug.Log($"[DfineryUnity] Updated existing {name} to {value}");
                }
            }
            else
            {
                var meta = doc.CreateElement("meta-data");
                meta.SetAttribute("name", ns, name);
                meta.SetAttribute("value", ns, value);
                parent.AppendChild(meta);
                Debug.Log($"[DfineryUnity] Added new {name}: {value}");
            }
        }

        // ---------------- iOS Info.plist ----------------
        [PostProcessBuild(999)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;

            var settings = Resources.Load<DfineryUnitySettings>("DfineryUnitySettings");
            if (settings == null || string.IsNullOrEmpty(settings.serviceId))
            {
                Debug.LogWarning("[DfineryUnity] Service ID not set. Skipping info.plist injection for iOS.");
                return;
            }

            try
            {
                string plistPath = Path.Combine(path, "Info.plist");

                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                plist.root.SetString("com.igaworks.dfinery.unity.serviceId", settings.serviceId);
                plist.root.SetBoolean("com.igaworks.dfinery.unity.iosLogEnable", settings.iosLogEnabled);

                plist.WriteToFile(plistPath);
                Debug.Log("[DfineryUnity] Successfully updated iOS Info.plist with Dfinery settings.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DfineryUnity] Error processing iOS build: {e.Message}\n{e.StackTrace}");
            }
        }

#if UNITY_IOS
        private const string NSE_NAME   = "DfineryNotificationService";
        private const string NSE_FOLDER = "DfineryNSE";
        private const string NSE_PLIST  = "Info.plist";
        private const string NSE_SWIFT  = "NotificationService.swift";

        [PostProcessBuild(35)]
        public static void CreateNseTarget(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS) return;

            var projPath = PBXProject.GetPBXProjectPath(buildPath);
            var projText = File.ReadAllText(projPath);
            if (projText.Contains($"/* {NSE_NAME} */ = {{"))
            {
                Debug.Log("[DfineryUnity] NSE already exists. Skip creation.");
                return;
            }

            var packageRoot  = DfineryPathUtil.GetPackageRoot();
            var templateRoot = Path.Combine(packageRoot, "Plugins", "NSETemplate").Replace("\\", "/");
            
            var dstRoot      = Path.Combine(buildPath, NSE_FOLDER);
            Directory.CreateDirectory(dstRoot);

            File.Copy(Path.Combine(templateRoot, NSE_PLIST),  Path.Combine(dstRoot, NSE_PLIST),  true);
            File.Copy(Path.Combine(templateRoot, NSE_SWIFT),  Path.Combine(dstRoot, NSE_SWIFT),  true);

            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

#if UNITY_2019_3_OR_NEWER
            string mainTargetGuid = proj.GetUnityMainTargetGuid();
#else
            string mainTargetGuid = proj.TargetGuidByName("Unity-iPhone");
#endif

            var mainBundleId = proj.GetBuildPropertyForAnyConfig(mainTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER");
            var nseBundleId  = mainBundleId + ".nse";

            var plistRelPath = Path.Combine(NSE_FOLDER, NSE_PLIST).Replace("\\", "/");
            string nseTargetGuid = AddAppExtensionCompat(proj, mainTargetGuid, NSE_NAME, nseBundleId, plistRelPath);
            var deploy = proj.GetBuildPropertyForAnyConfig(mainTargetGuid, "IPHONEOS_DEPLOYMENT_TARGET");

            if (!string.IsNullOrEmpty(deploy))
                proj.SetBuildProperty(nseTargetGuid, "IPHONEOS_DEPLOYMENT_TARGET", deploy);

            var swiftRel = Path.Combine(NSE_FOLDER, NSE_SWIFT).Replace("\\", "/");
            var swiftGuid = proj.AddFile(swiftRel, swiftRel, PBXSourceTree.Source);
            proj.AddFileToBuild(nseTargetGuid, swiftGuid);

            proj.AddBuildProperty(nseTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.AddBuildProperty(nseTargetGuid, "SWIFT_VERSION", "5.0");

            proj.WriteToFile(projPath);
            Debug.Log("[DfineryUnity] NSE target created before EDM4U.");
        }

        private static string AddAppExtensionCompat(PBXProject proj,
                                                    string mainTargetGuid,
                                                    string name,
                                                    string bundleId,
                                                    string infoPlistRelPath)
        {
            var methods = typeof(PBXProjectExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                      .Where(m => m.Name == "AddAppExtension");
            foreach (var m in methods)
            {
                var ps = m.GetParameters();
                try
                {
                    if (ps.Length == 5)
                    {
                        return (string)m.Invoke(null, new object[]
                        { proj, mainTargetGuid, name, bundleId, infoPlistRelPath });
                    }
                    if (ps.Length == 6)
                    {
                        return (string)m.Invoke(null, new object[]
                        { proj, mainTargetGuid, name, bundleId, infoPlistRelPath, name });
                    }
                }
                catch { }
            }
            return AddAppExtensionManual(proj, mainTargetGuid, name, bundleId, infoPlistRelPath);
        }

        private static string AddAppExtensionManual(PBXProject proj,
                                                    string mainTargetGuid,
                                                    string name,
                                                    string bundleId,
                                                    string infoPlistRelPath)
        {
            const string productType = "com.apple.product-type.app-extension.notification-service";
            string targetGuid = proj.AddTarget(name, productType, "iphoneos");

            proj.SetBuildProperty(targetGuid, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
            proj.SetBuildProperty(targetGuid, "INFOPLIST_FILE", infoPlistRelPath);
            proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS",
                "$(inherited) @executable_path/Frameworks @executable_path/../../Frameworks");

            var copyPhaseGuid = proj.AddCopyFilesBuildPhase(mainTargetGuid, "Embed App Extensions", "", "13");

            return targetGuid;
        }
        

        [PostProcessBuild(45)]
        public static void PatchPodfile(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS) return;

            string version = GetDfineryVersion() ?? ">= 0";

            var podfilePath = Path.Combine(buildPath, "Podfile");
            if (!File.Exists(podfilePath))
            {
                Debug.LogWarning("[DfineryUnity] Podfile not found. Skip patch.");
                return;
            }

            string content = File.ReadAllText(podfilePath);
            if (!Regex.IsMatch(content, $@"target\s+'{Regex.Escape(NSE_NAME)}'\s+do"))
            {
                content += $@"# === Pods for {NSE_NAME} (auto-added by Dfinery) ===
target '{NSE_NAME}' do
  pod 'DfinerySDKServiceExtension', '{version}'
end
";
                File.WriteAllText(podfilePath, content);
                Debug.Log("[DfineryUnity] Podfile patched for NSE target.");
            }
        }

        private static string GetDfineryVersion()
        {
            var guids = AssetDatabase.FindAssets("DfineryDependencies t:TextAsset");
            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("Dependencies t:TextAsset Dfinery");

            if (guids.Length == 0) return null;

            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var absolute = Path.GetFullPath(assetPath);

            try
            {
                var xml = XDocument.Load(absolute);
                var node = xml.Descendants("iosPod")
                              .FirstOrDefault(x => (string)x.Attribute("name") == "DfinerySDK");
                return node?.Attribute("version")?.Value;
            }
            catch (Exception e)
            {
                Debug.LogError("[DfineryUnity] read DfineryDependencies.xml failed: " + e.Message);
                return null;
            }
        }
#endif // UNITY_IOS
    }
}
