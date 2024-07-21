using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AnalyticsService(IBookingRepository bookingRepository ,IRoomRepository roomRepository ,IUserRepository userRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public List<int> BookingsPerMonth()
        {
            var bookings = _bookingRepository.GetBookings()
                .Where(x => x.CreatedDt.HasValue)
                .AsEnumerable()
                .GroupBy(x => x.CreatedDt.Value.Month)
                .ToDictionary(g => g.Key, g => g.Count());

            var monthlyBookings = new List<int>(new int[12]);

            foreach (var kvp in bookings)
            {
                monthlyBookings[kvp.Key - 1] = kvp.Value;
            }

            return monthlyBookings;
        }

        public Dictionary<int, List<Booking>> DailyBookings()
        {
            DateTime now = DateTime.Now;
            int currentMonth = now.Month;
            int currentYear = now.Year;

            var bookings = _bookingRepository.GetBookings()
                .Where(x => x.CreatedDt.HasValue && x.CreatedDt.Value.Month == currentMonth && x.CreatedDt.Value.Year == currentYear)
                .AsEnumerable()
                .GroupBy(x => x.CreatedDt.Value.Day)
                .ToDictionary(g => g.Key, g => g.ToList());

            return bookings;
        }

        public IEnumerable<UserBookingFrequency> GetUserBookingFrequency()
        {
            var bookings = _bookingRepository.GetBookings()
                .Select(b => new
                {
                    b.User.FirstName,
                    b.CreatedDt,
                    b.TimeFrom,
                    b.TimeTo,
                    b.StartDate,
                    b.EndDate,
                    RecurrenceDays = b.Recurrences.Select(r => r.DayOfWeek.DayName).ToList(),
                    b.Room.RoomName
                }).ToList();

            var data = bookings
                .GroupBy(s => s.FirstName)
                .Select(g => new UserBookingFrequency
                {
                    UserName = g.Key,
                    NoOfBookings = g.Count(),
                    LastBookingDate = g.Max(b => b.CreatedDt).ToString(),
                    TotalDuration = g.Sum(b =>
                    {
                        if (!b.TimeFrom.HasValue || !b.TimeTo.HasValue || !b.StartDate.HasValue || !b.EndDate.HasValue)
                        {
                            Console.WriteLine($"Skipping booking due to missing values: {b}");
                            return 0;
                        }

                        
                        var durationPerDay = (b.TimeTo.Value - b.TimeFrom.Value).TotalMinutes;
                        Console.WriteLine($"Duration per day for booking: {durationPerDay} minutes");

                        
                        var totalDuration = 0.0;

                        foreach (var dayName in b.RecurrenceDays)
                        {
                            var dayOfWeek = GetDayOfWeek(dayName);
                            var occurrences = CountOccurrences(dayOfWeek, b.StartDate.Value, b.EndDate.Value);
                            totalDuration += occurrences * durationPerDay;
                            Console.WriteLine($"Occurrences for {dayName} between {b.StartDate.Value} and {b.EndDate.Value}: {occurrences}, Total Duration: {totalDuration}");
                        }

                        
                        if (!b.RecurrenceDays.Any())
                        {
                            var totalDays = (b.EndDate.Value - b.StartDate.Value).TotalDays + 1; 
                            totalDuration = totalDays * durationPerDay;
                            Console.WriteLine($"No recurrences. Total Duration: {totalDuration}");
                        }

                        var hours = totalDuration / 60;
                        return hours;
                    }).ToString(), 
                    MostBookedRoom = g.GroupBy(b => b.RoomName)
                                      .OrderByDescending(x => x.Count())
                                      .FirstOrDefault().Key
                }).ToList();

            return data;
        }

        public IEnumerable<RoomUsageSummary> GetRoomUsageSummary()
        {
            var rooms = _roomRepository.GetRooms()
                .Select(room => new
                {
                    RoomName = room.RoomName,
                    Bookings = room.Bookings.Select(b => new
                    {
                        b.StartDate,
                        b.EndDate,
                        b.TimeFrom,
                        b.TimeTo,
                        b.CreatedDt,
                        RecurrenceDays = b.Recurrences.Select(r => r.DayOfWeek.DayName).ToList()
                    }).ToList()
                }).ToList();

            var roomUsageSummary = rooms
                .Select(room => new RoomUsageSummary
                {
                    RoomName = room.RoomName,
                    TotalBooking = room.Bookings.Count,
                    PeakDay = room.Bookings.GroupBy(b => b.CreatedDt.Value.Date)
                              .OrderByDescending(g => g.Count())
                              .FirstOrDefault()?.Key.ToString("yyyy-MM-dd"),
                    PeakTime = room.Bookings.GroupBy(b => b.TimeFrom.Value)
                               .OrderByDescending(g => g.Count())
                               .FirstOrDefault()?.Key.ToString(@"hh\:mm"),
                    TotalDuration = room.Bookings.Sum(b =>
                    {
                        if (!b.TimeFrom.HasValue || !b.TimeTo.HasValue || !b.StartDate.HasValue || !b.EndDate.HasValue)
                        {
                          return 0;
                        }

                    var durationPerDay = (b.TimeTo.Value - b.TimeFrom.Value).TotalMinutes;
                    var totalDuration = 0.0;

                    foreach (var dayName in b.RecurrenceDays)
                    {
                        var dayOfWeek = GetDayOfWeek(dayName);
                        var occurrences = CountOccurrences(dayOfWeek, b.StartDate.Value, b.EndDate.Value);
                        totalDuration += occurrences * durationPerDay;
                    }

                    if (!b.RecurrenceDays.Any())
                    {
                        var totalDays = (b.EndDate.Value - b.StartDate.Value).TotalDays + 1; 
                        totalDuration = totalDays * durationPerDay;
                    }
                        var hours = totalDuration / 60;
                        return hours;
                    }).ToString()
                }).ToList();

            return roomUsageSummary;
        }
        private DayOfWeek GetDayOfWeek(string dayName)
        {
            return dayName switch
            {
                "Sunday" => DayOfWeek.Sunday,
                "Monday" => DayOfWeek.Monday,
                "Tuesday" => DayOfWeek.Tuesday,
                "Wednesday" => DayOfWeek.Wednesday,
                "Thursday" => DayOfWeek.Thursday,
                "Friday" => DayOfWeek.Friday,
                "Saturday" => DayOfWeek.Saturday,
                _ => throw new ArgumentException("Invalid day name", nameof(dayName)),
            };
        }


        private int CountOccurrences(DayOfWeek dayOfWeek, DateTime startDate, DateTime endDate)
        {
            int count = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == dayOfWeek)
                {
                    count++;
                }
            }
            Console.WriteLine($"Occurrences of {dayOfWeek} between {startDate} and {endDate}: {count}");
            return count;
        }



    }
}
