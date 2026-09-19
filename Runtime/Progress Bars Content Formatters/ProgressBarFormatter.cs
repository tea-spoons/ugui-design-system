
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;

    [RequireComponent(typeof(ProgressBarController))]
    public abstract class ProgressBarFormatter : MonoBehaviour
    {
        private ProgressBarController cachedController;

        protected internal ProgressBarController controller
        {
            get
            {
                InitializeIfNeeded();
                return cachedController;
            }
        }

        private bool isInitialized;

        private void Awake()
        {
            InitializeIfNeeded();
        }

        private void InitializeIfNeeded()
        {
            if (isInitialized) return;

            cachedController = GetComponent<ProgressBarController>();
            cachedController.ValueChanged += UpdateContent;

            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (!isInitialized) return;

            cachedController.ValueChanged -= UpdateContent;
        }

        protected abstract void UpdateContent(int value);
    }
}
