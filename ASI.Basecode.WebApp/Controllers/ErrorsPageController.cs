using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ErrorPageController : Controller
    {
        public IActionResult NotFound()
        {
            HttpContext.Items["Error404"] = true;
            return View();
        }
        public IActionResult Forbidden()
        {
            HttpContext.Items["Error403"] = true;
            return View();
        }
    }
}

