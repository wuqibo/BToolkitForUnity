using UnityEngine;
using UnityEngine.UI;

//打字机效果
namespace BToolkit
{
    public class TextWriter : MonoBehaviour
    {

        public bool writeOnStart;
        Text text;
        string fullStr = "";
        int currIndex, textLeng;
        System.Action OnWriteFinish;

        void Start()
        {
            if (writeOnStart)
            {
                if (!text)
                {
                    text = GetComponent<Text>();
                }
                SetTextWriting(text.text);
            }
        }

        public void SetTextWriting(string textStr)
        {
            SetTextWriting(textStr, null);
        }
        public void SetTextWriting(string textStr, System.Action OnWriteFinishk)
        {
            if (textStr != null && textStr.Length > 0)
            {
                this.OnWriteFinish = OnWriteFinishk;
                if (!text)
                {
                    text = GetComponent<Text>();
                }
                text.text = "";
                this.fullStr = textStr;
                currIndex = 0;
                textLeng = textStr.Length;
                CancelInvoke();
                InvokeRepeating("UpdateTest", 0.2f, 0.1f);
            }
        }

        public void Clear()
        {
            CancelInvoke();
            if (!text)
            {
                text = GetComponent<Text>();
            }
            text.text = "";
        }

        public void SetTextNoWriting(string textStr)
        {
            if (!text)
            {
                text = GetComponent<Text>();
            }
            this.fullStr = textStr;
            Finish();
        }

        void UpdateTest()
        {
            currIndex++;
            text.text = fullStr.Substring(0, currIndex);
            if (currIndex >= textLeng)
            {
                Finish();
            }
        }

        public void Finish()
        {
            CancelInvoke("UpdateTest");
            if (fullStr != null && fullStr.Length > 0)
            {
                text.text = fullStr;
            }
            if (OnWriteFinish != null)
            {
                OnWriteFinish();
                OnWriteFinish = null;
            }
        }
    }
}