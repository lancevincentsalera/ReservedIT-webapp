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
                    RoomEquipments = s.RoomEquipments.Select(re => new RoomEquipmentViewModel
                    {
                        RoomEquipmentId = re.RoomEquipmentId,
                        EquipmentId = re.EquipmentId,
                        EquipmentName = re.Equipment.EquipmentName
                    }).ToList()
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

            newModel.RoomEquipments = new List<RoomEquipment>();

            if (model.RoomEquipments != null && model.RoomEquipments.Any())
            {
                foreach (var item in model.RoomEquipments)
                {
                    var equipment = new Equipment { EquipmentName = item.EquipmentName };
                    newModel.RoomEquipments.Add(new RoomEquipment
                    {
                        Equipment = equipment,
                        RoomId = newModel.RoomId,
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

            existingData.RoomEquipments.Clear();

            if (model.RoomEquipments != null && model.RoomEquipments.Any())
            {
                foreach (var item in model.RoomEquipments)
                {
                    var equipment = new Equipment { EquipmentName = item.EquipmentName };
                    existingData.RoomEquipments.Add(new RoomEquipment
                    {
                        Equipment = equipment,
                        RoomId = existingData.RoomId,
                    });
                }
            }

            _roomRepository.UpdateRoom(existingData);
        }

        public void UpdateGallery(RoomGalleryViewModel model)
        {
            var existingData = _roomRepository.GetRoomGalleries().Where(s => s.RoomId == model.RoomId).FirstOrDefault();
            existingData.ImageName = model.GalleryName;
            existingData.Path = model.GalleryUrl;

            _roomRepository.UpdateGallery(existingData);
        }

        public void DeleteRoom(RoomViewModel room)
        {
            var RoomToBeDeleted = _roomRepository.GetRooms().Where(u => u.RoomId == room.RoomId).FirstOrDefault();
            if (RoomToBeDeleted != null)
            {
                _roomRepository.DeleteRoom(RoomToBeDeleted);
            }
        }

        public void DeleteRoomEquipmentByRoomId(int roomId)
        {
            var roomEquipments = _roomRepository.GetRoomEquipments().Where(re => re.RoomId == roomId).ToList();
            if (roomEquipments != null && roomEquipments.Any())
            {
                foreach (var item in roomEquipments)
                {
                    _roomRepository.DeleteRoomEquipment(item);
                }
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
        public void DeleteImage(int roomId)
        {
            var roomImages = _roomRepository.GetRoomGalleries().Where(x => x.RoomId == roomId).ToList();
            if (roomImages != null && roomImages.Any())
            {
                foreach (var item in roomImages)
                {
                    _roomRepository.DeleteRoomImage(item);
                }
            }
        }

        public void DeleteUnusedEquipment()
        {
            var unusedEquipments = _roomRepository.GetEquipments()
                .Where(e => !_roomRepository.GetRoomEquipments().Any(re => re.EquipmentId == e.EquipmentId))
                .ToList();

            if (unusedEquipments != null && unusedEquipments.Any())
            {
                foreach (var equipment in unusedEquipments)
                {
                    _roomRepository.DeleteEquipment(equipment);
                }
            }
        }



    }
}
    