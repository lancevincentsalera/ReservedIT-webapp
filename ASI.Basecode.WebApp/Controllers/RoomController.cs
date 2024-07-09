using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
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
    public class RoomController : ControllerBase<RoomController>
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
        public RoomController(IRoomService roomService,
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

                var model = new RoomViewModelList
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
        public async Task<IActionResult> PostCreate(RoomViewModel model, string[] Equipments)
        {
            _logger.LogInformation("=======Sample Crud : PostCreate Start=======");
            try
            {
                bool isExist = _roomService.RetrieveAll().Any(data => data.RoomName == model.RoomName);
                if (isExist)
                {
                    TempData["DuplicateErr"] = "Duplicate Data";
                    _logger.LogError($"Duplicate Name: {model.RoomName}");
                    return RedirectToAction("Create", model);
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
                    model.RoomEquipments = new List<RoomEquipmentViewModel>();

                    foreach (var equipmentName in Equipments)
                    {
                        if (!string.IsNullOrEmpty(equipmentName))
                        {
                            var equipment = new RoomEquipmentViewModel
                            {
                                EquipmentName = equipmentName
                            };
                            model.RoomEquipments.Add(equipment);
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
            return RedirectToAction("Index");
        }
        #endregion

        #region Room Edit
        [HttpGet]
        public IActionResult Edit(int id) 
        {
            var data = _roomService.RetrieveRoom(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> PostEdit(RoomViewModel model)
        {
            _logger.LogInformation("=======PostEdit Start=======");

            try
            {
                var existingRoom = _roomService.RetrieveRoom(model.RoomId);

                if (existingRoom == null)
                {
                    _logger.LogError($"Room with ID {model.RoomId} not found.");
                    TempData["ErrorMessage"] = "Room not found.";
                    return RedirectToAction("Index");
                }

                bool isDuplicate = _roomService.RetrieveAll().Any(data => data.RoomName == model.RoomName && data.RoomId != model.RoomId);
                if (isDuplicate)
                {
                    TempData["DuplicateErr"] = "Duplicate room name.";
                    _logger.LogError($"Duplicate Room Name: {model.RoomName}");
                    return RedirectToAction("Edit", new { id = model.RoomId });
                }

                if (model.RoomThumbnailImg != null)
                {
                    string folder = "room/thumbnail/";
                    string newThumbnailPath = await UploadImage(folder, model.RoomThumbnailImg);

                    if (!string.IsNullOrEmpty(existingRoom.Thumbnail))
                    {
                        string oldThumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, existingRoom.Thumbnail.TrimStart('/'));
                        DeleteFileWithRetry(oldThumbnailPath, 3, 1000);
                    }

                    model.Thumbnail = newThumbnailPath;
                }
                else
                {
                    model.Thumbnail = existingRoom.Thumbnail;
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

                    if (ModelState.IsValid)
                    {
                        _roomService.UpdateRoom(model);
                        TempData["UpdateMessage"] = "Updated Successfully";
                        _logger.LogInformation("=======PostEdit End=======");
                    }
                    else
                    {
                        _logger.LogWarning("Model state is invalid.");
                        TempData["ErrorMessage"] = "Failed to update the room.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the room.");
                TempData["ErrorMessage"] = "An error occurred while updating the room.";
            }

            return RedirectToAction("Index");
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
        public IActionResult PostDelete(int roomId)
        {
            var existingRoom = _roomService.RetrieveRoom(roomId);

            if (existingRoom == null)
            {
                _logger.LogError($"Room with ID {roomId} not found.");
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(existingRoom.Thumbnail))
            {
                string oldThumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, existingRoom.Thumbnail.TrimStart('/'));
                DeleteFileWithRetry(oldThumbnailPath, 3, 1000);
            }
            _roomService.DeleteRoom(roomId);
            return RedirectToAction("Index");
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
