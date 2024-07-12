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
            return _repository.GetBookings().Select(booking => new BookingViewModel
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                BookingStatus = booking.BookingStatus,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TimeFrom = booking.TimeFrom,
                TimeTo = booking.TimeTo,
                /*RoomName = booking.Room.RoomName,*/
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList()
            });
        }

        public void UpdateBooking(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
