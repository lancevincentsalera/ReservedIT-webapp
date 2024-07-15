using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public void AddBooking(BookingViewModel model)
        {
            var booking = new Booking();
            if(!_repository.BookingExists(model.BookingId))
            {
                _mapper.Map(model, booking);
                booking.BookingStatus = BookingStatus.PENDING.ToString();
                booking.CreatedDt = DateTime.Now;
                booking.UpdatedDt = DateTime.Now;
                booking.CreatedBy = System.Environment.UserName;
                booking.UpdatedBy = System.Environment.UserName;

                _repository.AddBooking(booking);

                if(model.DayOfTheWeekIds != null && model.DayOfTheWeekIds.Any())
                {
                    foreach (var id in model.DayOfTheWeekIds)
                    {
                        var recurrence = new Recurrence
                        {
                            DayOfWeekId = id,
                            BookingId = booking.BookingId,
                        };

                        _repository.AddRecurrence(recurrence);
                    }
                }
            }
        }

        public void DeleteBooking(Booking booking)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookingViewModel> GetBookings()
        {
            var bookings = _repository.GetBookings();
            return bookings.Select(booking => new BookingViewModel
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                BookingStatus = booking.BookingStatus,
                StartDate = booking.StartDate.Value,
                EndDate = booking.EndDate.Value,
                TimeFrom = booking.TimeFrom,
                TimeTo = booking.TimeTo,
                RoomName = booking.Room.RoomName,
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList(),
                User = booking.User,
                Room = booking.Room,
            });
        }

        public IEnumerable<BookingViewModel> GetBookingsByUser(int userId)
        {
            var bookings = _repository.GetBookingsByUser(userId);
            return bookings.Select(booking => new BookingViewModel
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                BookingStatus = booking.BookingStatus,
                StartDate = booking.StartDate.Value,
                EndDate =  booking.EndDate.Value,
                TimeFrom = booking.TimeFrom,
                TimeTo = booking.TimeTo,
                RoomName = booking.Room.RoomName,
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList(),
            });
        }

        public void UpdateBooking(BookingViewModel booking)
        {
            var bookingToBeUpdated = _repository.GetBookings().Where(u => u.BookingId == booking.BookingId).FirstOrDefault();
            if (bookingToBeUpdated != null)
            {
                _mapper.Map(booking, bookingToBeUpdated);
                bookingToBeUpdated.UpdatedDt = DateTime.Now;
                bookingToBeUpdated.UpdatedBy = System.Environment.UserName;

                _repository.UpdateBooking(bookingToBeUpdated);
            }
        }

        public void DeleteBooking(BookingViewModel booking)
        {
            var bookingToBeDeleted = _repository.GetBookings().Where(u => u.BookingId == booking.BookingId).FirstOrDefault();
            if (bookingToBeDeleted != null)
            {
                _repository.DeleteBooking(bookingToBeDeleted);
            }

        }
    }
}
