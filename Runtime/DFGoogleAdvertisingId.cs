using System;
using UnityEngine;

namespace DfineryPlugin {
    [Serializable]
    public class DFGoogleAdvertisingId {
        public string googleAdvertisingId;
        public bool isLimitAdTrackingEnabled;
        public static DFGoogleAdvertisingId CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<DFGoogleAdvertisingId>(jsonString);
        }
    }
}