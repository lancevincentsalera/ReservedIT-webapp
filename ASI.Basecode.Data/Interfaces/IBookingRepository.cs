using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetBookings();
        bool BookingExists(int bookingID);
        void AddBooking(Booking booking);
        void UpdateBooking(Booking booking);
        void DeleteBooking(Booking booking);
        IQueryable<Recurrence> GetBookingRecurrence(int bookingID);
        IQueryable<Booking> GetBookingsByUser(int userId);
        void AddRecurrence(Recurrence recurrence);
        List<int> GetDayOfWeekIdsForBooking(int bookingID);
    }
}
