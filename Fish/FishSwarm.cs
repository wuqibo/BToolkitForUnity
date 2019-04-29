using UnityEngine;
using System.Collections;

namespace BToolkit
{
    public class FishSwarm : MonoBehaviour
    {

        public GameObject leaderPrefab, followsPrefab;
        public int followsCount = 10;
        public float moveSpeed = 10;
        public float rotateSpeed = 2f;
        public Vector3 limitMax = new Vector3(5, 5, 5f);
        public Vector3 limitMin = new Vector3(-5, -5, -5f);
        Fish leader;
        Fish[] follows;

        void Awake()
        {
            StartSpawn();
        }

        public void StartSpawn()
        {
            if (leaderPrefab)
            {
                leader = new Fish(this, true);
                leader.trans = Instantiate(leaderPrefab).transform;
                leader.trans.SetParent(transform, false);
                leader.trans.localPosition = new Vector3(Random.Range(limitMin.x, limitMax.x), Random.Range(limitMin.y, limitMax.y), Random.Range(limitMin.z, limitMax.z));
            }
            if (followsPrefab && followsCount > 0)
            {
                follows = new Fish[followsCount];
                for (int i = 0; i < followsCount; i++)
                {
                    follows[i] = new Fish(this, false);
                    follows[i].trans = Instantiate(followsPrefab).transform;
                    follows[i].trans.SetParent(transform, false);
                    follows[i].trans.localPosition = new Vector3(Random.Range(limitMin.x, limitMax.x), Random.Range(limitMin.y, limitMax.y), Random.Range(limitMin.z, limitMax.z));
                }
            }
        }

        void Update()
        {
            leader.Update();
            if (follows != null)
            {
                for (int i = 0; i < followsCount; i++)
                {
                    follows[i].Update();
                }
            }
        }

        public class Fish
        {
            public Transform trans;
            FishSwarm fishSwarm;
            float timer;
            bool isLeader;
            Vector3 targetPos;
            public Fish(FishSwarm fishSwarm, bool isLeader)
            {
                this.fishSwarm = fishSwarm;
                this.isLeader = isLeader;
            }
            public void Update()
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = Random.Range(1f, 3f);
                    if (isLeader)
                    {
                        targetPos = new Vector3(Random.Range(fishSwarm.limitMin.x, fishSwarm.limitMax.x), Random.Range(fishSwarm.limitMin.y, fishSwarm.limitMax.y), Random.Range(fishSwarm.limitMin.z, fishSwarm.limitMax.z));
                    }
                    else
                    {
                        targetPos = fishSwarm.leader.targetPos;
                    }
                }
                Wander(targetPos);
            }
            void Wander(Vector3 targetPos)
            {
                Vector3 targetDir = targetPos - trans.localPosition;
                Vector3 newDir = Vector3.RotateTowards(trans.forward, targetDir, fishSwarm.rotateSpeed * Time.deltaTime, 0f);
                trans.rotation = Quaternion.LookRotation(newDir);
                trans.Translate(Vector3.forward * fishSwarm.moveSpeed * 0.5f * Time.deltaTime);
            }
        }
    }
}