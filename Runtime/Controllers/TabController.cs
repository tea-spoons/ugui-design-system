
namespace TeaSpoons.UGuiDesignSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// A UI controller for a tab button.
    /// </summary>
    public class TabController : UIController,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        private bool isSelected, isHovered;
        
        [Header("References")]
        [SerializeField]
        private Graphic raycastTarget;

        [SerializeField, FormerlySerializedAs("content")]
        [Tooltip("The content panel associated with this tab. " +
                 "This GameObject will be activated when the tab is selected and deactivated when another tab is selected.")]
        private GameObject contentPanel;

        [Header("Visuals")]
        [SerializeField]
        private Image targetImage;

        [SerializeField]
        private Sprite activeSprite;

        [SerializeField]
        private Sprite highlightedSprite;

        [SerializeField]
        private Sprite inactiveSprite;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onActivate;
        
        public event Action<TabController> Clicked = delegate { };

        public bool IsSelected => isSelected;
        public GameObject ContentPanel => contentPanel;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked.Invoke(this);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isSelected) { return; }

            isHovered = true;
            UpdateVisuals();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            UpdateVisuals();
        }

        internal void Initialize()
        {
            isSelected = false;
            isHovered = false;

            if (contentPanel)
            {
                contentPanel.SetActive(false);
            }

            if (targetImage)
            {
                targetImage.sprite = inactiveSprite;
            }
        }
        
        internal void SetSelected(bool selected)
        {
            if (isSelected == selected) { return; }

            isSelected = selected;
            isHovered = false;

            if (contentPanel)
            {
                contentPanel.SetActive(selected);
            }
            
            UpdateVisuals();

            if (selected)
            {
                onActivate.Invoke();
            }
        }

        private void UpdateVisuals()
        {
            if (!targetImage) { return; }

            if (isSelected)
            {
                targetImage.sprite = activeSprite;
            }
            else if (isHovered && highlightedSprite)
            {
                targetImage.sprite = highlightedSprite;
            }
            else
            {
                targetImage.sprite = inactiveSprite;
            }
        }

        public void SetContent(GameObject contentObject)
        {
            contentPanel = contentObject;
            Initialize();
        }
    }
}
