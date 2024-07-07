using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Setting Controller
    /// </summary>
    public class SettingController : ControllerBase<SettingController>
    {
        private readonly ISettingService _settingService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public SettingController(ISettingService settingService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._settingService = settingService;
        }

        /// <summary>
        /// Returns Setting View.
        /// </summary>
        /// <returns> Setting View </returns>
        public IActionResult Index()
        {
            /*
            int? userId = _session.GetInt32("UserId");
            var setting = _settingService.GetSetting(userId);
            return View(setting);
            */

            int? userId = _session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "User ID not found in session!";
            }

            var setting = _settingService.GetSetting(userId.Value);
            return View(setting);
        }

    }
}
