using UnityEngine;

namespace DfineryPlugin {
#if UNITY_IOS
    public class DfineryiOSCallbackReceiver : MonoBehaviour {
        private static DfineryiOSCallbackReceiver _instance;
        
        public static DfineryiOSCallbackReceiver Instance {
            get {
                if (_instance == null) {
                    CreateInstance();
                }
                return _instance;
            }
        }
        
        private static void CreateInstance() {
            if (_instance != null) return;
            
            GameObject go = new GameObject("DfineryiOSCallbackReceiver");
            _instance = go.AddComponent<DfineryiOSCallbackReceiver>();
            DontDestroyOnLoad(go);
            
            Debug.Log("[Dfinery] iOS Callback Receiver created");
        }
        
        public void OnPushTokenReceived(string pushToken) {
            try {
                Debug.Log($"[Dfinery] Received push token from native iOS: {pushToken}");
                DfineryiOS.OnPushTokenReceived(pushToken);
            }
            catch (System.Exception e) {
                Debug.LogError($"[Dfinery] Error handling push token callback: {e.Message}\n{e.StackTrace}");
            }
        }
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy() {
            if (_instance == this) {
                _instance = null;
            }
        }
    }
#endif
} 