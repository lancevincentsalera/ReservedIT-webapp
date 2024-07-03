using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Notification
    {
        public Notification()
        {
            BookingNotifications = new HashSet<BookingNotification>();
        }

        public int NotificationId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public string NotificationType { get; set; }

        public virtual ICollection<BookingNotification> BookingNotifications { get; set; }
    }
}
