using System.Collections.ObjectModel;
using WeighBridge.ViewModels.Base;

namespace WeighBridge.ViewModels
{
    // ── Single notification item ──────────────────────────────────────────────
    public class NotificationItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "weight" | "container" | "vgm"
        public bool IsRead { get; set; } = false;

        // Icon per category
        public string Icon => Category switch
        {
            "weight" => "⚖",
            "container" => "⊡",
            "vgm" => "📋",
            _ => "🔔"
        };

        // Accent colour per category
        public string AccentColor => Category switch
        {
            "weight" => "#E67E22",
            "container" => "#E8001D",
            "vgm" => "#2ECC71",
            _ => "#9DA3C0"
        };
    }

    // ── ViewModel ─────────────────────────────────────────────────────────────
    public class NotificationViewModel : BaseViewModel
    {
        // ── Notification settings (checkbox logic from the photo) ─────────────
        private bool _alertePoids = false;
        private bool _alerteConteneur = false;
        private bool _notifVGMExpire = false;

        public bool AlertePoids
        {
            get => _alertePoids;
            set
            {
                SetProperty(ref _alertePoids, value);
                ApplyFilter();
            }
        }

        public bool AlerteConteneur
        {
            get => _alerteConteneur;
            set
            {
                SetProperty(ref _alerteConteneur, value);
                ApplyFilter();
            }
        }

        public bool NotifVGMExpire
        {
            get => _notifVGMExpire;
            set
            {
                SetProperty(ref _notifVGMExpire, value);
                ApplyFilter();
            }
        }

        // ── All notifications (master list) ───────────────────────────────────
        public ObservableCollection<NotificationItem> AllNotifications { get; } = new()
        {
            new NotificationItem
            {
                Title       = "Abnormal weight detected",
                Description = "WB-01: 48.32 MT — exceeds the authorized threshold (45 MT).",
                Time        = "3 min ago",
                Category    = "weight",
                IsRead      = false
            },
            new NotificationItem
            {
                Title       = "Unauthorized container",
                Description = "TCKU3456781 is not referenced in the TOS.",
                Time        = "12 min ago",
                Category    = "container",
                IsRead      = false
            },
            new NotificationItem
            {
                Title       = "VGM expired",
                Description = "VGM certificate for MSCU1234567 expired yesterday.",
                Time        = "1 hr ago",
                Category    = "vgm",
                IsRead      = false
            },
            new NotificationItem
            {
                Title       = "Abnormal weight detected",
                Description = "WB-01: 51.10 MT — suspicious gross weight.",
                Time        = "2 hr ago",
                Category    = "weight",
                IsRead      = true
            },
            new NotificationItem
            {
                Title       = "Unauthorized container",
                Description = "HLXU9876543 not found in the manifest.",
                Time        = "3 hr ago",
                Category    = "container",
                IsRead      = true
            },
        };

        // ── Filtered list shown in the popup ──────────────────────────────────
        public ObservableCollection<NotificationItem> FilteredNotifications { get; } = new();

        // ── Badge count (unread, filtered) ────────────────────────────────────
        public int UnreadCount => FilteredNotifications.Count(n => !n.IsRead);
        public string BadgeText => UnreadCount > 9 ? "9+" : UnreadCount.ToString();
        public bool HasUnread => UnreadCount > 0;

        // ── Popup visibility ──────────────────────────────────────────────────
        private bool _isOpen = false;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public RelayCommand TogglePopupCommand { get; }
        public RelayCommand ClosePopupCommand { get; }
        public RelayCommand MarkAllReadCommand { get; }
        public RelayCommand ClearAllCommand { get; }

        // ── Constructor ───────────────────────────────────────────────────────
        public NotificationViewModel()
        {
            TogglePopupCommand = new RelayCommand(_ => IsOpen = !IsOpen);
            ClosePopupCommand = new RelayCommand(_ => IsOpen = false);

            MarkAllReadCommand = new RelayCommand(_ =>
            {
                foreach (var n in FilteredNotifications)
                    n.IsRead = true;
                RefreshCounts();
            });

            ClearAllCommand = new RelayCommand(_ =>
            {
                // Remove filtered items from master list too
                foreach (var n in FilteredNotifications.ToList())
                    AllNotifications.Remove(n);
                ApplyFilter();
            });

            // Initial load — no filter checked → show nothing
            ApplyFilter();
        }

        // ── Filter logic: show only categories whose checkbox is ON ───────────
        public void ApplyFilter()
        {
            FilteredNotifications.Clear();

            // If no checkbox is ticked → show ALL notifications (default / overview mode)
            bool noneChecked = !AlertePoids && !AlerteConteneur && !NotifVGMExpire;

            foreach (var n in AllNotifications)
            {
                if (noneChecked)
                {
                    FilteredNotifications.Add(n);
                }
                else
                {
                    bool include = (AlertePoids && n.Category == "weight")
                               || (AlerteConteneur && n.Category == "container")
                               || (NotifVGMExpire && n.Category == "vgm");
                    if (include) FilteredNotifications.Add(n);
                }
            }

            RefreshCounts();
        }

        private void RefreshCounts()
        {
            OnPropertyChanged(nameof(UnreadCount));
            OnPropertyChanged(nameof(BadgeText));
            OnPropertyChanged(nameof(HasUnread));
            OnPropertyChanged(nameof(FilteredNotifications));
        }
    }
}