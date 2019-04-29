using System;
using UnityEngine;

namespace BToolkit
{
    public class LanguageManager
    {

        private LanguageManager() { }
        private static int currLoadIndex = -1;
        private static LanguageData currLanguageData;
        private const string ResourcesPath = "Configs/Language";
        private static string[] dataNames = { "ChineseSimple", "English" };
        public static Action OnLanguageChangeEvent;
#if UNITY_EDITOR
        public static int DebugLanguageIndex = 1;
#endif

        /// <summary>
        /// 语言选择
        /// </summary>
        public static int CurrLanguageIndex
        {
            get
            {
#if UNITY_EDITOR
                return DebugLanguageIndex;
#else
            if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional) {
                return 0;
            } else {
                return 1;
            }
            //return PlayerPrefs.GetInt("Language", 0);
#endif
            }
            set
            {
                UnityEngine.PlayerPrefs.SetInt("Language", value);
                if (OnLanguageChangeEvent != null)
                {
                    OnLanguageChangeEvent();
                }
            }
        }

        /// <summary>
        /// 获取LanguageData
        /// </summary>
        public static LanguageData CurrLanguageData
        {
            get
            {
                if (currLoadIndex != CurrLanguageIndex)
                {
                    currLoadIndex = CurrLanguageIndex;
                    currLanguageData = Resources.Load<LanguageData>(ResourcesPath + "/" + dataNames[currLoadIndex]);
                }
                return currLanguageData;
            }
        }

    }
}