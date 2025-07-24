using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DfineryPlugin {
    public interface IDfinery {
        void Init(string serviceId);
        void InitWithConfig(string serviceId, Dictionary<string, object> properties = null);
        void LogEvent(string eventName);
        void LogEvent(string eventName, Dictionary<string, object> properties = null);
        void EnableSDK();
        void DisableSDK();
        void SetUserProfile(string key, object value);
        void SetUserProfiles(Dictionary<string, object> values);
        void SetIdentity(string key, string value);
        void SetIdentities(Dictionary<string, string> values);
        void ResetIdentity();
        void SetPushToken(string pushToken);
        void GetPushToken(DFGetPushTokenCallback callback);
        void SetGoogleAdvertisingId(string googleAdvertisingId, bool isLimitAdTrackingEnabled);
        void GetGoogleAdvertisingId(DFGetGoogleAdvertisingIdCallback callback);
        void CreateNotificationChannel(Dictionary<string, object> properties);
        void DeleteNotificationChannel(string channelId);
        void CreateNotificationChannelGroup(Dictionary<string, object> properties);
        void DeleteNotificationChannelGroup(string channelGroupId);
    }
}
