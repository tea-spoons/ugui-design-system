namespace TeaSpoons.UGuiDesignSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using RuntimeToolbox;
#if TEXTMESHPRO
    using TMPro;
#endif
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// For enabling and disabling notification highlights (like a red dot) on UI elements.
    /// </summary>
    [DisallowMultipleComponent]
    public class UINotificationHighlight : MonoBehaviour
    {
        [Header("Notification paths")]
        [SerializeField]
        [Tooltip("The notification paths our UI highlighting is subscribed to. " +
                 "Multiple paths should only be needed in special cases, like you want to subscribe to path/a, path/b but not path/c.")]
        private List<string> notificationPaths;

        [SerializeField]
        [Tooltip("Instead of hard-coding the notification path string(s) manually above, you may also set one or more string providers here.")]
        private List<InterfaceField<IUINotificationPathProvider>> additionalNotificationPathProviders;

        [SerializeField]
        [TextArea]
        [Tooltip("Printout of the path provider strings (for debugging)")]
        private string providersDebugOutput = "";

        private bool hasPathProviders => additionalNotificationPathProviders != null && additionalNotificationPathProviders.Exists(p => p.Value != null);

        [Space]
        [Header("Settings")]
        [SerializeField]
        [Tooltip("If null, this gameObject will be toggled on and off. If a remote object is to be used instead, link it here.")]
        private GameObject optionalRemoteTargetObject;
#if TEXTMESHPRO
        [SerializeField]
        private TMP_Text counterLabel;
#endif
        [SerializeField]
        [Tooltip("Will shorten the notification count shown in the label if > 100, and show 99+ instead.")]
        private bool abbreviateHighCounts = true;

        [Space]
        [SerializeField]
        [Tooltip("If true, the target gameObject will be turned on/off depending on the notification count being > 0." +
                 "Users may want to set this to 'false' if they define their own behavior using the Unity Events below.")]
        private bool toggleGameObject = true;

        [Space]
        [SerializeField]
        private UnityEvent onEnable;
        [Space]
        [SerializeField]
        private UnityEvent onDisable;

        private bool? wasEnabled;

        private readonly List<string> subscribedPaths = new();
        private readonly List<string> tmpStringList = new(8);

        private void Awake()
        {
            SetDebugProviderStrings();

            var paths = GetNotificationPaths();
            SubscribeToNotificationPaths(paths.ToArray());

            // force an update
            OnNotificationCountChanged();
        }

        private void OnDestroy()
        {
            UnsubscribeFromAllNotificationPaths();
        }

        private void OnEnable()
        {
            // we didn't update the label while we were disabled. Do it now.
            OnNotificationCountChanged();
        }

        private void OnValidate()
        {
            SetDebugProviderStrings();
        }

        private void SubscribeToNotificationPaths(params string[] paths)
        {
            foreach (var path in paths)
            {
                // note: callback will be added and immediately executed
                NotificationDatabase.AddCallback(path, OnNotificationCountChanged);
                subscribedPaths.Add(path);
            }
        }
        private void UnsubscribeFromAllNotificationPaths()
        {
            foreach (var subscribedPath in subscribedPaths)
            {
                NotificationDatabase.RemoveCallback(subscribedPath, OnNotificationCountChanged);
            }
            subscribedPaths.Clear();
        }

        private void OnNotificationCountChanged()
        {
            var count = NotificationDatabase.CountNotifications(GetNotificationPaths());

            var shouldEnable = count > 0;

#if TEXTMESHPRO
            // set notification counter to label (if visible)
            if (counterLabel != null && counterLabel.gameObject.activeInHierarchy)
            {
                if (abbreviateHighCounts && count > 99)
                {
                    counterLabel.SetText("99+");
                }
                else
                {
                    counterLabel.SetText(count.ToString());
                }
            }
#endif

            // return if enabled (=visibility) state didn't change
            if (wasEnabled == shouldEnable)
            {
                return;
            }

            if (toggleGameObject)
            {
                SetVisible(shouldEnable);
            }

            var evt = shouldEnable ? onEnable : onDisable;
            evt.Invoke();

            wasEnabled = shouldEnable;
        }

        public void SetCurrentNotificationPathsActiveInDatabase(bool setActive)
        {
            // copy to avoid collection modification exception
            var pathListCopy = GetNotificationPaths().ToList();

            foreach (var notificationPath in pathListCopy)
            {
                if (setActive)
                {
                    NotificationDatabase.AddNotification(notificationPath);
                }
                else
                {
                    NotificationDatabase.RemoveNotification(notificationPath);
                }
            }
        }

        public bool IsAnyNotificationPathActiveInDatabase => GetNotificationPaths().Any(NotificationDatabase.HasNotification);
        
        private IReadOnlyList<string> GetNotificationPaths()
        {
            if (!hasPathProviders)
            {
                return notificationPaths;
            }

            // collect paths from manual string list and providers
            tmpStringList.Clear();
            tmpStringList.AddRange(notificationPaths);
            foreach (var notificationPathProvider in additionalNotificationPathProviders)
            {
                if (notificationPathProvider.Value == null)
                {
                    continue;
                }

                tmpStringList.Add(notificationPathProvider.Value.UiNotificationPath);
            }
            
            return tmpStringList;
        }

        public void SetNotificationPath(string path)
        {
            SetNotificationPaths(new List<string> { path });
        }

        public void SetNotificationPaths(List<string> paths)
        {
            UnsubscribeFromAllNotificationPaths();
            SubscribeToNotificationPaths(paths.ToArray());
            additionalNotificationPathProviders.Clear();
            notificationPaths = paths;
        }

        private void SetVisible(bool isVisible)
        {
            if (optionalRemoteTargetObject != null)
            {
                optionalRemoteTargetObject.SetActive(isVisible);
            }
            else
            {
                gameObject.SetActive(isVisible);
            }
        }

        private void SetDebugProviderStrings()
        {
#if UNITY_EDITOR
            if (!hasPathProviders)
            {
                providersDebugOutput = "[no additional providers set]";
                return;
            }

            providersDebugOutput = string.Join(",\r\n",
                additionalNotificationPathProviders.FindAll(p => p.Value != null)
                    .ConvertAll(p => p.Value?.UiNotificationPath));

#endif
        }
    }
}
