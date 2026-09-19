namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;

    public struct TooltipSetup
    {
        public RectTransform TargetRectTransform;
        public float AdditionalOffsetToTarget;
        public RectTransform AllowedTooltipArea;
        public float AreaEdgePadding;
        public Camera UiCamera;

        /// <summary>
        /// Container with tooltips settings
        /// </summary>
        /// <param name="targetRectTransform">The icon/button rect which triggered our tooltip and that we want to point towards</param>
        /// <param name="additionalOffsetToTarget">In case we want to modify the distance between tooltip and the target (on top of the auto-calculated one, based on the target's rect) </param>
        /// <param name="allowedTooltipArea">Tooltips will try to not exceed these given bounds. If null, the tooltips will use the entire root canvas.</param>
        /// <param name="areaEdgePadding">If you want the tooltip to keep some distance from the area edges</param>
        /// <param name="uiCamera">Optional. Only needed when not using Screen Space - Overlay Mode</param>
        public TooltipSetup(RectTransform targetRectTransform, float additionalOffsetToTarget = 0f, RectTransform allowedTooltipArea = null, float areaEdgePadding = 0, Camera uiCamera = null)
        {
            TargetRectTransform = targetRectTransform;
            AdditionalOffsetToTarget = additionalOffsetToTarget;
            AllowedTooltipArea = allowedTooltipArea;
            AreaEdgePadding = areaEdgePadding;
            UiCamera = uiCamera;
        }
    }
}
