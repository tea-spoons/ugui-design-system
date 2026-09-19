
namespace TeaSpoons.UGuiDesignSystem
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public static class NotificationDatabase
    {
        private static readonly StringBuilder tmpStringBuilder = new();
        private static readonly List<string> tmpListWrapper = new();

        private class Entry
        {
            public event Action Callback = delegate { };
            public int NotificationCount
            {
                get => notificationCount;
                set
                {
                    if (value < 0) return;

                    notificationCount = value;
                    Callback();
                }
            }
            private int notificationCount = 0;
        }

        private static readonly Dictionary<string, Entry> notificationPaths = new();

        public static void AddCallback(string path, Action callback)
        {
            var entry = GetOrCreateEntry(path);
            entry.Callback += callback;
            callback();
        }

        public static void RemoveCallback(string path, Action callback)
        {
            if (notificationPaths.TryGetValue(path, out var entry))
            {
                entry.Callback -= callback;
            }
            else
            {
                Debug.LogWarning($"RemoveCallback() failed. Path not registered: {path}");
            }
        }

        public static void AddNotification(string path)
        {
            if (notificationPaths.TryGetValue(path, out var value) && value.NotificationCount > 0)
            {
                // for now: require unique path - else high risk to write bugged code by registering a notification path twice
                Debug.LogWarning($"Couldn't add notification for path {path}. Path already registered and each added notification path must be unique.");
                return;
            }

            foreach (var partialPath in GetPartialPaths(path))
            {
                var entry = GetOrCreateEntry(partialPath);
                entry.NotificationCount++;
            }
        }

        public static bool HasNotification(string path)
        {
            return notificationPaths.TryGetValue(path, out var value) && value.NotificationCount > 0;
        }

        public static void RemoveNotification(string path)
        {
            if (!notificationPaths.TryGetValue(path, out var value) || value.NotificationCount <= 0)
            {
                // for now: require unique path - else high risk to write bugged code by unregistering a notification that was never registered (and sub-path counts will yet be subtracted)
                Debug.LogWarning($"Couldn't remove notification for path {path}. Path not registered.");
                return;
            }

            foreach (var partialPath in GetPartialPaths(path))
            {
                if (notificationPaths.TryGetValue(partialPath, out var entry))
                {
                    entry.NotificationCount--;
                }
                else
                {
                    Debug.LogWarning($"RemoveNotification() failed. Partial path not registered: {partialPath}");
                }
            }
        }

        private static Entry GetOrCreateEntry(string path)
        {
            if (!notificationPaths.TryGetValue(path, out var entry))
            {
                entry = new Entry();
                notificationPaths.Add(path, entry);
            }
            return entry;
        }

        private static IEnumerable<string> GetPartialPaths(string id)
        {
            // TODO garbage, meh
            var partialPath = tmpStringBuilder;
            partialPath.Clear();

            foreach (var segment in id.Split('/'))
            {
                if (partialPath.Length > 0)
                {
                    partialPath.Append('/');
                }
                partialPath.Append(segment);
                yield return partialPath.ToString();
            }
        }

        public static string CreateLog()
        {
            var output = "NotificationDatabase:\n";

            foreach (var notificationPath in notificationPaths)
            {
                output += $"({notificationPath.Value.NotificationCount}) notifications @ {notificationPath.Key} \n";
            }

            return output;
        }
        
        public static int CountNotifications(string path)
        {
            tmpListWrapper.Clear();
            tmpListWrapper.Add(path);
            var result = CountNotifications(tmpListWrapper);
            tmpListWrapper.Clear();
            return result;
        }

        public static int CountNotifications(IEnumerable<string> paths)
        {
            var count = 0;
            foreach (var path in paths)
            {
                if (notificationPaths.TryGetValue(path, out var entry))
                {
                    count += entry.NotificationCount;
                }
            }

            return count;
        }

        public static void RemoveAllNotifications()
        {
            notificationPaths.Clear();
        }
    }
}
