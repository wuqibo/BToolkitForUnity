using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class ObjectPool<T> where T : Component
    {

        public List<T> objs;
        private T prefab;
        public int Length { get { return objs.Count; } }

        public ObjectPool(T prefab)
        {
            this.prefab = prefab;
            prefab.gameObject.SetActive(false);
            objs = new List<T>();
        }

        /// <summary>
        /// 取出
        /// </summary>
        public T Pop(Transform parent = null)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                T obj = objs[i];
                if (!obj.gameObject.activeSelf)
                {
                    if (parent)
                    {
                        obj.transform.SetParent(parent, false);
                    }
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            T newObj = GameObject.Instantiate(prefab);
            objs.Add(newObj);
            if (parent)
            {
                newObj.transform.SetParent(parent, false);
            }
            if (!newObj.gameObject.activeSelf)
            {
                newObj.gameObject.SetActive(true);
            }
            return newObj;
        }

        /// <summary>
        /// 放回
        /// </summary>
        public void Push(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        /// <summary>
        /// 全部放回
        /// </summary>
        public void PushAll()
        {
            for (int i = 0; i < objs.Count; i++)
            {
                T obj = objs[i];
                if (obj)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 销毁所有对象
        /// </summary>
        public void Destroy()
        {
            for (int i = 0; i < objs.Count; i++)
            {
                T obj = objs[i];
                if (obj)
                {
                    GameObject.Destroy(obj.gameObject);
                }
            }
            objs.Clear();
        }

    }
}