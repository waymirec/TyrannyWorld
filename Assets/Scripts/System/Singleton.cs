using UnityEngine;

namespace System
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        #region Fields
        private static object Lock = new object();
        private static T instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (Lock)
                    {
                        if (instance == null)
                        {
                            instance = FindObjectOfType<T>();
                            if (instance == null)
                            {
                                GameObject obj = new GameObject();
                                obj.name = typeof(T).Name;
                                instance = obj.AddComponent<T>();
                            }
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }
        
        #endregion
    }
}