#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
#define HAS_DFINERY_SDK
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DfineryPlugin {
    public static class Constants {
        public const string KEY_STRING_CLASS_NAME = "className";
        public const string KEY_STRING_METHOD_NAME = "methodName";
        public const string KEY_OBJECT_METHOD_PARAM = "methodParam";
        public const string KEY_ANY_METHOD_ARGS_1 = "args1";
        public const string KEY_ANY_METHOD_ARGS_2 = "args2";
        public const string KEY_ANY_METHOD_ARGS_3 = "args3";
        public const string KEY_ANY_METHOD_ARGS_4 = "args4";
        public const string KEY_ANY_METHOD_ARGS_5 = "args5";
        public const string KEY_INVOKE_METHOD_NAME = "invoke";
    }
    public static class DFEvent {
        public const string LOGIN = "df_login";
        public const string LOGOUT = "df_logout";
        public const string SIGN_UP = "df_sign_up";
        public const string PURCHASE = "df_purchase";
        public const string REFUND = "df_refund";
        public const string VIEW_HOME = "df_view_home";
        public const string VIEW_PRODUCT_DETAILS = "df_view_product_details";
        public const string ADD_TO_CART = "df_add_to_cart";
        public const string ADD_TO_WISHLIST = "df_add_to_wishlist";
        public const string VIEW_SEARCH_RESULT = "df_view_search_result";
        public const string SHARE_PRODUCT = "df_share_product";
        public const string VIEW_LIST = "df_view_list";
        public const string VIEW_CART = "df_view_cart";
        public const string REMOVE_CART = "df_remove_cart";
        public const string ADD_PAYMENT_INFO = "df_add_payment_info";
    }
    public static class DFEventProperty {
        public const string ITEMS = "df_items";
        public const string ITEM_ID = "df_item_id";
        public const string ITEM_NAME = "df_item_name";
        public const string ITEM_PRICE = "df_price";
        public const string ITEM_QUANTITY = "df_quantity";
        public const string ITEM_DISCOUNT = "df_discount";
        public const string ITEM_CATEGORY1 = "df_category1";
        public const string ITEM_CATEGORY2 = "df_category2";
        public const string ITEM_CATEGORY3 = "df_category3";
        public const string ITEM_CATEGORY4 = "df_category4";
        public const string ITEM_CATEGORY5 = "df_category5";
        public const string TOTAL_REFUND_AMOUNT = "df_total_refund_amount";
        public const string ORDER_ID = "df_order_id";
        public const string DELIVERY_CHARGE = "df_delivery_charge";
        public const string PAYMENT_METHOD = "df_payment_method";
        public const string TOTAL_PURCHASE_AMOUNT = "df_total_purchase_amount";
        public const string SHARING_CHANNEL = "df_sharing_channel";
        public const string SIGN_CHANNEL = "df_sign_channel";
        public const string KEYWORD = "df_keyword";
        public const string DISCOUNT = "df_discount";
    }
    public static class DFUserProfile {
        public const string NAME = "df_name";
        public const string GENDER = "df_gender";
        public const string BIRTH = "df_birth";
        public const string MEMBERSHIP = "df_membership";
        public const string PUSH_OPTIN = "df_push_optin";
        public const string PUSH_ADS_OPTIN = "df_push_ads_optin";
        public const string SMS_ADS_OPTIN = "df_sms_ads_optin";
        public const string KAKAO_ADS_OPTIN = "df_kakao_ads_optin";
        public const string PUSH_NIGHT_ADS_OPTIN = "df_push_night_ads_optin";
    }
    public static class DFGender {
        public const string MALE = "Male";
        public const string FEMALE = "Female";
        public const string NON_BINARY = "NonBinary";
        public const string OTHER = "Other";
    }
    public enum DFIdentity {
        EXTERNAL_ID,
        EMAIL,
        PHONE_NO,
        KAKAO_USER_ID,
        LINE_USER_ID
    }

    public static class DFConfig
    {
        public const string IOS_LOG_ENABLE = "df_config_log_enable";
        public const string ANDROID_LOG_ENABLE = "android_log_enable";
        public const string ANDROID_LOG_LEVEL = "android_log_level";
        public const string ANDROID_NOTIFICATION_ICON_NAME = "android_notification_icon_name";
        public const string ANDROID_NOTIFICATION_CHANNEL_ID = "android_notification_channel_id";
        public const string ANDROID_NOTIFICATION_ACCENT_COLOR = "android_notification_accent_color";
    }
    public static class DFAndroidLogLevel {
        public const int VERBOSE = 2;
        public const int DEBUG = 3;
        public const int INFO = 4;
        public const int WARN = 5;
        public const int ERROR = 6;
        public const int ASSERT = 7;
    }

    public static class DFAndroidNotificationChannelProperty {
        public const string ID = "id";
        public const string NAME = "name";
        public const string DESCRIPTION = "description";
        public const string BADGE = "badge";
        public const string SOUND = "sound";
        public const string SOUND_URI = "soundUri";
        public const string IMPORTANCE = "importance";
        public const string LIGHTS = "lights";
        public const string VIBRATION = "vibration";
        public const string VISIBILITY = "visibility";
        public const string BYPASS_DND = "bypassDnd";
        public const string GROUP_ID = "groupId";
    }

    public static class DFAndroidNotificationChannelGroupProperty {
        public const string ID = "id";
        public const string NAME = "name";
    }

    public static class DFAndroidNotificationChannelImportance {
        public const int NONE = 0;
        public const int MIN = 1;
        public const int LOW = 2;
        public const int DEFAULT = 3;
        public const int HIGH = 4;
        public const int MAX = 5;
    }

    public static class DFAndroidNotificationChannelVisibility {
        public const int PUBLIC = 1;
        public const int PRIVATE = 0;
        public const int SECRET = -1;
    }
    public class Dfinery {
        
        private static IDfinery binding;
        public static IDfinery Binding {
            get {
                if (binding != null) {
                    return binding;
                }
#if UNITY_ANDROID
                binding = new DfineryAndroid();
#elif UNITY_IOS
                binding = new DfineryiOS();
#endif
                return binding;
            }
        }
        public static void Init(string serviceId) {
#if HAS_DFINERY_SDK
            Binding.Init(serviceId);
#endif
        }
        public static void InitWithConfig(string serviceId, Dictionary<string, object> config = null) {
#if HAS_DFINERY_SDK
            Binding.InitWithConfig(serviceId, config);
#endif
        }
        public static void LogEvent(string eventName) {
#if HAS_DFINERY_SDK
            Binding.LogEvent(eventName);
#endif
        }
        public static void LogEvent(string eventName, Dictionary<string, object> properties = null) {
#if HAS_DFINERY_SDK
            if(properties != null){
                ConvertDatesToIsoString(properties);
                Binding.LogEvent(eventName, properties);
            } else{
                Binding.LogEvent(eventName, null);
            }
#endif
        }
        public static void EnableSDK() {
#if HAS_DFINERY_SDK
            Binding.EnableSDK();
#endif
        }
        public static void DisableSDK() {
#if HAS_DFINERY_SDK
            Binding.DisableSDK();
#endif
        }
        public static void SetUserProfile(string key, object value) {
#if HAS_DFINERY_SDK
            object converted = value;
            if (value is DateTime dateTime)
            {
                converted = dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
            }
            Binding.SetUserProfile(key, converted);
#endif
        }
        public static void SetUserProfiles(Dictionary<string, object> values) {
#if HAS_DFINERY_SDK
            Dictionary<string, object> converted = new Dictionary<string, object>();
            foreach (var kvp in values)
            {
                if (kvp.Value is DateTime dateTime) {
                    converted[kvp.Key] = dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
                } else {
                    converted[kvp.Key] = kvp.Value;
                }
            }
            Binding.SetUserProfiles(converted);
#endif
        }
        public static void SetIdentity(DFIdentity key, string value) {
#if HAS_DFINERY_SDK
            Binding.SetIdentity(GetIdentity(key), value);
#endif
        }
        public static void SetIdentities(Dictionary<DFIdentity, string> values) {
#if HAS_DFINERY_SDK
            Dictionary<string, string> converted = ConvertIdentityToString(values);
            Binding.SetIdentities(converted);
#endif
        }
        public static void ResetIdentity() {
#if HAS_DFINERY_SDK
            Binding.ResetIdentity();
#endif
        }
        public static void SetGoogleAdvertisingId(string googleAdvertisingId, bool isLimitAdTrackingEnabled) {
#if HAS_DFINERY_SDK
            Binding.SetGoogleAdvertisingId(googleAdvertisingId, isLimitAdTrackingEnabled);
#endif
        }
        public static void SetPushToken(string pushToken) {
#if HAS_DFINERY_SDK
            Binding.SetPushToken(pushToken);
#endif
        }
        public static void GetPushToken(DFGetPushTokenCallback callback) {
#if HAS_DFINERY_SDK
            Binding.GetPushToken(callback);
#endif
        }
        public static void GetGoogleAdvertisingId(DFGetGoogleAdvertisingIdCallback callback) {
#if UNITY_ANDROID
            Binding.GetGoogleAdvertisingId(callback);
#endif
        }
        public static void CreateNotificationChannel(Dictionary<string, object> properties = null) {
#if UNITY_ANDROID
            Binding.CreateNotificationChannel(properties);
#endif
        }
        public static void DeleteNotificationChannel(string channelId) {
#if UNITY_ANDROID
            Binding.DeleteNotificationChannel(channelId);
#endif
        }
        public static void CreateNotificationChannelGroup(Dictionary<string, object> properties = null) {
#if UNITY_ANDROID
            Binding.CreateNotificationChannelGroup(properties);
#endif
        }
        public static void DeleteNotificationChannelGroup(string channelGroupId) {
#if UNITY_ANDROID
            Binding.DeleteNotificationChannelGroup(channelGroupId);
#endif
        }
        private static string GetIdentity(DFIdentity identity) {
            switch (identity) {
                case DFIdentity.EXTERNAL_ID: return "external_id";
                case DFIdentity.EMAIL: return "email";
                case DFIdentity.PHONE_NO: return "phone_no";
                case DFIdentity.KAKAO_USER_ID: return "kakao_user_id";
                case DFIdentity.LINE_USER_ID: return "line_user_id";
                default: return null;
            }
        }

        private static Dictionary<string, string> ConvertIdentityToString(Dictionary<DFIdentity, string> values) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (KeyValuePair<DFIdentity, string> pair in values) {
                string identityString = GetIdentity(pair.Key);
                if (identityString != null) {
                    result[identityString] = pair.Value;
                }
            }
            return result;
        }

        private static void ConvertDatesToIsoString(object obj)
        {
            switch (obj)
            {
                case null:
                    return;

                // Dictionary<string, object>인 경우
                case Dictionary<string, object> map:
                    {
                        foreach (var key in new List<string>(map.Keys))
                        {
                            var value = map[key];
                            // 해당 value를 재귀적으로 처리
                            map[key] = ConvertObject(value);
                        }
                        break;
                    }

                // IList (예: List<Dictionary<string, object>>, List<object>, 등)인 경우
                case IList list:
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            // 리스트 항목을 재귀적으로 처리
                            list[i] = ConvertObject(item);
                        }
                        break;
                    }
            }
        }

        // Dictionary, List, DateTime 등을 처리하는 ConvertObject
        private static object ConvertObject(object value)
        {
            switch (value)
            {
                case DateTime dt:
                    return dt.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");

                case Dictionary<string, object> dict:
                    ConvertDatesToIsoString(dict);
                    return dict;

                case IList list:
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = ConvertObject(list[i]);
                    }
                    return list;

                default:
                    // DateTime, Dictionary, IList가 아니면 그대로 반환
                    return value;
            }
        }
    }
}