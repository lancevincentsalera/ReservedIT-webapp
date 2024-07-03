using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class DayOfWeek
    {
        public int? DayOfWeek1 { get; set; }
        public string DayName { get; set; }
        public int? RecurrenceId { get; set; }

        public virtual Recurrence Recurrence { get; set; }
    }
}
