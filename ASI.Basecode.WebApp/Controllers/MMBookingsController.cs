
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
    [Authorize(Policy = "ManagerOnly")]
    public class MMBookingsController : ControllerBase<MMBookingsController>
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        public MMBookingsController(IRoomService roomService, IBookingService bookingService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookingService = bookingService;
            _roomService = roomService;
        }

        #region Bookings Page View
        /// <summary>
        /// Bookings Page View
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var model = _bookingService.GetBookings();
            ViewData["rooms"] = _roomService.RetrieveAll();
            return View(model);
        }
        #endregion


        #region Filter Bookings
        /// <summary>
        /// filter bookings by date, room name, user name, and/or booking status
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IActionResult FilterBookings(BookingViewModel filter)
        {
            var model = _bookingService.GetBookings();
            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    model = model.Where(b => b.StartDate.Value == filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    model = model.Where(b => b.EndDate.Value == filter.EndDate.Value);

                if (!string.IsNullOrEmpty(filter.RoomName))
                    model = model.Where(b => b.Room.RoomName.Equals(filter.RoomName, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(filter.UserName))
                    model = model.Where(b => b.User.FirstName.Contains(filter.UserName) || b.User.LastName.Contains(filter.UserName));

                if (filter.BookingStatus != null)
                {
                    if (filter.BookingStatus != "All") // No filter
                    {
                        model = model.Where(b => b.BookingStatus == filter.BookingStatus);
                    }
                }
            }

            TempData["SuccessMessage"] = "Filters applied successfully";
            ViewData["rooms"] = _roomService.RetrieveAll();
            return View("Index", model);
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
                    TempData["ErrorMessage"] = ex.Message;
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
