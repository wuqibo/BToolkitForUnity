using UnityEngine;
using System;

namespace BToolkit
{
    public class BTimer:MonoBehaviour
    {
        class Function
        {
            public Delegate action;
            public virtual void Execute()
            {
                (action as Action)();
            }
        }
        class FunctionWithParams<T>:Function
        {
            public T param;
            public override void Execute()
            {
                (action as Action<T>)(param);
            }
        }
        class FunctionWithParams<T, U>:Function
        {
            public T param1;
            public U param2;
            public override void Execute()
            {
                (action as Action<T,U>)(param1,param2);
            }
        }
        class FunctionWithParams<T, U, V>:Function
        {
            public T param1;
            public U param2;
            public V param3;
            public override void Execute()
            {
                (action as Action<T,U,V>)(param1,param2,param3);
            }
        }
        class FunctionWithParams<T, U, V, W>:Function
        {
            public T param1;
            public U param2;
            public V param3;
            public W param4;
            public override void Execute()
            {
                (action as Action<T,U,V,W>)(param1,param2,param3,param4);
            }
        }
        public float t;
        Function function;

        public void SetId(string timerId)
        {
            if(string.IsNullOrEmpty(timerId))
            {
                gameObject.name = "BTimer";
            }
            else
            {
                gameObject.name = timerId;
            }
        }

        void Update()
        {
            if(t >= 0f)
            {
                t -= Time.deltaTime;
                if(t < 0f)
                {
                    if(function != null)
                    {
                        function.Execute();
                    }
                    try
                    {
                        DestroyImmediate(gameObject);
                    }catch{ }
                }
            }
        }

        /// <summary>
        /// timerId参数用于确保仅有一个Timer触发同一个方法，和任何时刻提前销毁特定Timer,通过DestroyByTimerId(string timerId)方法
        /// </summary>
        public static void Invoke(float daley,Action function,string timerId = null)
        {
            if(daley == 0)
            {
                function();
            }
            else
            {
                BTimer timer = null;
                if(timerId != null)
                {
                    GameObject go = GameObject.Find(timerId);
                    if(go)
                    {
                        timer = go.GetComponent<BTimer>();
                    }
                }
                if(!timer)
                {
                    GameObject go = new GameObject();
                    timer = go.AddComponent<BTimer>();
                }
                timer.t = daley;
                Function fun = new Function();
                fun.action = function;
                timer.function = fun;
                timer.SetId(timerId);
            }
        }

        /// <summary>
        /// timerId参数用于确保仅有一个Timer触发同一个方法，和任何时刻提前销毁特定Timer,通过DestroyByTimerId(string timerId)方法
        /// </summary>
        public static void Invoke<T>(float daley,Action<T> function,T param,string timerId = null)
        {
            if(daley == 0)
            {
                function.Invoke(param);
            }
            else
            {
                BTimer timer = null;
                if(timerId != null)
                {
                    GameObject go = GameObject.Find(timerId);
                    if(go)
                    {
                        timer = go.GetComponent<BTimer>();
                    }
                }
                if(!timer)
                {
                    GameObject go = new GameObject();
                    timer = go.AddComponent<BTimer>();
                }
                timer.t = daley;
                FunctionWithParams<T> fun = new FunctionWithParams<T>();
                fun.action = function;
                fun.param = param;
                timer.function = fun;
                timer.SetId(timerId);
            }
        }

        /// <summary>
        /// timerId参数用于确保仅有一个Timer触发同一个方法，和任何时刻提前销毁特定Timer,通过DestroyByTimerId(string timerId)方法
        /// </summary>
        public static void Invoke<T, U>(float daley,Action<T,U> function,T param1,U param2,string timerId = null)
        {
            if(daley == 0)
            {
                function.Invoke(param1,param2);
            }
            else
            {
                BTimer timer = null;
                if(timerId != null)
                {
                    GameObject go = GameObject.Find(timerId);
                    if(go)
                    {
                        timer = go.GetComponent<BTimer>();
                    }
                }
                if(!timer)
                {
                    GameObject go = new GameObject();
                    timer = go.AddComponent<BTimer>();
                }
                timer.t = daley;
                FunctionWithParams<T,U> fun = new FunctionWithParams<T,U>();
                fun.action = function;
                fun.param1 = param1;
                fun.param2 = param2;
                timer.function = fun;
                timer.SetId(timerId);
            }
        }

        /// <summary>
        /// timerId参数用于确保仅有一个Timer触发同一个方法，和任何时刻提前销毁特定Timer,通过DestroyByTimerId(string timerId)方法
        /// </summary>
        public static void Invoke<T, U, V>(float daley,Action<T,U,V> function,T param1,U param2,V param3,string timerId = null)
        {
            if(daley == 0)
            {
                function.Invoke(param1,param2,param3);
            }
            else
            {
                BTimer timer = null;
                if(timerId != null)
                {
                    GameObject go = GameObject.Find(timerId);
                    if(go)
                    {
                        timer = go.GetComponent<BTimer>();
                    }
                }
                if(!timer)
                {
                    GameObject go = new GameObject();
                    timer = go.AddComponent<BTimer>();
                }
                timer.t = daley;
                FunctionWithParams<T,U,V> fun = new FunctionWithParams<T,U,V>();
                fun.action = function;
                fun.param1 = param1;
                fun.param2 = param2;
                fun.param3 = param3;
                timer.function = fun;
                timer.SetId(timerId);
            }
        }

        /// <summary>
        /// timerId参数用于确保仅有一个Timer触发同一个方法，和任何时刻提前销毁特定Timer,通过DestroyByTimerId(string timerId)方法
        /// </summary>
        public static void Invoke<T, U, V, W>(float daley,Action<T,U,V,W> function,T param1,U param2,V param3,W param4,string timerId = null)
        {
            if(daley == 0)
            {
                function.Invoke(param1,param2,param3,param4);
            }
            else
            {
                BTimer timer = null;
                if(timerId != null)
                {
                    GameObject go = GameObject.Find(timerId);
                    if(go)
                    {
                        timer = go.GetComponent<BTimer>();
                    }
                }
                if(!timer)
                {
                    GameObject go = new GameObject();
                    timer = go.AddComponent<BTimer>();
                }
                timer.t = daley;
                FunctionWithParams<T,U,V,W> fun = new FunctionWithParams<T,U,V,W>();
                fun.action = function;
                fun.param1 = param1;
                fun.param2 = param2;
                fun.param3 = param3;
                fun.param4 = param4;
                timer.function = fun;
                timer.SetId(timerId);
            }
        }

        /// <summary>
        /// 若执行Invoke时传入了timerId参数的,任何时刻都可执行该方法销毁Timer，否则Timer结束时才会自动销毁
        /// </summary>
        public static void DestroyByTimerId(string timerId)
        {
            GameObject go = GameObject.Find(timerId);
            if(go)
            {
                DestroyImmediate(go);
            }
        }
    }
}