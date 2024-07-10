using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public string BookingStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string RoomName { get; set; }
        public List<Recurrence> Recurrence { get; set; }

    }
}
