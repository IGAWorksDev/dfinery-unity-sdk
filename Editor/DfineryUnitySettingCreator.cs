#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class DfineryUnitySettingsCreator
{
    private const string AssetName = "DfineryUnitySettings";
    private const string ResourcesPath = "Assets/Dfinery/Resources";
    private const string FullPath = ResourcesPath + "/" + AssetName + ".asset";

    [MenuItem("Assets/Dfinery/Settings")]
    public static void CreateSettingsAsset()
    {
        if (!Directory.Exists(ResourcesPath))
        {
            Directory.CreateDirectory(ResourcesPath);
            AssetDatabase.Refresh();
        }

        var existingAsset = AssetDatabase.LoadAssetAtPath<DfineryUnitySettings>(FullPath);
        if (existingAsset != null)
        {
            Selection.activeObject = existingAsset;
            return;
        }

        var settings = ScriptableObject.CreateInstance<DfineryUnitySettings>();
        AssetDatabase.CreateAsset(settings, FullPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = settings;

    }
}
#endif
