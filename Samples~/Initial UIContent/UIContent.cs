
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections.Generic;

    /// <summary>
    /// Component for referencing and directly editing the content of a button, tab, progress bar, etc.
    /// </summary>
    public class UIContent : MonoBehaviour
    {
#if UNITY_EDITOR
        internal IEnumerable<PropertyLocator> GetQuickEditUiElements()
        {
            yield return new("Label", components.Label, "m_text");
            yield return new("Icon", components.Icon, "m_Sprite");
        }
#endif

        [System.Serializable]
        protected struct Components
        {
            public TMP_Text Label;
            public Image Icon;
        }

        [SerializeField]
        protected Components components;

        public string Label
        {
            get => components.Label?.text ?? string.Empty;
            set
            {
                if (components.Label != null)
                {
                    components.Label.text = value;
                }
            }
        }

        public Sprite Icon
        {
            get => components.Icon?.sprite;
            set
            {
                if (components.Icon != null)
                {
                    components.Icon.sprite = value;
                }
            }
        }
    }
}
