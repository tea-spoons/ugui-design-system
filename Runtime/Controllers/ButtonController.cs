
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class ButtonController : UIController
    {
        private Button cachedButton;
        internal Button button
        {
            get
            {
                if (cachedButton == null)
                {
                    cachedButton = GetComponent<Button>();
                }
                return cachedButton;
            }
        }

        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        public void OverrideOnClickListener(UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        public void AddOnClickListener(UnityAction callback)
        {
            button.onClick.AddListener(callback);
        }

        public void RemoveOnClickListener(UnityAction callback)
        {
            button.onClick.RemoveListener(callback);
        }
    }
}
