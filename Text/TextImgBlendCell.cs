using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace BToolkit
{
    public class TextImgBlendCell : MonoBehaviour
    {

        private enum Type
        {
            None,
            Text,
            Photo
        }

        Text text;
        Image img;
        RectTransform rectTrans;
        PhotoLoader photoLoader;
        Type currType;
        TextImgBlend textImgBlend;
        float widthRatio = 0.9f;

        void OnDestroy()
        {
            if (photoLoader != null)
            {
                photoLoader.Destroy();
            }
        }

        public void Update()
        {
            if (currType == Type.Text)
            {
                Vector2 size = rectTrans.sizeDelta;
                size.y = text.preferredHeight;
                rectTrans.sizeDelta = size;
            }
        }

        public void SetContent(TextImgBlend textImgBlend, string content)
        {
            this.textImgBlend = textImgBlend;
            if (!rectTrans)
            {
                rectTrans = transform as RectTransform;
            }
            rectTrans.sizeDelta = new Vector2(BUtils.ScreenUISize.x * widthRatio, 0);
            if (content.Contains(".jpg")
               || content.Contains(".png")
               || content.Contains(".gif")
               || content.Contains(".jpeg"))
            {
                if (!img)
                {
                    img = gameObject.AddComponent<Image>();
                }
                if (photoLoader == null)
                {
                    photoLoader = new PhotoLoader();
                }
                content = content.Replace("../../", "");
                string photoPath = "";// AccountManager.ServerURL + content;
                photoLoader.LoadAndSetPhoto(img, photoPath, 800, 0, true, null, OnFinishCallback);
                currType = Type.Photo;
            }
            else
            {
                if (!string.IsNullOrEmpty(content))
                {
                    if (!text)
                    {
                        text = gameObject.AddComponent<Text>();
                        text.verticalOverflow = VerticalWrapMode.Overflow;
                        text.color = new Color(0.3f, 0.3f, 0.3f);
                    }

                    text.text = NoHTML(content);
                    text.enabled = true;
                    text.fontSize = 45;
                    text.lineSpacing = 1.2f;
                    text.font = textImgBlend.font;
                    currType = Type.Text;
                }
            }
        }

        private void OnFinishCallback(bool success, int width, int height)
        {
            if (success)
            {
                img.enabled = true;
                Vector2 size = rectTrans.sizeDelta;
                size.y = size.x * height / (float)width;
                rectTrans.sizeDelta = size;
                textImgBlend.RefreshContent();
            }
        }

        string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");

            return Htmlstring;
        }
    }
}