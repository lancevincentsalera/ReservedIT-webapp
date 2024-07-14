using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
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
    [Authorize(Policy = "ManagerOnly")]
    public class MMBookingsController : ControllerBase<MMBookingsController>
    {
        private readonly IBookingService _bookingService;
        public MMBookingsController(IBookingService bookingService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookingService = bookingService;
        }

        #region Bookings Page View
        /// <summary>
        /// Bookings Page View
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var model = _bookingService.GetBookings();
            return View(model);
        }
        #endregion

        #region Approve Booking
        /// <summary>
        /// Booking status will change to APPROVED
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ApproveBooking(int bookingId)
        {
            var booking = _bookingService.GetBookings().Where(u => u.BookingId == bookingId).FirstOrDefault();
            if (booking != null)
            {
                try
                {
                    if (booking.BookingStatus == BookingStatus.PENDING.ToString() ||
                        booking.BookingStatus == BookingStatus.REJECTED.ToString())
                    {
                        booking.BookingStatus = BookingStatus.APPROVED.ToString();
                        _bookingService.UpdateBooking(booking);
                        TempData["SuccessMessage"] = "Booking approved successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Cannot approve booking: Only rejected or pending bookings can be rejected.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while approving the booking: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Booking not found.";
            }
            return RedirectToAction("Index");
        }
        #endregion


        #region Reject Booking
        /// <summary>
        /// Booking status will change to REJECTED
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RejectBooking(int bookingId)
        {
            var booking = _bookingService.GetBookings().Where(u => u.BookingId == bookingId).FirstOrDefault();
            if (booking != null)
            {
                try
                {
                    if (booking.BookingStatus == BookingStatus.PENDING.ToString() ||
                        booking.BookingStatus == BookingStatus.APPROVED.ToString())
                    {
                        booking.BookingStatus = BookingStatus.REJECTED.ToString();
                        _bookingService.UpdateBooking(booking);
                        TempData["SuccessMessage"] = "Booking rejected successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Cannot reject booking: Only approved or pending bookings can be rejected.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while rejecting the booking: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Booking not found.";
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete Booking
        /// <summary>
        /// Dele
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteBookingPost(int Id)
        {
            var bookingToBeDeleted = _bookingService.GetBookings().Where(u => u.BookingId == Id).FirstOrDefault();
            if (bookingToBeDeleted != null)
            {
                try
                {
                    if(bookingToBeDeleted.BookingStatus == BookingStatus.CANCELLED.ToString())
                    {
                        _bookingService.DeleteBooking(bookingToBeDeleted);
                        return Json(new { success = true, message = "Booking deleted successfully!" });
                    } 
                    else
                    {
                        return Json(new { success = false, message = "Cannot delete booking: Only cancelled bookings can be deleted." });
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
