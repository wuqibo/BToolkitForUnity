using System;
using System.Collections;
using UnityEngine;

namespace BToolkit
{
    public class Task : MonoBehaviour
    {
        private Action work;

        public static void NewTask(float delay, float interval, int times, Action work)
        {
            GameObject go = new GameObject("Task_" + UnityEngine.Random.Range(0, 100));
            Task task = go.AddComponent<Task>();
            task.StartCoroutine(task.StartWork(delay, interval, times, work));
        }

        IEnumerator StartWork(float delay, float interval, int times, Action work)
        {
            yield return new WaitForSeconds(delay);
            for (int i = 0; i < times; i++)
            {
                if (work == null)
                {
                    break;
                }
                work();
                yield return new WaitForSeconds(interval);
            }
            Destroy(gameObject);
        }

    }
}