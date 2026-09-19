#if ADDRESSABLES && ADDRESSABLES_TOOLBOX

namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using System;
    using TeaSpoons.AddressablesToolbox;
    using Cysharp.Threading.Tasks;

    [Serializable]
    public class TooltipTReference<TTooltip, TParameter> : SmartComponentReference<TTooltip> where TTooltip : AsyncLoadableTooltip<TParameter>
    {
        public async UniTask<TTooltip> InstantiateAndOpenAsync(Transform tooltipParent, TParameter tooltipParameter, TooltipSetup customTooltipSetup)
        {
            var prefab = await GetAssetAsync();

            var instance = (TTooltip)await prefab.InstantiateAndOpenAsync(tooltipParent, tooltipParameter, customTooltipSetup);

            return instance;
        }
    }
}
#endif
