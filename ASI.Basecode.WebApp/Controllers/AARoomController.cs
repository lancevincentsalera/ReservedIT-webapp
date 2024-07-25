using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Room Controller
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    public class AARoomController : ControllerBase<AARoomController>
    {
        private readonly IRoomService _roomService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AARoomController(IRoomService roomService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWebHostEnvironment webHostEnvironment,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomService = roomService;
            _webHostEnvironment = webHostEnvironment;
        }

        #region View Rooms
        /// <summary>
        /// Returns Room View.
        /// </summary>
        /// <returns> Home View </returns>
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("=======Sample Crud : Retrieve All Start=======");
                var data = _roomService.RetrieveAll();

                var model = new RoomViewModel
                {
                    roomList = data
                };
                _logger.LogInformation("=======Sample Crud : Retrieve All End=======");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }
        #endregion

        #region Room Create
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostCreate(RoomViewModel model)
        {
            try
            {
                _logger.LogInformation("=======Sample Crud : PostCreate Start=======");
                try
                {
                    bool isExist = _roomService.RetrieveAll().Any(data => data.RoomName == model.RoomName);
                    if (isExist)
                    {
                        TempData["DuplicateErr"] = "Duplicate Data";
                        _logger.LogError($"Duplicate Name: {model.RoomName}");
                        return Json(new { success = false, message = "Room Name is already taken" });
                    }
                    if (ModelState.IsValid)
                    {
                        if (model.RoomThumbnailImg != null)
                        {
                            string folder = "room/thumbnail/";
                            model.Thumbnail = await UploadImage(folder, model.RoomThumbnailImg);
                        }

                        if (model.RoomGalleryImg != null)
                        {
                            string folder = "room/gallery/";

                            model._RoomGallery = new List<RoomGalleryViewModel>();

                            foreach (var file in model.RoomGalleryImg)
                            {
                                var roomGallery = new RoomGalleryViewModel()
                                {
                                    GalleryName = file.FileName,
                                    GalleryUrl = await UploadImage(folder, file)
                                };
                                model._RoomGallery.Add(roomGallery);
                            }
                        }
                    }
                    _roomService.AddRoom(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                TempData["CreateMessage"] = "Added Successfully";
                return Json(new { success = true, message = "Room creation successful!" });
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
        #endregion

        #region Room Edit
        [HttpGet]
        public IActionResult GetRoomDetails(int userId)
        {
            var user = _roomService.RetrieveAll().Where(u => u.RoomId == userId).FirstOrDefault();
            if (user != null)
            {
                return Json(user);
            }
            TempData["ErrorMessage"] = "Room not found. Unable to retrieve room details.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostEdit(RoomViewModel model)
        {
            try
            {
                _logger.LogInformation("=======PostEdit Start=======");

                    if (model.RoomThumbnailImg != null)
                    {
                        string folder = "room/thumbnail/";
                        string newThumbnailPath = await UploadImage(folder, model.RoomThumbnailImg);

                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            string oldThumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, model.Thumbnail.TrimStart('/'));
                            DeleteFileWithRetry(oldThumbnailPath, 3, 1000);
                        }
                        model.Thumbnail = newThumbnailPath;
                    }
                    else
                    {
                        model.Thumbnail = model.Thumbnail;
                    }

                    if (model.RoomGalleryImg != null)
                    {
                        string folder = "room/gallery/";
                        model._RoomGallery = new List<RoomGalleryViewModel>();
                        
                        var Images = _roomService.GetRoomGallery().Where(x => x.RoomId == model.RoomId).ToList();
                        foreach (var item in Images) 
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, item.GalleryUrl.TrimStart('/'));
                            DeleteFileWithRetry(oldImagePath, 3, 1000);
                            _roomService.DeleteImage(item);
                        }
                        
                        foreach (var file in model.RoomGalleryImg)
                        {
                            var roomGallery = new RoomGalleryViewModel()
                            {
                                GalleryName = file.FileName,
                                GalleryUrl = await UploadImage(folder, file)
                            };
                       
                        model._RoomGallery.Add(roomGallery);
                        _roomService.UpdateGallery(roomGallery);
                        }
                    }
                _roomService.UpdateRoom(model);
                return Json(new { success = true, message = "Room updated successfully!" });
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
        #endregion

        #region RoomDelete
        [HttpGet]
        public IActionResult DeleteRoom(int roomId) 
        {
            var data = _roomService.RetrieveAll().Where(x => x.RoomId.Equals(roomId)).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        public IActionResult PostDelete(int Id)
        {
            var RoomToBeDeleted = _roomService.RetrieveAll().Where(u => u.RoomId == Id).FirstOrDefault();
            var RoomImages = _roomService.GetRoomGallery().Where(x => x.RoomId == Id).ToList();
            if (RoomToBeDeleted != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(RoomToBeDeleted.Thumbnail))
                    {
                        string oldThumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, RoomToBeDeleted.Thumbnail.TrimStart('/'));
                        DeleteFileWithRetry(oldThumbnailPath, 3, 1000);
                    }

          
                    if (RoomImages != null && RoomImages.Any())
                    {
                        foreach (var item in RoomImages)
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, item.GalleryUrl.TrimStart('/'));
                            DeleteFileWithRetry(oldImagePath, 3, 1000);
                        }
                    }   
                    _roomService.DeleteRoom(RoomToBeDeleted);

                    return Json(new { success = true, message = "Room deleted successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Room not found." });

            }
        }
        #endregion

        #region Methods
        private void DeleteFileWithRetry(string path, int maxRetries, int delayMilliseconds)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        _logger.LogInformation($"Deleting old thumbnail at path: {path}");
                        System.IO.File.Delete(path);
                    }
                    break; // Exit loop if file deletion is successful
                }
                catch (IOException ex)
                {
                    _logger.LogWarning($"Failed to delete file at {path}. Attempt {i + 1} of {maxRetries}. Retrying in {delayMilliseconds}ms. Exception: {ex.Message}");
                    System.Threading.Thread.Sleep(delayMilliseconds); // Wait before retrying
                }
            }
        }


        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Ensure the folder exists
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Use FileStream with a using statement to ensure proper disposal
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + folderPath + uniqueFileName;
        }
        #endregion
    }
}
