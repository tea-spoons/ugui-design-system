
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;

    /// <summary>
    /// Base class for a <see cref="MonoBehaviour"/> that represents a object of type <typeparamref name="T"/>.
    /// </summary>
    public abstract class UIRepresentation<T> : MonoBehaviour
    {
        protected T target { get; private set; }

        public void SetTarget(T target)
        {
            ResetUI();
            this.target = target;
            SetupUI();
        }

        protected virtual void ResetUI() {}

        protected abstract void SetupUI();
    }
}
