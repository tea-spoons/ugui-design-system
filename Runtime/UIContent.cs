
namespace TeaSpoons.UGuiDesignSystem
{
    using UnityEngine;
    using UnityEngine.UI;
#if TEXTMESHPRO
    using TMPro;
#endif
    using System.Collections.Generic;

    /// <summary>
    /// Component for referencing and directly editing the content of a button, tab, progress bar, etc.
    /// </summary>
    /// <remarks>
    /// The label is a TextMeshPro text when TextMeshPro is in the project, and a uGUI <see cref="UnityEngine.UI.Text"/> otherwise.
    /// To adjust this class to a project, derive from it (the <c>components</c> field is protected),
    /// or copy the package into the project's <c>Packages</c> folder and edit this file.
    /// </remarks>
    public class UIContent : MonoBehaviour
    {
#if UNITY_EDITOR
        internal IEnumerable<PropertyLocator> GetQuickEditUiElements()
        {
#if TEXTMESHPRO
            yield return new("Label", components.Label, "m_text");
#else
            yield return new("Label", components.Label, "m_Text");
#endif
            yield return new("Icon", components.Icon, "m_Sprite");
        }
#endif

        [System.Serializable]
        protected struct Components
        {
#if TEXTMESHPRO
            public TMP_Text Label;
#else
            public Text Label;
#endif
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
