using UnityEngine;

namespace BToolkit
{
    public class UIController_Video : MonoBehaviour
    {
        public static UIController_Video instance;
        public BButton btnClose;
        public BButton btnDirection;
        public GameObject loading;
        OffCardController_Video offCardController;
        GameObject panelDefault;

        public static void Show(OffCardController_Video offCardController)
        {
            if (!instance)
            {
                instance = Instantiate(Resources.Load<UIController_Video>("UIController_Video"));
            }
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            instance.offCardController = offCardController;
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
                    if (instance.offCardController)
                    {
                        instance.offCardController.ToTracking();
                        instance.offCardController.gameObject.SetActive(false);
                    }
                });
            }
            if (btnDirection)
            {
                btnDirection.onTrigger.AddListener(() =>
                {
                    if (instance.offCardController)
                    {
                        instance.offCardController.SwitchDirection();
                        ChangeBtnsPos();
                    }
                });
            }
        }

        void Update()
        {
            if (loading.activeInHierarchy)
            {
                loading.transform.Rotate(0, 0, -300 * Time.deltaTime);
            }
        }

        void OnLoadingActiveChange(bool b)
        {
            loading.SetActive(b);
        }

        void ChangeBtnsPos()
        {
            if (instance.offCardController)
            {
                if (instance.offCardController.transform.localEulerAngles.z == 0)
                {
                    //按钮
                    if (btnClose)
                    {
                        RectTransform btnCloseTrans = btnClose.rectTransform;
                        btnCloseTrans.anchorMin = new Vector2(0, 1);
                        btnCloseTrans.anchorMax = new Vector2(0, 1);
                        btnCloseTrans.anchoredPosition = new Vector2(75, -70);
                        btnCloseTrans.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    if (btnDirection)
                    {
                        RectTransform btnDirectionTrans = btnDirection.rectTransform;
                        btnDirectionTrans.anchorMin = new Vector2(1, 1);
                        btnDirectionTrans.anchorMax = new Vector2(1, 1);
                        btnDirectionTrans.anchoredPosition = new Vector2(-85, -70);
                        btnDirectionTrans.localEulerAngles = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    //按钮
                    if (btnClose)
                    {
                        RectTransform btnCloseTrans = btnClose.rectTransform;
                        btnCloseTrans.anchorMin = new Vector2(1, 1);
                        btnCloseTrans.anchorMax = new Vector2(1, 1);
                        btnCloseTrans.anchoredPosition = new Vector2(-75, -80);
                        btnCloseTrans.localEulerAngles = new Vector3(0, 0, -90);
                    }
                    if (btnDirection)
                    {
                        RectTransform btnDirectionTrans = btnDirection.rectTransform;
                        btnDirectionTrans.anchorMin = new Vector2(1, 0);
                        btnDirectionTrans.anchorMax = new Vector2(1, 0);
                        btnDirectionTrans.anchoredPosition = new Vector2(-75, 80);
                        btnDirectionTrans.localEulerAngles = new Vector3(0, 0, -90);
                    }
                }
            }
        }

    }
}