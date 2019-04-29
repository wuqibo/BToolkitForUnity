namespace BToolkit
{
    public abstract class Singleton<T> : SingletonInterface where T : new()
    {
        private static T instance;
        private static readonly object syslock = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        instance = new T();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 主动销毁
        /// </summary>
        public static void Destroy()
        {
            if (instance != null)
            {
                ((SingletonInterface)instance).OnDestroy();
            }
            instance = default(T);
        }

        public virtual void OnDestroy() { }
    }

}