using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace BToolkit
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class TextImgBlend : MonoBehaviour
    {

        public Font font;
        internal RectTransform rectTrans;
        VerticalLayoutGroup verticalLayoutGroup;
        int contentItemsCount;
        List<TextImgBlendCell> contentItems;
        string _text;
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                _text = _text.Replace("<p>", "");
                _text = _text.Replace("<br>", "\n");
                _text = _text.Replace("</p>", "\n");
                string[] itemsArr = SplitByImgTag(_text);
                contentItemsCount = itemsArr.Length;
                contentItems = new List<TextImgBlendCell>();
                //正文,循环内容
                for (int i = 0; i < contentItemsCount; i++)
                {
                    string itemStr = itemsArr[i];
                    itemStr = itemStr.Replace("\n", "");
                    if (itemStr.Length > 0)
                    {
                        GameObject go = new GameObject("cell_" + i);
                        TextImgBlendCell cell = go.AddComponent<TextImgBlendCell>();
                        RectTransform rectTrans = go.AddComponent<RectTransform>();
                        rectTrans.SetParent(transform, false);
                        rectTrans.SetSiblingIndex(i + 3);
                        cell.name = "Cell_" + i;
                        cell.SetContent(this, itemStr);
                        contentItems.Add(cell);
                    }
                }
                contentItemsCount = contentItems.Count;//重新读取，因为有空段落不加入数组参与展示
            }
        }

        void Awake()
        {
            rectTrans = transform as RectTransform;
            if (!verticalLayoutGroup)
            {
                verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            }
        }

        void Update()
        {
            if (contentItems != null)
            {
                if (Time.frameCount % 50 == 0)
                {
                    Vector2 size = rectTrans.sizeDelta;
                    size.y = 0;
                    for (int i = 0; i < contentItems.Count; i++)
                    {
                        size.y += (contentItems[i].transform as RectTransform).sizeDelta.y;
                        size.y += verticalLayoutGroup.spacing;
                    }
                    size.y += 60;
                    rectTrans.sizeDelta = size;
                }
            }
        }

        public void RefreshContent()
        {
            if (!verticalLayoutGroup)
            {
                verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            }
            verticalLayoutGroup.enabled = false;
            verticalLayoutGroup.enabled = true;
            verticalLayoutGroup.SetLayoutVertical();
        }

        // 用img标签拆分成数组
        string[] SplitByImgTag(string htmlText)
        {
            return Regex.Split(htmlText, "<img[^>]*src=\"(?<key>.*?)\"[^>]*>", RegexOptions.IgnoreCase);
        }

        // 取得HTML中所有图片的URL。 
        string[] GetImgsPathFromHtml(string htmlText)
        {
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            MatchCollection matches = regImg.Matches(htmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];
            foreach (Match match in matches)
            {
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            }
            return sUrlList;
        }

    }
}