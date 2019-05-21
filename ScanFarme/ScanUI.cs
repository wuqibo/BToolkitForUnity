using BToolkit;
using UnityEngine;

public class ScanUI : MonoBehaviour
{
    public static ScanUI instance;
    public RectTransform scanLine;
    public float minY, maxY;
    public float speed = 200f;
    public AudioClip recognizedSound;
    bool moveDown;
    GameObject previousTarget;
    float screenUI;

    void Awake()
    {
        instance = this;
        screenUI = 1080 * Screen.height / (float)Screen.width;
        if (minY == 0)
        {
            minY = -screenUI * 0.5f;
        }
        if (maxY == 0)
        {
            maxY = screenUI * 0.5f;
        }
    }

    void Start()
    {
        Show();
    }

    void Update()
    {
        if (scanLine)
        {
            if (scanLine.gameObject.activeInHierarchy)
            {
                if (moveDown)
                {
                    scanLine.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
                    if (scanLine.anchoredPosition.y < minY)
                    {
                        moveDown = false;
                        Tween.Scale(0, scanLine, new Vector3(1, -1, 1), 2, Tween.EaseType.ExpoEaseOut);
                    }
                }
                else
                {
                    scanLine.anchoredPosition += new Vector2(0, speed * Time.deltaTime);
                    if (scanLine.anchoredPosition.y > maxY)
                    {
                        moveDown = true;
                        Tween.Scale(0, scanLine, new Vector3(1, 1, 1), 2, Tween.EaseType.ExpoEaseOut);
                    }
                }
            }
        }
    }

    //重新开始
    public void Show()
    {
        moveDown = true;
        if (scanLine)
        {
            scanLine.gameObject.SetActive(true);
            scanLine.localScale = new Vector3(1, 1, 1);
        }
    }

    /// <summary>
    /// 识别到的触发
    /// </summary>
    public void OnRecognized(GameObject target)
    {
        if (scanLine && scanLine.gameObject.activeInHierarchy)
        {
            scanLine.gameObject.SetActive(false);
        }
        if (previousTarget != target)
        {
            previousTarget = target;
            SoundPlayer.Play(0, recognizedSound);
        }
    }
}
