using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class SceneLoading : MonoBehaviour
    {

        public Slider slider;
        public Text text;
        public bool fromFirstScene = true;
        public bool fakeLoading;
        [Header("Language")]
        public Text tip;
        static string targetLevelName;

        float fakeProgress;

        void Start()
        {
            if (tip)
            {
                tip.text = LanguageManager.CurrLanguageData.loadingTip;
            }
            if (string.IsNullOrEmpty(targetLevelName))
            {
                if (fromFirstScene)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
            }
            else
            {
                StartCoroutine(LoadSceneAsynchronously());
            }
        }

        IEnumerator LoadSceneAsynchronously()
        {
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetLevelName);
            operation.allowSceneActivation = false;

            if (fakeLoading)
            {
                fakeProgress = 0;
                while (true)
                {
                    fakeProgress = Mathf.Clamp01(fakeProgress + Random.Range(0.1f, 0.3f) * Time.deltaTime);
                    if (fakeProgress >= 0.9f)
                    {
                        if (operation.progress >= 0.9f)
                        {
                            // Switch to new loaded scene.
                            operation.allowSceneActivation = true;
                            UpdateUI(1);
                            yield return null;
                            break;
                        }
                    }
                    UpdateUI(fakeProgress);

                    yield return null;
                }
            }
            else
            {
                while (!operation.isDone)
                {
                    // The scene is fully loaded when progress == 0.9f
                    if (operation.progress >= 0.9f)
                    {
                        // Fully loaded. Switch to new loaded scene.
                        UpdateUI(1);
                        operation.allowSceneActivation = true;
                    }
                    else
                    {
                        // Map to correct progress.
                        var progress = Mathf.Clamp01(operation.progress / 0.9f);
                        UpdateUI(progress);
                    }

                    yield return null;
                }
            }
        }

        void UpdateUI(float progress)
        {
            if (slider)
            {
                slider.value = progress;
            }
            if (text)
            {
                text.text = (int)(progress * 100) + "%";
            }
        }

        public static void LoadScene(string levelName)
        {
            targetLevelName = levelName;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
        }

    }
}