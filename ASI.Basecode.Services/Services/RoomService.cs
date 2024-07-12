using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public IEnumerable<RoomViewModel> RetrieveAll(string roomName= null)
        {
            var data = _roomRepository.GetRooms()
                .Where(x => (string.IsNullOrEmpty(roomName) || x.RoomName.Contains(roomName))).Select(s => new RoomViewModel
                {
                    RoomId = s.RoomId,
                    RoomName = s.RoomName,
                    Description = s.Description,
                    Location = s.Location,
                    Capacity = s.Capacity.Value,
                    Thumbnail = s.Thumbnail,
                    Equipment = s.Equipment,


                });
            return data;
        }

        public void AddRoom(RoomViewModel model)
        {
            var newModel = new Room();
            _mapper.Map(model, newModel);
            newModel.CreatedDt = DateTime.Now;

            newModel.ImageGalleries = new List<ImageGallery>();

            if (model._RoomGallery != null && model._RoomGallery.Any())
            {
                foreach (var file in model._RoomGallery)
                {
                    newModel.ImageGalleries.Add(new ImageGallery()
                    {
                        ImageName = file.GalleryName,
                        Path = file.GalleryUrl,
                    });
                }
            }

            _roomRepository.AddRoom(newModel);
        }

        public void UpdateRoom(RoomViewModel model) 
        {
            var existingData = _roomRepository.GetRooms().Where(s => s.RoomId == model.RoomId).FirstOrDefault();
            _mapper.Map(model, existingData);
            existingData.UpdatedDt = DateTime.Now;
            existingData.Thumbnail = model.Thumbnail;
            existingData.UpdatedDt = DateTime.Now;
            existingData.UpdatedBy = System.Environment.UserName;

            if (model._RoomGallery != null && model._RoomGallery.Any())
            {
                foreach (var file in model._RoomGallery)
                {
                    existingData.ImageGalleries.Add(new ImageGallery()
                    {
                        ImageName = file.GalleryName,
                        Path = file.GalleryUrl,
                    });
                }
            }

            _roomRepository.UpdateRoom(existingData);
        }

        public void UpdateGallery(RoomGalleryViewModel model)
        {
            var existingData = _roomRepository.GetRoomGalleries().Where(s => s.RoomId == model.RoomId).ToList();

            if (existingData != null && existingData.Any())
            {
                foreach (var item in existingData)
                {
                    _roomRepository.UpdateGallery(item);
                }
            }
        }

        public void DeleteRoom(RoomViewModel room)
        {
            var RoomToBeDeleted = _roomRepository.GetRooms().Where(u => u.RoomId == room.RoomId).FirstOrDefault();
            if (RoomToBeDeleted != null)
            {
                _roomRepository.DeleteRoom(RoomToBeDeleted);
            }
        }

        public IEnumerable<RoomGalleryViewModel> GetRoomGallery()
        {
            var data = _roomRepository.GetRoomGalleries().Select(s => new RoomGalleryViewModel
                {
                    RoomId = (int)s.RoomId,
                    GalleryId = s.ImageId,
                    GalleryUrl = s.Path,
                    GalleryName = s.ImageName,
                });
            return data;
        }
        public void DeleteImage(RoomGalleryViewModel model)
        {
            var roomImages = _roomRepository.GetRoomGalleries().Where(x => x.RoomId == model.RoomId).ToList();
            if (roomImages != null && roomImages.Any())
            {
                foreach (var item in roomImages)
                {
                    _roomRepository.DeleteRoomImage(item);
                }
            }
        }
    }
}
    