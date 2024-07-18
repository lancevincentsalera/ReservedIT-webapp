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
using System.IO;
using Microsoft.EntityFrameworkCore;

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
            if (!_repository.BookingExists(model.BookingId))
            {
                if (!IsBookingConflict(model))
                {
                    _mapper.Map(model, booking);
                    booking.BookingStatus = BookingStatus.PENDING.ToString();
                    booking.CreatedDt = DateTime.Now;
                    booking.UpdatedDt = DateTime.Now;
                    booking.CreatedBy = System.Environment.UserName;
                    booking.UpdatedBy = System.Environment.UserName;

                    _repository.AddBooking(booking);

                    if (model.DayOfTheWeekIds != null && model.DayOfTheWeekIds.Any())
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
                else
                {
                    throw new InvalidDataException("Cannot create booking: A conflicting approved booking already exists for the selected date, time, and room.");
                }

            }
            else
            {
                throw new InvalidDataException("Booking already exists!");
            }
        }

        public bool IsBookingConflict(BookingViewModel model)
        {
            List<Booking> createdBookings = BookingInfo(model);
            var approvedBookings = _repository.GetBookings()
                .Where(b => b.BookingStatus.Equals(BookingStatus.APPROVED.ToString()) &&
                    b.Room.RoomName.Equals(model.RoomName) &&
                    (
                        // Check for date overlap or containment
                        (b.StartDate <= model.EndDate && b.EndDate >= model.StartDate) ||
                        (b.StartDate >= model.StartDate && b.EndDate <= model.EndDate)
                    ) &&
                    (
                        // Check for time overlap or containment
                        (b.TimeFrom <= model.TimeTo && b.TimeTo >= model.TimeFrom) ||
                        (b.TimeFrom >= model.TimeFrom && b.TimeTo <= model.TimeTo)
                    ))
                .ToList();



            List<List<Booking>> approvedBookingsWithRecurrences = ApprovedBookingsWithRecurrences(approvedBookings);

            foreach (var created in createdBookings)
            {
                foreach (var approved in approvedBookingsWithRecurrences)
                {
                    foreach (var a in approved)
                    {
                        if ((created.StartDate.Value.Date == a.StartDate.Value.Date) &&
                                ((a.TimeFrom <= created.TimeTo && a.TimeTo >= created.TimeFrom) ||
                                 (a.TimeFrom >= created.TimeFrom && a.TimeTo <= created.TimeTo)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool isRecurrentBooking(Booking booking)
        {
            if (booking != null && booking.Recurrences.Any()) { return true; }
            return false;
        }

        public List<Booking> BookingInfo(BookingViewModel model)
        {
            List<Booking> createdBookings = new List<Booking>();
            var currCreatedDate = model.StartDate.Value;
            var endDate = model.EndDate.Value;
            if (model.DayOfTheWeekIds.Any())
            {
                TimeSpan diff = endDate.Subtract(currCreatedDate);
                int weeksDiff = (int)Math.Ceiling(diff.TotalDays / 7);

                List<int> totalDaysId = new List<int>();
                for (int x = 0; x < weeksDiff; x++)
                {
                    for (int y = 0; y < model.DayOfTheWeekIds.Count(); y++)
                    {
                        totalDaysId.Add(model.DayOfTheWeekIds[y]);
                    }
                }
                for (int i = 0; i < totalDaysId.Count() - 1; i++)
                {
                    if (currCreatedDate <= model.EndDate.Value)
                    {
                        createdBookings.Add(new Booking
                        {
                            StartDate = currCreatedDate,
                            EndDate = currCreatedDate,
                            TimeFrom = model.TimeFrom,
                            TimeTo = model.TimeTo,
                        });
                    }
                    int currentCreatedDateId = totalDaysId[i];
                    int nextCreatedDateId = totalDaysId[(i + 1) % totalDaysId.Count()];

                    int todayToNextInterval = (nextCreatedDateId > currentCreatedDateId)
                                              ? nextCreatedDateId - currentCreatedDateId
                                              : (7 - currentCreatedDateId) + nextCreatedDateId;
                    currCreatedDate = currCreatedDate.AddDays(todayToNextInterval);
                    if (currCreatedDate <= endDate && i + 1 == totalDaysId.Count() - 1)
                    {
                        createdBookings.Add(new Booking
                        {
                            StartDate = currCreatedDate,
                            EndDate = currCreatedDate,
                            TimeFrom = model.TimeFrom,
                            TimeTo = model.TimeTo,
                        });
                    }
                }
            }
            else
            {
                createdBookings.Add(new Booking
                {
                    StartDate = currCreatedDate,
                    EndDate = currCreatedDate,
                    TimeFrom = model.TimeFrom,
                    TimeTo = model.TimeTo,
                });
            }

            return createdBookings;
        }

        public List<List<Booking>> ApprovedBookingsWithRecurrences(List<Booking> approvedBookings)
        {
            List<List<Booking>> approvedBookingsWithRecurrences = new List<List<Booking>>();

            List<BookingViewModel> approvedBookingsModel = approvedBookings.Select(a => new BookingViewModel
            {
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                TimeFrom = a.TimeFrom,
                TimeTo = a.TimeTo,
                DayOfTheWeekIds = a.Recurrences.Select(r => r.DayOfWeekId ?? 0).ToList(),
            }).ToList();

            foreach (var approvedModel in approvedBookingsModel)
            {
                approvedBookingsWithRecurrences.Add(BookingInfo(approvedModel));
            }

            return approvedBookingsWithRecurrences;
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
                EndDate = booking.EndDate.Value,
                TimeFrom = booking.TimeFrom,
                TimeTo = booking.TimeTo,
                RoomName = booking.Room.RoomName,
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList(),
            });
        }

        public IEnumerable<BookingViewModel> GetBookingsByDate(int year, int month, int? day)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var bookings = _repository.GetBookings().Include(b => b.Recurrences).Where(u => u.StartDate >= startDate).Where(u => u.EndDate <= endDate);

/*            if (day != null)
            {
                DateTime fullDate = new DateTime(year, month, day.GetValueOrDefault());
                bookings = bookings.Where(u => u.StartDate >= fullDate).Where(u => u.EndDate <= fullDate);
            }*/

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
            });
        }

        public void UpdateBooking(BookingViewModel booking)
        {
            if (IsBookingConflict(booking))
            {
                throw new InvalidDataException("Cannot update booking: A conflicting approved booking already exists for the selected date, time, and room.");
            }
            var bookingToBeUpdated = _repository.GetBookings()
                .Include(b => b.Recurrences)
                .Where(u => u.BookingId == booking.BookingId)
                .FirstOrDefault();
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
