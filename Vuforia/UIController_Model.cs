using UnityEngine;

namespace BToolkit
{
    public class UIController_Model : MonoBehaviour
    {
        public static UIController_Model instance;
        public BButton btnClose;
        public GameObject loading;
        public RectTransform finger;
        public BButton btnTakePhoto;
        OffCardController_Model offCardController;
        GameObject panelDefault;
        bool isOffCard;

        /// <summary>
        /// 观看记录时显示
        /// </summary>
        public static void Show(OffCardController_Model offCardController, bool isOffCard)
        {
            if (!instance)
            {
                instance = Instantiate(Resources.Load<UIController_Model>("UIController_Model"));
            }
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            instance.offCardController = offCardController;
            instance.isOffCard = isOffCard;
            instance.loading.SetActive(VuforiaHelper.idLoadingActive);
        }

        public static void Destroy()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
            }
        }

        void OnDestroy()
        {
            VuforiaHelper.LoadingActiveAction -= OnLoadingActiveChange;
        }

        void OnDisable()
        {
            if (panelDefault)
            {
                panelDefault.SetActive(true);
            }
        }

        void Awake()
        {
            VuforiaHelper.LoadingActiveAction += OnLoadingActiveChange;
            panelDefault = GameObject.Find("PanelDefault");
            if (panelDefault)
            {
                panelDefault.SetActive(false);
            }
            if (btnClose)
            {
                btnClose.onTrigger.AddListener(() =>
                {
                    Destroy(gameObject);
                    if (isOffCard)
                    {
                        offCardController.ToTracking();
                        offCardController.gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(offCardController.gameObject);
                    }
                });
            }
            if (btnTakePhoto)
            {
                btnTakePhoto.onTrigger.AddListener(TakePhoto);
                btnTakePhoto.gameObject.SetActive(WeiXin.IsAppInstalled());
            }
        }

        void Start()
        {
            FingerGuid();
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
            if (loading.activeInHierarchy)
            {
                loading.transform.Rotate(0, 0, -300 * Time.deltaTime);
            }
        }

        void OnLoadingActiveChange(bool b)
        {
            loading.SetActive(b);
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