using UnityEngine;

namespace Rizing.Abstract {
    public abstract class SingletonMono<T> : MonoBehaviour where T : Component {
        public bool Persistent;
    
        private static T _instance;
    
        public static T Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
            
                Debug.Log("Couldn't find SingletonMono type " + typeof(T));
                return null;
            }
            private set => _instance = value;
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            
            if (Instance == this) {
                if (!Persistent) return;
                
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}