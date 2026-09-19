namespace TeaSpoons.UGuiDesignSystem.Samples
{
    using UnityEngine;

    public class SampleOuterWindow : MonoBehaviour
    {
        public static class NotificationPaths
        {
            public static string OpenInnerWindow = SampleHudButtonOpenOuterWindow.NotificationPath + "/btnOpenInnerWindow";
        }
        

        [SerializeField]
        private ButtonController btnOpenInnerWindow;

        private void Awake()
        {
            // disable window at start
            gameObject.SetActive(false);

            // set notification path for 'open inner window' button
            btnOpenInnerWindow.GetComponentInChildren<UINotificationHighlight>(true)?
                .SetNotificationPath(NotificationPaths.OpenInnerWindow);
        }
    }
}