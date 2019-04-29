using System.Collections.Generic;
using System;

namespace BToolkit {

    /// <summary>
    /// 子线程的函数需要在主线程里运行，使用该工具类的Run()方法
    /// <para>例如：</para>
    /// <para>MainThread.Run(() => {</para>
    /// <para>子线程里的逻辑代码</para>
    /// <para>}</para>
    /// </summary>
    public class MainThread {

        private static List<Action> actions = new List<Action>();
        private MainThread() { }//禁止实例化

        /// <summary>
        /// 子线程传入函数，在主线程执行
        /// </summary>
        public static void Run(Action action) {
            actions.Add(action);
        }

        /// <summary>
        /// 在主线程里Update
        /// </summary>
        public static void Update() {
            if(actions.Count > 0) {
                for(int i = 0;i < actions.Count;i++) {
                    Action action = actions[i];
                    if(action != null) {
                        try {
                            action();
                        } catch { }
                        action = null;
                    }
                }
                actions.Clear();
            }
        }
    }
}
