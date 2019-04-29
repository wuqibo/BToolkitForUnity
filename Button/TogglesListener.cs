using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/TogglesListener")]
    public class TogglesListener : MonoBehaviour
    {
        class ToggleHolder
        {
            public ToggleHolder(int index, Toggle toggle, Action<int, bool> ValueEvent)
            {
                toggle.onValueChanged.AddListener((bool isOn) =>
                {
                    ValueEvent(index, isOn);
                });
            }
        }
        public Toggle[] toggles;
        [System.Serializable]
        public class ValueEvent : UnityEvent<int> { }
        public ValueEvent onValueChange;
        ToggleHolder[] toggleHolders;

        void Awake()
        {
            toggleHolders = new ToggleHolder[toggles.Length];
            for (int i = 0; i < toggleHolders.Length; i++)
            {
                toggleHolders[i] = new ToggleHolder(i, toggles[i], OnToggleChange);
            }
            SetDefaultTrue(0);
        }

        void OnToggleChange(int index, bool isOn)
        {
            if (isOn)
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (i != index)
                    {
                        toggles[i].isOn = false;
                        toggles[i].enabled = true;
                    }
                    else
                    {
                        toggles[i].enabled = true;
                    }
                }
                if (onValueChange != null)
                {
                    onValueChange.Invoke(index);
                }
            }
        }

        public void SetDefaultTrue(int index)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = (i == index);
                toggles[i].enabled = (i != index);
            }
        }
    }
}