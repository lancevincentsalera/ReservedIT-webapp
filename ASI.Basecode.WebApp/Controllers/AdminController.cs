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

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase<AdminController>
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
        public AdminController(IUserService userService, IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
        }

        public IActionResult Index()
        {
            var roles = _userService.GetRoles();
            var userViewModel = new UserViewModel()
            {
                Roles = roles
            };
            ViewData["users"] = _userService.GetUsers();
            return View(userViewModel);
        }


        [HttpPost]
        public IActionResult ActivateUser(int userId)
        {
            var user = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    user.AccountStatus = "ACTIVE";
                    _userService.UpdateUser(user);
                    TempData["SuccessMessage"] = "User activation successful!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while activating the user: {ex.Message}";
                }
            } 
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public IActionResult RestrictUser(int userId)
        {
            var user = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    user.AccountStatus = "RESTRICTED";
                    _userService.UpdateUser(user);
                    TempData["SuccessMessage"] = "User restriction successful!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while restricting the user: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateUserPost([FromForm] UserViewModel model)
        {
            try
            {
                _userService.AddUser(model);
                return Json(new { success = true, message = "User creation successful!" });
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
                return Json(new { success = true, message = "User updated successfully!" });
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
        public IActionResult DeleteUserPost(int userId)
        {
            var userToBeDeleted = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (userToBeDeleted != null)
            {
                try
                {
                    _userService.DeleteUser(userToBeDeleted);
                    return Json(new { success = true, message = "User deleted successfully!" });
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
