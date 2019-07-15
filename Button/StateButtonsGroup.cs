using System;
using UnityEngine;
using UnityEngine.Events;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/StateButtonsGroup")]
    public class StateButtonsGroup : MonoBehaviour
    {
        class ButtonHolder
        {
            public ButtonHolder(int index, StateButton button, Action<int, bool> ValueEvent)
            {
                button.onValueChanged.AddListener((bool isOn) =>
                {
                    ValueEvent(index, isOn);
                });
            }
        }
        public StateButton[] buttons;
        [System.Serializable]
        public class ValueEvent : UnityEvent<int> { }
        public ValueEvent onValueChange;
        ButtonHolder[] toggleHolders;

        void Awake()
        {
            toggleHolders = new ButtonHolder[buttons.Length];
            for (int i = 0; i < toggleHolders.Length; i++)
            {
                toggleHolders[i] = new ButtonHolder(i, buttons[i], OnToggleChange);
            }
            SetDefaultTrue(0);
        }

        void OnToggleChange(int index, bool isOn)
        {
            if (isOn)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (i != index)
                    {
                        buttons[i].IsOn = false;
                        buttons[i].enabled = true;
                    }
                    else
                    {
                        buttons[i].enabled = true;
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
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].IsOn = (i == index);
                buttons[i].enabled = (i != index);
            }
        }
    }
}