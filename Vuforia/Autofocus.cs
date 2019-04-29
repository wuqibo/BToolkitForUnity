#define Vuforia6_2

using UnityEngine;
using System.Collections;
using Vuforia;

namespace BToolkit
{
    public class Autofocus : MonoBehaviour
    {

        public bool autofocus;
        public float interval = 5;
        float timer;

        void Start()
        {
#if Vuforia6_2
            VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
            VuforiaARController.Instance.RegisterOnPauseCallback(OnPaused);
#else
        VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaBehaviour.Instance.RegisterOnPauseCallback(OnPaused);
#endif
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
                {
                    StartCoroutine(TriggerAutofocus());
                }
            }
            if (autofocus)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = interval;
                    StartCoroutine(TriggerAutofocus());
                }
            }
        }

        private void OnVuforiaStarted()
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }

        private void OnPaused(bool paused)
        {
            if (!paused)
            {
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
            }
        }

        private IEnumerator TriggerAutofocus()
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
            yield return new WaitForSeconds(2);
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }
}