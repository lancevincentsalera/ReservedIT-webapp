using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Analytics Controller
    /// </summary>
    public class MMAnalyticsController : ControllerBase<MMAnalyticsController>
    {
        private readonly IAnalyticsService _analyticsService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public MMAnalyticsController(IAnalyticsService analyticsService,
                                     IHttpContextAccessor httpContextAccessor,
                                     ILoggerFactory loggerFactory,
                                     IConfiguration configuration,
                                     IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._analyticsService = analyticsService;
        }

        /// <summary>
        /// Returns Analytics View.
        /// </summary>
        /// <returns> Analytics View </returns>
        public IActionResult Index()
        {
            var currentMonth = DateTime.Now.Month;
            var bookingsGroupedByDay = _analyticsService.DailyBookings(DateTime.Now.Month);
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var lineChartData = new List<int>(new int[daysInMonth]);

            foreach (var kvp in bookingsGroupedByDay)
            {
                lineChartData[kvp.Key - 1] = kvp.Value.Count;
            }

            var barChartData = _analyticsService.BookingsPerMonth();
            var userBookingFrequencies = _analyticsService.GetUserBookingFrequency();
            var roomUsageSummaries = _analyticsService.GetRoomUsageSummary();

            ViewData["BarChartData"] = barChartData;
            ViewData["LineChartData"] = lineChartData;
            ViewData["UserBookingFrequencies"] = userBookingFrequencies;
            ViewData["RoomUsageSummaries"] = roomUsageSummaries;
            return View();
        }

        [HttpGet]
        public JsonResult GetDailyBookings(int month)
        {
            var bookingsGroupedByDay = _analyticsService.DailyBookings(month);
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, month);
            var lineChartData = new List<int>(new int[daysInMonth]);

            foreach (var kvp in bookingsGroupedByDay)
            {
                lineChartData[kvp.Key - 1] = kvp.Value.Count;
            }

            Console.WriteLine("Line chart data for month " + month + ": " + string.Join(", ", lineChartData));

            return Json(lineChartData);
        }

    }

}
