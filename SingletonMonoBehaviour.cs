using UnityEngine;

namespace BToolkit
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<T>();
                    if (!instance)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            instance = GetComponent<T>();
        }
		
		/// <summary>
        /// 当前有实例存在
        /// </summary>
        public static bool HaveInstance { get { return instance; } }

        /// <summary>
        /// 主动销毁
        /// </summary>
        public static void Destroy()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
            }
        }
    }
}