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
                });
            return data;
        }

        public RoomViewModel RetrieveRoom(int roomId) 
        { 
            var data = _roomRepository.GetRooms().FirstOrDefault(x => x.RoomId == roomId);
            var model = new RoomViewModel
            {
                RoomId = roomId,
                RoomName = data.RoomName,
                Description = data.Description,
                Location = data.Location,
                
                Capacity = data.Capacity.Value,
                Thumbnail = data.Thumbnail,
            };
            return model;
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

            _roomRepository.UpdateRoom(existingData);
        }

        public void UpdateGallery(RoomGalleryViewModel model)
        {
            var existingData = _roomRepository.GetRoomGalleries().Where(s => s.RoomId == model.RoomId).FirstOrDefault();
            existingData.ImageName = model.GalleryName;
            existingData.Path = model.GalleryUrl;

            _roomRepository.UpdateGallery(existingData);
        }

        public void DeleteRoom(int roomId)
        {
            _roomRepository.DeleteRoom(roomId);
        }
    }
}
    