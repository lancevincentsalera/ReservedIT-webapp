using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Setting
    {
        public int SettingId { get; set; }
        public int? UserId { get; set; }
        public int? BookingSuccess { get; set; }
        public int? BookingStatusChange { get; set; }
        public DateTime? BookingReminder { get; set; }
        public TimeSpan? BookingDuration { get; set; }

        public virtual User User { get; set; }
    }
}
