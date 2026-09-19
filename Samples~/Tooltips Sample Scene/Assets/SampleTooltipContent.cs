namespace TeaSpoons.UGuiDesignSystem
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class SampleTooltipContent : TooltipContent<string>
    {
        [SerializeField]
        private UIContent textToSet;

        /// <summary>
        /// Take care to await any dynamic instantiations inside the tooltip, so the layout is fixed at the end. Else the controller can't measure the dimensions correctly.
        /// </summary>
        protected override async UniTask PopulateDataDisplay(string tooltipParameter)
        {
            textToSet.Label = tooltipParameter;

            await UniTask.CompletedTask;
        }
    }
}
