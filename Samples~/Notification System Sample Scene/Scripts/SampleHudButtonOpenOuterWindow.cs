using UnityEngine;

namespace TeaSpoons.UGuiDesignSystem.Samples
{
    public class SampleHudButtonOpenOuterWindow : MonoBehaviour
    {
        public static string NotificationPath = "btnOpenOuterWindow";

        private void Awake()
        {
            // set notification highlight path for this button
            GetComponentInChildren<UINotificationHighlight>(true)?
                .SetNotificationPath(NotificationPath);
        }
    }
}
