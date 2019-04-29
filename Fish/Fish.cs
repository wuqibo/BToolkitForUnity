using UnityEngine;

namespace BToolkit
{
    public class Fish : MonoBehaviour
    {

        [System.Serializable]
        public class Data
        {
            public float moveSpeed, rotateSpeed, swaySpeed, swayRange;
        }
        public Vector3 limitMax = new Vector3(5, 5, 5f);
        public Vector3 limitMin = new Vector3(-5, -5, -5f);
        public bool canMove = true;
        public bool canRotate = true;
        public bool canSway = true;
        public Transform ctrlBone;
        public float dataProgress = 0.5f;
        public Data idle, chase;
        float moveSpeed, rotateSpeed;
        float wanderTimer;
        Vector3 targetPos;
        float swaySpeed, swayRange;
        Vector3 boneStartupPos;
        float r;
        //状态
        enum State
        {
            Wander,
            Chase
        }
        State currState = State.Wander;

        void Start()
        {
            boneStartupPos = ctrlBone.localPosition;
        }

        void Update()
        {
            moveSpeed = idle.moveSpeed + (chase.moveSpeed - idle.moveSpeed) * dataProgress;
            rotateSpeed = idle.rotateSpeed + (chase.rotateSpeed - idle.rotateSpeed) * dataProgress;
            swaySpeed = idle.swaySpeed + (chase.swaySpeed - idle.swaySpeed) * dataProgress;
            swayRange = idle.swayRange + (chase.swayRange - idle.swayRange) * dataProgress;

            switch (currState)
            {
                case State.Wander:
                    wanderTimer -= Time.deltaTime;
                    if (wanderTimer < 0)
                    {
                        wanderTimer = Random.Range(1f, 3f);
                        targetPos = new Vector3(Random.Range(limitMin.x, limitMax.x), Random.Range(limitMin.y, limitMax.y), Random.Range(limitMin.z, limitMax.z));
                    }
                    break;
                case State.Chase:
                    float dis = Vector3.Distance(transform.position, targetPos);
                    if (dis < 2)
                    {
                        currState = State.Wander;
                    }
                    break;
            }
            MoveUpdate();
            if (canSway)
            {
                SwayUpdate();
            }
        }

        void MoveUpdate()
        {
            if (canRotate)
            {
                Vector3 targetDir = targetPos - transform.localPosition;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
            if (canMove)
            {
                transform.Translate(Vector3.forward * moveSpeed * 0.5f * Time.deltaTime);
            }
        }

        void SwayUpdate()
        {
            r += swaySpeed * Time.deltaTime;
            Vector3 pos = ctrlBone.localPosition;
            pos.x = boneStartupPos.x + Mathf.Sin(r) * swayRange;
            ctrlBone.localPosition = pos;
        }

        public void SetMoveTarget(Vector3 targetPos)
        {
            currState = State.Chase;
            this.targetPos = targetPos;
        }
    }
}