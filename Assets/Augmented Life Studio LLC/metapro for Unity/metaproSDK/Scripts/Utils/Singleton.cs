using UnityEngine;

namespace metaproSDK.Scripts.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
    
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    _instance?.GetComponent<Singleton<T>>().Initialize();
                }

                return _instance;
            }
        }
    
        private void Awake() 
        { 
            if (_instance != null && _instance != this as T) 
            { 
                Destroy(gameObject); 
            } 
            else 
            { 
                _instance = this as T; 
            } 
        }

        protected virtual void Initialize()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}