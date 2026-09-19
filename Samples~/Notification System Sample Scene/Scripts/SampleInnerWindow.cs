namespace TeaSpoons.UGuiDesignSystem.Samples
{
    using UnityEngine;

    public class SampleInnerWindow : MonoBehaviour
    {
        public static class NotificationPaths
        {
            // propagate (bubble-up the dot): reference preceding paths in notification paths
            public static string RedeemCoins = SampleOuterWindow.NotificationPaths.OpenInnerWindow + "/btnCoinRedeem";
            public static string RedeemGems = SampleOuterWindow.NotificationPaths.OpenInnerWindow + "/btnGemRedeem";

            // don't propagate (dot stays local): no path, but unique prefix recommended
            public static string ShowInfoBox = "sampleInnerWindow_btnShowInfoBox";
        }

        [SerializeField]
        private ButtonController btnRedeemCoins;
        [SerializeField]
        private ButtonController btnRedeemGems;
        [SerializeField]
        private ButtonController btnInfobox;
        private void Start()
        {
            // set notification paths
            SetNotificationPaths(btnRedeemCoins, NotificationPaths.RedeemCoins);
            SetNotificationPaths(btnRedeemGems, NotificationPaths.RedeemGems);
            SetNotificationPaths(btnInfobox, NotificationPaths.ShowInfoBox);

            // trigger highlightings for all buttons
            NotificationDatabase.AddNotification(NotificationPaths.RedeemCoins);
            NotificationDatabase.AddNotification(NotificationPaths.RedeemGems);
            NotificationDatabase.AddNotification(NotificationPaths.ShowInfoBox);

            // remove highlightings on click
            btnRedeemCoins.AddOnClickListener(() => OnClick(btnRedeemCoins, NotificationPaths.RedeemCoins, true));
            btnRedeemGems.AddOnClickListener(() => OnClick(btnRedeemGems, NotificationPaths.RedeemGems, true));
            btnInfobox.AddOnClickListener(() => OnClick(btnInfobox, NotificationPaths.ShowInfoBox, false));
        }

        private void SetNotificationPaths(ButtonController button, string notificationPath)
        {
            button.GetComponentInChildren<UINotificationHighlight>(true)?
                .SetNotificationPath(notificationPath);
        }

        private void OnClick(ButtonController button, string notificationPath, bool makeBtnNonInteractive)
        {
            NotificationDatabase.RemoveNotification(notificationPath);

            if (makeBtnNonInteractive)
            {
                button.Interactable = false;
            }
        }
    }
}