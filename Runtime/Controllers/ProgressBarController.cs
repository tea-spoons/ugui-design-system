
namespace TeaSpoons.UGuiDesignSystem
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class ProgressBarController : UIController
    {
        [Serializable]
        internal struct VisualLimits
        {
            [Min(0)]
            public float Left;
            [Min(0)]
            public float Right;
        }

        [Serializable]
        internal struct FillElements
        {
            public Image FillMask;
            public RectTransform RightSideCap;

            public readonly bool IsAvailable => FillMask;

            public readonly void SetActive(bool active)
            {
                if (!IsAvailable) return;

                FillMask.gameObject.SetActive(active);

                if (RightSideCap)
                {
                    RightSideCap.gameObject.SetActive(active);
                }
            }
        }

        [field: SerializeField]
        public int Value { get; private set; }
        public float Percentage { get; private set; }

        [Tooltip("The value at which the bar is full. A higher value will cause overfill if the Overfill Bar is set up.")]
        [Min(1)]
        [SerializeField, FormerlySerializedAs("MaxValue")]
        private int maxValue = 100;
        public int MaxValue
        {
            get => maxValue;
            set => maxValue = Mathf.Max(1, value);
        }

        [SerializeField]
        private FillElements primaryBar;
        [SerializeField]
        private FillElements overfillBar;

        [Tooltip("The distance from either side between the min and max fill positions and the actual object width.")]
        [SerializeField]
        private VisualLimits visualLimits;

        [Obsolete("OnValueChanged is obsolete, use ValueChanged instead.")]
        public event Action<int> OnValueChanged
        {
            add => ValueChanged += value;
            remove => ValueChanged -= value;
        }
        public event Action<int> ValueChanged = delegate { };

        private Func<float, int> round = Mathf.CeilToInt;

        private void Start()
        {
            // Bugfix: SIS-2164 - Green line appears in the middle of progress bar
            // Bar rendering accesses FillMask's RectTransform. But RT's values will only stabilize after the first Update.
            // see: https://discussions.unity.com/t/when-its-safe-to-get-recttransform-position/612865/3
            // expensive alternative: Call Canvas.ForceUpdateCanvases();
            StartCoroutine(HandleRectTransformsHaveStabilized());
        }

        private IEnumerator HandleRectTransformsHaveStabilized()
        {
            // We apply the currently set value again, now that the bar will render fine
            yield return new WaitForEndOfFrame();

            SetValue(Value);
        }

        /// <summary>
        /// Sets the fill amount of the progress bar in units in relation to the <see cref="MaxValue"/>.
        /// </summary>
        /// <remarks>
        /// A <paramref name="value"/> higher than <see cref="MaxValue"/> will cause the overfill bar to appear.
        /// </remarks>
        public void SetValue(int value)
        {
            Value = Mathf.Max(0, value);

            Percentage = Value / (float)MaxValue;
            SetVisualAmount(Percentage);

            ValueChanged(Value);
        }

        /// <summary>
        /// Sets the fill amount to the <paramref name="value"/> in the range 0..1.
        /// </summary>
        /// <remarks>
        /// A <paramref name="value"/> higher than 1 will cause the overfill bar to appear.
        /// </remarks>
        public void SetValue(float value)
        {
            Percentage = Mathf.Max(0f, value);

            SetVisualAmount(Percentage);

            Value = round(Percentage * MaxValue);
            ValueChanged(Value);
        }

        public void SetRoundingFunction(Func<float, int> round)
        {
            this.round = round;
        }

        private void SetVisualAmount(float value)
        {
            if (value <= 1f ||
                Mathf.Approximately(value, 1f))
            {
                SetVisualAmount(primaryBar, value);
                overfillBar.SetActive(false);
            }
            else
            {
                SetVisualAmount(primaryBar, 1f);
                overfillBar.SetActive(true);
                SetVisualAmount(overfillBar, Repeat01(value));
            }
        }

        private void SetVisualAmount(FillElements bar, float value)
        {
            bar.SetActive(value != 0);

            if (!bar.IsAvailable) return;
            value = Mathf.Clamp01(value);

            value = ApplyVisualLimits(bar, value);

            bar.FillMask.fillAmount = value;

            if (!bar.RightSideCap)
            {
                return;
            }

            var fillRect = bar.FillMask.rectTransform.rect;
            var middleHeight = fillRect.yMin + (fillRect.height * 0.5f);
            var left = new Vector2(fillRect.xMin, middleHeight);
            var right = new Vector2(fillRect.xMax, middleHeight);
            var capProgress = bar.FillMask.fillOrigin == 0 ? value : (1 - value); // left: 0, right: 1
            bar.RightSideCap.anchoredPosition = Vector2.Lerp(left, right, capProgress);
        }

        private float ApplyVisualLimits(FillElements bar, float value)
        {
            var width = bar.FillMask.rectTransform.rect.width;
            if (Mathf.Approximately(width, 0))
            {
                return value; // Nothing to clamp against yet
            }
            var min = visualLimits.Left / width;
            var max = 1 - (visualLimits.Right / width);
            value = Mathf.Lerp(min, max, value);
            return value;
        }

        private void OnDrawGizmosSelected()
        {
            if (!primaryBar.IsAvailable) return;

            var canvas = GetComponentInParent<Canvas>();

            if (!canvas)
            {
                return;
            }

            var rectTransform = (RectTransform)transform;

            Gizmos.matrix = Matrix4x4.TRS(rectTransform.position,
                rectTransform.rotation,
                Vector3.Scale(canvas.transform.localToWorldMatrix.lossyScale, rectTransform.localScale));
            Gizmos.color = Color.cyan;

            var width = rectTransform.rect.width;

            if (visualLimits.Left > 0)
            {
                var left = (width - visualLimits.Left) * 0.5f;
                Gizmos.DrawWireCube(Vector3.left * left,
                    new Vector2(visualLimits.Left, rectTransform.rect.height));
            }

            if (visualLimits.Right > 0)
            {
                var right = (width - visualLimits.Right) * 0.5f;
                Gizmos.DrawWireCube(Vector3.right * right,
                    new Vector2(visualLimits.Right, rectTransform.rect.height));
            }
        }

        /// <summary>
        /// Like <see cref="Mathf.Repeat(float, float)"/>, but returns more intuitive values for progress bars when close to a whole number.
        /// </summary>
        private static float Repeat01(float value)
        {
            if (value <= 2f)
            {
                return value - 1f;
            }

            var fractionalDigits = value - (int)value;
            if (Mathf.Approximately(fractionalDigits, 0f) ||
                Mathf.Approximately(fractionalDigits, 1f))
            {
                return 1f;
            }

            return Mathf.Repeat(value, 1f);
        }

        // for easier setup/debugging in the inspector, any inspector change would reapply the value and update the layout
        private void OnValidate()
        {
            SetValue(Value);
        }
    }
}
