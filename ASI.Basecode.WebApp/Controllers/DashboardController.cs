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
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Policy = "ManagerRegular")]
    public class DashboardController : ControllerBase<DashboardController>
    {
        private readonly IBookingService _bookingService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public DashboardController(IBookingService bookingService, IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._bookingService = bookingService;
        }

        public IActionResult Index()
        {
            var bookings = _bookingService.GetBookingsByUser(UserId);
            return View(bookings);
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
    }
}
