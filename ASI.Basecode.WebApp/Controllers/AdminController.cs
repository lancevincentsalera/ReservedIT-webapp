using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Interfaces;
using System.Linq;

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
            var users = _userService.GetUsers();
            return View(users);
        }


        [HttpPost]
        public IActionResult ActivateUser(int userId)
        {
            var user = _userService.GetUsers().Where(u => u.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                user.AccountStatus = "ACTIVE";

                _userService.ActivateOrRestrictUser(user);
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

                _userService.ActivateOrRestrictUser(user);
            }
            return RedirectToAction("Index");
        }
    }
}
