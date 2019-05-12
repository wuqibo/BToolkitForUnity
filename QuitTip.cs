using UnityEngine;

namespace BToolkit
{
    public class QuitTip : MonoBehaviour
    {

        static bool isShowing;
        const string prefabPath = "Prefabs/UI/QuitTip";

        /// <summary>
        /// 该方法放在 Input.GetKeyDown(KeyCode.Escape) 里监听
        /// </summary>
        public static void Show()
        {
            if (!isShowing)
            {
                GameObject prefab = Resources.Load<GameObject>(prefabPath);
                if (!prefab)
                {
                    Debug.LogError("目录 " + prefabPath + " 下找不到预设体");
                    return;
                }
                GameObject go = Instantiate(prefab) as GameObject;
                QuitTip quitTip = go.GetComponent<QuitTip>();
                go.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
                if (!quitTip)
                {
                    go.AddComponent<QuitTip>();
                }
                isShowing = true;
            }
        }

        void Start()
        {
            Destroy(gameObject, 2);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        void OnDestroy()
        {
            isShowing = false;
        }

    }
}