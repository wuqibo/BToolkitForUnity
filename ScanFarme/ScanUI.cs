using BToolkit;
using UnityEngine;

public class ScanUI : MonoBehaviour
{

    public RectTransform scanLine;
    public float maxY = 450;
    public float minY = -450;
    public float speed = 200f;
    public AudioClip sound;
    AudioSource audioSource;
    bool moveDown;
    GameObject previousTarget;

    void Awake()
    {
        if (sound)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
            }
        }
    }

    void Start()
    {
        Init();
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

    public void In()
    {
        gameObject.SetActive(true);
    }

    public void Out()
    {
        gameObject.SetActive(false);
    }

    internal void Init()
    {
        moveDown = true;
        if (scanLine)
        {
            scanLine.gameObject.SetActive(true);
            scanLine.localScale = new Vector3(1, 1, 1);
        }
    }

    internal void OnScaned(GameObject target)
    {
        if (scanLine)
        {
            if (scanLine.gameObject.activeInHierarchy)
            {
                scanLine.gameObject.SetActive(false);
                if (previousTarget != target)
                {
                    previousTarget = target;
                    if (sound)
                    {
                        audioSource.clip = sound;
                        audioSource.Play();
                    }
                }
            }
        }
    }
}
