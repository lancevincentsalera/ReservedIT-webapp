using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void AddBooking(Booking booking)
        {
            this.GetDbSet<Booking>().Add(booking);
            UnitOfWork.SaveChanges();
        }

        public bool BookingExists(int bookingID)
        {
            return this.GetDbSet<Booking>().Any(b => b.BookingId == bookingID);
        }

        public void DeleteBooking(Booking booking)
        {
            this.GetDbSet<Booking>().Remove(booking);
            UnitOfWork.SaveChanges();
        }

        public IQueryable<Booking> GetBookings()
        {
            return this.GetDbSet<Booking>();
        }

        public IQueryable<Booking> GetBookingsByUser(int userId)
        {
            return this.GetDbSet<Booking>()
                .Where(b => b.UserId == userId);
        }
        public void UpdateBooking(Booking booking)
        {
            this.GetDbSet<Booking>().Update(booking);
            UnitOfWork.SaveChanges();
        }


        public void AddRecurrence(Recurrence recurrence)
        {
            this.GetDbSet<Recurrence>().Add(recurrence);
            UnitOfWork.SaveChanges();
        }

        public IQueryable<Recurrence> GetBookingRecurrence(int bookingID)
        {
            return this.GetDbSet<Recurrence>()
                .Include(r => r.DayOfWeek)
                .Where(r => r.BookingId == bookingID);
        }

        public List<int> GetDayOfWeekIdsForBooking(int bookingID)
        {
            List<int> DayOfTheWeekIds = this.GetDbSet<Recurrence>()
                .Where(r => r.BookingId == bookingID)
                .Select(r => r.DayOfWeekId ?? -1)
                .ToList();

            return DayOfTheWeekIds;
        }

    }
}
