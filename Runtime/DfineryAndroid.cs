using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DfineryPlugin;

#if UNITY_ANDROID
public class DfineryAndroid : IDfinery {
    private string Dfinery = "Dfinery";
    private string DfineryProperties = "DfineryProperties";

    private static AndroidJavaObject dfineryUnityActivity;
    private static AndroidJavaObject dfineryUnityBridge;
    public AndroidJavaObject DfineryUnityActivity {
        get {
            if (dfineryUnityActivity == null) {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    dfineryUnityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
            }
            return dfineryUnityActivity;
        }
    }
    public AndroidJavaObject DfineryUnityBridge {
        get {
            if (dfineryUnityBridge == null) {
                using (AndroidJavaClass dfineryBridgeClass = new AndroidJavaClass("com.igaworks.dfinery.unity.DfineryUnityBridge")) {
                    AndroidJavaObject dfineryBridgeInstance = dfineryBridgeClass.CallStatic<AndroidJavaObject>("getInstance");
                    dfineryUnityBridge = dfineryBridgeInstance;
                }
            }
        
            return dfineryUnityBridge;
        }
    }
    public void Init(string serviceId) {
        DfineryUnityBridge.Call("init", DfineryUnityActivity, serviceId);
        DfineryUnityBridge.Call("registerActivityLifecycleCallbacks", DfineryUnityActivity);
    }
    public void InitWithConfig(string serviceId, Dictionary<string, object> config = null) {
        DfineryUnityBridge.Call("initWithConfig", DfineryUnityActivity, serviceId, Json.Serialize(config));
        DfineryUnityBridge.Call("registerActivityLifecycleCallbacks", DfineryUnityActivity);
    }

    public void LogEvent(string eventName) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, eventName }
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, Dfinery },
                { Constants.KEY_STRING_METHOD_NAME, "logEvent" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
        };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void LogEvent(string eventName, Dictionary<string, object> properties = null) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, eventName }
            };
        if (properties != null) {
            methodParam[Constants.KEY_ANY_METHOD_ARGS_2] = properties;
        }
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, Dfinery },
                { Constants.KEY_STRING_METHOD_NAME, "logEvent" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void ResetIdentity() {
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties },
                { Constants.KEY_STRING_METHOD_NAME, "resetIdentity" }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void SetIdentities(Dictionary<string, string> values) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, values }
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties },
                { Constants.KEY_STRING_METHOD_NAME, "setIdentities" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void SetIdentity(string key, string value) {
        var methodParam = new Dictionary<string, string>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, key },
                { Constants.KEY_ANY_METHOD_ARGS_2, value },
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties },
                { Constants.KEY_STRING_METHOD_NAME, "setIdentity" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void SetUserProfile(string key, object value) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, key },
                { Constants.KEY_ANY_METHOD_ARGS_2, value },
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties },
                { Constants.KEY_STRING_METHOD_NAME, "setUserProfile" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void SetUserProfiles(Dictionary<string, object> values) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, values }
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties },
                { Constants.KEY_STRING_METHOD_NAME, "setUserProfiles" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void EnableSDK() {
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, Dfinery},
                { Constants.KEY_STRING_METHOD_NAME, "enableSDK" }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }
    public void DisableSDK() {
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, Dfinery},
                { Constants.KEY_STRING_METHOD_NAME, "disableSDK" }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }
    public void SetGoogleAdvertisingId(string googleAdvertisingId, bool isLimitAdTrackingEnabled) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, googleAdvertisingId },
                { Constants.KEY_ANY_METHOD_ARGS_2, isLimitAdTrackingEnabled },
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties},
                { Constants.KEY_STRING_METHOD_NAME, "setGoogleAdvertisingId" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void SetPushToken(string pushToken) {
        var methodParam = new Dictionary<string, object>
        {
                { Constants.KEY_ANY_METHOD_ARGS_1, pushToken }
            };
        var message = new Dictionary<string, object>
        {
                { Constants.KEY_STRING_CLASS_NAME, DfineryProperties},
                { Constants.KEY_STRING_METHOD_NAME, "setPushToken" },
                { Constants.KEY_OBJECT_METHOD_PARAM, methodParam }
            };
        DfineryUnityBridge.Call<string>("invokeWithString", Json.Serialize(message));
    }

    public void GetPushToken(DFGetPushTokenCallback callback) {
        DfineryUnityBridge.Call("getPushToken", new DFGetPushTokenCallbackConnector(callback));
    }
    public void GetGoogleAdvertisingId(DFGetGoogleAdvertisingIdCallback callback) {
        DfineryUnityBridge.Call("getGoogleAdvertisingId", DfineryUnityActivity, new DFGetGoogleAdvertisingIdCallbackConnector(callback));
    }

    public void CreateNotificationChannel(Dictionary<string, object> properties) {
        DfineryUnityBridge.Call("createNotificationChannel", DfineryUnityActivity, Json.Serialize(properties));
    }

    public void DeleteNotificationChannel(string channelId) {
        DfineryUnityBridge.Call("deleteNotificationChannel", DfineryUnityActivity, channelId);
    }

    public void CreateNotificationChannelGroup(Dictionary<string, object> properties) {
        DfineryUnityBridge.Call("createNotificationChannelGroup", DfineryUnityActivity, Json.Serialize(properties));
    }

    public void DeleteNotificationChannelGroup(string channelGroupId) {
        DfineryUnityBridge.Call("deleteNotificationChannelGroup", DfineryUnityActivity, channelGroupId);
    }
}
#endif