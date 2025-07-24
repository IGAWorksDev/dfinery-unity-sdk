using UnityEngine;
using DfineryPlugin;

#if UNITY_ANDROID
public class DFGetPushTokenCallbackConnector : AndroidJavaProxy {
    private DFGetPushTokenCallback callback;
    public const string ANDROID_UNITY_CALLBACK_CLASS_NAME = "com.igaworks.dfinery.unity.DfineryUnityCallback";

    public DFGetPushTokenCallbackConnector(DFGetPushTokenCallback callback) : base(ANDROID_UNITY_CALLBACK_CLASS_NAME) {
        this.callback = callback;
    }
    
    void onCallback(string result) {
        if (callback != null) {
            if (result != null && result.Length > 0) {
                callback.OnGetPushToken(result);
            } else {
                callback.OnGetPushToken(null);
            }
        }
    }
}
#elif UNITY_IOS
public class DFGetPushTokenCallbackConnector {
    private DFGetPushTokenCallback callback;

    public DFGetPushTokenCallbackConnector(DFGetPushTokenCallback callback) {
        this.callback = callback;
    }

    public DFGetPushTokenCallback GetCallback() {
        return callback;
    }
}
#else
// Unity Editor나 다른 플랫폼에서는 빈 구현
public class DFGetPushTokenCallbackConnector {
    private DFGetPushTokenCallback callback;

    public DFGetPushTokenCallbackConnector(DFGetPushTokenCallback callback) {
        this.callback = callback;
    }

    public DFGetPushTokenCallback GetCallback() {
        return callback;
    }
}
#endif
