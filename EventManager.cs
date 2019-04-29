using System;
using System.Collections.Generic;

namespace BToolkit
{
    public class EventManager
    {

        private Dictionary<string, List<Delegate>> eventsBuffer = new Dictionary<string, List<Delegate>>();

        #region Register
        /// <summary>
        /// 注册事件监听
        /// </summary>
        public void Register(string eventName, Action callback)
        {
            if (!eventsBuffer.ContainsKey(eventName))
            {
                eventsBuffer.Add(eventName, new List<Delegate>());
            }
            List<Delegate> callbacks = eventsBuffer[eventName];
            if (!callbacks.Contains(callback))
            {
                callbacks.Add(callback);
            }
        }
        /// <summary>
        /// 注册事件监听
        /// </summary>
        public void Register<T>(string eventName, Action<T> callback)
        {
            if (!eventsBuffer.ContainsKey(eventName))
            {
                eventsBuffer.Add(eventName, new List<Delegate>());
            }
            List<Delegate> callbacks = eventsBuffer[eventName];
            if (!callbacks.Contains(callback))
            {
                callbacks.Add(callback);
            }
        }
        /// <summary>
        /// 注册事件监听
        /// </summary>
        public void Register<T, U>(string eventName, Action<T, U> callback)
        {
            if (!eventsBuffer.ContainsKey(eventName))
            {
                eventsBuffer.Add(eventName, new List<Delegate>());
            }
            List<Delegate> callbacks = eventsBuffer[eventName];
            if (!callbacks.Contains(callback))
            {
                callbacks.Add(callback);
            }
        }
        /// <summary>
        /// 注册事件监听
        /// </summary>
        public void Register<T, U, V>(string eventName, Action<T, U, V> callback)
        {
            if (!eventsBuffer.ContainsKey(eventName))
            {
                eventsBuffer.Add(eventName, new List<Delegate>());
            }
            List<Delegate> callbacks = eventsBuffer[eventName];
            if (!callbacks.Contains(callback))
            {
                callbacks.Add(callback);
            }
        }
        /// <summary>
        /// 注册事件监听
        /// </summary>
        public void Register<T, U, V, W>(string eventName, Action<T, U, V, W> callback)
        {
            if (!eventsBuffer.ContainsKey(eventName))
            {
                eventsBuffer.Add(eventName, new List<Delegate>());
            }
            List<Delegate> callbacks = eventsBuffer[eventName];
            if (!callbacks.Contains(callback))
            {
                callbacks.Add(callback);
            }
        }
        #endregion


        #region Unregister Simple
        /// <summary>
        /// 注销事件监听
        /// </summary>
        public void Unregister(string eventName, Action callback)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                if (callbacks.Contains(callback))
                {
                    callbacks.Remove(callback);
                }
            }
        }
        /// <summary>
        /// 注销事件监听
        /// </summary>
        public void Unregister<T>(string eventName, Action<T> callback)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                if (callbacks.Contains(callback))
                {
                    callbacks.Remove(callback);
                }
            }
        }
        /// <summary>
        /// 注销事件监听
        /// </summary>
        public void Unregister<T, U>(string eventName, Action<T, U> callback)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                if (callbacks.Contains(callback))
                {
                    callbacks.Remove(callback);
                }
            }
        }
        /// <summary>
        /// 注销事件监听
        /// </summary>
        public void Unregister<T, U, V>(string eventName, Action<T, U, V> callback)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                if (callbacks.Contains(callback))
                {
                    callbacks.Remove(callback);
                }
            }
        }
        /// <summary>
        /// 注销事件监听
        /// </summary>
        public void Unregister<T, U, V, W>(string eventName, Action<T, U, V, W> callback)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                if (callbacks.Contains(callback))
                {
                    callbacks.Remove(callback);
                }
            }
        }

        /// <summary>
        /// 移除所有同名事件
        /// </summary>
        public void Unregister(string eventName)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                eventsBuffer.Remove(eventName);
            }
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public void UnregisterAll()
        {
            Debuger.Log(">>>>>>>>>>清空所有事件");
            eventsBuffer.Clear();
        }
        #endregion


        #region Execute
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Emit(string eventName)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                for (int i = 0; i < callbacks.Count; i++)
                {
                    if ((callbacks[i] as Action) != null)
                    {
                        (callbacks[i] as Action)();
                    }
                    else
                    {
                        Debuger.LogError("事件[ " + eventName + " ]所对应的回调为null或参数不统一，请检查注册参数和执行参数是否一致");
                    }
                }
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Emit<T>(string eventName, T param)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                for (int i = 0; i < callbacks.Count; i++)
                {
                    if ((callbacks[i] as Action<T>) != null)
                    {
                        (callbacks[i] as Action<T>)(param);
                    }
                    else
                    {
                        Debuger.LogError("事件[ " + eventName + " ]所对应的回调为null或参数不统一，请检查注册参数和执行参数是否一致");
                    }
                }
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Emit<T, U>(string eventName, T param1, U param2)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                for (int i = 0; i < callbacks.Count; i++)
                {
                    if ((callbacks[i] as Action<T, U>) != null)
                    {
                        (callbacks[i] as Action<T, U>)(param1, param2);
                    }
                    else
                    {
                        Debuger.LogError("事件[ " + eventName + " ]所对应的回调为null或参数不统一，请检查注册参数和执行参数是否一致");
                    }
                }
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Emit<T, U, V>(string eventName, T param1, U param2, V param3)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                for (int i = 0; i < callbacks.Count; i++)
                {
                    if ((callbacks[i] as Action<T, U, V>) != null)
                    {
                        (callbacks[i] as Action<T, U, V>)(param1, param2, param3);
                    }
                    else
                    {
                        Debuger.LogError("事件[ " + eventName + " ]所对应的回调为null或参数不统一，请检查注册参数和执行参数是否一致");
                    }
                }
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Emit<T, U, V, W>(string eventName, T param1, U param2, V param3, W param4)
        {
            if (eventsBuffer.ContainsKey(eventName))
            {
                List<Delegate> callbacks = eventsBuffer[eventName];
                for (int i = 0; i < callbacks.Count; i++)
                {
                    if ((callbacks[i] as Action<T, U, V, W>) != null)
                    {
                        (callbacks[i] as Action<T, U, V, W>)(param1, param2, param3, param4);
                    }
                    else
                    {
                        Debuger.LogError("事件[ " + eventName + " ]所对应的回调为null或参数不统一，请检查注册参数和执行参数是否一致");
                    }
                }
            }
        }
        #endregion
    }
}