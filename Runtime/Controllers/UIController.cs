
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using System.Collections.Generic;

    [SelectionBase]
    public abstract class UIController : MonoBehaviour
    {
        internal static class PropertyNames
        {
            public const string PreferredContent = nameof(preferredContent);
        }

        private static readonly Queue<Transform> traversalTransforms = new();

        [Tooltip("Reference a UIContent here to make sure that it is used over other UIContent children that this object has.\n" +
            "Not required if there's no more than one UIContent under this controller.")]
        [SerializeField, HideInInspector]
        private UIContent preferredContent;

        public bool HasContent => Content != null;

        private UIContent cachedContent;

        public UIContent Content
        {
            get
            {
                if (cachedContent == null)
                {
                    cachedContent = FindContent();
                }
                return cachedContent;
            }
        }
#if UNITY_EDITOR
        private void Awake()
        {
            if (!preferredContent && HasMultipleUIContents())
            {
                Debug.LogError($"{GetType().Name} \"{name}\" has multiple direct children UIContents. One of them must be referenced in the \"Preferred Content\" field.", this);
            }
        }

        internal bool HasMultipleUIContents()
        {
            var foundContent = false;
            foreach (var uiContent in GetComponentsInChildren<UIContent>())
            {
                if (uiContent.GetComponentInParent<UIController>() == this)
                {
                    if (foundContent)
                    {
                        return true;
                    }
                    foundContent = true;
                }
            }
            return false;
        }
#endif

        /// <summary>
        /// Performs a breadth-first search for a <see cref="UIContent"/> in children GameObjects that stops at a child that has its own UIController.
        /// </summary>
        internal UIContent FindContent()
        {
            if (preferredContent)
            {
                return preferredContent;
            }

            traversalTransforms.Enqueue(transform);

            while (traversalTransforms.Count > 0)
            {
                var child = traversalTransforms.Dequeue();

                if (child != transform && child.TryGetComponent<UIController>(out _))
                {
                    continue;
                }

                if (child.TryGetComponent<UIContent>(out var result))
                {
                    traversalTransforms.Clear();
                    return result;
                }

                foreach (Transform grandChild in child)
                {
                    traversalTransforms.Enqueue(grandChild);
                }
            }

            return null;
        }
    }
}
