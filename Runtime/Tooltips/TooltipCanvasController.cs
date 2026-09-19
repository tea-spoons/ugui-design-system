namespace TeaSpoons.UGuiDesignSystem
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// Positions and sets up the tooltip in our UI.
    /// - Makes sure the tooltip stays inside the given allowed area
    /// - Renders above or below the target (wherever is more space)
    /// - Positions the tip to point at the target
    /// - Creates a click blocker, spanning the entire canvas, which closes the tooltip on click
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class TooltipCanvasController : MonoBehaviour
    {
        [Header("Canvas / Raycaster")]
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private GraphicRaycaster raycaster;

        [Header("Tips (Arrows)")]
        [SerializeField]
        private RectTransform tipTop;
        [SerializeField]
        private RectTransform tipBottom;

        [Header("Animation Settings")]
        [SerializeField]
        private bool animateIn = true;
        [SerializeField]
        private float animationDuration = 0.2f;
        [SerializeField]
        private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private RectTransform tooltipBoxRect => transform as RectTransform; // note: the rectTransform should just comprise our tooltip box, without the tips
        private GameObject backgroundBlocker;

        private const int newTooltipCanvasSortingGroup = 32767; // a newly spawned tooltip will be set to this canvas sorting order
        private static readonly List<TooltipCanvasController> allOpenTooltipControllers = new();

        protected virtual void Awake()
        {
            // To support nesting of tooltips (one tooltip opening another tooltip) we have to
            // a) make sure each tooltip has its own canvas and raycaster (most headache-free approach)
            // b) make sure that the last opened tooltip has the highest canvas sorting order
            // Reason: Unfortunately raycasts won't work reliably if two canvases are overriding the sorting order by the *same* index.
            // Unity won't honor the hierarchy then anymore - and it's then pretty much random which canvas's graphic would be hit.
            MakeMeTopCanvas();
            allOpenTooltipControllers.Add(this);
        }

        protected virtual void OnDestroy()
        {
            UndoMakeMeTopCanvas();
            allOpenTooltipControllers.Remove(this);
        }

        public void ApplyTooltipSetup(TooltipSetup tooltipSetup)
        {
            // in case we were instantiated right into a layout group, let's make sure, we ignore the layout
            IgnoreLayout();

            PositionTooltip(tooltipSetup);
            CreateBackgroundBlocker(); // fullscreen background trigger to close the tooltip
            AnimateIn(tooltipSetup);
        }

        private void PositionTooltip(TooltipSetup tooltipSetup)
        {
            // force immediate UI layout recalculation for accurate size detection
            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipBoxRect);
            var uiCam = tooltipSetup.UiCamera;
            var tooltipBoxScreenSize = GetPixelSizeOfRect(tooltipBoxRect, uiCam);

            // determine allowed render area for tooltips, if none defined, let's take the entire root canvas
            var allowedTooltipArea = tooltipSetup.AllowedTooltipArea;
            if (allowedTooltipArea == null)
            {
                allowedTooltipArea = GetComponentInParent<Canvas>().rootCanvas.transform as RectTransform;
            }

            // change pivot to center to be independent of UI artist's setup, before positioning the tooltip
            tooltipBoxRect.pivot = Vector2.one * 0.5f;

            // find target center (target = the UI element we want to point towards with our tooltip)
            var targetRectTransform = tooltipSetup.TargetRectTransform;
            var targetCenterWorld = targetRectTransform.TransformPoint(targetRectTransform.rect.center);
            var targetCenterScreen = RectTransformUtility.WorldToScreenPoint(uiCam, targetCenterWorld);
            var targetRectSizeScreen = GetPixelSizeOfRect(targetRectTransform, uiCam);
            
            // horizontal clamping (Prevents the tooltip from sliding off the screen)
            var areaEdgePadding = tooltipSetup.AreaEdgePadding * canvas.rootCanvas.scaleFactor;
            
            var boundsCenterWorld = allowedTooltipArea.TransformPoint(allowedTooltipArea.rect.center);
            var boundsCenterScreen = RectTransformUtility.WorldToScreenPoint(uiCam, boundsCenterWorld);
            var boundsSizeScreen = GetPixelSizeOfRect(allowedTooltipArea, uiCam);
            
            var tooltipScreenPosX = targetCenterScreen.x;
            var minX = boundsCenterScreen.x - (boundsSizeScreen.x / 2) + (tooltipBoxScreenSize.x / 2) + areaEdgePadding;
            var maxX = boundsCenterScreen.x + (boundsSizeScreen.x / 2) - (tooltipBoxScreenSize.x / 2) - areaEdgePadding;
            
            tooltipScreenPosX = Mathf.Clamp(tooltipScreenPosX, minX, maxX);

            // determine y-position (if target is in upper half of the area, we want to place the tooltip below the target, if it is in the lower half, then above the target)
            var tooltipBoxScreenHeight = tooltipBoxScreenSize.y;
            var targetIsInUpperHalf = targetCenterScreen.y > boundsCenterScreen.y;
            float yPos = 0;
            
            if (targetIsInUpperHalf)
            {
                // determine the additional height added by the tip. As the tip's image may be overlapping with the tooltip, we can't just take its size for that
                var tipProtrusion = tipTop == null ? 0 : +(GetHighestScreenPoint(tipTop, uiCam) - GetHighestScreenPoint(tooltipBoxRect, uiCam));
                // calculate desired offset between tooltip's tip and target center
                var offset = (tooltipBoxScreenHeight / 2) + tipProtrusion + (targetRectSizeScreen.y / 2) + tooltipSetup.AdditionalOffsetToTarget;
                // determine the final yPos. We want to point exactly at the edge of the targetRect.
                yPos = targetCenterScreen.y - offset;
                // assert that we don't exceed the bottom bounds of the allowed area. If yes, we shift the box up again and accept that we then don't point precisely at the target anymore
                var yMin = GetLowestScreenPoint(allowedTooltipArea, uiCam) + (tooltipBoxScreenHeight / 2f);
                yPos = Mathf.Max(yMin, yPos);
            }
            else
            {
                var tipProtrusion = tipBottom == null ? 0 : -(GetLowestScreenPoint(tipBottom, uiCam) - GetLowestScreenPoint(tooltipBoxRect, uiCam));
                var offset = (tooltipBoxScreenHeight / 2) + tipProtrusion + (targetRectSizeScreen.y / 2) + tooltipSetup.AdditionalOffsetToTarget;
                yPos = targetCenterScreen.y + offset;
                var yMax = GetHighestScreenPoint(allowedTooltipArea, uiCam) - (tooltipBoxScreenHeight / 2f);
                yPos = Mathf.Min(yMax, yPos);
            }

            // set final tooltip box position
            var wantedScreenPos = new Vector2(tooltipScreenPosX, yPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipBoxRect.parent as RectTransform, wantedScreenPos, uiCam, out var wantedLocalPoint);
            tooltipBoxRect.localPosition = wantedLocalPoint;

            // set final tip positions / active state: Make tip point at target
            if (tipTop != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(tipTop.parent as RectTransform, targetCenterScreen, uiCam, out var targetCenterAsLocalPoint);
                tipTop.transform.localPosition = new Vector3(targetCenterAsLocalPoint.x, tipTop.transform.localPosition.y, tipTop.transform.localPosition.z);
                tipTop.gameObject.SetActive(targetIsInUpperHalf);
            }

            if (tipBottom != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(tipBottom.parent as RectTransform, targetCenterScreen, uiCam, out var targetCenterAsLocalPoint);
                tipBottom.transform.localPosition = new Vector3(targetCenterAsLocalPoint.x, tipBottom.transform.localPosition.y, tipBottom.transform.localPosition.z);
                tipBottom?.gameObject.SetActive(!targetIsInUpperHalf);
            }
        }

        private void Close()
        {
            Destroy(this.gameObject);
        }

        private void IgnoreLayout()
        {
            var layoutElement = GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.ignoreLayout = true;
        }

        private float GetLowestScreenPoint(RectTransform rect, Camera uiCam)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return RectTransformUtility.WorldToScreenPoint(uiCam, corners[0]).y;
        }

        private float GetHighestScreenPoint(RectTransform rect, Camera uiCam)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return RectTransformUtility.WorldToScreenPoint(uiCam, corners[1]).y;
        }

        protected virtual void AnimateIn(TooltipSetup tooltipSetup)
        {
            if (!animateIn)
            {
                return;
            }

            // We neither have UniTask (yet) in this package, nor a tweening library, so let's use a good old coroutine
            StartCoroutine(AnimateInCoroutine(tooltipSetup));
        }

        private IEnumerator AnimateInCoroutine(TooltipSetup tooltipSetup)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Set tooltip pivot to target center and "blow up" our tooltip from there
            var targetRectTransform = tooltipSetup.TargetRectTransform;
            var targetCenter = targetRectTransform.TransformPoint(targetRectTransform.rect.center);
            SetPivotFromWorldPosition(tooltipBoxRect, targetCenter);

            var elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                var timeRatio = Mathf.Clamp01(elapsedTime / animationDuration);

                // Use the animation curve for an organic feel
                var evaluatedProgress = animationCurve.Evaluate(timeRatio);

                // Scale and fade simultaneously
                canvasGroup.alpha = evaluatedProgress;
                tooltipBoxRect.localScale = new Vector3(evaluatedProgress, evaluatedProgress, 1f);

                yield return null;
            }

            // Guarantee exact final values
            canvasGroup.alpha = 1f;
            tooltipBoxRect.localScale = Vector3.one;
        }

        public void SetPivotFromWorldPosition(RectTransform source, Vector3 targetWorldPos)
        {
            // Convert that world position into the source's local space
            var localPoint = source.InverseTransformPoint(targetWorldPos);

            // Normalize the local coordinate based on the source's bounding rectangle [0,1]
            var newPivot = new Vector2(
                (localPoint.x - source.rect.xMin) / source.rect.width,
                (localPoint.y - source.rect.yMin) / source.rect.height
            );

            // Shift the pivot while applying a position correction offset to prevent visual jumping
            var pivotDelta = newPivot - source.pivot;
            var positionOffset = new Vector2(pivotDelta.x * source.rect.width, pivotDelta.y * source.rect.height);

            // Update pivot & position
            source.pivot = newPivot;
            source.anchoredPosition += positionOffset;
        }

        private void CreateBackgroundBlocker()
        {
            // Find the root canvas for the fullscreen stretch
            var rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
            if (rootCanvas == null) return;
            var rootCanvasRect = rootCanvas.GetComponent<RectTransform>();

            // Create the blocker game object
            if (backgroundBlocker == null)
            {
                backgroundBlocker = new GameObject("TooltipBlocker", typeof(RectTransform), typeof(Image));
                backgroundBlocker.AddComponent<LayoutElement>().ignoreLayout = true;
            }
            var blockerRect = backgroundBlocker.GetComponent<RectTransform>();

            // Make blocker our first child (blocker is automatically on the same canvas that way)
            blockerRect.SetParent(transform, false);
            blockerRect.SetAsFirstSibling();

            // Make the blocker span across the entire screen (i.e. the root canvas)
            SpanAcrossRootCanvas(blockerRect, rootCanvasRect);

            // make it invisible but targetable by graphics raycaster
            var blockerImg = backgroundBlocker.GetComponent<Image>();
            blockerImg.color = new Color(0, 0, 0, 0f);

            // add click listener to self-destruct this specific tooltip when tapping outside
            var trigger = backgroundBlocker.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(_ =>
            {
                Destroy(backgroundBlocker);
                Close();
            });
            trigger.triggers.Add(entry);
        }

        private void MakeMeTopCanvas()
        {
            allOpenTooltipControllers.RemoveAll(c => c == null);

            foreach (var canvasController in allOpenTooltipControllers)
            {
                canvasController.canvas.sortingOrder -= 1;
            }

            if (canvas == null && (canvas = GetComponent<Canvas>()) == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }

            if (raycaster == null && (raycaster = GetComponent<GraphicRaycaster>()) == null)
            {
                raycaster = gameObject.AddComponent<GraphicRaycaster>();
            }

            canvas.overrideSorting = true;
            canvas.sortingOrder = newTooltipCanvasSortingGroup;
        }

        private void UndoMakeMeTopCanvas()
        {
            allOpenTooltipControllers.RemoveAll(c => c == null);

            foreach (var canvasController in allOpenTooltipControllers)
            {
                canvasController.canvas.sortingOrder += 1;
            }
        }

        private Vector2 GetPixelSizeOfRect(RectTransform rectTransform, Camera uiCamera)
        {
            // Array to hold the four world corners
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Convert world corners to screen pixel positions
            var bottomLeft = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]);
            var topRight = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]);

            // Calculate the differences
            var pixelWidth = topRight.x - bottomLeft.x;
            var pixelHeight = topRight.y - bottomLeft.y;

            var pixelSize = new Vector2(pixelWidth, pixelHeight);

            return pixelSize;
        }

        public static void SpanAcrossRootCanvas(RectTransform targetRect, RectTransform rootCanvasRect)
        {
            // Get the 4 corners of the Root Canvas in World Space
            var canvasWorldCorners = new Vector3[4];
            rootCanvasRect.GetWorldCorners(canvasWorldCorners);

            // Center anchors and pivot
            targetRect.anchorMin = new Vector2(0.5f, 0.5f);
            targetRect.anchorMax = new Vector2(0.5f, 0.5f);
            targetRect.pivot = new Vector2(0.5f, 0.5f);

            // Transform the world corners into the local space of the target's parent.
            var parentTransform = targetRect.parent;
            var localBottomLeft = parentTransform.InverseTransformPoint(canvasWorldCorners[0]);
            var localTopRight = parentTransform.InverseTransformPoint(canvasWorldCorners[2]);

            // Calculate width and height in local space
            var localWidth = Mathf.Abs(localTopRight.x - localBottomLeft.x);
            var localHeight = Mathf.Abs(localTopRight.y - localBottomLeft.y);

            // Apply the calculated size
            targetRect.sizeDelta = new Vector2(localWidth, localHeight);

            // Match the exact World Space position of the Root Canvas
            targetRect.position = rootCanvasRect.position;
        }

        // Useful for debugging
        //void OnGUI()
        //{
        //    GUILayout.Label($"<size=30>{Input.mousePosition}</size>");
        //}
    }
}
