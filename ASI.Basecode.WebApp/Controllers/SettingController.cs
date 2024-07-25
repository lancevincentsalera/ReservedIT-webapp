using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
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
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Security.Claims;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Setting Controller
    /// </summary>
    public class SettingController : ControllerBase<SettingController>
    {
        private readonly ISettingService _settingService;
        private readonly IUserService _userService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public SettingController(ISettingService settingService,
                              IUserService userService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._settingService = settingService;
            this._userService = userService;
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
                setting.User = _userService.GetUser(setting.UserId.GetValueOrDefault());
                setting.User.Role = _settingService.GetRole(setting.User.RoleId.GetValueOrDefault());

                return View(setting);
            }

            if (_settingService.SettingExists(userId.GetValueOrDefault()))
            {
                var setting = _settingService.GetSetting(userId.Value);
                setting.User = _userService.GetUser(setting.UserId.GetValueOrDefault());
                setting.User.Role = _settingService.GetRole(setting.User.RoleId.GetValueOrDefault());
                return View(setting);
            }
            return RedirectToAction("SignOutUser", "Account");
        }

        /// <summary>
        /// Returns the setting view for admins
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AAIndex()
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
                setting.User = _userService.GetUser(setting.UserId.GetValueOrDefault());
                setting.User.Role = _settingService.GetRole(setting.User.RoleId.GetValueOrDefault());

                return View(setting);
            }

            if (_settingService.SettingExists(userId.GetValueOrDefault()))
            {
                var setting = _settingService.GetSetting(userId.Value);
                setting.User = _userService.GetUser(setting.UserId.GetValueOrDefault());
                setting.User.Role = _settingService.GetRole(setting.User.RoleId.GetValueOrDefault());
                return View(setting);
            }
            return RedirectToAction("SignOutUser", "Account");
        }

        /// <summary>
        /// Edits user setting.
        /// </summary>
        /// <param name="model">SettingViewModel</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditSetting([FromBody] SettingViewModel model)
        {
            try
            {
                var modelCopy = _settingService.GetSetting(model.UserId.GetValueOrDefault());
                _settingService.Update(model);
                return Json(new { success = true, message = "Setting updated successfully!" });
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
                return Json(new { success = false, message = Resources.Messages.Errors.ServerError });
            }
        }

        /// <summary>
        /// Edits the user password.
        /// </summary>
        /// <param name="model">SettingViewModel</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditUserPassword(SettingViewModel model)
        {

            try
            {
                var user = _userService.GetUser(model.UserId.GetValueOrDefault());
                model.Password = PasswordManager.EncryptPassword(model.Password);

                if (_userService.GetUser(model.UserId.GetValueOrDefault()).Password != model.Password)
                {
                    user.Password = model.Password;
                    _userService.UpdateUser(user);
                    TempData["SuccessMessage"] = "User password updated successfully!";
                }
                else if (_userService.GetUser(model.UserId.GetValueOrDefault()).Password == model.Password)
                {
                    TempData["ErrorMessage"] = "User password update failed! Set password is current password.";
                }
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            bool isAdmin = _userService.GetUser(UserId).RoleId.GetValueOrDefault() == (int)UserRoleManager.ROLE_SUPER
                        || _userService.GetUser(UserId).RoleId.GetValueOrDefault() == (int)UserRoleManager.ROLE_ADMIN;
            if (isAdmin)
                return RedirectToAction("AAIndex");
            return RedirectToAction("Index");
        }

    }
}
