using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Calendar Controller
    /// </summary>
    public class CalendarController : ControllerBase<CalendarController>
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
        public CalendarController(IBookingService bookingService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Returns Calendar Month View.
        /// </summary>
        /// <returns> Calendar Month View </returns>
        public IActionResult Month(string year, string month)
        {
            if (year == null || month == null)
            {
                DateTime now = DateTime.Now;
                year = now.Year.ToString();
                month = now.Month.ToString();
            }
            var intYear = int.Parse(year);
            var intMonth = int.Parse(month);

            DateTime startDay = new DateTime(intYear, intMonth, 1);
            DateTime endDay = startDay.AddMonths(1).AddDays(-1);

            ViewBag.StartDay = startDay;
            ViewBag.EndDay = endDay;

            var viewModel = _bookingService.GetBookingsByDate(intYear, intMonth, null);
            return View(viewModel);
        }

        /// <summary>
        /// Returns Calendar Day View.
        /// </summary>
        /// <returns> Calendar Day View </returns>
        public IActionResult Day(string year, string month, string day)
        {
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Day = day;

            return View();
        }
    }
}
