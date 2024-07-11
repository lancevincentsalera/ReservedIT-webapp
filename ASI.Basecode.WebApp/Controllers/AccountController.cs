﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                this._session.SetString("UserName", nameClaim);
                if (roleClaim != null)
                {
                    if (roleClaim.Value == "ROLE_ADMIN")
                    {
                        return RedirectToAction("Index", "AAUser");
                    }
                    else if (roleClaim.Value == "ROLE_MANAGER")
                    {
                        return RedirectToAction("Index", "MMDashboard");
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View();
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            this._session.SetString("HasSession", "Exist");

            User user = null;

            /*User user = new() { UserId = 0, Email = "0", FirstName = "Name", Password = "Password" };
            
            await this._signInManager.SignInAsync(user);
            this._session.SetString("UserName", model.UserId);

            return RedirectToAction("Index", "Admin");*/

            var loginResult = _userService.AuthenticateUser(model.Email, model.Password, ref user);
            if (loginResult == LoginResult.Success)
            {
                // 認証OK
                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserName", string.Join(' ', user.FirstName, user.LastName));
                this._session.SetInt32("UserId", user.UserId);
                if (user.RoleId == 1)
                {
                    return RedirectToAction("Index", "AAUser");
                } 
                else if (user.RoleId == 2)
                {
                    return RedirectToAction("Index", "MMDashboard");
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // 認証NG
                TempData["ErrorMessage"] = "Incorrect Email or Password";
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var roles = _userService.GetRoles();
            var model = new UserViewModel { Roles = roles };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(UserViewModel model)
        {
            var roles = _userService.GetRoles();
            model.Roles = roles;
            try
            {
                _userService.AddUser(model);
                return RedirectToAction("Login", "Account");
            }
            catch(InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View(model);
        }

        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
