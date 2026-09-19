
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(Toggle))]
    public class ToggleController : UIController
    {
        private Toggle cachedToggle;

        internal Toggle toggle
        {
            get
            {
                if (cachedToggle == null)
                {
                    cachedToggle = GetComponent<Toggle>();
                }
                return cachedToggle;
            }
        }

        public bool Interactable
        {
            get => toggle.interactable;
            set => toggle.interactable = value;
        }

        public bool IsOn
        {
            get => toggle.isOn;
            set => toggle.SetIsOnWithoutNotify(value);
        }

        public void OverrideOnValueChangedListener(UnityAction<bool> callback)
        {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(callback);
        }

        public void AddOnValueChangedListener(UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
        }

        public void RemoveOnValueChangedListener(UnityAction<bool> callback)
        {
            toggle.onValueChanged.RemoveListener(callback);
        }
    }
}
