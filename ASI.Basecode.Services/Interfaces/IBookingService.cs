using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookingService
    {
        void AddBooking(BookingViewModel model);
        IEnumerable<BookingViewModel> GetBookings();
        void UpdateBooking(Booking booking);
        void DeleteBooking(Booking booking);
    }
}
