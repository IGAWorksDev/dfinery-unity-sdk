using System.Collections;
using System.Collections.Generic;
using DfineryPlugin;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;

namespace DfineryPlugin {
    public class DfineryiOS : IDfinery {
        [DllImport("__Internal")]
        private static extern void _dfineryInit(string serviceId);

        [DllImport("__Internal")]
        private static extern void _dfineryInitWithConfig(string serviceId, string properties);

        [DllImport("__Internal")]
        private static extern void _dfineryLogEvent(string eventName);

        [DllImport("__Internal")]
        private static extern void _dfineryLogEventWithProps(string eventName, string properties);

        [DllImport("__Internal")]
        private static extern void _dfineryEnableSDK();

        [DllImport("__Internal")]
        private static extern void _dfineryDisableSDK();

        [DllImport("__Internal")]
        private static extern void _dfinerySetUserProfile(string key, string value);

        [DllImport("__Internal")]
        private static extern void _dfinerySetUserProfiles(string properties);

        [DllImport("__Internal")]
        private static extern void _dfinerySetIdentity(string key, string value);

        [DllImport("__Internal")]
        private static extern void _dfinerySetIdentities(string properties);

        [DllImport("__Internal")]
        private static extern void _dfineryResetIdentity();

        [DllImport("__Internal")]
        private static extern void _dfinerySetPushToken(string pushToken);

        [DllImport("__Internal")]
        private static extern void _dfineryGetPushToken();

        private static DFGetPushTokenCallbackConnector _pushTokenCallbackConnector;

        public void Init(string serviceId) {
            _dfineryInit(serviceId);
        }

        public void InitWithConfig(string serviceId, Dictionary<string, object> properties = null) {
            string propsJson = properties != null ? Json.Serialize(properties) : "{}";
            _dfineryInitWithConfig(serviceId, propsJson);
        }

        public void LogEvent(string eventName) {
            _dfineryLogEvent(eventName);
        }

        public void LogEvent(string eventName, Dictionary<string, object> properties = null) {
            string propsJson = properties != null ? Json.Serialize(properties) : "{}";
            _dfineryLogEventWithProps(eventName, propsJson);
        }

        public void EnableSDK() {
            _dfineryEnableSDK();
        }

        public void DisableSDK() {
            _dfineryDisableSDK();
        }

        public void SetUserProfile(string key, object value) {
            string valueStr = value?.ToString() ?? "";
            _dfinerySetUserProfile(key, valueStr);
        }

        public void SetUserProfiles(Dictionary<string, object> values) {
            string propsJson = values != null ? Json.Serialize(values) : "{}";
            _dfinerySetUserProfiles(propsJson);
        }

        public void SetIdentity(string key, string value) {
            _dfinerySetIdentity(key, value);
        }

        public void SetIdentities(Dictionary<string, string> values) {
            string propsJson = values != null ? Json.Serialize(values) : "{}";
            _dfinerySetIdentities(propsJson);
        }

        public void ResetIdentity() {
            _dfineryResetIdentity();
        }

        public void SetPushToken(string pushToken) {
            _dfinerySetPushToken(pushToken);
        }

        public void GetPushToken(DFGetPushTokenCallback callback) {
            _pushTokenCallbackConnector = new DFGetPushTokenCallbackConnector(callback);
            
            var receiver = DfineryiOSCallbackReceiver.Instance;
            
            _dfineryGetPushToken();
        }

        public static void OnPushTokenReceived(string pushToken) {
            if (_pushTokenCallbackConnector != null) {
                var callback = _pushTokenCallbackConnector.GetCallback();
                if (callback != null) {
                    callback.OnGetPushToken(pushToken);
                }
                _pushTokenCallbackConnector = null; 
            }
        }

        public void SetGoogleAdvertisingId(string googleAdvertisingId, bool isLimitAdTrackingEnabled) {
        }

        public void GetGoogleAdvertisingId(DFGetGoogleAdvertisingIdCallback callback) {
        }

        public void CreateNotificationChannel(Dictionary<string, object> properties) {
        }

        public void DeleteNotificationChannel(string channelId) {
        }

        public void CreateNotificationChannelGroup(Dictionary<string, object> properties) {
        }

        public void DeleteNotificationChannelGroup(string channelGroupId) {
        }
    }
}
#endif