
#if ADDRESSABLES
namespace TeaSpoons.UGuiDesignSystem
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public abstract class AsyncLoadableTooltip<T> : TooltipContent<T>
    {
        /// <summary>
        /// Instantiate using a user defined TooltipSetup
        /// </summary>
        public async UniTask<TooltipContent<T>> InstantiateAndOpenAsync(Transform tooltipParent, T tooltipParameter, TooltipSetup customTooltipSetup)
        {
            try
            {
                var instances = await InstantiateAsync(this, tooltipParent);
                var instance = instances[0];

                instance.SetupTooltip(tooltipParameter, customTooltipSetup);

                return instance;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
#endif
