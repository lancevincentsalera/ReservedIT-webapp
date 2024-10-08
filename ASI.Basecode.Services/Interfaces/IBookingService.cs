﻿using ASI.Basecode.Data.Models;
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
        int AddBooking(BookingViewModel model);
        IEnumerable<BookingViewModel> GetBookings();
        void UpdateBooking(BookingViewModel booking);
        IEnumerable<BookingViewModel> GetBookingsByUser(int userId);
        IEnumerable<BookingViewModel> GetBookingsByDate(int year, int month, int? day);
        void DeleteBooking(BookingViewModel booking);
        bool IsBookingConflict(BookingViewModel model);
    }
}
