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

namespace ASI.Basecode.WebApp.Controllers
{
    public class RoomsController : ControllerBase<RoomsController>
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        public RoomsController(IBookingService bookingService,IRoomService roomService,IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomService = roomService;
            _bookingService = bookingService;
        }

        public IActionResult Index()
        {
            var data = _roomService.RetrieveAll();
            var model = new RoomViewModel
            {
                roomList = data,
                BookingViewModel = new BookingViewModel
                {
                    Days = _roomService.GetDays().ToList()
                }
            };
            return View(model);
        }

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
                    RoomGallery = room._RoomGallery
                };
                return Json(response);
            }
            TempData["ErrorMessage"] = "Room not found. Unable to retrieve room details.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult CreateBookingPost(BookingViewModel model)
        {
            try
            {
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
                _bookingService.AddBooking(model);
                TempData["SuccessMessage"] = "Booking created successfully";
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return RedirectToAction("Index");
        }
    }
}
