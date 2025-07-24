using UnityEngine;
using DfineryPlugin;

public class DFGetGoogleAdvertisingIdCallbackConnector : AndroidJavaProxy {
    private DFGetGoogleAdvertisingIdCallback callback;
    public const string ANDROID_UNITY_CALLBACK_CLASS_NAME = "com.igaworks.dfinery.unity.DfineryUnityCallback";

    public DFGetGoogleAdvertisingIdCallbackConnector(DFGetGoogleAdvertisingIdCallback callback) : base(ANDROID_UNITY_CALLBACK_CLASS_NAME) {
        this.callback = callback;
    }
    void onCallback(string result) {
        if (callback != null) {
            if (result != null && result.Length > 0) {
                DFGoogleAdvertisingId advertisingIdInfo = DFGoogleAdvertisingId.CreateFromJSON(result);
                callback.OnGetGoogleAdvertisingId(advertisingIdInfo);
            } else {
                callback.OnGetGoogleAdvertisingId(null);
            }
        }
    }
}