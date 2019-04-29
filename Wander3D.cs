using UnityEngine;

namespace BToolkit
{
    public class Wander3D : MonoBehaviour
    {

        public float moveSpeed = 10;
        public float rotateSpeed = 2f;
        public Vector3 limitMax = new Vector3(5, 5, 5f);
        public Vector3 limitMin = new Vector3(-5, -5, -5f);
        float timer;
        Vector3 targetPos;

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = Random.Range(1f, 3f);
                targetPos = new Vector3(Random.Range(limitMin.x, limitMax.x), Random.Range(limitMin.y, limitMax.y), Random.Range(limitMin.z, limitMax.z));
            }
            Wander(targetPos);
        }
        void Wander(Vector3 targetPos)
        {
            Vector3 targetDir = targetPos - transform.localPosition;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.Translate(Vector3.forward * moveSpeed * 0.5f * Time.deltaTime);
        }
    }
}