using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Interfaces;
using System.Linq;
using ASI.Basecode.Services.ServiceModels;
using System.IO;
using System;
using Microsoft.AspNetCore.Authorization;
using static ASI.Basecode.Resources.Constants.Enums;
using ASI.Basecode.Resources.Constants;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AAUserController : ControllerBase<AAUserController>
    {
        private readonly IUserService _userService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AAUserController(IUserService userService, IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
        }

        /// <summary>
        /// DASHBOARD PAGE OF ADMIN
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var roles = _userService.GetRolesByRoleOrDefault(RoleId);
            var userViewModel = new UserViewModel()
            {
                Roles = roles
            };
            ViewData["users"] = _userService.GetUsersByRoleOrDefault(RoleId);
            return View(userViewModel);
        }

        [HttpPost]
        public IActionResult CreateUserPost([FromForm] UserViewModel model)
        {
            try
            {
                _userService.AddUser(model);
                return Json(new { success = true, message = "User has been created successfully. Applying changes..." });
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

        [HttpGet]
        public IActionResult GetUserDetails(int userId)
        {
            var user = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null) {
                return Json(user);
            }
            TempData["ErrorMessage"] = "User not found. Unable to retrieve user details.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateUserPost(UserViewModel model)
        {

            try
            {
                _userService.UpdateUser(model);
                return Json(new { success = true, message = "User has been updated successfully. Applying changes..." });
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

        [HttpPost]
        public IActionResult DeleteUserPost(int Id)
        {
            var userToBeDeleted = _userService.GetUsers().Where(u => u.UserId == Id).FirstOrDefault();
            if (userToBeDeleted != null)
            {
                try
                {
                    _userService.DeleteUser(userToBeDeleted);
                    return Json(new { success = true, message = "User deleted successfully. Applying changes..." });
                } catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "User not found." });

            }
        }
    }
}
