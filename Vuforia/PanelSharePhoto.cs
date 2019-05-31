using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class PanelSharePhoto : MonoBehaviour
    {
        public BButton btnClose;
        public RawImage photo;
        public BButton btnFriend, btnMoment;
        public Texture2D waterMarking;
        public AudioClip sound;

        public static void Show(Texture2D texture)
        {
            PanelSharePhoto panelSharePhoto = Instantiate(Resources.Load<PanelSharePhoto>("PanelSharePhoto"));
            panelSharePhoto.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            panelSharePhoto.SetContent(texture);
        }

        void Awake()
        {
            btnClose.onTrigger.AddListener(() =>
            {
                Destroy(gameObject);
            });
            btnFriend.onTrigger.AddListener(ShareToFriend);
            btnMoment.onTrigger.AddListener(ShareToMoment);
            Tween.Scale(photo.transform.parent, Vector3.one);
            Tween.Move(photo.transform.parent, Vector3.zero, false);
            Tween.Rotate(photo.transform.parent, Vector3.zero, false);
        }

        void SetContent(Texture2D texture)
        {
            //添加水印
            if (waterMarking)
            {
                texture = TextureUtils.AddWaterMarking(texture, waterMarking);
            }
            photo.enabled = false;
            SavePhotoToAlbum.Save(texture.EncodeToJPG(), TimeHelper.GetCurrTimeAsName() + ".jpg");
            photo.enabled = true;
            photo.texture = texture;
            StartAnim();
        }

        void StartAnim()
        {
            SoundPlayer.PlayAndDestroy(0, sound);
            float time = 0.5f;
            Tween.Scale(0, photo.transform.parent, Vector3.one * 0.8f, time, Tween.EaseType.ExpoEaseOut);
            Tween.Move(0, photo.transform.parent, new Vector2(0, 38), time, false, Tween.EaseType.ExpoEaseOut);
            Tween.Rotate(0.1f, photo.transform.parent, new Vector3(0, 0, Random.Range(0, 2) == 0 ? 3 : -3), time * 2, false, Tween.EaseType.ExpoEaseOut);
        }

        void ShareToFriend()
        {
            string tempSavePath = SaveForShare();
            WeiXinShare.Instance.ShareImageToFriend("鸿雁湿地景区", "满洲里跨国湿地景区欢迎您", tempSavePath);
        }

        void ShareToMoment()
        {
            string tempSavePath = SaveForShare();
            WeiXinShare.Instance.ShareImageToMoments("鸿雁湿地景区", "满洲里跨国湿地景区欢迎您", tempSavePath);
        }

        string SaveForShare()
        {
            string tempSavePath = Application.persistentDataPath + "/share.jpg";
            Storage.SaveBytesToFile((photo.texture as Texture2D).EncodeToJPG(), tempSavePath);
            return tempSavePath;
        }
    }
}