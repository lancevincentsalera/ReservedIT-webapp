using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserBookingFrequency
    {
        public string UserName { get; set; }
        public int NoOfBookings { get; set; }
        public string LastBookingDate { get; set; }
        public string TotalDuration { get; set; }
        public string MostBookedRoom { get; set; }
    }

    public class RoomUsageSummary
    {
        public string RoomName { get; set; }
        public int TotalBooking { get; set; }
        public string PeakDay { get; set; }
        public string PeakTime { get; set; }
        public string TotalDuration { get; set; }
    }

}
