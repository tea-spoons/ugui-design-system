
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using System;

    /// <summary>
    /// A UI controller for a "counter" - an element that has a increment and a decrement button
    /// for manipulating an integer value.
    /// </summary>
    public class CounterController : UIController, ISerializationCallbackReceiver
    {
        [SerializeField, Min(0)]
        private int max;
        public int Max
        {
            get => max;
            set
            {
                max = value;
                SetValue(Value);
            }
        }

        [SerializeField, Min(0)]
        private int value;
        public int Value
        {
            get => value;
            set => SetValue(value);
        }

        [Header("Elements")]
        [SerializeField]
        private ButtonController incrementButton;
        [SerializeField]
        private ButtonController decrementButton;

        public event Action<int> OnValueChanged = delegate { };

        protected virtual void Start()
        {
            incrementButton.AddOnClickListener(Increment);
            decrementButton.AddOnClickListener(Decrement);

            UpdateUiElements();
        }

        public void Increment()
        {
            Value++;
        }

        public void Decrement()
        {
            Value--;
        }

        public virtual void SetValueWithoutNotify(int value)
        {
            this.value = Mathf.Clamp(value, 0, max);

            UpdateUiElements();
        }

        protected virtual bool CanIncrease()
        {
            return value < max;
        }

        protected virtual bool CanDecrease()
        {
            return value > 0;
        }

        private void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, max);

            if (newValue != Value)
            {
                value = newValue;
                OnValueChanged(newValue);
            }

            UpdateUiElements();
        }

        protected void UpdateUiElements()
        {
            incrementButton.Interactable = CanIncrease();
            decrementButton.Interactable = CanDecrease();

            if (Content)
            {
                Content.Label = $"{value} / {max}";
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            value = Mathf.Clamp(value, 0, max);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (didAwake)
                {
                    SetValue(value);
                }
            }
            else
            {
                if (value > max)
                {
                    value = max;
                }
            }
        }
#endif
    }
}
