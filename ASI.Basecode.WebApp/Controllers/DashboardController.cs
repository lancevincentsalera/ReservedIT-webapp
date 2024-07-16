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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public DashboardController(IRoomService roomService, IBookingService bookingService, IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._bookingService = bookingService;
            this._roomService = roomService;
        }

        public IActionResult Index()
        {
            var bookings = _bookingService.GetBookingsByUser(UserId);
            var model = new BookingViewModel
            {
                bookingList = bookings,
                Days = _roomService.GetDays().ToList()
            };
            return View(model);
        }


        #region Reject Booking
        /// <summary>
        /// Booking status will change to CANCELLED
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CancelBooking(int Id)
        {
            var booking = _bookingService.GetBookings().Where(u => u.BookingId == Id).FirstOrDefault();
            if (booking != null)
            {
                try
                {
                    if (booking.BookingStatus == BookingStatus.APPROVED.ToString())
                    {
                        booking.BookingStatus = BookingStatus.CANCELLED.ToString();
                        _bookingService.UpdateBooking(booking);
                        return Json(new { success = true, message = "Booking cancelled successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Cannot cancel booking: Only approved bookings can be cancelled." });
                    }
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
        [HttpGet]
        public IActionResult GetBookingDetails(int bookingId)
        {
            var booking = _bookingService.GetBookings().Where(u => u.BookingId == bookingId).FirstOrDefault();
            if (booking != null)
            {
                return Json(booking);
            }
            TempData["ErrorMessage"] = "Booking not found. Unable to retrieve booking details.";
            return RedirectToAction("Index");
        }
        #endregion



        #region Edit Booking
        [HttpPost]
        public IActionResult UpdateBookingPost(BookingViewModel model)
        {
            try
            {
                _bookingService.UpdateBooking(model);
                return Json(new { success = true, message = "Booking updated successfully!" });
            }
            catch (InvalidDataException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message   = Resources.Messages.Errors.ServerError });
            }
        }


        #endregion
    }
}
