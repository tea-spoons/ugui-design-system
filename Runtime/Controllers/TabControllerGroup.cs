
using System;

namespace TeaSpoons.UGuiDesignSystem
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A group for <see cref="TabController"/> components.
    /// </summary>
    public class TabControllerGroup : MonoBehaviour
    {
        [SerializeField]
        private List<TabController> tabs = new();
        
        [SerializeField]
        private int initiallyActiveTabIndex;
        
        private TabController currentTab;

        public event Action<TabController> CurrentChanged = delegate { };

        private void Awake()
        {
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            foreach (var tab in tabs)
            {
                if (!tab) continue;

                tab.Initialize();
                tab.Clicked += SelectTab;
            }
            
            if (initiallyActiveTabIndex >= 0 && initiallyActiveTabIndex < tabs.Count)
            {
                SelectTabByIndex(initiallyActiveTabIndex);
            }
        }

        public void SetTabs(List<TabController> newTabs)
        {
            tabs = newTabs;
            currentTab = null;
            InitializeTabs();
        }

        private void OnDestroy()
        {
            UnsubscribeToComponentsEvents();
        }

        private void UnsubscribeToComponentsEvents()
        {
            foreach (var tab in tabs)
            {
                if (!tab) continue;

                tab.Clicked -= SelectTab;
            }
        }

        public void SelectTab(TabController tab)
        {
            if (!tab || !tabs.Contains(tab) || currentTab == tab) return;

            // Deselect previous tab
            if (currentTab)
            {
                currentTab.SetSelected(false);
            }

            currentTab = tab;
            currentTab.SetSelected(true);
            CurrentChanged.Invoke(currentTab);
        }
        
        public void SelectTabByIndex(int index)
        {
            if (index < 0 || index >= tabs.Count)
            {
                Debug.LogWarning($"TabControllerGroup :: Invalid tab index: {index}. Valid range is 0-{tabs.Count - 1}.", gameObject);
                return;
            }

            SelectTab(tabs[index]);
        }

        /// <summary>
        /// Select the next tab in the list (wraps around to first).
        /// </summary>
        public void SelectNextTab()
        {
            if (tabs.Count <= 1 || !currentTab) return;

            var currentIndex = tabs.IndexOf(currentTab);
            var nextIndex = (currentIndex + 1) % tabs.Count;
            SelectTabByIndex(nextIndex);
        }

        /// <summary>
        /// Select the previous tab in the list (wraps around to last).
        /// </summary>
        public void SelectPreviousTab()
        {
            if (tabs.Count <= 1 || !currentTab) return;

            var currentIndex = tabs.IndexOf(currentTab);
            var previousIndex = (currentIndex - 1 + tabs.Count) % tabs.Count;
            SelectTabByIndex(previousIndex);
        }
    }
}
