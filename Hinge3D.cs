using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class Hinge3D : MonoBehaviour
    {

        public Transform target;
        public bool targetForward = true;
        float maxDis;

        void Start()
        {
            InitMaxDis();
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            InitMaxDis();
        }

        public void InitMaxDis()
        {
            if (target)
            {
                maxDis = Vector3.Distance(transform.position, target.position);
            }
        }

        void Update()
        {
            if (target)
            {
                float dis = Vector3.Distance(transform.position, target.position);
                if (dis > maxDis)
                {
                    target.position = transform.position + (target.position - transform.position) * maxDis / dis;
                }
                if (targetForward)
                {
                    target.LookAt(transform);
                }
            }
        }

        void OnDrawGizmos()
        {
            //连线
            if (target)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}