using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class RoomsController : ControllerBase<RoomsController>
    {
        public readonly IRoomService _roomService;
        public RoomsController(IRoomService roomService,IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomService = roomService;
        }

        public IActionResult Index()
        {
            var model = _roomService.RetrieveAll();
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
    }
}
