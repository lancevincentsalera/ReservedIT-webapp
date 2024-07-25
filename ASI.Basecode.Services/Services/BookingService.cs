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



        #region Check Booking Conflict
        /// <summary>
        /// A method to determine if a created or updated booking conflicting one or more bookings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsBookingConflict(BookingViewModel model)
        {
            List<Booking> createdBookings = BookingInfo(model); // list of created bookings, recurrence considered
            var approvedBookings = _repository.GetBookings() // list of approved bookings that overlaps the date/time attr of model
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



            List<List<Booking>> approvedBookingsWithRecurrences = ApprovedBookingsWithRecurrences(approvedBookings); // list of approved bookings, along with each of their recurrences considered

            foreach (var created in createdBookings)
            {
                foreach (var approved in approvedBookingsWithRecurrences)
                {
                    foreach (var a in approved)
                    {
                        if ((created.StartDate.Value.Date == a.StartDate.Value.Date) && // overlapping dates 
                                ((a.TimeFrom < created.TimeTo && a.TimeTo > created.TimeFrom) || 
                                 (a.TimeFrom >= created.TimeFrom && a.TimeTo <= created.TimeTo))) // overlapping timespan, then -> 
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


        /// <summary>
        /// CALCULATES the booking info of the passed model along with each of its recurrences, returns a list of booking
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Booking> BookingInfo(BookingViewModel model)
        {
            List<Booking> createdBookings = new List<Booking>(); // returned value
            var currCreatedDate = model.StartDate.Value;
            var endDate = model.EndDate.Value;

            // if there are recurrences
            if (model.DayOfTheWeekIds.Any())
            {
                // calculates the difference between end date and start date
                // and then converts that into weeks 
                TimeSpan diff = endDate.Subtract(currCreatedDate);
                int weeksDiff = (int)Math.Ceiling(diff.TotalDays / 7);


                // to calculate how many times a recurrence will happen
                // in the span of weeksDiff
                List<int> totalDaysId = new List<int>();
                for (int x = 0; x < weeksDiff; x++)
                {
                    for (int y = 0; y < model.DayOfTheWeekIds.Count(); y++)
                    {
                        totalDaysId.Add(model.DayOfTheWeekIds[y]);
                    }
                }

                // if start date is also the start of booking, then
                // remove first value of the list totalDaysId
                // if not, then total count of IDs remain unchanged
                if (((int)currCreatedDate.DayOfWeek) + 1 == totalDaysId[0])
                {
                    if (currCreatedDate <= endDate)
                    {
                        createdBookings.Add(new Booking
                        {
                            StartDate = currCreatedDate,
                            EndDate = currCreatedDate,
                            TimeFrom = model.TimeFrom,
                            TimeTo = model.TimeTo,
                        });
                    }
                    totalDaysId.RemoveAt(0);
                }

                int total = totalDaysId.Count();

                /* alternative 
                    initialize to original total id count
                    if start date == end date, decrease total count by 1
                    if not, total count remains unchanged
                    
                 */

                // loop through the count of IDs
                // calculate the intervals between ids
                // to determine the exact date values of each recurrence
                for (int i = 0; i < total; i++)
                {
                    int currentCreatedDateId = ((int)currCreatedDate.DayOfWeek) + 1; // sunday = 1, monday = 2, ... , saturday = 7
                    int nextCreatedDateId = totalDaysId[i];

                    /* if alternative was chosen
                        
                        conditionally update the nextCreatedDateId
                        to the value next to the current iteration
                        if total count is less than the original count
                        e.g., instead of totalDaysId[i], it will be totalDaysId[i+1]
                     */


                    // Calculate the interval to the next recurrence date
                    // If the next day is after today, the interval is the difference
                    // Otherwise, the interval is the remaining days in the week plus the next day
                    // This is because the week wraps around, so we need to account for the days left in the current week
                    int todayToNextInterval = (nextCreatedDateId > currentCreatedDateId)
                                              ? nextCreatedDateId - currentCreatedDateId
                                              : (7 - currentCreatedDateId) + nextCreatedDateId;

                    // Add the interval to the current date
                    currCreatedDate = currCreatedDate.AddDays(todayToNextInterval);

                    // If the current date is before or equal to the end date, add a booking
                    if (currCreatedDate <= endDate)
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

                // if not, then just add the booking
                createdBookings.Add(new Booking { 
                    StartDate = currCreatedDate,
                    EndDate = currCreatedDate,
                    TimeFrom = model.TimeFrom,
                    TimeTo = model.TimeTo,
                });
            }

            return createdBookings;
        }



        /// <summary>
        /// Returns a list of approved bookings with their recurrences
        /// </summary>
        /// <param name="approvedBookings"></param>
        /// <returns></returns>
        public List<List<Booking>> ApprovedBookingsWithRecurrences(List<Booking> approvedBookings)
        {
            List<List<Booking>> approvedBookingsWithRecurrences = new List<List<Booking>>();

            // map the approved bookings to a list of BookingViewModel
            List<BookingViewModel> approvedBookingsModel = approvedBookings.Select(a => new BookingViewModel
            {
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                TimeFrom = a.TimeFrom,
                TimeTo = a.TimeTo,
                DayOfTheWeekIds = a.Recurrences.Select(r => r.DayOfWeekId ?? 0).ToList(),
            }).ToList();


            // for each approved booking, calculate the booking info
            foreach (var approvedModel in approvedBookingsModel)
            {
                // add the booking info of each approved booking to the list
                approvedBookingsWithRecurrences.Add(BookingInfo(approvedModel));
            }

            return approvedBookingsWithRecurrences;
        }
        #endregion




        #region Get Bookings
        /// <summary>
        /// Returns all the bookings from the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BookingViewModel> GetBookings()
        {
            var bookings = _repository.GetBookings().OrderByDescending(b => b.UpdatedDt);
            return bookings.Select(booking => 
            new BookingViewModel
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
                DayOfTheWeekIds = _repository.GetDayOfWeekIdsForBooking(booking.BookingId),
                modelUser = booking.User,
                modelRoom = booking.Room,
                DtCreated = booking.CreatedDt,
                DtUpdated = booking.UpdatedDt,
            });
        }




        
        /// <summary>
        /// Returns all the bookings of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<BookingViewModel> GetBookingsByUser(int userId)
        {
            var bookings = _repository.GetBookingsByUser(userId).OrderByDescending(b => b.UpdatedDt);
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
                DayOfTheWeekIds = _repository.GetDayOfWeekIdsForBooking(booking.BookingId),
                modelUser = booking.User,
                modelRoom = booking.Room,
                DtCreated = booking.CreatedDt,
                DtUpdated = booking.UpdatedDt,
            });
        }

        public IEnumerable<BookingViewModel> GetBookingsByDate(int year, int month, int? day)
        {
            int actualDay = day ?? 1; // Use 1 if day is null

            DateTime startDate = new DateTime(year, month, actualDay);
            DateTime endDate;

            if (day == null)
            {
                endDate = startDate.AddMonths(1).AddDays(-1); // End of the month
            }
            else
            {
                endDate = startDate; // Specific day
            }

            var bookings =
                _repository.GetBookings()
                    .Include(b => b.Recurrences)
                    .Include(b => b.Room)
                    .AsEnumerable() // Switch to LINQ to Objects
                    .Where(booking =>
                        {   // Filter recurrent bookings
                            bool isRecurrent = isRecurrentBooking(booking);
                            DateTime overlapStart = (DateTime)(booking.StartDate.GetValueOrDefault() > startDate ? booking.StartDate : startDate);
                            DateTime overlapEnd = (DateTime)(booking.EndDate.GetValueOrDefault() < endDate ? booking.EndDate : endDate);
                            HashSet<int> dayOfWeekNumbers = new HashSet<int>();
                            var bookingRecurrence = _repository.GetBookingRecurrence(booking.BookingId);

                            if (overlapStart <= overlapEnd)
                            {
                                // Iterate through the range of booking dates
                                for (DateTime date = overlapStart; date <= overlapEnd; date = date.AddDays(1))
                                {
                                    int dayOfWeekNumber = (int)date.DayOfWeek;

                                    // Add the day of the week to the HashSet if it's not already present
                                    if (!dayOfWeekNumbers.Contains(dayOfWeekNumber))
                                    {
                                        dayOfWeekNumbers.Add(dayOfWeekNumber);
                                    }
                                }
                            }

                            bool hasMatchingRecurrence = isRecurrent
                                ? bookingRecurrence
                                    .Any(recurrence => dayOfWeekNumbers.Contains(recurrence.DayOfWeek.DayOfWeekId-1))
                                : false;

                            return booking.BookingStatus == BookingStatus.APPROVED.ToString()
                                   && !(endDate < booking.StartDate)
                                   && !(startDate > booking.EndDate)
                                   && (!isRecurrent || hasMatchingRecurrence);
                        })
                    .OrderBy(booking => booking.TimeFrom);

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
                modelRoom = booking.Room,
                Recurrence = _repository.GetBookingRecurrence(booking.BookingId).ToList(),
                DayOfTheWeekIds = _repository.GetDayOfWeekIdsForBooking(booking.BookingId)
            });
        }

        #endregion



        #region BOOKING CrUD operations
        /// <summary>
        /// Add a booking to the database
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="InvalidDataException"></exception>
        public int AddBooking(BookingViewModel model)
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

                    if (model.DayOfTheWeekIds != null && model.DayOfTheWeekIds.Any())
                    {
                        foreach (var id in model.DayOfTheWeekIds)
                        {
                            var recurrence = new Recurrence
                            {
                                DayOfWeekId = id,
                                BookingId = booking.BookingId,
                            };

                            booking.Recurrences.Add(recurrence);
                        }
                    }

                    _repository.AddBooking(booking);
                    return booking.BookingId;
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



        /// <summary>
        /// Updates the booking with the passed model
        /// </summary>
        /// <param name="booking"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void UpdateBooking(BookingViewModel booking)
        {

            // if there is a conflict, throw an exception
            if (IsBookingConflict(booking) && !booking.BookingChangeOnly)
            {
                throw new InvalidDataException("Cannot update booking: A conflicting approved booking already exists for the selected date, time, and room.");
            }


            // get the booking to be updated
            var bookingToBeUpdated = _repository.GetBookings()
                .Include(b => b.Recurrences)
                .Where(u => u.BookingId == booking.BookingId)
                .FirstOrDefault();
            if (bookingToBeUpdated != null)
            {
                // map the model to the booking, if not null
                _mapper.Map(booking, bookingToBeUpdated);
                bookingToBeUpdated.UpdatedDt = DateTime.Now;
                bookingToBeUpdated.UpdatedBy = System.Environment.UserName;
                bookingToBeUpdated.Recurrences.Clear();

                _repository.UpdateBooking(bookingToBeUpdated);

                // if there are recurrences, add them to the booking
                if (booking.DayOfTheWeekIds != null && booking.DayOfTheWeekIds.Any())
                {
                    foreach (var id in booking.DayOfTheWeekIds)
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
                // if the booking does not exist, throw an exception
                throw new InvalidDataException("Booking does not exist!");
            }
        }


        /// <summary>
        /// Deletes a booking
        /// </summary>
        /// <param name="booking"></param>
        public void DeleteBooking(BookingViewModel booking)
        {
            var bookingToBeDeleted = _repository.GetBookings().Where(u => u.BookingId == booking.BookingId).FirstOrDefault();
            if (bookingToBeDeleted != null)
            {
                _repository.DeleteBooking(bookingToBeDeleted);
            }

        }

        #endregion
    }
}
