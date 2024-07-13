using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        public IActionResult Index()
        {
            var model = _bookingService.GetBookings();
            return View(model);
        }
    }
}
