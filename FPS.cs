using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class FPS : MonoBehaviour
    {

        public static FPS instance;
        public Text text;
        int frame;

        void Awake()
        {
            instance = this;
            gameObject.SetActive(false);
        }

        void Start()
        {
            InvokeRepeating("RefreshFPS", 1, 1);
        }

        void Update()
        {
            frame++;
        }

        void RefreshFPS()
        {
            text.text = "FPS:" + frame;
            frame = 0;
        }

        public void Show(bool b)
        {
            if (gameObject.activeInHierarchy != b)
            {
                gameObject.SetActive(b);
            }
        }
    }
}