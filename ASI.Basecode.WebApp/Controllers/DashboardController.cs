using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Policy = "ManagerRegular")]
    public class DashboardController : ControllerBase<DashboardController>
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly IEmailService _emailService;
        private readonly ISettingService _settingService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public DashboardController(ISettingService settingService, IEmailService emailService, IRoomService roomService, IBookingService bookingService, IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._bookingService = bookingService;
            this._roomService = roomService;
            this._emailService = emailService;
            this._settingService = settingService;
        }


        #region Index
        /// <summary>
        /// Index page for the dashboard
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            var bookings = _bookingService.GetBookingsByUser(UserId);
            var model = new BookingViewModel
            {
                roomList = _roomService.RetrieveAll(),
                bookingList = bookings,
                Days = _roomService.GetDays().ToList()
            };
            return View(model);
        }
        #endregion


        #region Cancel Booking
        /// <summary>
        /// Booking status will change to CANCELLED
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CancelBooking(int Id)
        {
            var booking = _bookingService.GetBookingsByUser(UserId).Where(u => u.BookingId == Id).FirstOrDefault();
            if (booking != null)
            {
                try
                {
                        booking.BookingStatus = BookingStatus.CANCELLED.ToString();
                        booking.BookingChangeOnly = true;
                        _bookingService.UpdateBooking(booking);

                        bool bookingStatusChange = _settingService.GetSetting(UserId).BookingStatusChange.GetValueOrDefault() == 1;
                        if (bookingStatusChange)
                            _emailService.SendEmail(_bookingService.GetBookings().ToList().Where(b => b.BookingId == booking.BookingId).FirstOrDefault(), "Your booking has been cancelled");
                        
                        return Json(new { success = true, message = "Booking cancelled successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Booking not found." });

            }
        }
        #endregion


        #region Get Booking Details
        /// <summary>
        /// Get details to populate the edit booking modal
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetBookingDetails(int bookingId)
        {
            var booking = _bookingService.GetBookingsByUser(UserId).Where(u => u.BookingId == bookingId).FirstOrDefault();
            if (booking != null)
            {
                return Json(booking);
            }
            TempData["ErrorMessage"] = "Booking not found. Unable to retrieve booking details.";
            return RedirectToAction("Index");
        }
        #endregion



        #region Edit Booking
        /// <summary>
        /// Action for updating the booking
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateBookingPost(BookingViewModel model)
        {
            try
            {
                if (model.BookingStatus.Equals(BookingStatus.APPROVED.ToString()))
                {
                    throw new InvalidDataException("Booking is already approved. Edit is restricted");
                }
                model.TimeFrom = model.StartDate.Value.TimeOfDay;
                model.TimeTo = model.EndDate.Value.TimeOfDay;
                if (model.DayOfTheWeekIds.Count() > 0 && (model.StartDate.Value.Date == model.EndDate.Value.Date))
                {
                    throw new InvalidDataException("Invalid booking date range");
                }
                if (model.TimeTo <= model.TimeFrom)
                {
                    throw new InvalidDataException("Booking time duration should be valid");
                }
                model.UserId = UserId;
                model.BookingChangeOnly = false;
                _bookingService.UpdateBooking(model);

                bool bookingStatusChange = _settingService.GetSetting(UserId).BookingStatusChange.GetValueOrDefault() == 1;
                if (bookingStatusChange)
                    _emailService.SendEmail(_bookingService.GetBookings().ToList().Where(b => b.BookingId == model.BookingId).FirstOrDefault(), "Your booking has been edited!");
                
                TempData["SuccessMessage"] = "Booking successfully updated.";
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred.";
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
