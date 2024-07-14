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
using System;
using System.Drawing.Printing;
using System.IO;
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
            int? userId = UserId;

            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "User ID not found in session!";
                return RedirectToAction("SignOutUser", "Account");

            }

            if (!_settingService.SettingExists(userId.GetValueOrDefault()))
            {
                var setting = new SettingViewModel
                {
                    UserId = userId.GetValueOrDefault(),
                    BookingSuccess = 1,
                    BookingStatusChange = 1,
                    BookingReminder = (int)new TimeSpan(1, 0, 0, 0).TotalSeconds,
                    BookingDuration = (int)new TimeSpan(1, 0, 0).TotalSeconds
                };
                _settingService.Add(setting);

                setting = _settingService.GetSetting(userId.GetValueOrDefault());
                setting.User = _settingService.GetUser(setting.UserId.GetValueOrDefault());
                setting.User.Role = _settingService.GetRole(setting.User.RoleId.GetValueOrDefault());

                return View(setting);
            }

            if (_settingService.SettingExists(userId.GetValueOrDefault()))
            {
                var setting = _settingService.GetSetting(userId.Value);
                setting.User = _settingService.GetUser(setting.UserId.GetValueOrDefault());
                setting.User.Role = _settingService.GetRole(setting.User.RoleId.GetValueOrDefault());
                return View(setting);
            }
            return RedirectToAction("SignOutUser", "Account");
        }

        /// <summary>
        /// Edits user setting.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit([FromBody] SettingViewModel model)
        {
            try
            {
                model.User = _settingService.GetUser(model.UserId.GetValueOrDefault());
                _settingService.Update(model);
                return Json(new { success = true, message = "Setting updated successfully!" });
            }
            catch (InvalidDataException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = Resources.Messages.Errors.ServerError });
            }
        }

    }
}
