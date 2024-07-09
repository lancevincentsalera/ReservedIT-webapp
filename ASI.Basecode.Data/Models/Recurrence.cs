using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Recurrence
    {
        public int RecurrenceId { get; set; }
        public int BookingId { get; set; }
        public int? DayOfWeekId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual DayOfTheWeek DayOfWeek { get; set; }
    }
}
