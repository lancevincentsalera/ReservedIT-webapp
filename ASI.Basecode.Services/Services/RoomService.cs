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
                    RoomDescription = s.RoomDescription,
                    RoomLocation = s.RoomLocation,
                    RoomFacility = s.RoomFacility,
                    RoomCapacity = s.RoomCapacity.Value,
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
                RoomDescription = data.RoomDescription,
                RoomLocation = data.RoomLocation,
                RoomFacility = data.RoomFacility,
                RoomCapacity = data.RoomCapacity.Value,
                RoomThumbnail = data.RoomThumbnail,
            };
            return model;
        }

        public void AddRoom(RoomViewModel model)
        {
            var newModel = new Room();
            _mapper.Map(model, newModel);
            var incID = this._roomRepository.GetRooms().Max(x => x.RoomId) + 1;
            newModel.RoomId = incID;
            newModel.RoomInsDt = DateTime.Now;

            newModel.roomGallery = new List<RoomGallery>();

            if (model._RoomGallery != null && model._RoomGallery.Any())
            {
                var galleryMaxId = this._roomRepository.GetRoomGalleries().Max(x => x.GalleryId);
                foreach (var file in model._RoomGallery)
                {
                    galleryMaxId++;
                    newModel.roomGallery.Add(new RoomGallery()
                    {
                        GalleryId = galleryMaxId,
                        GalleryName = file.GalleryName,
                        GalleryPath = file.GalleryUrl,
                    });
                }
            }
            _roomRepository.AddRoom(newModel);
        }

        public void UpdateRoom(RoomViewModel model) 
        {
            var existingData = _roomRepository.GetRooms().Where(s => s.RoomId == model.RoomId).FirstOrDefault();
            _mapper.Map(model, existingData);
            existingData.RoomUpdDt = DateTime.Now;
            existingData.RoomThumbnail = model.RoomThumbnail;

            _roomRepository.UpdateRoom(existingData);
        }

        public void UpdateGallery(RoomGalleryViewModel model)
        {
            var existingData = _roomRepository.GetRoomGalleries().Where(s => s.RoomId == model.RoomId).FirstOrDefault();
            existingData.GalleryName = model.GalleryName;
            existingData.GalleryPath = model.GalleryUrl;

            _roomRepository.UpdateGallery(existingData);
        }
    }
}
    