
namespace TeaSpoons.UGuiDesignSystem.Editor.Tests
{
    using NUnit.Framework;

    public class NotificationDatabaseTest
    {
        [Test]
        public void AddAndInvokeCallbacks()
        {
            var eventCount = 0;
            var summerEventCount = 0;
            var summerEventChestCount = 0;
            var winterEventCount = 0;

            NotificationDatabase.RemoveAllNotifications();

            NotificationDatabase.AddNotification("events/summer2025/free-chest");

            var subPathEvents = "events";
            NotificationDatabase.AddCallback(subPathEvents, () => eventCount = NotificationDatabase.CountNotifications(subPathEvents));
            var subPathSummer = "events/summer2025";
            NotificationDatabase.AddCallback(subPathSummer, () => summerEventCount = NotificationDatabase.CountNotifications(subPathSummer));
            var subPathSummerFreeChest = "events/summer2025/free-chest";
            NotificationDatabase.AddCallback(subPathSummerFreeChest, () => summerEventChestCount = NotificationDatabase.CountNotifications(subPathSummerFreeChest));
            var subPathWinterFreeChest = "events/winter2025/free-chest";
            NotificationDatabase.AddCallback(subPathWinterFreeChest, () => winterEventCount = NotificationDatabase.CountNotifications(subPathWinterFreeChest));

            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(1, summerEventCount);
            Assert.AreEqual(1, summerEventChestCount);
            Assert.AreEqual(0, winterEventCount);

            NotificationDatabase.AddNotification("events/winter2025/free-chest");

            Assert.AreEqual(2, eventCount);
            Assert.AreEqual(1, summerEventCount);
            Assert.AreEqual(1, summerEventChestCount);
            Assert.AreEqual(1, winterEventCount);

            NotificationDatabase.RemoveNotification("events/summer2025/free-chest");

            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(0, summerEventCount);
            Assert.AreEqual(0, summerEventChestCount);
            Assert.AreEqual(1, winterEventCount);

            NotificationDatabase.RemoveNotification("events/winter2025/free-chest");

            Assert.AreEqual(0, eventCount);
            Assert.AreEqual(0, summerEventCount);
            Assert.AreEqual(0, summerEventChestCount);
            Assert.AreEqual(0, winterEventCount);
        }
    }
}
