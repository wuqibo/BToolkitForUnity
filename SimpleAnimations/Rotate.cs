using UnityEngine;

namespace BToolkit
{
    public class Rotate : MonoBehaviour
    {

        public Vector3 velocity = Vector3.one * 100;

        void Update()
        {
            transform.Rotate(velocity * Time.deltaTime);
        }
    }
}