#if UNITASK
namespace TeaSpoons.UGuiDesignSystem
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(TooltipCanvasController))]
    public abstract class TooltipContent<T> : MonoBehaviour
    {
        [SerializeField]
        private TooltipCanvasController tooltipController;

        protected TooltipCanvasController controller
        {
            get
            {
                if (tooltipController == null)
                {
                    tooltipController = GetComponent<TooltipCanvasController>();
                }
                return tooltipController;
            }
        }

        /// <summary>
        /// Take care to await any dynamic instantiations inside the tooltip, so the layout is fixed at the end. Else the controller can't measure the dimensions correctly.
        /// </summary>
        protected abstract UniTask PopulateDataDisplay(T tooltipParameter);

        /// <summary>
        /// Updates the data view and positions the tooltip according to its target, required size and allowed space
        /// </summary>
        public void SetupTooltip(T tooltipParameter, TooltipSetup customTooltipSetup)
        {
            SetupTooltipAsync(tooltipParameter, customTooltipSetup).Forget();
        }

        private async UniTask SetupTooltipAsync(T tooltipParameter, TooltipSetup customTooltipSetup)
        {
            await PopulateDataDisplay(tooltipParameter);
            controller.ApplyTooltipSetup(customTooltipSetup);
        }
    }
}
#endif
