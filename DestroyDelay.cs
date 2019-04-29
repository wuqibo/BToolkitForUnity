using UnityEngine;
using System.Collections;

namespace BToolkit
{
    public class DestroyDelay : MonoBehaviour
    {

        public float delay;

        void Start()
        {
            Destroy(gameObject, delay);
        }

    }
}