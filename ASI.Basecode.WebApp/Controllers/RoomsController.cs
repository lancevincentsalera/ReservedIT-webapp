using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class RoomsController : ControllerBase<RoomsController>
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly IEmailService _emailService;
        public RoomsController(IEmailService emailService, IBookingService bookingService,IRoomService roomService,IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomService = roomService;
            _bookingService = bookingService;
            _emailService = emailService;
        }

        #region Index
        /// <summary>
        /// Index page for the dashboard
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var data = _roomService.RetrieveAll();
            var model = new RoomViewModel
            {
                roomList = data,
                BookingViewModel = new BookingViewModel
                {
                    Days = _roomService.GetDays().ToList(),
                    roomList = data
                }
            };
            return View(model);
        }
        #endregion



        #region Get Room Details
        /// <summary>
        /// Get Room Details for AJAX call, stored @ modal.js
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetRoomDetails(int roomId)
        {
            var room = _roomService.RetrieveAll().Where(u => u.RoomId == roomId).FirstOrDefault();
            if (room != null)
            {
                var response = new
                {
                    RoomName = room.RoomName,
                    Description = room.Description,
                    RoomGallery = room._RoomGallery,
                    Capacity = room.Capacity,
                    Location = room.Location,
                    Equipments = room.Equipments
                };
                return Json(response);
            }
            TempData["ErrorMessage"] = "Room not found. Unable to retrieve room details.";
            return RedirectToAction("Index");
        }
        #endregion



        #region Check Booking Conflict
        /// <summary>
        /// Checks the conflict of the booking for dynamic validation
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="roomName"></param>
        /// <param name="dayOfTheWeekIds"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckBookingConflict(DateTime? startDate, DateTime? endDate, string roomName, List<int> dayOfTheWeekIds)
        {
            // Validate that startDate and endDate are not null
            if (!endDate.HasValue || !startDate.HasValue)
            {
                return Json(new { isConflict = false, errorMessage = "Start date and end date must be provided." });
            }

            var timeFrom = startDate.Value.TimeOfDay;
            var timeTo = endDate.Value.TimeOfDay;

            // Calculate the total duration in hours and days
            var totalDuration = (endDate.Value - startDate.Value).TotalDays;
            var totalDurationHours = (endDate.Value - startDate.Value).TotalHours;

            // Validate same time conflicts
            if (totalDurationHours <= 0 || (totalDurationHours >= 24 && timeFrom == timeTo))
            {
                return Json(new { isConflict = true, errorMessage = "Booking end time must be later than the start time." });
            }

            // Validate bookings that span across midnight but are less than 24 hours
            if (endDate.Value.Date > startDate.Value.Date)
            {

                if(totalDurationHours < 24 && dayOfTheWeekIds.Count > 0)
                {
                    return Json(new { isConflict = true, errorMessage = "The booking date range must align with the selected recurrence type." });
                }

                // Allow booking if the total duration is less than 24 hours
                if (totalDuration > 7)
                {
                    // Check if the booking aligns with the recurrence type for multi-day bookings
                    if (dayOfTheWeekIds.Count == 0)
                    {
                        return Json(new { isConflict = true, errorMessage = "The booking date range must align with the selected recurrence type." });
                    }
                }
                else if ((totalDuration > 1 && totalDuration <= 7))
                {
                    if(dayOfTheWeekIds.Count >0)
                    {
                        int start = (int)startDate.Value.DayOfWeek + 1;
                        int end = (int)endDate.Value.DayOfWeek + 1;
                        if (start < end)
                        {
                            if (dayOfTheWeekIds.Any(d => d < start || d > end))
                            {
                                return Json(new { isConflict = true, errorMessage = "The selected days of the week are outside the range of the start and end dates." });
                            }
                        } 
                        else if(start > end)
                        {
                            if(dayOfTheWeekIds.Any( d => d < start && d > end))
                            {
                                return Json(new { isConflict = true, errorMessage = "Txxxxxxhe selected days of the week are outside the range of the start and end dates." });
                            }
                        } 
                    }
                    else
                    {
                        return Json(new { isConflict = true, errorMessage = "The booking date range must align with the selected recurrence type." });
                    }
                }
            } 
            else if(endDate.Value.Date == startDate.Value.Date)
            {
                // Validate that the booking aligns with the recurrence type for single-day bookings
                if (dayOfTheWeekIds.Count > 0)
                {
                    return Json(new { isConflict = true, errorMessage = "The booking date range must align with the selected recurrence type." });
                }


                // Validate same time conflicts
                if (timeTo <= timeFrom)
                {
                    return Json(new { isConflict = true, errorMessage = "Booking end time must be later than the start time." });
                }
            }
            else
            {
                // Validate that endDate is not before startDate
                return Json(new { isConflict = true, errorMessage = "Booking end date must be later than the start date." });
            }


            var model = new BookingViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                RoomName = roomName,
                BookingStatus = BookingStatus.APPROVED.ToString(),
                DayOfTheWeekIds = dayOfTheWeekIds
            };

            var isConflict = _bookingService.IsBookingConflict(model);
            var errorMessage = isConflict ? "Booking conflict detected. Please select a different date/time." : string.Empty;

            return Json(new { isConflict, errorMessage });
        }
        #endregion



        #region Create Booking
        /// <summary>
        /// Controller action for creating a booking
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateBookingPost(BookingViewModel model)
        {
            try
            {
                if (!model.StartDate.HasValue || !model.EndDate.HasValue)
                {
                    throw new InvalidDataException("Start and end date are required.");
                }

                // initialize timespan from start and end date
                model.TimeFrom = model.StartDate.Value.TimeOfDay;
                model.TimeTo = model.EndDate.Value.TimeOfDay;


                if (model.DayOfTheWeekIds.Count() > 0 && (model.StartDate.Value.Date == model.EndDate.Value.Date))
                {
                    throw new InvalidDataException("Invalid booking date range");
                }


                if(model.TimeTo <= model.TimeFrom)
                {
                    throw new InvalidDataException("Booking time duration should be valid");
                }


                model.UserId = UserId;
                int bookingId = _bookingService.AddBooking(model);
                TempData["SuccessMessage"] = "Booking created successfully";
                _emailService.SendEmail(_bookingService.GetBookings().ToList().Where(b => b.BookingId == bookingId).FirstOrDefault(), "Your booking has been created!");
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
