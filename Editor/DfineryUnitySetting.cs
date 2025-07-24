using UnityEngine;

public enum DfineryLogLevel
{
    Verbose = 2,
    Debug = 3,
    Info = 4,
    Warn = 5,
    Error = 6
}


public class DfineryUnitySettings : ScriptableObject
{
    [Header("Basic Settings")]
    [Tooltip("Enter your Dfinery Service ID")]

    // com.igaworks.dfinery.unity.serviceId
    public string serviceId;
    
    [Header("Android Settings")]
    [Tooltip("Enter the name of the drawable resource for the small icon to be used in Android notifications.")]
    
    // com.igaworks.dfinery.unity.androidNotificationIconName
    public string androidNotificationIconName;

    [Tooltip("Enter the notification channel id to be used for notifications on Android 8.0 and higher.")]
    
    // com.igaworks.dfinery.unity.androidNotificationChannelId
    public string androidNotificationChannelId;

    [Tooltip("Enter the notification accent color to be used for notifications in Android in hex string format, such as “#FFFFFF”.")]

    // com.igaworks.dfinery.unity.androidNotificationAccentColor
    public string androidNotificationAccentColor;

    [Header("Log Settings")]
    [Header("Android Log Settings")]
    [Tooltip("Enable Android logging")]
    
    // com.igaworks.dfinery.unity.androidLogEnable
    public bool androidLogEnabled;
    
    [Tooltip("Set the Android log output level")]
    [SerializeField]

    // com.igaworks.dfinery.unity.androidLogLevel
    private DfineryLogLevel androidLogLevel = DfineryLogLevel.Verbose;
    
    public DfineryLogLevel AndroidLogLevel
    {
        get => androidLogLevel;
        set => androidLogLevel = value;
    }

    [Header("iOS Log Settings")]
    [Tooltip("Enable iOS logging")]

    // com.igaworks.dfinery.unity.iosLogEnable
    public bool iosLogEnabled;
}
