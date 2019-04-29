using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class LunPan_Cell : MonoBehaviour
    {
        public Text textPoint;
        public Image icon;
        
        public RectTransform rectTransform { get { return transform as RectTransform; } }

        public void SetContent(LunPan lunpan, int cellValue)
        {
            textPoint.text = "123";// Client.Common.Utils.MoneyStr(cellValue, 0);
            if (cellValue < 10000)
            {
                icon.sprite = lunpan.goldIconLevels[0];
            }
            else if (cellValue < 50000)
            {
                icon.sprite = lunpan.goldIconLevels[1];
            }
            else if (cellValue < 300000)
            {
                icon.sprite = lunpan.goldIconLevels[2];
            }
            else
            {
                icon.sprite = lunpan.goldIconLevels[3];
            }
            icon.SetNativeSize();
        }

    }
}