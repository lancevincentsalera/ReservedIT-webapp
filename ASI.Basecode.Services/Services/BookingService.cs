using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
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
                booking.CreatedDt = DateTime.Now;
                booking.UpdatedDt = DateTime.Now;
                booking.CreatedBy = System.Environment.UserName;
                booking.UpdatedBy = System.Environment.UserName;

                _repository.AddBooking(booking);
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
                StartDate = booking.StartDate.HasValue ? booking.StartDate.Value.ToString("dd MMMM yyyy") : string.Empty,
                EndDate = booking.EndDate.HasValue ? booking.StartDate.Value.ToString("dd MMMM yyyy") : string.Empty,
                TimeFrom = new DateTime(booking.TimeFrom.Value.Ticks).ToString("h:mm tt"),
                TimeTo = new DateTime(booking.TimeTo.Value.Ticks).ToString("h:mm tt"),
                RoomName = booking.Room.RoomName,
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList(),
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
                StartDate = booking.StartDate.HasValue ? booking.StartDate.Value.ToString("dd MMMM yyyy") : string.Empty,
                EndDate = booking.EndDate.HasValue ? booking.StartDate.Value.ToString("dd MMMM yyyy") : string.Empty,
                TimeFrom = new DateTime(booking.TimeFrom.Value.Ticks).ToString("h:mm tt"),
                TimeTo = new DateTime(booking.TimeTo.Value.Ticks).ToString("h:mm tt"),
                RoomName = booking.Room.RoomName,
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList(),
            });
        }

        public void UpdateBooking(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
