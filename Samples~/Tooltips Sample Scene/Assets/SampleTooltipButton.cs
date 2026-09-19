namespace TeaSpoons.UGuiDesignSystem.Samples
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SampleTooltipButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private TMP_Text btnText;
        [SerializeField]
        private SampleTooltipContent tooltipPrefab;
        [SerializeField]
        private RectTransform tooltipRoot;
        [SerializeField]
        private RectTransform allowedTooltipArea;

        private static int buttonCount = 0;

        private void Awake()
        {
            buttonCount++;
            btnText.text = $"Item#{buttonCount}";

            button.onClick.AddListener(OpenTooltip);
        }

        private void OpenTooltip()
        {
            var tooltip = Instantiate(tooltipPrefab, tooltipRoot);
            tooltip.SetupTooltip(btnText.text, new TooltipSetup(this.transform as RectTransform, allowedTooltipArea: allowedTooltipArea));
        }
    }
}
