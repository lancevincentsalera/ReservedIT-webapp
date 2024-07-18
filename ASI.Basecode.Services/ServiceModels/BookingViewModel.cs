using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookingViewModel
    {
        public BookingViewModel()
        {
            DayOfTheWeekIds = new List<int>();
        }
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public string BookingStatus { get; set; }


        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime? StartDate { get; set; }


        [Required(ErrorMessage = "End Date is required.")]
        public DateTime? EndDate { get; set; }


        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string RoomName { get; set; }
        public List<Recurrence> Recurrence { get; set; }
        public string UserName { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }

        public List<DayOfTheWeek> Days { get; set; }
        public List<int> DayOfTheWeekIds { get; set; }
        public IEnumerable<BookingViewModel> bookingList { get; set; }
        public IEnumerable<RoomViewModel> roomList { get; set; }
    }
}
