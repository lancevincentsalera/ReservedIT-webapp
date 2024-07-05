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

namespace ASI.Basecode.WebApp.Controllers
{
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
                user.AccountStatus = "ACTIVE";
                _userService.UpdateUser(user);
                TempData["SuccessMessage"] = "User activation successful!";
            } 
            else
            {
                TempData["ErrorMessage"] = "There was an error activating the user";
            }
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public IActionResult RestrictUser(int userId)
        {
            var user = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                user.AccountStatus = "RESTRICTED";
                _userService.UpdateUser(user);
                TempData["SuccessMessage"] = "User restriction successful!";
            }
            else
            {
                TempData["ErrorMessage"] = "There was an error restricting the user";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateUserPost([FromForm] UserViewModel model)
        {
            try
            {
                _userService.AddUser(model);
                return RedirectToAction("Index");
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetUserDetails(int userId)
        {
            var user = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null) {
                return Json(user);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateUserPost(UserViewModel model)
        {

            try
            {
                _userService.UpdateUser(model);
                return RedirectToAction("Index");
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View();
        }
    }
}
