namespace TeaSpoons.UGuiDesignSystem.Samples
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SampleTooltipNestedButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private SampleTooltipContent tooltipPrefab;
        [SerializeField]
        private RectTransform tooltipRoot;
        [SerializeField]
        private RectTransform allowedTooltipArea;

        private void Awake()
        {
            button.onClick.AddListener(OpenTooltip);
        }

        private void OpenTooltip()
        {
            var tooltip = Instantiate(tooltipPrefab, tooltipRoot);
            tooltip.SetupTooltip("You're awesome!", new TooltipSetup(this.transform as RectTransform, allowedTooltipArea: allowedTooltipArea));
        }
    }
}
