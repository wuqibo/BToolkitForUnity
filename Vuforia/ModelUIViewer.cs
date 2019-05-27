using UnityEngine;

namespace BToolkit
{
    public class ModelUIViewer : MonoBehaviour
    {
        static ModelUIViewer instance;
        public BButton btnClose;
        public RectTransform finger;
        public BButton btnTakePhoto;
        Model model;
        ModelController modelController;
        GameObject panelDefault;

        public static ModelUIViewer Show(Model model, ModelController modelController)
        {
            if (!instance)
            {
                instance = Instantiate(Resources.Load<ModelUIViewer>("Prefabs/UI/UIModelViewer"));
            }
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            instance.model = model;
            instance.modelController = modelController;
            return instance;
        }

        void Awake()
        {
            panelDefault = GameObject.Find("PanelDefault");
            if (panelDefault)
            {
                panelDefault.SetActive(false);
            }
            if (btnClose)
            {
                btnClose.onTrigger.AddListener(() =>
                {
                    if (panelDefault)
                    {
                        panelDefault.SetActive(true);
                    }
                    Destroy(gameObject);
                    if (modelController)
                    {
                        modelController.ToTrackingPos();
                    }
                    if (SceneRecordUIManager.instance)
                    {
                        if (model)
                        {
                            Destroy(model.gameObject);
                        }
                        SceneRecordUIManager.instance.arCamera.gameObject.SetActive(false);
                    }
                });
            }
            if (btnTakePhoto)
            {
                btnTakePhoto.onTrigger.AddListener(TakePhoto);
            }
        }

        void Start()
        {
            FingerGuid();
        }

        void FingerGuid()
        {
            if (finger)
            {
                Tween.Alpha(finger, 0.3f);
                Tween.Move(finger, new Vector2(-100, -700), false);
                Tween.Move(0, finger, new Vector2(150, -700), 2, false, Tween.EaseType.SineEaseInOut, () =>
                {
                    if (finger)
                    {
                        Tween.Alpha(0, finger, 0, 1f, Tween.EaseType.Linear, () =>
                        {
                            FingerGuid();
                        });
                    }
                });
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (finger)
                {
                    Destroy(finger.gameObject);
                }
            }
        }

        void TakePhoto()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            canvas.enabled = false;
            BUtils.ScreenShot((Texture2D texture) =>
            {
                canvas.enabled = true;
                PanelSharePhoto.Show(texture);
            });
        }

    }
}