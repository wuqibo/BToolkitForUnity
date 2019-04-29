using LitJson;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace BToolkit
{
    public class VersionChecker : MonoBehaviour
    {

        public Text title, content;
        public Button btnCancel, btnConfirm;
        string discription, apkUrl;
        static Action<bool> HaveNewVersionEvent;
        static Action DontUpgradeEvent;
        static bool currCanTipWhenHaveNew, currCanTipWhenNoNew;
        const string PrefabPath = "Prefabs/UI/VersionChecker";

        public static void CheckVersion(bool canTipWhenHaveNew, bool canTipWhenNoNew, Action<bool> OnHaveNewVersionCallback = null, Action OnDontUpgradeCallback = null)
        {
            currCanTipWhenHaveNew = canTipWhenHaveNew;
            currCanTipWhenNoNew = canTipWhenNoNew;
            HaveNewVersionEvent = OnHaveNewVersionCallback;
            DontUpgradeEvent = OnDontUpgradeCallback;
            string fileName = "androidversion.json";
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                fileName = "iosversion.json";
            }
            string url = null;// AccountManager.ServerURL + "/appdownload/" + fileName;
            BUtils.Instance.StartCoroutine(DownloadJsonAndCheck(url));
        }

        static IEnumerator DownloadJsonAndCheck(string jsonUrl)
        {
            Debug.Log("DownloadJsonAndCheck:" + jsonUrl);
            UnityWebRequest request = UnityWebRequest.Get(jsonUrl + "?random=" + DateTime.Now.ToString());
            yield return request.Send();
            if (request.isError)
            {
                Debug.LogError(">>>>>>DownloadJsonAndCheck Error:" + request.error);
            }
            else
            {
                Debug.Log("JsonStr:" + request.downloadHandler.text);
                JsonData json = JsonMapper.ToObject(request.downloadHandler.text.Trim());
                string versionName = (string)json["versionName"];
                string discription = (string)json["discription"];
                string url = (string)json["url"];
                if (!GetAppInfo.GetAppPackageName().Equals(versionName))
                {
                    if (HaveNewVersionEvent != null)
                    {
                        HaveNewVersionEvent(true);
                    }
                    if (currCanTipWhenHaveNew)
                    {
                        ShowDialog(true, discription, url);
                    }
                    yield break;
                }
                else
                {
                    if (HaveNewVersionEvent != null)
                    {
                        HaveNewVersionEvent(false);
                    }
                    if (currCanTipWhenNoNew)
                    {
                        ShowDialog(false, discription, url);
                    }
                    yield break;
                }
            }
            if (DontUpgradeEvent != null)
            {
                DontUpgradeEvent();
            }
        }

        static void ShowDialog(bool haveNewVersion, string discription, string apkUrl)
        {
            VersionChecker prefab = Resources.Load<VersionChecker>(PrefabPath);
            if (!prefab)
            {
                Debug.LogError(PrefabPath + "不存在");
                return;
            }
            VersionChecker versionChecker = Instantiate(prefab);
            versionChecker.transform.SetParent(BUtils.GetTopCanvas(), false);
            if (!haveNewVersion)
            {
                versionChecker.title.text = "当前已是最新版本";
                Vector2 pos = (versionChecker.btnCancel.transform as RectTransform).anchoredPosition;
                pos.x = 0;
                (versionChecker.btnCancel.transform as RectTransform).anchoredPosition = pos;
                versionChecker.btnCancel.GetComponentInChildren<Text>().text = "知道了";
            }
            else
            {
                versionChecker.title.text = "有新版本可以更新";
                versionChecker.title.color = Color.red;
            }
            versionChecker.content.text = discription;
            versionChecker.apkUrl = apkUrl;
            versionChecker.btnConfirm.gameObject.SetActive(haveNewVersion);
        }

        void Start()
        {
            btnCancel.onClick.AddListener(() =>
            {
                Destroy(gameObject);
                if (DontUpgradeEvent != null)
                {
                    DontUpgradeEvent();
                }
            });
            btnConfirm.onClick.AddListener(() =>
            {
                Application.OpenURL(apkUrl);
            });
        }

    }
}