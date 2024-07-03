using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Recurrence
    {
        public Recurrence()
        {
            Bookings = new HashSet<Booking>();
        }

        public int RecurrenceId { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
